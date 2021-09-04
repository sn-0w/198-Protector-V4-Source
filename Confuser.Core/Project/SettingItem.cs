using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Confuser.Core.Project
{
	// Token: 0x02000084 RID: 132
	public class SettingItem<T> : Dictionary<string, string>
	{
		// Token: 0x06000324 RID: 804 RVA: 0x00003563 File Offset: 0x00001763
		public SettingItem(string id = null, SettingItemAction action = SettingItemAction.Add)
		{
			this.Id = id;
			this.Action = action;
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0000357D File Offset: 0x0000177D
		// (set) Token: 0x06000326 RID: 806 RVA: 0x00003585 File Offset: 0x00001785
		public string Id { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000358E File Offset: 0x0000178E
		// (set) Token: 0x06000328 RID: 808 RVA: 0x00003596 File Offset: 0x00001796
		public SettingItemAction Action { get; set; }

		// Token: 0x06000329 RID: 809 RVA: 0x00013DD8 File Offset: 0x00011FD8
		internal XmlElement Save(XmlDocument xmlDoc)
		{
			XmlElement xmlElement = xmlDoc.CreateElement((typeof(T) == typeof(Packer)) ? "packer" : "protection", "http://confuser.codeplex.com");
			XmlAttribute xmlAttribute = xmlDoc.CreateAttribute("id");
			xmlAttribute.Value = this.Id;
			xmlElement.Attributes.Append(xmlAttribute);
			bool flag = this.Action > SettingItemAction.Add;
			if (flag)
			{
				XmlAttribute xmlAttribute2 = xmlDoc.CreateAttribute("action");
				xmlAttribute2.Value = this.Action.ToString().ToLower();
				xmlElement.Attributes.Append(xmlAttribute2);
			}
			foreach (KeyValuePair<string, string> keyValuePair in this)
			{
				XmlElement xmlElement2 = xmlDoc.CreateElement("argument", "http://confuser.codeplex.com");
				XmlAttribute xmlAttribute3 = xmlDoc.CreateAttribute("name");
				xmlAttribute3.Value = keyValuePair.Key;
				xmlElement2.Attributes.Append(xmlAttribute3);
				XmlAttribute xmlAttribute4 = xmlDoc.CreateAttribute("value");
				xmlAttribute4.Value = keyValuePair.Value;
				xmlElement2.Attributes.Append(xmlAttribute4);
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00013F44 File Offset: 0x00012144
		internal void Load(XmlElement elem)
		{
			this.Id = elem.Attributes["id"].Value;
			bool flag = elem.Attributes["action"] != null;
			if (flag)
			{
				this.Action = (SettingItemAction)Enum.Parse(typeof(SettingItemAction), elem.Attributes["action"].Value, true);
			}
			else
			{
				this.Action = SettingItemAction.Add;
			}
			base.Clear();
			foreach (XmlElement xmlElement in elem.ChildNodes.OfType<XmlElement>())
			{
				base.Add(xmlElement.Attributes["name"].Value, xmlElement.Attributes["value"].Value);
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00014038 File Offset: 0x00012238
		public SettingItem<T> Clone()
		{
			SettingItem<T> settingItem = new SettingItem<T>(this.Id, this.Action);
			foreach (KeyValuePair<string, string> keyValuePair in this)
			{
				settingItem.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return settingItem;
		}
	}
}
