using Godot;
using System;
using System.Security.Cryptography;

namespace KRU.Networking.Security
{
    public static class ENetSecurity
    {
        private const int SaltByteLength = 24;
        private const int DerivedKeyLength = 24;
        private const int IterationCount = 24000;

        private static byte[] GenerateRandomSalt()
        {
            var csprng = new RNGCryptoServiceProvider();
            var salt = new byte[SaltByteLength];
            csprng.GetBytes(salt);
            return salt;
        }

        private static byte[] GenerateHashValue(string password, byte[] salt, int iterationCount)
        {
            byte[] hashValue;
            var valueToHash = string.IsNullOrEmpty(password) ? string.Empty : password;
            using (var pbkdf2 = new Rfc2898DeriveBytes(valueToHash, salt, iterationCount))
            {
                hashValue = pbkdf2.GetBytes(DerivedKeyLength);
            }
            return hashValue;
        }

        public static string CreatePasswordHash(string password)
        {
            var salt = GenerateRandomSalt();
            var iterationCount = 24000;
            var hashValue = GenerateHashValue(password, salt, iterationCount);
            var iterationCountBtyeArr = BitConverter.GetBytes(iterationCount);
            var valueToSave = new byte[SaltByteLength + DerivedKeyLength + iterationCountBtyeArr.Length];
            Buffer.BlockCopy(salt, 0, valueToSave, 0, SaltByteLength);
            Buffer.BlockCopy(hashValue, 0, valueToSave, SaltByteLength, DerivedKeyLength);
            Buffer.BlockCopy(iterationCountBtyeArr, 0, valueToSave, salt.Length + hashValue.Length, iterationCountBtyeArr.Length);
            return Convert.ToBase64String(valueToSave);
            GD.Print("blah.");
        }

        private static bool ConstantTimeComparison(byte[] passwordGuess, byte[] actualPassword)
        {
            uint difference = (uint)passwordGuess.Length ^ (uint)actualPassword.Length;
            for (var i = 0; i < passwordGuess.Length && i < actualPassword.Length; i++)
            {
                difference |= (uint)(passwordGuess[i] ^ actualPassword[i]);
            }

            return difference == 0;
        }
    }
}