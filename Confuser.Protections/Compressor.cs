using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Protections.Compress;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.PE;

namespace Confuser.Protections
{
	// Token: 0x0200000D RID: 13
	internal class Compressor : Packer
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00004508 File Offset: 0x00002708
		public override string Name
		{
			get
			{
				return "Compressing Packer";
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00004520 File Offset: 0x00002720
		public override string Description
		{
			get
			{
				return "This packer reduces the size of output.";
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00004048 File Offset: 0x00002248
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00004538 File Offset: 0x00002738
		public override string Id
		{
			get
			{
				return "compressor";
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00004550 File Offset: 0x00002750
		public override string FullId
		{
			get
			{
				return "Ki.Compressor";
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000021F6 File Offset: 0x000003F6
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new ExtractPhase(this));
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004568 File Offset: 0x00002768
		protected override void Pack(ConfuserContext context, ProtectionParameters parameters)
		{
			CompressorContext compressorContext = context.Annotations.Get<CompressorContext>(context, Compressor.ContextKey, null);
			bool flag = compressorContext == null;
			if (flag)
			{
				context.Logger.Error("No executable module!");
				throw new ConfuserException(null);
			}
			ModuleDefMD moduleDefMD = context.Modules[compressorContext.ModuleIndex];
			compressorContext.OriginModuleDef = moduleDefMD;
			ModuleDefUser moduleDefUser = new ModuleDefUser(compressorContext.ModuleName, moduleDefMD.Mvid, moduleDefMD.CorLibTypes.AssemblyRef);
			bool compatMode = compressorContext.CompatMode;
			if (compatMode)
			{
				AssemblyDefUser assemblyDefUser = new AssemblyDefUser(moduleDefMD.Assembly);
				AssemblyDefUser assemblyDefUser2 = assemblyDefUser;
				assemblyDefUser2.Name += ".cr";
				assemblyDefUser.Modules.Add(moduleDefUser);
			}
			else
			{
				compressorContext.Assembly.Modules.Insert(0, moduleDefUser);
				this.ImportAssemblyTypeReferences(moduleDefMD, moduleDefUser);
			}
			moduleDefUser.Context.AssemblyResolver = moduleDefMD.Context.AssemblyResolver;
			moduleDefUser.Context.Resolver = moduleDefMD.Context.Resolver;
			moduleDefUser.Characteristics = moduleDefMD.Characteristics;
			moduleDefUser.Cor20HeaderFlags = moduleDefMD.Cor20HeaderFlags;
			moduleDefUser.Cor20HeaderRuntimeVersion = moduleDefMD.Cor20HeaderRuntimeVersion;
			moduleDefUser.DllCharacteristics = moduleDefMD.DllCharacteristics;
			moduleDefUser.EncBaseId = moduleDefMD.EncBaseId;
			moduleDefUser.EncId = moduleDefMD.EncId;
			moduleDefUser.Generation = moduleDefMD.Generation;
			moduleDefUser.Kind = compressorContext.Kind;
			moduleDefUser.Machine = moduleDefMD.Machine;
			moduleDefUser.RuntimeVersion = moduleDefMD.RuntimeVersion;
			moduleDefUser.TablesHeaderVersion = moduleDefMD.TablesHeaderVersion;
			moduleDefUser.Win32Resources = moduleDefMD.Win32Resources;
			this.InjectStub(context, compressorContext, parameters, moduleDefUser);
			StrongNameKey strongNameKey = context.Annotations.Get<StrongNameKey>(moduleDefMD, Marker.SNKey, null);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ModuleWriterOptions moduleWriterOptions = new ModuleWriterOptions(moduleDefUser)
				{
					StrongNameKey = strongNameKey
				};
				Compressor.KeyInjector @object = new Compressor.KeyInjector(compressorContext);
				moduleWriterOptions.WriterEvent += @object.WriterEvent;
				moduleDefUser.Write(memoryStream, moduleWriterOptions);
				context.CheckCancellation();
				base.ProtectStub(context, context.OutputPaths[compressorContext.ModuleIndex], memoryStream.ToArray(), strongNameKey, new StubProtection(compressorContext, moduleDefMD));
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000047CC File Offset: 0x000029CC
		private static string GetId(byte[] module)
		{
			dnlib.DotNet.MD.Metadata metadata = MetadataFactory.CreateMetadata(new PEImage(module));
			AssemblyNameInfo assemblyNameInfo = new AssemblyNameInfo();
			RawAssemblyRow rawAssemblyRow;
			bool flag = metadata.TablesStream.TryReadAssemblyRow(1U, out rawAssemblyRow);
			if (flag)
			{
				assemblyNameInfo.Name = metadata.StringsStream.ReadNoNull(rawAssemblyRow.Name);
				assemblyNameInfo.Culture = metadata.StringsStream.ReadNoNull(rawAssemblyRow.Locale);
				assemblyNameInfo.PublicKeyOrToken = new PublicKey(metadata.BlobStream.Read(rawAssemblyRow.PublicKey));
				assemblyNameInfo.HashAlgId = (AssemblyHashAlgorithm)rawAssemblyRow.HashAlgId;
				assemblyNameInfo.Version = new Version((int)rawAssemblyRow.MajorVersion, (int)rawAssemblyRow.MinorVersion, (int)rawAssemblyRow.BuildNumber, (int)rawAssemblyRow.RevisionNumber);
				assemblyNameInfo.Attributes = (AssemblyAttributes)rawAssemblyRow.Flags;
			}
			return Compressor.GetId(assemblyNameInfo);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000489C File Offset: 0x00002A9C
		private static string GetId(IAssembly assembly)
		{
			return new AssemblyName(assembly.FullName).FullName.ToUpperInvariant();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000048C4 File Offset: 0x00002AC4
		private void PackModules(ConfuserContext context, CompressorContext compCtx, ModuleDef stubModule, ICompressionService comp, RandomGenerator random)
		{
			int num = 0;
			Dictionary<string, byte[]> modules = new Dictionary<string, byte[]>();
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				bool flag = i == compCtx.ModuleIndex;
				if (!flag)
				{
					string id = Compressor.GetId(context.Modules[i].Assembly);
					modules.Add(id, context.OutputModules[i]);
					int byteCount = Encoding.UTF8.GetByteCount(id);
					bool flag2 = byteCount > num;
					if (flag2)
					{
						num = byteCount;
					}
				}
			}
			foreach (byte[] array in context.ExternalModules)
			{
				string text = Compressor.GetId(array).ToUpperInvariant();
				modules.Add(text, array);
				int byteCount2 = Encoding.UTF8.GetByteCount(text);
				bool flag3 = byteCount2 > num;
				if (flag3)
				{
					num = byteCount2;
				}
			}
			byte[] array2 = random.NextBytes(4 + num);
			array2[0] = (byte)compCtx.EntryPointToken;
			array2[1] = (byte)(compCtx.EntryPointToken >> 8);
			array2[2] = (byte)(compCtx.EntryPointToken >> 16);
			array2[3] = (byte)(compCtx.EntryPointToken >> 24);
			for (int j = 4; j < array2.Length; j++)
			{
				byte[] array3 = array2;
				int num2 = j;
				array3[num2] |= 1;
			}
			compCtx.KeySig = array2;
			int moduleIndex = 0;
			Action<double> <>9__0;
			foreach (KeyValuePair<string, byte[]> keyValuePair in modules)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(keyValuePair.Key);
				for (int k = 0; k < bytes.Length; k++)
				{
					byte[] array4 = bytes;
					int num3 = k;
					array4[num3] *= array2[k + 4];
				}
				uint num4 = 7339873U;
				foreach (byte b in bytes)
				{
					num4 = num4 * 6176543U + (uint)b;
				}
				byte[] value = keyValuePair.Value;
				uint seed = num4;
				Action<double> progressFunc;
				if ((progressFunc = <>9__0) == null)
				{
					progressFunc = (<>9__0 = delegate(double progress)
					{
						progress = (progress + (double)moduleIndex) / (double)modules.Count;
						context.Logger.Progress((int)(progress * 10000.0), 10000);
					});
				}
				byte[] data = compCtx.Encrypt(comp, value, seed, progressFunc);
				context.CheckCancellation();
				EmbeddedResource item = new EmbeddedResource(Convert.ToBase64String(bytes), data, ManifestResourceAttributes.Private);
				stubModule.Resources.Add(item);
				int moduleIndex2 = moduleIndex;
				moduleIndex = moduleIndex2 + 1;
			}
			context.Logger.EndProgress();
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004BC8 File Offset: 0x00002DC8
		private void InjectData(ModuleDef stubModule, MethodDef method, byte[] data)
		{
			TypeDefUser typeDefUser = new TypeDefUser("", "DataType", stubModule.CorLibTypes.GetTypeRef("System", "ValueType"));
			typeDefUser.Layout = dnlib.DotNet.TypeAttributes.ExplicitLayout;
			typeDefUser.Visibility = dnlib.DotNet.TypeAttributes.NestedPrivate;
			typeDefUser.IsSealed = true;
			typeDefUser.ClassLayout = new ClassLayoutUser(1, (uint)data.Length);
			stubModule.GlobalType.NestedTypes.Add(typeDefUser);
			stubModule.UpdateRowId<ClassLayout>(typeDefUser.ClassLayout);
			stubModule.UpdateRowId<TypeDefUser>(typeDefUser);
			FieldDefUser dataField = new FieldDefUser("DataField", new FieldSig(typeDefUser.ToTypeSig(true)))
			{
				IsStatic = true,
				HasFieldRVA = true,
				InitialValue = data,
				Access = dnlib.DotNet.FieldAttributes.PrivateScope
			};
			stubModule.GlobalType.Fields.Add(dataField);
			MutationHelper.ReplacePlaceholder(method, delegate(Instruction[] arg)
			{
				List<Instruction> list = new List<Instruction>();
				list.AddRange(arg);
				list.Add(Instruction.Create(OpCodes.Dup));
				list.Add(Instruction.Create(OpCodes.Ldtoken, dataField));
				TypeRef typeRef = stubModule.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "RuntimeHelpers");
				MethodDef method2 = typeRef.ResolveThrow().FindMethod("InitializeArray");
				list.Add(Instruction.Create(OpCodes.Call, stubModule.Import(method2)));
				return list.ToArray();
			});
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004CE8 File Offset: 0x00002EE8
		private void InjectStub(ConfuserContext context, CompressorContext compCtx, ProtectionParameters parameters, ModuleDef stubModule)
		{
			IRuntimeService service = context.Registry.GetService<IRuntimeService>();
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(this.Id);
			ICompressionService service2 = context.Registry.GetService<ICompressionService>();
			TypeDef runtimeType = service.GetRuntimeType(compCtx.CompatMode ? "Confuser.Runtime.CompressorCompat" : "Confuser.Runtime.Compressor");
			IEnumerable<IDnlibDef> source = InjectHelper.Inject(runtimeType, stubModule.GlobalType, stubModule);
			Mode parameter = parameters.GetParameter<Mode>(context, context.CurrentModule, "key", Mode.Normal);
			Mode mode = parameter;
			if (mode != Mode.Normal)
			{
				if (mode != Mode.Dynamic)
				{
					throw new UnreachableException();
				}
				compCtx.Deriver = new DynamicDeriver();
			}
			else
			{
				compCtx.Deriver = new NormalDeriver();
			}
			compCtx.Deriver.Init(context, randomGenerator);
			context.Logger.Debug("Encrypting modules...");
			MethodDef methodDef = source.OfType<MethodDef>().Single((MethodDef method) => method.Name == "Main");
			stubModule.EntryPoint = methodDef;
			bool flag = compCtx.EntryPoint.HasAttribute("System.STAThreadAttribute");
			if (flag)
			{
				TypeRef typeRef = stubModule.CorLibTypes.GetTypeRef("System", "STAThreadAttribute");
				MethodSig sig = MethodSig.CreateInstance(stubModule.CorLibTypes.Void);
				methodDef.CustomAttributes.Add(new CustomAttribute(new MemberRefUser(stubModule, ".ctor", sig, typeRef)));
			}
			else
			{
				bool flag2 = compCtx.EntryPoint.HasAttribute("System.MTAThreadAttribute");
				if (flag2)
				{
					TypeRef typeRef2 = stubModule.CorLibTypes.GetTypeRef("System", "MTAThreadAttribute");
					MethodSig sig2 = MethodSig.CreateInstance(stubModule.CorLibTypes.Void);
					methodDef.CustomAttributes.Add(new CustomAttribute(new MemberRefUser(stubModule, ".ctor", sig2, typeRef2)));
				}
			}
			uint num = randomGenerator.NextUInt32();
			compCtx.OriginModule = context.OutputModules[compCtx.ModuleIndex];
			byte[] array = compCtx.Encrypt(service2, compCtx.OriginModule, num, delegate(double progress)
			{
				context.Logger.Progress((int)(progress * 10000.0), 10000);
			});
			context.Logger.EndProgress();
			context.CheckCancellation();
			compCtx.EncryptedModule = array;
			MutationHelper.InjectKeys(methodDef, new int[]
			{
				0,
				1
			}, new int[]
			{
				array.Length >> 2,
				(int)num
			});
			this.InjectData(stubModule, methodDef, array);
			MethodDef methodDef2 = source.OfType<MethodDef>().Single((MethodDef method) => method.Name == "Decrypt");
			methodDef2.Body.SimplifyMacros(methodDef2.Parameters);
			List<Instruction> list = methodDef2.Body.Instructions.ToList<Instruction>();
			for (int i = 0; i < list.Count; i++)
			{
				Instruction instruction = list[i];
				bool flag3 = instruction.OpCode == OpCodes.Call;
				if (flag3)
				{
					IMethod method2 = (IMethod)instruction.Operand;
					bool flag4 = method2.DeclaringType.Name == "Mutation" && method2.Name == "Crypt";
					if (flag4)
					{
						Instruction instruction2 = list[i - 2];
						Instruction instruction3 = list[i - 1];
						Debug.Assert(instruction2.OpCode == OpCodes.Ldloc && instruction3.OpCode == OpCodes.Ldloc);
						list.RemoveAt(i);
						list.RemoveAt(i - 1);
						list.RemoveAt(i - 2);
						list.InsertRange(i - 2, compCtx.Deriver.EmitDerivation(methodDef2, context, (Local)instruction2.Operand, (Local)instruction3.Operand));
					}
					else
					{
						bool flag5 = method2.DeclaringType.Name == "Lzma" && method2.Name == "Decompress";
						if (flag5)
						{
							MethodDef runtimeDecompressor = service2.GetRuntimeDecompressor(stubModule, delegate(IDnlibDef member)
							{
							});
							instruction.Operand = runtimeDecompressor;
						}
					}
				}
			}
			methodDef2.Body.Instructions.Clear();
			foreach (Instruction item in list)
			{
				methodDef2.Body.Instructions.Add(item);
			}
			this.PackModules(context, compCtx, stubModule, service2, randomGenerator);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000051E8 File Offset: 0x000033E8
		private void ImportAssemblyTypeReferences(ModuleDef originModule, ModuleDef stubModule)
		{
			AssemblyDef assembly = stubModule.Assembly;
			foreach (CustomAttribute customAttribute in assembly.CustomAttributes)
			{
				bool flag = customAttribute.AttributeType.Scope == originModule;
				if (flag)
				{
					customAttribute.Constructor = (ICustomAttributeType)stubModule.Import(customAttribute.Constructor);
				}
			}
			foreach (CustomAttribute customAttribute2 in assembly.DeclSecurities.SelectMany((DeclSecurity declSec) => declSec.CustomAttributes))
			{
				bool flag2 = customAttribute2.AttributeType.Scope == originModule;
				if (flag2)
				{
					customAttribute2.Constructor = (ICustomAttributeType)stubModule.Import(customAttribute2.Constructor);
				}
			}
		}

		// Token: 0x0400000B RID: 11
		public const string _Id = "compressor";

		// Token: 0x0400000C RID: 12
		public const string _FullId = "Ki.Compressor";

		// Token: 0x0400000D RID: 13
		public const string _ServiceId = "Ki.Compressor";

		// Token: 0x0400000E RID: 14
		public static readonly object ContextKey = new object();

		// Token: 0x0200000E RID: 14
		private class KeyInjector
		{
			// Token: 0x0600004C RID: 76 RVA: 0x0000221C File Offset: 0x0000041C
			public KeyInjector(CompressorContext ctx)
			{
				this.ctx = ctx;
			}

			// Token: 0x0600004D RID: 77 RVA: 0x0000222D File Offset: 0x0000042D
			public void WriterEvent(object sender, ModuleWriterEventArgs args)
			{
				this.OnWriterEvent(args.Writer, args.Event);
			}

			// Token: 0x0600004E RID: 78 RVA: 0x000052FC File Offset: 0x000034FC
			public void OnWriterEvent(ModuleWriterBase writer, ModuleWriterEvent evt)
			{
				bool flag = evt == ModuleWriterEvent.MDBeginCreateTables;
				if (flag)
				{
					uint signature = writer.Metadata.BlobHeap.Add(this.ctx.KeySig);
					uint num = writer.Metadata.TablesHeap.StandAloneSigTable.Add(new RawStandAloneSigRow(signature));
					Debug.Assert(num == 1U);
					uint num2 = 285212672U | num;
					this.ctx.KeyToken = num2;
					MutationHelper.InjectKey(writer.Module.EntryPoint, 2, (int)num2);
				}
				else
				{
					bool flag2 = evt == ModuleWriterEvent.MDBeginAddResources && !this.ctx.CompatMode;
					if (flag2)
					{
						byte[] data = SHA1.Create().ComputeHash(this.ctx.OriginModule.ToArray<byte>());
						uint hashValue = writer.Metadata.BlobHeap.Add(data);
						MDTable<RawFileRow> fileTable = writer.Metadata.TablesHeap.FileTable;
						uint rid = fileTable.Add(new RawFileRow(0U, writer.Metadata.StringsHeap.Add("AptitudeProtections"), hashValue));
						uint implementation = CodedToken.Implementation.Encode(new MDToken(Table.File, rid));
						MDTable<RawManifestResourceRow> manifestResourceTable = writer.Metadata.TablesHeap.ManifestResourceTable;
						foreach (ValueTuple<uint, uint, UTF8String> valueTuple in this.ctx.ManifestResources)
						{
							manifestResourceTable.Add(new RawManifestResourceRow(valueTuple.Item1, valueTuple.Item2, writer.Metadata.StringsHeap.Add(valueTuple.Item3), implementation));
						}
						MDTable<RawExportedTypeRow> exportedTypeTable = writer.Metadata.TablesHeap.ExportedTypeTable;
						foreach (TypeDef typeDef in this.ctx.OriginModuleDef.GetTypes())
						{
							bool flag3 = !typeDef.IsVisibleOutside(true, true);
							if (!flag3)
							{
								exportedTypeTable.Add(new RawExportedTypeRow((uint)typeDef.Attributes, 0U, writer.Metadata.StringsHeap.Add(typeDef.Name), writer.Metadata.StringsHeap.Add(typeDef.Namespace), implementation));
							}
						}
					}
				}
			}

			// Token: 0x0400000F RID: 15
			private readonly CompressorContext ctx;
		}
	}
}
