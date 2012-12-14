var ErrorGun;
(function (ErrorGun) {
    (function (ViewModels) {
        var App = (function () {
            function App() {
                this.Id = ko.observable("");
                this.Name = ko.observable("");
                this.ContactEmails = ko.observableArray([]);
                this.ApiKey = ko.observable("");
                this.CreatedTimestampUtc = ko.observable("");
                this.ErrorMessage = ko.observable("");
            }
            return App;
        })();
        ViewModels.App = App;        
    })(ErrorGun.ViewModels || (ErrorGun.ViewModels = {}));
    var ViewModels = ErrorGun.ViewModels;
})(ErrorGun || (ErrorGun = {}));
