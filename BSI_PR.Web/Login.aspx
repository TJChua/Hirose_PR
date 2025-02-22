﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="LoginPage" EnableViewState="false"
    ValidateRequest="false" CodeBehind="Login.aspx.cs" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v20.2, Version=20.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers" TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v20.2, Version=20.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.ExpressApp.Web.Templates.Controls" TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v20.2, Version=20.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.ExpressApp.Web.Controls" TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v20.2, Version=20.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" 
    Namespace="DevExpress.ExpressApp.Web.Templates" TagPrefix="cc3" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Logon</title>
</head>
<body class="Dialog">
    <div id="PageContent" class="PageContent DialogPageContent">
        <form id="form1" runat="server">
        <cc4:ASPxProgressControl ID="ProgressControl" runat="server" />
        <div id="Content" runat="server" />
        </form>
    </div>
    <%--This part is added.--%>
    <script type="text/javascript">
    //<![CDATA[
        function encodeLocationHash() {
        var search = window.location.search;
        if (search && search.indexOf('ReturnUrl=') > -1) {
            var hash = window.location.hash;
            if (hash && hash.length > 1) {
                var newUrl = window.location.href.replace(hash, '');
                var paramPosition = newUrl.search('&');
                if (paramPosition > 0) {
                    newUrl = newUrl.substring(0, paramPosition) + encodeURIComponent(hash) + newUrl.substring(paramPosition);
                }
                else {
                    newUrl += encodeURIComponent(hash);
                }
                window.location.replace(newUrl);
            }
        }
    }
    attachWindowEvent('load', encodeLocationHash);
    //]]>
    </script>
</body>
</html>
