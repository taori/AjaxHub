var AjaxHub;
(function (AjaxHub) {
    var _Statistics = (function () {
        function _Statistics() {
            this.runningRequests = 0;
        }
        return _Statistics;
    })();
    AjaxHub._Statistics = _Statistics;
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
        Request.getSignatureUrl = function (signature) {
            if (signature.signature.routeTemplate == null)
                return signature.signature.url;
            var templatedUrl = signature.signature.routeTemplate;
            templatedUrl = templatedUrl.replace("[action]", signature.signature.action);
            templatedUrl = templatedUrl.replace("[controller]", signature.signature.controller);
            for (var valueName in signature.values) {
                var parameterRegex = new RegExp("\{(" + valueName + "|" + valueName + ":[^}]+)\}", "gi");
                templatedUrl = templatedUrl.replace(parameterRegex, signature.values[valueName]);
            }
            return templatedUrl;
        };
        Request.prototype.endRequest = function () {
            this.$executionTarget.remove();
            this.$executionTarget = null;
            this.callEvent = null;
            this.signatureCall = null;
            _Invoker.statistics.runningRequests--;
        };
        Request.prototype.execute = function () {
            var _this = this;
            this.$executionTarget = _Invoker.getRequestContainer();
            this.callEvent = new CallEvent(this.signatureCall);
            _Invoker.statistics.runningRequests++;
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
                'url': Request.getSignatureUrl(this.signatureCall),
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
    var _Invoker = (function () {
        function _Invoker() {
        }
        _Invoker.getRequestContainer = function () {
            var hub = $("div#AjaxHubRequestContainer");
            if (!hub || hub.length === 0) {
                hub = $("<div id='AjaxHubRequestContainer' style='display: none;'><div>");
                $("body").append(hub);
            }
            var requestContainer = $("<div></div>");
            hub.append(requestContainer);
            return requestContainer;
        };
        _Invoker.execute = function (options) {
            var request = new Request(options);
            request.execute();
        };
        _Invoker.statistics = new _Statistics();
        return _Invoker;
    })();
    AjaxHub._Invoker = _Invoker;
})(AjaxHub || (AjaxHub = {}));
//# sourceMappingURL=AjaxHub.js.map