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
                Console.WriteLine("Got Authorization code");
                //Console.WriteLine($"Authorization code : {code}");
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
                    Console.WriteLine("Got Access Token");
                    //Console.WriteLine($"Access Token : {token}");
                }
                response.Close();
                return token;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error. Can't get the Access Token");
                Console.WriteLine(e);
                throw e;
            }
        }

        public static string Encode(string str)
        {
            byte[] byteDataParams = UTF8Encoding.UTF8.GetBytes(str);
            string encodedParams = HttpUtility.UrlEncode(byteDataParams, 0, byteDataParams.Length);
            return encodedParams;
        }

        // https://spirit32.tistory.com/21
        public class FormFile
        {
            public string Name { get; set; }
            public string ContentType { get; set; }
            public string FilePath { get; set; }
            public Stream Stream { get; set; }
        }

        public class RequestHelper
        {
            public static string PostMultipart(string url, Dictionary<string, object> parameters)
            {

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = CredentialCache.DefaultCredentials;

                if (parameters != null && parameters.Count > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        foreach (KeyValuePair<string, object> pair in parameters)
                        {
                            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                            if (pair.Value is FormFile)
                            {
                                FormFile file = pair.Value as FormFile;
                                string header = "Content-Disposition: form-data; name=\"" + pair.Key + "\"; filename=\"" + file.Name + "\"\r\nContent-Type: " + file.ContentType + "\r\n\r\n";
                                byte[] bytes = Encoding.UTF8.GetBytes(header);
                                requestStream.Write(bytes, 0, bytes.Length);
                                byte[] buffer = new byte[32768];
                                int bytesRead;
                                if (file.Stream == null)
                                {
                                    // upload from file
                                    Console.WriteLine("Uploading...");
                                    using (FileStream fileStream = File.OpenRead(file.FilePath))
                                    {
                                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                            requestStream.Write(buffer, 0, bytesRead);
                                        fileStream.Close();
                                    }
                                }
                                else
                                {
                                    // upload from given stream
                                    while ((bytesRead = file.Stream.Read(buffer, 0, buffer.Length)) != 0)
                                        requestStream.Write(buffer, 0, bytesRead);
                                }
                            }
                            else
                            {
                                string data = "Content-Disposition: form-data; name=\"" + pair.Key + "\"\r\n\r\n" + pair.Value;
                                byte[] bytes = Encoding.UTF8.GetBytes(data);
                                requestStream.Write(bytes, 0, bytes.Length);
                            }
                        }
                        byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                        requestStream.Write(trailer, 0, trailer.Length);
                        requestStream.Close();
                    }
                }
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                        return reader.ReadToEnd();
                }
            }
            
            public static string GetRequest(string url, Dictionary<string, object> parameters)
            {
                string stringParameters = "?";
                foreach(KeyValuePair<string, object> kv in parameters)
                {
                    stringParameters += kv.Key + "=" + kv.Value + "&";
                }
                stringParameters = stringParameters.TrimEnd('&');

                string requestUrl = url + stringParameters;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Credentials = CredentialCache.DefaultCredentials;
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                        return reader.ReadToEnd();
                }
            }
        }

        public class Category
        {
            public string id { get; set; }
            public string name { get; set; }
            public string parent { get; set; }
            public string label { get; set; }
            public string entries { get; set; }
        }

        public int FindCategory(string categoryName)
        {
            string apiUrl = "https://www.tistory.com/apis/category/list";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("access_token", accessToken);
            parameters.Add("output", "json");
            parameters.Add("blogName", blogName);

            string result = RequestHelper.GetRequest(apiUrl, parameters);
            JObject json = JObject.Parse(result);
            var list = json["tistory"]["item"]["categories"] as JArray;
            Console.WriteLine(list);

            var List = list.ToObject<List<Category>>();

            var FindList = List.Where(x => x.name == categoryName);
            if (FindList.Count() > 0)
            {
                Category category = List.Where(x => x.name == categoryName).First();
                return Int32.Parse(category.id);
            }
            else
            {
                return 0;
            }
        }

        public List<AttachedImage> UploadImages(List<AttachedImage> list)
        {
            foreach(AttachedImage image in list)
            {
                Console.WriteLine("Uploading {0}", image.originalPath);

                FormFile file = new FormFile()
                {
                    Name = Uri.EscapeUriString(image.originalPath),
                    ContentType = "image/png",
                    FilePath = image.originalPath
                };

                FileInfo fileInfo = new FileInfo(file.FilePath);
                if (fileInfo.Exists)
                {
                    Console.WriteLine("exist");
                }
                else
                {
                    Console.WriteLine("NOP");
                }

                string result = RequestHelper.PostMultipart(
                    "https://www.tistory.com/apis/post/attach",
                    new Dictionary<string, object>() {
                        { "access_token", accessToken },
                        { "output", "json" },
                        { "blogName", blogName },
                        {
                            "uploadedfile", file
                        }
                    }
                );

                JObject json = JObject.Parse(result);
                Console.WriteLine(json);
                string replacer = json["tistory"]["replacer"].ToString();
                string url = json["tistory"]["url"].ToString();
                
                string imageId = url.Substring(url.IndexOf("image/") + 6, url.Length - url.IndexOf("image/") - 10);
                string imageReplacer = "[##_Image|t/cfile@" + imageId + "|alignCenter|data-filename=\"thumbnail.png\" data-origin-width=&quot;0&quot; data-origin-height=&quot;0&quot; data-ke-mobilestyle=&quot;widthContent&quot;|" + image.originalCaption + "||_##]";
                Console.WriteLine("===========================================================");
                Console.WriteLine("Replacer : {0}", replacer);
                Console.WriteLine("Url      : {0}", url);
                Console.WriteLine("===========================================================");
                Console.WriteLine("Image Replacer : {0}", imageReplacer);
                Console.WriteLine("===========================================================");

                image.replacer = imageReplacer;
            }

            return list;
        }

        public void UploadPost(Content content)
        {
            Dictionary<string, object> postDict = new Dictionary<string, object>();
            postDict.Add("access_token", accessToken);
            postDict.Add("output", "json");
            postDict.Add("blogName", blogName);
            postDict.Add("title", content.Title);
            postDict.Add("content", content.Article);
            postDict.Add("visibility", content.Visibility.ToString());
            postDict.Add("category", content.CategoryId.ToString());
            // postDict.Add("published", published);
            // postDict.Add("slogan", slogan);
            postDict.Add("tag", string.Join(",", content.Tags));
            postDict.Add("acceptComent", content.AcceptComent ? "1" : "0");
            postDict.Add("password", content.Password);

            string result = RequestHelper.PostMultipart("https://www.tistory.com/apis/post/write", postDict);
            JObject json = JObject.Parse(result);
            Console.WriteLine(json);
        }
    }
}
