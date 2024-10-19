using NewBilletterie.Classes;
//using NewBilletterie.EmailWS;
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
using System.Text;

namespace NewBilletterie
{
    public partial class AdminLogin : System.Web.UI.Page
    {

        //BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

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

            string userOption = Request.QueryString["userToken"];

            if (!IsPostBack)
            {
                if (Session["officerObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject officerSession = new BilletterieAPIWS.userProfileObject();
                    officerSession = (BilletterieAPIWS.userProfileObject)Session["officerObjectCookie"];

                    if (officerSession.noError)
                    {
                        if (officerSession.STS_PKID == 20)
                        {

                            #region Record user log details

                            try
                            {
                                if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                                {
                                    //NewBilletterie.CIPCUserLoginWS.LogUserActivityWS cipcLogsWS = new NewBilletterie.CIPCUserLoginWS.LogUserActivityWS();
                                    //cipcLogsWS.Url = ConfigurationManager.AppSettings["CIPCUserLoginWSURL"];

                                    NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                    userLogObj = PopulateUserLogObject(officerSession.USR_PKID, 1, 3, 1, "", GetIPAddress(), "", "", officerSession.USR_UserLogin, officerSession.USR_PassKey);
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
                            Session["officerObjectCookie"] = officerSession;
                            Session["preferredEmail"] = null;

                            if (officerSession.OFL_PKID != 1)
                            {
                                if (officerSession.OFC_IsApprover == 1 || officerSession.OFC_IsApprover == 2 || officerSession.OFC_IsApprover == 3 || officerSession.OFC_IsApprover == 4)
                                {
                                    Response.Redirect("~/InternalPages/ProcessRefunds.aspx", false);
                                }
                                else
                                {
                                    Response.Redirect("~/InternalPages/OfficeTicketView.aspx", false);
                                }
                            }
                            else
                            {
                                if (bool.Parse(ConfigurationManager.AppSettings["DefaultToAdmin"]))
                                {
                                    Response.Redirect("~/InternalPages/Administration.aspx", false);
                                }
                                else
                                {
                                    Response.Redirect("~/InternalPages/OfficeTicketView.aspx", false);
                                }
                            }

                        }
                    }
                }

                //Redirection from AD
                else if (userOption != "")
                {
                    Session["ViewResults"] = null;
                    Session["ViewTime"] = null;
                    Session["ViewResultsExport"] = null;
                    Common cm = new Common();
                    //DataAccess dAc = new DataAccess();
                    BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();

                    usrProfile = cm.GetOfficerProfileObject(userOption);

                    #region Authenticate from Billetterie database
                    if (usrProfile.noError)
                    {
                        if (usrProfile.STS_PKID == 20)
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
                            Session["officerObjectCookie"] = usrProfile;
                            Session["preferredEmail"] = null;

                            if (usrProfile.OFL_PKID != 1)
                            {
                                if (usrProfile.OFC_IsApprover == 1 || usrProfile.OFC_IsApprover == 2 || usrProfile.OFC_IsApprover == 3 || usrProfile.OFC_IsApprover == 4)
                                {
                                    Response.Redirect("~/InternalPages/ProcessRefunds.aspx", false);
                                }
                                else
                                {
                                    Response.Redirect("~/InternalPages/OfficeTicketView.aspx", false);
                                }
                            }
                            else
                            {
                                if (bool.Parse(ConfigurationManager.AppSettings["DefaultToAdmin"]))
                                {
                                    Response.Redirect("~/InternalPages/Administration.aspx", false);
                                }
                                else
                                {
                                    if (usrProfile.OFC_IsApprover == 1 || usrProfile.OFC_IsApprover == 2 || usrProfile.OFC_IsApprover == 3)
                                    {
                                        Response.Redirect("~/InternalPages/ProcessRefunds.aspx", false);
                                    }
                                    else
                                    {
                                        Response.Redirect("~/InternalPages/OfficeTicketView.aspx", false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            #region Record user log details
                            try
                            {
                                if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                                {
                                    //NewBilletterie.CIPCUserLoginWS.LogUserActivityWS cipcLogsWS = new NewBilletterie.CIPCUserLoginWS.LogUserActivityWS();
                                    //cipcLogsWS.Url = ConfigurationManager.AppSettings["CIPCUserLoginWSURL"];
                                    NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                    userLogObj = PopulateUserLogObject(0, 1, 3, 2, "", GetIPAddress(), "", "This account has been disabled by the system administrator.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));
                                    bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            #endregion

                            lblMainErrorMessage.Visible = true;
                            lblMainErrorMessage.Text = "  [This account has been disabled by the system administrator. Contact " + ConfigurationManager.AppSettings["AdminContact"] + " for help.]";
                        }

                    }
                    #endregion

                }
                //Straight login
                else
                {
                    #region Straight Login
                    lnkAccountCreationLink.Text = "Do not have a " + ConfigurationManager.AppSettings["OrganisationName"] + " account?";
                    lnkAccountCreationExternalLink.Text = "Do not have a " + ConfigurationManager.AppSettings["OrganisationName"] + " account?";

                    lnkAccountCreationExternalLink.Visible = false;
                    lnkAccountCreationLink.Visible = false;
                    lnkChangePassword.Visible = true;

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
                    #endregion
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Session["ViewResults"] = null;
                Session["ViewTime"] = null;
                Session["ViewResultsExport"] = null;
                Common cm = new Common();
                //DataAccess dAc = new DataAccess();
                BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();

                if (ValidateAntiXSS(txtUsername.Text.Trim()) && ValidateAntiXSS(txtPassword.Text.Trim()))
                {
                    if (AllowDomainAccount(txtUsername.Text.Trim()))
                    {
                        usrProfile = cm.GetOfficerProfileObject(txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));

                        #region Authenticate from Billetterie database
                        if (usrProfile.noError)
                        {
                            if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()))
                            {
                                if (usrProfile.STS_PKID == 20)
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
                                    Session["officerObjectCookie"] = usrProfile;
                                    Session["preferredEmail"] = null;

                                    bilAPIWS.ResetBilletterieOfficerLoginCount(usrProfile.USR_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                                    if (usrProfile.OFL_PKID != 1)
                                    {
                                        if (usrProfile.OFC_IsApprover == 1 || usrProfile.OFC_IsApprover == 2 || usrProfile.OFC_IsApprover == 3 || usrProfile.OFC_IsApprover == 4)
                                        {
                                            Response.Redirect("~/InternalPages/ProcessRefunds.aspx", false);
                                        }
                                        else
                                        {
                                            Response.Redirect("~/InternalPages/OfficeTicketView.aspx", false);
                                        }
                                    }
                                    else
                                    {
                                        if (bool.Parse(ConfigurationManager.AppSettings["DefaultToAdmin"]))
                                        {
                                            Response.Redirect("~/InternalPages/Administration.aspx", false);
                                        }
                                        else
                                        {
                                            if (usrProfile.OFC_IsApprover == 1 || usrProfile.OFC_IsApprover == 2 || usrProfile.OFC_IsApprover == 3 || usrProfile.OFC_IsApprover == 4)
                                            {
                                                Response.Redirect("~/InternalPages/ProcessRefunds.aspx", false);
                                            }
                                            else
                                            {
                                                Response.Redirect("~/InternalPages/OfficeTicketView.aspx", false);
                                            }
                                        }
                                    }

                                }
                                else if (usrProfile.STS_PKID == 22)
                                {
                                    lblConfirmActionPKID.Text = usrProfile.USR_PKID.ToString();
                                    lblConfirmActionHeading.Text = "            Cancel your leave status";
                                    lblConfirmActionWarning.Text = "Your account status will be set to Active. Your name will be available for ticket assignment. Do you want to continue?";
                                    lblConfirmActionDefinition.Text = "Activate";
                                    //Show popu up for account activation.
                                    ModalPopupExtenderActivateLeave.Show();
                                }
                                else
                                {
                                    #region Record user log details
                                    try
                                    {
                                        if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                                        {
                                            //NewBilletterie.CIPCUserLoginWS.LogUserActivityWS cipcLogsWS = new NewBilletterie.CIPCUserLoginWS.LogUserActivityWS();
                                            //cipcLogsWS.Url = ConfigurationManager.AppSettings["CIPCUserLoginWSURL"];
                                            NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                            userLogObj = PopulateUserLogObject(0, 1, 3, 2, "", GetIPAddress(), "", "This account has been disabled by the system administrator.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));
                                            bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    #endregion

                                    lblMainErrorMessage.Visible = true;
                                    lblMainErrorMessage.Text = "  [This account has been disabled by the system administrator. Contact " + ConfigurationManager.AppSettings["AdminContact"] + " for help.]";
                                }
                            }
                            else
                            {
                                #region Record user log details
                                try
                                {
                                    if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                                    {
                                        //NewBilletterie.CIPCUserLoginWS.LogUserActivityWS cipcLogsWS = new NewBilletterie.CIPCUserLoginWS.LogUserActivityWS();
                                        //cipcLogsWS.Url = ConfigurationManager.AppSettings["CIPCUserLoginWSURL"];
                                        NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                        userLogObj = PopulateUserLogObject(0, 1, 3, 2, "", GetIPAddress(), "", "Invalid credentials.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));
                                        bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //Add log for 
                                        //if count of attempts is greater than 3 the set status to 22
                                        bilAPIWS.IncreaseBilletterieOfficerLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        int loginCount = Int32.Parse(bilAPIWS.GetBilletterieOfficerLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
                                        if (loginCount > Int32.Parse(ConfigurationManager.AppSettings["LoginAttempts"]))
                                        {
                                            bilAPIWS.UpdateBilletterieOfficerStatus(txtUsername.Text, "21", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }
                                #endregion
                                lblMainErrorMessage.Visible = true;
                                lblMainErrorMessage.Text = "  [Username/Password. Please try again.]";
                            }
                        }
                        else
                        {
                            //Get users from Ptolemy
                            if (bool.Parse(ConfigurationManager.AppSettings["GetPtolemyUsers"]))
                            {
                                usrProfile = cm.GetPtolemyUserProfileObject(txtUsername.Text.Trim().ToLower(), cm.GetMD5Hash(txtPassword.Text.Trim()));
                                if (usrProfile.noError)
                                {
                                    #region Authenticate from Ptolemy record
                                    if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()))
                                    {
                                        if (!IsEmail(usrProfile.USR_EmailAccount))
                                        {
                                            lblMainErrorMessage.Visible = true;
                                            lblMainErrorMessage.Text = "  [Your account profile does not have a valid email address. Please supply the preferred email address in the first textbox.]";
                                        }
                                        else
                                        {

                                            #region Record user log details
                                            try
                                            {
                                                if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                                                {
                                                    //NewBilletterie.CIPCUserLoginWS.LogUserActivityWS cipcLogsWS = new NewBilletterie.CIPCUserLoginWS.LogUserActivityWS();
                                                    //cipcLogsWS.Url = ConfigurationManager.AppSettings["CIPCUserLoginWSURL"];
                                                    NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                                    userLogObj = PopulateUserLogObject(usrProfile.USR_PKID, 2, 3, 1, "", GetIPAddress(), "", "", usrProfile.USR_UserLogin, usrProfile.USR_PassKey);
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
                                            Session["preferredEmail"] = null;
                                            Response.Redirect("~/ExternalPages/NewTicket.aspx", false);
                                        }
                                    }


                                    #endregion
                                }

                                else
                                {
                                    bilAPIWS.IncreaseBilletterieOfficerLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    int loginCount = Int32.Parse(bilAPIWS.GetBilletterieOfficerLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
                                    if (loginCount > 3)
                                    {
                                        bilAPIWS.UpdateBilletterieOfficerStatus(txtUsername.Text, "21", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }

                                    lblMainErrorMessage.Visible = true;
                                    lblMainErrorMessage.Text = " [Username/Password. Please try again.]";
                                }
                            }
                            else
                            {
                                //Failed 
                                //Add log for 
                                //if count of attempts is greater than 3 the set status to 22
                                bilAPIWS.IncreaseBilletterieOfficerLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                int loginCount = Int32.Parse(bilAPIWS.GetBilletterieOfficerLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
                                if (loginCount > Int32.Parse(ConfigurationManager.AppSettings["LoginAttempts"]))
                                {
                                    bilAPIWS.UpdateBilletterieOfficerStatus(txtUsername.Text, "21", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }

                                lblMainErrorMessage.Visible = true;
                                lblMainErrorMessage.Text = " [Username/Password. Please try again.]";
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        lblMainErrorMessage.Visible = true;
                        lblMainErrorMessage.Text = "  [This account is not allowed login outside the domain environment. Contact " + ConfigurationManager.AppSettings["AdminContact"] + " for help.]";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMainErrorMessage.Visible = true;
                lblMainErrorMessage.Text = ex.Message;
            }
        }

        private bool AllowDomainAccount(string userName)
        {
            bool retValue = false;
            if (userName.ToUpper().Contains(ConfigurationManager.AppSettings["AccountDomainPrefix"]))
            {
                if (bool.Parse(ConfigurationManager.AppSettings["AllowDomainAccount"]) == false)
                {
                    retValue = false;
                }
                else
                {
                    retValue = true;
                }
            }
            else
            {
                retValue = true;
            }
            
            return retValue;
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

        public static bool ValidateAntiXSS(string inputParameter)
        {
            if (string.IsNullOrEmpty(inputParameter))
                return true;

            var pattren = new StringBuilder();

            //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.             
            pattren.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");

            //Checks any html tags i.e. <script, <embed, <object etc.
            pattren.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");

            return !Regex.IsMatch(System.Web.HttpUtility.UrlDecode(inputParameter), pattren.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
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

        private EmailMessageObject PopulateEmailObject(string ofcPKID)
        {
            string uniqueGUID = "";
            EmailMessageObject returnValue = new EmailMessageObject();
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

        protected void btnConfirmActionYes_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblConfirmActionDefinition.Text == "Activate")
                {
                    if (lblConfirmActionPKID.Text != "")
                    {
                        DataAccess da = new DataAccess();
                        BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                        BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();

                        updResp = bilAPIWS.UpdateBilletterieOfficerRecordA(20, Int32.Parse(lblConfirmActionPKID.Text.Trim()), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //updResp = da.UpdateBilletterieOfficerRecord(20, Int32.Parse(lblConfirmActionPKID.Text.Trim()));

                        BilletterieAPIWS.userLogObject usrLogObject = new BilletterieAPIWS.userLogObject();
                        usrLogObject.USL_Description = "User account set to active";
                        usrLogObject.ULT_PKID = 3;
                        usrLogObject.OFC_PKID = Int32.Parse(lblConfirmActionPKID.Text); 

                        if (updResp.noError)
                        {
                            insResp = bilAPIWS.InsertBilletterieUserLogRecord(usrLogObject, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //insResp = da.InsertBilletterieUserLogRecord(usrLogObject);
                        }

                        btnLogin_Click(btnLogin, null);
                    }
                }
            }
            catch (Exception ex)
            {
                ModalPopupExtenderActivateLeave.Show();
            }
        }

        protected void btnConfirmActionNo_Click(object sender, EventArgs e)
        {

        }



    }
}