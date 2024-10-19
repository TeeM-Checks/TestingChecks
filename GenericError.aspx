<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenericError.aspx.cs" Inherits="NewBilletterie.GenericError" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row cells12">
            <h1>System Error<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1>
        </div>


        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-sm-3" style="margin-left: -15px;" id="divReport" runat="server">
                        <asp:Label runat="server" ForeColor="Red" ID="lblError">You are getting this message bacause the system has encountered an error. You need to refresh in to continue with your operation.</asp:Label>
                        <%--<asp:Label runat="server" ForeColor="Red" ID="lblSource"></asp:Label>
                        <asp:Label runat="server" ForeColor="Red" ID="lblInnerEx"></asp:Label>
                        <asp:Label runat="server" ForeColor="Red" ID="lblStackTrace"></asp:Label>--%>
                    </div>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
