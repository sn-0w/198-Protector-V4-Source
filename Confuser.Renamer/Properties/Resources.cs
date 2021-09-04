using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Confuser.Renamer.Properties
{
	// Token: 0x0200001F RID: 31
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x060000BE RID: 190 RVA: 0x0000A2FF File Offset: 0x000084FF
		internal Resources()
		{
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000BF RID: 191 RVA: 0x0000A30C File Offset: 0x0000850C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				bool flag = Resources.resourceMan == null;
				if (flag)
				{
					ResourceManager resourceManager = new ResourceManager("Confuser.Renamer.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x0000A354 File Offset: 0x00008554
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x0000A36B File Offset: 0x0000856B
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

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x0000A374 File Offset: 0x00008574
		internal static string InvalidPathSyntax
		{
			get
			{
				return Resources.ResourceManager.GetString("InvalidPathSyntax", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x0000A39C File Offset: 0x0000859C
		internal static string UnmatchedBracket
		{
			get
			{
				return Resources.ResourceManager.GetString("UnmatchedBracket", Resources.resourceCulture);
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x0000A3C4 File Offset: 0x000085C4
		internal static string UnmatchedParen
		{
			get
			{
				return Resources.ResourceManager.GetString("UnmatchedParen", Resources.resourceCulture);
			}
		}

		// Token: 0x0400005C RID: 92
		private static ResourceManager resourceMan;

		// Token: 0x0400005D RID: 93
		private static CultureInfo resourceCulture;
	}
}
