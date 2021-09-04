using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005C RID: 92
	internal class BamlReader
	{
		// Token: 0x06000262 RID: 610 RVA: 0x0000DDA8 File Offset: 0x0000BFA8
		public static BamlDocument ReadDocument(Stream str)
		{
			BamlDocument bamlDocument = new BamlDocument();
			BamlBinaryReader bamlBinaryReader = new BamlBinaryReader(str);
			BinaryReader binaryReader = new BinaryReader(str, Encoding.Unicode);
			uint num = binaryReader.ReadUInt32();
			bamlDocument.Signature = new string(binaryReader.ReadChars((int)(num >> 1)));
			binaryReader.ReadBytes((int)(((ulong)(num + 3U) & 18446744073709551612UL) - (ulong)num));
			bool flag = bamlDocument.Signature != "MSBAML";
			if (flag)
			{
				throw new NotSupportedException();
			}
			bamlDocument.ReaderVersion = new BamlDocument.BamlVersion
			{
				Major = bamlBinaryReader.ReadUInt16(),
				Minor = bamlBinaryReader.ReadUInt16()
			};
			bamlDocument.UpdaterVersion = new BamlDocument.BamlVersion
			{
				Major = bamlBinaryReader.ReadUInt16(),
				Minor = bamlBinaryReader.ReadUInt16()
			};
			bamlDocument.WriterVersion = new BamlDocument.BamlVersion
			{
				Major = bamlBinaryReader.ReadUInt16(),
				Minor = bamlBinaryReader.ReadUInt16()
			};
			bool flag2 = bamlDocument.ReaderVersion.Major != 0 || bamlDocument.ReaderVersion.Minor != 96 || bamlDocument.UpdaterVersion.Major != 0 || bamlDocument.UpdaterVersion.Minor != 96 || bamlDocument.WriterVersion.Major != 0 || bamlDocument.WriterVersion.Minor != 96;
			if (flag2)
			{
				throw new NotSupportedException();
			}
			Dictionary<long, BamlRecord> recs = new Dictionary<long, BamlRecord>();
			while (str.Position < str.Length)
			{
				long position = str.Position;
				BamlRecord bamlRecord;
				switch (bamlBinaryReader.ReadByte())
				{
				case 1:
					bamlRecord = new DocumentStartRecord();
					break;
				case 2:
					bamlRecord = new DocumentEndRecord();
					break;
				case 3:
					bamlRecord = new ElementStartRecord();
					break;
				case 4:
					bamlRecord = new ElementEndRecord();
					break;
				case 5:
					bamlRecord = new PropertyRecord();
					break;
				case 6:
					bamlRecord = new PropertyCustomRecord();
					break;
				case 7:
					bamlRecord = new PropertyComplexStartRecord();
					break;
				case 8:
					bamlRecord = new PropertyComplexEndRecord();
					break;
				case 9:
					bamlRecord = new PropertyArrayStartRecord();
					break;
				case 10:
					bamlRecord = new PropertyArrayEndRecord();
					break;
				case 11:
					bamlRecord = new PropertyListStartRecord();
					break;
				case 12:
					bamlRecord = new PropertyListEndRecord();
					break;
				case 13:
					bamlRecord = new PropertyDictionaryStartRecord();
					break;
				case 14:
					bamlRecord = new PropertyDictionaryEndRecord();
					break;
				case 15:
					bamlRecord = new LiteralContentRecord();
					break;
				case 16:
					bamlRecord = new TextRecord();
					break;
				case 17:
					bamlRecord = new TextWithConverterRecord();
					break;
				case 18:
					bamlRecord = new RoutedEventRecord();
					break;
				case 19:
				case 21:
				case 22:
				case 23:
				case 24:
				case 26:
				case 57:
					goto IL_4A4;
				case 20:
					bamlRecord = new XmlnsPropertyRecord();
					break;
				case 25:
					bamlRecord = new DefAttributeRecord();
					break;
				case 27:
					bamlRecord = new PIMappingRecord();
					break;
				case 28:
					bamlRecord = new AssemblyInfoRecord();
					break;
				case 29:
					bamlRecord = new TypeInfoRecord();
					break;
				case 30:
					bamlRecord = new TypeSerializerInfoRecord();
					break;
				case 31:
					bamlRecord = new AttributeInfoRecord();
					break;
				case 32:
					bamlRecord = new StringInfoRecord();
					break;
				case 33:
					bamlRecord = new PropertyStringReferenceRecord();
					break;
				case 34:
					bamlRecord = new PropertyTypeReferenceRecord();
					break;
				case 35:
					bamlRecord = new PropertyWithExtensionRecord();
					break;
				case 36:
					bamlRecord = new PropertyWithConverterRecord();
					break;
				case 37:
					bamlRecord = new DeferableContentStartRecord();
					break;
				case 38:
					bamlRecord = new DefAttributeKeyStringRecord();
					break;
				case 39:
					bamlRecord = new DefAttributeKeyTypeRecord();
					break;
				case 40:
					bamlRecord = new KeyElementStartRecord();
					break;
				case 41:
					bamlRecord = new KeyElementEndRecord();
					break;
				case 42:
					bamlRecord = new ConstructorParametersStartRecord();
					break;
				case 43:
					bamlRecord = new ConstructorParametersEndRecord();
					break;
				case 44:
					bamlRecord = new ConstructorParameterTypeRecord();
					break;
				case 45:
					bamlRecord = new ConnectionIdRecord();
					break;
				case 46:
					bamlRecord = new ContentPropertyRecord();
					break;
				case 47:
					bamlRecord = new NamedElementStartRecord();
					break;
				case 48:
					bamlRecord = new StaticResourceStartRecord();
					break;
				case 49:
					bamlRecord = new StaticResourceEndRecord();
					break;
				case 50:
					bamlRecord = new StaticResourceIdRecord();
					break;
				case 51:
					bamlRecord = new TextWithIdRecord();
					break;
				case 52:
					bamlRecord = new PresentationOptionsAttributeRecord();
					break;
				case 53:
					bamlRecord = new LineNumberAndPositionRecord();
					break;
				case 54:
					bamlRecord = new LinePositionRecord();
					break;
				case 55:
					bamlRecord = new OptimizedStaticResourceRecord();
					break;
				case 56:
					bamlRecord = new PropertyWithStaticResourceIdRecord();
					break;
				default:
					goto IL_4A4;
				}
				bamlRecord.Position = position;
				bamlRecord.Read(bamlBinaryReader);
				bamlDocument.Add(bamlRecord);
				recs.Add(position, bamlRecord);
				continue;
				IL_4A4:
				throw new NotSupportedException();
			}
			Func<long, BamlRecord> <>9__0;
			for (int i = 0; i < bamlDocument.Count; i++)
			{
				IBamlDeferRecord bamlDeferRecord = bamlDocument[i] as IBamlDeferRecord;
				bool flag3 = bamlDeferRecord != null;
				if (flag3)
				{
					IBamlDeferRecord bamlDeferRecord2 = bamlDeferRecord;
					BamlDocument doc = bamlDocument;
					int index = i;
					Func<long, BamlRecord> resolve;
					if ((resolve = <>9__0) == null)
					{
						resolve = (<>9__0 = ((long _) => recs[_]));
					}
					bamlDeferRecord2.ReadDefer(doc, index, resolve);
				}
			}
			return bamlDocument;
		}
	}
}
