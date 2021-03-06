﻿module AjaxHub {

	export
	class _Statistics {
		constructor() {
			this.runningRequests = 0;
		}

		runningRequests : number;
	}

	class CallEvent {

		constructor(public callSignature: ISignatureCall) {
			
		}

		private canceled: boolean;

		public cancel(): void {
			this.canceled = true;
		}

		public isCanceled(): boolean {
			return this.canceled;
		}
	}

	class Request {
		constructor(public signatureCall: ISignatureCall) {
			
		}

		private $executionTarget : JQuery;
		private callEvent: CallEvent;
		private callEventArgumentArray: Array<any>;

		private static getSignatureUrl(signature: ISignatureCall): string {
			if (signature.signature.routeTemplate == null)
				return signature.signature.url;

			var templatedUrl = signature.signature.routeTemplate;
			templatedUrl = templatedUrl.replace("[action]", signature.signature.action);
			templatedUrl = templatedUrl.replace("[controller]", signature.signature.controller);

			for (var valueName in signature.values) {
				var parameterRegex = new RegExp("\{(" + valueName + "|" + valueName+":[^}]+)\}", "gi");
				templatedUrl = templatedUrl.replace(parameterRegex, signature.values[valueName]);
			}

			return templatedUrl;
		}

		public endRequest(): void {
			this.$executionTarget.remove();
			this.$executionTarget = null;
			this.callEvent = null;
			this.callEventArgumentArray = null;
			this.signatureCall = null;
			_Invoker.statistics.runningRequests--;
		}

		public execute(): void {

			this.$executionTarget = _Invoker.getRequestContainer();
			this.callEvent = new CallEvent(this.signatureCall);
			this.callEventArgumentArray = this.createCallArray(this.callEvent, this.signatureCall);
			
			_Invoker.statistics.runningRequests++;


			if (typeof window["AjaxHubCallRequestStart"] === "function")
				window["AjaxHubCallRequestStart"].apply(this, this.callEventArgumentArray);

			if (this.callEvent.isCanceled()) {
				this.endRequest();
				return;
			}

			if (typeof window[this.signatureCall.signature.callBefore] === "function")
				window[this.signatureCall.signature.callBefore].apply(this, this.callEventArgumentArray);

			if (this.callEvent.isCanceled()) {
				this.endRequest();
				return;
			}
			
			$.ajax({
				'url': Request.getSignatureUrl(this.signatureCall),
				'data': this.signatureCall.values,
				'traditional': true,
				'method': this.signatureCall.signature.method,
				'type': this.signatureCall.signature.method
			}).done(data => {

				this.$executionTarget.append(data);

				if (typeof window[this.signatureCall.signature.callAfter] === "function")
					window[this.signatureCall.signature.callAfter].apply(this, this.callEventArgumentArray);

				if (this.callEvent.isCanceled()) {
					this.endRequest();
					return;
				}

				if (typeof window["AjaxHubCallRequestDone"] === "function")
					window["AjaxHubCallRequestDone"].apply(this, this.callEventArgumentArray);

				this.endRequest();
			});
		}

		public createCallArray(callEvent: CallEvent, signatureCall: ISignatureCall): any[] {
			var result = [];
			result.push(callEvent);
			for (var index in signatureCall.signature.argumentNames) {
				var name = signatureCall.signature.argumentNames[index];
				result.push(signatureCall.values[name]);
			}
			return result;
		}
	}

	export 
	class _Invoker {

		public static statistics: _Statistics = new _Statistics();

		public static getRequestContainer(): JQuery {
			var hub = $("div#AjaxHubRequestContainer");
			if (!hub || hub.length === 0) {
				hub = $("<div id='AjaxHubRequestContainer' style='display: none;'><div>");
				$("body").append(hub);
			}

			var requestContainer = $("<div></div>");
			hub.append(requestContainer);

			return requestContainer;
		}

		public static execute(options: ISignatureCall): void {
			var request = new Request(options);
			request.execute();
		}
	}
}