angular
    .module("logictracker.common")
    .factory("ErrorHelper", [ErrorHelper]);

function ErrorHelper() {
    var helper = {
        onFail : onFail
    };

    function onFail(notify, error) {
        if (!notify) return;
        try {
            if (error.data.ExceptionMessage) {
                notify.show(error.data.ExceptionMessage, "error");
                return;
            }
        } catch (x) { }

        if (error.errorThrown !== undefined)
            notify.show(error.errorThrown, "error");
        else
            notify.show(error, "error");
    };

    return helper;
}