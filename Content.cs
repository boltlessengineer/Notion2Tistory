using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Notion2TistoryConsole
{
    class Content
    {
        private const string apiBaseUrl = "https://www.tistory.com/apis/";

        public string title; // 제목
        public string content; // 내용
        public int visibility; // 0: 비공개 - 기본값, 1: 보호, 3: 발행
        public int categoryId; // 카테고리 id
        // public string published; // 날짜 (TIMESTAMP 이며 미래의 시간을 넣을 경우 예약. 기본값: 현재시간)
        //public string slogan; // 문자 주소
        public List<string> tags; // 태그 (POST 요청은 ","로 구분)
        public bool acceptComent; // 댓글 허용 (POST 요청은 0, 1)
        public string password; // 보호글 비밀번호

        public Content(string t, string c = "empty page")
        {
            title = t;
            content = c;
            visibility = 0;
            categoryId = 0;
            //slogan = "";
            tags = new List<string>();
            acceptComent = true;
            password = "";
        }

        public void SetContent(string c)
        {
            content = c;
        }

        public void SetVisbility(int v = 0)
        {
            visibility = v;
        }
        public void SetCategory(int c = 0)
        {
            categoryId = c;
        }
        public void SetTags(List<string> tag)
        {
            tags = tag;
        }
        public void AddTag(string tag)
        {
            tags.Add(tag);
        }
        public void SetCommentAccept(bool comment = true)
        {
            acceptComent = comment;
        }
        public void SetPassword(string pw = "")
        {
            password = pw;
        }

        public Dictionary<string, string> GetPostDict(string token, string blogName)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("access_token", token);
            dict.Add("output", "json");
            dict.Add("blogName", blogName);
            dict.Add("title", title);
            dict.Add("content", content);
            dict.Add("visibility", visibility.ToString());
            dict.Add("categoryId", categoryId.ToString());
            // dict.Add("published", published);
            // dict.Add("slogan", slogan);
            dict.Add("tag", string.Join(",", tags));
            dict.Add("acceptComent", acceptComent ? "1" : "0" );
            dict.Add("tagpassword", password);
            return dict;
        }

        public async Task WritePost(TistoryAPI client)
        {
            string accessToken = client.AccessToken();
            string blogName = client.BlogName();
            var j = Task.Run(() => SendAPIPost("post/write", GetPostDict(accessToken, blogName)));
            JObject json = await j;
            Console.WriteLine(json);
            Console.WriteLine("Post Url : {0}", json["tistory"]["url"]);
        }

        public static async Task<JObject> SendAPIPost(string postUrl, IEnumerable<KeyValuePair<string, string>> contentDict)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            var param = new FormUrlEncodedContent(contentDict);
            Console.WriteLine(contentDict);
            var result = await client.PostAsync(postUrl, param);
            string responseString = await result.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseString);
            if (json["tistory"]["status"].ToString() == "200")
            {
                Console.WriteLine("Task Success!");
            }
            else
            {
                Console.WriteLine("ERROR : Server returned error | status: {0}", json["tistory"]["status"]);
                Console.WriteLine(json["tistory"]["error_message"]);
            }
            return json;
        }
    }
}
