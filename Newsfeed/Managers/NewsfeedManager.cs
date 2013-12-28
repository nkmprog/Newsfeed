using System;
using System.Linq;
using System.Net.WebSockets;
using System.ServiceModel;
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

        public Model.Message ProcessMessage(Message message)
        {
            var content = this.GetMessage(message);

            //a new message has been sent to the server
            if (string.IsNullOrEmpty(content.Id))
            {
                content.SentDate = DateTime.Now;

                //TODO: assign username
                content.Username = OperationContext.Current.SessionId;
                
                //TODO: assign id
                content.Id = Guid.NewGuid().ToString();
            }
            else //if(content.Likes > originalMessage.Likes)
            {
                //TODO: implement here the like and dislike logic
                //originalMessage.Likes++;
            }

            return content;
        }
        #endregion        
    }
}