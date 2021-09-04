using System;
using System.Diagnostics;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005E RID: 94
	public class BAMLStringReference : IBAMLReference
	{
		// Token: 0x06000266 RID: 614 RVA: 0x0000E49C File Offset: 0x0000C69C
		public BAMLStringReference(Instruction instr)
		{
			this.instr = instr;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000E4AC File Offset: 0x0000C6AC
		public bool CanRename(string oldName, string newName)
		{
			return this.instr.OpCode.Code == Code.Ldstr;
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000E4C4 File Offset: 0x0000C6C4
		public void Rename(string oldName, string newName)
		{
			string text = (string)this.instr.Operand;
			for (;;)
			{
				bool flag = text.EndsWith(oldName, StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					break;
				}
				bool flag2 = oldName.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);
				if (!flag2)
				{
					return;
				}
				oldName = BAMLStringReference.ToXaml(oldName);
				newName = BAMLStringReference.ToXaml(newName);
			}
			text = text.Substring(0, text.Length - oldName.Length) + newName;
			this.instr.Operand = text;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000E548 File Offset: 0x0000C748
		private static string ToXaml(string refName)
		{
			Debug.Assert(refName.EndsWith(".baml", StringComparison.OrdinalIgnoreCase));
			return refName.Substring(0, refName.Length - 5) + ".xaml";
		}

		// Token: 0x04000100 RID: 256
		private readonly Instruction instr;
	}
}
