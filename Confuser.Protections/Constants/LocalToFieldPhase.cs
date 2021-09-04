using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000A0 RID: 160
	internal class LocalToFieldPhase : ProtectionPhase
	{
		// Token: 0x060002C1 RID: 705 RVA: 0x00003083 File Offset: 0x00001283
		public LocalToFieldPhase(ConstantProtection parent) : base(parent)
		{
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x00003097 File Offset: 0x00001297
		public override string Name
		{
			get
			{
				return "Local To Field";
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00012088 File Offset: 0x00010288
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			using (IEnumerator<MethodDef> enumerator = parameters.Targets.OfType<MethodDef>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MethodDef method = enumerator.Current;
					if (method.HasBody)
					{
						method.Body.SimplifyMacros(method.Parameters);
						IList<Instruction> instructions = method.Body.Instructions;
						for (int i = 0; i < instructions.Count; i++)
						{
							Local local;
							if ((local = (instructions[i].Operand as Local)) != null)
							{
								FieldDef fieldDef;
								if (!this.convertedLocals.ContainsKey(local))
								{
									fieldDef = new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(local.Type), FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static);
									context.CurrentModule.GlobalType.Fields.Add(fieldDef);
									this.convertedLocals.Add(local, fieldDef);
								}
								else
								{
									fieldDef = this.convertedLocals[local];
								}
								OpCode opCode = null;
								switch (instructions[i].OpCode.Code)
								{
								case Code.Ldloc:
									opCode = OpCodes.Ldsfld;
									break;
								case Code.Ldloca:
									opCode = OpCodes.Ldsflda;
									break;
								case Code.Stloc:
									opCode = OpCodes.Stsfld;
									break;
								}
								instructions[i].OpCode = opCode;
								instructions[i].Operand = fieldDef;
							}
						}
						this.convertedLocals.ToList<KeyValuePair<Local, FieldDef>>().ForEach(delegate(KeyValuePair<Local, FieldDef> x)
						{
							method.Body.Variables.Remove(x.Key);
						});
						this.convertedLocals = new Dictionary<Local, FieldDef>();
					}
				}
			}
		}

		// Token: 0x0400013C RID: 316
		private Dictionary<Local, FieldDef> convertedLocals = new Dictionary<Local, FieldDef>();
	}
}
