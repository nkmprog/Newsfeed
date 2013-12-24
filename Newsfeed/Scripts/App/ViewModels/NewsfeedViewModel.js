define(["knockout"], function (ko) {
    var NewsfeedViewModel = function (client) {
        this.client = client;

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

        send: function () {
            var message = { Text: this.message };
            this.client.send(message);
        },

        onMessage: function (message) {
            this.messages.push(message);
        }
    };

    return NewsfeedViewModel;
});