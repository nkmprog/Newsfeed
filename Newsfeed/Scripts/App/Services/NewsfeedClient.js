define([], function () {
    var NewsfeedClient = function (wsUri) {
        this.client = new WebSocket(wsUri);
    };
    NewsfeedClient.prototype = {
        constructor: NewsfeedClient,

        send: function (message) {
            this.client.send(message);
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
                handler(e);
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