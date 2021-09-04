using System;
using System.Diagnostics;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000026 RID: 38
	internal abstract class SizedBamlRecord : BamlRecord
	{
		// Token: 0x06000108 RID: 264 RVA: 0x0000C6A0 File Offset: 0x0000A8A0
		public override void Read(BamlBinaryReader reader)
		{
			long position = reader.BaseStream.Position;
			int num = reader.ReadEncodedInt();
			this.ReadData(reader, num - (int)(reader.BaseStream.Position - position));
			Debug.Assert(reader.BaseStream.Position - position == (long)num);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000C6F0 File Offset: 0x0000A8F0
		private int SizeofEncodedInt(int val)
		{
			bool flag = (val & -128) == 0;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				bool flag2 = (val & -16384) == 0;
				if (flag2)
				{
					result = 2;
				}
				else
				{
					bool flag3 = (val & -2097152) == 0;
					if (flag3)
					{
						result = 3;
					}
					else
					{
						bool flag4 = (val & -268435456) == 0;
						if (flag4)
						{
							result = 4;
						}
						else
						{
							result = 5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000C750 File Offset: 0x0000A950
		public override void Write(BamlBinaryWriter writer)
		{
			long position = writer.BaseStream.Position;
			this.WriteData(writer);
			int num = (int)(writer.BaseStream.Position - position);
			num = this.SizeofEncodedInt(this.SizeofEncodedInt(num) + num) + num;
			writer.BaseStream.Position = position;
			writer.WriteEncodedInt(num);
			this.WriteData(writer);
		}

		// Token: 0x0600010B RID: 267
		protected abstract void ReadData(BamlBinaryReader reader, int size);

		// Token: 0x0600010C RID: 268
		protected abstract void WriteData(BamlBinaryWriter writer);
	}
}
