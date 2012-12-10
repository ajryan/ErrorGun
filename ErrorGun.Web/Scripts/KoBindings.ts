/// <reference path="knockout-2.2.d.ts" />
/// <reference path="jquery-1.8.d.ts" />

ko.bindingHandlers["fadeVisible"] = {
    init: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var visible = $(element).is(':visible');
        if ((value && !visible) || (!value && visible)) {
            $(element).toggle(value);
        }
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        value ? $(element).fadeIn() : $(element).fadeOut();
    }
};