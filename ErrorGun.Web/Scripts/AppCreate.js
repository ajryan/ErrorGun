"use strict";
var ErrorGun;
(function (ErrorGun) {
    (function (AppCreate) {
        var ViewModel = (function () {
            function ViewModel() {
                var _this = this;
                this.Id = ko.observable("");
                this.ApiKey = ko.observable("");
                this.CreatedTimestampUtc = ko.observable("");
                this.Name = ko.observable("");
                this.NewContactEmail = ko.observable("");
                this.ContactEmails = ko.observableArray([]);
                this.ErrorMessage = ko.observable("");
                this.NewContactEmailValid = ko.computed(function () {
                    var newEmail = _this.NewContactEmail();
                    if(newEmail == null || newEmail.length == 0) {
                        return true;
                    }
                    var emailRegex = /^[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;
                    return emailRegex.test(newEmail);
                });
                this.Create = function () {
                    var $regButton = $('#registerButton');
                    $regButton.attr('disabled', true);
                    _this.NewContactEmail("");
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
                            errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(responseJson.ErrorCodes);
                        } catch (error) {
                            errorMessage = "An unexpected server error occurred.";
                        }
                        _this.ErrorMessage(errorMessage);
                        $regButton.removeAttr('disabled');
                    }).done(function (ajaxData) {
                        $('input, button').attr('disabled', true);
                        _this.ErrorMessage("");
                        _this.ApiKey(ajaxData.ApiKey);
                        _this.Id(ajaxData.Id);
                    });
                };
                this.AddContactEmail = function () {
                    if(!_this.NewContactEmailValid()) {
                        return;
                    }
                    _this.ContactEmails.push(_this.NewContactEmail());
                    _this.NewContactEmail("");
                    $('#emailAddress').focus();
                };
                this.AddContactEmailOnEnter = function (data, event) {
                    var keyCode = (event.which ? event.which : event.keyCode);
                    if(keyCode === 13) {
                        _this.AddContactEmail();
                        return false;
                    }
                    return true;
                };
                this.RemoveContactEmail = function (email) {
                    _this.ContactEmails.remove(email);
                    _this.NewContactEmail(email);
                    $('#emailAddress').focus();
                };
                this.toJSON = function () {
                    return {
                        Name: _this.Name(),
                        ContactEmails: _this.ContactEmails()
                    };
                };
            }
            return ViewModel;
        })();
        AppCreate.ViewModel = ViewModel;        
    })(ErrorGun.AppCreate || (ErrorGun.AppCreate = {}));
    var AppCreate = ErrorGun.AppCreate;
})(ErrorGun || (ErrorGun = {}));
