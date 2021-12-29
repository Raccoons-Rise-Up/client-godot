using Godot;
using KRU.IO;
using KRU.Networking;
using KRU.Utils;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
        private static HTTPRequest HttpRequest { get; set; }

        private static Control ControlSceneMainMenu { get; set; }
        private static Control ControlSceneGame { get; set; }
        private static Button BtnLogout { get; set; }
        private static LineEdit InputUsername { get; set; }
        private static LineEdit InputPassword { get; set; }
        private static Label LoginAsSection { get; set; }
        private static Label LabelResponse { get; set; }
        private static VBoxContainer LoginSection { get; set; }

        // Web Properties
        private static string Token { get; set; }

        private static bool AttemptedToRenewInvalidToken { get; set; }

        public override void _Ready()
        {
            HttpRequest = GetNode<HTTPRequest>(nodePathHttp);
            HttpRequest.Connect("request_completed", this, "OnRequestCompleted");

            InputUsername = GetNode<LineEdit>(nodePathInputUsername);
            InputPassword = GetNode<LineEdit>(nodePathInputPassword);

            LoginAsSection = GetNode<Label>(nodePathLoginAsSection);
            LoginSection = GetNode<VBoxContainer>(nodePathLoginSection);

            BtnLogout = GetNode<Button>(nodePathBtnLogout);

            LabelResponse = GetNode<Label>(nodePathLabelResponse);

            ControlSceneMainMenu = GetNode<Control>(nodePathSceneMainMenu);
            ControlSceneGame = GetNode<Control>(nodePathSceneGame);
        }

        private void _on_Btn_Login_pressed() => Login();

        private void _on_Btn_Logout_pressed() => Logout();

        public static void LoadGameScene()
        {
            ControlSceneMainMenu.Visible = false;
            ControlSceneGame.Visible = true;
        }

        public static void LoadMenuScene()
        {
            ControlSceneMainMenu.Visible = true;
            ControlSceneGame.Visible = false;
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
                BtnLogout.Visible = true;
                LoginAsSection.Visible = true;

                if (InputUsername.Text != "")
                    LoginAsSection.Text = $"Connect as {InputUsername.Text}";
                else
                    LoginAsSection.Text = $"Connect as {contents["username"]}";

                LoginSection.Visible = false;
            }
        }

        public static void UpdateResponse(string text) => LabelResponse.Text = text;

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

            GD.Print("Web Response: " + webResponse.Message);

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
                    // Reset token in local file system
                    AppData.SaveJsonWebToken(null, "");
                    Token = null;

                    GD.Print("Invalid token, going to try relogging...");
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
                        AppData.SaveJsonWebToken(Token, InputUsername.Text);
                        ENetClient.JsonWebToken = Token;
                    }
                    else
                    {
                        AppData.SaveJsonWebToken(res.Token, InputUsername.Text);
                        ENetClient.JsonWebToken = res.Token;
                    }

                    await Task.Delay(100); // Add a small delay of 100 ms to give application a chance to keep up (after all we did just do a POST request)

                    GD.Print("Attempting to connect to the game server...");
                    //GD.Print("A");
                    //System.Console.WriteLine("A");

                    ENetClient.Connect();
                    //GD.Print("C");
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

            if (InputUsername.Text != AppData.GetStorage()["username"])
            {
                // A new username was entered that was different from the one in storage, asking for a new JWT
                GD.Print("A new username was entered that was different from the one in storage, asking for a new JWT");
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
                Username = InputUsername.Text,
                From = "Godot-Client"
            };
        }

        private static WebPostLoginContent BasicLoginInfo() => new WebPostLoginContent
        {
            Username = InputUsername.Text,
            Password = InputPassword.Text,
            From = "Godot-Client"
        };

        private static void HideConnectAsSection()
        {
            LoginAsSection.Visible = false;
            LoginSection.Visible = true;
            BtnLogout.Visible = false;
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