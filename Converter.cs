using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notion2TistoryConsole
{
    class Converter
    {
        public Content GetContent(string c)
        {
            string header = ExtractHeader(c);
            //Console.WriteLine(header);

            string title = GetTitle(header);

            Content content = new Content(title);

            Dictionary<string[], string> Table = ReadTable(header);

            content.SetVisbility(GetVisibilityType(header));
            content.SetCategory(GetCategoryId(header));
            content.SetTags(GetTags(header));
            content.SetCommentAccept(GetAcceptComment(header));
            content.SetPassword(GetPassword(header));

            return content;
        }

        public string ExtractHeader(string c)
        {
            string header = c.Split("<header>")[1].Split("</header>")[0];
            return header;
        }

        private string GetTitle(string header)
        {
            string title = "Notion Page";
            try
            {
                title = header.Split("<h1 class=\"page-title\">")[1].Split()[0];
                Console.WriteLine("Page title : {0}", title);
            }
            catch
            {
                Console.WriteLine("Error : Can't find page title");
                Console.WriteLine("Default value is 'Notion Page'");
            }
            return title;
        }

        private int GetVisibilityType(string header)
        {
            int type = 0;
            try
            {
                // 시도
            }
            catch
            {
                Console.WriteLine("Error : Can't find Visibility row");
                Console.WriteLine("Default value is 'private'");
            }
            return type;
        }

        private int GetCategoryId(string header)
        {
            int category = 0;
            try
            {
                // 시도
            }
            catch
            {
                Console.WriteLine("Error : Can't find Category-id row");
                Console.WriteLine("Default value is '0'");
            }
            return category;
        }

        private List<string> GetTags(string header)
        {
            List<string> tags = new List<string>();
            try
            {
                string row = header.Split("Tags</th><td>")[1].Split("</td>")[0];
                try
                {
                    tags = new List<string>(row.Split("</span>"));
                    tags.Remove("");
                    tags = tags.Select(tag => tag.Split("\">")[1]).ToList();
                    Console.WriteLine("Tags : {0}", string.Join(",", tags));
                }
                catch
                {
                    Console.WriteLine("Error : Can't extract Tags");
                }
            }
            catch
            {
                Console.WriteLine("Error : Can't find Tags row");
            }
            return tags;
        }

        private bool GetAcceptComment(string header)
        {
            bool accept = false;
            try
            {
                // 시도
            }
            catch
            {
                Console.WriteLine("Error : Can't find Comment row");
                Console.WriteLine("Default value is 'false'");
            }
            return accept;
        }

        private string GetPassword(string header)
        {
            string password = "";
            try
            {
                // 시도
            }
            catch
            {
                Console.WriteLine("Error : Can't find Password row");
                Console.WriteLine("Default value is ''");
            }
            return password;
        }

        private Dictionary<string[], string> ReadTable(string table)
        {
            Dictionary<string[], string> Table = new Dictionary<string[], string>();
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
            static string SubByString(string s, string from, string to)
            {
                if (s.Contains(from) && s.Contains(to))
                {
                    s = s.Substring(s.IndexOf(from) + from.Length);
                    s = s.Substring(0, s.IndexOf(to));
                }
                return s;
            }
            table = SubByString(table, "<tbody>", "</tbody>");
            List<string> rows = table.Split("</tr>").ToList();
            rows.Remove("");
            rows.Select(row => DeleteIcon(row));
            foreach(string row in rows)
            {
                string r = DeleteIcon(row);
                string type, name, content;
                type = SubByString(r, "<tr class=\"property-row property-row-", "\">");
                name = SubByString(r, "<th>", "</th>");
                Console.WriteLine("type : {0} | name : {1}", type, name);
                content = SubByString(r, "<td>", "</td>");
                Table.Add(new string[]{ type, name }, content);
            }
            var lines = Table.Select(kvp => kvp.Key[0] + " & " + kvp.Key[1] + " : " + kvp.Value.ToString());
            Console.WriteLine(string.Join(Environment.NewLine, lines));
            return Table;
        }
    }
}
