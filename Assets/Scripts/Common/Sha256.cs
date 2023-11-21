using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public static class Sha256
    {
        public static string ComputeHash(string value)
        {
            using var hash = SHA256Managed.Create();
            return string.Concat(hash
                .ComputeHash(Encoding.UTF8.GetBytes(value))
                .Select(item => item.ToString("x2")));
        }
    }
}
