using System;
using System.IO;
using System.Reflection;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x02000079 RID: 121
	internal class RuntimeService : IRuntimeService
	{
		// Token: 0x060002EF RID: 751 RVA: 0x000132B0 File Offset: 0x000114B0
		public TypeDef GetRuntimeType(string fullName)
		{
			bool flag = this.rtModule == null;
			if (flag)
			{
				this.LoadConfuserRuntimeModule();
			}
			return this.rtModule.Find(fullName, true);
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x000132E8 File Offset: 0x000114E8
		private void LoadConfuserRuntimeModule()
		{
			Module manifestModule = typeof(RuntimeService).Assembly.ManifestModule;
			string text = "Confuser.Runtime.dll";
			ModuleCreationOptions options = new ModuleCreationOptions
			{
				TryToLoadPdbFromDisk = true
			};
			bool flag = manifestModule.FullyQualifiedName[0] != '<';
			if (flag)
			{
				text = Path.Combine(Path.GetDirectoryName(manifestModule.FullyQualifiedName), text);
				bool flag2 = File.Exists(text);
				if (flag2)
				{
					try
					{
						this.rtModule = ModuleDefMD.Load(text, options);
					}
					catch (IOException)
					{
					}
				}
				bool flag3 = this.rtModule == null;
				if (flag3)
				{
					text = "Confuser.Runtime.dll";
				}
			}
			bool flag4 = this.rtModule == null;
			if (flag4)
			{
				this.rtModule = ModuleDefMD.Load(text, options);
			}
			this.rtModule.EnableTypeDefFindCache = true;
		}

		// Token: 0x04000230 RID: 560
		private ModuleDef rtModule;
	}
}
