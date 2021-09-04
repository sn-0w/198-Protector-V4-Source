using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000F9 RID: 249
	internal class JITMethodBody : IChunk
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x0000356B File Offset: 0x0000176B
		// (set) Token: 0x06000402 RID: 1026 RVA: 0x00003573 File Offset: 0x00001773
		public JITExceptionHandlerClause[] ExceptionHandlers { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x0000357C File Offset: 0x0000177C
		// (set) Token: 0x06000404 RID: 1028 RVA: 0x00003584 File Offset: 0x00001784
		public byte[] ILCode { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000405 RID: 1029 RVA: 0x0000358D File Offset: 0x0000178D
		// (set) Token: 0x06000406 RID: 1030 RVA: 0x00003595 File Offset: 0x00001795
		public byte[] LocalVars { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x0000359E File Offset: 0x0000179E
		// (set) Token: 0x06000408 RID: 1032 RVA: 0x000035A6 File Offset: 0x000017A6
		public uint MaxStack { get; set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x000035AF File Offset: 0x000017AF
		// (set) Token: 0x0600040A RID: 1034 RVA: 0x000035B7 File Offset: 0x000017B7
		public uint MulSeed { get; set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x000035C0 File Offset: 0x000017C0
		// (set) Token: 0x0600040C RID: 1036 RVA: 0x000035C8 File Offset: 0x000017C8
		public uint Offset { get; set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x000035D1 File Offset: 0x000017D1
		// (set) Token: 0x0600040E RID: 1038 RVA: 0x000035D9 File Offset: 0x000017D9
		public uint Options { get; set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x000035E2 File Offset: 0x000017E2
		// (set) Token: 0x06000410 RID: 1040 RVA: 0x000035EA File Offset: 0x000017EA
		public FileOffset FileOffset { get; private set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x000035F3 File Offset: 0x000017F3
		// (set) Token: 0x06000412 RID: 1042 RVA: 0x000035FB File Offset: 0x000017FB
		public RVA RVA { get; private set; }

		// Token: 0x06000413 RID: 1043 RVA: 0x00003604 File Offset: 0x00001804
		public void SetOffset(FileOffset offset, RVA rva)
		{
			this.FileOffset = offset;
			this.RVA = rva;
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00003617 File Offset: 0x00001817
		public uint GetFileLength()
		{
			return (uint)(this._body.Length + 4);
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00003623 File Offset: 0x00001823
		public uint GetVirtualSize()
		{
			return this.GetFileLength();
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x0000362B File Offset: 0x0000182B
		public void WriteTo(DataWriter writer)
		{
			writer.WriteUInt32((uint)(this._body.Length >> 2));
			writer.WriteBytes(this._body.ToArray<byte>());
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x0001A8BC File Offset: 0x00018ABC
		public void Serialize(uint token, uint key, byte[] fieldLayout)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DataWriter dataWriter = new DataWriter(memoryStream);
				for (int i = 0; i < fieldLayout.Length; i++)
				{
					switch (fieldLayout[i])
					{
					case 0:
						dataWriter.WriteUInt32((uint)this.ILCode.Length);
						break;
					case 1:
						dataWriter.WriteUInt32(this.MaxStack);
						break;
					case 2:
						dataWriter.WriteUInt32((uint)this.ExceptionHandlers.Length);
						break;
					case 3:
						dataWriter.WriteUInt32((uint)this.LocalVars.Length);
						break;
					case 4:
						dataWriter.WriteUInt32(this.Options);
						break;
					case 5:
						dataWriter.WriteUInt32(this.MulSeed);
						break;
					default:
						throw new NotImplementedException("Invalid field layout index.");
					}
				}
				dataWriter.WriteBytes(this.ILCode.ToArray<byte>());
				dataWriter.WriteBytes(this.LocalVars.ToArray<byte>());
				foreach (JITExceptionHandlerClause jitexceptionHandlerClause in this.ExceptionHandlers)
				{
					dataWriter.WriteUInt32(jitexceptionHandlerClause.Flags);
					dataWriter.WriteUInt32(jitexceptionHandlerClause.TryOffset);
					dataWriter.WriteUInt32(jitexceptionHandlerClause.TryLength);
					dataWriter.WriteUInt32(jitexceptionHandlerClause.HandlerOffset);
					dataWriter.WriteUInt32(jitexceptionHandlerClause.HandlerLength);
					dataWriter.WriteUInt32(jitexceptionHandlerClause.ClassTokenOrFilterOffset);
				}
				dataWriter.WriteZeroes(4 - ((int)memoryStream.Length & 3));
				array = memoryStream.ToArray();
			}
			Debug.Assert(array.Length % 4 == 0);
			uint num = token * key;
			uint num2 = num;
			uint num3 = 0U;
			while ((ulong)num3 < (ulong)((long)array.Length))
			{
				uint num4 = (uint)((int)array[(int)num3] | (int)array[(int)(num3 + 1U)] << 8 | (int)array[(int)(num3 + 2U)] << 16 | (int)array[(int)(num3 + 3U)] << 24);
				byte[] array2 = array;
				uint num5 = num3;
				array2[(int)num5] = (array2[(int)num5] ^ (byte)num);
				byte[] array3 = array;
				uint num6 = num3 + 1U;
				array3[(int)num6] = (array3[(int)num6] ^ (byte)(num >> 8));
				byte[] array4 = array;
				uint num7 = num3 + 2U;
				array4[(int)num7] = (array4[(int)num7] ^ (byte)(num >> 16));
				byte[] array5 = array;
				uint num8 = num3 + 3U;
				array5[(int)num8] = (array5[(int)num8] ^ (byte)(num >> 24));
				num += (num4 ^ num2);
				num2 ^= (num >> 5 | num << 27);
				num3 += 4U;
			}
			this._body = array;
		}

		// Token: 0x040001FA RID: 506
		private byte[] _body;
	}
}
