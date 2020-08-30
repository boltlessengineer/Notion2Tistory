const { handlefileSelect } = require("./Page/Main.js");
const {
    handleDownload,
    handleCopy,
    handleUpload
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
        disappear(prevComment, "Horizontal");
        commentContainer.appendChild(newComment);
        appear(newComment, "Horizontal");
    }
};

const changeBtnMenu = btnList => {
    const prevMenu = btnContainer.querySelector(".btnMenu");
    const newMenu = document.createElement("div");
    newMenu.className = "btnMenu";
    btnList.map(btn => newMenu.append(btn));
    // newMenu.innerHTML = btnList;
    disappear(prevMenu, "BtnHorizontal");
    btnContainer.appendChild(newMenu);
    appear(newMenu, "BtnHorizontal");
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
        }, 600);
    });
    btn.addEventListener("mouseout", () => {
        clearTimeout(timeout);
    });
    return btn;
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
            changeText("HTML convert done!");
            setTimeout(() => {
                CheckConvertPage(convertedPage);
            }, 1000);
        }
    );
    changeBtnMenu([fileSelectBtn]);
};

const CheckConvertPage = convertedPage => {
    console.log(convertedPage);
    changeText(convertedPage.notionPage.Title);
    const checkBtn = createBtn("yes", "해당 페이지를 변환합니다.", () => {
        SAPage(convertedPage);
    });
    const cancelBtn = createBtn("cancel", "메인으로 돌아가기", () => {
        MainPage();
    });
    changeBtnMenu([checkBtn, cancelBtn]);
};

const SAPage = ({ notionPage, imageList }) => {
    changeText("Select Action");
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
    const uploadBtn = createBtn("tistory", "티스토리에 포스트로 업로드", () => {
        handleUpload({ notionPage, imageList });
    });
    changeBtnMenu([downloadBtn, copyBtn, uploadBtn]);
};

MainPage();
