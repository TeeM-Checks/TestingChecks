<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PasswordResetLogin.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="NewBilletterie.PasswordResetLogin" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
                    <div id="site_content">

                <table style="width:100%">
                    <tr>
                        <td style="width:100%">
                          <%--<div id="content">--%>
                            <h1>Customer details update/ Password Recovery</h1>

                            <div class="form_settings">
                                <table >
                                    <tr>
                                         <td style="width:25%">&nbsp;</td><td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                         <td>
                                             <asp:Label runat="server" Text="CIPC Customer Code"></asp:Label>
                                         </td>
                                        <td>
                                            <asp:TextBox ID="txtUsername" CssClass="largertextbox" runat="server" placeholder="CIPC Customer Code" ToolTip="Please use your username or use alternative credentials provided to you." Visible="true" MaxLength="12" AutoCompleteType="None"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtUsername"  Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red" runat="server" ValidationGroup="UserLogin" />
                                        </td>
                                    </tr>
                                    <tr>
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>
                                    <tr>
                                         <td>
                                             <asp:Label runat="server" Text="SA ID No"></asp:Label>
                                         </td>
                                        <td>
                                            <asp:TextBox ID="txtIDNumber" CssClass="largertextbox" runat="server" placeholder="SA ID or Foreign passport number" ToolTip="Please use your SA Greenbook ID Number or Passport Number that you used to register with CIPC." Visible="true" MaxLength="50" AutoCompleteType="None"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtIDNumber"  Display="Dynamic" ErrorMessage="ID /Passport Number number is required." ForeColor="Red" runat="server" ValidationGroup="UserLogin" />
                                        </td>
                                    </tr>
                                    <tr>
                                         <td>&nbsp;</td>
                                        <td>&nbsp;</td>
                                    </tr>

                                    <tr>
                                         <td>
                                             <asp:Label runat="server" Text="Names"></asp:Label>
                                         </td>
                                        <td>
                                            <asp:TextBox ID="txtNames" CssClass="largertextbox" runat="server" placeholder="First name and surname" ToolTip="Please provide your first name and surname." Visible="true" MaxLength="250" AutoCompleteType="None"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtNames"  Display="Dynamic" ErrorMessage="Name is required." ForeColor="Red" runat="server" ValidationGroup="UserLogin" />
                                        </td>
                                    </tr>

                                    <tr id="trBlankRow1" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>

                                    <br />

                                    <tr id="tr1" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                         <td>&nbsp;</td>
                                         <td>&nbsp;</td>
                                    </tr>

                                    <tr id="trVerifyButtons" runat="server" visible="true">
                                         <td>&nbsp;</td>
                                        <td class="buttonTD">
                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnVerify" runat="server" Width="80px" ToolTip="Verify your CIPC credentials." Text="Verify" OnClick="btnVerify_Click" ValidationGroup="UserLogin" />&nbsp;&nbsp;&nbsp;
                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelVerify" runat="server" Width="80px" ToolTip="Cancel" Text="Cancel" OnClick="btnCancelVerify_Click" CausesValidation="false" /><br />
                                        </td>
                                    </tr>


                                    <tr id="trEmailAccount" runat="server" visible="false">
                                         <td>
                                             <asp:Label runat="server" Text="Email Account"></asp:Label>
                                         </td>
                                        <td>
                                            <asp:TextBox ID="txtEmailAccount" CssClass="largertextbox" runat="server" placeholder="Email account" ToolTip="Please provide the exact email address that you used to register with CIPC." Visible="true" MaxLength="150"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtEmailAccount" ForeColor="Red" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="Invalid email address" ValidationGroup="UserLogin"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>

                                    <tr id="trPhoneNumber" runat="server" visible="false">
                                         <td>
                                             <asp:Label runat="server" Text="Phone Number"></asp:Label>
                                         </td>
                                        <td>
                                            <asp:TextBox ID="txtMobileNo" CssClass="largertextbox" runat="server" placeholder="Mobile phone number" ToolTip="Please provide the exact mobile phone number that you used to register with CIPC." Visible="true" MaxLength="50"></asp:TextBox>
                                            <asp:RegularExpressionValidator ValidationGroup="UserLogin" ID="RegularExpressionValidator6" ControlToValidate="txtMobileNo" ForeColor="Red" runat="server" ValidationExpression="^[0-9+xX\x20\x28\x29\x2D\x2E]*$" ErrorMessage="Invalid phone number"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>


                                    <tr id="trBlankRow2" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>

                                    <tr id="trProvince" runat="server" visible="false">
                                         <td>
                                             <asp:Label runat="server" Text="Province"></asp:Label>
                                         </td>
                                        <td>
                                            <asp:DropDownList ID="ddlProvince" Width="150px" runat="server" Visible="true" AutoPostBack="false" Enabled="true"></asp:DropDownList>
                                        </td>
                                    </tr>

                                    <tr id="trBlankRow3" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>

                                    <tr id="trPhysicalAddress" runat="server" visible="false">
                                         <td>
                                             <asp:Label runat="server" Text="Physical Address"></asp:Label>
                                         </td>
                                         <td>
                                            <table style="width:100%">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtStreetName1" CssClass="largertextbox" runat="server" placeholder="Street number and name" ToolTip="Street number and name" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtStreetName2" CssClass="largertextbox" runat="server" placeholder="" ToolTip="" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtCityTown" CssClass="largertextbox" runat="server" placeholder="Town / City" ToolTip="Town / City" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtStateProvince" CssClass="largertextbox" runat="server" placeholder="State / Province" ToolTip="State / Province" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtPostalCode" CssClass="largertextbox" runat="server" placeholder="Postal / ZIP code" ToolTip="Postal / ZIP code" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>


                                    <tr id="trBlankRow4" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>

                                    <tr id="trPostalAddress" runat="server" visible="false">
                                         <td>
                                             <asp:Label runat="server" Text="Postal Address"></asp:Label>
                                         </td>
                                         <td>
                                            <table style="width:100%">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtPostStreetName1" CssClass="largertextbox" runat="server" placeholder="PO Box / Private Bag / Street address" ToolTip="PO Box / Private Bag / Street address" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtPostStreetName2" CssClass="largertextbox" runat="server" placeholder="" ToolTip="" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtPostCityTown" CssClass="largertextbox" runat="server" placeholder="Town / City" ToolTip="Town / City" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtPostStateProvince" CssClass="largertextbox" runat="server" placeholder="State / Province" ToolTip="State / Province" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr><td>&nbsp;</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtPostPostalCode" CssClass="largertextbox" runat="server" placeholder="Postal / ZIP code" ToolTip="Postal / ZIP code" Visible="true"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>


                                    <tr id="trBlankRow5" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>

                                    <tr id="trPreviousCodes" runat="server" visible="false">
                                         <td>
                                             <asp:Label runat="server" Text="Previous Customer Codes?" ToolTip="Tick check box for YES"></asp:Label>
                                         </td>
                                         <td>
                                            <table style="width:100%">
                                                <tr>
                                                    <td style="display:inline-block; text-align:right;">
                                                        <asp:CheckBox runat="server" ID="chkPreviousCodes" Width="20px" ToolTip="Tick for YES" TextAlign="Left" OnCheckedChanged="chkPreviousCodes_CheckedChanged" AutoPostBack="true" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPreviousCC" CssClass="largertextbox" runat="server" placeholder="Previous customer codes" ToolTip="" Visible="false" Width="100%"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtPreviousCC"  Display="Dynamic" ErrorMessage="Customer Code is required." ForeColor="Red" runat="server" ValidationGroup="PreviousCC" />
                                                    </td>
                                                    <td>
                                                        &nbsp;&nbsp;
                                                        <asp:Button runat="server" ID="btnFindCC" Text="Find" ValidationGroup="UserLogin" Visible="false" ToolTip="Find previous Customer Codes" Width="50px" BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" OnClick="btnFindCC_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>

                                    <tr id="trPreviousCodesGrid" runat="server" visible="false">
                                         <td>
                                             &nbsp;
                                         </td>
                                        <td>
                                            <asp:GridView HeaderStyle-Height="30" BorderWidth="1px" RowStyle-Height="30" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="GridViewPreviousCodes" AutoGenerateColumns="false" AllowPaging="true" PageSize="5" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="100%" runat="server" AllowSorting="False" OnPageIndexChanging="GridViewPreviousCodes_PageIndexChanging" OnRowDataBound="GridViewPreviousCodes_RowDataBound" OnRowCommand="GridViewPreviousCodes_RowBoundOperations">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Agent Code" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="25%" ItemStyle-Width="25%" ItemStyle-Wrap="true">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkAgentCodeLink" runat="server" CausesValidation="false" CommandArgument='<%#Eval("agent_code")%>' Text='<%#Eval("agent_code")%>' OnClick="lnkAgentCodeLink_Click" CssClass="lnkStyle"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                     <asp:BoundField HeaderText="Names" DataField="agent_name" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="true"></asp:BoundField>
                                                    <asp:BoundField HeaderText="Email" DataField="email" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="true"></asp:BoundField>
                                                    <asp:BoundField HeaderText="Status" DataField="status" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="15%" ItemStyle-Width="15%" ItemStyle-Wrap="true"></asp:BoundField>
                                                    <asp:BoundField HeaderText="Cell Number" DataField="cell_no" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="15%" ItemStyle-Width="15%" ItemStyle-Wrap="true"></asp:BoundField>
                                                    <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/deleteicon.png" Width="25" Height="25" BackColor="Transparent" BorderStyle="None" ToolTip="De-activate account" ImageAlign="Middle" CommandArgument='<%#Eval("agent_code")%>' CommandName="DeleteAgentCode" AlternateText="Delete"></asp:ImageButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>

                                                <PagerSettings PageButtonCount="5"></PagerSettings>
                                                <AlternatingRowStyle BackColor="White" />
                                                <EditRowStyle BackColor="#7C6F57" />
                                                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                                <HeaderStyle BackColor="#1C5E55" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BorderStyle="None" BorderWidth="0px" Height="30" BackColor="#666666" ForeColor="White" CssClass="gridview" HorizontalAlign="Left" />
                                                <RowStyle Font-Size="12px" BackColor="#E3EAEB" />
                                                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                                <SortedAscendingHeaderStyle BackColor="#246B61" />
                                                <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                                <SortedDescendingHeaderStyle BackColor="#15524A" />

                                            </asp:GridView>                                               
                                        </td>
                                    </tr>

                                    <tr id="trBlankRow6" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>

                                    <tr id="trDisclaimer" runat="server" visible="false">
                                         <td colspan="2">
                                             <asp:Label runat="server" Font-Bold="true" Text="By clicking on AGREE you confirm that the Terms & Conditions have been read and understood and that the information provides is both true and correct. Providing false information is an offence in terms of Sect 215(2) of the Companies Act, 71 of 2008."></asp:Label><br />
                                             <table style="width:100%">

                                                 <tr>
                                                     <td>
                                                         <asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small" Visible="false"></asp:Label>
                                                     </td>
                                                 </tr>

                                                <tr style="text-align:center;">
                                                    <td style="text-align:right;"><asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnAgree" runat="server" Width="100px" ToolTip="Agree to Terms and Conditions" Text="I Agree" OnClick="btnAgree_Click" />&nbsp;&nbsp;&nbsp;</td>
                                                    <td style="text-align:left;"><asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnDisagree" runat="server" Width="100px" ToolTip="Cancel" Text="I Disagree" OnClick="btnDisagree_Click" CausesValidation="false" /></td>
                                                </tr>
                                            </table>
                                         </td>
                                    </tr>

                                    <tr id="trBlankRow7" runat="server" visible="false">
                                         <td>&nbsp;</td><td>&nbsp;</td>
                                    </tr>

                                    <tr id="trSubmitButtons" runat="server" visible="false">
                                         <td>&nbsp;</td>
                                        <td class="buttonTD">
                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnContinue" runat="server" Width="80px" ToolTip="Verify your CIPC credentials." Text="Continue" OnClick="btnContinue_Click" ValidationGroup="UserLogin" />&nbsp;&nbsp;&nbsp;
                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancel" runat="server" Width="80px" ToolTip="Cancel" Text="Cancel" OnClick="btnCancel_Click" CausesValidation="false" /><br />
                                        </td>
                                    </tr>

                                    <tr>
                                         <td colspan="2">For more information about password requests, <a href="http://www.cipc.co.za/index.php/Access/reset-password/" target="_blank">click here</a></td>
                                    </tr>


                                 <%--Dummy officer password reset--%>
                                 <tr>
                                     <td class="valignTableTop" colspan="4">
                                        <asp:Button ID="btnShowPasswordChangePopup" runat="server" style="display:none" />
                                        <cc1:ModalPopupExtender ID="ModalPopupExtenderChangePassword" runat="server"  TargetControlID="btnShowPasswordChangePopup" PopupControlID="pnlChangePasswordPopup"
                                            CancelControlID="btnCancelEdit"  BackgroundCssClass="modalBackground">
                                        </cc1:ModalPopupExtender>
                                        <asp:Panel ID="pnlChangePasswordPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="500px" style="display:none">
                                                <table class="popTableV">
                                                <tr style="background-color:#007073">
                                                    <td colspan="2" style=" height:20px; color:#349748; font-weight:bold; font-size:larger" class="popCell">
                                                        <asp:Label ID="lblChangePasswordHeading" ForeColor="#E4EC04" runat="server"></asp:Label>
                                                        <asp:Label ID="lblChangePasswordOFCPKID" runat="server" Visible="false"></asp:Label>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td colspan="2" class="popCell">&nbsp;</td>
                                                </tr>

                                                <%--Main details row--%>
                                                <tr>
                                                    <td colspan="2" class="popCellLeft">
                                                        <hr class="hr1" />
                                                        <table>

                                                            <%--Username row--%>
                                                            <tr>
                                                                <td class="tddd">
                                                                    <asp:Label runat="server" ID="lblPopUserID" Font-Bold="true" Text="User Login ID"  Width="150px"></asp:Label>
                                                                </td>
                                                                <td class="td">
                                                                    <asp:TextBox runat="server" ID="txtPopUserID" MaxLength="25" CssClass="largertextbox" placeholder="Enter your user id." Height="25px" Width="350px"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtPopUserID"  Display="Dynamic" ErrorMessage="Username is required." ForeColor="Red" runat="server" ValidationGroup="UserLoginPop" />
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtPopUserID" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Username must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>

                                                            <%--Old Password row--%>
                                                            <tr>
                                                                <td class="tddd">
                                                                    <asp:Label runat="server" ID="lblPopOldPassword" Font-Bold="true" Text="Current Password" Width="150px"></asp:Label>
                                                                </td>
                                                                <td class="td">
                                                                    <asp:TextBox runat="server" ID="txtPopOldPassword" MaxLength="15" CssClass="largertextbox" placeholder="Enter your current password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtPopOldPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                    <asp:Label runat="server" ID="lblPopOldPasswordBlank" Text="Leave blank" ForeColor="Red" Visible="false"></asp:Label>
                                                                </td>
                                                            </tr>

                                                            <%--New Password row--%>
                                                            <tr>
                                                                <td class="tddd">
                                                                    <asp:Label runat="server" ID="lblPopNewPassword" Font-Bold="true" Text="New Password"  Width="150px"></asp:Label>
                                                                </td>
                                                                <td class="td">
                                                                    <asp:TextBox runat="server" ID="txtPopNewPassword" MaxLength="15" CssClass="largertextbox" placeholder="Enter your new password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPopNewPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                    <asp:Label runat="server" ID="lblPopNewPasswordBlank" Text="Leave blank" ForeColor="Red" Visible="false"></asp:Label>
                                                                </td>
                                                            </tr>

                                                            <%--New Password confirmation row--%>
                                                            <tr>
                                                                <td class="tddd">
                                                                    <asp:Label runat="server" ID="lblPopConfirmPassword" Font-Bold="true" Text="Confirm Password"  Width="150px"></asp:Label>
                                                                </td>
                                                                <td class="td">
                                                                    <asp:TextBox runat="server" ID="txtPopConfirmPassword" MaxLength="15" CssClass="largertextbox" placeholder="Repeat your new password here." TextMode="Password" Height="25px" Width="350px"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPopConfirmPassword" ForeColor="Red" runat="server" ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Password must be alphanumeric only" ValidationGroup="UserLoginPop"></asp:RegularExpressionValidator>
                                                                    <asp:Label runat="server" ID="lblPopConfirmPasswordBlank" Text="Leave blank" ForeColor="Red" Visible="false"></asp:Label>
                                                                </td>
                                                            </tr>

                                                            <tr><td colspan="2">&nbsp;</td></tr>

                                                            <tr>
                                                                <td  class="tddd">
                                                                    <asp:Label runat="server" ID="lblForgotCheckBox" Font-Bold="true" Text="Forgot your current password?" Width="150px"></asp:Label>
                                                                </td>
                                                                <td  class="td">
                                                                    <asp:CheckBox runat="server" ID="chkForgotPassword" TextAlign="Right" Checked="false" AutoPostBack="true" Width="500px" OnCheckedChanged="chkForgotPassword_CheckedChanged"/>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td colspan="2">
                                                                    <asp:Label ID="lblGridRowError" Visible="false" runat="server" Text="" ForeColor="Red" ></asp:Label>
                                                                </td>
                                                            </tr>

                                                            <tr><td colspan="2"><hr class="hr2" /></td></tr>

                                                            <tr>
                                                                <td colspan="2" class="td4">
                                                                    <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnChangePassword" ValidationGroup="UserLoginPop" runat="server" Width="100px" Text="Submit" Visible="true" CausesValidation="true" OnClick="btnChangePassword_Click"/>&nbsp; 
                                                                    <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelEdit" runat="server" Width="100px" Text="Cancel" Visible="true" CausesValidation="true" />&nbsp;
                                                                </td>
                                                            </tr>
                                                                                                              
                                                        </table>
                                                    </td>
                                                </tr>

                                                <%--Close popup bottom--%>
                                                <tr style="background-color:#6BAB4D" class="popRow">
                                                    <td colspan="2" class="popCellCenter">
                                                        &nbsp;
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
                </table>

            </div>
</asp:Content>
