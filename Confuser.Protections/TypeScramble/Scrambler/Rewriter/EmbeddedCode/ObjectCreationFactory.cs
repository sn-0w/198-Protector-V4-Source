using System;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.EmbeddedCode
{
	// Token: 0x02000046 RID: 70
	public class ObjectCreationFactory
	{
		// Token: 0x0600017F RID: 383 RVA: 0x0000816C File Offset: 0x0000636C
		public static void Import(ModuleDef mod)
		{
			ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(ObjectCreationFactory).Module);
			TypeDef typeDef = moduleDefMD.Find(typeof(ObjectCreationFactory).FullName, true);
			moduleDefMD.Types.Remove(typeDef);
			TypeDefUser typeDefUser = new TypeDefUser("ObjectCreationFactory");
			MethodDef[] array = typeDef.Methods.ToArray<MethodDef>();
			foreach (MethodDef methodDef in array)
			{
				methodDef.DeclaringType = null;
				typeDefUser.Methods.Add(methodDef);
			}
			mod.Types.Add(typeDef);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00008214 File Offset: 0x00006414
		public static T Create<T>()
		{
			return Activator.CreateInstance<T>();
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000822C File Offset: 0x0000642C
		public static TR Create<TR, T0>(T0 p0)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0
			}));
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00008264 File Offset: 0x00006464
		public static TR Create<TR, T0, T1>(T0 p0, T1 p1)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1
			}));
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000082A4 File Offset: 0x000064A4
		public static TR Create<TR, T0, T1, T2>(T0 p0, T1 p1, T2 p2)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2
			}));
		}

		// Token: 0x06000184 RID: 388 RVA: 0x000082EC File Offset: 0x000064EC
		public static TR Create<TR, T0, T1, T2, T3>(T0 p0, T1 p1, T2 p2, T3 p3)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3
			}));
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000833C File Offset: 0x0000653C
		public static TR Create<TR, T0, T1, T2, T3, T4>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4
			}));
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008398 File Offset: 0x00006598
		public static TR Create<TR, T0, T1, T2, T3, T4, T5>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5
			}));
		}

		// Token: 0x06000187 RID: 391 RVA: 0x000083FC File Offset: 0x000065FC
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6
			}));
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000846C File Offset: 0x0000666C
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7
			}));
		}

		// Token: 0x06000189 RID: 393 RVA: 0x000084E4 File Offset: 0x000066E4
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8
			}));
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00008568 File Offset: 0x00006768
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9
			}));
		}

		// Token: 0x0600018B RID: 395 RVA: 0x000085F8 File Offset: 0x000067F8
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10
			}));
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00008694 File Offset: 0x00006894
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11
			}));
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00008738 File Offset: 0x00006938
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11,
				p12
			}));
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000087E8 File Offset: 0x000069E8
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11,
				p12,
				p13
			}));
		}

		// Token: 0x0600018F RID: 399 RVA: 0x000088A4 File Offset: 0x00006AA4
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11,
				p12,
				p13,
				p14
			}));
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000896C File Offset: 0x00006B6C
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11,
				p12,
				p13,
				p14,
				p15
			}));
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00008A3C File Offset: 0x00006C3C
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11,
				p12,
				p13,
				p14,
				p15,
				p16
			}));
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00008B18 File Offset: 0x00006D18
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11,
				p12,
				p13,
				p14,
				p15,
				p16,
				p17
			}));
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00008C00 File Offset: 0x00006E00
		public static TR Create<TR, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18)
		{
			return (TR)((object)Activator.CreateInstance(typeof(TR), new object[]
			{
				p0,
				p1,
				p2,
				p3,
				p4,
				p5,
				p6,
				p7,
				p8,
				p9,
				p10,
				p11,
				p12,
				p13,
				p14,
				p15,
				p16,
				p17,
				p18
			}));
		}
	}
}
