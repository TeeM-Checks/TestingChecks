using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mobile;
using System.Web.UI.MobileControls;
using NewBilletterie.Classes;
using System.Web.Security;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Globalization;
//using NewBilletterie.IFX_WS;
using System.Data;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class MobileIndex : System.Web.UI.Page
    {
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                               + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                               + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                               + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        bool invalid = false;

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        protected void Page_Load(object sender, EventArgs e)
        {
            string lockDownEnd = GetShutDownPeriod();

            if (lockDownEnd != null && lockDownEnd != "")
            {
                btnLogin.Enabled = false;
                btnPasswordResetLogin.Enabled = false;
                //txtPreferredEmail.Enabled = false;
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
            }
            else
            {
                btnLogin.Enabled = true;
                btnPasswordResetLogin.Enabled = true;
                //txtPreferredEmail.Enabled = true;
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
            }

            if (bool.Parse(ConfigurationManager.AppSettings["ShowPublicPasswordReset"].ToLower()))
            {
                btnPasswordResetLogin.Visible = true;
            }
            else
            {
                btnPasswordResetLogin.Visible = false;
            }

        }

        private string GetShutDownPeriod()
        {
            string retValue = "";
            DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject respObj = new BilletterieAPIWS.SelectStringResponseObject();
            respObj = bilAPIWS.GetBilletterieScalar("select SSD_DateTo from TB_SSD_SystemShutdown where SSD_DateFrom <= GETDATE() and SSD_DateTo >= GETDATE()", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //respObj = da.GetBilletterieGenericScalar("select SSD_DateTo from TB_SSD_SystemShutdown where SSD_DateFrom <= GETDATE() and SSD_DateTo >= GETDATE()");
            retValue = respObj.selectedPKID;
            return retValue;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();
                DataAccess dAc = new DataAccess();
                BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();
                if (cm.ValidateAntiXSS(txtUsername.Text.Trim()) && cm.ValidateAntiXSS(txtPassword.Text.Trim()))
                {
                    usrProfile = cm.GetUserProfileObject(txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));

                    #region Authenticate from Billetterie database
                    if (usrProfile.noError)
                    {
                        if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()))
                        {
                            if (usrProfile.STS_PKID == 10)
                            {
                                //if (!IsEmail(usrProfile.USR_EmailAccount) && !IsEmail(txtPreferredEmail.Text.Trim()))
                                if (!IsEmail(usrProfile.USR_EmailAccount))
                                {
                                    lblErrorMessage.Visible = true;
                                    lblErrorMessage.Text = "  [Your account profile does not have a valid email address. Please supply an alternative email address.]";
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
                                    lblErrorMessage.Visible = false;
                                    lblErrorMessage.Text = "";
                                    Session["GlobalSession"] = "LoggedIn";
                                    Session["userObjectCookie"] = usrProfile;

                                    //if (txtPreferredEmail.Text.Trim() != "")
                                    //{
                                    //    if (IsEmail(txtPreferredEmail.Text.Trim()))
                                    //    {
                                    //        Session["preferredEmail"] = txtPreferredEmail.Text.Trim();
                                    //    }
                                    //    else
                                    //    {
                                    //        lblErrorMessage.Text = "Invalid email account.";
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    Session["preferredEmail"] = null;
                                    //}

                                    Session["CustomerBankDetailDisplayed"] = false;

                                    //Display popup for banking details
                                    if (bool.Parse(ConfigurationManager.AppSettings["RequireBankDetails"]))
                                    {
                                        if (UserIsNotRestricted(usrProfile.USR_UserLogin.ToUpper().Trim()))
                                        {
                                            if (!UserAlreadySubmitted(usrProfile.USR_UserLogin.ToUpper().Trim()))
                                            {
                                                //IPCUBAERMSBillingWS ipERMS = new IPCUBAERMSBillingWS();
                                                //ipERMS.Url = ConfigurationManager.AppSettings["IFX_WSURL"];
                                                ERMSAgentProfileObject ermsAgentObj = new ERMSAgentProfileObject();
                                                ERMSAgentProfileResponse ermsAgentResp = new ERMSAgentProfileResponse();

                                                //Check if the customer code exist
                                                if (bilAPIWS.GetAgentAccount(usrProfile.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]))
                                                {
                                                    #region Bank Details
                                                    if (Session["CustomerBankDetailDisplayed"] != null && bool.Parse(Session["CustomerBankDetailDisplayed"].ToString()) == false)
                                                    {
                                                        //Check if user is coming from Password Reset session
                                                        if (usrProfile.USC_PKID == 1 || usrProfile.USC_PKID == 3)
                                                        {
                                                            DataAccess da = new DataAccess();
                                                            #region Delete banking temporary documents
                                                            DataSet dsA = new DataSet();
                                                            dsA = bilAPIWS.GetTemporaryCBRSDocuments(usrProfile.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                            //dsA = da.GetTemporaryCBRSDocuments(usrProfile.USR_PKID.ToString(), "1");
                                                            if (dsA != null)
                                                            {
                                                                if (dsA.Tables.Count > 0)
                                                                {
                                                                    for (int i = 0; i < dsA.Tables[0].Rows.Count; i++)
                                                                    {
                                                                        //Delete all documents
                                                                        if ((System.IO.File.Exists(dsA.Tables[0].Rows[i]["TPD_FileURL"].ToString())))
                                                                        {
                                                                            try
                                                                            {
                                                                                System.IO.File.Delete(dsA.Tables[0].Rows[i]["TPD_FileURL"].ToString());
                                                                            }
                                                                            catch (Exception) { }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            BilletterieAPIWS.UpdateResponseObject apCustResp = new BilletterieAPIWS.UpdateResponseObject();
                                                            apCustResp = bilAPIWS.DeleteAllCBRSTempDocuments(usrProfile.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                            //apCustResp = da.DeleteAllCBRSTempDocuments(usrProfile.USR_PKID.ToString(), "1");

                                                            #endregion

                                                            ermsAgentResp = bilAPIWS.GetERMSAgentProfile(usrProfile.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                            DoubleObjectResponse balObj = new DoubleObjectResponse();
                                                            balObj = bilAPIWS.GetAgentBalance(usrProfile.USR_UserLogin.ToUpper().Trim(), 0, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                                                            if (double.Parse(ermsAgentResp.ermsAgentProfileObject.sAgentBalance.Replace(',', '.'), CultureInfo.InvariantCulture) >= double.Parse(ConfigurationManager.AppSettings["MinRefundAmount"].ToString(), CultureInfo.InvariantCulture))
                                                            {
                                                                Response.Redirect("~/Mobile/MobileRefund.aspx", false);
                                                            }
                                                            else
                                                            {
                                                                Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                                }
                                            }
                                            else
                                            {
                                                Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                            }
                                        }
                                        else
                                        {
                                            Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                        }
                                    }
                                    else
                                    {
                                        Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
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

                                lblErrorMessage.Visible = true;
                                lblErrorMessage.Text = "  [This account has been disabled by the system administrator. Contact " + ConfigurationManager.AppSettings["AdminContact"] + " for help.]";
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
                                    userLogObj = PopulateUserLogObject(0, 2, 3, 2, "", GetIPAddress(), "", "Invalid credentials.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));
                                    bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            #endregion
                            lblErrorMessage.Visible = true;
                            lblErrorMessage.Text = "Invalid credentials. Please try again.";
                        }
                    }
                    else
                    {
                        #region Authenicate from other DB
                        if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["AutoSynchUsers"]))
                        {
                            //Get users from ERMS
                            if (bool.Parse(ConfigurationManager.AppSettings["GetERMSUsers"]))
                            {
                                usrProfile = cm.GetERMSUserProfileObject(txtUsername.Text.Trim().ToUpper(), txtPassword.Text.Trim());
                                if (usrProfile.noError)
                                {
                                    #region Authenticate from ERMS record
                                    if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()))
                                    {
                                        if (!IsEmail(usrProfile.USR_EmailAccount))
                                        {
                                            lblErrorMessage.Visible = true;
                                            lblErrorMessage.Text = "Your account profile does not have a valid email address. Please supply the preferred email address in the first textbox.";
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
                                            lblErrorMessage.Visible = false;
                                            lblErrorMessage.Text = "";
                                            Session["GlobalSession"] = "LoggedIn";
                                            Session["userObjectCookie"] = usrProfile;
                                            Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                        }
                                    }
                                    #endregion
                                }
                            }

                            //Get users from CUBA
                            if (bool.Parse(ConfigurationManager.AppSettings["GetCUBAUsers"]))
                            {
                                usrProfile = cm.GetCUBAUserProfileObject(txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));
                                if (usrProfile.noError)
                                {
                                    #region Authenticate from CUBA record
                                    if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()))
                                    {
                                        if (!IsEmail(usrProfile.USR_EmailAccount))
                                        {
                                            lblErrorMessage.Visible = true;
                                            lblErrorMessage.Text = "Your account profile does not have a valid email address. Please supply the preferred email address in the first textbox.";
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
                                            lblErrorMessage.Visible = false;
                                            lblErrorMessage.Text = "";
                                            Session["GlobalSession"] = "LoggedIn";
                                            Session["userObjectCookie"] = usrProfile;
                                            Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                        }
                                    }
                                    #endregion
                                }
                            }

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
                                            lblErrorMessage.Visible = true;
                                            lblErrorMessage.Text = "Your account profile does not have a valid email address. Please supply the preferred email address in the first textbox.";
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
                                            lblErrorMessage.Visible = false;
                                            lblErrorMessage.Text = "";
                                            Session["GlobalSession"] = "LoggedIn";
                                            Session["userObjectCookie"] = usrProfile;
                                            Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                                        }
                                    }
                                    #endregion
                                }
                            }

                            lblErrorMessage.Visible = true;
                            lblErrorMessage.Text = "Invalid credentials. Please try again.";
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
                                    userLogObj = PopulateUserLogObject(0, 2, 3, 2, "", GetIPAddress(), "", "Invalid credentials.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));
                                    bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            #endregion
                            lblErrorMessage.Visible = true;
                            lblErrorMessage.Text = "Invalid credentials. Please try again.";
                        }
                        #endregion
                        lblErrorMessage.Visible = true;
                        lblErrorMessage.Text = usrProfile.errorMessage;

                    }

                    #endregion

                }
                else
                {
                    lblErrorMessage.Visible = true;
                    lblErrorMessage.Text = "Illegal characters detected.";
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Visible = true;
                lblErrorMessage.Text = ex.Message;
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

        private string GetLastDepositDate(string agentCode)
        {
            string retValue = "";
            InformixDataSetResponse inforMixDS = new InformixDataSetResponse();
            //IPCUBAERMSBillingWS ipERMS = new IPCUBAERMSBillingWS();
            //ipERMS.Url = ConfigurationManager.AppSettings["IFX_WSURL"];
            inforMixDS = bilAPIWS.GetInformixDataSet("select trans_date, effective_date from agent_trans where agent_code = '" + agentCode + "' and trans_type_id = 1 order by trans_id ", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (inforMixDS != null)
            {
                if (inforMixDS.ermsDataSetObject != null)
                {
                    if (inforMixDS.ermsDataSetObject.Tables.Count > 0)
                    {
                        if (inforMixDS.ermsDataSetObject.Tables[0].Rows.Count > 0)
                        {
                            retValue = DateTime.Parse(inforMixDS.ermsDataSetObject.Tables[0].Rows[0]["effective_date"].ToString()).ToString("yyyy-MM-dd");
                        }
                    }
                }
            }
            return retValue;
        }

        private bool UserIsNotRestricted(string userName)
        {
            bool returnValue = false;
            if (ConfigurationManager.AppSettings["TestingUsers"].ToString() != "")
            {
                string[] stringArray = null;
                stringArray = ConfigurationManager.AppSettings["TestingUsers"].ToString().Split(';');
                for (int i = 0; i < stringArray.Length; i++)
                {
                    if (stringArray[i] != "")
                    {
                        if (userName == stringArray[i])
                        {
                            returnValue = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                returnValue = true;
            }
            return returnValue;
        }

        private bool UserAlreadySubmitted(string userName)
        {
            DataAccess da = new DataAccess();
            bool returnValue = false;

            //if (da.BankingDetailsExist(userName))
            if (bilAPIWS.BankingDetailsExist(userName, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
            {
                returnValue = true;
            }
            return returnValue;
        }

        private ERMSCustomerObject GetERMSDetailObject(string AgentAccount)
        {
            ERMSCustomerObject retValue = new Classes.ERMSCustomerObject();
            return retValue;
        } 

        public bool IsEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid email format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        protected void btnPasswordResetLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PasswordResetLogin.aspx", false);
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {

        }

    }
}