const request = require("request");
const curr_version = require("electron").app.getVersion();

const checkUpdate = async () => {
    console.log("myUpdateChecker start!");
    const options = {
        uri:
            "https://api.github.com/repos/boltlessengineer/Notion2Tistory/releases/latest",
        headers: { "user-agent": "node.js" },
    };
    return new Promise((resolve, reject) => {
        request(options, (err, response, body) => {
            const resBody = JSON.parse(body);
            if (resBody.message) {
                console.log(resBody.message);
                reject(resBody.message);
            } else {
                console.log(`Latest release version  : ${resBody.tag_name}`);
                console.log(`Current app version     : ${curr_version}`);
                const latestV = resBody.tag_name;
                const currentV = curr_version;
                if (compareVersions(latestV, currentV)) {
                    console.log(resBody.html_url);
                    resolve(resBody.html_url);
                } else {
                    console.log("nothing to update");
                    resolve(null);
                }
            }
        });
    });
};

const compareVersions = (versionA, versionB) => {
    // return true if versionA > versionB
    // if vA = latest & vB = curr
    // return true if vA > vB false if vA <= vB
    const reg = /\d+/g; //  /\d+|\-\w+/g
    versionA = versionA.match(reg);
    versionB = versionB.match(reg);

    const length = Math.max(versionA.length, versionB.length);

    for (let i = 0; i < length; i++) {
        const a = versionA[i] ? parseInt(versionA[i], 10) : 0;
        const b = versionB[i] ? parseInt(versionB[i], 10) : 0;
        console.log(`${a} vs ${b}`);
        if (a > b) {
            return true;
        } else {
            if (b > a) {
                return false;
            }
        }
    }
    return false;
};

module.exports = {
    checkUpdate,
};
