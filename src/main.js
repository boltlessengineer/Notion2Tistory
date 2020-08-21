const { dialog } = require("electron").remote;
const AdmZip = require("adm-zip");
const notion = require("./coverter.js");

const searchFileBtn = document.querySelector("#selectNotionBtn");

function searchFile() {
    const promise = dialog.showOpenDialog({
        properties: ["openFile"],
        filters: [{ name: "ZipFile", extensions: ["zip"] }]
    });

    promise.then(readFile);
}

function readFile(result) {
    const filePath = result.filePaths[0];
    const zipfile = new AdmZip(filePath);
    const zipEntries = zipfile.getEntries();
    console.log(filePath);
    zipEntries.forEach(zipEntry => {
        console.log(zipEntry.toString());
        if (zipEntry.entryName === zipEntry.name) {
            if (zipEntry.name.indexOf(".html") !== -1) {
                console.log(zipEntry.name + " is html file!");
                zipfile.readAsTextAsync(zipEntry, notion.convertHtml, "utf8");
            }
        }
    });
}

searchFileBtn.addEventListener("click", searchFile);
