using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Pdb;

namespace Confuser.Renamer
{
	// Token: 0x0200000D RID: 13
	internal class RenamePhase : ProtectionPhase
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00002050 File Offset: 0x00000250
		public RenamePhase(NameProtection parent) : base(parent)
		{
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600005D RID: 93 RVA: 0x0000808D File Offset: 0x0000628D
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.AllDefinitions;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00008091 File Offset: 0x00006291
		public override string Name
		{
			get
			{
				return "Renaming";
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00008098 File Offset: 0x00006298
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			NameService nameService = (NameService)context.Registry.GetService<INameService>();
			context.Logger.Debug("Renaming...");
			foreach (IRenamer renamer in nameService.Renamers)
			{
				foreach (IDnlibDef def in parameters.Targets)
				{
					renamer.PreRename(context, nameService, parameters, def);
				}
				context.CheckCancellation();
			}
			List<IDnlibDef> list = parameters.Targets.ToList<IDnlibDef>();
			nameService.GetRandom().Shuffle<IDnlibDef>(list);
			HashSet<string> hashSet = new HashSet<string>();
			foreach (IDnlibDef dnlibDef in list.WithProgress(context.Logger))
			{
				bool flag = nameService.CanRename(dnlibDef);
				RenameMode renameMode = nameService.GetRenameMode(dnlibDef);
				bool flag2 = dnlibDef is MethodDef;
				if (flag2)
				{
					MethodDef methodDef = (MethodDef)dnlibDef;
					bool flag3 = (flag || methodDef.IsConstructor) && parameters.GetParameter<bool>(context, dnlibDef, "renameArgs", true);
					if (flag3)
					{
						foreach (ParamDef paramDef in ((MethodDef)dnlibDef).ParamDefs)
						{
							paramDef.Name = null;
						}
					}
					bool flag4 = parameters.GetParameter<bool>(context, dnlibDef, "renPdb", false) && methodDef.HasBody;
					if (flag4)
					{
						foreach (Instruction instruction in methodDef.Body.Instructions)
						{
							bool flag5 = instruction.SequencePoint != null && !hashSet.Contains(instruction.SequencePoint.Document.Url);
							if (flag5)
							{
								instruction.SequencePoint.Document.Url = nameService.ObfuscateName(instruction.SequencePoint.Document.Url, renameMode);
								hashSet.Add(instruction.SequencePoint.Document.Url);
							}
						}
						foreach (Local local in methodDef.Body.Variables)
						{
							bool flag6 = !string.IsNullOrEmpty(local.Name);
							if (flag6)
							{
								local.Name = nameService.ObfuscateName(local.Name, renameMode);
							}
						}
						methodDef.Body.PdbMethod.Scope = new PdbScope();
					}
				}
				bool flag7 = !flag;
				if (!flag7)
				{
					IList<INameReference> references = nameService.GetReferences(dnlibDef);
					bool flag8 = false;
					foreach (INameReference nameReference in references)
					{
						flag8 |= nameReference.ShouldCancelRename();
						bool flag9 = flag8;
						if (flag9)
						{
							break;
						}
					}
					bool flag10 = flag8;
					if (!flag10)
					{
						bool flag11 = dnlibDef is TypeDef;
						if (flag11)
						{
							TypeDef typeDef = (TypeDef)dnlibDef;
							bool parameter = parameters.GetParameter<bool>(context, dnlibDef, "flatten", true);
							if (parameter)
							{
								typeDef.Name = nameService.ObfuscateName(typeDef.FullName, renameMode);
								typeDef.Namespace = "";
							}
							else
							{
								typeDef.Namespace = nameService.ObfuscateName(typeDef.Namespace, renameMode);
								typeDef.Name = nameService.ObfuscateName(typeDef.Name, renameMode);
							}
							foreach (GenericParam genericParam in typeDef.GenericParameters)
							{
								genericParam.Name = ((char)(genericParam.Number + 1)).ToString();
							}
						}
						else
						{
							bool flag12 = dnlibDef is MethodDef;
							if (flag12)
							{
								foreach (GenericParam genericParam2 in ((MethodDef)dnlibDef).GenericParameters)
								{
									genericParam2.Name = ((char)(genericParam2.Number + 1)).ToString();
								}
								dnlibDef.Name = nameService.ObfuscateName(dnlibDef.Name, renameMode);
							}
							else
							{
								dnlibDef.Name = nameService.ObfuscateName(dnlibDef.Name, renameMode);
							}
						}
						foreach (INameReference nameReference2 in references.ToList<INameReference>())
						{
							bool flag13 = !nameReference2.UpdateNameReference(context, nameService);
							if (flag13)
							{
								context.Logger.ErrorFormat("Failed to update name reference on '{0}'.", new object[]
								{
									dnlibDef
								});
								throw new ConfuserException(null);
							}
						}
						context.CheckCancellation();
					}
				}
			}
		}
	}
}
