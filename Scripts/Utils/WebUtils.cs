using Godot;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;

namespace KRU.Utils 
{
    public static class WebUtils 
    {
        private static readonly HttpClient webClient = new HttpClient();
        private static bool SendingRequest { get; set; }

        public static async Task<string> GetRequest(string path)
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

        public static async Task<WebPostResponse> PostRequest(string path, string content)
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
                        GD.Print("STATUS CODE: " + response.StatusCode);

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
}