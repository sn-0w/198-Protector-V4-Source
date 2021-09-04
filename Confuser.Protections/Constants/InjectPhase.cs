using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000B5 RID: 181
	internal class InjectPhase : ProtectionPhase
	{
		// Token: 0x0600030C RID: 780 RVA: 0x00002136 File Offset: 0x00000336
		public InjectPhase(ConstantProtection parent) : base(parent)
		{
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600030D RID: 781 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600030E RID: 782 RVA: 0x00003150 File Offset: 0x00001350
		public override string Name
		{
			get
			{
				return "Constant encryption helpers injection";
			}
		}

		// Token: 0x0600030F RID: 783 RVA: 0x000155BC File Offset: 0x000137BC
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool flag = parameters.Targets.Any<IDnlibDef>();
			if (flag)
			{
				ICompressionService service = context.Registry.GetService<ICompressionService>();
				INameService name = context.Registry.GetService<INameService>();
				IMarkerService marker = context.Registry.GetService<IMarkerService>();
				IRuntimeService service2 = context.Registry.GetService<IRuntimeService>();
				CEContext cecontext = new CEContext
				{
					Protection = (ConstantProtection)base.Parent,
					Random = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.Id),
					Context = context,
					Module = context.CurrentModule,
					Marker = marker,
					DynCipher = context.Registry.GetService<IDynCipherService>(),
					Name = name
				};
				cecontext.Mode = parameters.GetParameter<Mode>(context, context.CurrentModule, "mode", Mode.x86);
				cecontext.DecoderCount = parameters.GetParameter<int>(context, context.CurrentModule, "decoderCount", 5);
				switch (cecontext.Mode)
				{
				case Mode.Normal:
					cecontext.ModeHandler = new NormalMode();
					break;
				case Mode.Dynamic:
					cecontext.ModeHandler = new DynamicMode();
					break;
				case Mode.x86:
				{
					cecontext.ModeHandler = new x86Mode();
					bool flag2 = (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U;
					if (flag2)
					{
						context.CurrentModuleWriterOptions.Cor20HeaderOptions.Flags &= ~ComImageFlags.ILOnly;
					}
					break;
				}
				default:
					throw new UnreachableException();
				}
				MethodDef runtimeDecompressor = service.GetRuntimeDecompressor(context.CurrentModule, delegate(IDnlibDef member)
				{
					name.MarkHelper(member, marker, (Protection)this.Parent);
					bool flag3 = member is MethodDef;
					if (flag3)
					{
						ProtectionParameters.GetParameters(context, member).Remove(this.Parent);
					}
				});
				this.InjectHelpers(context, service, service2, cecontext);
				this.MutateInitializer(cecontext, runtimeDecompressor);
				MethodDef methodDef = context.CurrentModule.GlobalType.FindStaticConstructor();
				methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, cecontext.InitMethod));
				context.Annotations.Set<CEContext>(context.CurrentModule, ConstantProtection.ContextKey, cecontext);
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x000158CC File Offset: 0x00013ACC
		private void InjectHelpers(ConfuserContext context, ICompressionService compression, IRuntimeService rt, CEContext moduleCtx)
		{
			IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(rt.GetRuntimeType("Confuser.Runtime.Constant"), context.CurrentModule.GlobalType, context.CurrentModule);
			foreach (IDnlibDef dnlibDef in enumerable)
			{
				bool flag = dnlibDef.Name == "Get";
				if (flag)
				{
					context.CurrentModule.GlobalType.Remove((MethodDef)dnlibDef);
				}
				else
				{
					bool flag2 = dnlibDef.Name == "b";
					if (flag2)
					{
						moduleCtx.BufferField = (FieldDef)dnlibDef;
					}
					else
					{
						bool flag3 = dnlibDef.Name == "XorShit";
						if (flag3)
						{
							moduleCtx.XorShit = (MethodDef)dnlibDef;
						}
						else
						{
							bool flag4 = dnlibDef.Name == "Initialize";
							if (flag4)
							{
								moduleCtx.InitMethod = (MethodDef)dnlibDef;
							}
						}
					}
					moduleCtx.Name.MarkHelper(dnlibDef, moduleCtx.Marker, (Protection)base.Parent);
				}
			}
			ProtectionParameters.GetParameters(context, moduleCtx.InitMethod).Remove(base.Parent);
			TypeDefUser typeDefUser = new TypeDefUser("", moduleCtx.Name.RandomName(), context.CurrentModule.CorLibTypes.GetTypeRef("System", "ValueType"));
			typeDefUser.Layout = TypeAttributes.ExplicitLayout;
			typeDefUser.Visibility = TypeAttributes.NestedPrivate;
			typeDefUser.IsSealed = true;
			moduleCtx.DataType = typeDefUser;
			context.CurrentModule.GlobalType.NestedTypes.Add(typeDefUser);
			moduleCtx.Name.MarkHelper(typeDefUser, moduleCtx.Marker, (Protection)base.Parent);
			moduleCtx.DataField = new FieldDefUser(moduleCtx.Name.RandomName(), new FieldSig(typeDefUser.ToTypeSig(true)))
			{
				IsStatic = true,
				Access = FieldAttributes.PrivateScope
			};
			context.CurrentModule.GlobalType.Fields.Add(moduleCtx.DataField);
			moduleCtx.Name.MarkHelper(moduleCtx.DataField, moduleCtx.Marker, (Protection)base.Parent);
			MethodDef methodDef = rt.GetRuntimeType("Confuser.Runtime.Constant").FindMethod("Get");
			moduleCtx.Decoders = new List<Tuple<MethodDef, DecoderDesc>>();
			for (int i = 0; i < moduleCtx.DecoderCount; i++)
			{
				MethodDef methodDef2 = InjectHelper.Inject(methodDef, context.CurrentModule);
				for (int j = 0; j < methodDef2.Body.Instructions.Count; j++)
				{
					Instruction instruction = methodDef2.Body.Instructions[j];
					IMethod method = instruction.Operand as IMethod;
					IField field = instruction.Operand as IField;
					bool flag5 = instruction.OpCode == OpCodes.Call && method.DeclaringType.Name == "Mutation" && method.Name == "Value";
					if (flag5)
					{
						methodDef2.Body.Instructions[j] = Instruction.Create(OpCodes.Sizeof, new GenericMVar(0).ToTypeDefOrRef());
					}
					else
					{
						bool flag6 = instruction.OpCode == OpCodes.Call && method.DeclaringType.Name == "Mutation" && method.Name == "ForXor";
						if (flag6)
						{
							instruction.Operand = moduleCtx.XorShit;
						}
						else
						{
							bool flag7 = instruction.OpCode == OpCodes.Ldsfld && method.DeclaringType.Name == "Constant";
							if (flag7)
							{
								bool flag8 = field.Name == "b";
								if (!flag8)
								{
									throw new UnreachableException();
								}
								instruction.Operand = moduleCtx.BufferField;
							}
						}
					}
				}
				context.CurrentModule.GlobalType.Methods.Add(methodDef2);
				moduleCtx.Name.MarkHelper(methodDef2, moduleCtx.Marker, (Protection)base.Parent);
				ProtectionParameters.GetParameters(context, methodDef2).Remove(base.Parent);
				DecoderDesc decoderDesc = new DecoderDesc();
				decoderDesc.StringID = (moduleCtx.Random.NextByte() & 3);
				do
				{
					decoderDesc.NumberID = (moduleCtx.Random.NextByte() & 3);
				}
				while (decoderDesc.NumberID == decoderDesc.StringID);
				do
				{
					decoderDesc.InitializerID = (moduleCtx.Random.NextByte() & 3);
				}
				while (decoderDesc.InitializerID == decoderDesc.StringID || decoderDesc.InitializerID == decoderDesc.NumberID);
				MutationHelper.InjectKeys(methodDef2, new int[]
				{
					0,
					1,
					2
				}, new int[]
				{
					(int)decoderDesc.StringID,
					(int)decoderDesc.NumberID,
					(int)decoderDesc.InitializerID
				});
				decoderDesc.Data = moduleCtx.ModeHandler.CreateDecoder(methodDef2, moduleCtx);
				moduleCtx.Decoders.Add(Tuple.Create<MethodDef, DecoderDesc>(methodDef2, decoderDesc));
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00015E40 File Offset: 0x00014040
		private void MutateInitializer(CEContext moduleCtx, MethodDef decomp)
		{
			moduleCtx.InitMethod.Body.SimplifyMacros(moduleCtx.InitMethod.Parameters);
			List<Instruction> list = moduleCtx.InitMethod.Body.Instructions.ToList<Instruction>();
			for (int i = 0; i < list.Count; i++)
			{
				Instruction instruction = list[i];
				IMethod method = instruction.Operand as IMethod;
				bool flag = instruction.OpCode == OpCodes.Call;
				if (flag)
				{
					bool flag2 = method.DeclaringType.Name == "Mutation" && method.Name == "Crypt";
					if (flag2)
					{
						Instruction instruction2 = list[i - 2];
						Instruction instruction3 = list[i - 1];
						Debug.Assert(instruction2.OpCode == OpCodes.Ldloc && instruction3.OpCode == OpCodes.Ldloc);
						list.RemoveAt(i);
						list.RemoveAt(i - 1);
						list.RemoveAt(i - 2);
						list.InsertRange(i - 2, moduleCtx.ModeHandler.EmitDecrypt(moduleCtx.InitMethod, moduleCtx, (Local)instruction2.Operand, (Local)instruction3.Operand));
					}
					else
					{
						bool flag3 = method.DeclaringType.Name == "Lzma" && method.Name == "Decompress";
						if (flag3)
						{
							instruction.Operand = decomp;
						}
					}
				}
			}
			moduleCtx.InitMethod.Body.Instructions.Clear();
			foreach (Instruction item in list)
			{
				moduleCtx.InitMethod.Body.Instructions.Add(item);
			}
		}
	}
}
