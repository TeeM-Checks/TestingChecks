<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Mobile.Master" AutoEventWireup="true" CodeBehind="MobileViewTickets.aspx.cs" Inherits="NewBilletterie.MobileViewTickets" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        
       <div id="main">

            <div id="site_content2">
                
            <div id="content100">
                    <h1>View Tickets<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1>

                    <div class="form_ticket">

                        <asp:Panel ID="pnlGridView" runat="server" Visible="true" Width="100%">

                        <h4>
                            You currently have: <asp:Label ID="lblNoOfTickets" runat="server" Text="" ForeColor="Green" Font-Size="Large"></asp:Label>  <br />
                            View open tickets only <asp:CheckBox runat="server" Text="" ID="chkViewOpen" Checked="true" OnCheckedChanged="chkViewOpen_CheckedChanged" AutoPostBack="true" /> <br />
                            &nbsp;&nbsp;&nbsp;
                            <%--<asp:Button CssClass="toogleButtons" ID="btnViewOldTickets" Width="200px" runat="server" Text="View old tickets" ToolTip="View tickets from the old ticketing system." OnClientClick="window.open('../Legacy/ViewOldTickets.aspx', 'View Old Tickets');" Visible="false" />--%>
                        </h4>

                        <table class="table22">

                         <%--Grid view for results--%>
                         <tr>
                             <td >
                                 <asp:GridView HeaderStyle-Height="30"  BorderWidth="1px" RowStyle-Height="30" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="gridTickets" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" runat="server" AllowSorting="True" OnPageIndexChanging="gridTickets_PageIndexChanging" OnRowDataBound="gridTickets_RowDataBound" Width="100%">
                                    <Columns>
                                      <asp:TemplateField  HeaderText="Ticket Number" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="110px">
                                          <ItemTemplate>
                                              <asp:LinkButton ID="lnkTicketLink" runat="server" CommandName="OpenTicket" CommandArgument='<%#Eval("TCK_PKID")%>' OnClick="lnkTicketLink_Click" Text='<%#Eval("TCK_TicketNumber")%>' ForeColor="Blue" CssClass="lnkStyle"></asp:LinkButton>
                                          </ItemTemplate>
                                       </asp:TemplateField> 
                                        <asp:BoundField HeaderText="Subject" DataField="TCK_Subject" HeaderStyle-Width="110px" ItemStyle-HorizontalAlign="Left"  HeaderStyle-HorizontalAlign="Left" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                        </asp:BoundField>
                                        <asp:BoundField HeaderText="Status" DataField="STS_StatusName" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        </asp:BoundField>
                                   </Columns>

                                <PagerSettings PageButtonCount="5" ></PagerSettings>
                                <AlternatingRowStyle BackColor="White" />
                                <EditRowStyle BackColor="#7C6F57" />
                                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#1C5E55" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                <PagerStyle  BorderStyle="None" BorderWidth="0px" Height="30"  BackColor="#666666" ForeColor="White" cssclass="gridview" HorizontalAlign="Left" />
                                <RowStyle Font-Size="12px" BackColor="#E3EAEB" />
                                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F8FAFA" />
                                <SortedAscendingHeaderStyle BackColor="#246B61" />
                                <SortedDescendingCellStyle BackColor="#D4DFE1" />
                                <SortedDescendingHeaderStyle BackColor="#15524A" />

                                </asp:GridView>
                                 <br />
                                 <asp:Label ID="lblDelEditWarning" runat="server" ForeColor="Green" Text="NB. Only tickets with Submitted status can be deleted or edited." Visible="false"></asp:Label>
                             </td>
                         </tr>

                         <tr>
                            <td align="left">
                                <asp:Button ID="btnNewTicket" BackColor="#4CAF50" Height="35px" Width="120px" Font-Size="Large" ForeColor="White"  runat="server" Text="New Ticket" OnClick="btnNewTicket_Click" /><br />
                            </td>
                         </tr>

                         <tr>
                            <td>&nbsp;</td>
                         </tr>
                       </table>
                       </asp:Panel>

                        <asp:Panel ID="pnlOpenTicket" runat="server" Visible="false">

                            <table style="table-layout:auto; width:98%">
                            <tr>
                                <td>
                                    <asp:Label ID="lblTCKPKID" runat="server" CssClass="loginLabel" Visible="false" Text=""></asp:Label>
                                    <asp:Label ID="lblSTSPKID" runat="server" CssClass="loginLabel" Visible="false" Text=""></asp:Label>
                                    <asp:Label ID="lblSubjectText" runat="server" CssClass="loginLabel" Visible="true" Text="Subject"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblSubjectValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblCategoryText" runat="server" CssClass="loginLabel" Visible="true" Text="(Sub) Category"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblCategoryValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblDateCreatedText" runat="server" CssClass="loginLabel" Visible="true" Text="Date Created"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblDateCreatedValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblMessageText" runat="server" CssClass="loginLabel" Visible="true" Text="Message"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblMessageValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                            </tr>

                           <tr>
                                <td>
                                    <asp:Label ID="lblStatusText" runat="server" CssClass="loginLabel" Visible="true" Text="Status"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblStatusValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                            </tr>

                           <tr>
                                <td>
                                    <asp:Label ID="lblMainAttachmentText" runat="server" CssClass="loginLabel" Visible="true" Text="Attachment"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblNoneMainDocument" runat="server" CssClass="loginLabel" Visible="true" Text="None"></asp:Label>
                                    <asp:LinkButton ID="lnkMainAttachmentValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:LinkButton>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">
                                    <hr />
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">
                                    <asp:TextBox runat="server" ID="txtMobileResponseMessage" TextMode="MultiLine" Rows="4" placeholder="Add response message" MaxLength="2500" Width="98%" BorderStyle="Solid" BorderColor="Black"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">
                                    <asp:FileUpload runat="server" ID="flpResponseUpload" Width="98%" CssClass="textarea" Visible="true"></asp:FileUpload>
                                </td>
                            </tr>

                           <tr>
                                <td colspan="2">
                                    <asp:Button ID="btnSendResponse" BackColor="#4CAF50" Height="35px" Width="150px" Font-Size="Large" ForeColor="White"  runat="server" Text="Send Response" OnClick="btnSendResponse_Click" />
                                    <asp:Button ID="btnDeleteTicket" BackColor="#4CAF50" Height="35px" Width="150px" Font-Size="Large" ForeColor="White"  runat="server" Text="Delete Ticket" OnClick="btnDeleteTicket_Click" />
                                    <asp:Button ID="btnAcceptSolution" BackColor="#4CAF50" Height="35px" Width="150px" Font-Size="Large" ForeColor="White"  runat="server" Text="Accept Solution" OnClick="btnAcceptSolution_Click" />
                                    <asp:Button ID="btnRejectSolution" BackColor="#4CAF50" Height="35px" Width="150px" Font-Size="Large" ForeColor="White"  runat="server" Text="Reject Solution" OnClick="btnRejectSolution_Click" />
                                    <asp:Label runat="server" ID="lblSendResponseError" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                </td>
                            </tr>

                            <tr runat="server" id="lnrResponse">
                                <td colspan="2">
                                    <hr />
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">
                                    <asp:GridView HeaderStyle-Height="20" BorderWidth="1px" RowStyle-Height="20" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="GridViewResponses" AutoGenerateColumns="false" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="100%" runat="server" AllowSorting="True" AllowPaging="true" PageSize="3" OnRowDataBound="GridViewResponses_RowDataBound" OnRowCommand="GridViewResponses_RowBoundOperations" OnPageIndexChanging="GridViewResponses_PageIndexChanging">
                                    <Columns>
                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="20px">
                                            <ItemTemplate>
                                                <asp:Image ID="imgDirectionIcon" runat="server" Width="16" Height="16" BackColor="Transparent" BorderStyle="None" ToolTip="" ImageAlign="Middle" AlternateText=""></asp:Image>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="TKR_ResponseDate" HeaderText="Date" HeaderStyle-Width="150px" />
                                        <asp:TemplateField HeaderText="Response" ItemStyle-HorizontalAlign="Left" >
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkTicketResponseLink" runat="server" CommandArgument='<%#Eval("UST_PKID")%>' Text='<%#Eval("TKR_ResponseMessage")%>' ForeColor="Blue" CssClass="lnkStyle" Enabled="false" ></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" >
                                            <ItemTemplate>
                                                <asp:Button ID="btnViewMobileResponse" runat="server" CommandArgument='<%#Eval("TKR_PKID")%>' Text="View"  ForeColor="White" Enabled="true" CommandName="OpenResponse" BackColor="#4CAF50"></asp:Button>
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
                              </td>
                            </tr>

                           </table>

                            <table class="table22">
                         <tr>
                            <td align="left">
                                <asp:Button ID="btnBackToList" BackColor="#4CAF50" Height="35px" Width="200px" Font-Size="Large" ForeColor="White"  runat="server" Text="Back to list" OnClick="btnBackToList_Click" /><br />
                            </td>
                         </tr>

                         <tr>
                            <td>&nbsp;</td>
                         </tr>
                       </table>

                       </asp:Panel>

                        <asp:Panel ID="pnlOpenResponse" runat="server" Visible="false">

                            <table style="table-layout:auto; width:98%">
                              <tr>
                                <td>
                                    <asp:Label ID="lblResponseMessageText" runat="server" CssClass="loginLabel" Visible="true" Text="Response"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblResponseMessageValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                             </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblResponseDateText" runat="server" CssClass="loginLabel" Visible="true" Text="Date"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblResponseDateValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                            </tr>

                            <tr>
                                <td>
                                    <asp:Label ID="lblResponseFromText" runat="server" CssClass="loginLabel" Visible="true" Text="Sender"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblResponseFromValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:Label>
                                </td>
                            </tr>

                           <tr>
                                <td>
                                    <asp:Label ID="lblResponseAttachmentText" runat="server" CssClass="loginLabel" Visible="true" Text="Attachment"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblNoneResponseDocument" runat="server" CssClass="loginLabel" Visible="true" Text="None"></asp:Label>
                                    <asp:LinkButton ID="lnkResponseAttachmentValue" runat="server" CssClass="loginLabel" Visible="true" Text=""></asp:LinkButton>
                                </td>
                            </tr>

                          </table>

                         <table class="table22">
                         <tr>
                            <td align="left">
                                <asp:Button ID="btnBackToResponse" BackColor="#4CAF50" Height="35px" Width="180px" Font-Size="Large" ForeColor="White"  runat="server" Text="Back to ticket" OnClick="btnBackToResponse_Click" /><br />
                            </td>
                         </tr>

                         <tr>
                            <td align="left">
                            </td>
                         </tr>

                         <tr>
                            <td>&nbsp;</td>
                         </tr>

                       </table>

                        </asp:Panel>


                    </div>       
   
                </div>

            </div>

      </div>


</asp:Content>
