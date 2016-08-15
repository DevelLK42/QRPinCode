namespace Kuehl.Chipher
{
    using System.IO;
    using System.Security.Cryptography;

    public class EncrypthDHBase
    {
        private readonly object myLock = new object();
        public ECDiffieHellmanCng EDHCng { get; private set; }

        public byte[] PublicKey
        {
            get
            {
                return this.EDHCng == null ? null : this.EDHCng.PublicKey.ToByteArray();
            }
        }

        public EncrypthDHBase()
        {
            this.EDHCng = new ECDiffieHellmanCng();
            this.EDHCng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            this.EDHCng.HashAlgorithm = CngAlgorithm.Sha256;
        }

        public byte[] Key { get; private set; }

        public void InitKey(byte[] publicKey, CngKeyBlobFormat format)
        {
            this.Key = this.EDHCng?.DeriveKeyMaterial(CngKey.Import(publicKey, format));
        }

        public byte[] Receive(EncrypthedData encdata)
        {
            byte[] result = null;
            try
            {
                if (encdata?.Data != null && encdata.Data.Length > 0)
                {
                    lock (this.myLock)
                    {
                        using (Aes aes = new AesCryptoServiceProvider())
                        {
                            aes.Key = this.Key;
                            aes.IV = encdata.Vector;
                            // Decrypt the message
                            using (MemoryStream plaintext = new MemoryStream())
                            {
                                using (MemoryStream encryptedtext = new MemoryStream(encdata.Data))
                                {
                                    using (CryptoStream cs = new CryptoStream(encryptedtext, aes.CreateDecryptor(), CryptoStreamMode.Read))
                                    {
                                        var clearText = new byte[encdata.Data.LongLength];
                                        cs.CopyTo(plaintext);
                                        var  clearTextByteSize = cs.Read(clearText, 0, clearText.Length);
                                        encryptedtext.Close();
                                        cs.Close();
                                        result = clearText;
                                        var result2 = plaintext.ToArray();
                                        result = result2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            return result;
        }

        public EncrypthedData Send(byte[] key, byte[] secretMessage)
        {
            byte[] vector = null;
            byte[] data = null;
            try
            {
                lock (this.myLock)
                {
                    using (Aes aes = new AesCryptoServiceProvider())
                    {
                        aes.Key = key;
                        vector = aes.IV;

                        // Encrypt the message
                        using (MemoryStream ciphertext = new MemoryStream())
                        using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            var plaintextMessage = secretMessage;
                            cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                            cs.FlushFinalBlock();

                            data = ciphertext.ToArray();
                            ciphertext.Close();
                            cs.Close();
                        }
                    }
                }
            }
            catch
            {
            }

            return new EncrypthedData(data, vector);
        }
    }
}