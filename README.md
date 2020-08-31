# Notion2Tistory

목차

-   [개요](#개요)
-   [사용 방법](#사용-방법)

## 개요

**Notion2Tistory**는 Notion에서 작성한 페이지를 Tistory 블로그 포스트로 업로드 해주는 앱입니다.

## 사용 방법

### 노션 페이지 속성

티스토리 게시물 업로드에 필요한 정보들을 노션 페이지 `속성(Property)` 에 입력합니다.

| 속성 이름   | 속성 유형 | 용도                                                                     |
| ----------- | --------- | ------------------------------------------------------------------------ |
| Visibility  | 선택      | 발행상태 (`Private`: 비공개 - 기본값, `Protected`: 보호, `Public`: 발행) |
| Category    |           | 카테고리 이름                                                            |
| PublishDate | 날짜      | 발행시간 (미래의 시간을 넣을 경우 예약글로 등록)                         |
| Tag         | 다중 선택 | 태그                                                                     |
| Comment     | 체크박스  | 댓글 허용 (![][v] - 기본값)                                              |
| Password    | 텍스트    | 보호글 비밀번호                                                          |

<br/>
속성이 존재하지 않을 시, 기본값으로 적용됩니다.
<br/>
<br/>

---

[<img height="32" widht="32" src="data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%2232%22%20height%3D%2232%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20viewBox%3D%220%200%20459%20459%22%3E%3Cpath%20d%3D%22M229.5%2C0C102.75%2C0%2C0%2C102.75%2C0%2C229.5S102.75%2C459%2C229.5%2C459%2C459%2C356.25%2C459%2C229.5%2C356.25%2C0%2C229.5%2C0ZM130.21%2C191.45a39.57%2C39.57%2C0%2C1%2C1%2C39.56-39.57A39.58%2C39.58%2C0%2C0%2C1%2C130.21%2C191.45ZM229.5%2C390a39.56%2C39.56%2C0%2C1%2C1%2C39.56-39.56A39.56%2C39.56%2C0%2C0%2C1%2C229.5%2C390Zm0-99.29a39.56%2C39.56%2C0%2C1%2C1%2C39.56-39.56A39.56%2C39.56%2C0%2C0%2C1%2C229.5%2C290.74Zm0-99.29a39.57%2C39.57%2C0%2C1%2C1%2C39.56-39.57A39.57%2C39.57%2C0%2C0%2C1%2C229.5%2C191.45Zm99.29%2C0a39.57%2C39.57%2C0%2C1%2C1%2C39.57-39.57A39.57%2C39.57%2C0%2C0%2C1%2C328.79%2C191.45Z%22%2F%3E%3C%2Fsvg%3E" />](https://boltlessengineer.tistory.com/category/%EA%B0%9C%EB%B0%9C%20%EC%9D%BC%EC%A7%80/Notion%20to%20Tistory "티스토리 블로그")
[<img height="32" width="32" src="https://unpkg.com/simple-icons@v3/icons/github.svg" />](https://github.com/boltlessengineer "GitHub 프로필")

[v]: data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%2216%22%20height%3D%2216%22%20viewBox%3D%220%200%2016%2016%22%20fill%3D%22none%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%3E%0A%3Crect%20width%3D%2216%22%20height%3D%2216%22%20fill%3D%22%2358A9D7%22%2F%3E%0A%3Cpath%20d%3D%22M6.71429%2012.2852L14%204.9995L12.7143%203.71436L6.71429%209.71378L3.28571%206.2831L2%207.57092L6.71429%2012.2852Z%22%20fill%3D%22white%22%2F%3E%0A%3C%2Fsvg%3E
[o]: data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%2216%22%20height%3D%2216%22%20viewBox%3D%220%200%2016%2016%22%20fill%3D%22none%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%3E%0A%3Crect%20x%3D%220.75%22%20y%3D%220.75%22%20width%3D%2214.5%22%20height%3D%2214.5%22%20fill%3D%22white%22%20stroke%3D%22%2336352F%22%20stroke-width%3D%221.5%22%2F%3E%0A%3C%2Fsvg%3E
