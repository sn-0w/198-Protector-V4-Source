using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;
using dnlib.DotNet;
using dnlib.DotNet.Pdb;

namespace Confuser.Core
{
	// Token: 0x0200004D RID: 77
	public class Marker
	{
		// Token: 0x060001E5 RID: 485 RVA: 0x0000E1BC File Offset: 0x0000C3BC
		public virtual void Initalize(IList<Protection> protections, IList<Packer> packers)
		{
			this.protections = protections.ToDictionary((Protection prot) => prot.Id, (Protection prot) => prot, StringComparer.OrdinalIgnoreCase);
			this.packers = packers.ToDictionary((Packer packer) => packer.Id, (Packer packer) => packer, StringComparer.OrdinalIgnoreCase);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000E268 File Offset: 0x0000C468
		private void FillPreset(ProtectionPreset preset, ProtectionSettings settings)
		{
			foreach (Protection protection in this.protections.Values)
			{
				bool flag = protection.Preset != ProtectionPreset.None && protection.Preset <= preset && !settings.ContainsKey(protection);
				if (flag)
				{
					settings.Add(protection, new Dictionary<string, string>());
				}
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000E2EC File Offset: 0x0000C4EC
		public static StrongNameKey LoadSNKey(ConfuserContext context, string path, string pass)
		{
			bool flag = path == null;
			StrongNameKey result;
			if (flag)
			{
				result = null;
			}
			else
			{
				try
				{
					bool flag2 = pass != null;
					if (flag2)
					{
						X509Certificate2 x509Certificate = new X509Certificate2();
						x509Certificate.Import(path, pass, X509KeyStorageFlags.Exportable);
						RSACryptoServiceProvider rsacryptoServiceProvider = x509Certificate.PrivateKey as RSACryptoServiceProvider;
						bool flag3 = rsacryptoServiceProvider == null;
						if (flag3)
						{
							throw new ArgumentException("RSA key does not present in the certificate.", "path");
						}
						result = new StrongNameKey(rsacryptoServiceProvider.ExportCspBlob(true));
					}
					else
					{
						result = new StrongNameKey(path);
					}
				}
				catch (Exception ex)
				{
					context.Logger.ErrorException("Cannot load the Strong Name Key located at: " + path, ex);
					throw new ConfuserException(ex);
				}
			}
			return result;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000E39C File Offset: 0x0000C59C
		protected internal virtual MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			Packer packer = null;
			Dictionary<string, string> dictionary = null;
			bool flag = proj.Packer != null;
			if (flag)
			{
				bool flag2 = !this.packers.ContainsKey(proj.Packer.Id);
				if (flag2)
				{
					context.Logger.ErrorFormat("Cannot find packer with ID '{0}'.", new object[]
					{
						proj.Packer.Id
					});
					throw new ConfuserException(null);
				}
				bool debug = proj.Debug;
				if (debug)
				{
					context.Logger.Warn("Generated Debug symbols might not be usable with packers!");
				}
				packer = this.packers[proj.Packer.Id];
				dictionary = new Dictionary<string, string>(proj.Packer, StringComparer.OrdinalIgnoreCase);
			}
			List<Tuple<ProjectModule, ModuleDefMD>> list = new List<Tuple<ProjectModule, ModuleDefMD>>();
			List<byte[]> list2 = new List<byte[]>();
			foreach (ProjectModule projectModule in proj)
			{
				bool isExternal = projectModule.IsExternal;
				if (isExternal)
				{
					list2.Add(projectModule.LoadRaw(proj.BaseDirectory));
				}
				else
				{
					ModuleDefMD moduleDefMD = projectModule.Resolve(proj.BaseDirectory, context.Resolver.DefaultModuleContext);
					context.CheckCancellation();
					bool debug2 = proj.Debug;
					if (debug2)
					{
						moduleDefMD.LoadPdb();
					}
					context.Resolver.AddToCache(moduleDefMD);
					list.Add(Tuple.Create<ProjectModule, ModuleDefMD>(projectModule, moduleDefMD));
				}
			}
			foreach (Tuple<ProjectModule, ModuleDefMD> tuple in list)
			{
				context.Logger.InfoFormat("Loading '{0}'...", new object[]
				{
					tuple.Item1.Path
				});
				Dictionary<Rule, PatternExpression> dictionary2 = this.ParseRules(proj, tuple.Item1, context);
				context.Annotations.Set<StrongNameKey>(tuple.Item2, Marker.SNKey, Marker.LoadSNKey(context, (tuple.Item1.SNKeyPath == null) ? null : Path.Combine(proj.BaseDirectory, tuple.Item1.SNKeyPath), tuple.Item1.SNKeyPassword));
				context.Annotations.Set<Dictionary<Rule, PatternExpression>>(tuple.Item2, Marker.RulesKey, dictionary2);
				context.Annotations.Set<string>(tuple.Item2, Marker.SubDirKey, tuple.Item1.BelongsToSubFolder);
				foreach (IDnlibDef dnlibDef in tuple.Item2.FindDefinitions())
				{
					this.ApplyRules(context, dnlibDef, dictionary2, null);
					MethodDef methodDef = dnlibDef as MethodDef;
					bool flag3 = methodDef != null;
					if (flag3)
					{
						IList<PdbCustomDebugInfo> customDebugInfos = methodDef.CustomDebugInfos;
					}
					context.CheckCancellation();
				}
				bool flag4 = dictionary != null;
				if (flag4)
				{
					ProtectionParameters.GetParameters(context, tuple.Item2)[packer] = dictionary;
				}
			}
			return new MarkerResult((from module in list
			select module.Item2).ToList<ModuleDefMD>(), packer, list2);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000E718 File Offset: 0x0000C918
		protected internal virtual void MarkMember(IDnlibDef member, ConfuserContext context)
		{
			ModuleDef module = ((IMemberRef)member).Module;
			Dictionary<Rule, PatternExpression> rules = context.Annotations.Get<Dictionary<Rule, PatternExpression>>(module, Marker.RulesKey, null);
			this.ApplyRules(context, member, rules, null);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000E750 File Offset: 0x0000C950
		protected Dictionary<Rule, PatternExpression> ParseRules(ConfuserProject proj, ProjectModule module, ConfuserContext context)
		{
			Dictionary<Rule, PatternExpression> dictionary = new Dictionary<Rule, PatternExpression>();
			PatternParser patternParser = new PatternParser();
			foreach (Rule rule in proj.Rules.Concat(module.Rules))
			{
				try
				{
					dictionary.Add(rule, patternParser.Parse(rule.Pattern));
				}
				catch (InvalidPatternException ex)
				{
					context.Logger.ErrorFormat("Invalid rule pattern: " + rule.Pattern + ".", new object[]
					{
						ex
					});
					throw new ConfuserException(ex);
				}
				foreach (SettingItem<Protection> settingItem in rule)
				{
					bool flag = !this.protections.ContainsKey(settingItem.Id);
					if (flag)
					{
						context.Logger.ErrorFormat("Cannot find protection with ID '{0}'.", new object[]
						{
							settingItem.Id
						});
						throw new ConfuserException(null);
					}
				}
			}
			return dictionary;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000E89C File Offset: 0x0000CA9C
		protected void ApplyRules(ConfuserContext context, IDnlibDef target, Dictionary<Rule, PatternExpression> rules, ProtectionSettings baseSettings = null)
		{
			ProtectionSettings protectionSettings = (baseSettings == null) ? new ProtectionSettings() : new ProtectionSettings(baseSettings);
			foreach (KeyValuePair<Rule, PatternExpression> keyValuePair in rules)
			{
				bool flag = !(bool)keyValuePair.Value.Evaluate(target);
				if (!flag)
				{
					bool flag2 = !keyValuePair.Key.Inherit;
					if (flag2)
					{
						protectionSettings.Clear();
					}
					this.FillPreset(keyValuePair.Key.Preset, protectionSettings);
					foreach (SettingItem<Protection> settingItem in keyValuePair.Key)
					{
						bool flag3 = settingItem.Action == SettingItemAction.Add;
						if (flag3)
						{
							protectionSettings[this.protections[settingItem.Id]] = new Dictionary<string, string>(settingItem, StringComparer.OrdinalIgnoreCase);
						}
						else
						{
							protectionSettings.Remove(this.protections[settingItem.Id]);
						}
					}
				}
			}
			ProtectionParameters.SetParameters(context, target, protectionSettings);
		}

		// Token: 0x0400018C RID: 396
		public static readonly object SNKey = new object();

		// Token: 0x0400018D RID: 397
		public static readonly object RulesKey = new object();

		// Token: 0x0400018E RID: 398
		public static readonly object SubDirKey = new object();

		// Token: 0x0400018F RID: 399
		protected Dictionary<string, Packer> packers;

		// Token: 0x04000190 RID: 400
		protected Dictionary<string, Protection> protections;
	}
}
