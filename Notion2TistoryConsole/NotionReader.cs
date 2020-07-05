using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Notion2TistoryConsole
{
    class NotionReader
    {
        public static Content DefaultContent { get; set; }
        public static Content Read(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            string header = ExtractHeader(fileContent);

            Dictionary<string, string> Table = ReadTable(header);

            Content content = DefaultContent;

            content.Title = GetTitle(header);
            content.Article = GetBody(fileContent);
            content.Visibility = GetVisibilityType(Table);
            content.CategoryName = GetCategoryName(Table);
            content.PublishDate = GetUtcPublishDate(Table);
            content.Published = ConvertToTimestamp(content.PublishDate);
            content.Tags = GetTags(Table);
            content.AcceptComent = GetAcceptComment(Table);
            content.Password = GetPassword(Table);

            return content;
        }

        public static string ExtractHeader(string c)
        {
            string header = c.Split("<header>")[1].Split("</header>")[0];
            return header;
        }

        private static string GetTitle(string header)
        {
            string title = DefaultContent.Title;
            try
            {
                title = header.Split("<h1 class=\"page-title\">")[1].Split("</h1>")[0];
                Console.WriteLine("Page title : {0}", title);
            }
            catch
            {
                Console.WriteLine("Error : Can't find page title");
            }
            return title;
        }

        private static string GetBody(string content)
        {
            // <body> 태그 안쪽만 남김 (<article> 태그)
            string article = SubByString(content, "<body>", "</body>");

            // 헤더 삭제
            article = article.Split("<header>")[0] + article.Split("</header>")[1];
            return article;
        }

        private static int GetVisibilityType(Dictionary<string, string> table)
        {
            int type = DefaultContent.Visibility;
            try
            {
                try
                {
                    string rowValue = table["select" + "Visibility"].Split("\">")[1].Split("</span>")[0];
                    try
                    {
                        if (rowValue == "Private")
                        {
                            type = 0;
                        }
                        else if (rowValue == "Protect")
                        {
                            type = 1;
                        }
                        else if (rowValue == "Public")
                        {
                            type = 2;
                        }
                        else
                        {
                            Exception e = new Exception();
                            throw e;
                        }
                        Console.WriteLine("Visbility type : {0}", rowValue);
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read Visbility type");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Visbility row");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract CommentAccept");
            }
            return type;
        }

        private static string GetCategoryName(Dictionary<string, string> table)
        {
            string category = "None";
            try
            {
                try
                {
                    string rowValue = table["relation" + "Category"];
                    try
                    {
                        // 시도
                        category = rowValue.Substring(0, rowValue.LastIndexOf("</"));
                        category = category.Substring(category.LastIndexOf(">") + 1);
                        Console.WriteLine("Category Name : {0}", category);
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read Category Name");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Category row");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract Category Name");
            }
            return category;
        }

        private static DateTime GetUtcPublishDate(Dictionary<string, string> table)
        {
            DateTime datetime = DateTime.UtcNow;
            try
            {
                try
                {
                    string rowValue = table["date" + "Publish Date"];
                    try
                    {
                        string time = SubByString(rowValue, "<time>", "</time>");
                        datetime = DateTime.Parse(time.Trim('@'));
                        datetime = datetime.ToUniversalTime();
                        Console.WriteLine(datetime);
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read Publish Date");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Publish Date row");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract Publish Date");
            }
            return datetime;
        }

        private static long ConvertToTimestamp(DateTime dateTime)
        {
            var timeSpan = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0));

            long timestamp = (long)timeSpan.TotalSeconds;

            Console.WriteLine(timestamp);

            return timestamp;
        }

        private static List<string> GetTags(Dictionary<string, string> table)
        {
            List<string> tags = DefaultContent.Tags;
            try
            {
                try
                {
                    string rowValue = table["multi_select" + "Tags"];
                    try
                    {
                        tags = new List<string>(rowValue.Split("</span>"));
                        tags.Remove("");
                        tags = tags.Select(tag => tag.Split("\">")[1]).ToList();
                        Console.WriteLine("Tags : {0}", string.Join(",", tags));
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read tags");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Tags row");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract tags");
            }
            return tags;
        }

        private static bool GetAcceptComment(Dictionary<string, string> table)
        {
            bool accept = DefaultContent.AcceptComent;
            try
            {
                try
                {
                    string rowValue = table["checkbox" + "Comment"];
                    try
                    {
                        if (rowValue == "<div class=\"checkbox checkbox-on\"></div>")
                        {
                            accept = true;
                        }
                        else if (rowValue == "<div class=\"checkbox checkbox-off\"></div>")
                        {
                            accept = false;
                        }
                        else
                        {
                            Exception e = new Exception();
                            throw e;
                        }
                        Console.WriteLine("Accept Comment : {0}", accept.ToString());
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read CommentAccept");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Comment row");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract CommentAccept");
            }
            return accept;
        }

        private static string GetPassword(Dictionary<string, string> table)
        {
            string password = DefaultContent.Password;
            try
            {
                try
                {
                    string rowValue = table["text" + "Password"];
                    try
                    {
                        password = rowValue;
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read Password");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Password row");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract Password");
            }
            return password;
        }

        private static Dictionary<string, string> ReadTable(string table)
        {
            Dictionary<string, string> Table = new Dictionary<string, string>();
            // type : number, relation, select, multiselect, text, checkbox...
            static string DeleteIcon(string s)
            {
                if (s.Contains("<span class=\"icon property-icon\">"))
                {
                    string[] icon = { "<span class=\"icon property-icon\">", "</svg></span>" };
                    int iconStart = s.IndexOf(icon[0]);
                    int iconEnd = s.IndexOf(icon[1]) + icon[1].Length;
                    s = $"{s.Substring(0, iconStart)}{s[iconEnd..]}";
                }
                return s;
            }
            table = SubByString(table, "<tbody>", "</tbody>");
            List<string> rows = table.Split("</tr>").ToList();
            rows.Remove("");
            rows.Select(row => DeleteIcon(row));
            foreach (string row in rows)
            {
                string r = DeleteIcon(row);
                string type, name, content;
                type = SubByString(r, "<tr class=\"property-row property-row-", "\">");
                name = SubByString(r, "<th>", "</th>");
                content = SubByString(r, "<td>", "</td>");
                Table.Add(type + name, content);
            }
            /*
            var lines = Table.Select(kvp => kvp.Key[0] + " & " + kvp.Key[1] + " : " + kvp.Value.ToString());
            Console.WriteLine(string.Join(Environment.NewLine, lines));
            */
            return Table;
        }

        public static List<AttachedImage> GetImageList(string content, string folderPath)
        {
            List<AttachedImage> imageList = new List<AttachedImage>();
            int mark1 = 0;
            string folderName = Uri.EscapeDataString(folderPath.Split(@"\")[^1]);
            string tmpPath = String.Join(@"\", folderPath.Split(@"\").SkipLast(1));
            Console.WriteLine(">{0}<", folderName);
            Console.WriteLine(">{0}<", folderPath);
            while (content.Substring(mark1).Contains(" class=\"image\"><a href=\"" + folderName))
            {
                Console.WriteLine(mark1);
                AttachedImage image = new AttachedImage();
                mark1 += content.Substring(mark1).IndexOf(" class=\"image\"><a href=\"") + 24;
                int tagStart = content.Substring(0, mark1 - 24).LastIndexOf("<figure id=\"");
                int tagLength = mark1 + content.Substring(mark1).IndexOf("</figure>") + 9 - tagStart;
                string tag = content.Substring(tagStart, tagLength);
                string path = SubByString(tag, "<a href=\"", "\"><img");
                string style = SubByString(tag, "<img style=\"", "\" src=\"");

                image.originalTag = tag; // <figure><a><img></img></a></figure>
                image.originalPath = tmpPath + @"\" + Uri.UnescapeDataString(path);
                image.originalStyle = style;

                if (tag.Contains("<figcaption>"))
                {
                    string caption = SubByString(tag, "<figcaption>", "</figcaption>");
                    image.originalCaption = caption;
                }
                imageList.Add(image);
                break;
            }
            return imageList;
        }

        // 자주 사용하는 함수
        private static string SubByString(string s, string from, string to)
        {
            if (s.Contains(from))
            {
                s = s.Substring(s.IndexOf(from) + from.Length);
            }
            if (s.Contains(to))
            {
                s = s.Substring(0, s.IndexOf(to));
            }
            return s;
        }
    }
}
