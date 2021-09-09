using System;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using KRU.Networking;
using KRU.IO;

using HttpClient = System.Net.Http.HttpClient;

namespace KRU.UI
{
    public class UILogin : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathSceneMainMenu, nodePathSceneGame;
        [Export] private NodePath nodePathHttp, nodePathInputUsername, nodePathInputPassword, nodePathLoginAsSection, nodePathLoginSection, nodePathBtnLogout, nodePathLabelResponse;
#pragma warning restore CS0649 // Values are assigned in the editor
        private static HTTPRequest httpRequest;
        private static Control controlSceneMainMenu, controlSceneGame;
        private static Button btnLogout;
        private static LineEdit inputUsername, inputPassword;
        private static Label loginAsSection, labelResponse;
        private static VBoxContainer loginSection;
        private static ENetClient ENetClient;
        private static string Token { get; set; }
        private static bool AttemptedToRenewInvalidToken { get; set; }

        private static readonly HttpClient webClient = new HttpClient();
        private static bool SendingRequest { get; set; }

        public override void _Ready()
        {
            httpRequest = GetNode<HTTPRequest>(nodePathHttp);
            httpRequest.Connect("request_completed", this, "OnRequestCompleted");

            inputUsername = GetNode<LineEdit>(nodePathInputUsername);
            inputPassword = GetNode<LineEdit>(nodePathInputPassword);

            loginAsSection = GetNode<Label>(nodePathLoginAsSection);
            loginSection = GetNode<VBoxContainer>(nodePathLoginSection);

            btnLogout = GetNode<Button>(nodePathBtnLogout);

            labelResponse = GetNode<Label>(nodePathLabelResponse);

            controlSceneMainMenu = GetNode<Control>(nodePathSceneMainMenu);
            controlSceneGame = GetNode<Control>(nodePathSceneGame);

            ENetClient = GetNode<ENetClient>("/root/ENetClient");
        }

        public static void LoadGameScene()
        {
            controlSceneMainMenu.Visible = false;
            controlSceneGame.Visible = true;
        }

        public static void LoadMenuScene()
        {
            controlSceneMainMenu.Visible = true;
            controlSceneGame.Visible = false;
        }

        public static void InitLoginSection()
        {
            UpdateResponse("");

            var contents = AppData.GetJsonWebToken();
            if (contents == null || contents["token"] == null)
            {
                HideConnectAsSection();
            }
            else
            {
                btnLogout.Visible = true;
                loginAsSection.Visible = true;
                loginAsSection.Text = $"Connect as {contents["username"]}";

                loginSection.Visible = false;
            }
        }

        public static void UpdateResponse(string text) => labelResponse.Text = text;

        private static void Logout()
        {
            AppData.SaveJsonWebToken(null, "");
            Token = null;
            HideConnectAsSection();
        }

        private static async Task<string> GetRequest(string path)
        {
            if (SendingRequest)
                return "Currently sending a request please wait...";

            SendingRequest = true;

            string stringTask = "";
            try
            {
                stringTask = await webClient.GetStringAsync("http://127.0.0.1:4000/" + path);
            }
            catch (Exception e)
            {
                SendingRequest = false;

                return e.Message;
            }

            SendingRequest = false;

            return stringTask;
        }

        private static async Task<WebPostResponse> PostRequest(string path, string content)
        {
            if (SendingRequest)
                return WebPostResponse(WebPostResponseOpcode.InMiddleOfRequest, "Currently sending a request please wait...", false);

            SendingRequest = true;

            string stringTask = "";
            using (var requestContent = new StringContent(content, Encoding.UTF8, "application/json"))
            {
                try
                {
                    using (var response = await webClient.PostAsync("http://127.0.0.1:4000/" + path, requestContent))
                    {
                        GD.Print(response.StatusCode);

                        switch (response.StatusCode)
                        {
                            case (HttpStatusCode)429: // HttpStatusCode.TooManyRequests not supported in this version of .NET
                                return WebPostResponse(WebPostResponseOpcode.TooManyRequests, "Please wait a bit before making another request");
                            case HttpStatusCode.OK:
                                break;
                        }

                        stringTask = await response.Content.ReadAsStringAsync();
                    };
                }
                catch (Exception e)
                {
                    if (e is HttpRequestException)
                        return WebPostResponse(WebPostResponseOpcode.WebServerOffline, "Web server is offline");
                    else
                        return WebPostResponse(WebPostResponseOpcode.Exception, e.GetType().ToString());
                }
            }

            return WebPostResponse(WebPostResponseOpcode.Success, stringTask);
        }

