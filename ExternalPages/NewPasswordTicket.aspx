<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewPasswordTicket.aspx.cs" Inherits="NewBilletterie.NewPasswordTicket" MaintainScrollPositionOnPostback="true" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <div id="main">

           <div id="site_content1">
                
            <div id="content1">
                    <h1><asp:Literal ID="litCreateNew" runat="server" Text="Create Ticket"></asp:Literal><asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1>

                    <div class="form_ticket">
                        <table class="table2">

                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>

                           <tr>
                                <td>
                                    <asp:Label ID="lblUserGroup" runat="server" Text="User group " Visible="false"></asp:Label>
                                </td>
                                <td colspan="5">
                                    <asp:DropDownList ID="ddlUserGroup" Width="150px" runat="server" Visible="false" AutoPostBack="false" Enabled="false"></asp:DropDownList>
                                </td>
                            </tr>
                            
                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="Subject "></asp:Label>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="txtTicketSubject" Width="800px" CssClass="largertextbox" runat="server" placeholder="Enter ticket subject." ToolTip="Use this field to provide the title/subject of the query. Maximum is 150 characters." Visible="true" AutoCompleteType="None" MaxLength="150" Enabled="false"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtTicketSubject" ValidationGroup="SubmitTicket" Display="Dynamic" ErrorMessage="Ticket subject is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="Reference No"></asp:Label>
                                </td>
                                <td colspan="5">
                                    <asp:TextBox ID="txtReferenceNo" Width="800px" CssClass="largertextbox" runat="server" placeholder="Enter tracking number, enterprise number, application number or case number if any." ToolTip="This field is optional. Maximum is 50 characters." Visible="true" AutoCompleteType="None" Enabled="false" MaxLength="100"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label runat="server" Text="Province"></asp:Label>
                                </td>
                                <td colspan="5">
                                    <asp:DropDownList ID="ddlProvince" Width="150px" runat="server" Visible="true" AutoPostBack="false" Enabled="true"></asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>

                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Message "></asp:Label>
                                 </td>
                                <td colspan="5">
                                    <asp:TextBox ID="txtTicketMessage" Width="800px" CssClass="largertextbox" placeholder="Add message relating to your password reset request." runat="server" Columns="12" TextMode="MultiLine" Visible="true" MaxLength="2500" ToolTip="2500 maximum characters" AutoCompleteType="None" Height="150px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTicketMessage" ValidationGroup="SubmitTicket" Display="Dynamic" ErrorMessage="Ticket message is required." ForeColor="Red" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>
                            <tr>
                                 <td>
                                     &nbsp;
                                 </td>
                                <td colspan="5">
                                    <asp:Label runat="server" Font-Bold="true" Text="You need to attach a CERTIFIED ID COPY (not older than 3 months) to be able to proceed. Select Add files and attach EnableTheming required document."></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                 <td>
                                     <asp:Label runat="server" Text="Attach Files "></asp:Label>
                                 </td>
                                <td colspan="5">
                                    [ <img src="../Images/icon_add.gif" />
                                    <asp:LinkButton ID="lnkAttachFiles" runat="server" CausesValidation="false" Text="Add Files" OnClick="lnkAttachFiles_Click" >
                                    </asp:LinkButton>
                                     ] <asp:Label runat="server" ID="lblAllowedExtentions" Text="" Font-Italic="true"></asp:Label>
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>

                           <tr id="imageUploadRow">
                               <td>
                                   <asp:Label runat="server"></asp:Label>
                               </td>
                                <td colspan="5" class="td2">
                                    
                                     <fieldset id="attachFieldset">
                                        <legend></legend>
                                            <table class="smalltable">
                                                <tr class="tr4">
                                                    <td class="td22">
                                                        <asp:LinkButton ID="lnkDeleteAttachedFiles" runat="server" CausesValidation="false" OnClick="lnkDeleteAttachedFiles_Click" Text="">
                                                            <asp:Image runat="server" ID="imgDelete" ImageUrl="../Images/icon_trash.gif" Width="20" Height="20" ImageAlign="Bottom" />
                                                        </asp:LinkButton>
                                                    </td>
                                                    <td class="td2">                                    
                                                        &nbsp;&nbsp;&nbsp;<asp:FileUpload ID="fupAttachFile" AllowMultiple="false" runat="server" Width="100%"/>
                                                    </td>
                                                </tr>
                                            </table>
                                    </fieldset>
                                 
                                </td>
                            </tr>

                            <tr>
                                 <td colspan="6">&nbsp;</td>
                            </tr>

                            <tr>

                                <td>CAPTCHA</td>
                                 <td colspan="5">
                                  <fieldset style="width: 100%;">
                                    <legend>Enter the text in the image shown</legend>
                                             <table class="tableX">
                                                <tr><td colspan="6">&nbsp;</td></tr>
                                                <tr>
                                                    <td>&nbsp;&nbsp;</td>
                                                    <td><img src="../GetCaptcha.ashx" width="110" height="40" /></td>
                                                    <td>&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtCaptchaText" AutoCompleteType="Disabled" MaxLength="6" Font-Size="Large" Font-Bold="true" Width="150px" runat="server" CssClass="textboxCaptcha"></asp:TextBox></td>
                                                    <td>&nbsp;&nbsp;&nbsp;<asp:Label ID="Label18" runat="server" Text="Can't read it?"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                                                    <td><asp:Button BackColor="PaleGreen" ID="btnNewCaptcha" runat="server" CssClass="captchabutton" Text="Try another one." OnClick="btnNewCaptcha_Click" Height="34px" Width="170px" CausesValidation="false" /></td>
                                                    <td>&nbsp;&nbsp;&nbsp;<asp:Label ID="lblStatus" runat="server" Font-Bold="true"></asp:Label></td>
                                                </tr>
                                                 <tr><td colspan="6"><asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtCaptchaText" ValidationGroup="SubmitTicket" Display="Dynamic" ErrorMessage="Please type in the text in the image above." ForeColor="Red" runat="server" /></td></tr>
                                            </table>
                                </fieldset>
                                 </td>
                            </tr>

                            <tr>
                                 <td colspan="6">&nbsp;<br /><br /></td>
                            </tr>

                            <tr>
                                 <td>&nbsp;</td>
                                <td>
                                    <asp:Button ID="btnSubmitTicket" runat="server" Width="100px" CssClass="insideButtons"  ValidationGroup="SubmitTicket"  Text="Submit" OnClick="btnSubmitTicket_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td colspan="4">
                                    &nbsp;<%--<asp:Button BackColor="#F37636" ForeColor="White" CssClass="toogleButtons" ID="btnViewOldTickets" runat="server" Width="170px" Text="View old tickets" ToolTip="View tickets from the old ticketing system." OnClientClick="window.open('../Legacy/ViewOldTickets.aspx', 'View Old Tickets');" Visible="true" />--%>
                                </td>
                            </tr>

                            <!-- Ticket confirmation popup -->
                            <tr>
                                 <td colspan="6">                                                    
                                    <asp:Button ID="btnSuccess" runat="server" style="display:none" />
                                    <cc1:ModalPopupExtender ID="ModalPopupExtenderSuccess" runat="server" TargetControlID="btnSuccess" PopupControlID="pnlSuccessPopup"
                                        CancelControlID="btnCancelOK" BackgroundCssClass="modalBackground">
                                    </cc1:ModalPopupExtender>

                                    <asp:Panel ID="pnlSuccessPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Height="200px" Width="550px" style="display:none">
                                            <table class="popTableV">

                                            <tr style="background-color:#007073">
                                                <td style=" height:10%; color:#349748; font-weight:bold; font-size:larger" class="popCell">
                                                <asp:Label ID="lblSuccessHeading" ForeColor="#E4EC04" runat="server" Text="Ticket successfully created."></asp:Label>
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
                                                            <td colspan="2"><asp:Label runat="server" ID="lblTicketConfirmation"></asp:Label> </td>
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
                                 <td colspan="6" class="td"><asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Width="300px"></asp:Label></td>
                            </tr>
                       </table>
                    
                    </div>       
   
                </div>

            </div>

      </div>
</asp:Content>
