using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000070 RID: 112
	internal class InterReferenceAnalyzer : IRenamer
	{
		// Token: 0x060002B9 RID: 697 RVA: 0x00025C68 File Offset: 0x00023E68
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD moduleDefMD = def as ModuleDefMD;
			bool flag = moduleDefMD == null;
			if (!flag)
			{
				IEnumerable<MethodDef> enumerable = moduleDefMD.GetTypes().SelectMany((TypeDef type) => type.Methods);
				foreach (MethodDef methodDef in enumerable)
				{
					foreach (MethodOverride methodOverride in methodDef.Overrides)
					{
						this.ProcessMemberRef(context, service, moduleDefMD, methodOverride.MethodBody);
						this.ProcessMemberRef(context, service, moduleDefMD, methodOverride.MethodDeclaration);
					}
					bool flag2 = !methodDef.HasBody;
					if (!flag2)
					{
						foreach (Instruction instruction in methodDef.Body.Instructions)
						{
							bool flag3 = instruction.Operand is MemberRef || instruction.Operand is MethodSpec;
							if (flag3)
							{
								this.ProcessMemberRef(context, service, moduleDefMD, (IMemberRef)instruction.Operand);
							}
						}
					}
				}
				MDTable mdtable = moduleDefMD.TablesStream.Get(Table.TypeRef);
				uint rows = mdtable.Rows;
				for (uint num = 1U; num <= rows; num += 1U)
				{
					TypeRef typeRef = moduleDefMD.ResolveTypeRef(num);
					TypeDef typeDef = typeRef.ResolveTypeDefThrow();
					bool flag4 = typeDef.Module != moduleDefMD && context.Modules.Contains((ModuleDefMD)typeDef.Module);
					if (flag4)
					{
						service.AddReference<TypeDef>(typeDef, new TypeRefReference(typeRef, typeDef));
					}
				}
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00025E74 File Offset: 0x00024074
		private void ProcessMemberRef(ConfuserContext context, INameService service, ModuleDefMD module, IMemberRef r)
		{
			MemberRef memberRef = r as MemberRef;
			bool flag = r is MethodSpec;
			if (flag)
			{
				memberRef = (((MethodSpec)r).Method as MemberRef);
			}
			bool flag2 = memberRef != null;
			if (flag2)
			{
				bool flag3 = memberRef.DeclaringType.TryGetArraySig() != null;
				if (!flag3)
				{
					TypeDef typeDef = memberRef.DeclaringType.ResolveTypeDefThrow();
					bool flag4 = typeDef.Module != module && context.Modules.Contains((ModuleDefMD)typeDef.Module);
					if (flag4)
					{
						IDnlibDef dnlibDef = (IDnlibDef)typeDef.ResolveThrow(memberRef);
						service.AddReference<IDnlibDef>(dnlibDef, new MemberRefReference(memberRef, dnlibDef));
					}
				}
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}
	}
}
