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

        public void SaveMessage(Model.Message message)
        {
            var messagesRepo = new Domain.MessageRepository();

            //a new message has been sent to the server
            message.SentDate = DateTime.Now;

            //TODO: assign username
            message.Username = OperationContext.Current.SessionId;
            message.SenderId = ObjectId.GenerateNewId().ToString();

            //Save the message to the db
            var messageDto = MapClientMessageToDomain(message);

            messagesRepo.InsertMessage(messageDto);

            message.Id = messageDto.Id.ToString();
        }

        public void LikeMessage(Model.Message message)
        {
            if (!string.IsNullOrEmpty(message.Id))
            {
                var repo = new Domain.MessageRepository();
                repo.UpdateMessageLikes(new ObjectId(message.Id), 1);
                message.Likes++;
            }
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