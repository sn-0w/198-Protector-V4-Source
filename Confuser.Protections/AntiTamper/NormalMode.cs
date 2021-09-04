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
	// Token: 0x02000104 RID: 260
	internal class NormalMode : IModeHandler
	{
		// Token: 0x06000459 RID: 1113 RVA: 0x0001C6D0 File Offset: 0x0001A8D0
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
			TypeDef runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiTamperNormal");
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

		// Token: 0x0600045A RID: 1114 RVA: 0x00003796 File Offset: 0x00001996
		public void HandleMD(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.methods = ImmutableList.ToImmutableList<MethodDef>(parameters.Targets.OfType<MethodDef>());
			context.CurrentModuleWriterOptions.WriterEvent += this.WriterEvent;
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0001CB08 File Offset: 0x0001AD08
		private void WriterEvent(object sender, ModuleWriterEventArgs e)
		{
			ModuleWriterBase writer = e.Writer;
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

		// Token: 0x0600045C RID: 1116 RVA: 0x0001CB54 File Offset: 0x0001AD54
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

		// Token: 0x0600045D RID: 1117 RVA: 0x0001CE44 File Offset: 0x0001B044
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

		// Token: 0x0600045E RID: 1118 RVA: 0x0001D088 File Offset: 0x0001B288
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

		// Token: 0x0600045F RID: 1119 RVA: 0x0001D11C File Offset: 0x0001B31C
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

		// Token: 0x04000241 RID: 577
		protected const uint CNT_CODE = 32U;

		// Token: 0x04000242 RID: 578
		protected const uint CNT_INITIALIZED_DATA = 64U;

		// Token: 0x04000243 RID: 579
		protected const uint MEM_EXECUTE = 536870912U;

		// Token: 0x04000244 RID: 580
		protected const uint MEM_READ = 1073741824U;

		// Token: 0x04000245 RID: 581
		protected const uint MEM_WRITE = 2147483648U;

		// Token: 0x04000246 RID: 582
		private IKeyDeriver deriver;

		// Token: 0x04000247 RID: 583
		private IImmutableList<MethodDef> methods;

		// Token: 0x04000248 RID: 584
		[TupleElementNames(new string[]
		{
			"Part1",
			"Part2"
		})]
		private ValueTuple<uint, uint> _sectionName;

		// Token: 0x04000249 RID: 585
		private RandomGenerator random;

		// Token: 0x0400024A RID: 586
		private uint c;

		// Token: 0x0400024B RID: 587
		private uint v;

		// Token: 0x0400024C RID: 588
		private uint x;

		// Token: 0x0400024D RID: 589
		private uint z;
	}
}
