/// <reference path="defs\knockout-2.2.d.ts" />
/// <reference path="defs\jquery-1.8.d.ts" />
/// <reference path="defs\moment-1.7.d.ts" />
/// <reference path="ErrorCodes.ts" />
/// <reference path="App.ts" />

module ErrorGun {
    export module ViewModels {

        export class AppView extends App {

            // properties
            public ApiKey = ko.observable("");
            public AppLoaded = ko.observable(false);
            public Working = ko.observable(false);
            public ReportedErrorId = ko.observable("");
            public ContactEmailsFlat: KnockoutComputed;
            public ErrorMessageSend = ko.observable("");
            public Message = ko.observable("");
            public Detail = ko.observable("");
            public Category = ko.observable("");
            public Source = ko.observable("");
            public UserEmail = ko.observable("");
            public ErrorReportPage = ko.observable(1);
            public ErrorReportPageCount = ko.observable(0);
            public ErrorReports = ko.observableArray([]);

            // fields
            private _loadedApiKey: string;

            // methods
            public LoadApp: () => void;
            public LoadErrors: () => void;
            public SendTestErrorReport: () => void;
            public ClearErrorReport: () => void;
            public PrevErrorReportPage: () => void;
            public NextErrorReportPage: () => void;

            constructor() {
                super();

                this.ContactEmailsFlat = ko.computed(() => {
                    return this.ContactEmails().join(", ");
                });

                this.LoadApp = () => {
                    if (this._loadedApiKey === this.ApiKey())
                        return;

                    this._loadedApiKey = "";
                    this.AppLoaded(false);

                    $.getJSON(
                        '/api/apps',
                        { apiKey: this.ApiKey() }
                    )
                    .fail((jqXHR: JQueryXHR) => {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR);
                        this.ErrorMessage(errorMessage);
                    })
                    .done((ajaxData) => {
                        this._loadedApiKey = ajaxData.ApiKey;
                        this.Id(ajaxData.Id);
                        this.Name(ajaxData.Name);
                        this.CreatedTimestampUtc( moment(ajaxData.CreatedTimestampUtc).format('YYYY MMM DD hh:mm A') );
                        this.ContactEmails(ajaxData.ContactEmails);

                        this.ErrorMessage("");
                        this.AppLoaded(true);
                    });
                };

                this.LoadErrors = () => {
                    if (this.AppLoaded() !== true)
                        return;

                    $.getJSON(
                        '/api/errors',
                        { 
                            apiKey: this.ApiKey(),
                            pageIndex: this.ErrorReportPage() - 1,
                            pageSize: 4
                        }
                    )
                    .fail((jqXHR: JQueryXHR) => {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR);
                        this.ErrorMessage(errorMessage);
                    })
                    .done((ajaxData) => {
                        this.ErrorReportPageCount(ajaxData.PageCount);

                        // pretty-format dates
                        $.each(ajaxData.Items, (i, item) => {
                            item.ReportedTimestampUtc = moment(item.ReportedTimestampUtc).format('YYYY MMM DD hh:mm A');
                        });

                        this.ErrorReports(ajaxData.Items);
                    });
                };

                this.SendTestErrorReport = () => {
                    if (this.AppLoaded() !== true)
                        return;

                    this.ReportedErrorId("");

                    var json = {
                        AppId: this.Id(),
                        ApiKey: this.ApiKey(),
                        Message: this.Message(),
                        Detail: this.Detail(),
                        Category: this.Category(),
                        Source: this.Source(),
                        UserEmail: this.UserEmail()
                    };
                    $.ajax({
                        url: "/api/errors?apiKey=" + this.ApiKey(),
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(json)
                    })
                    .fail((jqXHR: JQueryXHR) => {
                        var errorMessage = ErrorGun.ErrorCodes.GetErrorMessages(jqXHR);
                        this.ErrorMessageSend(errorMessage);
                    })
                    .done((ajaxData) => {
                        this.ErrorMessageSend("");
                        this.ReportedErrorId(ajaxData.Id);
                    });
                };

                this.ClearErrorReport = () => {
                    this.Message("");
                    this.Detail("");
                    this.Category("");
                    this.Source("");
                    this.UserEmail("");
                }

                this.PrevErrorReportPage = () => {
                    var pageNumber = Math.max(1, this.ErrorReportPage() - 1);
                    this.ErrorReportPage(pageNumber);

                    this.LoadErrors();
                }

                this.NextErrorReportPage = () => {
                    var pageNumber = Math.min(this.ErrorReportPageCount(), this.ErrorReportPage() + 1);
                    this.ErrorReportPage(pageNumber);

                    this.LoadErrors();
                }
            }
        }
    }
}