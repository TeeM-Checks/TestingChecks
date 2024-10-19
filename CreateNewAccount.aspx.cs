using NewBilletterie.Classes;
//using NewBilletterie.EmailWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;


namespace NewBilletterie
{
    public partial class CreateNewAccount : System.Web.UI.Page
    {
       BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateCaptchaText();
            }
        }

        private void UpdateCaptchaText()
        {

            txtCaptchaText.Text = string.Empty;
            lblStatus.Visible = false;
            Session["UserCaptcha"] = Guid.NewGuid().ToString().Substring(0, 6);
        }

        private void LoadDropDowns()
        {
            DataSet dsCountry = new DataSet();
            dsCountry = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName] union select CAT_PKID, CAT_Order, CAT_CategoryName from TB_CAT_Category where CAT_MasterID = 0 and STS_PKID = 50 and CAT_Visible = 1 order by CAT_Order asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsCountry != null)
            {
                ddlCountry.DataSource = dsCountry.Tables[0];
                ddlCountry.DataTextField = "CAT_CategoryName";
                ddlCountry.DataValueField = "CAT_PKID";
                ddlCountry.DataBind();
            }
        }

        private BilletterieAPIWS.userProfileObject PopulateExternalUserObject()
        {
            Common cm = new Common();
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                returnValue.USC_PKID = 5;
                returnValue.USR_UserLogin = txtUserName.Text.Trim();
                returnValue.USR_PassKey = cm.GetMD5Hash(txtUserPassword.Text.Trim());
                returnValue.USR_FirstName = txtFirstName.Text;
                returnValue.USR_LastName = txtSurname.Text.Trim();
                returnValue.USR_MobileNumber = txtContactNumber.Text.Trim();
                returnValue.USR_EmailAccount = txtUserEmail.Text.Trim();
                returnValue.USR_DateCreated = DateTime.Now.ToString();
                returnValue.USR_ActivationDate = DateTime.Now.ToString();
                returnValue.STS_PKID = 13;
                returnValue.USG_PKID = 1;
                returnValue.USR_Comments = "";
                returnValue.noError = true;
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;

        }

        private BilletterieAPIWS.organisationObject PopulateOrganisationObject()
        {
            Common cm = new Common();
            BilletterieAPIWS.organisationObject returnValue = new BilletterieAPIWS.organisationObject();
            try
            {
                returnValue.OGA_OrganisationName = txtOrganisation.Text.Trim();
                returnValue.OGA_AddressLine = txtStreetAddress.Text.Trim();
                returnValue.OGA_Surburb = txtSurburb.Text.Trim();
                returnValue.OGA_City = txtCityTown.Text.Trim();
                returnValue.OGA_Country = ddlCountry.SelectedValue;
                returnValue.OGA_Code = txtPostalCode.Text.Trim();
                returnValue.noError = true;
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;

        }

        private string ValidateExternalUser(BilletterieAPIWS.userProfileObject usrObj)
        {
            //DataAccess da = new DataAccess();
            string returnValue = "";

            try
            {
                if (usrObj.USC_PKID <= 0)
                {
                    returnValue = returnValue + "Select user source." + "\n\r";
                }

                if (usrObj.USR_UserLogin == "")
                {
                    returnValue = returnValue + "Username is a required field." + "\n\r";
                }

                if (usrObj.USR_PassKey == "")
                {
                    returnValue = returnValue + "User password is a required field" + "\n\r";
                }

                if (usrObj.USR_FirstName == "")
                {
                    returnValue = returnValue + "First name is a required field" + "\n\r";
                }

                if (usrObj.USR_LastName == "")
                {
                    returnValue = returnValue + "Last name is a required field" + "\n\r";
                }

                if (usrObj.USR_MobileNumber == "")
                {
                    returnValue = returnValue + "Mobile contact number is a required field" + "\n\r";
                }

                if (usrObj.USR_EmailAccount == "")
                {
                    returnValue = returnValue + "Email address is a required field" + "\n\r";
                }

                if (usrObj.STS_PKID <= 0)
                {
                    returnValue = returnValue + "Select user's status." + "\n\r";
                }

                BilletterieAPIWS.SelectStringResponseObject sResp = new BilletterieAPIWS.SelectStringResponseObject();
                sResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + usrObj.USR_UserLogin + "' or USR_EmailAccount = '" + usrObj.USR_EmailAccount + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                if (Int32.Parse(sResp.selectedPKID) > 0)
                {
                    returnValue = "Username or email account must be unique." + "\n\r";
                }

            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        private string ValidateOrganisation(BilletterieAPIWS.organisationObject ogaObj)
        {
            DataAccess da = new DataAccess();
            string returnValue = "";

            try
            {
                if (ogaObj.OGA_OrganisationName == "")
                {
                    returnValue = returnValue + "Organisation name is required." + "\n\r";
                }

                if (ogaObj.OGA_AddressLine == "")
                {
                    returnValue = returnValue + "Address is required." + "\n\r";
                }

                if (ogaObj.OGA_Surburb == "")
                {
                    returnValue = returnValue + "Surburb is a required field" + "\n\r";
                }

                if (ogaObj.OGA_City == "")
                {
                    returnValue = returnValue + "City is a required field" + "\n\r";
                }

                if (ogaObj.OGA_Country == "")
                {
                    returnValue = returnValue + "Country is a required field" + "\n\r";
                }

            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        protected void btnCreateNewUser_Click(object sender, EventArgs e)
        {
            if (txtCaptchaText.Text.Trim() == Session["UserCaptcha"].ToString())
            {
                //DataAccess da = new DataAccess();
                BilletterieAPIWS.userProfileObject cubaUserObj = new BilletterieAPIWS.userProfileObject();
                cubaUserObj = PopulateExternalUserObject();

                BilletterieAPIWS.organisationObject organisationObj = new BilletterieAPIWS.organisationObject();
                organisationObj = PopulateOrganisationObject();

                if (cubaUserObj.noError)
                {
                    string errorMess = ValidateExternalUser(cubaUserObj);
                    if (errorMess == "")
                    {
                        string errorMessOGA = ValidateOrganisation(organisationObj);

                        if (errorMessOGA == "")
                        {
                         BilletterieAPIWS.InsertResponseObject insRespOGA = new BilletterieAPIWS.InsertResponseObject();
                            insRespOGA = bilAPIWS.InsertBilletterieOrganisationRecord(organisationObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                            if (insRespOGA.noError)
                            {
                                cubaUserObj.OGA_PKID = Int32.Parse(insRespOGA.insertedPKID);
                                BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                                insResp = bilAPIWS.InsertBilletterieUserRecord(cubaUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                                lblErrorMessage.Visible = false;
                                lblErrorMessage.Text = "";

                                BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                                try
                                {
                                    if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                                    {
                                        if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                        {
                                            EmailMessageObject emlObj = new EmailMessageObject();
                                            emlObj = PopulateEmailObject(insResp.insertedPKID);
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
                                            emlObj = PopulateEmailObject(insResp.insertedPKID);
                                            opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        }
                                        ModalPopupExtenderSuccess.Show();
                                        lblUserConfirmation.Text = "User account successfully submitted, an email has been sent to you. You need to activate your account before you can login to the system.";
                                    }
                                    else
                                    {
                                        ModalPopupExtenderSuccess.Show();
                                        lblUserConfirmation.Text = "User account successfully submitted, your account will be activated shortly.";
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                        else
                        {
                            lblErrorMessage.Visible = true;
                            lblErrorMessage.Text = errorMess;
                        }
                    }
                    else
                    {
                        lblErrorMessage.Visible = true;
                        lblErrorMessage.Text = errorMess;
                    }
                }
            }
        }

        private BilletterieAPIWS.EmailMessageObject PopulateEmailObject(string usrPKID)
        {
            string uniqueGUID = "";
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
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
            returnValue.EML_MailBody = GetAccountActivationEmailBody(usrObj, uniqueGUID);
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Index.aspx", false);
        }

        protected void btnNewCaptcha_Click(object sender, EventArgs e)
        {
            UpdateCaptchaText();
        }

        private string GetAccountActivationEmailBody(BilletterieAPIWS.userProfileObject usrObj, string uniqueGUID)
        {
            string returnValue = "";
            string activeLink = "";
            if (Request.Url != null)
            {
                if (Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]))
                {
                    activeLink = ConfigurationManager.AppSettings["PublicAccountActivationURL"] + uniqueGUID;
                }
                else
                {
                    activeLink = ConfigurationManager.AppSettings["PublicAccountActivationURL"] + uniqueGUID;
                }
            }

            string responseMessage = "<br /><br /> Thank you for registering on the " + ConfigurationManager.AppSettings["OrganisationName"] + " " + ConfigurationManager.AppSettings["SystemTitle"] + " with the following username: <br /><br /> Username: " + usrObj.USR_UserLogin;
            responseMessage = responseMessage + "<br /><br /> Please click on the link below to activate your account: <br />" + activeLink;
            responseMessage = responseMessage + "<br /><br /> Please note that account data not used for more than 15 days will be automatically deleted and new registration will be necessary.";
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>User account activation for user [ " + usrObj.USR_UserLogin + " ].</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : Activation for user account: " + usrObj.USR_UserLogin + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/><b>Date : </b>" + DateTime.Now.ToString() + "<br/><p>Dear " + usrObj.USR_FirstName + " " + usrObj.USR_LastName + ",<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>" + responseMessage + "<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please feel free to ask if you may have any other query. <br /><br />Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
            return returnValue;
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Index.aspx", false);
        }

    }
}