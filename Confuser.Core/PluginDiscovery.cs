using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Confuser.Core
{
	// Token: 0x0200005C RID: 92
	public class PluginDiscovery
	{
		// Token: 0x06000230 RID: 560 RVA: 0x00002583 File Offset: 0x00000783
		protected PluginDiscovery()
		{
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00002EAC File Offset: 0x000010AC
		public void GetPlugins(ConfuserContext context, out IList<Protection> protections, out IList<Packer> packers, out IList<ConfuserComponent> components)
		{
			protections = new List<Protection>();
			packers = new List<Packer>();
			components = new List<ConfuserComponent>();
			this.GetPluginsInternal(context, protections, packers, components);
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00010F24 File Offset: 0x0000F124
		public static bool HasAccessibleDefConstructor(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			bool flag = constructor == null;
			return !flag && constructor.IsPublic;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00010F58 File Offset: 0x0000F158
		protected static void AddPlugins(ConfuserContext context, IList<Protection> protections, IList<Packer> packers, IList<ConfuserComponent> components, Assembly asm)
		{
			foreach (Module module in asm.GetLoadedModules())
			{
				foreach (Type type in module.GetTypes())
				{
					bool flag = type.IsAbstract || !PluginDiscovery.HasAccessibleDefConstructor(type);
					if (!flag)
					{
						bool flag2 = typeof(Protection).IsAssignableFrom(type);
						if (flag2)
						{
							try
							{
								protections.Add((Protection)Activator.CreateInstance(type));
							}
							catch (Exception ex)
							{
								context.Logger.ErrorException("Failed to instantiate protection '" + type.Name + "'.", ex);
							}
						}
						else
						{
							bool flag3 = typeof(Packer).IsAssignableFrom(type);
							if (flag3)
							{
								try
								{
									packers.Add((Packer)Activator.CreateInstance(type));
								}
								catch (Exception ex2)
								{
									context.Logger.ErrorException("Failed to instantiate packer '" + type.Name + "'.", ex2);
								}
							}
							else
							{
								bool flag4 = typeof(ConfuserComponent).IsAssignableFrom(type);
								if (flag4)
								{
									try
									{
										components.Add((ConfuserComponent)Activator.CreateInstance(type));
									}
									catch (Exception ex3)
									{
										context.Logger.ErrorException("Failed to instantiate component '" + type.Name + "'.", ex3);
									}
								}
							}
						}
					}
				}
			}
			context.CheckCancellation();
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00011114 File Offset: 0x0000F314
		protected virtual void GetPluginsInternal(ConfuserContext context, IList<Protection> protections, IList<Packer> packers, IList<ConfuserComponent> components)
		{
			try
			{
				Assembly asm = Assembly.Load("Confuser.Protections");
				PluginDiscovery.AddPlugins(context, protections, packers, components, asm);
			}
			catch (Exception ex)
			{
				context.Logger.WarnException("Failed to load built-in protections.", ex);
			}
			try
			{
				Assembly asm2 = Assembly.Load("Confuser.Renamer");
				PluginDiscovery.AddPlugins(context, protections, packers, components, asm2);
			}
			catch (Exception ex2)
			{
				context.Logger.WarnException("Failed to load renamer.", ex2);
			}
			try
			{
				Assembly asm3 = Assembly.Load("Confuser.DynCipher");
				PluginDiscovery.AddPlugins(context, protections, packers, components, asm3);
			}
			catch (Exception ex3)
			{
				context.Logger.WarnException("Failed to load dynamic cipher library.", ex3);
			}
			foreach (string text in context.Project.PluginPaths)
			{
				string path = Path.Combine(context.BaseDirectory, text);
				try
				{
					Assembly asm4 = Assembly.LoadFile(path);
					PluginDiscovery.AddPlugins(context, protections, packers, components, asm4);
				}
				catch (Exception ex4)
				{
					context.Logger.WarnException("Failed to load plugin '" + text + "'.", ex4);
				}
			}
		}

		// Token: 0x040001CD RID: 461
		internal static readonly PluginDiscovery Instance = new PluginDiscovery();
	}
}
