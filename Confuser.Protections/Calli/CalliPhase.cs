using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Calli
{
	// Token: 0x020000D1 RID: 209
	internal class CalliPhase : ProtectionPhase
	{
		// Token: 0x06000377 RID: 887 RVA: 0x00002136 File Offset: 0x00000336
		public CalliPhase(CalliProtection parent) : base(parent)
		{
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000378 RID: 888 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Types;
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000379 RID: 889 RVA: 0x000032F5 File Offset: 0x000014F5
		public override string Name
		{
			get
			{
				return "Call to Calli Replacer";
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000189C0 File Offset: 0x00016BC0
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			CalliContext calliContext = context.Annotations.Get<CalliContext>(context.CurrentModule, CalliProtection.ContextKey, null);
			foreach (TypeDef typeDef in parameters.Targets.OfType<TypeDef>())
			{
				bool flag = typeDef.InGlobalModuleType();
				if (!flag)
				{
					foreach (MethodDef methodDef in typeDef.Methods)
					{
						bool flag2 = methodDef.InGlobalModuleType();
						if (!flag2)
						{
							bool flag3 = methodDef.FullName.Contains("My.");
							if (!flag3)
							{
								bool flag4 = !methodDef.HasBody;
								if (!flag4)
								{
									bool flag5 = !methodDef.Body.HasInstructions;
									if (!flag5)
									{
										int i = 0;
										while (i < methodDef.Body.Instructions.Count)
										{
											bool flag6 = methodDef.Body.Instructions[i].OpCode == OpCodes.Call || methodDef.Body.Instructions[i].OpCode == OpCodes.Callvirt;
											if (flag6)
											{
												try
												{
													CalliContext.CalliMode mode = calliContext.Mode;
													CalliContext.CalliMode calliMode = mode;
													if (calliMode != CalliContext.CalliMode.Normal)
													{
														if (calliMode == CalliContext.CalliMode.Ldftn)
														{
															MemberRef memberRef = (MemberRef)methodDef.Body.Instructions[i].Operand;
															methodDef.Body.Instructions[i].OpCode = OpCodes.Calli;
															methodDef.Body.Instructions[i].Operand = memberRef.MethodSig;
															methodDef.Body.Instructions.Insert(i, Instruction.Create(OpCodes.Ldftn, memberRef));
														}
													}
													else
													{
														bool flag7 = methodDef.Equals(calliContext.CalliMethod);
														if (!flag7)
														{
															MemberRef memberRef2 = (MemberRef)methodDef.Body.Instructions[i].Operand;
															bool flag8 = !CalliPhase.CanObfuscate(calliContext, memberRef2);
															if (!flag8)
															{
																methodDef.Body.Instructions[i].OpCode = OpCodes.Calli;
																methodDef.Body.Instructions[i].Operand = memberRef2.MethodSig;
																methodDef.Body.Instructions.Insert(i, Instruction.Create(OpCodes.Call, calliContext.CalliMethod));
																methodDef.Body.Instructions.Insert(i, Instruction.CreateLdcI4((int)memberRef2.MDToken.Raw));
															}
														}
													}
												}
												catch
												{
												}
											}
											IL_296:
											i++;
											continue;
											goto IL_296;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00018D08 File Offset: 0x00016F08
		private static bool CanObfuscate(CalliContext ctx, MemberRef mRef)
		{
			bool flag;
			if (!mRef.ResolveMethodDef().ReturnType.FullName.ToLower().Contains("bool"))
			{
				if (!mRef.ResolveMethodDef().ParamDefs.Any((ParamDef x) => x.IsOut) && !ctx.DontObf.Any((string x) => x.ToLower().Contains(mRef.Name.ToLower())))
				{
					flag = ctx.DontObf.Any((string x) => x.ToLower().Contains(mRef.ResolveMethodDef().ReturnType.FullName.ToLower()));
					goto IL_9D;
				}
			}
			flag = true;
			IL_9D:
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool isVirtual = mRef.ResolveMethodDef().IsVirtual;
				if (isVirtual)
				{
					bool flag3 = mRef.Name != "GetMethod";
					if (flag3)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}
	}
}
