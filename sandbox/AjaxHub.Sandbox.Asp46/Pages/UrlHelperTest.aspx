<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Mvc" %>

Url.Action <%= new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Contact","Home") %><br/>
leftpart <%= HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) %><br/>
vpuToAbsolute<%= VirtualPathUtility.ToAbsolute("~/") %><br/>