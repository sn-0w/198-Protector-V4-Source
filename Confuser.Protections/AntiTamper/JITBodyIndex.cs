using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.PE;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000FC RID: 252
	internal class JITBodyIndex : IChunk
	{
		// Token: 0x06000424 RID: 1060 RVA: 0x0001AED0 File Offset: 0x000190D0
		public JITBodyIndex(IEnumerable<uint> tokens)
		{
			this._bodies = tokens.ToDictionary((uint token) => token, (uint token) => null);
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x00003665 File Offset: 0x00001865
		// (set) Token: 0x06000426 RID: 1062 RVA: 0x0000366D File Offset: 0x0000186D
		public FileOffset FileOffset { get; private set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x00003676 File Offset: 0x00001876
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x0000367E File Offset: 0x0000187E
		public RVA RVA { get; private set; }

		// Token: 0x06000429 RID: 1065 RVA: 0x00003687 File Offset: 0x00001887
		public void SetOffset(FileOffset offset, RVA rva)
		{
			this.FileOffset = offset;
			this.RVA = rva;
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x0000369A File Offset: 0x0000189A
		public uint GetFileLength()
		{
			return (uint)(this._bodies.Count * 8 + 4);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x000036AB File Offset: 0x000018AB
		public uint GetVirtualSize()
		{
			return this.GetFileLength();
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0001AF30 File Offset: 0x00019130
		public void WriteTo(DataWriter writer)
		{
			uint num = this.GetFileLength() - 4U;
			writer.WriteUInt32((uint)this._bodies.Count);
			foreach (KeyValuePair<uint, JITMethodBody> keyValuePair in from entry in this._bodies
			orderby entry.Key
			select entry)
			{
				writer.WriteUInt32(keyValuePair.Key);
				Debug.Assert(keyValuePair.Value != null);
				Debug.Assert((num + keyValuePair.Value.Offset) % 4U == 0U);
				writer.WriteUInt32(num + keyValuePair.Value.Offset >> 2);
			}
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x000036B3 File Offset: 0x000018B3
		public void Add(uint token, JITMethodBody body)
		{
			Debug.Assert(this._bodies.ContainsKey(token));
			this._bodies[token] = body;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001B008 File Offset: 0x00019208
		public void PopulateSection(PESection section)
		{
			uint num = 0U;
			foreach (KeyValuePair<uint, JITMethodBody> keyValuePair in from entry in this._bodies
			orderby entry.Key
			select entry)
			{
				Debug.Assert(keyValuePair.Value != null);
				section.Add(keyValuePair.Value, 4U);
				keyValuePair.Value.Offset = num;
				Debug.Assert(keyValuePair.Value.GetFileLength() % 4U == 0U);
				num += keyValuePair.Value.GetFileLength();
			}
		}

		// Token: 0x0400020A RID: 522
		private readonly Dictionary<uint, JITMethodBody> _bodies;
	}
}
