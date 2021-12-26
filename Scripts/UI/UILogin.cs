using Godot;
using KRU.IO;
using KRU.Networking;
using KRU.Utils;
using Newtonsoft.Json;

namespace KRU.UI
{
    public class UILogin : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathSceneMainMenu;
        [Export] private readonly NodePath nodePathSceneGame;
        [Export] private readonly NodePath nodePathHttp;
        [Export] private readonly NodePath nodePathInputUsername;
        [Export] private readonly NodePath nodePathInputPassword;
        [Export] private readonly NodePath nodePathLoginAsSection;
        [Export] private readonly NodePath nodePathLoginSection;
        [Export] private readonly NodePath nodePathBtnLogout;
        [Export] private readonly NodePath nodePathLabelResponse;
#pragma warning restore CS0649 // Values are assigned in the editor

        // Nodes
        private static HTTPRequest httpRequest;

        private static Control controlSceneMainMenu, controlSceneGame;
        private static Button btnLogout;
        private static LineEdit inputUsername, inputPassword;
        private static Label loginAsSection, labelResponse;
        private static VBoxContainer loginSection;

        // Web Properties
        private static string Token { get; set; }

        private static bool AttemptedToRenewInvalidToken { get; set; }

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
        }

        private void _on_Btn_Login_pressed() => Login();

        private void _on_Btn_Logout_pressed() => Logout();

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

            var contents = AppData.GetStorage();
            if (contents == null || contents["token"] == null)
            {
                HideConnectAsSection();
            }
            else
            {
                btnLogout.Visible = true;
                loginAsSection.Visible = true;

                if (inputUsername.Text != "")
                    loginAsSection.Text = $"Connect as {inputUsername.Text}";
                else
                    loginAsSection.Text = $"Connect as {contents["username"]}";

                loginSection.Visible = false;
            }
        }

        public static void UpdateResponse(string text) => labelResponse.Text = text;

        private static void Logout()
        {
            HideConnectAsSection();
        }

        private async static void Login()
        {
            var jsonStr = JsonConvert.SerializeObject(GetLoginInfo());
            UpdateResponse("Sending login request to web server...");
            var webResponse = await WebUtils.PostRequest("api/login", jsonStr);

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

            var res = JsonConvert.DeserializeObject<WebPostLoginResponse>(webResponse.Message);

            UpdateResponse(res.Message);

            switch ((LoginOpcode)res.Opcode)
            {
                case LoginOpcode.AccountDoesNotExist:
                case LoginOpcode.InvalidUsernameOrPassword:
                case LoginOpcode.PasswordsDoNotMatch:
                    break;

                case LoginOpcode.TokenUsernameDoesNotMatchWithProvidedUsername:
                    // Reset token in local file system
                    AppData.SaveJsonWebToken(null, "");
                    Token = null;
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
                        AppData.SaveJsonWebToken(Token, inputUsername.Text);
                        ENetClient.JsonWebToken = Token;
                    }
                    else
                    {
                        AppData.SaveJsonWebToken(res.Token, inputUsername.Text);
                        ENetClient.JsonWebToken = res.Token;
                    }

                    GD.Print("Attempting to connect to the game server...");
                    ENetClient.Connect();
                    break;
            }
        }

        private static WebPostLoginContent GetLoginInfo()
        {
            if (!AppData.JsonWebTokenFileExists()) 
            {
                // Token does not exist, lets create a request to the web server for a new one
                GD.Print("Token does not exist in local file system");
                return BasicLoginInfo();
            }

            if (AppData.GetStorage() == null)
            {
                // Invalid JSON
                GD.Print("Token.json is invalid");
                return BasicLoginInfo();
            }

            if (inputUsername.Text != AppData.GetStorage()["username"])
            {
                // A new username was entered that was different from the one in storage, asking for a new JWT
                return BasicLoginInfo();
            }

            Token = AppData.GetStorage()["token"];

            if (Token == null)
            {
                // Token is null in JSON
                GD.Print("Token in token.json is null");
                return BasicLoginInfo();
            }

            // Token exists in local file system
            GD.Print("Token exists in local file system");
            return new WebPostLoginContent
            {
                Token = Token,
                Username = inputUsername.Text,
                From = "Godot-Client"
            };
        }

        private static WebPostLoginContent BasicLoginInfo() => new WebPostLoginContent
        {
            Username = inputUsername.Text,
            Password = inputPassword.Text,
            From = "Godot-Client"
        };

        private static void HideConnectAsSection()
        {
            loginAsSection.Visible = false;
            loginSection.Visible = true;
            btnLogout.Visible = false;
        }
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
        InvalidToken,
        TokenUsernameDoesNotMatchWithProvidedUsername
    }
}