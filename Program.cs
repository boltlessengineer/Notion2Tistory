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
            string path = @"C:\Users\seong\OneDrive\Documents\Personal\Blog\_Notion_Export\";

            string clientTxtPath = @"C:\Users\seong\OneDrive\Documents\Personal\Blog\_User\info.txt";
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
            Console.WriteLine("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", clientId, clientSK, redirect, userID, userPW, blogName);

            TistoryAPI client = new TistoryAPI(clientId, clientSK, redirect, userID, userPW, blogName);

            void EventHandler (string tmpPath)
            {
                // Delay(2000);
                // 여기에 apiClient, Converter등을 이용한 코드 입력
                DirectoryInfo tmpDir = new DirectoryInfo(tmpPath);
                foreach (FileInfo file in tmpDir.GetFiles())
                {
                    if (file.Extension.ToLower() == ".html")
                    {
                        Console.WriteLine("{0} is html file", file.Name);
                        Content content = NotionReader.Read(file.FullName);

                        string attachedDIrPath = file.FullName.Replace(".html", "");
                        if(Directory.Exists(attachedDIrPath))
                        {/*
                            List<FileInfo> AttachedList = new List<FileInfo>();
                            DirectoryInfo attachedFileDir = new DirectoryInfo(attachedDIrPath);
                            foreach (FileInfo ImageFile in attachedFileDir.GetFiles())
                            {
                                Console.WriteLine(ImageFile.FullName);
                                AttachedList.Add(ImageFile);
                            }*/

                            content.SetImages(NotionReader.GetImageList(content.content, tmpPath));
                            
                        }
                        content.images = client.UploadImages(content.images);
                        content = Converter.ChangeHtml(content);
                        /*
                        content.content =
                        "<h1>이미지1</h1>[##_Image|t/cfile@99729C4A5EF7641D19|alignCenter|data-origin-width=\"0\" data-origin-height=\"0\" data-ke-mobilestyle=\"widthContent\"|일단 테스트를 위해 Program.cs 파일의 Main class 안에서 돌리고 있다.||_##]"
                        + "<h1>이미지2</h1>[##_Image|t/cfile@995098455EF764222C|alignCenter|data-origin-width=\"0\" data-origin-height=\"0\" data-ke-mobilestyle=\"widthContent\"|||_##]"
                        + "<h1>이미지3</h1>[##_Image|t/cfile@997E123D5EF7642518|alignCenter|data-origin-width=\"0\" data-origin-height=\"0\" data-ke-mobilestyle=\"widthContent\"|오늘도 가독성을 위해 열일하는 200% 확대||_##]"
                        + "<h1>이미지4</h1>[##_Image|t/cfile@99BF9E505EF764271E|alignCenter|data-origin-width=\"0\" data-origin-height=\"0\" data-ke-mobilestyle=\"widthContent\"|||_##]";
                        */
                        Console.WriteLine(content.content);
                        Console.WriteLine(">{0}<", TistoryAPI.accessToken);
                        //while (Console.ReadKey().Key != ConsoleKey.A) ;
                        client.UploadPost(content);
                        //TistoryAPI.UploadPost2(content);
                        //ㅅ 이게 문제였음... multipart-form data 방식 말고 원래 하던 그거로 해보자.
                        //아니면 같은 방식으로 하되, 작동하는 치환자를 미리 넣어놓고 해보자.
                        //[*] 작동하는 치환자를 업로드했을 때, 치환자가 계속 작동하는지 확인
                        //[X] multipart-form data 방식 말고 다른 방식으로 업로드 했을 때 확인
                    }
                }
                try
                {
                    Directory.Delete(tmpPath, true);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
            FileWatcher watcher = new FileWatcher(path); //path는 _NotionExport 경로
            watcher.InitWatcher(EventHandler); // EventHandler = Created 함수

            /*
            string example = "<figure id=\"a3b39743-5dc4-4723-a2b0-c9c96f492b66\" class=\"image\"><a href=\"Notion%20to%20Tistory%2026%20C%20a3b397435dc44723a2b0c9c96f492b66/Untitled.png\"><img style=\"width:1468px\" src=\"Notion%20to%20Tistory%2026%20C%20a3b397435dc44723a2b0c9c96f492b66/Untitled.png\"/></a><figcaption>일단 테스트를 위해 Program.cs 파일의 Main class 안에서 돌리고 있다.</figcaption></figure><p id=\"111117df-2717-4f34-9014-a301497a4395\" class=\"\">그리고 성공했다!</p>";
            List<Converter.ReplaceImage.NotionImage> n = Converter.ReplaceImage.FindImageTag("Notion to Tistory 26 C a3b397435dc44723a2b0c9c96f492b66", example);
            example = Converter.ReplaceImage.ChangeImageTag(example, n, client);
            Console.WriteLine(example);
            */
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }
    }
}