        private static WebPostResponse WebPostResponse(WebPostResponseOpcode opcode, string message, bool resetSendingRequest = true)
        {
            if (resetSendingRequest)
                SendingRequest = false;

            return new WebPostResponse
            {
                Opcode = opcode,
                Message = message
            };
        }

        private async void Login()
        {
            Token = AppData.GetJsonWebToken()["token"];

            WebPostLoginContent loginInfo;
            if (Token == null)
            {
                loginInfo = new WebPostLoginContent
                {
                    Username = inputUsername.Text,
                    Password = inputPassword.Text,
                    From = "Godot-Client"
                };
            }
            else
            {
                loginInfo = new WebPostLoginContent
                {
                    Token = Token,
                    From = "Godot-Client"
                };
            }

            var jsonStr = JsonConvert.SerializeObject(loginInfo);
            UpdateResponse("Sending login request to web server...");
            var webResponse = await PostRequest("api/login", jsonStr);

            switch (webResponse.Opcode)
            {
                case WebPostResponseOpcode.InMiddleOfRequest:
                case WebPostResponseOpcode.WebServerOffline:
                case WebPostResponseOpcode.Exception:
                case WebPostResponseOpcode.TooManyRequests:
                    UpdateResponse(webResponse.Message);
                    return;
                case WebPostResponseOpcode.Success:
                    break;
            }



            GD.Print(webResponse.Message);

            WebPostLoginResponse res;
            try
            {
                res = JsonConvert.DeserializeObject<WebPostLoginResponse>(webResponse.Message);
            }
            catch (JsonReaderException e)
            {

                GD.Print(e.Message);
                return;
            }

            UpdateResponse(res.Message);

            switch ((LoginOpcode)res.Opcode)
            {
                case LoginOpcode.AccountDoesNotExist:
                case LoginOpcode.InvalidUsernameOrPassword:
                case LoginOpcode.PasswordsDoNotMatch:
                    break;
                case LoginOpcode.InvalidToken:
                    Logout();

                    // Automatically try to login one more time
                    if (!AttemptedToRenewInvalidToken)
                    {
                        AttemptedToRenewInvalidToken = true;
                        Login();
                    }
                    break;
                case LoginOpcode.LoginSuccess:
                    if (Token != null)
                    {
                        ENetClient.JsonWebToken = Token;
                    }
                    else
                    {
                        AppData.SaveJsonWebToken(res.Token, inputUsername.Text);
                        ENetClient.JsonWebToken = res.Token;
                    }

                    ENetClient.Connect();
                    break;
            }
        }

        private void _on_Btn_Login_pressed()
        {
            Login();
        }

        private void _on_Btn_Logout_pressed()
        {
            Logout();
        }

        private static void HideConnectAsSection()
        {
            loginAsSection.Visible = false;
            loginSection.Visible = true;
            btnLogout.Visible = false;
        }
    }

    public struct WebPostResponse
    {
        public string Message { get; set; }
        public WebPostResponseOpcode Opcode { get; set; }
    }

    public enum WebPostResponseOpcode
    {
        InMiddleOfRequest,
        WebServerOffline,
        TooManyRequests,
        Exception,
        Success
    }

    public struct WebPostLoginContent
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string From { get; set; }
    }

    public struct WebPostLoginResponse
    {
        public int Opcode;
        public string Message;
        public string Token;
    }

    public enum LoginOpcode
    {
        LoginSuccess,
        InvalidUsernameOrPassword,
        AccountDoesNotExist,
        PasswordsDoNotMatch,
        InvalidToken
    }
}

