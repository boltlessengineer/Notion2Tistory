// custom popup html & js things here.


// 👇이거 나중에 위치 옮길것! DesignCourse 영상 참조
const { BrowserWindow } = require("electron");

const Popup = popfile => {
    const popWin = new BrowserWindow({
        width: 640,
        height: 420,
        alwaysOnTop: true,
        frame: false
    });
    popWin.on("close", () => {
        popWin = null;
    });
    popWin.loadFile(popfile);
    popWin.show();
};

module.exports = Popup;
