using System;
using System.Linq;
using System.ServiceModel.Channels;
using Newsfeed.Managers;
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

            if (!message.IsEmpty)
            {
                var content = manager.GetMessage(message);
                content.SentDate = DateTime.Now;

                foreach (var client in ClientsManager.Instance.Clients.Values)
                {
                    client.Send(
                        manager.CreateMessage(content));
                }                
            }
            else
            {
                var hello = new Model.Message() 
                {
                    Text = "Welcome to the newsfeed!",
                    SentDate = DateTime.Now
                };
                this.currentClient.Send(manager.CreateMessage(hello));
            }
        }
        #endregion

        #region Private fields and constants
        private readonly INewsfeedServiceCallback currentClient;
        #endregion
    }
}