using System;
using System.Linq;
using System.Net.WebSockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Newsfeed.Managers;

namespace Newsfeed.Services
{
    public class NewsfeedService : INewsfeedService
    {
        #region Construction
        public NewsfeedService()
        {
            this.currentClient = ClientsManager.Instance.RegisterClient();
        }
        #endregion

        #region INewsfeedService implementation
        public void Recieve(Message message)
        {
            if (!message.IsEmpty)
            {
                var msgBytes = message.GetBody<byte[]>();
                var content = Encoding.UTF8.GetString(msgBytes);

                foreach (var client in ClientsManager.Instance.Clients.Values)
                {
                    client.Send(
                        this.CreateMessage(String.Format("Recieved message on server from {0}: ", OperationContext.Current.SessionId) + content));
                }                
            }
            else
            {
                this.currentClient.Send(this.CreateMessage("Connection opened!"));
            }
        }
        #endregion

        #region Private methods
        private Message CreateMessage(string content)
        {
            var message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(Encoding.UTF8.GetBytes(content)));

            message.Properties["WebSocketMessageProperty"] = new WebSocketMessageProperty() { MessageType = WebSocketMessageType.Text };

            return message;
        }
        #endregion

        #region Private fields and constants
        private readonly INewsfeedServiceCallback currentClient;
        #endregion
    }
}