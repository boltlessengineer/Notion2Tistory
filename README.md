# Notion2Tistory

목차

-   [개요](#개요)
-   [사용 방법](#사용-방법)
-   [더보기](#더보기)
-   [연락하기](#연락하기)

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
| Comment     | 체크박스  | 댓글 허용 (✅ - 기본값)                                                  |
| Password    | 텍스트    | 보호글 비밀번호                                                          |

<br/>
속성이 존재하지 않을 시, 기본값으로 적용됩니다.

### 티스토리 블로그 스타일 추가

티스토리 블로그 `설정` -> `꾸미기` -> `스킨 편집` 에 들어가서 `HTML 편집`의 `HTML` 란 `<head>` 아래에 다음 코드를 붙여넣습니다.

```html
<link
    rel="stylesheet"
    href="https://boltlessengineer.github.io/Notion2Tistory/assets/n2t_style.css"
/>
```

`적용` 버튼을 누르시고 나면 앞으로 Notion2Tistory로 업로드하는 모든 글에 Notion 스타일이 적용됩니다.

<br/>
<br/>

## 더보기

[<img height="32" width="32" src="https://raw.githubusercontent.com/iconic/open-iconic/master/svg/globe.svg" />](https://boltlessengineer.github.io/Notion2Tistory/ "공식 홈페이지")
[<img height="32" width="32" src="https://unpkg.com/simple-icons@v3/icons/notion.svg" />](https://www.notion.so/boltlessengineer/Notion2Tistory-f46185df1db14f8eb571d366b66c5e9c "Notion 페이지")
[<img height="32" widht="32" src="assets\images\svg\tistory_fill_black.svg" />](https://boltlessengineer.tistory.com/category/%EA%B0%9C%EB%B0%9C%20%EC%9D%BC%EC%A7%80/Notion%20to%20Tistory "티스토리 블로그")

<br/>
<br/>

## 연락하기

[<img height="32" width="32" src="https://unpkg.com/simple-icons@v3/icons/gmail.svg" />](mailto:boltlessengineer@gmail.com "메일 보내기")
[<img height="32" widht="32" src="assets\images\svg\tistory_fill_black.svg" />](https://boltlessengineer.tistory.com "티스토리 블로그")
[<img height="32" width="32" src="https://unpkg.com/simple-icons@v3/icons/github.svg" />](https://github.com/boltlessengineer "GitHub 프로필")
