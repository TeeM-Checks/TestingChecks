<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewTicket.aspx.cs" Inherits="NewBilletterie.NewTicket" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        .pager span {
            color: #f1ff2f;
            font-weight: bold;
            font-size: 16pt;
        }

        .checkbox label {
            padding-left: 5px;
        }
    </style>

    <div id="main">

        <div id="site_content1">

            <div id="content1">
                <h1>
                    <asp:Literal ID="litCreateNew" runat="server" Text="Create Ticket"></asp:Literal>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label>
                </h1>

                <div class="form_ticket">
                    <table class="table2">
                        <tr class="tr">
                            <td class="td2">
                                <asp:Label runat="server" ID="lblDDLDepartment" Width="90px" Text="Department "></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlDepartment" Width="150px" runat="server" Visible="true" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                            <td class="td3">
                                <asp:Label runat="server" ID="lblDDLCategory" Width="100px" Visible="false" Text="Category "></asp:Label>&nbsp;&nbsp;
                            </td>
                            <td class="td2">
                                <asp:DropDownList ID="ddlCategory" Width="200px" runat="server" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                            <td class="td3" nowrap="true">
                                <asp:Label runat="server" ID="lblDDLSubCategory" Width="100px" Visible="false" Text="Sub Category "></asp:Label>&nbsp;&nbsp;
                            </td>
                            <td class="td2">
                                <asp:DropDownList ID="ddlSubCategory" Width="200px" runat="server" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddlSubCategory_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                        </tr>

                        <tr runat="server" id="deptToolTipDisplayRowEmpty" visible="false">
                            <td colspan="6">&nbsp;</td>
                        </tr>

                        <tr runat="server" id="deptToolTipDisplayRow" visible="false">
                            <td>&nbsp;</td>
                            <td colspan="5" style="border: 10px; background-color: #cd9148;">
                                <asp:Label runat="server" ID="lblCategoryToolTip" Text="" ForeColor="White" Visible="true"></asp:Label></td>
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
                                <asp:TextBox ID="txtTicketSubject" Width="800px" CssClass="largertextbox" runat="server" placeholder="Enter ticket subject." ToolTip="Use this field to provide the title/subject of the query. Maximum is 150 characters." Visible="true" AutoCompleteType="None" MaxLength="150"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtTicketSubject" ValidationGroup="SubmitTicket" Display="Dynamic" ErrorMessage="Ticket subject is required." ForeColor="Red" runat="server" />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="6">&nbsp;</td>
                        </tr>

                        <tr>
                            <td>
                                <asp:Label runat="server" Text="Enterprise No"></asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtEnterpriseNo" Width="300px" CssClass="largertextbox" runat="server" placeholder="Enter enterprise number if any." ToolTip="This field is optional. Maximum is 50 characters." Visible="true" AutoCompleteType="None" MaxLength="50"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label runat="server" Text="Reference No"></asp:Label>
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtReferenceNo" Width="300px" CssClass="largertextbox" runat="server" placeholder="Enter reference number if any." ToolTip="This field is optional. Maximum is 50 characters." Visible="true" AutoCompleteType="None" MaxLength="50"></asp:TextBox>
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
                                <asp:TextBox ID="txtTicketMessage" Width="800px" CssClass="largertextbox" runat="server" Columns="12" TextMode="MultiLine" Visible="true" MaxLength="2500" ToolTip="2500 maximum characters" AutoCompleteType="None" Height="150px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTicketMessage" ValidationGroup="SubmitTicket" Display="Dynamic" ErrorMessage="Ticket message is required." ForeColor="Red" runat="server" />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="6">&nbsp;</td>
                        </tr>

                        <tr>
                            <td>
                                <asp:Label runat="server" Text="Attach Files "></asp:Label>
                            </td>
                            <td colspan="5">
                                <asp:Label runat="server" ID="lblAllowedExtentions" Text="" Font-Italic="true"></asp:Label>
                                <asp:Label runat="server" ID="lblSizeLimit" Text="" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="6">&nbsp;</td>
                        </tr>


                        <tr>
                            <td>
                                <asp:Label runat="server"></asp:Label>
                            </td>
                            <td colspan="5" class="td2">

                                <table class="smalltable">
                                    <tr class="tr4">
                                        <td class="td22Top">&nbsp;
                                        </td>
                                        <td class="td2">
                                            <table style="width: 100%">
                                                <tr style="align-content: center; vertical-align: middle;">
                                                    <td style="align-content: center; vertical-align: middle;">&nbsp;&nbsp;&nbsp;
                                                        <asp:FileUpload ID="fupAttachFile" AllowMultiple="false" runat="server" Width="100%" />
                                                    </td>
                                                    <td style="align-content: center; vertical-align: bottom;">&nbsp;&nbsp;&nbsp;
                                                        <asp:Button runat="server" ID="btnSaveUploadedFile" OnClick="btnSaveUploadedFile_Click" Width="100px" BackColor="#F37636" CssClass="insideButtons" Text="Upload" />
                                                    </td>
                                                </tr>
                                                <tr id="imageUploadRows">
                                                    <td colspan="2">
                                                        <asp:GridView HeaderStyle-Height="20" BorderWidth="1px" RowStyle-Height="20" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="GridViewUploadedDocs" AutoGenerateColumns="false" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="250px" runat="server" AllowPaging="true" PageSize="5" AllowSorting="True" OnRowCommand="GridViewUploadedDocs_RowBoundOperations" OnRowDataBound="GridViewUploadedDocs_RowDataBound">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Document Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="true" HeaderStyle-Width="140px">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkOFCPKIDLink" runat="server" CommandArgument='<%#Eval("originalFileName")%>' Text='<%#Eval("originalFileName")%>' ForeColor="Blue" CssClass="lnkStyle" Enabled="false"></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                                                    <ItemTemplate>
                                                                        <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/deleteicon.png" Width="32" Height="32" BackColor="Transparent" BorderStyle="None" ToolTip="Delete this attachment" ImageAlign="Middle" CommandArgument='<%#Eval("derivedFileName")%>' CommandName="DeleteAttachement" AlternateText="Delete"></asp:ImageButton>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <PagerSettings PageButtonCount="5"></PagerSettings>
                                                            <AlternatingRowStyle BackColor="White" />
                                                            <EditRowStyle BackColor="#7C6F57" />
                                                            <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                                            <HeaderStyle BackColor="#1C5E55" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                                            <PagerStyle BorderStyle="None" BorderWidth="0px" Height="20" BackColor="#666666" ForeColor="White" CssClass="gridview" HorizontalAlign="Left" />
                                                            <RowStyle Font-Size="12px" BackColor="#E3EAEB" />
                                                            <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                                            <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                                            <SortedAscendingHeaderStyle BackColor="#246B61" />
                                                            <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                                            <SortedDescendingHeaderStyle BackColor="#15524A" />
                                                        </asp:GridView>
                                                        <br />
                                                        <asp:Button runat="server" ID="btnCancelAllUpload" OnClick="btnCancelAllUpload_Click" Width="100px" BackColor="#F37636" CssClass="insideButtons" ToolTip="Cancel all attachements" Visible="false" Text="Cancel" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>

                            </td>
                        </tr>

                        <tr>
                            <td colspan="6">&nbsp;</td>
                        </tr>

                        <tr>
                            <td colspan="6">CAPTCHA - Enter the text in the image shown</td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <fieldset style="width: 100%;">
                                    <legend style="font-size: small;"></legend>
                                    <table class="tableX">
                                        <tr>
                                            <td colspan="6">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;&nbsp;</td>
                                            <td>
                                                <img src="../GetCaptcha.ashx" width="110" height="40" /></td>
                                            <td>&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtCaptchaText" MaxLength="6" Font-Size="Large" Font-Bold="true" Width="150px" runat="server" CssClass="textboxCaptcha"></asp:TextBox></td>
                                            <td>&nbsp;&nbsp;&nbsp;<asp:Label ID="Label18" runat="server" Text="Can't read it?"></asp:Label>&nbsp;&nbsp;&nbsp;</td>
                                            <td>
                                                <asp:Button BackColor="PaleGreen" ID="btnNewCaptcha" runat="server" CssClass="captchabutton" Text="Try another one." OnClick="btnNewCaptcha_Click" Height="34px" Width="170px" CausesValidation="false" /></td>
                                            <td>&nbsp;&nbsp;&nbsp;<asp:Label ID="lblStatus" runat="server" Font-Bold="true"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td colspan="6">
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtCaptchaText" ValidationGroup="SubmitTicket" Display="Dynamic" ErrorMessage="Please type in the text in the image above." ForeColor="Red" runat="server" /></td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="6">&nbsp;</td>
                        </tr>

                        <tr>
                            <td colspan="6">&nbsp;<br />
                                <br />
                            </td>
                        </tr>


                        <tr>
                            <td colspan="6">
                                <asp:Button ID="btnSubmitTicket" runat="server" Width="100px" CssClass="insideButtons" ValidationGroup="SubmitTicket" Text="Submit" OnClick="btnSubmitTicket_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnRequestRefund" runat="server" Width="120px" CssClass="insideButtons" Text="Request refund" OnClick="btnRequestRefund_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            </td>
                        </tr>



                        <!-- Ticket confirmation popup -->
                        <tr>
                            <td colspan="6">
                                <asp:Button ID="btnSuccess" runat="server" Style="display: none" />
                                <cc1:ModalPopupExtender ID="ModalPopupExtenderSuccess" runat="server" TargetControlID="btnSuccess" PopupControlID="pnlSuccessPopup"
                                    CancelControlID="btnCancelOK" BackgroundCssClass="modalBackground">
                                </cc1:ModalPopupExtender>

                                <asp:Panel ID="pnlSuccessPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="550px" Style="display: none">
                                    <table class="popTableV">

                                        <tr style="background-color: #007073">
                                            <td style="height: 10%; color: #349748; font-weight: bold; font-size: larger" class="popCell">
                                                <asp:Label ID="lblSuccessHeading" ForeColor="#E4EC04" runat="server" Text="Ticket successfully created."></asp:Label>
                                                <asp:Label ID="lblCommandName" Visible="false" Text="Ticket" ForeColor="#E4EC04" runat="server"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td colspan="2" class="popCell">&nbsp;</td>
                                        </tr>

                                        <tr>
                                            <td colspan="2" class="popCellLeft">
                                                <hr class="hr1" />
                                                <div id="prinatbleDiv">
                                                    <table class="insideTable" style="width: 92%; margin-left: 5%;">

                                                        <%--Subject row--%>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:Label runat="server" ID="lblTicketConfirmation"></asp:Label>
                                                            </td>
                                                        </tr>

                                                    </table>
                                                </div>
                                            </td>
                                        </tr>

                                        <tr style="background-color: #007073" class="popRow">
                                            <td class="popCellCenter">
                                                <asp:Button ID="btnOK" runat="server" Text="OK" Width="50px" Height="25px" OnClick="btnOK_Click" />
                                                <asp:Button ID="btnCancelOK" runat="server" Text="" Width="1px" Height="1px" CssClass="hiddenClass" Visible="true" />
                                            </td>
                                        </tr>

                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="6" class="td">
                                <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Width="300px"></asp:Label></td>
                        </tr>


                        <%--Dummy row for Customer Bank Details Popup--%>
                        <tr>
                            <td class="valignTableTop" colspan="6">
                                <asp:Button ID="btnShowCustomerDetailPopup" runat="server" Style="display: none" />
                                <cc1:ModalPopupExtender ID="ModalPopupExtenderCustomerDetail" runat="server" TargetControlID="btnShowCustomerDetailPopup" PopupControlID="pnlCustomerDetailPopup"
                                    CancelControlID="btnCancelDetails" BackgroundCssClass="modalBackground">
                                </cc1:ModalPopupExtender>

                                <asp:Panel ID="pnlCustomerDetailPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="70%" Style="max-height: 650px; display: none">
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
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="popCellLeft">
                                                <asp:Panel ID="Panel4" runat="server" ScrollBars="Vertical" BackColor="Transparent" BorderStyle="None" BorderWidth="1" Width="100%" Style="max-height: 450px; min-height: 400px">
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td width="20%">
                                                                <div class="DetailLabel">&nbsp;Agent Account</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ReadOnly="true" ID="txtAgentAccount" BorderColor="DarkGray" Enabled="false" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td width="20%">
                                                                <div class="DetailLabel">&nbsp;Account Balance</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ReadOnly="true" ID="txtAccountBalance" BorderColor="DarkGray" Enabled="false" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;ID Number</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtIDNumber" placeholder="ID Number of the CIPC Customer Account holder" MaxLength="150" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
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
                                                                    <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtFullName" ForeColor="Red" runat="server" ValidationExpression="[a-zA-Z]" ErrorMessage="Invalid input" ValidationGroup="BankPopUpGroup"></asp:RegularExpressionValidator>--%>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td width="20%">
                                                                <div class="DetailLabel">&nbsp;Bank Name</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:DropDownList ID="ddlBankName" Width="98%" runat="server" Height="30px" AutoPostBack="true" Enabled="true" OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged">
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td width="20%">
                                                                <div class="DetailLabel">&nbsp;Branch Code</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:DropDownList ID="ddlBranchCode" Width="98%" runat="server" Height="30px" AutoPostBack="false" Enabled="true"></asp:DropDownList>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Account Holder</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtAccountHolder" MaxLength="150" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>


                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Account Number</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtAccountNumber" MaxLength="16" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator ID="regExAccountNumber" ControlToValidate="txtAccountNumber" ForeColor="Red" runat="server" ValidationExpression="\d+$" ErrorMessage="Please enter numeric values only" ValidationGroup="BankPopUpGroup"></asp:RegularExpressionValidator>

                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Contact Number</div>
                                                            </td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtCustContact" MaxLength="50" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
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
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Attach Files</div>
                                                            </td>
                                                            <td class="td4">
                                                                <table style="width: 100%;">
                                                                    <tr>
                                                                        <td style="vertical-align: middle;">
                                                                            <asp:FileUpload runat="server" ID="flpCustResponseUpload" Width="80%" AllowMultiple="false" BackColor="WhiteSmoke" CssClass="textarea" Visible="true"></asp:FileUpload>
                                                                        </td>
                                                                        <td>
                                                                            <asp:DropDownList ID="ddlCustFileType" Width="150px" runat="server" Height="30px" AutoPostBack="false" Enabled="true"></asp:DropDownList>
                                                                        </td>
                                                                        <td style="vertical-align: middle;">
                                                                            <asp:Button runat="server" ID="btnSaveUploadedCustFile" OnClick="btnSaveUploadedCustFile_Click" Width="100px" BackColor="#F37636" Text="Upload" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="3">
                                                                            <asp:Label ID="lblUploadErrorMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                <div class="DetailLabel">&nbsp;Attached Document</div>
                                                            </td>
                                                            <td>

                                                                <asp:GridView HeaderStyle-Height="20" BorderWidth="1px" RowStyle-Height="20" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="GridViewCustUploadedDocs" AutoGenerateColumns="false" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="80%" runat="server" AllowPaging="true" PageSize="5" AllowSorting="True" OnRowCommand="GridViewCustUploadedDocs_RowBoundOperations" OnRowDataBound="GridViewCustUploadedDocs_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Document Name" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="true" HeaderStyle-Width="140px">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="lnkOFCPKIDLink" runat="server" CommandName="OpenAttachement" CommandArgument='<%#Eval("TPD_FileURL")%>' Text='<%#Eval("TPD_OriginalFileName")%>' ForeColor="Blue" CssClass="lnkStyle" Enabled="false"></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="TPD_FileSize" ItemStyle-Wrap="true" HeaderText="Size (Kb)" HeaderStyle-Width="90px" />
                                                                        <asp:BoundField DataField="DCT_PKID" ItemStyle-Wrap="true" HeaderText="Document Type" HeaderStyle-Width="120px" />
                                                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                                                            <ItemTemplate>
                                                                                <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/deleteicon.png" Width="32" Height="32" BackColor="Transparent" BorderStyle="None" ToolTip="Delete this attachment" ImageAlign="Middle" CommandArgument='<%#Eval("TPD_PKID")%>' CommandName="DeleteAttachement" AlternateText="Delete"></asp:ImageButton>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                    <PagerSettings PageButtonCount="5"></PagerSettings>
                                                                    <AlternatingRowStyle BackColor="White" />
                                                                    <EditRowStyle BackColor="#7C6F57" />
                                                                    <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                                                    <HeaderStyle BackColor="#1C5E55" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                                                    <PagerStyle BorderStyle="None" BorderWidth="0px" Height="20" BackColor="#666666" ForeColor="White" CssClass="gridview" HorizontalAlign="Left" />
                                                                    <RowStyle Font-Size="12px" BackColor="#E3EAEB" />
                                                                    <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                                                    <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                                                    <SortedAscendingHeaderStyle BackColor="#246B61" />
                                                                    <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                                                    <SortedDescendingHeaderStyle BackColor="#15524A" />
                                                                </asp:GridView>
                                                                <asp:LinkButton runat="server" ID="lnkCancelCustAllUpload" OnClick="lnkCancelCustAllUpload_Click" Text="Delete All Attachments" Visible="false"></asp:LinkButton>
                                                            </td>
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
                                                            <%--<div runat="server" id="blinkText">--%>
                                                            <asp:Label ID="lblPopErrorMessage" Visible="false" runat="server" Text="" ForeColor="Red"></asp:Label>
                                                            <%-- </div>--%>
                                                        </td>
                                                    </tr>

                                                    <tr align="center">
                                                        <td>
                                                            <asp:Label ID="lblBottomMessage" runat="server" Visible="true" ForeColor="White"></asp:Label>

                                                        </td>
                                                    </tr>

                                                    <tr align="center">
                                                        <td>
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnSubmitBankDetails" runat="server" Width="120px" ValidationGroup="BankPopUpGroup" Text="Submit" Visible="true" OnClick="btnSubmitBankDetails_Click" CausesValidation="true" />&nbsp; 
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelDetails" runat="server" Width="120px" Text="Cancel" Visible="true" CausesValidation="true" />&nbsp;
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnCancelForever" runat="server" Width="150px" Text="Don't Ask Me Again" OnClick="btnCancelForever_Click" Visible="true" CausesValidation="true" />&nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>

                            </td>
                        </tr>

                    </table>

                </div>

            </div>

        </div>

    </div>
</asp:Content>
