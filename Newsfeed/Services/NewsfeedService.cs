using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;

namespace Newsfeed.Services
{
    public class NewsfeedService : INewsfeedService
    {
        #region Construction
        public NewsfeedService()
        {
            this.callback = OperationContext.Current.GetCallbackChannel<INewsfeedServiceCallback>();
        }
        #endregion

        #region INewsfeedService implementation
        public void Recieve(Message message)
        {
            if (!message.IsEmpty)
            {
                var msgBytes = message.GetBody<byte[]>();
                var content = Encoding.UTF8.GetString(msgBytes);

                this.callback.Send(this.CreateMessage("Recieved message on server: " + content));
            }
            else
            {
                this.callback.Send(this.CreateMessage("Connection opened!"));
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
        private INewsfeedServiceCallback callback;
        #endregion
    }
}