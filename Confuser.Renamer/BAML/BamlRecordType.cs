using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000024 RID: 36
	internal enum BamlRecordType : byte
	{
		// Token: 0x0400007F RID: 127
		ClrEvent = 19,
		// Token: 0x04000080 RID: 128
		Comment = 23,
		// Token: 0x04000081 RID: 129
		AssemblyInfo = 28,
		// Token: 0x04000082 RID: 130
		AttributeInfo = 31,
		// Token: 0x04000083 RID: 131
		ConstructorParametersStart = 42,
		// Token: 0x04000084 RID: 132
		ConstructorParametersEnd,
		// Token: 0x04000085 RID: 133
		ConstructorParameterType,
		// Token: 0x04000086 RID: 134
		ConnectionId,
		// Token: 0x04000087 RID: 135
		ContentProperty,
		// Token: 0x04000088 RID: 136
		DefAttribute = 25,
		// Token: 0x04000089 RID: 137
		DefAttributeKeyString = 38,
		// Token: 0x0400008A RID: 138
		DefAttributeKeyType,
		// Token: 0x0400008B RID: 139
		DeferableContentStart = 37,
		// Token: 0x0400008C RID: 140
		DefTag = 24,
		// Token: 0x0400008D RID: 141
		DocumentEnd = 2,
		// Token: 0x0400008E RID: 142
		DocumentStart = 1,
		// Token: 0x0400008F RID: 143
		ElementEnd = 4,
		// Token: 0x04000090 RID: 144
		ElementStart = 3,
		// Token: 0x04000091 RID: 145
		EndAttributes = 26,
		// Token: 0x04000092 RID: 146
		KeyElementEnd = 41,
		// Token: 0x04000093 RID: 147
		KeyElementStart = 40,
		// Token: 0x04000094 RID: 148
		LastRecordType = 57,
		// Token: 0x04000095 RID: 149
		LineNumberAndPosition = 53,
		// Token: 0x04000096 RID: 150
		LinePosition,
		// Token: 0x04000097 RID: 151
		LiteralContent = 15,
		// Token: 0x04000098 RID: 152
		NamedElementStart = 47,
		// Token: 0x04000099 RID: 153
		OptimizedStaticResource = 55,
		// Token: 0x0400009A RID: 154
		PIMapping = 27,
		// Token: 0x0400009B RID: 155
		PresentationOptionsAttribute = 52,
		// Token: 0x0400009C RID: 156
		ProcessingInstruction = 22,
		// Token: 0x0400009D RID: 157
		Property = 5,
		// Token: 0x0400009E RID: 158
		PropertyArrayEnd = 10,
		// Token: 0x0400009F RID: 159
		PropertyArrayStart = 9,
		// Token: 0x040000A0 RID: 160
		PropertyComplexEnd = 8,
		// Token: 0x040000A1 RID: 161
		PropertyComplexStart = 7,
		// Token: 0x040000A2 RID: 162
		PropertyCustom = 6,
		// Token: 0x040000A3 RID: 163
		PropertyDictionaryEnd = 14,
		// Token: 0x040000A4 RID: 164
		PropertyDictionaryStart = 13,
		// Token: 0x040000A5 RID: 165
		PropertyListEnd = 12,
		// Token: 0x040000A6 RID: 166
		PropertyListStart = 11,
		// Token: 0x040000A7 RID: 167
		PropertyStringReference = 33,
		// Token: 0x040000A8 RID: 168
		PropertyTypeReference,
		// Token: 0x040000A9 RID: 169
		PropertyWithConverter = 36,
		// Token: 0x040000AA RID: 170
		PropertyWithExtension = 35,
		// Token: 0x040000AB RID: 171
		PropertyWithStaticResourceId = 56,
		// Token: 0x040000AC RID: 172
		RoutedEvent = 18,
		// Token: 0x040000AD RID: 173
		StaticResourceEnd = 49,
		// Token: 0x040000AE RID: 174
		StaticResourceId,
		// Token: 0x040000AF RID: 175
		StaticResourceStart = 48,
		// Token: 0x040000B0 RID: 176
		StringInfo = 32,
		// Token: 0x040000B1 RID: 177
		Text = 16,
		// Token: 0x040000B2 RID: 178
		TextWithConverter,
		// Token: 0x040000B3 RID: 179
		TextWithId = 51,
		// Token: 0x040000B4 RID: 180
		TypeInfo = 29,
		// Token: 0x040000B5 RID: 181
		TypeSerializerInfo,
		// Token: 0x040000B6 RID: 182
		XmlAttribute = 21,
		// Token: 0x040000B7 RID: 183
		XmlnsProperty = 20
	}
}
