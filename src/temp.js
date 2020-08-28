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
    const newComment = document.createElement("span");
    newComment.className = "comment";
    newComment.innerText = comment;
    disappear(prevComment, "Horizontal");
    commentContainer.appendChild(newComment);
    appear(newComment, "Horizontal");
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

const MainPage = () => {
    changeText("Select zip file");
    const fileSelectBtn = document.createElement("div");
    fileSelectBtn.className = "button";
    fileSelectBtn.innerHTML = "<span>file</span>";
    fileSelectBtn.addEventListener("click", () => {
        handlefileSelect();
    });
    changeBtnMenu([fileSelectBtn]);
};

const SAPage = () => {
    changeText("Select Action");
    const downloadBtn = document.createElement("div");
    downloadBtn.className = "button";
    downloadBtn.innerHTML = "<span>Download</span>";
    downloadBtn.addEventListener("click", () => {});
    changeBtnMenu([downloadBtn]);
};

MainPage();
