<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Mobile.Master" AutoEventWireup="true" CodeBehind="MobileNewTicket.aspx.cs" Inherits="NewBilletterie.MobileNewTicket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="main">

        <div id="site_content2">

            <div id="content100">
                <h1>New Ticket<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1>

                <div class="form_ticket">

                    <asp:Panel ID="pnlCategory" runat="server" Visible="true">
                        <table style="table-layout: auto; width: 98%">
                            <tr>
                                <td>
                                    <asp:Label ID="lblDepartment" runat="server" CssClass="loginLabel" Visible="true" Text="Department"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlDepartment" class="inputs" runat="server" Visible="true" Width="100%" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCategory" runat="server" CssClass="loginLabel" Visible="false" Text="Category"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlCategory" class="inputs" runat="server" Visible="false" Width="100%" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblSubCategory" runat="server" CssClass="loginLabel" Text="Sub Category" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlSubCategory" class="inputs" runat="server" Visible="false" Width="100%" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><br />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <asp:Panel ID="pnlMessage" runat="server" Visible="false">

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
                    </asp:Panel>

                    <asp:Panel ID="pnlAttachment" runat="server" Visible="false">

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

                    </asp:Panel>

                    <table style="table-layout: auto; width: 98%">
                        <tr>
                            <td align="left">
                                <asp:Button ID="btnBack" CssClass="customButtons" runat="server" Text="<< Back" OnClick="btnBack_Click" Visible="true" /><br />
                            </td>
                            <td align="right">
                                <asp:Button ID="btnNext" CssClass="customButtons" runat="server" Text="Next >>" OnClick="btnNext_Click" /><br />
                            </td>
                        </tr>
                    </table>

                </div>

            </div>

        </div>

    </div>
</asp:Content>
