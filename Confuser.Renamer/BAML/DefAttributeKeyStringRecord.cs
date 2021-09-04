using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000030 RID: 48
	internal class DefAttributeKeyStringRecord : SizedBamlRecord, IBamlDeferRecord
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000156 RID: 342 RVA: 0x0000CC48 File Offset: 0x0000AE48
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DefAttributeKeyString;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000157 RID: 343 RVA: 0x0000CC5C File Offset: 0x0000AE5C
		// (set) Token: 0x06000158 RID: 344 RVA: 0x0000CC64 File Offset: 0x0000AE64
		public ushort ValueId { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000159 RID: 345 RVA: 0x0000CC6D File Offset: 0x0000AE6D
		// (set) Token: 0x0600015A RID: 346 RVA: 0x0000CC75 File Offset: 0x0000AE75
		public bool Shared { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600015B RID: 347 RVA: 0x0000CC7E File Offset: 0x0000AE7E
		// (set) Token: 0x0600015C RID: 348 RVA: 0x0000CC86 File Offset: 0x0000AE86
		public bool SharedSet { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600015D RID: 349 RVA: 0x0000CC8F File Offset: 0x0000AE8F
		// (set) Token: 0x0600015E RID: 350 RVA: 0x0000CC97 File Offset: 0x0000AE97
		public BamlRecord Record { get; set; }

		// Token: 0x0600015F RID: 351 RVA: 0x0000CCA0 File Offset: 0x0000AEA0
		public void ReadDefer(BamlDocument doc, int index, Func<long, BamlRecord> resolve)
		{
			for (;;)
			{
				BamlRecordType type = doc[index].Type;
				BamlRecordType bamlRecordType = type;
				bool flag;
				if (bamlRecordType <= BamlRecordType.KeyElementStart)
				{
					if (bamlRecordType - BamlRecordType.DefAttributeKeyString <= 1)
					{
						goto IL_36;
					}
					if (bamlRecordType != BamlRecordType.KeyElementStart)
					{
						goto IL_5C;
					}
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
					flag = true;
				}
				else if (bamlRecordType != BamlRecordType.StaticResourceStart)
				{
					if (bamlRecordType != BamlRecordType.OptimizedStaticResource)
					{
						goto IL_5C;
					}
					goto IL_36;
				}
				else
				{
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
					flag = true;
				}
				IL_65:
				index++;
				if (!flag)
				{
					break;
				}
				continue;
				IL_36:
				flag = true;
				goto IL_65;
				IL_5C:
				flag = false;
				index--;
				goto IL_65;
			}
			this.Record = resolve(doc[index].Position + (long)((ulong)this.pos));
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000CD40 File Offset: 0x0000AF40
		public void WriteDefer(BamlDocument doc, int index, BinaryWriter wtr)
		{
			for (;;)
			{
				BamlRecordType type = doc[index].Type;
				BamlRecordType bamlRecordType = type;
				bool flag;
				if (bamlRecordType <= BamlRecordType.KeyElementStart)
				{
					if (bamlRecordType - BamlRecordType.DefAttributeKeyString <= 1)
					{
						goto IL_36;
					}
					if (bamlRecordType != BamlRecordType.KeyElementStart)
					{
						goto IL_5C;
					}
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
					flag = true;
				}
				else if (bamlRecordType != BamlRecordType.StaticResourceStart)
				{
					if (bamlRecordType != BamlRecordType.OptimizedStaticResource)
					{
						goto IL_5C;
					}
					goto IL_36;
				}
				else
				{
					DefAttributeKeyStringRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
					flag = true;
				}
				IL_65:
				index++;
				if (!flag)
				{
					break;
				}
				continue;
				IL_36:
				flag = true;
				goto IL_65;
				IL_5C:
				flag = false;
				index--;
				goto IL_65;
			}
			wtr.BaseStream.Seek((long)((ulong)this.pos), SeekOrigin.Begin);
			wtr.Write((uint)(this.Record.Position - doc[index].Position));
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000CDF1 File Offset: 0x0000AFF1
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.ValueId = reader.ReadUInt16();
			this.pos = reader.ReadUInt32();
			this.Shared = reader.ReadBoolean();
			this.SharedSet = reader.ReadBoolean();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000CE28 File Offset: 0x0000B028
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.ValueId);
			this.pos = (uint)writer.BaseStream.Position;
			writer.Write(0U);
			writer.Write(this.Shared);
			writer.Write(this.SharedSet);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000CE78 File Offset: 0x0000B078
		private static void NavigateTree(BamlDocument doc, BamlRecordType start, BamlRecordType end, ref int index)
		{
			index++;
			for (;;)
			{
				bool flag = doc[index].Type == start;
				if (flag)
				{
					DefAttributeKeyStringRecord.NavigateTree(doc, start, end, ref index);
				}
				else
				{
					bool flag2 = doc[index].Type == end;
					if (flag2)
					{
						break;
					}
				}
				index++;
			}
		}

		// Token: 0x040000CB RID: 203
		internal uint pos = uint.MaxValue;
	}
}
