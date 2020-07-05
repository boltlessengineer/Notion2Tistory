using System;
using System.IO;

namespace Notion2TistoryConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string jsonPath = @"C:\Users\seong\OneDrive\Documents\Personal\Blog\_User\test.json";
            Settings setting = new Settings(jsonPath);

            TistoryAPI client = new TistoryAPI(setting);
            
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
                            /*
                            DirectoryInfo attachedFileDir = new DirectoryInfo(attachedDirPath);
                            List<FileInfo> AttachedList = new List<FileInfo>();
                            foreach (FileInfo ImageFile in attachedFileDir.GetFiles())
                            {
                                Console.WriteLine(ImageFile.FullName);
                                AttachedList.Add(ImageFile);
                            }
                            */
                            content.Images = NotionReader.GetImageList(content.Article, attachedDirPath);
                        }
                        content.Images = client.UploadImages(content.Images);
                        if(content.CategoryName == "")
                        {
                            content.CategoryId = 0;
                        }
                        else
                        {
                            content.CategoryId = client.FindCategory(content.CategoryName);
                        }
                        content = Converter.ChangeHtml(content);

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
            
            FileWatcher watcher = new FileWatcher(setting.HtmlPath);
            watcher.InitWatcher(EventHandler);
        }
    }
}