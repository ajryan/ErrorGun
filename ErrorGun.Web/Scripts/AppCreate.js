"use strict";
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
                        Address: ko.observable(""),
                        Focused: false
                    }
                ]);
                this.ErrorMessage = ko.observable("");
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
                        var errorMessage = "";
                        try  {
                            var responseJson = JSON.parse(jqXHR.responseText);
                            var errorMessages = [];
                            $.each(responseJson.ErrorCodes, function (i, errorCode) {
                                errorMessages.push(ErrorGun.ErrorCodes.MessageMap[errorCode]);
                            });
                            errorMessage = errorMessages.join("\n");
                        } catch (error) {
                            errorMessage = "An unexpected server error occurred.";
                        }
                        _this.ErrorMessage(errorMessage);
                        $regButton.removeAttr('disabled');
                    }).done(function (ajaxData) {
                        $('input').attr('disabled', true);
                        _this.ApiKey(ajaxData.ApiKey);
                        _this.Id(ajaxData.Id);
                    });
                };
                this.AddContactEmail = function () {
                    var contactCount = _this.ContactEmails().length;
                    if(_this.ContactEmails()[contactCount - 1].Address() === '') {
                        return false;
                    }
                    _this.ContactEmails.push({
                        Address: ko.observable(""),
                        Focused: true
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
