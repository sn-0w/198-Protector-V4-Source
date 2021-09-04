using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Confuser.Core;
using Confuser.Renamer.Analyzers;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000020 RID: 32
	internal class BAMLAnalyzer
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x0000A3EA File Offset: 0x000085EA
		private PropertyPathParser PathParser { get; } = new PropertyPathParser();

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000C6 RID: 198 RVA: 0x0000A3F4 File Offset: 0x000085F4
		// (remove) Token: 0x060000C7 RID: 199 RVA: 0x0000A42C File Offset: 0x0000862C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BAMLAnalyzer, BamlElement> AnalyzeElement;

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x0000A464 File Offset: 0x00008664
		public ConfuserContext Context
		{
			get
			{
				return this.context;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x0000A47C File Offset: 0x0000867C
		public INameService NameService
		{
			get
			{
				return this.service;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000CA RID: 202 RVA: 0x0000A494 File Offset: 0x00008694
		// (set) Token: 0x060000CB RID: 203 RVA: 0x0000A49C File Offset: 0x0000869C
		public string CurrentBAMLName { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000CC RID: 204 RVA: 0x0000A4A5 File Offset: 0x000086A5
		// (set) Token: 0x060000CD RID: 205 RVA: 0x0000A4AD File Offset: 0x000086AD
		public ModuleDefMD Module { get; set; }

		// Token: 0x060000CE RID: 206 RVA: 0x0000A4B8 File Offset: 0x000086B8
		public BAMLAnalyzer(ConfuserContext context, INameService service)
		{
			this.context = context;
			this.service = service;
			this.PreInit();
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000A55C File Offset: 0x0000875C
		private void PreInit()
		{
			foreach (TypeDef typeDef in this.context.Modules.SelectMany((ModuleDefMD m) => m.GetTypes()))
			{
				foreach (PropertyDef propertyDef in typeDef.Properties)
				{
					bool flag = propertyDef.IsPublic() && !propertyDef.IsStatic();
					if (flag)
					{
						this.properties.AddListEntry(propertyDef.Name, propertyDef);
					}
				}
				foreach (EventDef eventDef in typeDef.Events)
				{
					bool flag2 = eventDef.IsPublic() && !eventDef.IsStatic();
					if (flag2)
					{
						this.events.AddListEntry(eventDef.Name, eventDef);
					}
				}
				foreach (MethodDef methodDef in typeDef.Methods)
				{
					bool flag3 = methodDef.IsPublic && !methodDef.IsStatic;
					if (flag3)
					{
						this.methods.AddListEntry(methodDef.Name, methodDef);
					}
				}
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000A75C File Offset: 0x0000895C
		public IEnumerable<PropertyDef> LookupProperty(string name)
		{
			List<PropertyDef> list;
			bool flag = !this.properties.TryGetValue(name, out list);
			IEnumerable<PropertyDef> result;
			if (flag)
			{
				result = Enumerable.Empty<PropertyDef>();
			}
			else
			{
				result = list;
			}
			return result;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000A78C File Offset: 0x0000898C
		public IEnumerable<EventDef> LookupEvent(string name)
		{
			List<EventDef> list;
			bool flag = !this.events.TryGetValue(name, out list);
			IEnumerable<EventDef> result;
			if (flag)
			{
				result = Enumerable.Empty<EventDef>();
			}
			else
			{
				result = list;
			}
			return result;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000A7BC File Offset: 0x000089BC
		public IEnumerable<MethodDef> LookupMethod(string name)
		{
			List<MethodDef> list;
			bool flag = !this.methods.TryGetValue(name, out list);
			IEnumerable<MethodDef> result;
			if (flag)
			{
				result = Enumerable.Empty<MethodDef>();
			}
			else
			{
				result = list;
			}
			return result;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000A7EC File Offset: 0x000089EC
		public BamlDocument Analyze(ModuleDefMD module, string bamlName, byte[] data)
		{
			this.Module = module;
			this.CurrentBAMLName = bamlName;
			bool isClr = module.IsClr40;
			if (isClr)
			{
				KnownThingsv4 knownThingsv;
				if ((knownThingsv = this.thingsv4) == null)
				{
					knownThingsv = (this.thingsv4 = new KnownThingsv4(this.context, module));
				}
				this.things = knownThingsv;
			}
			else
			{
				KnownThingsv3 knownThingsv2;
				if ((knownThingsv2 = this.thingsv3) == null)
				{
					knownThingsv2 = (this.thingsv3 = new KnownThingsv3(this.context, module));
				}
				this.things = knownThingsv2;
			}
			Debug.Assert(BitConverter.ToInt32(data, 0) == data.Length - 4);
			BamlDocument bamlDocument = BamlReader.ReadDocument(new MemoryStream(data, 4, data.Length - 4));
			bamlDocument.RemoveWhere((BamlRecord rec) => rec is LineNumberAndPositionRecord || rec is LinePositionRecord);
			this.PopulateReferences(bamlDocument);
			BamlElement bamlElement = BamlElement.Read(bamlDocument);
			BamlElement root = bamlElement.Children.Single<BamlElement>();
			Stack<BamlElement> stack = new Stack<BamlElement>();
			stack.Push(bamlElement);
			while (stack.Count > 0)
			{
				BamlElement bamlElement2 = stack.Pop();
				this.ProcessBAMLElement(root, bamlElement2);
				foreach (BamlElement item in bamlElement2.Children)
				{
					stack.Push(item);
				}
			}
			return bamlDocument;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000A958 File Offset: 0x00008B58
		private void PopulateReferences(BamlDocument document)
		{
			Dictionary<string, List<Tuple<AssemblyDef, string>>> dictionary = new Dictionary<string, List<Tuple<AssemblyDef, string>>>();
			this.assemblyRefs.Clear();
			foreach (AssemblyInfoRecord assemblyInfoRecord in document.OfType<AssemblyInfoRecord>())
			{
				AssemblyDef assembly = this.context.Resolver.ResolveThrow(assemblyInfoRecord.AssemblyFullName, this.Module);
				this.assemblyRefs.Add(assemblyInfoRecord.AssemblyId, assembly);
				bool flag = !this.context.Modules.Any((ModuleDefMD m) => m.Assembly == assembly);
				if (!flag)
				{
					foreach (CustomAttribute customAttribute in assembly.CustomAttributes.FindAll("System.Windows.Markup.XmlnsDefinitionAttribute"))
					{
						dictionary.AddListEntry((UTF8String)customAttribute.ConstructorArguments[0].Value, Tuple.Create<AssemblyDef, string>(assembly, (UTF8String)customAttribute.ConstructorArguments[1].Value));
					}
				}
			}
			this.xmlnsCtx = new BAMLAnalyzer.XmlNsContext(document, this.assemblyRefs);
			this.typeRefs.Clear();
			foreach (TypeInfoRecord typeInfoRecord in document.OfType<TypeInfoRecord>())
			{
				short num = (short)(typeInfoRecord.AssemblyId & 4095);
				bool flag2 = num == -1;
				AssemblyDef assemblyDef;
				if (flag2)
				{
					assemblyDef = this.things.FrameworkAssembly;
				}
				else
				{
					assemblyDef = this.assemblyRefs[(ushort)num];
				}
				AssemblyDef assemblyDef2 = (this.Module.Assembly == assemblyDef) ? null : assemblyDef;
				TypeSig typeSig = TypeNameParser.ParseAsTypeSigReflectionThrow(this.Module, typeInfoRecord.TypeFullName, new BAMLAnalyzer.DummyAssemblyRefFinder(assemblyDef2));
				this.typeRefs[typeInfoRecord.TypeId] = typeSig;
				this.AddTypeSigReference(typeSig, new BAMLTypeReference(typeSig, typeInfoRecord));
			}
			this.attrRefs.Clear();
			foreach (AttributeInfoRecord attributeInfoRecord in document.OfType<AttributeInfoRecord>())
			{
				TypeSig typeSig2;
				bool flag3 = this.typeRefs.TryGetValue(attributeInfoRecord.OwnerTypeId, out typeSig2);
				if (flag3)
				{
					TypeDef declType = typeSig2.ToBasicTypeDefOrRef().ResolveTypeDefThrow();
					this.attrRefs[attributeInfoRecord.AttributeId] = this.AnalyzeAttributeReference(declType, attributeInfoRecord);
				}
				else
				{
					Debug.Assert((short)attributeInfoRecord.OwnerTypeId < 0);
					TypeDef declType2 = this.things.Types(-(KnownTypes)attributeInfoRecord.OwnerTypeId);
					this.attrRefs[attributeInfoRecord.AttributeId] = this.AnalyzeAttributeReference(declType2, attributeInfoRecord);
				}
			}
			this.strings.Clear();
			foreach (StringInfoRecord stringInfoRecord in document.OfType<StringInfoRecord>())
			{
				this.strings[stringInfoRecord.StringId] = stringInfoRecord;
			}
			foreach (PIMappingRecord pimappingRecord in document.OfType<PIMappingRecord>())
			{
				short num2 = (short)(pimappingRecord.AssemblyId & 4095);
				bool flag4 = num2 == -1;
				AssemblyDef item;
				if (flag4)
				{
					item = this.things.FrameworkAssembly;
				}
				else
				{
					item = this.assemblyRefs[(ushort)num2];
				}
				Tuple<AssemblyDef, string> value = Tuple.Create<AssemblyDef, string>(item, pimappingRecord.ClrNamespace);
				dictionary.AddListEntry(pimappingRecord.XmlNamespace, value);
			}
			this.xmlns.Clear();
			foreach (XmlnsPropertyRecord xmlnsPropertyRecord in document.OfType<XmlnsPropertyRecord>())
			{
				List<Tuple<AssemblyDef, string>> list;
				bool flag5 = dictionary.TryGetValue(xmlnsPropertyRecord.XmlNamespace, out list);
				if (flag5)
				{
					this.xmlns[xmlnsPropertyRecord.Prefix] = list;
					foreach (Tuple<AssemblyDef, string> scope in list)
					{
						this.xmlnsCtx.AddNsMap(scope, xmlnsPropertyRecord.Prefix);
					}
				}
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000AEC4 File Offset: 0x000090C4
		public TypeDef ResolveType(ushort typeId)
		{
			bool flag = (short)typeId < 0;
			TypeDef result;
			if (flag)
			{
				result = this.things.Types(-(KnownTypes)typeId);
			}
			else
			{
				result = this.typeRefs[typeId].ToBasicTypeDefOrRef().ResolveTypeDefThrow();
			}
			return result;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000AF0C File Offset: 0x0000910C
		private TypeSig ResolveType(string typeName, out string prefix)
		{
			int num = typeName.IndexOf(':');
			bool flag = num == -1;
			List<Tuple<AssemblyDef, string>> list;
			if (flag)
			{
				prefix = "";
				bool flag2 = !this.xmlns.TryGetValue(prefix, out list);
				if (flag2)
				{
					return null;
				}
			}
			else
			{
				prefix = typeName.Substring(0, num);
				bool flag3 = !this.xmlns.TryGetValue(prefix, out list);
				if (flag3)
				{
					return null;
				}
				typeName = typeName.Substring(num + 1);
			}
			foreach (Tuple<AssemblyDef, string> tuple in list)
			{
				TypeSig typeSig = TypeNameParser.ParseAsTypeSigReflectionThrow(this.Module, tuple.Item2 + "." + typeName, new BAMLAnalyzer.DummyAssemblyRefFinder(tuple.Item1));
				bool flag4 = typeSig.ToBasicTypeDefOrRef().ResolveTypeDef() != null;
				if (flag4)
				{
					return typeSig;
				}
			}
			return null;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000B018 File Offset: 0x00009218
		public Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> ResolveAttribute(ushort attrId)
		{
			bool flag = (short)attrId < 0;
			Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> result;
			if (flag)
			{
				Tuple<KnownTypes, PropertyDef, TypeDef> tuple = this.things.Properties(-(KnownProperties)attrId);
				result = Tuple.Create<IDnlibDef, AttributeInfoRecord, TypeDef>(tuple.Item2, null, tuple.Item3);
			}
			else
			{
				result = this.attrRefs[attrId];
			}
			return result;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000B06C File Offset: 0x0000926C
		private void AddTypeSigReference(TypeSig typeSig, INameReference<IDnlibDef> reference)
		{
			foreach (ITypeDefOrRef typeDefOrRef in typeSig.FindTypeRefs())
			{
				TypeDef typeDef = typeDefOrRef.ResolveTypeDefThrow();
				bool flag = this.context.Modules.Contains((ModuleDefMD)typeDef.Module);
				if (flag)
				{
					this.service.ReduceRenameMode(typeDef, RenameMode.Letters);
					bool flag2 = typeDefOrRef is TypeRef;
					if (flag2)
					{
						this.service.AddReference<TypeDef>(typeDef, new TypeRefReference((TypeRef)typeDefOrRef, typeDef));
					}
					this.service.AddReference<IDnlibDef>(typeDef, reference);
				}
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000B124 File Offset: 0x00009324
		private void ProcessBAMLElement(BamlElement root, BamlElement elem)
		{
			this.ProcessElementHeader(elem);
			this.ProcessElementBody(root, elem);
			bool flag = this.AnalyzeElement != null;
			if (flag)
			{
				this.AnalyzeElement(this, elem);
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000B160 File Offset: 0x00009360
		private void ProcessElementHeader(BamlElement elem)
		{
			BamlRecordType type = elem.Header.Type;
			BamlRecordType bamlRecordType = type;
			if (bamlRecordType > BamlRecordType.PropertyDictionaryStart)
			{
				if (bamlRecordType <= BamlRecordType.ConstructorParametersStart)
				{
					if (bamlRecordType != BamlRecordType.KeyElementStart)
					{
						if (bamlRecordType != BamlRecordType.ConstructorParametersStart)
						{
							return;
						}
						elem.Type = elem.Parent.Type;
						elem.Attribute = elem.Parent.Attribute;
						return;
					}
				}
				else
				{
					if (bamlRecordType == BamlRecordType.NamedElementStart)
					{
						goto IL_AA;
					}
					if (bamlRecordType != BamlRecordType.StaticResourceStart)
					{
						return;
					}
				}
				elem.Type = this.Module.CorLibTypes.Object.TypeDefOrRef.ResolveTypeDef();
				elem.Attribute = null;
				return;
			}
			if (bamlRecordType == BamlRecordType.DocumentStart)
			{
				return;
			}
			if (bamlRecordType != BamlRecordType.ElementStart)
			{
				switch (bamlRecordType)
				{
				case BamlRecordType.PropertyComplexStart:
				case BamlRecordType.PropertyArrayStart:
				case BamlRecordType.PropertyListStart:
				case BamlRecordType.PropertyDictionaryStart:
				{
					Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple = this.ResolveAttribute(((PropertyComplexStartRecord)elem.Header).AttributeId);
					elem.Type = tuple.Item3;
					elem.Attribute = tuple.Item1;
					bool flag = elem.Attribute != null;
					if (flag)
					{
						elem.Type = this.GetAttributeType(elem.Attribute);
					}
					return;
				}
				case BamlRecordType.PropertyComplexEnd:
				case BamlRecordType.PropertyArrayEnd:
				case BamlRecordType.PropertyListEnd:
					return;
				default:
					return;
				}
			}
			IL_AA:
			elem.Type = this.ResolveType(((ElementStartRecord)elem.Header).TypeId);
			elem.Attribute = elem.Parent.Attribute;
			bool flag2 = elem.Attribute != null;
			if (flag2)
			{
				elem.Type = this.GetAttributeType(elem.Attribute);
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000B2EC File Offset: 0x000094EC
		private TypeDef GetAttributeType(IDnlibDef attr)
		{
			ITypeDefOrRef typeDefOrRef = null;
			bool flag = attr is PropertyDef;
			if (flag)
			{
				typeDefOrRef = ((PropertyDef)attr).PropertySig.RetType.ToBasicTypeDefOrRef();
			}
			else
			{
				bool flag2 = attr is EventDef;
				if (flag2)
				{
					typeDefOrRef = ((EventDef)attr).EventType;
				}
			}
			return (typeDefOrRef == null) ? null : typeDefOrRef.ResolveTypeDefThrow();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000B34C File Offset: 0x0000954C
		private void ProcessElementBody(BamlElement root, BamlElement elem)
		{
			foreach (BamlRecord bamlRecord in elem.Body)
			{
				bool flag = bamlRecord is PropertyRecord;
				if (flag)
				{
					PropertyRecord propertyRecord = (PropertyRecord)bamlRecord;
					Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple = this.ResolveAttribute(propertyRecord.AttributeId);
					TypeDef type = tuple.Item3;
					IDnlibDef item = tuple.Item1;
					bool flag2 = item != null;
					if (flag2)
					{
						type = this.GetAttributeType(item);
					}
					bool flag3 = tuple.Item1 is EventDef;
					if (flag3)
					{
						MethodDef methodDef = root.Type.FindMethod(propertyRecord.Value);
						bool flag4 = methodDef == null;
						if (flag4)
						{
							this.context.Logger.WarnFormat("Cannot resolve method '{0}' in '{1}'.", new object[]
							{
								root.Type.FullName,
								propertyRecord.Value
							});
						}
						else
						{
							BAMLAttributeReference reference = new BAMLAttributeReference(methodDef, propertyRecord);
							this.service.AddReference<IDnlibDef>(methodDef, reference);
						}
					}
					bool flag5 = bamlRecord is PropertyWithConverterRecord;
					if (flag5)
					{
						this.ProcessConverter((PropertyWithConverterRecord)bamlRecord, type);
					}
				}
				else
				{
					bool flag6 = bamlRecord is PropertyComplexStartRecord;
					if (flag6)
					{
						Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple2 = this.ResolveAttribute(((PropertyComplexStartRecord)bamlRecord).AttributeId);
						TypeDef type = tuple2.Item3;
						IDnlibDef item = tuple2.Item1;
						bool flag7 = item != null;
						if (flag7)
						{
							type = this.GetAttributeType(item);
						}
					}
					else
					{
						bool flag8 = bamlRecord is ContentPropertyRecord;
						if (flag8)
						{
							Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple3 = this.ResolveAttribute(((ContentPropertyRecord)bamlRecord).AttributeId);
							TypeDef type = tuple3.Item3;
							IDnlibDef item = tuple3.Item1;
							bool flag9 = elem.Attribute != null && item != null;
							if (flag9)
							{
								type = this.GetAttributeType(item);
							}
							foreach (BamlElement bamlElement in elem.Children)
							{
								bamlElement.Type = type;
								bamlElement.Attribute = item;
							}
						}
						else
						{
							bool flag10 = bamlRecord is PropertyCustomRecord;
							if (flag10)
							{
								PropertyCustomRecord propertyCustomRecord = (PropertyCustomRecord)bamlRecord;
								Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple4 = this.ResolveAttribute(propertyCustomRecord.AttributeId);
								TypeDef type = tuple4.Item3;
								IDnlibDef item = tuple4.Item1;
								bool flag11 = elem.Attribute != null && item != null;
								if (flag11)
								{
									type = this.GetAttributeType(item);
								}
								bool flag12 = ((int)propertyCustomRecord.SerializerTypeId & -16385) != 0 && ((int)propertyCustomRecord.SerializerTypeId & -16385) == 137;
								if (flag12)
								{
								}
							}
							else
							{
								bool flag13 = bamlRecord is PropertyWithExtensionRecord;
								if (flag13)
								{
									PropertyWithExtensionRecord propertyWithExtensionRecord = (PropertyWithExtensionRecord)bamlRecord;
									Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple5 = this.ResolveAttribute(propertyWithExtensionRecord.AttributeId);
									TypeDef type = tuple5.Item3;
									IDnlibDef item = tuple5.Item1;
									bool flag14 = elem.Attribute != null && item != null;
									if (flag14)
									{
										type = this.GetAttributeType(item);
									}
									bool flag15 = propertyWithExtensionRecord.Flags == 602;
									if (flag15)
									{
										bool flag16 = (short)propertyWithExtensionRecord.ValueId >= 0;
										if (flag16)
										{
											tuple5 = this.ResolveAttribute(propertyWithExtensionRecord.ValueId);
											IDnlibDef dnlibDef = tuple5.Item1;
											bool flag17 = dnlibDef == null;
											if (flag17)
											{
												TypeSig typeSig;
												bool flag18 = this.typeRefs.TryGetValue(tuple5.Item2.OwnerTypeId, out typeSig);
												TypeDef typeDef;
												if (flag18)
												{
													typeDef = typeSig.ToBasicTypeDefOrRef().ResolveTypeDefThrow();
												}
												else
												{
													Debug.Assert((short)tuple5.Item2.OwnerTypeId < 0);
													typeDef = this.things.Types(-(KnownTypes)tuple5.Item2.OwnerTypeId);
												}
												dnlibDef = typeDef.FindField(tuple5.Item2.Name);
											}
											bool flag19 = dnlibDef != null;
											if (flag19)
											{
												this.service.AddReference<IDnlibDef>(dnlibDef, new BAMLAttributeReference(dnlibDef, tuple5.Item2));
											}
										}
									}
								}
								else
								{
									bool flag20 = bamlRecord is TextRecord;
									if (flag20)
									{
										TextRecord textRecord = (TextRecord)bamlRecord;
										string value = textRecord.Value;
										bool flag21 = textRecord is TextWithIdRecord;
										if (flag21)
										{
											value = this.strings[((TextWithIdRecord)textRecord).ValueId].Value;
										}
										TypeSig typeSig2 = null;
										try
										{
											string text;
											typeSig2 = this.ResolveType(value.Trim(), out text);
										}
										catch (TypeNameParserException)
										{
										}
										bool flag22 = typeSig2 != null && this.context.Modules.Contains((ModuleDefMD)typeSig2.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
										if (flag22)
										{
											BAMLConverterTypeReference reference2 = new BAMLConverterTypeReference(this.xmlnsCtx, typeSig2, textRecord);
											this.AddTypeSigReference(typeSig2, reference2);
										}
										else
										{
											this.AnalyzePropertyPath(value);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000B87C File Offset: 0x00009A7C
		private void ProcessConverter(PropertyWithConverterRecord rec, TypeDef type)
		{
			TypeDef typeDef = this.ResolveType(rec.ConverterTypeId);
			bool flag = typeDef.FullName == "System.ComponentModel.EnumConverter";
			if (flag)
			{
				bool flag2 = type != null && this.context.Modules.Contains((ModuleDefMD)type.Module);
				if (flag2)
				{
					FieldDef fieldDef = type.FindField(rec.Value);
					bool flag3 = fieldDef != null;
					if (flag3)
					{
						this.service.AddReference<FieldDef>(fieldDef, new BAMLEnumReference(fieldDef, rec));
					}
				}
			}
			else
			{
				bool flag4 = typeDef.FullName == "System.Windows.Input.CommandConverter";
				if (flag4)
				{
					string text = rec.Value.Trim();
					int num = text.IndexOf('.');
					bool flag5 = num != -1;
					if (flag5)
					{
						string typeName = text.Substring(0, num);
						string text2;
						TypeSig typeSig = this.ResolveType(typeName, out text2);
						bool flag6 = typeSig != null;
						if (flag6)
						{
							string s = text.Substring(num + 1);
							TypeDef typeDef2 = typeSig.ToBasicTypeDefOrRef().ResolveTypeDefThrow();
							bool flag7 = this.context.Modules.Contains((ModuleDefMD)typeDef2.Module);
							if (flag7)
							{
								PropertyDef propertyDef = typeDef2.FindProperty(s);
								bool flag8 = propertyDef != null;
								if (flag8)
								{
									BAMLConverterMemberReference reference = new BAMLConverterMemberReference(this.xmlnsCtx, typeSig, propertyDef, rec);
									this.AddTypeSigReference(typeSig, reference);
									this.service.ReduceRenameMode(propertyDef, RenameMode.Letters);
									this.service.AddReference<IDnlibDef>(propertyDef, reference);
								}
								FieldDef fieldDef2 = typeDef2.FindField(s);
								bool flag9 = fieldDef2 != null;
								if (flag9)
								{
									BAMLConverterMemberReference reference2 = new BAMLConverterMemberReference(this.xmlnsCtx, typeSig, fieldDef2, rec);
									this.AddTypeSigReference(typeSig, reference2);
									this.service.ReduceRenameMode(fieldDef2, RenameMode.Letters);
									this.service.AddReference<IDnlibDef>(fieldDef2, reference2);
								}
								bool flag10 = propertyDef == null && fieldDef2 == null;
								if (flag10)
								{
									this.context.Logger.WarnFormat("Could not resolve command '{0}' in '{1}'.", new object[]
									{
										text,
										this.CurrentBAMLName
									});
								}
							}
						}
					}
				}
				else
				{
					bool flag11 = typeDef.FullName == "System.Windows.Markup.DependencyPropertyConverter";
					if (!flag11)
					{
						bool flag12 = typeDef.FullName == "System.Windows.PropertyPathConverter";
						if (flag12)
						{
							this.AnalyzePropertyPath(rec.Value);
						}
						else
						{
							bool flag13 = typeDef.FullName == "System.Windows.Markup.RoutedEventConverter";
							if (!flag13)
							{
								bool flag14 = typeDef.FullName == "System.Windows.Markup.TypeTypeConverter";
								if (flag14)
								{
									string text3;
									TypeSig typeSig2 = this.ResolveType(rec.Value.Trim(), out text3);
									bool flag15 = typeSig2 != null && this.context.Modules.Contains((ModuleDefMD)typeSig2.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
									if (flag15)
									{
										BAMLConverterTypeReference reference3 = new BAMLConverterTypeReference(this.xmlnsCtx, typeSig2, rec);
										this.AddTypeSigReference(typeSig2, reference3);
									}
								}
							}
						}
					}
				}
			}
			Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> tuple = this.ResolveAttribute(rec.AttributeId);
			string a = null;
			bool flag16 = tuple.Item1 != null;
			if (flag16)
			{
				a = tuple.Item1.Name;
			}
			else
			{
				bool flag17 = tuple.Item2 != null;
				if (flag17)
				{
					a = tuple.Item2.Name;
				}
			}
			bool flag18 = a == "DisplayMemberPath";
			if (flag18)
			{
				this.AnalyzePropertyPath(rec.Value);
			}
			else
			{
				bool flag19 = a == "Source";
				if (flag19)
				{
					string a2 = null;
					bool flag20 = tuple.Item1 is IMemberDef;
					if (flag20)
					{
						a2 = ((IMemberDef)tuple.Item1).DeclaringType.FullName;
					}
					else
					{
						bool flag21 = tuple.Item2 != null;
						if (flag21)
						{
							a2 = this.ResolveType(tuple.Item2.OwnerTypeId).FullName;
						}
					}
					bool flag22 = a2 == "System.Windows.ResourceDictionary";
					if (flag22)
					{
						string text4 = rec.Value.ToUpperInvariant();
						bool flag23 = text4.EndsWith(".BAML") || text4.EndsWith(".XAML");
						if (flag23)
						{
							Match match = WPFAnalyzer.UriPattern.Match(text4);
							bool success = match.Success;
							if (success)
							{
								string text5 = match.Groups[1].Success ? match.Groups[1].Value : string.Empty;
								bool flag24 = !string.IsNullOrWhiteSpace(text5) && !text5.Equals(this.Module.Assembly.Name.String, StringComparison.OrdinalIgnoreCase);
								if (flag24)
								{
									return;
								}
								text4 = match.Groups[2].Value;
							}
							else
							{
								bool flag25 = rec.Value.Contains("/");
								if (flag25)
								{
									this.context.Logger.WarnFormat("Fail to extract XAML name from '{0}'.", new object[]
									{
										rec.Value
									});
								}
							}
							bool flag26 = !text4.StartsWith(this.packScheme, StringComparison.OrdinalIgnoreCase);
							if (flag26)
							{
								Uri uri = new Uri(new Uri(this.packScheme + "application:,,,/" + this.CurrentBAMLName), text4);
								text4 = uri.LocalPath;
							}
							BAMLPropertyReference value = new BAMLPropertyReference(rec);
							text4 = WebUtility.UrlDecode(text4.TrimStart(new char[]
							{
								'/'
							}));
							string key = text4.Substring(0, text4.Length - 5) + ".BAML";
							string key2 = text4.Substring(0, text4.Length - 5) + ".XAML";
							Dictionary<string, List<IBAMLReference>> bamlRefs = this.service.FindRenamer<WPFAnalyzer>().bamlRefs;
							bamlRefs.AddListEntry(key, value);
							bamlRefs.AddListEntry(key2, value);
						}
					}
				}
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000BE5C File Offset: 0x0000A05C
		private Tuple<IDnlibDef, AttributeInfoRecord, TypeDef> AnalyzeAttributeReference(TypeDef declType, AttributeInfoRecord rec)
		{
			IDnlibDef item = null;
			ITypeDefOrRef typeDefOrRef = null;
			while (declType != null)
			{
				PropertyDef propertyDef = declType.FindProperty(rec.Name);
				bool flag = propertyDef != null;
				if (flag)
				{
					item = propertyDef;
					typeDefOrRef = propertyDef.PropertySig.RetType.ToBasicTypeDefOrRef();
					bool flag2 = this.context.Modules.Contains((ModuleDefMD)declType.Module);
					if (flag2)
					{
						this.service.AddReference<IDnlibDef>(propertyDef, new BAMLAttributeReference(propertyDef, rec));
					}
					break;
				}
				EventDef eventDef = declType.FindEvent(rec.Name);
				bool flag3 = eventDef != null;
				if (flag3)
				{
					item = eventDef;
					typeDefOrRef = eventDef.EventType;
					bool flag4 = this.context.Modules.Contains((ModuleDefMD)declType.Module);
					if (flag4)
					{
						this.service.AddReference<IDnlibDef>(eventDef, new BAMLAttributeReference(eventDef, rec));
					}
					break;
				}
				bool flag5 = declType.BaseType == null;
				if (flag5)
				{
					break;
				}
				declType = declType.BaseType.ResolveTypeDefThrow();
			}
			return Tuple.Create<IDnlibDef, AttributeInfoRecord, TypeDef>(item, rec, (typeDefOrRef == null) ? null : typeDefOrRef.ResolveTypeDefThrow());
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000BF80 File Offset: 0x0000A180
		private void AnalyzePropertyPath(string path)
		{
			SourceValueInfo[] array = this.PathParser.Parse(path);
			foreach (SourceValueInfo sourceValueInfo in array)
			{
				SourceValueType type = sourceValueInfo.type;
				SourceValueType sourceValueType = type;
				if (sourceValueType != SourceValueType.Property)
				{
					if (sourceValueType == SourceValueType.Indexer)
					{
						foreach (IndexerParamInfo indexerParamInfo in sourceValueInfo.paramList)
						{
							bool flag = !string.IsNullOrWhiteSpace(indexerParamInfo.parenString);
							if (flag)
							{
								string text;
								TypeSig typeSig = this.ResolveType(indexerParamInfo.parenString, out text);
								bool flag2 = typeSig != null && this.Context.Modules.Contains((ModuleDefMD)typeSig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
								if (flag2)
								{
									BAMLPathTypeReference reference = new BAMLPathTypeReference(this.xmlnsCtx, typeSig, sourceValueInfo);
									this.AddTypeSigReference(typeSig, reference);
									break;
								}
							}
						}
					}
				}
				else
				{
					string typeName = sourceValueInfo.GetTypeName();
					string propertyName = sourceValueInfo.GetPropertyName();
					bool flag3 = !string.IsNullOrWhiteSpace(typeName);
					if (flag3)
					{
						string text2;
						TypeSig typeSig2 = this.ResolveType(typeName, out text2);
						bool flag4 = typeSig2 != null && this.Context.Modules.Contains((ModuleDefMD)typeSig2.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module);
						if (flag4)
						{
							BAMLPathTypeReference reference2 = new BAMLPathTypeReference(this.xmlnsCtx, typeSig2, sourceValueInfo);
							this.AddTypeSigReference(typeSig2, reference2);
							goto IL_1CE;
						}
					}
					List<PropertyDef> list;
					bool flag5 = this.properties.TryGetValue(propertyName, out list);
					if (flag5)
					{
						foreach (PropertyDef obj in list)
						{
							this.service.SetCanRename(obj, false);
						}
					}
				}
				IL_1CE:;
			}
		}

		// Token: 0x0400005E RID: 94
		private readonly ConfuserContext context;

		// Token: 0x0400005F RID: 95
		private readonly INameService service;

		// Token: 0x04000060 RID: 96
		private readonly Dictionary<string, List<MethodDef>> methods = new Dictionary<string, List<MethodDef>>();

		// Token: 0x04000061 RID: 97
		private readonly Dictionary<string, List<EventDef>> events = new Dictionary<string, List<EventDef>>();

		// Token: 0x04000062 RID: 98
		private readonly Dictionary<string, List<PropertyDef>> properties = new Dictionary<string, List<PropertyDef>>();

		// Token: 0x04000063 RID: 99
		private readonly Dictionary<ushort, AssemblyDef> assemblyRefs = new Dictionary<ushort, AssemblyDef>();

		// Token: 0x04000064 RID: 100
		private readonly Dictionary<ushort, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef>> attrRefs = new Dictionary<ushort, Tuple<IDnlibDef, AttributeInfoRecord, TypeDef>>();

		// Token: 0x04000065 RID: 101
		private readonly Dictionary<ushort, StringInfoRecord> strings = new Dictionary<ushort, StringInfoRecord>();

		// Token: 0x04000066 RID: 102
		private readonly Dictionary<ushort, TypeSig> typeRefs = new Dictionary<ushort, TypeSig>();

		// Token: 0x04000067 RID: 103
		private readonly Dictionary<string, List<Tuple<AssemblyDef, string>>> xmlns = new Dictionary<string, List<Tuple<AssemblyDef, string>>>();

		// Token: 0x04000069 RID: 105
		private readonly string packScheme = PackUriHelper.UriSchemePack + "://";

		// Token: 0x0400006A RID: 106
		private IKnownThings things;

		// Token: 0x0400006B RID: 107
		private KnownThingsv3 thingsv3;

		// Token: 0x0400006C RID: 108
		private KnownThingsv4 thingsv4;

		// Token: 0x0400006D RID: 109
		private BAMLAnalyzer.XmlNsContext xmlnsCtx;

		// Token: 0x0200008F RID: 143
		private class DummyAssemblyRefFinder : IAssemblyRefFinder
		{
			// Token: 0x06000360 RID: 864 RVA: 0x0002A9DF File Offset: 0x00028BDF
			public DummyAssemblyRefFinder(AssemblyDef assemblyDef)
			{
				this.assemblyDef = assemblyDef;
			}

			// Token: 0x06000361 RID: 865 RVA: 0x0002A9F0 File Offset: 0x00028BF0
			public AssemblyRef FindAssemblyRef(TypeRef nonNestedTypeRef)
			{
				return this.assemblyDef.ToAssemblyRef();
			}

			// Token: 0x0400057A RID: 1402
			private readonly AssemblyDef assemblyDef;
		}

		// Token: 0x02000090 RID: 144
		internal class XmlNsContext
		{
			// Token: 0x06000362 RID: 866 RVA: 0x0002AA10 File Offset: 0x00028C10
			public XmlNsContext(BamlDocument doc, Dictionary<ushort, AssemblyDef> assemblyRefs)
			{
				this.doc = doc;
				this.assemblyRefs = new Dictionary<AssemblyDef, ushort>();
				foreach (KeyValuePair<ushort, AssemblyDef> keyValuePair in assemblyRefs)
				{
					this.assemblyRefs[keyValuePair.Value] = keyValuePair.Key;
				}
				for (int i = 0; i < doc.Count; i++)
				{
					bool flag = doc[i] is ElementStartRecord;
					if (flag)
					{
						this.rootIndex = i + 1;
						break;
					}
				}
				Debug.Assert(this.rootIndex != -1);
			}

			// Token: 0x06000363 RID: 867 RVA: 0x0002AAE8 File Offset: 0x00028CE8
			public void AddNsMap(Tuple<AssemblyDef, string> scope, string prefix)
			{
				this.xmlNsMap[scope] = prefix;
			}

			// Token: 0x06000364 RID: 868 RVA: 0x0002AAFC File Offset: 0x00028CFC
			public string GetPrefix(string clrNs, AssemblyDef assembly)
			{
				string text;
				bool flag = !this.xmlNsMap.TryGetValue(Tuple.Create<AssemblyDef, string>(assembly, clrNs), out text);
				if (flag)
				{
					object arg = "_";
					int num = this.x;
					this.x = num + 1;
					text = arg + num;
					ushort num2 = this.assemblyRefs[assembly];
					this.doc.Insert(this.rootIndex, new XmlnsPropertyRecord
					{
						AssemblyIds = new ushort[]
						{
							num2
						},
						Prefix = text,
						XmlNamespace = "clr-namespace:" + clrNs
					});
					this.doc.Insert(this.rootIndex - 1, new PIMappingRecord
					{
						AssemblyId = num2,
						ClrNamespace = clrNs,
						XmlNamespace = "clr-namespace:" + clrNs
					});
					this.rootIndex++;
				}
				return text;
			}

			// Token: 0x0400057B RID: 1403
			private readonly Dictionary<AssemblyDef, ushort> assemblyRefs;

			// Token: 0x0400057C RID: 1404
			private readonly BamlDocument doc;

			// Token: 0x0400057D RID: 1405
			private readonly Dictionary<Tuple<AssemblyDef, string>, string> xmlNsMap = new Dictionary<Tuple<AssemblyDef, string>, string>();

			// Token: 0x0400057E RID: 1406
			private int rootIndex = -1;

			// Token: 0x0400057F RID: 1407
			private int x;
		}
	}
}
