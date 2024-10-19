<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewOldTickets.aspx.cs" Inherits="NewBilletterie.ViewOldTickets" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
           <div id="main">

            <div id="site_content2">
                
            <div id="content1">
                    <h1>View Old Tickets<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1>
                    <h4>
                        You currently have: <asp:Label ID="lblNoOfTickets" runat="server" Text="" ForeColor="Green" Font-Size="Large"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox runat="server" Text="   View open tickets only" ID="chkViewOpen" Checked="true" AutoPostBack="true" OnCheckedChanged="chkViewOpen_CheckedChanged"/>
                    </h4>

                    <div class="form_ticket">

                        <table class="table2">

                         <%--Grid view for results--%>
                         <tr>
                             <td colspan="4">
                                 
                                 <asp:GridView HeaderStyle-Height="30" BorderWidth="1px" RowStyle-Height="30" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="gridTickets" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="150%" runat="server" AllowSorting="True" OnPageIndexChanging="gridTickets_PageIndexChanging" OnRowDataBound="gridTickets_RowDataBound" OnRowCommand="gridTickets_RowBoundOperations">
                                    <Columns>
                                      <asp:TemplateField HeaderText="Ticket No" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%">
                                          <ItemTemplate>
                                              <asp:LinkButton ID="lnkTicketLink" runat="server" CommandName="OpenTicket" CommandArgument='<%#Eval("TCK_PKID")%>' OnClick="lnkTicketLink_Click" Text='<%#Eval("TCK_TicketNumber")%>' ForeColor="Blue" CssClass="lnkStyle"></asp:LinkButton>
                                          </ItemTemplate>
                                       </asp:TemplateField> 
                                        <asp:BoundField HeaderText="Subject" DataField="TCK_Subject" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Date Created" DataField="TCK_DateCreated" HeaderStyle-Width="20%" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Category" DataField="CAT_CategoryName" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" >
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Status" DataField="STS_StatusName" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        </asp:BoundField>
                                        <%--<asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                          <ItemTemplate>
                                              <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="~/Images/pencil.png" Width="16" Height="16" BackColor="Transparent" BorderStyle="None" ToolTip="Edit ticket" ImageAlign="Middle" CommandArgument='<%#Eval("TCK_PKID")%>' CommandName="EditTicket" AlternateText="Edit"></asp:ImageButton>
                                          </ItemTemplate>
                                       </asp:TemplateField>--%>
                                       <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                          <ItemTemplate>
                                              <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="~/Images/deleteicon.png" Width="25" Height="25" BackColor="Transparent" BorderStyle="None" ToolTip="Delete ticket" ImageAlign="Middle" CommandArgument='<%#Eval("TCK_PKID")%>' CommandName="DeleteTicket" AlternateText="Delete"></asp:ImageButton>
                                          </ItemTemplate>
                                       </asp:TemplateField>
                                   </Columns>

                                <PagerSettings PageButtonCount="5" ></PagerSettings>
                                <AlternatingRowStyle BackColor="White" />
                                <EditRowStyle BackColor="#7C6F57" />
                                <FooterStyle BackColor="#ABABAB" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#ABABAB" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                <PagerStyle  BorderStyle="None" BorderWidth="0px" Height="30"  BackColor="#666666" ForeColor="White" cssclass="gridview" HorizontalAlign="Left" />
                                <RowStyle Font-Size="12px" BackColor="#E3EAEB" />
                                <SelectedRowStyle BackColor="#FFF4DF" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                <SortedAscendingHeaderStyle BackColor="#246B61" />
                                <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                <SortedDescendingHeaderStyle BackColor="#15524A" />

                                </asp:GridView>
                                 <br />
                                 <asp:Label ID="lblDelEditWarning" runat="server" ForeColor="Green" Text="NB. Tickets from the old ticketing system are only available for viewing. CIPC officials are addressing tickets from both the new and the old system."></asp:Label>
                             </td>
                         </tr>

                         <%--Dummy row for Ticket details Popup--%>
                         <tr>
                             <td class="valignTableTop" colspan="4">
                                <asp:Button ID="btnShowTicketDetailPopup" runat="server" style="display:none" />
                                <cc1:ModalPopupExtender ID="ModalPopupExtenderTicketDetail" runat="server" TargetControlID="btnShowTicketDetailPopup" PopupControlID="pnlTicketDetailPopup"
                                    CancelControlID="imbTCKCloseBottom" BackgroundCssClass="modalBackground">
                                </cc1:ModalPopupExtender>

                                <asp:Panel ID="pnlTicketDetailPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="800px" style="display:none">
                                        <table class="popTableV">
                                        
                                        <tr style="background-color:#007073">
                                            <td colspan="2" style=" height:20px; color:#349748; font-weight:bold; font-size:larger" class="popCell">
                                                <asp:Label ID="lblTicketDetailHeading" ForeColor="#E4EC04" runat="server"></asp:Label>
                                                <asp:Label ID="lblTCKPKID" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblSTSPKID" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblUserPKID" ForeColor="#E4EC04" runat="server" Visible="false"></asp:Label>
                                            </td>
                                        </tr>

                                            <tr><td colspan="2" class="popCell">&nbsp;</td></tr>

                                            <%--Main details row--%>
                                            <tr>
                                                <td colspan="2" class="popCellLeft">
                                                    <hr class="hr1" />
                                                    <div id="prinatbleDiv">
                                                    <table class="insideTable" style="width: 92%; margin-left: 5%;">

                                                        <%--Subject row--%>
                                                        <tr>
                                                            <td width="20%"><div class="DetailLabel">Subject</div></td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ReadOnly="true" ID="txtSubject" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>

                                                        <%--Reference row--%>
                                                        <%--<tr>
                                                            <td width="20%"><div class="DetailLabel">Reference No</div></td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ReadOnly="true" ID="txtReference" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>--%>

                                                        <%--Date row--%>
                                                        <tr>
                                                            <td><div class="DetailLabel">Date Created</div></td><td><div class="DetailLabel"><asp:TextBox runat="server" ID="txtDateCreated" ReadOnly="true" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox></div></td>
                                                        </tr> 

                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>
                                                        
                                                        <%--Category row--%>
                                                        <tr>
                                                            <td><div class="DetailLabel">Category</div></td><td><div class="DetailLabel"><asp:TextBox runat="server" ID="txtCategory" ReadOnly="true" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox></div></td>
                                                        </tr> 

                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>

                                                        <%--Message row--%>
                                                        <tr>
                                                            <td><div class="DetailLabel">Message</div></td>
                                                            <td>
                                                                <div class="DetailLabel">
                                                                    <asp:TextBox runat="server" ID="txtMessage" CssClass="textarea" TextMode="MultiLine" Height="80px" ReadOnly="true" Rows="7" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox>
                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>

                                                        <tr>
                                                            <td><div class="DetailLabel">Attached Document</div></td>
                                                            <td><div class="DetailLabel">
                                                                <%--<asp:LinkButton runat="server" ID="lnkAttachedDocument" Text="Document" BorderStyle="None" Visible="false"></asp:LinkButton>--%>
                                                                <asp:Label runat="server" ID="lblNoneDocument" Text="None" BorderStyle="None" Visible="false"></asp:Label>
                                                                    <asp:GridView HeaderStyle-Height="20" BorderWidth="1px" RowStyle-Height="20" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="GridViewOldDocuments" AutoGenerateColumns="false" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="100%" runat="server" AllowSorting="True" AllowPaging="true" PageSize="3" OnRowDataBound="GridViewOldDocuments_RowDataBound" OnPageIndexChanging="GridViewOldDocuments_PageIndexChanging">
                                                                        <Columns>
                                                                            <%--select id,filename, filepath, users_id, tickets_id, date_mod from glpi_documents--%>
                                                                            <%--<asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                                                                <ItemTemplate>
                                                                                    <asp:Image ID="imgDirectionIcon" runat="server" Width="16" Height="16" BackColor="Transparent" BorderStyle="None" ToolTip="" ImageAlign="Middle" AlternateText=""></asp:Image>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>--%>
                                                                            <asp:BoundField DataField="date_mod" HeaderText="Document Date" HeaderStyle-Width="150px" />
                                                                            <asp:TemplateField HeaderText="Document Link" ItemStyle-HorizontalAlign="Left" >
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="lnkOldDocumentLink" runat="server" CommandArgument='<%#Eval("sha1sum")%>' Text='<%#Eval("filename")%>' ForeColor="Blue" CssClass="lnkStyle" Enabled="false" ></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                        <PagerSettings PageButtonCount="5" ></PagerSettings>
                                                                        <AlternatingRowStyle BackColor="White" />
                                                                        <EditRowStyle BackColor="#7C6F57" />
                                                                        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                                                        <HeaderStyle BackColor="#1C5E55" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                                                        <PagerStyle  BorderStyle="None" BorderWidth="0px" Height="20"  BackColor="#666666" ForeColor="White" cssclass="gridview" HorizontalAlign="Left" />
                                                                        <RowStyle Font-Size="12px" BackColor="#E3EAEB" />
                                                                        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                                                        <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                                                        <SortedAscendingHeaderStyle BackColor="#246B61" />
                                                                        <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                                                        <SortedDescendingHeaderStyle BackColor="#15524A" />
                                                                    </asp:GridView>


                                                                </div>
                                                            </td>
                                                        </tr>

                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>

                                                        <%--Status row--%>
                                                        <tr>
                                                            <td><div class="DetailLabel">Status</div></td><td><div class="DetailLabel"><asp:TextBox runat="server" ID="txtStatus" ReadOnly="true" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox></div></td>
                                                        </tr>     
                                                                                                           
                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>

                                                        <%--Priority row--%>
                                                        <%--<tr>
                                                            <td><div class="DetailLabel">Priority</div></td><td><div class="DetailLabel"><asp:TextBox runat="server" ID="txtPriority" ReadOnly="true" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1"></asp:TextBox></div></td>
                                                        </tr>

                                                        <tr><td colspan="2"><hr class="hr2" /></td></tr>--%>

                                                        <%--Responses--%>
                                                        <tr>
                                                            <td><div class="DetailLabel">Correspondence</div></td>
                                                            <td>
                                                                   <asp:GridView HeaderStyle-Height="20" BorderWidth="1px" RowStyle-Height="20" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="GridViewResponses" AutoGenerateColumns="false" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="100%" runat="server" AllowSorting="True" AllowPaging="true" PageSize="3" OnRowDataBound="GridViewResponses_RowDataBound" OnPageIndexChanging="GridViewResponses_PageIndexChanging">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                                                                <ItemTemplate>
                                                                                    <asp:Image ID="imgDirectionIcon" runat="server" Width="16" Height="16" BackColor="Transparent" BorderStyle="None" ToolTip="" ImageAlign="Middle" AlternateText=""></asp:Image>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="TKR_ResponseDate" HeaderText="Date" HeaderStyle-Width="150px" />
                                                                            <asp:TemplateField HeaderText="Response Message" ItemStyle-HorizontalAlign="Left" >
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="lnkTicketResponseLink" runat="server" CommandArgument='<%#Eval("OFC_PKID")%>' Text='<%#Eval("TKR_ResponseMessage")%>' ForeColor="Blue" CssClass="lnkStyle" Enabled="false" ></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <%--<asp:TemplateField HeaderText="Document" ItemStyle-HorizontalAlign="Left" >
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton ID="lnkResponseDocumentLink" runat="server" CommandArgument='<%#Eval("DCM_UniqueID")%>' Text='<%#Eval("DCM_OriginalName")%>' ForeColor="Blue" CssClass="lnkStyle" Enabled="true" ></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>--%>
                                                                            <%--<asp:BoundField DataField="STS_StatusName"  HeaderText="Status" HeaderStyle-Width="100px" />--%>
                                                                        </Columns>
                                                                        <PagerSettings PageButtonCount="5" ></PagerSettings>
                                                                        <AlternatingRowStyle BackColor="White" />
                                                                        <EditRowStyle BackColor="#7C6F57" />
                                                                        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                                                        <HeaderStyle BackColor="#1C5E55" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                                                        <PagerStyle  BorderStyle="None" BorderWidth="0px" Height="20"  BackColor="#666666" ForeColor="White" cssclass="gridview" HorizontalAlign="Left" />
                                                                        <RowStyle Font-Size="12px" BackColor="#E3EAEB" />
                                                                        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                                                        <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                                                        <SortedAscendingHeaderStyle BackColor="#246B61" />
                                                                        <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                                                        <SortedDescendingHeaderStyle BackColor="#15524A" />
                                                                    </asp:GridView>
                                                                    <asp:Label ID="lblNoneResponses" runat="server" Text="None" Visible="false"></asp:Label>
                                                           </td>
                                                      </tr>

                                                      <tr><td colspan="2"><hr class="hr2" /></td></tr>

                                                      <%--Action buttons row--%>

                                                      <tr>
                                                          <td colspan="2" class="td4">
                                                              <asp:TextBox runat="server" ID="txtResponseFeedback" BackColor="#ccff99" CssClass="textarea" TextMode="MultiLine" MaxLength="1000" Height="40px" ReadOnly="false" Rows="3" BorderColor="DarkGray" Width="98%" BorderStyle="Solid" BorderWidth="1" placeholder="Enter reasons for deleting ticket." Visible="false"></asp:TextBox>
                                                          </td>
                                                      </tr>

                                                      <%--<tr>
                                                          <td colspan="2" class="td4">
                                                              <asp:FileUpload runat="server" ID="flpResponseUpload" Width="98%" CssClass="textarea" Visible="false"></asp:FileUpload>
                                                          </td>
                                                      </tr>--%>

                                                      <tr>
                                                          <td colspan="2" class="td4">
                                                              <%--<asp:Button BackColor="#2E6373" CssClass="btn btn-default" ID="btnAcceptSolution" runat="server" Width="100px" Text="Accept Solution" Visible="false" CausesValidation="true" OnClick="btnAcceptSolution_Click"/>&nbsp; 
                                                              <asp:Button BackColor="#2E6373" CssClass="btn btn-default" ID="btnRejectSolution" runat="server" Width="100px" Text="Reject Solution" Visible="false" CausesValidation="true" OnClick="btnRejectSolution_Click"/>&nbsp;
                                                              <asp:Button BackColor="#2E6373" CssClass="btn btn-default" ID="btnSendInformation" runat="server" Width="100px" Text="Add Information" Visible="true" CausesValidation="true" OnClick="btnSendInformation_Click"/>&nbsp;--%>
                                                              <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnDeleteTicket" runat="server" Width="100px" Text="Delete Ticket" Visible="false" CausesValidation="true" OnClick="btnDeleteTicket_Click"/>&nbsp;
                                                              <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnRefresh" runat="server" Width="80px" Text="Refresh" Visible="true" CausesValidation="true" OnClick="btnRefresh_Click"/>&nbsp;
                                                          </td>
                                                      </tr>
                                                              
                                                       <tr>
                                                           <td>&nbsp;</td>
                                                           <td colspan="2">
                                                               <asp:Label ID="lblPopErrorMessage" Visible="false" runat="server" Text="" ForeColor="Red" ></asp:Label>
                                                           </td>
                                                       </tr>
                                                                                                                                                            
                                                    </table>
                                                   </div>
                                                  <hr class="hr1" />
                                                </td>
                                            </tr>

                                        <%--Close popup bottom--%>
                                        <tr style="background-color:#6BAB4D" class="popRow">
                                            <td colspan="2" class="popCellRight">
                                                <asp:ImageButton id="imbTCKCloseBottom" runat="server" BackColor="Transparent" ToolTip="Close window" BorderStyle="None" width="25px" height="25px" alt="Close" ImageUrl="~/Images/cancel.png" />
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

                                <asp:Panel ID="pnlConfirmationPopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="450px" Height="150px" style="display:none">
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


                         <%--Dummy row for deletion of message--%>
                         <tr>
                             <td class="valignTableTop" colspan="4">
                                <asp:Button ID="btnShowDeletePopup" runat="server" style="display:none" />
                                <cc1:ModalPopupExtender ID="ModalPopupExtenderDelete" runat="server"  TargetControlID="btnShowDeletePopup" PopupControlID="pnlDeletePopup"
                                    CancelControlID="btnCancelEdit"  BackgroundCssClass="modalBackground">
                                </cc1:ModalPopupExtender>
                                <asp:Panel ID="pnlDeletePopup" runat="server" ScrollBars="None" BackColor="#007073" BorderStyle="Solid" BorderWidth="1" Width="600px" style="display:none">
                                        <table class="popTableV">
                                        <tr style="background-color:#007073">
                                            <td colspan="2" style=" height:20px; color:#349748; font-weight:bold; font-size:larger" class="popCell">
                                                <asp:Label ID="lblEditDeleteHeading" ForeColor="#E4EC04" runat="server"></asp:Label>
                                                <asp:Label ID="lblEditDeleteTCKPKID" runat="server" Visible="false"></asp:Label>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td colspan="2" class="popCell">&nbsp;</td>
                                        </tr>

                                        <%--Main details row--%>
                                        <tr>
                                            <td colspan="2" class="popCellLeft">
                                                <hr class="hr1" />
                                                <table class="insideTable">

                                                    <%--Message row--%>
                                                    <tr>
                                                        <td>
                                                            <asp:Label runat="server" ID="Label4" Font-Bold="true" Text="Message"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox runat="server" ID="txtDeleteEditResponse" placeholder="Enter your action message here." CssClass="textarea" TextMode="MultiLine" Height="50px" Width="97%"></asp:TextBox>
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
                                                            <%--<asp:Button BackColor="#2E6373" CssClass="btn btn-default" ID="btnSaveEdit" runat="server" Width="100px" Text="Save" Visible="false" CausesValidation="true" OnClick="btnSaveEdit_Click"/>&nbsp;--%> 
                                                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnDeleteRow" runat="server" Width="100px" Text="Delete" Visible="false" CausesValidation="true" OnClick="btnDeleteRow_Click"/>&nbsp; 
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
                                                <%--<asp:ImageButton id="imgCloseDeletePop" runat="server" BackColor="Transparent" ToolTip="Close window" BorderStyle="None" width="25px" height="25px" alt="Close" ImageUrl="~/Images/cancel.png" />--%>
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
