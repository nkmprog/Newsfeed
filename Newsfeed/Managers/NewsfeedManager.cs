using System;
using System.Linq;
using System.Net.WebSockets;
using System.ServiceModel.Channels;
using System.Text;
using Newtonsoft.Json;
using Model = Newsfeed.Models;

namespace Newsfeed.Managers
{
    public class NewsfeedManager
    {
        #region Public methods
        public Message CreateMessage(Model.Message content)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            var message = ByteStreamMessage.CreateMessage(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonContent)));

            message.Properties["WebSocketMessageProperty"] = new WebSocketMessageProperty() { MessageType = WebSocketMessageType.Text };

            return message;
        }

        public Model.Message GetMessage(Message message)
        {
            var msgBytes = message.GetBody<byte[]>();
            var content = Encoding.UTF8.GetString(msgBytes);

            return JsonConvert.DeserializeObject<Model.Message>(content);
        }
        #endregion
    }
}