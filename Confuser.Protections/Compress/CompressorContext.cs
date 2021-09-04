using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using dnlib.DotNet;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000C3 RID: 195
	internal class CompressorContext
	{
		// Token: 0x06000342 RID: 834 RVA: 0x00017A18 File Offset: 0x00015C18
		public byte[] Encrypt(ICompressionService compress, byte[] data, uint seed, Action<double> progressFunc)
		{
			data = (byte[])data.Clone();
			uint[] array = new uint[16];
			uint[] array2 = new uint[16];
			ulong num = (ulong)seed;
			for (int i = 0; i < 16; i++)
			{
				num = num * num % 339722377UL;
				array2[i] = (uint)num;
				array[i] = (uint)(num * num % 1145919227UL);
			}
			uint[] array3 = this.Deriver.DeriveKey(array, array2);
			uint num2 = (uint)(num % 9067703UL);
			for (int j = 0; j < data.Length; j++)
			{
				byte[] array4 = data;
				int num3 = j;
				array4[num3] ^= (byte)num;
				bool flag = (j & 255) == 0;
				if (flag)
				{
					num = num * num % 9067703UL;
				}
			}
			data = compress.Compress(data, progressFunc);
			Array.Resize<byte>(ref data, data.Length + 3 & -4);
			byte[] array5 = new byte[data.Length];
			int num4 = 0;
			for (int k = 0; k < data.Length; k += 4)
			{
				uint num5 = (uint)((int)data[k] | (int)data[k + 1] << 8 | (int)data[k + 2] << 16 | (int)data[k + 3] << 24);
				uint num6 = num5 ^ array3[num4 & 15];
				array3[num4 & 15] = (array3[num4 & 15] ^ num5) + 1037772825U;
				array5[k] = (byte)num6;
				array5[k + 1] = (byte)(num6 >> 8);
				array5[k + 2] = (byte)(num6 >> 16);
				array5[k + 3] = (byte)(num6 >> 24);
				num4++;
			}
			return array5;
		}

		// Token: 0x04000187 RID: 391
		public AssemblyDef Assembly;

		// Token: 0x04000188 RID: 392
		public IKeyDeriver Deriver;

		// Token: 0x04000189 RID: 393
		public byte[] EncryptedModule;

		// Token: 0x0400018A RID: 394
		public MethodDef EntryPoint;

		// Token: 0x0400018B RID: 395
		public uint EntryPointToken;

		// Token: 0x0400018C RID: 396
		public byte[] KeySig;

		// Token: 0x0400018D RID: 397
		public uint KeyToken;

		// Token: 0x0400018E RID: 398
		public ModuleKind Kind;

		// Token: 0x0400018F RID: 399
		[TupleElementNames(new string[]
		{
			"Offset",
			"Flags",
			"Value"
		})]
		public List<ValueTuple<uint, uint, UTF8String>> ManifestResources;

		// Token: 0x04000190 RID: 400
		public int ModuleIndex;

		// Token: 0x04000191 RID: 401
		public string ModuleName;

		// Token: 0x04000192 RID: 402
		public byte[] OriginModule;

		// Token: 0x04000193 RID: 403
		public ModuleDef OriginModuleDef;

		// Token: 0x04000194 RID: 404
		public bool CompatMode;
	}
}
