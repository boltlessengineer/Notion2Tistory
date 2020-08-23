const {
    remote: { dialog },
    clipboard
} = require("electron");
const { searchFile, readFile } = require("./readFile.js");
const { convertToPost } = require("./coverter.js");

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

downloadHtmlBtn.addEventListener("click", () => {});

uploadToTistoryBtn.addEventListener("click", () => {});
