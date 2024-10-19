<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TestList.aspx.cs" Inherits="NewBilletterie.TestList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:DataList ID="dlCustomers" runat="server" RepeatDirection="Horizontal" RepeatColumns="4">
            <ItemTemplate>
            <table cellpadding="2" cellspacing="0" class="Item">
                <tr>
                    <td class="header">
                        <b><u>
                            <%# Eval("TCK_TicketNumber") %></u></b>
                    </td>
                </tr>
                <tr>
                    <td class="body">
                        <b>Subject: </b>
                        <%# Eval("TCK_Subject") %><br />
                        <b>Reference: </b>
                        <%# Eval("TCK_Reference") %><br />
                        <b>Date Created: </b>
                        <%# Eval("TCK_DateCreated")%><br />
                        <b>Date Closed: </b>
                        <%# Eval("TCK_DateClosed")%><br />
                        <b>Email: </b>
                        <%# Eval("TCK_AlternateEmail")%>
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
            </table>
            </ItemTemplate>
    </asp:DataList>


    <asp:Repeater ID="rptPager" runat="server">
        <ItemTemplate>
            <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
                CssClass='<%# Convert.ToBoolean(Eval("Enabled")) ? "page_enabled" : "page_disabled" %>' OnClick="Page_Changed" OnClientClick='<%# !Convert.ToBoolean(Eval("Enabled")) ? "return false;" : "" %>'></asp:LinkButton>
       </ItemTemplate>
    </asp:Repeater>


</asp:Content>
