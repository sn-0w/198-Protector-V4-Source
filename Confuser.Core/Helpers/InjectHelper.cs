using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	// Token: 0x020000AB RID: 171
	public static class InjectHelper
	{
		// Token: 0x060003F1 RID: 1009 RVA: 0x000172C4 File Offset: 0x000154C4
		private static TypeDefUser Clone(TypeDef origin)
		{
			TypeDefUser typeDefUser = new TypeDefUser(origin.Namespace, origin.Name);
			typeDefUser.Attributes = origin.Attributes;
			bool flag = origin.ClassLayout != null;
			if (flag)
			{
				typeDefUser.ClassLayout = new ClassLayoutUser(origin.ClassLayout.PackingSize, origin.ClassSize);
			}
			foreach (GenericParam genericParam in origin.GenericParameters)
			{
				typeDefUser.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));
			}
			return typeDefUser;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00017384 File Offset: 0x00015584
		private static MethodDefUser Clone(MethodDef origin)
		{
			MethodDefUser methodDefUser = new MethodDefUser(origin.Name, null, origin.ImplAttributes, origin.Attributes);
			foreach (GenericParam genericParam in origin.GenericParameters)
			{
				methodDefUser.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));
			}
			return methodDefUser;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00017414 File Offset: 0x00015614
		private static FieldDefUser Clone(FieldDef origin)
		{
			return new FieldDefUser(origin.Name, null, origin.Attributes);
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0001743C File Offset: 0x0001563C
		private static TypeDef PopulateContext(TypeDef typeDef, InjectHelper.InjectContext ctx)
		{
			IDnlibDef dnlibDef;
			bool flag = !ctx.map.TryGetValue(typeDef, out dnlibDef);
			TypeDef typeDef2;
			if (flag)
			{
				typeDef2 = InjectHelper.Clone(typeDef);
				ctx.map[typeDef] = typeDef2;
			}
			else
			{
				typeDef2 = (TypeDef)dnlibDef;
			}
			foreach (TypeDef typeDef3 in typeDef.NestedTypes)
			{
				typeDef2.NestedTypes.Add(InjectHelper.PopulateContext(typeDef3, ctx));
			}
			foreach (MethodDef methodDef in typeDef.Methods)
			{
				typeDef2.Methods.Add((MethodDef)(ctx.map[methodDef] = InjectHelper.Clone(methodDef)));
			}
			foreach (FieldDef fieldDef in typeDef.Fields)
			{
				typeDef2.Fields.Add((FieldDef)(ctx.map[fieldDef] = InjectHelper.Clone(fieldDef)));
			}
			return typeDef2;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x000175A8 File Offset: 0x000157A8
		private static void CopyTypeDef(TypeDef typeDef, InjectHelper.InjectContext ctx)
		{
			TypeDef typeDef2 = (TypeDef)ctx.map[typeDef];
			typeDef2.BaseType = ctx.Importer.Import(typeDef.BaseType);
			foreach (InterfaceImpl interfaceImpl in typeDef.Interfaces)
			{
				typeDef2.Interfaces.Add(new InterfaceImplUser(ctx.Importer.Import(interfaceImpl.Interface)));
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00017644 File Offset: 0x00015844
		private static void CopyMethodDef(MethodDef methodDef, InjectHelper.InjectContext ctx)
		{
			MethodDef methodDef2 = (MethodDef)ctx.map[methodDef];
			methodDef2.Signature = ctx.Importer.Import(methodDef.Signature);
			methodDef2.Parameters.UpdateParameterTypes();
			bool flag = methodDef.ImplMap != null;
			if (flag)
			{
				methodDef2.ImplMap = new ImplMapUser(new ModuleRefUser(ctx.TargetModule, methodDef.ImplMap.Module.Name), methodDef.ImplMap.Name, methodDef.ImplMap.Attributes);
			}
			foreach (CustomAttribute customAttribute in methodDef.CustomAttributes)
			{
				methodDef2.CustomAttributes.Add(new CustomAttribute((ICustomAttributeType)ctx.Importer.Import(customAttribute.Constructor)));
			}
			bool hasBody = methodDef.HasBody;
			if (hasBody)
			{
				methodDef2.Body = new CilBody(methodDef.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>());
				methodDef2.Body.MaxStack = methodDef.Body.MaxStack;
				Dictionary<object, object> bodyMap = new Dictionary<object, object>();
				foreach (Local local in methodDef.Body.Variables)
				{
					Local local2 = new Local(ctx.Importer.Import(local.Type));
					methodDef2.Body.Variables.Add(local2);
					local2.Name = local.Name;
					bodyMap[local] = local2;
				}
				foreach (Instruction instruction in methodDef.Body.Instructions)
				{
					Instruction instruction2 = new Instruction(instruction.OpCode, instruction.Operand);
					instruction2.SequencePoint = instruction.SequencePoint;
					bool flag2 = instruction2.Operand is IType;
					if (flag2)
					{
						instruction2.Operand = ctx.Importer.Import((IType)instruction2.Operand);
					}
					else
					{
						bool flag3 = instruction2.Operand is IMethod;
						if (flag3)
						{
							instruction2.Operand = ctx.Importer.Import((IMethod)instruction2.Operand);
						}
						else
						{
							bool flag4 = instruction2.Operand is IField;
							if (flag4)
							{
								instruction2.Operand = ctx.Importer.Import((IField)instruction2.Operand);
							}
						}
					}
					methodDef2.Body.Instructions.Add(instruction2);
					bodyMap[instruction] = instruction2;
				}
				Func<Instruction, Instruction> <>9__0;
				foreach (Instruction instruction3 in methodDef2.Body.Instructions)
				{
					bool flag5 = instruction3.Operand != null && bodyMap.ContainsKey(instruction3.Operand);
					if (flag5)
					{
						instruction3.Operand = bodyMap[instruction3.Operand];
					}
					else
					{
						bool flag6 = instruction3.Operand is Instruction[];
						if (flag6)
						{
							Instruction instruction4 = instruction3;
							IEnumerable<Instruction> source = (Instruction[])instruction3.Operand;
							Func<Instruction, Instruction> selector;
							if ((selector = <>9__0) == null)
							{
								selector = (<>9__0 = ((Instruction target) => (Instruction)bodyMap[target]));
							}
							instruction4.Operand = source.Select(selector).ToArray<Instruction>();
						}
					}
				}
				foreach (ExceptionHandler exceptionHandler in methodDef.Body.ExceptionHandlers)
				{
					methodDef2.Body.ExceptionHandlers.Add(new ExceptionHandler(exceptionHandler.HandlerType)
					{
						CatchType = ((exceptionHandler.CatchType == null) ? null : ctx.Importer.Import(exceptionHandler.CatchType)),
						TryStart = (Instruction)bodyMap[exceptionHandler.TryStart],
						TryEnd = (Instruction)bodyMap[exceptionHandler.TryEnd],
						HandlerStart = (Instruction)bodyMap[exceptionHandler.HandlerStart],
						HandlerEnd = (Instruction)bodyMap[exceptionHandler.HandlerEnd],
						FilterStart = ((exceptionHandler.FilterStart == null) ? null : ((Instruction)bodyMap[exceptionHandler.FilterStart]))
					});
				}
				methodDef2.Body.SimplifyMacros(methodDef2.Parameters);
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00017BCC File Offset: 0x00015DCC
		private static void CopyFieldDef(FieldDef fieldDef, InjectHelper.InjectContext ctx)
		{
			FieldDef fieldDef2 = (FieldDef)ctx.map[fieldDef];
			fieldDef2.Signature = ctx.Importer.Import(fieldDef.Signature);
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00017C08 File Offset: 0x00015E08
		private static void Copy(TypeDef typeDef, InjectHelper.InjectContext ctx, bool copySelf)
		{
			if (copySelf)
			{
				InjectHelper.CopyTypeDef(typeDef, ctx);
			}
			foreach (TypeDef typeDef2 in typeDef.NestedTypes)
			{
				InjectHelper.Copy(typeDef2, ctx, true);
			}
			foreach (MethodDef methodDef in typeDef.Methods)
			{
				InjectHelper.CopyMethodDef(methodDef, ctx);
			}
			foreach (FieldDef fieldDef in typeDef.Fields)
			{
				InjectHelper.CopyFieldDef(fieldDef, ctx);
			}
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00017CF0 File Offset: 0x00015EF0
		public static TypeDef Inject(TypeDef typeDef, ModuleDef target)
		{
			InjectHelper.InjectContext injectContext = new InjectHelper.InjectContext(typeDef.Module, target);
			InjectHelper.PopulateContext(typeDef, injectContext);
			InjectHelper.Copy(typeDef, injectContext, true);
			return (TypeDef)injectContext.map[typeDef];
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00017D34 File Offset: 0x00015F34
		public static MethodDef Inject(MethodDef methodDef, ModuleDef target)
		{
			InjectHelper.InjectContext injectContext = new InjectHelper.InjectContext(methodDef.Module, target);
			injectContext.map[methodDef] = InjectHelper.Clone(methodDef);
			InjectHelper.CopyMethodDef(methodDef, injectContext);
			return (MethodDef)injectContext.map[methodDef];
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00017D80 File Offset: 0x00015F80
		public static IEnumerable<IDnlibDef> Inject(TypeDef typeDef, TypeDef newType, ModuleDef target)
		{
			InjectHelper.InjectContext injectContext = new InjectHelper.InjectContext(typeDef.Module, target);
			injectContext.map[typeDef] = newType;
			InjectHelper.PopulateContext(typeDef, injectContext);
			InjectHelper.Copy(typeDef, injectContext, false);
			return injectContext.map.Values.Except(new TypeDef[]
			{
				newType
			});
		}

		// Token: 0x020000AC RID: 172
		private class InjectContext : ImportMapper
		{
			// Token: 0x060003FC RID: 1020 RVA: 0x00017DD8 File Offset: 0x00015FD8
			public InjectContext(ModuleDef module, ModuleDef target)
			{
				this.OriginModule = module;
				this.TargetModule = target;
				this.importer = new Importer(target, ImporterOptions.TryToUseTypeDefs, default(GenericParamContext), this);
			}

			// Token: 0x1700008E RID: 142
			// (get) Token: 0x060003FD RID: 1021 RVA: 0x00017E20 File Offset: 0x00016020
			public Importer Importer
			{
				get
				{
					return this.importer;
				}
			}

			// Token: 0x060003FE RID: 1022 RVA: 0x00017E38 File Offset: 0x00016038
			public override ITypeDefOrRef Map(ITypeDefOrRef typeDefOrRef)
			{
				TypeDef typeDef = typeDefOrRef as TypeDef;
				bool flag = typeDef != null;
				if (flag)
				{
					bool flag2 = this.map.ContainsKey(typeDef);
					if (flag2)
					{
						return (TypeDef)this.map[typeDef];
					}
				}
				return null;
			}

			// Token: 0x060003FF RID: 1023 RVA: 0x00017E80 File Offset: 0x00016080
			public override IMethod Map(MethodDef methodDef)
			{
				bool flag = this.map.ContainsKey(methodDef);
				IMethod result;
				if (flag)
				{
					result = (MethodDef)this.map[methodDef];
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x06000400 RID: 1024 RVA: 0x00017EB8 File Offset: 0x000160B8
			public override IField Map(FieldDef fieldDef)
			{
				bool flag = this.map.ContainsKey(fieldDef);
				IField result;
				if (flag)
				{
					result = (FieldDef)this.map[fieldDef];
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x04000294 RID: 660
			public readonly Dictionary<IDnlibDef, IDnlibDef> map = new Dictionary<IDnlibDef, IDnlibDef>();

			// Token: 0x04000295 RID: 661
			public readonly ModuleDef OriginModule;

			// Token: 0x04000296 RID: 662
			public readonly ModuleDef TargetModule;

			// Token: 0x04000297 RID: 663
			private readonly Importer importer;
		}
	}
}
