using System;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x0200000A RID: 10
	public interface INameService
	{
		// Token: 0x06000029 RID: 41
		VTableStorage GetVTables();

		// Token: 0x0600002A RID: 42
		void Analyze(IDnlibDef def);

		// Token: 0x0600002B RID: 43
		bool CanRename(object obj);

		// Token: 0x0600002C RID: 44
		void SetCanRename(object obj, bool val);

		// Token: 0x0600002D RID: 45
		void SetParam(IDnlibDef def, string name, string value);

		// Token: 0x0600002E RID: 46
		string GetParam(IDnlibDef def, string name);

		// Token: 0x0600002F RID: 47
		RenameMode GetRenameMode(object obj);

		// Token: 0x06000030 RID: 48
		void SetRenameMode(object obj, RenameMode val);

		// Token: 0x06000031 RID: 49
		void ReduceRenameMode(object obj, RenameMode val);

		// Token: 0x06000032 RID: 50
		string ObfuscateName(string name, RenameMode mode);

		// Token: 0x06000033 RID: 51
		string RandomName();

		// Token: 0x06000034 RID: 52
		string RandomName(RenameMode mode);

		// Token: 0x06000035 RID: 53
		void RegisterRenamer(IRenamer renamer);

		// Token: 0x06000036 RID: 54
		T FindRenamer<T>();

		// Token: 0x06000037 RID: 55
		void AddReference<T>(T obj, INameReference<T> reference);

		// Token: 0x06000038 RID: 56
		void SetOriginalName(object obj, string name);

		// Token: 0x06000039 RID: 57
		void SetOriginalNamespace(object obj, string ns);

		// Token: 0x0600003A RID: 58
		void MarkHelper(IDnlibDef def, IMarkerService marker, ConfuserComponent parentComp);
	}
}
