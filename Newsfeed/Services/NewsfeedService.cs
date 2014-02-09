using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Newsfeed.Managers;
using Domain = Newsfeed.Domain;
using Model = Newsfeed.Models;

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
            var manager = new NewsfeedManager();

            if (!message.IsEmpty) //New message is recieved
            {
                var content = manager.GetMessage(message);               

                var action = (ServiceAction)Enum.Parse(typeof(ServiceAction), content.Action);

                switch (action)
                {
                    case ServiceAction.ShowMore:
                        var messagesRepo = new Domain.MessageRepository();
                        var messages = messagesRepo.GetLatestMessages(content.DisplayedMessages, 20);
                        this.SendOlderMessagesToClient(messages, manager);
                        break;
                    case ServiceAction.NewMessage:
                        manager.SaveMessage(content);
                        this.BroadcastMessage(manager.CreateMessage(content));
                        break;
                    case ServiceAction.LikeMessage:
                        manager.LikeMessage(content);
                        this.BroadcastMessage(manager.CreateMessage(content));
                        break;
                    default:
                        break;
                }                            
            }
            else //New connection has been created and this is her opening message
            {
                var messagesRepo = new Domain.MessageRepository();
                var messages = messagesRepo.GetLatestMessages(0, 20);

                this.SendOlderMessagesToClient(messages, manager);

                var hello = new Model.Message()
                {
                    Text = "Welcome to the newsfeed!",
                    SentDate = DateTime.Now
                };
                this.currentClient.Send(manager.CreateMessage(hello));
            }
        }
        #endregion

        #region Private methods
        private void BroadcastMessage(Message message)
        {
            foreach (var client in ClientsManager.Instance.Clients)
            {
                try
                {                    
                    client.Value.Send(message);
                }
                //TODO Exception handling
                catch (ObjectDisposedException ex)
                {
                    //TODO: this will modify the collection and will throw exception
                    //ClientsManager.Instance.RemoveClient(client.Key);
                }
                catch
                {

                }
            }
        }

        private void SendOlderMessagesToClient(IEnumerable<Domain.Message> messages, NewsfeedManager manager)
        {
            foreach (var m in messages)
            {
                var uiMessage = manager.MapDomainMessageToClient(m);
                uiMessage.Action = ServiceAction.ShowMore.ToString();
                this.currentClient.Send(manager.CreateMessage(uiMessage));
            }
        }
        #endregion

        #region Private fields and constants
        private readonly INewsfeedServiceCallback currentClient;
        #endregion
    }
}