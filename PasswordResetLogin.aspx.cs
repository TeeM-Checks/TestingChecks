using NewBilletterie.Classes;
//using NewBilletterie.EmailWS;
//using NewBilletterie.IFX_WS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class PasswordResetLogin : System.Web.UI.Page
    {

        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        protected void Page_Load(object sender, EventArgs e)
        {

            sidebar_container.Visible = false;


            string userOption = Request.QueryString["option"];
            if (!IsPostBack)
            {
                LoadDropDowns();
                string prevURL = "";
                string prevOption = "";
                if (Request.UrlReferrer != null)
                {
                    prevURL = Request.UrlReferrer.ToString();
                    prevOption = Request.UrlReferrer.Query;
                }

                if (Session["userObjectCookie"] != null)
                {
                    (Page.Master.FindControl("btnLogout") as Button).Visible = true;
                    Response.Redirect("~/ExternalPages/NewTicket.aspx", false);
                }
                if (Session["officerObjectCookie"] != null)
                {
                    (Page.Master.FindControl("btnLogout") as Button).Visible = true;
                    Response.Redirect("~/InternalPages/OfficeTicketView.aspx", false);
                }

                //Clear sessions
                Session["ViewResults"] = null;
                Session["ViewTime"] = null;

            }
        }

        private NewBilletterie.BilletterieAPIWS.UserLogObject PopulateUserLogObject(int _usrPKID, int _ustPKID, int _ussPKID, int _ldsPKID, string _uslSessionID, string _ipAddress, string _deviceName, string _errorDescription, string _uslUserIdentifier, string _uslEncryptedCode)
        {
            NewBilletterie.BilletterieAPIWS.UserLogObject returnValue = new NewBilletterie.BilletterieAPIWS.UserLogObject();
            returnValue.usrPKID = _usrPKID;
            returnValue.ustPKID = _ustPKID;
            returnValue.ussPKID = _ussPKID;
            returnValue.ldsPKID = _ldsPKID;
            returnValue.uslSessionID = _uslSessionID;
            returnValue.ipAddress = GetIPAddress();
            returnValue.deviceName = _deviceName;
            returnValue.errorDescription = _errorDescription;
            returnValue.uslDateTime = DateTime.Now;
            returnValue.uslUserIdentifier = _uslUserIdentifier;
            returnValue.uslEncryptedCode = _uslEncryptedCode;
            return returnValue;
        }

        private string GetIPAddress()
        {
            string ipaddress = "";
            try
            {
                ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (ipaddress == "" || ipaddress == null)
                    ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception)
            {

            }
            return ipaddress;
        }

        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

        private BilletterieAPIWS.UpdateResponseObject ChangeUserPassword(string ofc_username, string ofc_password)
        {
            BilletterieAPIWS.UpdateResponseObject returnValue;
            Common cm = new Common();
            DataAccess da = new DataAccess();
            BilletterieAPIWS.UpdateResponseObject updObj = new BilletterieAPIWS.UpdateResponseObject();
            updObj = bilAPIWS.UpdateBilletterieRecord("update TB_OFC_Officer set OFC_PassKey = '" + cm.GetMD5Hash(ofc_password) + "' where OFC_UserLogin = '" + ofc_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //updObj = da.UpdateGenericBilletterieRecord("update TB_OFC_Officer set OFC_PassKey = '" + cm.GetMD5Hash(ofc_password) + "' where OFC_UserLogin = '" + ofc_username + "'");
            returnValue = updObj;
            return returnValue;
        }

        private BilletterieAPIWS.InsertResponseObject RequestNewPassword(string ofc_username)
        {
            BilletterieAPIWS.InsertResponseObject returnValue = new BilletterieAPIWS.InsertResponseObject();
            Common cm = new Common();
            DataAccess da = new DataAccess();
            BilletterieAPIWS.UpdateResponseObject updObj = new BilletterieAPIWS.UpdateResponseObject();
            updObj = bilAPIWS.UpdateBilletterieRecord("update TB_OFC_Officer set OFC_ResetRequested = 1 where OFC_UserLogin = '" + ofc_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //updObj = da.UpdateGenericBilletterieRecord("update TB_OFC_Officer set OFC_ResetRequested = 1 where OFC_UserLogin = '" + ofc_username + "'");
            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select top 1 OFC_PKID from TB_OFC_Officer where OFC_UserLogin = '" + ofc_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //selResp = da.GetBilletterieGenericScalar("select top 1 OFC_PKID from TB_OFC_Officer where OFC_UserLogin = '" + ofc_username + "'");
            BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();

            if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
            {
                if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                {
                    EmailMessageObject emlObj = new EmailMessageObject();
                    emlObj = PopulateEmailObject(selResp.selectedPKID);
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
                    EmailMessageObject emlObj = new EmailMessageObject();
                    emlObj = PopulateEmailObject(selResp.selectedPKID);
                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                }
            }
            returnValue = opResp;

            return returnValue;
        }

        private bool CurrentPasswordCorrect(string ofc_username, string ofc_password)
        {
            Common cm = new Common();
            DataAccess da = new DataAccess();
            bool returnValue = false;
            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_OFC_Officer where OFC_PassKey = '" + cm.GetMD5Hash(ofc_password) + "' and OFC_UserLogin = '" + ofc_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //selResp = da.GetBilletterieGenericIntScalar("select count(*) from TB_OFC_Officer where OFC_PassKey = '" + cm.GetMD5Hash(ofc_password) + "' and OFC_UserLogin = '" + ofc_username + "'");
            if (Int32.Parse(selResp.selectedPKID) == 1)
            {
                returnValue = true;
            }
            return returnValue;
        }

        private bool UserAccountExist(string ofc_username)
        {
            DataAccess da = new DataAccess();
            bool returnValue = false;
            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_OFC_Officer where OFC_UserLogin = '" + ofc_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //selResp = da.GetBilletterieGenericIntScalar("select count(*) from TB_OFC_Officer where OFC_UserLogin = '" + ofc_username + "'");
            if (Int32.Parse(selResp.selectedPKID) == 1)
            {
                returnValue = true;
            }
            return returnValue;
        }

        private BilletterieAPIWS.EmailMessageObject PopulateEmailObject(string ofcPKID)
        {
            string uniqueGUID = "";
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            BilletterieAPIWS.userProfileObject usrObj = new BilletterieAPIWS.userProfileObject();
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select OFC_PKID, OFC_EmailAccount, OFC_UserLogin, OFC_FirstName, OFC_Surname, OFC_UniqueID from TB_OFC_Officer where OFC_PKID = " + ofcPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_OFC_Officer", "TB_OFC_OfficerDS", "select OFC_PKID, OFC_EmailAccount, OFC_UserLogin, OFC_FirstName, OFC_Surname, OFC_UniqueID from TB_OFC_Officer where OFC_PKID = " + ofcPKID);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    returnValue.EML_ToEmailList = ds.Tables[0].Rows[0]["OFC_EmailAccount"].ToString();
                    returnValue.EML_ToEmailAdmin = ds.Tables[0].Rows[0]["OFC_EmailAccount"].ToString();     // ConfigurationManager.AppSettings["To"];
                    returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ": User password reset request";
                    usrObj.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFC_PKID"].ToString());
                    usrObj.USR_UserLogin = ds.Tables[0].Rows[0]["OFC_UserLogin"].ToString();
                    usrObj.USR_FirstName = ds.Tables[0].Rows[0]["OFC_FirstName"].ToString();
                    usrObj.USR_LastName = ds.Tables[0].Rows[0]["OFC_Surname"].ToString();
                    uniqueGUID = ds.Tables[0].Rows[0]["OFC_UniqueID"].ToString();
                }
            }
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_MailBody = GetPasswordResetEmailBody(usrObj, uniqueGUID);
            returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = "1";
            returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
            returnValue.EML_KeyField = "OFC_PKID";
            returnValue.EML_KeyValue = ofcPKID;
            returnValue.EML_Domain = "0";
            returnValue.EML_Priority = "0";
            returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];
            return returnValue;
        }

        private string GetPasswordResetEmailBody(BilletterieAPIWS.userProfileObject usrObj, string uniqueGUID)
        {
            string returnValue = "";
            string activeLink = "";
            if (Request.Url != null)
            {
                if (Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]))
                {
                    activeLink = ConfigurationManager.AppSettings["LocalUserActivationURL"] + uniqueGUID;
                }
                else
                {
                    activeLink = ConfigurationManager.AppSettings["ExternalUserActivationURL"] + uniqueGUID;
                }
            }

            string actualDefaultPassword = "<br /><br /> Your password will be reset to a randomly created string upon clicking the link.";
            string responseMessage = "You have requested that your user password be reset to a new password. Please note that in order to complete this process you need to click the link below. If the link is not active you need to copy and paste it onto your browser.<br /><br />" + activeLink + actualDefaultPassword;

            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>User password change request for user [ " + usrObj.USR_UserLogin + " ].</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : Reset of password for user account: " + usrObj.USR_UserLogin + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Date : </b>" + DateTime.Now.ToString() + "<br/><p>Dear " + usrObj.USR_FirstName + " " + usrObj.USR_LastName + ",<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>" + responseMessage + "<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Note that any attached documents are ONLY accessible through the help desk system. Please feel free to ask if you may have any other query. <br /><br />Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
            return returnValue;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (chkForgotPassword.Checked)
            {
                if (txtPopUserID.Text.Trim() != "")
                {
                    //Action to send link to Officer
                    if (UserAccountExist(txtPopUserID.Text.Trim()))
                    {
                        BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                        opResp = RequestNewPassword(txtPopUserID.Text.Trim());
                        if (opResp.noError)
                        {
                            ModalPopupExtenderChangePassword.Hide();
                            txtUsername.Text = txtPopUserID.Text.Trim();
                            lblConfirmHeading.Text = "User password change request.";
                            lblConfirmationMessage.Text = "You have successfully requested to change you user password. A link will be sent to you email account shortly.";
                            ModalPopupExtenderConfirm.Show();
                        }
                        else
                        {
                            lblGridRowError.Text = opResp.errorMessage;
                            lblGridRowError.Visible = true;
                            ModalPopupExtenderChangePassword.Show();
                        }
                    }
                    else
                    {
                        lblGridRowError.Text = "Username not found.";
                        lblGridRowError.Visible = true;
                        ModalPopupExtenderChangePassword.Show();
                    }
                }
                else
                {
                    lblGridRowError.Text = "User Login ID is required.";
                    lblGridRowError.Visible = true;
                    ModalPopupExtenderChangePassword.Show();
                }
            }
            else
            {
                if (txtPopUserID.Text.Trim() != "" && txtPopOldPassword.Text.Trim() != "" && txtPopNewPassword.Text.Trim() != "" && txtPopConfirmPassword.Text.Trim() != "")
                {
                    if (txtPopNewPassword.Text.Trim() == txtPopConfirmPassword.Text.Trim())
                    {
                        if (txtPopNewPassword.Text.Trim().Length > 5)
                        {
                            //Action to change user password
                            if (CurrentPasswordCorrect(txtPopUserID.Text.Trim(), txtPopOldPassword.Text.Trim()))
                            {
                                ChangeUserPassword(txtPopUserID.Text.Trim(), txtPopNewPassword.Text.Trim());
                                ModalPopupExtenderChangePassword.Hide();
                                txtUsername.Text = txtPopUserID.Text.Trim();
                                lblConfirmHeading.Text = "User password successfully changed.";
                                lblConfirmationMessage.Text = "You have successfully changed your password. You can use your new password immediately to login.";
                                ModalPopupExtenderConfirm.Show();

                            }
                            else
                            {
                                lblGridRowError.Text = "You have entered wrong current password. If you have forgotten your password select the checkbox to reset the password.";
                                lblGridRowError.Visible = true;
                                ModalPopupExtenderChangePassword.Show();
                            }
                        }
                        else
                        {
                            lblGridRowError.Text = "Your new password must be at least 6 characters long.";
                            lblGridRowError.Visible = true;
                            ModalPopupExtenderChangePassword.Show();
                        }
                    }
                    else
                    {
                        lblGridRowError.Text = "Your new passwords do not match.";
                        lblGridRowError.Visible = true;
                        ModalPopupExtenderChangePassword.Show();
                    }
                }
                else
                {
                    lblGridRowError.Text = "You are required to supply all the information.";
                    lblGridRowError.Visible = true;
                    ModalPopupExtenderChangePassword.Show();
                }
            }
        }

        protected void lnkChangePassword_Click(object sender, EventArgs e)
        {
            txtPopUserID.Text = txtUsername.Text.Trim();
            txtPopNewPassword.Text = "";
            txtPopConfirmPassword.Text = "";
            lblGridRowError.Text = "";
            lblGridRowError.Visible = false;
            ModalPopupExtenderChangePassword.Show();
            lblChangePasswordHeading.Text = "Change your password";
        }

        protected void chkForgotPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkForgotPassword.Checked)
            {
                txtPopOldPassword.Enabled = false;
                txtPopNewPassword.Enabled = false;
                txtPopConfirmPassword.Enabled = false;

                lblPopOldPasswordBlank.Visible = true;
                lblPopNewPasswordBlank.Visible = true;
                lblPopConfirmPasswordBlank.Visible = true;
            }
            else
            {
                txtPopOldPassword.Enabled = true;
                txtPopNewPassword.Enabled = true;
                txtPopConfirmPassword.Enabled = true;
                lblPopOldPasswordBlank.Visible = false;
                lblPopNewPasswordBlank.Visible = false;
                lblPopConfirmPasswordBlank.Visible = false;
            }
            ModalPopupExtenderChangePassword.Show();
        }

        protected void lnkAccountCreationLink_Click(object sender, EventArgs e)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UseInternalAccountLink"]) == true)
            {
                Session["GlobalSession"] = "New Account";
                Response.Redirect("~/CreateNewAccount.aspx", false);
            }

        }

        protected void chkPreviousCodes_CheckedChanged(object sender, EventArgs e)
        {
            txtPreviousCC.Visible = chkPreviousCodes.Checked;
            btnFindCC.Visible = chkPreviousCodes.Checked;
        }

        protected void btnFindCC_Click(object sender, EventArgs e)
        {
            try
            {
                lblGridRowError.Visible = false;
                lblGridRowError.Text = "";
                GridViewPreviousCodes.DataSource = null;
                GridViewPreviousCodes.DataBind();

                if (txtIDNumber.Text.Trim() != "" && txtPreviousCC.Text.Trim() != "")
                {
                    DataAccess da = new DataAccess();

                    //Search for users in ERMS database 
                    DataSet ermsDS = new DataSet();
                    if (bool.Parse(ConfigurationManager.AppSettings["GetERMSUsers"]))
                    {
                        #region ERMS Search
                        try
                        {
                            //IPCUBAERMSBillingWS ifx = new IPCUBAERMSBillingWS();
                            //ifx.Url = ConfigurationManager.AppSettings["IFX_WSURL"];

                            InformixDataSetResponse dsResp = new InformixDataSetResponse();

                            dsResp = bilAPIWS.GetInformixDataSet("select agent_code, agent_name, email, status, cell_no, current_login from agents where agent_id_no = '" + txtIDNumber.Text.Trim() + "' and agent_code = '" + txtPreviousCC.Text.Trim() + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            if (dsResp.noError)
                            {
                                ermsDS = dsResp.ermsDataSetObject;
                            }
                        }
                        catch (Exception)
                        {
                            //Ignore exceptions
                        }
                        #endregion
                    }

                    if (ermsDS != null)
                    {
                        if (ermsDS.Tables.Count > 0)
                        {
                            Session["ViewSearchedUsers"] = ermsDS.Tables[0];
                            GridViewPreviousCodes.DataSource = null;
                            GridViewPreviousCodes.DataSource = ermsDS.Tables[0];
                            GridViewPreviousCodes.DataBind();
                        }
                    }
                    else
                    {
                        GridViewPreviousCodes.DataSource = null;
                        GridViewPreviousCodes.DataBind();
                    }
                }
                else
                {
                    GridViewPreviousCodes.DataSource = null;
                    GridViewPreviousCodes.DataBind();
                    lblGridRowError.Visible = true;
                    lblGridRowError.Text = "Agent code cannot be null.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected void GridViewPreviousCodes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                GridViewPreviousCodes.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewResponsePreviousCodes"];
                GridViewPreviousCodes.DataSource = dt;
                GridViewPreviousCodes.DataBind();
            }
            catch (Exception)
            {

            }
        }

        protected void GridViewPreviousCodes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Attributes.Add("style", "cursor:pointer;font-size:12px;font-weight:600;");
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#6BAB4D'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");
                if (e.Row.Cells[0] != null && e.Row.RowIndex >= 0)
                {
                    //N	New
                    //A	Active
                    //D	Dormant
                    //Q	Query
                    //B	BLANK ID MADE DORMANT
                    //C	DUPLICATE ID ZERO BALANCE MADE DORMANT
                    //E	Invalid ID Zero Balance

                    if (e.Row.Cells[3].Text.ToLower() == "a")
                    {
                        e.Row.Cells[3].Text = "Active";
                    }
                    else if (e.Row.Cells[3].Text.ToLower() == "b")
                    {
                        e.Row.Cells[3].Text = "BLANK ID MADE DORMANT";
                    }
                    else if (e.Row.Cells[3].Text.ToLower() == "c")
                    {
                        e.Row.Cells[3].Text = "DUPLICATE ID ZERO BALANCE MADE DORMANT";
                    }
                    else if (e.Row.Cells[3].Text.ToLower() == "d")
                    {
                        e.Row.Cells[3].Text = "Dormant";
                    }
                    else if (e.Row.Cells[3].Text.ToLower() == "e")
                    {
                        e.Row.Cells[3].Text = "Invalid ID Zero Balance";
                    }
                    else if (e.Row.Cells[3].Text.ToLower() == "n")
                    {
                        e.Row.Cells[3].Text = "New";
                    }
                    else if (e.Row.Cells[3].Text.ToLower() == "q")
                    {
                        e.Row.Cells[3].Text = "Query";
                    }
                }
            }
        }

        protected void GridViewPreviousCodes_RowBoundOperations(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DeleteAgentCode")
                {

                }
            }
            catch (Exception)
            {
            }
        }

        private void LoadDropDowns()
        {
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["officerObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["officerObjectCookie"];
            }

            DataAccess da = new DataAccess();
            DataSet dsProvince = new DataSet();
            dsProvince = bilAPIWS.GetBilletterieDataSet("select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province  from TB_PRV_Province", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsProvince = da.GetGenericBilletterieDataSet("TB_PRV_Province", "TB_PRV_ProvinceDS", "select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province  from TB_PRV_Province");
            if (dsProvince != null)
            {
                ddlProvince.DataSource = dsProvince.Tables[0];
                ddlProvince.DataTextField = "PRV_Province";
                ddlProvince.DataValueField = "PRV_PKID";
                ddlProvince.DataBind();
            }
        }

        protected void lnkAgentCodeLink_Click(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Index.aspx", true);
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUsername.Text.Trim() != "" && txtIDNumber.Text.Trim() != "" && txtNames.Text.Trim() != "")
                {
                    Session["ViewResults"] = null;
                    Session["ViewTime"] = null;
                    Session["ViewResultsExport"] = null;
                    Common cm = new Common();
                    DataAccess dAc = new DataAccess();
                    BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();
                    usrProfile = cm.GetERMSUserProfileObjectNOPWD(txtUsername.Text.Trim(), txtIDNumber.Text.Trim());

                    #region Authenticate from Billetterie database

                    if (usrProfile.noError)
                    {

                        txtIDNumber.Enabled = false;
                        txtUsername.Enabled = false;

                        trBlankRow1.Visible = true;
                        trVerifyButtons.Visible = false;
                        trEmailAccount.Visible = true;
                        trPhoneNumber.Visible = true;
                        trBlankRow2.Visible = true;
                        trProvince.Visible = true;
                        trBlankRow3.Visible = true;
                        trPhysicalAddress.Visible = true;
                        trBlankRow4.Visible = true;
                        trPostalAddress.Visible = true;
                        trBlankRow5.Visible = true;
                        trPreviousCodes.Visible = true;
                        trPreviousCodesGrid.Visible = true;
                        trBlankRow6.Visible = true;

                        trDisclaimer.Visible = true;
                        trBlankRow7.Visible = true;

                        lblMainErrorMessage.Visible = false;
                        lblMainErrorMessage.Text = "";

                    }
                    else
                    {
                        txtIDNumber.Enabled = true;
                        txtUsername.Enabled = true;

                        trBlankRow1.Visible = false;
                        trVerifyButtons.Visible = true;
                        trEmailAccount.Visible = false;
                        trPhoneNumber.Visible = false;
                        trBlankRow2.Visible = false;
                        trProvince.Visible = false;
                        trBlankRow3.Visible = false;
                        trPhysicalAddress.Visible = false;
                        trBlankRow4.Visible = false;
                        trPostalAddress.Visible = false;
                        trBlankRow5.Visible = false;
                        trPreviousCodes.Visible = false;
                        trPreviousCodesGrid.Visible = false;
                        trBlankRow6.Visible = false;
                        trSubmitButtons.Visible = false;

                        lblMainErrorMessage.Visible = true;
                        lblMainErrorMessage.Text = "User details not found.  Email  the password reset request form, available on the CIPC website under Access/Password reset request form, as well as certified ID copy (Certification not older than three months) to resetpassword@cipc.co.za";
                    }

                    #endregion

                }
                else
                {
                    lblMainErrorMessage.Visible = true;
                    lblMainErrorMessage.Text = "  [Customer code and ID/Passport Number must be provided.]";
                }
            }
            catch (Exception ex)
            {
                lblMainErrorMessage.Visible = true;
                lblMainErrorMessage.Text = ex.Message;
            }

            #region comments
           
            #endregion
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            #region
            try
            {
                if (txtUsername.Text.Trim() != "" && txtIDNumber.Text.Trim() != "" && txtMobileNo.Text.Trim() != "" && txtEmailAccount.Text.Trim() != "")
                {
                    Session["ViewResults"] = null;
                    Session["ViewTime"] = null;
                    Session["ViewResultsExport"] = null;
                    Common cm = new Common();
                    DataAccess dAc = new DataAccess();

                    ticketInformationObject tckInfoObject = new ticketInformationObject();
                    tckInfoObject = PopulateTicketInformation();

                    BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();
                    usrProfile = cm.GetERMSUserProfileObjectNOPWD(txtUsername.Text.Trim(), txtIDNumber.Text.Trim(), tckInfoObject);

                    #region Authenticate from Billetterie database

                    if (usrProfile.noError)
                    {
                       
                        #region Record user log details
                        try
                        {
                            if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                            {
                                //NewBilletterie.CIPCUserLoginWS.LogUserActivityWS cipcLogsWS = new NewBilletterie.CIPCUserLoginWS.LogUserActivityWS();
                                //cipcLogsWS.Url = ConfigurationManager.AppSettings["CIPCUserLoginWSURL"];

                                NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                userLogObj = PopulateUserLogObject(usrProfile.USR_PKID, 1, 3, 1, "", GetIPAddress(), "", "", usrProfile.USR_UserLogin, usrProfile.USR_PassKey);
                                bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                        }
                        catch (Exception)
                        {

                        }

                        #endregion

                        FormsAuthentication.RedirectFromLoginPage(txtUsername.Text, false);
                        lblMainErrorMessage.Visible = false;
                        lblMainErrorMessage.Text = "";
                        Session["GlobalSession"] = "LoggedIn";
                        Session["userObjectCookie"] = usrProfile;

                        Session["PassworsResetCookie"] = tckInfoObject;

                        if (txtEmailAccount.Text.Trim() != "")
                        {
                            Session["preferredEmail"] = txtEmailAccount.Text.Trim();
                        }
                        else
                        {
                            Session["preferredEmail"] = null;
                        }

                        Response.Redirect("~/ExternalPages/NewPasswordTicket.aspx?Subject=" + txtUsername.Text.Trim() + "&Reference=" + txtNames.Text.Trim(), false);

                    }
                    else
                    {
                        lblMainErrorMessage.Visible = true;
                        lblMainErrorMessage.Text = "User details not found.  Email  the password reset request form, available on the CIPC website under Access/Password reset request form, as well as certified ID copy (Certification not older than three months) to resetpassword@cipc.co.za";
                    }

                    #region Alternative logins
                   
                    #endregion

                    #endregion

                }
                else
                {
                    lblMainErrorMessage.Visible = true;
                    lblMainErrorMessage.Text = "  [Email account Or Mobile phone number must be provided.]";
                }
            }
            catch (Exception ex)
            {
                lblMainErrorMessage.Visible = true;
                lblMainErrorMessage.Text = ex.Message;
            }
            #endregion
        }

        private ticketInformationObject PopulateTicketInformation()
        {
            ticketInformationObject returnValue = new ticketInformationObject();

            returnValue.IdentityNumber = txtIDNumber.Text.Trim();
            returnValue.CustomerCode = txtUsername.Text.Trim();

            returnValue.Names = txtNames.Text.Trim();
            returnValue.EmailAccount = txtEmailAccount.Text.Trim();
            returnValue.PhoneNumber = txtMobileNo.Text.Trim();
            returnValue.Province = ddlProvince.SelectedValue.ToString();


            returnValue.PhyStreetAddress1 = txtStreetName1.Text.Trim();
            returnValue.PhyStreetAddress2 = txtStreetName2.Text.Trim();
            returnValue.PhyCityTown = txtCityTown.Text.Trim();
            returnValue.PhyStateProvince = txtStateProvince.Text.Trim();
            returnValue.PhyAreaCode = txtPostalCode.Text.Trim();

            returnValue.PosStreetAddress1 = txtPostStreetName1.Text.Trim();
            returnValue.PosStreetAddress2 = txtPostStreetName2.Text.Trim();
            returnValue.PosCityTown = txtPostCityTown.Text.Trim();
            returnValue.PosStateProvince = txtPostStateProvince.Text.Trim();
            returnValue.PosAreaCode = txtPostPostalCode.Text.Trim();

            return returnValue;
        }

        protected void btnCancelVerify_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Index.aspx", true);
        }

        protected void btnAgree_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateUserInput();
            if (errorMessage == "")
            {
                trSubmitButtons.Visible = true;
                trDisclaimer.Visible = false;
                trBlankRow7.Visible = false;


                txtNames.Enabled = false;
                txtEmailAccount.Enabled = false;
                txtMobileNo.Enabled = false;
                ddlProvince.Enabled = false;

                txtStreetName1.Enabled = false;
                txtStreetName2.Enabled = false;
                txtCityTown.Enabled = false;
                txtStateProvince.Enabled = false;
                txtPostalCode.Enabled = false;

                txtPostStreetName1.Enabled = false;
                txtPostStreetName2.Enabled = false;
                txtPostCityTown.Enabled = false;
                txtPostStateProvince.Enabled = false;
                txtPostPostalCode.Enabled = false;

                lblMainErrorMessage.Visible = false;
                lblMainErrorMessage.Text = "";

            }
            else
            {
                lblMainErrorMessage.Visible = true;
                lblMainErrorMessage.Text = "  [" + errorMessage + "]";
            }
        }

        protected void btnDisagree_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Index.aspx", true);
        }

        private string ValidateUserInput()
        {
            string returnValue = "";

            if (txtNames.Text.Trim() == "")
            {
                returnValue = "Please enter name(s)";
            }
            if (txtEmailAccount.Text.Trim() == "")
            {
                returnValue = "Please enter email account";
            }
            if (txtMobileNo.Text.Trim() == "")
            {
                returnValue = "Please enter mobile phone number";
            }
            if (ddlProvince.SelectedValue == "0")
            {
                returnValue = "Please select province";
            }

            if (txtStreetName1.Text.Trim() == "")
            {
                returnValue = "Please enter street address";
            }
            if (txtCityTown.Text.Trim() == "")
            {
                returnValue = "Please enter city/town";
            }
            if (txtStateProvince.Text.Trim() == "")
            {
                returnValue = "Please enter province";
            }
            if (txtPostalCode.Text.Trim() == "")
            {
                returnValue = "Please enter area code";
            }

            if (txtPostStreetName1.Text.Trim() == "")
            {
                returnValue = "Please enter postal address";
            }
            if (txtPostCityTown.Text.Trim() == "")
            {
                returnValue = "Please enter postal city/town";
            }
            if (txtPostStateProvince.Text.Trim() == "")
            {
                returnValue = "Please enter postal province";
            }
            if (txtPostPostalCode.Text.Trim() == "")
            {
                returnValue = "Please enter postal code";
            }


            return returnValue;
        }

    }
}