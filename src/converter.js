const { JSDOM } = require("jsdom");
const tistory = require("./api/tistory.js");

const N2T_STYLE = `https://boltlessengineer.github.io/Notion2Tistory/assets/n2t_style_v0.11.0.css`;

class NotionPage {
    static defaultPage = {
        Title: "Notion2Tistory Post",
        Visibility: "Private",
        Category: "",
        Slogan: "",
        Tag: [],
        Comment: true,
        Password: "",
    };

    constructor(tempArg) {
        const defaultPage = Object.assign({}, NotionPage.defaultPage);
        const arg = Object.assign(defaultPage, tempArg);
        Object.assign(this, arg);
    }

    /*
    title ---------> title={title}                   //제목
    content -------> content={content}               //내용
    visibility ----> visibility={visibility}         //발행상태 (0:비공개, 1:보호, 2:발행)
    category ------> category={category-id}          //카테고리 아이디
    publishDate ---> published={published}           //발행시간 (TIMESTAMP)
    slogan --------> slogan={slogan}                 //문자주소
    tag -----------> tag={tag}                       //태그 (','로 구분)
    acceptComment -> acceptComment={acceptComment}   //댓글허용 (0, 1-기본)
    password ------> password={password}             //보호글 비밀번호
    */
}

function readPage(html) {
    const dom = new JSDOM(html, { runscripts: "outside-only" });
    const { document: notiondoc } = dom.window;

    const article = notiondoc.querySelector("article");
    console.log(article);
    console.log(notiondoc);

    //끝에 주석 추가
    const comment = notiondoc.createElement("div");
    comment.classList.add("n2t_comment");
    comment.innerHTML =
        '<p>\n</p><p class="block-color-gray"><a href="https://boltlessengineer.tistory.com">Uploaded by Notion2Tistory v0.10</a></p>';
    //[ToDo]
    //👆 app.getVersion() 으로 버전 직접 가져와서 합치기
    article.querySelector("div.page-body").appendChild(comment);

    const header = notiondoc.querySelector("header");
    const title = header.querySelector(".page-title").textContent;
    const table = header.querySelector("table");

    let properties = [];

    if (table) {
        const tableList = Array.prototype.slice.call(
            table.querySelectorAll("tr")
        );

        properties = tableList.map((tr) => {
            const propName = tr.querySelector("th").textContent;
            const csplit = Array.prototype.slice
                .call(tr.classList)[1]
                .split("-");
            const propType = csplit[csplit.length - 1];
            let propVal;
            if (propType === "checkbox") {
                const checkbox = tr.querySelector("td .checkbox");
                const boxState = Array.prototype.slice.call(
                    checkbox.classList
                )[1];
                if (boxState === "checkbox-on") {
                    propVal = true;
                } else {
                    propVal = false;
                }
            } else if (propType === "multi_select") {
                const spans = tr.querySelectorAll("td span");
                const spanArr = Array.prototype.slice.call(spans);
                propVal = spanArr.map((span) => span.textContent);
            } else if (propType === "relation") {
                const tempNodes = tr.querySelector("td a").childNodes;
                propVal = tempNodes[tempNodes.length - 1].nodeValue;
            } else {
                propVal = tr.querySelector("td").textContent;
            }
            const prop = {
                propName: propName,
                propType: propType,
                propVal: propVal,
            };
            return prop;
        });
    }
    console.log(properties);
    const tistoryProps = [
        "Visibility",
        "Category",
        "Slogan",
        "PublishDate",
        "Tag",
        "AcceptComment",
        "Password",
    ];

    let notionProps = properties.reduce((result, curr) => {
        if (tistoryProps.includes(curr.propName)) {
            result[curr.propName] = curr.propVal;
        }
        return result;
    }, {});

    notionProps["Title"] = title;
    notionProps["content"] = article;

    const notionPage = new NotionPage(notionProps);

    console.log("Reading process done.");
    //console.log(notionPage);
    return notionPage;
}

function convertToPost({ content: html, imageList }) {
    const notionPage = readPage(html);
    convertHtml(notionPage);
    console.log(notionPage);
    return { notionPage, imageList };
}

function convertHtml(page) {
    const article = page.content;

    //헤더 삭제
    article.querySelector("header").remove();

    //Notion_P css 적용
    article.classList.add("Notion_P");

    //토글 전부 접기
    const toggles = article.querySelectorAll("ul.toggle>li>details");
    toggles.forEach((toggle) => toggle.removeAttribute("open"));

    //Embed 형식 변환
    const embedBlocks = article.querySelectorAll(
        "figure>div.source:not(.bookmark)"
    );
    embedBlocks.forEach((embedBlock) => {
        const url = embedBlock.querySelector("a").getAttribute("href");
        const embedUrl = getEmbedUrl(url);
        console.log(embedBlock);
        const embedHtml =
            "<div class='embed_container'><iframe src='" +
            embedUrl +
            "' class='embed_inner'></iframe></div>";
        embedBlock.outerHTML = embedHtml;
    });

    return page;
}

function getEmbedUrl(url) {
    const website = url.split("://")[1].split("/")[0];
    let embedUrl = "";
    switch (website) {
        case "www.youtube.com":
            embedUrl = url.replace("watch?v=", "embed/");
            break;
        case "codepen.io":
            embedUrl = url.replace("/pen/", "/embed/");
            break;
        case "whimsical.com":
            embedUrl =
                "https://whimsical.com/embed" +
                url.substring(url.lastIndexOf("/"), url.length);
            break;
        case "www.figma.com":
            embedUrl =
                "https://www.figma.com/embed?embed_host=share&url=" +
                encodeURIComponent(url);
            break;
        default:
            embedUrl = url;
    }
    return embedUrl;
}

async function replaceImage(apiClient, NotionPage, ImageList) {
    const htmlImageList = NotionPage.content.querySelectorAll("figure.image");
    const htmlImageArr = Array.prototype.slice.call(htmlImageList);
    console.log(htmlImageArr);
    const promises = ImageList.map(async (imageData, index) => {
        const imageName = imageData.entryName;
        const htmlImage = htmlImageArr.filter(
            (fig) =>
                decodeURI(fig.querySelector("a").getAttribute("href")) ===
                imageName
        )[0];
        console.log(htmlImage);
        console.log(imageData);
        const { replacer, url } = await tistory.uploadImage(
            apiClient,
            imageData
        );
        //htmlImage.innerHTML = replacer;
        htmlImage.querySelector("a").setAttribute("href", url);
        htmlImage.querySelector("img").setAttribute("src", url);
    });
    await Promise.all(promises);
    console.log(NotionPage.content.outerHTML);
    console.log("done!");
}

function maketoHTMLdocument(article) {
    return `<!DOCTYPE html><html lang="ko"><head><link rel="stylesheet" href="${N2T_STYLE}" /></head><body>${article}</body></html>`;
}

module.exports = {
    NotionPage,
    readHtml: readPage,
    convertToPost,
    replaceImage,
    maketoHTMLdocument,
};
