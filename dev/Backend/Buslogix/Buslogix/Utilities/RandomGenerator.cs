using System.Security.Cryptography;

namespace Buslogix.Utilities
{
    public class RandomGenerator
    {

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] result = new char[length];
            byte[] buffer = new byte[length];

            RandomNumberGenerator.Fill(buffer);

            for (int i = 0; i < length; i++)
            {
                int index = buffer[i] % chars.Length;
                result[i] = chars[index];
            }

            return new string(result);
        }
    }
}
