using System;
using System.Collections.Generic;

namespace Notion2TistoryConsole
{
    class Content
    {
        public string Title { get; set; } // 제목
        public string Article { get; set; } // 내용
        public int Visibility { get; set; } // 0: 비공개 - 기본값, 1: 보호, 3: 발행
        public string CategoryName { get; set; } // 카데고리 이름
        public int CategoryId { get; set; } // 카테고리 id
        public DateTime PublishDate { get; set; } // 날짜 (DateTime 형식)
        public long Published { get; set; } // 날짜 (TIMESTAMP 이며 미래의 시간을 넣을 경우 예약. 기본값: 현재시간)
        //public string slogan { get; set; } // 문자 주소
        public List<string> Tags { get; set; } // 태그 (POST 요청은 ","로 구분)
        public bool AcceptComent { get; set; } // 댓글 허용 (POST 요청은 0, 1)
        public string Password { get; set; } // 보호글 비밀번호
        public List<AttachedFile> AttachedFiles { get; set; }
        public List<AttachedImage> Images { get; set; }
    }
}
