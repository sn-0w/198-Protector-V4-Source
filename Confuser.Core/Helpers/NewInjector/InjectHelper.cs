using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000B0 RID: 176
	public static class InjectHelper
	{
		// Token: 0x0600040C RID: 1036 RVA: 0x00018300 File Offset: 0x00016500
		private static InjectHelper.InjectContext GetOrCreateContext(ModuleDef sourceModule, ModuleDef targetModule)
		{
			ValueTuple<ModuleDef, ModuleDef> valueTuple = new ValueTuple<ModuleDef, ModuleDef>(sourceModule, targetModule);
			InjectHelper.InjectContext injectContext;
			bool flag = !InjectHelper._contextMap.TryGetValue(valueTuple, out injectContext);
			if (flag)
			{
				bool flag2 = !InjectHelper._parentMaps.Any<IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext>>() || !InjectHelper._parentMaps.Peek().TryGetValue(valueTuple, out injectContext);
				if (flag2)
				{
					injectContext = new InjectHelper.InjectContext(sourceModule, targetModule);
				}
				else
				{
					injectContext = new InjectHelper.InjectContext(injectContext);
				}
				InjectHelper._contextMap = InjectHelper._contextMap.Add(valueTuple, injectContext);
			}
			return injectContext;
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00018384 File Offset: 0x00016584
		public static IDisposable CreateChildContext()
		{
			IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext> immutableDictionary = InjectHelper._contextMap;
			bool flag = InjectHelper._parentMaps.Any<IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext>>();
			if (flag)
			{
				IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext> immutableDictionary2 = InjectHelper._parentMaps.Peek();
				bool flag2 = immutableDictionary.Any<KeyValuePair<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext>>();
				if (flag2)
				{
					foreach (KeyValuePair<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext> keyValuePair in immutableDictionary2)
					{
						bool flag3 = !immutableDictionary.ContainsKey(keyValuePair.Key);
						if (flag3)
						{
							immutableDictionary = immutableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
				else
				{
					immutableDictionary = immutableDictionary2;
				}
			}
			InjectHelper._parentMaps.Push(immutableDictionary);
			InjectHelper._contextMap = InjectHelper._contextMap.Clear();
			return new InjectHelper.ChildContextRelease(new Action(InjectHelper.ReleaseChildContext));
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00018460 File Offset: 0x00016660
		private static void ReleaseChildContext()
		{
			bool flag = !InjectHelper._parentMaps.Any<IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext>>();
			if (flag)
			{
				throw new InvalidOperationException("There is not child context to release. Disposed twice?!");
			}
			InjectHelper._contextMap = InjectHelper._parentMaps.Pop();
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x0001849C File Offset: 0x0001669C
		public static InjectResult<MethodDef> Inject(MethodDef methodDef, ModuleDef target, IInjectBehavior behavior, params IMethodInjectProcessor[] methodInjectProcessors)
		{
			bool flag = methodDef == null;
			if (flag)
			{
				throw new ArgumentNullException("methodDef");
			}
			bool flag2 = target == null;
			if (flag2)
			{
				throw new ArgumentNullException("target");
			}
			bool flag3 = behavior == null;
			if (flag3)
			{
				throw new ArgumentNullException("behavior");
			}
			bool flag4 = methodInjectProcessors == null;
			if (flag4)
			{
				throw new ArgumentNullException("methodInjectProcessors");
			}
			InjectHelper.InjectContext orCreateContext = InjectHelper.GetOrCreateContext(methodDef.Module, target);
			orCreateContext.ApplyMapping(methodDef.DeclaringType, target.GlobalType);
			InjectHelper.Injector injector = new InjectHelper.Injector(orCreateContext, behavior, methodInjectProcessors);
			MethodDef mappedMethod = injector.Inject(methodDef);
			return InjectResult.Create<MethodDef>(methodDef, mappedMethod, from m in injector.InjectedMembers
			where m.Value != mappedMethod
			select m);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00018564 File Offset: 0x00016764
		public static InjectResult<TypeDef> Inject(TypeDef typeDef, ModuleDef target, IInjectBehavior behavior, params IMethodInjectProcessor[] methodInjectProcessors)
		{
			bool flag = typeDef == null;
			if (flag)
			{
				throw new ArgumentNullException("typeDef");
			}
			bool flag2 = target == null;
			if (flag2)
			{
				throw new ArgumentNullException("target");
			}
			bool flag3 = behavior == null;
			if (flag3)
			{
				throw new ArgumentNullException("behavior");
			}
			bool flag4 = methodInjectProcessors == null;
			if (flag4)
			{
				throw new ArgumentNullException("methodInjectProcessors");
			}
			InjectHelper.InjectContext orCreateContext = InjectHelper.GetOrCreateContext(typeDef.Module, target);
			InjectHelper.Injector injector = new InjectHelper.Injector(orCreateContext, behavior, methodInjectProcessors);
			TypeDef mappedType = injector.Inject(typeDef);
			return InjectResult.Create<TypeDef>(typeDef, mappedType, from m in injector.InjectedMembers
			where m.Value != mappedType
			select m);
		}

		// Token: 0x0400029C RID: 668
		[TupleElementNames(new string[]
		{
			"SourceModule",
			"TargetModule"
		})]
		private static Stack<IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext>> _parentMaps = new Stack<IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext>>();

		// Token: 0x0400029D RID: 669
		[TupleElementNames(new string[]
		{
			"SourceModule",
			"TargetModule"
		})]
		private static IImmutableDictionary<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext> _contextMap = ImmutableDictionary.Create<ValueTuple<ModuleDef, ModuleDef>, InjectHelper.InjectContext>();

		// Token: 0x020000B1 RID: 177
		private sealed class ChildContextRelease : IDisposable
		{
			// Token: 0x06000412 RID: 1042 RVA: 0x00003A30 File Offset: 0x00001C30
			internal ChildContextRelease(Action releaseAction)
			{
				this._releaseAction = releaseAction;
			}

			// Token: 0x06000413 RID: 1043 RVA: 0x00018618 File Offset: 0x00016818
			private void Dispose(bool disposing)
			{
				bool flag = !this._disposed;
				if (flag)
				{
					if (disposing)
					{
						this._releaseAction();
					}
					this._disposed = true;
				}
			}

			// Token: 0x06000414 RID: 1044 RVA: 0x00003A48 File Offset: 0x00001C48
			void IDisposable.Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Token: 0x0400029E RID: 670
			private readonly Action _releaseAction;

			// Token: 0x0400029F RID: 671
			private bool _disposed = false;
		}

		// Token: 0x020000B2 RID: 178
		private sealed class InjectContext
		{
			// Token: 0x1700008F RID: 143
			// (get) Token: 0x06000415 RID: 1045 RVA: 0x00003A5A File Offset: 0x00001C5A
			internal ModuleDef OriginModule { get; }

			// Token: 0x17000090 RID: 144
			// (get) Token: 0x06000416 RID: 1046 RVA: 0x00003A62 File Offset: 0x00001C62
			internal ModuleDef TargetModule { get; }

			// Token: 0x06000417 RID: 1047 RVA: 0x00003A6A File Offset: 0x00001C6A
			internal InjectContext(InjectHelper.InjectContext parentContext)
			{
				this.OriginModule = parentContext.OriginModule;
				this.TargetModule = parentContext.TargetModule;
				this._map = parentContext._map;
			}

			// Token: 0x06000418 RID: 1048 RVA: 0x00018650 File Offset: 0x00016850
			internal InjectContext(ModuleDef module, ModuleDef target)
			{
				if (module == null)
				{
					throw new ArgumentNullException("module");
				}
				this.OriginModule = module;
				if (target == null)
				{
					throw new ArgumentNullException("target");
				}
				this.TargetModule = target;
				this._map = ImmutableDictionary.Create<IMemberDef, IMemberDef>();
			}

			// Token: 0x06000419 RID: 1049 RVA: 0x0001869C File Offset: 0x0001689C
			internal void ApplyMapping(IMemberDef source, IMemberDef target)
			{
				bool flag = source == null;
				if (flag)
				{
					throw new ArgumentNullException("source");
				}
				bool flag2 = target == null;
				if (flag2)
				{
					throw new ArgumentNullException("target");
				}
				this._map = this._map.SetItem(source, target);
			}

			// Token: 0x0600041A RID: 1050 RVA: 0x000186E4 File Offset: 0x000168E4
			internal TDef ResolveMapped<TDef>(TDef def) where TDef : class, IMemberDef
			{
				IMemberDef memberDef;
				TDef tdef;
				bool flag;
				if (this._map.TryGetValue(def, out memberDef))
				{
					tdef = (memberDef as TDef);
					flag = (tdef != null);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				TDef result;
				if (flag2)
				{
					result = tdef;
				}
				else
				{
					result = default(TDef);
				}
				return result;
			}

			// Token: 0x040002A0 RID: 672
			private IImmutableDictionary<IMemberDef, IMemberDef> _map;
		}

		// Token: 0x020000B3 RID: 179
		private sealed class Injector : ImportMapper
		{
			// Token: 0x17000091 RID: 145
			// (get) Token: 0x0600041B RID: 1051 RVA: 0x00003A98 File Offset: 0x00001C98
			private InjectHelper.InjectContext InjectContext { get; }

			// Token: 0x17000092 RID: 146
			// (get) Token: 0x0600041C RID: 1052 RVA: 0x00003AA0 File Offset: 0x00001CA0
			private IInjectBehavior InjectBehavior { get; }

			// Token: 0x17000093 RID: 147
			// (get) Token: 0x0600041D RID: 1053 RVA: 0x00003AA8 File Offset: 0x00001CA8
			internal IReadOnlyDictionary<IMemberDef, IMemberDef> InjectedMembers
			{
				get
				{
					return this._injectedMembers;
				}
			}

			// Token: 0x17000094 RID: 148
			// (get) Token: 0x0600041E RID: 1054 RVA: 0x00003AB0 File Offset: 0x00001CB0
			private Queue<IMemberDef> PendingForInject { get; }

			// Token: 0x17000095 RID: 149
			// (get) Token: 0x0600041F RID: 1055 RVA: 0x00003AB8 File Offset: 0x00001CB8
			private IImmutableList<IMethodInjectProcessor> MethodInjectProcessors { get; }

			// Token: 0x06000420 RID: 1056 RVA: 0x00018738 File Offset: 0x00016938
			internal Injector(InjectHelper.InjectContext injectContext, IInjectBehavior injectBehavior, IEnumerable<IMethodInjectProcessor> injectProcessors)
			{
				if (injectContext == null)
				{
					throw new ArgumentNullException("injectContext");
				}
				this.InjectContext = injectContext;
				if (injectBehavior == null)
				{
					throw new ArgumentNullException("injectBehavior");
				}
				this.InjectBehavior = injectBehavior;
				this.PendingForInject = new Queue<IMemberDef>();
				this.MethodInjectProcessors = ImmutableList.ToImmutableList<IMethodInjectProcessor>(injectProcessors);
				this._injectedMembers = new Dictionary<IMemberDef, IMemberDef>();
			}

			// Token: 0x06000421 RID: 1057 RVA: 0x0001879C File Offset: 0x0001699C
			private TypeDefUser CopyDef(TypeDef source)
			{
				IMemberDef memberDef;
				bool flag = this._injectedMembers.TryGetValue(source, out memberDef);
				TypeDefUser result;
				if (flag)
				{
					result = (TypeDefUser)memberDef;
				}
				else
				{
					TypeDefUser typeDefUser = new TypeDefUser(source.Namespace, source.Name)
					{
						Attributes = source.Attributes
					};
					bool hasClassLayout = source.HasClassLayout;
					if (hasClassLayout)
					{
						typeDefUser.ClassLayout = new ClassLayoutUser(source.ClassLayout.PackingSize, source.ClassSize);
					}
					InjectHelper.Injector.CloneGenericParameters(source, typeDefUser);
					this._injectedMembers.Add(source, typeDefUser);
					this.PendingForInject.Enqueue(source);
					bool isDelegate = source.IsDelegate;
					if (isDelegate)
					{
						foreach (MethodDef item in source.Methods)
						{
							this.PendingForInject.Enqueue(item);
						}
					}
					bool isEnum = source.IsEnum;
					if (isEnum)
					{
						foreach (FieldDef item2 in from f in source.Fields
						where !f.IsStatic
						select f)
						{
							this.PendingForInject.Enqueue(item2);
						}
					}
					result = typeDefUser;
				}
				return result;
			}

			// Token: 0x06000422 RID: 1058 RVA: 0x00018910 File Offset: 0x00016B10
			private MethodDefUser CopyDef(MethodDef source)
			{
				IMemberDef memberDef;
				bool flag = this._injectedMembers.TryGetValue(source, out memberDef);
				MethodDefUser result;
				if (flag)
				{
					result = (MethodDefUser)memberDef;
				}
				else
				{
					MethodDefUser methodDefUser = new MethodDefUser(source.Name, null, source.ImplAttributes, source.Attributes)
					{
						Attributes = source.Attributes
					};
					InjectHelper.Injector.CloneGenericParameters(source, methodDefUser);
					this._injectedMembers.Add(source, methodDefUser);
					this.PendingForInject.Enqueue(source);
					result = methodDefUser;
				}
				return result;
			}

			// Token: 0x06000423 RID: 1059 RVA: 0x00018988 File Offset: 0x00016B88
			private FieldDefUser CopyDef(FieldDef source)
			{
				IMemberDef memberDef;
				bool flag = this._injectedMembers.TryGetValue(source, out memberDef);
				FieldDefUser result;
				if (flag)
				{
					result = (FieldDefUser)memberDef;
				}
				else
				{
					FieldDefUser fieldDefUser = new FieldDefUser(source.Name, null, source.Attributes);
					this._injectedMembers.Add(source, fieldDefUser);
					this.PendingForInject.Enqueue(source);
					result = fieldDefUser;
				}
				return result;
			}

			// Token: 0x06000424 RID: 1060 RVA: 0x000189E4 File Offset: 0x00016BE4
			private EventDefUser CopyDef(EventDef source)
			{
				IMemberDef memberDef;
				bool flag = this._injectedMembers.TryGetValue(source, out memberDef);
				EventDefUser result;
				if (flag)
				{
					result = (EventDefUser)memberDef;
				}
				else
				{
					EventDefUser eventDefUser = new EventDefUser(source.Name, null, source.Attributes);
					this._injectedMembers.Add(source, eventDefUser);
					this.PendingForInject.Enqueue(source);
					result = eventDefUser;
				}
				return result;
			}

			// Token: 0x06000425 RID: 1061 RVA: 0x00018A40 File Offset: 0x00016C40
			private PropertyDefUser CopyDef(PropertyDef source)
			{
				IMemberDef memberDef;
				bool flag = this._injectedMembers.TryGetValue(source, out memberDef);
				PropertyDefUser result;
				if (flag)
				{
					result = (PropertyDefUser)memberDef;
				}
				else
				{
					PropertyDefUser propertyDefUser = new PropertyDefUser(source.Name, null, source.Attributes);
					this._injectedMembers.Add(source, propertyDefUser);
					this.PendingForInject.Enqueue(source);
					result = propertyDefUser;
				}
				return result;
			}

			// Token: 0x06000426 RID: 1062 RVA: 0x00018A9C File Offset: 0x00016C9C
			private static void CloneGenericParameters(ITypeOrMethodDef origin, ITypeOrMethodDef result)
			{
				bool hasGenericParameters = origin.HasGenericParameters;
				if (hasGenericParameters)
				{
					foreach (GenericParam genericParam in origin.GenericParameters)
					{
						result.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));
					}
				}
			}

			// Token: 0x06000427 RID: 1063 RVA: 0x00018B18 File Offset: 0x00016D18
			private IReadOnlyCollection<IMemberDef> InjectRemaining(Importer importer, IImmutableList<IMethodInjectProcessor> methodInjectProcessors)
			{
				ImmutableList<IMemberDef>.Builder builder = ImmutableList.CreateBuilder<IMemberDef>();
				while (this.PendingForInject.Count > 0)
				{
					IMemberDef memberDef = this.PendingForInject.Dequeue();
					TypeDef typeDef = memberDef as TypeDef;
					bool flag = typeDef != null;
					if (flag)
					{
						builder.Add(this.InjectTypeDef(typeDef, importer));
					}
					else
					{
						MethodDef methodDef = memberDef as MethodDef;
						bool flag2 = methodDef != null;
						if (flag2)
						{
							builder.Add(this.InjectMethodDef(methodDef, importer, methodInjectProcessors));
						}
						else
						{
							FieldDef fieldDef = memberDef as FieldDef;
							bool flag3 = fieldDef != null;
							if (flag3)
							{
								builder.Add(this.InjectFieldDef(fieldDef, importer));
							}
							else
							{
								EventDef eventDef = memberDef as EventDef;
								bool flag4 = eventDef != null;
								if (flag4)
								{
									builder.Add(this.InjectEventDef(eventDef, importer));
								}
								else
								{
									PropertyDef propertyDef = memberDef as PropertyDef;
									bool flag5 = propertyDef != null;
									if (!flag5)
									{
										throw new Exception("Unexpected member in remaining import list:" + memberDef.GetType().Name);
									}
									builder.Add(this.InjectPropertyDef(propertyDef, importer));
								}
							}
						}
					}
				}
				return builder.ToImmutable();
			}

			// Token: 0x06000428 RID: 1064 RVA: 0x00018C38 File Offset: 0x00016E38
			internal MethodDef Inject(MethodDef methodDef)
			{
				MethodDef methodDef2 = this.InjectContext.ResolveMapped<MethodDef>(methodDef);
				bool flag = methodDef2 != null;
				MethodDef result;
				if (flag)
				{
					result = methodDef2;
				}
				else
				{
					Importer importer = new Importer(this.InjectContext.TargetModule, ImporterOptions.TryToUseDefs, default(GenericParamContext), this);
					IImmutableList<IMethodInjectProcessor> methodInjectProcessors = this.MethodInjectProcessors.Add(new InjectHelper.Injector.ImportProcessor(importer));
					MethodDef methodDef3 = this.InjectMethodDef(methodDef, importer, methodInjectProcessors);
					this.InjectRemaining(importer, methodInjectProcessors);
					result = methodDef3;
				}
				return result;
			}

			// Token: 0x06000429 RID: 1065 RVA: 0x00018CB4 File Offset: 0x00016EB4
			internal TypeDef Inject(TypeDef typeDef)
			{
				TypeDef typeDef2 = this.InjectContext.ResolveMapped<TypeDef>(typeDef);
				bool flag = typeDef2 != null;
				TypeDef result;
				if (flag)
				{
					result = typeDef2;
				}
				else
				{
					Importer importer = new Importer(this.InjectContext.TargetModule, ImporterOptions.TryToUseDefs, default(GenericParamContext), this);
					IImmutableList<IMethodInjectProcessor> methodInjectProcessors = this.MethodInjectProcessors.Add(new InjectHelper.Injector.ImportProcessor(importer));
					TypeDef typeDef3 = this.InjectTypeDef(typeDef, importer);
					foreach (MethodDef source in typeDef.Methods)
					{
						this.CopyDef(source);
					}
					foreach (FieldDef source2 in typeDef.Fields)
					{
						this.CopyDef(source2);
					}
					foreach (EventDef source3 in typeDef.Events)
					{
						this.CopyDef(source3);
					}
					foreach (PropertyDef source4 in typeDef.Properties)
					{
						this.CopyDef(source4);
					}
					foreach (TypeDef typeDef4 in typeDef.NestedTypes)
					{
						this.Inject(typeDef4);
					}
					this.InjectRemaining(importer, methodInjectProcessors);
					result = typeDef3;
				}
				return result;
			}

			// Token: 0x0600042A RID: 1066 RVA: 0x00018E94 File Offset: 0x00017094
			private TypeDef InjectTypeDef(TypeDef typeDef, Importer importer)
			{
				bool flag = typeDef == null;
				if (flag)
				{
					throw new ArgumentNullException("typeDef");
				}
				TypeDef typeDef2 = this.InjectContext.ResolveMapped<TypeDef>(typeDef);
				bool flag2 = typeDef2 != null;
				TypeDef result;
				if (flag2)
				{
					result = typeDef2;
				}
				else
				{
					TypeDefUser typeDefUser = this.CopyDef(typeDef);
					typeDefUser.BaseType = importer.Import(typeDef.BaseType);
					bool flag3 = typeDef.DeclaringType != null;
					if (flag3)
					{
						typeDefUser.DeclaringType = this.InjectTypeDef(typeDef.DeclaringType, importer);
					}
					foreach (InterfaceImpl interfaceImpl in typeDef.Interfaces)
					{
						typeDefUser.Interfaces.Add(this.InjectInterfaceImpl(interfaceImpl, importer));
					}
					foreach (CustomAttribute attribute in typeDef.CustomAttributes)
					{
						typeDefUser.CustomAttributes.Add(InjectHelper.Injector.InjectCustomAttribute(attribute, importer));
					}
					this.InjectBehavior.Process(typeDef, typeDefUser, importer);
					bool flag4 = !typeDefUser.IsNested;
					if (flag4)
					{
						this.InjectContext.TargetModule.Types.Add(typeDefUser);
					}
					this.InjectContext.TargetModule.UpdateRowId<TypeDefUser>(typeDefUser);
					this.InjectContext.ApplyMapping(typeDef, typeDefUser);
					MethodDef methodDef = typeDef.FindDefaultConstructor();
					bool flag5 = methodDef != null;
					if (flag5)
					{
						this.PendingForInject.Enqueue(methodDef);
					}
					MethodDef methodDef2 = typeDef.FindStaticConstructor();
					bool flag6 = methodDef2 != null;
					if (flag6)
					{
						this.PendingForInject.Enqueue(methodDef2);
					}
					result = typeDefUser;
				}
				return result;
			}

			// Token: 0x0600042B RID: 1067 RVA: 0x00019058 File Offset: 0x00017258
			private InterfaceImplUser InjectInterfaceImpl(InterfaceImpl interfaceImpl, Importer importer)
			{
				bool flag = interfaceImpl == null;
				if (flag)
				{
					throw new ArgumentNullException("interfaceImpl");
				}
				ITypeDefOrRef typeDefOrRef = importer.Import(interfaceImpl.Interface);
				TypeDef typeDef = typeDefOrRef as TypeDef;
				bool flag2 = typeDef == null;
				if (flag2)
				{
					typeDef = ((TypeRef)typeDefOrRef).Resolve();
				}
				bool flag3 = typeDef != null && !typeDef.IsInterface;
				if (flag3)
				{
					throw new InvalidOperationException("Type for Interface is not a interface?!");
				}
				InterfaceImplUser interfaceImplUser = new InterfaceImplUser(typeDefOrRef);
				this.InjectContext.TargetModule.UpdateRowId<InterfaceImplUser>(interfaceImplUser);
				return interfaceImplUser;
			}

			// Token: 0x0600042C RID: 1068 RVA: 0x000190E8 File Offset: 0x000172E8
			private static CustomAttribute InjectCustomAttribute(CustomAttribute attribute, Importer importer)
			{
				CustomAttribute customAttribute = new CustomAttribute((ICustomAttributeType)importer.Import(attribute.Constructor));
				foreach (CAArgument caargument in attribute.ConstructorArguments)
				{
					customAttribute.ConstructorArguments.Add(new CAArgument(importer.Import(caargument.Type), caargument.Value));
				}
				foreach (CANamedArgument canamedArgument in attribute.NamedArguments)
				{
					customAttribute.NamedArguments.Add(new CANamedArgument(canamedArgument.IsField, importer.Import(canamedArgument.Type), canamedArgument.Name, new CAArgument(importer.Import(canamedArgument.Argument.Type), canamedArgument.Argument.Value)));
				}
				return customAttribute;
			}

			// Token: 0x0600042D RID: 1069 RVA: 0x00019208 File Offset: 0x00017408
			private FieldDef InjectFieldDef(FieldDef fieldDef, Importer importer)
			{
				FieldDef fieldDef2 = this.InjectContext.ResolveMapped<FieldDef>(fieldDef);
				bool flag = fieldDef2 != null;
				FieldDef result;
				if (flag)
				{
					result = fieldDef2;
				}
				else
				{
					FieldDefUser fieldDefUser = this.CopyDef(fieldDef);
					fieldDefUser.Signature = importer.Import(fieldDef.Signature);
					fieldDefUser.DeclaringType = (TypeDef)importer.Import(fieldDef.DeclaringType);
					foreach (CustomAttribute attribute in fieldDef.CustomAttributes)
					{
						fieldDefUser.CustomAttributes.Add(InjectHelper.Injector.InjectCustomAttribute(attribute, importer));
					}
					this.InjectBehavior.Process(fieldDef, fieldDefUser, importer);
					this.InjectContext.TargetModule.UpdateRowId<FieldDefUser>(fieldDefUser);
					this.InjectContext.ApplyMapping(fieldDef, fieldDefUser);
					result = fieldDefUser;
				}
				return result;
			}

			// Token: 0x0600042E RID: 1070 RVA: 0x000192F4 File Offset: 0x000174F4
			private MethodDef InjectMethodDef(MethodDef methodDef, Importer importer, IEnumerable<IMethodInjectProcessor> methodInjectProcessors)
			{
				MethodDef methodDef2 = this.InjectContext.ResolveMapped<MethodDef>(methodDef);
				bool flag = methodDef2 != null;
				MethodDef result;
				if (flag)
				{
					result = methodDef2;
				}
				else
				{
					MethodDefUser methodDefUser = this.CopyDef(methodDef);
					methodDefUser.DeclaringType = (TypeDef)importer.Import(methodDef.DeclaringType);
					methodDefUser.Signature = importer.Import(methodDef.Signature);
					methodDefUser.Parameters.UpdateParameterTypes();
					bool flag2 = methodDef.ImplMap != null;
					if (flag2)
					{
						methodDefUser.ImplMap = new ImplMapUser(new ModuleRefUser(this.InjectContext.TargetModule, methodDef.ImplMap.Module.Name), methodDef.ImplMap.Name, methodDef.ImplMap.Attributes);
					}
					foreach (CustomAttribute attribute in methodDef.CustomAttributes)
					{
						methodDefUser.CustomAttributes.Add(InjectHelper.Injector.InjectCustomAttribute(attribute, importer));
					}
					bool hasBody = methodDef.HasBody;
					if (hasBody)
					{
						methodDef.Body.SimplifyBranches();
						methodDef.Body.SimplifyMacros(methodDef.Parameters);
						methodDefUser.Body = new CilBody(methodDef.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>());
						methodDefUser.Body.MaxStack = methodDef.Body.MaxStack;
						Dictionary<object, object> bodyMap = new Dictionary<object, object>();
						foreach (Local local in methodDef.Body.Variables)
						{
							Local local2 = new Local(importer.Import(local.Type));
							methodDefUser.Body.Variables.Add(local2);
							local2.Name = local.Name;
							bodyMap[local] = local2;
						}
						foreach (Instruction instruction in methodDef.Body.Instructions)
						{
							Instruction instruction2 = new Instruction(instruction.OpCode, instruction.Operand)
							{
								SequencePoint = instruction.SequencePoint
							};
							methodDefUser.Body.Instructions.Add(instruction2);
							bodyMap[instruction] = instruction2;
						}
						Func<Instruction, Instruction> <>9__0;
						foreach (Instruction instruction3 in methodDefUser.Body.Instructions)
						{
							bool flag3 = instruction3.Operand != null && bodyMap.ContainsKey(instruction3.Operand);
							if (flag3)
							{
								instruction3.Operand = bodyMap[instruction3.Operand];
							}
							else
							{
								object operand = instruction3.Operand;
								Instruction[] array = operand as Instruction[];
								bool flag4 = array != null;
								if (flag4)
								{
									Instruction instruction4 = instruction3;
									IEnumerable<Instruction> source = array;
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
							methodDefUser.Body.ExceptionHandlers.Add(new ExceptionHandler(exceptionHandler.HandlerType)
							{
								CatchType = ((exceptionHandler.CatchType == null) ? null : importer.Import(exceptionHandler.CatchType)),
								TryStart = (Instruction)bodyMap[exceptionHandler.TryStart],
								TryEnd = (Instruction)bodyMap[exceptionHandler.TryEnd],
								HandlerStart = (Instruction)bodyMap[exceptionHandler.HandlerStart],
								HandlerEnd = (Instruction)bodyMap[exceptionHandler.HandlerEnd],
								FilterStart = ((exceptionHandler.FilterStart == null) ? null : ((Instruction)bodyMap[exceptionHandler.FilterStart]))
							});
						}
						foreach (IMethodInjectProcessor methodInjectProcessor in methodInjectProcessors)
						{
							methodInjectProcessor.Process(methodDefUser);
						}
					}
					this.InjectBehavior.Process(methodDef, methodDefUser, importer);
					this.InjectContext.TargetModule.UpdateRowId<MethodDefUser>(methodDefUser);
					this.InjectContext.ApplyMapping(methodDef, methodDefUser);
					result = methodDefUser;
				}
				return result;
			}

			// Token: 0x0600042F RID: 1071 RVA: 0x0001981C File Offset: 0x00017A1C
			private EventDef InjectEventDef(EventDef eventDef, Importer importer)
			{
				EventDef eventDef2 = this.InjectContext.ResolveMapped<EventDef>(eventDef);
				bool flag = eventDef2 != null;
				EventDef result;
				if (flag)
				{
					result = eventDef2;
				}
				else
				{
					EventDefUser eventDefUser = this.CopyDef(eventDef);
					eventDefUser.AddMethod = this.CopyDef(eventDef.AddMethod);
					eventDefUser.InvokeMethod = this.CopyDef(eventDef.InvokeMethod);
					eventDefUser.RemoveMethod = this.CopyDef(eventDef.RemoveMethod);
					bool hasOtherMethods = eventDef.HasOtherMethods;
					if (hasOtherMethods)
					{
						foreach (MethodDef source in eventDef.OtherMethods)
						{
							eventDefUser.OtherMethods.Add(this.CopyDef(source));
						}
					}
					eventDefUser.DeclaringType = (TypeDef)importer.Import(eventDef.DeclaringType);
					foreach (CustomAttribute attribute in eventDef.CustomAttributes)
					{
						eventDefUser.CustomAttributes.Add(InjectHelper.Injector.InjectCustomAttribute(attribute, importer));
					}
					this.InjectBehavior.Process(eventDef, eventDefUser, importer);
					this.InjectContext.TargetModule.UpdateRowId<EventDefUser>(eventDefUser);
					this.InjectContext.ApplyMapping(eventDef, eventDefUser);
					result = eventDefUser;
				}
				return result;
			}

			// Token: 0x06000430 RID: 1072 RVA: 0x0001998C File Offset: 0x00017B8C
			private PropertyDef InjectPropertyDef(PropertyDef propertyDef, Importer importer)
			{
				PropertyDef propertyDef2 = this.InjectContext.ResolveMapped<PropertyDef>(propertyDef);
				bool flag = propertyDef2 != null;
				PropertyDef result;
				if (flag)
				{
					result = propertyDef2;
				}
				else
				{
					PropertyDefUser propertyDefUser = this.CopyDef(propertyDef);
					foreach (MethodDef source in propertyDef.GetMethods)
					{
						propertyDefUser.GetMethods.Add(this.CopyDef(source));
					}
					foreach (MethodDef source2 in propertyDef.SetMethods)
					{
						propertyDefUser.SetMethods.Add(this.CopyDef(source2));
					}
					bool hasOtherMethods = propertyDef.HasOtherMethods;
					if (hasOtherMethods)
					{
						foreach (MethodDef source3 in propertyDef.OtherMethods)
						{
							propertyDefUser.OtherMethods.Add(this.CopyDef(source3));
						}
					}
					propertyDefUser.DeclaringType = (TypeDef)importer.Import(propertyDef.DeclaringType);
					foreach (CustomAttribute attribute in propertyDef.CustomAttributes)
					{
						propertyDefUser.CustomAttributes.Add(InjectHelper.Injector.InjectCustomAttribute(attribute, importer));
					}
					this.InjectBehavior.Process(propertyDef, propertyDefUser, importer);
					this.InjectContext.TargetModule.UpdateRowId<PropertyDefUser>(propertyDefUser);
					this.InjectContext.ApplyMapping(propertyDef, propertyDefUser);
					result = propertyDefUser;
				}
				return result;
			}

			// Token: 0x06000431 RID: 1073 RVA: 0x00019B64 File Offset: 0x00017D64
			public override ITypeDefOrRef Map(ITypeDefOrRef typeDefOrRef)
			{
				TypeDef typeDef = typeDefOrRef as TypeDef;
				bool flag = typeDef != null;
				if (flag)
				{
					TypeDef typeDef2 = this.InjectContext.ResolveMapped<TypeDef>(typeDef);
					bool flag2 = typeDef2 != null;
					if (flag2)
					{
						return typeDef2;
					}
					bool flag3 = typeDef.Module == this.InjectContext.OriginModule;
					if (flag3)
					{
						return this.CopyDef(typeDef);
					}
				}
				return base.Map(typeDefOrRef);
			}

			// Token: 0x06000432 RID: 1074 RVA: 0x00019BD0 File Offset: 0x00017DD0
			public override IMethod Map(MethodDef methodDef)
			{
				MethodDef methodDef2 = this.InjectContext.ResolveMapped<MethodDef>(methodDef);
				bool flag = methodDef2 != null;
				IMethod result;
				if (flag)
				{
					result = methodDef2;
				}
				else
				{
					bool flag2 = methodDef.Module == this.InjectContext.OriginModule;
					if (flag2)
					{
						result = this.CopyDef(methodDef);
					}
					else
					{
						result = base.Map(methodDef);
					}
				}
				return result;
			}

			// Token: 0x06000433 RID: 1075 RVA: 0x00019C24 File Offset: 0x00017E24
			public override IField Map(FieldDef fieldDef)
			{
				FieldDef fieldDef2 = this.InjectContext.ResolveMapped<FieldDef>(fieldDef);
				bool flag = fieldDef2 != null;
				IField result;
				if (flag)
				{
					result = fieldDef2;
				}
				else
				{
					bool flag2 = fieldDef.Module == this.InjectContext.OriginModule;
					if (flag2)
					{
						result = this.CopyDef(fieldDef);
					}
					else
					{
						result = base.Map(fieldDef);
					}
				}
				return result;
			}

			// Token: 0x040002A3 RID: 675
			private readonly Dictionary<IMemberDef, IMemberDef> _injectedMembers;

			// Token: 0x020000B4 RID: 180
			private struct ImportProcessor : IMethodInjectProcessor
			{
				// Token: 0x06000434 RID: 1076 RVA: 0x00003AC0 File Offset: 0x00001CC0
				internal ImportProcessor(Importer importer)
				{
					this._importer = importer;
				}

				// Token: 0x06000435 RID: 1077 RVA: 0x00019C78 File Offset: 0x00017E78
				void IMethodInjectProcessor.Process(MethodDef method)
				{
					bool flag = method.HasBody && method.Body.HasInstructions;
					if (flag)
					{
						foreach (Instruction instruction in method.Body.Instructions)
						{
							object operand = instruction.Operand;
							IType type = operand as IType;
							bool flag2 = type != null;
							if (flag2)
							{
								IType operand2 = this._importer.Import(type);
								instruction.Operand = operand2;
							}
							else
							{
								object operand3 = instruction.Operand;
								IMethod method2 = operand3 as IMethod;
								bool flag3 = method2 != null;
								if (flag3)
								{
									IMethod operand4 = this._importer.Import(method2);
									instruction.Operand = operand4;
								}
								else
								{
									object operand5 = instruction.Operand;
									IField field = operand5 as IField;
									bool flag4 = field != null;
									if (flag4)
									{
										IField operand6 = this._importer.Import(field);
										instruction.Operand = operand6;
									}
								}
							}
						}
					}
				}

				// Token: 0x040002A8 RID: 680
				private Importer _importer;
			}
		}
	}
}
