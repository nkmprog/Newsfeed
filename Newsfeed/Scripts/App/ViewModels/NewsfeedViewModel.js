define(["knockout", "knockoutMap"], function (ko, koMap) {
    var NewsfeedViewModel = function (client) {
        this.client = client;

        //we store the ids of the messages in another collection to simulate a dictionary for fast access
        //since knockout looks like doesn't suport it
        this.messagesKeys = [];
        this.messages = ko.observableArray();
        
        this.message = ko.observable();
        this.messagesListCapacity = 20;

        this.initialize();
    }
    NewsfeedViewModel.prototype = {
        initialize: function () {
            var self = this;

            this.client.onMessage(function (e) {
                self.onMessage(e);
            });
        },

        like: function (data, event) {
            var message = koMap.toJS(data);
            message.Action="LikeMessage",
            this.client.send(message);
        },

        send: function () {
            if (!this.message()) {
                return;
            }

            var message = {
                Text: this.message(),
                Action: "NewMessage"
            };
            this.client.send(message);

            this.message("");
        },

        sendKeypress: function (data, event) {
            if (event.keyCode == 13) {
                this.send();                
                return false;
            }
            return true;
        },

        showMore: function () {
            var message = {
                DisplayedMessages: this.messages().length,
                Action: "ShowMore"
            }

            this.client.send(message);
        },

        onMessage: function (message) {
            var originalMessage = this.messagesKeys[message.Id];
            if (originalMessage) {
                //update the message
                for (var property in message) {
                    if (originalMessage.hasOwnProperty(property)) {
                        originalMessage[property](message[property]);
                    }
                }
            }
            else {
                //add new message
                var observable = koMap.fromJS(message);
                this.messagesKeys[message.Id] = observable;

                var messageList = $("#messagesList");
                var isScrolledToBottom = messageList.scrollTop() + messageList.height() == messageList[0].scrollHeight;

                if (message.Action == "ShowMore") {
                    this.messages.unshift(observable);
                }
                else {
                    this.messages.push(observable);
                    if (this.messages().length > this.messagesListCapacity) {
                        this.messages.shift();
                    }                    
                }

                if (isScrolledToBottom) {
                    //Scroll to bottom only if the user has already scrolled to bottom
                    messageList.scrollTop(messageList.prop('scrollHeight'));
                }
            }          
        }
    };

    return NewsfeedViewModel;
});