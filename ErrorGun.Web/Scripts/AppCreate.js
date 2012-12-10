var ErrorGun;
(function (ErrorGun) {
    (function (AppCreate) {
        var ViewModel = (function () {
            function ViewModel(completeUrl) {
                var _this = this;
                this.Id = ko.observable("");
                this.ApiKey = ko.observable("");
                this.CreatedTimestampUtc = ko.observable("");
                this.Name = ko.observable("");
                this.ContactEmails = ko.observableArray([
                    {
                        Address: ko.observable("")
                    }
                ]);
                this.ErrorMessage = ko.observable("");
                this._errorCodes = new ErrorGun.ErrorCodes();
                this._completeUrl = completeUrl;
                this.Create = function () {
                    var $regButton = $('#registerButton');
                    _this.ErrorMessage("");
                    $regButton.attr('disabled', true);
                    var json = ko.toJSON(_this);
                    $.ajax({
                        url: "/api/apps",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: json
                    }).fail(function (jqXHR, textStatus) {
                        var responseJson = JSON.parse(jqXHR.responseText);
                        var errorMessages = [];
                        responseJson.ErrorCodes.forEach(function (errorCode) {
                            errorMessages.push(ErrorGun.ErrorCodes.MessageMap[errorCode]);
                        });
                        var errorMessage = errorMessages.join("\n");
                        _this.ErrorMessage(errorMessage);
                        $regButton.removeAttr('disabled');
                    }).done(function (ajaxData) {
                        _this.ApiKey(ajaxData.ApiKey);
                        _this.Id(ajaxData.Id);
                    });
                };
                this.AddContactEmail = function () {
                    _this.ContactEmails.push({
                        Address: ko.observable("")
                    });
                };
                this.RemoveContactEmail = function (email) {
                    if(_this.ContactEmails().length > 1) {
                        _this.ContactEmails.remove(email);
                    }
                };
                this.toJSON = function () {
                    return {
                        Name: _this.Name(),
                        ContactEmails: ko.utils.arrayMap(_this.ContactEmails(), function (ce) {
                            return ce.Address();
                        })
                    };
                };
            }
            return ViewModel;
        })();
        AppCreate.ViewModel = ViewModel;        
    })(ErrorGun.AppCreate || (ErrorGun.AppCreate = {}));
    var AppCreate = ErrorGun.AppCreate;
})(ErrorGun || (ErrorGun = {}));
