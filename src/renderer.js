const {
    remote: { dialog, app },
    clipboard
} = require("electron");
const fs = require("fs");
const path = require("path");
const { searchFile, readFile } = require("./readFile.js");
const { convertToPost } = require("./coverter.js");

const homeBtn = document.getElementById("homeBtn");
const searchFileBtn = document.getElementById("uploadZipBtn");
const copyHtmlBtn = document.getElementById("copyHtmlBtn");
const downloadHtmlBtn = document.getElementById("downloadHtmlBtn");
const uploadToTistoryBtn = document.getElementById("uploadToTistoryBtn");

const changePage = pageNum => {
    const page = "page" + pageNum;
    const prevOpenPage = document.querySelectorAll(".open");
    const openPage = document.querySelectorAll("." + page);
    prevOpenPage.forEach(p => {
        p.classList.remove("open");
        p.classList.add("close");
    });
    openPage.forEach(p => {
        p.classList.remove("close");
        p.classList.add("open");
    });
};
const animate = () => {
    changePage(2);
    setTimeout(() => {
        changePage(3);
    }, 1500);
};

let notionPage;

homeBtn.addEventListener("click", () => {
    notionPage = null;
    changePage(1);
});

searchFileBtn.addEventListener("click", async () => {
    const filePath = dialog.showOpenDialogSync({
        properties: ["openFile"],
        filters: [{ name: "ZipFile", extensions: ["zip"] }]
    })[0];
    notionPage = convertToPost(readFile(filePath));
    animate();
    console.log(notionPage);
});

copyHtmlBtn.addEventListener("click", () => {
    clipboard.writeText(notionPage.content.outerHTML);
    console.log("clipboard copied!");
});

downloadHtmlBtn.addEventListener("click", async () => {
    let saveName = notionPage.Title;
    //특수문자 제거 정규식
    const reg = /[\{\}\[\]\/?.,;:|\)*~`!^\-_+<>@\#$%&\\\=\(\'\"]/gi;
    if (reg.test(saveName)) {
        saveName.replace(reg, "");
    }
    const savePath = await dialog.showSaveDialogSync({
        title: "Save converted HTML file",
        defaultPath: path.join(app.getPath("documents"), saveName + ".html"),
        filters: [{ name: "HTML", extensions: ["html"] }]
    });
    fs.writeFile(savePath, notionPage.content.outerHTML, "utf8", err => {
        if (err === null) {
            console.log("success");
        } else {
            console.log("fail");
        }
    });
});

uploadToTistoryBtn.addEventListener("click", async () => {
    await dialog.showMessageBox({
        title: "Error",
        message: "아직 준비중인 기능입니다.",
        detail: "Tistory 블로그 자동 업로드 기능은 v1.0 부터 지원할 예정입니다."
    });
});
