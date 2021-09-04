using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer.BAML;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x0200007A RID: 122
	internal class WPFAnalyzer : IRenamer
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000303 RID: 771 RVA: 0x000290D4 File Offset: 0x000272D4
		// (remove) Token: 0x06000304 RID: 772 RVA: 0x0002910C File Offset: 0x0002730C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BAMLAnalyzer, BamlElement> AnalyzeBAMLElement;

		// Token: 0x06000305 RID: 773 RVA: 0x00029144 File Offset: 0x00027344
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			MethodDef methodDef = def as MethodDef;
			bool flag = methodDef != null;
			if (flag)
			{
				bool flag2 = !methodDef.HasBody;
				if (flag2)
				{
					return;
				}
				this.AnalyzeMethod(context, service, methodDef);
			}
			ModuleDefMD moduleDefMD = def as ModuleDefMD;
			bool flag3 = moduleDefMD != null;
			if (flag3)
			{
				this.AnalyzeResources(context, service, moduleDefMD);
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0002919C File Offset: 0x0002739C
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD moduleDefMD = def as ModuleDefMD;
			bool flag = moduleDefMD == null || !parameters.GetParameter<bool>(context, def, "renXaml", true);
			if (!flag)
			{
				Dictionary<string, Dictionary<string, BamlDocument>> dictionary = context.Annotations.Get<Dictionary<string, Dictionary<string, BamlDocument>>>(moduleDefMD, WPFAnalyzer.BAMLKey, null);
				bool flag2 = dictionary == null;
				if (!flag2)
				{
					foreach (Dictionary<string, BamlDocument> dictionary2 in dictionary.Values)
					{
						foreach (BamlDocument bamlDocument in dictionary2.Values)
						{
							string decodedName = HttpUtility.UrlDecode(bamlDocument.DocumentName);
							string encodedName = bamlDocument.DocumentName;
							List<IBAMLReference> list;
							bool flag3 = this.bamlRefs.TryGetValue(decodedName, out list);
							if (flag3)
							{
								string str = decodedName.Substring(0, decodedName.LastIndexOf('/') + 1);
								string str2 = encodedName.Substring(0, encodedName.LastIndexOf('/') + 1);
								string text = service.RandomName(RenameMode.Letters).ToLowerInvariant();
								bool flag4 = decodedName.EndsWith(".BAML", StringComparison.OrdinalIgnoreCase);
								if (flag4)
								{
									text += ".baml";
								}
								else
								{
									bool flag5 = decodedName.EndsWith(".XAML", StringComparison.OrdinalIgnoreCase);
									if (flag5)
									{
										text += ".xaml";
									}
								}
								string decodedNewName = str + text;
								string encodedNewName = str2 + text;
								context.Logger.Debug(string.Format("Preserving virtual paths. Replaced {0} with {1}", decodedName, decodedNewName));
								bool flag6 = list.All((IBAMLReference r) => r.CanRename(decodedName, decodedNewName) || r.CanRename(encodedName, encodedNewName));
								bool flag7 = flag6;
								if (flag7)
								{
									foreach (IBAMLReference ibamlreference in list)
									{
										ibamlreference.Rename(decodedName, decodedNewName);
										ibamlreference.Rename(encodedName, encodedNewName);
									}
									bamlDocument.DocumentName = encodedNewName;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x000294A4 File Offset: 0x000276A4
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD moduleDefMD = def as ModuleDefMD;
			bool flag = moduleDefMD == null;
			if (!flag)
			{
				Dictionary<string, Dictionary<string, BamlDocument>> dictionary = context.Annotations.Get<Dictionary<string, Dictionary<string, BamlDocument>>>(moduleDefMD, WPFAnalyzer.BAMLKey, null);
				bool flag2 = dictionary == null;
				if (!flag2)
				{
					List<EmbeddedResource> list = new List<EmbeddedResource>();
					foreach (EmbeddedResource embeddedResource in moduleDefMD.Resources.OfType<EmbeddedResource>())
					{
						Dictionary<string, BamlDocument> dictionary2;
						bool flag3 = !dictionary.TryGetValue(embeddedResource.Name, out dictionary2);
						if (!flag3)
						{
							MemoryStream memoryStream = new MemoryStream();
							ResourceWriter resourceWriter = new ResourceWriter(memoryStream);
							ResourceReader resourceReader = new ResourceReader(embeddedResource.CreateReader().AsStream());
							IDictionaryEnumerator enumerator2 = resourceReader.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								string text = (string)enumerator2.Key;
								string typeName;
								byte[] serializedData;
								resourceReader.GetResourceData(text, out typeName, out serializedData);
								BamlDocument bamlDocument;
								bool flag4 = dictionary2.TryGetValue(text, out bamlDocument);
								if (flag4)
								{
									MemoryStream memoryStream2 = new MemoryStream();
									memoryStream2.Position = 4L;
									BamlWriter.WriteDocument(bamlDocument, memoryStream2);
									memoryStream2.Position = 0L;
									memoryStream2.Write(BitConverter.GetBytes((int)memoryStream2.Length - 4), 0, 4);
									serializedData = memoryStream2.ToArray();
									text = bamlDocument.DocumentName;
								}
								resourceWriter.AddResourceData(text, typeName, serializedData);
							}
							resourceWriter.Generate();
							list.Add(new EmbeddedResource(embeddedResource.Name, memoryStream.ToArray(), embeddedResource.Attributes));
						}
					}
					foreach (EmbeddedResource embeddedResource2 in list)
					{
						int index = moduleDefMD.Resources.IndexOfEmbeddedResource(embeddedResource2.Name);
						moduleDefMD.Resources[index] = embeddedResource2;
					}
				}
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x000296D4 File Offset: 0x000278D4
		private void AnalyzeMethod(ConfuserContext context, INameService service, MethodDef method)
		{
			List<Tuple<bool, Instruction>> list = new List<Tuple<bool, Instruction>>();
			List<Instruction> list2 = new List<Instruction>();
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				bool flag = instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt;
				if (flag)
				{
					IMethod method2 = (IMethod)instruction.Operand;
					bool flag2 = method2.DeclaringType.FullName == "System.Windows.DependencyProperty" && method2.Name.String.StartsWith("Register");
					if (flag2)
					{
						list.Add(Tuple.Create<bool, Instruction>(method2.Name.String.StartsWith("RegisterAttached"), instruction));
					}
					else
					{
						bool flag3 = method2.DeclaringType.FullName == "System.Windows.EventManager" && method2.Name.String == "RegisterRoutedEvent";
						if (flag3)
						{
							list2.Add(instruction);
						}
					}
				}
				else
				{
					bool flag4 = instruction.OpCode.Code == Code.Newobj;
					if (flag4)
					{
						IMethod method3 = (IMethod)instruction.Operand;
						bool flag5 = method3.DeclaringType.FullName == "System.Windows.Data.PropertyGroupDescription" && method3.Name == ".ctor" && i - 1 >= 0 && method.Body.Instructions[i - 1].OpCode.Code == Code.Ldstr;
						if (flag5)
						{
							foreach (PropertyDef obj in this.analyzer.LookupProperty((string)method.Body.Instructions[i - 1].Operand))
							{
								service.SetCanRename(obj, false);
							}
						}
					}
					else
					{
						bool flag6 = instruction.OpCode == OpCodes.Ldstr;
						if (flag6)
						{
							string text = ((string)instruction.Operand).ToUpperInvariant();
							bool flag7 = text.EndsWith(".BAML") || text.EndsWith(".XAML");
							if (flag7)
							{
								Match match = WPFAnalyzer.UriPattern.Match(text);
								bool success = match.Success;
								if (success)
								{
									string text2 = match.Groups[1].Success ? match.Groups[1].Value : string.Empty;
									bool flag8 = !string.IsNullOrWhiteSpace(text2) && !text2.Equals(method.Module.Assembly.Name.String, StringComparison.OrdinalIgnoreCase);
									if (flag8)
									{
										return;
									}
									text = match.Groups[2].Value;
								}
								else
								{
									bool flag9 = text.Contains("/");
									if (flag9)
									{
										context.Logger.WarnFormat("Fail to extract XAML name from '{0}'.", new object[]
										{
											instruction.Operand
										});
									}
								}
								BAMLStringReference value = new BAMLStringReference(instruction);
								text = WebUtility.UrlDecode(text.TrimStart(new char[]
								{
									'/'
								}));
								string key = text.Substring(0, text.Length - 5) + ".BAML";
								string key2 = text.Substring(0, text.Length - 5) + ".XAML";
								this.bamlRefs.AddListEntry(key, value);
								this.bamlRefs.AddListEntry(key2, value);
							}
						}
					}
				}
			}
			bool flag10 = list.Count == 0;
			if (flag10)
			{
				return;
			}
			ITraceService service2 = context.Registry.GetService<ITraceService>();
			MethodTrace methodTrace = service2.Trace(method);
			bool flag11 = false;
			foreach (Tuple<bool, Instruction> tuple in list)
			{
				int[] array = methodTrace.TraceArguments(tuple.Item2);
				bool flag12 = array == null;
				if (flag12)
				{
					bool flag13 = !flag11;
					if (flag13)
					{
						context.Logger.WarnFormat("Failed to extract dependency property name in '{0}'.", new object[]
						{
							method.FullName
						});
					}
					flag11 = true;
				}
				else
				{
					Instruction instruction2 = method.Body.Instructions[array[0]];
					bool flag14 = instruction2.OpCode.Code != Code.Ldstr;
					if (flag14)
					{
						bool flag15 = !flag11;
						if (flag15)
						{
							context.Logger.WarnFormat("Failed to extract dependency property name in '{0}'.", new object[]
							{
								method.FullName
							});
						}
						flag11 = true;
					}
					else
					{
						string text3 = (string)instruction2.Operand;
						TypeDef declaringType = method.DeclaringType;
						bool flag16 = false;
						bool item = tuple.Item1;
						if (item)
						{
							MethodDef methodDef;
							bool flag17 = (methodDef = declaringType.FindMethod("Get" + text3)) != null && methodDef.IsStatic;
							if (flag17)
							{
								service.SetCanRename(methodDef, false);
								flag16 = true;
							}
							bool flag18 = (methodDef = declaringType.FindMethod("Set" + text3)) != null && methodDef.IsStatic;
							if (flag18)
							{
								service.SetCanRename(methodDef, false);
								flag16 = true;
							}
						}
						PropertyDef propertyDef;
						bool flag19 = (propertyDef = declaringType.FindProperty(text3)) != null;
						if (flag19)
						{
							service.SetCanRename(propertyDef, false);
							flag16 = true;
							bool flag20 = propertyDef.GetMethod != null;
							if (flag20)
							{
								service.SetCanRename(propertyDef.GetMethod, false);
							}
							bool flag21 = propertyDef.SetMethod != null;
							if (flag21)
							{
								service.SetCanRename(propertyDef.SetMethod, false);
							}
							bool hasOtherMethods = propertyDef.HasOtherMethods;
							if (hasOtherMethods)
							{
								foreach (MethodDef obj2 in propertyDef.OtherMethods)
								{
									service.SetCanRename(obj2, false);
								}
							}
						}
						bool flag22 = !flag16;
						if (flag22)
						{
							bool item2 = tuple.Item1;
							if (item2)
							{
								context.Logger.WarnFormat("Failed to find the accessors of attached dependency property '{0}' in type '{1}'.", new object[]
								{
									text3,
									declaringType.FullName
								});
							}
							else
							{
								context.Logger.WarnFormat("Failed to find the CLR property of normal dependency property '{0}' in type '{1}'.", new object[]
								{
									text3,
									declaringType.FullName
								});
							}
						}
					}
				}
			}
			flag11 = false;
			foreach (Instruction instr in list2)
			{
				int[] array2 = methodTrace.TraceArguments(instr);
				bool flag23 = array2 == null;
				if (flag23)
				{
					bool flag24 = !flag11;
					if (flag24)
					{
						context.Logger.WarnFormat("Failed to extract routed event name in '{0}'.", new object[]
						{
							method.FullName
						});
					}
					flag11 = true;
				}
				else
				{
					Instruction instruction3 = method.Body.Instructions[array2[0]];
					bool flag25 = instruction3.OpCode.Code != Code.Ldstr;
					if (flag25)
					{
						bool flag26 = !flag11;
						if (flag26)
						{
							context.Logger.WarnFormat("Failed to extract routed event name in '{0}'.", new object[]
							{
								method.FullName
							});
						}
						flag11 = true;
					}
					else
					{
						string text4 = (string)instruction3.Operand;
						TypeDef declaringType2 = method.DeclaringType;
						EventDef eventDef;
						bool flag27 = (eventDef = declaringType2.FindEvent(text4)) == null;
						if (flag27)
						{
							context.Logger.WarnFormat("Failed to find the CLR event of routed event '{0}' in type '{1}'.", new object[]
							{
								text4,
								declaringType2.FullName
							});
						}
						else
						{
							service.SetCanRename(eventDef, false);
							bool flag28 = eventDef.AddMethod != null;
							if (flag28)
							{
								service.SetCanRename(eventDef.AddMethod, false);
							}
							bool flag29 = eventDef.RemoveMethod != null;
							if (flag29)
							{
								service.SetCanRename(eventDef.RemoveMethod, false);
							}
							bool flag30 = eventDef.InvokeMethod != null;
							if (flag30)
							{
								service.SetCanRename(eventDef.InvokeMethod, false);
							}
							bool hasOtherMethods2 = eventDef.HasOtherMethods;
							if (hasOtherMethods2)
							{
								foreach (MethodDef obj3 in eventDef.OtherMethods)
								{
									service.SetCanRename(obj3, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00029FE0 File Offset: 0x000281E0
		private void AnalyzeResources(ConfuserContext context, INameService service, ModuleDefMD module)
		{
			bool flag = this.analyzer == null;
			if (flag)
			{
				this.analyzer = new BAMLAnalyzer(context, service);
				this.analyzer.AnalyzeElement += this.AnalyzeBAMLElement;
			}
			Dictionary<string, Dictionary<string, BamlDocument>> dictionary = new Dictionary<string, Dictionary<string, BamlDocument>>();
			foreach (EmbeddedResource embeddedResource in module.Resources.OfType<EmbeddedResource>())
			{
				Match match = WPFAnalyzer.ResourceNamePattern.Match(embeddedResource.Name);
				bool flag2 = !match.Success;
				if (!flag2)
				{
					Dictionary<string, BamlDocument> dictionary2 = new Dictionary<string, BamlDocument>();
					ResourceReader resourceReader = new ResourceReader(embeddedResource.CreateReader().AsStream());
					IDictionaryEnumerator enumerator2 = resourceReader.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						string text = (string)enumerator2.Key;
						bool flag3 = !text.EndsWith(".baml");
						if (!flag3)
						{
							string text2;
							byte[] data;
							resourceReader.GetResourceData(text, out text2, out data);
							BamlDocument bamlDocument = this.analyzer.Analyze(module, text, data);
							bamlDocument.DocumentName = text;
							dictionary2.Add(text, bamlDocument);
						}
					}
					bool flag4 = dictionary2.Count > 0;
					if (flag4)
					{
						dictionary.Add(embeddedResource.Name, dictionary2);
					}
				}
			}
			bool flag5 = dictionary.Count > 0;
			if (flag5)
			{
				context.Annotations.Set<Dictionary<string, Dictionary<string, BamlDocument>>>(module, WPFAnalyzer.BAMLKey, dictionary);
			}
		}

		// Token: 0x04000537 RID: 1335
		private static readonly object BAMLKey = new object();

		// Token: 0x04000538 RID: 1336
		private static readonly Regex ResourceNamePattern = new Regex("^.*\\.g\\.resources$");

		// Token: 0x04000539 RID: 1337
		internal static readonly Regex UriPattern = new Regex("^(?:PACK\\://(?:COMPONENT|APPLICATION)\\:,,,)?(?:/(.+?)(?:;V\\d+\\.\\d+\\.\\d+\\.\\d+)?;COMPONENT)?(/?[^/].*\\.[BX]AML)$");

		// Token: 0x0400053A RID: 1338
		private BAMLAnalyzer analyzer;

		// Token: 0x0400053B RID: 1339
		internal Dictionary<string, List<IBAMLReference>> bamlRefs = new Dictionary<string, List<IBAMLReference>>(StringComparer.OrdinalIgnoreCase);
	}
}
