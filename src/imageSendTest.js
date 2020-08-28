const { Duplex } = require("stream");
const AdmZip = require("adm-zip");
const tistory = require("./api/tistory.js");
const fs = require("fs");

function bufferToStream(myBuuffer) {
    let tmp = new Duplex();
    tmp.push(myBuuffer);
    tmp.push(null);
    return tmp;
}

const test = async () => {
    const zipPath = "C:\\Users\\seong\\Desktop\\images.zip";
    const zipfile = new AdmZip(zipPath);
    const zipEntries = zipfile.getEntries();
    console.log(zipEntries);
    const zipdata = zipEntries[0].getData();
    console.log(zipdata);

    if (!tistory.user) {
        await tistory.getUser();
    }
    tistory.uploadData({
        value: zipdata,
        options: { filename: "78473532_p0_master1200.jpg" }
    });
    // works! 🎉🎉
};
