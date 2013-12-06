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
            this.client.send(this.message);
        },

        onMessage: function (e) {
            this.messages.push(e.data);
        }
    };

    return NewsfeedViewModel;
});