using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Notion2TistoryConsole
{
    class TistoryAPI
    {
        public const string oauthUrl = "https://www.tistory.com/oauth/authorize";
        public const string accessUrl = "https://www.tistory.com/oauth/access_token";
        private const string apiBaseUrl = "https://www.tistory.com/apis/";

        public static string clientID;
        public static string clientSK;
        public static string redirect;
        public static string userID;
        public static string userPW;
        public static string blogName;
        public static string accessToken;

        public TistoryAPI(string cID, string cSK, string redir, string uID, string uPW, string blog)
        {
            clientID = cID;
            clientSK = cSK;
            redirect = redir;
            userID = uID;
            userPW = uPW;
            blogName = blog;
            accessToken = "";

            string requestUrl = $"{oauthUrl}?client_id={clientID}&redirect_uri={redirect}&response_type=code";
            string authCode = GetAccessCode(userID, userPW, requestUrl);
            string accessTokenUrl = $"{accessUrl}?client_id={clientID}&client_secret={clientSK}&redirect_uri={redirect}&code={authCode}&grant_type=authorization_code";

            accessToken = GetAccessToken(accessTokenUrl);
        }

        public string AccessToken()
        {
            return accessToken;
        }

        public string BlogName()
        {
            return blogName;
        }

        public void ShowClient()
        {
            Console.WriteLine("Client ID  : {0}", clientID);
            Console.WriteLine("Secret Key : {0}", clientSK);
        }

        public void ChangeClient(string id, string sk)
        {
            clientID = id;
            clientSK = sk;
        }

        private static string GetAccessCode(string ID, string PW, string reqUrl)
        {
            try
            {
                string loginUrl = "https://www.tistory.com/auth/login";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loginUrl);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "POST";
                request.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = new CookieContainer();
                request.Host = "www.tistory.com";
                request.Referer = "https://www.tistory.com";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

                StringBuilder DataParams = new StringBuilder();
                DataParams.Append("?fp=4452e4b0f02c73e63703b45b8e6bfeaf");//a668ac102f81b86f521c07dfb6dc992c
                DataParams.Append("&keepLogin=on");
                DataParams.Append("&loginId=" + Encode(ID));
                DataParams.Append("&password=" + Encode(PW));
                DataParams.Append("&redirectUrl=" + Encode(reqUrl));

                StreamWriter swriter = new StreamWriter(request.GetRequestStream());
                swriter.Write(DataParams.ToString());
                swriter.Close();

                Console.WriteLine("Login success"); // 로그인 시도 후 redirect된 경로가 다시 로그인 창인지 확인해야 함

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream sReadData = response.GetResponseStream();
                StreamReader srReadData = new StreamReader(sReadData, Encoding.Default, true);

                string strResult = srReadData.ReadToEnd();

                //Console.WriteLine(strResult);

                int findFrom = strResult.IndexOf("?code=") + 6;
                int findTo = strResult.IndexOf("&state") - findFrom;
                string code = strResult.Substring(findFrom, findTo);
                Console.WriteLine($"Authorization code : {code}");
                return code;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error. Can't get the Authorization code");
                throw e;
            }
        }

        private static string GetAccessToken(string reqUrl)
        {
            try
            {
                string token;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqUrl);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    token = responseFromServer.Substring(13);
                    Console.WriteLine($"Access Token : {token}");
                }
                response.Close();
                return token;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error. Can't get the Access Token");
                throw e;
            }
        }

        private static string Encode(string str)
        {
            byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(str);
            string encodedParams = HttpUtility.UrlEncode(byteDataParams, 0, byteDataParams.Length);
            return encodedParams;
        }

        public static async string UploadImage()
        {
            string replacer = "";

            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent(accessToken), "username");

            return replacer;
        }

        public static async Task<JObject> SendAPIPost(string postUrl, IEnumerable<KeyValuePair<string, string>> contentDict)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            var param = new FormUrlEncodedContent(contentDict);
            Console.WriteLine(contentDict);
            var result = await client.PostAsync(postUrl, param);
            string responseString = await result.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseString);
            if (json["tistory"]["status"].ToString() == "200")
            {
                Console.WriteLine("Task Success!");
            }
            else
            {
                Console.WriteLine("ERROR : Server returned error | status: {0}", json["tistory"]["status"]);
                Console.WriteLine(json["tistory"]["error_message"]);
            }
            return json;
        }
    }
}
