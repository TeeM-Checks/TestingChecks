<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Mobile.Master" AutoEventWireup="true" CodeBehind="MobileMainMenu.aspx.cs" Inherits="NewBilletterie.MobileMainMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="table-layout:auto; width:100%">
        <tr>
            <td>
                <asp:Button runat="server" CssClass="toogleButtons" ID="btnCreateNewTicket" Width="100%" Height="50px" OnClick="btnCreateNewTicket_Click" Text="Create New Ticket" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button runat="server" CssClass="toogleButtons" ID="btnViewTickets" Width="100%" Height="50px" OnClick="btnViewTickets_Click" Text="View Tickets" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button runat="server" CssClass="toogleButtons" ID="btnFAQs" Width="100%" Height="50px" OnClick="btnFAQs_Click" Text="Frequently Asked Questions" />
            </td>
        </tr>
    </table>
</asp:Content>
