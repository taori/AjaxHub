var AjaxHub;
(function (AjaxHub) {
    var Statistics = (function () {
        function Statistics() {
            this.runningRequests = 0;
        }
        return Statistics;
    })();
    AjaxHub.Statistics = Statistics;
    var CallEvent = (function () {
        function CallEvent(callSignature) {
            this.callSignature = callSignature;
        }
        CallEvent.prototype.cancel = function () {
            this.canceled = true;
        };
        CallEvent.prototype.isCanceled = function () {
            return this.canceled;
        };
        return CallEvent;
    })();
    var Request = (function () {
        function Request(signatureCall) {
            this.signatureCall = signatureCall;
        }
        Request.prototype.endRequest = function () {
            this.$executionTarget.remove();
            this.$executionTarget = null;
            this.callEvent = null;
            this.signatureCall = null;
            Invoker.statistics.runningRequests--;
        };
        Request.prototype.execute = function () {
            var _this = this;
            this.$executionTarget = Invoker.getRequestContainer();
            this.callEvent = new CallEvent(this.signatureCall);
            Invoker.statistics.runningRequests++;
            if (window["AjaxHubCallRequestStart"] === "function")
                window["AjaxHubCallRequestStart"](this.callEvent);
            if (this.callEvent.isCanceled()) {
                this.endRequest();
                return;
            }
            if (window[this.signatureCall.signature.callBefore] === "function")
                window[this.signatureCall.signature.callBefore](this.callEvent);
            if (this.callEvent.isCanceled()) {
                this.endRequest();
                return;
            }
            $.ajax({
                'url': this.signatureCall.signature.url,
                'data': this.signatureCall.values,
                'traditional': true,
                'method': this.signatureCall.signature.method
            }).done(function (data) {
                _this.$executionTarget.append(data);
                if (window[_this.signatureCall.signature.callAfter] === "function")
                    window[_this.signatureCall.signature.callAfter](_this.callEvent);
                if (_this.callEvent.isCanceled()) {
                    _this.endRequest();
                    return;
                }
                if (window["AjaxHubCallRequestDone"] === "function")
                    window["AjaxHubCallRequestDone"](_this.callEvent);
                _this.endRequest();
            });
        };
        return Request;
    })();
    var Invoker = (function () {
        function Invoker() {
        }
        Invoker.getRequestContainer = function () {
            var hub = $("div#AjaxHubRequestContainer");
            if (!hub || hub.length === 0) {
                hub = $("<div id='AjaxHubRequestContainer' style='display: none;'><div>");
                $("body").append(hub);
            }
            var requestContainer = $("<div></div>");
            hub.append(requestContainer);
            return requestContainer;
        };
        Invoker.call = function (options) {
            var request = new Request(options);
            request.execute();
        };
        Invoker.statistics = new Statistics();
        return Invoker;
    })();
    AjaxHub.Invoker = Invoker;
})(AjaxHub || (AjaxHub = {}));
//# sourceMappingURL=AjaxHub.js.map