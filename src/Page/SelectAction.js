const fs = require("fs");
const path = require("path");
const {
    remote: { dialog, app },
    clipboard,
} = require("electron");
const tistory = require("../api/tistory.js");
const { replaceImage, maketoHTMLdocument } = require("../converter.js");

const handleDownload = async (notionPage) => {
    let saveName = notionPage.Title;
    //특수문자 제거 정규식
    const reg = /[\{\}\[\]\/?.,;:|\)*~`!^\-_+<>@\#$%&\\\=\(\'\"]/gi;
    if (reg.test(saveName)) {
        saveName.replace(reg, "");
    }
    const savePath = await dialog.showSaveDialogSync({
        title: "Save converted HTML file",
        defaultPath: path.join(app.getPath("documents"), saveName + ".html"),
        filters: [{ name: "HTML", extensions: ["html"] }],
    });

    const htmldoc = maketoHTMLdocument(notionPage.content.outerHTML);

    fs.writeFile(savePath, htmldoc, "utf8", (err) => {
        if (err === null) {
            console.log("success");
        } else {
            console.log("fail");
        }
    });
};

const handleCopy = async (notionPage) => {
    clipboard.writeText(notionPage.content.outerHTML);
    console.log("clipboard copied!");
    await dialog.showMessageBox({
        title: "HTML Copied!",
        message: "COPIED!",
    });
};

let tistoryClient = { accessToken: "", blogName: "" };

const isTistoryUser = () => {
    return tistoryClient.accessToken && tistoryClient.blogName;
};

const removeTistoryUser = () => {
	tistoryClient = { accessToken: "", blogName: "" };
}

const handleLogin = async () => {
    tistoryClient.accessToken = await tistory.getAccessToken();
};

const handleBlogName = (blogName) => {
    console.log(blogName);
    tistoryClient.blogName = blogName;
};

const handleUpload = async ({ notionPage, imageList }) => {
    const convertedPage = await replaceImage(
        tistoryClient,
        notionPage,
        imageList
    );
    const postUrl = await tistory.uploadPost(tistoryClient, notionPage);
    console.log(postUrl);
    console.log("upload done!");
    return postUrl;
};

module.exports = {
    handleDownload,
    handleCopy,
    handleUpload,
    handleLogin,
    handleBlogName,
    isTistoryUser,
};
