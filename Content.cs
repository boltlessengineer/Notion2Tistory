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

        public string title { get; set; } // 제목
        public string content { get; set;} // 내용
        public int visibility { get; set;} // 0: 비공개 - 기본값, 1: 보호, 3: 발행
        public int categoryId { get; set;} // 카테고리 id
        // public string published {get; set;} // 날짜 (TIMESTAMP 이며 미래의 시간을 넣을 경우 예약. 기본값: 현재시간)
        //public string slogan {get; set;} // 문자 주소
        public List<string> tags { get; set;} // 태그 (POST 요청은 ","로 구분)
        public bool acceptComent { get; set;} // 댓글 허용 (POST 요청은 0, 1)
        public string password { get; set;} // 보호글 비밀번호
        public List<AttachedFile> attachedFiles { get; set;}
        public List<AttachedImage> images { get; set;}

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
            attachedFiles = new List<AttachedFile>();
            images = new List<AttachedImage>();
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
        public void SetImages(List<AttachedImage> list)
        {
            images = list;
        }
    }
}
