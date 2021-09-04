using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003A RID: 58
	internal class DocumentStartRecord : BamlRecord
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000D324 File Offset: 0x0000B524
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DocumentStart;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000D337 File Offset: 0x0000B537
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x0000D33F File Offset: 0x0000B53F
		public bool LoadAsync { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x0000D348 File Offset: 0x0000B548
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x0000D350 File Offset: 0x0000B550
		public uint MaxAsyncRecords { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000D359 File Offset: 0x0000B559
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x0000D361 File Offset: 0x0000B561
		public bool DebugBaml { get; set; }

		// Token: 0x060001B6 RID: 438 RVA: 0x0000D36A File Offset: 0x0000B56A
		public override void Read(BamlBinaryReader reader)
		{
			this.LoadAsync = reader.ReadBoolean();
			this.MaxAsyncRecords = reader.ReadUInt32();
			this.DebugBaml = reader.ReadBoolean();
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000D394 File Offset: 0x0000B594
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.LoadAsync);
			writer.Write(this.MaxAsyncRecords);
			writer.Write(this.DebugBaml);
		}
	}
}
