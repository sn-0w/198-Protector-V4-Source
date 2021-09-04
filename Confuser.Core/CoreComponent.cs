using System;
using Confuser.Core.API;
using Confuser.Core.Services;

namespace Confuser.Core
{
	// Token: 0x0200003C RID: 60
	public class CoreComponent : ConfuserComponent
	{
		// Token: 0x06000147 RID: 327 RVA: 0x000028BA File Offset: 0x00000ABA
		internal CoreComponent(ConfuserParameters parameters, Marker marker)
		{
			this.parameters = parameters;
			this.marker = marker;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000148 RID: 328 RVA: 0x000028D2 File Offset: 0x00000AD2
		public override string Name
		{
			get
			{
				return "Confuser Core";
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000149 RID: 329 RVA: 0x000028D9 File Offset: 0x00000AD9
		public override string Description
		{
			get
			{
				return "Initialization of Confuser core services.";
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600014A RID: 330 RVA: 0x000028E0 File Offset: 0x00000AE0
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600014B RID: 331 RVA: 0x000028E7 File Offset: 0x00000AE7
		public override string Id
		{
			get
			{
				return "Confuser.Core";
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600014C RID: 332 RVA: 0x000028E7 File Offset: 0x00000AE7
		public override string FullId
		{
			get
			{
				return "Confuser.Core";
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000C3CC File Offset: 0x0000A5CC
		protected internal override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Confuser.Random", typeof(IRandomService), new RandomService(this.parameters.Project.Seed));
			context.Registry.RegisterService("Confuser.Marker", typeof(IMarkerService), new MarkerService(context, this.marker));
			context.Registry.RegisterService("Confuser.Trace", typeof(ITraceService), new TraceService(context));
			context.Registry.RegisterService("Confuser.Runtime", typeof(IRuntimeService), new RuntimeService());
			context.Registry.RegisterService("Confuser.Compression", typeof(ICompressionService), new CompressionService(context));
			context.Registry.RegisterService("Confuser.APIStore", typeof(IAPIStore), new APIStore(context));
		}

		// Token: 0x0600014E RID: 334 RVA: 0x000026EF File Offset: 0x000008EF
		protected internal override void PopulatePipeline(ProtectionPipeline pipeline)
		{
		}

		// Token: 0x0400013A RID: 314
		public const string _RandomServiceId = "Confuser.Random";

		// Token: 0x0400013B RID: 315
		public const string _MarkerServiceId = "Confuser.Marker";

		// Token: 0x0400013C RID: 316
		public const string _TraceServiceId = "Confuser.Trace";

		// Token: 0x0400013D RID: 317
		public const string _RuntimeServiceId = "Confuser.Runtime";

		// Token: 0x0400013E RID: 318
		public const string _CompressionServiceId = "Confuser.Compression";

		// Token: 0x0400013F RID: 319
		public const string _APIStoreId = "Confuser.APIStore";

		// Token: 0x04000140 RID: 320
		private readonly Marker marker;

		// Token: 0x04000141 RID: 321
		private readonly ConfuserParameters parameters;
	}
}
