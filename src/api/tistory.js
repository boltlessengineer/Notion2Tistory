const request = require("request");
const { BrowserWindow } = require("electron").remote;

const TISTORY_OAUTH =
    "https://www.tistory.com/oauth/authorize?client_id=ff97cbe9c5811dbf23fc9f9622f3d675&redirect_uri=http://boltlessengineer.tistory.com&response_type=token";

//should use this. or use `${}` way.
const urlparams = {
    client_id: "ff97cbe9c5811dbf23fc9f9622f3d675",
    redirect_uri: "https://boltlessengineer.tistory.com",
    response_type: "token"
};
//

const getAccessToken = () => {
    // [ToDo]
    // 나중에 로그인 창(브라우저) 띄우는건 index.js로 옮기거나
    // window 창만 관리하는 js파일을 따로 만들것!
    return new Promise((res, rej) => {
        const authWin = new BrowserWindow({
            width: 1000,
            height: 800,
            show: false,
            "node-integration": false
        });

        const handleNavigation = url => {
            const raw_token = /\#access_token=([^&]*)/.exec(url);
            const accessToken =
                raw_token && raw_token.length > 1 ? raw_token[1] : null;

            if (accessToken) {
                console.log(`access_token : ${accessToken}`);
                authWin.close();
                res(accessToken);
            } else {
                authWin.show();
            }
        };
        authWin.loadURL(TISTORY_OAUTH);

        authWin.webContents.on("did-navigate", (event, url) => {
            console.log("did-navigate");
            handleNavigation(url);
        });

        authWin.webContents.on(
            "did-get-redirect-request",
            (event, oldUrl, newUrl) => {
                console.log("did-get-redirect-request");
                handleNavigation(newUrl);
            }
        );
    });
};

const findCategory = async categoryName => {
    request(
        {
            uri: "https://www.tistory.com/apis/category/list",
            method: "GET",
            qs: {
                accessToken: await getAccessToken(),
                output: "json",
                blogName: "boltless-sub"
            }
        },
        (err, res, body) => {
            console.log(err);
            console.log(res);
            console.log(body);
        }
    );
};

const uploadPost = async notionPage => {
    const accessToken = await getAccessToken();
    console.log("hm");
    //const categoryId = await findCategory(notionPage.Category);
    console.log("start");
    request(
        {
            uri: "https://www.tistory.com/apis/post/write",
            method: "POST",
            form: {
                access_token: accessToken,
                output: "json",
                blogName: "boltlessengineer",
                title: notionPage.Title,
                content: notionPage.content.outerHTML,
                visibility: "",
                tag: notionPage.Tag.join(","),
                acceptComment: notionPage.Comment ? "1" : "0",
                password: notionPage.Password
            }
        },
        (err, res, body) => {
            const resBody = JSON.parse(body).tistory;
            console.log(resBody.status);
            if (resBody.status != 200) {
                console.log(
                    `Error [${resBody.status}] : ${resBody.error_message}`
                );
            } else {
                console.log(resBody);
            }
        }
    );
};

module.exports = {
    send: uploadPost
};
