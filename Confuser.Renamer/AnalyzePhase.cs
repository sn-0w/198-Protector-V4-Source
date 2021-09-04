using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Renamer.Analyzers;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000002 RID: 2
	internal class AnalyzePhase : ProtectionPhase
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public AnalyzePhase(NameProtection parent) : base(parent)
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
		public override bool ProcessAll
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002070 File Offset: 0x00000270
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.AllDefinitions;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002084 File Offset: 0x00000284
		public override string Name
		{
			get
			{
				return "Name analysis";
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000209C File Offset: 0x0000029C
		private void ParseParameters(IDnlibDef def, ConfuserContext context, NameService service, ProtectionParameters parameters)
		{
			RenameMode? parameter = parameters.GetParameter<RenameMode?>(context, def, "mode", null);
			bool flag = parameter != null;
			if (flag)
			{
				service.SetRenameMode(def, parameter.Value);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020DC File Offset: 0x000002DC
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			NameService nameService = (NameService)context.Registry.GetService<INameService>();
			context.Logger.Debug("Building VTables & identifier list...");
			foreach (IDnlibDef dnlibDef in parameters.Targets.WithProgress(context.Logger))
			{
				this.ParseParameters(dnlibDef, context, nameService, parameters);
				bool flag = dnlibDef is ModuleDef;
				if (flag)
				{
					ModuleDef moduleDef = (ModuleDef)dnlibDef;
					foreach (Resource resource in moduleDef.Resources)
					{
						nameService.SetOriginalName(resource, resource.Name);
					}
				}
				else
				{
					nameService.SetOriginalName(dnlibDef, dnlibDef.Name);
				}
				bool flag2 = dnlibDef is TypeDef;
				if (flag2)
				{
					nameService.GetVTables().GetVTable((TypeDef)dnlibDef);
					nameService.SetOriginalNamespace(dnlibDef, ((TypeDef)dnlibDef).Namespace);
				}
				context.CheckCancellation();
			}
			context.Logger.Debug("Analyzing...");
			this.RegisterRenamers(context, nameService);
			IList<IRenamer> renamers = nameService.Renamers;
			foreach (IDnlibDef def in parameters.Targets.WithProgress(context.Logger))
			{
				this.Analyze(nameService, context, parameters, def, true);
				context.CheckCancellation();
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000022AC File Offset: 0x000004AC
		private void RegisterRenamers(ConfuserContext context, NameService service)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			foreach (ModuleDefMD moduleDefMD in context.Modules)
			{
				foreach (AssemblyRef assemblyRef in moduleDefMD.GetAssemblyRefs())
				{
					bool flag8 = assemblyRef.Name == "WindowsBase" || assemblyRef.Name == "PresentationCore" || assemblyRef.Name == "PresentationFramework" || assemblyRef.Name == "System.Xaml";
					if (flag8)
					{
						flag = true;
					}
					else
					{
						bool flag9 = assemblyRef.Name == "Caliburn.Micro";
						if (flag9)
						{
							flag2 = true;
						}
						else
						{
							bool flag10 = assemblyRef.Name == "System.Windows.Forms";
							if (flag10)
							{
								flag3 = true;
							}
							else
							{
								bool flag11 = assemblyRef.Name == "Newtonsoft.Json";
								if (flag11)
								{
									flag4 = true;
								}
								else
								{
									bool flag12 = assemblyRef.Name.StartsWith("Microsoft.AspNetCore.");
									if (flag12)
									{
										flag6 = true;
									}
									else
									{
										bool flag13 = assemblyRef.Name == "Nancy";
										if (flag13)
										{
											flag7 = true;
										}
									}
								}
							}
						}
					}
				}
				TypeDef typeDef = moduleDefMD.FindNormal("Microsoft.VisualBasic.Embedded");
				bool flag14 = typeDef != null && typeDef.BaseType.FullName.Equals("System.Attribute");
				if (flag14)
				{
					flag5 = true;
				}
			}
			bool flag15 = flag;
			if (flag15)
			{
				WPFAnalyzer wpfanalyzer = new WPFAnalyzer();
				context.Logger.Debug("WPF found, enabling compatibility.");
				service.Renamers.Add(wpfanalyzer);
				bool flag16 = flag2;
				if (flag16)
				{
					context.Logger.Debug("Caliburn.Micro found, enabling compatibility.");
					service.Renamers.Add(new CaliburnAnalyzer(wpfanalyzer));
				}
			}
			bool flag17 = flag3;
			if (flag17)
			{
				WinFormsAnalyzer item = new WinFormsAnalyzer();
				context.Logger.Debug("WinForms found, enabling compatibility.");
				service.Renamers.Add(item);
			}
			bool flag18 = flag4;
			if (flag18)
			{
				JsonAnalyzer item2 = new JsonAnalyzer();
				context.Logger.Debug("Newtonsoft.Json found, enabling compatibility.");
				service.Renamers.Add(item2);
			}
			bool flag19 = flag5;
			if (flag19)
			{
				VisualBasicRuntimeAnalyzer item3 = new VisualBasicRuntimeAnalyzer();
				context.Logger.Debug("Visual Basic Embedded Runtime found, enabling compatibility.");
				service.Renamers.Add(item3);
			}
			bool flag20 = flag6;
			if (flag20)
			{
				AspNetCoreAnalyzer item4 = new AspNetCoreAnalyzer();
				context.Logger.Debug("ASP.NET Core found, enabling compatibility.");
				service.Renamers.Add(item4);
			}
			bool flag21 = flag7;
			if (flag21)
			{
				NancyFxAnalyzer item5 = new NancyFxAnalyzer();
				context.Logger.Debug("NancyFx found, enabling compatibility.");
				service.Renamers.Add(item5);
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000025E0 File Offset: 0x000007E0
		internal void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, IDnlibDef def, bool runAnalyzer)
		{
			bool flag = def is TypeDef;
			if (flag)
			{
				this.Analyze(service, context, parameters, (TypeDef)def);
			}
			else
			{
				bool flag2 = def is MethodDef;
				if (flag2)
				{
					this.Analyze(service, context, parameters, (MethodDef)def);
				}
				else
				{
					bool flag3 = def is FieldDef;
					if (flag3)
					{
						this.Analyze(service, context, parameters, (FieldDef)def);
					}
					else
					{
						bool flag4 = def is PropertyDef;
						if (flag4)
						{
							this.Analyze(service, context, parameters, (PropertyDef)def);
						}
						else
						{
							bool flag5 = def is EventDef;
							if (flag5)
							{
								this.Analyze(service, context, parameters, (EventDef)def);
							}
							else
							{
								bool flag6 = def is ModuleDef;
								if (flag6)
								{
									string parameter = parameters.GetParameter<string>(context, def, "password", null);
									bool flag7 = parameter != null;
									if (flag7)
									{
										service.reversibleRenamer = new ReversibleRenamer(parameter);
									}
									uint parameter2 = parameters.GetParameter<uint>(context, def, "idOffset", 0U);
									bool flag8 = parameter2 > 0U;
									if (flag8)
									{
										service.SetNameId(parameter2);
									}
									service.SetCanRename(def, false);
								}
							}
						}
					}
				}
			}
			bool flag9 = !runAnalyzer || parameters.GetParameter<bool>(context, def, "forceRen", false);
			if (!flag9)
			{
				bool flag10 = def.HasAttribute("UnityEngine.SerializeField");
				if (flag10)
				{
					service.SetCanRename(def, false);
				}
				foreach (IRenamer renamer in service.Renamers)
				{
					renamer.Analyze(context, service, parameters, def);
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002794 File Offset: 0x00000994
		private static bool IsVisibleOutside(ConfuserContext context, ProtectionParameters parameters, IMemberDef def)
		{
			bool? parameter = parameters.GetParameter<bool?>(context, def, "renPublic", null);
			bool? parameter2 = parameters.GetParameter<bool?>(context, def, "renInternal", null);
			TypeDef typeDef = def as TypeDef;
			bool flag = typeDef == null;
			if (flag)
			{
				typeDef = def.DeclaringType;
			}
			else
			{
				bool isSerializable = typeDef.IsSerializable;
				if (isSerializable)
				{
					return true;
				}
			}
			MethodDef methodDef = def as MethodDef;
			bool flag2 = methodDef != null;
			if (flag2)
			{
				bool flag3 = parameter2 == null || parameter2.Value;
				if (flag3)
				{
					bool flag4 = !methodDef.IsFamily && !methodDef.IsFamilyOrAssembly && !methodDef.IsPublic;
					if (flag4)
					{
						return false;
					}
					bool flag5 = methodDef.HasPrivateFlags() && !methodDef.IsFamily && !methodDef.IsFamilyOrAssembly && !methodDef.IsPublic;
					if (flag5)
					{
						return false;
					}
				}
			}
			FieldDef fieldDef = def as FieldDef;
			bool flag6 = fieldDef != null;
			if (flag6)
			{
				bool flag7 = parameter2 == null || parameter2.Value;
				if (flag7)
				{
					bool flag8 = !fieldDef.IsFamily && !fieldDef.IsFamilyOrAssembly && !fieldDef.IsPublic;
					if (flag8)
					{
						return false;
					}
					bool flag9 = fieldDef.HasPrivateFlags() && !fieldDef.IsFamily && !fieldDef.IsFamilyOrAssembly && !fieldDef.IsPublic;
					if (flag9)
					{
						return false;
					}
				}
			}
			PropertyDef propertyDef = def as PropertyDef;
			bool flag10 = propertyDef != null;
			if (flag10)
			{
				bool flag11 = parameter2 == null || parameter2.Value;
				if (flag11)
				{
					bool flag12 = !propertyDef.IsFamily() && !propertyDef.IsFamilyOrAssembly() && !propertyDef.IsPublic();
					if (flag12)
					{
						return false;
					}
					bool flag13 = propertyDef.HasAllPrivateFlags() && !propertyDef.IsFamily() && !propertyDef.IsFamilyOrAssembly() && !propertyDef.IsPublic();
					if (flag13)
					{
						return false;
					}
				}
			}
			EventDef eventDef = def as EventDef;
			bool flag14 = eventDef != null;
			if (flag14)
			{
				bool flag15 = parameter2 == null || parameter2.Value;
				if (flag15)
				{
					bool flag16 = !eventDef.IsFamily() && !eventDef.IsFamilyOrAssembly() && !eventDef.IsPublic();
					if (flag16)
					{
						return false;
					}
					bool flag17 = eventDef.HasAllPrivateFlags() && !eventDef.IsFamily() && !eventDef.IsFamilyOrAssembly() && !eventDef.IsPublic();
					if (flag17)
					{
						return false;
					}
				}
			}
			bool flag18 = parameter == null;
			bool result;
			if (flag18)
			{
				result = typeDef.IsVisibleOutside(true, true);
			}
			else
			{
				bool flag19 = parameter2 == null;
				if (flag19)
				{
					result = (typeDef.IsVisibleOutside(false, true) && !parameter.Value);
				}
				else
				{
					result = (typeDef.IsVisibleOutside(false, parameter2.Value) && !parameter.Value);
				}
			}
			return result;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002AA4 File Offset: 0x00000CA4
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, TypeDef type)
		{
			bool flag = AnalyzePhase.IsVisibleOutside(context, parameters, type);
			if (flag)
			{
				service.SetCanRename(type, false);
			}
			else
			{
				bool flag2 = type.IsRuntimeSpecialName || type.IsGlobalModuleType;
				if (flag2)
				{
					service.SetCanRename(type, false);
				}
				else
				{
					bool flag3 = type.FullName == "ProtectedByAttribute";
					if (flag3)
					{
						service.SetCanRename(type, false);
					}
				}
			}
			bool flag4 = type.CustomAttributes.Any((CustomAttribute x) => x.AttributeType.FullName == "System.Reflection.ObfuscationAttribute");
			if (flag4)
			{
				CustomAttribute customAttribute = type.CustomAttributes.First((CustomAttribute x) => x.AttributeType.FullName == "System.Reflection.ObfuscationAttribute");
				CANamedArgument canamedArgument = customAttribute.NamedArguments.FirstOrDefault((CANamedArgument x) => x.Name == "Exclude");
				bool flag5 = canamedArgument != null && (bool)canamedArgument.Value;
				if (flag5)
				{
					service.SetCanRename(type, false);
					foreach (MethodDef obj in type.Methods)
					{
						service.SetCanRename(obj, false);
					}
					foreach (FieldDef obj2 in from x in type.Fields
					where x.IsStatic && x.IsPublic
					select x)
					{
						service.SetCanRename(obj2, false);
					}
					type.CustomAttributes.Remove(customAttribute);
				}
			}
			bool flag6 = type != null;
			if (flag6)
			{
				bool isSerializable = type.IsSerializable;
				if (isSerializable)
				{
					service.SetCanRename(type, false);
				}
				bool flag7 = type.DeclaringType != null;
				if (flag7)
				{
					bool isSerializable2 = type.DeclaringType.IsSerializable;
					if (isSerializable2)
					{
						service.SetCanRename(type, false);
					}
				}
			}
			bool parameter = parameters.GetParameter<bool>(context, type, "forceRen", false);
			if (!parameter)
			{
				bool flag8 = type.InheritsFromCorlib("System.Attribute");
				if (flag8)
				{
					service.ReduceRenameMode(type, RenameMode.ASCII);
				}
				bool flag9 = type.InheritsFrom("System.Configuration.SettingsBase");
				if (flag9)
				{
					service.SetCanRename(type, false);
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002D38 File Offset: 0x00000F38
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, MethodDef method)
		{
			bool flag = AnalyzePhase.IsVisibleOutside(context, parameters, method.DeclaringType) && (method.IsFamily || method.IsFamilyOrAssembly || method.IsPublic) && AnalyzePhase.IsVisibleOutside(context, parameters, method);
			if (flag)
			{
				service.SetCanRename(method, false);
			}
			else
			{
				bool isRuntimeSpecialName = method.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(method, false);
				}
				else
				{
					bool flag2 = method.IsExplicitlyImplementedInterfaceMember();
					if (flag2)
					{
						service.SetCanRename(method, false);
					}
					else
					{
						bool parameter = parameters.GetParameter<bool>(context, method, "forceRen", false);
						if (!parameter)
						{
							bool flag3 = method.DeclaringType.IsComImport() && !method.HasAttribute("System.Runtime.InteropServices.DispIdAttribute");
							if (flag3)
							{
								service.SetCanRename(method, false);
							}
							else
							{
								bool flag4 = method.DeclaringType.IsDelegate();
								if (flag4)
								{
									service.SetCanRename(method, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002E24 File Offset: 0x00001024
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, FieldDef field)
		{
			bool flag = AnalyzePhase.IsVisibleOutside(context, parameters, field.DeclaringType) && (field.IsFamily || field.IsFamilyOrAssembly || field.IsPublic) && AnalyzePhase.IsVisibleOutside(context, parameters, field);
			if (flag)
			{
				service.SetCanRename(field, false);
			}
			else
			{
				bool isRuntimeSpecialName = field.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(field, false);
				}
				else
				{
					bool parameter = parameters.GetParameter<bool>(context, field, "forceRen", false);
					if (!parameter)
					{
						bool isSerializable = field.DeclaringType.IsSerializable;
						if (isSerializable)
						{
							service.SetCanRename(field, false);
						}
						else
						{
							bool flag2 = field.DeclaringType.IsSerializable && (field.CustomAttributes.IsDefined("XmlIgnore") || field.CustomAttributes.IsDefined("XmlIgnoreAttribute") || field.CustomAttributes.IsDefined("System.Xml.Serialization.XmlIgnore") || field.CustomAttributes.IsDefined("System.Xml.Serialization.XmlIgnoreAttribute") || field.CustomAttributes.IsDefined("T:System.Xml.Serialization.XmlIgnoreAttribute"));
							if (flag2)
							{
								service.SetCanRename(field, true);
							}
							else
							{
								bool flag3 = field.HasAttribute("UnityEngine.SerializeField");
								if (flag3)
								{
									service.SetCanRename(field, false);
								}
								else
								{
									bool flag4 = field.HasAttribute("FullInspector.ShowInInspector");
									if (flag4)
									{
										service.SetCanRename(field, false);
									}
									else
									{
										bool flag5 = field.IsLiteral && field.DeclaringType.IsEnum && !parameters.GetParameter<bool>(context, field, "renEnum", false);
										if (flag5)
										{
											service.SetCanRename(field, false);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002FC8 File Offset: 0x000011C8
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, PropertyDef property)
		{
			bool flag = AnalyzePhase.IsVisibleOutside(context, parameters, property.DeclaringType) && property.IsPublic() && AnalyzePhase.IsVisibleOutside(context, parameters, property);
			if (flag)
			{
				service.SetCanRename(property, false);
			}
			else
			{
				bool isRuntimeSpecialName = property.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(property, false);
				}
				else
				{
					bool flag2 = property.IsExplicitlyImplementedInterfaceMember();
					if (flag2)
					{
						service.SetCanRename(property, false);
					}
					else
					{
						bool parameter = parameters.GetParameter<bool>(context, property, "forceRen", false);
						if (!parameter)
						{
							bool isSerializable = property.DeclaringType.IsSerializable;
							if (isSerializable)
							{
								service.SetCanRename(property, false);
							}
							else
							{
								bool flag3 = property.DeclaringType.IsSerializable && (property.CustomAttributes.IsDefined("XmlIgnore") || property.CustomAttributes.IsDefined("XmlIgnoreAttribute") || property.CustomAttributes.IsDefined("System.Xml.Serialization.XmlIgnore") || property.CustomAttributes.IsDefined("System.Xml.Serialization.XmlIgnoreAttribute") || property.CustomAttributes.IsDefined("T:System.Xml.Serialization.XmlIgnoreAttribute"));
								if (flag3)
								{
									service.SetCanRename(property, true);
								}
								else
								{
									bool flag4 = property.HasAttribute("UnityEngine.SerializeField");
									if (flag4)
									{
										service.SetCanRename(property, false);
									}
									else
									{
										bool flag5 = property.HasAttribute("FullInspector.ShowInInspector");
										if (flag5)
										{
											service.SetCanRename(property, false);
										}
										else
										{
											bool flag6 = property.DeclaringType.Implements("System.ComponentModel.INotifyPropertyChanged");
											if (flag6)
											{
												service.SetCanRename(property, false);
											}
											else
											{
												bool flag7 = property.DeclaringType.Name.String.Contains("AnonymousType");
												if (flag7)
												{
													service.SetCanRename(property, false);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000318C File Offset: 0x0000138C
		private void Analyze(NameService service, ConfuserContext context, ProtectionParameters parameters, EventDef evt)
		{
			bool flag = AnalyzePhase.IsVisibleOutside(context, parameters, evt.DeclaringType) && evt.IsPublic() && AnalyzePhase.IsVisibleOutside(context, parameters, evt);
			if (flag)
			{
				service.SetCanRename(evt, false);
			}
			else
			{
				bool isRuntimeSpecialName = evt.IsRuntimeSpecialName;
				if (isRuntimeSpecialName)
				{
					service.SetCanRename(evt, false);
				}
				else
				{
					bool flag2 = evt.IsExplicitlyImplementedInterfaceMember();
					if (flag2)
					{
						service.SetCanRename(evt, false);
					}
				}
			}
		}
	}
}
