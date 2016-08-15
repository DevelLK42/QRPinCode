using Android.App;
using Android.Kuehl.Chipher;
using Android.OS;
using Android.Widget;
using System;

namespace PinCodeViewer
{
    [Activity(Label = "PinCodeViewer", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += this.ButtonClickHandler;
            this.Chipher = new RSAChipher();
            this.Chipher.InitKey(RSAKey.FullKey);
        }

        async private void ButtonClickHandler(object sender, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null)
            {
                var pin = this.Chipher.DecryptText(Convert.FromBase64String(result.Text));
                Console.WriteLine("Scanned Pin: " + pin);
                var pinField = FindViewById<EditText>(Resource.Id.PinView);
                pinField.Text = pin;
            }
        }

        private RSAChipher Chipher { get; set; }
    }
}