using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using Confuser.Protections.TypeScramble.Scrambler.Analyzers;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler
{
	// Token: 0x0200003A RID: 58
	internal sealed class ScannedMethod : ScannedItem
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600014E RID: 334 RVA: 0x0000283F File Offset: 0x00000A3F
		internal MethodDef TargetMethod { get; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00002847 File Offset: 0x00000A47
		private ContextAnalyzerFactory Analyzers { get; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000150 RID: 336 RVA: 0x0000284F File Offset: 0x00000A4F
		private bool ScramblePublicMethods { get; }

		// Token: 0x06000151 RID: 337 RVA: 0x00007218 File Offset: 0x00005418
		internal ScannedMethod(TypeService service, MethodDef target, bool scramblePublic) : base(target)
		{
			Debug.Assert(service != null, "service != null");
			Debug.Assert(target != null, "target != null");
			this.TargetMethod = target;
			this.ScramblePublicMethods = scramblePublic;
			this.Analyzers = new ContextAnalyzerFactory(this)
			{
				new MemberRefAnalyzer(),
				new TypeRefAnalyzer(),
				new MethodSpecAnalyzer(),
				new MethodDefAnalyzer(service)
			};
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007298 File Offset: 0x00005498
		internal override void Scan()
		{
			bool flag = !ScannedMethod.CanScrambleMethod(this.TargetMethod, this.ScramblePublicMethods);
			if (!flag)
			{
				bool hasBody = this.TargetMethod.HasBody;
				if (hasBody)
				{
					foreach (Local local in this.TargetMethod.Body.Variables)
					{
						base.RegisterGeneric(local.Type);
					}
				}
				bool flag2 = this.TargetMethod.ReturnType != this.TargetMethod.Module.CorLibTypes.Void;
				if (flag2)
				{
					base.RegisterGeneric(this.TargetMethod.ReturnType);
				}
				foreach (Parameter parameter in this.TargetMethod.Parameters.Where(new Func<Parameter, bool>(ScannedMethod.ProcessParameter)))
				{
					base.RegisterGeneric(parameter.Type);
				}
				bool hasBody2 = this.TargetMethod.HasBody;
				if (hasBody2)
				{
					foreach (Instruction instruction in this.TargetMethod.Body.Instructions)
					{
						bool flag3 = instruction.Operand != null;
						if (flag3)
						{
							this.Analyzers.Analyze(instruction);
						}
					}
				}
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007444 File Offset: 0x00005644
		private static bool CanScrambleMethod(MethodDef method, bool scramblePublic)
		{
			Debug.Assert(method != null, "method != null");
			bool flag = !method.IsManaged;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = method.IsEntryPoint();
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = method.HasOverrides || method.IsAbstract;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = method.IsConstructor || method.IsGetter || method.IsSetter;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool hasGenericParameters = method.DeclaringType.HasGenericParameters;
							if (hasGenericParameters)
							{
								result = false;
							}
							else
							{
								bool flag5 = method.DeclaringType.FindMethods(method.Name).Take(2).Count<MethodDef>() > 1;
								if (flag5)
								{
									result = false;
								}
								else
								{
									bool flag6 = method.IsInterfaceImplementation();
									if (flag6)
									{
										result = false;
									}
									else
									{
										bool isDelegate = method.DeclaringType.IsDelegate;
										if (isDelegate)
										{
											result = false;
										}
										else
										{
											bool flag7 = method.DeclaringType.IsComImport();
											if (flag7)
											{
												result = false;
											}
											else
											{
												bool flag8 = !scramblePublic && method.IsVisibleOutside();
												result = !flag8;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000755C File Offset: 0x0000575C
		private static bool ProcessParameter(Parameter parameter)
		{
			Debug.Assert(parameter != null, "parameter != null");
			bool flag = !parameter.IsNormalMethodParameter;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ParamDef paramDef = parameter.ParamDef;
				bool flag2 = paramDef != null && paramDef.IsOut;
				result = !flag2;
			}
			return result;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000075AC File Offset: 0x000057AC
		protected override void PrepareGenerics(IEnumerable<GenericParam> scrambleParams)
		{
			Debug.Assert(scrambleParams != null, "scrambleParams != null");
			bool flag = !base.IsScambled;
			if (!flag)
			{
				this.TargetMethod.GenericParameters.Clear();
				foreach (GenericParam item in scrambleParams)
				{
					this.TargetMethod.GenericParameters.Add(item);
				}
				this.TargetMethod.MethodSig.GenParamCount = (uint)((ushort)this.TargetMethod.GenericParameters.Count);
				bool hasBody = this.TargetMethod.HasBody;
				if (hasBody)
				{
					foreach (Local local in this.TargetMethod.Body.Variables)
					{
						local.Type = base.ConvertToGenericIfAvalible(local.Type);
					}
				}
				foreach (Parameter parameter in this.TargetMethod.Parameters.Where(new Func<Parameter, bool>(ScannedMethod.ProcessParameter)))
				{
					parameter.Type = base.ConvertToGenericIfAvalible(parameter.Type);
					Debug.Assert(parameter.Type == this.TargetMethod.MethodSig.Params[parameter.MethodSigIndex], "parameter.Type == TargetMethod.MethodSig.Params[parameter.MethodSigIndex]");
				}
				bool flag2 = this.TargetMethod.ReturnType != this.TargetMethod.Module.CorLibTypes.Void;
				if (flag2)
				{
					this.TargetMethod.ReturnType = base.ConvertToGenericIfAvalible(this.TargetMethod.ReturnType);
				}
				Debug.Assert(this.TargetMethod.ReturnType == this.TargetMethod.MethodSig.RetType, "TargetMethod.ReturnType == TargetMethod.MethodSig.RetType");
				this.TargetMethod.Signature.Generic = true;
				Debug.Assert(this.TargetMethod.Signature.Generic, "(TargetMethod.Signature.Generic");
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00007800 File Offset: 0x00005A00
		internal GenericInstMethodSig CreateGenericMethodSig(ScannedMethod from, GenericInstMethodSig original = null)
		{
			List<TypeSig> list = new List<TypeSig>(base.TrueTypes.Count);
			foreach (TypeSig typeSig in base.TrueTypes)
			{
				bool isGenericMethodParameter = typeSig.IsGenericMethodParameter;
				if (isGenericMethodParameter)
				{
					Debug.Assert(original != null, "original != null");
					uint number = ((GenericSig)typeSig).Number;
					Debug.Assert((ulong)number < (ulong)((long)original.GenericArguments.Count), "number < original.GenericArguments.Count");
					TypeSig item = original.GenericArguments[(int)number];
					list.Add(item);
				}
				else
				{
					bool flag = from != null && from.IsScambled;
					if (flag)
					{
						list.Add(from.ConvertToGenericIfAvalible(typeSig));
					}
					else
					{
						list.Add(typeSig);
					}
				}
			}
			return new GenericInstMethodSig(list);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00002857 File Offset: 0x00000A57
		internal override IMemberDef GetMemberDef()
		{
			return this.TargetMethod;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000285F File Offset: 0x00000A5F
		internal override ClassOrValueTypeSig GetTarget()
		{
			return this.TargetMethod.DeclaringType.TryGetClassOrValueTypeSig();
		}
	}
}
