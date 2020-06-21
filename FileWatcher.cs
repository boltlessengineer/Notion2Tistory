using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Notion2TistoryConsole
{
    class FileWatcher
    {
        public static string TargetPath = @"C:\Users\seong\OneDrive\Documents\Personal\Blog\_Notion_Export\";
        private string targetFile = "";
        public TistoryAPI apiClient;

        public FileWatcher(TistoryAPI api)
        {
            apiClient = api;
        }
        
        public void InitWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = TargetPath;

            watcher.NotifyFilter = NotifyFilters.FileName;

            watcher.Filter = "*.zip";

            watcher.Created += new FileSystemEventHandler(Created);

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press 'q' to quit the sample.");
            //while (Console.ReadKey().Key != ConsoleKey.Q) ;
        }

        private void Created(object source, FileSystemEventArgs ev)
        {
            if(targetFile == ev.Name)//파일 생성 감지 이벤트 중복 방지
            {
                targetFile = "";
                Console.WriteLine(ev.FullPath);
                string tmpPath = TargetPath + @"tmp\";
                ZipFile.ExtractToDirectory(ev.FullPath, tmpPath);
                DirectoryInfo tmpDir = new DirectoryInfo(tmpPath);
                foreach (FileInfo file in tmpDir.GetFiles())
                {
                    if (file.Extension.ToLower() == ".html")
                    {
                        Console.WriteLine("{0} is html file", file.Name);

                        string htmlContent = File.ReadAllText(file.FullName);
                        
                        Converter converter = new Converter();

                        Content content = converter.GetContent(htmlContent);

                        content.WritePost(apiClient).Wait();
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
            else
            {
                targetFile = ev.Name;
            }
        }
    }
}
