using System.Security.Cryptography;
using System.Text;

namespace GrillPizzeriaOrderMiddleware.Services.AlgoritamCryptography
{
    public static class Hashing
    {

        public static string sha256(string keyWord)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(keyWord));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
