using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Notion2TistoryConsole
{
    class NotionReader
    {
        public static Content Read(string filePath)
        {
            string fileContent = File.ReadAllText(filePath);
            string header = ExtractHeader(fileContent);

            string title = GetTitle(header);
            Dictionary<string, string> Table = ReadTable(header);

            Content content = new Content(title);
            content.SetContent(GetBody(fileContent));
            content.SetVisbility(GetVisibilityType(Table));
            content.SetCategory(GetCategoryId(Table));
            content.SetTags(GetTags(Table));
            content.SetCommentAccept(GetAcceptComment(Table));
            content.SetPassword(GetPassword(Table));
            //content.SetImages(GetImageList(content.content));

            return content;
        }

        public static string ExtractHeader(string c)
        {
            string header = c.Split("<header>")[1].Split("</header>")[0];
            return header;
        }

        private static string GetTitle(string header)
        {
            string title = "Notion Page";
            try
            {
                title = header.Split("<h1 class=\"page-title\">")[1].Split("</h1>")[0];
                Console.WriteLine("Page title : {0}", title);
            }
            catch
            {
                Console.WriteLine("Error : Can't find page title");
                Console.WriteLine("Default value is 'Notion Page'");
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
            int type = 0;
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
                        Console.WriteLine("Default value is 'false'");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Visbility row");
                    Console.WriteLine("Default value is 'false'");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract CommentAccept");
                Console.WriteLine("Default value is 'false'");
            }
            return type;
        }

        private static int GetCategoryId(Dictionary<string, string> table)
        {
            int category = 0;
            try
            {
                try
                {
                    string rowValue = table["relation" + "Category"];
                    try
                    {
                        // 시도
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read Category-id");
                        Console.WriteLine("Default value is '0'");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Category row");
                    Console.WriteLine("Default value is '0'");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract Category-id");
                Console.WriteLine("Default value is '0'");
            }
            return category;
        }

        private static List<string> GetTags(Dictionary<string, string> table)
        {
            List<string> tags = new List<string>();
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
            bool accept = false;
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
                        Console.WriteLine("Default value is 'false'");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Comment row");
                    Console.WriteLine("Default value is 'false'");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract CommentAccept");
                Console.WriteLine("Default value is 'false'");
            }
            return accept;
        }

        private static string GetPassword(Dictionary<string, string> table)
        {
            string password = "";
            try
            {
                try
                {
                    string rowValue = table["text" + "Password"];
                    try
                    {
                        // 시도
                    }
                    catch
                    {
                        Console.WriteLine("Error : Can't read Password");
                        Console.WriteLine("Default value is ''");
                    }
                }
                catch
                {
                    Console.WriteLine("Error : Can't find Password row");
                    Console.WriteLine("Default value is ''");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't extract Password");
                Console.WriteLine("Default value is ''");
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
                    s = s.Substring(0, iconStart) + s.Substring(iconEnd, s.Length - iconEnd);
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
                Console.WriteLine("type : {0} | name : {1}", type, name);
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
