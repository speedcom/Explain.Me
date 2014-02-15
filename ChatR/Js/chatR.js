// Namespace
var chatR = {};

// Models
chatR.chatMessage = function(sender, content, dateSent) {
    var self = this;
    self.username = sender;
    self.content = content;
    if (dateSent != null) {
        self.timestamp = dateSent;
    }
};

chatR.user = function (username, userId) {
    var self = this;
    self.username = username;
    self.id = userId;
};

chatR.userRoom = function (user, userId) {
    var self = this;
    self.username = user[0];
    self.roomname = user[1];
    self.id = userId;
};

// ViewModels
chatR.userRoomViewModel = function(username, roomname) {
    var self = this;
    self.username = ko.observable(username);
    self.roomname = ko.observable(roomname);
};

chatR.chatViewModel = function() {
    var self = this;
    self.messages = ko.observableArray();
};

chatR.connectedUsersViewModel = function() {
    var self = this;
    self.contacts = ko.observableArray();
    self.customRemove = function(userToRemove) {
        var userIdToRemove = userToRemove.id;
        self.contacts.remove(function(item) {
            return item.id === userIdToRemove;
        });
    };
};