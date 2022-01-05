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
                    UIMainMenu.LoadMainMenu();
            }
        }

        private void _on_Login_pressed() => Login();

        public static void Init()
        {
            UpdateResponse("");

            var contents = AppData.GetLoginInfo();
            if (contents == null || contents["token"] == null)
                ShowLoginNew();
            else
                ShowLoginExisting(contents["username"], contents["password"]);
        }

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
                    ResetLoginInfo();
                    break;

                case LoginOpcode.InvalidToken:
                    ResetLoginInfo();
                    //GD.Print("Invalid token");
                    break;

                case LoginOpcode.LoginSuccess:
                    string token;
                    if (Token != null)
                    {
                        AppData.SaveLoginInfo(Token, InputUsername.Text, InputPassword.Text);
                        token = Token;
                    }
                    else
                    {
                        AppData.SaveLoginInfo(res.Token, InputUsername.Text, InputPassword.Text);
                        token = res.Token;
                    }

                    await Task.Delay(100); // Add a small delay of 100 ms to give application a chance to keep up (after all we did just do a POST request)

                    ENetClient.Connect(gameServerIp, gameServerPort, token);
                    break;
            }
        }

        private static WebPostLoginContent GetLoginInfo()
        {
            if (!AppData.LoginInfoFileExist()) 
            {
                // Token does not exist, lets create a request to the web server for a new one
                //GD.Print("Token does not exist in local file system");
                return BasicLoginInfo();
            }

            if (AppData.GetLoginInfo() == null)
            {
                // Invalid JSON
                //GD.Print("Token.json is invalid");
                return BasicLoginInfo();
            }

            if (InputUsername.Text != AppData.GetLoginInfo()["username"])
            {
                // A new username was entered that was different from the one in storage, asking for a new JWT
                //GD.Print("A new username was entered that was different from the one in storage, asking for a new JWT");
                return BasicLoginInfo();
            }

            Token = AppData.GetLoginInfo()["token"];

            if (Token == null)
            {
                // Token is null in JSON
                //GD.Print("Token in token.json is null");
                return BasicLoginInfo();
            }

            // Token exists in local file system
            //GD.Print("Token exists in local file system");
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

        public static void LoadGameScene() 
        {
            //GD.Print("Loading game scene");
            Tree.ChangeScene("res://Scenes/Main/Game.tscn");
        }

        private static void ShowLoginNew()
        {
            LoginExisting.Visible = false;
            LoginNew.Visible = true;
        }

        private static void ShowLoginExisting(string username, string password)
        {
            LoginExisting.Visible = true;
            LoginNew.Visible = false;

            if (InputUsername.Text != "") 
                LoginExisting.Text = $"Connect as {InputUsername.Text}";
            else 
            {
                InputUsername.Text = username;
                LoginExisting.Text = $"Connect as {username}";
            }

            if (!string.IsNullOrEmpty(password))
                InputPassword.Text = password;
        }

        private static void ResetLoginInfo() 
        {
            AppData.SaveLoginInfo(null, "", "");
            Token = null;
        }

        public static void UpdateResponse(string text) => LoginResponse.Text = text;
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