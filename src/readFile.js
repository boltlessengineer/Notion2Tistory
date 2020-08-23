const AdmZip = require("adm-zip");

const searchFile = () => {
    const promise = dialog.showOpenDialog({
        properties: ["openFile"],
        filters: [{ name: "ZipFile", extensions: ["zip"] }]
    });

    promise.then(readFile);
};

function readFile(filePath) {
    const zipfile = new AdmZip(filePath);
    const zipEntries = zipfile.getEntries();
    let content = "";
    console.log(filePath);
    zipEntries.forEach(zipEntry => {
        console.log(zipEntry.toString());
        if (zipEntry.entryName === zipEntry.name) {
            if (zipEntry.name.indexOf(".html") !== -1) {
                console.log(zipEntry.name + " is main page html file!");
                content = zipfile.readAsText(zipEntry, "utf8");
            }
        }
    });
    return content;
}

module.exports = {
    searchFile: searchFile,
    readFile: readFile
};
