using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Web;
using Newsfeed.Domain;
using Newsfeed.Services;

namespace Newsfeed.Managers
{
    /// <summary>
    /// Adds client information to an INewsfeedServiceCallback instance
    /// </summary>
    public class ChannelWrapper
    {
        #region Construction
        public ChannelWrapper(string username, INewsfeedServiceCallback callback)
        {
            this.callback = callback;
            this.Username = username;

            this.OriginalChannel.Closed += channel_Closed;
            this.OriginalChannel.Faulted += channel_Faulted;
        }     
        #endregion

        #region Wrapper public properties
        public string Username { get; private set; }

        public User User { get; set; }

        public IChannel OriginalChannel
        {
            get
            {
                return (IChannel)this.callback;
            }
        }

        public INewsfeedServiceCallback Callback
        {
            get
            {
                return this.callback;
            }
        }

        public bool ShouldTerminate { get; set; }
        #endregion

        #region Events
        public event EventHandler Closed;

        public event EventHandler Faulted;
        #endregion

        #region Private members
        void channel_Closed(object sender, EventArgs e)
        {
            var closed = this.Closed;
            if (closed != null)
            {
                closed(this, e);
            }
        }

        void channel_Faulted(object sender, EventArgs e)
        {
            var faulted = this.Faulted;
            if (faulted != null)
            {
                faulted(this, e);
            }
        }
        #endregion

        #region Private fields and constants
        private readonly INewsfeedServiceCallback callback;
        #endregion
    }
}