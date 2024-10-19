<%--<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GetCBRSDocument.aspx.cs" Inherits="NewBilletterie.GetCBRSDocument" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetCBRSDocument.aspx.cs" Inherits="NewBilletterie.GetCBRSDocument" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row cells12">
                <h1>
                    <asp:Label ID="lblMainErrorMessage" runat="server" Text="Document Not Found" ForeColor="Red" Font-Size="Large"></asp:Label>
                </h1>
            </div>

            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-sm-3" style="margin-left: -15px;" id="divReport" runat="server">
                            <asp:Label runat="server" ForeColor="Red" ID="lblError">&nbsp;&nbsp;You are getting this message the document you are looking for is yet to be migrated. You need to check again later.</asp:Label>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </form>
</body>
</html>