using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Confuser.Protections.Properties
{
	// Token: 0x02000132 RID: 306
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000544 RID: 1348 RVA: 0x00002DF9 File Offset: 0x00000FF9
		internal Resources()
		{
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000545 RID: 1349 RVA: 0x00003C68 File Offset: 0x00001E68
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("Confuser.Protections.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x00003C97 File Offset: 0x00001E97
		// (set) Token: 0x06000547 RID: 1351 RVA: 0x00003C9E File Offset: 0x00001E9E
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x00003CA6 File Offset: 0x00001EA6
		internal static string raper
		{
			get
			{
				return Resources.ResourceManager.GetString("raper", Resources.resourceCulture);
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000549 RID: 1353 RVA: 0x00003CBC File Offset: 0x00001EBC
		internal static string StubCode
		{
			get
			{
				return Resources.ResourceManager.GetString("StubCode", Resources.resourceCulture);
			}
		}

		// Token: 0x04000281 RID: 641
		private static ResourceManager resourceMan;

		// Token: 0x04000282 RID: 642
		private static CultureInfo resourceCulture;
	}
}
