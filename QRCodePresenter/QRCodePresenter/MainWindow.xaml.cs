using Kuehl.Chipher;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace QRCodePresenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private RSAChipher Cipher { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Cipher = new RSAChipher();
            this.Cipher.InitKey(RSAKey.PublicKey);
            this.CreateNewPinRequest();
        }

        public void CreateNewPinRequest()
        {
            PinManager.Instance.CreatePin(this.Cipher);
            var encrypted = Convert.ToBase64String(PinManager.Instance.EncryptedPin);
            this.CipherQRCode.Source = CreateQRCode(encrypted);
        }

        public static BitmapImage CreateQRCode(string qrValue)
        {
            BitmapImage result = null;
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,

                Options = new EncodingOptions
                {
                    Height = 500,
                    Width = 500,
                    Margin = 1,
                }
            };

            using (var bitmap = barcodeWriter.Write(qrValue))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                var bi = new BitmapImage();
                bi.BeginInit();
                stream.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = stream;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                result = bi; //A WPF Image control
            }

            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.VerifyResult.Text = PinManager.Instance.VerifyPin(this.PinText.Text) ? "Access" : "Failed";
        }

        private void PinText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.VerifyResult != null)
            {
                this.VerifyResult.Text = "Enter pin and verify";
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            this.CreateNewPinRequest();
        }
    }
}