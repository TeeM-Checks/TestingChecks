<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FAQ.aspx.cs" Inherits="NewBilletterie.FAQ" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
            <style type="text/css">
        .pager span {
            color: #f1ff2f;
            font-weight: bold;
            font-size: 16pt;
        }
    </style>
    
    <div id="main">

            <div id="site_content2">
                
            <div id="content1">
                    <h1>Frequently Asked Questions<asp:Label ID="lblMainErrorMessage" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label></h1>


                    <div class="form_ticket">

                        <table width="100%">

                    <tr>
                        <td>
                            <h4>Total number of FAQs: <asp:Label ID="lblNoOfFAQs" runat="server" Text="" ForeColor="Green" Font-Size="Large"></asp:Label></h4>
                        </td>

                        <td colspan="2" class="td3">
                            <asp:TextBox runat="server" ID="txtFAQSearch" placeholder="Search by question or answer key words." ToolTip="Enter at least 3 consecutive characters." CssClass="textboxsmall"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button BackColor="#2E6373" ForeColor="White" CssClass="btn btn-default" ID="btnFindFAQ" runat="server" Width="85px" Text="Search" OnClick="btnFindFAQ_Click" />
                        </td>
                    </tr>

                    <tr>
                        <td colspan="4">
                             <asp:Label runat="server" ForeColor="Red" ID="lblExtUserSearchErr" Visible="false"></asp:Label>
                        </td>
                    </tr>

                    </table>

                        <br />

                        <table class="table22">

                            <tr>
                                <td>
                                 <asp:GridView HeaderStyle-Height="30" BorderWidth="1px" CssClass="gridViews" RowStyle-Height="30" PagerSettings-PageButtonCount="5" HeaderStyle-BackColor="LightGray" GridLines="Horizontal" ID="gridFAQs" AutoGenerateColumns="false" AllowPaging="true" PageSize="10" EnableColumnVirtualization="True" HorizontalScrollBarVisibility="Auto" Width="120%" runat="server" AllowSorting="True" OnPageIndexChanging="gridFAQs_PageIndexChanging" OnRowDataBound="gridFAQs_RowDataBound" OnRowCommand="gridFAQs_RowBoundOperations">
                                    <Columns>
                                        <asp:BoundField DataField="FAQ_EntryText" HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                        </asp:BoundField>
                                   </Columns>

                                <PagerSettings PageButtonCount="5" ></PagerSettings>
                                <AlternatingRowStyle BackColor="White" />
                                <EditRowStyle BackColor="#7C6F57" />
                                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#1C5E55" CssClass="GridHeaderStyle" HorizontalAlign="Left" Font-Bold="True" ForeColor="White" />
                                <%--<PagerStyle  BorderStyle="None" BorderWidth="0px" Height="30" BackColor="#666666" ForeColor="White" cssclass="gridview" HorizontalAlign="Left" />--%>
                                <PagerStyle CssClass="pager" BorderStyle="None" BorderWidth="0px" Height="30" BackColor="#1C5E55" ForeColor="White" HorizontalAlign="Left" />
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
                    
                    </div>       
   
                </div>

            </div>

      </div>
</asp:Content>
