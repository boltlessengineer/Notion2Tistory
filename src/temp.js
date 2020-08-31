const { handlefileSelect } = require("./Page/Main.js");
const {
    handleDownload,
    handleCopy,
    handleUpload,
    handleLogin,
    handleBlogName,
    isTistoryUser
} = require("./Page/SelectAction.js");

const textContainer = document.getElementById("textContainer");
const commentContainer = document.getElementById("commentContainer");
const btnContainer = document.getElementById("btnContainer");

const btnBg = document.getElementById("btnBg");

const appear = (el, option = "Vertical") => {
    el.classList.add("appear" + option);
    el.addEventListener("animationend", () => {
        el.classList.remove("appear" + option);
    });
};
const disappear = (el, option = "Vertical") => {
    if (el) {
        el.classList.add("disappear" + option);
        el.addEventListener("animationend", () => {
            el.remove();
        });
        el.addEventListener("animationcancel", () => {
            el.remove();
        });
    }
};

const changeText = text => {
    const prevText = textContainer.querySelector(".pageText");
    const newText = document.createElement("span");
    newText.className = "pageText";
    newText.innerText = text;
    disappear(prevText);
    textContainer.appendChild(newText);
    appear(newText);
};

const changeComment = (comment, direction) => {
    const prevComment = commentContainer.querySelector(".comment");
    if (prevComment.innerHTML !== comment) {
        const newComment = document.createElement("span");
        newComment.className = "comment";
        newComment.innerText = comment;
        //disappear(prevComment, "Horizontal");
        //[ToDo]
        //👆 fix animation
        if (prevComment) {
            prevComment.remove();
        }
        commentContainer.appendChild(newComment);
        //appear(newComment, "Horizontal");
        //[ToDo]
        //👆 fix animation
    }
};

const changeBtnMenu = btnList => {
    const prevMenu = btnContainer.querySelector(".btnMenu");
    const newMenu = document.createElement("div");
    newMenu.className = "btnMenu";
    btnList.map(btn => newMenu.append(btn));
    // newMenu.innerHTML = btnList;
    //disappear(prevMenu, "BtnHorizontal");
    //[ToDo]
    //👆 fix animation
    if (prevMenu) {
        prevMenu.remove();
    }
    btnContainer.appendChild(newMenu);
    //appear(newMenu, "BtnHorizontal");
    //[ToDo]
    //👆 fix animation
    btnBg.style.marginLeft = "0px";
    btnBg.style.opacity = "0";
};

// const handleBtnHover = ({ target }) => {
//   const targetPos = target.offsetLeft;
//   const targetComment = target.getAttribute("comment");
//   btnBg.style.backgroundColor = target.getAttribute("theme");
//   btnBg.style.marginLeft = targetPos + "px";
//   btnBg.style.opacity = 1;
//   if (commentContainer.innerText !== targetComment) {
//     // changeComment(target.getAttribute("comment"));
//     // 👇 put animation here
//     commentContainer.querySelector("span").innerText = targetComment;
//   }
// };
// const BtnList = btnContainer.querySelectorAll(".button");
// BtnList.forEach((btn) => btn.addEventListener("mouseenter", handleBtnHover));

const createBtn = (text, comment, clickHandler) => {
    const btn = document.createElement("div");
    btn.className = "button";
    btn.innerHTML = "<span>" + text + "</span>";
    btn.addEventListener("click", clickHandler);
    var timeout;
    btn.addEventListener("mouseover", () => {
        timeout = setTimeout(() => {
            //console.log(comment);
            changeComment(comment);
        }, 400);
    });
    btn.addEventListener("mouseout", () => {
        clearTimeout(timeout);
    });
    return btn;
};

const createTextInput = (placeHolder, clickHandler) => {
    const textInput = document.createElement("div");
    const inputInner = document.createElement("div");
    const inputTag = document.createElement("input");
    const inputCheck = createBtn("확인", "확인", () => {
        clickHandler(inputTag.value);
    });
    textInput.className = "textInput";
    inputInner.className = "tInput";
    inputTag.setAttribute("type", "text");
    inputTag.setAttribute("placeholder", placeHolder);
    inputTag.setAttribute("autofocus", "");

    textInput.appendChild(inputInner);
    textInput.appendChild(inputCheck);
    inputInner.appendChild(inputTag);
    return textInput;
};

homeBtn.addEventListener("click", () => {
    MainPage();
});

const MainPage = () => {
    changeText("Select zip file");
    const fileSelectBtn = createBtn(
        "file",
        "노션에서 export한 zip 파일 선택",
        async () => {
            const convertedPage = await handlefileSelect();
            CheckConvertPage(convertedPage);
        }
    );
    changeBtnMenu([fileSelectBtn]);
};

const CheckConvertPage = convertedPage => {
    changeText(convertedPage.notionPage.Title);
    changeComment("이 페이지로 계속하시겠습니까?");
    const checkBtn = createBtn("yes", "이 페이지로 계속", () => {
        SAPage(convertedPage);
    });
    const cancelBtn = createBtn("cancel", "메인으로 돌아가기", () => {
        MainPage();
    });
    changeBtnMenu([checkBtn, cancelBtn]);
};

const SAPage = ({ notionPage, imageList }, comment = "") => {
    changeText("Select Action");
    changeComment(comment);
    const downloadBtn = createBtn(
        "download",
        "변환된 HTML 파일 다운로드",
        () => {
            handleDownload(notionPage);
        }
    );
    const copyBtn = createBtn("copy", "변환된 HTML 복사", () => {
        handleCopy(notionPage);
    });
    const uploadBtn = createBtn(
        "tistory",
        "티스토리에 포스트로 업로드",
        async () => {
            console.log(isTistoryUser());
            if (!isTistoryUser()) {
                await TistoryLoginPage();
            }
            const postUrl = handleUpload({ notionPage, imageList });
            console.log(postUrl);
            SAPage({ notionPage, imageList });
        }
    );
    changeBtnMenu([downloadBtn, copyBtn, uploadBtn]);
};

const TistoryLoginPage = () => {
    changeText("티스토리에 로그인해주세요");
    return new Promise((resolve, reject) => {
        const goBackBtn = createBtn("back", "뒤로가기", () => {
            reject("go back");
        });
        const loginBtn = createBtn("login", "로그인하기", async () => {
            await handleLogin();
            const blogName = await BlogNamePage();
            handleBlogName(blogName);
            resolve();
        });
        changeBtnMenu([goBackBtn, loginBtn]);
    });
};

const BlogNamePage = () => {
    changeText("블로그 이름을 입력해주세요");
    return new Promise((resolve, reject) => {
        const goBackBtn = createBtn("back", "뒤로가기", () => {
            reject("go back");
        });
        const txtInput = createTextInput("블로그명 입력", value => {
            resolve(value);
        });
        changeBtnMenu([goBackBtn, txtInput]);
    });
};

MainPage();
