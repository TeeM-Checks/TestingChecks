using NewBilletterie.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewBilletterie
{
    public partial class ViewOldTickets : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Session["userObjectCookie"] != null)
                    {
                        userProfileObject usrSession = new userProfileObject();
                        usrSession = (userProfileObject)Session["userObjectCookie"];

                        PopulateOldTicketGrid(usrSession.USR_UserLogin, true);
                    }
                    else
                    {
                        Response.Redirect("~/Index.aspx", false);
                    }
                }
                catch (Exception ex)
                {
                    lblMainErrorMessage.Text = " Error accessing old tickets. Please try again later.";
                }
            }
        }

        //private void PopulateOldTicketGrid(string userLogin, bool viewOpenOnly)
        //{
        //    string countMessage = "";
        //    DataAccess da = new DataAccess();
        //    DataSet ds = new DataSet();
        //    DataTable dt = new DataTable();
        //    if (viewOpenOnly)
        //    {
        //        dt = da.GetGenericMySQLDataTable("select T.id TCK_PKID, T.id TCK_TicketNumber, T.name TCK_Subject, T.date TCK_DateCreated, T.status STS_StatusName, I.completename CAT_CategoryName from glpi_tickets T inner join glpi_itilcategories I on T.itilcategories_id = I.id inner join glpi_tickets_users TU on T.id = TU.tickets_id inner join glpi_users U on TU.users_id = U.id where U.name = '" + userLogin + "' and T.status in (1,2) order by T.id");
        //        countMessage = " open ticket(s)";
        //    }
        //    else
        //    {
        //        dt = da.GetGenericMySQLDataTable("select T.id TCK_PKID, T.id TCK_TicketNumber, T.name TCK_Subject, T.date TCK_DateCreated, T.status STS_StatusName, I.completename CAT_CategoryName from glpi_tickets T inner join glpi_itilcategories I on T.itilcategories_id = I.id inner join glpi_tickets_users TU on T.id = TU.tickets_id inner join glpi_users U on TU.users_id = U.id where U.name = '" + userLogin + "' order by T.id");
        //        countMessage = " total tickets";
        //    }

        //    if (dt != null)
        //    {
        //        Session["ViewResultsOldTickets"] = dt;    // ds.Tables[0];
        //        gridTickets.DataSource = null;
        //        gridTickets.DataSource = dt;
        //        gridTickets.DataBind();
        //        gridTickets.PageIndex = 0;
        //        lblNoOfTickets.Text = dt.Rows.Count.ToString() + countMessage;
        //    }
        //}

        protected void lnkTicketLink_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = (LinkButton)sender;
                ModalPopupExtenderTicketDetail.Show();
                lblTicketDetailHeading.Text = "Ticket [" + lnk.Text + "]";
                LoadOldTicketDetails(lnk.CommandArgument);
            }
            catch (Exception)
            {

            }

        }

        private void LoadOldTicketDetails(string ticketPKID)
        {
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, TCK_Reference from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID);
                dt = da.GetGenericMySQLDataTable("select distinct T.id TCK_PKID, T.id TCK_TicketNumber, T.name TCK_Subject, T.date TCK_DateCreated, T.status STS_StatusName, I.completename CAT_CategoryName, T.content TCK_Message, T.status STS_PKID, TU.users_id USR_PKID from glpi_tickets T inner join glpi_itilcategories I on T.itilcategories_id = I.id inner join glpi_tickets_users TU on T.id = TU.tickets_id inner join glpi_users U on TU.users_id = U.id where T.id = " + ticketPKID);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        txtSubject.Text = dt.Rows[0]["TCK_Subject"].ToString();
                        //txtReference.Text = dt.Rows[0]["TCK_Reference"].ToString();
                        txtMessage.Text = dt.Rows[0]["TCK_Message"].ToString();
                        txtDateCreated.Text = dt.Rows[0]["TCK_DateCreated"].ToString();
                        txtStatus.Text = dt.Rows[0]["STS_StatusName"].ToString();
                        txtCategory.Text = dt.Rows[0]["CAT_CategoryName"].ToString();
                        lblTCKPKID.Text = dt.Rows[0]["TCK_PKID"].ToString();
                        lblSTSPKID.Text = dt.Rows[0]["STS_PKID"].ToString();
                        lblUserPKID.Text = dt.Rows[0]["USR_PKID"].ToString();
                        txtResponseFeedback.Text = "";
                        if (dt.Rows[0]["STS_PKID"].ToString() == "1") //Submitted
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = true;
                            txtStatus.Text = "New";
                            //btnAcceptSolution.Visible = false;
                            //btnRejectSolution.Visible = false;
                            //btnSendInformation.Visible = true;
                            //flpResponseUpload.Visible = true;
                        }
                        else if (dt.Rows[0]["STS_PKID"].ToString() == "2") //Under Treatment
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = true;
                            txtStatus.Text = "Processing";
                            //btnAcceptSolution.Visible = false;
                            //btnRejectSolution.Visible = false;
                            //btnSendInformation.Visible = true;
                            //flpResponseUpload.Visible = true;
                        }
                        //else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "3") //Re-Submitted
                        //{
                        //    txtResponseFeedback.Visible = true;
                        //    btnDeleteTicket.Visible = true;
                        //    //btnAcceptSolution.Visible = false;
                        //    //btnRejectSolution.Visible = false;
                        //    //btnSendInformation.Visible = true;
                        //    //flpResponseUpload.Visible = true;
                        //}
                        //else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "4") //Error Submitting
                        //{
                        //    txtResponseFeedback.Visible = true;
                        //    btnDeleteTicket.Visible = true;
                        //    //btnAcceptSolution.Visible = false;
                        //    //btnRejectSolution.Visible = false;
                        //    //btnSendInformation.Visible = true;
                        //    //flpResponseUpload.Visible = true;
                        //}
                        else if (dt.Rows[0]["STS_PKID"].ToString() == "5") //Resolved
                        {
                            txtResponseFeedback.Visible = true;
                            btnDeleteTicket.Visible = false;
                            txtStatus.Text = "Solved";
                            //btnAcceptSolution.Visible = true;
                            //btnRejectSolution.Visible = true;
                            //btnSendInformation.Visible = true;
                            //flpResponseUpload.Visible = true;
                        }
                        else if (dt.Rows[0]["STS_PKID"].ToString() == "6") //Closed
                        {
                            txtResponseFeedback.Visible = false;
                            btnDeleteTicket.Visible = false;
                            txtStatus.Text = "Closed";
                            //btnAcceptSolution.Visible = false;
                            //btnRejectSolution.Visible = false;
                            //btnSendInformation.Visible = false;
                            //flpResponseUpload.Visible = false;
                        }
                        else
                        {
                            txtResponseFeedback.Visible = false;
                            btnDeleteTicket.Visible = false;
                            txtStatus.Text = "No Status";
                            //btnAcceptSolution.Visible = false;
                            //btnRejectSolution.Visible = false;
                            //btnSendInformation.Visible = false;
                            //flpResponseUpload.Visible = false;
                        }
                        //else if (ds.Tables[0].Rows[0]["STS_PKID"].ToString() == "8") //Deleted By Office
                        //{
                        //    txtResponseFeedback.Visible = false;
                        //    btnDeleteTicket.Visible = false;
                        //    //btnAcceptSolution.Visible = false;
                        //    //btnRejectSolution.Visible = false;
                        //    //btnSendInformation.Visible = false;
                        //    //flpResponseUpload.Visible = false;
                        //}

                    }
                }

                //DataSet dsResp = new DataSet();
                DataTable dtResp = new DataTable();
                //dsResp = da.GetGenericBilletterieDataSet("TB_TKR_TicketResponse", "TB_TKR_TicketResponseDS", "select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, R.STS_PKID, S.STS_StatusName, D.DCM_OriginalName, D.DCM_DerivedName, DCM_PKID, DCM_UniqueID from TB_TKR_TicketResponse R inner join TB_STS_Status S on R.STS_PKID  = S.STS_PKID left outer join TB_DCM_Document D on (R.TKR_PKID = D.TNK_PKID and D.DCL_PKID = 2) where TCK_PKID = " + ticketPKID + " and TKR_VisibleToClient = 1 order by TKR_PKID desc");
                dtResp = da.GetGenericMySQLDataTable("select F.date TKR_ResponseDate, F.users_id OFC_PKID, F.content TKR_ResponseMessage, F.id TKR_PKID from glpi_ticketfollowups F inner join glpi_tickets T on F.tickets_id = T.id where F.content != '' and T.id = " + ticketPKID + " order by TKR_PKID desc");
                if (dtResp != null)
                {
                    if (dtResp.Rows.Count > 0)
                    {
                        Session["ViewResponseResultsOld"] = dtResp;
                        GridViewResponses.DataSource = null;
                        GridViewResponses.DataSource = dtResp;
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


                DataTable dtDocResp = new DataTable();
                dtDocResp = da.GetGenericMySQLDataTable("select id,filename, filepath, users_id, tickets_id, date_mod, sha1sum from glpi_documents where tickets_id = " + ticketPKID + " order by id desc");
                if (dtDocResp != null)
                {
                    if (dtDocResp.Rows.Count > 0)
                    {
                        Session["ViewDocumentsResultsOld"] = dtDocResp;
                        GridViewOldDocuments.DataSource = null;
                        GridViewOldDocuments.DataSource = dtDocResp;
                        GridViewOldDocuments.DataBind();
                        GridViewOldDocuments.PageIndex = 0;

                        lblNoneDocument.Visible = false;
                        GridViewOldDocuments.Visible = true;
                    }
                    else
                    {
                        lblNoneDocument.Visible = true;
                        GridViewOldDocuments.Visible = false;
                    }
                }
                else
                {
                    lblNoneDocument.Visible = true;
                    GridViewOldDocuments.Visible = false;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void gridTickets_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                gridTickets.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewResultsOldTickets"];
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
                //e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#ff6a00'");
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#FFF4DF'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");

                if (e.Row.Cells[4].Text != "")
                {
                    //ImageButton imgEdt = (ImageButton)e.Row.FindControl("imgEdit");
                    ImageButton imgDel = (ImageButton)e.Row.FindControl("imgDelete");

                    //1 - New
                    //2 - Processing (assigned)
                    //5 - Solved
                    //6 - Closed


                    if (e.Row.Cells[4].Text.Trim() == "1")
                    {
                        e.Row.Cells[4].Text = "New";
                        //imgEdt.Enabled = true;
                        imgDel.Enabled = true;
                    }
                    else if (e.Row.Cells[4].Text.Trim() == "2")
                    {
                        e.Row.Cells[4].Text = "Processing";
                        //imgEdt.Enabled = true;
                        imgDel.Enabled = true;
                    }
                    else if (e.Row.Cells[4].Text.Trim() == "5")
                    {
                        e.Row.Cells[4].Text = "Solved";
                        //imgEdt.Enabled = true;
                        imgDel.Enabled = false;
                    }
                    else if (e.Row.Cells[4].Text.Trim() == "6")
                    {
                        e.Row.Cells[4].Text = "Closed";
                        //imgEdt.Enabled = true;
                        imgDel.Enabled = false;
                    }
                    else
                    {
                        e.Row.Cells[4].Text = "No Status";
                        //imgEdt.Enabled = false;
                        imgDel.Enabled = false;
                    }
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
                if (lnk.CommandArgument != lblUserPKID.Text.Trim())
                {
                    img.ImageUrl = "~/Images/incoming.png";
                }
                else
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

            }
        }

        protected void btnDeleteTicket_Click(object sender, EventArgs e)
        {
            //try
            //{

            //    if (txtResponseFeedback.Text.Trim() != "")
            //    {
            //        DataAccess da = new DataAccess();
            //        Common cm = new Common();
            //        //Billetterie.Classes.InsertResponseObject insResp = new Billetterie.Classes.InsertResponseObject();

            //        //Save document
            //        //ticketResponseObject tickRespObj = new ticketResponseObject();
            //        //tickRespObj = PopulateTicketResponse(7, Int32.Parse(lblTCKPKID.Text), txtResponseFeedback.Text.Trim());
            //        //insResp = da.InsertBilletterieTicketResponseRecord(tickRespObj);

            //        //if (insResp.noError)
            //        //{
            //        //Update ticket with new status

            //        ticketResponseObject repObj = new ticketResponseObject();
            //        repObj = PopulateTicketResponse();
            //        da.InsertMySQLTicketResponseRecord(repObj);

            //        da.UpdateGenericMySQLRecord("update glpi_tickets set status = 6 where id = " + lblTCKPKID.Text.Trim());
            //        lblPopErrorMessage.Text = "";
            //        lblPopErrorMessage.Visible = false;
            //        ModalPopupExtenderTicketDetail.Hide();

            //        //EmailDispatcherService emsWS = new EmailDispatcherService(); ;
            //        //EmailMessageObject emlObj = new EmailMessageObject();
            //        //emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
            //        //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);

            //        lblConfirmHeading.Text = "Ticket deletion confirmation.";
            //        lblConfirmationMessage.Text = "Thank you for deleting your ticket.";
            //        ModalPopupExtenderConfirm.Show();

            //        userProfileObject usrSession = new userProfileObject();
            //        usrSession = (userProfileObject)Session["userObjectCookie"];
            //        PopulateOldTicketGrid(usrSession.USR_UserLogin, chkViewOpen.Checked);
            //        //}
            //        //else
            //        //{
            //        //    lblPopErrorMessage.Text = insResp.errorMessage;
            //        //    lblPopErrorMessage.Visible = true;
            //        //    ModalPopupExtenderTicketDetail.Show();
            //        //}

            //    }
            //    else
            //    {
            //        lblPopErrorMessage.Text = "You need to provide your comments or feedback in order to continue.";
            //        lblPopErrorMessage.Visible = true;
            //        ModalPopupExtenderTicketDetail.Show();
            //    }
            //}
            //catch (Exception)
            //{

            //}
        }

        protected void btnDeleteRow_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (txtDeleteEditResponse.Text.Trim() != "")
            //    {
            //        DataAccess da = new DataAccess();
            //        Common cm = new Common();
            //        //Update ticket with new status
            //        ticketResponseObject repObj = new ticketResponseObject();
            //        repObj = PopulateTicketResponse();
            //        da.InsertMySQLTicketResponseRecord(repObj);
            //        da.UpdateGenericMySQLRecord("update glpi_tickets set status = 6 where id = " + lblTCKPKID.Text.Trim());
            //        lblGridRowError.Text = "";
            //        lblGridRowError.Visible = false;
            //        ModalPopupExtenderDelete.Hide();

            //        //EmailDispatcherService emsWS = new EmailDispatcherService(); ;
            //        //EmailMessageObject emlObj = new EmailMessageObject();
            //        //emlObj = PopulateEmailObject(lblTCKPKID.Text, insResp.insertedPKID);
            //        //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);

            //        userProfileObject usrSession = new userProfileObject();
            //        usrSession = (userProfileObject)Session["userObjectCookie"];
            //        PopulateOldTicketGrid(usrSession.USR_UserLogin, chkViewOpen.Checked);

            //    }
            //    else
            //    {
            //        lblGridRowError.Text = "You need to provide your comments or feedback in order to continue.";
            //        lblGridRowError.Visible = true;
            //        ModalPopupExtenderDelete.Show();
            //    }

            //    //if (txtDeleteEditResponse.Text.Trim() != "")
            //    //{
            //    //    DataAccess da = new DataAccess();
            //    //    Common cm = new Common();
            //    //    da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TCK_Message = " + txtDeleteEditResponse.Text + " where TCK_PKID = " + lblDeleteTCKPKID.Text);
            //    //}
            //    //else
            //    //{
            //    //    lblGridRowError.Text = "You need to provide your comments or feedback in order to continue.";
            //    //    lblGridRowError.Visible = true;
            //    //    ModalPopupExtenderDelete.Show();
            //    //}
            //}
            //catch (Exception)
            //{

            //}
        }

        private ticketResponseObject PopulateTicketResponse()
        {
            userProfileObject usrSession = new userProfileObject();
            usrSession = (userProfileObject)Session["userObjectCookie"];

            ticketResponseObject returnValue = new ticketResponseObject();
            returnValue.TCK_PKID = Int32.Parse(lblTCKPKID.Text);
            returnValue.OFC_PKID = Int32.Parse(lblUserPKID.Text.Trim()); // usrSession.USR_PKID;
            returnValue.UST_PKID = 1;
            returnValue.TKR_ResponseMessage = txtResponseFeedback.Text.Trim();
            returnValue.TKR_ResponseDate = DateTime.Now.ToString();
            returnValue.TKR_VisibleToClient = false;
            returnValue.STS_PKID = 1;

            return returnValue;
        }

        protected void chkViewOpen_CheckedChanged(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();

            if (Session["userObjectCookie"] != null)
            {
                userProfileObject usrSession = new userProfileObject();
                usrSession = (userProfileObject)Session["userObjectCookie"];
                if (chkViewOpen.Checked)
                {
                    PopulateOldTicketGrid(usrSession.USR_UserLogin, true);
                }
                else
                {
                    PopulateOldTicketGrid(usrSession.USR_UserLogin, false);
                }
            }
        }

        protected void GridViewResponses_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                GridViewResponses.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewResponseResultsOld"];
                GridViewResponses.DataSource = dt;
                GridViewResponses.DataBind();
                ModalPopupExtenderTicketDetail.Show();
            }
            catch (Exception)
            {

            }
        }

        protected void gridTickets_RowBoundOperations(object sender, GridViewCommandEventArgs e)
        {
            DataAccess da = new DataAccess();
            try
            {
                txtDeleteEditResponse.Text = "";
                //if (e.CommandName == "EditTicket")
                //{
                //    btnSaveEdit.Visible = true;
                //    btnDeleteRow.Visible = false;
                //    string ticketNumber = "";
                //    SelectResponseObject selResp = new SelectResponseObject();
                //    selResp = da.GetBilletterieGenericScalar("select TCK_TicketNumber from TB_TCK_Ticket where TCK_PKID = " + e.CommandArgument.ToString());
                //    ticketNumber = selResp.selectedPKID;
                //    lblEditDeleteHeading.Text = "Edit Ticket [" + ticketNumber + "]";
                //    lblEditDeleteTCKPKID.Text = e.CommandArgument.ToString();
                //    ModalPopupExtenderDelete.Show();
                //}
                if (e.CommandName == "DeleteTicket")
                {
                    //btnSaveEdit.Visible = false;
                    btnDeleteRow.Visible = true;

                    string ticketNumber = "";
                    //SelectResponseObject selResp = new SelectResponseObject();
                    //selResp = da.GetBilletterieGenericScalar("select TCK_TicketNumber from TB_TCK_Ticket where TCK_PKID = " + e.CommandArgument.ToString());
                    ticketNumber = e.CommandArgument.ToString();
                    lblEditDeleteHeading.Text = "Delete Ticket [" + ticketNumber + "]";
                    lblEditDeleteTCKPKID.Text = e.CommandArgument.ToString();
                    ModalPopupExtenderDelete.Show();
                }

            }
            catch (Exception)
            {

            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            ModalPopupExtenderTicketDetail.Show();
            LoadOldTicketDetails(lblTCKPKID.Text);
        }

        protected void GridViewOldDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.Cells[0] != null && e.Row.RowIndex >= 0)
            {
                LinkButton lnk = (LinkButton)e.Row.FindControl("lnkOldDocumentLink");
                //lnk.ToolTip = lnk.Text;
                //if (lnk.Text.Length > 50)
                //{
                //    lnk.Text = lnk.Text.Substring(0, 50) + " ...";
                //    lnk.Text = lnk.Text.Replace(System.Environment.NewLine, " ");
                //}
                //else
                //{
                //    lnk.Text = lnk.Text;
                //    lnk.Text = lnk.Text.Replace(System.Environment.NewLine, " ");
                //}

                //Image img = (Image)e.Row.FindControl("imgDirectionIcon");
                //if (lnk.CommandArgument != lblUserPKID.Text.Trim())
                //{
                //    img.ImageUrl = "~/Images/incoming.png";
                //}
                //else if (lnk.CommandArgument == "2")
                //{
                //    img.ImageUrl = "~/Images/outgoing.png";
                //}

                HyperLink hp = new HyperLink();
                LinkButton lnkDoc = (LinkButton)e.Row.FindControl("lnkOldDocumentLink");
                string Url = "~/GetOldQRSDocument.aspx?docID=" + lnkDoc.CommandArgument;
                hp.Attributes.Add("onclick", "javascript: OpenInNewTab('" + Url + "')");
                hp.NavigateUrl = Url;
                hp.Text = lnkDoc.Text;
                lnkDoc.Controls.Add(hp);

            }
        }

        protected void GridViewOldDocuments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                GridViewOldDocuments.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewResultsOldTickets"];
                GridViewOldDocuments.DataSource = dt;
                GridViewOldDocuments.DataBind();
            }
            catch (Exception)
            {

            }
        }


    }
}