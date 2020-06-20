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
        
        public void InitWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = TargetPath;

            watcher.NotifyFilter = NotifyFilters.FileName;

            watcher.Filter = "*.zip";

            watcher.Created += new FileSystemEventHandler(Created);

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.ReadKey().Key != ConsoleKey.Q) ;
        }

        private void Created(object source, FileSystemEventArgs e)
        {
            if(targetFile == e.Name)//파일 생성 감지 이벤트 중복 방지
            {
                targetFile = "";
                Console.WriteLine(e.FullPath);
                string extractPath = TargetPath + @"tmp\";
                ZipFile.ExtractToDirectory(e.FullPath, extractPath);
                DirectoryInfo tmpDir = new DirectoryInfo(extractPath);
                foreach (FileInfo file in tmpDir.GetFiles())
                {
                    if (file.Extension.ToLower() == ".html")
                    {
                        Console.WriteLine("{0} is html file", file.Name);
                        string htmlContent = File.ReadAllText(file.FullName);
                        Console.WriteLine(htmlContent);
                    }
                }
                // /tmp 경로에서 html 형식의 파일 찾아서 열고 string으로 저장해서 다른 객체로 전달
                // 마지막엔 tmp 폴더 삭제
                // 전체랑 조금조금씩 try catch 문으로 쌀것!
            }
            else
            {
                targetFile = e.Name;
            }
        }
    }
}
