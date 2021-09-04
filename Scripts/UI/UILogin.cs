using System;
using System.Threading.Tasks;
using System.IO;
using System.Text;
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
            UpdateResponse("");
            AppData.SaveJsonWebToken(null, "");
            Token = null;
            HideConnectAsSection();
        }

        private static readonly HttpClient webClient = new HttpClient();
        private static bool sendingRequest;

        private static async Task<string> GetRequest(string path)
        {
            if (sendingRequest)
                return "";

            sendingRequest = true;

            string stringTask = "";
            try
            {
                stringTask = await webClient.GetStringAsync("http://127.0.0.1:4000/" + path);
            }
            catch (Exception e)
            {
                UpdateResponse(e.Message);
            }

            sendingRequest = false;

            return stringTask;
        }

        private static async Task<string> PostRequest(string path, string content)
        {
            if (sendingRequest)
                return "";

            sendingRequest = true;

            string stringTask = "";
            using (var requestContent = new StringContent(content, Encoding.UTF8, "application/json"))
            {
                try
                {
                    using (var response = await webClient.PostAsync("http://127.0.0.1:4000/" + path, requestContent))
                    {
                        stringTask = await response.Content.ReadAsStringAsync();
                    };
                }
                catch (Exception e)
                {
                    if (e is HttpRequestException)
                        UpdateResponse("Web server is offline");
                    else
                        UpdateResponse(e.GetType().ToString());
                }
            }

            sendingRequest = false;

            return stringTask;
        }

        private async void _on_Btn_Login_pressed()
        {
            Token = AppData.GetJsonWebToken()["token"];

            WebPostLoginContent loginInfo;
            if (Token == null)
            {
                loginInfo = new WebPostLoginContent
                {
                    username = inputUsername.Text,
                    password = inputPassword.Text,
                    from = "Godot-Client"
                };
            }
            else
            {
                loginInfo = new WebPostLoginContent
                {
                    token = Token,
                    from = "Godot-Client"
                };
            }

            var jsonStr = JsonConvert.SerializeObject(loginInfo);
            UpdateResponse("Sending login request to web server...");
            var resStr = await PostRequest("api/login", jsonStr);
            var res = JsonConvert.DeserializeObject<WebPostLoginResponse>(resStr);
            UpdateResponse(res.message);

            switch ((LoginOpcode)res.opcode)
            {
                case LoginOpcode.AccountDoesNotExist:
                case LoginOpcode.InvalidUsernameOrPassword:
                case LoginOpcode.PasswordsDoNotMatch:
                    break;
                case LoginOpcode.InvalidToken:
                    Logout();
                    break;
                case LoginOpcode.LoginSuccess:
                    if (Token != null)
                    {
                        ENetClient.JsonWebToken = Token;
                    }
                    else
                    {
                        AppData.SaveJsonWebToken(res.token, inputUsername.Text);
                        ENetClient.JsonWebToken = res.token;
                    }

                    ENetClient.Connect();
                    break;
            }
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

    public class WebPostLoginContent
    {
        public string username { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public string from { get; set; }
    }

    public class WebPostLoginResponse
    {
        public int opcode;
        public string message;
        public string token;
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

