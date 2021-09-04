using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x0200006F RID: 111
	internal class CaliburnAnalyzer : IRenamer
	{
		// Token: 0x060002B1 RID: 689 RVA: 0x0002569B File Offset: 0x0002389B
		public CaliburnAnalyzer(WPFAnalyzer wpfAnalyzer)
		{
			wpfAnalyzer.AnalyzeBAMLElement += this.AnalyzeBAMLElement;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x000256B8 File Offset: 0x000238B8
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			TypeDef typeDef = def as TypeDef;
			bool flag = typeDef == null || typeDef.DeclaringType != null;
			if (!flag)
			{
				bool flag2 = typeDef.Name.Contains("ViewModel");
				if (flag2)
				{
					string str = typeDef.Namespace.Replace("ViewModels", "Views");
					string str2 = typeDef.Name.Replace("PageViewModel", "Page").Replace("ViewModel", "View");
					TypeDef typeDef2 = typeDef.Module.Find(str + "." + str2, true);
					bool flag3 = typeDef2 != null;
					if (flag3)
					{
						service.SetCanRename(typeDef, false);
						service.SetCanRename(typeDef2, false);
					}
					string right = typeDef.Namespace + "." + typeDef.Name.Replace("ViewModel", "");
					foreach (TypeDef typeDef3 in typeDef.Module.Types)
					{
						bool flag4 = typeDef3.Namespace == right;
						if (flag4)
						{
							service.SetCanRename(typeDef, false);
							service.SetCanRename(typeDef3, false);
						}
					}
				}
			}
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00025820 File Offset: 0x00023A20
		private void AnalyzeBAMLElement(BAMLAnalyzer analyzer, BamlElement elem)
		{
			foreach (BamlRecord bamlRecord in elem.Body)
			{
				PropertyWithConverterRecord propertyWithConverterRecord = bamlRecord as PropertyWithConverterRecord;
				bool flag = propertyWithConverterRecord == null;
				if (!flag)
				{
					Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple = analyzer.ResolveAttribute(propertyWithConverterRecord.AttributeId);
					string a = null;
					bool flag2 = tuple.Item2 != null;
					if (flag2)
					{
						a = tuple.Item2.Name;
					}
					else
					{
						bool flag3 = tuple.Item1 != null;
						if (flag3)
						{
							a = tuple.Item1.Name;
						}
					}
					bool flag4 = a == "Attach" || (a == "Value" && propertyWithConverterRecord.Value.Contains("Action"));
					if (flag4)
					{
						this.AnalyzeMessageAttach(analyzer, tuple, propertyWithConverterRecord.Value);
					}
					bool flag5 = a == "Name";
					if (flag5)
					{
						this.AnalyzeAutoBind(analyzer, tuple, propertyWithConverterRecord.Value);
					}
					bool flag6 = a == "MethodName";
					if (flag6)
					{
						this.AnalyzeActionMessage(analyzer, tuple, propertyWithConverterRecord.Value);
					}
				}
			}
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0002596C File Offset: 0x00023B6C
		private void AnalyzeMessageAttach(BAMLAnalyzer analyzer, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attr, string value)
		{
			bool flag = attr.Item2 == null;
			if (!flag)
			{
				TypeDef typeDef = analyzer.ResolveType(attr.Item2.OwnerTypeId);
				bool flag2 = typeDef.FullName != "Caliburn.Micro.Message" && typeDef.FullName != "System.Windows.Setter";
				if (!flag2)
				{
					foreach (string text in value.Split(new char[]
					{
						';'
					}))
					{
						bool flag3 = text.Contains("=");
						string text2;
						if (flag3)
						{
							text2 = text.Split(new char[]
							{
								'='
							})[1].Trim(new char[]
							{
								'[',
								']',
								' '
							});
						}
						else
						{
							text2 = text.Trim(new char[]
							{
								'[',
								']',
								' '
							});
						}
						bool flag4 = text2.StartsWith("Action");
						if (flag4)
						{
							text2 = text2.Substring(6);
						}
						int num = text2.IndexOf('(');
						bool flag5 = num != -1;
						if (flag5)
						{
							text2 = text2.Substring(0, num);
						}
						string name = text2.Trim();
						foreach (MethodDef obj in analyzer.LookupMethod(name))
						{
							analyzer.NameService.SetCanRename(obj, false);
						}
					}
				}
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00025AFC File Offset: 0x00023CFC
		private void AnalyzeAutoBind(BAMLAnalyzer analyzer, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attr, string value)
		{
			bool flag = !(attr.Item1 is PropertyDef) || ((PropertyDef)attr.Item1).DeclaringType.FullName != "System.Windows.FrameworkElement";
			if (!flag)
			{
				foreach (MethodDef obj in analyzer.LookupMethod(value))
				{
					analyzer.NameService.SetCanRename(obj, false);
				}
				foreach (PropertyDef obj2 in analyzer.LookupProperty(value))
				{
					analyzer.NameService.SetCanRename(obj2, false);
				}
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00025BD4 File Offset: 0x00023DD4
		private void AnalyzeActionMessage(BAMLAnalyzer analyzer, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> attr, string value)
		{
			bool flag = attr.Item2 == null;
			if (!flag)
			{
				TypeDef typeDef = analyzer.ResolveType(attr.Item2.OwnerTypeId);
				bool flag2 = typeDef.FullName != "Caliburn.Micro.ActionMessage";
				if (!flag2)
				{
					foreach (MethodDef obj in analyzer.LookupMethod(value))
					{
						analyzer.NameService.SetCanRename(obj, false);
					}
				}
			}
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}
	}
}
