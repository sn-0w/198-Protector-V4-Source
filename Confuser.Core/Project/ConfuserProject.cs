using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace Confuser.Core.Project
{
	// Token: 0x02000087 RID: 135
	public class ConfuserProject : List<ProjectModule>
	{
		// Token: 0x06000339 RID: 825 RVA: 0x00003623 File Offset: 0x00001823
		public ConfuserProject()
		{
			this.ProbePaths = new List<string>();
			this.PluginPaths = new List<string>();
			this.Rules = new List<Rule>();
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600033A RID: 826 RVA: 0x00003651 File Offset: 0x00001851
		// (set) Token: 0x0600033B RID: 827 RVA: 0x00003659 File Offset: 0x00001859
		public string Seed { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600033C RID: 828 RVA: 0x00003662 File Offset: 0x00001862
		// (set) Token: 0x0600033D RID: 829 RVA: 0x0000366A File Offset: 0x0000186A
		public bool Debug { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600033E RID: 830 RVA: 0x00003673 File Offset: 0x00001873
		// (set) Token: 0x0600033F RID: 831 RVA: 0x0000367B File Offset: 0x0000187B
		public string OutputDirectory { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000340 RID: 832 RVA: 0x00003684 File Offset: 0x00001884
		// (set) Token: 0x06000341 RID: 833 RVA: 0x0000368C File Offset: 0x0000188C
		public string BaseDirectory { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000342 RID: 834 RVA: 0x00003695 File Offset: 0x00001895
		// (set) Token: 0x06000343 RID: 835 RVA: 0x0000369D File Offset: 0x0000189D
		public string InputSymbolMap { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000344 RID: 836 RVA: 0x000036A6 File Offset: 0x000018A6
		// (set) Token: 0x06000345 RID: 837 RVA: 0x000036AE File Offset: 0x000018AE
		public IList<Rule> Rules { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000346 RID: 838 RVA: 0x000036B7 File Offset: 0x000018B7
		// (set) Token: 0x06000347 RID: 839 RVA: 0x000036BF File Offset: 0x000018BF
		public SettingItem<Packer> Packer { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000348 RID: 840 RVA: 0x000036C8 File Offset: 0x000018C8
		// (set) Token: 0x06000349 RID: 841 RVA: 0x000036D0 File Offset: 0x000018D0
		public IList<string> ProbePaths { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600034A RID: 842 RVA: 0x000036D9 File Offset: 0x000018D9
		// (set) Token: 0x0600034B RID: 843 RVA: 0x000036E1 File Offset: 0x000018E1
		public IList<string> PluginPaths { get; private set; }

		// Token: 0x0600034C RID: 844 RVA: 0x00014404 File Offset: 0x00012604
		public XmlDocument Save()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Schemas.Add(ConfuserProject.Schema);
			XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			xmlDocument.AppendChild(newChild);
			XmlElement xmlElement = xmlDocument.CreateElement("project", "http://confuser.codeplex.com");
			XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("outputDir");
			xmlAttribute.Value = this.OutputDirectory;
			xmlElement.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("baseDir");
			xmlAttribute2.Value = this.BaseDirectory;
			xmlElement.Attributes.Append(xmlAttribute2);
			bool flag = this.Seed != null;
			if (flag)
			{
				XmlAttribute xmlAttribute3 = xmlDocument.CreateAttribute("seed");
				xmlAttribute3.Value = this.Seed;
				xmlElement.Attributes.Append(xmlAttribute3);
			}
			bool debug = this.Debug;
			if (debug)
			{
				XmlAttribute xmlAttribute4 = xmlDocument.CreateAttribute("debug");
				xmlAttribute4.Value = this.Debug.ToString().ToLower();
				xmlElement.Attributes.Append(xmlAttribute4);
			}
			foreach (Rule rule in this.Rules)
			{
				xmlElement.AppendChild(rule.Save(xmlDocument));
			}
			bool flag2 = this.Packer != null;
			if (flag2)
			{
				xmlElement.AppendChild(this.Packer.Save(xmlDocument));
			}
			foreach (ProjectModule projectModule in this)
			{
				xmlElement.AppendChild(projectModule.Save(xmlDocument));
			}
			foreach (string innerText in this.ProbePaths)
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("probePath", "http://confuser.codeplex.com");
				xmlElement2.InnerText = innerText;
				xmlElement.AppendChild(xmlElement2);
			}
			foreach (string innerText2 in this.PluginPaths)
			{
				XmlElement xmlElement3 = xmlDocument.CreateElement("plugin", "http://confuser.codeplex.com");
				xmlElement3.InnerText = innerText2;
				xmlElement.AppendChild(xmlElement3);
			}
			xmlDocument.AppendChild(xmlElement);
			return xmlDocument;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x000146B4 File Offset: 0x000128B4
		public void Load(XmlDocument doc)
		{
			doc.Schemas.Add(ConfuserProject.Schema);
			List<XmlSchemaException> exceptions = new List<XmlSchemaException>();
			doc.Validate(delegate(object sender, ValidationEventArgs e)
			{
				bool flag9 = e.Severity > XmlSeverityType.Error;
				if (!flag9)
				{
					exceptions.Add(e.Exception);
				}
			});
			bool flag = exceptions.Count > 0;
			if (flag)
			{
				throw new ProjectValidationException(exceptions);
			}
			XmlElement documentElement = doc.DocumentElement;
			this.OutputDirectory = documentElement.Attributes["outputDir"].Value;
			this.BaseDirectory = documentElement.Attributes["baseDir"].Value;
			bool flag2 = documentElement.Attributes["seed"] != null;
			if (flag2)
			{
				this.Seed = documentElement.Attributes["seed"].Value.NullIfEmpty();
			}
			else
			{
				this.Seed = null;
			}
			bool flag3 = documentElement.Attributes["inputSymbolMap"] != null;
			if (flag3)
			{
				this.InputSymbolMap = documentElement.Attributes["inputSymbolMap"].Value.NullIfEmpty();
			}
			else
			{
				this.InputSymbolMap = null;
			}
			bool flag4 = documentElement.Attributes["debug"] != null;
			if (flag4)
			{
				this.Debug = bool.Parse(documentElement.Attributes["debug"].Value);
			}
			else
			{
				this.Debug = false;
			}
			this.Packer = null;
			base.Clear();
			this.ProbePaths.Clear();
			this.PluginPaths.Clear();
			this.Rules.Clear();
			foreach (XmlElement xmlElement in documentElement.ChildNodes.OfType<XmlElement>())
			{
				bool flag5 = xmlElement.Name == "rule";
				if (flag5)
				{
					Rule rule = new Rule("true", ProtectionPreset.None, false);
					rule.Load(xmlElement);
					this.Rules.Add(rule);
				}
				else
				{
					bool flag6 = xmlElement.Name == "packer";
					if (flag6)
					{
						this.Packer = new SettingItem<Packer>(null, SettingItemAction.Add);
						this.Packer.Load(xmlElement);
					}
					else
					{
						bool flag7 = xmlElement.Name == "probePath";
						if (flag7)
						{
							this.ProbePaths.Add(xmlElement.InnerText);
						}
						else
						{
							bool flag8 = xmlElement.Name == "plugin";
							if (flag8)
							{
								this.PluginPaths.Add(xmlElement.InnerText);
							}
							else
							{
								ProjectModule projectModule = new ProjectModule();
								projectModule.Load(xmlElement);
								base.Add(projectModule);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600034E RID: 846 RVA: 0x000149A0 File Offset: 0x00012BA0
		public ConfuserProject Clone()
		{
			ConfuserProject confuserProject = new ConfuserProject();
			confuserProject.Seed = this.Seed;
			confuserProject.InputSymbolMap = this.InputSymbolMap;
			confuserProject.Debug = this.Debug;
			confuserProject.OutputDirectory = this.OutputDirectory;
			confuserProject.BaseDirectory = this.BaseDirectory;
			confuserProject.Packer = ((this.Packer == null) ? null : this.Packer.Clone());
			confuserProject.ProbePaths = new List<string>(this.ProbePaths);
			confuserProject.PluginPaths = new List<string>(this.PluginPaths);
			foreach (ProjectModule projectModule in this)
			{
				confuserProject.Add(projectModule.Clone());
			}
			foreach (Rule item in this.Rules)
			{
				confuserProject.Rules.Add(item);
			}
			return confuserProject;
		}

		// Token: 0x0400024B RID: 587
		public const string Namespace = "http://confuser.codeplex.com";

		// Token: 0x0400024C RID: 588
		public static readonly XmlSchema Schema = XmlSchema.Read(typeof(ConfuserProject).Assembly.GetManifestResourceStream("Confuser.Core.Project.ConfuserPrj.xsd"), null);
	}
}
