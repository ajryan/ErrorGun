using System;
using System.Configuration;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace ErrorGun.Web.Services
{
    public static class KeyGenerator
    {
        const int KEY_LENGTH = 16;
        const int ITERATION_COUNT = 1000;

        private static readonly byte[] _Salt = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["KeyGeneratorSalt"]);

        public static string Generate()
        {
            byte[] guid = Guid.NewGuid().ToByteArray();

            var hMac = new HMac(new Sha256Digest());
            int hmacSize = hMac.GetMacSize();
            int lengthMultipler = (KEY_LENGTH + hmacSize - 1) / hmacSize;

            byte[] block = new byte[4];
            byte[] outBytes = new byte[lengthMultipler * hmacSize];

            for (int index = 1; index <= lengthMultipler; index++)
            {
                IntToOctet(index, block);
                HashFunc(hMac, guid, block, outBytes, (index - 1) * hmacSize);
            }

            byte[] keyBytes = new byte[KEY_LENGTH];
            Buffer.BlockCopy(outBytes, 0, keyBytes, 0, KEY_LENGTH);

            return new Guid(keyBytes).ToString(); // for friendly formatting
        }

        private static void IntToOctet(int index, byte[] block)
        {
            block[0] = (byte)((uint)index >> 24);
            block[1] = (byte)((uint)index >> 16);
            block[2] = (byte)((uint)index >> 8);
            block[3] = (byte)index;
        }

        private static void HashFunc(HMac hMac, byte[] unique, byte[] block, byte[] outBytes, int outOff)
        {
            byte[] state = new byte[hMac.GetMacSize()];
            
            var cipherParam = new KeyParameter(unique);

            hMac.Init(cipherParam);
            hMac.BlockUpdate(_Salt, 0, _Salt.Length);
            hMac.BlockUpdate(block, 0, block.Length);
            hMac.DoFinal(state, 0);

            Array.Copy(state, 0, outBytes, outOff, state.Length);
            for (int iteration = 1; iteration < ITERATION_COUNT; iteration++)
            {
                hMac.Init(cipherParam);
                hMac.BlockUpdate(state, 0, state.Length);
                hMac.DoFinal(state, 0);

                for (int stateIndex = 0; stateIndex < state.Length; stateIndex++)
                {
                    outBytes[outOff + stateIndex] ^= state[stateIndex];
                }
            }
        }
    }
}