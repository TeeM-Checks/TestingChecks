using NewBilletterie.CaseDocuments;
using NewBilletterie.Classes;
//using NewBilletterie.CUBAServerService;
//using NewBilletterie.EmailWS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class NewPasswordTicket : System.Web.UI.Page
    {
        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                txtTicketSubject.Text = Request.QueryString["Subject"];
                txtReferenceNo.Text = Request.QueryString["Reference"];

                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));

                btnSubmitTicket.BackColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonBackColor"]);
                btnSubmitTicket.ForeColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonForeColor"]);
                Session["SelectedCATPKID"] = "0";
                Session["MobileViewLink"] = "../Mobile/MobileNewTicket.aspx";

                if (bool.Parse(ConfigurationManager.AppSettings["UseDefaultLabels"]) == false)
                {
                    try
                    {
                        litCreateNew.Text = ConfigurationManager.AppSettings["litCreateNewText"];
                    }
                    catch (Exception)
                    {

                    }
                }

                UpdateCaptchaText();
                if (Session["userObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                }
                else
                {
                    Response.Redirect("~/Index.aspx", false);
                }
                LoadDropDowns();

                ticketInformationObject ticketInfo = new ticketInformationObject();
                ticketInfo = (ticketInformationObject)Session["PassworsResetCookie"];
              

                ddlProvince.SelectedValue = ticketInfo.Province;
                ddlProvince.Enabled = false;

                //if (bool.Parse(ConfigurationManager.AppSettings["HideOldLinks"]))
                //{
                //    btnViewOldTickets.Visible = false;
                //}

                if (bool.Parse(ConfigurationManager.AppSettings["ShowUserGroup"]))
                {
                    lblUserGroup.Visible = true;
                    ddlUserGroup.Visible = true;
                }


            }
        }

        #region Custom Methods

        private void UpdateCaptchaText()
        {

            txtCaptchaText.Text = string.Empty;
            lblStatus.Visible = false;
            Session["BillCaptcha"] = Guid.NewGuid().ToString().Substring(0, 6);
        }

        private void LoadDropDowns()
        {
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


            Common cm = new Common();
            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select top 1 (select ', ' + AMT_Extention  from TB_AMT_AllowedMimeType T ORDER BY T.AMT_PKID FOR XML PATH('')) [Escalation List]  from TB_AMT_AllowedMimeType where STS_PKID = 60", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //selResp = da.GetBilletterieGenericScalar("select top 1 (select ', ' + AMT_Extention  from TB_AMT_AllowedMimeType T ORDER BY T.AMT_PKID FOR XML PATH('')) [Escalation List]  from TB_AMT_AllowedMimeType where STS_PKID = 60");
            lblAllowedExtentions.Text = "Allowed extentions " + cm.CleanUpValues(selResp.selectedPKID);


            if (Session["userObjectCookie"] != null)
            {
                BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

                DataSet dsUserGroup = new DataSet();
                dsUserGroup = bilAPIWS.GetBilletterieDataSet("select USG_PKID, USG_UserGroupName from TB_USG_UserGroup where USG_PKID = " + usrSession.USG_PKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dsUserGroup = da.GetGenericBilletterieDataSet("TB_USG_UserGroup", "TB_USG_UserGroupDS", "select USG_PKID, USG_UserGroupName from TB_USG_UserGroup where USG_PKID = " + usrSession.USG_PKID);
                if (dsUserGroup != null)
                {
                    ddlUserGroup.DataSource = dsUserGroup.Tables[0];
                    ddlUserGroup.DataTextField = "USG_UserGroupName";
                    ddlUserGroup.DataValueField = "USG_PKID";
                    ddlUserGroup.DataBind();
                }

                ddlUserGroup.SelectedIndex = 0;
            }




        }

        //private bool PopulateCategoryDDL(string masterPKID)
        //{
        //    bool returnValue = false;
        //    DataAccess da = new DataAccess();
        //    DataSet dsCategory = new DataSet();
        //    dsCategory = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName] union select CAT_PKID, CAT_Order, CAT_CategoryName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 order by CAT_CategoryName asc");
        //    if (dsCategory != null)
        //    {
        //        if (dsCategory.Tables[0].Rows.Count > 1)
        //        {
        //            ddlCategory.DataSource = dsCategory.Tables[0];
        //            ddlCategory.DataTextField = "CAT_CategoryName";
        //            ddlCategory.DataValueField = "CAT_PKID";
        //            ddlCategory.DataBind();
        //            returnValue = true;
        //            Session["CategoryVisible"] = true;
        //        }
        //        else
        //        {
        //            returnValue = false;
        //        }
        //    }
        //    else
        //    {
        //        returnValue = false;
        //    }

        //    return returnValue;
        //}

        //private bool PopulateSubCategoryDDL(string masterPKID)
        //{
        //    bool returnValue = false;
        //    DataAccess da = new DataAccess();
        //    DataSet dsSubCategory = new DataSet();
        //    dsSubCategory = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName] union select CAT_PKID, CAT_Order, CAT_CategoryName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 order by CAT_CategoryName asc");
        //    if (dsSubCategory != null)
        //    {
        //        if (dsSubCategory.Tables[0].Rows.Count > 1)
        //        {
        //            ddlSubCategory.DataSource = dsSubCategory.Tables[0];
        //            ddlSubCategory.DataTextField = "CAT_CategoryName";
        //            ddlSubCategory.DataValueField = "CAT_PKID";
        //            ddlSubCategory.DataBind();
        //            returnValue = true;
        //            Session["SubCategoryVisible"] = true;
        //        }
        //        else
        //        {
        //            returnValue = false;
        //        }
        //    }
        //    else
        //    {
        //        returnValue = false;
        //    }

        //    return returnValue;
        //}

        private string GetDefaultPriority(string catPKID)
        {
            string returnValue = "";
            DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject resp = new BilletterieAPIWS.SelectStringResponseObject();
            resp = bilAPIWS.GetBilletterieScalar("select TPT_PKID from TB_CAT_Category where CAT_PKID = " + catPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //resp = da.GetBilletterieGenericScalar("select TPT_PKID from TB_CAT_Category where CAT_PKID = " + catPKID);
            if (resp.noError)
            {
                returnValue = resp.selectedPKID;
            }
            else
            {
                //Error handling
                returnValue = "0";
            }
            return returnValue;

        }

        private string ValidateFileBeforeUpload()
        {
            Common cm = new Common();
            string returnValue = "";
            try
            {
                if (fupAttachFile.HasFile == true)
                {
                    if (!cm.ValidMimeType(fupAttachFile.PostedFile.ContentType, Path.GetExtension(fupAttachFile.FileName)))
                    {
                        return returnValue = "File type not supported.";
                    }
                    if (fupAttachFile.FileBytes.Length > 10000000)
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

        private BilletterieAPIWS.ticketObject PopulateTicketObject()
        {
            BilletterieAPIWS.ticketObject returnValue = new BilletterieAPIWS.ticketObject();
            try
            {
                if (Session["userObjectCookie"] != null)
                {
                    //Populate ticket user ID
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    returnValue.USR_PKID = usrSession.USR_PKID;

                    #region Populate file attachments
                    Common cm = new Common();
                    try
                    {
                        cm.CleanUpTempFiles(returnValue.USR_PKID);
                    }
                    catch (Exception)
                    {

                    }

                    BilletterieAPIWS.fileAttachmentObject atchObj = new BilletterieAPIWS.fileAttachmentObject();
                    if (fupAttachFile.HasFile == true)
                    {
                        atchObj.DCM_OriginalName = fupAttachFile.FileName;
                        atchObj.AttachmentSize = fupAttachFile.FileBytes.Length;
                        atchObj.MimeType = fupAttachFile.PostedFile.ContentType;
                        atchObj.DCM_Extention = Path.GetExtension(fupAttachFile.FileName);
                        atchObj.DCM_DerivedName = "doc" + returnValue.USR_PKID.ToString() + Path.GetExtension(fupAttachFile.FileName);
                        atchObj.DCT_PKID = 1;
                        atchObj.DCS_PKID = 1;
                        atchObj.DCL_PKID = 1;
                        atchObj.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + atchObj.DCM_DerivedName;  //Server.MapPath("~/Temp/" + atchObj.DCM_DerivedName);
                        atchObj.STS_PKID = 30;
                        atchObj.DCM_FileField = fupAttachFile.FileBytes;
                        string filePath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + atchObj.DCM_DerivedName; //Server.MapPath("~/Temp/" + atchObj.DCM_DerivedName);
                        if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                        {
                            fupAttachFile.SaveAs(filePath);
                        }

                        returnValue.AttachedFile = atchObj;
                        returnValue.TCK_HasFile = true;
                    }
                    else
                    {
                        returnValue.AttachedFile = null;
                    }
                    #endregion

                    returnValue.CAT_PKID = Int32.Parse(ConfigurationManager.AppSettings["PasswordResetCategory"]);

                    returnValue.UST_PKID = 1;
                    returnValue.OFC_PKID = 0;
                    returnValue.TCK_TicketNumber = "";
                    returnValue.TCK_Subject = txtTicketSubject.Text.Trim();
                    returnValue.TCK_Reference = txtReferenceNo.Text.Trim();
                    returnValue.PRV_PKID = Int32.Parse(ddlProvince.SelectedValue);
                    returnValue.TCK_Message = "CUSTOMER CODE " + txtTicketSubject.Text + " with name(s) " + txtReferenceNo.Text + " has requested password reset.\n\r " +   txtTicketMessage.Text.Trim();
                    if (Session["preferredEmail"] != null)
                    {
                        returnValue.TCK_AlternateEmail = Session["preferredEmail"].ToString();
                    }
                    else
                    {
                        returnValue.TCK_AlternateEmail = usrSession.USR_EmailAccount;
                    }
                    returnValue.CAT_RequireAttachment = cm.GetCategoryAttachmentRequirement(returnValue.CAT_PKID);
                    returnValue.STS_PKID = 1;
                    returnValue.TCT_PKID = 1;
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        private string GenerateTicketNumber(string tickID)
        {
            string returnValue = "";
            try
            {
                BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

                if (usrSession != null)
                {
                    returnValue = usrSession.USR_PKID.ToString();
                }
                else
                {
                    returnValue = GetUserIDFromTicket(tickID);
                }

                returnValue = returnValue + DateTime.Now.Year.ToString();
                returnValue = returnValue + DateTime.Now.Month.ToString("D2");
                returnValue = returnValue + "T";
                returnValue = returnValue + tickID;
            }
            catch (Exception ex)
            {
                //EmailDispatcherService emsWS = new EmailDispatcherService();;
                //EmailMessageObject emlObj = new EmailMessageObject();
                //emlObj = PopulateErrorEmailObject("GenerateTicketNumber", ex.Message, tickID);
                //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);
            }
            return returnValue;
        }

        private string GetUserIDFromTicket(string tckPKID)
        {
            DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject selObj = new BilletterieAPIWS.SelectStringResponseObject();
            selObj = bilAPIWS.GetBilletterieScalar("select USR_PKID from TB_TCK_Ticket where TCK_PKID = " + tckPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //selObj = da.GetBilletterieGenericIntScalar("select USR_PKID from TB_TCK_Ticket where TCK_PKID = " + tckPKID);
            string returnValue = selObj.selectedPKID;

            return returnValue;
        }

        private BilletterieAPIWS.EmailMessageObject PopulateEmailObject(BilletterieAPIWS.ticketObject tckObj)
        {
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();

            returnValue.EML_ToEmailAdmin = tckObj.TCK_AlternateEmail;
            returnValue.EML_ToEmailList = tckObj.TCK_AlternateEmail;
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ":" + tckObj.TCK_Subject;
            returnValue.EML_MailBody = GetConfirmationEmailBody(tckObj);
            returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = "1";
            returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
            returnValue.EML_KeyField = "TCK_PKID";
            returnValue.EML_KeyValue = tckObj.TCK_PKID.ToString();
            returnValue.EML_Domain = "0";
            returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];

            return returnValue;
        }

        private string GetConfirmationEmailBody(BilletterieAPIWS.ticketObject tckObj)
        {
            string returnValue = "";
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>Ticket # [ T" + tckObj.TCK_PKID + " ] has been successfully submitted.</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : " + tckObj.TCK_Subject + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/>Date :" + tckObj.TCK_DateCreated + "<br/><p>Dear " + ConfigurationManager.AppSettings["OrganisationName"] + " Client,<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Thank you for submitting your query using " + ConfigurationManager.AppSettings["OrganisationName"] + " " + ConfigurationManager.AppSettings["SystemTitle"] + " system.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><table style='margin-left:10px; border-collapse: collapse;'><tr style='border: none;'><td style='border-left:  solid 3px blue; min-height:30px; color: green;'><i>" + tckObj.TCK_Message.Replace("\n", "<br />") + "</i></td></tr></table><br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Note that any attached documents are ONLY accessible through the help desk system. Please quote this ticket number for any further correspondence regarding this query.<br /><br />This email serves as confirmation of receipt of query only. Another email will be sent upon resolving or progress update thereof.<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";

            return returnValue;
        }

        private void UpdateTicketDueDate(string ticketPKID)
        {
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select V.SVL_Hours, T.TCK_DateCreated from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_SVL_ServiceLevel V on C.SVL_PKID = V.SVL_PKID where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_SVL_ServiceLevel", "TB_SVL_ServiceLevelDS", "select V.SVL_Hours, T.TCK_DateCreated from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_SVL_ServiceLevel V on C.SVL_PKID = V.SVL_PKID where TCK_PKID = " + ticketPKID);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DateTime newCalculatedDate = DateTime.Parse(ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString()).AddHours(double.Parse(ds.Tables[0].Rows[0]["SVL_Hours"].ToString()));
                    string newDateString = "";
                    if (bool.Parse(ConfigurationManager.AppSettings["GetCalculatedDates"]))
                    {
                        //ServerServices svrWS = new ServerServices();
                        //svrWS.Url = ConfigurationManager.AppSettings["CUBAServerServiceURL"];

                        DataSet fbdDS = new DataSet();
                        fbdDS = bilAPIWS.GetCUBADataSet("select * from TB_FBD_ForbiddenDates where FBD_Year = " + DateTime.Now.Year.ToString() + " and FBD_ForbiddenDate between '" + DateTime.Parse(ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString()).ToString("yyyy-MM-dd") + "' and '" + newCalculatedDate.ToString("yyyy-MM-dd") + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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
                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TCK_DateDue = '" + newDateString + "' where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TCK_DateDue = '" + newDateString + "' where TCK_PKID = " + ticketPKID);
                    }
                    else
                    {
                        //newDateString = GetAllowedOfficeDate(newCalculatedDate.ToString("yyyy-MM-dd"));
                        newDateString = GetFormattedOfficeDate(newCalculatedDate.ToString("yyyy-MM-dd"));
                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TCK_DateDue = '" + newDateString + "' where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TCK_DateDue = '" + newDateString + "' where TCK_PKID = " + ticketPKID);

                    }
                }
            }
        }

        private BilletterieAPIWS.EmailMessageObject PopulateErrorEmailObject(string errorMethod, string errorMessage, string valueID)
        {
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();

            returnValue.EML_ToEmailAdmin = ConfigurationManager.AppSettings["FailureAddress"];
            returnValue.EML_ToEmailList = ConfigurationManager.AppSettings["FailureAddress"];
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_Subject = "Error in method: " + errorMethod;
            returnValue.EML_MailBody = "Dear System Administrator.<br /><br /> Billetterie has generated the following error. <br /><br /> Please urgently attend to it." + errorMessage;
            returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = "1";
            returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
            returnValue.EML_KeyField = "QRS_ERROR";
            returnValue.EML_KeyValue = valueID;
            returnValue.EML_Domain = "0";
            returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];

            return returnValue;
        }

        #endregion

        #region Control Events

        protected void btnNewCaptcha_Click(object sender, EventArgs e)
        {
            UpdateCaptchaText();
        }

        protected void lnkAttachFiles_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "str", "<script language='javascript' type='text/javascript'>document.getElementById(\"imageUploadRow\").style.visibility = \"visible\";</script>", false);
            }
            catch (Exception ex)
            {

            }

        }

        protected void lnkDeleteAttachedFiles_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "str", "<script language='javascript' type='text/javascript'>document.getElementById(\"imageUploadRow\").style.visibility = \"collapse\";</script>", false);
                fupAttachFile = new FileUpload();
            }
            catch (Exception)
            {

            }

        }

        private string ValidateTicketInput(BilletterieAPIWS.ticketObject tckObj)
        {
            Common cm = new Common();
            string returnValue = "";

            if (tckObj != null)
            {

                if (tckObj.CAT_PKID == 0)
                {
                    return returnValue = "Please select department/category.";
                }
                if (tckObj.USR_PKID == 0)
                {
                    return returnValue = "Please make sure you are logged in.";
                }
                if (tckObj.TCK_Subject == "")
                {
                    return returnValue = "Ticket subject is required.";
                }
                if (tckObj.TCK_Subject.Trim().Length > 150)
                {
                    return returnValue = "Ticket subject is too long. Max is 150.";
                }
                if (tckObj.TCK_Message == "")
                {
                    return returnValue = "Ticket message is required.";
                }

                if (tckObj.TCK_Message.Trim().Length > 2500)
                {
                    return returnValue = "Ticket message is too long. Max is 2500.";
                }

                if (tckObj.AttachedFile != null)
                {
                    if (!cm.ValidMimeType(tckObj.AttachedFile.MimeType, tckObj.AttachedFile.DCM_Extention))
                    {
                        return returnValue = "File type not supported.";
                    }
                    if (tckObj.AttachedFile.AttachmentSize > 10000000)
                    {
                        return returnValue = "File is too large.";
                    }
                }

                if (tckObj.CAT_RequireAttachment == true)
                {
                    if (tckObj.AttachedFile == null)
                    {
                        return returnValue = "You need to attach proof of transaction for this type of query.";
                    }
                }

                if (tckObj.PRV_PKID == 0)
                {
                    return returnValue = "Please select province.";
                }

            }
            else
            {
                return returnValue = "Ticket object is empty.";
            }
            return returnValue;
        }

        //protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlDepartment.SelectedValue != "" && ddlDepartment.SelectedValue != "0")
        //    {
        //        //Populate category depending on selected department
        //        bool displayCategory = false;
        //        displayCategory = PopulateCategoryDDL(ddlDepartment.SelectedValue);

        //        Session["SelectedCATPKID"] = ddlDepartment.SelectedValue;

        //        lblDDLCategory.Visible = displayCategory;
        //        ddlCategory.Visible = displayCategory;

        //        lblDDLSubCategory.Visible = false;
        //        ddlSubCategory.Visible = false;
        //    }
        //    else
        //    {
        //        lblDDLCategory.Visible = false;
        //        ddlCategory.Visible = false;

        //        lblDDLSubCategory.Visible = false;
        //        ddlSubCategory.Visible = false;
        //    }
        //}

        //protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlCategory.SelectedValue != "")
        //    {
        //        //Populate category depending on selected department
        //        bool displaySubCategory = false;
        //        displaySubCategory = PopulateSubCategoryDDL(ddlCategory.SelectedValue);

        //        Session["SelectedCATPKID"] = ddlCategory.SelectedValue;

        //        lblDDLSubCategory.Visible = displaySubCategory;
        //        ddlSubCategory.Visible = displaySubCategory;
        //    }
        //    else
        //    {
        //        lblDDLSubCategory.Visible = false;
        //        ddlSubCategory.Visible = false;
        //    }
        //}

        //protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlSubCategory.SelectedValue != "")
        //    {
        //        Session["SelectedCATPKID"] = ddlSubCategory.SelectedValue;
        //    }
        //}

        protected void btnSubmitTicket_Click(object sender, EventArgs e)
        {
            if (txtCaptchaText.Text.Trim() == Session["BillCaptcha"].ToString())
            {
                BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

                DataAccess da = new DataAccess();
                Common cm = new Common();
                string errMessage = "";
                BilletterieAPIWS.ticketObject tickObj = new BilletterieAPIWS.ticketObject();
                BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                errMessage = ValidateFileBeforeUpload();
                if (errMessage == "")
                {
                    tickObj = PopulateTicketObject();
                  errMessage = ValidateTicketInput(tickObj);
                    if (errMessage == "")
                    {
                        BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                        BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                        //Save ticket
                        insResp = bilAPIWS.InsertBilletterieTicketRecord(tickObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //insResp = bilAPIWS.CreateNewTicket(tickObj, usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //insResp = da.InsertBilletterieTicketRecord(tickObj);
                        if (!insResp.noError)
                        {
                            //Show error message
                            lblMainErrorMessage.Text = "[" + insResp.errorMessage + "]";
                        }
                        else
                        {
                            //Update ticket number
                            string insertedPKID = insResp.insertedPKID;
                            tickObj.TCK_PKID = Int32.Parse(insertedPKID);
                            tickObj.TCK_DateCreated = DateTime.Now.ToString();

                            if (tickObj.AttachedFile != null)
                            {
                                //Save document
                                insResp = bilAPIWS.InsertBilletterieDocumentRecord(tickObj.AttachedFile, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //insResp = da.InsertBilletterieDocumentRecord(tickObj.AttachedFile);
                                string destFile = tickObj.AttachedFile.DCM_DerivedName;
                                if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                                {
                                    destFile = cm.MoveDocuments(tickObj.AttachedFile.DCM_DocumentPath, insResp.insertedPKID.ToString());
                                }
                                updResp = bilAPIWS.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), destFile, insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //updResp = da.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), destFile, insertedPKID);
                            }

                            string ticketNumber = GenerateTicketNumber(insertedPKID);
                            updResp = bilAPIWS.UpdateBilletterieTicketRecord(insertedPKID, ticketNumber, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //updResp = da.UpdateBilletterieTicketRecord(insertedPKID, ticketNumber);

                            tickObj.TCK_TicketNumber = ticketNumber;

                            //Update if file has been attached from external
                            if (tickObj.TCK_HasFile == true)
                            {
                                bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TKR_HasFile = 1 where TCK_PKID = " + insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TKR_HasFile = 1 where TCK_PKID = " + insertedPKID);
                            }

                            #region Case form
                            if (bool.Parse(ConfigurationManager.AppSettings["GenerateCaseForm"]))
                            {
                                //BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

                                //string docExtention = ".pdf";
                                string docExtention = ".pdf";
                                //CaseDocument1 cd1 = new CaseDocument1();
                                CaseDocument1PDF cd1 = new CaseDocument1PDF();
                                cd1.CreatePackage(ConfigurationManager.AppSettings["LocalDocumentsContentPath"] + ticketNumber + docExtention, tickObj, usrSession);


                                BilletterieAPIWS.fileAttachmentObject caseDocObj = new BilletterieAPIWS.fileAttachmentObject();

                                caseDocObj.DCT_PKID = 4;
                                caseDocObj.DCS_PKID = 1;
                                caseDocObj.DCL_PKID = 1;
                                caseDocObj.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentsContentPath"] + ticketNumber + docExtention;
                                caseDocObj.DCM_OriginalName = ticketNumber + docExtention;
                                caseDocObj.DCM_DerivedName = ticketNumber + docExtention;
                                caseDocObj.DCM_Extention = docExtention;
                                caseDocObj.STS_PKID = 30;

                                insResp = bilAPIWS.InsertBilletterieDocumentRecord(caseDocObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //insResp = da.InsertBilletterieDocumentRecord(caseDocObj);

                                updResp = bilAPIWS.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), caseDocObj.DCM_DocumentPath, insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //updResp = da.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), caseDocObj.DCM_DocumentPath, insertedPKID);
                            }
                            #endregion
                            
                            if (!updResp.noError)
                            {
                                //Show error message
                                lblMainErrorMessage.Text = "[" + updResp.errorMessage + "]";
                            }
                            else
                            {
                                tickObj.TCK_TicketNumber = ticketNumber;

                                //Assign due date and auto assign if possible
                                if (bool.Parse(ConfigurationManager.AppSettings["AutoAssignDueDate"]))
                                {
                                    UpdateTicketDueDate(tickObj.TCK_PKID.ToString());
                                }

                                ticketInformationObject ticketInfo = new ticketInformationObject();
                                ticketInfo = (ticketInformationObject)Session["PassworsResetCookie"];
                                ticketInfo.TCK_PKID = insertedPKID;
                                //Add ticket information data


                                InsertTicketInformationRecord(ticketInfo);

                                //Send emails if configured
                                #region Send Mail
                                if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                                {
                                    ////Send email record
                                    //EmailDispatcherService emsWS = new EmailDispatcherService();
                                    //NewBilletterie.EmailWS.EmailMessageObject emlObj = new NewBilletterie.EmailWS.EmailMessageObject();
                                    //emlObj = PopulateEmailObject(tickObj);
                                    //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);

                                    if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                    {
                                        BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                        emlObj = PopulateEmailObject(tickObj);
                                        SendMail sm = new SendMail();
                                        SMTPMailResponseObject smtRespObj = new SMTPMailResponseObject();

                                        #region Normal sending of emails
                                        //smtRespObj = sm.SendSMTPMail(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer);
                                        //if (smtRespObj.noError)
                                        //{
                                        //    emlObj.EML_Status = "2";
                                        //}
                                        //else
                                        //{
                                        //    emlObj.EML_Status = "1";
                                        //}
                                        //EmailDispatcherService emsWS = new EmailDispatcherService();
                                        //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                        //opResp = emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);
                                        #endregion

                                        #region Asynchronous sending of email

                                        int threadId;
                                        // Create an instance of the test class.
                                        SendMail ad = new SendMail();
                                        // Create the delegate.
                                        //NewBilletterie.Classes.SendMail.AsyncMethodCaller caller = new NewBilletterie.Classes.SendMail.AsyncMethodCaller(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer,0,0 );
                                        NewBilletterie.Classes.SendMail.AsyncMethodCaller caller = new NewBilletterie.Classes.SendMail.AsyncMethodCaller(ad.SendSMTPMailAsync);
                                        // Initiate the asychronous call.
                                        IAsyncResult result = caller.BeginInvoke(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer, "TCK_PKID", tickObj.TCK_PKID.ToString(), "0", 5000, out threadId, null, null);
                                        Thread.Sleep(0);
                                        //Console.WriteLine("Main thread {0} does some work.",Thread.CurrentThread.ManagedThreadId);
                                        // Call EndInvoke to wait for the asynchronous call to complete,
                                        // and to retrieve the results.
                                        SMTPMailResponseObject returnValue = caller.EndInvoke(out threadId, result);
                                        //Console.WriteLine("The call executed on thread {0}, with return value \"{1}\".",threadId, returnValue);

                                        #endregion

                                    }
                                    else
                                    {
                                        //EmailDispatcherService emsWS = new EmailDispatcherService();
                                        //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];

                                        BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                        emlObj = PopulateEmailObject(tickObj);
                                        opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }

                                }
                                #endregion

                                lblMainErrorMessage.Text = "";
                                ModalPopupExtenderSuccess.Show();
                                lblSuccessHeading.Text = "Ticket successfully created: [T " + tickObj.TCK_PKID + "]";
                                lblTicketConfirmation.Text = "Thank you for contacting " + ConfigurationManager.AppSettings["OrganisationName"] + ". <br /><br /> Your query has been assigned ticket reference number [<b>[T " + tickObj.TCK_PKID + "]"+ "</b>]. <br /><br />A confirmation email will be sent to email account [<b>" + tickObj.TCK_AlternateEmail + "</b>] shortly. <br /><br />Please quote this ticket number for any further correspondence regarding this query.";
                            }
                        }
                    }
                    else
                    {
                        lblMainErrorMessage.Text = "[" + errMessage + "]";
                    }
                }
                else
                {
                    lblMainErrorMessage.Text = "[" + errMessage + "]";
                }
            }
            else
            {
                lblMainErrorMessage.Text = "[Invalid CAPTCHA Text]";
            }
        }

        private string InsertTicketInformationRecord(ticketInformationObject ticketObj)
        {
            string returnValue = "";
            DataAccess da = new DataAccess();

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_CustomerCode", ticketObj.CustomerCode, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_CustomerCode", ticketObj.CustomerCode, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_IdentityNumber", ticketObj.IdentityNumber, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_IdentityNumber", ticketObj.IdentityNumber, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_Names", ticketObj.Names, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_Names", ticketObj.Names, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_EmailAccount", ticketObj.EmailAccount, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_EmailAccount", ticketObj.EmailAccount, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhoneNumber", ticketObj.PhoneNumber, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhoneNumber", ticketObj.PhoneNumber, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_Province", ticketObj.Province, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_Province", ticketObj.Province, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyStreetAddress1", ticketObj.PhyStreetAddress1, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyStreetAddress1", ticketObj.PhyStreetAddress1, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyStreetAddress2", ticketObj.PhyStreetAddress2, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyStreetAddress2", ticketObj.PhyStreetAddress2, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyCityTown", ticketObj.PhyCityTown, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyCityTown", ticketObj.PhyCityTown, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyStateProvince", ticketObj.PhyStateProvince, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyStateProvince", ticketObj.PhyStateProvince, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyAreaCode", ticketObj.PhyAreaCode, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PhyAreaCode", ticketObj.PhyAreaCode, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosStreetAddress1", ticketObj.PosStreetAddress1, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosStreetAddress1", ticketObj.PosStreetAddress1, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosStreetAddress2", ticketObj.PosStreetAddress2, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosStreetAddress2", ticketObj.PosStreetAddress2, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosCityTown", ticketObj.PosCityTown, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosCityTown", ticketObj.PosCityTown, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosStateProvince", ticketObj.PosStateProvince, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosStateProvince", ticketObj.PosStateProvince, 1);

            bilAPIWS.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosAreaCode", ticketObj.PosAreaCode, 1, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //da.InsertTicketInformationRecord(ticketObj.TCK_PKID, "_PosAreaCode", ticketObj.PosAreaCode, 1);

            return returnValue;
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            Session["GlobalSession"] = null; 
            Session["userObjectCookie"] = null;
            Response.Redirect("~/Index.aspx", false);
        }

        #endregion

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


    }
}