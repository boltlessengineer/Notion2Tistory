using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Notion2TistoryConsole
{
    class FileWatcher
    {
        public static string TargetPath;
        private string targetFile = "";

        public FileWatcher(string path)
        {
            TargetPath = path;
        }
        
        public void InitWatcher(Action<string> EventHandler)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = TargetPath;

            watcher.NotifyFilter = NotifyFilters.FileName;

            watcher.Filter = "*.zip";

            watcher.Created += new FileSystemEventHandler((object source, FileSystemEventArgs ev) => {
                if(targetFile == ev.Name)//파일 생성 감지 이벤트 중복 방지
                {
                    targetFile = "";
                    Console.WriteLine(ev.FullPath);
                    string tmpPath = TargetPath + @"tmp\";
                    ZipFile.ExtractToDirectory(ev.FullPath, tmpPath);
                    EventHandler(tmpPath);
                    File.Delete(ev.FullPath);
                }
                else
                {
                    targetFile = ev.Name;
                }
            });

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press 'q' to quit the sample.");
            while (Console.ReadKey().Key != ConsoleKey.Q) ;
        }
    }
}
