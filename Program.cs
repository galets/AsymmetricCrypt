/*
    Copyright 2013, galets

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AsymmetricCrypt
{
    class Program
    {
        static readonly string HEADER = "ASCR";
        const int KEY_SIZE = 4096;
        const int MODULUS_SIZE = KEY_SIZE / 8;

        SymmetricAlgorithm AES = new RijndaelManaged();

        static RSACryptoServiceProvider NewAsymmetricAlgorithm(bool forExport = false)
        {
            const int PROV_RSA_FULL = 1;

            var cspParams = new CspParameters()
            {
                ProviderType = PROV_RSA_FULL,  
                Flags = forExport ? CspProviderFlags.UseArchivableKey : 0,
                KeyNumber = (int)KeyNumber.Exchange,
            };
            var rsaProvider = new RSACryptoServiceProvider(4096, cspParams);
            return rsaProvider;
        }

        static void Encrypt(Stream input, Stream output, RSACryptoServiceProvider asymmetricAlg)
        {
            var header = Encoding.ASCII.GetBytes(HEADER);
            output.Write(header, 0, header.Length);

            var aes = new RijndaelManaged() { KeySize = 256 };
            aes.GenerateIV();
            output.Write(aes.IV, 0, aes.IV.Length);

            aes.GenerateKey();
            var key = asymmetricAlg.Encrypt(aes.Key, false);
            if (key.Length != MODULUS_SIZE)
            {
                throw new InvalidOperationException();
            }
            output.Write(key, 0, key.Length);

            using (var cs = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] buffer = new byte[1024 * 16];
                int count;
                while ((count = input.Read(buffer, 0, buffer.Length)) != 0)
                {
                    cs.Write(buffer, 0, count);
                }
            }
        }

        static void Decrypt(Stream input, Stream output, RSACryptoServiceProvider asymmetricAlg)
        {
            var header = new byte[HEADER.Length]; 
            input.Read(header, 0, header.Length);
            if (Encoding.ASCII.GetString(header) != HEADER)
            {
                throw new InvalidOperationException();
            }

            var aes = new RijndaelManaged() { KeySize = 256 };
            var iv = new byte[aes.IV.Length];
            input.Read(iv, 0, iv.Length);
            aes.IV = iv;

            var key = new byte[MODULUS_SIZE];
            input.Read(key, 0, key.Length);
            aes.Key = asymmetricAlg.Decrypt(key, false);

            using (var cs = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                byte[] buffer = new byte[1024 * 16];
                int count;
                while ((count = cs.Read(buffer, 0, buffer.Length)) != 0)
                {
                    output.Write(buffer, 0, count);
                }
            }
        }

        static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new ArgumentException();
                }

                if (args[0] == "--genkey")
                {
                    Console.Error.WriteLine("Generating 4096 bit keypair. This could take a little bit of time...");

                    var pk = NewAsymmetricAlgorithm(true);
                    var xml = pk.ToXmlString(true);
                    Console.Out.WriteLine(xml);
                    Console.Out.Close();
                }
                else if (args[0] == "--publickey")
                {
                    var pk = NewAsymmetricAlgorithm();
                    var privateKey = Console.In.ReadToEnd();
                    pk.FromXmlString(privateKey);
                    var xml = pk.ToXmlString(false);
                    Console.WriteLine(xml);
                    Console.Out.Close();
                }
                else if (args[0] == "--encrypt")
                {
                    var pk = NewAsymmetricAlgorithm();

                    using (var tr = new StreamReader(args[1]))
                    {
                        var privateKey = tr.ReadToEnd();
                        pk.FromXmlString(privateKey);
                    }
                    Encrypt(Console.OpenStandardInput(), Console.OpenStandardOutput(), pk);
                }
                else if (args[0] == "--decrypt")
                {
                    var pk = NewAsymmetricAlgorithm();

                    using (var tr = new StreamReader(args[1]))
                    {
                        var privateKey = tr.ReadToEnd();
                        pk.FromXmlString(privateKey);
                    }
                    Decrypt(Console.OpenStandardInput(), Console.OpenStandardOutput(), pk);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine("Usage:");
                Console.Error.WriteLine("   AsymmetricCrypt [--encrypt|--decrypt|--genkey|--publickey]");
                Console.Error.WriteLine("Example:");
                Console.Error.WriteLine("   AsymmetricCrypt --genkey >private.key");
                Console.Error.WriteLine("   AsymmetricCrypt --publickey <private.key >public.key");
                Console.Error.WriteLine("   AsymmetricCrypt --encrypt public.key <plaintext.txt >encrypted.ascr");
                Console.Error.WriteLine("   AsymmetricCrypt --decrypt private.key <encrypted.ascr >plaintext.txt");
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            return 0;
        }
    }
}
