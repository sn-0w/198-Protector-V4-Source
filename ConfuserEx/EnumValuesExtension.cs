using System;
using System.Windows.Markup;

namespace ConfuserEx
{
	// Token: 0x0200000D RID: 13
	public class EnumValuesExtension : MarkupExtension
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00002226 File Offset: 0x00002226
		public EnumValuesExtension(Type enumType)
		{
			this.enumType = enumType;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000033E0 File Offset: 0x000033E0
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Enum.GetValues(this.enumType);
		}

		// Token: 0x04000015 RID: 21
		private readonly Type enumType;
	}
}
