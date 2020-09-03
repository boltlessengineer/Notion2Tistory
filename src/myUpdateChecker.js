const request = require("request");

const checkUpdate = async () => {
    console.log("myUpdateChecker start!");
    const options = {
        uri:
            "https://api.github.com/repos/boltlessengineer/Notion2Tistory/releases/latest",
        headers: { "user-agent": "node.js" },
    };
    request(options, (err, response, body) => {
        console.log(err);
        console.log(response);
        console.log(body);
        const resBody = JSON.parse(body);
        console.log(resBody);
        if (resBody.message) {
            console.log(resBody.message);
        }
    });
};

module.exports = {
    checkUpdate,
};
