using NewBilletterie.Classes;
//using NewBilletterie.EmailWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class Index : Page
    {
        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();
        Common cm = new Common();

        string passwordValue = "";  // txtPassword.Text;

        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                                + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string lockDownEnd = GetShutDownPeriod();


                if (lockDownEnd != null && lockDownEnd != "")
                {
                    btnLogin.Enabled = false;
                    btnPasswordResetLogin.Enabled = false;
                    txtPreferredEmail.Enabled = false;
                    txtUsername.Enabled = false;
                    txtPassword.Enabled = false;
                    lblMainErrorMessage.Text = "System will be availabe for use on " + lockDownEnd;
                }
                else
                {
                    btnLogin.Enabled = true;
                    btnPasswordResetLogin.Enabled = true;
                    txtPreferredEmail.Enabled = true;
                    txtUsername.Enabled = true;
                    txtPassword.Enabled = true;
                    lblMainErrorMessage.Text = "";
                }

                //if (bool.Parse(ConfigurationManager.AppSettings["ExternalResetPassword"].ToLower()))
                //{
                //    lnkChangePassword.Visible = true;
                //}
                //else
                //{
                //    lnkChangePassword.Visible = false;
                //}

                if (bool.Parse(ConfigurationManager.AppSettings["ShowPublicPasswordReset"].ToLower()))
                {
                    btnPasswordResetLogin.Visible = true;
                }
                else
                {
                    btnPasswordResetLogin.Visible = false;
                }


                if (bool.Parse(ConfigurationManager.AppSettings["ShowAccountCreationLink"].ToLower()))
                {
                    if (bool.Parse(ConfigurationManager.AppSettings["UseInternalAccountLink"]) == true)
                    {
                        //lnkAccountCreationLink.Visible = true;
                        //lnkAccountCreationExternalLink.Visible = true;
                    }
                    else
                    {
                        //lnkAccountCreationLink.Visible = false;
                        //lnkAccountCreationExternalLink.Visible = false;
                    }
                }
                else
                {
                    //lnkAccountCreationLink.Visible = false;
                    //lnkAccountCreationExternalLink.Visible = false;
                }

                string userOption = Request.QueryString["option"];


                if (!IsPostBack)
                {

                    if (bool.Parse(ConfigurationManager.AppSettings["ForceHTTPS"].ToLower()))
                    {
                        if (!Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]))
                        {
                            if (!HttpContext.Current.Request.IsSecureConnection)
                            {
                                Response.Redirect(ConfigurationManager.AppSettings["RootPublicURL"] + "Index.aspx", true);
                            }
                        }
                    }

                    //if (Request.Browser.IsMobileDevice)
                    //{
                    //    Response.Redirect(ConfigurationManager.AppSettings["RootPublicURL"] + "MobileIndex.aspx", true);
                    //}

                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));

                    btnLogin.BackColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonBackColor"]);
                    btnLogin.ForeColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonForeColor"]);

                    btnPasswordResetLogin.BackColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonBackColor"]);
                    btnPasswordResetLogin.ForeColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonForeColor"]);

                    //lnkAccountCreationLink.Text = "Do not have a " + ConfigurationManager.AppSettings["OrganisationName"] + " account?";
                    //lnkAccountCreationExternalLink.Text = "Do not have a " + ConfigurationManager.AppSettings["OrganisationName"] + " account?";

                    if (bool.Parse(ConfigurationManager.AppSettings["UseInternalAccountLink"]) == false)
                    {
                        //lnkAccountCreationExternalLink.PostBackUrl = ConfigurationManager.AppSettings["AccountCreationURL"];
                        //lnkAccountCreationExternalLink.Visible = false;
                    }

                    //if (Request.Url != null)
                    //{
                    //    if (bool.Parse(ConfigurationManager.AppSettings["UseActiveDirectory"]))
                    //    {
                    //        if (Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]) && !Request.Url.ToString().ToLower().Contains("transID="))
                    //        {
                    //            Response.Redirect("https://" + ConfigurationManager.AppSettings["LocalIPAddressAD"] + "/AdminLogin.aspx", true);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]) && !Request.Url.ToString().ToLower().Contains("transID="))
                    //        {
                    //            Response.Redirect("https://" + ConfigurationManager.AppSettings["LocalIPAddress"] + "/AdminLogin.aspx", false);
                    //        }
                    //    }
                    //}

                    string prevURL = "";
                    string prevOption = "";
                    if (Request.UrlReferrer != null)
                    {
                        prevURL = Request.UrlReferrer.ToString();
                        prevOption = Request.UrlReferrer.Query;
                    }

                    if (userOption != null)
                    {
                        if (userOption == "logout")
                        {
                            Session["userObjectCookie"] = null;
                            Session["preferredEmail"] = null;
                            Response.Redirect("~/Index.aspx", false);
                        }

                        if (userOption == "officerlogout")
                        {
                            Session["officerObjectCookie"] = null;
                            Session["preferredEmail"] = null;
                            Response.Redirect("~/Index.aspx?option=officerLogin", false);
                            //lnkChangePassword.Visible = true;
                        }

                        if (userOption == "officerLogin")
                        {
                            txtPreferredEmail.Enabled = false;
                            //lnkChangePassword.Visible = true;
                        }

                    }
                    else if (prevOption.Contains("officerLogin"))
                    {
                        Session["officerObjectCookie"] = null;
                        Session["preferredEmail"] = null;
                        Response.Redirect("~/Index.aspx?option=officerLogin", false);
                        //lnkChangePassword.Visible = true;
                    }
                    else
                    {
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
                    }
                    //Clear sessions
                    Session["ViewResults"] = null;
                    Session["ViewTime"] = null;

                }
                lblMainErrorMessage.Text = "";
                lblMainErrorMessage.Visible = false;

            }
            catch (System.Data.SqlClient.SqlException)
            {
                lblMainErrorMessage.Text = "System error. Please try again soon.";
                lblMainErrorMessage.Visible = true;
            }
            catch (Exception ex)
            {
                lblMainErrorMessage.Text = "System error. Please try again soon.";
                lblMainErrorMessage.Visible = true;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Session["ViewResults"] = null;
                Session["ViewTime"] = null;
                Session["ViewResultsExport"] = null;
                DataAccess dAc = new DataAccess();
                BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();

                //External user login
                if (cm.ValidateAntiXSS(txtUsername.Text.Trim()) && cm.ValidateAntiXSSS(txtUsername.Text.Trim()) && cm.ValidateAntiXSS(txtPassword.Text.Trim()))
                {
                    usrProfile = cm.GetUserProfileObject(txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));

                    #region Authenticate from Billetterie database

                    if (usrProfile.noError)
                    {
                        passwordValue = txtPassword.Text;
                        if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()) && usrProfile.STS_PKID == 10)
                        {
                            if (!IsEmail(usrProfile.USR_EmailAccount) && !IsEmail(txtPreferredEmail.Text.Trim()))
                            {
                                lblMainErrorMessage.Visible = true;
                                lblMainErrorMessage.Text = "  [Your account profile does not have a valid email address. Please supply an alternative email address.]";
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
                                if (txtPreferredEmail.Text.Trim() != "")
                                {
                                    Session["preferredEmail"] = txtPreferredEmail.Text.Trim();
                                }
                                else
                                {
                                    Session["preferredEmail"] = null;
                                }
                                bilAPIWS.ResetBilletterieUserLoginCount(usrProfile.USR_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                Session["CustomerBankDetailDisplayed"] = false;
                                Response.Redirect("~/ExternalPages/NewTicket.aspx", false);
                            }
                        }
                        else if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()) && usrProfile.STS_PKID == 15)
                        {
                            lblOTPUSRPKID.Text = usrProfile.USR_PKID.ToString();
                            lblWarningOTPMessage.Text = "You are logging into this system for the first time. You are required to correctly complete this form using your CIPC account details.";
                            lblCustomerOTPHeading.Text = ""; // "You are encouraged to use this process to request refunds as it is quicker to verify and process the transaction ";
                            lblOTPERMSPKID.Text = usrProfile.USR_UserLogin;
                            lblBottomMessage.Text = "NOTE: A One Time Password will be sent to the email that you used when creating your CIPC account."; // ConfigurationManager.AppSettings["CBRSBottomMessage"];

                            lblOTPPopErrorMessage.Text = "";
                            lblOTPPopErrorMessage.Visible = false;

                            ModalPopupExtenderOTPPopup.Show();

                        }
                        else if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()) && usrProfile.STS_PKID == 14)
                        {
                            lblUSRPKID.Text = usrProfile.USR_PKID.ToString();
                            lblWarningMessage.Text = "You are logging into this system for the first time. You are required to correctly complete this form using your CIPC account details.";
                            lblCustomerDetailHeading.Text = ""; // "You are encouraged to use this process to request refunds as it is quicker to verify and process the transaction ";
                            lblERMSPKID.Text = usrProfile.USR_UserLogin;
                            lblBottomMessage.Text = "NOTE: You are required to input the OTP that you have received on your email."; // ConfigurationManager.AppSettings["CBRSBottomMessage"];
                            lblPasswordValue.Text = txtPassword.Text;
                            lblPopErrorMessage.Text = "";
                            lblPopErrorMessage.Visible = false;

                            ModalPopupExtenderAccountDetail.Show();
                        }
                        else if (usrProfile.USR_UserLogin.ToLower() == txtUsername.Text.Trim().ToLower() && usrProfile.USR_PassKey == cm.GetMD5Hash(txtPassword.Text.Trim()) && (usrProfile.STS_PKID == 11 || usrProfile.STS_PKID == 12 || usrProfile.STS_PKID == 13))
                        {
                            #region Record user log details
                            try
                            {
                                if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                                {
                                    //NewBilletterie.CIPCUserLoginWS.LogUserActivityWS cipcLogsWS = new NewBilletterie.CIPCUserLoginWS.LogUserActivityWS();
                                    //cipcLogsWS.Url = ConfigurationManager.AppSettings["CIPCUserLoginWSURL"];
                                    NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                    userLogObj = PopulateUserLogObject(0, 2, 3, 2, "", GetIPAddress(), "", "Account Suspended.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()).ToString());
                                    bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //Add log for 
                                    //if count of attempts is greater than 3 the set status to 22
                                    //bilAPIWS.IncreaseBilletterieUserLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //int loginCount = Int32.Parse(bilAPIWS.GetBilletterieLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
                                    //if (loginCount > 3)
                                    //{
                                    //    bilAPIWS.UpdateBilletterieUserStatus(txtUsername.Text, "11", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //}
                                }
                            }
                            catch (Exception)
                            {

                            }
                            #endregion

                            lblMainErrorMessage.Visible = true;
                            lblMainErrorMessage.Text = "  [This account has been disabled by the system administrator. Contact " + ConfigurationManager.AppSettings["AdminContact"] + " for help.]";
                        }
                        else
                        {
                            #region Record user log details
                            try
                            {
                                if (bool.Parse(ConfigurationManager.AppSettings["RecordUserLogs"]))
                                {
                                    NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                    userLogObj = PopulateUserLogObject(0, 2, 3, 2, "", GetIPAddress(), "", "Invalid credentials.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()).ToString());
                                    bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            #endregion

                            bilAPIWS.IncreaseBilletterieUserLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            int loginCount = Int32.Parse(bilAPIWS.GetBilletterieLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
                            if (loginCount > 3)
                            {
                                bilAPIWS.UpdateBilletterieUserStatus(txtUsername.Text, "11", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }

                            lblMainErrorMessage.Visible = true;
                            lblMainErrorMessage.Text = "  [Incorrect Username/Password. Please try again.]";
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
                                                    NewBilletterie.BilletterieAPIWS.UserLogObject userLogObj = new NewBilletterie.BilletterieAPIWS.UserLogObject();
                                                    userLogObj = PopulateUserLogObject(usrProfile.USR_PKID, 2, 3, 1, "", GetIPAddress(), "", "", usrProfile.USR_UserLogin, usrProfile.USR_PassKey);
                                                    bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                }
                                            }
                                            catch (Exception)
                                            {

                                            }
                                            #endregion
                                            lblPopErrorMessage.Text = "";
                                            lblPopErrorMessage.Visible = false;
                                            lblUSRPKID.Text = usrProfile.USR_PKID.ToString();
                                            lblWarningMessage.Text = "You are logging into this system for the first time. You are required to correctly complete this form using your CIPC account details.";
                                            lblCustomerDetailHeading.Text = ""; // "You are encouraged to use this process to request refunds as it is quicker to verify and process the transaction ";
                                            lblBottomMessage.Text = ""; // ConfigurationManager.AppSettings["CBRSBottomMessage"];
                                            lblPasswordValue.Text = txtPassword.Text;

                                            //Add pou up page for 
                                            ModalPopupExtenderAccountDetail.Show();
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    //Not fount in ERMS
                                    lblMainErrorMessage.Visible = true;
                                    lblMainErrorMessage.Text = "  [Incorrect Username/Password. Please try again.]";
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
                                        if (!IsEmail(usrProfile.USR_EmailAccount) && !IsEmail(txtPreferredEmail.Text.Trim()))
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
                                            if (txtPreferredEmail.Text.Trim() != "")
                                            {
                                                Session["preferredEmail"] = txtPreferredEmail.Text.Trim();
                                            }
                                            else
                                            {
                                                Session["preferredEmail"] = null;
                                            }
                                            Response.Redirect("~/ExternalPages/NewTicket.aspx", false);
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
                                        if (!IsEmail(usrProfile.USR_EmailAccount) && !IsEmail(txtPreferredEmail.Text.Trim()))
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
                                            if (txtPreferredEmail.Text.Trim() != "")
                                            {
                                                Session["preferredEmail"] = txtPreferredEmail.Text.Trim();
                                            }
                                            else
                                            {
                                                Session["preferredEmail"] = null;
                                            }
                                            Response.Redirect("~/ExternalPages/NewTicket.aspx", false);
                                        }
                                    }
                                    #endregion
                                }
                            }

                            //Add log for 
                            //if count of attempts is greater than 3 the set status to 22
                            bilAPIWS.IncreaseBilletterieUserLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            int loginCount = Int32.Parse(bilAPIWS.GetBilletterieLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
                            if (loginCount > Int32.Parse(ConfigurationManager.AppSettings["LoginAttempts"]))
                            {
                                bilAPIWS.UpdateBilletterieUserStatus(txtUsername.Text, "11", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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
                                    userLogObj = PopulateUserLogObject(0, 2, 3, 2, "", GetIPAddress(), "", "Invalid credentials.", txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()).ToString());
                                    bilAPIWS.AddUserLogRecord(userLogObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //Add log for 
                                    //if count of attempts is greater than 3 the set status to 22
                                }
                            }
                            catch (Exception)
                            {

                            }
                            #endregion

                            bilAPIWS.IncreaseBilletterieUserLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            int loginCount = Int32.Parse(bilAPIWS.GetBilletterieLoginCount(txtUsername.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]));
                            if (loginCount > Int32.Parse(ConfigurationManager.AppSettings["LoginAttempts"]))
                            {
                                bilAPIWS.UpdateBilletterieUserStatus(txtUsername.Text, "11", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }

                            lblMainErrorMessage.Visible = true;
                            lblMainErrorMessage.Text = "  [Incorrect Username/Password. Please try again.]";
                        }
                        #endregion
                    }

                    #endregion
                }
                else
                {
                    lblMainErrorMessage.Visible = true;
                    lblMainErrorMessage.Text = "Illegal characters detected. ";
                }
            }
            catch (Exception ex)
            {
                lblMainErrorMessage.Visible = true;
                lblMainErrorMessage.Text = ex.Message;
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

        //private BilletterieAPIWS.UpdateResponseObject ChangeUserPassword(string usr_username, string usr_password)
        //{
        //    BilletterieAPIWS.UpdateResponseObject returnValue;
        //    Common cm = new Common();
        //    DataAccess da = new DataAccess();
        //    BilletterieAPIWS.UpdateResponseObject updObj = new BilletterieAPIWS.UpdateResponseObject();
        //    updObj = bilAPIWS.UpdateBilletterieRecord("update TB_USR_User set USR_PassKey = '" + cm.GetMD5Hash(usr_password) + "' where USR_UserLogin = '" + usr_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //    //updObj = da.UpdateGenericBilletterieRecord("update TB_USR_User set USR_PassKey = '" + cm.GetMD5Hash(usr_password) + "' where USR_UserLogin = '" + usr_username + "'");
        //    returnValue = updObj;
        //    return returnValue;
        //}

        //private BilletterieAPIWS.InsertResponseObject RequestNewPassword(string usr_username)
        //{
        //    BilletterieAPIWS.InsertResponseObject returnValue = new BilletterieAPIWS.InsertResponseObject();
        //    Common cm = new Common();
        //    DataAccess da = new DataAccess();
        //    BilletterieAPIWS.UpdateResponseObject updObj = new BilletterieAPIWS.UpdateResponseObject();
        //    updObj = bilAPIWS.UpdateBilletterieRecord("update TB_USR_User set USR_ResetRequested = 1 where USR_UserLogin = '" + usr_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //    //updObj = da.UpdateGenericBilletterieRecord("update TB_USR_User set USR_ResetRequested = 1 where USR_UserLogin = '" + usr_username + "'");
        //    BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
        //    selResp = bilAPIWS.GetBilletterieScalar("select top 1 USR_PKID from TB_USR_User where USR_UserLogin = '" + usr_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //    //selResp = da.GetBilletterieGenericScalar("select top 1 USR_PKID from TB_USR_User where USR_UserLogin = '" + usr_username + "'");
        //    BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();

        //    if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
        //    {
        //        if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
        //        {
        //            EmailMessageObject emlObj = new EmailMessageObject();
        //            emlObj = PopulateEmailObject(selResp.selectedPKID);
        //            SendMail sm = new SendMail();
        //            SMTPMailResponseObject smtRespObj = new SMTPMailResponseObject();
        //            smtRespObj = sm.SendSMTPMail(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer);
        //            if (smtRespObj.noError)
        //            {
        //                emlObj.EML_Status = "2";
        //            }
        //            else
        //            {
        //                emlObj.EML_Status = "1";
        //            }
        //            //EmailDispatcherService emsWS = new EmailDispatcherService();
        //            //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
        //            opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //        }
        //        else
        //        {
        //            //EmailDispatcherService emsWS = new EmailDispatcherService();
        //            //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
        //            EmailMessageObject emlObj = new EmailMessageObject();
        //            emlObj = PopulateEmailObject(selResp.selectedPKID);
        //            opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //        }
        //    }
        //    returnValue = opResp;

        //    return returnValue;
        //}

        //private bool CurrentPasswordCorrect(string usr_username, string usr_password)
        //{
        //    Common cm = new Common();
        //    DataAccess da = new DataAccess();
        //    bool returnValue = false;
        //    BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
        //    selResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_PassKey = '" + cm.GetMD5Hash(usr_password) + "' and USR_UserLogin = '" + usr_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //    //selResp = da.GetBilletterieGenericIntScalar("select count(*) from TB_USR_User where USR_PassKey = '" + cm.GetMD5Hash(usr_password) + "' and USR_UserLogin = '" + usr_username + "'");
        //    if (Int32.Parse(selResp.selectedPKID) == 1)
        //    {
        //        returnValue = true;
        //    }
        //    return returnValue;
        //}

        //private bool UserAccountExist(string usr_username)
        //{
        //    DataAccess da = new DataAccess();
        //    bool returnValue = false;
        //    BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
        //    selResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + usr_username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //    //selResp = da.GetBilletterieGenericIntScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + usr_username + "'");
        //    if (Int32.Parse(selResp.selectedPKID) == 1)
        //    {
        //        returnValue = true;
        //    }
        //    return returnValue;
        //}

        //protected void btnChangePassword_Click(object sender, EventArgs e)
        //{
           
        //}

        //protected void lnkChangePassword_Click(object sender, EventArgs e)
        //{

        //}

        //private BilletterieAPIWS.EmailMessageObject PopulateEmailObject(string usrPKID)
        //{
        //    string uniqueGUID = "";
        //    BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
        //    BilletterieAPIWS.userProfileObject usrObj = new BilletterieAPIWS.userProfileObject();
        //    DataAccess da = new DataAccess();
        //    DataSet ds = new DataSet();
        //    ds = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USR_EmailAccount, USR_UserLogin, USR_FirstName, USR_LastName, USR_UniqueID from TB_USR_User where USR_PKID =" + usrPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //    //ds = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USR_EmailAccount, USR_UserLogin, USR_FirstName, USR_LastName, USR_UniqueID from TB_USR_User where USR_PKID =" + usrPKID);
        //    if (ds != null)
        //    {
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            returnValue.EML_ToEmailList = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
        //            returnValue.EML_ToEmailAdmin = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();     
        //            returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ": User account activation";
        //            usrObj.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
        //            usrObj.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
        //            usrObj.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
        //            usrObj.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
        //            uniqueGUID = ds.Tables[0].Rows[0]["USR_UniqueID"].ToString();

        //        }
        //    }
        //    returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
        //    returnValue.EML_MailBody = GetTicketResponseEmailBody(usrObj, uniqueGUID);
        //    returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
        //    returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
        //    returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
        //    returnValue.EML_Status = "1";
        //    returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
        //    returnValue.EML_KeyField = "USR_PKID";
        //    returnValue.EML_KeyValue = usrPKID;
        //    returnValue.EML_Domain = "0";
        //    returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];
        //    return returnValue;
        //}

        //private string GetTicketResponseEmailBody(BilletterieAPIWS.userProfileObject usrObj, string uniqueGUID)
        //{
        //    string returnValue = "";
        //    string activeLink = "";
        //    if (Request.Url != null)
        //    {
        //        if (Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]))
        //        {
        //            activeLink = ConfigurationManager.AppSettings["ExternalPasswordResetURL"] + uniqueGUID;
        //        }
        //        else
        //        {
        //            activeLink = ConfigurationManager.AppSettings["ExternalPasswordResetURL"] + uniqueGUID;
        //        }
        //    }
        //    string actualDefaultPassword = "<br /><br /> Your password will be reset to a randomly created string upon clicking the link.";
        //    string responseMessage = "You have requested that your user password be reset to a new password. Please note that in order to complete this process you need to click the link below. If the link is not active you need to copy and paste it onto your browser.<br /><br />" + activeLink + actualDefaultPassword;

        //    returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>User password change request for user [ " + usrObj.USR_UserLogin + " ].</h3></td></tr> ";
        //    returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : Reset of password for user account: " + usrObj.USR_UserLogin + "</h4></td></tr>";
        //    returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Date : </b>" + DateTime.Now.ToString() + "<br/><p>Dear " + usrObj.USR_FirstName + " " + usrObj.USR_LastName + ",<br/></p></td></tr>";
        //    returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>" + responseMessage + "<br /><br /></td></tr>";
        //    returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Note that any attached documents are ONLY accessible through the help desk system. Please feel free to ask if you may have any other query. <br /><br />Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
        //    returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
        //    return returnValue;
        //}

        protected void chkForgotPassword_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        //protected void lnkAccountCreationLink_Click(object sender, EventArgs e)
        //{
        //    if (bool.Parse(ConfigurationManager.AppSettings["UseInternalAccountLink"]) == true)
        //    {
        //        Session["GlobalSession"] = "New Account";
        //        Response.Redirect("~/CreateNewAccount.aspx", false);
        //    }

        //}

        protected void btnPasswordResetLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PasswordResetLogin.aspx", false);
        }

        protected void btnSubmitAccountDetails_Click(object sender, EventArgs e)
        {
            try
            {
                lblPopErrorMessage.Text = "";
                lblPopErrorMessage.Visible = false;

                if (ValidateFirstUser(txtUsername.Text, lblPasswordValue.Text))
                {
                    bilAPIWS.UpdateBilletterieUserStatus(txtUsername.Text, "15", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    if (bool.Parse(ConfigurationManager.AppSettings["CreateOPT"]))
                    {
                        string userOTPValue = bilAPIWS.CreateOTPValue(lblUSRPKID.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                        BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                        emlObj = PopulateOTPEmailObject(txtEmailAccount.Text, userOTPValue, lblUSRPKID.Text, txtUsername.Text, txtFullName.Text);

                        if (bool.Parse(ConfigurationManager.AppSettings["SendOnDemandOPT"]))
                        {
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
                            opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        }

                        lblOTPUSRPKID.Text = lblUSRPKID.Text;
                        lblWarningOTPMessage.Text = "You are logging into this system for the first time. You are required to provide an OTP sent to your CIPC registered email account.";
                        lblCustomerOTPHeading.Text = ""; // "You are encouraged to use this process to request refunds as it is quicker to verify and process the transaction ";
                        lblOTPERMSPKID.Text = txtUsername.Text;
                        lblBottomMessage.Text = "NOTE: A One Time Password has been sent to your email account. Delays up to 30 minutes can be experienced with send of OTPs."; // ConfigurationManager.AppSettings["CBRSBottomMessage"];
                        ModalPopupExtenderOTPPopup.Show();
                    }
                }
                else
                {
                    lblPopErrorMessage.Text = "Incorrect details have been captured.";
                    lblPopErrorMessage.Visible = true;
                    ModalPopupExtenderAccountDetail.Show();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private BilletterieAPIWS.EmailMessageObject PopulateOTPEmailObject(string emailList, string otpValue, string usrPKID, string cusAccount, string fullName)
        {

            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            returnValue.EML_ToEmailAdmin = emailList.Trim();
            returnValue.EML_ToEmailList = emailList.Trim();

            BilletterieAPIWS.smtpSettingsObject smtpSettings = new BilletterieAPIWS.smtpSettingsObject();
            smtpSettings = bilAPIWS.GetQRSEmailSettings(ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

            returnValue.EML_FromEmail = smtpSettings.fromEmail;
            returnValue.EML_Subject =  ConfigurationManager.AppSettings["OTPSubject"] + ":" + cusAccount;
            returnValue.EML_MailBody = GetOTPConfirmationEmailBody(otpValue, cusAccount, fullName);
            returnValue.EML_SMTPServer = smtpSettings.smtpServer;
            returnValue.EML_SMTPPassword = smtpSettings.smtUserPass;
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = "1";
            returnValue.EML_CCEmail = smtpSettings.bccEmail;
            returnValue.EML_KeyField = "USR_PKIDOTP";
            returnValue.EML_KeyValue = usrPKID;
            returnValue.EML_Domain = "0";
            returnValue.EML_Priority = "0";
            returnValue.EML_SupportToEmail = smtpSettings.toCIPCEmail;

            return returnValue;
        }

        private string GetOTPConfirmationEmailBody(string OTPValue, string cusAccount, string userFullName)
        {
            string returnValue = "";
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>One Time Password (OTP) # [ " + cusAccount + " ].</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject request for OTP for CIPC Account : " + cusAccount + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/>Date :" + DateTime.Now.ToString() + "<br/><p>Dear " + userFullName + "  ,<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>An OTP has been created using " + ConfigurationManager.AppSettings["SystemTitle"] + " system; your attention is required.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><table style='margin-left:10px; border-collapse: collapse;'><tr style='border: none;'><td style='border-left:  solid 3px blue; min-height:30px; color: green;'><i>  Please use OTP value of " + OTPValue + " to proceed with login. </i></td></tr></table><br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>This OTP is valid for 24 hours from the generation of this email.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Help Desk <br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
            return returnValue;
        }

        private bool ValidateFirstUser(string username, string password)
        {
            bool returnValue = true;
            BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();

            InformixDataSetResponse dsResp = new InformixDataSetResponse();
            dsResp = bilAPIWS.GetInformixDataSet("select agent_code, password, agent_name, tel_code, tel_no, email, cell_no, agent_id_no from agents where agent_code = '" + username + "' and password = '" + password + "' and status = 'A'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            DataSet ds = new DataSet();
            if (dsResp.noError)
            {
                ds = dsResp.ermsDataSetObject;
                //BilletterieAPIWS.userProfileObject ermsUserObj = new BilletterieAPIWS.userProfileObject();
                usrProfile = PopulateUserObjectFromERMS(ds);
            }

            if (usrProfile.noError)
            {
                //usrProfile = cm.GetERMSUserProfileObject(txtUsername.Text.Trim().ToUpper(), txtPassword.Text.Trim());
                if (txtIDNumber.Text.ToLower().Trim() != usrProfile.USR_IDNumber.ToLower().Trim())
                {
                    returnValue = false;
                    return returnValue;
                }
                if (txtFullName.Text.ToLower() != usrProfile.USR_FullName.ToLower())
                {
                    returnValue = false;
                    return returnValue;
                }
                if (txtEmailAccount.Text.ToLower() != usrProfile.USR_EmailAccount.ToLower())
                {
                    returnValue = false;
                    return returnValue;
                }
                if (txtCustContact.Text.ToLower() != usrProfile.USR_MobileNumber.ToLower())
                {
                    returnValue = false;
                    return returnValue;
                }
            }
            else
            {
                returnValue = false;
                return returnValue;
            }

            return returnValue;
        }

        private BilletterieAPIWS.userProfileObject PopulateUserObjectFromERMS(DataSet ifxDS)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                if (ifxDS != null)
                {
                    if (ifxDS.Tables[0].Rows.Count > 0)
                    {
                        returnValue.USC_PKID = 1;
                        returnValue.USR_UserLogin = ifxDS.Tables[0].Rows[0]["agent_code"].ToString();
                        returnValue.USR_PassKey = GetMD5Hash(ifxDS.Tables[0].Rows[0]["password"].ToString());

                        returnValue.USR_FullName = ifxDS.Tables[0].Rows[0]["agent_name"].ToString().Trim();
                        returnValue.USR_IDNumber = ifxDS.Tables[0].Rows[0]["agent_id_no"].ToString().Trim();
                        returnValue.USR_FirstName = GetFirstSplitValue(ifxDS.Tables[0].Rows[0]["agent_name"].ToString()).Trim();
                        returnValue.USR_LastName = GetLastSplitValue(ifxDS.Tables[0].Rows[0]["agent_name"].ToString()).Trim();
                        returnValue.USR_MobileNumber = ifxDS.Tables[0].Rows[0]["cell_no"].ToString();
                        returnValue.USR_EmailAccount = ifxDS.Tables[0].Rows[0]["email"].ToString();
                        returnValue.USR_DateCreated = DateTime.Now.ToString();
                        returnValue.USR_ActivationDate = DateTime.Now.ToString();
                        returnValue.STS_PKID = 14;  //Account is new
                        returnValue.USG_PKID = Int32.Parse(ConfigurationManager.AppSettings["DefaultUserOffice"]);
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                    }
                }
                else
                {
                    returnValue.noError = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;

        }

        public string GetLastSplitValue(string fullTextValue)
        {
            string returnValue = "";
            string[] multiValues = fullTextValue.Split(' ');
            if (multiValues.Length > 1)
            {
                for (int i = 1; i < multiValues.Length; i++)
                {
                    returnValue = returnValue + " " + multiValues[i];
                }
            }
            else
            {
                returnValue = fullTextValue;
            }
            return returnValue;
        }

        public string GetFirstSplitValue(string fullTextValue)
        {
            string returnValue = fullTextValue;
            string[] multiValues = fullTextValue.Split(' ');
            if (fullTextValue.Contains(" "))
            {
                returnValue = multiValues[0];
            }
            return returnValue;
        }

        public string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        protected void btnOTPSubmit_Click(object sender, EventArgs e)
        {
            string userOTPValue = bilAPIWS.GetUserOTPValue(lblOTPUSRPKID.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (userOTPValue == txtCustomerOTP.Text)
            {
                bilAPIWS.UpdateBilletterieUserStatus(txtUsername.Text, "10", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                #region
                BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();
                usrProfile = cm.GetUserProfileObject(txtUsername.Text.Trim(), cm.GetMD5Hash(txtPassword.Text.Trim()));

                FormsAuthentication.RedirectFromLoginPage(txtUsername.Text, false);
                lblMainErrorMessage.Visible = false;
                lblMainErrorMessage.Text = "";
                Session["GlobalSession"] = "LoggedIn";
                Session["userObjectCookie"] = usrProfile;
                if (txtPreferredEmail.Text.Trim() != "")
                {
                    Session["preferredEmail"] = txtPreferredEmail.Text.Trim();
                }
                else
                {
                    Session["preferredEmail"] = null;
                }
                bilAPIWS.ResetBilletterieUserLoginCount(usrProfile.USR_PKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                Session["CustomerBankDetailDisplayed"] = false;
                Response.Redirect("~/ExternalPages/NewTicket.aspx", false);
                #endregion


            }
            else
            {
                lblOTPPopErrorMessage.Text = "Incorrect OTP has been captured.";
                ModalPopupExtenderOTPPopup.Show();
            }
        }


    }
}