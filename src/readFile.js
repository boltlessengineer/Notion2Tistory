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
    let imageList = [];
    console.log(filePath);
    zipEntries.forEach(zipEntry => {
        console.log(zipEntry.toString());
        if (zipEntry.entryName === zipEntry.name) {
            if (zipEntry.name.indexOf(".html") !== -1) {
                console.log(zipEntry.name + " is main page html file!");
                content = zipfile.readAsText(zipEntry, "utf8");
            }
        }
        else {
            if (zipEntry.entryName.indexOf("/") !== -1) {
                const imageName = zipEntry.entryName;
                const imageBuffer = zipEntry.getData();
                const imageData = {value: imageBuffer, options: {filename: imageName}};
                imageList.push(imageData);
            }
        }
    });
    return { content, imageList };
}

module.exports = {
    searchFile: searchFile,
    readFile: readFile
};
