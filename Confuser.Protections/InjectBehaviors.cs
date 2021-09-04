using System;
using Confuser.Core;
using Confuser.Core.Helpers.NewInjector;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000020 RID: 32
	public static class InjectBehaviors
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x00002475 File Offset: 0x00000675
		public static IInjectBehavior RenameAndNestBehavior(ConfuserContext context, TypeDef targetType)
		{
			return new InjectBehaviors.RenameEverythingNestedPrivateDependenciesBehavior(context, targetType);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000247E File Offset: 0x0000067E
		public static IInjectBehavior RenameAndInternalizeBehavior(ConfuserContext context)
		{
			return new InjectBehaviors.RenameEverythingInternalDependenciesBehavior(context);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00002486 File Offset: 0x00000686
		public static IInjectBehavior RenameBehavior(ConfuserContext context)
		{
			return new InjectBehaviors.RenameEverythingBehavior(context);
		}

		// Token: 0x02000021 RID: 33
		private class RenameEverythingBehavior : IInjectBehavior
		{
			// Token: 0x060000A7 RID: 167 RVA: 0x0000248E File Offset: 0x0000068E
			internal RenameEverythingBehavior(ConfuserContext context)
			{
				if (context == null)
				{
					throw new ArgumentNullException("context");
				}
				this._context = context;
				this._nameService = context.Registry.GetService<INameService>();
			}

			// Token: 0x060000A8 RID: 168 RVA: 0x00005C20 File Offset: 0x00003E20
			public virtual void Process(TypeDef source, TypeDefUser injected, Importer importer)
			{
				bool flag = source == null;
				if (flag)
				{
					throw new ArgumentNullException("source");
				}
				bool flag2 = injected == null;
				if (flag2)
				{
					throw new ArgumentNullException("injected");
				}
				this._nameService.SetOriginalNamespace(injected, injected.Namespace);
				this._nameService.SetOriginalName(injected, injected.Name);
				injected.Name = this._nameService.RandomName();
				injected.Namespace = null;
				this._nameService.SetCanRename(injected, false);
			}

			// Token: 0x060000A9 RID: 169 RVA: 0x00005CB4 File Offset: 0x00003EB4
			public virtual void Process(MethodDef source, MethodDefUser injected, Importer importer)
			{
				bool flag = source == null;
				if (flag)
				{
					throw new ArgumentNullException("source");
				}
				bool flag2 = injected == null;
				if (flag2)
				{
					throw new ArgumentNullException("injected");
				}
				this._nameService.SetOriginalName(injected, injected.Name);
				bool flag3 = !injected.IsSpecialName && !injected.DeclaringType.IsDelegate && !injected.IsOverride();
				if (flag3)
				{
					injected.Name = this._nameService.RandomName();
				}
				this._nameService.SetCanRename(injected, false);
			}

			// Token: 0x060000AA RID: 170 RVA: 0x00005D4C File Offset: 0x00003F4C
			public virtual void Process(FieldDef source, FieldDefUser injected, Importer importer)
			{
				bool flag = source == null;
				if (flag)
				{
					throw new ArgumentNullException("source");
				}
				bool flag2 = injected == null;
				if (flag2)
				{
					throw new ArgumentNullException("injected");
				}
				this._nameService.SetOriginalName(injected, injected.Name);
				bool flag3 = !injected.IsSpecialName;
				if (flag3)
				{
					injected.Name = this._nameService.RandomName();
				}
				this._nameService.SetCanRename(injected, false);
			}

			// Token: 0x060000AB RID: 171 RVA: 0x00005DCC File Offset: 0x00003FCC
			public virtual void Process(EventDef source, EventDefUser injected, Importer importer)
			{
				bool flag = source == null;
				if (flag)
				{
					throw new ArgumentNullException("source");
				}
				bool flag2 = injected == null;
				if (flag2)
				{
					throw new ArgumentNullException("injected");
				}
				this._nameService.SetOriginalName(injected, injected.Name);
				bool flag3 = !injected.IsSpecialName;
				if (flag3)
				{
					injected.Name = this._nameService.RandomName();
				}
				this._nameService.SetCanRename(injected, false);
			}

			// Token: 0x060000AC RID: 172 RVA: 0x00005E4C File Offset: 0x0000404C
			public virtual void Process(PropertyDef source, PropertyDefUser injected, Importer importer)
			{
				bool flag = source == null;
				if (flag)
				{
					throw new ArgumentNullException("source");
				}
				bool flag2 = injected == null;
				if (flag2)
				{
					throw new ArgumentNullException("injected");
				}
				this._nameService.SetOriginalName(injected, injected.Name);
				bool flag3 = !injected.IsSpecialName;
				if (flag3)
				{
					injected.Name = this._nameService.RandomName();
				}
				this._nameService.SetCanRename(injected, false);
			}

			// Token: 0x04000030 RID: 48
			private readonly ConfuserContext _context;

			// Token: 0x04000031 RID: 49
			private readonly INameService _nameService;
		}

		// Token: 0x02000022 RID: 34
		private class RenameEverythingInternalDependenciesBehavior : InjectBehaviors.RenameEverythingBehavior
		{
			// Token: 0x060000AD RID: 173 RVA: 0x000024BF File Offset: 0x000006BF
			internal RenameEverythingInternalDependenciesBehavior(ConfuserContext context) : base(context)
			{
			}

			// Token: 0x060000AE RID: 174 RVA: 0x00005ECC File Offset: 0x000040CC
			public override void Process(TypeDef source, TypeDefUser injected, Importer importer)
			{
				base.Process(source, injected, importer);
				bool isNested = injected.IsNested;
				if (isNested)
				{
					bool isNestedPublic = injected.IsNestedPublic;
					if (isNestedPublic)
					{
						injected.Visibility = TypeAttributes.NestedAssembly;
					}
				}
				else
				{
					bool isPublic = injected.IsPublic;
					if (isPublic)
					{
						injected.Visibility = TypeAttributes.NotPublic;
					}
				}
			}

			// Token: 0x060000AF RID: 175 RVA: 0x00005F18 File Offset: 0x00004118
			public override void Process(MethodDef source, MethodDefUser injected, Importer importer)
			{
				base.Process(source, injected, importer);
				bool flag = !injected.HasOverrides && injected.IsPublic && !injected.IsOverride();
				if (flag)
				{
					injected.Access = MethodAttributes.Assembly;
				}
			}

			// Token: 0x060000B0 RID: 176 RVA: 0x00005F58 File Offset: 0x00004158
			public override void Process(FieldDef source, FieldDefUser injected, Importer importer)
			{
				base.Process(source, injected, importer);
				bool isPublic = injected.IsPublic;
				if (isPublic)
				{
					injected.Access = FieldAttributes.Assembly;
				}
			}
		}

		// Token: 0x02000023 RID: 35
		private class RenameEverythingNestedPrivateDependenciesBehavior : InjectBehaviors.RenameEverythingInternalDependenciesBehavior
		{
			// Token: 0x060000B1 RID: 177 RVA: 0x000024CA File Offset: 0x000006CA
			internal RenameEverythingNestedPrivateDependenciesBehavior(ConfuserContext context, TypeDef targetType) : base(context)
			{
				if (targetType == null)
				{
					throw new ArgumentNullException("targetType");
				}
				this._targetType = targetType;
			}

			// Token: 0x060000B2 RID: 178 RVA: 0x00005F84 File Offset: 0x00004184
			public override void Process(TypeDef source, TypeDefUser injected, Importer importer)
			{
				base.Process(source, injected, importer);
				bool flag = !injected.IsNested;
				if (flag)
				{
					TypeDef typeDef = (TypeDef)importer.Import(this._targetType);
					bool flag2 = typeDef != injected;
					if (flag2)
					{
						injected.DeclaringType = typeDef;
						injected.Visibility = TypeAttributes.NestedPrivate;
					}
				}
			}

			// Token: 0x04000032 RID: 50
			private readonly TypeDef _targetType;
		}
	}
}
