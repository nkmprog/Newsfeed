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
        }
        #endregion

        #region INewsfeedService implementation
        public void Recieve(Message message)
        {
            var manager = new NewsfeedManager();

            string username;
            if (!manager.TryGetCurrentUsername(message, out username) || !manager.ValidateSameOrigin(message))
            {
                //If the sender is not logged user or the message comes from different domain do nothing
                return;
            }

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
                        manager.SaveMessage(content, username);
                        this.BroadcastMessage(content);
                        break;
                    case ServiceAction.LikeMessage:
                        manager.LikeMessage(content);
                        this.BroadcastMessage(content);
                        this.SendLikeNotification(content, username);
                        break;
                    default:
                        break;
                }                            
            }
            else //New connection has been created and this is her opening message
            {
                //Associate the opened channel with the logged user
                this.currentClient = ClientsManager.Instance.RegisterClient(message);

                var messagesRepo = new Domain.MessageRepository();
                var messages = messagesRepo.GetLatestMessages(0, 20);

                this.SendOlderMessagesToClient(messages, manager);

                var hello = new Model.Message()
                {
                    Action = ServiceAction.Notification.ToString(),
                    Text = "Welcome to the newsfeed!",
                    SentDate = DateTime.Now
                };
                this.currentClient.Send(manager.CreateMessage(hello));
            }
        }        
        #endregion

        #region Private methods
        private void BroadcastMessage(Model.Message message)
        {
            //Prevent modifying the collection when someone is sending message
            lock (NewsfeedService.clientsLock)
            {
                var manager = new NewsfeedManager();
                foreach (var client in ClientsManager.Instance.Clients)
                {
                    try
                    {
                        client.Value.Callback.Send(manager.CreateMessage(message));
                    }
                    catch
                    {
                    }
                }
                ClientsManager.Instance.ClearFailed();
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

        private void SendLikeNotification(Model.Message content, string username)
        {
            var notification = new Model.Message()
            {
                Action = ServiceAction.Notification.ToString(),
                Text = String.Format("{0} liked your message: {1}", username, content.Text),
                SentDate = DateTime.Now
            };

            var manager = new NewsfeedManager();
            var notificationMessage = manager.CreateMessage(notification);

            ChannelWrapper client;
            if (ClientsManager.Instance.Clients.TryGetValue(content.Username, out client))
            {
                //TODO: save the notifcation in the database so the user can recieve it later when he is logged;
                client.Callback.Send(notificationMessage);
            }
        }
        #endregion

        #region Private fields and constants
        private INewsfeedServiceCallback currentClient;
        private static object clientsLock = new object();
        #endregion
    }
}