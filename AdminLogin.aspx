<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminLogin.aspx.cs" Inherits="NewBilletterie.AdminLogin" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
                <div id="site_content">

                <table style="width:100%">
                    <tr>
                        <td style="width:500px">
                          <%--<div id="content">--%>
                            <h1>Log in <asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small" Visible="false"></asp:Label></h1>

                            <div class="form_settings">
                                <table style="align-content:center; max-width:100%;width:100%;height:100%;" >
                                    <%--<tr>
                                        <td>
                                            <asp:Label runat="server">Preferred Email Address (Optional)</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPreferredEmail" MaxLength="200" CssClass="largertextbox" TextMode="Email" runat="server" placeholder="Enter preferred email account" ToolTip="Use this field to provide an alternative email account if you do not wish to receive email on your registered email account." Visible="true" AutoCompleteType="None"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtPreferredEmail" ForeColor="Red" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="Invalid email address" ValidationGroup="UserLogin"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>--%>
                                    <tr>
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                         <td>
                                             <asp:Label runat="server">Username</asp:Label>
                                             &nbsp;&nbsp;&nbsp;
                                         </td>
                                        <td>
                                            <asp:TextBox ID="txtUsername" CssClass="form-control" runat="server" placeholder="Username" ToolTip="Please use your username or use alternative credentials provided to you." Visible="true" MaxLength="50" AutoCompleteType="None"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtUsername"  Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red" runat="server" ValidationGroup="UserLogin" />
                                        </td>
                                    </tr>
                                    <tr>
                                         <td>&nbsp;</td><td>&nbsp;&nbsp;&nbsp;</td>
                                    </tr>
                                    <tr>
                                         <td>
                                             <asp:Label runat="server">Password</asp:Label>
                                         </td>
                                        <td>
                                            <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" placeholder="Password" TextMode="Password" ToolTip="" Visible="true" MaxLength="50"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtPassword"  Display="Dynamic" ErrorMessage="Password is required." ForeColor="Red" runat="server" ValidationGroup="UserLogin"/>
                                        </td>
                                    </tr>
                                    <tr>
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                         <td>&nbsp;</td>
                                        <td class="buttonTD">
                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnLogin" runat="server" Width="70px" Text="Login" OnClick="btnLogin_Click" ValidationGroup="UserLogin" /><br />
                                            <asp:LinkButton ID="lnkAccountCreationExternalLink" runat="server" Text="Do not have a CIPC account?" OnClientClick="window.document.forms[0].target='_blank';" Visible="false" ></asp:LinkButton>
                                            <asp:LinkButton ID="lnkAccountCreationLink" runat="server" Text="Do not have a CIPC account?" OnClick="lnkAccountCreationLink_Click" Visible="false"></asp:LinkButton>
                                           &nbsp;&nbsp;<asp:LinkButton runat="server" ID="lnkChangePassword" Text="Reset password" Visible="false" OnClick="lnkChangePassword_Click"></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>



                                 <%--Dummy account activation from leave--%>
                                 <tr>
                                     <td class="valignTableTop" colspan="4">
                                        <asp:Button ID="btnLeaveActivationPopup" runat="server" style="display:none" />
                                        <cc1:ModalPopupExtender ID="ModalPopupExtenderActivateLeave" runat="server"  TargetControlID="btnLeaveActivationPopup" PopupControlID="pnlActivateLeavePopup"
                                            CancelControlID="btnConfirmActionNo"  BackgroundCssClass="modalBackground">
                                        </cc1:ModalPopupExtender>
                                            <asp:Panel ID="pnlActivateLeavePopup" runat="server" ScrollBars="None" BackColor="#349748" BorderStyle="Solid" BorderWidth="1" Width="450px" Height="150px" Style="display: none">
                                            <table class="popTableV">
                                                <tr style="background-color: #349748">
                                                    <td colspan="2" style="height: 20px; color: #349748; font-weight: bold; font-size: larger" class="popCell">
                                                        <asp:Label ID="lblConfirmActionHeading"  ForeColor="#E4EC04" runat="server"></asp:Label>
                                                        <asp:Label ID="lblConfirmActionDefinition" runat="server" Visible="false"></asp:Label>
                                                        <asp:Label ID="lblConfirmActionPKID" runat="server" Visible="false"></asp:Label>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td colspan="2" class="popCell">&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" class="popCellLeft">
                                                        <table class="insideTable" style="width: 92%; margin-left: 5%;">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label runat="server" ID="lblConfirmActionWarning"></asp:Label>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button runat="server" BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" CausesValidation="true" ID="btnConfirmActionYes" Text="Yes" OnClick="btnConfirmActionYes_Click" Width="150px" />
                                                                    &nbsp; 
                                                                        <asp:Button runat="server" BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" CausesValidation="true" ID="btnConfirmActionNo" Text="No" OnClick="btnConfirmActionNo_Click" Width="150px" />
                                                                    &nbsp; 
                                                                </td>
                                                            </tr>

                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr style="background-color: #6BAB4D" class="popRow">
                                                    <td colspan="2" class="popCellCenter"></td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                     </td>
                                 </tr>





                                 <%--Dummy officer password reset--%>
                                 <tr>
                                     <td class="valignTableTop" colspan="4">
                                        <asp:Button ID="btnShowPasswordChangePopup" runat="server" style="display:none" />
                                        <cc1:ModalPopupExtender ID="ModalPopupExtenderChangePassword" runat="server"  TargetControlID="btnShowPasswordChangePopup" PopupControlID="pnlChangePasswordPopup"
                                            CancelControlID="btnCancelEdit"  BackgroundCssClass="modalBackground">
                                        </cc1:ModalPopupExtender>
                                         <asp:Panel ID="pnlChangePasswordPopup" runat="server" ScrollBars="None" BackColor="lightgray" BorderStyle="Solid" BorderWidth="1" Width="600px" Style="display: none">
                                             <table class="popTableV">
                                                 <tr style="background-color: #007073">
                                                     <td colspan="2" style="height: 20px; color: #349748; font-weight: bold; font-size: larger; margin-left:10px" class="popCell">
                                                         <span style="margin-left:10px"><asp:Label ID="lblChangePasswordHeading"  ForeColor="#E4EC04" runat="server"></asp:Label></span>
                                                         <asp:Label ID="lblChangePasswordOFCPKID" runat="server" Visible="false"></asp:Label>
                                                     </td>
                                                 </tr>

                                                 <tr>
                                                     <td colspan="2" class="popCell">&nbsp;</td>
                                                 </tr>
                                             </table>
                                             <%--Main details row--%>
                                             <table style="margin-top: 40px; margin-left: 20px;">

                                                 <%--Username row--%>
                                                 <tr>
                                                     <td class="">
                                                         <span style="margin-top: -20px;"><asp:Label runat="server" ID="lblPopUserID" Font-Bold="true" Text="User Login ID  *" Width="150px"></asp:Label></span>
                                                     </td>
                                                     <td class="">
                                                         <asp:TextBox runat="server" ID="txtPopUserID" MaxLength="25" CssClass="largertextbox" placeholder="Enter your user id." Height="25px" Width="350px"></asp:TextBox>
                                                         <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtPopUserID" Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red" runat="server" ValidationGroup="UserLoginPop" />
                                                         <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtPopUserID" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Username must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                     </td>
                                                 </tr>

                                                 <%--Old Password row--%>
                                                 <tr>
                                                     <td class="tddd">
                                                         <span style="margin-top: -20px;"><asp:Label runat="server" ID="lblPopOldPassword" Font-Bold="true" Text="Current Password *" Width="150px"></asp:Label></span>
                                                     </td>
                                                     <td class="">
                                                         <asp:TextBox runat="server" ID="txtPopOldPassword" MaxLength="15" CssClass="largertextbox" placeholder="Enter your current password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                         <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtPopOldPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                         <asp:Label runat="server" ID="lblPopOldPasswordBlank" Text="Leave blank" ForeColor="Red" Visible="false"></asp:Label>
                                                     </td>
                                                 </tr>

                                                 <%--New Password row--%>
                                                 <tr>
                                                     <td class="tddd">
                                                         <span style="margin-top: -20px;"><asp:Label runat="server" ID="lblPopNewPassword" Font-Bold="true" Text="New Password *" Width="150px"></asp:Label></span>
                                                     </td>
                                                     <td class="">
                                                         <asp:TextBox runat="server" ID="txtPopNewPassword" MaxLength="15" CssClass="largertextbox" placeholder="Enter your new password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                         <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPopNewPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                         <asp:Label runat="server" ID="lblPopNewPasswordBlank" Text="Leave blank" ForeColor="Red" Visible="false"></asp:Label>
                                                     </td>
                                                 </tr>

                                                 <%--New Password confirmation row--%>
                                                 <tr>
                                                     <td class="tddd">
                                                         <span style="margin-top: -20px;"><asp:Label runat="server" ID="lblPopConfirmPassword" Font-Bold="true" Text="Confirm Password *" Width="150px"></asp:Label></span>
                                                     </td>
                                                     <td>
                                                         <asp:TextBox runat="server" ID="txtPopConfirmPassword" MaxLength="15" CssClass="largertextbox" placeholder="Repeat your new password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                         <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPopConfirmPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                         <asp:Label runat="server" ID="lblPopConfirmPasswordBlank" Text="Leave blank" ForeColor="Red" Visible="false"></asp:Label>
                                                     </td>
                                                 </tr>

                                                 <tr>
                                                     <td colspan="2">&nbsp;</td>
                                                 </tr>

                                                 <tr>
                                                     <td class="tddd">
                                                         <span style="margin-top: -20px;"><asp:Label runat="server" ID="lblForgotCheckBox" Font-Bold="true" Text="Forgot your current password? *" Width="150px"></asp:Label></span>
                                                     </td>
                                                     <td>
                                                        <span  style="margin-left:-200px; margin-top: -30px; background: transparent; width: 430px;"> <asp:CheckBox runat="server" CssClass="checkboxses" ID="chkForgotPassword" ForeColor="Transparent" BackColor="Transparent" TextAlign="Left" Checked="false" AutoPostBack="true"  OnCheckedChanged="chkForgotPassword_CheckedChanged" /></span>
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
                                             <table>

                                                 <%--Close popup bottom--%>
                                                 <tr style="background-color: #6BAB4D" class="popRow">
                                                     <td colspan="2" class="popCellCenter">
                                                     </td>
                                                 </tr>
                                             </table>


                                         </asp:Panel>
                                     </td>
                                 </tr>



                                 <%--Dummy row for Confirmation Message Box Popup--%>
                                 <tr>
                                     <td class="valignTableTop" colspan="4">
                                        <asp:Button ID="btnShowConfirmationPopup" runat="server" style="display:none" />
                                        <cc1:ModalPopupExtender ID="ModalPopupExtenderConfirm" runat="server"  TargetControlID="btnShowConfirmationPopup" PopupControlID="pnlConfirmationPopup"
                                            CancelControlID="imgCloseConfirm"  BackgroundCssClass="modalBackground">
                                        </cc1:ModalPopupExtender>

                                        <asp:Panel ID="pnlConfirmationPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="300px" Height="150px" style="display:none">
                                                <table class="popTableV">
                                        
                                                <tr style="background-color:#007073">
                                                    <td colspan="2" style=" height:20px; color:#349748; font-weight:bold; font-size:larger" class="popCell">
                                                        <asp:Label ID="lblConfirmHeading" ForeColor="#E4EC04" runat="server"></asp:Label><asp:Label ID="Label2" runat="server" Visible="false"></asp:Label>
                                                    </td>
                                                </tr>

                                                    <tr><td colspan="2" class="popCell">&nbsp;</td></tr>

                                                    <%--Main details row--%>
                                                    <tr>
                                                        <td colspan="2" class="popCellLeft">
                                                            <hr class="hr1" />
                                                            <table class="insideTable" style="width: 92%; margin-left: 5%;">
                                                                <%--Message row--%>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label runat="server" ID="lblConfirmationMessage"></asp:Label>
                                                                   </td>
                                                                </tr>

                                                                <tr><td><hr class="hr2" /></td></tr>
                                                                                                              
                                                            </table>
                                                        </td>
                                                    </tr>

                                                <%--Close popup bottom--%>
                                                <tr style="background-color:#6BAB4D" class="popRow">
                                                    <td colspan="2" class="popCellCenter">
                                                        <asp:ImageButton id="imgCloseConfirm" runat="server" BackColor="Transparent" ToolTip="Close window" BorderStyle="None" width="25px" height="25px" alt="Close" ImageUrl="~/Images/redOK.png" />
                                                    </td>
                                                </tr>
                                            </table>


                                        </asp:Panel>
                                     </td>
                                 </tr>


                               </table>

                    
                            </div>       
   
                        <%--</div>--%>
                        </td>

                        <td style="width:300px">
                            <div id="sidebar_container" runat="server" visible="false">
                                <div class="sidebar">
                                    <div class="sidebar_top"></div>
                                    <div class="sidebar_item">
                                        <h3>Frequently Asked Questions</h3>
                                        <h4><a href="http://www.cipc.co.za/index.php/faqs/close-corporations-faq/" target="_blank">Closed Corporations</a></h4>
                                        <h4><a href="http://www.cipc.co.za/index.php/faqs/companies-faq/" target="_blank">Companies</a></h4>
                                        <h4><a href="http://www.cipc.co.za/index.php/faqs/co-operatives-faq/" target="_blank">Co-operatives</a></h4>
                                        <h4>Intellectual Property</h4>
                                            <ul>
                                                <li><a href="http://www.cipc.co.za/index.php/faqs/copyright/" target="_blank">Copyright</a></li>
                                                <li><a href="http://www.cipc.co.za/index.php/faqs/patents/" target="_blank">Patents</a></li>
                                                <li><a href="http://www.cipc.co.za/index.php/faqs/trade/" target="_blank">Trade Marks</a></li>
                                            </ul> 
                        
                                    </div>
                                    <div class="sidebar_base"></div>
                                </div>
                                <div class="sidebar">
                                    <div class="sidebar_top"></div>
                                    <div class="sidebar_item">
                                        <h3>Useful Links</h3>
                                        <ul>
                                            <li><a href="http://www.cipc.co.za/index.php/Access/reset-password/" target="_blank">Reset Password</a></li>
                                            <li><a href="http://www.cipc.co.za/index.php/Access/how-2/" target="_blank">How to - Step by Step Guides</a></li>
                                            <li><a href="http://www.cipc.co.za/index.php/track-progress-application/" target="_blank">Track my Transaction</a></li>
                                        </ul>
                                    </div>
                                    <div class="sidebar_base"></div>
                                </div>
                            </div>
                            &nbsp;
                        </td>
                    </tr>
                    <%--<tr>
                          <td class="td"><asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Width="100%"></asp:Label></td>
                    </tr>--%>
                </table>

            </div>
</asp:Content>
