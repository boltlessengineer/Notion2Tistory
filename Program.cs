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
using Microsoft.VisualBasic;

namespace Notion2TistoryConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string clientTxtPath = @"C:\Users\seong\OneDrive\Documents\Personal\Blog\_User\info.txt";
            //Console.Write("Client id     : ");
            //Console.Write("Secret Key    : ");
            //Console.Write("Redirect url  : ");
            //Console.Write("User id       : ");
            //Console.Write("User password : ");
            //Console.Write("Blog name     : ");
            string clientId;
            string clientSK;
            string redirect;
            string userID;
            string userPW;
            string blogName;

            string readTxt = File.ReadAllText(clientTxtPath);
            clientId = readTxt.Split("|")[0];
            clientSK = readTxt.Split("|")[1];
            redirect = readTxt.Split("|")[2];
            userID = readTxt.Split("|")[3];
            userPW = readTxt.Split("|")[4];
            blogName = readTxt.Split("|")[5];
            //Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", clientId, clientSK, redirect, userID, userPW, blogName);

            TistoryAPI client = new TistoryAPI(clientId, clientSK, redirect, userID, userPW, blogName);
            /*

            watcher.InitWatcher();
            Console.Read();
            * /
            string at = client.AccessToken();
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
                            Name = "testimage",
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

            string imageId = url.Substring(url.IndexOf("image/") + 6, url.Length - url.IndexOf("image/") - 10);
            string imageReplacer = "[##_Image|t/cfile@" + imageId + "|alignCenter|data-origin-width=\"0\" data-origin-height=\"0\" data-ke-mobilestyle=\"widthContent\"|||_##]";
            Console.WriteLine("===========================================================");
            Console.WriteLine("Replacer : {0}", replacer);
            Console.WriteLine("Url      : {0}", url);
            Console.WriteLine("===========================================================");
            Console.WriteLine("Image Replacer : {0}", imageReplacer);
            Console.WriteLine("===========================================================");
            */

            void EventHandler (object source, FileSystemEventArgs ev)
            {
                // 여기에 apiClient, Converter등을 이용한 코드 입력
            }
            
            FileWatcher watcher = new FileWatcher(path); //path는 _NotionExport 경로
            watcher.InitWatcher(Func<object, FileSystemEventArgs> EventHandler); // EventHandler = Created 함수


            string example = "<figure id=\"a3b39743-5dc4-4723-a2b0-c9c96f492b66\" class=\"image\"><a href=\"Notion%20to%20Tistory%2026%20C%20a3b397435dc44723a2b0c9c96f492b66/Untitled.png\"><img style=\"width:1468px\" src=\"Notion%20to%20Tistory%2026%20C%20a3b397435dc44723a2b0c9c96f492b66/Untitled.png\"/></a><figcaption>일단 테스트를 위해 Program.cs 파일의 Main class 안에서 돌리고 있다.</figcaption></figure><p id=\"111117df-2717-4f34-9014-a301497a4395\" class=\"\">그리고 성공했다!</p>";
            List<Converter.ReplaceImage.NotionImage> n = Converter.ReplaceImage.FindImageTag("Notion to Tistory 26 C a3b397435dc44723a2b0c9c96f492b66", example);
            example = Converter.ReplaceImage.ChangeImageTag(example, n, client);
            Console.WriteLine(example);
        }

        public class MyFile
        {
            public string originalTag;
            public string originalPath;
            public string originalStyle;
            public string originalCaption;

            public string replacer;
        }
    }
}