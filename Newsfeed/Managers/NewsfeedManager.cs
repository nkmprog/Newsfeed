using System;
using System.Linq;
using System.Net.WebSockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Newtonsoft.Json;
using Model = Newsfeed.Models;
using Domain = Newsfeed.Domain;
using MongoDB.Bson;

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
            var messagesRepo = new Domain.MessageRepository();

            var content = this.GetMessage(message);

            //a new message has been sent to the server
            if (string.IsNullOrEmpty(content.Id))
            {
                content.SentDate = DateTime.Now;

                //TODO: assign username
                content.Username = OperationContext.Current.SessionId;
                content.SenderId = ObjectId.GenerateNewId().ToString();

                //Save the message to the db
                var messageDto = MapClientMessageToDomain(content);

                messagesRepo.InsertMessage(messageDto);

                content.Id = messageDto.Id.ToString();
            }
            else //if(content.Likes > originalMessage.Likes)
            {
                //TODO: implement here the like and dislike logic
                //originalMessage.Likes++;
            }

            return content;
        }        

        public Domain.Message MapClientMessageToDomain(Model.Message message)
        {           
            var messageDto = new Domain.Message()
            {
                SentDate = message.SentDate,
                Likes = message.Likes,
                Text = message.Text,
                Author = new BsonDocument
                    {
                        {"Id", new ObjectId(message.SenderId)},
                        {"Username", message.Username}
                    }
            };

            if (message.Id != null)
                messageDto.Id = new ObjectId(message.Id);

            return messageDto;
        }

        public Model.Message MapDomainMessageToClient(Domain.Message message)
        {
            var model = new Model.Message()
            {
                Id = message.Id.ToString(),
                SentDate = message.SentDate,
                Text = message.Text,
                Likes = message.Likes,
                SenderId = message.Author.GetValue("Id").ToString(),
                Username = message.Author.GetValue("Username").ToString()
            };

            return model;
        }
        #endregion
    }
}