require(["knockout", "nfClient", "nfViewModel", "domReady!"],
    function (ko, NewsfeedClient, NewsfeedViewModel) {
        var host = location.host;
        var feedUrl = "ws://" + host + "/feed";

        var client = new NewsfeedClient(feedUrl);

        var vm = new NewsfeedViewModel(client);

        ko.applyBindings(vm);
    });