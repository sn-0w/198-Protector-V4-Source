using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Pdb;

namespace Confuser.Core
{
	// Token: 0x02000051 RID: 81
	public class ObfAttrMarker : Marker
	{
		// Token: 0x06000202 RID: 514 RVA: 0x0000EDF8 File Offset: 0x0000CFF8
		private static IEnumerable<ObfAttrMarker.ObfuscationAttributeInfo> ReadObfuscationAttributes(IHasCustomAttribute item)
		{
			List<Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo>> list = new List<Tuple<int?, ObfAttrMarker.ObfuscationAttributeInfo>>();
			for (int i = item.CustomAttributes.Count - 1; i >= 0; i--)
			{
				CustomAttribute customAttribute = item.CustomAttributes[i];
				bool flag = customAttribute.TypeFullName != "System.Reflection.ObfuscationAttribute";
				if (!flag)
				{
					ObfAttrMarker.ObfuscationAttributeInfo item2 = default(ObfAttrMarker.ObfuscationAttributeInfo);
					int? item3 = null;
					item2.Owner = item;
					bool flag2 = true;
					foreach (CANamedArgument canamedArgument in customAttribute.Properties)
					{
						string text = canamedArgument.Name;
						string text2 = text;
						if (text2 != null)
						{
							if (!(text2 == "ApplyToMembers"))
							{
								if (!(text2 == "Exclude"))
								{
									if (!(text2 == "StripAfterObfuscation"))
									{
										if (!(text2 == "Feature"))
										{
											goto IL_256;
										}
										Debug.Assert(canamedArgument.Type.ElementType == ElementType.String);
										string text3 = (UTF8String)canamedArgument.Value;
										Match match = ObfAttrMarker.OrderPattern.Match(text3);
										bool success = match.Success;
										if (success)
										{
											string value = match.Groups[1].Value;
											string value2 = match.Groups[2].Value;
											int value3;
											bool flag3 = !int.TryParse(value, out value3);
											if (flag3)
											{
												throw new NotSupportedException(string.Format("Failed to parse feature '{0}' in {1} ", text3, item));
											}
											item3 = new int?(value3);
											text3 = value2;
										}
										int num = text3.IndexOf(':');
										bool flag4 = num == -1;
										if (flag4)
										{
											item2.FeatureName = "";
											item2.FeatureValue = text3;
										}
										else
										{
											item2.FeatureName = text3.Substring(0, num);
											item2.FeatureValue = text3.Substring(num + 1);
										}
									}
									else
									{
										Debug.Assert(canamedArgument.Type.ElementType == ElementType.Boolean);
										flag2 = (bool)canamedArgument.Value;
									}
								}
								else
								{
									Debug.Assert(canamedArgument.Type.ElementType == ElementType.Boolean);
									item2.Exclude = new bool?((bool)canamedArgument.Value);
								}
							}
							else
							{
								Debug.Assert(canamedArgument.Type.ElementType == ElementType.Boolean);
								item2.ApplyToMembers = new bool?((bool)canamedArgument.Value);
							}
							continue;
						}
						IL_256:
						throw new NotSupportedException("Unsupported property: " + canamedArgument.Name);
					}
					bool flag5 = flag2;
					if (flag5)
					{
						item.CustomAttributes.RemoveAt(i);
					}
					list.Add(Tuple.Create<int?, ObfAttrMarker.ObfuscationAttributeInfo>(item3, item2));
				}
			}
			list.Reverse();
			return from pair in list
			orderby pair.Item1
			select pair.Item2;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000F140 File Offset: 0x0000D340
		private bool ToInfo(ObfAttrMarker.ObfuscationAttributeInfo attr, out ObfAttrMarker.ProtectionSettingsInfo info)
		{
			info = default(ObfAttrMarker.ProtectionSettingsInfo);
			info.Condition = null;
			info.Exclude = (attr.Exclude ?? true);
			info.ApplyToMember = (attr.ApplyToMembers ?? true);
			info.Settings = attr.FeatureValue;
			bool flag = true;
			try
			{
				new ObfAttrParser(this.protections).ParseProtectionString(null, info.Settings);
			}
			catch
			{
				flag = false;
			}
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				this.context.Logger.WarnFormat("Ignoring rule '{0}' in {1}.", new object[]
				{
					info.Settings,
					attr.Owner
				});
				result = false;
			}
			else
			{
				bool flag3 = !string.IsNullOrEmpty(attr.FeatureName);
				if (flag3)
				{
					throw new ArgumentException("Feature name must not be set. Owner=" + attr.Owner);
				}
				bool flag4 = info.Exclude && (!string.IsNullOrEmpty(attr.FeatureName) || !string.IsNullOrEmpty(attr.FeatureValue));
				if (flag4)
				{
					throw new ArgumentException("Feature property cannot be set when Exclude is true. Owner=" + attr.Owner);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000F294 File Offset: 0x0000D494
		private ObfAttrMarker.ProtectionSettingsInfo ToInfo(Rule rule, PatternExpression expr)
		{
			ObfAttrMarker.ProtectionSettingsInfo result = default(ObfAttrMarker.ProtectionSettingsInfo);
			result.Condition = expr;
			result.Exclude = false;
			result.ApplyToMember = true;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = rule.Preset > ProtectionPreset.None;
			if (flag)
			{
				stringBuilder.AppendFormat("preset({0});", rule.Preset.ToString().ToLowerInvariant());
			}
			foreach (SettingItem<Protection> settingItem in rule)
			{
				stringBuilder.Append((settingItem.Action == SettingItemAction.Add) ? '+' : '-');
				stringBuilder.Append(settingItem.Id);
				bool flag2 = settingItem.Count > 0;
				if (flag2)
				{
					stringBuilder.Append('(');
					int num = 0;
					foreach (KeyValuePair<string, string> keyValuePair in settingItem)
					{
						bool flag3 = num != 0;
						if (flag3)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.AppendFormat("{0}='{1}'", keyValuePair.Key, keyValuePair.Value.Replace("'", "\\'"));
						num++;
					}
					stringBuilder.Append(')');
				}
				stringBuilder.Append(';');
			}
			result.Settings = stringBuilder.ToString();
			return result;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00002D30 File Offset: 0x00000F30
		private IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> ReadInfos(IHasCustomAttribute item)
		{
			foreach (ObfAttrMarker.ObfuscationAttributeInfo attr in ObfAttrMarker.ReadObfuscationAttributes(item))
			{
				bool flag = !string.IsNullOrEmpty(attr.FeatureName);
				ObfAttrMarker.ProtectionSettingsInfo info;
				if (flag)
				{
					yield return this.AddRule(attr, null);
				}
				else
				{
					bool flag2 = this.ToInfo(attr, out info);
					if (flag2)
					{
						yield return info;
					}
				}
				info = default(ObfAttrMarker.ProtectionSettingsInfo);
				attr = default(ObfAttrMarker.ObfuscationAttributeInfo);
			}
			IEnumerator<ObfAttrMarker.ObfuscationAttributeInfo> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000F42C File Offset: 0x0000D62C
		protected internal override void MarkMember(IDnlibDef member, ConfuserContext context)
		{
			ModuleDef module = ((IMemberRef)member).Module;
			ObfAttrMarker.ProtectionSettingsStack protectionSettingsStack = context.Annotations.Get<ObfAttrMarker.ProtectionSettingsStack>(module, ObfAttrMarker.ModuleSettingsKey, null);
			using (protectionSettingsStack.Apply(member, Enumerable.Empty<ObfAttrMarker.ProtectionSettingsInfo>()))
			{
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000F484 File Offset: 0x0000D684
		protected internal override MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			this.context = context;
			this.project = proj;
			this.extModules = new List<byte[]>();
			bool flag = proj.Packer != null;
			if (flag)
			{
				bool flag2 = !this.packers.ContainsKey(proj.Packer.Id);
				if (flag2)
				{
					context.Logger.ErrorFormat("Cannot find packer with ID '{0}'.", new object[]
					{
						proj.Packer.Id
					});
					throw new ConfuserException(null);
				}
				this.packer = this.packers[proj.Packer.Id];
				this.packerParams = new Dictionary<string, string>(proj.Packer, StringComparer.OrdinalIgnoreCase);
			}
			List<Tuple<ProjectModule, ModuleDefMD>> list = new List<Tuple<ProjectModule, ModuleDefMD>>();
			foreach (ProjectModule projectModule in proj)
			{
				bool isExternal = projectModule.IsExternal;
				if (isExternal)
				{
					this.extModules.Add(projectModule.LoadRaw(proj.BaseDirectory));
				}
				else
				{
					ModuleDefMD moduleDefMD = projectModule.Resolve(proj.BaseDirectory, context.Resolver.DefaultModuleContext);
					foreach (MethodDef methodDef in moduleDefMD.FindDefinitions().OfType<MethodDef>())
					{
						IList<PdbCustomDebugInfo> customDebugInfos = methodDef.CustomDebugInfos;
						context.CheckCancellation();
					}
					context.CheckCancellation();
					context.Resolver.AddToCache(moduleDefMD);
					list.Add(Tuple.Create<ProjectModule, ModuleDefMD>(projectModule, moduleDefMD));
				}
			}
			foreach (Tuple<ProjectModule, ModuleDefMD> tuple in list)
			{
				context.Logger.InfoFormat("Loading '{0}'...", new object[]
				{
					tuple.Item1.Path
				});
				Dictionary<Rule, PatternExpression> dictionary = base.ParseRules(proj, tuple.Item1, context);
				this.MarkModule(tuple.Item1, tuple.Item2, dictionary, tuple == list[0]);
				context.Annotations.Set<Dictionary<Rule, PatternExpression>>(tuple.Item2, Marker.RulesKey, dictionary);
				bool flag3 = this.packer != null;
				if (flag3)
				{
					ProtectionParameters.GetParameters(context, tuple.Item2)[this.packer] = this.packerParams;
				}
			}
			bool flag4 = proj.Debug && proj.Packer != null;
			if (flag4)
			{
				context.Logger.Warn("Generated Debug symbols might not be usable with packers!");
			}
			return new MarkerResult((from module in list
			select module.Item2).ToList<ModuleDefMD>(), this.packer, this.extModules);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000F790 File Offset: 0x0000D990
		private ObfAttrMarker.ProtectionSettingsInfo AddRule(ObfAttrMarker.ObfuscationAttributeInfo attr, List<ObfAttrMarker.ProtectionSettingsInfo> infos)
		{
			Debug.Assert(attr.FeatureName != null);
			string featureName = attr.FeatureName;
			PatternExpression condition;
			try
			{
				condition = new PatternParser().Parse(featureName);
			}
			catch (Exception innerException)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Error when parsing pattern ",
					featureName,
					" in ObfuscationAttribute. Owner=",
					attr.Owner
				}), innerException);
			}
			ObfAttrMarker.ProtectionSettingsInfo protectionSettingsInfo = default(ObfAttrMarker.ProtectionSettingsInfo);
			protectionSettingsInfo.Condition = condition;
			protectionSettingsInfo.Exclude = (attr.Exclude ?? true);
			protectionSettingsInfo.ApplyToMember = (attr.ApplyToMembers ?? true);
			protectionSettingsInfo.Settings = attr.FeatureValue;
			bool flag = true;
			try
			{
				new ObfAttrParser(this.protections).ParseProtectionString(null, protectionSettingsInfo.Settings);
			}
			catch
			{
				flag = false;
			}
			bool flag2 = !flag;
			if (flag2)
			{
				this.context.Logger.WarnFormat("Ignoring rule '{0}' in {1}.", new object[]
				{
					protectionSettingsInfo.Settings,
					attr.Owner
				});
			}
			else
			{
				bool flag3 = infos != null;
				if (flag3)
				{
					infos.Add(protectionSettingsInfo);
				}
			}
			return protectionSettingsInfo;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000F8F0 File Offset: 0x0000DAF0
		private void MarkModule(ProjectModule projModule, ModuleDefMD module, Dictionary<Rule, PatternExpression> rules, bool isMain)
		{
			string text = projModule.SNKeyPath;
			string pass = projModule.SNKeyPassword;
			ObfAttrMarker.ProtectionSettingsStack protectionSettingsStack = new ObfAttrMarker.ProtectionSettingsStack(this.context, this.protections);
			List<ObfAttrMarker.ProtectionSettingsInfo> list = new List<ObfAttrMarker.ProtectionSettingsInfo>();
			foreach (KeyValuePair<Rule, PatternExpression> keyValuePair in rules)
			{
				list.Add(this.ToInfo(keyValuePair.Key, keyValuePair.Value));
			}
			foreach (ObfAttrMarker.ObfuscationAttributeInfo obfuscationAttributeInfo in ObfAttrMarker.ReadObfuscationAttributes(module.Assembly))
			{
				bool flag = string.IsNullOrEmpty(obfuscationAttributeInfo.FeatureName);
				if (flag)
				{
					ObfAttrMarker.ProtectionSettingsInfo item;
					bool flag2 = this.ToInfo(obfuscationAttributeInfo, out item);
					if (flag2)
					{
						list.Add(item);
					}
				}
				else
				{
					bool flag3 = obfuscationAttributeInfo.FeatureName.Equals("generate debug symbol", StringComparison.OrdinalIgnoreCase);
					if (flag3)
					{
						bool flag4 = !isMain;
						if (flag4)
						{
							throw new ArgumentException("Only main module can set 'generate debug symbol'.");
						}
						this.project.Debug = bool.Parse(obfuscationAttributeInfo.FeatureValue);
					}
					else
					{
						bool flag5 = obfuscationAttributeInfo.FeatureName.Equals("random seed", StringComparison.OrdinalIgnoreCase);
						if (flag5)
						{
							bool flag6 = !isMain;
							if (flag6)
							{
								throw new ArgumentException("Only main module can set 'random seed'.");
							}
							this.project.Seed = obfuscationAttributeInfo.FeatureValue;
						}
						else
						{
							bool flag7 = obfuscationAttributeInfo.FeatureName.Equals("strong name key", StringComparison.OrdinalIgnoreCase);
							if (flag7)
							{
								text = Path.Combine(this.project.BaseDirectory, obfuscationAttributeInfo.FeatureValue);
							}
							else
							{
								bool flag8 = obfuscationAttributeInfo.FeatureName.Equals("strong name key password", StringComparison.OrdinalIgnoreCase);
								if (flag8)
								{
									pass = obfuscationAttributeInfo.FeatureValue;
								}
								else
								{
									bool flag9 = obfuscationAttributeInfo.FeatureName.Equals("packer", StringComparison.OrdinalIgnoreCase);
									if (flag9)
									{
										bool flag10 = !isMain;
										if (flag10)
										{
											throw new ArgumentException("Only main module can set 'packer'.");
										}
										new ObfAttrParser(this.packers).ParsePackerString(obfuscationAttributeInfo.FeatureValue, out this.packer, out this.packerParams);
									}
									else
									{
										bool flag11 = obfuscationAttributeInfo.FeatureName.Equals("external module", StringComparison.OrdinalIgnoreCase);
										if (flag11)
										{
											bool flag12 = !isMain;
											if (flag12)
											{
												throw new ArgumentException("Only main module can add external modules.");
											}
											byte[] item2 = new ProjectModule
											{
												Path = obfuscationAttributeInfo.FeatureValue
											}.LoadRaw(this.project.BaseDirectory);
											this.extModules.Add(item2);
										}
										else
										{
											this.AddRule(obfuscationAttributeInfo, list);
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag13 = this.project.Debug && module.PdbState == null;
			if (flag13)
			{
				module.LoadPdb();
			}
			text = ((text == null) ? null : Path.Combine(this.project.BaseDirectory, text));
			StrongNameKey value = Marker.LoadSNKey(this.context, text, pass);
			this.context.Annotations.Set<StrongNameKey>(module, Marker.SNKey, value);
			using (protectionSettingsStack.Apply(module, list))
			{
				this.ProcessModule(module, protectionSettingsStack);
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000FC74 File Offset: 0x0000DE74
		private void ProcessModule(ModuleDefMD module, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			this.context.Annotations.Set<ObfAttrMarker.ProtectionSettingsStack>(module, ObfAttrMarker.ModuleSettingsKey, new ObfAttrMarker.ProtectionSettingsStack(stack));
			foreach (TypeDef typeDef in module.Types)
			{
				using (stack.Apply(typeDef, this.ReadInfos(typeDef)))
				{
					this.ProcessTypeMembers(typeDef, stack);
				}
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000FD10 File Offset: 0x0000DF10
		private void ProcessTypeMembers(TypeDef type, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			foreach (TypeDef typeDef in type.NestedTypes)
			{
				using (stack.Apply(typeDef, this.ReadInfos(typeDef)))
				{
					this.ProcessTypeMembers(typeDef, stack);
				}
			}
			foreach (PropertyDef propertyDef in type.Properties)
			{
				using (stack.Apply(propertyDef, this.ReadInfos(propertyDef)))
				{
					bool flag = propertyDef.GetMethod != null;
					if (flag)
					{
						this.ProcessMember(propertyDef.GetMethod, stack);
					}
					bool flag2 = propertyDef.SetMethod != null;
					if (flag2)
					{
						this.ProcessMember(propertyDef.SetMethod, stack);
					}
					foreach (MethodDef member in propertyDef.OtherMethods)
					{
						this.ProcessMember(member, stack);
					}
				}
			}
			foreach (EventDef eventDef in type.Events)
			{
				using (stack.Apply(eventDef, this.ReadInfos(eventDef)))
				{
					bool flag3 = eventDef.AddMethod != null;
					if (flag3)
					{
						this.ProcessMember(eventDef.AddMethod, stack);
					}
					bool flag4 = eventDef.RemoveMethod != null;
					if (flag4)
					{
						this.ProcessMember(eventDef.RemoveMethod, stack);
					}
					bool flag5 = eventDef.InvokeMethod != null;
					if (flag5)
					{
						this.ProcessMember(eventDef.InvokeMethod, stack);
					}
					foreach (MethodDef member2 in eventDef.OtherMethods)
					{
						this.ProcessMember(member2, stack);
					}
				}
			}
			foreach (MethodDef methodDef in type.Methods)
			{
				bool flag6 = methodDef.SemanticsAttributes == MethodSemanticsAttributes.None;
				if (flag6)
				{
					this.ProcessMember(methodDef, stack);
				}
			}
			foreach (FieldDef member3 in type.Fields)
			{
				this.ProcessMember(member3, stack);
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0001003C File Offset: 0x0000E23C
		private void ProcessMember(IDnlibDef member, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			using (stack.Apply(member, this.ReadInfos(member)))
			{
				this.ProcessBody(member as MethodDef, stack);
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00010084 File Offset: 0x0000E284
		private void ProcessBody(MethodDef method, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			bool flag = method == null || method.Body == null;
			if (!flag)
			{
				TypeDef declaringType = method.DeclaringType;
				foreach (Instruction instruction in method.Body.Instructions)
				{
					bool flag2 = instruction.Operand is MethodDef;
					if (flag2)
					{
						TypeDef declaringType2 = ((MethodDef)instruction.Operand).DeclaringType;
						bool flag3 = declaringType2.DeclaringType == declaringType && declaringType2.IsCompilerGenerated();
						if (flag3)
						{
							using (stack.Apply(declaringType2, this.ReadInfos(declaringType2)))
							{
								this.ProcessTypeMembers(declaringType2, stack);
							}
						}
					}
				}
			}
		}

		// Token: 0x0400019A RID: 410
		private static readonly Regex OrderPattern = new Regex("^(\\d+)\\. (.+)$");

		// Token: 0x0400019B RID: 411
		private ConfuserContext context;

		// Token: 0x0400019C RID: 412
		private ConfuserProject project;

		// Token: 0x0400019D RID: 413
		private Packer packer;

		// Token: 0x0400019E RID: 414
		private Dictionary<string, string> packerParams;

		// Token: 0x0400019F RID: 415
		private List<byte[]> extModules;

		// Token: 0x040001A0 RID: 416
		private static readonly object ModuleSettingsKey = new object();

		// Token: 0x02000052 RID: 82
		private struct ObfuscationAttributeInfo
		{
			// Token: 0x040001A1 RID: 417
			public IHasCustomAttribute Owner;

			// Token: 0x040001A2 RID: 418
			public bool? ApplyToMembers;

			// Token: 0x040001A3 RID: 419
			public bool? Exclude;

			// Token: 0x040001A4 RID: 420
			public string FeatureName;

			// Token: 0x040001A5 RID: 421
			public string FeatureValue;
		}

		// Token: 0x02000053 RID: 83
		private struct ProtectionSettingsInfo
		{
			// Token: 0x040001A6 RID: 422
			public bool ApplyToMember;

			// Token: 0x040001A7 RID: 423
			public bool Exclude;

			// Token: 0x040001A8 RID: 424
			public PatternExpression Condition;

			// Token: 0x040001A9 RID: 425
			public string Settings;
		}

		// Token: 0x02000054 RID: 84
		private class ProtectionSettingsStack
		{
			// Token: 0x06000210 RID: 528 RVA: 0x00002D6B File Offset: 0x00000F6B
			public ProtectionSettingsStack(ConfuserContext context, Dictionary<string, Protection> protections)
			{
				this.context = context;
				this.stack = new Stack<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>>();
				this.parser = new ObfAttrParser(protections);
			}

			// Token: 0x06000211 RID: 529 RVA: 0x00002D93 File Offset: 0x00000F93
			public ProtectionSettingsStack(ObfAttrMarker.ProtectionSettingsStack copy)
			{
				this.context = copy.context;
				this.stack = new Stack<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>>(copy.stack);
				this.parser = copy.parser;
			}

			// Token: 0x06000212 RID: 530 RVA: 0x00002DC6 File Offset: 0x00000FC6
			private void Pop()
			{
				this.settings = this.stack.Pop().Item1;
			}

			// Token: 0x06000213 RID: 531 RVA: 0x0001016C File Offset: 0x0000E36C
			public IDisposable Apply(IDnlibDef target, IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> infos)
			{
				bool flag = this.settings == null;
				ProtectionSettings parameters;
				if (flag)
				{
					parameters = new ProtectionSettings();
				}
				else
				{
					parameters = new ProtectionSettings(this.settings);
				}
				ObfAttrMarker.ProtectionSettingsInfo[] array = infos.ToArray<ObfAttrMarker.ProtectionSettingsInfo>();
				bool flag2 = this.stack.Count > 0;
				if (flag2)
				{
					foreach (Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]> tuple in this.stack.Reverse<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>>())
					{
						this.ApplyInfo(target, parameters, tuple.Item2, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.ParentInfo);
					}
				}
				bool flag3 = array.Length != 0;
				IDisposable result;
				if (flag3)
				{
					ProtectionSettings item = this.settings;
					this.ApplyInfo(target, parameters, array, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoInherits);
					this.settings = new ProtectionSettings(parameters);
					this.ApplyInfo(target, parameters, array, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoOnly);
					this.stack.Push(Tuple.Create<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>(item, array));
					result = new ObfAttrMarker.ProtectionSettingsStack.PopHolder(this);
				}
				else
				{
					result = null;
				}
				ProtectionParameters.SetParameters(this.context, target, parameters);
				return result;
			}

			// Token: 0x06000214 RID: 532 RVA: 0x0001027C File Offset: 0x0000E47C
			private void ApplyInfo(IDnlibDef context, ProtectionSettings settings, IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> infos, ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType type)
			{
				foreach (ObfAttrMarker.ProtectionSettingsInfo protectionSettingsInfo in infos)
				{
					bool flag = protectionSettingsInfo.Condition != null && !(bool)protectionSettingsInfo.Condition.Evaluate(context);
					if (!flag)
					{
						bool flag2 = protectionSettingsInfo.Condition == null && protectionSettingsInfo.Exclude;
						if (flag2)
						{
							bool flag3 = type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoOnly || (type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoInherits && protectionSettingsInfo.ApplyToMember);
							if (flag3)
							{
								settings.Clear();
							}
						}
						bool flag4 = !string.IsNullOrEmpty(protectionSettingsInfo.Settings);
						if (flag4)
						{
							bool flag5 = (type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.ParentInfo && protectionSettingsInfo.Condition != null && protectionSettingsInfo.ApplyToMember) || type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoOnly || (type == ObfAttrMarker.ProtectionSettingsStack.ApplyInfoType.CurrentInfoInherits && protectionSettingsInfo.Condition == null && protectionSettingsInfo.ApplyToMember);
							if (flag5)
							{
								this.parser.ParseProtectionString(settings, protectionSettingsInfo.Settings);
							}
						}
					}
				}
			}

			// Token: 0x040001AA RID: 426
			private readonly ConfuserContext context;

			// Token: 0x040001AB RID: 427
			private readonly Stack<Tuple<ProtectionSettings, ObfAttrMarker.ProtectionSettingsInfo[]>> stack;

			// Token: 0x040001AC RID: 428
			private readonly ObfAttrParser parser;

			// Token: 0x040001AD RID: 429
			private ProtectionSettings settings;

			// Token: 0x02000055 RID: 85
			private enum ApplyInfoType
			{
				// Token: 0x040001AF RID: 431
				CurrentInfoOnly,
				// Token: 0x040001B0 RID: 432
				CurrentInfoInherits,
				// Token: 0x040001B1 RID: 433
				ParentInfo
			}

			// Token: 0x02000056 RID: 86
			private class PopHolder : IDisposable
			{
				// Token: 0x06000215 RID: 533 RVA: 0x00002DDF File Offset: 0x00000FDF
				public PopHolder(ObfAttrMarker.ProtectionSettingsStack parent)
				{
					this.parent = parent;
				}

				// Token: 0x06000216 RID: 534 RVA: 0x00002DF0 File Offset: 0x00000FF0
				public void Dispose()
				{
					this.parent.Pop();
				}

				// Token: 0x040001B2 RID: 434
				private ObfAttrMarker.ProtectionSettingsStack parent;
			}
		}
	}
}
