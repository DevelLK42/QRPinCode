namespace Kuehl.Chipher
{
    public class EncrypthedData
    {
        public byte[] Data { get; private set; }
        public byte[] Vector { get; private set; }

        public string DataText
        {
            get
            {
                return System.Convert.ToBase64String(this.Data);
            }
            set
            {
                this.Data = System.Convert.FromBase64String(value);
            }
        }

        public string VectorText
        {
            get
            {
                return System.Convert.ToBase64String(this.Vector);
            }
            set
            {
                this.Vector = System.Convert.FromBase64String(value);
            }
        }

        public static bool? IsVaild(EncrypthedData encdata) => encdata?.Data != null && encdata?.Vector != null && encdata.Data.Length > 0 && encdata.Vector.Length > 0;

        public EncrypthedData(byte[] data, byte[] vector)
        {
            this.Data = data;
            this.Vector = vector;
        }
    }
}