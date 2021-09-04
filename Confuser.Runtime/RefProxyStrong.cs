using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Confuser.Runtime
{
	// Token: 0x02000035 RID: 53
	internal static class RefProxyStrong
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x00006D90 File Offset: 0x00004F90
		internal static void Initialize(RuntimeFieldHandle field, byte opKey)
		{
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(field);
			byte[] array = fieldFromHandle.Module.ResolveSignature(fieldFromHandle.MetadataToken);
			int num = array.Length;
			int num2 = fieldFromHandle.GetOptionalCustomModifiers()[0].MetadataToken;
			num2 += (int)((int)(fieldFromHandle.Name[Mutation.KeyI0] ^ (char)array[--num]) << Mutation.KeyI4);
			num2 += (int)((int)(fieldFromHandle.Name[Mutation.KeyI1] ^ (char)array[--num]) << Mutation.KeyI5);
			num2 += (int)((int)(fieldFromHandle.Name[Mutation.KeyI2] ^ (char)array[--num]) << Mutation.KeyI6);
			num--;
			num2 += (int)((int)(fieldFromHandle.Name[Mutation.KeyI3] ^ (char)array[num - 1]) << (Mutation.KeyI7 & 31));
			int num3 = Mutation.Placeholder<int>(num2);
			num3 *= fieldFromHandle.GetCustomAttributes(false)[0].GetHashCode();
			MethodBase methodBase = fieldFromHandle.Module.ResolveMethod(num3);
			Type fieldType = fieldFromHandle.FieldType;
			if (methodBase.IsStatic)
			{
				fieldFromHandle.SetValue(null, Delegate.CreateDelegate(fieldType, (MethodInfo)methodBase));
				return;
			}
			DynamicMethod dynamicMethod = null;
			Type[] array2 = null;
			foreach (MethodInfo methodInfo in fieldFromHandle.FieldType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (methodInfo.DeclaringType == fieldType)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					array2 = new Type[parameters.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = parameters[j].ParameterType;
					}
					Type declaringType = methodBase.DeclaringType;
					dynamicMethod = new DynamicMethod("", methodInfo.ReturnType, array2, (declaringType.IsInterface || declaringType.IsArray) ? fieldType : declaringType, true);
					break;
				}
			}
			DynamicILInfo dynamicILInfo = dynamicMethod.GetDynamicILInfo();
			DynamicILInfo dynamicILInfo2 = dynamicILInfo;
			byte[] array3 = new byte[2];
			array3[0] = 7;
			dynamicILInfo2.SetLocalSignature(array3);
			byte[] array4 = new byte[7 * array2.Length + 6];
			int num4 = 0;
			ParameterInfo[] parameters2 = methodBase.GetParameters();
			int num5 = methodBase.IsConstructor ? 0 : -1;
			for (int k = 0; k < array2.Length; k++)
			{
				array4[num4++] = 14;
				array4[num4++] = (byte)k;
				Type type = (num5 == -1) ? methodBase.DeclaringType : parameters2[num5].ParameterType;
				if (type.IsClass && !type.IsPointer && !type.IsByRef)
				{
					int tokenFor = dynamicILInfo.GetTokenFor(type.TypeHandle);
					array4[num4++] = 116;
					array4[num4++] = (byte)tokenFor;
					array4[num4++] = (byte)(tokenFor >> 8);
					array4[num4++] = (byte)(tokenFor >> 16);
					array4[num4++] = (byte)(tokenFor >> 24);
				}
				else
				{
					num4 += 5;
				}
				num5++;
			}
			array4[num4++] = ((byte)fieldFromHandle.Name[Mutation.KeyI8] ^ opKey);
			int tokenFor2 = dynamicILInfo.GetTokenFor(methodBase.MethodHandle);
			array4[num4++] = (byte)tokenFor2;
			array4[num4++] = (byte)(tokenFor2 >> 8);
			array4[num4++] = (byte)(tokenFor2 >> 16);
			array4[num4++] = (byte)(tokenFor2 >> 24);
			array4[num4] = 42;
			dynamicILInfo.SetCode(array4, array2.Length + 1);
			fieldFromHandle.SetValue(null, dynamicMethod.CreateDelegate(fieldType));
		}
	}
}
