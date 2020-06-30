using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                DirectoryInfo tmpDir = new DirectoryInfo(tmpPath);
                foreach (FileInfo file in tmpDir.GetFiles())
                {
                    if (file.Extension.ToLower() == ".html")
                    {
                        Console.WriteLine("{0} is html file", file.Name);
                        Content content = NotionReader.Read(file.FullName);

                        string attachedDirPath = file.FullName.Replace(".html", "");
                        if(Directory.Exists(attachedDirPath))
                        {
                            DirectoryInfo attachedFileDir = new DirectoryInfo(attachedDirPath);
                            List<FileInfo> AttachedList = new List<FileInfo>();
                            /*
                            foreach (FileInfo ImageFile in attachedFileDir.GetFiles())
                            {
                                Console.WriteLine(ImageFile.FullName);
                                AttachedList.Add(ImageFile);
                            }
                            */
                            content.SetImages(NotionReader.GetImageList(content.content, attachedDirPath));
                            
                        }
                        content.images = client.UploadImages(content.images);
                        content.categoryId = client.FindCategory("A");
                        content = Converter.ChangeHtml(content);

                        Console.WriteLine(content.content);
                        client.UploadPost(content);
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
        }
    }
}