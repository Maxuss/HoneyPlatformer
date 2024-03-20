using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace DefaultNamespace
{
    public static class S
    {
        public static string IMPORTANT_DO_NOT_LOOK = "aHR0cHM6Ly93d3cueW91dHViZS5jb20vd2F0Y2g/dj1kUXc0dzlXZ1hjUQ==";

        private static long[] salt =
        {
            26, 75, 72, 78, 30, 29, 29, 30, 28, 78, 27, 79, 76, 27, 31, 28, 19, 73, 19, 78, 75, 78, 26, 19, 78, 18, 31,
            30, 31, 73, 18, 19, 27, 30, 24, 26, 25, 30, 75, 30, 29, 25, 75, 27, 75, 27, 72, 29, 18, 29, 24, 78, 27, 76,
            29, 31, 72, 18, 27, 72, 31, 24, 75, 73,
        };
        private static long[] pwd =
        {
            79, 78, 30, 26, 26, 79, 78, 28, 78, 75, 30, 30, 76, 72, 24, 19, 76, 26, 73, 30, 24, 73, 26, 28, 19, 19, 78,
            72, 79, 79, 75, 72, 19, 30, 73, 26, 73, 19, 24, 26, 25, 26, 27, 76, 26, 27, 72, 78, 18, 31, 73, 26, 79, 27,
            18, 31, 78, 30, 28, 73, 30, 25, 18, 19,
        };

        private static long[] vector = { 31, 28, 30, 76, 29, 78, 73, 30, 29, 79, 28, 26, 78, 24, 25, 29, };

        // public static void G()
        // {
        //     Debug.Log(IMPORTANT_DO_NOT_LOOK);
        //     
        //     var saltt = IlIllIl11IIII111("0abd47746d1ef1569c9dad09d8545c89142034a473a1a1b7872d1f75b81b52ac");
        //
        //     var vectort = IlIllIl11IIII111("564f7dc47e60d237");
        //
        //     var pwdt = IlIllIl11IIII111("ed400ed6da44fb29f0c42c0699dbeeab94c0c920301f01bd85c0e185d46c4389");
        //
        //     Debug.Log($"PATH2 {Enc("brz-sens", pwdt, saltt, vectort)}");
        //     Debug.Log($"PATH2 {Enc("logindata", pwdt, saltt, vectort)}");
        //     Debug.Log($"API URL {Enc("https://api.nti-gamedev.ru/", pwdt, saltt, vectort)}");
        // }
        
        public static string D(string val)
        {
            var decodedPwd = I1111Il1llIllI11(pwd);
            var decodedSalt = I1111Il1llIllI11(salt);
            var decodedVec = I1111Il1llIllI11(vector);
            return Dec(val, decodedPwd, decodedSalt, decodedVec);
        }

        private static string Dec(string val, long[] pwd, long[] salt, long[] vector)
        {
            var valBytes = Convert.FromBase64String(val);
            
            byte[] decrypted;
            var decryptedByteCount = 0;

            using (var aes = new AesManaged()) {
                using var _passwordBytes = new PasswordDeriveBytes(L2B(pwd), L2B(salt), "SHA1", 2);
                var keyBytes = _passwordBytes.GetBytes(256 / 8);

                aes.Mode = CipherMode.CBC;

                try {
                    using (var decryptor = aes.CreateDecryptor(keyBytes, L2B(vector))) {
                        using (var from = new MemoryStream(valBytes)) {
                            using (var reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read)) {
                                decrypted = new byte[valBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                } catch (Exception ex) {
                    return String.Empty;
                }

            }

            return Encoding.ASCII.GetString(decrypted, 0, decryptedByteCount);
        }

        private static string Enc(string val, long[] pwd, long[] salt, long[] vector)
        {
            byte[] v = Encoding.ASCII.GetBytes(val);
            byte[] encrypted;
            using (var aes = new AesManaged())
            {
                PasswordDeriveBytes _passwordBytes = 
                    new PasswordDeriveBytes(L2B(pwd), L2B(salt), "SHA1", 2);
                byte[] keyBytes = _passwordBytes.GetBytes(256 / 8);

                aes.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(keyBytes, L2B(vector))) {
                    using (MemoryStream to = new MemoryStream()) {
                        using (CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write)) {
                            writer.Write(v, 0, v.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                aes.Clear();
            }
            
            return Convert.ToBase64String(encrypted);
        }
        
        public static long[] IlIllIl11IIII111(string s)
        {
            var b = Encoding.UTF8.GetBytes(s);
            var l = new long[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                l[i] = b[i];
            }

            return l;
        }

        public static byte[] L2B(long[] v)
        {
            var b = new byte[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                b[i] = (byte)v[i];
            }

            return b;
        }
        
        public static long[] B2L(byte[] v)
        {
            var b = new long[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                b[i] =v[i];
            }

            return b;
        }

        public static long[] I1111Il1llIllI11(long[] x)
        {
            var xc = new long[x.Length];
            Array.Copy(x, xc, x.Length);
            IlI1lIl1llIllI11(xc);
            return xc;
        }
        
        // DECODE
        public static void IlI1lIl1llIllI11(long[] x)
        {
            for (var i = 0; i < x.Length; i++)
            {
                x[i] ^= 42;
            }
        }
        
    }
}