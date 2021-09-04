using System;
using System.IO;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher
{
	// Token: 0x02000006 RID: 6
	public static class CodeGenUtils
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002928 File Offset: 0x00000B28
		public static byte[] AssembleCode(x86CodeGen codeGen, x86Register reg)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(new byte[]
				{
					137,
					224
				});
				binaryWriter.Write(new byte[]
				{
					83
				});
				binaryWriter.Write(new byte[]
				{
					87
				});
				binaryWriter.Write(new byte[]
				{
					86
				});
				binaryWriter.Write(new byte[]
				{
					41,
					224
				});
				binaryWriter.Write(new byte[]
				{
					131,
					248,
					24
				});
				binaryWriter.Write(new byte[]
				{
					116,
					7
				});
				binaryWriter.Write(new byte[]
				{
					139,
					68,
					36,
					16
				});
				binaryWriter.Write(new byte[]
				{
					80
				});
				binaryWriter.Write(new byte[]
				{
					235,
					1
				});
				binaryWriter.Write(new byte[]
				{
					81
				});
				foreach (x86Instruction x86Instruction in codeGen.Instructions)
				{
					binaryWriter.Write(x86Instruction.Assemble());
				}
				bool flag = reg > x86Register.EAX;
				if (flag)
				{
					binaryWriter.Write(x86Instruction.Create(x86OpCode.MOV, new Ix86Operand[]
					{
						new x86RegisterOperand(x86Register.EAX),
						new x86RegisterOperand(reg)
					}).Assemble());
				}
				binaryWriter.Write(new byte[]
				{
					94
				});
				binaryWriter.Write(new byte[]
				{
					95
				});
				binaryWriter.Write(new byte[]
				{
					91
				});
				binaryWriter.Write(new byte[]
				{
					195
				});
			}
			return memoryStream.ToArray();
		}
	}
}
