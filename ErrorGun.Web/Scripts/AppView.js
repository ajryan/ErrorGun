var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ErrorGun;
(function (ErrorGun) {
    (function (ViewModels) {
        var AppView = (function (_super) {
            __extends(AppView, _super);
            function AppView() {
                var _this = this;
                        _super.call(this);
                this.ApiKey = ko.observable("");
                this.AppLoaded = ko.observable(false);
                this.Working = ko.observable(false);
                this.ReportedErrorId = ko.observable("");
                this.ErrorMessageSend = ko.observable("");
                this.Message = ko.observable("");
                this.Detail = ko.observable("");
                this.Category = ko.observable("");
                this.Source = ko.observable("");
                this.UserEmail = ko.observable("");
                this.ErrorReports = ko.observableArray([]);
                this.ContactEmailsFlat = ko.computed(function () {
                    return _this.ContactEmails().join(", ");
                });
                this.LoadApp = function () {
                    if(_this._loadedApiKey === _this.ApiKey()) {
                        return;
                    }
                    _this.AppLoaded(false);
                    $.getJSON('/api/apps', {
                        apiKey: _this.ApiKey()
                    }).fail(function (jqXHR) {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR.responseText);
                        _this.ErrorMessage(errorMessage);
                    }).done(function (ajaxData) {
                        _this._loadedApiKey = ajaxData.ApiKey;
                        _this.Id(ajaxData.Id);
                        _this.Name(ajaxData.Name);
                        _this.CreatedTimestampUtc(moment(ajaxData.CreatedTimestampUtc).format('YYYY MMM DD hh:mm A'));
                        _this.ContactEmails(ajaxData.ContactEmails);
                        _this.ErrorMessage("");
                        _this.AppLoaded(true);
                    });
                };
                this.LoadErrors = function () {
                    if(_this.AppLoaded() !== true) {
                        return;
                    }
                    $.getJSON('/api/errors', {
                        apiKey: _this.ApiKey()
                    }).fail(function (jqXHR) {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR.responseText);
                        _this.ErrorMessage(errorMessage);
                    }).done(function (ajaxData) {
                        _this.ErrorReports(ajaxData);
                    });
                };
                this.SendTestErrorReport = function () {
                    if(_this.AppLoaded() !== true) {
                        return;
                    }
                    _this.ReportedErrorId("");
                    var json = {
                        AppId: _this.Id(),
                        ApiKey: _this.ApiKey(),
                        Message: _this.Message(),
                        Detail: _this.Detail(),
                        Category: _this.Category(),
                        Source: _this.Source(),
                        UserEmail: _this.UserEmail()
                    };
                    $.ajax({
                        url: "/api/errors?apiKey=" + _this.ApiKey(),
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(json)
                    }).fail(function (jqXHR) {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR.responseText);
                        _this.ErrorMessageSend(errorMessage);
                    }).done(function (ajaxData) {
                        _this.ErrorMessageSend("");
                        _this.ReportedErrorId(ajaxData.Id);
                    });
                };
                this.ClearErrorReport = function () {
                    _this.Message("");
                    _this.Detail("");
                    _this.Category("");
                    _this.Source("");
                    _this.UserEmail("");
                };
            }
            return AppView;
        })(ViewModels.App);
        ViewModels.AppView = AppView;        
    })(ErrorGun.ViewModels || (ErrorGun.ViewModels = {}));
    var ViewModels = ErrorGun.ViewModels;
})(ErrorGun || (ErrorGun = {}));
