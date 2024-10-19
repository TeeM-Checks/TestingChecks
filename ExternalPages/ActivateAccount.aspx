<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActivateAccount.aspx.cs" Inherits="NewBilletterie.ActivateAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div>
         <div id="site_content">
             <div class="form_settings">
                     <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnClose" runat="server" Width="100px" Text="Close" Visible="true" OnClick="btnClose_Click" /><br /><br />
                     <asp:Label ID="lblGridRowError" Visible="false" runat="server" Text="" ForeColor="Red" ></asp:Label>
             </div>
         </div>
    </div>
</asp:Content>
