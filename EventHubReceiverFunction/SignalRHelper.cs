using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubReceiverFunction
{
    class SignalRHelper
    {
        private static string CoreInteractSignalRHubUrl = ConfigurationManager.AppSettings["CoreInteractSignalRHubUrl"];
        private HubConnection _hub = new HubConnection(CoreInteractSignalRHubUrl);
        private IHubProxy _proxy;
        public void ConnectSignalRAndSendMessage()
        {

            try
            {
                if (_hub.State != ConnectionState.Connected)
                {
                    //Start SignalRProxy
                    _proxy = _hub.CreateHubProxy("ConversationHub");
                    _hub.Start();
                }
                _proxy.Invoke("SendPrivateMessage", "hello");
                _hub.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}