define(["knockout", "knockoutMap"], function (ko, koMap) {
    var NewsfeedViewModel = function (client) {
        this.client = client;

        //we store the ids of the messages in another collection to simulate a dictionary for fast access
        //since knockout looks like doesn't suport it
        this.messagesKeys = [];
        this.messages = ko.observableArray();
        
        this.message = null;

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
            message.Likes++;
            this.client.send(message);
        },

        send: function () {
            var message = { Text: this.message };
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
                this.messages.push(observable);
            }          
        }
    };

    return NewsfeedViewModel;
});