using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005D RID: 93
	internal class BamlWriter
	{
		// Token: 0x06000264 RID: 612 RVA: 0x0000E310 File Offset: 0x0000C510
		public static void WriteDocument(BamlDocument doc, Stream str)
		{
			BamlBinaryWriter bamlBinaryWriter = new BamlBinaryWriter(str);
			BinaryWriter binaryWriter = new BinaryWriter(str, Encoding.Unicode);
			int num = doc.Signature.Length * 2;
			binaryWriter.Write(num);
			binaryWriter.Write(doc.Signature.ToCharArray());
			binaryWriter.Write(new byte[(num + 3 & -4) - num]);
			bamlBinaryWriter.Write(doc.ReaderVersion.Major);
			bamlBinaryWriter.Write(doc.ReaderVersion.Minor);
			bamlBinaryWriter.Write(doc.UpdaterVersion.Major);
			bamlBinaryWriter.Write(doc.UpdaterVersion.Minor);
			bamlBinaryWriter.Write(doc.WriterVersion.Major);
			bamlBinaryWriter.Write(doc.WriterVersion.Minor);
			List<int> list = new List<int>();
			for (int i = 0; i < doc.Count; i++)
			{
				BamlRecord bamlRecord = doc[i];
				bamlRecord.Position = str.Position;
				bamlBinaryWriter.Write((byte)bamlRecord.Type);
				bamlRecord.Write(bamlBinaryWriter);
				bool flag = bamlRecord is IBamlDeferRecord;
				if (flag)
				{
					list.Add(i);
				}
			}
			foreach (int index in list)
			{
				(doc[index] as IBamlDeferRecord).WriteDefer(doc, index, bamlBinaryWriter);
			}
		}
	}
}
