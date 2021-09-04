using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	// Token: 0x0200006F RID: 111
	public class ServiceRegistry : IServiceProvider
	{
		// Token: 0x060002B7 RID: 695 RVA: 0x00012048 File Offset: 0x00010248
		object IServiceProvider.GetService(Type serviceType)
		{
			return this.services.GetValueOrDefault(serviceType, null);
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00012068 File Offset: 0x00010268
		public T GetService<T>()
		{
			return (T)((object)this.services.GetValueOrDefault(typeof(T), null));
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00012098 File Offset: 0x00010298
		public void RegisterService(string serviceId, Type serviceType, object service)
		{
			bool flag = !this.serviceIds.Add(serviceId);
			if (flag)
			{
				throw new ArgumentException("Service with ID '" + this.serviceIds + "' has already registered.", "serviceId");
			}
			bool flag2 = this.services.ContainsKey(serviceType);
			if (flag2)
			{
				throw new ArgumentException("Service with type '" + service.GetType().Name + "' has already registered.", "service");
			}
			this.services.Add(serviceType, service);
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0001211C File Offset: 0x0001031C
		public bool Contains(string serviceId)
		{
			return this.serviceIds.Contains(serviceId);
		}

		// Token: 0x04000216 RID: 534
		private readonly HashSet<string> serviceIds = new HashSet<string>();

		// Token: 0x04000217 RID: 535
		private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
	}
}
