using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using dnlib.DotNet;

namespace Confuser.Core.Project
{
	// Token: 0x02000082 RID: 130
	public class ProjectModule
	{
		// Token: 0x06000311 RID: 785 RVA: 0x000034E7 File Offset: 0x000016E7
		public ProjectModule()
		{
			this.Rules = new List<Rule>();
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000312 RID: 786 RVA: 0x000034FD File Offset: 0x000016FD
		// (set) Token: 0x06000313 RID: 787 RVA: 0x00003505 File Offset: 0x00001705
		public string Path { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000314 RID: 788 RVA: 0x0000350E File Offset: 0x0000170E
		// (set) Token: 0x06000315 RID: 789 RVA: 0x00003516 File Offset: 0x00001716
		public bool IsExternal { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000316 RID: 790 RVA: 0x0000351F File Offset: 0x0000171F
		// (set) Token: 0x06000317 RID: 791 RVA: 0x00003527 File Offset: 0x00001727
		public string SNKeyPath { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000318 RID: 792 RVA: 0x00003530 File Offset: 0x00001730
		// (set) Token: 0x06000319 RID: 793 RVA: 0x00003538 File Offset: 0x00001738
		public string SNKeyPassword { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600031A RID: 794 RVA: 0x00003541 File Offset: 0x00001741
		// (set) Token: 0x0600031B RID: 795 RVA: 0x00003549 File Offset: 0x00001749
		public string BelongsToSubFolder { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600031C RID: 796 RVA: 0x00003552 File Offset: 0x00001752
		// (set) Token: 0x0600031D RID: 797 RVA: 0x0000355A File Offset: 0x0000175A
		public IList<Rule> Rules { get; private set; }

		// Token: 0x0600031E RID: 798 RVA: 0x00013968 File Offset: 0x00011B68
		public ModuleDefMD Resolve(string basePath, ModuleContext context = null)
		{
			bool flag = basePath == null;
			ModuleDefMD result;
			if (flag)
			{
				result = ModuleDefMD.Load(this.Path, context);
			}
			else
			{
				result = ModuleDefMD.Load(System.IO.Path.Combine(basePath, this.Path), context);
			}
			return result;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x000139A4 File Offset: 0x00011BA4
		public byte[] LoadRaw(string basePath)
		{
			bool flag = basePath == null;
			byte[] result;
			if (flag)
			{
				result = File.ReadAllBytes(this.Path);
			}
			else
			{
				result = File.ReadAllBytes(System.IO.Path.Combine(basePath, this.Path));
			}
			return result;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x000139E0 File Offset: 0x00011BE0
		internal XmlElement Save(XmlDocument xmlDoc)
		{
			XmlElement xmlElement = xmlDoc.CreateElement("module", "http://confuser.codeplex.com");
			XmlAttribute xmlAttribute = xmlDoc.CreateAttribute("path");
			xmlAttribute.Value = this.Path;
			xmlElement.Attributes.Append(xmlAttribute);
			bool isExternal = this.IsExternal;
			if (isExternal)
			{
				XmlAttribute xmlAttribute2 = xmlDoc.CreateAttribute("external");
				xmlAttribute2.Value = (this.IsExternal ? "true" : "false");
				xmlElement.Attributes.Append(xmlAttribute2);
			}
			bool flag = this.SNKeyPath != null;
			if (flag)
			{
				XmlAttribute xmlAttribute3 = xmlDoc.CreateAttribute("snKey");
				xmlAttribute3.Value = this.SNKeyPath;
				xmlElement.Attributes.Append(xmlAttribute3);
			}
			bool flag2 = this.SNKeyPassword != null;
			if (flag2)
			{
				XmlAttribute xmlAttribute4 = xmlDoc.CreateAttribute("snKeyPass");
				xmlAttribute4.Value = this.SNKeyPassword;
				xmlElement.Attributes.Append(xmlAttribute4);
			}
			bool flag3 = this.BelongsToSubFolder != null;
			if (flag3)
			{
				XmlAttribute xmlAttribute5 = xmlDoc.CreateAttribute("belongsToSubFolder");
				xmlAttribute5.Value = this.BelongsToSubFolder;
				xmlElement.Attributes.Append(xmlAttribute5);
			}
			foreach (Rule rule in this.Rules)
			{
				xmlElement.AppendChild(rule.Save(xmlDoc));
			}
			return xmlElement;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00013B6C File Offset: 0x00011D6C
		internal void Load(XmlElement elem)
		{
			this.Path = elem.Attributes["path"].Value;
			bool flag = elem.Attributes["external"] != null;
			if (flag)
			{
				this.IsExternal = bool.Parse(elem.Attributes["external"].Value);
			}
			else
			{
				this.IsExternal = false;
			}
			bool flag2 = elem.Attributes["snKey"] != null;
			if (flag2)
			{
				this.SNKeyPath = elem.Attributes["snKey"].Value.NullIfEmpty();
			}
			else
			{
				this.SNKeyPath = null;
			}
			bool flag3 = elem.Attributes["snKeyPass"] != null;
			if (flag3)
			{
				this.SNKeyPassword = elem.Attributes["snKeyPass"].Value.NullIfEmpty();
			}
			else
			{
				this.SNKeyPassword = null;
			}
			bool flag4 = elem.Attributes["belongsToSubFolder"] != null;
			if (flag4)
			{
				this.BelongsToSubFolder = elem.Attributes["belongsToSubFolder"].Value.NullIfEmpty();
			}
			else
			{
				this.BelongsToSubFolder = null;
			}
			this.Rules.Clear();
			foreach (XmlElement elem2 in elem.ChildNodes.OfType<XmlElement>())
			{
				Rule rule = new Rule("true", ProtectionPreset.None, false);
				rule.Load(elem2);
				this.Rules.Add(rule);
			}
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00013D18 File Offset: 0x00011F18
		public override string ToString()
		{
			return this.Path;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00013D30 File Offset: 0x00011F30
		public ProjectModule Clone()
		{
			ProjectModule projectModule = new ProjectModule();
			projectModule.Path = this.Path;
			projectModule.IsExternal = this.IsExternal;
			projectModule.SNKeyPath = this.SNKeyPath;
			projectModule.SNKeyPassword = this.SNKeyPassword;
			projectModule.BelongsToSubFolder = this.BelongsToSubFolder;
			foreach (Rule rule in this.Rules)
			{
				projectModule.Rules.Add(rule.Clone());
			}
			return projectModule;
		}
	}
}
