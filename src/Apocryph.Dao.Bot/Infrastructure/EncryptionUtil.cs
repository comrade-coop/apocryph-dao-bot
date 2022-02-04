using System;
using Org.BouncyCastle.Crypto.Digests;

namespace Apocryph.Dao.Bot.Infrastructure
{
    public static class EncryptionUtil
    {
        public static string ByteToHash(byte[] bytes)
        {
            var digest = new KeccakDigest(256);
            digest.BlockUpdate(bytes, 0, bytes.Length);
            var calculatedHash = new byte[digest.GetByteLength()];
            digest.DoFinal(calculatedHash, 0);
            var hash = BitConverter.ToString(calculatedHash, 0, 32).Replace("-", "").ToLower();
            return hash;
        }
    }
}