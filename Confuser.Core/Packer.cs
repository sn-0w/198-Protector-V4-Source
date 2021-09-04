using System;
using System.IO;
using System.Linq;
using System.Threading;
using Confuser.Core.Project;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x0200005E RID: 94
	public abstract class Packer : ConfuserComponent
	{
		// Token: 0x06000247 RID: 583
		protected internal abstract void Pack(ConfuserContext context, ProtectionParameters parameters);

		// Token: 0x06000248 RID: 584 RVA: 0x00011284 File Offset: 0x0000F484
		protected void ProtectStub(ConfuserContext context, string fileName, byte[] module, StrongNameKey snKey, Protection prot = null)
		{
			string text = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				string text2 = Path.Combine(text, Path.GetRandomFileName());
				Directory.CreateDirectory(text);
				for (int i = 0; i < context.OutputModules.Count; i++)
				{
					string fullPath = Path.GetFullPath(Path.Combine(text, context.OutputPaths[i]));
					string directoryName = Path.GetDirectoryName(fullPath);
					bool flag = !Directory.Exists(directoryName);
					if (flag)
					{
						Directory.CreateDirectory(directoryName);
					}
					File.WriteAllBytes(fullPath, context.OutputModules[i].ToArray<byte>());
				}
				File.WriteAllBytes(Path.Combine(text, fileName), module);
				ConfuserProject confuserProject = new ConfuserProject();
				confuserProject.Seed = context.Project.Seed;
				foreach (Rule item in context.Project.Rules)
				{
					confuserProject.Rules.Add(item);
				}
				confuserProject.Add(new ProjectModule
				{
					Path = fileName
				});
				confuserProject.BaseDirectory = text;
				confuserProject.OutputDirectory = text2;
				foreach (string item2 in context.Project.ProbePaths)
				{
					confuserProject.ProbePaths.Add(item2);
				}
				confuserProject.ProbePaths.Add(context.Project.BaseDirectory);
				PluginDiscovery pluginDiscovery = null;
				bool flag2 = prot != null;
				if (flag2)
				{
					Rule rule = new Rule("true", ProtectionPreset.None, false)
					{
						Preset = ProtectionPreset.None,
						Inherit = true,
						Pattern = "true"
					};
					rule.Add(new SettingItem<Protection>(null, SettingItemAction.Add)
					{
						Id = prot.Id,
						Action = SettingItemAction.Add
					});
					confuserProject.Rules.Add(rule);
					pluginDiscovery = new PackerDiscovery(prot);
				}
				try
				{
					ConfuserEngine.Run(new ConfuserParameters
					{
						Logger = new PackerLogger(context.Logger),
						PluginDiscovery = pluginDiscovery,
						Marker = new PackerMarker(snKey),
						Project = confuserProject,
						PackerInitiated = true
					}, new CancellationToken?(context.token)).Wait();
				}
				catch (AggregateException innerException)
				{
					context.Logger.Error("Failed to protect packer stub.");
					throw new ConfuserException(innerException);
				}
				context.OutputModules = new byte[][]
				{
					File.ReadAllBytes(Path.Combine(text2, fileName))
				};
				context.OutputPaths = new string[]
				{
					fileName
				};
			}
			finally
			{
				try
				{
					bool flag3 = Directory.Exists(text);
					if (flag3)
					{
						Directory.Delete(text, true);
					}
				}
				catch (IOException ex)
				{
					context.Logger.WarnException("Failed to remove temporary files of packer.", ex);
				}
			}
		}
	}
}
