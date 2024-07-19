using System.Security.Cryptography;

namespace PhotoCli.Tests.Utils;

public abstract class Sha1HashHelper
{
	public static string CalculateArray(byte[] data)
	{
		using var sha = SHA1.Create();
		using var memoryStream = new MemoryStream(data);
		var hashByte = sha.ComputeHash(memoryStream);
		var hex = Convert.ToHexString(hashByte);
		var hexLowered = hex.ToLower();
		return hexLowered;
	}
}
