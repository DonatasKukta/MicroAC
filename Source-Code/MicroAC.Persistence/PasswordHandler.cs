using Microsoft.Extensions.Configuration;
using MicroAC.Persistence.Entities;

using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace MicroAC.Persistence
{
    public interface IPasswordHandler
    {
        bool ConfirmPassword(User user, string password);

        byte[] HashPassword(byte[] password, byte[] salt);
    }

    public class PasswordHandler : IPasswordHandler
    {
        readonly byte[] _pepper;
        static readonly int _hashedPasswordLength = 64;

        public PasswordHandler(IConfiguration configuration)
        {
            var pepper = configuration.GetSection("Pepper").Value;
            _pepper = EncodeStr(pepper);
        }

        public PasswordHandler(string pepper)
        {
            _pepper = EncodeStr(pepper);
        }

        public bool ConfirmPassword(User user, string password)
        {
            var generatedHash = HashPassword(EncodeStr(password), user.Salt);
            return user.PasswordHash.SequenceEqual(generatedHash);
        }

        public byte[] HashPassword(byte[] password, byte[] salt)
        {
            using HashAlgorithm algorithm = HashAlgorithm.Create("SHA512");
            var allBytes = new byte[salt.Length + _pepper.Length + password.Length];

            _pepper.CopyTo(allBytes, 0);
            salt.CopyTo(allBytes, _pepper.Length);
            password.CopyTo(allBytes, salt.Length + _pepper.Length);
            
            var hashBytes = allBytes.Take(_hashedPasswordLength).ToArray();
            return algorithm.ComputeHash(hashBytes);
        }

        private static byte[] EncodeStr(string s) => Encoding.UTF8.GetBytes(s);
    }
}
