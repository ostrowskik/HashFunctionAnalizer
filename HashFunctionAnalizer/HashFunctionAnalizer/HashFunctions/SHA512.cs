﻿using System;

namespace HashFunctionAnalizer.HashFunctions
{
    internal class Sha512
    {
        private static readonly ulong[] K =
        {
            0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc,
            0x3956c25bf348b538, 0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118,
            0xd807aa98a3030242, 0x12835b0145706fbe, 0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2,
            0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235, 0xc19bf174cf692694,
            0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
            0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5,
            0x983e5152ee66dfab, 0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4,
            0xc6e00bf33da88fc2, 0xd5a79147930aa725, 0x06ca6351e003826f, 0x142929670a0e6e70,
            0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed, 0x53380d139d95b3df,
            0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
            0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30,
            0xd192e819d6ef5218, 0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8,
            0x19a4c116b8d2d0c8, 0x1e376c085141ab53, 0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8,
            0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373, 0x682e6ff3d6b2b8a3,
            0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
            0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b,
            0xca273eceea26619c, 0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178,
            0x06f067aa72176fba, 0x0a637dc5a2c898a6, 0x113f9804bef90dae, 0x1b710b35131c471b,
            0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc, 0x431d67c49c100d4c,
            0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817
        };

        private readonly ulong[] _h = new ulong[8];

        public Sha512()
        {
            Initialize();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual ulong[] Hash(byte[] data)
        {
            Initialize();
            return TransformBlock(data);
        }

        protected virtual void Initialize()
        {
            _h[0] = 0x6a09e667f3bcc908;
            _h[1] = 0xbb67ae8584caa73b;
            _h[2] = 0x3c6ef372fe94f82b;
            _h[3] = 0xa54ff53a5f1d36f1;
            _h[4] = 0x510e527fade682d1;
            _h[5] = 0x9b05688c2b3e6c1f;
            _h[6] = 0x1f83d9abfb41bd6b;
            _h[7] = 0x5be0cd19137e2179;
        }

        private ulong[] PadInput(byte[] input)
        {
            var bytesToPad = Convert.ToUInt32((128 - (input.Length%128))%128);

            var paddedInput = new byte[input.Length + bytesToPad];

            if (bytesToPad == 0)
            {
                Array.Copy(input, paddedInput, paddedInput.Length);
            }
            else
            {
                for (var i = 0; i < input.Length; i++)
                {
                    paddedInput[i] = input[i];
                }
                paddedInput[input.Length] = 0x80;

                for (var i = 1; i < bytesToPad - 2; i++)
                {
                    paddedInput[input.Length + i] = 0;
                }
                paddedInput[paddedInput.Length - 2] = GetByte((128 - bytesToPad)*8, 1);
                paddedInput[paddedInput.Length - 1] = GetByte((128 - bytesToPad)*8, 0);
            }

            ////Debug output for check propernes of message padding
            //Console.WriteLine("List of padded message before spliting in 1024bits blocks:");
            //var jb = 0;
            //string haszekBPad = String.Empty;
            //foreach (var bytek in paddedInput)
            //{
            //    jb++;
            //    haszekBPad = String.Format("{0:x}",bytek);
            //    Console.Write($"{haszekBPad}");
            //    if (jb % 4 == 0) Console.Write(" ");
            //    if (jb % 16 == 0) Console.WriteLine("");
            //}
            //Console.WriteLine("");

            //Input is padded to 1024bit size blocks

            var result = new ulong[paddedInput.Length/8];
            for (var i = 0; i < paddedInput.Length; i += 8)
            {
                ulong temp = 0;
                temp += paddedInput[i];
                temp = temp << 8;

                temp += paddedInput[i + 1];
                temp = temp << 8;

                temp += paddedInput[i + 2];
                temp = temp << 8;

                temp += paddedInput[i + 3];
                temp = temp << 8;

                temp += paddedInput[i + 4];
                temp = temp << 8;

                temp += paddedInput[i + 5];
                temp = temp << 8;

                temp += paddedInput[i + 6];
                temp = temp << 8;

                temp += paddedInput[i + 7];
                result[i/8] = temp;
            }

            ////Debug view of padded message
            //Console.WriteLine("List of padded message after spliting in 1024bits blocks:");
            //string haszekPad = string.Empty;
            //foreach (var bytek in result)
            //{
            //    haszekPad = String.Format("{0:x}",bytek);
            //    Console.Write($"{haszekPad} ");
            //}
            //Console.WriteLine("");

            return result;
        }

        private static byte GetByte(ulong x, int n)
        {
            return (byte) ((x >> 8*n) & 0xFF);
        }

        private static ulong Shr(int bit, ulong x)
        {
            return x >> bit;
        }

        private static ulong Rotr(int bit, ulong x)
        {
            return (x >> bit) | (x << (64 - bit));
        }

        private static ulong Rotl(int bit, ulong x)
        {
            return (x << bit) | (x >> 64 - bit);
        }

        private static ulong Ch(ulong x, ulong y, ulong z)
        {
            return ((x & y) ^ ((x ^ 0xffffffffffffffff) & z)) % 0xffffffffffffffff;
        }

        private static ulong Maj(ulong x, ulong y, ulong z)
        {
            return (x & y) ^ (x & z) ^ (y & z);
        }

        private static ulong Bsig0(ulong x)
        {
            return Rotr(28, x) ^ Rotr(34, x) ^ Rotr(39, x);
        }

        private static ulong Bsig1(ulong x)
        {
            return Rotr(14, x) ^ Rotr(18, x) ^ Rotr(41, x);
        }

        private static ulong Ssig0(ulong x)
        {
            return Rotr(1, x) ^ Rotr(8, x) ^ Shr(7, x);
        }

        private static ulong Ssig1(ulong x)
        {
            return Rotr(19, x) ^ Rotr(61, x) ^ Shr(6, x);
        }

        protected virtual ulong[] TransformBlock(byte[] aData)
        {
            var data = PadInput(aData);
            var resultHash = new ulong[80];

            int t;

            for (t = 0; t < 16; t++)
            {
                resultHash[t] = data[t];
            }

            for (t = 16; t < 80; t++)
            {
                resultHash[t] = (Ssig1(resultHash[t - 2]) + resultHash[t - 7] + Ssig0(resultHash[t - 15]) +
                                resultHash[t - 16])% 0xffffffffffffffff;
            }

            var a = _h[0];
            var b = _h[1];
            var c = _h[2];
            var d = _h[3];
            var e = _h[4];
            var f = _h[5];
            var g = _h[6];
            var h = _h[7];

            for (t = 0; t < 80; t++)
            {
                var tmp1 = h + Bsig1(e) + Ch(e, f, g) + K[t] + resultHash[t];
                var tmp2 = Bsig0(a) + Maj(a, b, c);
                h = g;
                g = f;
                f = e;
                e = d + tmp1;
                d = c;
                c = b;
                b = a;
                a = tmp1 + tmp2;
            }

            _h[0] = a + _h[0];
            _h[1] = b + _h[1];
            _h[2] = c + _h[2];
            _h[3] = d + _h[3];
            _h[4] = e + _h[4];
            _h[5] = f + _h[5];
            _h[6] = g + _h[6];
            _h[7] = h + _h[7];

            ////Debug view for checking proper change of hash massage
            //string hashString = string.Empty;
            //foreach (var cos in _h)
            //{
            //    hashString = String.Format("{0:x}",cos);
            //    Console.Write($"{hashString} ");
            //}

            return _h;
        }
    }
}