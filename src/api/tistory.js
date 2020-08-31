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

const findCategory = async (usr, categoryName) => {
    const categoryId = "";
    const query = {
        access_token: usr.accessToken,
        output: "json",
        blogName: usr.blogName
    };
    console.log(query);
    const myRequest = () => {
        return new Promise((res, rej) => {
            request(
                {
                    uri: "https://www.tistory.com/apis/category/list",
                    method: "GET",
                    qs: query
                },
                (err, response, body) => {
                    console.log(err);
                    console.log(response);
                    console.log(body);
                    const resBody = JSON.parse(body).tistory;
                    if (resBody.status != 200) {
                        rej(
                            `Error [${resBody.status}] : ${resBody.error_message}`
                        );
                    } else {
                        res(resBody.item.categories);
                    }
                }
            );
        });
    };
    const categoryList = await myRequest();
    console.log(categoryList);
    if (categoryList) {
        const foundList = categoryList.filter(
            category => category.name === categoryName
        );
        if (foundList.length > 0) {
            return foundList[0].id.toString();
        } else {
            return "";
        }
    }
};

const uploadData = (usr, data) => {
    // data : { value: buffer, options: { filename, } }
    console.log(data);
    const requestUri = "https://www.tistory.com/apis/post/attach";
    console.log(usr);
    const formData = {
        access_token: usr.accessToken,
        output: "json",
        blogName: usr.blogName,
        uploadedfile: data
    };
    return new Promise((resolve, reject) => {
        request.post(
            { url: requestUri, formData: formData },
            (err, res, body) => {
                const resBody = JSON.parse(body).tistory;
                if (resBody.status != 200) {
                    console.log(
                        `Error [${resBody.status}] : ${resBody.error_message}`
                    );
                    reject(resBody);
                } else {
                    console.log(resBody);
                    resolve(resBody);
                }
            }
        );
    });
};

const uploadImage = async (usr, data) => {
    console.log(data);
    const resBody = await uploadData(usr, data);
    const url = resBody.url;
    const replacer = getImageReplacer(url);
    return replacer;
};

const getImageReplacer = url => {
    const fileId = url.slice(url.lastIndexOf("/") + 1, url.length - 4);
    const newUrl = `https://t1.daumcdn.net/cfile/tistory/${fileId}?original`;
    const replacer = `[##_Image|t/cfile@${fileId}|alignCenter|data-origin-width="0" data-origin-height="0" data-ke-mobilestyle="widthContent"|||_##]`;

    return { replacer, url: newUrl };
};

const uploadPost = async (usr, notionPage) => {
    const categoryId = await findCategory(usr, notionPage.Category);
    console.log(categoryId);
    console.log("start");
    console.log(`Publish Date : ${notionPage.PublishDate}`);
    const publishTimestamp =
        Date.parse(notionPage.PublishDate.substring(1)) / 1000;
    console.log(publishTimestamp);
    const vis = notionPage.Visibility.toLowerCase();
    let visibility = "0"; //private
    switch (vis) {
        case "protected":
            visibility = "1";
            break;
        case "public":
            visibility = "3";
            break;
    }

    const form = {
        access_token: usr.accessToken,
        output: "json",
        blogName: usr.blogName,
        title: notionPage.Title,
        content: notionPage.content.outerHTML,
        visibility: visibility,
        category: categoryId,
        published: publishTimestamp.toString(),
        tag: notionPage.Tag.join(","),
        acceptComment: notionPage.Comment ? "1" : "0",
        password: notionPage.Password
    };
    console.log(form);

    return new Promise((resolve, reject) => {
        request(
            {
                uri: "https://www.tistory.com/apis/post/write",
                method: "POST",
                form: form
            },
            (err, res, body) => {
                const resBody = JSON.parse(body).tistory;
                if (resBody.status != 200) {
                    const error = `Error [${resBody.status}] : ${resBody.error_message}`;
                    console.log(error);
                    reject(error);
                } else {
                    console.log(resBody);

                    const postUrl = resBody.url;
                    resolve(postUrl);
                }
            }
        );
    });
};

module.exports = {
    getAccessToken,
    uploadPost,
    uploadData,
    uploadImage
};
