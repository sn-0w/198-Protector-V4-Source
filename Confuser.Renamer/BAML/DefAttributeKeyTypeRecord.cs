using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000046 RID: 70
	internal class DefAttributeKeyTypeRecord : ElementStartRecord, IBamlDeferRecord
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000D6D4 File Offset: 0x0000B8D4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DefAttributeKeyType;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000D6E8 File Offset: 0x0000B8E8
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x0000D6F0 File Offset: 0x0000B8F0
		public bool Shared { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000D6F9 File Offset: 0x0000B8F9
		// (set) Token: 0x060001FB RID: 507 RVA: 0x0000D701 File Offset: 0x0000B901
		public bool SharedSet { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001FC RID: 508 RVA: 0x0000D70A File Offset: 0x0000B90A
		// (set) Token: 0x060001FD RID: 509 RVA: 0x0000D712 File Offset: 0x0000B912
		public BamlRecord Record { get; set; }

		// Token: 0x060001FE RID: 510 RVA: 0x0000D71C File Offset: 0x0000B91C
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
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
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
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
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

		// Token: 0x060001FF RID: 511 RVA: 0x0000D7BC File Offset: 0x0000B9BC
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
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.KeyElementStart, BamlRecordType.KeyElementEnd, ref index);
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
					DefAttributeKeyTypeRecord.NavigateTree(doc, BamlRecordType.StaticResourceStart, BamlRecordType.StaticResourceEnd, ref index);
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

		// Token: 0x06000200 RID: 512 RVA: 0x0000D86D File Offset: 0x0000BA6D
		public override void Read(BamlBinaryReader reader)
		{
			base.Read(reader);
			this.pos = reader.ReadUInt32();
			this.Shared = reader.ReadBoolean();
			this.SharedSet = reader.ReadBoolean();
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000D89E File Offset: 0x0000BA9E
		public override void Write(BamlBinaryWriter writer)
		{
			base.Write(writer);
			this.pos = (uint)writer.BaseStream.Position;
			writer.Write(0U);
			writer.Write(this.Shared);
			writer.Write(this.SharedSet);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000D8E0 File Offset: 0x0000BAE0
		private static void NavigateTree(BamlDocument doc, BamlRecordType start, BamlRecordType end, ref int index)
		{
			index++;
			for (;;)
			{
				bool flag = doc[index].Type == start;
				if (flag)
				{
					DefAttributeKeyTypeRecord.NavigateTree(doc, start, end, ref index);
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

		// Token: 0x040000F0 RID: 240
		internal uint pos = uint.MaxValue;
	}
}
