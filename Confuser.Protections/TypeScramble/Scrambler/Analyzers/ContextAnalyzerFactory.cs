using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Analyzers
{
	// Token: 0x02000049 RID: 73
	internal sealed class ContextAnalyzerFactory : IEnumerable<ContextAnalyzer>, IEnumerable
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x0600019C RID: 412 RVA: 0x000029CF File Offset: 0x00000BCF
		private IDictionary<Type, ContextAnalyzer> Analyzers { get; } = new Dictionary<Type, ContextAnalyzer>();

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600019D RID: 413 RVA: 0x000029D7 File Offset: 0x00000BD7
		private ScannedMethod TargetMethod { get; }

		// Token: 0x0600019E RID: 414 RVA: 0x000029DF File Offset: 0x00000BDF
		internal ContextAnalyzerFactory(ScannedMethod method)
		{
			Debug.Assert(method != null, "method != null");
			this.TargetMethod = method;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00002A0A File Offset: 0x00000C0A
		internal void Add(ContextAnalyzer a)
		{
			this.Analyzers.Add(a.TargetType(), a);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00008CF4 File Offset: 0x00006EF4
		internal void Analyze(Instruction inst)
		{
			Debug.Assert(inst != null, "inst != null");
			Debug.Assert(inst.Operand != null, "inst.Operand != null");
			object operand = inst.Operand;
			Type type = operand.GetType();
			while (type != typeof(object))
			{
				ContextAnalyzer contextAnalyzer;
				bool flag = this.Analyzers.TryGetValue(type, out contextAnalyzer);
				if (flag)
				{
					contextAnalyzer.ProcessOperand(this.TargetMethod, inst, operand);
					break;
				}
				type = type.BaseType;
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00002A20 File Offset: 0x00000C20
		public IEnumerator<ContextAnalyzer> GetEnumerator()
		{
			return this.Analyzers.Values.GetEnumerator();
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00002A32 File Offset: 0x00000C32
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
