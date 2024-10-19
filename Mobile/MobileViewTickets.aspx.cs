using NewBilletterie.Classes;
//using NewBilletterie.CUBAServerService;
//using NewBilletterie.EmailWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;
using System.Text;
using System.Text.RegularExpressions;

namespace NewBilletterie
{
    public partial class MobileViewTickets : System.Web.UI.Page
    {
        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["DesktopViewLink"] = "../ExternalPages/ViewTickets.aspx";

                if (Session["userObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    PopulateTicketGrid(usrSession.USR_PKID, true);
                }
                else
                {
                    Response.Redirect("~/MobileIndex.aspx", false);
                }
            }
        }

        private void PopulateTicketGrid(int userPKID, bool viewOpenOnly)
        {
            string countMessage = "";
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            if (viewOpenOnly)
            {
                ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5) and TCK_Viewable = 1 and TCT_PKID in (1,4) and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5) and TCK_Viewable = 1 and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc");
                countMessage = " open ticket(s)";
            }
            else
            {
                ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5,6,7,8) and TCK_Viewable = 1 and TCT_PKID in (1,4) and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5,6,7,8) and TCK_Viewable = 1 and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc");
                countMessage = " total tickets";
            }
            if (ds != null)
            {
                Session["ViewTCKResults"] = ds.Tables[0];
                gridTickets.DataSource = null;
                gridTickets.DataSource = ds.Tables[0];
                gridTickets.DataBind();
                gridTickets.PageIndex = 0;
                lblNoOfTickets.Text = ds.Tables[0].Rows.Count.ToString() + countMessage;
            }
        }

        protected void lnkTicketLink_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                pnlOpenTicket.Visible = true;
                pnlGridView.Visible = false;
                //ModalPopupExtenderTicketDetail.Show();
                //lblTicketDetailHeading.Text = "Ticket [" + lnk.Text + "]";
                LoadTicketDetails(lnk.CommandArgument);
                //lblTCKPKID.Text = lnk.CommandArgument;
            }
            catch (Exception)
            {

            }

        }

        private void LoadTicketDetails(string ticketPKID)
        {
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, TCK_Reference from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, TCK_Reference from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblSubjectValue.Text = ds.Tables[0].Rows[0]["TCK_Subject"].ToString();
                        lblCategoryValue.Text = ds.Tables[0].Rows[0]["CAT_CategoryName"].ToString();
                        lblDateCreatedValue.Text = ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString();
                        lblMessageValue.Text = ds.Tables[0].Rows[0]["TCK_Message"].ToString();
                        lblStatusValue.Text = ds.Tables[0].Rows[0]["STS_StatusName"].ToString();
                        lblTCKPKID.Text = ds.Tables[0].Rows[0]["TCK_PKID"].ToString();
                        lblSTSPKID.Text = ds.Tables[0].Rows[0]["STS_PKID"].ToString();

                        //txtSubject.Text = ds.Tables[0].Rows[0]["TCK_Subject"].ToString();
                        //txtReference.Text = ds.Tables[0].Rows[0]["TCK_Reference"].ToString();
                        //txtMessage.Text = ds.Tables[0].Rows[0]["TCK_Message"].ToString();
                        //txtDateCreated.Text = ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString();
                        ////txtPriority.Text = ds.Tables[0].Rows[0]["TPT_Priority"].ToString();
                        //txtStatus.Text = ds.Tables[0].Rows[0]["STS_StatusName"].ToString();
                        //txtCategory.Text = ds.Tables[0].Rows[0]["CAT_CategoryName"].ToString();
                        //lblTCKPKID.Text = ds.Tables[0].Rows[0]["TCK_PKID"].ToString();
                        //lblSTSPKID.Text = ds.Tables[0].Rows[0]["STS_PKID"].ToString();
                        //txtResponseFeedback.Text = "";

                        if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "1") //Submitted
                        {
                            txtMobileResponseMessage.Visible = true;
                            lnrResponse.Visible = true;
                            btnDeleteTicket.Visible = true;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendResponse.Visible = true;
                            flpResponseUpload.Visible = true;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "2") //Under Treatment
                        {
                            txtMobileResponseMessage.Visible = true;
                            lnrResponse.Visible = true;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendResponse.Visible = true;
                            flpResponseUpload.Visible = true;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "3") //Re-Submitted
                        {
                            txtMobileResponseMessage.Visible = true;
                            lnrResponse.Visible = true;
                            btnDeleteTicket.Visible = true;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendResponse.Visible = true;
                            flpResponseUpload.Visible = true;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "4") //Error Submitting
                        {
                            txtMobileResponseMessage.Visible = true;
                            lnrResponse.Visible = true;
                            btnDeleteTicket.Visible = true;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendResponse.Visible = true;
                            flpResponseUpload.Visible = true;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "5") //Resolved
                        {
                            txtMobileResponseMessage.Visible = true;
                            lnrResponse.Visible = true;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = true;
                            btnRejectSolution.Visible = true;
                            btnSendResponse.Visible = true;
                            flpResponseUpload.Visible = true;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "6") //Closed
                        {
                            txtMobileResponseMessage.Visible = false;
                            lnrResponse.Visible = false;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendResponse.Visible = false;
                            flpResponseUpload.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "7") //Deleted By User
                        {
                            txtMobileResponseMessage.Visible = false;
                            lnrResponse.Visible = false;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendResponse.Visible = false;
                            flpResponseUpload.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "8") //Deleted By Office
                        {
                            txtMobileResponseMessage.Visible = false;
                            lnrResponse.Visible = false;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendResponse.Visible = false;
                            flpResponseUpload.Visible = false;
                        }

                    }
                }



                if (bool.Parse(ConfigurationManager.AppSettings["AllowDisableNewTickets"]))
                {
                    allowedDateResponse allwdDateResp = new allowedDateResponse();
                    allwdDateResp = NewTicketAllowed("New tickets cannot be created until ");
                    if (allwdDateResp.dateAllowed == false)
                    {
                        btnSendResponse.Enabled = false;
                        btnSendResponse.Font.Strikeout = true;
                        btnSendResponse.ToolTip = allwdDateResp.displayMessage;
                    }
                    else
                    {
                        btnSendResponse.Enabled = true;
                        btnSendResponse.Font.Strikeout = false;
                        btnSendResponse.ToolTip = "";
                    }
                }



                DataSet dsResp = new DataSet();
                dsResp = bilAPIWS.GetBilletterieDataSet("select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, R.STS_PKID, S.STS_StatusName, D.DCM_OriginalName, D.DCM_DerivedName, DCM_PKID, DCM_UniqueID from TB_TKR_TicketResponse R inner join TB_STS_Status S on R.STS_PKID  = S.STS_PKID left outer join TB_DCM_Document D on (R.TKR_PKID = D.TNK_PKID and D.DCL_PKID = 2) where TCK_PKID = " + ticketPKID + " and TKR_VisibleToClient = 1 order by TKR_PKID desc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dsResp = da.GetGenericBilletterieDataSet("TB_TKR_TicketResponse", "TB_TKR_TicketResponseDS", "select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, R.STS_PKID, S.STS_StatusName, D.DCM_OriginalName, D.DCM_DerivedName, DCM_PKID, DCM_UniqueID from TB_TKR_TicketResponse R inner join TB_STS_Status S on R.STS_PKID  = S.STS_PKID left outer join TB_DCM_Document D on (R.TKR_PKID = D.TNK_PKID and D.DCL_PKID = 2) where TCK_PKID = " + ticketPKID + " and TKR_VisibleToClient = 1 order by TKR_PKID desc");
                if (dsResp != null)
                {
                    if (dsResp.Tables[0].Rows.Count > 0)
                    {
                        Session["ViewResponseResults"] = dsResp.Tables[0];
                        GridViewResponses.DataSource = null;
                        GridViewResponses.DataSource = dsResp.Tables[0];
                        GridViewResponses.DataBind();
                        GridViewResponses.PageIndex = 0;

                        //lblNoneResponses.Visible = false;
                        GridViewResponses.Visible = true;
                    }
                    else
                    {
                        //lblNoneResponses.Visible = true;
                        GridViewResponses.Visible = false;
                    }
                }
                else
                {
                    //lblNoneResponses.Visible = true;
                    GridViewResponses.Visible = false;

                }


                DataSet dsDocs = new DataSet();
                dsDocs = bilAPIWS.GetBilletterieDataSet("select DCM_UniqueID, DCM_OriginalName from TB_DCM_Document where DCT_PKID = 1 and DCL_PKID = 1 and TNK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dsDocs = da.GetGenericBilletterieDataSet("TB_DCM_Document", "TB_DCM_DocumentDS", "select DCM_UniqueID, DCM_OriginalName from TB_DCM_Document where DCT_PKID = 1 and DCL_PKID = 1 and TNK_PKID = " + ticketPKID);
                if (dsDocs != null)
                {
                    if (dsDocs.Tables[0].Rows.Count > 0)
                    {
                        lnkMainAttachmentValue.Visible = true;
                        lblNoneMainDocument.Visible = false;
                        lnkMainAttachmentValue.Text = dsDocs.Tables[0].Rows[0]["DCM_OriginalName"].ToString();
                        string Url = "../GetDocument.aspx?docID=" + dsDocs.Tables[0].Rows[0]["DCM_UniqueID"].ToString();
                        lnkMainAttachmentValue.Attributes.Add("onclick", "javascript: OpenInNewTab('" + Url + "')");
                    }
                    else
                    {
                        lnkMainAttachmentValue.Visible = false;
                        lnkMainAttachmentValue.Text = "";
                        lblNoneMainDocument.Visible = true;
                    }
                }
                else
                {
                    lnkMainAttachmentValue.Visible = false;
                    lblNoneMainDocument.Visible = true;
                    lnkMainAttachmentValue.Text = "";
                }


                //DataSet dsDocs = new DataSet();
                //dsDocs = da.GetGenericBilletterieDataSet("TB_DCM_Document", "TB_DCM_DocumentDS", "select DCM_UniqueID, DCM_OriginalName from TB_DCM_Document where DCT_PKID = 1 and DCL_PKID = 1 and TNK_PKID = " + ticketPKID);
                //if (dsDocs != null)
                //{
                //    if (dsDocs.Tables[0].Rows.Count > 0)
                //    {
                //        lnkAttachedDocument.Visible = true;
                //        lblNoneDocument.Visible = false;
                //        lnkAttachedDocument.Text = dsDocs.Tables[0].Rows[0]["DCM_OriginalName"].ToString();
                //        string Url = "../GetDocument.aspx?docID=" + dsDocs.Tables[0].Rows[0]["DCM_UniqueID"].ToString();
                //        lnkAttachedDocument.Attributes.Add("onclick", "javascript: OpenInNewTab('" + Url + "')");
                //    }
                //    else
                //    {
                //        lnkAttachedDocument.Visible = false;
                //        lblNoneDocument.Visible = true;
                //    }
                //}
                //else
                //{
                //    lnkAttachedDocument.Visible = false;
                //    lblNoneDocument.Visible = true;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private allowedDateResponse NewTicketAllowed(string displayMessage)
        {
            DataAccess da = new DataAccess();
            allowedDateResponse allwdResponse = new allowedDateResponse();

            string newDateString = "";
            newDateString = GetAllowedOfficeDate(DateTime.Now.ToString("yyyy-MM-dd"));
            //newDateString = GetAllowedOfficeDate("2022-12-23");
            if (newDateString != "")
            {
                if (DateTime.Parse(newDateString).Date > DateTime.Now.Date)
                {
                    allwdResponse.dateAllowed = false;
                    allwdResponse.displayMessage = displayMessage + newDateString;
                }
                else
                {
                    allwdResponse.dateAllowed = true;
                    allwdResponse.displayMessage = displayMessage + newDateString;
                }
            }
            return allwdResponse;
        }

        public string GetAllowedOfficeDate(string _applicationDate)
        {
            string retValue = _applicationDate;
            try
            {
                DataSet ds = new DataSet();

                string applicationYear = DateTime.Parse(_applicationDate).Year.ToString();

                if (_applicationDate.Trim() != "")
                {
                    string formattedDate = GetFormattedOfficeDate(_applicationDate);
                    retValue = formattedDate;
                    //ServerServices svrWS = new ServerServices();
                    //svrWS.Url = ConfigurationManager.AppSettings["CUBAServerServiceURL"];

                    ds = bilAPIWS.GetCUBADataSet("select top 1 CONVERT(VARCHAR, FBD_AllowedDate, 106) from TB_FBD_ForbiddenDates where FBD_ForbiddenDate = '" + formattedDate + "' and FBD_Deleted = 0", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            retValue = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                            if (retValue == "")
                            {
                                retValue = formattedDate;
                            }
                            else
                            {
                                retValue = GetFormattedOfficeDate(retValue);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                retValue = "";
            }
            return retValue;
        }

        public string GetFormattedOfficeDate(string unformattedDate)
        {
            string retValue = "";
            try
            {
                retValue = DateTime.Parse(unformattedDate).Year.ToString() + "-" + DateTime.Parse(unformattedDate).Month.ToString("D2") + "-" + DateTime.Parse(unformattedDate).Day.ToString("D2") + " 00:00:00.000";
            }
            catch (Exception)
            {
                retValue = "";
            }
            return retValue;
        }

        protected void gridTickets_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                gridTickets.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewTCKResults"];
                gridTickets.DataSource = dt;
                gridTickets.DataBind();
            }
            catch (Exception)
            {

            }
        }

        protected void btnNewTicket_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Mobile/MobileNewTicket.aspx", false);
        }

        protected void btnBackToList_Click(object sender, EventArgs e)
        {
            pnlOpenTicket.Visible = false;
            pnlGridView.Visible = true;
        }

        protected void gridTickets_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Attributes.Add("style", "cursor:pointer;font-size:12px;font-weight:600;");
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#6BAB4D'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");

                if (e.Row.Cells[2].Text != "")
                {
                    //ImageButton imgEdt = (ImageButton)e.Row.FindControl("imgEdit");
                    //ImageButton imgDel = (ImageButton)e.Row.FindControl("imgDelete");

                    //if (e.Row.Cells[4].Text.Trim().ToLower() == "submitted")
                    //{
                    //    imgEdt.Enabled = true;
                    //    imgDel.Enabled = true;
                    //}
                    //else
                    //{
                    //    imgEdt.Enabled = false;
                    //    imgDel.Enabled = false;

                    if (e.Row.Cells[2].Text.ToLower() == "solved" || e.Row.Cells[2].Text.ToLower() == "deleted by office")
                    {
                        e.Row.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (e.Row.Cells[2].Text.ToLower() == "closed" || e.Row.Cells[2].Text.ToLower() == "deleted by user")
                    {
                        e.Row.ForeColor = System.Drawing.Color.DarkBlue;
                    }
                    //}


                }




            }



        }

        protected void chkViewOpen_CheckedChanged(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();

            if (Session["userObjectCookie"] != null)
            {
                BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                if (chkViewOpen.Checked)
                {
                    PopulateTicketGrid(usrSession.USR_PKID, true);
                }
                else
                {
                    PopulateTicketGrid(usrSession.USR_PKID, false);
                }
            }

        }

        protected void GridViewResponses_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.Cells[0] != null && e.Row.RowIndex >= 0)
            {
                LinkButton lnk = (LinkButton)e.Row.FindControl("lnkTicketResponseLink");
                lnk.ToolTip = lnk.Text;
                if (lnk.Text.Length > 50)
                {
                    lnk.Text = lnk.Text.Substring(0, 50) + " ...";
                    lnk.Text = lnk.Text.Replace(System.Environment.NewLine, " ");
                }
                else
                {
                    lnk.Text = lnk.Text;
                    lnk.Text = lnk.Text.Replace(System.Environment.NewLine, " ");
                }

                Image img = (Image)e.Row.FindControl("imgDirectionIcon");
                if (lnk.CommandArgument == "1")
                {
                    img.ImageUrl = "~/Images/incoming.png";
                }
                else if (lnk.CommandArgument == "2")
                {
                    img.ImageUrl = "~/Images/outgoing.png";
                }

                //HyperLink hp = new HyperLink();
                //LinkButton lnkDoc = (LinkButton)e.Row.FindControl("lnkResponseDocumentLink");

                //string Url = "../GetDocument.aspx?docID=" + lnkDoc.CommandArgument;
                //hp.Attributes.Add("onclick", "javascript: OpenInNewTab('" + Url + "')");
                //hp.NavigateUrl = Url;
                //hp.Text = lnkDoc.Text;
                //lnkDoc.Controls.Add(hp);


                //Button btnView = (Button)e.Row.FindControl("btnViewMobileResponse");


            }
        }

        protected void GridViewResponses_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                GridViewResponses.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewResponseResults"];
                GridViewResponses.DataSource = dt;
                GridViewResponses.DataBind();
                //ModalPopupExtenderTicketDetail.Show();
            }
            catch (Exception)
            {

            }
        }

        protected void GridViewResponses_RowBoundOperations(object sender, GridViewCommandEventArgs e)
        {
            DataAccess da = new DataAccess();
            try
            {
                //txtDeleteEditResponse.Text = "";
                if (e.CommandName == "OpenResponse")
                {
                    ////btnSaveEdit.Visible = true;
                    ////btnDeleteRow.Visible = false;
                    //string ticketNumber = "";
                    //SelectStringResponseObject selResp = new SelectStringResponseObject();
                    //selResp = da.GetBilletterieGenericScalar("select TCK_TicketNumber from TB_TCK_Ticket where TCK_PKID = " + e.CommandArgument.ToString());
                    //ticketNumber = selResp.selectedPKID;
                    ////lblEditDeleteHeading.Text = "Edit Ticket [" + ticketNumber + "]";
                    ////lblEditDeleteTCKPKID.Text = e.CommandArgument.ToString();
                    ////ModalPopupExtenderDelete.Show();



                    //Button lnk = (Button)sender;
                    pnlOpenResponse.Visible = true;
                    pnlOpenTicket.Visible = false;
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, STS_PKID, CASE WHEN UST_PKID = 1 THEN 'You' WHEN UST_PKID = 2 THEN 'Office' END TKR_Sender from TB_TKR_TicketResponse where TKR_PKID = " + e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_TKR_TicketResponse", "TB_TKR_TicketResponseDS", "select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, STS_PKID, CASE WHEN UST_PKID = 1 THEN 'You' WHEN UST_PKID = 2 THEN 'Office' END TKR_Sender from TB_TKR_TicketResponse where TKR_PKID = " + e.CommandArgument.ToString());
                    
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lblResponseMessageValue.Text = ds.Tables[0].Rows[0]["TKR_ResponseMessage"].ToString();
                            lblResponseDateValue.Text = ds.Tables[0].Rows[0]["TKR_ResponseDate"].ToString();
                            lblResponseFromValue.Text = ds.Tables[0].Rows[0]["TKR_Sender"].ToString();
                        }
                    }


                    DataSet dsDocs = new DataSet();
                    dsDocs = bilAPIWS.GetBilletterieDataSet("select DCM_UniqueID, DCM_OriginalName from TB_DCM_Document where DCT_PKID = 1 and DCL_PKID = 2 and TNK_PKID = " + e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //dsDocs = da.GetGenericBilletterieDataSet("TB_DCM_Document", "TB_DCM_DocumentDS", "select DCM_UniqueID, DCM_OriginalName from TB_DCM_Document where DCT_PKID = 1 and DCL_PKID = 2 and TNK_PKID = " + e.CommandArgument.ToString());
                    if (dsDocs != null)
                    {
                        if (dsDocs.Tables[0].Rows.Count > 0)
                        {
                            lnkResponseAttachmentValue.Visible = true;
                            lblNoneResponseDocument.Visible = false;
                            lnkResponseAttachmentValue.Text = dsDocs.Tables[0].Rows[0]["DCM_OriginalName"].ToString();
                            string Url = "../GetDocument.aspx?docID=" + dsDocs.Tables[0].Rows[0]["DCM_UniqueID"].ToString();
                            lnkResponseAttachmentValue.Attributes.Add("onclick", "javascript: OpenInNewTab('" + Url + "')");
                        }
                        else
                        {
                            lnkResponseAttachmentValue.Visible = false;
                            lnkResponseAttachmentValue.Text = "";
                            lblNoneResponseDocument.Visible = true;
                        }
                    }
                    else
                    {
                        lnkResponseAttachmentValue.Visible = false;
                        lblNoneResponseDocument.Visible = true;
                        lnkResponseAttachmentValue.Text = "";
                    }




                }

            }
            catch (Exception ex)
            {

            }
        }

        protected void btnViewMobileResponse_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            try
            {
                Button lnk = (Button)sender;
                pnlOpenResponse.Visible = true;
                pnlOpenTicket.Visible = false;
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, STS_PKID, CASE WHEN UST_PKID = 1 THEN 'You' WHEN UST_PKID = 2 THEN 'Office' END TKR_Sender from TB_TKR_TicketResponse where TKR_PKID = " + lnk.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TKR_TicketResponse", "TB_TKR_TicketResponseDS", "select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, STS_PKID, CASE WHEN UST_PKID = 1 THEN 'You' WHEN UST_PKID = 2 THEN 'Office' END TKR_Sender from TB_TKR_TicketResponse where TKR_PKID = " + lnk.CommandArgument.ToString());
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        lblResponseMessageValue.Text = ds.Tables[0].Rows[0]["TKR_ResponseMessage"].ToString();
                        lblResponseDateValue.Text = ds.Tables[0].Rows[0]["TKR_ResponseDate"].ToString();
                        lblResponseFromValue.Text = ds.Tables[0].Rows[0]["TKR_Sender"].ToString();
                        //lblMessageValue.Text = ds.Tables[0].Rows[0]["TCK_Message"].ToString();
                        //lblStatusValue.Text = ds.Tables[0].Rows[0]["STS_StatusName"].ToString();
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        protected void btnBackToResponse_Click(object sender, EventArgs e)
        {
            pnlOpenTicket.Visible = true;
            pnlOpenResponse.Visible = false;
        }

        private string ValidateFileBeforeUpload()
        {
            Common cm = new Common();
            string returnValue = "";
            try
            {
                if (flpResponseUpload.HasFile == true)
                {
                    if (!cm.ValidMimeType(flpResponseUpload.PostedFile.ContentType, Path.GetExtension(flpResponseUpload.FileName)))
                    {
                        return returnValue = "File type not supported.";
                    }
                    if (flpResponseUpload.FileBytes.Length > 10000000)
                    {
                        return returnValue = "File is too large.";
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        private BilletterieAPIWS.ticketResponseObject PopulateTicketResponse(int newStatus, int tckPKID, string responseMessage)
        {
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

            BilletterieAPIWS.ticketResponseObject returnValue = new BilletterieAPIWS.ticketResponseObject();
            returnValue.TCK_PKID = tckPKID; // Int32.Parse(lblEditDeleteTCKPKID.Text);
            returnValue.OFC_PKID = usrSession.USR_PKID;
            returnValue.UST_PKID = 1;
            returnValue.TKR_ResponseMessage = responseMessage;  //txtDeleteEditResponse.Text.Trim();
            returnValue.TKR_ResponseDate = DateTime.Now.ToString();
            returnValue.TKR_VisibleToClient = true;
            returnValue.STS_PKID = newStatus;
            returnValue.TKR_FromMobile = true;

            return returnValue;
        }

        private BilletterieAPIWS.ticketResponseObject PopulateTicketResponse()
        {
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

            BilletterieAPIWS.ticketResponseObject returnValue = new BilletterieAPIWS.ticketResponseObject();
            returnValue.TCK_PKID = Int32.Parse(lblTCKPKID.Text);
            returnValue.OFC_PKID = usrSession.USR_PKID;
            returnValue.UST_PKID = 1;
            returnValue.TKR_ResponseMessage = txtMobileResponseMessage.Text.Trim();
            returnValue.TKR_ResponseDate = DateTime.Now.ToString();
            returnValue.TKR_VisibleToClient = true;
            returnValue.STS_PKID = Int32.Parse(lblSTSPKID.Text);
            returnValue.TKR_FromMobile = true;
            return returnValue;
        }

        private BilletterieAPIWS.fileAttachmentObject PopulateDocumentObject(int userPKID)
        {
            BilletterieAPIWS.fileAttachmentObject returnValue = new BilletterieAPIWS.fileAttachmentObject();
            try
            {
                if (flpResponseUpload.HasFile == true)
                {
                    returnValue.DCM_OriginalName = flpResponseUpload.FileName;
                    returnValue.AttachmentSize = flpResponseUpload.FileBytes.Length;
                    returnValue.MimeType = flpResponseUpload.PostedFile.ContentType;
                    returnValue.DCM_Extention = Path.GetExtension(flpResponseUpload.FileName);
                    returnValue.DCM_DerivedName = "doc" + userPKID.ToString() + Path.GetExtension(flpResponseUpload.FileName);
                    returnValue.DCT_PKID = 1;
                    returnValue.DCS_PKID = 1;
                    returnValue.DCL_PKID = 2;
                    returnValue.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + returnValue.DCM_DerivedName;  //Server.MapPath("~/Temp/" + atchObj.DCM_DerivedName);
                    returnValue.STS_PKID = 30;
                    returnValue.DCM_FileField = flpResponseUpload.FileBytes;
                    string filePath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + returnValue.DCM_DerivedName; //Server.MapPath("~/Temp/" + atchObj.DCM_DerivedName);
                    if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                    {
                        flpResponseUpload.SaveAs(filePath);
                    }
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        private BilletterieAPIWS.EmailMessageObject PopulateEmailObject(string ticketPKID, string tickRespPKID)
        {
            string newStatusName = "";
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            BilletterieAPIWS.ticketObject tckObj = new BilletterieAPIWS.ticketObject();
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, T.USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateDue, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, USR_UserLogin, USR_FirstName + ' ' + USR_LastName [UserNames], U.USR_EmailAccount, U.USR_MobileNumber, R.USC_SourceName, (select count(*) from TB_TCK_Ticket TK where TK.USR_PKID = T.USR_PKID and STS_PKID in (1,2,3)) [TotalTickets], U.USC_PKID, TCK_CaseIdentifier from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_USR_User U on T.USR_PKID = U.USR_PKID inner join TB_USC_UserSource R on U.USC_PKID = R.USC_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, T.USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateDue, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, USR_UserLogin, USR_FirstName + ' ' + USR_LastName [UserNames], U.USR_EmailAccount, U.USR_MobileNumber, R.USC_SourceName, (select count(*) from TB_TCK_Ticket TK where TK.USR_PKID = T.USR_PKID and STS_PKID in (1,2,3)) [TotalTickets], U.USC_PKID, TCK_CaseIdentifier from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_USR_User U on T.USR_PKID = U.USR_PKID inner join TB_USC_UserSource R on U.USC_PKID = R.USC_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID);
            //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, T.USR_PKID, OFC_PKID, T.TPT_PKID, TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateDue, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, TPT_Priority, STS_StatusName, USR_UserLogin, USR_FirstName + ' ' + USR_LastName [UserNames], U.USR_EmailAccount, U.USR_MobileNumber, R.USC_SourceName, (select count(*) from TB_TCK_Ticket TK where TK.USR_PKID = T.USR_PKID and STS_PKID in (1,2,3)) [TotalTickets] from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_TPT_TicketPriority P on T.TPT_PKID = P.TPT_PKID inner join TB_USR_User U on T.USR_PKID = U.USR_PKID inner join TB_USC_UserSource R on U.USC_PKID = R.USC_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    returnValue.EML_ToEmailList = ds.Tables[0].Rows[0]["TCK_AlternateEmail"].ToString();
                    returnValue.EML_ToEmailAdmin = ds.Tables[0].Rows[0]["TCK_AlternateEmail"].ToString();     // ConfigurationManager.AppSettings["To"];
                    returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ":" + ds.Tables[0].Rows[0]["TCK_Subject"].ToString();
                    tckObj.TCK_PKID = Int32.Parse(ticketPKID);
                    tckObj.CAT_PKID = Int32.Parse(ds.Tables[0].Rows[0]["CAT_PKID"].ToString());
                    tckObj.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                    tckObj.OFC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFC_PKID"].ToString());
                    tckObj.TPT_PKID = Int32.Parse(ds.Tables[0].Rows[0]["TPT_PKID"].ToString());
                    tckObj.TCK_TicketNumber = ds.Tables[0].Rows[0]["TCK_TicketNumber"].ToString();
                    tckObj.TCK_Subject = ds.Tables[0].Rows[0]["TCK_Subject"].ToString();
                    tckObj.TCK_Message = ds.Tables[0].Rows[0]["TCK_Message"].ToString().Replace("\n", "<br />");
                    tckObj.TCK_AlternateEmail = ds.Tables[0].Rows[0]["TCK_AlternateEmail"].ToString();
                    newStatusName = ds.Tables[0].Rows[0]["STS_StatusName"].ToString();
                    tckObj.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                }
            }
            //returnValue.EML_ToEmailAdmin = returnValue.EML_ToEmailList;     // ConfigurationManager.AppSettings["To"];
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_MailBody = GetTicketResponseEmailBody(tckObj, txtMobileResponseMessage.Text.Trim(), newStatusName);
            returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = "1";
            returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
            returnValue.EML_KeyField = "TKR_PKID";
            returnValue.EML_KeyValue = tickRespPKID;
            returnValue.EML_Domain = "0";
            returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];
            return returnValue;
        }

        private string GetTicketResponseEmailBody(BilletterieAPIWS.ticketObject tckObj, string responseMessage, string stsName)
        {
            string returnValue = "";

            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>Ticket # [ T" + tckObj.TCK_PKID + " ] response has been successfully submitted.</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : " + tckObj.TCK_Subject + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Status:</b>" + stsName + "<br/></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Date :</b>" + DateTime.Now.ToString() + "<br/><p>Dear " + ConfigurationManager.AppSettings["OrganisationName"] + " Client,<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>" + responseMessage + "<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><table style='margin-left:10px; border-collapse: collapse;'><tr style='border: none;'><td style='border-left:  solid 3px blue; min-height:30px;'><i><b>" + tckObj.TCK_Message + "</b></i></td></tr></table><br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Note that any attached documents are ONLY accessible through the help desk system. Please feel free to ask if you may have any other query. <br /><br />Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";

            return returnValue;
        }

        protected void btnSendResponse_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();
                bool documentIsValid = true;

                if (txtMobileResponseMessage.Text.Trim() != "")
                {
                    string errorMessage = ValidateInputForXSS(txtMobileResponseMessage.Text.Trim());
                    if (errorMessage == "")
                    {
                        if (flpResponseUpload.HasFile)
                        {
                            MimeType mimeType = new MimeType();
                            string mimeTypeName = mimeType.GetMimeType(flpResponseUpload.FileBytes, flpResponseUpload.FileName);
                            if (!cm.ValidMimeType(mimeTypeName, Path.GetExtension(flpResponseUpload.FileName)))
                            {
                                documentIsValid = false;
                            }
                        }

                        if (documentIsValid)
                        {
                            DataAccess da = new DataAccess();
                            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                            BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                            BilletterieAPIWS.InsertResponseObject insRespMain = new BilletterieAPIWS.InsertResponseObject();
                            //Save document
                            BilletterieAPIWS.ticketResponseObject tickRespObj = new BilletterieAPIWS.ticketResponseObject();
                            tickRespObj = PopulateTicketResponse();
                            tickRespObj.RST_PKID = 1;
                            insRespMain = bilAPIWS.InsertBilletterieTicketResponseRecord(tickRespObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //insRespMain = da.InsertBilletterieTicketResponseRecord(tickRespObj);

                            if (insRespMain.noError)
                            {
                                //Upload document with response
                                if (flpResponseUpload.HasFile)
                                {
                                    BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();

                                    BilletterieAPIWS.fileAttachmentObject attObj = new BilletterieAPIWS.fileAttachmentObject();
                                    attObj = PopulateDocumentObject(usrSession.USR_PKID);
                                    string errMessage = ValidateFileBeforeUpload();
                                    if (errMessage == "")
                                    {
                                        insResp = bilAPIWS.InsertBilletterieDocumentRecord(attObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //insResp = da.InsertBilletterieDocumentRecord(attObj);
                                        tickRespObj.TKR_HasFile = true;
                                        string destFile = attObj.DCM_DerivedName;
                                        if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                                        {
                                            destFile = cm.MoveDocuments(attObj.DCM_DocumentPath, insResp.insertedPKID.ToString());
                                        }
                                        BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                                        updResp = bilAPIWS.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), destFile, insRespMain.insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //updResp = da.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), destFile, insRespMain.insertedPKID);
                                    }
                                }

                                //Update ticket with new status
                                if (tickRespObj.TKR_HasFile == true)
                                {
                                    bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + ", TCK_HasFile = 1 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + ", TCK_HasFile = 1 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString());
                                }
                                else
                                {
                                    bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + " where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + " where TCK_PKID = " + tickRespObj.TCK_PKID.ToString());
                                }
                                lblSendResponseError.Text = "";
                                lblSendResponseError.Visible = false;

                                if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                                {

                                    if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                    {
                                        BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                        emlObj = PopulateEmailObject(lblTCKPKID.Text, insRespMain.insertedPKID);
                                        SendMail sm = new SendMail();
                                        SMTPMailResponseObject smtRespObj = new SMTPMailResponseObject();
                                        smtRespObj = sm.SendSMTPMail(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer);
                                        if (smtRespObj.noError)
                                        {
                                            emlObj.EML_Status = "2";
                                        }
                                        else
                                        {
                                            emlObj.EML_Status = "1";
                                        }
                                        //EmailDispatcherService emsWS = new EmailDispatcherService();
                                        //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                        opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }
                                    else
                                    {
                                        //EmailDispatcherService emsWS = new EmailDispatcherService();
                                        //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                        BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                        emlObj = PopulateEmailObject(lblTCKPKID.Text, insRespMain.insertedPKID);
                                        opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }

                                }
                                PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                                LoadTicketDetails(lblTCKPKID.Text);
                                txtMobileResponseMessage.Text = "";
                            }
                            else
                            {
                                lblSendResponseError.Text = insRespMain.errorMessage;
                                lblSendResponseError.Visible = true;
                            }
                        }
                        else
                        {
                            lblSendResponseError.Text = "Attached document is invalid.";
                            lblSendResponseError.Visible = true;
                        }
                    }
                }
                else
                {
                    lblSendResponseError.Text = "You need to provide your comments or feedback in order to continue.";
                    lblSendResponseError.Visible = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnDeleteTicket_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMobileResponseMessage.Text.Trim() != "")
                {
                    DataAccess da = new DataAccess();
                    Common cm = new Common();
                    BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                    BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                    //Save document
                    BilletterieAPIWS.ticketResponseObject tickRespObj = new BilletterieAPIWS.ticketResponseObject();
                    tickRespObj = PopulateTicketResponse(7, Int32.Parse(lblTCKPKID.Text), txtMobileResponseMessage.Text.Trim());
                    tickRespObj.RST_PKID = 3;
                    insResp = bilAPIWS.InsertBilletterieTicketResponseRecord(tickRespObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    //insResp = da.InsertBilletterieTicketResponseRecord(tickRespObj);
                    if (insResp.noError)
                    {
                        //Update ticket with new status
                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 7, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 7, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + tickRespObj.TCK_PKID.ToString());
                        lblSendResponseError.Text = "";
                        lblSendResponseError.Visible = false;
                        //ModalPopupExtenderTicketDetail.Hide();

                        if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                        {
                            //EmailDispatcherService emsWS = new EmailDispatcherService(); ;
                            //EmailMessageObject emlObj = new EmailMessageObject();
                            //emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                            //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);

                            if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                            {
                                BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                SendMail sm = new SendMail();
                                SMTPMailResponseObject smtRespObj = new SMTPMailResponseObject();
                                smtRespObj = sm.SendSMTPMail(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer);
                                if (smtRespObj.noError)
                                {
                                    emlObj.EML_Status = "2";
                                }
                                else
                                {
                                    emlObj.EML_Status = "1";
                                }
                                //EmailDispatcherService emsWS = new EmailDispatcherService();
                                //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                            else
                            {
                                //EmailDispatcherService emsWS = new EmailDispatcherService();
                                //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }

                        }

                        //lblConfirmHeading.Text = "Ticket deletion confirmation.";
                        //lblConfirmationMessage.Text = "Thank you for deleting your ticket. A confirmation email will be sent to you shortly.";
                        //ModalPopupExtenderConfirm.Show();

                        BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                        usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                        txtMobileResponseMessage.Text = "";
                    }
                    else
                    {
                        lblSendResponseError.Text = insResp.errorMessage;
                        lblSendResponseError.Visible = true;
                        //ModalPopupExtenderTicketDetail.Show();
                    }

                }
                else
                {
                    lblSendResponseError.Text = "You need to provide your comments or feedback in order to continue.";
                    lblSendResponseError.Visible = true;
                    //ModalPopupExtenderTicketDetail.Show();
                }
            }
            catch (Exception)
            {

            }
        }

        protected void btnAcceptSolution_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMobileResponseMessage.Text.Trim() != "")
                {
                    string errorMessage = ValidateInputForXSS(txtMobileResponseMessage.Text.Trim());
                    if (errorMessage == "")
                    {
                        BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                        usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        Common cm = new Common();
                        DataAccess da = new DataAccess();
                        BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                        BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                        //Save document
                        BilletterieAPIWS.ticketResponseObject tickRespObj = new BilletterieAPIWS.ticketResponseObject();
                        tickRespObj = PopulateTicketResponse();
                        tickRespObj.RST_PKID = 5;
                        insResp = bilAPIWS.InsertBilletterieTicketResponseRecord(tickRespObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //insResp = da.InsertBilletterieTicketResponseRecord(tickRespObj);
                        if (insResp.noError)
                        {
                            //Update ticket with new status
                            //if ()
                            bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 6, TCK_ClosedBy = 1, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 6, TCK_ClosedBy = 1, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + tickRespObj.TCK_PKID.ToString());
                            lblSendResponseError.Text = "";
                            lblSendResponseError.Visible = false;
                            //ModalPopupExtenderTicketDetail.Hide();

                            if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                            {
                                //EmailDispatcherService emsWS = new EmailDispatcherService(); ;
                                //EmailMessageObject emlObj = new EmailMessageObject();
                                //emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);

                                if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                {
                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                    emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                    SendMail sm = new SendMail();
                                    SMTPMailResponseObject smtRespObj = new SMTPMailResponseObject();
                                    smtRespObj = sm.SendSMTPMail(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer);
                                    if (smtRespObj.noError)
                                    {
                                        emlObj.EML_Status = "2";
                                    }
                                    else
                                    {
                                        emlObj.EML_Status = "1";
                                    }
                                    //EmailDispatcherService emsWS = new EmailDispatcherService();
                                    //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                                else
                                {
                                    //EmailDispatcherService emsWS = new EmailDispatcherService();
                                    //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                    emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }


                            }
                            PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                            txtMobileResponseMessage.Text = "";
                        }
                        else
                        {
                            lblSendResponseError.Text = insResp.errorMessage;
                            lblSendResponseError.Visible = true;
                        }
                    }
                    else
                    {
                        lblSendResponseError.Text = errorMessage;
                        lblSendResponseError.Visible = true;
                    }
                }
                else
                {
                    lblSendResponseError.Text = "You need to provide your comments or feedback in order to continue.";
                    lblSendResponseError.Visible = true;
                }
            }
            catch (Exception)
            {

            }
        }

        protected void btnRejectSolution_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtMobileResponseMessage.Text.Trim() != "")
                {
                    string errorMessage = ValidateInputForXSS(txtMobileResponseMessage.Text.Trim());
                    if (errorMessage == "")
                    {
                        DataAccess da = new DataAccess();
                        BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                        BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                        //Save document
                        BilletterieAPIWS.ticketResponseObject tickRespObj = new BilletterieAPIWS.ticketResponseObject();
                        tickRespObj = PopulateTicketResponse(3, Int32.Parse(lblTCKPKID.Text), txtMobileResponseMessage.Text.Trim());
                        tickRespObj.RST_PKID = 6;
                        insResp = bilAPIWS.InsertBilletterieTicketResponseRecord(tickRespObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //insResp = da.InsertBilletterieTicketResponseRecord(tickRespObj);
                        if (insResp.noError)
                        {
                            //Update ticket with new status
                            bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 3 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 3 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString());
                            lblSendResponseError.Text = "";
                            lblSendResponseError.Visible = false;
                            //ModalPopupExtenderTicketDetail.Hide();

                            if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                            {
                                //EmailDispatcherService emsWS = new EmailDispatcherService(); ;
                                //EmailMessageObject emlObj = new EmailMessageObject();
                                //emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);

                                if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                {
                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                    emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                    SendMail sm = new SendMail();
                                    SMTPMailResponseObject smtRespObj = new SMTPMailResponseObject();
                                    smtRespObj = sm.SendSMTPMail(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer);
                                    if (smtRespObj.noError)
                                    {
                                        emlObj.EML_Status = "2";
                                    }
                                    else
                                    {
                                        emlObj.EML_Status = "1";
                                    }
                                    //EmailDispatcherService emsWS = new EmailDispatcherService();
                                    //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                                else
                                {
                                    //EmailDispatcherService emsWS = new EmailDispatcherService();
                                    //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                    emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }

                            }

                            //lblConfirmHeading.Text = "Ticket solution rejection confirmation.";
                            //lblConfirmationMessage.Text = "Thank you for rejecting the solution for your ticket. This ticket will be automatically re-submitted to " + ConfigurationManager.AppSettings["OrganisationName"] + ". A confirmation email will be sent to you shortly.";
                            //ModalPopupExtenderConfirm.Show();

                            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                            PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                            txtMobileResponseMessage.Text = "";
                        }
                        else
                        {
                            lblSendResponseError.Text = insResp.errorMessage;
                            lblSendResponseError.Visible = true;
                            //ModalPopupExtenderTicketDetail.Show();
                        }
                    }
                    else
                    {
                        lblSendResponseError.Text = errorMessage;
                        lblSendResponseError.Visible = true;
                    }
                }
                else
                {
                    lblSendResponseError.Text = "You need to provide your comments or feedback in order to continue.";
                    lblSendResponseError.Visible = true;
                    //ModalPopupExtenderTicketDetail.Show();
                }
            }
            catch (Exception)
            {

            }
        }



        public string ValidateInputForXSS(string inputStringValue)
        {
            Common cm = new Common();
            string returnValue = "";

            //if (tckObj != null)
            //{

            if (!ValidateAntiXSS(inputStringValue))
            {
                return returnValue = "Ticket response contains illegal characters.";
            }
            if (!ValidateAntiXSSS(inputStringValue))
            {
                return returnValue = "Ticket response contains illegal characters.";
            }

          
            return returnValue;
        }

        public static bool ValidateAntiXSS(string inputParameter)
        {
            if (string.IsNullOrEmpty(inputParameter))
                return true;

            // Following regex convers all the js events and html tags mentioned in followng links.
            //https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet                 
            //https://msdn.microsoft.com/en-us/library/ff649310.aspx

            var pattren = new StringBuilder();

            //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.             
            pattren.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");

            //Checks any html tags i.e. <script, <embed, <object etc.
            pattren.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");

            return !Regex.IsMatch(System.Web.HttpUtility.UrlDecode(inputParameter), pattren.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static bool ValidateAntiXSSS(string inputParameter)
        {
            bool returnValue = true;

            if (string.IsNullOrEmpty(inputParameter))
                return true;

            if (inputParameter.Contains(">"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("<"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("="))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("+"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("%"))
            {
                returnValue = false;
            }

            return returnValue;
        }


    }
}