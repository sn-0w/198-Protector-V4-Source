using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	// Token: 0x02000046 RID: 70
	public static class DnlibUtils
	{
		// Token: 0x0600017A RID: 378 RVA: 0x00002A63 File Offset: 0x00000C63
		public static IEnumerable<IDnlibDef> FindDefinitions(this ModuleDef module)
		{
			yield return module;
			foreach (TypeDef type in module.GetTypes())
			{
				yield return type;
				foreach (MethodDef method in type.Methods)
				{
					yield return method;
					method = null;
				}
				IEnumerator<MethodDef> enumerator2 = null;
				foreach (FieldDef field in type.Fields)
				{
					yield return field;
					field = null;
				}
				IEnumerator<FieldDef> enumerator3 = null;
				foreach (PropertyDef prop in type.Properties)
				{
					yield return prop;
					prop = null;
				}
				IEnumerator<PropertyDef> enumerator4 = null;
				foreach (EventDef evt in type.Events)
				{
					yield return evt;
					evt = null;
				}
				IEnumerator<EventDef> enumerator5 = null;
				type = null;
			}
			IEnumerator<TypeDef> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00002A73 File Offset: 0x00000C73
		public static IEnumerable<IDnlibDef> FindDefinitions(this TypeDef typeDef)
		{
			yield return typeDef;
			foreach (TypeDef nestedType in typeDef.NestedTypes)
			{
				yield return nestedType;
				nestedType = null;
			}
			IEnumerator<TypeDef> enumerator = null;
			foreach (MethodDef method in typeDef.Methods)
			{
				yield return method;
				method = null;
			}
			IEnumerator<MethodDef> enumerator2 = null;
			foreach (FieldDef field in typeDef.Fields)
			{
				yield return field;
				field = null;
			}
			IEnumerator<FieldDef> enumerator3 = null;
			foreach (PropertyDef prop in typeDef.Properties)
			{
				yield return prop;
				prop = null;
			}
			IEnumerator<PropertyDef> enumerator4 = null;
			foreach (EventDef evt in typeDef.Events)
			{
				yield return evt;
				evt = null;
			}
			IEnumerator<EventDef> enumerator5 = null;
			yield break;
			yield break;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000C9B4 File Offset: 0x0000ABB4
		public static bool IsVisibleOutside(this MethodDef methodDef)
		{
			bool flag = methodDef == null;
			if (flag)
			{
				throw new ArgumentNullException("methodDef");
			}
			MethodAttributes access = methodDef.Access;
			MethodAttributes methodAttributes = access;
			return methodAttributes - MethodAttributes.Family <= 2 && methodDef.DeclaringType.IsVisibleOutside(true, true);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000C9FC File Offset: 0x0000ABFC
		public static bool IsVisibleOutside(this FieldDef fieldDef)
		{
			bool flag = fieldDef == null;
			if (flag)
			{
				throw new ArgumentNullException("fieldDef");
			}
			FieldAttributes access = fieldDef.Access;
			FieldAttributes fieldAttributes = access;
			return fieldAttributes - FieldAttributes.Family <= 2 && fieldDef.DeclaringType.IsVisibleOutside(true, true);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000CA44 File Offset: 0x0000AC44
		public static bool IsVisibleOutside(this EventDef eventDef)
		{
			bool flag = eventDef == null;
			if (flag)
			{
				throw new ArgumentNullException("eventDef");
			}
			return eventDef.AllMethods().Any(new Func<MethodDef, bool>(DnlibUtils.IsVisibleOutside));
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000CA80 File Offset: 0x0000AC80
		public static bool IsVisibleOutside(this PropertyDef propertyDef)
		{
			bool flag = propertyDef == null;
			if (flag)
			{
				throw new ArgumentNullException("propertyDef");
			}
			return propertyDef.GetMethods.Any(new Func<MethodDef, bool>(DnlibUtils.IsVisibleOutside)) || propertyDef.SetMethods.Any(new Func<MethodDef, bool>(DnlibUtils.IsVisibleOutside));
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000CAD8 File Offset: 0x0000ACD8
		public static bool IsVisibleOutside(this TypeDef typeDef, bool exeNonPublic = true, bool hideInternals = true)
		{
			bool flag = exeNonPublic && (typeDef.Module.Kind == ModuleKind.Windows || typeDef.Module.Kind == ModuleKind.Console);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isSerializable = typeDef.IsSerializable;
				if (!isSerializable)
				{
					for (;;)
					{
						bool flag2 = typeDef.DeclaringType == null;
						if (flag2)
						{
							break;
						}
						if (hideInternals)
						{
							bool flag3 = !typeDef.IsNestedPublic && !typeDef.IsNestedFamily && !typeDef.IsNestedFamilyOrAssembly;
							if (flag3)
							{
								goto Block_10;
							}
							bool flag4 = (typeDef.IsNotPublic || typeDef.IsNestedPrivate) && !typeDef.IsNestedPublic && !typeDef.IsNestedFamily && !typeDef.IsNestedFamilyOrAssembly;
							if (flag4)
							{
								goto Block_14;
							}
						}
						typeDef = typeDef.DeclaringType;
						if (typeDef == null)
						{
							goto Block_15;
						}
					}
					return typeDef.IsPublic || !hideInternals;
					Block_10:
					return false;
					Block_14:
					return false;
					Block_15:
					throw new UnreachableException();
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000CBC8 File Offset: 0x0000ADC8
		public static bool HasAttribute(this IHasCustomAttribute obj, string fullName)
		{
			return obj.CustomAttributes.Any((CustomAttribute attr) => attr.TypeFullName == fullName);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000CC00 File Offset: 0x0000AE00
		public static bool IsComImport(this TypeDef type)
		{
			return type.IsImport || type.HasAttribute("System.Runtime.InteropServices.ComImportAttribute") || type.HasAttribute("System.Runtime.InteropServices.TypeLibTypeAttribute");
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000CC38 File Offset: 0x0000AE38
		public static bool IsCompilerGenerated(this TypeDef type)
		{
			return type.HasAttribute("System.Runtime.CompilerServices.CompilerGeneratedAttribute");
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000CC58 File Offset: 0x0000AE58
		public static bool InGlobalModuleType(this MethodDef method)
		{
			bool isGlobalModuleType = method.DeclaringType.IsGlobalModuleType;
			bool result;
			if (isGlobalModuleType)
			{
				result = true;
			}
			else
			{
				bool isGlobalModuleType2 = method.DeclaringType2.IsGlobalModuleType;
				if (isGlobalModuleType2)
				{
					result = true;
				}
				else
				{
					bool flag = method.FullName.Contains("My.");
					result = flag;
				}
			}
			return result;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000CCAC File Offset: 0x0000AEAC
		public static bool InGlobalModuleType(this TypeDef type)
		{
			bool isGlobalModuleType = type.IsGlobalModuleType;
			bool result;
			if (isGlobalModuleType)
			{
				result = true;
			}
			else
			{
				bool flag = type.DeclaringType != null;
				if (flag)
				{
					bool isGlobalModuleType2 = type.DeclaringType.IsGlobalModuleType;
					if (isGlobalModuleType2)
					{
						return true;
					}
				}
				bool flag2 = type.DeclaringType2 != null;
				if (flag2)
				{
					bool isGlobalModuleType3 = type.DeclaringType2.IsGlobalModuleType;
					if (isGlobalModuleType3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000CD14 File Offset: 0x0000AF14
		public static bool IsDelegate(this TypeDef type)
		{
			bool flag = type.BaseType == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string fullName = type.BaseType.FullName;
				result = (fullName == "System.Delegate" || fullName == "System.MulticastDelegate");
			}
			return result;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000CD60 File Offset: 0x0000AF60
		public static bool InheritsFromCorlib(this TypeDef type, string baseType)
		{
			bool flag = type.BaseType == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				TypeDef typeDef = type;
				for (;;)
				{
					typeDef = typeDef.BaseType.ResolveTypeDefThrow();
					bool flag2 = typeDef.ReflectionFullName == baseType;
					if (flag2)
					{
						break;
					}
					if (typeDef.BaseType == null || !typeDef.BaseType.DefinitionAssembly.IsCorLib())
					{
						goto Block_4;
					}
				}
				return true;
				Block_4:
				result = false;
			}
			return result;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000CDCC File Offset: 0x0000AFCC
		public static bool InheritsFrom(this TypeDef type, string baseType)
		{
			bool flag = type.BaseType == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				TypeDef typeDef = type;
				for (;;)
				{
					typeDef = typeDef.BaseType.ResolveTypeDefThrow();
					bool flag2 = typeDef.ReflectionFullName == baseType;
					if (flag2)
					{
						break;
					}
					if (typeDef.BaseType == null)
					{
						goto Block_3;
					}
				}
				return true;
				Block_3:
				result = false;
			}
			return result;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000CE24 File Offset: 0x0000B024
		public static bool Implements(this TypeDef type, string fullName)
		{
			for (;;)
			{
				foreach (InterfaceImpl interfaceImpl in type.Interfaces)
				{
					bool flag = interfaceImpl.Interface.ReflectionFullName == fullName;
					if (flag)
					{
						return true;
					}
				}
				bool flag2 = type.BaseType == null;
				if (flag2)
				{
					break;
				}
				type = type.BaseType.ResolveTypeDefThrow();
				if (type == null)
				{
					goto Block_2;
				}
			}
			return false;
			Block_2:
			throw new UnreachableException();
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000CEBC File Offset: 0x0000B0BC
		public static MethodDef ResolveThrow(this IMethod method)
		{
			MethodDef methodDef = method as MethodDef;
			bool flag = methodDef != null;
			MethodDef result;
			if (flag)
			{
				result = methodDef;
			}
			else
			{
				MethodSpec methodSpec = method as MethodSpec;
				bool flag2 = methodSpec != null;
				if (flag2)
				{
					result = methodSpec.Method.ResolveThrow();
				}
				else
				{
					result = ((MemberRef)method).ResolveMethodThrow();
				}
			}
			return result;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000CF0C File Offset: 0x0000B10C
		public static FieldDef ResolveThrow(this IField field)
		{
			FieldDef fieldDef = field as FieldDef;
			bool flag = fieldDef != null;
			FieldDef result;
			if (flag)
			{
				result = fieldDef;
			}
			else
			{
				result = ((MemberRef)field).ResolveFieldThrow();
			}
			return result;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000CF3C File Offset: 0x0000B13C
		public static ITypeDefOrRef ToBasicTypeDefOrRef(this TypeSig typeSig)
		{
			while (typeSig.Next != null)
			{
				typeSig = typeSig.Next;
			}
			bool flag = typeSig is GenericInstSig;
			ITypeDefOrRef result;
			if (flag)
			{
				result = ((GenericInstSig)typeSig).GenericType.TypeDefOrRef;
			}
			else
			{
				bool flag2 = typeSig is TypeDefOrRefSig;
				if (flag2)
				{
					result = ((TypeDefOrRefSig)typeSig).TypeDefOrRef;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000CFA4 File Offset: 0x0000B1A4
		public static IList<ITypeDefOrRef> FindTypeRefs(this TypeSig typeSig)
		{
			List<ITypeDefOrRef> list = new List<ITypeDefOrRef>();
			DnlibUtils.FindTypeRefsInternal(typeSig, list);
			return list;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000CFC8 File Offset: 0x0000B1C8
		private static void FindTypeRefsInternal(TypeSig typeSig, IList<ITypeDefOrRef> ret)
		{
			while (((typeSig != null) ? typeSig.Next : null) != null)
			{
				bool flag = typeSig is ModifierSig;
				if (flag)
				{
					ret.Add(((ModifierSig)typeSig).Modifier);
				}
				typeSig = typeSig.Next;
			}
			bool flag2 = typeSig is GenericInstSig;
			if (flag2)
			{
				GenericInstSig genericInstSig = (GenericInstSig)typeSig;
				ret.Add(genericInstSig.GenericType.TypeDefOrRef);
				foreach (TypeSig typeSig2 in genericInstSig.GenericArguments)
				{
					DnlibUtils.FindTypeRefsInternal(typeSig2, ret);
				}
			}
			else
			{
				bool flag3 = typeSig is TypeDefOrRefSig;
				if (flag3)
				{
					for (ITypeDefOrRef typeDefOrRef = ((TypeDefOrRefSig)typeSig).TypeDefOrRef; typeDefOrRef != null; typeDefOrRef = typeDefOrRef.DeclaringType)
					{
						ret.Add(typeDefOrRef);
					}
				}
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000D0C8 File Offset: 0x0000B2C8
		public static bool IsPublic(this PropertyDef property)
		{
			return property.AllMethods().Any((MethodDef method) => method.IsPublic);
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000D104 File Offset: 0x0000B304
		public static bool IsStatic(this PropertyDef property)
		{
			return property.AllMethods().Any((MethodDef method) => method.IsStatic);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000D140 File Offset: 0x0000B340
		public static bool IsPublic(this EventDef evt)
		{
			return evt.AllMethods().Any((MethodDef method) => method.IsPublic);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000D17C File Offset: 0x0000B37C
		public static bool IsStatic(this EventDef evt)
		{
			return evt.AllMethods().Any((MethodDef method) => method.IsStatic);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000D1B8 File Offset: 0x0000B3B8
		public static bool IsInterfaceImplementation(this MethodDef method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			return method.IsImplicitImplementedInterfaceMember() || method.IsExplicitlyImplementedInterfaceMember();
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000D1F0 File Offset: 0x0000B3F0
		public static bool IsImplicitImplementedInterfaceMember(this MethodDef method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			bool flag2 = method.IsPublic && method.IsNewSlot;
			if (flag2)
			{
				foreach (InterfaceImpl interfaceImpl in method.DeclaringType.Interfaces)
				{
					TypeDef typeDef = interfaceImpl.Interface.ResolveTypeDefThrow();
					bool flag3 = typeDef.FindMethod(method.Name, (MethodSig)method.Signature) != null;
					if (flag3)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000D2A4 File Offset: 0x0000B4A4
		public static bool IsExplicitlyImplementedInterfaceMember(this MethodDef method)
		{
			return method.IsFinal && method.IsPrivate;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000D2C8 File Offset: 0x0000B4C8
		public static bool IsExplicitlyImplementedInterfaceMember(this PropertyDef property)
		{
			return property.AllMethods().Any(new Func<MethodDef, bool>(DnlibUtils.IsExplicitlyImplementedInterfaceMember));
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000D2F4 File Offset: 0x0000B4F4
		public static bool IsExplicitlyImplementedInterfaceMember(this EventDef evt)
		{
			return evt.AllMethods().Any(new Func<MethodDef, bool>(DnlibUtils.IsExplicitlyImplementedInterfaceMember));
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000D320 File Offset: 0x0000B520
		public static bool IsOverride(this MethodDef method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			bool flag2 = !method.IsVirtual || method.IsPrivate;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				ITypeDefOrRef baseType = method.DeclaringType.BaseType;
				ITypeDefOrRef baseType2;
				for (TypeDef typeDef = (baseType != null) ? baseType.ResolveTypeDefThrow() : null; typeDef != null; typeDef = ((baseType2 != null) ? baseType2.ResolveTypeDefThrow() : null))
				{
					bool flag3 = typeDef.FindMethod(method.Name, method.MethodSig) != null;
					if (flag3)
					{
						return true;
					}
					baseType2 = typeDef.BaseType;
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000D3B8 File Offset: 0x0000B5B8
		public static bool IsFamilyOrAssembly(this PropertyDef property)
		{
			return property.AllMethods().Any((MethodDef method) => method.IsFamilyOrAssembly);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000D3F4 File Offset: 0x0000B5F4
		public static bool IsFamily(this PropertyDef property)
		{
			return property.AllMethods().Any((MethodDef method) => method.IsFamily);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000D430 File Offset: 0x0000B630
		public static bool HasAllPrivateFlags(this PropertyDef property)
		{
			return property.AllMethods().All((MethodDef method) => method.HasPrivateFlags());
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000D46C File Offset: 0x0000B66C
		public static bool IsFamily(this EventDef evt)
		{
			return evt.AllMethods().Any((MethodDef method) => method.IsFamily);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000D4A8 File Offset: 0x0000B6A8
		public static bool IsFamilyOrAssembly(this EventDef evt)
		{
			return evt.AllMethods().Any((MethodDef method) => method.IsFamilyOrAssembly);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000D4E4 File Offset: 0x0000B6E4
		public static bool HasAllPrivateFlags(this EventDef evt)
		{
			return evt.AllMethods().All((MethodDef method) => method.HasPrivateFlags());
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000D520 File Offset: 0x0000B720
		public static bool HasPrivateFlags(this MethodDef method)
		{
			return method.IsPrivate || method.IsPrivateScope || method.IsCompilerControlled;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000D54C File Offset: 0x0000B74C
		public static bool HasPrivateFlags(this FieldDef field)
		{
			return field.IsPrivate || field.IsPrivateScope || field.IsCompilerControlled;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000D578 File Offset: 0x0000B778
		private static IEnumerable<MethodDef> AllMethods(this EventDef evt)
		{
			return from m in new MethodDef[]
			{
				evt.AddMethod,
				evt.RemoveMethod,
				evt.InvokeMethod
			}.Concat(evt.OtherMethods)
			where m != null
			select m;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000D5DC File Offset: 0x0000B7DC
		private static IEnumerable<MethodDef> AllMethods(this PropertyDef property)
		{
			return from m in new MethodDef[]
			{
				property.GetMethod,
				property.SetMethod
			}.Concat(property.OtherMethods)
			where m != null
			select m;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000D638 File Offset: 0x0000B838
		public static void ReplaceReference(this CilBody body, Instruction target, Instruction newInstr)
		{
			foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
			{
				bool flag = exceptionHandler.TryStart == target;
				if (flag)
				{
					exceptionHandler.TryStart = newInstr;
				}
				bool flag2 = exceptionHandler.TryEnd == target;
				if (flag2)
				{
					exceptionHandler.TryEnd = newInstr;
				}
				bool flag3 = exceptionHandler.HandlerStart == target;
				if (flag3)
				{
					exceptionHandler.HandlerStart = newInstr;
				}
				bool flag4 = exceptionHandler.HandlerEnd == target;
				if (flag4)
				{
					exceptionHandler.HandlerEnd = newInstr;
				}
			}
			foreach (Instruction instruction in body.Instructions)
			{
				bool flag5 = instruction.Operand == target;
				if (flag5)
				{
					instruction.Operand = newInstr;
				}
				else
				{
					bool flag6 = instruction.Operand is Instruction[];
					if (flag6)
					{
						Instruction[] array = (Instruction[])instruction.Operand;
						for (int i = 0; i < array.Length; i++)
						{
							bool flag7 = array[i] == target;
							if (flag7)
							{
								array[i] = newInstr;
							}
						}
					}
				}
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000D784 File Offset: 0x0000B984
		public static bool IsArrayAccessors(this IMethod method)
		{
			TypeSig typeSig = method.DeclaringType.ToTypeSig(true);
			bool flag = typeSig is GenericInstSig;
			if (flag)
			{
				typeSig = ((GenericInstSig)typeSig).GenericType;
			}
			bool isArray = typeSig.IsArray;
			return isArray && (method.Name == "Get" || method.Name == "Set" || method.Name == "Address");
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000D804 File Offset: 0x0000BA04
		public static void AddBeforeReloc(this List<PESection> sections, PESection newSection)
		{
			bool flag = sections == null;
			if (flag)
			{
				throw new ArgumentNullException("sections");
			}
			sections.InsertBeforeReloc(sections.Count, newSection);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000D834 File Offset: 0x0000BA34
		public static void InsertBeforeReloc(this List<PESection> sections, int preferredIndex, PESection newSection)
		{
			bool flag = sections == null;
			if (flag)
			{
				throw new ArgumentNullException("sections");
			}
			bool flag2 = preferredIndex < 0 || preferredIndex > sections.Count;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("preferredIndex", preferredIndex, "Preferred index is out of range.");
			}
			bool flag3 = newSection == null;
			if (flag3)
			{
				throw new ArgumentNullException("newSection");
			}
			int num = sections.FindIndex(0, Math.Min(preferredIndex + 1, sections.Count), new Predicate<PESection>(DnlibUtils.IsRelocSection));
			bool flag4 = num == -1;
			if (flag4)
			{
				sections.Insert(preferredIndex, newSection);
			}
			else
			{
				sections.Insert(num, newSection);
			}
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00002A83 File Offset: 0x00000C83
		public static bool IsRelocSection(PESection section)
		{
			return section.Name.Equals(".reloc", StringComparison.Ordinal);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000D8D4 File Offset: 0x0000BAD4
		public static bool IsEntryPoint(this MethodDef methodDef)
		{
			bool flag = methodDef == null;
			if (flag)
			{
				throw new ArgumentNullException("methodDef");
			}
			return methodDef == methodDef.Module.EntryPoint;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000D908 File Offset: 0x0000BB08
		public static bool IsEntryPoint(this TypeDef typeDef)
		{
			bool flag = typeDef == null;
			if (flag)
			{
				throw new ArgumentNullException("typeDef");
			}
			MethodDef entryPoint = typeDef.Module.EntryPoint;
			return typeDef == ((entryPoint != null) ? entryPoint.DeclaringType : null);
		}

		// Token: 0x02000047 RID: 71
		public class RawHeap : HeapBase
		{
			// Token: 0x060001AA RID: 426 RVA: 0x00002A96 File Offset: 0x00000C96
			public RawHeap(string name, byte[] content)
			{
				this.name = name;
				this.content = content;
			}

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x060001AB RID: 427 RVA: 0x0000D948 File Offset: 0x0000BB48
			public override string Name
			{
				get
				{
					return this.name;
				}
			}

			// Token: 0x060001AC RID: 428 RVA: 0x0000D960 File Offset: 0x0000BB60
			public override uint GetRawLength()
			{
				return (uint)this.content.Length;
			}

			// Token: 0x060001AD RID: 429 RVA: 0x00002AAE File Offset: 0x00000CAE
			protected override void WriteToImpl(DataWriter writer)
			{
				writer.WriteBytes(this.content);
			}

			// Token: 0x0400015E RID: 350
			private readonly byte[] content;

			// Token: 0x0400015F RID: 351
			private readonly string name;
		}
	}
}
