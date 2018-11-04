using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private string name = "none";
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha1 = new HMACSHA1(keyByte))
            {
                byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        public string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public async Task post(string name, Image image)
        {
            VWS pavan = new VWS();
            pavan.AddTargetAsync(name, 1, image, true, name);
            /* 
            client.DefaultRequestHeaders.Add("Authorization", auth);
            var response = await client.PostAsync("https://vws.vuforia.com/targets", new StringContent(output, Encoding.UTF8, "application/json"));

            string responseString = await response.Content.ReadAsStringAsync(); */

        }
        public Form1()
        {
            InitializeComponent();

            lastBytesIn = getBytesIn();
            lastBytesOut = getBytesOut();
            this.InitTimer();
        }
        private Timer timer1;
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.GCP();
        }
        private long lastBytesOut;
        private long lastBytesIn;
        private long getBytesOut()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return 0;

            NetworkInterface[] interfaces
                = NetworkInterface.GetAllNetworkInterfaces();
            long bytesSent = 0;
            long bytesRec = 0;
            foreach (NetworkInterface ni in interfaces)
            {
                bytesSent += ni.GetIPv4Statistics().BytesSent;
                bytesRec += ni.GetIPv4Statistics().BytesReceived;
            }
            return bytesSent;

        }
        private long getBytesIn()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return 0;

            NetworkInterface[] interfaces
                = NetworkInterface.GetAllNetworkInterfaces();
            long bytesSent = 0;
            long bytesRec = 0;
            foreach (NetworkInterface ni in interfaces)
            {
                bytesSent += ni.GetIPv4Statistics().BytesSent;
                bytesRec += ni.GetIPv4Statistics().BytesReceived;
            }
            return bytesRec;
        }
        private async Task GCP()
        {
            string value = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            string test = "test";

            FirestoreDb db = FirestoreDb.Create("codered-451b3");
            DocumentReference docRef = db.Collection("clients").Document(this.name);
            
            long bytesI = getBytesIn();
            long bytesO = getBytesOut();
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "bytesOut", bytesI - this.lastBytesIn },
                { "bytesIn", bytesO -  this.lastBytesOut }
            };
            try {
                await docRef.SetAsync(user);
                lastBytesIn = bytesI;
                lastBytesOut = bytesO;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Zen.Barcode.CodeQrBarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
            pictureBox2.Image = barcode.Draw(textBox1.Text, 50);
            this.name = textBox1.Text;
            string image = Convert.ToBase64String(imageToByteArray(barcode.Draw(textBox1.Text, 50)));
            post(textBox1.Text, pictureBox2.Image);

        }
    }
}
