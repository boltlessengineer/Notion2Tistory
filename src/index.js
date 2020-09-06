const { app, BrowserWindow, dialog, Menu, shell } = require("electron");
const path = require("path");
const { checkUpdate } = require("./myUpdateChecker");

// Handle creating/removing shortcuts on Windows when installing/uninstalling.
if (require("electron-squirrel-startup")) {
    // eslint-disable-line global-require
    app.quit();
}

let mainWindow;

const createWindow = () => {
    // Create the browser window.
    mainWindow = new BrowserWindow({
        width: 460,
        height: 320,
        webPreferences: {
            nodeIntegration: true,
        },
        icon: path.join(__dirname, "../assets/icons/png/icon.png"),
    });

    const menu = Menu.buildFromTemplate([
        {
            label: "도움말",
            submenu: [
                {
                    label: "웹사이트",
                    click: () => {
                        shell.openExternal(
                            "https://boltlessengineer.github.io/Notion2Tistory"
                        );
                    },
                },
                {
                    label: "사용법",
                    click: () => {
                        shell.openExternal(
                            /*"https://boltlessengineer.github.io/Notion2Tistory/how-to-use"*/
                            "https://www.notion.so/boltlessengineer/Notion2Tistory-0fca7a4a72fc4a2abfc02f58f4c63501"
                        );
                        // [ToDo]
                        // 이거 url도 github page로 관리. 그래야 나중에 사이트가 바껴도 문제가 없음
                    },
                },
            ],
        },
        {
            label: "",
            accelerator: "CmdOrCtrl+Shift+I",
            click: () => {
                mainWindow.webContents.openDevTools();
            },
        },
    ]);

    mainWindow.setMenu(menu);

    // and load the index.html of the app.
    mainWindow.loadFile(path.join(__dirname, "index.html"));

    // Open the DevTools.
    // mainWindow.webContents.openDevTools();
};

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on("ready", async () => {
    createWindow();
    const updateUrl = await checkUpdate();
    if (updateUrl) {
        const option = {
            type: "question",
            buttons: ["업데이트", "취소"],
            defaultId: 0,
            title: "electron-updater",
            message: "업데이트가 있습니다. 프로그램을 업데이트 하시겠습니까?",
        };
        const wouldUpdate = await dialog.showMessageBox(mainWindow, option);

        if (wouldUpdate.response === 0) {
            shell.openExternal(updateUrl);
            app.quit();
        }
    }
});

// Quit when all windows are closed, except on macOS. There, it's common
// for applications and their menu bar to stay active until the user quits
// explicitly with Cmd + Q.
app.on("window-all-closed", () => {
    if (process.platform !== "darwin") {
        app.quit();
    }
});

app.on("activate", () => {
    // On OS X it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (BrowserWindow.getAllWindows().length === 0) {
        createWindow();
    }
});

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and import them here.
