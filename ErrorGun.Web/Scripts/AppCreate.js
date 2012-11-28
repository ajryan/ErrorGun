var AppCreate;
(function (AppCreate) {
    var ViewModel = (function () {
        function ViewModel(serializedModel) {
            this.serializedModel = serializedModel;
        }
        return ViewModel;
    })();
    AppCreate.ViewModel = ViewModel;    
})(AppCreate || (AppCreate = {}));

