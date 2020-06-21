using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Notion2TistoryConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string clientTxtPath = @"C:\Users\seong\OneDrive\Documents\Personal\Blog\_User\info.txt";
            //Console.Write("Client id     : ");
            string clientId;// = Console.ReadLine();
            //Console.Write("Secret Key    : ");
            string clientSK; // = Console.ReadLine();
            //Console.Write("Redirect url  : ");
            string redirect; // = Console.ReadLine();
            //Console.Write("User id       : ");
            string userID; // = Console.ReadLine();
            //Console.Write("User password : ");
            string userPW; // = Console.ReadLine();
            //Console.Write("Blog name     : ");
            string blogName; // = Console.ReadLine();

            string readTxt = File.ReadAllText(clientTxtPath);
            clientId = readTxt.Split("|")[0];
            clientSK = readTxt.Split("|")[1];
            redirect = readTxt.Split("|")[2];
            userID = readTxt.Split("|")[3];
            userPW = readTxt.Split("|")[4];
            blogName = readTxt.Split("|")[5];
            Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", clientId, clientSK, redirect, userID, userPW, blogName);

            TistoryAPI client = new TistoryAPI(clientId, clientSK, redirect, userID, userPW, blogName);
            string at = client.AccessToken();
            /*
            FileWatcher watcher = new FileWatcher(client);

            watcher.InitWatcher();
            Console.Read();
            */
            string result;
            result = RequestHelper.PostMultipart(
                "https://www.tistory.com/apis/post/attach",
                new Dictionary<string, object>() {
                    { "access_token", at },
                    { "output", "json" },
                    { "blogName", "boltlessengineer" },
                    {
                        "uploadedfile", new FormFile()
                        {
                            Name = string.Format("{0}_{1}", "blbd_no", "pstg_no"),
                            ContentType = "image/png",
                            FilePath = @"C:\Users\seong\Documents\ShareX\Screenshots\2020-06\VsDebugConsole_wtUybwvfqj.png"
                        }
                    }
                }
            );
            JObject json = JObject.Parse(result);
            Console.WriteLine(json);
            string replacer = json["tistory"]["replacer"].ToString();
            string url = json["tistory"]["url"].ToString();
            Console.WriteLine("Replacer : {0}", replacer);
            // [##_1N|cfile3.uf@9999893E5EEF34D434F433|width="500" height="500" filename="blbd_no_pstg_no" filemime="image/png"|_##]
            // http://cfile3.uf.tistory.com/image/9999893E5EEF34D434F433

            string imageReplacer = "[##_Image|t/" + replacer.Split("|")[1] + "|alignCenter|data-origin-width=\"0\" data-origin-height=\"0\" data-ke-mobilestyle=\"widthContent\"|||_##]";
            Console.WriteLine("Image Replacer : {0}", imageReplacer);
            
        }

        // https://spirit32.tistory.com/21
        public class FormFile
        {
            public string Name { get; set; }
            public string ContentType { get; set; }
            public string FilePath { get; set; }
            public Stream Stream { get; set; }
        }

        public class RequestHelper
        {
            public static string PostMultipart(string url, Dictionary<string, object> parameters)
            {

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;

                if (parameters != null && parameters.Count > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        foreach (KeyValuePair<string, object> pair in parameters)
                        {
                            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                            if (pair.Value is FormFile)
                            {
                                FormFile file = pair.Value as FormFile;
                                string header = "Content-Disposition: form-data; name=\"" + pair.Key + "\"; filename=\"" + file.Name + "\"\r\nContent-Type: " + file.ContentType + "\r\n\r\n";
                                byte[] bytes = Encoding.UTF8.GetBytes(header);
                                requestStream.Write(bytes, 0, bytes.Length);
                                byte[] buffer = new byte[32768];
                                int bytesRead;
                                if (file.Stream == null)
                                {
                                    // upload from file
                                    using (FileStream fileStream = File.OpenRead(file.FilePath))
                                    {
                                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                            requestStream.Write(buffer, 0, bytesRead);
                                        fileStream.Close();
                                    }
                                }
                                else
                                {
                                    // upload from given stream
                                    while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) != 0)
                                        requestStream.Write(buffer, 0, bytesRead);
                                }
                            }
                            else
                            {
                                string data = "Content-Disposition: form-data; name=\"" + pair.Key + "\"\r\n\r\n" + pair.Value;
                                byte[] bytes = Encoding.UTF8.GetBytes(data);
                                requestStream.Write(bytes, 0, bytes.Length);
                            }
                        }
                        byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                        requestStream.Write(trailer, 0, trailer.Length);
                        requestStream.Close();
                    }
                }
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                        return reader.ReadToEnd();
                }
            }
        }
    }
}