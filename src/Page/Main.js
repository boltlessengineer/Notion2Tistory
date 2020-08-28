const { remote : {dialog} } = require("electron");
const fs = require("fs");
const { searchFile, readFile } = require("../readFile.js");
const { convertToPost } = require("../coverter.js");

const handlefileSelect = () => {
    const filePath = dialog.showOpenDialogSync({
        properties: ["openFile"],
        filters: [{ name: "ZipFile", extensions: ["zip"] }]
    })[0];
    const convertedPage = convertToPost(readFile(filePath));
    return convertedPage
};

module.exports = {
    handlefileSelect
};
