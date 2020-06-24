using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Notion2TistoryConsole
{
    class Converter
    {
        static string comment = "<p>\n</p><p class=\"block-color-gray\">Uploaded by Notion2Tistory v1.0</p>";
        
        public static Content ChangeHtml(Content content)
        {
            string article = content.content;

            // <body> 태그 안쪽만 남김 (<article> 태그)
            //article = SubByString(content, "<body>", "</body>");
            // 헤더 삭제
            //article = article.Split("<header>")[0] + article.Split("</header>")[1];
            
            // Notion_P CSS 적용
            article = article.Replace("page sans\">", "Notion_P page sans\">");

            // 토글 전부 접기
            article = article.Replace("class=\"toggle\"><li><details open=\"\"><summary>", "class=\"toggle\"><li><details><summary>");

            // Embed 형식으로 변환
            article = MakeEmbed(article);

            // 끝에 주석 추가
            article = article.Replace("</div></article>", comment + "</div></article>");

            // 이미지 replacer 적용
            article = ReplaceImages(article, content.images);

            content.SetContent(article);

            return content;
        }

        private static string MakeEmbed(string content)
        {
            string url;
            string[] aTag = new string[] {
                "<div class=\"source\"><a href=\"",
                "\">",
                "</a></div></figure>"
            };
            string[] emebedaTag = new string[] {
                "<div class=\"embed_container\"><a href=\"",
                "\"><iframe src=\"",
                "\" class=\"embed_inner\"></iframe>"
            };

            int mark1, mark2, mark3;
            mark1 = 0;

            while (content.Substring(mark1).Contains(aTag[0]))
            {
                mark1 += content.Substring(mark1).IndexOf(aTag[0]) + 29;
                mark2 = content.Substring(mark1).IndexOf(aTag[1]);
                mark3 = mark1 + content.Substring(mark1).IndexOf(aTag[2]);
                url = content.Substring(mark1, mark2);
                Console.WriteLine("<this is url>");
                Console.WriteLine(url);
                url = GetEmbedUrl(url);
                Console.WriteLine("<this is converted url>");
                Console.WriteLine(url);
                content = content.Substring(0, mark1 - 29) + emebedaTag[0] + url + emebedaTag[1]
                    + url + emebedaTag[2] + content.Substring(mark3);
            }
            return content;
        }
        private static string GetEmbedUrl(string url)
        {
            string website;

            website = SubByString(url, "://", "#");
            website = SubByString(website , "#", "/");

            if (website == "www.youtube.com")
            {
                url = url.Replace("watch?v=", "embed/");
            }
            else if (website == "codepen.io")
            {
                url = url.Replace("/pen/", "/embed/") + "?theme-id=dark&default-tab=html,result";
            }
            else if (website == "whimsical.com")
            {
                url = url.Replace(".com/", ".com/embed/");
            }
            else if (website == "www.figma.com")
            {
                byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(url);
                string encodedString = System.Web.HttpUtility.UrlEncode(byteDataParams, 0, byteDataParams.Length);
                url = "https://www.figma.com/embed?embed_host=share&url=" + encodedString;
            }

            return url;
        }

        public static string ReplaceImages(string content, List<AttachedImage> imageList)
        {
            foreach(AttachedImage image in imageList)
            {
                Console.WriteLine(image.originalPath);
                Console.WriteLine(image.originalTag);
                Console.WriteLine(content.Contains(image.originalTag) ? "t" : "f");
                content = content.Replace(image.originalTag, image.replacer);
            }
            return content;
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
