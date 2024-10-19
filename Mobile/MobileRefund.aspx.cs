using NewBilletterie.Classes;
//using NewBilletterie.CUBAServerService;
//using NewBilletterie.EmailWS;
//using NewBilletterie.IFX_WS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using NewBilletterie.CipherWS;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class MobileRefund : System.Web.UI.Page
    {
        bool invalid = false;
        public List<BilletterieAPIWS.fileAttachmentObject> attachmentList { get; set; }

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnSubmit.Text = "Submit";
                Session["DesktopViewLink"] = "../ExternalPages/NewTicket.aspx";
                UpdateCaptchaText();
                if (Session["userObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                }
                else
                {
                    Response.Redirect("~/MobileIndex.aspx", false);
                }
                lblWarningMessage.Text = ConfigurationManager.AppSettings["CBRSWarningMessage"];    // "CIPC has started a process to refund all balances in the CIPC's prepaid accounts to ligitimate clients. Clients will be required to provide bank account details of the account that was used to transfer funds to CIPC. Supporting documents are required for positive verification of the funds transfered. Failure to provide correct supporting documents will result in refund being rejected.";
                //lblWarningMessage.Text = "CIPC has started a process to refund all balances in the CIPC's prepaid accounts to ligitimate clients. Clients will be required to provide bank account details of the account that was used to transfer funds to CIPC. Supporting documents are required for positive verification of the funds transfered. Failure to provide correct supporting documents will result in refund being rejected.";
                LoadPopUpDropDowns();
            }
        }

        private int GetSelectedCategory()
        {
            int returnValue = 0;

            //if (ddlSubCategory.SelectedValue != "" && ddlSubCategory.Visible == true)
            //{
            //    returnValue = Int32.Parse(ddlSubCategory.SelectedValue);
            //}
            //else if (ddlCategory.SelectedValue != "" && ddlCategory.Visible == true)
            //{
            //    returnValue = Int32.Parse(ddlCategory.SelectedValue);
            //}
            //else if (ddlDepartment.SelectedValue != "" && ddlDepartment.Visible == true)
            //{
            //    returnValue = Int32.Parse(ddlDepartment.SelectedValue);
            //}

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
                //ServerServices emsWS = new ServerServices();
                //EmailMessageObject emlObj = new EmailMessageObject();
                //emlObj = PopulateErrorEmailObject("GenerateTicketNumber", ex.Message, tickID);
                //emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);
            }
            return returnValue;
        }

        private ticketValidationObject ValidateInput(BilletterieAPIWS.ticketObject tckObj)
        {
            Common cm = new Common();
            ticketValidationObject returnValue = new ticketValidationObject();
            returnValue.errorMessage = "";
            if (tckObj.CAT_PKID == 0)
            {
                returnValue.errorNumber = "1";
                returnValue.errorMessage = "Please select department/category.";
            }

            if (tckObj.PRV_PKID == 0)
            {
                returnValue.errorNumber = "2";
                returnValue.errorMessage = "Please select province.";
            }

            if (tckObj.USR_PKID == 0)
            {
                returnValue.errorNumber = "0";
                returnValue.errorMessage = "Please make sure you are logged in.";
            }

            if (tckObj.TCK_Subject == "")
            {
                returnValue.errorNumber = "2";
                returnValue.errorMessage = "Ticket subject is required.";
            }

            if (tckObj.TCK_Subject.Trim().Length > 150)
            {
                returnValue.errorNumber = "3";
                returnValue.errorMessage = "Ticket subject is too long. Max is 150.";
            }

            if (tckObj.TCK_Message == "")
            {
                returnValue.errorNumber = "2";
                returnValue.errorMessage = "Ticket message is required.";
            }

            if (tckObj.TCK_Message.Trim().Length > 2500)
            {
                returnValue.errorNumber = "3";
                returnValue.errorMessage = "Ticket message is too long. Max is 2500.";
            }

            if (tckObj.AttachedFile != null)
            {
                if (!cm.ValidMimeType(tckObj.AttachedFile.MimeType, tckObj.AttachedFile.DCM_Extention))
                {
                    returnValue.errorNumber = "3";
                    returnValue.errorMessage = "File type not supported.";
                }
                if (tckObj.AttachedFile.AttachmentSize > 10000000)
                {
                    returnValue.errorNumber = "3";
                    returnValue.errorMessage = "File is too large.";
                }
            }

            if (tckObj.CAT_RequireAttachment == true)
            {
                if (tckObj.AttachedFile == null)
                {
                    returnValue.errorNumber = "3";
                    returnValue.errorMessage = "You need to attach proof of transaction for this type of query.";
                }
            }

            if (IsValidEmail(tckObj.TCK_AlternateEmail) == false)
            {
                returnValue.errorNumber = "0";
                returnValue.errorMessage = "Email account " + tckObj.TCK_AlternateEmail + " is invalid.";
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

            //Populate ticket user ID
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            returnValue.USR_PKID = usrSession.USR_PKID;

            #region Populate file attachments
            Common cm = new Common();
            cm.CleanUpTempFiles(returnValue.USR_PKID);

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

                string filePath = atchObj.DCM_DerivedName;  // ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + atchObj.DCM_DerivedName; //Server.MapPath("~/Temp/" + atchObj.DCM_DerivedName);
                if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                {
                    filePath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + atchObj.DCM_DerivedName; //Server.MapPath("~/Temp/" + atchObj.DCM_DerivedName);
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

            ////Populate selected category
            //if (ddlSubCategory.SelectedValue != "")
            //{
            //    returnValue.CAT_PKID = Int32.Parse(ddlSubCategory.SelectedValue);
            //}
            //else if (ddlCategory.SelectedValue != "")
            //{
            //    returnValue.CAT_PKID = Int32.Parse(ddlCategory.SelectedValue);
            //}
            //else if (ddlDepartment.SelectedValue != "")
            //{
            //    returnValue.CAT_PKID = Int32.Parse(ddlDepartment.SelectedValue);
            //}

            //returnValue.UST_PKID = 1;

            //returnValue.PRV_PKID = Int32.Parse(ddlProvince.SelectedValue);

            //// returnValue.PRV_PKID = 10;
            //returnValue.OFC_PKID = 0;

            //returnValue.TCK_TicketNumber = "";
            //returnValue.TCK_Subject = txtSubject.Text.Trim();
            //returnValue.TCK_Reference = txtReference.Text.Trim();
            //returnValue.TCK_Message = txtMessage.Text.Trim();
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
            return returnValue;
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ddlDepartment.SelectedValue != "" && ddlDepartment.SelectedValue != "0")
            //{
            //    //Populate category depending on selected department
            //    bool displayCategory = false;
            //    displayCategory = PopulateCategoryDDL(ddlDepartment.SelectedValue);

            //    lblCategory.Visible = displayCategory;
            //    ddlCategory.Visible = displayCategory;

            //    lblSubCategory.Visible = false;
            //    ddlSubCategory.Visible = false;
            //}
            //else
            //{
            //    lblCategory.Visible = false;
            //    ddlCategory.Visible = false;

            //    lblSubCategory.Visible = false;
            //    ddlSubCategory.Visible = false;
            //}
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ddlCategory.SelectedValue != "")
            //{
            //    //Populate category depending on selected department
            //    bool displaySubCategory = false;
            //    displaySubCategory = PopulateSubCategoryDDL(ddlCategory.SelectedValue);
            //    lblSubCategory.Visible = displaySubCategory;
            //    ddlSubCategory.Visible = displaySubCategory;
            //}
            //else
            //{
            //    lblSubCategory.Visible = false;
            //    ddlSubCategory.Visible = false;
            //}
        }

        protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ddlSubCategory.SelectedValue != "")
            //{

            //}
        }

        private bool PopulateCategoryDDL(string masterPKID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            DataSet dsCategory = new DataSet();
            dsCategory = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 order by CAT_ShortName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsCategory = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 order by CAT_ShortName asc");
            if (dsCategory != null)
            {
                if (dsCategory.Tables[0].Rows.Count > 1)
                {
                    //ddlCategory.DataSource = dsCategory.Tables[0];
                    //ddlCategory.DataTextField = "CAT_CategoryName";
                    //ddlCategory.DataValueField = "CAT_PKID";
                    //ddlCategory.DataBind();
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

        private bool PopulateSubCategoryDDL(string masterPKID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            DataSet dsSubCategory = new DataSet();
            dsSubCategory = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 and CTL_PKID = 3 order by CAT_Order asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsSubCategory = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 and CTL_PKID = 3 order by CAT_Order asc");
            if (dsSubCategory != null)
            {
                if (dsSubCategory.Tables[0].Rows.Count > 1)
                {
                    //ddlSubCategory.DataSource = dsSubCategory.Tables[0];
                    //ddlSubCategory.DataTextField = "CAT_CategoryName";
                    //ddlSubCategory.DataValueField = "CAT_PKID";
                    //ddlSubCategory.DataBind();
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

        private void LoadDropDowns()
        {
            DataAccess da = new DataAccess();
            DataSet dsDepartment = new DataSet();
            dsDepartment = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = 0 and STS_PKID = 50 and CAT_Visible = 1 order by CAT_ShortName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsDepartment = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = 0 and STS_PKID = 50 and CAT_Visible = 1 order by CAT_ShortName asc");
            if (dsDepartment != null)
            {
                //ddlDepartment.DataSource = dsDepartment.Tables[0];
                //ddlDepartment.DataTextField = "CAT_CategoryName";
                //ddlDepartment.DataValueField = "CAT_PKID";
                //ddlDepartment.DataBind();
            }

            DataSet dsProvince = new DataSet();
            dsProvince = bilAPIWS.GetBilletterieDataSet("select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province from TB_PRV_Province order by PRV_PKID", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsProvince = da.GetGenericBilletterieDataSet("TB_PRV_Province", "TB_PRV_ProvinceDS", "select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province from TB_PRV_Province order by PRV_PKID");
            if (dsProvince != null)
            {
                //ddlProvince.DataSource = dsProvince.Tables[0];
                //ddlProvince.DataTextField = "PRV_Province";
                //ddlProvince.DataValueField = "PRV_PKID";
                //ddlProvince.DataBind();
            }


            //Common cm = new Common();
            //SelectStringResponseObject selResp = new SelectStringResponseObject();
            //selResp = da.GetBilletterieGenericScalar("select top 1 (select ', ' + AMT_Extention  from TB_AMT_AllowedMimeType T ORDER BY T.AMT_PKID FOR XML PATH('')) [Escalation List]  from TB_AMT_AllowedMimeType where STS_PKID = 60");
            //lblAllowedExtentions.Text = "Allowed extentions " + cm.CleanUpValues(selResp.selectedPKID);
            //if (Session["userObjectCookie"] != null)
            //{
            //    userProfileObject usrSession = new userProfileObject();
            //    usrSession = (userProfileObject)Session["userObjectCookie"];

            //    DataSet dsUserGroup = new DataSet();
            //    dsUserGroup = da.GetGenericBilletterieDataSet("TB_USG_UserGroup", "TB_USG_UserGroupDS", "select USG_PKID, USG_UserGroupName from TB_USG_UserGroup where USG_PKID = " + usrSession.USG_PKID);
            //    if (dsUserGroup != null)
            //    {
            //        ddlUserGroup.DataSource = dsUserGroup.Tables[0];
            //        ddlUserGroup.DataTextField = "USG_UserGroupName";
            //        ddlUserGroup.DataValueField = "USG_PKID";
            //        ddlUserGroup.DataBind();
            //    }

            //    ddlUserGroup.SelectedIndex = 0;
            //}
        }

        protected void btnNewCaptcha_Click(object sender, EventArgs e)
        {
            UpdateCaptchaText();
        }

        private void UpdateCaptchaText()
        {

            //txtCaptchaText.Text = string.Empty;
            //lblStatus.Visible = false;
            Session["BillCaptcha"] = Guid.NewGuid().ToString().Substring(0, 6);
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

        public bool IsValidEmail(string strIn)
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

        private void ResetMobileInputFormat()
        {
            string hex1 = "#FFF";
            System.Drawing.Color _color1 = System.Drawing.ColorTranslator.FromHtml(hex1);

            //string hex2 = "#ccff99";
            //System.Drawing.Color _color2 = System.Drawing.ColorTranslator.FromHtml(hex2);

            txtIDNumber.BorderColor = _color1;
            txtFullName.BorderColor = _color1;
            txtContactNumber.BorderColor = _color1;
            ddlBankName.BorderColor = _color1;
            ddlBranchCode.BorderColor = _color1;
            txtAccountHolderName.BorderColor = _color1;
            txtAccountNumber.BorderColor = _color1;
            fupAttachFile.BorderColor = _color1;

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            Common cm = new Classes.Common();

            bool documentIsValid = true;

            lblMainErrorMessage.Text = "";
            lblMainErrorMessage.Visible = false;

            lblErrorMessage.Text = "";
            lblErrorMessage.Visible = false;

            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }

            ResetMobileInputFormat();

            #region Populate document

            if (fupAttachFile.HasFile)
            {
                string errMessage = "";
                MimeType mimeType = new MimeType();
                string mimeTypeName = mimeType.GetMimeType(fupAttachFile.FileBytes, fupAttachFile.FileName);

                if (!cm.ValidCBRSMimeType(mimeTypeName, Path.GetExtension(fupAttachFile.FileName)))
                {
                    documentIsValid = false;
                    errMessage = "Attached document is invalid.";
                    lblErrorMessage.Text = errMessage;
                    lblErrorMessage.Visible = true;
                }

                if (fupAttachFile.FileBytes.Length > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
                {
                    documentIsValid = false;
                    errMessage = "File '" + Path.GetFileName(fupAttachFile.FileName) + "' is too large. Max file size is 8MB.";
                    lblErrorMessage.Text = "File '" + Path.GetFileName(fupAttachFile.FileName) + "' is too large. Max file size is 8MB.";
                    lblErrorMessage.Visible = true;
                }

                if (errMessage == "")
                {
                    lblErrorMessage.Text = "";
                    lblErrorMessage.Visible = false;

                    attachmentList = new List<BilletterieAPIWS.fileAttachmentObject>();
                    BilletterieAPIWS.fileAttachmentObject fupObject = new BilletterieAPIWS.fileAttachmentObject();

                    fupObject.DCT_PKID = 5;
                    fupObject.DCS_PKID = 1;
                    fupObject.DCL_PKID = 1;
                    fupObject.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + fupAttachFile.FileName;
                    fupObject.DCM_OriginalName = fupAttachFile.FileName;
                    fupObject.DCM_DerivedName = "doc" + Path.GetFileNameWithoutExtension(fupAttachFile.FileName);
                    fupObject.DCM_Extention = Path.GetExtension(fupAttachFile.FileName);
                    fupObject.STS_PKID = 30;
                    fupObject.AttachmentSize = fupAttachFile.FileBytes.Length;
                    fupObject.MimeType = fupAttachFile.PostedFile.ContentType;
                    string filePath = ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + fupAttachFile.FileName;
                    if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                    {
                        fupAttachFile.SaveAs(filePath);
                    }

                    BilletterieAPIWS.InsertResponseObject appResp = new BilletterieAPIWS.InsertResponseObject();
                    appResp = bilAPIWS.AddTempCBRSDocument(fupObject, usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //appResp = da.AddTempCBRSDocument(fupObject, usrSession.USR_PKID.ToString(), "1");
                }
                else
                {
                    lblErrorMessage.Text = errMessage;
                    lblErrorMessage.Visible = true;
                }
            }
            #endregion

            if (documentIsValid)
            {
                //IPCUBAERMSBillingWS ipERMS = new IPCUBAERMSBillingWS();
                //ipERMS.Url = ConfigurationManager.AppSettings["IFX_WSURL"];
                ERMSAgentProfileObject ermsAgentObj = new ERMSAgentProfileObject();
                ERMSAgentProfileResponse ermsAgentResp = new ERMSAgentProfileResponse();
                ermsAgentResp = bilAPIWS.GetERMSAgentProfile(usrSession.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                BilletterieAPIWS.cbrsCustomerDetailObject custDetailObj = new BilletterieAPIWS.cbrsCustomerDetailObject();
                custDetailObj = PopulateCustomerBankDetails(ermsAgentResp.ermsAgentProfileObject.sAgentCode, ermsAgentResp.ermsAgentProfileObject.sAgentBalance, usrSession.USR_PKID, ermsAgentResp.ermsAgentProfileObject.sAgentStatus);
                string errorMessage = ValidateCustomerDetails(custDetailObj, ermsAgentResp.ermsAgentProfileObject.sAgentName, ermsAgentResp.ermsAgentProfileObject.sAgentIDNumber);
                if (errorMessage == "")
                {
                    BilletterieAPIWS.InsertResponseObject custIns = new BilletterieAPIWS.InsertResponseObject();
                    custIns = bilAPIWS.InsertCustomerDetailRecord(custDetailObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //custIns = da.InsertCustomerDetailRecord(custDetailObj);

                    custDetailObj.CUS_PKID = Int32.Parse(custIns.insertedPKID);

                    if (custDetailObj.AttachedFiles != null)
                    {
                        BilletterieAPIWS.InsertResponseObject docInsResp = new BilletterieAPIWS.InsertResponseObject();
                        BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();

                        //loop to get all documents
                        //Save document
                        for (int i = 0; i < custDetailObj.AttachedFiles.Length; i++)
                        {
                            docInsResp = bilAPIWS.InsertCBRSDocumentRecord(custDetailObj.AttachedFiles[i], ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //docInsResp = da.InsertCBRSDocumentRecord(custDetailObj.AttachedFiles[i]);
                            string destFile = custDetailObj.AttachedFiles[i].DCM_DerivedName;
                            if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                            {
                                destFile = cm.MoveCBRSDocuments(custDetailObj.AttachedFiles[i].DCM_DocumentPath, docInsResp.insertedPKID.ToString());
                            }
                            updResp = bilAPIWS.UpdateCBRSDocumentRecord(docInsResp.insertedPKID.ToString(), destFile, custIns.insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //updResp = da.UpdateCBRSDocumentRecord(docInsResp.insertedPKID.ToString(), destFile, custIns.insertedPKID);
                        }

                        string emailList = "";
                        emailList = usrSession.USR_EmailAccount;
                        BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                        if (bool.Parse(ConfigurationManager.AppSettings["SendRefundEmail"]))
                        {
                            if (emailList.Trim() != "")
                            {
                                //EmailDispatcherService emsWS = new EmailDispatcherService();
                                //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                                BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                emlObj = PopulateNewRefundEmailObject(emailList, custDetailObj);
                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                        }

                        //Create ticket that correspond with refund request
                        if (bool.Parse(ConfigurationManager.AppSettings["CreateRefundTicket"]))
                        {
                            //Create new ticket
                            BilletterieAPIWS.ticketObject cbrsTicketObj = new BilletterieAPIWS.ticketObject();
                            cbrsTicketObj = PopulateNewCBRSTicketObject(custDetailObj, custIns.insertedPKID);
                            string tckPKID = CreateBankRefundTicket(cbrsTicketObj);

                            //Update CBRS with ticket number
                            BilletterieAPIWS.UpdateResponseObject upddResp = new BilletterieAPIWS.UpdateResponseObject();
                            upddResp = bilAPIWS.UpdateCBRSCustomerRecordTicket(custIns.insertedPKID, tckPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //upddResp = da.UpdateCBRSCustomerRecordTicket(custIns.insertedPKID, tckPKID);
                        }


                        Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
                    }
                    else
                    {
                        lblMainErrorMessage.Text = "Please attach required documents";
                        lblMainErrorMessage.Visible = true;

                        lblErrorMessage.Text = "Please attach required documents";
                        lblErrorMessage.Visible = true;
                    }

                }
                else
                {
                    lblMainErrorMessage.Text = errorMessage;
                    lblMainErrorMessage.Visible = true;

                    lblErrorMessage.Text = errorMessage;
                    lblErrorMessage.Visible = true;
                }
            }
            else
            {
                lblMainErrorMessage.Text = "Error attaching documents.";
                lblMainErrorMessage.Visible = true;
            }
        }

        private BilletterieAPIWS.EmailMessageObject PopulateNewRefundEmailObject(string emailList, BilletterieAPIWS.cbrsCustomerDetailObject refundObj)
        {
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            returnValue.EML_ToEmailAdmin = emailList.Trim();   // tckObj.TCK_AlternateEmail;
            returnValue.EML_ToEmailList = emailList.Trim();    // tckObj.TCK_AlternateEmail;
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_Subject = ConfigurationManager.AppSettings["RefundSubject"] + ":" + refundObj.CUS_AgentCode;
            returnValue.EML_MailBody = GetNewRefundConfirmationEmailBody(refundObj);
            returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = "1";
            returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
            returnValue.EML_KeyField = "CUS_PKID";
            returnValue.EML_KeyValue = refundObj.CUS_PKID.ToString();
            returnValue.EML_Domain = "0";
            returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];

            return returnValue;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Mobile/MobileMainMenu.aspx", false);
        }

        protected void btnSaveUploadedCustFile_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
                lblErrorMessage.Visible = true;
            }
        }

        private string CreateBankRefundTicket(BilletterieAPIWS.ticketObject tickObj)
        {
            string retValue = "";

            #region Captcha is valid

            DataAccess da = new DataAccess();
            string errMessage = "";
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();

            #region Upload document if user did not click Upload button COMMENTS
            ////Upload documents
            //if (GridViewUploadedDocs.Rows.Count <= 0 && fupAttachFile.HasFile)
            //{
            //    attachmentList = new List<fileAttachmentObject>();

            //    if (Session["userObjectCookie"] != null)
            //    {
            //        usrSession = (userProfileObject)Session["userObjectCookie"];
            //    }

            //    errMessage = ValidateFileBeforeUpload(usrSession.USR_PKID.ToString());
            //    if (errMessage == "")
            //    {
            //        fileAttachmentObject fupObject = new fileAttachmentObject();

            //        fupObject.DCT_PKID = 1;
            //        fupObject.DCS_PKID = 1;
            //        fupObject.DCL_PKID = 1;
            //        fupObject.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + fupAttachFile.FileName;
            //        fupObject.DCM_OriginalName = fupAttachFile.FileName;
            //        fupObject.DCM_DerivedName = "doc" + Path.GetExtension(fupAttachFile.FileName);
            //        fupObject.DCM_Extention = Path.GetExtension(fupAttachFile.FileName);
            //        fupObject.STS_PKID = 30;
            //        fupObject.AttachmentSize = fupAttachFile.FileBytes.Length;
            //        fupObject.MimeType = fupAttachFile.PostedFile.ContentType;

            //        fupAttachFile.PostedFile.SaveAs(ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + fupAttachFile.FileName);

            //        NewBilletterie.Classes.InsertResponseObject appResp = new NewBilletterie.Classes.InsertResponseObject();
            //        appResp = da.AddTempDocument(fupObject, usrSession.USR_PKID.ToString(), "1");

            //        DataSet ds = new DataSet();
            //        ds = da.GetTemporaryDocuments(usrSession.USR_PKID.ToString(), "1");
            //        if (ds != null)
            //        {
            //            if (ds.Tables.Count > 0)
            //            {
            //                GridViewUploadedDocs.DataSource = ds.Tables[0];
            //                GridViewUploadedDocs.DataBind();
            //            }
            //        }

            //    }
            //}
            #endregion

            Common cm = new Common();
            BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();

            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }

            errMessage = cm.ValidateInput(tickObj);
            if (errMessage == "")
            {
                errMessage = ValidateInputForXSS(tickObj);
                if (errMessage == "")
                {
                    BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                    BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                    //Save ticket
                    insResp = bilAPIWS.InsertBilletterieTicketRecord(tickObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //insResp = bilAPIWS.CreateNewTicket(tickObj, usrSession.USR_PKID.ToString(), "1",   ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //insResp = da.InsertBilletterieTicketRecord(tickObj);
                    //Update ticket number
                    string insertedPKID = insResp.insertedPKID;
                    tickObj.TCK_PKID = Int32.Parse(insertedPKID);
                    tickObj.TCK_DateCreated = DateTime.Now.ToString();

                    retValue = insertedPKID;

                    string ticketNumber = GenerateTicketNumber(insertedPKID);
                    updResp = bilAPIWS.UpdateBilletterieTicketRecord(insertedPKID, ticketNumber, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //updResp = da.UpdateBilletterieTicketRecord(insertedPKID, ticketNumber);
                    tickObj.TCK_TicketNumber = ticketNumber;

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

                        //Send emails if configured
                        if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                        {
                            //Send ticket creation email
                            bool categoryNotifies = CheckCategoryNotifiesEmail(tickObj.CAT_PKID);
                            string emailList = "";
                            if (categoryNotifies)
                            {

                                emailList = cm.GetCategoryMembersList(tickObj.CAT_PKID.ToString());

                                if (emailList.Trim() != "")
                                {
                                    //EmailDispatcherService emsWS = new EmailDispatcherService();
                                    //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];

                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                    emlObj = PopulateNewTicketEmailObject(emailList, tickObj);
                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                }
                            }


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
                    }
                }
            }
            #endregion

            return retValue;
        }

        private BilletterieAPIWS.EmailMessageObject PopulateNewTicketEmailObject(string emailList, BilletterieAPIWS.ticketObject tckObj)
        {
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            returnValue.EML_ToEmailAdmin = emailList.Trim();   // tckObj.TCK_AlternateEmail;
            returnValue.EML_ToEmailList = emailList.Trim();    // tckObj.TCK_AlternateEmail;
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ":" + tckObj.TCK_Subject;
            returnValue.EML_MailBody = GetNewTicketConfirmationEmailBody(tckObj);
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

        private string GetNewTicketConfirmationEmailBody(BilletterieAPIWS.ticketObject tckObj)
        {
            string returnValue = "";
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>New ticket # [ T" + tckObj.TCK_PKID + " ] has been received.</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject : " + tckObj.TCK_Subject + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/>Date :" + tckObj.TCK_DateCreated + "<br/><p>Dear " + ConfigurationManager.AppSettings["OrganisationName"] + " Official,<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>A new ticket has been created using " + ConfigurationManager.AppSettings["SystemTitle"] + " system; your attention is required.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><table style='margin-left:10px; border-collapse: collapse;'><tr style='border: none;'><td style='border-left:  solid 3px blue; min-height:30px; color: green;'><i>" + tckObj.TCK_Message.Replace("\n", "<br />") + "</i></td></tr></table><br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>If you wish not to receive this notification email please contact your system administrator.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Support Team<br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
            return returnValue;
        }

        private bool CheckCategoryNotifiesEmail(int catPKID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject stndResponObj = new BilletterieAPIWS.SelectStringResponseObject();
            stndResponObj = bilAPIWS.GetBilletterieScalar("select CAT_NotifyEmail from TB_CAT_Category where CAT_PKID = " + catPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //stndResponObj = da.GetBilletterieGenericScalar("select CAT_NotifyEmail from TB_CAT_Category where CAT_PKID = " + catPKID);
            if (stndResponObj.selectedPKID == "1" || stndResponObj.selectedPKID.ToLower() == "true")
            {
                returnValue = true;
            }
            return returnValue;
        }

        private BilletterieAPIWS.ticketObject PopulateNewCBRSTicketObject(BilletterieAPIWS.cbrsCustomerDetailObject custDetailObj, string custInsInsertedPKID)
        {
            BilletterieAPIWS.ticketObject returnValue = new BilletterieAPIWS.ticketObject();
            DataAccess da = new Classes.DataAccess();
            try
            {
                if (Session["userObjectCookie"] != null)
                {
                    //Populate ticket user ID
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    returnValue.USR_PKID = usrSession.USR_PKID;

                    returnValue.UST_PKID = 1;

                    #region Populate file attachments COMMENTS

                    Common cm = new Common();
                    //cm.CleanUpTempFiles(returnValue.USR_PKID);

                    //fileAttachmentObject singleObj = new fileAttachmentObject();
                    //fileAttachmentObject atchObj = new fileAttachmentObject();

                    //ArrayList appAr = new ArrayList();

                    //if (GridViewUploadedDocs.Rows.Count > 0)
                    //{

                    //    DataSet ds = new DataSet();
                    //    ds = da.GetTemporaryDocuments(usrSession.USR_PKID.ToString(), "1");
                    //    if (ds != null)
                    //    {
                    //        if (ds.Tables.Count > 0)
                    //        {
                    //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //            {
                    //                //select TPD_PKID, USR_PKID, UST_PKID, DCT_PKID, TPD_FileSize, TPD_FileDescription, TPD_FileExtension, TPD_FileTypeCode, TPD_FileField, TPD_FileURL, TPD_OriginalFileName, TPD_DerivedFileName from TB_TPD_TemporaryDocument where USR_PKID = " + userPKID + " and UST_PKID = " + userType + " order by TPD_PKID desc
                    //                atchObj.DCM_OriginalName = ds.Tables[0].Rows[i]["TPD_OriginalFileName"].ToString();
                    //                atchObj.AttachmentSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
                    //                atchObj.MimeType = ds.Tables[0].Rows[i]["TPD_MimeType"].ToString();
                    //                atchObj.DCM_Extention = ds.Tables[0].Rows[i]["TPD_FileExtension"].ToString();
                    //                atchObj.DCM_DerivedName = "doc" + returnValue.USR_PKID.ToString() + "_" + i.ToString() + "_" + Path.GetExtension(ds.Tables[0].Rows[i]["TPD_FileExtension"].ToString());
                    //                atchObj.DCT_PKID = 1;
                    //                atchObj.DCS_PKID = 1;
                    //                atchObj.DCL_PKID = 1;
                    //                //fupAttachFile.PostedFile.SaveAs(ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + fupAttachFile.FileName);
                    //                //atchObj.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + atchObj.DCM_DerivedName;
                    //                atchObj.DCM_DocumentPath = ds.Tables[0].Rows[i]["TPD_FileURL"].ToString();
                    //                atchObj.STS_PKID = 30;

                    //                //string filePath = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + atchObj.DCM_DerivedName;
                    //                //fupAttachFile.SaveAs(filePath);

                    //                returnValue.AttachedFile = atchObj;
                    //                returnValue.TCK_HasFile = true;
                    //                appAr.Add(atchObj);
                    //                atchObj = new fileAttachmentObject();
                    //            }

                    //            returnValue.AttachedFiles = new fileAttachmentObject[appAr.Count];
                    //            appAr.CopyTo(returnValue.AttachedFiles);
                    //            appAr.Clear();
                    //        }
                    //    }
                    //}
                    #endregion

                    returnValue.CAT_PKID = Int32.Parse(ConfigurationManager.AppSettings["FinanceCBRSCAT_PKID"]);
                    returnValue.TCK_IsLog = false;
                    returnValue.OFC_PKID = 0;
                    returnValue.TCK_TicketNumber = "";
                    returnValue.TCK_Subject = ConfigurationManager.AppSettings["RefundSubject"];  // txtTicketSubject.Text.Trim();
                    returnValue.TCK_Reference = "REFUND" + custInsInsertedPKID;    // txtReferenceNo.Text.Trim();
                    returnValue.TCK_CaseIdentifier = "";    // txtEnterpriseNo.Text.Trim();

                    returnValue.CAT_ForcedFieldID = cm.GetCategoryForcedFieldID(returnValue.CAT_PKID);

                    returnValue.TCK_Message = ConfigurationManager.AppSettings["CBRSDefaultTicket"] + custDetailObj.CUS_AgentBalance;   // txtTicketMessage.Text.Trim();
                    if (Session["preferredEmail"] != null)
                    {
                        returnValue.TCK_AlternateEmail = Session["preferredEmail"].ToString();
                    }
                    else
                    {
                        returnValue.TCK_AlternateEmail = usrSession.USR_EmailAccount;
                    }
                    returnValue.CAT_RequireAttachment = false;
                    returnValue.STS_PKID = 1;
                    returnValue.PRV_PKID = 10;  //Other province 
                    returnValue.TCT_PKID = 4;
                    returnValue.TCK_FromMobile = true;
                    return returnValue;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        private string ValidateBankingFileUpload(string documentType)
        {

            return "";
        }

        private string GetNewRefundConfirmationEmailBody(BilletterieAPIWS.cbrsCustomerDetailObject refundObj)
        {
            string returnValue = "";
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>Refund Request # [ " + refundObj.CUS_AgentCode + " ] has been received.</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject request for CIPC Account : " + refundObj.CUS_AgentCode + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/>Date :" + refundObj.CUS_DateCreated + "<br/><p>Dear " + refundObj.CUS_FullName + "  ,<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>A refund request has been created using " + ConfigurationManager.AppSettings["SystemTitle"] + " system; your attention is required.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><table style='margin-left:10px; border-collapse: collapse;'><tr style='border: none;'><td style='border-left:  solid 3px blue; min-height:30px; color: green;'><i>  A refund of " + refundObj.CUS_AgentBalance + " has been requested from CIPC. The approved amount will be paid to bank account " + refundObj.CUS_BankName + " - " + refundObj.CUS_AccountNumber + " </i></td></tr></table><br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Approval of refund is subject to verification of all submitted documents.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Finance Division <br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
            return returnValue;
        }

        private BilletterieAPIWS.cbrsCustomerDetailObject PopulateCustomerBankDetails(string agentCode, string accountBalance, int USRPKID, string ERSMStatus)
        {
            Common cm = new Common();
            DataAccess da = new Classes.DataAccess();
            //EncryptDecrypt cipherWS = new EncryptDecrypt();

            BilletterieAPIWS.cbrsCustomerDetailObject retValue = new BilletterieAPIWS.cbrsCustomerDetailObject();
            retValue.CUS_PKID = 0;
            retValue.BKN_PKID = Int32.Parse(ddlBankName.SelectedValue);
            retValue.USR_PKID = USRPKID;
            retValue.CTS_PKID = 1;
            retValue.CUS_AgentCode = agentCode;
            retValue.CUS_AgentBalance = accountBalance.Replace(",",".");
            retValue.CUS_IDNumber = bilAPIWS.GetEncryptedValue(txtIDNumber.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //retValue.CUS_IDNumber = cipherWS.EncryptMethod("e38effb95631439d2affd2db94cc7053", txtIDNumber.Text);
            retValue.CUS_FullName = txtFullName.Text;

            if (ddlBankName.SelectedItem != null)
            {
                retValue.CUS_BankName = ddlBankName.SelectedItem.Text;
            }
            if (ddlBranchCode.SelectedItem != null)
            {
                retValue.CUS_BankBranchCode = ddlBranchCode.SelectedItem.Text;
            }
            retValue.CUS_AccountHolderName = txtFullName.Text;
            retValue.CUS_AccountNumber = bilAPIWS.GetEncryptedValue(txtAccountNumber.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //retValue.CUS_AccountNumber = cipherWS.EncryptMethod("e38effb95631439d2affd2db94cc7053", txtAccountNumber.Text);

            retValue.CUS_ContactNumber = bilAPIWS.GetEncryptedValue(txtContactNumber.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //retValue.CUS_ContactNumber = cipherWS.EncryptMethod("e38effb95631439d2affd2db94cc7053", txtContactNumber.Text);

            retValue.CUS_ERMSStatus = ERSMStatus;
            retValue.CUS_DateCreated = DateTime.Now.ToString();

            string lastDepositDate = GetLastDepositDate(agentCode);

            retValue.CUS_IsOldBalance = IsOldDepositDate(lastDepositDate);

            retValue.CUS_FromMobile = true;
            retValue.CUS_IsEncrypted = true;

            #region Populate file attachments
            if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
            {
                cm.CleanUpTempFiles(USRPKID);
            }
            BilletterieAPIWS.fileAttachmentObject atchObj = new BilletterieAPIWS.fileAttachmentObject();
            ArrayList appAr = new ArrayList();

            if (fupAttachFile.HasFile)
            {
                if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                {
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetTemporaryCBRSDocuments(USRPKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetTemporaryCBRSDocuments(USRPKID.ToString(), "1");
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                atchObj.DCM_OriginalName = ds.Tables[0].Rows[i]["TPD_OriginalFileName"].ToString();
                                atchObj.AttachmentSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
                                atchObj.MimeType = ds.Tables[0].Rows[i]["TPD_MimeType"].ToString();
                                atchObj.DCM_Extention = ds.Tables[0].Rows[i]["TPD_FileExtension"].ToString();
                                atchObj.DCM_DerivedName = "doc" + USRPKID.ToString() + "_" + i.ToString() + "_" + Path.GetExtension(ds.Tables[0].Rows[i]["TPD_FileExtension"].ToString());
                                atchObj.DCT_PKID = Int32.Parse(ds.Tables[0].Rows[i]["DCT_PKID"].ToString());
                                atchObj.DCS_PKID = 1;
                                atchObj.DCL_PKID = 1;
                                atchObj.DCM_DocumentPath = ds.Tables[0].Rows[i]["TPD_FileURL"].ToString();
                                atchObj.STS_PKID = 30;
                                atchObj.DCM_FileField = (byte[])ds.Tables[0].Rows[i]["DCM_FileField"];
                                appAr.Add(atchObj);
                                atchObj = new BilletterieAPIWS.fileAttachmentObject();
                            }
                            retValue.AttachedFiles = new BilletterieAPIWS.fileAttachmentObject[appAr.Count];
                            appAr.CopyTo(retValue.AttachedFiles);
                            appAr.Clear();
                        }
                    }
                }
            }
            #endregion

            return retValue;
        }

        public string ValidateInputForXSS(BilletterieAPIWS.ticketObject tckObj)
        {
            Common cm = new Common();
            string returnValue = "";

            if (tckObj != null)
            {

                if (!ValidateAntiXSS(tckObj.TCK_Subject))
                {
                    return returnValue = "Ticket subject contains illegal characters.";
                }

                if (!ValidateAntiXSS(tckObj.TCK_Message))
                {
                    return returnValue = "Ticket message contains illegal characters.";
                }

                if (!ValidateAntiXSS(tckObj.TCK_Reference))
                {
                    return returnValue = "Reference contains illegal characters.";
                }

                if (!ValidateAntiXSS(tckObj.TCK_Reference))
                {
                    return returnValue = "Reference contains illegal characters.";
                }

                if (!ValidateAntiXSS(tckObj.TCK_CaseIdentifier))
                {
                    return returnValue = "Enterprise number contains illegal characters.";
                }

            }
            else
            {
                return returnValue = "Ticket object is empty.";
            }
            return returnValue;
        }

        public static bool ValidateAntiXSS(string inputParameter)
        {
            if (string.IsNullOrEmpty(inputParameter))
                return true;

            // Following regex convers all the js events and html tags mentioned in followng links.
            //https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet                 
            //https://msdn.microsoft.com/en-us/library/ff649310.aspx

            var pattren = new StringBuilder();

            //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.             
            pattren.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");

            //Checks any html tags i.e. <script, <embed, <object etc.
            pattren.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");

            return !Regex.IsMatch(System.Web.HttpUtility.UrlDecode(inputParameter), pattren.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
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

        private bool IsOldDepositDate(string lastDepositDate)
        {
            bool retValue = false;
            if (lastDepositDate != "")
            {
                if ((DateTime.Now - DateTime.Parse(lastDepositDate)).Days >= Int32.Parse(ConfigurationManager.AppSettings["CBRSRelaxedDays"]))
                {
                    retValue = true;
                }
            }
            return retValue;
        }

        private void LoadPopUpDropDowns()
        {
            DataAccess da = new DataAccess();
            DataSet dsBankNames = new DataSet();
            dsBankNames = bilAPIWS.GetCBRSDataSet("select 0 [BKN_PKID], '' [BKN_Name], '' [BKN_Description] union select BKN_PKID, BKN_Name, BKN_Description from TB_BKN_BankName order by BKN_Name asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsBankNames = da.GetGenericCBRSDataSet("TB_BKN_BankName", "TB_BKN_BankNameDS", "select 0 [BKN_PKID], '' [BKN_Name], '' [BKN_Description] union select BKN_PKID, BKN_Name, BKN_Description from TB_BKN_BankName order by BKN_Name asc");
            if (dsBankNames != null)
            {
                ddlBankName.DataSource = dsBankNames.Tables[0];
                ddlBankName.DataTextField = "BKN_Name";
                ddlBankName.DataValueField = "BKN_PKID";
                ddlBankName.DataBind();
            }
        }

        private void LoadPopUpDropDowns(string BKN_PKID)
        {
            DataAccess da = new DataAccess();
            DataSet dsBranchCode = new DataSet();
            dsBranchCode = bilAPIWS.GetCBRSDataSet("select BKB_PKID, BKN_PKID, BKB_BranchCode, BKB_SwiftCode, BKB_BrachDescription from TB_BKB_BankBranchCode where BKN_PKID = " + BKN_PKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsBranchCode = da.GetGenericCBRSDataSet("TB_BKB_BankBranchCode", "TB_BKB_BankBranchCodeDS", "select BKB_PKID, BKN_PKID, BKB_BranchCode, BKB_SwiftCode, BKB_BrachDescription from TB_BKB_BankBranchCode where BKN_PKID = " + BKN_PKID);
            if (dsBranchCode != null)
            {
                ddlBranchCode.DataSource = dsBranchCode.Tables[0];
                ddlBranchCode.DataTextField = "BKB_BranchCode";
                ddlBranchCode.DataValueField = "BKB_PKID";
                //ddlBranchCode.ToolTip = "BKB_BrachDescription";
                ddlBranchCode.DataBind();

            }
        }

        protected void ddlBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPopUpDropDowns(ddlBankName.SelectedValue);

            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }

            //DataAccess da = new DataAccess();
            //DataSet ds = new DataSet();
            //ds = da.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1");
            //if (ds != null)
            //{
            //    if (ds.Tables.Count > 0)
            //    {
            //        GridViewCustUploadedDocs.DataSource = ds.Tables[0];
            //        GridViewCustUploadedDocs.DataBind();
            //    }
            //}

            //ModalPopupExtenderCustomerDetail.Show();
            //txtAccountHolder.Focus();
        }

        private bool UserAlreadySubmitted(string userName)
        {
            DataAccess da = new DataAccess();
            bool returnValue = false;
            if (bilAPIWS.BankingDetailsExist(userName, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]))
            //if (da.BankingDetailsExist(userName))
            {
                returnValue = true;
            }
            return returnValue;
        }

        private string ValidateCustomerDetails(BilletterieAPIWS.cbrsCustomerDetailObject cbrsBankObj, string fullName, string idNumber)
        {
            //string retValue = "";
            if (cbrsBankObj.BKN_PKID == 0)
            {
                ddlBankName.BorderColor = System.Drawing.Color.Red;
                return "Bank name not selected.";
            }

            else if (cbrsBankObj.USR_PKID == 0)
            {
                ddlBankName.BorderColor = System.Drawing.Color.Red;
                return "Bank name not selected.";
            }

            else if (cbrsBankObj.CUS_BankName == "")
            {
                ddlBankName.BorderColor = System.Drawing.Color.Red;
                return "Bank not selected.";
            }

            else if (cbrsBankObj.CUS_BankBranchCode == "")
            {
                ddlBranchCode.BorderColor = System.Drawing.Color.Red;
                return "Bank branch code not selected.";
            }

            else if (cbrsBankObj.CUS_AccountHolderName == "")
            {
                txtAccountHolderName.BorderColor = System.Drawing.Color.Red;
                return "Account holder name not provided.";
            }

            else if (cbrsBankObj.CUS_AccountNumber == "")
            {
                txtAccountNumber.BorderColor = System.Drawing.Color.Red;
                return "Account number not provided.";
            }

            else if (cbrsBankObj.CUS_ContactNumber == "")
            {
                txtContactNumber.BorderColor = System.Drawing.Color.Red;
                return "Contact number not provided.";
            }

            else if (cbrsBankObj.CUS_IDNumber == "")
            {
                txtIDNumber.BorderColor = System.Drawing.Color.Red;
                return "Identity number not provided.";
            }

            else if (cbrsBankObj.CUS_FullName == "")
            {
                txtFullName.BorderColor = System.Drawing.Color.Red;
                return "Full names not provided.";
            }

            else if (cbrsBankObj.AttachedFiles == null)
            {
                fupAttachFile.BorderColor = System.Drawing.Color.Red;
                return "Please upload supporting document.";
            }

            else if (cbrsBankObj.CUS_FullName.ToUpper() != fullName.ToUpper())
            {
                txtFullName.BorderColor = System.Drawing.Color.Red;
                return "Full names are not matching with the CIPC profile.";
            }

            else if (cbrsBankObj.CUS_IDNumber.ToUpper() != idNumber.ToUpper())
            {
                txtIDNumber.BorderColor = System.Drawing.Color.Red;
                return "ID Number not matching with the CIPC profile.";
            }
            else if (UserAlreadySubmitted(cbrsBankObj.CUS_AgentCode.ToUpper().Trim()) == true)
            {
                return "You have already submitted a refund.";
            }
            else
            {
                return "";
            }
        }


    }
}