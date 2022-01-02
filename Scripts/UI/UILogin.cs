using Godot;
using System;
using System.Threading.Tasks;
using Client.Netcode;
using Client.Utils;
using Newtonsoft.Json;

namespace Client.UI 
{
    public class UILogin : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly string gameServerIp;
        [Export] private readonly ushort gameServerPort;
        [Export] private readonly string webServerIp;
        [Export] private readonly ushort webServerPort;
        [Export] private readonly NodePath nodePathLoginExisting;
        [Export] private readonly NodePath nodePathLoginNew;
        [Export] private readonly NodePath nodePathInputUsername;
        [Export] private readonly NodePath nodePathInputPassword;
        [Export] private readonly NodePath nodePathLabelResponse;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static string WebServerIp { get; set; }
        public static ushort WebServerPort { get; set; }
        private static string Token { get; set; }
        private static bool AttemptedToRenewInvalidToken { get; set; }

        private static Label LoginExisting { get; set; }
        private static Control LoginNew { get; set; }
        private static LineEdit InputUsername { get; set; }
        private static LineEdit InputPassword { get; set; }
        private static Label LoginResponse { get; set; }
        private static SceneTree Tree { get; set; }

        public override void _Ready()
        {
            Tree = GetTree();
            WebServerIp = webServerIp;
            WebServerPort = webServerPort;

            LoginExisting = GetNode<Label>(nodePathLoginExisting);
            LoginNew = GetNode<Control>(nodePathLoginNew);
            InputUsername = GetNode<LineEdit>(nodePathInputUsername);
            InputPassword = GetNode<LineEdit>(nodePathInputPassword);
            LoginResponse = GetNode<Label>(nodePathLabelResponse);

            Init();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                if (!LoginNew.Visible)
                    ShowLoginNew();
                else
                    Tree.ChangeScene("res://Scenes/MainMenu.tscn");
            }
        }

        public static void LoadGameScene() 
        {
            GD.Print("Loading game scene");
            Tree.ChangeScene("res://Scenes/Game.tscn");
        }

        public static void Init()
        {
            UpdateResponse("");

            var contents = AppData.GetStorage();
            if (contents == null || contents["token"] == null)
                ShowLoginNew();
            else
                ShowLoginExisting(contents["username"]);
        }

        private void _on_Login_pressed() => Login();

        private static void ShowLoginNew()
        {
            LoginExisting.Visible = false;
            LoginNew.Visible = true;
        }

        private static void ShowLoginExisting(string username)
        {
            LoginExisting.Visible = true;
            LoginNew.Visible = false;

            if (InputUsername.Text != "") 
            {
                LoginExisting.Text = $"Connect as {InputUsername.Text}";
            }
            else 
            {
                InputUsername.Text = username;
                LoginExisting.Text = $"Connect as {username}";
            }
        }

        private static void ResetToken() 
        {
            AppData.SaveJsonWebToken(null, "");
            Token = null;
        }

        public static void UpdateResponse(string text) => LoginResponse.Text = text;

        private async void Login()
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

            //GD.Print("Web Response: " + webResponse.Message);

            var res = JsonConvert.DeserializeObject<WebPostLoginResponse>(webResponse.Message);

            UpdateResponse(res.Message);

            switch ((LoginOpcode)res.Opcode)
            {
                case LoginOpcode.AccountDoesNotExist:
                case LoginOpcode.InvalidUsernameOrPassword:
                case LoginOpcode.PasswordsDoNotMatch:
                    ShowLoginNew();
                    break;

                case LoginOpcode.TokenUsernameDoesNotMatchWithProvidedUsername:
                    ResetToken();
                    break;

                case LoginOpcode.InvalidToken:
                    ResetToken();

                    GD.Print("Invalid token, going to try relogging one more time...");
                    
                    // Automatically try to login one more time
                    if (!AttemptedToRenewInvalidToken)
                    {
                        AttemptedToRenewInvalidToken = true;
                        Login();
                    }
                    break;

                case LoginOpcode.LoginSuccess:
                    string token;
                    if (Token != null)
                    {
                        AppData.SaveJsonWebToken(Token, InputUsername.Text);
                        token = Token;
                    }
                    else
                    {
                        AppData.SaveJsonWebToken(res.Token, InputUsername.Text);
                        token = res.Token;
                    }

                    await Task.Delay(100); // Add a small delay of 100 ms to give application a chance to keep up (after all we did just do a POST request)

                    GD.Print("Attempting to connect to the game server...");

                    ENetClient.Connect(gameServerIp, gameServerPort, token);
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