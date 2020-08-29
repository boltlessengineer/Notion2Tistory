const AdmZip = require("adm-zip");
const tistory = require("./api/tistory.js");

const test = async () => {
    const zipPath = "C:\\Users\\seong\\Desktop\\images.zip";
    const zipfile = new AdmZip(zipPath);

    const zipEntries = zipfile.getEntries();
    const zipdata = zipEntries[0].getData();

    if (!tistory.user) {
        await tistory.getUser();
    }
    zipEntries.map(zipEntry => {
        console.log(zipEntry);
        const fileName = zipEntry.name;
        const fileBuffer = zipEntry.getData();
        console.log(fileBuffer);
        tistory.uploadImage({
            value: fileBuffer,
            options: { filename: fileName }
        });
    });
    // works! 🎉🎉
    // tistory.js 에서 uploadImage() 함수 수정해서 https t1.daumcdn.net url로 반환
};
