define([], function () {
    var NewsfeedClient = function (wsUri) {
        this.client = new WebSocket(wsUri);
    };
    NewsfeedClient.prototype = {
        constructor: NewsfeedClient,

        send: function (message) {
            var content = JSON.stringify(message);
            this.client.send(content);
        },

        onOpen: function (handler) {
            this.client.onopen = function (e) {
                handler(e);
            }
        },

        onClose: function (handler) {
            this.client.onclose = function (e) {
                handler(e);
            }
        },

        onMessage: function (handler) {
            this.client.onmessage = function (e) {
                var message = JSON.parse(e.data);
                handler(message);
            }
        },

        onError: function (handler) {
            this.client.onerror = function (e) {
                handler(e);
            }
        }
    };

    return NewsfeedClient;    
});