using minecraft_launcher_v2.ConstantValues;
using minecraft_launcher_v2.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;

namespace minecraft_launcher_v2.Classes.Controls.Static
{
    static class AuthenticationControl
    {
        public static string Authenticate(string username, SecureString password)
        {
            RootAuthenticationRequest jsonRequestData = new RootAuthenticationRequest();
            Agent agent = new Agent();

            agent.name = "Minecraft";
            agent.version = 1;

            jsonRequestData.username = username;
            jsonRequestData.agent = agent;
            jsonRequestData.requestUser = false;
            jsonRequestData.password = new NetworkCredential(string.Empty, password).Password;

            return MakeRequest(JsonConvert.SerializeObject(jsonRequestData), Constants.URL_AUTHENTICATE);
        }

        public static string Refresh(string accessToken, string clientToken, string id, string name)
        {
            RootRefreshRequestResponse jsonRequestData = new RootRefreshRequestResponse();
            SelectedProfileRefresh profile = new SelectedProfileRefresh();

            if (!string.IsNullOrWhiteSpace(id))
            {
                profile.id = id;
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                profile.name = name;
            }

            jsonRequestData.accessToken = accessToken;
            jsonRequestData.clientToken = clientToken;
            jsonRequestData.selectedProfile = profile;
            jsonRequestData.requestUser = false;

            return MakeRequest(JsonConvert.SerializeObject(jsonRequestData), Constants.URL_REFRESH);
        }

        public static string SignOut(string username, SecureString password)
        {
            RootSignOutRequest jsonRequestData = new RootSignOutRequest();

            jsonRequestData.username = username;
            jsonRequestData.password = new NetworkCredential(string.Empty, password).Password;

            return MakeRequest(JsonConvert.SerializeObject(jsonRequestData), Constants.URL_SIGNOUT);
        }

        public static string Validate(string accessToken, string clientToken)
        {
            RootValidateInvalidateRequest jsonRequestData = new RootValidateInvalidateRequest();

            jsonRequestData.accessToken = accessToken;
            jsonRequestData.clientToken = clientToken;

            return MakeRequest(JsonConvert.SerializeObject(jsonRequestData), Constants.URL_VALIDATE);
        }

        public static string Invalidate(string accessToken, string clientToken)
        {
            RootValidateInvalidateRequest jsonRequestData = new RootValidateInvalidateRequest();

            jsonRequestData.accessToken = accessToken;
            jsonRequestData.clientToken = clientToken;

            return MakeRequest(JsonConvert.SerializeObject(jsonRequestData), Constants.URL_INVALIDATE);
        }


        private static string MakeRequest(string sendData, string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postBytes = Encoding.ASCII.GetBytes(sendData);
                sendData = null;

                requestStream.Write(postBytes, 0, postBytes.Length);

                postBytes = null;

                requestStream.Close();
                requestStream.Dispose();
            }

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);


            string responseString = Constants.RESPONSE_EMPTY;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.NoContent)
                    {
                        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }

                    response.Close();
                    response.Dispose();
                }
            }
            catch (WebException ex)
            {
                try
                {
                    responseString = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                    ex.Response.Close();
                    ex.Response.Dispose();
                }
                catch
                {
                    responseString = "";
                }
            }
            catch
            {
                responseString = "";
            }

            return responseString;
        }

    }
}
