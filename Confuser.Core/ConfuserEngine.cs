using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using dnlib.PE;
using Microsoft.Win32;

namespace Confuser.Core
{
	// Token: 0x0200002F RID: 47
	public static class ConfuserEngine
	{
		// Token: 0x060000FB RID: 251 RVA: 0x0000A154 File Offset: 0x00008354
		static ConfuserEngine()
		{
			Assembly assembly = typeof(ConfuserEngine).Assembly;
			AssemblyProductAttribute assemblyProductAttribute = (AssemblyProductAttribute)assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
			AssemblyInformationalVersionAttribute assemblyInformationalVersionAttribute = (AssemblyInformationalVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0];
			AssemblyCopyrightAttribute assemblyCopyrightAttribute = (AssemblyCopyrightAttribute)assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
			ConfuserEngine.Version = assemblyProductAttribute.Product;
			ConfuserEngine.Copyright = assemblyCopyrightAttribute.Copyright;
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
			{
				Assembly result;
				try
				{
					AssemblyName assemblyName = new AssemblyName(e.Name);
					foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
					{
						bool flag = assembly2.GetName().Name == assemblyName.Name;
						if (flag)
						{
							return assembly2;
						}
					}
					result = null;
				}
				catch
				{
					result = null;
				}
				return result;
			};
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000A1F0 File Offset: 0x000083F0
		public static Task Run(ConfuserParameters parameters, CancellationToken? token = null)
		{
			bool flag = parameters.Project == null;
			if (flag)
			{
				throw new ArgumentNullException("parameters");
			}
			bool flag2 = token == null;
			if (flag2)
			{
				token = new CancellationToken?(new CancellationTokenSource().Token);
			}
			return Task.Factory.StartNew(delegate()
			{
				ConfuserEngine.RunInternal(parameters, token.Value);
			}, token.Value);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000A27C File Offset: 0x0000847C
		private static void RunInternal(ConfuserParameters parameters, CancellationToken token)
		{
			using (ConfuserContext confuserContext = new ConfuserContext())
			{
				confuserContext.Logger = parameters.GetLogger();
				confuserContext.Project = parameters.Project.Clone();
				confuserContext.PackerInitiated = parameters.PackerInitiated;
				confuserContext.token = token;
				ConfuserEngine.PrintInfo(confuserContext);
				bool successful = false;
				try
				{
					AssemblyResolver assemblyResolver = new AssemblyResolver();
					assemblyResolver.EnableTypeDefCache = true;
					assemblyResolver.DefaultModuleContext = new ModuleContext(assemblyResolver);
					confuserContext.Resolver = assemblyResolver;
					confuserContext.BaseDirectory = Path.Combine(Environment.CurrentDirectory, parameters.Project.BaseDirectory.TrimEnd(new char[]
					{
						Path.DirectorySeparatorChar
					}) + Path.DirectorySeparatorChar.ToString());
					confuserContext.OutputDirectory = Path.Combine(parameters.Project.BaseDirectory, parameters.Project.OutputDirectory.TrimEnd(new char[]
					{
						Path.DirectorySeparatorChar
					}) + Path.DirectorySeparatorChar.ToString());
					bool flag = !string.IsNullOrWhiteSpace(parameters.Project.InputSymbolMap);
					if (flag)
					{
						confuserContext.InputSymbolMap = Path.Combine(parameters.Project.BaseDirectory, parameters.Project.InputSymbolMap);
					}
					foreach (string path in parameters.Project.ProbePaths)
					{
						assemblyResolver.PostSearchPaths.Insert(0, Path.Combine(confuserContext.BaseDirectory, path));
					}
					confuserContext.CheckCancellation();
					Marker marker = parameters.GetMarker();
					confuserContext.Logger.Debug("Discovering plugins...");
					IList<Protection> list;
					IList<Packer> list2;
					IList<ConfuserComponent> list3;
					parameters.GetPluginDiscovery().GetPlugins(confuserContext, out list, out list2, out list3);
					confuserContext.Logger.InfoFormat("Discovered {0} protections, {1} packers.", new object[]
					{
						list.Count,
						list2.Count
					});
					confuserContext.CheckCancellation();
					confuserContext.Logger.Debug("Resolving component dependency...");
					try
					{
						DependencyResolver dependencyResolver = new DependencyResolver(list);
						list = dependencyResolver.SortDependency();
					}
					catch (CircularDependencyException ex)
					{
						confuserContext.Logger.ErrorException("", ex);
						throw new ConfuserException(ex);
					}
					list3.Insert(0, new CoreComponent(parameters, marker));
					foreach (Protection item in list)
					{
						list3.Add(item);
					}
					foreach (Packer item2 in list2)
					{
						list3.Add(item2);
					}
					confuserContext.CheckCancellation();
					confuserContext.Logger.Info("Loading input modules...");
					marker.Initalize(list, list2);
					MarkerResult markerResult = marker.MarkProject(parameters.Project, confuserContext);
					confuserContext.Modules = new ModuleSorter(markerResult.Modules).Sort().ToList<ModuleDefMD>().AsReadOnly();
					foreach (ModuleDefMD moduleDefMD in confuserContext.Modules)
					{
						moduleDefMD.EnableTypeDefFindCache = false;
					}
					confuserContext.OutputModules = Enumerable.Repeat<byte[]>(null, confuserContext.Modules.Count).ToArray<byte[]>();
					confuserContext.OutputSymbols = Enumerable.Repeat<byte[]>(null, confuserContext.Modules.Count).ToArray<byte[]>();
					confuserContext.OutputPaths = Enumerable.Repeat<string>(null, confuserContext.Modules.Count).ToArray<string>();
					confuserContext.Packer = markerResult.Packer;
					confuserContext.ExternalModules = markerResult.ExternalModules;
					confuserContext.CheckCancellation();
					confuserContext.Logger.Info("Initializing...");
					foreach (ConfuserComponent confuserComponent in list3)
					{
						try
						{
							confuserComponent.Initialize(confuserContext);
						}
						catch (Exception ex2)
						{
							confuserContext.Logger.ErrorException("Error occured during initialization of '" + confuserComponent.Name + "'.", ex2);
							throw new ConfuserException(ex2);
						}
						confuserContext.CheckCancellation();
					}
					confuserContext.CheckCancellation();
					confuserContext.Logger.Debug("Building pipeline...");
					ProtectionPipeline pipeline = new ProtectionPipeline();
					confuserContext.Pipeline = pipeline;
					foreach (ConfuserComponent confuserComponent2 in list3)
					{
						confuserComponent2.PopulatePipeline(pipeline);
					}
					confuserContext.CheckCancellation();
					ConfuserEngine.RunPipeline(pipeline, confuserContext);
					successful = true;
				}
				catch (AssemblyResolveException ex3)
				{
					confuserContext.Logger.ErrorException("Failed to resolve an assembly, check if all dependencies are present in the correct version.", ex3);
					ConfuserEngine.PrintEnvironmentInfo(confuserContext);
				}
				catch (TypeResolveException ex4)
				{
					confuserContext.Logger.ErrorException("Failed to resolve a type, check if all dependencies are present in the correct version.", ex4);
					ConfuserEngine.PrintEnvironmentInfo(confuserContext);
				}
				catch (MemberRefResolveException ex5)
				{
					confuserContext.Logger.ErrorException("Failed to resolve a member, check if all dependencies are present in the correct version.", ex5);
					ConfuserEngine.PrintEnvironmentInfo(confuserContext);
				}
				catch (IOException ex6)
				{
					confuserContext.Logger.ErrorException("An IO error occurred, check if all input/output locations are readable/writable.", ex6);
				}
				catch (OperationCanceledException)
				{
					confuserContext.Logger.Error("Operation cancelled.");
				}
				catch (ConfuserException)
				{
				}
				catch (Exception ex7)
				{
					confuserContext.Logger.ErrorException("Unknown error occurred.", ex7);
				}
				finally
				{
					bool flag2 = confuserContext.Resolver != null;
					if (flag2)
					{
						confuserContext.Resolver.Clear();
					}
					confuserContext.Logger.Finish(successful);
				}
				confuserContext.Dispose();
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000A9CC File Offset: 0x00008BCC
		private static void RunPipeline(ProtectionPipeline pipeline, ConfuserContext context)
		{
			Func<IList<IDnlibDef>> getAllDefs = () => context.Modules.SelectMany((ModuleDefMD module) => module.FindDefinitions()).ToList<IDnlibDef>();
			Func<ModuleDef, IList<IDnlibDef>> getModuleDefs = (ModuleDef module) => module.FindDefinitions().ToList<IDnlibDef>();
			context.CurrentModuleIndex = -1;
			pipeline.ExecuteStage(PipelineStage.Inspection, new Action<ConfuserContext>(ConfuserEngine.Inspection), () => getAllDefs(), context);
			ModuleWriterOptionsBase[] array = new ModuleWriterOptionsBase[context.Modules.Count];
			Func<IList<IDnlibDef>> <>9__7;
			Func<IList<IDnlibDef>> <>9__8;
			Func<IList<IDnlibDef>> <>9__9;
			Func<IList<IDnlibDef>> <>9__10;
			for (int i = 0; i < context.Modules.Count; i++)
			{
				context.CurrentModuleIndex = i;
				context.CurrentModuleWriterOptions = null;
				PipelineStage stage = PipelineStage.BeginModule;
				Action<ConfuserContext> func = new Action<ConfuserContext>(ConfuserEngine.BeginModule);
				Func<IList<IDnlibDef>> targets;
				if ((targets = <>9__7) == null)
				{
					targets = (<>9__7 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage, func, targets, context);
				PipelineStage stage2 = PipelineStage.ProcessModule;
				Action<ConfuserContext> func2 = new Action<ConfuserContext>(ConfuserEngine.ProcessModule);
				Func<IList<IDnlibDef>> targets2;
				if ((targets2 = <>9__8) == null)
				{
					targets2 = (<>9__8 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage2, func2, targets2, context);
				PipelineStage stage3 = PipelineStage.OptimizeMethods;
				Action<ConfuserContext> func3 = new Action<ConfuserContext>(ConfuserEngine.OptimizeMethods);
				Func<IList<IDnlibDef>> targets3;
				if ((targets3 = <>9__9) == null)
				{
					targets3 = (<>9__9 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage3, func3, targets3, context);
				PipelineStage stage4 = PipelineStage.EndModule;
				Action<ConfuserContext> func4 = new Action<ConfuserContext>(ConfuserEngine.EndModule);
				Func<IList<IDnlibDef>> targets4;
				if ((targets4 = <>9__10) == null)
				{
					targets4 = (<>9__10 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage4, func4, targets4, context);
				array[i] = context.CurrentModuleWriterOptions;
			}
			Func<IList<IDnlibDef>> <>9__11;
			for (int j = 0; j < context.Modules.Count; j++)
			{
				context.CurrentModuleIndex = j;
				context.CurrentModuleWriterOptions = array[j];
				PipelineStage stage5 = PipelineStage.WriteModule;
				Action<ConfuserContext> func5 = new Action<ConfuserContext>(ConfuserEngine.WriteModule);
				Func<IList<IDnlibDef>> targets5;
				if ((targets5 = <>9__11) == null)
				{
					targets5 = (<>9__11 = (() => getModuleDefs(context.CurrentModule)));
				}
				pipeline.ExecuteStage(stage5, func5, targets5, context);
				context.OutputModules[j] = context.CurrentModuleOutput;
				context.OutputSymbols[j] = context.CurrentModuleSymbol;
				context.CurrentModuleWriterOptions = null;
				context.CurrentModuleOutput = null;
				context.CurrentModuleSymbol = null;
			}
			context.CurrentModuleIndex = -1;
			pipeline.ExecuteStage(PipelineStage.Debug, new Action<ConfuserContext>(ConfuserEngine.Debug), () => getAllDefs(), context);
			pipeline.ExecuteStage(PipelineStage.Pack, new Action<ConfuserContext>(ConfuserEngine.Pack), () => getAllDefs(), context);
			pipeline.ExecuteStage(PipelineStage.SaveModules, new Action<ConfuserContext>(ConfuserEngine.SaveModules), () => getAllDefs(), context);
			bool flag = !context.PackerInitiated;
			if (flag)
			{
				context.Logger.Info("Done.");
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000AD2C File Offset: 0x00008F2C
		private static void Inspection(ConfuserContext context)
		{
			context.Logger.Info("Resolving dependencies...");
			foreach (Tuple<AssemblyRef, ModuleDefMD> tuple in context.Modules.SelectMany((ModuleDefMD module) => from asmRef in module.GetAssemblyRefs()
			select Tuple.Create<AssemblyRef, ModuleDefMD>(asmRef, module)))
			{
				try
				{
					context.Resolver.ResolveThrow(tuple.Item1, tuple.Item2);
				}
				catch (AssemblyResolveException ex)
				{
					context.Logger.ErrorException("Failed to resolve dependency of '" + tuple.Item2.Name + "'.", ex);
					throw new ConfuserException(ex);
				}
			}
			context.Logger.Debug("Checking Strong Name...");
			foreach (ModuleDefMD moduleDefMD in context.Modules)
			{
				StrongNameKey strongNameKey = context.Annotations.Get<StrongNameKey>(moduleDefMD, Marker.SNKey, null);
				if (strongNameKey == null && moduleDefMD.IsStrongNameSigned)
				{
					context.Logger.WarnFormat("[{0}] SN Key is not provided for a signed module, the output may not be working.", new object[]
					{
						moduleDefMD.Name
					});
				}
				else if (strongNameKey != null && !moduleDefMD.IsStrongNameSigned)
				{
					context.Logger.WarnFormat("[{0}] SN Key is provided for an unsigned module, the output may not be working.", new object[]
					{
						moduleDefMD.Name
					});
				}
				else if (strongNameKey != null && moduleDefMD.IsStrongNameSigned && !moduleDefMD.Assembly.PublicKey.Data.SequenceEqual(strongNameKey.PublicKey))
				{
					context.Logger.WarnFormat("[{0}] Provided SN Key and signed module's public key do not match, the output may not be working.", new object[]
					{
						moduleDefMD.Name
					});
				}
			}
			IMarkerService service = context.Registry.GetService<IMarkerService>();
			context.Logger.Debug("Creating global .cctors...");
			foreach (ModuleDefMD moduleDefMD2 in context.Modules)
			{
				TypeDef typeDef = moduleDefMD2.GlobalType;
				if (typeDef == null)
				{
					typeDef = new TypeDefUser("", "<Module>", null);
					typeDef.Attributes = dnlib.DotNet.TypeAttributes.NotPublic;
					moduleDefMD2.Types.Add(typeDef);
					service.Mark(typeDef, null);
				}
				MethodDef methodDef = typeDef.FindOrCreateStaticConstructor();
				if (!service.IsMarked(methodDef))
				{
					service.Mark(methodDef, null);
				}
			}
			context.Logger.Debug("Watermarking...");
			foreach (ModuleDefMD moduleDefMD3 in context.Modules)
			{
				TypeRef typeRef = moduleDefMD3.CorLibTypes.GetTypeRef("System", "Attribute");
				TypeDefUser typeDefUser = new TypeDefUser("", "198-ᴘʀᴏᴛᴇᴄᴛᴏʀ-ᴠ4-ʙʏ-ᴀᴘᴛɪᴛᴜᴅᴇ#2684", typeRef);
				moduleDefMD3.Types.Add(typeDefUser);
				service.Mark(typeDefUser, null);
				MethodDefUser methodDefUser = new MethodDefUser(".ctor", MethodSig.CreateInstance(moduleDefMD3.CorLibTypes.Void, moduleDefMD3.CorLibTypes.String), dnlib.DotNet.MethodImplAttributes.IL, dnlib.DotNet.MethodAttributes.FamANDAssem | dnlib.DotNet.MethodAttributes.Family | dnlib.DotNet.MethodAttributes.HideBySig | dnlib.DotNet.MethodAttributes.SpecialName | dnlib.DotNet.MethodAttributes.RTSpecialName);
				methodDefUser.Body = new CilBody();
				methodDefUser.Body.MaxStack = 1;
				methodDefUser.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
				methodDefUser.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(moduleDefMD3, ".ctor", MethodSig.CreateInstance(moduleDefMD3.CorLibTypes.Void), typeRef)));
				methodDefUser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
				typeDefUser.Methods.Add(methodDefUser);
				service.Mark(methodDefUser, null);
				CustomAttribute customAttribute = new CustomAttribute(methodDefUser);
				customAttribute.ConstructorArguments.Add(new CAArgument(moduleDefMD3.CorLibTypes.String, ConfuserEngine.Version));
				moduleDefMD3.CustomAttributes.Add(customAttribute);
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000B1B8 File Offset: 0x000093B8
		private static void CopyPEHeaders(PEHeadersOptions writerOptions, ModuleDefMD module)
		{
			IPEImage peimage = module.Metadata.PEImage;
			writerOptions.MajorImageVersion = new ushort?(peimage.ImageNTHeaders.OptionalHeader.MajorImageVersion);
			writerOptions.MajorLinkerVersion = new byte?(peimage.ImageNTHeaders.OptionalHeader.MajorLinkerVersion);
			writerOptions.MajorOperatingSystemVersion = new ushort?(peimage.ImageNTHeaders.OptionalHeader.MajorOperatingSystemVersion);
			writerOptions.MajorSubsystemVersion = new ushort?(peimage.ImageNTHeaders.OptionalHeader.MajorSubsystemVersion);
			writerOptions.MinorImageVersion = new ushort?(peimage.ImageNTHeaders.OptionalHeader.MinorImageVersion);
			writerOptions.MinorLinkerVersion = new byte?(peimage.ImageNTHeaders.OptionalHeader.MinorLinkerVersion);
			writerOptions.MinorOperatingSystemVersion = new ushort?(peimage.ImageNTHeaders.OptionalHeader.MinorOperatingSystemVersion);
			writerOptions.MinorSubsystemVersion = new ushort?(peimage.ImageNTHeaders.OptionalHeader.MinorSubsystemVersion);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000B2AC File Offset: 0x000094AC
		private static void BeginModule(ConfuserContext context)
		{
			context.Logger.InfoFormat("Processing module '{0}'...", new object[]
			{
				context.CurrentModule.Name
			});
			context.CurrentModuleWriterOptions = new ModuleWriterOptions(context.CurrentModule);
			context.CurrentModuleWriterOptions.WriterEvent += delegate(object sender, ModuleWriterEventArgs e)
			{
				context.CheckCancellation();
			};
			ConfuserEngine.CopyPEHeaders(context.CurrentModuleWriterOptions.PEHeadersOptions, context.CurrentModule);
			bool flag = !context.CurrentModule.IsILOnly || context.CurrentModule.VTableFixups != null;
			if (flag)
			{
				context.RequestNative();
			}
			StrongNameKey signatureKey = context.Annotations.Get<StrongNameKey>(context.CurrentModule, Marker.SNKey, null);
			context.CurrentModuleWriterOptions.InitializeStrongNameSigning(context.CurrentModule, signatureKey);
			foreach (TypeDef typeDef in context.CurrentModule.GetTypes())
			{
				foreach (MethodDef methodDef in typeDef.Methods)
				{
					bool flag2 = methodDef.Body != null;
					if (flag2)
					{
						methodDef.Body.Instructions.SimplifyMacros(methodDef.Body.Variables, methodDef.Parameters);
					}
				}
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000026EF File Offset: 0x000008EF
		private static void ProcessModule(ConfuserContext context)
		{
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000B480 File Offset: 0x00009680
		private static void OptimizeMethods(ConfuserContext context)
		{
			foreach (TypeDef typeDef in context.CurrentModule.GetTypes())
			{
				foreach (MethodDef methodDef in typeDef.Methods)
				{
					bool flag = methodDef.Body != null;
					if (flag)
					{
						methodDef.Body.Instructions.OptimizeMacros();
					}
				}
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000B528 File Offset: 0x00009728
		private static void EndModule(ConfuserContext context)
		{
			foreach (ModuleDefMD moduleDefMD in context.Modules)
			{
				foreach (TypeDef typeDef in moduleDefMD.Types)
				{
					for (int i = typeDef.CustomAttributes.Count - 1; i > -1; i--)
					{
						CustomAttribute customAttribute = typeDef.CustomAttributes[i];
						bool flag = customAttribute.AttributeType.FullName == "System.Diagnostics.DebuggerDisplayAttribute";
						if (flag)
						{
							typeDef.CustomAttributes.Remove(customAttribute);
						}
					}
				}
			}
			string text = context.Modules[context.CurrentModuleIndex].Location;
			bool flag2 = text != null;
			if (flag2)
			{
				bool flag3 = !Path.IsPathRooted(text);
				if (flag3)
				{
					text = Path.Combine(Environment.CurrentDirectory, text);
				}
				text = Utils.GetRelativePath(text, context.BaseDirectory);
			}
			else
			{
				text = context.CurrentModule.Name;
			}
			string text2 = context.Annotations.Get<string>(context.CurrentModule, Marker.SubDirKey, null);
			bool flag4 = !string.IsNullOrWhiteSpace(text2);
			if (flag4)
			{
				text = Path.Combine(Path.GetDirectoryName(text), text2, Path.GetFileName(text));
			}
			context.OutputPaths[context.CurrentModuleIndex] = text;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000B6C8 File Offset: 0x000098C8
		private static void WriteModule(ConfuserContext context)
		{
			context.Logger.InfoFormat("Writing module '{0}'...", new object[]
			{
				context.CurrentModule.Name
			});
			MemoryStream memoryStream = null;
			MemoryStream memoryStream2 = new MemoryStream();
			bool flag = context.CurrentModule.PdbState != null;
			if (flag)
			{
				memoryStream = new MemoryStream();
				context.CurrentModuleWriterOptions.WritePdb = true;
				context.CurrentModuleWriterOptions.PdbFileName = Path.ChangeExtension(Path.GetFileName(context.OutputPaths[context.CurrentModuleIndex]), "pdb");
				context.CurrentModuleWriterOptions.PdbStream = memoryStream;
			}
			context.CurrentModuleWriterOptions.MetadataLogger = DummyLogger.NoThrowInstance;
			bool flag2 = context.CurrentModuleWriterOptions is ModuleWriterOptions;
			if (flag2)
			{
				context.CurrentModule.Write(memoryStream2, (ModuleWriterOptions)context.CurrentModuleWriterOptions);
			}
			else
			{
				context.CurrentModule.NativeWrite(memoryStream2, (NativeModuleWriterOptions)context.CurrentModuleWriterOptions);
			}
			context.CurrentModuleOutput = memoryStream2.ToArray();
			bool flag3 = context.CurrentModule.PdbState != null;
			if (flag3)
			{
				context.CurrentModuleSymbol = memoryStream.ToArray();
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000B7E8 File Offset: 0x000099E8
		private static void Debug(ConfuserContext context)
		{
			context.Logger.Info("Finalizing...");
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				bool flag = context.OutputSymbols[i] == null;
				if (!flag)
				{
					string fullPath = Path.GetFullPath(Path.Combine(context.OutputDirectory, context.OutputPaths[i]));
					string directoryName = Path.GetDirectoryName(fullPath);
					bool flag2 = !Directory.Exists(directoryName);
					if (flag2)
					{
						Directory.CreateDirectory(directoryName);
					}
					File.WriteAllBytes(Path.ChangeExtension(fullPath, "pdb"), context.OutputSymbols[i].ToArray<byte>());
				}
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000B89C File Offset: 0x00009A9C
		private static void Pack(ConfuserContext context)
		{
			bool flag = context.Packer != null;
			if (flag)
			{
				context.Logger.Info("Packing...");
				context.Packer.Pack(context, new ProtectionParameters(context.Packer, context.Modules.OfType<IDnlibDef>().ToList<IDnlibDef>()));
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000B8F4 File Offset: 0x00009AF4
		private static void SaveModules(ConfuserContext context)
		{
			context.Resolver.Clear();
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				string fullPath = Path.GetFullPath(Path.Combine(context.OutputDirectory, context.OutputPaths[i]));
				ModuleDefMD moduleDefMD = context.Modules[i];
				bool flag = moduleDefMD.Metadata != null;
				if (flag)
				{
					string fullPath2 = Path.GetFullPath(moduleDefMD.Metadata.PEImage.Filename);
					bool flag2 = string.Equals(fullPath, fullPath2, StringComparison.OrdinalIgnoreCase);
					if (flag2)
					{
						IInternalPEImage internalPEImage = moduleDefMD.Metadata.PEImage as IInternalPEImage;
						if (internalPEImage != null)
						{
							internalPEImage.UnsafeDisableMemoryMappedIO();
						}
					}
				}
				string directoryName = Path.GetDirectoryName(fullPath);
				bool flag3 = !Directory.Exists(directoryName);
				if (flag3)
				{
					Directory.CreateDirectory(directoryName);
				}
				context.Logger.DebugFormat("Saving to '{0}'...", new object[]
				{
					fullPath
				});
				File.WriteAllBytes(fullPath, context.OutputModules[i].ToArray<byte>());
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000BA04 File Offset: 0x00009C04
		private static void PrintInfo(ConfuserContext context)
		{
			bool packerInitiated = context.PackerInitiated;
			if (packerInitiated)
			{
				context.Logger.Info("Protecting packer stub...");
			}
			else
			{
				context.Logger.InfoFormat("{0} {1}", new object[]
				{
					ConfuserEngine.Version,
					ConfuserEngine.Copyright
				});
				Type type = Type.GetType("Mono.Runtime");
				context.Logger.InfoFormat("Running on {0}, {1}, {2} bits", new object[]
				{
					Environment.OSVersion,
					(type == null) ? (".NET Framework v" + Environment.Version) : type.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null),
					IntPtr.Size * 8
				});
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x000026F2 File Offset: 0x000008F2
		private static IEnumerable<string> GetFrameworkVersions()
		{
			using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\"))
			{
				foreach (string versionKeyName in ndpKey.GetSubKeyNames())
				{
					bool flag = !versionKeyName.StartsWith("v");
					if (!flag)
					{
						RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
						string name = (string)versionKey.GetValue("Version", "");
						string sp = versionKey.GetValue("SP", "").ToString();
						string install = versionKey.GetValue("Install", "").ToString();
						bool flag2 = install == "" || (sp != "" && install == "1");
						if (flag2)
						{
							yield return versionKeyName + "  " + name;
						}
						bool flag3 = name != "";
						if (!flag3)
						{
							foreach (string subKeyName in versionKey.GetSubKeyNames())
							{
								RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
								name = (string)subKey.GetValue("Version", "");
								bool flag4 = name != "";
								if (flag4)
								{
									sp = subKey.GetValue("SP", "").ToString();
								}
								install = subKey.GetValue("Install", "").ToString();
								bool flag5 = install == "";
								if (flag5)
								{
									yield return versionKeyName + "  " + name;
								}
								else
								{
									bool flag6 = install == "1";
									if (flag6)
									{
										yield return "  " + subKeyName + "  " + name;
									}
								}
								subKey = null;
								subKeyName = null;
							}
							string[] array2 = null;
							versionKey = null;
							name = null;
							sp = null;
							install = null;
							versionKeyName = null;
						}
					}
				}
				string[] array = null;
			}
			RegistryKey ndpKey = null;
			using (RegistryKey ndpKey2 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
			{
				bool flag7 = ndpKey2.GetValue("Release") == null;
				if (flag7)
				{
					yield break;
				}
				int releaseKey = (int)ndpKey2.GetValue("Release");
				yield return "v4.5 " + releaseKey;
			}
			RegistryKey ndpKey2 = null;
			yield break;
			yield break;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000BAC8 File Offset: 0x00009CC8
		private static void PrintEnvironmentInfo(ConfuserContext context)
		{
			bool packerInitiated = context.PackerInitiated;
			if (!packerInitiated)
			{
				context.Logger.Error("---BEGIN DEBUG INFO---");
				context.Logger.Error("Installed Framework Versions:");
				foreach (string text in ConfuserEngine.GetFrameworkVersions())
				{
					context.Logger.ErrorFormat("    {0}", new object[]
					{
						text.Trim()
					});
				}
				context.Logger.Error("");
				bool flag = context.Resolver != null;
				if (flag)
				{
					context.Logger.Error("Cached assemblies:");
					foreach (AssemblyDef assemblyDef in context.Resolver.GetCachedAssemblies())
					{
						bool flag2 = string.IsNullOrEmpty(assemblyDef.ManifestModule.Location);
						if (flag2)
						{
							context.Logger.ErrorFormat("    {0}", new object[]
							{
								assemblyDef.FullName
							});
						}
						else
						{
							context.Logger.ErrorFormat("    {0} ({1})", new object[]
							{
								assemblyDef.FullName,
								assemblyDef.ManifestModule.Location
							});
						}
						foreach (AssemblyRef assemblyRef in assemblyDef.Modules.OfType<ModuleDefMD>().SelectMany((ModuleDefMD m) => m.GetAssemblyRefs()))
						{
							context.Logger.ErrorFormat("        {0}", new object[]
							{
								assemblyRef.FullName
							});
						}
					}
				}
				context.Logger.Error("---END DEBUG INFO---");
			}
		}

		// Token: 0x0400010B RID: 267
		public static readonly string Version;

		// Token: 0x0400010C RID: 268
		private static readonly string Copyright;
	}
}
