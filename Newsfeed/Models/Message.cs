using System;
using System.Linq;

namespace Newsfeed.Models
{
    public class Message
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the likes.
        /// </summary>
        /// <value>The likes.</value>
        public int Likes { get; set; }

        /// <summary>
        /// Gets or sets the sent date.
        /// </summary>
        /// <value>The sent date.</value>
        public DateTime SentDate { get; set; }

        /// <summary>
        /// Gets or sets the sender id.
        /// </summary>
        /// <value>The sender id.</value>
        public string SenderId { get; set; }

        /// <summary>
        /// Gets or sets the action to which the server should react.
        /// </summary>
        /// <value>The action.</value>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the number of displayed messages on the client.
        /// </summary>
        /// <value>The displayed messages count.</value>
        public int DisplayedMessages { get; set; }
    }
}