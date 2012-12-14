"use strict";
var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ErrorGun;
(function (ErrorGun) {
    (function (ViewModels) {
        var AppCreate = (function (_super) {
            __extends(AppCreate, _super);
            function AppCreate() {
                var _this = this;
                        _super.call(this);
                this.NewContactEmail = ko.observable("");
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
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR.responseText);
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
            return AppCreate;
        })(ViewModels.App);
        ViewModels.AppCreate = AppCreate;        
    })(ErrorGun.ViewModels || (ErrorGun.ViewModels = {}));
    var ViewModels = ErrorGun.ViewModels;
})(ErrorGun || (ErrorGun = {}));
