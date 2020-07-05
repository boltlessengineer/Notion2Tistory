using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Notion2TistoryConsole
{
    class Settings
    {
        public string Version { get; set; }
        public string HtmlPath { get; set; }
        public JObject UserAccount { get; set; }
        public JObject UserClient { get; set; }
        public Content DefaultContent { get; set; }

        public Settings(string path)
        {
            string json = File.ReadAllText(path);
            JObject jsonDoc = JObject.Parse(json);
            JObject user = jsonDoc["settings"]["user"] as JObject;
            JObject defaultPost = jsonDoc["settings"]["default"]["tistory"]["post"] as JObject;

            Version = jsonDoc["version"].ToString();
            HtmlPath = jsonDoc["settings"]["htmlPath"].ToString();

            UserAccount = user["account"] as JObject;
            UserClient = user["client"] as JObject;

            DefaultContent = new Content()
            {
                Title = defaultPost["title"].ToString(),
                Article = defaultPost["content"].ToString(),
                Visibility = defaultPost["visibility"].ToObject<int>(),
                CategoryName = defaultPost["category"].ToString(),
                PublishDate = DateTime.UtcNow,
                //slogan = "",
                Tags = defaultPost["tag"].ToObject<List<string>>(),
                AcceptComent = defaultPost["acceptComment"].ToObject<bool>(),
                Password = defaultPost["password"].ToString(),
                AttachedFiles = new List<AttachedFile>(),
                Images = new List<AttachedImage>()
            };

            NotionReader.DefaultContent = DefaultContent;
        }
    }
}
