using System.IO;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using KRU.Networking;
using KRU.IO;

namespace KRU.UI
{
    public class UILogin : Control
    {
        [Export] private NodePath nodePathHttp, nodePathInputUsername, nodePathInputPassword, nodePathLoginAsSection, nodePathLoginSection, nodePathBtnLogout, nodePathLabelResponse;
        private static HTTPRequest httpRequest;
        private static Button btnLogout;
        private static LineEdit inputUsername, inputPassword;
        private static Label loginAsSection, labelResponse;
        private static VBoxContainer loginSection;
        private static ENetClient ENetClient;
        private static Dictionary<string, string> theContents; // token contents

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

            ENetClient = GetNode<ENetClient>("/root/ENetClient");
        }

        public static void InitLoginSection()
        {
            UpdateResponse("");

            var contents = AppData.GetJsonWebToken();
            if (contents == null || contents["token"] == "")
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
            AppData.SaveJsonWebToken("", "");
            theContents = null;
            HideConnectAsSection();
        }

        private void _on_Btn_Login_pressed()
        {
            var contents = AppData.GetJsonWebToken();

            Dictionary<string, string> data_to_send;
            if (contents == null || contents["token"] == "")
            {
                data_to_send = new Dictionary<string, string>{
                    { "username", inputUsername.Text },
                    { "password", inputPassword.Text },
                    { "from", "Godot-Client"}
                };
            }
            else
            {
                theContents = contents;
                data_to_send = new Dictionary<string, string>{
                    { "token", contents["token"] },
                    { "from", "Godot-Client" }
                };
            }

            string[] headers = new string[] { "Content-Type: application/json" };

            string query = JSON.Print(data_to_send);

            var error = httpRequest.Request("http://127.0.0.1:4000/api/login", headers, false, HTTPClient.Method.Post, query);

            if (error != Error.Ok)
            {
                GD.Print(error);
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

        private void OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
        {
            if (response_code == 429)
            {
                UpdateResponse("Please wait a bit before making another request");
                return;
            }
            if (response_code == 404)
            {
                UpdateResponse("Web server responded with 404"); // page not found
                return;
            }

            var json = System.Text.Encoding.UTF8.GetString(body);
            var res = JsonConvert.DeserializeObject<WebPostLoginResponse>(json);

            switch ((LoginOpcode)res.opcode)
            {
                case LoginOpcode.AccountDoesNotExist:
                case LoginOpcode.InvalidUsernameOrPassword:
                case LoginOpcode.PasswordsDoNotMatch:
                    UpdateResponse(res.message);
                    break;
                case LoginOpcode.InvalidToken:
                    UpdateResponse(res.message);
                    Logout();
                    break;
                case LoginOpcode.LoginSuccess:
                    UpdateResponse(res.message);
                    if (theContents != null && theContents["token"] != "")
                    {
                        ENetClient.JsonWebToken = theContents["token"];
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

