namespace Kuehl.Chipher
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class PinManager
    {
        private byte[] Hash { get; set; }
        public byte[] EncryptedPin { get; set; }
        private readonly static Random ourRandomizer = new System.Random();

        private readonly static PinManager ourInstance = new PinManager();

        public static PinManager Instance
        {
            get
            {
                return ourInstance;
            }
        }

        private string DateString
        {
            get
            {
                return DateTime.UtcNow.Date.ToBinary().ToString();
            }
        }

        public static Random Randomizer
        {
            get
            {
                return ourRandomizer;
            }
        }

        public void CreatePin(ICipher cipher)
        {
            var pin = $"{System.Math.Ceiling(PinManager.Randomizer.NextDouble() * 1000000):000000}";
            this.Hash = this.HashPin(pin);
            this.EncryptedPin = cipher.EncryptText(pin);
        }

        private byte[] HashPin(string pin)
        {
            byte[] result = null;
            if (!string.IsNullOrWhiteSpace(pin))
            {
                var hashData = Encoding.Default.GetBytes(pin + this.DateString);
                var sha = SHA256.Create();
                result = sha.ComputeHash(hashData);
            }

            return result;
        }

        public bool VerifyPin(string pin)
        {
            var result = false;
            var data = this.HashPin(pin);
            if (this.Hash != null && data != null && this.Hash.LongLength == data.LongLength)
            {
                var test = true;

                for (long i = 0; i < this.Hash.LongLength && test; i++)
                {
                    test = this.Hash[i] == data[i];
                }

                result = test;
            }

            return result;
        }
    }
}