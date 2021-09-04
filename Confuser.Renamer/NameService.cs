using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer.Analyzers;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x0200000B RID: 11
	internal class NameService : INameService
	{
		// Token: 0x0600003B RID: 59 RVA: 0x000071A8 File Offset: 0x000053A8
		public NameService(ConfuserContext context)
		{
			this.mscorlib = ModuleDefMD.Load(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "mscorlib.dll"), null);
			this.UsedNames = new List<string>();
			this.CryproUsedNames = new List<string>();
			this.context = context;
			bool flag = context.InputSymbolMap != null;
			if (flag)
			{
				this.LoadInputSymbolMap(context.InputSymbolMap);
			}
			this.storage = new VTableStorage(context.Logger);
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.Rename");
			this.nameSeed = this.random.NextBytes(20);
			this.Renamers = new List<IRenamer>
			{
				new InterReferenceAnalyzer(),
				new VTableAnalyzer(),
				new TypeBlobAnalyzer(),
				new ResourceAnalyzer(),
				new LdtokenEnumAnalyzer(),
				new ReflectionAnalyzer()
			};
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00007336 File Offset: 0x00005536
		// (set) Token: 0x0600003D RID: 61 RVA: 0x0000733E File Offset: 0x0000553E
		public IList<IRenamer> Renamers { get; private set; }

		// Token: 0x0600003E RID: 62 RVA: 0x00007348 File Offset: 0x00005548
		public VTableStorage GetVTables()
		{
			return this.storage;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00007360 File Offset: 0x00005560
		public bool CanRename(object obj)
		{
			bool flag = obj is IDnlibDef;
			bool result;
			if (flag)
			{
				bool flag2 = this.analyze == null;
				if (flag2)
				{
					this.analyze = this.context.Pipeline.FindPhase<AnalyzePhase>();
				}
				NameProtection key = (NameProtection)this.analyze.Parent;
				ProtectionSettings parameters = ProtectionParameters.GetParameters(this.context, (IDnlibDef)obj);
				bool flag3 = parameters == null || !parameters.ContainsKey(key);
				result = (!flag3 && this.context.Annotations.Get<bool>(obj, NameService.CanRenameKey, true));
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000073FF File Offset: 0x000055FF
		public void SetCanRename(object obj, bool val)
		{
			this.context.Annotations.Set<bool>(obj, NameService.CanRenameKey, val);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000741C File Offset: 0x0000561C
		public void SetParam(IDnlibDef def, string name, string value)
		{
			ProtectionSettings protectionSettings = ProtectionParameters.GetParameters(this.context, def);
			bool flag = protectionSettings == null;
			if (flag)
			{
				ProtectionParameters.SetParameters(this.context, def, protectionSettings = new ProtectionSettings());
			}
			Dictionary<string, string> dictionary;
			bool flag2 = !protectionSettings.TryGetValue(this.analyze.Parent, out dictionary);
			if (flag2)
			{
				dictionary = (protectionSettings[this.analyze.Parent] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
			}
			dictionary[name] = value;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00007494 File Offset: 0x00005694
		public string GetParam(IDnlibDef def, string name)
		{
			ProtectionSettings parameters = ProtectionParameters.GetParameters(this.context, def);
			bool flag = parameters == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Dictionary<string, string> dictionary;
				bool flag2 = !parameters.TryGetValue(this.analyze.Parent, out dictionary);
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = dictionary.GetValueOrDefault(name, null);
				}
			}
			return result;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000074E8 File Offset: 0x000056E8
		public RenameMode GetRenameMode(object obj)
		{
			return this.context.Annotations.Get<RenameMode>(obj, NameService.RenameModeKey, RenameMode.RealNames);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00007515 File Offset: 0x00005715
		public void SetRenameMode(object obj, RenameMode val)
		{
			this.context.Annotations.Set<RenameMode>(obj, NameService.RenameModeKey, val);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00007530 File Offset: 0x00005730
		public void ReduceRenameMode(object obj, RenameMode val)
		{
			RenameMode renameMode = this.GetRenameMode(obj);
			bool flag = renameMode < val;
			if (flag)
			{
				this.context.Annotations.Set<RenameMode>(obj, NameService.RenameModeKey, val);
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00007568 File Offset: 0x00005768
		public void AddReference<T>(T obj, INameReference<T> reference)
		{
			this.context.Annotations.GetOrCreate<List<INameReference>>(obj, NameService.ReferencesKey, (object key) => new List<INameReference>()).Add(reference);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000075B8 File Offset: 0x000057B8
		public void Analyze(IDnlibDef def)
		{
			bool flag = this.analyze == null;
			if (flag)
			{
				this.analyze = this.context.Pipeline.FindPhase<AnalyzePhase>();
			}
			this.SetOriginalName(def, def.Name);
			bool flag2 = def is TypeDef;
			if (flag2)
			{
				this.GetVTables().GetVTable((TypeDef)def);
				this.SetOriginalNamespace(def, ((TypeDef)def).Namespace);
			}
			this.analyze.Analyze(this, this.context, ProtectionParameters.Empty, def, true);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00007650 File Offset: 0x00005850
		public void SetNameId(uint id)
		{
			for (int i = this.nameId.Length - 1; i >= 0; i--)
			{
				this.nameId[i] = (byte)(id & 255U);
				id >>= 8;
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00007694 File Offset: 0x00005894
		private void IncrementNameId()
		{
			for (int i = this.nameId.Length - 1; i >= 0; i--)
			{
				byte[] array = this.nameId;
				int num = i;
				array[num] += 1;
				bool flag = this.nameId[i] > 0;
				if (flag)
				{
					break;
				}
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000076E4 File Offset: 0x000058E4
		private string ObfuscateNameInternal(byte[] hash, RenameMode mode)
		{
			if (mode <= RenameMode.Decodable)
			{
				switch (mode)
				{
				case RenameMode.Empty:
					return "";
				case RenameMode.Unicode:
					return Utils.EncodeString(hash, NameService.unicodeCharset) + "‮";
				case RenameMode.ASCII:
					return Utils.EncodeString(hash, NameService.asciiCharset);
				case RenameMode.Letters:
					return Utils.EncodeString(hash, NameService.letterCharset);
				default:
					if (mode == RenameMode.Decodable)
					{
						this.IncrementNameId();
						return "_" + Utils.EncodeString(hash, NameService.alphaNumCharset);
					}
					break;
				}
			}
			else
			{
				if (mode == RenameMode.Sequential)
				{
					this.IncrementNameId();
					return "_" + Utils.EncodeString(this.nameId, NameService.alphaNumCharset);
				}
				if (mode == RenameMode.MSCorLib)
				{
					int count = this.mscorlib.Types.Count;
					TypeDef typeDef = this.mscorlib.Types[this.random.NextInt32(count)];
					while (this.UsedNames.Contains(typeDef.Name))
					{
						typeDef = this.mscorlib.Types[this.random.NextInt32(count)];
					}
					this.UsedNames.Add(typeDef.Name);
					return typeDef.Name;
				}
				if (mode == RenameMode.CryptoObfuscator)
				{
					int index = this.random.NextInt32(0, CryptoObfuscatorHelper.instru.Count);
					int index2 = this.random.NextInt32(0, CryptoObfuscatorHelper.calc.Count);
					int index3 = this.random.NextInt32(0, CryptoObfuscatorHelper.Type.Count);
					string text = string.Format("{0}{1}{2}", CryptoObfuscatorHelper.instru[index], CryptoObfuscatorHelper.calc[index2], CryptoObfuscatorHelper.Type[index3]);
					while (this.CryproUsedNames.Contains(text))
					{
						text = string.Format("{0}{1}{2}", CryptoObfuscatorHelper.instru[index], CryptoObfuscatorHelper.calc[index2], CryptoObfuscatorHelper.Type[index3]);
					}
					this.CryproUsedNames.Add(text);
					return text;
				}
			}
			throw new NotSupportedException("Rename mode '" + mode + "' is not supported.");
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00007954 File Offset: 0x00005B54
		private string ParseGenericName(string name, out int? count)
		{
			bool flag = name.LastIndexOf('`') != -1;
			if (flag)
			{
				int num = name.LastIndexOf('`');
				int value;
				bool flag2 = int.TryParse(name.Substring(num + 1), out value);
				if (flag2)
				{
					count = new int?(value);
					return name.Substring(0, num);
				}
			}
			count = null;
			return name;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000079BC File Offset: 0x00005BBC
		private string MakeGenericName(string name, int? count)
		{
			bool flag = count == null;
			string result;
			if (flag)
			{
				result = name;
			}
			else
			{
				result = string.Format("{0}`{1}", name, count.Value);
			}
			return result;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000079F8 File Offset: 0x00005BF8
		public string ObfuscateName(string name, RenameMode mode)
		{
			string text = null;
			int? count;
			name = this.ParseGenericName(name, out count);
			bool flag = string.IsNullOrEmpty(name);
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = mode == RenameMode.RealNames;
				if (flag2)
				{
					this._index++;
					bool flag3 = this._index >= this._names.Count;
					if (flag3)
					{
						this.ObfuscateName(name, RenameMode.Letters);
					}
					result = this._names[this._index];
				}
				else
				{
					bool flag4 = mode == RenameMode.Empty;
					if (flag4)
					{
						result = "";
					}
					else
					{
						bool flag5 = mode == RenameMode.Debug;
						if (flag5)
						{
							result = "_" + name;
						}
						else
						{
							bool flag6 = mode == RenameMode.Reversible;
							if (flag6)
							{
								bool flag7 = this.reversibleRenamer == null;
								if (flag7)
								{
									throw new ArgumentException("Password not provided for reversible renaming.");
								}
								text = this.reversibleRenamer.Encrypt(name);
								result = this.MakeGenericName(text, count);
							}
							else
							{
								bool flag8 = this.nameMap1.ContainsKey(name);
								if (flag8)
								{
									result = this.MakeGenericName(this.nameMap1[name], count);
								}
								else
								{
									byte[] array = Utils.Xor(Utils.SHA1(Encoding.UTF8.GetBytes(name)), this.nameSeed);
									for (int i = 0; i < 100; i++)
									{
										text = this.ObfuscateNameInternal(array, mode);
										bool flag9 = !this.identifiers.Contains(this.MakeGenericName(text, count)) && !this.nameMap2.ContainsKey(text);
										if (flag9)
										{
											break;
										}
										array = Utils.SHA1(array);
									}
									bool flag10 = (mode & RenameMode.Decodable) > RenameMode.Empty;
									if (flag10)
									{
										this.nameMap2[text] = name;
										this.nameMap1[name] = text;
									}
									result = this.MakeGenericName(text, count);
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00007BD1 File Offset: 0x00005DD1
		public string RandomName()
		{
			return this.RandomName(RenameMode.RealNames);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00007BE0 File Offset: 0x00005DE0
		public string RandomName(RenameMode mode)
		{
			return this.ObfuscateName(Utils.ToHexString(this.random.NextBytes(16)), mode);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00007C0B File Offset: 0x00005E0B
		public void SetOriginalName(object obj, string name)
		{
			this.identifiers.Add(name);
			this.context.Annotations.Set<string>(obj, NameService.OriginalNameKey, name);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00007C33 File Offset: 0x00005E33
		public void SetOriginalNamespace(object obj, string ns)
		{
			this.identifiers.Add(ns);
			this.context.Annotations.Set<string>(obj, NameService.OriginalNamespaceKey, ns);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00007C5B File Offset: 0x00005E5B
		public void RegisterRenamer(IRenamer renamer)
		{
			this.Renamers.Add(renamer);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00007C6C File Offset: 0x00005E6C
		public T FindRenamer<T>()
		{
			return this.Renamers.OfType<T>().Single<T>();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00007C90 File Offset: 0x00005E90
		public void MarkHelper(IDnlibDef def, IMarkerService marker, ConfuserComponent parentComp)
		{
			bool flag = marker.IsMarked(def);
			if (!flag)
			{
				bool flag2 = def is MethodDef;
				if (flag2)
				{
					MethodDef methodDef = (MethodDef)def;
					methodDef.Access = MethodAttributes.Assembly;
					bool flag3 = !methodDef.IsSpecialName && !methodDef.IsRuntimeSpecialName && !methodDef.DeclaringType.IsDelegate();
					if (flag3)
					{
						methodDef.Name = this.RandomName();
					}
				}
				else
				{
					bool flag4 = def is FieldDef;
					if (flag4)
					{
						FieldDef fieldDef = (FieldDef)def;
						fieldDef.Access = FieldAttributes.Assembly;
						bool flag5 = !fieldDef.IsSpecialName && !fieldDef.IsRuntimeSpecialName;
						if (flag5)
						{
							fieldDef.Name = this.RandomName();
						}
					}
					else
					{
						bool flag6 = def is TypeDef;
						if (flag6)
						{
							TypeDef typeDef = (TypeDef)def;
							typeDef.Visibility = ((typeDef.DeclaringType == null) ? TypeAttributes.NotPublic : TypeAttributes.NestedAssembly);
							typeDef.Namespace = "";
							bool flag7 = !typeDef.IsSpecialName && !typeDef.IsRuntimeSpecialName;
							if (flag7)
							{
								typeDef.Name = this.RandomName();
							}
						}
					}
				}
				this.SetCanRename(def, false);
				this.Analyze(def);
				marker.Mark(def, parentComp);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00007DE4 File Offset: 0x00005FE4
		public RandomGenerator GetRandom()
		{
			return this.random;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00007DEC File Offset: 0x00005FEC
		public IList<INameReference> GetReferences(object obj)
		{
			return this.context.Annotations.GetLazy<List<INameReference>>(obj, NameService.ReferencesKey, (object key) => new List<INameReference>());
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00007E23 File Offset: 0x00006023
		public string GetOriginalName(object obj)
		{
			return this.context.Annotations.Get<string>(obj, NameService.OriginalNameKey, "");
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00007E40 File Offset: 0x00006040
		public string GetOriginalNamespace(object obj)
		{
			return this.context.Annotations.Get<string>(obj, NameService.OriginalNamespaceKey, "");
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00007E5D File Offset: 0x0000605D
		public ICollection<KeyValuePair<string, string>> GetNameMap()
		{
			return this.nameMap2;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00007E68 File Offset: 0x00006068
		private void LoadInputSymbolMap(string inputSymbolMapPath)
		{
			int num = 0;
			foreach (string text in File.ReadAllLines(inputSymbolMapPath))
			{
				num++;
				bool flag = string.IsNullOrWhiteSpace(text);
				if (!flag)
				{
					string[] array2 = text.Split(new char[]
					{
						'\t'
					});
					bool flag2 = array2.Length != 2;
					if (flag2)
					{
						throw new FileFormatException(string.Format("Cannot read input symbol map {0}:{1}", inputSymbolMapPath, num));
					}
					string text2 = array2[0];
					string text3 = array2[1];
					this.nameMap2.Add(text2, text3);
					this.nameMap1.Add(text3, text2);
				}
			}
		}

		// Token: 0x04000009 RID: 9
		private static readonly object CanRenameKey = new object();

		// Token: 0x0400000A RID: 10
		private static readonly object RenameModeKey = new object();

		// Token: 0x0400000B RID: 11
		private static readonly object ReferencesKey = new object();

		// Token: 0x0400000C RID: 12
		private static readonly object OriginalNameKey = new object();

		// Token: 0x0400000D RID: 13
		private static readonly object OriginalNamespaceKey = new object();

		// Token: 0x0400000E RID: 14
		private readonly ConfuserContext context;

		// Token: 0x0400000F RID: 15
		private readonly byte[] nameSeed;

		// Token: 0x04000010 RID: 16
		private readonly RandomGenerator random;

		// Token: 0x04000011 RID: 17
		private readonly VTableStorage storage;

		// Token: 0x04000012 RID: 18
		private AnalyzePhase analyze;

		// Token: 0x04000013 RID: 19
		private readonly HashSet<string> identifiers = new HashSet<string>();

		// Token: 0x04000014 RID: 20
		private readonly byte[] nameId = new byte[8];

		// Token: 0x04000015 RID: 21
		private readonly Dictionary<string, string> nameMap1 = new Dictionary<string, string>();

		// Token: 0x04000016 RID: 22
		private readonly Dictionary<string, string> nameMap2 = new Dictionary<string, string>();

		// Token: 0x04000017 RID: 23
		internal ReversibleRenamer reversibleRenamer;

		// Token: 0x04000018 RID: 24
		public ModuleDefMD mscorlib;

		// Token: 0x04000019 RID: 25
		public List<string> UsedNames;

		// Token: 0x0400001A RID: 26
		public List<string> CryproUsedNames;

		// Token: 0x0400001C RID: 28
		private int _index;

		// Token: 0x0400001D RID: 29
		private List<string> _names = (from x in File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Names.txt")
		orderby x.Length
		orderby Guid.NewGuid().ToString()
		select x).ToList<string>();

		// Token: 0x0400001E RID: 30
		private static readonly char[] asciiCharset = (from ord in Enumerable.Range(32, 95)
		select (char)ord).Except(new char[]
		{
			'.'
		}).ToArray<char>();

		// Token: 0x0400001F RID: 31
		private static readonly char[] letterCharset = Enumerable.Range(0, 26).SelectMany((int ord) => new char[]
		{
			(char)(97 + ord),
			(char)(65 + ord)
		}).ToArray<char>();

		// Token: 0x04000020 RID: 32
		private static readonly char[] alphaNumCharset = Enumerable.Range(0, 26).SelectMany((int ord) => new char[]
		{
			(char)(97 + ord),
			(char)(65 + ord)
		}).Concat(from ord in Enumerable.Range(0, 10)
		select (char)(48 + ord)).ToArray<char>();

		// Token: 0x04000021 RID: 33
		private static readonly char[] unicodeCharset = new char[0].Concat(from ord in Enumerable.Range(8203, 5)
		select (char)ord).Concat(from ord in Enumerable.Range(8233, 6)
		select (char)ord).Concat(from ord in Enumerable.Range(8298, 6)
		select (char)ord).Except(new char[]
		{
			'\u2029'
		}).ToArray<char>();
	}
}
