using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notion2TistoryConsole
{
    class Content
    {
        public string title; // 제목
        public string content; // 내용
        public int visibility; // 0: 비공개 - 기본값, 1: 보호, 3: 발행
        public int categoryId; // 카테고리 id
        // public string published; // 날짜 (TIMESTAMP 이며 미래의 시간을 넣을 경우 예약. 기본값: 현재시간)
        //public string slogan; // 문자 주소
        public List<string> tags; // 태그 (POST 요청은 ","로 구분)
        public bool acceptComent; // 댓글 허용 (POST 요청은 0, 1)
        public string password; // 보호글 비밀번호

        public Content(string t, string c)
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

        public void SetVisbility(int v = 0)
        {
            visibility = v;
        }
        public void SetCategory(int c = 0)
        {
            categoryId = c;
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

        public Dictionary<string, string> GetPostDict(string token)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("title", title);
            dict.Add("content", content);
            dict.Add("visibility", visibility.ToString());
            dict.Add("categoryId", categoryId.ToString());
            // dict.Add("published", published);
            //dict.Add("slogan", slogan);
            dict.Add("tag", string.Join(",", tags));
            dict.Add("acceptComent", acceptComent ? "1" : "0" );
            dict.Add("tagpassword", password);
            return dict;
        }
    }
}
