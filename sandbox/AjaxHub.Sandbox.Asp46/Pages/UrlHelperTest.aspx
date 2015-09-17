<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Mvc" %>

<%= new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Contact","Home") %>