using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x02000069 RID: 105
	public class ProtectionParameters
	{
		// Token: 0x0600029C RID: 668 RVA: 0x000031EF File Offset: 0x000013EF
		internal ProtectionParameters(ConfuserComponent component, IList<IDnlibDef> targets)
		{
			this.comp = component;
			this.Targets = targets;
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00003208 File Offset: 0x00001408
		// (set) Token: 0x0600029E RID: 670 RVA: 0x00003210 File Offset: 0x00001410
		public IList<IDnlibDef> Targets { get; private set; }

		// Token: 0x0600029F RID: 671 RVA: 0x00011980 File Offset: 0x0000FB80
		public T GetParameter<T>(ConfuserContext context, IDnlibDef target, string name, T defValue = default(T))
		{
			bool flag = this.comp == null;
			T result;
			if (flag)
			{
				result = defValue;
			}
			else
			{
				bool flag2 = this.comp is Packer && target == null;
				if (flag2)
				{
					target = context.Modules[0];
				}
				ProtectionSettings protectionSettings = context.Annotations.Get<ProtectionSettings>(target, ProtectionParameters.ParametersKey, null);
				bool flag3 = protectionSettings == null;
				if (flag3)
				{
					result = defValue;
				}
				else
				{
					Dictionary<string, string> dictionary;
					bool flag4 = !protectionSettings.TryGetValue(this.comp, out dictionary);
					if (flag4)
					{
						result = defValue;
					}
					else
					{
						string value;
						bool flag5 = dictionary.TryGetValue(name, out value);
						if (flag5)
						{
							Type type = typeof(T);
							Type underlyingType = Nullable.GetUnderlyingType(type);
							bool flag6 = underlyingType != null;
							if (flag6)
							{
								type = underlyingType;
							}
							bool isEnum = type.IsEnum;
							if (isEnum)
							{
								result = (T)((object)Enum.Parse(type, value, true));
							}
							else
							{
								result = (T)((object)Convert.ChangeType(value, type));
							}
						}
						else
						{
							result = defValue;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00003219 File Offset: 0x00001419
		public static void SetParameters(ConfuserContext context, IDnlibDef target, ProtectionSettings parameters)
		{
			context.Annotations.Set<ProtectionSettings>(target, ProtectionParameters.ParametersKey, parameters);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00011A80 File Offset: 0x0000FC80
		public static ProtectionSettings GetParameters(ConfuserContext context, IDnlibDef target)
		{
			return context.Annotations.Get<ProtectionSettings>(target, ProtectionParameters.ParametersKey, null);
		}

		// Token: 0x040001F3 RID: 499
		private static readonly object ParametersKey = new object();

		// Token: 0x040001F4 RID: 500
		public static readonly ProtectionParameters Empty = new ProtectionParameters(null, new IDnlibDef[0]);

		// Token: 0x040001F5 RID: 501
		private readonly ConfuserComponent comp;
	}
}
