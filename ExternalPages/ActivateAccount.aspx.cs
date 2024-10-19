using NewBilletterie.Classes;
//using NewBilletterie.CUBAServerService;
//using NewBilletterie.EmailWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class ActivateAccount : System.Web.UI.Page
    {

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string sObjID = Request.QueryString["transID"];
                if (sObjID != null)
                {
                    if (UserCanActivateAccount(sObjID))
                    {
                        lblGridRowError.Text = "";
                        lblGridRowError.Visible = false;
                        Classes.InsertResponseObject insResp = new Classes.InsertResponseObject();
                        insResp = ActivateUserAccount(sObjID);
                        if (insResp.noError)
                        {
                            SendConfirmationEmail(sObjID, insResp.insertedPKID);
                            lblGridRowError.Text = "Account successfully activated. A confirmation email will be sent to you shortly.";
                            lblGridRowError.ForeColor = Color.Green;
                            lblGridRowError.Visible = true;
                        }
                        else
                        {
                            lblGridRowError.Text = insResp.errorMessage;
                            lblGridRowError.Visible = true;
                            lblGridRowError.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        lblGridRowError.Text = "User account not availbale for activation or it has been more than 15 days since you requested a user account. You are required to use the password reset option in order to continue.";
                        lblGridRowError.Visible = true;
                        lblGridRowError.ForeColor = Color.Red;
                    }
                }
            }
        }

        private BilletterieAPIWS.InsertResponseObject SendConfirmationEmail(string sObjID, string randomPassKey)
        {
            BilletterieAPIWS.InsertResponseObject returnValue = new BilletterieAPIWS.InsertResponseObject();
            Common cm = new Common();
            //DataAccess da = new DataAccess();

            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select top 1 USR_PKID from TB_USR_User where USR_UniqueID = '" + sObjID + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();

            if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
            {
                if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                {
                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
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
                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                    emlObj = PopulateEmailObject(selResp.selectedPKID);
                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                }
            }
            returnValue = opResp;

            return returnValue;
        }

        private EmailMessageObject PopulateEmailObject(string usrPKID)
        {
            string uniqueGUID = "";
            EmailMessageObject returnValue = new EmailMessageObject();
            BilletterieAPIWS.userProfileObject usrObj = new BilletterieAPIWS.userProfileObject();
            //DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USR_EmailAccount, USR_UserLogin, USR_FirstName, USR_LastName, USR_UniqueID from TB_USR_User where USR_PKID =" + usrPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    returnValue.EML_ToEmailList = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
                    returnValue.EML_ToEmailAdmin = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();     // ConfigurationManager.AppSettings["To"];
                    returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ": User account activation";
                    usrObj.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                    usrObj.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
                    usrObj.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
                    usrObj.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
                    uniqueGUID = ds.Tables[0].Rows[0]["USR_UniqueID"].ToString();

                }
            }
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_MailBody = GetActivationConfirmationEmailBody(usrObj, uniqueGUID);
            returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = "1";
            returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
            returnValue.EML_KeyField = "USR_PKID";
            returnValue.EML_KeyValue = usrPKID;
            returnValue.EML_Domain = "0";
            returnValue.EML_Priority = "0";
            returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];
            return returnValue;
        }

        private string GetActivationConfirmationEmailBody(BilletterieAPIWS.userProfileObject usrObj, string uniqueGUID)
        {
            string returnValue = "";
            string responseMessage = "Your user account has been activated. You can now proceed with login using your username " + usrObj.USR_UserLogin;
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>Account activation confirmation for user [<b>" + usrObj.USR_UserLogin + "</b>].</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : Activation for user account: " + usrObj.USR_UserLogin + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Date : </b>" + DateTime.Now.ToString() + "<br/><p>Dear " + usrObj.USR_FirstName + " " + usrObj.USR_LastName + ",<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>" + responseMessage + "<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please feel free to ask if you may have any other query. <br /><br />Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
            return returnValue;
        }

        private NewBilletterie.Classes.InsertResponseObject ActivateUserAccount(string usr_uniqueID)
        {
            Classes.InsertResponseObject returnValue = new Classes.InsertResponseObject();
            try
            {
                Common cm = new Common();
                //DataAccess da = new DataAccess();
                string randomPassword = RandomString(Int32.Parse(ConfigurationManager.AppSettings["DefaultPasswordLength"]));
                BilletterieAPIWS.UpdateResponseObject updObj = new BilletterieAPIWS.UpdateResponseObject();
                updObj = bilAPIWS.UpdateBilletterieRecord("update TB_USR_User set STS_PKID = 10, USR_ActivationDate = GETDATE() where USR_UniqueID = '" + usr_uniqueID + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                returnValue.insertedPKID = randomPassword;
                returnValue.noError = true;
                returnValue.errorMessage = "";
            }
            catch (Exception ex)
            {
                returnValue.insertedPKID = "";
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        private string RandomString(int passwordLength)
        {
            Random rnd = new Random();
            int inputPosition = 0;
            string inputString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < passwordLength; i++)
            {
                inputPosition = rnd.Next(0, inputString.Length);
                ch = inputString[inputPosition];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private bool UserCanActivateAccount(string uniqueGUID)
        {
            bool returnValue = false;
            //DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select top 1 USR_DateCreated from TB_USR_User where USR_UniqueID = '" + uniqueGUID + "' and STS_PKID = 13", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if ((DateTime.Now - DateTime.Parse(selResp.selectedPKID)).Days < 15)
            {
                returnValue = true;
            }
            return returnValue;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AdminLogin.aspx", false);
        }

    }
}