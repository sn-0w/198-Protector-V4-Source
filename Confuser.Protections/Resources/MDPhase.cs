using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Resources
{
	// Token: 0x02000055 RID: 85
	internal class MDPhase
	{
		// Token: 0x060001BE RID: 446 RVA: 0x00002AD6 File Offset: 0x00000CD6
		public MDPhase(REContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00002AE7 File Offset: 0x00000CE7
		public void Hook()
		{
			this.ctx.Context.CurrentModuleWriterOptions.WriterEvent += this.OnWriterEvent;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000098DC File Offset: 0x00007ADC
		private void OnWriterEvent(object sender, ModuleWriterEventArgs e)
		{
			ModuleWriterBase moduleWriterBase = (ModuleWriterBase)sender;
			bool flag = e.Event == ModuleWriterEvent.MDBeginAddResources;
			if (flag)
			{
				this.ctx.Context.CheckCancellation();
				this.ctx.Context.Logger.Debug("Encrypting resources...");
				bool flag2 = this.ctx.Context.Packer != null;
				List<EmbeddedResource> list = this.ctx.Module.Resources.OfType<EmbeddedResource>().ToList<EmbeddedResource>();
				bool flag3 = !flag2;
				if (flag3)
				{
					this.ctx.Module.Resources.RemoveWhere((Resource res) => res is EmbeddedResource);
				}
				string text = this.ctx.Name.RandomName(RenameMode.Letters);
				PublicKey publicKey = null;
				bool flag4 = moduleWriterBase.TheOptions.StrongNameKey != null;
				if (flag4)
				{
					publicKey = PublicKeyBase.CreatePublicKey(moduleWriterBase.TheOptions.StrongNameKey.PublicKey);
				}
				ModuleDefUser moduleDefUser = new ModuleDefUser(text + ".dll", new Guid?(Guid.NewGuid()), this.ctx.Module.CorLibTypes.AssemblyRef);
				moduleDefUser.Kind = ModuleKind.Dll;
				AssemblyDefUser assemblyDefUser = new AssemblyDefUser(text, new Version(1, 0), publicKey);
				assemblyDefUser.Modules.Add(moduleDefUser);
				moduleDefUser.Characteristics = this.ctx.Module.Characteristics;
				moduleDefUser.Cor20HeaderFlags = this.ctx.Module.Cor20HeaderFlags;
				moduleDefUser.Cor20HeaderRuntimeVersion = this.ctx.Module.Cor20HeaderRuntimeVersion;
				moduleDefUser.DllCharacteristics = this.ctx.Module.DllCharacteristics;
				moduleDefUser.EncBaseId = this.ctx.Module.EncBaseId;
				moduleDefUser.EncId = this.ctx.Module.EncId;
				moduleDefUser.Generation = this.ctx.Module.Generation;
				moduleDefUser.Machine = this.ctx.Module.Machine;
				moduleDefUser.RuntimeVersion = this.ctx.Module.RuntimeVersion;
				moduleDefUser.TablesHeaderVersion = this.ctx.Module.TablesHeaderVersion;
				moduleDefUser.RuntimeVersion = this.ctx.Module.RuntimeVersion;
				AssemblyRefUser assemblyRefUser = new AssemblyRefUser(moduleDefUser.Assembly);
				bool flag5 = publicKey == null;
				if (flag5)
				{
					assemblyRefUser.Attributes &= ~AssemblyAttributes.PublicKey;
				}
				bool flag6 = !flag2;
				if (flag6)
				{
					foreach (EmbeddedResource embeddedResource in list)
					{
						embeddedResource.Attributes = ManifestResourceAttributes.Public;
						moduleDefUser.Resources.Add(embeddedResource);
						this.ctx.Module.Resources.Add(new AssemblyLinkedResource(embeddedResource.Name, assemblyRefUser, embeddedResource.Attributes));
					}
				}
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					moduleDefUser.Write(memoryStream, new ModuleWriterOptions(moduleWriterBase.Module)
					{
						StrongNameKey = moduleWriterBase.TheOptions.StrongNameKey
					});
					array = memoryStream.ToArray();
				}
				array = this.ctx.Context.Registry.GetService<ICompressionService>().Compress(array, delegate(double progress)
				{
					this.ctx.Context.Logger.Progress((int)(progress * 10000.0), 10000);
				});
				this.ctx.Context.Logger.EndProgress();
				this.ctx.Context.CheckCancellation();
				uint num = (uint)((array.Length + 3) / 4);
				num = (num + 15U & 4294967280U);
				uint[] array2 = new uint[num];
				Buffer.BlockCopy(array, 0, array2, 0, array.Length);
				Debug.Assert(num % 16U == 0U);
				uint num2 = this.ctx.Random.NextUInt32() | 16U;
				uint[] array3 = new uint[16];
				uint num3 = num2;
				for (int i = 0; i < 16; i++)
				{
					num3 ^= num3 >> 13;
					num3 ^= num3 << 25;
					num3 ^= num3 >> 27;
					array3[i] = num3;
				}
				byte[] array4 = new byte[array2.Length * 4];
				int j;
				for (j = 0; j < array2.Length; j += 16)
				{
					uint[] src = this.ctx.ModeHandler.Encrypt(array2, j, array3);
					for (int k = 0; k < 16; k++)
					{
						array3[k] ^= array2[j + k];
					}
					Buffer.BlockCopy(src, 0, array4, j * 4, 64);
				}
				Debug.Assert(j == array2.Length);
				uint num4 = (uint)array4.Length;
				TablesHeap tablesHeap = moduleWriterBase.Metadata.TablesHeap;
				uint classLayoutRid = moduleWriterBase.Metadata.GetClassLayoutRid(this.ctx.DataType);
				RawClassLayoutRow rawClassLayoutRow = tablesHeap.ClassLayoutTable[classLayoutRid];
				tablesHeap.ClassLayoutTable[classLayoutRid] = new RawClassLayoutRow(rawClassLayoutRow.PackingSize, num4, rawClassLayoutRow.Parent);
				uint rid = moduleWriterBase.Metadata.GetRid(this.ctx.DataField);
				RawFieldRow rawFieldRow = tablesHeap.FieldTable[rid];
				tablesHeap.FieldTable[rid] = new RawFieldRow(rawFieldRow.Flags | 256, rawFieldRow.Name, rawFieldRow.Signature);
				this.encryptedResource = moduleWriterBase.Constants.Add(new ByteArrayChunk(array4), 8U);
				MutationHelper.InjectKeys(this.ctx.InitMethod, new int[]
				{
					0,
					1
				}, new int[]
				{
					(int)(num4 / 4U),
					(int)num2
				});
			}
			else
			{
				bool flag7 = e.Event == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
				if (flag7)
				{
					TablesHeap tablesHeap2 = moduleWriterBase.Metadata.TablesHeap;
					uint fieldRVARid = moduleWriterBase.Metadata.GetFieldRVARid(this.ctx.DataField);
					RawFieldRVARow rawFieldRVARow = tablesHeap2.FieldRVATable[fieldRVARid];
					tablesHeap2.FieldRVATable[fieldRVARid] = new RawFieldRVARow((uint)this.encryptedResource.RVA, rawFieldRVARow.Field);
				}
			}
		}

		// Token: 0x04000059 RID: 89
		private readonly REContext ctx;

		// Token: 0x0400005A RID: 90
		private ByteArrayChunk encryptedResource;
	}
}
