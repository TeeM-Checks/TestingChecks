<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Mobile.Master" AutoEventWireup="true" CodeBehind="MobileIndex.aspx.cs" Inherits="NewBilletterie.MobileIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table style="table-layout:auto; width:98%">

         <tr>
                <td>&nbsp;</td>
        </tr>

        <%--<tr>
            <td>
                <asp:Label runat="server" CssClass="loginLabel">Preferred Email Address (Optional)</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtPreferredEmail" class="inputs" runat="server" placeholder="Preferred email account" Visible="true" Width="90%" MaxLength="100" AutoCompleteType="None"></asp:TextBox><br />
                <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtPreferredEmail" ForeColor="Red" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="Invalid email address" ValidationGroup="UserLogin"></asp:RegularExpressionValidator>
            </td>
        </tr>

        <tr>
                <td>&nbsp;</td>
        </tr>--%>

        <tr>
            <td>
                <asp:Label runat="server" CssClass="loginLabel">Username</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtUsername" class="inputs" runat="server" placeholder="Username" Visible="true" Width="90%" MaxLength="6" AutoCompleteType="None"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtUsername"  Display="Dynamic" ErrorMessage="*" ForeColor="Red" runat="server" Visible="false" ValidationGroup="UserLogin" />
            </td>
        </tr>


        <tr>
                <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" CssClass="loginLabel">Password</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" class="inputs" placeholder="Password" TextMode="Password" ToolTip="" Visible="true" Width="90%" MaxLength="25"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtPassword"  Display="Dynamic" ErrorMessage="*" ForeColor="Red" runat="server" Visible="false" ValidationGroup="UserLogin"/>
            </td>
        </tr>
        <tr>
                <td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <asp:Button BackColor="#4CAF50" Height="50px" Font-Size="Large" ForeColor="White" ID="btnLogin" OnClick="btnLogin_Click" runat="server" Width="200px" Text="Login" ValidationGroup="UserLogin" /><br />
                <asp:Button BackColor="#4CAF50" Height="50px" Font-Size="Large" ForeColor="White" ID="btnPasswordResetLogin" OnClick="btnPasswordResetLogin_Click" runat="server" Width="200px" Visible="false" Text="Password Reset" ValidationGroup="UserLogin" /><br />
            </th>
        </tr>
        <tr>
                <td>&nbsp;</td>
        </tr>

        <tr>
                <td class="td"><asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Width="100%"></asp:Label></td>
        </tr>
    </table>
</asp:Content>
