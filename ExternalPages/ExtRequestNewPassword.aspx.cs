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
    public partial class ExtRequestNewPassword : System.Web.UI.Page
    {

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        protected void Page_Load(object sender, EventArgs e)
        {
            //btnClose.Attributes.Add("OnClick", "window.close();");

            string sObjID = Request.QueryString["transID"];
            if (sObjID != null)
            {
                if (UserCanResetPassword(sObjID))
                {
                    lblGridRowError.Text = "";
                    lblGridRowError.Visible = false;
                    NewBilletterie.Classes.InsertResponseObject insResp = new NewBilletterie.Classes.InsertResponseObject();
                    insResp = ChangeUserPassword(sObjID);
                    if (insResp.noError)
                    {
                        SendConfirmationEmail(sObjID, insResp.insertedPKID);
                        lblGridRowError.Text = "Password successfully reset. A confirmation email will be sent to you shortly.";
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
                    lblGridRowError.Text = "Request for change has not been submitted.";
                    lblGridRowError.Visible = true;
                    lblGridRowError.ForeColor = Color.Red;
                }
            }
        }

        private BilletterieAPIWS.InsertResponseObject SendConfirmationEmail(string sObjID, string randomPassKey)
        {
            BilletterieAPIWS.InsertResponseObject returnValue = new BilletterieAPIWS.InsertResponseObject();
            Common cm = new Common();
            DataAccess da = new DataAccess();

           BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select top 1 USR_PKID from TB_USR_User where USR_UniqueID = '" + sObjID + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //selResp = da.GetBilletterieGenericScalar("select top 1 USR_PKID from TB_USR_User where USR_UniqueID = '" + sObjID + "'");
            BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();

            if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
            {
                //EmailDispatcherService emsWS = new EmailDispatcherService(); ;
                //EmailMessageObject emlObj = new EmailMessageObject();
                //emlObj = PopulateEmailObject(selResp.selectedPKID, randomPassKey);
                //opResp = emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);

                if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                {
                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                    emlObj = PopulateEmailObject(selResp.selectedPKID, randomPassKey);
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
                    emlObj = PopulateEmailObject(selResp.selectedPKID, randomPassKey);
                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                }
            }
            returnValue = opResp;

            return returnValue;
        }

        private EmailMessageObject PopulateEmailObject(string usrPKID, string randomPassKey)
        {
            string uniqueGUID = "";
            EmailMessageObject returnValue = new EmailMessageObject();
            BilletterieAPIWS.userProfileObject usrObj = new BilletterieAPIWS.userProfileObject();
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            //ds = da.GetGenericBilletterieDataSet("TB_OFC_Officer", "TB_OFC_OfficerDS", "select OFC_PKID, OFC_EmailAccount, OFC_UserLogin, OFC_FirstName, OFC_Surname, OFC_UniqueID from TB_OFC_Officer where OFC_PKID = " + ofcPKID);
            ds = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USR_EmailAccount, USR_UserLogin, USR_FirstName, USR_LastName, USR_UniqueID from TB_USR_User where USR_PKID =" + usrPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USR_EmailAccount, USR_UserLogin, USR_FirstName, USR_LastName, USR_UniqueID from TB_USR_User where USR_PKID =" + usrPKID);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //returnValue.EML_ToEmailList = ds.Tables[0].Rows[0]["OFC_EmailAccount"].ToString();
                    //returnValue.EML_ToEmailAdmin = ds.Tables[0].Rows[0]["OFC_EmailAccount"].ToString();
                    //returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ": User password reset request";
                    //usrObj.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFC_PKID"].ToString());
                    //usrObj.USR_UserLogin = ds.Tables[0].Rows[0]["OFC_UserLogin"].ToString();
                    //usrObj.USR_FirstName = ds.Tables[0].Rows[0]["OFC_FirstName"].ToString();
                    //usrObj.USR_LastName = ds.Tables[0].Rows[0]["OFC_Surname"].ToString();
                    //uniqueGUID = ds.Tables[0].Rows[0]["OFC_UniqueID"].ToString();

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
            returnValue.EML_MailBody = GetPasswordResetEmailBody(usrObj, uniqueGUID, randomPassKey);
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

        private string GetPasswordResetEmailBody(BilletterieAPIWS.userProfileObject usrObj, string uniqueGUID, string clearPassword)
        {
            string returnValue = "";
            string responseMessage = "Your password has been reset to <b>" + clearPassword + "</b>. You are encouraged to change this password immediately for security reasons.";
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>Password reset confirmation for user [<b>" + usrObj.USR_UserLogin + "</b>].</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : Password successfully reset for user account: " + usrObj.USR_UserLogin + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Date : </b>" + DateTime.Now.ToString() + "<br/><p>Dear " + usrObj.USR_FirstName + " " + usrObj.USR_LastName + ",<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>" + responseMessage + "<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please feel free to ask if you may have any other query. <br /><br />Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
            return returnValue;
        }

        private NewBilletterie.Classes.InsertResponseObject ChangeUserPassword(string usr_uniqueID)
        {
            NewBilletterie.Classes.InsertResponseObject returnValue = new NewBilletterie.Classes.InsertResponseObject();
            try
            {
                Common cm = new Common();
                DataAccess da = new DataAccess();
                string randomPassword = RandomString(Int32.Parse(ConfigurationManager.AppSettings["DefaultPasswordLength"]));
                BilletterieAPIWS.UpdateResponseObject updObj = new BilletterieAPIWS.UpdateResponseObject();
                updObj = bilAPIWS.UpdateBilletterieRecord("update TB_USR_User set USR_PassKey = '" + cm.GetMD5Hash(randomPassword) + "', USR_ResetRequested = 0 where USR_UniqueID = '" + usr_uniqueID + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //updObj = da.UpdateGenericBilletterieRecord("update TB_USR_User set USR_PassKey = '" + cm.GetMD5Hash(randomPassword) + "', USR_ResetRequested = 0 where USR_UniqueID = '" + usr_uniqueID + "'");
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

        private bool UserCanResetPassword(string uniqueGUID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select top 1 USR_ResetRequested from TB_USR_User where USR_UniqueID = '" + uniqueGUID + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //selResp = da.GetBilletterieGenericScalar("select top 1 USR_ResetRequested from TB_USR_User where USR_UniqueID = '" + uniqueGUID + "'");
            if (bool.Parse(selResp.selectedPKID) == true)
            {
                returnValue = true;
            }
            return returnValue;
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Index.aspx", false);
        }


    }
}