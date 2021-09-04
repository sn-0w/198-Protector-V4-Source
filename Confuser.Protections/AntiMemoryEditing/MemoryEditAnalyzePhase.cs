using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections.AntiMemoryEditing
{
	// Token: 0x02000106 RID: 262
	public class MemoryEditAnalyzePhase : ProtectionPhase
	{
		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Fields;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x000037D3 File Offset: 0x000019D3
		public override string Name
		{
			get
			{
				return "Memory Protection analysis";
			}
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00002136 File Offset: 0x00000336
		public MemoryEditAnalyzePhase(ConfuserComponent parent) : base(parent)
		{
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001D1D8 File Offset: 0x0001B3D8
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			IMemoryEditService service = context.Registry.GetService<IMemoryEditService>();
			foreach (IDnlibDef dnlibDef in parameters.Targets.WithProgress(context.Logger))
			{
				FieldDef fieldDef = dnlibDef as FieldDef;
				bool flag = fieldDef != null;
				if (flag)
				{
					bool flag2 = MemoryEditAnalyzePhase.CorrectType(fieldDef);
					if (flag2)
					{
						service.AddToList(fieldDef);
						context.Logger.DebugFormat("Added {0} to list", new object[]
						{
							fieldDef.Name
						});
					}
					else
					{
						context.Logger.WarnFormat("{0} was marked for memory protection, but type '{1}' is not supported!", new object[]
						{
							fieldDef,
							fieldDef.FieldType
						});
					}
				}
				else
				{
					context.Logger.WarnFormat("{0} (of type {1}) was marked for memory protection, but this is not a field!", new object[]
					{
						dnlibDef,
						dnlibDef.GetType()
					});
				}
			}
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001D2D8 File Offset: 0x0001B4D8
		private static bool CorrectType(FieldDef f)
		{
			string fullName = f.FieldType.FullName;
			string text = fullName;
			if (text != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1764058053U)
				{
					if (num <= 942540437U)
					{
						if (num != 848225627U)
						{
							if (num != 875577056U)
							{
								if (num != 942540437U)
								{
									goto IL_19A;
								}
								if (!(text == "System.UInt16"))
								{
									goto IL_19A;
								}
							}
							else if (!(text == "System.UInt64"))
							{
								goto IL_19A;
							}
						}
						else if (!(text == "System.Double"))
						{
							goto IL_19A;
						}
					}
					else if (num != 1697786220U)
					{
						if (num != 1741144581U)
						{
							if (num != 1764058053U)
							{
								goto IL_19A;
							}
							if (!(text == "System.Int64"))
							{
								goto IL_19A;
							}
						}
						else if (!(text == "System.Decimal"))
						{
							goto IL_19A;
						}
					}
					else if (!(text == "System.Int16"))
					{
						goto IL_19A;
					}
				}
				else if (num <= 3079944380U)
				{
					if (num != 2185383742U)
					{
						if (num != 2747029693U)
						{
							if (num != 3079944380U)
							{
								goto IL_19A;
							}
							if (!(text == "System.Byte"))
							{
								goto IL_19A;
							}
						}
						else if (!(text == "System.SByte"))
						{
							goto IL_19A;
						}
					}
					else if (!(text == "System.Single"))
					{
						goto IL_19A;
					}
				}
				else if (num != 3291009739U)
				{
					if (num != 4180476474U)
					{
						if (num != 4201364391U)
						{
							goto IL_19A;
						}
						if (!(text == "System.String"))
						{
							goto IL_19A;
						}
					}
					else if (!(text == "System.Int32"))
					{
						goto IL_19A;
					}
				}
				else if (!(text == "System.UInt32"))
				{
					goto IL_19A;
				}
				return true;
			}
			IL_19A:
			return false;
		}
	}
}
