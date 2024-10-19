using NewBilletterie.Classes;
//using NewBilletterie.CUBAServerService;
//using NewBilletterie.EmailWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class ViewTickets : System.Web.UI.Page
    {
        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();
        
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();

        public List<BilletterieAPIWS.fileAttachmentObject> attachmentList { get; set; }
        List<RecordAttachments> ListTicketPDFAttachments { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                TypeConverter converter = TypeDescriptor.GetConverter(typeof(System.Drawing.Color));
                gridTickets.HeaderStyle.BackColor = (System.Drawing.Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["MainThemeBackColor"]);

                if (bool.Parse(ConfigurationManager.AppSettings["GenerateCaseForm"]))
                {
                    litCaseFileLabel.Visible = true;
                    lnkCaseFileDocument.Visible = true;
                }

                if (bool.Parse(ConfigurationManager.AppSettings["UseDefaultLabels"]) == false)
                {
                    try
                    {
                        litViewTickets.Text = ConfigurationManager.AppSettings["litViewTicketsText"];
                    }
                    catch (Exception)
                    {

                    }
                }

                Session["MobileViewLink"] = "~/Mobile/MobileViewTickets.aspx";

                if (Session["userObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

                    //Check if user is coming from Password Reset session
                    if (usrSession.OFL_PKID == 3)
                    {
                        Session["GlobalSession"] = null;
                        Session["userObjectCookie"] = null;
                        Response.Redirect("~/Index.aspx", false);
                    }
                    PopulateTicketGrid(usrSession.USR_PKID, true);
                }
                else
                {
                    Response.Redirect("~/Index.aspx", false);
                }
            }
        }

        private allowedDateResponse NewTicketAllowed(string displayMessage)
        {
            DataAccess da = new DataAccess();
            allowedDateResponse allwdResponse = new allowedDateResponse();

            string newDateString = "";
            newDateString = GetAllowedOfficeDate(DateTime.Now.ToString("yyyy-MM-dd"));
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

        private void PopulateTicketGrid(int userPKID, bool viewOpenOnly)
        {
            string countMessage = "";
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            if (viewOpenOnly)
            {
                ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5) and TCK_Viewable = 1 and TCT_PKID in (1,4) and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                countMessage = " open ticket(s)";
            }
            else
            {
                ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5,6,7,8) and TCK_Viewable = 1 and TCT_PKID in (1,4) and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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
                ModalPopupExtenderTicketDetail.Show();
                lblTicketDetailHeading.Text = "Ticket [" + lnk.Text + "]";
                LoadTicketDetails(lnk.CommandArgument);
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
                ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, TCK_Reference, TCK_DateClosed, TCK_CaseIdentifier from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                #region Ticket
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtSubject.Text = ds.Tables[0].Rows[0]["TCK_Subject"].ToString();
                        txtReference.Text = ds.Tables[0].Rows[0]["TCK_Reference"].ToString();
                        txtEntepriseNo.Text = ds.Tables[0].Rows[0]["TCK_CaseIdentifier"].ToString();
                        txtMessage.Text = ds.Tables[0].Rows[0]["TCK_Message"].ToString();
                        txtDateCreated.Text = ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString();
                        txtStatus.Text = ds.Tables[0].Rows[0]["STS_StatusName"].ToString();
                        txtCategory.Text = ds.Tables[0].Rows[0]["CAT_CategoryName"].ToString();
                        lblTCKPKID.Text = ds.Tables[0].Rows[0]["TCK_PKID"].ToString();
                        lblSTSPKID.Text = ds.Tables[0].Rows[0]["STS_PKID"].ToString();
                        txtResponseFeedback.Text = "";

                        if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "1") //Submitted
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = true;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendInformation.Visible = true;
                            flpResponseUpload.Visible = true;

                            btnOmbudsman.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "2") //Under Treatment
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendInformation.Visible = true;
                            flpResponseUpload.Visible = true;

                            btnOmbudsman.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "3") //Re-Submitted
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = true;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendInformation.Visible = true;
                            flpResponseUpload.Visible = true;

                            btnOmbudsman.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "4") //Error Submitting
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = true;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendInformation.Visible = true;
                            flpResponseUpload.Visible = true;

                            btnOmbudsman.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "5") //Resolved
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = true;
                            btnRejectSolution.Visible = true;
                            btnSendInformation.Visible = true;
                            flpResponseUpload.Visible = true;

                            btnOmbudsman.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "6") //Closed
                        {
                            txtResponseFeedback.Visible = false;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendInformation.Visible = false;
                            flpResponseUpload.Visible = false;

                            if (DateTime.Parse(GetOmbudsDueDate(ds.Tables[0].Rows[0]["TCK_DateClosed"].ToString())) <= DateTime.Now)
                            {
                                btnOmbudsman.Visible = true;
                            }
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "7") //Deleted By User
                        {
                            txtResponseFeedback.Visible = false;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendInformation.Visible = false;
                            flpResponseUpload.Visible = false;

                            btnOmbudsman.Visible = false;
                        }
                        else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "8") //Deleted By Office
                        {
                            txtResponseFeedback.Visible = false;
                            btnDeleteTicket.Visible = false;
                            btnAcceptSolution.Visible = false;
                            btnRejectSolution.Visible = false;
                            btnSendInformation.Visible = false;
                            flpResponseUpload.Visible = false;

                            if (DateTime.Parse(GetOmbudsDueDate(ds.Tables[0].Rows[0]["TCK_DateClosed"].ToString())) <= DateTime.Now)
                            {
                                btnOmbudsman.Visible = true;
                            }
                        }
                    }
                }



                if (bool.Parse(ConfigurationManager.AppSettings["AllowDisableNewTickets"]))
                {
                    allowedDateResponse allwdDateResp = new allowedDateResponse();
                    allwdDateResp = NewTicketAllowed("New tickets cannot be created until ");
                    if (allwdDateResp.dateAllowed == false)
                    {
                        btnSendInformation.Enabled = false;
                        btnSendInformation.Font.Strikeout = true;
                        btnSendInformation.ToolTip = allwdDateResp.displayMessage;
                    }
                    else
                    {
                        btnSendInformation.Enabled = true;
                        btnSendInformation.Font.Strikeout = false;
                        btnSendInformation.ToolTip = "";
                    }
                }



                #endregion

                DataSet dsResp = new DataSet();
                dsResp = bilAPIWS.GetBilletterieDataSet("select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, R.STS_PKID, S.STS_StatusName, D.DCM_OriginalName, D.DCM_DerivedName, DCM_PKID, DCM_UniqueID from TB_TKR_TicketResponse R inner join TB_STS_Status S on R.STS_PKID  = S.STS_PKID left outer join TB_DCM_Document D on (R.TKR_PKID = D.TNK_PKID and D.DCL_PKID = 2) where TCK_PKID = " + ticketPKID + " and TKR_VisibleToClient = 1 order by TKR_PKID desc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                #region Ticket responses
                if (dsResp != null)
                {
                    if (dsResp.Tables[0].Rows.Count > 0)
                    {
                        Session["ViewResponseResults"] = dsResp.Tables[0];
                        GridViewResponses.DataSource = null;
                        GridViewResponses.DataSource = dsResp.Tables[0];
                        GridViewResponses.DataBind();
                        GridViewResponses.PageIndex = 0;

                        lblNoneResponses.Visible = false;
                        GridViewResponses.Visible = true;
                    }
                    else
                    {
                        lblNoneResponses.Visible = true;
                        GridViewResponses.Visible = false;
                    }
                }
                else
                {
                    lblNoneResponses.Visible = true;
                    GridViewResponses.Visible = false;

                }
                #endregion

                BilletterieAPIWS.SelectStringResponseObject strResp = new BilletterieAPIWS.SelectStringResponseObject();
                strResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_TKR_TicketResponse R where TCK_PKID = " + ticketPKID + " and STS_PKID = 3", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
               
                #region Limit resubmissions
                if (strResp != null)
                {
                    if (strResp.selectedPKID != "0")
                    {
                        btnRejectSolution.Visible = false;
                        btnSendInformation.Visible = false;
                    }
                }
                #endregion

                #region Populate documents
                DataSet dsDocs = new DataSet();
                dsDocs = bilAPIWS.GetBilletterieDataSet("select DCM_UniqueID, DCM_OriginalName, DCM_PKID from TB_DCM_Document where DCT_PKID = 1 and DCL_PKID = 1 and TNK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                if (dsDocs != null)
                {
                    if (dsDocs.Tables[0].Rows.Count > 0)
                    {
                        GridViewUploadedDocs.DataSource = dsDocs.Tables[0];
                        GridViewUploadedDocs.DataBind();
                    }
                }

                #endregion

                #region Case form

                if (bool.Parse(ConfigurationManager.AppSettings["GenerateCaseForm"]))
                {
                    DataSet dsCaseDocs = new DataSet();
                    dsCaseDocs = bilAPIWS.GetBilletterieDataSet("select DCM_UniqueID, DCM_OriginalName from TB_DCM_Document where DCT_PKID = 4 and DCL_PKID = 1 and TNK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    if (dsCaseDocs != null)
                    {
                        if (dsCaseDocs.Tables[0].Rows.Count > 0)
                        {
                            lnkCaseFileDocument.Visible = true;
                            litCaseFileLabel.Visible = true;
                            lnkCaseFileDocument.Text = dsCaseDocs.Tables[0].Rows[0]["DCM_OriginalName"].ToString();
                            string Url = "../GetDocument.aspx?docID=" + dsCaseDocs.Tables[0].Rows[0]["DCM_UniqueID"].ToString();
                            lnkCaseFileDocument.Attributes.Add("onclick", "javascript: OpenInNewTab('" + Url + "')");
                        }
                        else
                        {
                            lnkCaseFileDocument.Visible = false;
                            litCaseFileLabel.Visible = false;
                        }
                    }
                    else
                    {
                        lnkCaseFileDocument.Visible = false;
                        litCaseFileLabel.Visible = false;
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetOmbudsDueDate(string dateClosed)
        {
            string retValue = "";
            DataAccess da = new DataAccess();
            DateTime newCalculatedDate = DateTime.Parse(dateClosed).AddHours(240);
            string newDateString = "";
            if (bool.Parse(ConfigurationManager.AppSettings["GetCalculatedDates"]))
            {
                DataSet fbdDS = new DataSet();
                fbdDS = bilAPIWS.GetCUBADataSet("select * from TB_FBD_ForbiddenDates where FBD_Year = " + DateTime.Now.Year.ToString() + " and FBD_ForbiddenDate between '" + DateTime.Parse(dateClosed).ToString("yyyy-MM-dd") + "' and '" + newCalculatedDate.ToString("yyyy-MM-dd") + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                double daysIncrease = 0;
                if (fbdDS != null)
                {
                    if (fbdDS.Tables[0].Rows.Count > 0)
                    {
                        daysIncrease = fbdDS.Tables[0].Rows.Count * 24;
                    }
                }
                newCalculatedDate = newCalculatedDate.AddHours(daysIncrease);
                newDateString = GetAllowedOfficeDate(newCalculatedDate.ToString("yyyy-MM-dd"));
                retValue = newDateString;
            }
            else
            {
                newDateString = GetFormattedOfficeDate(newCalculatedDate.ToString("yyyy-MM-dd"));
                retValue = newDateString;
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

        protected void gridTickets_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Attributes.Add("style", "cursor:pointer;font-size:12px;font-weight:600;");
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#6BAB4D'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");

                if (e.Row.Cells[4].Text != "")
                {

                    Button lnk = (Button)e.Row.FindControl("lnkTicketMessageHover");
                    if (lnk.Text.Length > 25)
                    {
                        lnk.ToolTip = "SUBJECT: " + lnk.Text + System.Environment.NewLine + System.Environment.NewLine + "MESSAGE:  " + lnk.CommandArgument;
                        lnk.Text = lnk.Text.Substring(0, 25) + "...";
                    }
                    else
                    {
                        lnk.Text = lnk.Text;
                        lnk.ToolTip = "SUBJECT: " + lnk.Text + System.Environment.NewLine + System.Environment.NewLine + "MESSAGE:  " + lnk.CommandArgument;
                    }

                    ImageButton imgEdt = (ImageButton)e.Row.FindControl("imgEdit");
                    ImageButton imgDel = (ImageButton)e.Row.FindControl("imgDelete");

                    if (e.Row.Cells[4].Text.Trim().ToLower() == "submitted")
                    {
                        imgEdt.Enabled = true;
                        imgDel.Enabled = true;
                    }
                    else
                    {
                        imgEdt.Enabled = false;
                        imgDel.Enabled = false;

                        imgEdt.Visible = false;
                        imgDel.Visible = false;

                        if (e.Row.Cells[4].Text.ToLower() == "solved" || e.Row.Cells[4].Text.ToLower() == "deleted by office")
                        {
                            e.Row.ForeColor = System.Drawing.Color.Red;
                        }
                        else if (e.Row.Cells[4].Text.ToLower() == "closed" || e.Row.Cells[4].Text.ToLower() == "deleted by user")
                        {
                            e.Row.ForeColor = System.Drawing.Color.DarkBlue;
                        }
                    }
                }
            }
        }

        protected void gridTickets_RowBoundOperations(object sender, GridViewCommandEventArgs e)
        {
            DataAccess da = new DataAccess();
            try
            {
                txtEditResponse.Text = "";
                if (e.CommandName == "EditTicket")
                {
                    btnSaveEdit.Visible = true;
                    btnDeleteRow.Visible = false;
                    string ticketNumber = "";
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Message, CAT_PKID from TB_TCK_Ticket where TCK_PKID = " + e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Message, CAT_PKID from TB_TCK_Ticket where TCK_PKID = " + e.CommandArgument.ToString());
                    ticketNumber = ds.Tables[0].Rows[0]["TCK_TicketNumber"].ToString();
                    txtEditResponse.Text = ds.Tables[0].Rows[0]["TCK_Message"].ToString();
                    lblEditHeading.Text = "Edit Ticket [" + ticketNumber + "]";
                    lblEditTCKPKID.Text = e.CommandArgument.ToString();
                    LoadDropDowns();
                    trCategory.Visible = false;
                    trSubCategory.Visible = false;
                    lblEditErrorMessage.Text = "";
                    lblEditErrorMessage.Visible = false;
                    ModalPopupExtenderEdit.Show();
                }
                else if (e.CommandName == "DeleteTicket")
                {
                    btnSaveEdit.Visible = false;
                    btnDeleteRow.Visible = true;

                    string ticketNumber = "";
                    BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                    selResp = bilAPIWS.GetBilletterieScalar("select SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber from TB_TCK_Ticket where TCK_PKID = " + e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    ticketNumber = selResp.selectedPKID;
                    lblDeleteHeading.Text = "Delete Ticket [" + ticketNumber + "]";
                    lblDeleteTCKPKID.Text = e.CommandArgument.ToString();
                    ModalPopupExtenderDelete.Show();
                }

            }
            catch (Exception)
            {

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
                else if (lnk.CommandArgument == "1002")
                {
                    img.ImageUrl = "~/Images/outgoing.png";
                }

                HyperLink hp = new HyperLink();
                LinkButton lnkDoc = (LinkButton)e.Row.FindControl("lnkResponseDocumentLink");
                string Url = "../GetDocument.aspx?docID=" + lnkDoc.CommandArgument;
                hp.Attributes.Add("onclick", "javascript: OpenInNewTab('" + Url + "')");
                hp.NavigateUrl = Url;
                hp.Text = lnkDoc.Text;
                lnkDoc.Controls.Add(hp);
            }
        }

        private BilletterieAPIWS.EmailMessageObject PopulateEmailObject(string ticketPKID, string tickRespPKID)
        {
            string newStatusName = "";
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            BilletterieAPIWS.ticketObject tckObj = new BilletterieAPIWS.ticketObject();
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, T.USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateDue, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, USR_UserLogin, USR_FirstName + ' ' + USR_LastName [UserNames], U.USR_EmailAccount, U.USR_MobileNumber, R.USC_SourceName, (select count(*) from TB_TCK_Ticket TK where TK.USR_PKID = T.USR_PKID and STS_PKID in (1,2,3)) [TotalTickets], U.USC_PKID, TCK_CaseIdentifier from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_USR_User U on T.USR_PKID = U.USR_PKID inner join TB_USC_UserSource R on U.USC_PKID = R.USC_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_MailBody = GetTicketResponseEmailBody(tckObj, txtResponseFeedback.Text.Trim(), newStatusName);
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

        private EmailMessageObject PopulateEmailObject(string ticketPKID, string tickRespPKID, string responseMessage)
        {
            string newStatusName = "";
            EmailMessageObject returnValue = new EmailMessageObject();
            BilletterieAPIWS.ticketObject tckObj = new BilletterieAPIWS.ticketObject();
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select TCK_PKID, T.CAT_PKID, T.USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateDue, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, USR_UserLogin, USR_FirstName + ' ' + USR_LastName [UserNames], U.USR_EmailAccount, U.USR_MobileNumber, R.USC_SourceName, (select count(*) from TB_TCK_Ticket TK where TK.USR_PKID = T.USR_PKID and STS_PKID in (1,2,3)) [TotalTickets], U.USC_PKID, TCK_CaseIdentifier from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_USR_User U on T.USR_PKID = U.USR_PKID inner join TB_USC_UserSource R on U.USC_PKID = R.USC_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_MailBody = GetTicketResponseEmailBody(tckObj, responseMessage.Trim(), newStatusName);
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

            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>Ticket # [T" + tckObj.TCK_PKID + " ] has been successfully submitted.</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : " + tckObj.TCK_Subject + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Status:</b>" + stsName + "<br/></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Date :</b>" + DateTime.Now.ToString() + "<br/><p>Dear " + ConfigurationManager.AppSettings["OrganisationName"] + " Client,<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>" + responseMessage + "<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><table style='margin-left:10px; border-collapse: collapse;'><tr style='border: none;'><td style='border-left:  solid 3px blue; min-height:30px;'><i><b>" + tckObj.TCK_Message + "</b></i></td></tr></table><br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Note that any attached documents are ONLY accessible through the help desk system. Please feel free to ask if you may have any other query. <br /><br />Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
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
                    string filePath = returnValue.DCM_DerivedName;
                    if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                    {
                        filePath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + returnValue.DCM_DerivedName; //Server.MapPath("~/Temp/" + atchObj.DCM_DerivedName);
                        flpResponseUpload.SaveAs(filePath);
                    }
                    
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        protected void btnAcceptSolution_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtResponseFeedback.Text.Trim() != "")
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
                    if (insResp.noError)
                    {
                        //Update ticket with new status
                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 6, TCK_ClosedBy = 1 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        lblPopErrorMessage.Text = "";
                        lblPopErrorMessage.Visible = false;
                        ModalPopupExtenderTicketDetail.Hide();

                        if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                        {
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
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                            else
                            {
                                BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                        }
                        lblConfirmHeading.Text = "Ticket solution acceptance confirmation.";
                        lblConfirmationMessage.Text = "Thank you for accepting the solution for your ticket. A confirmation email will be sent to you shortly.";
                        ModalPopupExtenderConfirm.Show();

                        PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                    }
                    else
                    {
                        lblPopErrorMessage.Text = insResp.errorMessage;
                        lblPopErrorMessage.Visible = true;
                        ModalPopupExtenderTicketDetail.Show();
                    }
                }
                else
                {
                    lblPopErrorMessage.Text = "You need to provide your comments or feedback in order to continue.";
                    lblPopErrorMessage.Visible = true;
                    ModalPopupExtenderTicketDetail.Show();
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

                if (txtResponseFeedback.Text.Trim() != "")
                {
                    DataAccess da = new DataAccess();
                    BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                    BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                    //Save document
                    BilletterieAPIWS.ticketResponseObject tickRespObj = new BilletterieAPIWS.ticketResponseObject();
                    tickRespObj = PopulateTicketResponse(3, Int32.Parse(lblTCKPKID.Text), txtResponseFeedback.Text.Trim());
                    tickRespObj.RST_PKID = 6;
                    insResp = bilAPIWS.InsertBilletterieTicketResponseRecord(tickRespObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    if (insResp.noError)
                    {
                        //Update ticket with new status
                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 3, OFC_PKID = 0 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        lblPopErrorMessage.Text = "";
                        lblPopErrorMessage.Visible = false;
                        ModalPopupExtenderTicketDetail.Hide();

                        if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                        {
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
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                            else
                            {
                                BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }

                        }
                        lblConfirmHeading.Text = "Ticket solution rejection confirmation.";
                        lblConfirmationMessage.Text = "Thank you for rejecting the solution for your ticket. This ticket will be automatically re-submitted to " + ConfigurationManager.AppSettings["OrganisationName"] + ". A confirmation email will be sent to you shortly.";
                        ModalPopupExtenderConfirm.Show();

                        BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                        usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                    }
                    else
                    {
                        lblPopErrorMessage.Text = insResp.errorMessage;
                        lblPopErrorMessage.Visible = true;
                        ModalPopupExtenderTicketDetail.Show();
                    }

                }
                else
                {
                    lblPopErrorMessage.Text = "You need to provide your comments or feedback in order to continue.";
                    lblPopErrorMessage.Visible = true;
                    ModalPopupExtenderTicketDetail.Show();
                }
            }
            catch (Exception)
            {

            }
        }

        protected void btnSendInformation_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();
                bool documentIsValid = true;
                if (cm.ValidateAntiXSSS(txtResponseFeedback.Text) && cm.ValidateAntiXSS(txtResponseFeedback.Text))
                {
                    if (txtResponseFeedback.Text.Trim() != "")
                    {
                        if (flpResponseUpload.HasFile)
                        {
                            MimeType mimeType = new MimeType();
                            string mimeTypeName = mimeType.GetMimeType(flpResponseUpload.FileBytes, flpResponseUpload.FileName);
                            if (!cm.ValidMimeType(mimeTypeName, Path.GetExtension(flpResponseUpload.FileName)))
                            {
                                documentIsValid = false;
                                lblPopErrorMessage.Text = "Attached document is invalid.";
                                lblPopErrorMessage.Visible = true;
                                ModalPopupExtenderTicketDetail.Show();
                            }
                            else
                            {
                                string linksPDF = ValidatePDFFileLinks(flpResponseUpload.FileBytes, Path.GetExtension(flpResponseUpload.FileName));
                                if (linksPDF != "")
                                {
                                    documentIsValid = false;
                                    lblPopErrorMessage.Text = "Attached document is invalid.";
                                    lblPopErrorMessage.Visible = true;
                                    ModalPopupExtenderTicketDetail.Show();
                                }
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
                            if (insRespMain.noError)
                            {
                                //Upload document with response
                                if (flpResponseUpload.HasFile)
                                {
                                    string errMessage = ValidateFileBeforeUpload(usrSession.USR_PKID.ToString(), Path.GetExtension(flpResponseUpload.FileName));
                                    if (errMessage == "")
                                    {
                                        string linksPDF = ValidatePDFFileLinks(flpResponseUpload.FileBytes, Path.GetExtension(flpResponseUpload.FileName));
                                        if (linksPDF == "")
                                        {
                                            MimeType mimeType = new MimeType();

                                            string mimeTypeName = mimeType.GetMimeType(flpResponseUpload.FileBytes, flpResponseUpload.FileName);

                                            if (cm.ValidMimeType(mimeTypeName, Path.GetExtension(flpResponseUpload.FileName)))
                                            {

                                                BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();

                                                BilletterieAPIWS.fileAttachmentObject attObj = new BilletterieAPIWS.fileAttachmentObject();
                                                attObj = PopulateDocumentObject(usrSession.USR_PKID);

                                                errMessage = ValidateFileBeforeUpload();
                                                if (errMessage == "")
                                                {
                                                    insResp = bilAPIWS.InsertBilletterieDocumentRecord(attObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                    tickRespObj.TKR_HasFile = true;
                                                    string destFile = attObj.DCM_DerivedName;
                                                    if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                                                    {
                                                        destFile = cm.MoveDocuments(attObj.DCM_DocumentPath, insResp.insertedPKID.ToString());
                                                    }
                                                    BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                                                    updResp = bilAPIWS.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), destFile, insRespMain.insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                }
                                            }
                                            else
                                            {
                                                lblPopErrorMessage.Text = "Attached document is invalid.";
                                                lblPopErrorMessage.Visible = true;
                                                ModalPopupExtenderTicketDetail.Show();
                                            }
                                        }
                                        else
                                        {
                                            lblPopErrorMessage.Text = linksPDF;
                                            lblPopErrorMessage.Visible = true;
                                            ModalPopupExtenderTicketDetail.Show();
                                        }
                                    }
                                    else
                                    {
                                        lblPopErrorMessage.Text = errMessage;
                                        lblPopErrorMessage.Visible = true;
                                        ModalPopupExtenderTicketDetail.Show();
                                    }
                                }

                                //Update ticket with new status
                                if (tickRespObj.TKR_HasFile == true)
                                {
                                    bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + ", TCK_HasFile = 1 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                                else
                                {
                                    bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + " where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                                lblPopErrorMessage.Text = "";
                                lblPopErrorMessage.Visible = false;

                                //ModalPopupExtenderTicketDetail.Hide();
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
                                        opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }
                                    else
                                    {
                                        BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                        emlObj = PopulateEmailObject(lblTCKPKID.Text, insRespMain.insertedPKID);
                                        opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }

                                }

                                lblConfirmHeading.Text = "Ticket additional information confirmation.";
                                lblConfirmationMessage.Text = "Thank you for supplying " + ConfigurationManager.AppSettings["OrganisationName"] + " with additional information for your ticket. A confirmation email will be sent to you shortly.";
                                ModalPopupExtenderConfirm.Show();
                                PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);

                                ModalPopupExtenderTicketDetail.Show();
                                LoadTicketDetails(lblTCKPKID.Text);
                            }
                            else
                            {
                                lblPopErrorMessage.Text = insRespMain.errorMessage;
                                lblPopErrorMessage.Visible = true;
                                ModalPopupExtenderTicketDetail.Show();
                            }
                        }
                        else
                        {
                            lblPopErrorMessage.Text = "Attached document is invalid.";
                            lblPopErrorMessage.Visible = true;
                            ModalPopupExtenderTicketDetail.Show();
                        }
                    }
                    else
                    {
                        lblPopErrorMessage.Text = "You need to provide your comments or feedback in order to continue.";
                        lblPopErrorMessage.Visible = true;
                        ModalPopupExtenderTicketDetail.Show();
                    }
                }
                else
                {
                    lblPopErrorMessage.Text = "Illegal characters detected in the response.";
                    lblPopErrorMessage.Visible = true;
                    ModalPopupExtenderTicketDetail.Show();
                }
            }
            catch (Exception)
            {

            }
        }

        protected void btnDeleteTicket_Click(object sender, EventArgs e)
        {
            try
            {

                if (txtResponseFeedback.Text.Trim() != "")
                {
                    DataAccess da = new DataAccess();
                    Common cm = new Common();
                    BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                    BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                    //Save document
                    BilletterieAPIWS.ticketResponseObject tickRespObj = new BilletterieAPIWS.ticketResponseObject();
                    tickRespObj = PopulateTicketResponse(7, Int32.Parse(lblTCKPKID.Text), txtResponseFeedback.Text.Trim());
                    tickRespObj.RST_PKID = 3;
                    insResp = bilAPIWS.InsertBilletterieTicketResponseRecord(tickRespObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    if (insResp.noError)
                    {
                        //Update ticket with new status
                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 7, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        lblPopErrorMessage.Text = "";
                        lblPopErrorMessage.Visible = false;
                        ModalPopupExtenderTicketDetail.Hide();

                        if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                        {
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
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                            else
                            {
                                BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                        }
                        lblConfirmHeading.Text = "Ticket deletion confirmation.";
                        lblConfirmationMessage.Text = "Thank you for deleting your ticket. A confirmation email will be sent to you shortly.";
                        ModalPopupExtenderConfirm.Show();

                        BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                        usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                    }
                    else
                    {
                        lblPopErrorMessage.Text = insResp.errorMessage;
                        lblPopErrorMessage.Visible = true;
                        ModalPopupExtenderTicketDetail.Show();
                    }
                }
                else
                {
                    lblPopErrorMessage.Text = "You need to provide your comments or feedback in order to continue.";
                    lblPopErrorMessage.Visible = true;
                    ModalPopupExtenderTicketDetail.Show();
                }
            }
            catch (Exception)
            {

            }
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();

                if (cm.ValidateAntiXSSS(txtEditResponse.Text) && cm.ValidateAntiXSS(txtEditResponse.Text))
                {
                    if (txtEditResponse.Text.Trim() != "")
                    {
                        BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                        if (Session["userObjectCookie"] != null)
                        {
                            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        }
                        DataAccess da = new DataAccess();
                        string categoryID = GetSelectedCategory();
                        BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                        if (ddlEditDepartment.SelectedValue != "0" && categoryID != "0")
                        {
                            updResp = bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TCK_Message = '" + txtEditResponse.Text + "', CAT_PKID = " + categoryID + "  where TCK_PKID = " + lblEditTCKPKID.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                        }
                        else if (ddlEditDepartment.SelectedValue == "0" && categoryID == "0")
                        {
                            updResp = bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TCK_Message = '" + txtEditResponse.Text + "' where TCK_PKID = " + lblEditTCKPKID.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                        }
                        else
                        {
                            updResp.errorMessage = "You need to correctly select categories in order to continue.";
                            ModalPopupExtenderEdit.Show();
                        }

                        if (!updResp.noError)
                        {
                            lblEditErrorMessage.Text = updResp.errorMessage;    // "You need to provide your comments or feedback in order to continue.";
                            lblEditErrorMessage.Visible = true;
                            ModalPopupExtenderEdit.Show();
                        }
                    }
                    else
                    {
                        lblEditErrorMessage.Text = "You need to provide your comments or feedback in order to continue.";
                        lblEditErrorMessage.Visible = true;
                        ModalPopupExtenderEdit.Show();
                    }
                }
                else
                {
                    lblEditErrorMessage.Text = "Illegal characters detected in the response.";
                    lblEditErrorMessage.Visible = true;
                    ModalPopupExtenderEdit.Show();
                }
            }
            catch (Exception)
            {

            }
        }

        private string GetSelectedCategory()
        {
            string returnValue = "";

            if (ddlEditSubCategory.Visible == true)
            {
                returnValue = ddlEditSubCategory.SelectedValue;
            }
            else if (ddlEditCategory.Visible == true)
            {
                returnValue = ddlEditCategory.SelectedValue;
            }
            else if (ddlEditDepartment.Visible == true)
            {
                returnValue = ddlEditDepartment.SelectedValue;
            }
            return returnValue;
        }

        protected void btnDeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();

                if (cm.ValidateAntiXSSS(txtDeleteResponse.Text) && cm.ValidateAntiXSS(txtDeleteResponse.Text))
                {
                    if (txtDeleteResponse.Text.Trim() != "")
                    {
                        DataAccess da = new DataAccess();
                        BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                        BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                        //Save document
                        BilletterieAPIWS.ticketResponseObject tickRespObj = new BilletterieAPIWS.ticketResponseObject();
                        tickRespObj = PopulateTicketResponse(7, Int32.Parse(lblDeleteTCKPKID.Text), txtDeleteResponse.Text.Trim());
                        tickRespObj.RST_PKID = 3;
                        insResp = bilAPIWS.InsertBilletterieTicketResponseRecord(tickRespObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        if (insResp.noError)
                        {
                            //Update ticket with new status
                            bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 7, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + tickRespObj.TCK_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            lblGridRowError.Text = "";
                            lblGridRowError.Visible = false;
                            ModalPopupExtenderDelete.Hide();

                            if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                            {
                                if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                {
                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                    emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID, txtDeleteResponse.Text.Trim());
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
                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                                else
                                {
                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                    emlObj = PopulateEmailObject(tickRespObj.TCK_PKID.ToString(), insResp.insertedPKID, txtDeleteResponse.Text.Trim());
                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                            }

                            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                            PopulateTicketGrid(usrSession.USR_PKID, chkViewOpen.Checked);
                        }
                        else
                        {
                            lblGridRowError.Text = insResp.errorMessage;
                            lblGridRowError.Visible = true;
                            ModalPopupExtenderDelete.Show();
                        }

                    }
                    else
                    {
                        lblGridRowError.Text = "You need to provide your comments or feedback in order to continue.";
                        lblGridRowError.Visible = true;
                        ModalPopupExtenderDelete.Show();
                    }
                }
                else
                {
                    lblGridRowError.Text = "Illegal characters detected in the response.";
                    lblGridRowError.Visible = true;
                    ModalPopupExtenderDelete.Show();
                }
            }
            catch (Exception)
            {

            }
        }

        private BilletterieAPIWS.ticketResponseObject PopulateTicketResponse()
        {
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

            BilletterieAPIWS.ticketResponseObject returnValue = new BilletterieAPIWS.ticketResponseObject();
            returnValue.TCK_PKID = Int32.Parse(lblTCKPKID.Text);
            returnValue.OFC_PKID = usrSession.USR_PKID;
            returnValue.UST_PKID = 1;
            returnValue.TKR_ResponseMessage = txtResponseFeedback.Text.Trim();
            returnValue.TKR_ResponseDate = DateTime.Now.ToString();
            returnValue.TKR_VisibleToClient = true;
            returnValue.STS_PKID = Int32.Parse(lblSTSPKID.Text);
            returnValue.STR_PKID = 0;
            return returnValue;
        }

        private BilletterieAPIWS.ticketResponseObject PopulateTicketResponse(int newStatus, int tckPKID, string responseMessage)
        {
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

            BilletterieAPIWS.ticketResponseObject returnValue = new BilletterieAPIWS.ticketResponseObject();
            returnValue.TCK_PKID = tckPKID; 
            returnValue.OFC_PKID = usrSession.USR_PKID;
            returnValue.UST_PKID = 1;
            returnValue.TKR_ResponseMessage = responseMessage; 
            returnValue.TKR_ResponseDate = DateTime.Now.ToString();
            returnValue.TKR_VisibleToClient = true;
            returnValue.STS_PKID = newStatus;
            returnValue.STR_PKID = 0;

            return returnValue;
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

        protected void GridViewResponses_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                GridViewResponses.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewResponseResults"];
                GridViewResponses.DataSource = dt;
                GridViewResponses.DataBind();
                ModalPopupExtenderTicketDetail.Show();
            }
            catch (Exception)
            {

            }
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

        protected void btnSaveUploadedFile_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();
                lblMainErrorMessage.Text = "";

                if (flpResponseUpload.HasFile)
                {
                    MimeType mimeType = new MimeType();

                    string mimeTypeName = mimeType.GetMimeType(flpResponseUpload.FileBytes, flpResponseUpload.FileName);

                    if (cm.ValidMimeType(mimeTypeName, Path.GetExtension(flpResponseUpload.FileName)))
                    {
                        #region 
                        attachmentList = new List<BilletterieAPIWS.fileAttachmentObject>();
                        DataAccess da = new DataAccess();

                        BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                        if (Session["userObjectCookie"] != null)
                        {
                            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        }

                        string errMessage = ValidateFileBeforeUpload(usrSession.USR_PKID.ToString(), Path.GetExtension(flpResponseUpload.FileName));
                        if (errMessage == "")
                        {
                            string linksPDF = ValidatePDFFileLinks(flpResponseUpload.FileBytes, Path.GetExtension(flpResponseUpload.FileName));
                            if (linksPDF == "")
                            {
                                RecordAttachments fupObject = new RecordAttachments();
                                fupObject.FileExtension = Path.GetExtension(flpResponseUpload.FileName);
                                fupObject.FileType = "TCK";
                                fupObject.FileSize = flpResponseUpload.FileBytes.Length;
                                fupObject.FileMimeType = mimeTypeName;
                                fupObject.FileURL = ConfigurationManager.AppSettings["IPOnlineDocumentsGN"] + usrSession.USR_PKID.ToString() + flpResponseUpload.FileName;
                                fupObject.FileField = flpResponseUpload.FileBytes;
                                fupObject.FileDescription = "Ticket attachment";
                                fupObject.OriginalFileName = flpResponseUpload.FileName;
                                fupObject.DerivedFileName = usrSession.USR_PKID.ToString() + flpResponseUpload.FileName;

                                if (Session["ticketPDFUploadObject"] != null)
                                {
                                    ListTicketPDFAttachments = (List<RecordAttachments>)Session["ticketPDFUploadObject"];
                                }
                                else
                                {
                                    ListTicketPDFAttachments = new List<RecordAttachments>();
                                }
                                ListTicketPDFAttachments.Add(new RecordAttachments { FileDescription = fupObject.FileDescription, FileExtension = fupObject.FileExtension, FileType = fupObject.FileType, FileURL = fupObject.FileURL, FileField = fupObject.FileField, OriginalFileName = fupObject.OriginalFileName, DerivedFileName = fupObject.DerivedFileName, FileMimeType = fupObject.FileMimeType, FileSize = fupObject.FileSize });
                                if (ListTicketPDFAttachments.Count > 0)
                                {
                                    GridViewUploadedDocs.Visible = true;
                                    Session["ticketPDFUploadObject"] = ListTicketPDFAttachments;
                                    GridViewUploadedDocs.DataSource = ListTicketPDFAttachments;
                                    GridViewUploadedDocs.DataBind();
                                }
                            }
                            else
                            {
                                lblMainErrorMessage.Text = linksPDF;
                                lblMainErrorMessage.Visible = true;
                            }
                        }
                        else
                        {
                            lblMainErrorMessage.Text = errMessage;
                            lblMainErrorMessage.Visible = true;
                        }
                        #endregion
                    }
                    else
                    {
                        lblMainErrorMessage.Text = "Document type not allowed.";
                        lblMainErrorMessage.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMainErrorMessage.Text = ex.Message;
                lblMainErrorMessage.Visible = true;
            }
        }

        //private string ValidateFilesBeforeUpload(string userPKID)
        //{
        //    Common cm = new Common();
        //    DataAccess da = new Classes.DataAccess();
        //    string returnValue = "";
        //    try
        //    {
        //        if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
        //        {
        //            //DataSet ds = new DataSet();
        //            //ds = da.GetTemporaryDocuments(userPKID, "1");
        //            //int documentCount = 0;
        //            //int totalFileSize = 0;

        //            //if (ds != null)
        //            //{
        //            //    if (ds.Tables.Count > 0)
        //            //    {
        //            //        documentCount = ds.Tables[0].Rows.Count;
        //            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            //        {
        //            //            totalFileSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
        //            //        }
        //            //    }
        //            //}

        //            //if (documentCount > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadFiles"]))
        //            //{
        //            //    return returnValue = "Number of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadFiles"];
        //            //}

        //            //if (totalFileSize > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
        //            //{
        //            //    return returnValue = "Total size of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadSize"] + " MB";
        //            //}
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue = ex.Message;
        //    }
        //    return returnValue;
        //}

        private string ValidateFileBeforeUpload(string userPKID, string fileExtention)
        {
            Common cm = new Common();
            DataAccess da = new Classes.DataAccess();
            string returnValue = "";
            try
            {

                if (!cm.ValidMimeType(flpResponseUpload.PostedFile.ContentType, fileExtention))
                {
                    return returnValue = "File type '" + Path.GetExtension(flpResponseUpload.FileName) + "' is not supported.";
                }

                if (flpResponseUpload.FileBytes.Length > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
                {
                    return returnValue = "File '" + Path.GetFileName(flpResponseUpload.FileName) + "' is too large. Max file size is 8MB.";
                }

                if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                {

                }
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        private string ValidatePDFFileLinks(byte[] fileField, string fileExtention)
        {
            Common cm = new Common();
            DataAccess da = new Classes.DataAccess();
            string returnValue = "";
            try
            {
                string errorMessage = "";   
                errorMessage = cm.GetPDFHyperLinks(fileField, fileExtention);
                returnValue = errorMessage; 
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            ModalPopupExtenderTicketDetail.Show();
            LoadTicketDetails(lblTCKPKID.Text);
        }

        //protected void btnViewOldTickets_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("ViewOldTickets.aspx", false);
        //}

        private void LoadDropDowns()
        {
            DataAccess da = new DataAccess();
            DataSet dsDepartment = new DataSet();
            dsDepartment = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName] union select CAT_PKID, CAT_Order, CAT_CategoryName from TB_CAT_Category where CAT_MasterID = 0 and STS_PKID = 50 and CAT_Visible = 1 order by CAT_CategoryName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsDepartment != null)
            {
                ddlEditDepartment.DataSource = dsDepartment.Tables[0];
                ddlEditDepartment.DataTextField = "CAT_CategoryName";
                ddlEditDepartment.DataValueField = "CAT_PKID";
                ddlEditDepartment.DataBind();
            }
        }

        protected void ddlEditDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEditDepartment.SelectedValue != "" && ddlEditDepartment.SelectedValue != "")
            {
                if (PopulateCategoryEditDDL(ddlEditDepartment.SelectedValue))
                {
                    trCategory.Visible = true;
                }
                ModalPopupExtenderEdit.Show();
            }
            else
            {
                ddlEditCategory.DataSource = null;
                ddlEditCategory.DataBind();
                ddlEditCategory.Items.Clear();

                ddlEditSubCategory.DataSource = null;
                ddlEditSubCategory.DataBind();
                ddlEditSubCategory.Items.Clear();
            }
        }

        protected void ddlEditCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEditCategory.SelectedValue != "" && ddlEditCategory.SelectedValue != "")
            {
                if (PopulateSubCategoryEditDDL(ddlEditCategory.SelectedValue))
                {
                    trSubCategory.Visible = true;
                }
                ModalPopupExtenderEdit.Show();
            }
            else
            {
                ddlEditSubCategory.DataSource = null;
                ddlEditSubCategory.DataBind();
                ddlEditSubCategory.Items.Clear();
            }
        }

        private void PopulateDepartmentEditDDL()
        {
            DataAccess da = new DataAccess();
            DataSet dsDepartment = new DataSet();
            dsDepartment = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName] union select CAT_PKID, CAT_Order, CAT_CategoryName from TB_CAT_Category where CAT_MasterID = 0 and STS_PKID = 50 order by CAT_CategoryName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsDepartment != null)
            {
                ddlEditDepartment.DataSource = dsDepartment.Tables[0];
                ddlEditDepartment.DataTextField = "CAT_CategoryName";
                ddlEditDepartment.DataValueField = "CAT_PKID";
                ddlEditDepartment.DataBind();

                ddlEditCategory.DataSource = null;
                ddlEditCategory.DataBind();

                ddlEditSubCategory.DataSource = null;
                ddlEditSubCategory.DataBind();
            }
        }

        private bool PopulateCategoryEditDDL(string masterPKID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            DataSet dsCategory = new DataSet();
            dsCategory = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName] union select CAT_PKID, CAT_Order, CAT_CategoryName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 order by CAT_CategoryName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsCategory != null)
            {
                if (dsCategory.Tables[0].Rows.Count > 1)
                {
                    ddlEditCategory.DataSource = dsCategory.Tables[0];
                    ddlEditCategory.DataTextField = "CAT_CategoryName";
                    ddlEditCategory.DataValueField = "CAT_PKID";
                    ddlEditCategory.DataBind();
                    returnValue = true;
                }
                else
                {
                    returnValue = false;
                }
            }
            else
            {
                returnValue = false;
            }

            return returnValue;
        }

        private bool PopulateSubCategoryEditDDL(string masterPKID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            DataSet dsSubCategory = new DataSet();
            dsSubCategory = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName] union select CAT_PKID, CAT_Order, CAT_CategoryName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 order by CAT_CategoryName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsSubCategory != null)
            {
                if (dsSubCategory.Tables[0].Rows.Count > 1)
                {
                    ddlEditSubCategory.DataSource = dsSubCategory.Tables[0];
                    ddlEditSubCategory.DataTextField = "CAT_CategoryName";
                    ddlEditSubCategory.DataValueField = "CAT_PKID";
                    ddlEditSubCategory.DataBind();
                    returnValue = true;
                }
                else
                {
                    returnValue = false;
                }
            }
            else
            {
                returnValue = false;
            }

            return returnValue;
        }

        protected void GridViewUploadedDocs_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            HyperLink hp = new HyperLink();
            if (e.Row.RowIndex >= 0)
            {

                LinkButton myLinkButton = new LinkButton();
                myLinkButton = (LinkButton)e.Row.Cells[0].FindControl("lnkOFCPKIDLink");
                string fileLink = ConfigurationManager.AppSettings["RootPublicURL"] + "GetDocument.aspx?docID=" + myLinkButton.CommandArgument;
                myLinkButton.Visible = false;

                hp.Text = myLinkButton.Text;
                hp.NavigateUrl = fileLink;
                hp.Attributes.Add("OnClick", "window.open('" + ConfigurationManager.AppSettings["RootPublicURL"] + "GetDocument.aspx?docID=" + myLinkButton.CommandArgument + "','name','height=600, width=900,toolbar=no, directories=no,status=no, menubar=no,scrollbars=yes,resizable=no'); return false;");
                hp.Target = "_blank";
                e.Row.Cells[0].Controls.Add(hp);
            }
        }

        protected void btnOmbudsman_Click(object sender, EventArgs e)
        {

        }

    }
}