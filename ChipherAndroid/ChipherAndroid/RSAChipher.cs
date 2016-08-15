namespace Android.Kuehl.Chipher
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class RSAChipher : ICipher
    {
        private RSACryptoServiceProvider myProivder;

        public RSACryptoServiceProvider Proivder
        {
            get { return this.myProivder; }
            set
            {
                if (this.myProivder != null)
                {
                    this.myProivder.Clear();
                }

                this.myProivder = value;
            }
        }

        public string ExportKey()
        {
            return Convert.ToBase64String(this.Proivder.ExportCspBlob(true));
        }

        public string ExportPublicKey()
        {
            return Convert.ToBase64String(this.Proivder.ExportCspBlob(false));
        }

        public RSAParameters PublicKey
        {
            get
            {
                return this.Proivder.ExportParameters(false);
            }
        }

        public byte[] EncryptText(string plainText)
        {
            return this.Proivder.Encrypt(Encoding.Default.GetBytes(plainText), false);
        }

        public string DecryptText(byte[] encrypted)
        {
            var data = this.Proivder.Decrypt(encrypted, false);
            return data != null ? Encoding.Default.GetString(data) : null;
        }

        public void CreateKey(int bitLength)
        {
            this.Proivder = new RSACryptoServiceProvider(bitLength);
        }

        public void InitKey(RSAParameters parameters)
        {
            this.Proivder = new RSACryptoServiceProvider();
            this.Proivder.ImportParameters(parameters);
        }

        public void InitKey(string key)
        {
            this.Proivder = new RSACryptoServiceProvider();
            if (!string.IsNullOrEmpty(key))
            {
                this.Proivder.ImportCspBlob(Convert.FromBase64String(key));
            }
        }
    }
}