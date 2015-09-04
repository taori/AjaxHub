// Based on Info from SignatureJavascriptSerializerBase
declare module AjaxHub {

	function Call(option: ISignatureCall): void;

	export interface ISignatureCall {
		signature: IDefaultSerialization;
		values: Object;
		extraArguments : Array<any>;
	}

	export interface IDefaultSerialization {
		url: string;
		action: string;
		controller: string;
		method: string;
		argumentNames: string[];
		callBefore : string;
		callAfter : string;
		routeTemplate : string;
		routeName : string;
	}
}