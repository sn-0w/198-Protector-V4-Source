using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Confuser.Runtime
{
	// Token: 0x02000029 RID: 41
	internal static class IntegrityProtection
	{
		// Token: 0x0600008F RID: 143 RVA: 0x00005F50 File Offset: 0x00004150
		internal static void Initialize()
		{
			BinaryReader binaryReader = new BinaryReader(new StreamReader(typeof(IntegrityProtection).Assembly.Location).BaseStream);
			byte[] metin = binaryReader.ReadBytes(File.ReadAllBytes(typeof(IntegrityProtection).Assembly.Location).Length - 32);
			binaryReader.BaseStream.Position = binaryReader.BaseStream.Length - 32L;
			if (IntegrityProtection.MD5(metin) != Encoding.ASCII.GetString(binaryReader.ReadBytes(32)))
			{
				Environment.FailFast(null);
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005FE8 File Offset: 0x000041E8
		internal static string MD5(byte[] metin)
		{
			metin = new MD5CryptoServiceProvider().ComputeHash(metin);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in metin)
			{
				stringBuilder.Append(b.ToString("x2").ToLower());
			}
			return stringBuilder.ToString();
		}
	}
}
