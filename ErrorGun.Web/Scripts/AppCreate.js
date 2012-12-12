"use strict";
var ErrorGun;
(function (ErrorGun) {
    (function (AppCreate) {
        var ContactEmail = (function () {
            function ContactEmail(address, focused, canRemove) {
                if (typeof address === "undefined") { address = ""; }
                if (typeof focused === "undefined") { focused = false; }
                if (typeof canRemove === "undefined") { canRemove = true; }
                this.Address = ko.observable(address);
                this.Focused = ko.observable(focused);
                this.CanRemove = ko.observable(canRemove);
            }
            return ContactEmail;
        })();
        AppCreate.ContactEmail = ContactEmail;        
        var ViewModel = (function () {
            function ViewModel() {
                var _this = this;
                this.Id = ko.observable("");
                this.ApiKey = ko.observable("");
                this.CreatedTimestampUtc = ko.observable("");
                this.Name = ko.observable("");
                this.ContactEmails = ko.observableArray([
                    new ContactEmail("", false, false)
                ]);
                this.ErrorMessage = ko.observable("");
                this.Create = function () {
                    var $regButton = $('#registerButton');
                    $regButton.attr('disabled', true);
                    var removeEmails = $.grep(_this.ContactEmails(), function (item) {
                        return (item.CanRemove() && !/\S/.test(item.Address()));
                    });
                    _this.ContactEmails.removeAll(removeEmails);
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
                        $('input').attr('disabled', true);
                        _this.ErrorMessage("");
                        _this.ApiKey(ajaxData.ApiKey);
                        _this.Id(ajaxData.Id);
                    });
                };
                this.AddContactEmail = function () {
                    _this.ContactEmails.push(new ContactEmail("", true, true));
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
