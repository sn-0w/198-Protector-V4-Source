using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Core
{
	// Token: 0x02000050 RID: 80
	internal class NativeEraser
	{
		// Token: 0x060001FC RID: 508 RVA: 0x00002D18 File Offset: 0x00000F18
		private static void Erase(Tuple<uint, uint, byte[]> section, uint offset, uint len)
		{
			Array.Clear(section.Item3, (int)(offset - section.Item1), (int)len);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000E9EC File Offset: 0x0000CBEC
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, uint beginOffset, uint size)
		{
			foreach (Tuple<uint, uint, byte[]> tuple in sections)
			{
				bool flag = beginOffset >= tuple.Item1 && beginOffset + size < tuple.Item2;
				if (flag)
				{
					NativeEraser.Erase(tuple, beginOffset, size);
					break;
				}
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000EA60 File Offset: 0x0000CC60
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, IFileSection s)
		{
			foreach (Tuple<uint, uint, byte[]> tuple in sections)
			{
				bool flag = s.StartOffset >= (FileOffset)tuple.Item1 && s.EndOffset < (FileOffset)tuple.Item2;
				if (flag)
				{
					NativeEraser.Erase(tuple, (uint)s.StartOffset, s.EndOffset - s.StartOffset);
					break;
				}
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000EAEC File Offset: 0x0000CCEC
		private static void Erase(List<Tuple<uint, uint, byte[]>> sections, uint methodOffset)
		{
			foreach (Tuple<uint, uint, byte[]> tuple in sections)
			{
				bool flag = methodOffset >= tuple.Item1 && (ulong)(methodOffset - tuple.Item1) < (ulong)((long)tuple.Item3.Length);
				if (flag)
				{
					uint num = (uint)tuple.Item3[(int)(methodOffset - tuple.Item1)];
					uint num2;
					switch (num & 7U)
					{
					case 2U:
					case 6U:
						num2 = (num >> 2) + 1U;
						break;
					case 3U:
					{
						num |= (uint)((uint)tuple.Item3[(int)(methodOffset - tuple.Item1 + 1U)] << 8);
						num2 = (num >> 12) * 4U;
						uint num3 = BitConverter.ToUInt32(tuple.Item3, (int)(methodOffset - tuple.Item1 + 4U));
						num2 += num3;
						break;
					}
					case 4U:
					case 5U:
						goto IL_BD;
					default:
						goto IL_BD;
					}
					NativeEraser.Erase(tuple, methodOffset, num2);
					continue;
					IL_BD:
					break;
				}
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000EBF0 File Offset: 0x0000CDF0
		public static void Erase(NativeModuleWriter writer, ModuleDefMD module)
		{
			bool flag = writer == null || module == null;
			if (!flag)
			{
				List<Tuple<uint, uint, byte[]>> list = new List<Tuple<uint, uint, byte[]>>();
				MemoryStream memoryStream = new MemoryStream();
				foreach (NativeModuleWriter.OrigSection origSection in writer.OrigSections)
				{
					DataReaderChunk chunk = origSection.Chunk;
					ImageSectionHeader pesection = origSection.PESection;
					memoryStream.SetLength(0L);
					chunk.WriteTo(new DataWriter(memoryStream));
					byte[] item = memoryStream.ToArray();
					DataReaderChunk dataReaderChunk = new DataReaderChunk(chunk.CreateReader());
					dataReaderChunk.SetOffset(chunk.FileOffset, chunk.RVA);
					origSection.Chunk = dataReaderChunk;
					list.Add(Tuple.Create<uint, uint, byte[]>(pesection.PointerToRawData, pesection.PointerToRawData + pesection.SizeOfRawData, item));
				}
				dnlib.DotNet.MD.Metadata metadata = module.Metadata;
				uint rows = metadata.TablesStream.MethodTable.Rows;
				for (uint num = 1U; num <= rows; num += 1U)
				{
					RawMethodRow rawMethodRow;
					bool flag2 = metadata.TablesStream.TryReadMethodRow(num, out rawMethodRow);
					if (flag2)
					{
						MethodImplAttributes methodImplAttributes = (MethodImplAttributes)(rawMethodRow.ImplFlags & 3);
						bool flag3 = methodImplAttributes == MethodImplAttributes.IL;
						if (flag3)
						{
							NativeEraser.Erase(list, (uint)metadata.PEImage.ToFileOffset((RVA)rawMethodRow.RVA));
						}
					}
				}
				ImageDataDirectory resources = metadata.ImageCor20Header.Resources;
				bool flag4 = resources.Size > 0U;
				if (flag4)
				{
					NativeEraser.Erase(list, (uint)resources.StartOffset, resources.Size);
				}
				NativeEraser.Erase(list, metadata.ImageCor20Header);
				NativeEraser.Erase(list, metadata.MetadataHeader);
				foreach (DotNetStream s in metadata.AllStreams)
				{
					NativeEraser.Erase(list, s);
				}
			}
		}
	}
}
