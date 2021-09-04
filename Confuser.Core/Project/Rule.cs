using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Confuser.Core.Project
{
	// Token: 0x02000085 RID: 133
	public class Rule : List<SettingItem<Protection>>
	{
		// Token: 0x0600032C RID: 812 RVA: 0x0000359F File Offset: 0x0000179F
		public Rule(string pattern = "true", ProtectionPreset preset = ProtectionPreset.None, bool inherit = false)
		{
			this.Pattern = pattern;
			this.Preset = preset;
			this.Inherit = inherit;
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600032D RID: 813 RVA: 0x000035C1 File Offset: 0x000017C1
		// (set) Token: 0x0600032E RID: 814 RVA: 0x000035C9 File Offset: 0x000017C9
		public string Pattern { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600032F RID: 815 RVA: 0x000035D2 File Offset: 0x000017D2
		// (set) Token: 0x06000330 RID: 816 RVA: 0x000035DA File Offset: 0x000017DA
		public ProtectionPreset Preset { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000331 RID: 817 RVA: 0x000035E3 File Offset: 0x000017E3
		// (set) Token: 0x06000332 RID: 818 RVA: 0x000035EB File Offset: 0x000017EB
		public bool Inherit { get; set; }

		// Token: 0x06000333 RID: 819 RVA: 0x000140B0 File Offset: 0x000122B0
		internal XmlElement Save(XmlDocument xmlDoc)
		{
			XmlElement xmlElement = xmlDoc.CreateElement("rule", "http://confuser.codeplex.com");
			XmlAttribute xmlAttribute = xmlDoc.CreateAttribute("pattern");
			xmlAttribute.Value = this.Pattern;
			xmlElement.Attributes.Append(xmlAttribute);
			bool flag = this.Preset > ProtectionPreset.None;
			if (flag)
			{
				XmlAttribute xmlAttribute2 = xmlDoc.CreateAttribute("preset");
				xmlAttribute2.Value = this.Preset.ToString().ToLower();
				xmlElement.Attributes.Append(xmlAttribute2);
			}
			bool flag2 = !this.Inherit;
			if (flag2)
			{
				XmlAttribute xmlAttribute3 = xmlDoc.CreateAttribute("inherit");
				xmlAttribute3.Value = this.Inherit.ToString().ToLower();
				xmlElement.Attributes.Append(xmlAttribute3);
			}
			foreach (SettingItem<Protection> settingItem in this)
			{
				xmlElement.AppendChild(settingItem.Save(xmlDoc));
			}
			return xmlElement;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x000141DC File Offset: 0x000123DC
		internal void Load(XmlElement elem)
		{
			this.Pattern = elem.Attributes["pattern"].Value;
			bool flag = elem.Attributes["preset"] != null;
			if (flag)
			{
				this.Preset = (ProtectionPreset)Enum.Parse(typeof(ProtectionPreset), elem.Attributes["preset"].Value, true);
			}
			else
			{
				this.Preset = ProtectionPreset.None;
			}
			bool flag2 = elem.Attributes["inherit"] != null;
			if (flag2)
			{
				this.Inherit = bool.Parse(elem.Attributes["inherit"].Value);
			}
			else
			{
				this.Inherit = true;
			}
			base.Clear();
			foreach (XmlElement elem2 in elem.ChildNodes.OfType<XmlElement>())
			{
				SettingItem<Protection> settingItem = new SettingItem<Protection>(null, SettingItemAction.Add);
				settingItem.Load(elem2);
				base.Add(settingItem);
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00014300 File Offset: 0x00012500
		public Rule Clone()
		{
			Rule rule = new Rule("true", ProtectionPreset.None, false);
			rule.Preset = this.Preset;
			rule.Pattern = this.Pattern;
			rule.Inherit = this.Inherit;
			foreach (SettingItem<Protection> settingItem in this)
			{
				SettingItem<Protection> settingItem2 = new SettingItem<Protection>(null, SettingItemAction.Add);
				settingItem2.Id = settingItem.Id;
				settingItem2.Action = settingItem.Action;
				foreach (string key in settingItem.Keys)
				{
					settingItem2.Add(key, settingItem[key]);
				}
				rule.Add(settingItem2);
			}
			return rule;
		}
	}
}
