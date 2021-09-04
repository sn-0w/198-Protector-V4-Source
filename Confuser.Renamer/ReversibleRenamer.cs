using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Confuser.Renamer
{
	// Token: 0x0200000E RID: 14
	public class ReversibleRenamer
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00008708 File Offset: 0x00006908
		public ReversibleRenamer(string password)
		{
			this.cipher = new RijndaelManaged();
			using (SHA256 sha = SHA256.Create())
			{
				this.cipher.Key = (this.key = sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00008774 File Offset: 0x00006974
		private static string Base64Encode(byte[] buf)
		{
			return Convert.ToBase64String(buf).Trim(new char[]
			{
				'='
			}).Replace('+', '$').Replace('/', '_');
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000087B0 File Offset: 0x000069B0
		private static byte[] Base64Decode(string str)
		{
			str = str.Replace('$', '+').Replace('_', '/').PadRight(str.Length + 3 & -4, '=');
			return Convert.FromBase64String(str);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000087F0 File Offset: 0x000069F0
		private byte[] GetIV(byte ivId)
		{
			byte[] array = new byte[this.cipher.BlockSize / 8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (ivId ^ this.key[i]);
			}
			return array;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00008838 File Offset: 0x00006A38
		private byte GetIVId(string str)
		{
			byte b = (byte)str[0];
			for (int i = 1; i < str.Length; i++)
			{
				b = b * 3 + (byte)str[i];
			}
			return b;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00008878 File Offset: 0x00006A78
		public string Encrypt(string name)
		{
			byte ivid = this.GetIVId(name);
			this.cipher.IV = this.GetIV(ivid);
			byte[] array = Encoding.UTF8.GetBytes(name);
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.WriteByte(ivid);
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, this.cipher.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cryptoStream.Write(array, 0, array.Length);
				}
				array = memoryStream.ToArray();
				result = ReversibleRenamer.Base64Encode(array);
			}
			return result;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00008924 File Offset: 0x00006B24
		public string Decrypt(string name)
		{
			string @string;
			using (MemoryStream memoryStream = new MemoryStream(ReversibleRenamer.Base64Decode(name)))
			{
				byte ivId = (byte)memoryStream.ReadByte();
				this.cipher.IV = this.GetIV(ivId);
				MemoryStream memoryStream2 = new MemoryStream();
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, this.cipher.CreateDecryptor(), CryptoStreamMode.Read))
				{
					cryptoStream.CopyTo(memoryStream2);
				}
				@string = Encoding.UTF8.GetString(memoryStream2.ToArray());
			}
			return @string;
		}

		// Token: 0x0400002E RID: 46
		private RijndaelManaged cipher;

		// Token: 0x0400002F RID: 47
		private byte[] key;
	}
}
