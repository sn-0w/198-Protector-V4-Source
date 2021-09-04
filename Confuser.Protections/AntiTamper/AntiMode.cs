using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000F1 RID: 241
	internal class AntiMode : IModeHandler
	{
		// Token: 0x060003EB RID: 1003 RVA: 0x00019C78 File Offset: 0x00017E78
		public void HandleInject(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator(parent.FullId);
			this.z = this.random.NextUInt32();
			this.x = this.random.NextUInt32();
			this.c = this.random.NextUInt32();
			this.v = this.random.NextUInt32();
			this._sectionName = new ValueTuple<uint, uint>(this.random.NextUInt32() & 2139062143U, this.random.NextUInt32() & 2139062143U);
			Mode parameter = parameters.GetParameter<Mode>(context, context.CurrentModule, "key", Mode.Dynamic);
			Mode mode = parameter;
			if (mode != Mode.Normal)
			{
				if (mode != Mode.Dynamic)
				{
					throw new UnreachableException();
				}
				this.deriver = new DynamicDeriver();
			}
			else
			{
				this.deriver = new NormalDeriver();
			}
			this.deriver.Init(context, this.random);
			IRuntimeService service = context.Registry.GetService<IRuntimeService>();
			TypeDef runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiTamperAnti");
			IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, context.CurrentModule.GlobalType, context.CurrentModule);
			MethodDef methodDef = (MethodDef)enumerable.Single((IDnlibDef m) => m.Name == "Initialize");
			methodDef.Body.SimplifyMacros(methodDef.Parameters);
			List<Instruction> list = methodDef.Body.Instructions.ToList<Instruction>();
			for (int i = 0; i < list.Count; i++)
			{
				Instruction instruction = list[i];
				bool flag = instruction.OpCode == OpCodes.Ldtoken;
				if (flag)
				{
					instruction.Operand = context.CurrentModule.GlobalType;
				}
				else
				{
					bool flag2 = instruction.OpCode == OpCodes.Call;
					if (flag2)
					{
						IMethod method = (IMethod)instruction.Operand;
						bool flag3 = method.DeclaringType.Name == "Mutation" && method.Name == "Crypt";
						if (flag3)
						{
							Instruction instruction2 = list[i - 2];
							Instruction instruction3 = list[i - 1];
							Debug.Assert(instruction2.OpCode == OpCodes.Ldloc && instruction3.OpCode == OpCodes.Ldloc);
							list.RemoveAt(i);
							list.RemoveAt(i - 1);
							list.RemoveAt(i - 2);
							list.InsertRange(i - 2, this.deriver.EmitDerivation(methodDef, context, (Local)instruction2.Operand, (Local)instruction3.Operand));
						}
					}
				}
			}
			methodDef.Body.Instructions.Clear();
			foreach (Instruction item in list)
			{
				methodDef.Body.Instructions.Add(item);
			}
			MutationHelper.InjectKeys(methodDef, new int[]
			{
				0,
				1,
				2,
				3,
				4
			}, new int[]
			{
				(int)(this._sectionName.Item1 * this._sectionName.Item2),
				(int)this.z,
				(int)this.x,
				(int)this.c,
				(int)this.v
			});
			INameService service2 = context.Registry.GetService<INameService>();
			IMarkerService service3 = context.Registry.GetService<IMarkerService>();
			foreach (IDnlibDef dnlibDef in enumerable)
			{
				service2.MarkHelper(dnlibDef, service3, parent);
				bool flag4 = dnlibDef is MethodDef;
				if (flag4)
				{
					parent.ExcludeMethod(context, (MethodDef)dnlibDef);
				}
			}
			MethodDef methodDef2 = context.CurrentModule.GlobalType.FindStaticConstructor();
			methodDef2.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, methodDef));
			parent.ExcludeMethod(context, methodDef2);
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00003515 File Offset: 0x00001715
		public void HandleMD(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.methods = ImmutableList.ToImmutableList<MethodDef>(parameters.Targets.OfType<MethodDef>());
			context.CurrentModuleWriterOptions.WriterEvent += this.WriterEvent;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0001A0B0 File Offset: 0x000182B0
		private void WriterEvent(object sender, ModuleWriterEventArgs e)
		{
			ModuleWriterBase writer = (ModuleWriterBase)sender;
			bool flag = e.Event == ModuleWriterEvent.MDEndCreateTables;
			if (flag)
			{
				this.CreateSections(writer);
			}
			else
			{
				bool flag2 = e.Event == ModuleWriterEvent.BeginStrongNameSign;
				if (flag2)
				{
					this.EncryptSection(writer);
				}
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x0001A0FC File Offset: 0x000182FC
		private void CreateSections(ModuleWriterBase writer)
		{
			byte[] bytes = new byte[]
			{
				(byte)this._sectionName.Item1,
				(byte)(this._sectionName.Item1 >> 8),
				(byte)(this._sectionName.Item1 >> 16),
				(byte)(this._sectionName.Item1 >> 24),
				(byte)this._sectionName.Item2,
				(byte)(this._sectionName.Item2 >> 8),
				(byte)(this._sectionName.Item2 >> 16),
				(byte)(this._sectionName.Item2 >> 24)
			};
			PESection pesection = new PESection(Encoding.ASCII.GetString(bytes), 3758096448U);
			writer.Sections.Insert(0, pesection);
			uint value = writer.TextSection.Remove(writer.Metadata).Value;
			writer.TextSection.Add(writer.Metadata, value);
			value = writer.TextSection.Remove(writer.NetResources).Value;
			writer.TextSection.Add(writer.NetResources, value);
			value = writer.TextSection.Remove(writer.Constants).Value;
			pesection.Add(writer.Constants, value);
			PESection pesection2 = new PESection("", 1610612768U);
			bool flag = false;
			bool flag2 = writer.StrongNameSignature != null;
			if (flag2)
			{
				value = writer.TextSection.Remove(writer.StrongNameSignature).Value;
				pesection2.Add(writer.StrongNameSignature, value);
				flag = true;
			}
			ModuleWriter moduleWriter = writer as ModuleWriter;
			bool flag3 = moduleWriter != null;
			if (flag3)
			{
				bool flag4 = moduleWriter.ImportAddressTable != null;
				if (flag4)
				{
					value = writer.TextSection.Remove(moduleWriter.ImportAddressTable).Value;
					pesection2.Add(moduleWriter.ImportAddressTable, value);
					flag = true;
				}
				bool flag5 = moduleWriter.StartupStub != null;
				if (flag5)
				{
					value = writer.TextSection.Remove(moduleWriter.StartupStub).Value;
					pesection2.Add(moduleWriter.StartupStub, value);
					flag = true;
				}
			}
			bool flag6 = flag;
			if (flag6)
			{
				writer.Sections.AddBeforeReloc(pesection2);
			}
			MethodBodyChunks methodBodyChunks = new MethodBodyChunks(writer.TheOptions.ShareMethodBodies);
			pesection.Add(methodBodyChunks, 4U);
			foreach (MethodDef methodDef in this.methods)
			{
				bool flag7 = !methodDef.HasBody;
				if (!flag7)
				{
					dnlib.DotNet.Writer.MethodBody methodBody = writer.Metadata.GetMethodBody(methodDef);
					writer.MethodBodies.Remove(methodBody);
					methodBodyChunks.Add(methodBody);
				}
			}
			pesection.Add(new ByteArrayChunk(new byte[4]), 4U);
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0001A3EC File Offset: 0x000185EC
		private void EncryptSection(ModuleWriterBase writer)
		{
			Stream destinationStream = writer.DestinationStream;
			BinaryReader binaryReader = new BinaryReader(destinationStream);
			destinationStream.Position = 60L;
			destinationStream.Position = (long)((ulong)binaryReader.ReadUInt32());
			destinationStream.Position += 6L;
			ushort num = binaryReader.ReadUInt16();
			destinationStream.Position += 12L;
			ushort num2 = binaryReader.ReadUInt16();
			destinationStream.Position += (long)(2 + num2);
			uint num3 = 0U;
			uint num4 = 0U;
			int num5 = -1;
			ModuleDefMD moduleDefMD;
			bool flag;
			if (writer is NativeModuleWriter)
			{
				ModuleDef module = writer.Module;
				moduleDefMD = (module as ModuleDefMD);
				flag = (moduleDefMD != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				num5 = moduleDefMD.Metadata.PEImage.ImageSectionHeaders.Count;
			}
			for (int i = 0; i < (int)num; i++)
			{
				bool flag3 = num5 > 0;
				uint num6;
				if (flag3)
				{
					num5--;
					destinationStream.Write(new byte[8], 0, 8);
					num6 = 0U;
				}
				else
				{
					num6 = binaryReader.ReadUInt32() * binaryReader.ReadUInt32();
				}
				destinationStream.Position += 8L;
				bool flag4 = num6 == this._sectionName.Item1 * this._sectionName.Item2;
				if (flag4)
				{
					num4 = binaryReader.ReadUInt32();
					num3 = binaryReader.ReadUInt32();
				}
				else
				{
					bool flag5 = num6 > 0U;
					if (flag5)
					{
						uint size = binaryReader.ReadUInt32();
						uint offset = binaryReader.ReadUInt32();
						this.Hash(destinationStream, binaryReader, offset, size);
					}
					else
					{
						destinationStream.Position += 8L;
					}
				}
				destinationStream.Position += 16L;
			}
			uint[] array = this.DeriveKey();
			num4 >>= 2;
			destinationStream.Position = (long)((ulong)num3);
			uint[] array2 = new uint[num4];
			for (uint num7 = 0U; num7 < num4; num7 += 1U)
			{
				uint num8 = binaryReader.ReadUInt32();
				array2[(int)num7] = (num8 ^ array[(int)(num7 & 15U)]);
				array[(int)(num7 & 15U)] = (array[(int)(num7 & 15U)] ^ num8) + 1035675673U;
			}
			byte[] array3 = new byte[num4 << 2];
			Buffer.BlockCopy(array2, 0, array3, 0, array3.Length);
			destinationStream.Position = (long)((ulong)num3);
			destinationStream.Write(array3, 0, array3.Length);
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0001A630 File Offset: 0x00018830
		private void Hash(Stream stream, BinaryReader reader, uint offset, uint size)
		{
			long position = stream.Position;
			stream.Position = (long)((ulong)offset);
			size >>= 2;
			for (uint num = 0U; num < size; num += 1U)
			{
				uint num2 = reader.ReadUInt32();
				uint num3 = (this.z ^ num2) + this.x + this.c * this.v;
				this.z = this.x;
				this.x = this.c;
				this.x = this.v;
				this.v = num3;
			}
			stream.Position = position;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0001A6C4 File Offset: 0x000188C4
		private uint[] DeriveKey()
		{
			uint[] array = new uint[16];
			uint[] array2 = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				array[i] = this.v;
				array2[i] = this.x;
				this.z = (this.x >> 5 | this.x << 27);
				this.x = (this.c >> 3 | this.c << 29);
				this.c = (this.v >> 7 | this.v << 25);
				this.v = (this.z >> 11 | this.z << 21);
			}
			return this.deriver.DeriveKey(array, array2);
		}

		// Token: 0x040001DE RID: 478
		protected const uint CNT_CODE = 32U;

		// Token: 0x040001DF RID: 479
		protected const uint CNT_INITIALIZED_DATA = 64U;

		// Token: 0x040001E0 RID: 480
		protected const uint MEM_EXECUTE = 536870912U;

		// Token: 0x040001E1 RID: 481
		protected const uint MEM_READ = 1073741824U;

		// Token: 0x040001E2 RID: 482
		protected const uint MEM_WRITE = 2147483648U;

		// Token: 0x040001E3 RID: 483
		private IKeyDeriver deriver;

		// Token: 0x040001E4 RID: 484
		private IImmutableList<MethodDef> methods;

		// Token: 0x040001E5 RID: 485
		[TupleElementNames(new string[]
		{
			"Part1",
			"Part2"
		})]
		private ValueTuple<uint, uint> _sectionName;

		// Token: 0x040001E6 RID: 486
		private RandomGenerator random;

		// Token: 0x040001E7 RID: 487
		private uint c;

		// Token: 0x040001E8 RID: 488
		private uint v;

		// Token: 0x040001E9 RID: 489
		private uint x;

		// Token: 0x040001EA RID: 490
		private uint z;
	}
}
