using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009B RID: 155
	public class ModuleFunction : PatternFunction
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x00015E90 File Offset: 0x00014090
		public override string Name
		{
			get
			{
				return "module";
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00015EA8 File Offset: 0x000140A8
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = !(definition is IOwnerModule) && !(definition is IModule);
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				object obj = base.Arguments[0].Evaluate(definition);
				bool flag2 = definition is IModule;
				if (flag2)
				{
					result = (((IModule)definition).Name == obj.ToString());
				}
				else
				{
					result = (((IOwnerModule)definition).Module.Name == obj.ToString());
				}
			}
			return result;
		}

		// Token: 0x04000273 RID: 627
		internal const string FnName = "module";
	}
}
