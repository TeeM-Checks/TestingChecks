<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Mobile.Master" AutoEventWireup="true" CodeBehind="MobileRefund.aspx.cs" Inherits="NewBilletterie.MobileRefund" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="main">

        <div id="site_content2">

            <div id="content100">
                <h1>Refund Request<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1><br />
                
                <asp:Label ID="lblWarningMessage" runat="server" CssClass="loginLabel" Visible="true"></asp:Label>
                
                <div class="form_ticket">

                    <asp:Panel ID="pnlCategory" runat="server" Visible="true">
                        <table style="table-layout: auto; width: 98%">
                            <tr>
                                <td>
                                    <asp:Label ID="lblIDNumber" runat="server" CssClass="loginLabel" Visible="true" Text="ID Number"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtIDNumber" class="inputs" runat="server" placeholder="ID Number" Visible="true" MaxLength="150" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblFullName" runat="server" CssClass="loginLabel" Visible="true" Text="Full Names"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtFullName" class="inputs" runat="server" placeholder="Full Names" Visible="true" MaxLength="150" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td>&nbsp;</td>
                            </tr>

                              <tr>
                                <td>
                                    <asp:Label ID="lblContactNumber" runat="server" CssClass="loginLabel" Visible="true" Text="Contact Number"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtContactNumber" class="inputs" runat="server" placeholder="Contact Number" Visible="true" MaxLength="150" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>
                             <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblBankName" runat="server" CssClass="loginLabel" Text="Bank Name" Visible="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlBankName" class="inputs" runat="server" Visible="true" Width="92%" AutoPostBack="true" OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged"></asp:DropDownList><br />
                                </td>
                            </tr>
                             <tr>
                                <td>&nbsp;</td>
                            </tr>

                             <tr>
                                <td>
                                    <asp:Label ID="lblBranchCode" runat="server" CssClass="loginLabel" Text="Branch Code" Visible="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlBranchCode" class="inputs" runat="server" Visible="true" Width="92%" AutoPostBack="true"></asp:DropDownList><br />
                                </td>
                            </tr>

                              <tr>
                                <td>&nbsp;</td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblAccountHolder" runat="server" CssClass="loginLabel" Visible="true" Text="Account Holder Name"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtAccountHolderName" class="inputs" runat="server" placeholder="Account Holder" Visible="true" MaxLength="150" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>

                             <tr>
                                <td>&nbsp;</td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblAccountNumber" runat="server" CssClass="loginLabel" Visible="true" Text="Account Number"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtAccountNumber" class="inputs" runat="server" placeholder="Account Number" Visible="true" MaxLength="150" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>

                              <tr>
                                <td>&nbsp;</td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblAttachFile" runat="server" CssClass="loginLabel" Visible="true" Text="Attach File"></asp:Label>
                                </td>
                            </tr>

                             <tr>
                                <td>
                                    <asp:FileUpload ID="fupAttachFile" AllowMultiple="false" Width="90%" runat="server" CssClass="inputs" />
                                </td>
                            </tr>

                              <tr>
                                <td>
                                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="loginLabel" ForeColor="Red" Visible="false"></asp:Label>
                                </td>
                            </tr>

                        </table>
                    </asp:Panel>

                    <%--<asp:Panel ID="pnlMessage" runat="server" Visible="false">

                        <table style="table-layout: auto; width: 98%">
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblProvince" runat="server" CssClass="loginLabel" Visible="true" Text="Province"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlProvince" class="inputs" runat="server" Width="100%"></asp:DropDownList><br />
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblSubject" runat="server" CssClass="loginLabel" Visible="true" Text="Subject"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtSubject" class="inputs" runat="server" placeholder="Subject" ToolTip="Use this field to provide the title/subject of the query. Maximum is 150 characters." Visible="true" MaxLength="150" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblReference" runat="server" CssClass="loginLabel" Visible="true" Text="Reference No"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtReference" class="inputs" runat="server" placeholder="Reference" ToolTip="This field is optional. Maximum is 20 characters." Visible="true" MaxLength="20" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblMessage" runat="server" CssClass="loginLabel" Text="Message" Visible="true"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtMessage" class="inputs" TextMode="MultiLine" Columns="5" runat="server" placeholder="Message" Visible="true" AutoCompleteType="None"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>--%>

                    <%--<asp:Panel ID="pnlAttachment" runat="server" Visible="false">

                        <table style="table-layout: auto; width: 98%">

                            <tr>
                                <td>
                                    <asp:Label ID="lblAttach" runat="server" CssClass="loginLabel" Visible="true" Text="Attach File"></asp:Label>
                                    <asp:FileUpload ID="fupAttachFile" AllowMultiple="false" runat="server" CssClass="inputs" />
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblCAPTCHA" runat="server" CssClass="loginLabel" Visible="true" Text="CAPTCHA"></asp:Label>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <fieldset style="width: 98%;" class="loginLabel">
                                        <legend>Enter the text in the image shown</legend>
                                        <table>
                                            <tr>
                                                <td>
                                                    <img src="../GetCaptcha.ashx" width="95%" height="60px" />
                                                    <asp:TextBox ID="txtCaptchaText" AutoCompleteType="Disabled" MaxLength="6" Font-Size="Large" Font-Bold="true" Width="85%" Height="40px" runat="server" CssClass="inputs"></asp:TextBox>
                                                    
                                                    <asp:Label ID="lblStatus" runat="server" Font-Bold="true"></asp:Label>       <asp:Button ID="btnNewCaptcha" runat="server" CssClass="captchabutton" Text="Get New" OnClick="btnNewCaptcha_Click" CausesValidation="false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtCaptchaText" ValidationGroup="SubmitTicket" Display="Dynamic" ErrorMessage="Please type in the text in the image above." ForeColor="Red" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>

                    </asp:Panel>--%>

                    <table style="table-layout: auto; width: 98%">
                        <tr>
                            <td align="left">
                                <asp:Button ID="btnSubmit" CssClass="customButtons" runat="server" Text="Submit" OnClick="btnSubmit_Click" Visible="true" /><br />
                            </td>
                            <td align="right">
                                <asp:Button ID="btnCancel" CssClass="customButtons" runat="server" Text="Cancel" OnClick="btnCancel_Click" /><br />
                            </td>
                        </tr>
                    </table>

                </div>

            </div>

        </div>

    </div>
</asp:Content>
