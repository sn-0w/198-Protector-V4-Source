using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x0200006B RID: 107
	public class ProtectionPipeline
	{
		// Token: 0x060002A3 RID: 675 RVA: 0x00011AA4 File Offset: 0x0000FCA4
		public ProtectionPipeline()
		{
			PipelineStage[] source = (PipelineStage[])Enum.GetValues(typeof(PipelineStage));
			this.preStage = source.ToDictionary((PipelineStage stage) => stage, (PipelineStage stage) => new List<ProtectionPhase>());
			this.postStage = source.ToDictionary((PipelineStage stage) => stage, (PipelineStage stage) => new List<ProtectionPhase>());
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000324C File Offset: 0x0000144C
		public void InsertPreStage(PipelineStage stage, ProtectionPhase phase)
		{
			this.preStage[stage].Add(phase);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00003262 File Offset: 0x00001462
		public void InsertPostStage(PipelineStage stage, ProtectionPhase phase)
		{
			this.postStage[stage].Add(phase);
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00011B64 File Offset: 0x0000FD64
		public T FindPhase<T>() where T : ProtectionPhase
		{
			foreach (List<ProtectionPhase> list in this.preStage.Values)
			{
				foreach (ProtectionPhase protectionPhase in list)
				{
					bool flag = protectionPhase is T;
					if (flag)
					{
						return (T)((object)protectionPhase);
					}
				}
			}
			foreach (List<ProtectionPhase> list2 in this.postStage.Values)
			{
				foreach (ProtectionPhase protectionPhase2 in list2)
				{
					bool flag2 = protectionPhase2 is T;
					if (flag2)
					{
						return (T)((object)protectionPhase2);
					}
				}
			}
			return default(T);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00011CBC File Offset: 0x0000FEBC
		internal void ExecuteStage(PipelineStage stage, Action<ConfuserContext> func, Func<IList<IDnlibDef>> targets, ConfuserContext context)
		{
			foreach (ProtectionPhase protectionPhase in this.preStage[stage])
			{
				context.CheckCancellation();
				IList<IDnlibDef> source = ProtectionPipeline.Filter(context, targets(), protectionPhase);
				bool flag = source.Any<IDnlibDef>();
				if (flag)
				{
					context.Logger.DebugFormat("Executing '{0}' phase...", new object[]
					{
						protectionPhase.Name
					});
				}
				protectionPhase.Execute(context, new ProtectionParameters(protectionPhase.Parent, ProtectionPipeline.Filter(context, targets(), protectionPhase)));
			}
			context.CheckCancellation();
			func(context);
			context.CheckCancellation();
			foreach (ProtectionPhase protectionPhase2 in this.postStage[stage])
			{
				IList<IDnlibDef> source2 = ProtectionPipeline.Filter(context, targets(), protectionPhase2);
				bool flag2 = source2.Any<IDnlibDef>();
				if (flag2)
				{
					context.Logger.DebugFormat("Executing '{0}' phase...", new object[]
					{
						protectionPhase2.Name
					});
				}
				protectionPhase2.Execute(context, new ProtectionParameters(protectionPhase2.Parent, ProtectionPipeline.Filter(context, targets(), protectionPhase2)));
				context.CheckCancellation();
			}
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00011E48 File Offset: 0x00010048
		private static IList<IDnlibDef> Filter(ConfuserContext context, IList<IDnlibDef> targets, ProtectionPhase phase)
		{
			ProtectionTargets targets2 = phase.Targets;
			IEnumerable<IDnlibDef> source = targets;
			bool flag = (targets2 & ProtectionTargets.Modules) == (ProtectionTargets)0;
			if (flag)
			{
				source = from def in source
				where !(def is ModuleDef)
				select def;
			}
			bool flag2 = (targets2 & ProtectionTargets.Types) == (ProtectionTargets)0;
			if (flag2)
			{
				source = from def in source
				where !(def is TypeDef)
				select def;
			}
			bool flag3 = (targets2 & ProtectionTargets.Methods) == (ProtectionTargets)0;
			if (flag3)
			{
				source = from def in source
				where !(def is MethodDef)
				select def;
			}
			bool flag4 = (targets2 & ProtectionTargets.Fields) == (ProtectionTargets)0;
			if (flag4)
			{
				source = from def in source
				where !(def is FieldDef)
				select def;
			}
			bool flag5 = (targets2 & ProtectionTargets.Properties) == (ProtectionTargets)0;
			if (flag5)
			{
				source = from def in source
				where !(def is PropertyDef)
				select def;
			}
			bool flag6 = (targets2 & ProtectionTargets.Events) == (ProtectionTargets)0;
			if (flag6)
			{
				source = from def in source
				where !(def is EventDef)
				select def;
			}
			bool processAll = phase.ProcessAll;
			IList<IDnlibDef> result;
			if (processAll)
			{
				result = source.ToList<IDnlibDef>();
			}
			else
			{
				result = source.Where(delegate(IDnlibDef def)
				{
					ProtectionSettings parameters = ProtectionParameters.GetParameters(context, def);
					Debug.Assert(parameters != null);
					bool flag7 = parameters == null;
					if (flag7)
					{
						context.Logger.ErrorFormat("'{0}' not marked for obfuscation, possibly a bug.", new object[]
						{
							def
						});
						throw new ConfuserException(null);
					}
					return parameters.ContainsKey(phase.Parent);
				}).ToList<IDnlibDef>();
			}
			return result;
		}

		// Token: 0x04000201 RID: 513
		private readonly Dictionary<PipelineStage, List<ProtectionPhase>> postStage;

		// Token: 0x04000202 RID: 514
		private readonly Dictionary<PipelineStage, List<ProtectionPhase>> preStage;
	}
}
