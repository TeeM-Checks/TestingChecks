using NewBilletterie.Classes;
//using NewBilletterie.CUBAServerService;
//using NewBilletterie.EmailWS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;
using System.Text;

namespace NewBilletterie
{
    public partial class MobileNewTicket : System.Web.UI.Page
    {
        bool invalid = false;

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();
        Common cm = new Common();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnBack.Text = "View Tickets";
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
                LoadDropDowns();

                if (bool.Parse(ConfigurationManager.AppSettings["AllowDisableNewTickets"]))
                {
                    allowedDateResponse allwdDateResp = new allowedDateResponse();
                    allwdDateResp = NewTicketAllowed("New tickets cannot be created until ");
                    if (allwdDateResp.dateAllowed == false)
                    {
                        btnNext.Enabled = false;
                        btnNext.Font.Strikeout = true;
                        btnNext.ToolTip = allwdDateResp.displayMessage;
                    }
                    else
                    {
                        btnNext.Enabled = true;
                        btnNext.Font.Strikeout = false;
                        btnNext.ToolTip = "";
                    }
                }

            }
        }

        private allowedDateResponse NewTicketAllowed(string displayMessage)
        {
            DataAccess da = new DataAccess();
            allowedDateResponse allwdResponse = new allowedDateResponse();

            string newDateString = "";
            newDateString = GetAllowedOfficeDate(DateTime.Now.ToString("yyyy-MM-dd"));
            //newDateString = GetAllowedOfficeDate("2022-12-23");
            if (newDateString != "")
            {
                if (DateTime.Parse(newDateString).Date > DateTime.Now.Date)
                {
                    allwdResponse.dateAllowed = false;
                    allwdResponse.displayMessage = displayMessage + newDateString;
                }
                else
                {
                    allwdResponse.dateAllowed = true;
                    allwdResponse.displayMessage = displayMessage + newDateString;
                }
            }
            return allwdResponse;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (btnBack.Text != "View Tickets")
            {
                if (pnlMessage.Visible)
                {
                    pnlCategory.Visible = true;
                    pnlMessage.Visible = false;
                    pnlAttachment.Visible = false;
                    btnBack.Visible = true;
                    btnBack.Text = "View Tickets";
                    btnNext.Text = "Next >>";
                }

                if (pnlAttachment.Visible)
                {
                    pnlCategory.Visible = false;
                    pnlMessage.Visible = true;
                    pnlAttachment.Visible = false;
                    btnNext.Text = "Next >>";
                }
            }
            else
            {
                Response.Redirect("~/Mobile/MobileViewTickets.aspx", false);
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            lblMainErrorMessage.Text = "";

            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }

            if (pnlCategory.Visible)
            {
                if (GetSelectedCategory() != 0)
                {
                    btnBack.Text = "<< Back";
                    btnBack.Visible = true;
                    pnlCategory.Visible = false;
                    pnlAttachment.Visible = false;
                    pnlMessage.Visible = true;
                    lblMainErrorMessage.Text = "";
                }
                else
                {
                    lblMainErrorMessage.Text = "[ Please select department/category. ]";
                }
            }
            else if (pnlMessage.Visible)
            {
                if (cm.ValidateAntiXSSS(txtSubject.Text.Trim()) && cm.ValidateAntiXSSS(txtMessage.Text.Trim()))
                //if (errorMessage == "")
                {
                    if (ddlProvince.SelectedValue != "0")
                    {
                        if (txtSubject.Text.Trim() != "" && txtMessage.Text.Trim() != "")
                        {
                            btnBack.Text = "<< Back";
                            pnlCategory.Visible = false;
                            pnlMessage.Visible = false;
                            pnlAttachment.Visible = true;
                            btnNext.Text = "Submit";
                            lblMainErrorMessage.Text = "";
                        }
                        else
                        {
                            lblMainErrorMessage.Text = "[ Please populate both subject and message. ]";
                        }
                    }
                    else
                    {
                        lblMainErrorMessage.Text = "[ Please select province. ]";
                    }
                }
                else
                {
                    //Add message
                    lblMainErrorMessage.Text = "[Invalid ticket]";
                }
            }

            else if (pnlAttachment.Visible)
            {
                string errorMessage = cm.ValidateInputForXSS(txtCaptchaText.Text.Trim());
                if (errorMessage == "")
                {

                    if (txtCaptchaText.Text.Trim() == Session["BillCaptcha"].ToString())
                    {
                        #region Submit
                        DataAccess da = new DataAccess();
                        Common cm = new Common();

                        string errMessage = "";
                        BilletterieAPIWS.ticketObject tickObj = new BilletterieAPIWS.ticketObject();
                        errMessage = ValidateFileBeforeUpload();

                        if (fupAttachFile.HasFile)
                        {
                            MimeType mimeType = new MimeType();
                            string mimeTypeName = mimeType.GetMimeType(fupAttachFile.FileBytes, fupAttachFile.FileName);
                            if (!cm.ValidMimeType(mimeTypeName, Path.GetExtension(fupAttachFile.FileName)))
                            {
                                errMessage = "Attached document is invalid.";
                            }
                            else
                            {
                                string linksPDF = ValidatePDFFileLinks(fupAttachFile.FileBytes, Path.GetExtension(fupAttachFile.FileName));
                                if (linksPDF != "")
                                {
                                    errMessage = "Attached document is invalid.";
                                }
                            }
                        }


                        if (errMessage == "")
                        {
                            tickObj = PopulateTicketObject();
                            ticketValidationObject errorObject = new ticketValidationObject();
                            errorObject = ValidateInput(tickObj);
                            if (errorObject.errorMessage == "")
                            {
                                errMessage = cm.ValidateInputForXSS(tickObj);
                                if (errMessage == "")
                                {
                                    BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                                    BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
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

                                        //Update column 
                                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TCK_FromMobile = 1 where TCK_PKID = " + insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TCK_FromMobile = 1 where TCK_PKID = " + insertedPKID);

                                        //Update if file has been attached from external
                                        if (tickObj.TCK_HasFile == true)
                                        {
                                            bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TKR_HasFile = 1 where TCK_PKID = " + insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                            //da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TKR_HasFile = 1 where TCK_PKID = " + insertedPKID);
                                        }

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

                                                if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                                {
                                                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                                    emlObj = PopulateEmailObject(tickObj);
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
                                                    emlObj = PopulateEmailObject(tickObj);
                                                    opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                }

                                            }
                                            lblMainErrorMessage.Text = "";
                                        }
                                    }
                                    Response.Redirect("~/Mobile/MobileViewTickets.aspx", false);
                                }
                                else
                                {
                                    //Add message
                                    lblMainErrorMessage.Text = "[Invalid ticket]";
                                }
                            }
                            else
                            {
                                lblMainErrorMessage.Text = "[" + errorObject.errorMessage + "]";
                                if (errorObject.errorNumber == "0")
                                {
                                    Response.Redirect("~/MobileIndex.aspx", false);
                                }
                                if (errorObject.errorNumber == "1")
                                {
                                    pnlCategory.Visible = true;
                                    pnlAttachment.Visible = false;
                                    pnlMessage.Visible = false;
                                    btnBack.Text = "View Tickets";
                                    btnNext.Text = "Next >>";
                                }
                                if (errorObject.errorNumber == "2")
                                {
                                    pnlCategory.Visible = false;
                                    pnlAttachment.Visible = false;
                                    pnlMessage.Visible = true;
                                    btnBack.Text = "<< Back";
                                    btnNext.Text = "Next >>";
                                }
                            }
                        }
                        else
                        {
                            lblMainErrorMessage.Text = "[" + errMessage + "]";
                        }
                        #endregion
                    }
                    else
                    {
                        lblMainErrorMessage.Text = "[Invalid CAPTCHA Text]";
                    }
                }
                else
                {
                    lblMainErrorMessage.Text = "[Invalid CAPTCHA Text]";
                }

            }
        }

        private string ValidatePDFFileLinks(byte[] fileField, string fileExtention)
        {
            Common cm = new Common();
            DataAccess da = new Classes.DataAccess();
            string returnValue = "";
            try
            {

                string errorMessage = "";   // List<string> linkList = new List<string>();
                errorMessage = cm.GetPDFHyperLinks(fileField, fileExtention);
                //if (linkList != null)
                //{
                returnValue = errorMessage;  // "Invalid file type.";
                //}
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        //public string ValidateInputForXSS(BilletterieAPIWS.ticketObject tckObj)
        //{
        //    Common cm = new Common();
        //    string returnValue = "";

        //    if (tckObj != null)
        //    {

        //        //if (tckObj.CAT_PKID == 0)
        //        //{
        //        //    return returnValue = "Please select department/category.";
        //        //}
        //        //if (tckObj.USR_PKID == 0)
        //        //{
        //        //    return returnValue = "Please make sure you are logged in.";
        //        //}
        //        if (!ValidateAntiXSS(tckObj.TCK_Subject))
        //        {
        //            return returnValue = "Ticket subject contains illegal characters.";
        //        }
        //        //if (tckObj.TCK_Subject.Trim().Length > 150)
        //        //{
        //        //    return returnValue = "Ticket subject is too long. Max is 150.";
        //        //}
        //        if (!ValidateAntiXSS(tckObj.TCK_Message))
        //        {
        //            return returnValue = "Ticket message contains illegal characters.";
        //        }

        //        if (!ValidateAntiXSS(tckObj.TCK_Reference))
        //        {
        //            return returnValue = "Reference contains illegal characters.";
        //        }

        //        if (!ValidateAntiXSS(tckObj.TCK_AlternateEmail))
        //        {
        //            return returnValue = "Email contains illegal characters.";
        //        }

        //        if (!ValidateAntiXSS(tckObj.TCK_CaseIdentifier))
        //        {
        //            return returnValue = "Enterprise number contains illegal characters.";
        //        }

        //        //if (tckObj.TCK_Message.Trim().Length > 2500)
        //        //{
        //        //    return returnValue = "Ticket message is too long. Max is 2500.";
        //        //}

        //        //if (tckObj.AttachedFile != null)
        //        //{
        //        //    if (!cm.ValidMimeType(tckObj.AttachedFile.MimeType, tckObj.AttachedFile.DCM_Extention))
        //        //    {
        //        //        return returnValue = "File type not supported.";
        //        //    }
        //        //    if (tckObj.AttachedFile.AttachmentSize > 10000000)
        //        //    {
        //        //        return returnValue = "File is too large.";
        //        //    }
        //        //}

        //        //if (tckObj.CAT_RequireAttachment == true)
        //        //{
        //        //    if (tckObj.AttachedFile == null)
        //        //    {
        //        //        return returnValue = "You need to attach proof of transaction for this type of query.";
        //        //    }
        //        //}

        //        //if (tckObj.CAT_ForcedFieldID != null && tckObj.CAT_ForcedFieldID != "")
        //        //{
        //        //    if (tckObj.CAT_ForcedFieldID == "1")
        //        //    {
        //        //        if (tckObj.TCK_CaseIdentifier == "")
        //        //        {
        //        //            return returnValue = "You need to provide Enterprise Number for this type of query.";
        //        //        }
        //        //    }
        //        //}

        //        //if (tckObj.PRV_PKID == 0)
        //        //{
        //        //    return returnValue = "Please select province.";
        //        //}

        //    }
        //    else
        //    {
        //        return returnValue = "Ticket object is empty.";
        //    }
        //    return returnValue;
        //}

        //public static bool ValidateAntiXSS(string inputParameter)
        //{
        //    if (string.IsNullOrEmpty(inputParameter))
        //        return true;

        //    // Following regex convers all the js events and html tags mentioned in followng links.
        //    //https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet                 
        //    //https://msdn.microsoft.com/en-us/library/ff649310.aspx

        //    var pattren = new StringBuilder();

        //    //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.             
        //    pattren.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");

        //    //Checks any html tags i.e. <script, <embed, <object etc.
        //    pattren.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");

        //    return !Regex.IsMatch(System.Web.HttpUtility.UrlDecode(inputParameter), pattren.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //}

        private int GetSelectedCategory()
        {
            int returnValue = 0;

            if (ddlSubCategory.SelectedValue != "" && ddlSubCategory.Visible == true)
            {
                returnValue = Int32.Parse(ddlSubCategory.SelectedValue);
            }
            else if (ddlCategory.SelectedValue != "" && ddlCategory.Visible == true)
            {
                returnValue = Int32.Parse(ddlCategory.SelectedValue);
            }
            else if (ddlDepartment.SelectedValue != "" && ddlDepartment.Visible == true)
            {
                returnValue = Int32.Parse(ddlDepartment.SelectedValue);
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

            if (tckObj.AttachedFile != null)
            {
                string errorMessage = "";
                errorMessage = cm.GetPDFHyperLinks(tckObj.AttachedFile.DCM_FileField, tckObj.AttachedFile.DCM_Extention);
                if (errorMessage != "")
                {
                    returnValue.errorNumber = "3";
                    returnValue.errorMessage = errorMessage;
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

            //Populate selected category
            if (ddlSubCategory.SelectedValue != "")
            {
                returnValue.CAT_PKID = Int32.Parse(ddlSubCategory.SelectedValue);
            }
            else if (ddlCategory.SelectedValue != "")
            {
                returnValue.CAT_PKID = Int32.Parse(ddlCategory.SelectedValue);
            }
            else if (ddlDepartment.SelectedValue != "")
            {
                returnValue.CAT_PKID = Int32.Parse(ddlDepartment.SelectedValue);
            }

            returnValue.UST_PKID = 1;

            returnValue.PRV_PKID = Int32.Parse(ddlProvince.SelectedValue);

           // returnValue.PRV_PKID = 10;
            returnValue.OFC_PKID = 0;

            returnValue.TCK_TicketNumber = "";
            returnValue.TCK_Subject = txtSubject.Text.Trim();
            returnValue.TCK_Reference = txtReference.Text.Trim();
            returnValue.TCK_Message = txtMessage.Text.Trim();
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
            returnValue.TCK_FromMobile = true;
            return returnValue;
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDepartment.SelectedValue != "" && ddlDepartment.SelectedValue != "0")
            {
                //Populate category depending on selected department
                bool displayCategory = false;
                displayCategory = PopulateCategoryDDL(ddlDepartment.SelectedValue);

                lblCategory.Visible = displayCategory;
                ddlCategory.Visible = displayCategory;

                lblSubCategory.Visible = false;
                ddlSubCategory.Visible = false;
            }
            else
            {
                lblCategory.Visible = false;
                ddlCategory.Visible = false;

                lblSubCategory.Visible = false;
                ddlSubCategory.Visible = false;
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedValue != "")
            {
                //Populate category depending on selected department
                bool displaySubCategory = false;
                displaySubCategory = PopulateSubCategoryDDL(ddlCategory.SelectedValue);
                lblSubCategory.Visible = displaySubCategory;
                ddlSubCategory.Visible = displaySubCategory;
            }
            else
            {
                lblSubCategory.Visible = false;
                ddlSubCategory.Visible = false;
            }
        }

        protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSubCategory.SelectedValue != "")
            {

            }
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
                    ddlCategory.DataSource = dsCategory.Tables[0];
                    ddlCategory.DataTextField = "CAT_CategoryName";
                    ddlCategory.DataValueField = "CAT_PKID";
                    ddlCategory.DataBind();
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
                    ddlSubCategory.DataSource = dsSubCategory.Tables[0];
                    ddlSubCategory.DataTextField = "CAT_CategoryName";
                    ddlSubCategory.DataValueField = "CAT_PKID";
                    ddlSubCategory.DataBind();
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
                ddlDepartment.DataSource = dsDepartment.Tables[0];
                ddlDepartment.DataTextField = "CAT_CategoryName";
                ddlDepartment.DataValueField = "CAT_PKID";
                ddlDepartment.DataBind();
            }

            DataSet dsProvince = new DataSet();
            dsProvince = bilAPIWS.GetBilletterieDataSet("select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province from TB_PRV_Province order by PRV_PKID", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsProvince = da.GetGenericBilletterieDataSet("TB_PRV_Province", "TB_PRV_ProvinceDS", "select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province from TB_PRV_Province order by PRV_PKID");
            
            if (dsProvince != null)
            {
                ddlProvince.DataSource = dsProvince.Tables[0];
                ddlProvince.DataTextField = "PRV_Province";
                ddlProvince.DataValueField = "PRV_PKID";
                ddlProvince.DataBind();
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

            txtCaptchaText.Text = string.Empty;
            lblStatus.Visible = false;
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



        //public string ValidateInputForXSS(string inputStringValue)
        //{
        //    Common cm = new Common();
        //    string returnValue = "";

        //    //if (tckObj != null)
        //    //{

        //    if (!ValidateAntiXSS(inputStringValue))
        //    {
        //        return returnValue = "Ticket response contains illegal characters.";
        //    }
        //    if (!ValidateAntiXSSS(inputStringValue))
        //    {
        //        return returnValue = "Ticket response contains illegal characters.";
        //    }
        //    return returnValue;
        //}

        //public static bool ValidateAntiXSS(string inputParameter)
        //{
        //    if (string.IsNullOrEmpty(inputParameter))
        //        return true;

        //    // Following regex convers all the js events and html tags mentioned in followng links.
        //    //https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet                 
        //    //https://msdn.microsoft.com/en-us/library/ff649310.aspx

        //    var pattren = new StringBuilder();

        //    //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.             
        //    pattren.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");

        //    //Checks any html tags i.e. <script, <embed, <object etc.
        //    pattren.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");

        //    return !Regex.IsMatch(System.Web.HttpUtility.UrlDecode(inputParameter), pattren.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //}

        //public static bool ValidateAntiXSSS(string inputParameter)
        //{
        //    bool returnValue = true;

        //    if (string.IsNullOrEmpty(inputParameter))
        //        return true;

        //    if (inputParameter.Contains(">"))
        //    {
        //        returnValue = false;
        //    }

        //    if (inputParameter.Contains("<"))
        //    {
        //        returnValue = false;
        //    }

        //    if (inputParameter.Contains("="))
        //    {
        //        returnValue = false;
        //    }

        //    if (inputParameter.Contains("+"))
        //    {
        //        returnValue = false;
        //    }

        //    if (inputParameter.Contains("%"))
        //    {
        //        returnValue = false;
        //    }

        //    return returnValue;
        //}


    }
}