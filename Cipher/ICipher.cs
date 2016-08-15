namespace Kuehl.Chipher
{
    public interface ICipher
    {
        string DecryptText(byte[] encrypted);

        byte[] EncryptText(string plainText);
    }
}