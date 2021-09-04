using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Confuser.Core;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001B RID: 27
	public class ProjectRuleVM : ViewModelBase, IViewModel<Rule>
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00004918 File Offset: 0x00004918
		public ProjectRuleVM(ProjectVM parent, Rule rule)
		{
			this.parent = parent;
			this.rule = rule;
			ObservableCollection<ProjectSettingVM<Protection>> observableCollection = Utils.Wrap<SettingItem<Protection>, ProjectSettingVM<Protection>>(rule, (SettingItem<Protection> setting) => new ProjectSettingVM<Protection>(parent, setting));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				parent.IsModified = true;
			};
			this.Protections = observableCollection;
			this.ParseExpression();
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00004984 File Offset: 0x00004984
		public ProjectVM Project
		{
			get
			{
				return this.parent;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000093 RID: 147 RVA: 0x0000499C File Offset: 0x0000499C
		// (set) Token: 0x06000094 RID: 148 RVA: 0x000049BC File Offset: 0x000049BC
		public string Pattern
		{
			get
			{
				return this.rule.Pattern;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.rule.Pattern != value, delegate(string val)
				{
					this.rule.Pattern = val;
				}, value, "Pattern");
				if (flag)
				{
					this.parent.IsModified = true;
					this.ParseExpression();
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00004A10 File Offset: 0x00004A10
		// (set) Token: 0x06000096 RID: 150 RVA: 0x00002441 File Offset: 0x00002441
		public PatternExpression Expression
		{
			get
			{
				return this.exp;
			}
			set
			{
				base.SetProperty<PatternExpression>(ref this.exp, value, "Expression");
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00004A28 File Offset: 0x00004A28
		// (set) Token: 0x06000098 RID: 152 RVA: 0x00002457 File Offset: 0x00002457
		public string ExpressionError
		{
			get
			{
				return this.error;
			}
			set
			{
				base.SetProperty<string>(ref this.error, value, "ExpressionError");
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000099 RID: 153 RVA: 0x00004A40 File Offset: 0x00004A40
		// (set) Token: 0x0600009A RID: 154 RVA: 0x00004A60 File Offset: 0x00004A60
		public ProtectionPreset Preset
		{
			get
			{
				return this.rule.Preset;
			}
			set
			{
				bool flag = base.SetProperty<ProtectionPreset>(this.rule.Preset != value, delegate(ProtectionPreset val)
				{
					this.rule.Preset = val;
				}, value, "Preset");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00004AA8 File Offset: 0x00004AA8
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00004AC8 File Offset: 0x00004AC8
		public bool Inherit
		{
			get
			{
				return this.rule.Inherit;
			}
			set
			{
				bool flag = base.SetProperty<bool>(this.rule.Inherit != value, delegate(bool val)
				{
					this.rule.Inherit = val;
				}, value, "Inherit");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600009D RID: 157 RVA: 0x0000246D File Offset: 0x0000246D
		// (set) Token: 0x0600009E RID: 158 RVA: 0x00002475 File Offset: 0x00002475
		public IList<ProjectSettingVM<Protection>> Protections { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00004B10 File Offset: 0x00004B10
		Rule IViewModel<Rule>.Model
		{
			get
			{
				return this.rule;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004B28 File Offset: 0x00004B28
		private void ParseExpression()
		{
			bool flag = this.Pattern == null;
			if (!flag)
			{
				PatternExpression expression;
				try
				{
					expression = new PatternParser().Parse(this.Pattern);
					this.ExpressionError = null;
				}
				catch (Exception ex)
				{
					this.ExpressionError = ex.Message;
					expression = null;
				}
				this.Expression = expression;
			}
		}

		// Token: 0x04000046 RID: 70
		private readonly ProjectVM parent;

		// Token: 0x04000047 RID: 71
		private readonly Rule rule;

		// Token: 0x04000048 RID: 72
		private string error;

		// Token: 0x04000049 RID: 73
		private PatternExpression exp;
	}
}
