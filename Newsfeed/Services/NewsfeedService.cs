using System;
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
                var content = manager.ProcessMessage(message);               

                foreach (var client in ClientsManager.Instance.Clients)
                {
                    try
                    {
                        client.Value.Send(
                            manager.CreateMessage(content));
                    }
                    catch (ObjectDisposedException ex)
                    {
                        ClientsManager.Instance.RemoveClient(client.Key);
                    }
                }                
            }
            else //New connection has been created and this is her opening message
            {
                var hello = new Model.Message() 
                {
                    Text = "Welcome to the newsfeed!",
                    SentDate = DateTime.Now
                };
                this.currentClient.Send(manager.CreateMessage(hello));

                var messagesRepo = new Domain.MessageRepository();
                var messages = messagesRepo.GetLatestMessages(0, 20);

                foreach (var m in messages)
                {
                    var uiMessage = manager.MapDomainMessageToClient(m);
                    this.currentClient.Send(manager.CreateMessage(uiMessage));
                }
            }
        }
        #endregion

        #region Private fields and constants
        private readonly INewsfeedServiceCallback currentClient;
        #endregion
    }
}