const fs = require("fs");
const path = require("path");
const {
    remote: { dialog, app },
    clipboard
} = require("electron");
const tistory = require("../api/tistory.js");

const handleDownload = async notionPage => {
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
};

const handleCopy = async notionPage => {
    clipboard.writeText(notionPage.content.outerHTML);
    console.log("clipboard copied!");
    await dialog.showMessageBox({
        title: "HTML Copied!",
        message: "COPIED!"
    });
};

const handleUpload = async notionPage => {
    if (!tistory.user) {
        await tistory.getUser();
    }
    console.log("made user");
    await tistory.send(notionPage);
};

module.exports = {
    handleDownload,
    handleCopy,
    handleUpload
};
