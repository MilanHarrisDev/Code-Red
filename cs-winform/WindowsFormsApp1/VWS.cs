using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class VWSTarget
    {
        public string name;
        public float width;
        public string image;
        public bool active_flag;
        public string application_metadata;
        
        public VWSTarget(string name, float width, string image, bool active_flag, string application_metadata)
        {
            this.name = name;
            this.width = width;
            this.image = image;
            this.active_flag = active_flag;
            this.application_metadata = application_metadata;
        }
    }

    class VWS
    {
        public string accessKey = "875ced593756205edccf39969ff685ff4ce1f07a";
        public string secretKey = "75e85e49f3db11f3d331e9544669f15b669454ad";
        private static readonly HttpClient client = new HttpClient();
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        public async Task AddTargetAsync(string targetName, float width, Image image, bool active_flag, string metadata)
        {
            String imageString = Convert.ToBase64String(imageToByteArray(image));
            string meta = Convert.ToBase64String(Encoding.UTF8.GetBytes(metadata));

            VWSTarget newTarget = new VWSTarget(targetName, width, imageString, active_flag, meta);
            
            string content = JsonConvert.SerializeObject(newTarget);
            string[] query = new string[5];
            String date = DateTime.UtcNow.ToString("ddd, dd MMM yyy HH:mm:ss") + " GMT";
            query[0] = "POST"; // method
            query[1] = CalculateMD5Hash(content).ToLower(); // content
            query[2] = "application/json"; // content type
            query[3] = date; // date
            query[4] = "/targets"; // url

            string stringToSign = string.Join("\n", query);

            string signature = "VWS " + accessKey + ":" + BuildSignature(secretKey, stringToSign);
            client.DefaultRequestHeaders.Add("Authorization", signature);
            client.DefaultRequestHeaders.Add("Date", date);
            /* client.DefaultRequestHeaders.Add("Content-Type", "application/json"); */
            var response = await client.PostAsync("https://vws.vuforia.com/targets", new StringContent(content, Encoding.UTF8, "application/json"));

            string responseString = await response.Content.ReadAsStringAsync();

        }
        private static string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
        private static string BuildSignature(string keyString, string stringToSign)
        {
            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] data = Encoding.UTF8.GetBytes(stringToSign);

            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            myhmacsha1.Initialize();
            MemoryStream stream = new MemoryStream(data);

            byte[] hash = myhmacsha1.ComputeHash(stream);

            string signature = System.Convert.ToBase64String(hash);
            return signature;
        }
    }
}
