using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections
{
	// Token: 0x02000024 RID: 36
	[BeforeProtection(new string[]
	{
		"Ki.AntiTamper"
	})]
	public class IntegrityProtection : Protection
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000024EA File Offset: 0x000006EA
		public override string Name
		{
			get
			{
				return "Integrity Protection";
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000024F1 File Offset: 0x000006F1
		public override string Description
		{
			get
			{
				return "This protection hashs the module to preventing file modifications.";
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00002105 File Offset: 0x00000305
		public override string Author
		{
			get
			{
				return "Wadu";
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000024F8 File Offset: 0x000006F8
		public override string Id
		{
			get
			{
				return "integrity prot";
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x000024FF File Offset: 0x000006FF
		public override string FullId
		{
			get
			{
				return "Wadu.IntegrityProtection";
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00002506 File Offset: 0x00000706
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new IntegrityProtection.IntegrityPhase(this));
			pipeline.InsertPreStage(PipelineStage.EndModule, new IntegrityProtection.HashPhase(this));
		}

		// Token: 0x02000025 RID: 37
		private class IntegrityPhase : ProtectionPhase
		{
			// Token: 0x060000BC RID: 188 RVA: 0x00002136 File Offset: 0x00000336
			public IntegrityPhase(IntegrityProtection parent) : base(parent)
			{
			}

			// Token: 0x17000056 RID: 86
			// (get) Token: 0x060000BD RID: 189 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000057 RID: 87
			// (get) Token: 0x060000BE RID: 190 RVA: 0x000024EA File Offset: 0x000006EA
			public override string Name
			{
				get
				{
					return "Integrity Protection";
				}
			}

			// Token: 0x060000BF RID: 191 RVA: 0x00005FDC File Offset: 0x000041DC
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.IntegrityProtection");
				IMarkerService service = context.Registry.GetService<IMarkerService>();
				INameService service2 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>().WithProgress(context.Logger))
				{
					IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, moduleDef.GlobalType, moduleDef);
					MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
					MethodDef method2 = (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "Initialize");
					methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, method2));
					foreach (IDnlibDef def in enumerable)
					{
						service2.MarkHelper(def, service, (Protection)base.Parent);
					}
				}
			}
		}

		// Token: 0x02000027 RID: 39
		internal class HashPhase : ProtectionPhase
		{
			// Token: 0x060000C3 RID: 195 RVA: 0x00002136 File Offset: 0x00000336
			public HashPhase(ConfuserComponent parent) : base(parent)
			{
			}

			// Token: 0x17000058 RID: 88
			// (get) Token: 0x060000C4 RID: 196 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000059 RID: 89
			// (get) Token: 0x060000C5 RID: 197 RVA: 0x00002531 File Offset: 0x00000731
			public override string Name
			{
				get
				{
					return "Hash Phase";
				}
			}

			// Token: 0x060000C6 RID: 198 RVA: 0x00006128 File Offset: 0x00004328
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				bool flag = parameters.Targets.Contains(context.CurrentModule);
				if (flag)
				{
					context.CurrentModuleWriterOptions.WriterEvent += this.CurrentModuleWriterListener_WriterEvent;
				}
			}

			// Token: 0x060000C7 RID: 199 RVA: 0x00006164 File Offset: 0x00004364
			private void CurrentModuleWriterListener_WriterEvent(object sender, ModuleWriterEventArgs e)
			{
				ModuleWriterBase writer = (ModuleWriterBase)sender;
				bool flag = e.Event == ModuleWriterEvent.End;
				if (flag)
				{
					this.HashFile(writer);
				}
			}

			// Token: 0x060000C8 RID: 200 RVA: 0x00006190 File Offset: 0x00004390
			internal string MD5(byte[] metin)
			{
				MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] array = md5CryptoServiceProvider.ComputeHash(metin);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("x2").ToLower());
				}
				return stringBuilder.ToString();
			}

			// Token: 0x060000C9 RID: 201 RVA: 0x000061F4 File Offset: 0x000043F4
			private void HashFile(ModuleWriterBase writer)
			{
				StreamReader streamReader = new StreamReader(writer.DestinationStream);
				byte[] metin = new BinaryReader(streamReader.BaseStream)
				{
					BaseStream = 
					{
						Position = 0L
					}
				}.ReadBytes((int)streamReader.BaseStream.Length);
				string s = this.MD5(metin);
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				writer.DestinationStream.Position = writer.DestinationStream.Length;
				writer.DestinationStream.Write(bytes, 0, bytes.Length);
			}
		}
	}
}
