using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using Domain = Newsfeed.Domain;
using Newsfeed.Services;

namespace Newsfeed.Managers
{
    /// <summary>
    /// Manages all clients connected to the service
    /// </summary>
    public class ClientsManager
    {
        #region Construction
        private ClientsManager()
        {
            this.clients = new Dictionary<string, ChannelWrapper>();
            this.failedClients = new List<string>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the singleton instance of the manager.
        /// </summary>
        /// <value>The instance.</value>
        public static ClientsManager Instance
        { 
            get            
            {
                lock (ClientsManager.managerLock)
                {
                    if (ClientsManager.instance == null)
                    {
                        ClientsManager.instance = new ClientsManager();
                    }
                    return ClientsManager.instance;
                }
            }
        }

        /// <summary>
        /// Gets all clients connected to the service.
        /// </summary>
        /// <value>The clients.</value>
        public Dictionary<string, ChannelWrapper> Clients
        {
            get { return clients; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the current callback channel and adds it to the collection of all channels.
        /// </summary>
        /// <returns></returns>
        public ChannelWrapper RegisterClient(Message message)
        {
            var manager = new NewsfeedManager();

            string username;
            if (manager.TryGetCurrentUsername(message, out username))
            {
                ChannelWrapper wrapper;

                //If the user is logged and resfresh the page, a new channel will be opened and the old should be removed
                if (this.clients.TryGetValue(username, out wrapper))
                {
                    this.clients.Remove(username);
                }                

                var client = OperationContext.Current.GetCallbackChannel<INewsfeedServiceCallback>();                

                wrapper = new ChannelWrapper(username, client);
                wrapper.Closed += Connection_Closed;
                wrapper.Faulted += Connection_Closed;

                var usersRepository = new Domain.UserRepository();
                wrapper.User = usersRepository.Get(username);

                this.clients.Add(username, wrapper);

                return wrapper;
            }

            return null;
        }

        public void MarkAsFailed(string key)
        {
            this.failedClients.Add(key);
        }

        public void ClearFailed()
        {
            foreach (var client in this.failedClients)
            {
                this.clients.Remove(client);
            }
            this.failedClients.Clear();
        }
        #endregion

        #region Private methods
        private void Connection_Closed(object sender, EventArgs e)
        {
            var wrapper = (ChannelWrapper)sender;
            this.MarkAsFailed(wrapper.Username);           
        }
        #endregion

        #region Private fields and constants
        private static volatile ClientsManager instance;
        private readonly Dictionary<string, ChannelWrapper> clients;
        private readonly List<string> failedClients;
        private static readonly object managerLock = new object();
        #endregion
    }
}