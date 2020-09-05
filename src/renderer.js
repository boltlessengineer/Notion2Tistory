const {
    remote: { app },
} = require("electron");
const { handlefileSelect } = require("./Page/Main.js");
const {
    handleDownload,
    handleCopy,
    handleUpload,
    handleLogin,
    handleBlogName,
    isTistoryUser,
} = require("./Page/SelectAction.js");

const fileSelectBtn = document.getElementById("fileSelectBtn");

const cancelBtn = document.getElementById("cancelBtn");
const checkBtn = document.getElementById("checkBtn");

const goHomeBtn = document.getElementById("goHomeBtn");
const copyBtn = document.getElementById("copyBtn");
const downloadBtn = document.getElementById("downloadBtn");
const uploadBtn = document.getElementById("uploadBtn");

const loginBtn = document.getElementById("loginBtn");
const txtInput = document.getElementById("txtInput");
const inputBtn = document.getElementById("inputBtn");

const loginPage = document.querySelector(".container.loginPage");
const BNInputPage = document.querySelector(".container.BNInputPage");

const changePage = (pageName, header, comment) => {
    const prevPage = document.querySelector(".container.active");
    const newPage = document.querySelector(".container." + pageName);
    prevPage.classList.remove("active");
    newPage.classList.add("active");
    changeHeader(header);
    changeComment(comment);
};

const changeHeader = (text) => {
    const currPage = document.querySelector(".container.active");
    const textContainer = currPage.querySelector(".textContainer");
    const prevHeaders = Array.prototype.slice.call(textContainer.children);
    if (text) {
        prevHeaders.map((prevHeader) => {
            prevHeader.classList.add("hidden");
        });
        const newHeader = document.createElement("span");
        newHeader.className = "pageText";
        newHeader.innerHTML = text;
        textContainer.appendChild(newHeader);
    } else {
        const defaultText = textContainer.getAttribute("default");
        if (defaultText) {
            textContainer.innerHTML = `<span class="pageText">${defaultText}</span>`;
            prevHeaders.map((prevHeader) => prevHeader.remove);
        }
    }
};

const changeComment = (comment) => {
    const currPage = document.querySelector(".container.active");
    const commentContainer = currPage.querySelector(".commentContainer");
    const prevComments = Array.prototype.slice.call(commentContainer.children);
    if (comment) {
        prevComments.map((prevComment) => {
            prevComment.classList.add("hidden");
        });
        const newHeader = document.createElement("span");
        newHeader.className = "comment";
        newHeader.innerHTML = comment;
        commentContainer.appendChild(newHeader);
    } else {
        const defaultComment = commentContainer.getAttribute("default");
        if (defaultComment) {
            commentContainer.innerHTML = `<span class="comment">${defaultComment}</span>`;
            prevComments.map((prevComment) => prevComment.remove);
        }
    }
};

changeComment("Notion2Tistory v" + app.getVersion());

let convertedPage;

fileSelectBtn.addEventListener("click", async () => {
    convertedPage = await handlefileSelect();
    const pageTitle = convertedPage.notionPage.Title;
    changePage("checkPage", pageTitle);
});

// CheckConvertPage
cancelBtn.addEventListener("click", () => {
    convertedPage = null;
    changePage("mainPage");
});
checkBtn.addEventListener("click", () => {
    changePage("SAPage");
});

// SelectActionPage
goHomeBtn.addEventListener("click", () => {
    convertedPage = null;
    changePage("mainPage");
});
copyBtn.addEventListener("click", () => {
    handleCopy(convertedPage.notionPage);
});
downloadBtn.addEventListener("click", () => {
    handleDownload(convertedPage.notionPage);
});
uploadBtn.addEventListener("click", async () => {
    if (!isTistoryUser()) {
        changePage("loginPage");
    } else {
        changeComment("티스토리에 업로드 중...");
        const postUrl = await handleUpload(convertedPage);
        const blogLink = `<span class="aTag" onclick="openLink('${postUrl}')">업로드된 포스트 보러가기</span>`;
        changeComment(blogLink);
    }
});

// loginPage
const goBackBtn_login = loginPage.querySelector(".goBackBtn");
goBackBtn_login.addEventListener("click", () => {
    changePage("SAPage");
});
loginBtn.addEventListener("click", async () => {
    await handleLogin();
    changePage("BNInputPage");
});

// BlogNameInputPage
const goBackBtn_BNInput = BNInputPage.querySelector(".goBackBtn");
goBackBtn_BNInput.addEventListener("click", () => {
    changePage("loginPage");
});
inputBtn.addEventListener("click", () => {
    const value = txtInput.value;
    handleBlogName(value);
    changePage("SAPage");
});

const openLink = (url) => {
    require("electron").shell.openExternal(url);
};
