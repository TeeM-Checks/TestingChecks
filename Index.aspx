<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="NewBilletterie.Index" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

            <div id="site_content">

                <table style="width: 100%">
                    <tr>
                        <td style="width: 500px">
                            <div class="form_settings">
                                <table style="align-content: center; max-width: 100%; width: 100%; height: 100%;"> 
                                        
                                    
                        <%--Dummy row for Customer Bank Details Popup--%>
                        <tr>
                            <td class="valignTableTop" colspan="6">
                                <asp:Button ID="btnShowCustomerDetailPopup" runat="server" Style="display: none" />
                                <cc1:ModalPopupExtender ID="ModalPopupExtenderAccountDetail" runat="server" TargetControlID="btnShowCustomerDetailPopup" PopupControlID="pnlCustomerDetailPopup"
                                    CancelControlID="btnCancelDetails" BackgroundCssClass="modalBackground">
                                </cc1:ModalPopupExtender>

                                <asp:Panel ID="pnlCustomerDetailPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="70%" Style="max-height: 550px; display: none">
                                    <table class="popTableV" style="width: 100%; border: none;">
                                        <tr style="background-color: #007073">
                                            <td style="height: 40px; color: #349748; font-weight: bold; font-size: medium" class="popCell">
                                                <asp:Panel ID="Panel3" runat="server" ScrollBars="None" BackColor="#349748" BorderStyle="None" BorderWidth="0" Height="100px" Width="100%">
                                                    <table style="width: 100%;">
                                                        <tr align="center">
                                                            <td>
                                                                <asp:Label ID="lblWarningMessage" ForeColor="White" runat="server"></asp:Label><br />
                                                                <asp:Label ID="lblCustomerDetailHeading" ForeColor="#E4EC04" runat="server"></asp:Label>
                                                                <asp:Label ID="lblUSRPKID" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                                                <asp:Label ID="lblERMSPKID" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                                                <asp:Label ID="lblPasswordValue" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="popCellLeft">
                                                <asp:Panel ID="Panel4" runat="server" ScrollBars="Vertical" BackColor="Transparent" BorderStyle="None" BorderWidth="1" Width="100%" Style="max-height: 450px; min-height: 200px">
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                       <%-- <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;User Account</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" Enabled="false" ID="txtConfUserAccount" MaxLength="50" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ControlToValidate="txtConfUserAccount" ValidationGroup="ConfirmPopUpGroup" Display="Dynamic" ErrorMessage="User account is required." ForeColor="Red" runat="server" />
                                                                </div>
                                                            </td>
                                                        </tr>

                                                         <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                         <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Account Password</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" Enabled="false" ID="txtConfAccountPass" MaxLength="50" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1" TextMode="Password"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" ControlToValidate="txtConfAccountPass" ValidationGroup="ConfirmPopUpGroup" Display="Dynamic" ErrorMessage="User password is required." ForeColor="Red" runat="server" />
                                                                </div>
                                                            </td>
                                                        </tr>

                                                         <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>--%>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;ID / Passport Number</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtIDNumber" placeholder="ID or passport number of the CIPC Customer Account holder" MaxLength="150" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" ControlToValidate="txtIDNumber" ValidationGroup="ConfirmPopUpGroup" Display="Dynamic" ErrorMessage="ID number is required." ForeColor="Red" runat="server" />
                                                                </div>
                                                            </td>
                                                        </tr>


                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Full Names</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtFullName" placeholder="Full names of CIPC Customer Account holder" MaxLength="150" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtFullName" ValidationGroup="ConfirmPopUpGroup" Display="Dynamic" ErrorMessage="Full name is required." ForeColor="Red" runat="server" />
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Email Account</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtEmailAccount" placeholder="Email account as per CIPC records" MaxLength="150" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtEmailAccount" ValidationGroup="ConfirmPopUpGroup" Display="Dynamic" ErrorMessage="Email is required." ForeColor="Red" runat="server" />
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Mobile Phone Number</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtCustContact" placeholder="Mobile phone number as per CIPC records" MaxLength="50" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ControlToValidate="txtCustContact" ValidationGroup="ConfirmPopUpGroup" Display="Dynamic" ErrorMessage="Mobile phone number is required." ForeColor="Red" runat="server" />
                                                                    <asp:RegularExpressionValidator ID="regExCustomerContact" ControlToValidate="txtCustContact" ForeColor="Red" runat="server" ValidationExpression="\d+$" ErrorMessage="Please enter numeric values only" ValidationGroup="BankPopUpGroup"></asp:RegularExpressionValidator>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">
                                                                <hr class="hr2" />
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                    </table>
                                                </asp:Panel>

                                            </td>
                                        </tr>

                                        <tr style="background-color: #6BAB4D" class="popRow">
                                            <td>
                                                <table style="width: 100%; height: 90px;">

                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblPopErrorMessage" Visible="false" runat="server" Text="" ForeColor="Red"></asp:Label>
                                                        </td>
                                                    </tr>

                                                    <tr align="center">
                                                        <td>
                                                            <asp:Label ID="lblBottomMessage" runat="server" Visible="true" ForeColor="White"></asp:Label>

                                                        </td>
                                                    </tr>

                                                    <tr align="center">
                                                        <td>
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnSubmitAccountDetails" runat="server" OnClick="btnSubmitAccountDetails_Click" Width="120px" ValidationGroup="ConfirmPopUpGroup" Text="Submit" Visible="true" CausesValidation="true" />&nbsp; 
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelDetails" runat="server" Width="120px" Text="Cancel" Visible="true" CausesValidation="true" />&nbsp;
                                                            <%--<asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelForever" runat="server" Width="150px" Text="Don't Ask Me Again" Visible="true" CausesValidation="true" />&nbsp;--%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>

                            </td>
                        </tr>




                        <%--Dummy row for Customer OTP Popup--%>
                        <tr>
                            <td class="valignTableTop" colspan="6">
                                <asp:Button ID="btnShowCustomerOTPPopup" runat="server" Style="display: none" />
                                <cc1:ModalPopupExtender ID="ModalPopupExtenderOTPPopup" runat="server" TargetControlID="btnShowCustomerOTPPopup" PopupControlID="pnlCustomerOTPPopup"
                                    CancelControlID="btnCancelDetails" BackgroundCssClass="modalBackground">
                                </cc1:ModalPopupExtender>

                                <asp:Panel ID="pnlCustomerOTPPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="60%" Style="max-height: 550px; display: none">
                                    <table class="popTableV" style="width: 100%; border: none;">
                                        <tr style="background-color: #007073">
                                            <td style="height: 40px; color: #349748; font-weight: bold; font-size: medium" class="popCell">
                                                <asp:Panel ID="Panel2" runat="server" ScrollBars="None" BackColor="#349748" BorderStyle="None" BorderWidth="0" Height="100px" Width="100%">
                                                    <table style="width: 100%;">
                                                        <tr align="center">
                                                            <td>
                                                                <asp:Label ID="lblWarningOTPMessage" ForeColor="White" runat="server"></asp:Label><br />
                                                                <asp:Label ID="lblCustomerOTPHeading" ForeColor="#E4EC04" runat="server"></asp:Label>
                                                                <asp:Label ID="lblOTPUSRPKID" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                                                <asp:Label ID="lblOTPERMSPKID" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="popCellLeft">
                                                <asp:Panel ID="Panel5" runat="server" ScrollBars="Vertical" BackColor="Transparent" BorderStyle="None" BorderWidth="1" Width="100%" Style="max-height: 450px; min-height: 200px">
                                                    <table style="width: 100%;">
                                                        
                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td style="width: 30%;">
                                                                <div class="DetailLabel">&nbsp;One Time Password (OTP)</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtCustomerOTP" placeholder="Enter OTP Value" MaxLength="6" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1" Font-Size="Larger"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtCustomerOTP" ValidationGroup="OTPSubmit" Display="Dynamic" ErrorMessage="OTP is required." ForeColor="Red" runat="server" />
                                                                </div>
                                                            </td>
                                                        </tr>

                                                       <%-- <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>--%>

                                                        <%--<tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Full Names</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="TextBox2" placeholder="Full names of CIPC Customer Account holder" MaxLength="150" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Email Account</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="TextBox3" placeholder="Email account as per CIPC records" MaxLength="150" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Phone Number</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="TextBox4" placeholder="Phone number as per CIPC records" MaxLength="50" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtCustContact" ForeColor="Red" runat="server" ValidationExpression="\d+$" ErrorMessage="Please enter numeric values only" ValidationGroup="BankPopUpGroup"></asp:RegularExpressionValidator>
                                                                </div>
                                                            </td>
                                                        </tr>--%>

                                                        <tr>
                                                            <td colspan="2">
                                                                <hr class="hr2" />
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                    </table>
                                                </asp:Panel>

                                            </td>
                                        </tr>

                                        <tr style="background-color: #6BAB4D" class="popRow">
                                            <td>
                                                <table style="width: 100%; height: 90px;">

                                                    <tr>
                                                        <td align="center">
                                                            <asp:Label ID="lblOTPPopErrorMessage" Visible="false" runat="server" Text="" ForeColor="Red"></asp:Label>
                                                        </td>
                                                    </tr>

                                                    <tr align="center">
                                                        <td>
                                                            <asp:Label ID="lblOTPBottomMessage" runat="server" Visible="true" ForeColor="White"></asp:Label>
                                                            
                                                        </td>
                                                    </tr>

                                                    <tr align="center">
                                                        <td>
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnOTPSubmit" runat="server" OnClick="btnOTPSubmit_Click" Width="120px" ValidationGroup="OTPSubmit" Text="Submit" Visible="true" CausesValidation="true" />&nbsp; 
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnOTPCancel" runat="server" Width="120px" Text="Cancel" Visible="true" CausesValidation="true" />&nbsp;
                                                            <%--<asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelForever" runat="server" Width="150px" Text="Don't Ask Me Again" Visible="true" CausesValidation="true" />&nbsp;--%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>

                            </td>
                        </tr>



                                    <%--Dummy officer password reset--%>
                                 <%--   <tr>
                                        <td class="valignTableTop" colspan="4">
                                            <asp:Button ID="btnShowPasswordChangePopup" runat="server" Style="display: none" />
                                            <cc1:ModalPopupExtender ID="ModalPopupExtenderChangePassword" runat="server" TargetControlID="btnShowPasswordChangePopup" PopupControlID="pnlChangePasswordPopup"
                                                CancelControlID="btnCancelEdit" BackgroundCssClass="modalBackground">
                                            </cc1:ModalPopupExtender>
                                            <asp:Panel ID="pnlChangePasswordPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="500px" Style="display: none">
                                                <table class="popTableV">
                                                    <tr style="background-color: #007073">
                                                        <td colspan="2" style="height: 20px; color: #349748; font-weight: bold; font-size: larger" class="popCell">
                                                            <asp:Label ID="lblChangePasswordHeading" ForeColor="#E4EC04" runat="server"></asp:Label>
                                                            <asp:Label ID="lblChangePasswordOFCPKID" runat="server" Visible="false"></asp:Label>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td colspan="2" class="popCell">&nbsp;</td>
                                                    </tr>--%>

                                                    <%--Main details row--%>
                                               <%--     <tr>
                                                        <td colspan="2" class="popCellLeft">
                                                            <hr class="hr1" />
                                                            <table>--%>

                                                                <%--Username row--%>
                                                        <%--        <tr>
                                                                    <td class="tddd">
                                                                        <asp:Label runat="server" ID="lblPopUserID" Font-Bold="true" Text="User Login ID" Width="150px"></asp:Label>
                                                                    </td>
                                                                    <td class="td">
                                                                        <asp:TextBox runat="server" ID="txtPopUserID" MaxLength="25" CssClass="largertextbox" placeholder="Enter your user id." Height="25px" Width="350px"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtPopUserID" Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red" runat="server" ValidationGroup="UserLoginPop" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtPopUserID" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Username must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>--%>

                                                                <%--Old Password row--%>
                                                             <%--   <tr>
                                                                    <td class="tddd">
                                                                        <asp:Label runat="server" ID="lblPopOldPassword" Font-Bold="true" Text="Current Password" Width="150px"></asp:Label>
                                                                    </td>
                                                                    <td class="td">
                                                                        <asp:TextBox runat="server" ID="txtPopOldPassword" MaxLength="15" CssClass="largertextbox" placeholder="Enter your current password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtPopOldPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>--%>

                                                                <%--New Password row--%>
                                                           <%--     <tr>
                                                                    <td class="tddd">
                                                                        <asp:Label runat="server" ID="lblPopNewPassword" Font-Bold="true" Text="New Password" Width="150px"></asp:Label>
                                                                    </td>
                                                                    <td class="td">
                                                                        <asp:TextBox runat="server" ID="txtPopNewPassword" MaxLength="15" CssClass="largertextbox" placeholder="Enter your new password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPopNewPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>--%>

                                                                <%--New Password confirmation row--%>
                                          <%--                      <tr>
                                                                    <td class="tddd">
                                                                        <asp:Label runat="server" ID="lblPopConfirmPassword" Font-Bold="true" Text="Confirm Password" Width="150px"></asp:Label>
                                                                    </td>
                                                                    <td class="td">
                                                                        <asp:TextBox runat="server" ID="txtPopConfirmPassword" MaxLength="15" CssClass="largertextbox" placeholder="Repeat your new password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPopConfirmPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td colspan="2">&nbsp;</td>
                                                                </tr>

                                                                <tr>
                                                                    <td class="tddd">
                                                                        <asp:Label runat="server" ID="lblForgotCheckBox" Font-Bold="true" Text="Forgot your current password?" Width="150px"></asp:Label>
                                                                    </td>
                                                                    <td class="td">
                                                                        <asp:CheckBox runat="server" ID="chkForgotPassword" TextAlign="Right" Checked="false" AutoPostBack="true" Width="500px" OnCheckedChanged="chkForgotPassword_CheckedChanged" />
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td colspan="2">
                                                                        <asp:Label ID="lblGridRowError" Visible="false" runat="server" Text="" ForeColor="Red"></asp:Label>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td colspan="2">
                                                                        <hr class="hr2" />
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td colspan="2" class="td4">
                                                                        <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnChangePassword" ValidationGroup="UserLoginPop" runat="server" Width="100px" Text="Submit" Visible="true" CausesValidation="true" OnClick="btnChangePassword_Click" />&nbsp; 
                                                                    <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelEdit" runat="server" Width="100px" Text="Cancel" Visible="true" CausesValidation="true" />&nbsp;
                                                                    </td>
                                                                </tr>

                                                            </table>
                                                        </td>
                                                    </tr>--%>

                                                    <%--Close popup bottom--%>
                                <%--                    <tr style="background-color: #6BAB4D" class="popRow">
                                                        <td colspan="2" class="popCellCenter">&nbsp;
                                                        </td>
                                                    </tr>
                                                </table>


                                            </asp:Panel>
                                        </td>
                                    </tr>--%>

                                    <%--Dummy row for Confirmation Message Box Popup--%>
                                    <%--<tr>
                                        <td class="valignTableTop" colspan="4">
                                            <asp:Button ID="btnShowConfirmationPopup" runat="server" Style="display: none" />
                                            <cc1:ModalPopupExtender ID="ModalPopupExtenderConfirm" runat="server" TargetControlID="btnShowConfirmationPopup" PopupControlID="pnlConfirmationPopup"
                                                CancelControlID="imgCloseConfirm" BackgroundCssClass="modalBackground">
                                            </cc1:ModalPopupExtender>

                                            <asp:Panel ID="pnlConfirmationPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="300px" Height="150px" Style="display: none">
                                                <table class="popTableV">

                                                    <tr style="background-color: #007073">
                                                        <td colspan="2" style="height: 20px; color: #349748; font-weight: bold; font-size: larger" class="popCell">
                                                            <asp:Label ID="lblConfirmHeading" ForeColor="#E4EC04" runat="server"></asp:Label><asp:Label ID="Label2" runat="server" Visible="false"></asp:Label>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td colspan="2" class="popCell">&nbsp;</td>
                                                    </tr>--%>

                                                    <%--Main details row--%>
                                                   <%-- <tr>
                                                        <td colspan="2" class="popCellLeft">
                                                            <hr class="hr1" />
                                                            <table class="insideTable" style="width: 92%; margin-left: 5%;">--%>
                                                                <%--Message row--%>
                                                                <%--<tr>
                                                                    <td>
                                                                        <asp:Label runat="server" ID="lblConfirmationMessage"></asp:Label>
                                                                    </td>
                                                                </tr>

                                                                <tr>
                                                                    <td>
                                                                        <hr class="hr2" />
                                                                    </td>--%>
                                                               <%-- </tr>

                                                            </table>
                                                        </td>
                                                    </tr>--%>

                                                    <%--Close popup bottom--%>
                                                    <%--<tr style="background-color: #6BAB4D" class="popRow">
                                                        <td colspan="2" class="popCellCenter">
                                                            <asp:ImageButton ID="imgCloseConfirm" runat="server" BackColor="Transparent" ToolTip="Close window" BorderStyle="None" Width="25px" Height="25px" alt="Close" ImageUrl="~/Images/redOK.png" />
                                                        </td>
                                                    </tr>
                                                </table>--%>


                                          <%--  </asp:Panel>
                                        </td>
                                    </tr>--%>



                                </table>


                            </div>
                        </td>

                    </tr>
                </table>

                <div class="row show-grid">
                    <h1 style="color: green;margin-left: 20px;">Log in
                    <asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small" Visible="false"></asp:Label></h1>

                    <div class="col-md-8">
                        <div class="form-group">
                            <%--<label for="exampleInputEmail1">Preferred Email Address (Optional)</label>--%>
                            <asp:TextBox ID="txtPreferredEmail" MaxLength="200" CssClass="form-control" TextMode="Email" runat="server" placeholder="Enter preferred email account" ToolTip="Use this field to provide an alternative email account if you do not wish to receive email on your registered email account." Visible="false" AutoCompleteType="None"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtPreferredEmail" ForeColor="Red" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="Invalid email address" ValidationGroup="UserLogin"></asp:RegularExpressionValidator>
                        </div>
                        <div class="form-group">
                            <label for="exampleInputPassword1">Username</label>
                            <asp:TextBox ID="txtUsername" CssClass="form-control" runat="server" placeholder="Username" ToolTip="Please use your username or use alternative credentials provided to you." Visible="true" MaxLength="6" AutoCompleteType="None"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtUsername" Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red" runat="server" ValidationGroup="UserLogin" />
                        </div>
                        <div class="form-group">
                            <label for="exampleInputPassword1">Password</label>
                            <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" placeholder="Password" TextMode="Password" ToolTip="" Visible="true" MaxLength="25"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtPassword" Display="Dynamic" ErrorMessage="Password is required." ForeColor="Red" runat="server" ValidationGroup="UserLogin" />

                        </div>

                        <asp:Button ID="btnLogin" CssClass="btn btn-default" runat="server" ToolTip="Login with your CIPC username and password." Text="Login" OnClick="btnLogin_Click" ValidationGroup="UserLogin" />&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnPasswordResetLogin" CssClass="insideButtons btn btn-default" runat="server" Width="130px" ToolTip="Request reset of your CIPC password." Text="Password Reset" Visible="false" OnClick="btnPasswordResetLogin_Click" /><br />
                        <br /><br />
                         <%--<asp:LinkButton ID="lnkAccountCreationExternalLink" runat="server" Text="Do not have a CIPC account?" OnClientClick="window.document.forms[0].target='_blank';" Visible="false"></asp:LinkButton>--%>
                        <h5><a href="http://eservices.cipc.co.za/Customer_register_id.aspx" target="_blank">Do not have a CIPC account?</a></h5>
                                     
                    </div>
                    <%--<asp:LinkButton ID="lnkAccountCreationLink" runat="server" Text="Do not have a CIPC account?" OnClick="lnkAccountCreationLink_Click" Visible="false"></asp:LinkButton>--%>
                         &nbsp;&nbsp;<%--<asp:LinkButton runat="server" ID="lnkChangePassword" Text="Reset password" Visible="false" OnClick="lnkChangePassword_Click"></asp:LinkButton> --%>
                          
                    <div class="col-md-4">
                        <ul class="navn navn-list" style="padding: 0px 0px; position: relative; display: block;">
                            <li>
                                <h4 class="tree-toggle nav-header">Frequently Asked Questions</h4>
                                <ul class=" nav-list tree">
                                    <li>

                                    <li><a href="https://www.bizportal.gov.za/" target="_blank">BizPortal</a></li>
                                    <li><a href="https://www.cipc.co.za/?page_id=149" target="_blank">Companies</a></li>
                                    <li><a href="https://www.cipc.co.za/?page_id=2197" target="_blank">Co-operatives</a></li>

                                    <li>
                                        <h4 class="tree-toggle nav-header">Intellectual Property</h4>
                                        <ul class=" nav-list tree">
                                            <li><a style="color: none;" href="https://www.cipc.co.za/?page_id=4586" target="_blank">Copyright</a></li>
                                            <li><a href="https://www.cipc.co.za/?page_id=4351" target="_blank">Designs</a></li>
                                            <li><a href="https://www.cipc.co.za/?page_id=4184" target="_blank">Patents</a></li>
                                            <li><a href="https://www.cipc.co.za/?page_id=4118" target="_blank">Trade Marks</a></li>

                                            <li>
                                                <h4 class="tree-toggle nav-header">Useful Links</h4>
                                                <ul class="navn navn-list tree">
                                                    <li><a href="https://www.cipc.co.za/?page_id=4447" target="_blank">How to - Step by Step Guides</a></li>
                                                    <li><a href="https://www.cipc.co.za/?page_id=1975" target="_blank">Password Reset</a></li>
                                                    <li><a href="https://eservices.cipc.co.za/" target="_blank">Track my Transaction</a></li>
                                                </ul>
                                            </li>
                                        </ul>
                                    </li>
                                </ul>
                            </li>
                        </ul>

                    </div>
                </div>

            </div>

</asp:Content>
