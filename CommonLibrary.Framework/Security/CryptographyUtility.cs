using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CommonLibrary.Framework.Security {
	public static class CryptographyUtility {
		public enum EncodingFormat {
			Clear,
			Encrypted,
			Hashed
		}
		
		public static string EncodeValue(string value, EncodingFormat format, string cryptoKey) {
			string encodedValue = value;
			switch (format) {
				case EncodingFormat.Clear:
					break;
				case EncodingFormat.Encrypted:
					//encodedValue = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(value)));
					break;
				case EncodingFormat.Hashed:
					HMACSHA1 hash = new HMACSHA1();
					if (string.IsNullOrWhiteSpace(cryptoKey)) {
						throw new ApplicationException("Crypto key is null or empty.");
					}
					hash.Key = CryptographyUtility.HexToByte(cryptoKey);
					encodedValue = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(value)));
					break;
				default:
					throw new ApplicationException("Unsupported encoding format.");
			}

			return encodedValue;
		}

		public static string UnEncodeValue(string encodedPassword, EncodingFormat format) {
			string password = encodedPassword;

			switch (format) {
				case EncodingFormat.Clear:
					break;
				case EncodingFormat.Encrypted:
					//password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
					break;
				case EncodingFormat.Hashed:
					throw new ApplicationException("Cannot unencode a hashed value.");
				default:
					throw new ApplicationException("Unsupported encoding format.");
			}

			return password;
		}

		public static byte[] HexToByte(string hexString) {
			byte[] returnBytes = new byte[hexString.Length / 2];
			for (int i = 0; i < returnBytes.Length; i++)
				returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			return returnBytes;
		}
	}
}