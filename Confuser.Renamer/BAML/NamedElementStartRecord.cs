using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000059 RID: 89
	internal class NamedElementStartRecord : ElementStartRecord
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000DCF0 File Offset: 0x0000BEF0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.NamedElementStart;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000DD04 File Offset: 0x0000BF04
		// (set) Token: 0x0600025A RID: 602 RVA: 0x0000DD0C File Offset: 0x0000BF0C
		public string RuntimeName { get; set; }

		// Token: 0x0600025B RID: 603 RVA: 0x0000DD15 File Offset: 0x0000BF15
		public override void Read(BamlBinaryReader reader)
		{
			base.TypeId = reader.ReadUInt16();
			this.RuntimeName = reader.ReadString();
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000DD34 File Offset: 0x0000BF34
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(base.TypeId);
			bool flag = this.RuntimeName != null;
			if (flag)
			{
				writer.Write(this.RuntimeName);
			}
		}
	}
}
