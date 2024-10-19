<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateNewAccount.aspx.cs" Inherits="NewBilletterie.CreateNewAccount" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <div id="main">
           <div id="site_content1">
            <div id="content1">
                    <h1>Create New Account<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1>
                    <div class="form_ticket">
                        <table class="table2">

                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="Username "></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUserName" Width="650px" CssClass="largertextbox" runat="server" placeholder="Enter username." ToolTip="Use this field to provide your username. Maximum is 15 characters." Visible="true" AutoCompleteType="None" MaxLength="15"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtUserName" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="Password"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUserPassword" Width="650px" CssClass="largertextbox" runat="server" placeholder="Enter password." ToolTip="Enter a strong password that you will always remember. Maximum is 15 characters." Visible="true" TextMode="Password" AutoCompleteType="None" MaxLength="15"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtUserPassword" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Password is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Email "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtUserEmail" Width="650px" CssClass="largertextbox" runat="server" placeholder="Enter email address." ToolTip="Enter email address. Maximum is 250 characters." Visible="true" AutoCompleteType="None" MaxLength="250"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtUserEmail" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Email is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td colspan="2"><hr class="hr2" /></td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Title "></asp:Label>
                                 </td>
                                <td>
                                    <asp:DropDownList ID="ddlUserTitle" Width="150px" runat="server" Visible="true" AutoPostBack="false">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Dr" Value="Dr"></asp:ListItem>
                                        <asp:ListItem Text="Miss" Value="Miss"></asp:ListItem>
                                        <asp:ListItem Text="Mr" Value="Mr"></asp:ListItem>
                                        <asp:ListItem Text="Mrs" Value="Mrs"></asp:ListItem>
                                        <asp:ListItem Text="Ms" Value="Ms"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="First Name(s) "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtFirstName" Width="650px" CssClass="largertextbox" runat="server" placeholder="Enter password." ToolTip="Enter first name. Maximum is 150 characters." Visible="true" AutoCompleteType="None" MaxLength="150"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="txtFirstName" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="First name is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Surname "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtSurname" Width="650px" CssClass="largertextbox" runat="server" placeholder="Enter surname." ToolTip="Enter surname. Maximum is 150 characters." Visible="true" AutoCompleteType="None" MaxLength="15"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" ControlToValidate="txtSurname" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Surname is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Contact Number "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtContactNumber" Width="650px" CssClass="largertextbox" runat="server" placeholder="Enter password." ToolTip="Mobile number is required. Maximum is 15 characters." Visible="true" AutoCompleteType="None" MaxLength="15"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ControlToValidate="txtContactNumber" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Mobile number is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td colspan="2"><hr class="hr2" /></td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Organisation Name "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtOrganisation" Width="650px" CssClass="largertextbox" runat="server" placeholder="Organisation name." ToolTip="" Visible="true" AutoCompleteType="None" MaxLength="150"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Street Address "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtStreetAddress" Width="650px" CssClass="largertextbox" runat="server" placeholder="Street address." ToolTip="" Visible="true" AutoCompleteType="None" MaxLength="150"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtStreetAddress" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Street address is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Surburb "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtSurburb" Width="650px" CssClass="largertextbox" runat="server" placeholder="Street address." ToolTip="" Visible="true" AutoCompleteType="None" MaxLength="150"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator11" ControlToValidate="txtStreetAddress" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Street address is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="City / Town "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtCityTown" Width="650px" CssClass="largertextbox" runat="server" placeholder="City or town." ToolTip="" Visible="true" AutoCompleteType="None" MaxLength="150"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" ControlToValidate="txtCityTown" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="City is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Postal Code "></asp:Label>
                                 </td>
                                <td>
                                    <asp:TextBox ID="txtPostalCode" Width="650px" CssClass="largertextbox" runat="server" placeholder="Surburb." ToolTip="" Visible="true" AutoCompleteType="None" MaxLength="150"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" ControlToValidate="txtSurburb" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Surburb is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Country "></asp:Label>
                                 </td>
                                <td>
                                    <asp:DropDownList ID="ddlCountry" Width="150px" runat="server" Visible="true" AutoPostBack="false" Enabled="false">
                                        <asp:ListItem Text="South Africa" Value="South Africa" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>                                                                

                            <!-- CAPTCHA Row -->
                            <tr>
                                <td>CAPTCHA</td>
                                 <td>
                                  <fieldset style="width: 100%;">
                                    <legend>Enter the text in the image shown</legend>
                                             <table class="tableX">
                                                <tr><td colspan="6">&nbsp;</td></tr>
                                                <tr>
                                                    <td>&nbsp;&nbsp;</td>
                                                    <td><img src="GetUserCaptcha.ashx" width="110" height="40" /></td>
                                                    <td>&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtCaptchaText" AutoCompleteType="Disabled" MaxLength="6" Font-Size="Large" Font-Bold="true" Width="150px" runat="server" CssClass="textboxCaptcha"></asp:TextBox></td>
                                                    <td>&nbsp;&nbsp;&nbsp;<asp:Label ID="Label18" runat="server" Text="Can't read it?"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                                                    <td><asp:Button BackColor="PaleGreen" ID="btnNewCaptcha" runat="server" CssClass="captchabutton" Text="Try another one." OnClick="btnNewCaptcha_Click" Height="34px" Width="170px" CausesValidation="false" /></td>
                                                    <td>&nbsp;&nbsp;&nbsp;<asp:Label ID="lblStatus" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                 <tr><td colspan="6"><asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtCaptchaText" ValidationGroup="SubmitNewUser" Display="Dynamic" ErrorMessage="Please type in the text in the image above." ForeColor="Red" runat="server" /></td></tr>
                                            </table>
                                </fieldset>
                                 </td>
                            </tr>


                            <tr>
                                 <td colspan="2">&nbsp;</td>
                            </tr>

                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCreateNewUser" ValidationGroup="SubmitNewUser" runat="server" Width="170px" Text="Create User" OnClick="btnCreateNewUser_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancel" runat="server" Width="80px" Text="Cancel" ToolTip="" OnClick="btnCancel_Click" Visible="true" />
                                </td>
                            </tr>

                            <!-- Ticket confirmation popup -->
                            <tr>
                                 <td colspan="2">                                                    
                                    <asp:Button ID="btnSuccess" runat="server" style="display:none" />
                                    <cc1:ModalPopupExtender ID="ModalPopupExtenderSuccess" runat="server" TargetControlID="btnSuccess" PopupControlID="pnlSuccessPopup"
                                        CancelControlID="btnCancelOK" BackgroundCssClass="modalBackground">
                                    </cc1:ModalPopupExtender>

                                    <asp:Panel ID="pnlSuccessPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Height="200px" Width="550px" style="display:none">
                                            <table class="popTableV">

                                            <tr style="background-color:#007073">
                                                <td style=" height:10%; color:#349748; font-weight:bold; font-size:larger" class="popCell">
                                                <asp:Label ID="lblSuccessHeading" ForeColor="#E4EC04" runat="server" Text="User account successfully submitted."></asp:Label>
                                                </td>
                                            </tr>

                                            <tr><td colspan="2" class="popCell">&nbsp;</td></tr>

                                                   <tr>
                                                    <td colspan="2" class="popCellLeft">
                                                    <hr class="hr1" />
                                                    <div id="prinatbleDiv">
                                                    <table class="insideTable" style="width: 92%; margin-left: 5%;">

                                                        <%--Subject row--%>
                                                        <tr>
                                                            <td colspan="2"><asp:Label runat="server" ID="lblUserConfirmation"></asp:Label> </td>
                                                        </tr>

                                                    </table>
                                                        </div>
                                                    </td>
                                                 </tr>

                                            <tr style="background-color:#007073" class="popRow">
                                                <td class="popCellCenter">
                                                    <asp:Button ID="btnOK" runat="server" Text="OK"  Width="50px" Height="25px" OnClick="btnOK_Click" />  
                                                    <asp:Button ID="btnCancelOK" runat="server" Text="" Width="1px" Height="1px" CssClass="hiddenClass" Visible="true"/>
                                              </td>
                                            </tr>

                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="2" class="td"><asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Width="300px"></asp:Label></td>
                            </tr>

                       </table>
                    </div>       
                </div>
            </div>
      </div>



</asp:Content>
