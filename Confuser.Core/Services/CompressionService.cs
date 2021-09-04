using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Confuser.Core.Helpers;
using dnlib.DotNet;
using SevenZip;
using SevenZip.Compression.LZMA;

namespace Confuser.Core.Services
{
	// Token: 0x02000073 RID: 115
	internal class CompressionService : ICompressionService
	{
		// Token: 0x060002D3 RID: 723 RVA: 0x0000336D File Offset: 0x0000156D
		public CompressionService(ConfuserContext context)
		{
			this.context = context;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000125DC File Offset: 0x000107DC
		public MethodDef TryGetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init)
		{
			Tuple<MethodDef, List<IDnlibDef>> tuple = this.context.Annotations.Get<Tuple<MethodDef, List<IDnlibDef>>>(module, CompressionService.Decompressor, null);
			bool flag = tuple == null;
			MethodDef result;
			if (flag)
			{
				result = null;
			}
			else
			{
				foreach (IDnlibDef obj in tuple.Item2)
				{
					init(obj);
				}
				result = tuple.Item1;
			}
			return result;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00012664 File Offset: 0x00010864
		public MethodDef GetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init)
		{
			Tuple<MethodDef, List<IDnlibDef>> orCreate = this.context.Annotations.GetOrCreate<Tuple<MethodDef, List<IDnlibDef>>>(module, CompressionService.Decompressor, delegate(object m)
			{
				IRuntimeService service = this.context.Registry.GetService<IRuntimeService>();
				List<IDnlibDef> list = InjectHelper.Inject(service.GetRuntimeType("Confuser.Runtime.Lzma"), module.GlobalType, module).ToList<IDnlibDef>();
				MethodDef methodDef = null;
				foreach (IDnlibDef dnlibDef in list)
				{
					bool flag = dnlibDef is MethodDef;
					if (flag)
					{
						MethodDef methodDef2 = (MethodDef)dnlibDef;
						bool flag2 = methodDef2.Access == MethodAttributes.Public;
						if (flag2)
						{
							methodDef2.Access = MethodAttributes.Assembly;
						}
						bool flag3 = !methodDef2.IsConstructor;
						if (flag3)
						{
							methodDef2.IsSpecialName = false;
						}
						bool flag4 = methodDef2.Name == "Decompress";
						if (flag4)
						{
							methodDef = methodDef2;
						}
					}
					else
					{
						bool flag5 = dnlibDef is FieldDef;
						if (flag5)
						{
							FieldDef fieldDef = (FieldDef)dnlibDef;
							bool flag6 = fieldDef.Access == FieldAttributes.Public;
							if (flag6)
							{
								fieldDef.Access = FieldAttributes.Assembly;
							}
							bool isLiteral = fieldDef.IsLiteral;
							if (isLiteral)
							{
								fieldDef.DeclaringType.Fields.Remove(fieldDef);
							}
						}
					}
				}
				list.RemoveWhere((IDnlibDef def) => def is FieldDef && ((FieldDef)def).IsLiteral);
				Debug.Assert(methodDef != null);
				return Tuple.Create<MethodDef, List<IDnlibDef>>(methodDef, list);
			});
			foreach (IDnlibDef obj in orCreate.Item2)
			{
				init(obj);
			}
			return orCreate.Item1;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00012704 File Offset: 0x00010904
		public byte[] Compress(byte[] data, Action<double> progressFunc = null)
		{
			CoderPropID[] propIDs = new CoderPropID[]
			{
				CoderPropID.DictionarySize,
				CoderPropID.PosStateBits,
				CoderPropID.LitContextBits,
				CoderPropID.LitPosBits,
				CoderPropID.Algorithm,
				CoderPropID.NumFastBytes,
				CoderPropID.MatchFinder,
				CoderPropID.EndMarker
			};
			object[] properties = new object[]
			{
				8388608,
				2,
				3,
				0,
				2,
				128,
				"bt4",
				false
			};
			MemoryStream memoryStream = new MemoryStream();
			Encoder encoder = new Encoder();
			encoder.SetCoderProperties(propIDs, properties);
			encoder.WriteCoderProperties(memoryStream);
			long num = (long)data.Length;
			for (int i = 0; i < 8; i++)
			{
				memoryStream.WriteByte((byte)(num >> 8 * i));
			}
			ICodeProgress progress = null;
			bool flag = progressFunc != null;
			if (flag)
			{
				progress = new CompressionService.CompressionLogger(progressFunc, data.Length);
			}
			encoder.Code(new MemoryStream(data), memoryStream, -1L, -1L, progress);
			return memoryStream.ToArray();
		}

		// Token: 0x04000222 RID: 546
		private static readonly object Decompressor = new object();

		// Token: 0x04000223 RID: 547
		private readonly ConfuserContext context;

		// Token: 0x02000074 RID: 116
		private class CompressionLogger : ICodeProgress
		{
			// Token: 0x060002D8 RID: 728 RVA: 0x0000338A File Offset: 0x0000158A
			public CompressionLogger(Action<double> progressFunc, int size)
			{
				this.progressFunc = progressFunc;
				this.size = size;
			}

			// Token: 0x060002D9 RID: 729 RVA: 0x00012800 File Offset: 0x00010A00
			public void SetProgress(long inSize, long outSize)
			{
				double obj = (double)inSize / (double)this.size;
				this.progressFunc(obj);
			}

			// Token: 0x04000224 RID: 548
			private readonly Action<double> progressFunc;

			// Token: 0x04000225 RID: 549
			private readonly int size;
		}
	}
}
