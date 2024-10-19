using AjaxControlToolkit;
using NewBilletterie.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security.AntiXss;
using System.Globalization;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class NewTicket : System.Web.UI.Page
    {

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();
        public List<BilletterieAPIWS.fileAttachmentObject> fileUploadList { get; set; }
        public List<BilletterieAPIWS.fileAttachmentObject> attachmentList { get; set; }
        Common cm = new Common();

        List<RecordAttachments> ListTicketPDFAttachments { get; set; }

        /// <summary>
        /// Page to create new ticket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["AttachmentList"] = null;
                Session["SubCategoryVisible"] = false;
                Session["CategoryVisible"] = false;

                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));

                btnSubmitTicket.BackColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonBackColor"]);
                btnSubmitTicket.ForeColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonForeColor"]);

                btnRequestRefund.BackColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonBackColor"]);
                btnRequestRefund.ForeColor = (Color)converter.ConvertFromInvariantString(ConfigurationManager.AppSettings["ButtonForeColor"]);

                Session["SelectedCATPKID"] = "0";
                Session["MobileViewLink"] = "~/Mobile/MobileNewTicket.aspx";

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
                BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                if (Session["userObjectCookie"] != null)
                {
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    //Check if user is coming from Password Reset session
                    if (usrSession.OFL_PKID == 3)
                    {
                        Session["GlobalSession"] = null;
                        Session["userObjectCookie"] = null;
                        Response.Redirect("~/Index.aspx", false);
                    }
                }
                else
                {
                    Response.Redirect("~/Index.aspx", false);
                }

                LoadDropDowns();

                if (bool.Parse(ConfigurationManager.AppSettings["AllowDisableNewTickets"]))
                {
                    allowedDateResponse allwdDateResp = new allowedDateResponse();
                    allwdDateResp = NewTicketAllowed("New tickets cannot be created until ");
                    if (allwdDateResp.dateAllowed == false)
                    {
                        btnSubmitTicket.Enabled = false;
                        btnSubmitTicket.Font.Strikeout = true;
                        btnSubmitTicket.ToolTip = allwdDateResp.displayMessage;
                    }
                    else
                    {
                        btnSubmitTicket.Enabled = true;
                        btnSubmitTicket.Font.Strikeout = false;
                        btnSubmitTicket.ToolTip = "";
                    }
                }


                BilletterieAPIWS.UpdateResponseObject apResp = new BilletterieAPIWS.UpdateResponseObject();
                apResp = bilAPIWS.DeleteAllTempDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                GridViewUploadedDocs.DataSource = null;
                GridViewUploadedDocs.DataBind();

                if (bool.Parse(ConfigurationManager.AppSettings["ShowUserGroup"]))
                {
                    lblUserGroup.Visible = true;
                    ddlUserGroup.Visible = true;
                }

                //Display popup for banking details
                if (usrSession.USC_PKID == 1)
                {
                    if (bool.Parse(ConfigurationManager.AppSettings["RequireBankDetails"]))
                    {
                        if (UserIsNotRestricted(usrSession.USR_UserLogin.ToUpper().Trim()))
                        {
                            if (!UserAlreadySubmitted(usrSession.USR_UserLogin.ToUpper().Trim()))
                            {
                                ERMSAgentProfileObject ermsAgentObj = new ERMSAgentProfileObject();
                                ERMSAgentProfileResponse ermsAgentResp = new ERMSAgentProfileResponse();

                                //Check if the customer code exist
                                if (bilAPIWS.GetAgentAccount(usrSession.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]))
                                {
                                    #region Bank Details
                                    if (Session["CustomerBankDetailDisplayed"] != null && bool.Parse(Session["CustomerBankDetailDisplayed"].ToString()) == false)
                                    {
                                        //Check if user is coming from Password Reset session
                                        if (usrSession.USC_PKID == 1 || usrSession.USC_PKID == 3)
                                        {
                                            BilletterieAPIWS.UpdateResponseObject apCustResp = new BilletterieAPIWS.UpdateResponseObject();
                                            apCustResp = bilAPIWS.DeleteAllCBRSTempDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                            GridViewCustUploadedDocs.DataSource = null;
                                            GridViewCustUploadedDocs.DataBind();

                                            ermsAgentResp = bilAPIWS.GetERMSAgentProfile(usrSession.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                            DoubleObjectResponse balObj = new DoubleObjectResponse();
                                            balObj = bilAPIWS.GetAgentBalance(usrSession.USR_UserLogin.ToUpper().Trim(), 0, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                            LoadPopUpDropDowns();

                                            txtAccountBalance.Text = ermsAgentResp.ermsAgentProfileObject.sAgentBalance.Replace(',', '.');

                                            if (double.Parse(txtAccountBalance.Text.Trim(), CultureInfo.InvariantCulture) >= double.Parse(ConfigurationManager.AppSettings["MinRefundAmount"].ToString(), CultureInfo.InvariantCulture))
                                            {
                                                lblWarningMessage.Text = ConfigurationManager.AppSettings["CBRSWarningMessage"];    // "CIPC has started a process to refund all balances in the CIPC's prepaid accounts to ligitimate clients. Clients will be required to provide bank account details of the account that was used to transfer funds to CIPC. Supporting documents are required for positive verification of the funds transfered. Failure to provide correct supporting documents will result in refund being rejected.";
                                                lblCustomerDetailHeading.Text = ConfigurationManager.AppSettings["CBRSCustomerDetailHeading"];
                                                lblBottomMessage.Text = ConfigurationManager.AppSettings["CBRSBottomMessage"];  // "NOTE: Intellectual Property business users have an option to ignore this refund requirement as the prepaid system will temporarily remain in place for the purpose of Filing, Renewal and Searching of Copyright, Designs, Patents and Trade Marks";

                                                lblERMSPKID.Text = ermsAgentResp.ermsAgentProfileObject.sAgentStatus;
                                                txtAgentAccount.Text = usrSession.USR_UserLogin;
                                                ModalPopupExtenderCustomerDetail.Show();
                                            }
                                            else
                                            {
                                                btnRequestRefund.Visible = false;
                                            }

                                            string auditErrorMessage = ValidateAccountForAudit(usrSession.USR_UserLogin.ToUpper().Trim());
                                            if (auditErrorMessage != "")
                                            {
                                                lblPopErrorMessage.Text = auditErrorMessage;
                                                lblPopErrorMessage.Visible = true;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                btnRequestRefund.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        btnRequestRefund.Visible = false;
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

        private string ValidateAccountForAudit(string agentAccount)
        {
            string returnValue = "";
            try
            {
                InformixDataSetResponse inforMixDS = new InformixDataSetResponse();
                inforMixDS = bilAPIWS.GetInformixDataSet("select first " + ConfigurationManager.AppSettings["ERMSTopLimit"] + " trans_id, agent_code, trans_date, trans_type_id, trak_no, ent_no, form_code, trans_reference, service_rend_code, trans_desc, original_amount, open_amount, on_hold, trans_status_id, effective_date, closing_balance from agent_trans where trans_type_id = '2' and service_rend_code = '330' and agent_code = '" + agentAccount + "' order by trans_id desc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                if (inforMixDS.noError == true)
                {
                    if (inforMixDS.ermsDataSetObject != null)
                    {
                        if (inforMixDS.ermsDataSetObject.Tables.Count > 0)
                        {
                            if (inforMixDS.ermsDataSetObject.Tables[0].Rows.Count > 0)
                            {
                                returnValue = "Customer Account has been selected for audit. All neccesary supporting documents will be required to proceed with this refunf request.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

        protected void OnUploadComplete(object sender, AjaxFileUploadEventArgs e)
        {
           
        }

        #region Custom Methods

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
            BilletterieAPIWS.SelectBoolResponseObject respValue = new BilletterieAPIWS.SelectBoolResponseObject();
            bool returnValue = false;
            respValue = bilAPIWS.ClientBankingDetailsExist(userName, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (respValue.selectedPKID && respValue.noError)
            {
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }
            return returnValue;
        }

        private ERMSCustomerObject GetERMSDetailObject(string AgentAccount)
        {
            ERMSCustomerObject retValue = new Classes.ERMSCustomerObject();
            return retValue;
        } 

        private void UpdateCaptchaText()
        {

            txtCaptchaText.Text = string.Empty;
            lblStatus.Visible = false;
            Session["BillCaptcha"] = Guid.NewGuid().ToString().Substring(0, 6);
        }

        private void LoadDropDowns()
        {
            DataSet dsDepartment = new DataSet();
            dsDepartment = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName from TB_CAT_Category where CAT_MasterID = 0 and STS_PKID = 50 and CAT_Visible = 1   union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = 0 and STS_PKID = 50 and CAT_Visible = 1 order by CAT_ShortName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsDepartment != null)
            {
                ddlDepartment.DataSource = dsDepartment.Tables[0];
                ddlDepartment.DataTextField = "CAT_ShortName";
                ddlDepartment.DataValueField = "CAT_PKID";
                ddlDepartment.DataBind();
            }

            DataSet dsProvince = new DataSet();
            dsProvince = bilAPIWS.GetBilletterieDataSet("select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province  from TB_PRV_Province", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsProvince != null)
            {
                ddlProvince.DataSource = dsProvince.Tables[0];
                ddlProvince.DataTextField = "PRV_Province";
                ddlProvince.DataValueField = "PRV_PKID";
                ddlProvince.DataBind();
            }

            Common cm = new Common();
            BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
            selResp = bilAPIWS.GetBilletterieScalar("select top 1 (select ', ' + AMT_Extention  from TB_AMT_AllowedMimeType T WHERE T.STS_PKID = 60 ORDER BY T.AMT_PKID FOR XML PATH('')) [Escalation List]  from TB_AMT_AllowedMimeType where STS_PKID = 60", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            lblAllowedExtentions.Text = "Allowed extentions " + cm.CleanUpValues(selResp.selectedPKID);
            lblSizeLimit.Text = "   Total allowed file size [" +  (Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]) / 1048576).ToString() + " Mb.]";

            if (Session["userObjectCookie"] != null)
            {
                BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];

                DataSet dsUserGroup = new DataSet();
                dsUserGroup = bilAPIWS.GetBilletterieDataSet("select USG_PKID, USG_UserGroupName from TB_USG_UserGroup where USG_PKID = " + usrSession.USG_PKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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

        private void LoadPopUpDropDowns()
        {
            DataSet dsBankNames = new DataSet();
            dsBankNames = bilAPIWS.GetCBRSDataSet("select 0 [BKN_PKID], '' [BKN_Name], '' [BKN_Description] union select BKN_PKID, BKN_Name, BKN_Description from TB_BKN_BankName order by BKN_Name asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsBankNames != null)
            {
                ddlBankName.DataSource = dsBankNames.Tables[0];
                ddlBankName.DataTextField = "BKN_Name";
                ddlBankName.DataValueField = "BKN_PKID";
                ddlBankName.DataBind();
            }

            DataSet dsCustFileType = new DataSet();
            dsCustFileType = bilAPIWS.GetCBRSDataSet( "select 0 [DCT_PKID], '' [DCT_DocumentType], '' [DCT_TypeDescription] union select DCT_PKID, DCT_DocumentType, DCT_TypeDescription from TB_DCT_DocumentType order by DCT_DocumentType asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsCustFileType != null)
            {
                ddlCustFileType.DataSource = dsCustFileType.Tables[0];
                ddlCustFileType.DataTextField = "DCT_DocumentType";
                ddlCustFileType.DataValueField = "DCT_PKID";
                ddlCustFileType.DataBind();
            }

        }

        private void LoadPopUpDropDowns(string BKN_PKID)
        {
            DataSet dsBranchCode = new DataSet();
            dsBranchCode = bilAPIWS.GetCBRSDataSet("select BKB_PKID, BKN_PKID, BKB_BranchCode, BKB_SwiftCode, BKB_BrachDescription from TB_BKB_BankBranchCode where BKN_PKID = " + BKN_PKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsBranchCode != null)
            {
                ddlBranchCode.DataSource = dsBranchCode.Tables[0];
                ddlBranchCode.DataTextField = "BKB_BranchCode";
                ddlBranchCode.DataValueField = "BKB_PKID";
                ddlBranchCode.DataBind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterPKID"></param>
        /// <returns></returns>
        private bool PopulateCategoryDDL(string masterPKID)
        {
            bool returnValue = false;
            DataSet dsCategory = new DataSet();
            dsCategory = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 and CAT_Visible = 1 order by CAT_ShortName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsCategory != null)
            {
                if (dsCategory.Tables[0].Rows.Count > 1)
                {
                    ddlCategory.DataSource = dsCategory.Tables[0];
                    ddlCategory.DataTextField = "CAT_ShortName";
                    ddlCategory.DataValueField = "CAT_PKID";
                    ddlCategory.DataBind();
                    returnValue = true;
                    Session["CategoryVisible"] = true;
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
            DataSet dsSubCategory = new DataSet();
            dsSubCategory = bilAPIWS.GetBilletterieDataSet("select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = " + masterPKID + " and STS_PKID = 50 and CAT_Visible = 1 and CTL_PKID = 3 order by CAT_ShortName asc", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (dsSubCategory != null)
            {
                if (dsSubCategory.Tables[0].Rows.Count > 1)
                {
                    ddlSubCategory.DataSource = dsSubCategory.Tables[0];
                    ddlSubCategory.DataTextField = "CAT_ShortName";
                    ddlSubCategory.DataValueField = "CAT_PKID";
                    ddlSubCategory.DataBind();
                    returnValue = true;
                    Session["SubCategoryVisible"] = true;
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

        private string GetDefaultPriority(string catPKID)
        {
            string returnValue = "";
            BilletterieAPIWS.SelectStringResponseObject resp = new BilletterieAPIWS.SelectStringResponseObject();
            resp = bilAPIWS.GetBilletterieScalar("select TPT_PKID from TB_CAT_Category where CAT_PKID = " + catPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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

        //private string ValidateFileBeforeUpload(string uploadMimeType, string uploadFileName, int uploadFileSize)
        //{
        //    Common cm = new Common();
        //    string returnValue = "";
        //    try
        //    {
        //        if (!cm.ValidMimeType(uploadMimeType, Path.GetExtension(uploadFileName)))
        //        {
        //            returnValue = "File type not supported.";
        //        }

        //        if (!cm.ValidFileSize(uploadFileSize))
        //        {
        //            returnValue = "File is too large.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue = ex.Message;
        //    }
        //    return returnValue;
        //}

        private string ValidateFilesBeforeUpload(string userPKID)
        {
            Common cm = new Common();
            DataAccess da = new Classes.DataAccess();
            string returnValue = "";
            try
            {
                if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                {
                    //DataSet ds = new DataSet();
                    //ds = da.GetTemporaryDocuments(userPKID, "1");
                    //int documentCount = 0;
                    //int totalFileSize = 0;

                    //if (ds != null)
                    //{
                    //    if (ds.Tables.Count > 0)
                    //    {
                    //        documentCount = ds.Tables[0].Rows.Count;
                    //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //        {
                    //            totalFileSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
                    //        }
                    //    }
                    //}

                    //if (documentCount > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadFiles"]))
                    //{
                    //    return returnValue = "Number of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadFiles"];
                    //}

                    //if (totalFileSize > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
                    //{
                    //    return returnValue = "Total size of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadSize"] + " MB";
                    //}
                }

            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        private string ValidateFileBeforeUpload(string userPKID, string fileExtention)
        {
            Common cm = new Common();
            DataAccess da = new Classes.DataAccess();
            string returnValue = "";
            try
            {

                if (!cm.ValidMimeType(fupAttachFile.PostedFile.ContentType, fileExtention))
                {
                    return returnValue = "File type '" + Path.GetExtension(fupAttachFile.FileName) + "' is not supported.";
                }

                if (fupAttachFile.FileBytes.Length > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
                {
                    return returnValue = "File '" + Path.GetFileName(fupAttachFile.FileName) + "' is too large. Max file size is 8MB.";
                }

                if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                {
                    #region Comments
                    //DataSet ds = new DataSet();
                    //ds = da.GetTemporaryDocuments(userPKID, "1");
                    //int documentCount = 0;
                    //int totalFileSize = 0;

                    //if (ds != null)
                    //{
                    //    if (ds.Tables.Count > 0)
                    //    {
                    //        documentCount = ds.Tables[0].Rows.Count;
                    //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //        {
                    //            totalFileSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
                    //        }
                    //    }
                    //}

                    //if (documentCount > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadFiles"]))
                    //{
                    //    return returnValue = "Number of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadFiles"];
                    //}

                    //if (totalFileSize > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
                    //{
                    //    return returnValue = "Total size of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadSize"] + " MB";
                    //}
                    #endregion
                }
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
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


        //private string ValidateCustFilesBeforeUpload(string userPKID)
        //{
        //    Common cm = new Common();
        //    DataAccess da = new Classes.DataAccess();
        //    string returnValue = "";
        //    try
        //    {
        //        //DataSet ds = new DataSet();
        //        //ds = da.GetTemporaryCBRSDocuments(userPKID, "1");
        //        //int documentCount = 0;
        //        //int totalFileSize = 0;

        //        //if (ds != null)
        //        //{
        //        //    if (ds.Tables.Count > 0)
        //        //    {
        //        //        documentCount = ds.Tables[0].Rows.Count;
        //        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        //        {
        //        //            totalFileSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
        //        //        }
        //        //    }
        //        //}

        //        //if (documentCount > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadFiles"]))
        //        //{
        //        //    return returnValue = "Number of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadFiles"];
        //        //}

        //        //if (totalFileSize > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
        //        //{
        //        //    return returnValue = "Total size of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadSize"] + " MB";
        //        //}


        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue = ex.Message;
        //    }
        //    return returnValue;
        //}

        //private string ValidateCustFileBeforeUpload(string userPKID)
        //{
        //    Common cm = new Common();
        //    DataAccess da = new Classes.DataAccess();
        //    string returnValue = "";
        //    try
        //    {
        //        if (flpCustResponseUpload.HasFile == true)
        //        {
        //            if (!cm.ValidMimeType(flpCustResponseUpload.PostedFile.ContentType, Path.GetExtension(flpCustResponseUpload.FileName)))
        //            {
        //                return returnValue = "File type '" + Path.GetExtension(flpCustResponseUpload.FileName) + "' is not supported.";
        //            }

        //            if (flpCustResponseUpload.FileBytes.Length > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
        //            {
        //                return returnValue = "File '" + Path.GetFileName(flpCustResponseUpload.FileName) + "' is too large. Max file size is 8MB.";
        //            }


        //            //DataSet ds = new DataSet();
        //            //ds = da.GetTemporaryCBRSDocuments(userPKID, "1");
        //            //int documentCount = 0;
        //            //int totalFileSize = 0;

        //            //if (ds != null)
        //            //{
        //            //    if (ds.Tables.Count > 0)
        //            //    {
        //            //        documentCount = ds.Tables[0].Rows.Count;
        //            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            //        {
        //            //            totalFileSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
        //            //        }
        //            //    }
        //            //}

        //            //if (documentCount > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadFiles"]))
        //            //{
        //            //    return returnValue = "Number of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadFiles"];
        //            //}

        //            //if (totalFileSize > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
        //            //{
        //            //    return returnValue = "Total size of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadSize"] + " MB";
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue = ex.Message;
        //    }
        //    return returnValue;
        //}

        private BilletterieAPIWS.ticketObject PopulateTicketObject()
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

                   #region Populate file attachments

                    Common cm = new Common();
                    if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                    {
                        cm.CleanUpTempFiles(returnValue.USR_PKID);
                    }

                    BilletterieAPIWS.fileAttachmentObject singleObj = new BilletterieAPIWS.fileAttachmentObject();
                    BilletterieAPIWS.fileAttachmentObject atchObj = new BilletterieAPIWS.fileAttachmentObject();

                    ArrayList appAr = new ArrayList();

                    if (GridViewUploadedDocs.Rows.Count > 0)
                    {
                        List<RecordAttachments> ListTicketPDFAttachments = null;
                        if (Session["ticketPDFUploadObject"] != null)
                        {
                            ListTicketPDFAttachments = (List<RecordAttachments>)Session["ticketPDFUploadObject"];
                            atchObj = new BilletterieAPIWS.fileAttachmentObject();
                            for (int i = 0; i < ListTicketPDFAttachments.Count; i++)
                            {
                                atchObj.DCM_OriginalName = ListTicketPDFAttachments[i].OriginalFileName;
                                atchObj.AttachmentSize = ListTicketPDFAttachments[i].FileSize;
                                atchObj.MimeType = ListTicketPDFAttachments[i].FileMimeType;
                                atchObj.DCM_Extention = ListTicketPDFAttachments[i].FileExtension;
                                atchObj.DCM_DerivedName = ListTicketPDFAttachments[i].DerivedFileName;
                                atchObj.DCT_PKID = 1;
                                atchObj.DCS_PKID = 1;
                                atchObj.DCL_PKID = 1;
                                atchObj.DCM_DocumentPath = ListTicketPDFAttachments[i].FileURL;
                                atchObj.STS_PKID = 30;
                                atchObj.DCM_FileField = ListTicketPDFAttachments[i].FileField;
                                appAr.Add(atchObj);
                                atchObj = new BilletterieAPIWS.fileAttachmentObject();
                            }
                            returnValue.AttachedFiles = new BilletterieAPIWS.fileAttachmentObject[appAr.Count];
                            appAr.CopyTo(returnValue.AttachedFiles);
                            appAr.Clear();
                            //returnValue.AttachedFile = atchObj;
                            returnValue.TCK_HasFile = true;
                        }

                        #region Comments
                        /*
                        DataSet ds = new DataSet();
                        ds = da.GetTemporaryDocuments(usrSession.USR_PKID.ToString(), "1");
                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    //select TPD_PKID, USR_PKID, UST_PKID, DCT_PKID, TPD_FileSize, TPD_FileDescription, TPD_FileExtension, TPD_FileTypeCode, TPD_FileField, TPD_FileURL, TPD_OriginalFileName, TPD_DerivedFileName from TB_TPD_TemporaryDocument where USR_PKID = " + userPKID + " and UST_PKID = " + userType + " order by TPD_PKID desc
                                    atchObj.DCM_OriginalName = ds.Tables[0].Rows[i]["TPD_OriginalFileName"].ToString();
                                    atchObj.AttachmentSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
                                    atchObj.MimeType = ds.Tables[0].Rows[i]["TPD_MimeType"].ToString();
                                    atchObj.DCM_Extention = ds.Tables[0].Rows[i]["TPD_FileExtension"].ToString();
                                    atchObj.DCM_DerivedName = "doc" + returnValue.USR_PKID.ToString() + "_" + i.ToString() + "_" + Path.GetExtension(ds.Tables[0].Rows[i]["TPD_FileExtension"].ToString());
                                    atchObj.DCT_PKID = 1;
                                    atchObj.DCS_PKID = 1;
                                    atchObj.DCL_PKID = 1;
                                    atchObj.DCM_DocumentPath = ds.Tables[0].Rows[i]["TPD_FileURL"].ToString();
                                    atchObj.STS_PKID = 30;

                                    returnValue.AttachedFile = atchObj;
                                    returnValue.TCK_HasFile = true;
                                    appAr.Add(atchObj);
                                    atchObj = new fileAttachmentObject();
                                }

                                returnValue.AttachedFiles = new fileAttachmentObject[appAr.Count];
                                appAr.CopyTo(returnValue.AttachedFiles);
                                appAr.Clear();
                            }
                        }
                        */
                        #endregion
                    }
                    #endregion

                    //Populate selected category
                    if (ddlSubCategory.Visible == true)
                    {
                        returnValue.CAT_PKID = Int32.Parse(ddlSubCategory.SelectedValue);
                    }
                    else if (ddlCategory.Visible == true)
                    {
                        returnValue.CAT_PKID = Int32.Parse(ddlCategory.SelectedValue);
                    }
                    else if (ddlDepartment.Visible == true)
                    {
                        returnValue.CAT_PKID = Int32.Parse(ddlDepartment.SelectedValue);
                    }

                    returnValue.TCK_IsLog = false;
                    returnValue.OFC_PKID = 0;
                    returnValue.TCK_TicketNumber = "";
                    returnValue.TCK_Subject = txtTicketSubject.Text.Trim();
                    returnValue.TCK_Reference = txtReferenceNo.Text.Trim();
                    returnValue.TCK_CaseIdentifier = txtEnterpriseNo.Text.Trim();

                    returnValue.CAT_ForcedFieldID = cm.GetCategoryForcedFieldID(returnValue.CAT_PKID);
                    
                    returnValue.TCK_Message = txtTicketMessage.Text.Trim();
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
                    returnValue.PRV_PKID = Int32.Parse(ddlProvince.SelectedValue);
                    returnValue.TCT_PKID = 1;
                    returnValue.TCK_FromMobile = false;

                    return returnValue;
                }
            }
            catch (Exception ex)
            {

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

                    Common cm = new Common();

                    returnValue.CAT_PKID = Int32.Parse(ConfigurationManager.AppSettings["FinanceCBRSCAT_PKID"]);
                    returnValue.TCK_IsLog = false;
                    returnValue.OFC_PKID = 0;
                    returnValue.TCK_TicketNumber = "";
                    returnValue.TCK_Subject = ConfigurationManager.AppSettings["RefundSubject"];
                    returnValue.TCK_Reference = "REFUND" + custInsInsertedPKID;
                    returnValue.TCK_CaseIdentifier = "";
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
                    returnValue.PRV_PKID = 10;
                    returnValue.TCT_PKID = 4;
                    returnValue.TCK_FromMobile = false;
                    return returnValue;
                }
            }
            catch (Exception ex)
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

            }
            return returnValue;
        }

        private string GetUserIDFromTicket(string tckPKID)
        {
            //DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject selObj = new BilletterieAPIWS.SelectStringResponseObject();
            selObj = bilAPIWS.GetBilletterieIntScalar("select USR_PKID from TB_TCK_Ticket where TCK_PKID = " + tckPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            string returnValue = selObj.selectedPKID;

            return returnValue;
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

        private BilletterieAPIWS.EmailMessageObject PopulateNewRefundEmailObject(string emailList, BilletterieAPIWS.cbrsCustomerDetailObject refundObj)
        {
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            returnValue.EML_ToEmailAdmin = emailList.Trim();
            returnValue.EML_ToEmailList = emailList.Trim();
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

        private string GetNewRefundConfirmationEmailBody(BilletterieAPIWS.cbrsCustomerDetailObject refundObj)
        {
            string returnValue = "";
            returnValue = "<table border='1' frame='vsides' rules='cols'><tr style='border:none; width:100%; height:10px; padding:0px; background-color:#007073; color: #E4EC04;'><td><h3>Refund Request # [ " + refundObj.CUS_AgentCode + " ] has been received.</h3></td></tr> ";
            returnValue = returnValue + "<tr style='font-size:13px; border:inherit; width:100%; height:5px; padding:0px; background-color: lightgray;'><td><h4>Subject request for CIPC Account : " + refundObj.CUS_AgentCode + "</h4></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><br/>Date :" + refundObj.CUS_DateCreated + "<br/><p>Dear " + refundObj.CUS_FullName + "  ,<br/></p></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>A refund request has been created using " + ConfigurationManager.AppSettings["SystemTitle"] + " system; your attention is required.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td><table style='margin-left:10px; border-collapse: collapse;'><tr style='border: none;'><td style='border-left:  solid 3px blue; min-height:30px; color: green;'><i>  A refund of "+ refundObj.CUS_AgentBalance + " has been requested from CIPC. The approved amount will be paid to bank account " + refundObj.CUS_BankName + " - " + refundObj.CUS_AccountNumber + " </i></td></tr></table><br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Approval of refund is subject to verification of all submitted documents.<br /><br /></td></tr>";
            returnValue = returnValue + "<tr style='font-size: 13px; background-color: white;'><td>Please note that this is an automated mail response. Please do NOT reply to this message as it is sent from an unattended mailbox. <br /><br />Best Regards,<br/><br/>" + ConfigurationManager.AppSettings["OrganisationName"] + " Finance Division <br/><br/></td></tr>";
            returnValue = returnValue + "<tr style='border:inherit; width:100%; height:10px; padding:0px; background-color:#007073;'><td></td></tr></table>";
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
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select V.SVL_Hours, T.TCK_DateCreated from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_SVL_ServiceLevel V on C.SVL_PKID = V.SVL_PKID where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DateTime newCalculatedDate = DateTime.Parse(ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString()).AddHours(double.Parse(ds.Tables[0].Rows[0]["SVL_Hours"].ToString()));
                    string newDateString = "";
                    if (bool.Parse(ConfigurationManager.AppSettings["GetCalculatedDates"]))
                    {
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
                    }
                    else
                    {
                        newDateString = GetFormattedOfficeDate(newCalculatedDate.ToString("yyyy-MM-dd"));
                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TCK_DateDue = '" + newDateString + "' where TCK_PKID = " + ticketPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        
                    }
                }
            }
        }

        //private BilletterieAPIWS.EmailMessageObject PopulateErrorEmailObject(string errorMethod, string errorMessage, string valueID)
        //{
        //    BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();

        //    returnValue.EML_ToEmailAdmin = ConfigurationManager.AppSettings["FailureAddress"];
        //    returnValue.EML_ToEmailList = ConfigurationManager.AppSettings["FailureAddress"];
        //    returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
        //    returnValue.EML_Subject = "Error in method: " + errorMethod;
        //    returnValue.EML_MailBody = "Dear System Administrator.<br /><br /> Billetterie has generated the following error. <br /><br /> Please urgently attend to it." + errorMessage;
        //    returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
        //    returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
        //    returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
        //    returnValue.EML_Status = "1";
        //    returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
        //    returnValue.EML_KeyField = "QRS_ERROR";
        //    returnValue.EML_KeyValue = valueID;
        //    returnValue.EML_Domain = "0";
        //    returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];

        //    return returnValue;
        //}

        #endregion

        #region Control Events

        protected void btnNewCaptcha_Click(object sender, EventArgs e)
        {
            UpdateCaptchaText();
        }

        //protected void lnkAttachFiles_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        ScriptManager.RegisterStartupScript(Page, this.GetType(), "str", "<script language='javascript' type='text/javascript'>document.getElementById(\"imageUploadRow\").style.visibility = \"visible\";document.getElementById(\"AjaxFileUpload11\").style.visibility = \"visible\";</script>", false);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //protected void lnkDeleteAttachedFiles_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        ScriptManager.RegisterStartupScript(Page, this.GetType(), "str", "<script language='javascript' type='text/javascript'>document.getElementById(\"imageUploadRow\").style.visibility = \"collapse\";</script>", false);
        //        fupAttachFile = new FileUpload();
        //    }
        //    catch (Exception)
        //    {

        //    }

        //}

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlDepartment.SelectedValue != "" && ddlDepartment.SelectedValue != "0")
            {
                //Populate category depending on selected department
                bool displayCategory = false;
                displayCategory = PopulateCategoryDDL(ddlDepartment.SelectedValue);
                
                if (bool.Parse(ConfigurationManager.AppSettings["displayCategoryToolTip"]))
                {
                    Common cm = new Common();
                    string toolTipMessage = cm.GetToolTipMessage(ddlDepartment.SelectedValue);
                    if (toolTipMessage.Trim() != "")
                    {
                        lblCategoryToolTip.Text = toolTipMessage;
                        deptToolTipDisplayRow.Visible = true;
                        deptToolTipDisplayRowEmpty.Visible = true;
                    }
                    else
                    {
                        lblCategoryToolTip.Text = "";
                        deptToolTipDisplayRow.Visible = false;
                        deptToolTipDisplayRowEmpty.Visible = false;
                    }
                }
                else
                {
                    lblCategoryToolTip.Text = "";
                    deptToolTipDisplayRow.Visible = false;
                    deptToolTipDisplayRowEmpty.Visible = false;
                }

                Session["SelectedCATPKID"] = ddlDepartment.SelectedValue;

                lblDDLCategory.Visible = displayCategory;
                ddlCategory.Visible = displayCategory;

                lblDDLSubCategory.Visible = false;
                ddlSubCategory.Visible = false;
            }
            else
            {
                lblDDLCategory.Visible = false;
                ddlCategory.Visible = false;

                lblDDLSubCategory.Visible = false;
                ddlSubCategory.Visible = false;
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCategory.SelectedValue != "" && ddlCategory.SelectedValue != "0")
            {
                //Populate category depending on selected department
                bool displaySubCategory = false;
                displaySubCategory = PopulateSubCategoryDDL(ddlCategory.SelectedValue);
                if (bool.Parse(ConfigurationManager.AppSettings["displayCategoryToolTip"]))
                {
                    Common cm = new Common();
                    string toolTipMessage = cm.GetToolTipMessage(ddlCategory.SelectedValue);
                    if (toolTipMessage.Trim() != "")
                    {
                        lblCategoryToolTip.Text = toolTipMessage;
                        lblCategoryToolTip.BackColor = System.Drawing.Color.Red;
                        deptToolTipDisplayRow.Visible = true;
                        deptToolTipDisplayRowEmpty.Visible = true;
                    }
                    else
                    {
                        lblCategoryToolTip.Text = "";
                        deptToolTipDisplayRow.Visible = false;
                        deptToolTipDisplayRowEmpty.Visible = false;
                    }
                }
                else
                {
                    lblCategoryToolTip.Text = "";
                    deptToolTipDisplayRow.Visible = false;
                    deptToolTipDisplayRowEmpty.Visible = false;
                }

                Session["SelectedCATPKID"] = ddlCategory.SelectedValue;

                lblDDLSubCategory.Visible = displaySubCategory;
                ddlSubCategory.Visible = displaySubCategory;
            }
            else
            {
                lblCategoryToolTip.Text = "";
                deptToolTipDisplayRow.Visible = false;
                deptToolTipDisplayRowEmpty.Visible = false;

                lblDDLSubCategory.Visible = false;
                ddlSubCategory.Visible = false;
            }
        }

        protected void ddlSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSubCategory.SelectedValue != "" && ddlSubCategory.SelectedValue != "0")
            {
                if (bool.Parse(ConfigurationManager.AppSettings["displayCategoryToolTip"]))
                {
                    Common cm = new Common();
                    string toolTipMessage = cm.GetToolTipMessage(ddlSubCategory.SelectedValue);
                    if (toolTipMessage.Trim() != "")
                    {
                        lblCategoryToolTip.Text = toolTipMessage;
                        lblCategoryToolTip.BackColor = System.Drawing.Color.Transparent;
                        deptToolTipDisplayRow.Visible = true;
                        deptToolTipDisplayRowEmpty.Visible = true;
                    }
                    else
                    {
                        lblCategoryToolTip.Text = "";
                        deptToolTipDisplayRow.Visible = false;
                        deptToolTipDisplayRowEmpty.Visible = false;
                    }
                }
                else
                {
                    lblCategoryToolTip.Text = "";
                    deptToolTipDisplayRow.Visible = false;
                    deptToolTipDisplayRowEmpty.Visible = false;
                }
                Session["SelectedCATPKID"] = ddlSubCategory.SelectedValue;
            }
            else
            {
                lblCategoryToolTip.Text = "";
                deptToolTipDisplayRow.Visible = false;
                deptToolTipDisplayRowEmpty.Visible = false;
            }
        }

        protected void btnSubmitTicket_Click(object sender, EventArgs e)
        {
           string errorMessage = cm.ValidateInputForXSS(txtCaptchaText.Text.Trim());
            if (errorMessage == "")
            {
                if (txtCaptchaText.Text.Trim() == Session["BillCaptcha"].ToString())
                {
                    #region Captcha is valid
                    DataAccess da = new DataAccess();
                    string errMessage = "";
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();

                    #region Upload document if user did not click Upload button
                    //Upload documents
                    if (GridViewUploadedDocs.Rows.Count <= 0 && fupAttachFile.HasFile)
                    {
                        attachmentList = new List<BilletterieAPIWS.fileAttachmentObject>();

                        if (Session["userObjectCookie"] != null)
                        {
                            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        }

                        errMessage = ValidateFileBeforeUpload(usrSession.USR_PKID.ToString(), Path.GetExtension(fupAttachFile.FileName));
                        if (errMessage == "")
                        {
                            BilletterieAPIWS.fileAttachmentObject fupObject = new BilletterieAPIWS.fileAttachmentObject();

                            fupObject.DCT_PKID = 1;
                            fupObject.DCS_PKID = 1;
                            fupObject.DCL_PKID = 1;
                            fupObject.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + fupAttachFile.FileName;
                            fupObject.DCM_OriginalName = fupAttachFile.FileName;
                            fupObject.DCM_DerivedName = "doc" + Path.GetExtension(fupAttachFile.FileName);
                            fupObject.DCM_Extention = Path.GetExtension(fupAttachFile.FileName);
                            fupObject.STS_PKID = 30;
                            fupObject.AttachmentSize = fupAttachFile.FileBytes.Length;
                            fupObject.MimeType = fupAttachFile.PostedFile.ContentType;

                            BilletterieAPIWS.InsertResponseObject appResp = new BilletterieAPIWS.InsertResponseObject();
                            appResp = bilAPIWS.AddBilletterieTempDocument(fupObject, usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                            DataSet ds = new DataSet();
                            ds = bilAPIWS.GetBilletterieTemporaryDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            if (ds != null)
                            {
                                if (ds.Tables.Count > 0)
                                {
                                    GridViewUploadedDocs.DataSource = ds.Tables[0];
                                    GridViewUploadedDocs.DataBind();
                                }
                            }

                        }
                    }
                    #endregion

                    Common cm = new Common();
                    BilletterieAPIWS.ticketObject tickObj = new BilletterieAPIWS.ticketObject();
                    BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();

                    if (Session["userObjectCookie"] != null)
                    {
                        usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    }

                    errMessage = ValidateFilesBeforeUpload(usrSession.USR_PKID.ToString());
                    if (errMessage == "")
                    {
                        tickObj = PopulateTicketObject();
                        errMessage = cm.ValidateInputForXSS(tickObj);
                        if (errMessage == "")
                        {
                            errMessage = cm.ValidateInput(tickObj);
                            if (errMessage == "")
                            {
                                BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                                BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                                //Save ticket
                                insResp = bilAPIWS.InsertBilletterieTicketRecord(tickObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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

                                    if (GridViewUploadedDocs.Rows.Count > 0)
                                    {
                                        //loop to get all documents
                                        //Save document
                                        for (int i = 0; i < tickObj.AttachedFiles.Length; i++)
                                        {
                                            insResp = bilAPIWS.InsertBilletterieDocumentRecord(tickObj.AttachedFiles[i], ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                            string destFile = tickObj.AttachedFiles[i].DCM_DerivedName;
                                            if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                                            {
                                                destFile = cm.MoveDocuments(tickObj.AttachedFiles[i].DCM_DocumentPath, insResp.insertedPKID.ToString());
                                            }
                                            updResp = bilAPIWS.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), destFile, insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        }
                                    }

                                    string ticketNumber = GenerateTicketNumber(insertedPKID);

                                    updResp = bilAPIWS.UpdateBilletterieTicketRecord(insertedPKID, ticketNumber, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                                    tickObj.TCK_TicketNumber = ticketNumber;

                                    //Update if file has been attached from external
                                    if (GridViewUploadedDocs.Rows.Count > 0)
                                    {
                                        bilAPIWS.UpdateBilletterieRecord("update TB_TCK_Ticket set TKR_HasFile = 1 where TCK_PKID = " + insertedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }

                                    if (bool.Parse(ConfigurationManager.AppSettings["GenerateCaseForm"]))
                                    {

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
                                            //Send ticket creation email
                                            bool categoryNotifies = CheckCategoryNotifiesEmail(tickObj.CAT_PKID);
                                            string emailList = "";
                                            if (categoryNotifies)
                                            {

                                                emailList = cm.GetCategoryMembersList(tickObj.CAT_PKID.ToString());

                                                if (emailList.Trim() != "")
                                                {
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
                                                // Call EndInvoke to wait for the asynchronous call to complete,
                                                // and to retrieve the results.
                                                SMTPMailResponseObject returnValue = caller.EndInvoke(out threadId, result);
                                                //Console.WriteLine("The call executed on thread {0}, with return value \"{1}\".",threadId, returnValue);

                                                #endregion

                                            }
                                            else
                                            {
                                                BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                                                emlObj = PopulateEmailObject(tickObj);
                                                opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                            }
                                        }

                                        lblMainErrorMessage.Text = "";
                                        ModalPopupExtenderSuccess.Show();
                                        lblSuccessHeading.Text = "Ticket successfully created: [T " + tickObj.TCK_PKID + "]";
                                        lblTicketConfirmation.Text = "Thank you for contacting " + ConfigurationManager.AppSettings["OrganisationName"] + ". <br /><br /> Your query has been assigned ticket reference number [<b> T" + tickObj.TCK_PKID + "</b>]. <br /><br />A confirmation email will be sent to email account [<b>" + tickObj.TCK_AlternateEmail + "</b>] shortly. <br /><br />Please quote this ticket number for any further correspondence regarding this query.";
                                    }
                                }
                            }
                            else
                            {
                                //Add message
                                lblMainErrorMessage.Text = "[" + errMessage + "]";
                            }
                        }
                        else
                        {
                            //Add message
                            lblMainErrorMessage.Text = "[Invalid ticket - " + errMessage + "]";
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

        protected void btnOK_Click(object sender, EventArgs e)
        {
            if (lblCommandName.Text != "Bank")
            {
                Response.Redirect("~/ExternalPages/ViewTickets.aspx", false);
            }
        }

        #endregion

        private bool CheckCategoryNotifiesEmail(int catPKID)
        {
            bool returnValue = false;
            BilletterieAPIWS.SelectStringResponseObject stndResponObj = new BilletterieAPIWS.SelectStringResponseObject();
            stndResponObj = bilAPIWS.GetBilletterieScalar("select CAT_NotifyEmail from TB_CAT_Category where CAT_PKID = " + catPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (stndResponObj.selectedPKID == "1" || stndResponObj.selectedPKID.ToLower() == "true")
            {
                returnValue = true;
            }
            return returnValue;
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

        //protected void AjaxFileUpload11_UploadStart(object sender, AjaxFileUploadStartEventArgs e)
        //{
        //    //AjaxFileUpload11.
        //    //for (int i = 0; i < e.FilesInQueue; i++)
        //    //{
        //    //    AjaxFileUpload11.ClearFileListAfterUpload
        //    //}
        //    //string servArg = e.ServerArguments;
        //    //int fileCount = e.FilesInQueue;
        //}

        //protected void AjaxFileUpload11_UploadCompleteAll(object sender, AjaxFileUploadCompleteAllEventArgs e)
        //{
        //    ScriptManager.RegisterStartupScript(Page, this.GetType(), "str", "<script language='javascript' type='text/javascript'> alert('File upload completed successfully.'); </script>", false);

        //    //Response.Write("<script language='javascript'> alert(' problem');</script>");

        //    //lblFileUploadError.Text = "File upload completed successfully.";
        //    //lblFileUploadError.ForeColor = System.Drawing.Color.Green;
        //}

        protected void btnCancelAllUpload_Click(object sender, EventArgs e)
        {

        }

        protected void GridViewUploadedDocs_RowBoundOperations(object sender, GridViewCommandEventArgs e)
        {
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }
            try
            {
                if (e.CommandName == "DeleteAttachement")
                {
                    #region

                    string tempAttachURL = bilAPIWS.GetBilletterieTempDocumentURL(e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    if ((System.IO.File.Exists(tempAttachURL)))
                    {
                        System.IO.File.Delete(tempAttachURL);
                    }

                    BilletterieAPIWS.UpdateResponseObject apResp = new BilletterieAPIWS.UpdateResponseObject();
                    apResp = bilAPIWS.DeleteBilletterieSingleTempDocuments(usrSession.USR_PKID.ToString(), e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    Session["ticketPDFUploadObject"] = null;
                    GridViewUploadedDocs.DataSource = null;
                    GridViewUploadedDocs.DataBind();

                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieTemporaryDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            GridViewUploadedDocs.DataSource = ds.Tables[0];
                            GridViewUploadedDocs.DataBind();
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
                lblErrorMessage.Visible = true;
            }
        }

        protected void btnSaveUploadedFile_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();
                lblMainErrorMessage.Text = "";

                if (fupAttachFile.HasFile)
                {
                    MimeType mimeType = new MimeType();

                    string mimeTypeName = mimeType.GetMimeType(fupAttachFile.FileBytes, fupAttachFile.FileName);

                    if (cm.ValidMimeType(mimeTypeName, Path.GetExtension(fupAttachFile.FileName)))
                    {
                        #region 
                        attachmentList = new List<BilletterieAPIWS.fileAttachmentObject>();
                        DataAccess da = new DataAccess();

                        BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                        if (Session["userObjectCookie"] != null)
                        {
                            usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                        }

                        string errMessage = ValidateFileBeforeUpload(usrSession.USR_PKID.ToString(), Path.GetExtension(fupAttachFile.FileName));
                        if (errMessage == "")
                        {
                            string linksPDF = ValidatePDFFileLinks(fupAttachFile.FileBytes, Path.GetExtension(fupAttachFile.FileName));
                            if (linksPDF == "")
                            {
                                RecordAttachments fupObject = new RecordAttachments();
                                fupObject.FileExtension = Path.GetExtension(fupAttachFile.FileName);
                                fupObject.FileType = "TCK";
                                fupObject.FileSize = fupAttachFile.FileBytes.Length;
                                fupObject.FileMimeType = mimeTypeName;  
                                fupObject.FileURL = ConfigurationManager.AppSettings["IPOnlineDocumentsGN"] + usrSession.USR_PKID.ToString() + fupAttachFile.FileName;
                                fupObject.FileField = fupAttachFile.FileBytes;
                                fupObject.FileDescription = "Ticket attachment";
                                fupObject.OriginalFileName = fupAttachFile.FileName;
                                fupObject.DerivedFileName = usrSession.USR_PKID.ToString() + fupAttachFile.FileName;

                                if (Session["ticketPDFUploadObject"] != null)
                                {
                                    ListTicketPDFAttachments = (List<RecordAttachments>)Session["ticketPDFUploadObject"];
                                }
                                else
                                {
                                    ListTicketPDFAttachments = new List<RecordAttachments>();
                                }
                                ListTicketPDFAttachments.Add(new RecordAttachments { FileDescription = fupObject.FileDescription, FileExtension = fupObject.FileExtension, FileType = fupObject.FileType, FileURL = fupObject.FileURL, FileField = fupObject.FileField, OriginalFileName = fupObject.OriginalFileName, DerivedFileName = fupObject.DerivedFileName, FileMimeType = fupObject.FileMimeType, FileSize = fupObject.FileSize });
                                if (ListTicketPDFAttachments.Count > 0)
                                {
                                    GridViewUploadedDocs.Visible = true;
                                    Session["ticketPDFUploadObject"] = ListTicketPDFAttachments;
                                    GridViewUploadedDocs.DataSource = ListTicketPDFAttachments;
                                    GridViewUploadedDocs.DataBind();
                                }

                            }
                            else
                            {
                                lblMainErrorMessage.Text = linksPDF;
                                lblMainErrorMessage.Visible = true;
                            }
                        }
                        else
                        {
                            lblMainErrorMessage.Text = errMessage;
                            lblMainErrorMessage.Visible = true;
                        }
                        #endregion
                    }
                    else
                    {
                        lblMainErrorMessage.Text = "Document type not allowed.";
                        lblMainErrorMessage.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMainErrorMessage.Text = ex.Message;
                lblMainErrorMessage.Visible = true;
            }
        }

        protected void GridViewUploadedDocs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //ORADataAccess oraDAC = new ORADataAccess();
            //AuthenticationObject authObj = new AuthenticationObject();
            //authObj = (AuthenticationObject)Session["userObjectCookie"];

            if (e.Row.Cells[0] != null && e.Row.RowIndex >= 0)
            {

                if (e.Row.Cells[1].Text.Trim() != "" && e.Row.RowIndex >= 0)
                {
                    if (e.Row.Cells[1].Text.Trim() != "&nbsp;")
                    {
                        e.Row.Cells[1].Text = (Int32.Parse(e.Row.Cells[1].Text)/1024).ToString();
                    }
                }
            }

        }

        protected void GridViewCustUploadedDocs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink hp = new HyperLink();
            if (e.Row.RowIndex >= 0)
            {
                if (e.Row.Cells[0] != null && e.Row.RowIndex >= 0)
                {

                    if (e.Row.Cells[2].Text.Trim() != "" && e.Row.RowIndex >= 0)
                    {
                        if (e.Row.Cells[2].Text.Trim() == "1")
                        {
                            e.Row.Cells[2].Text = "ID Document";
                        }
                        else if (e.Row.Cells[2].Text.Trim() == "2")
                        {
                            e.Row.Cells[2].Text = "Bank Confirmation";
                        }
                        else if (e.Row.Cells[2].Text.Trim() == "3")
                        {
                            e.Row.Cells[2].Text = "Payment Proof";
                        }
                        else if (e.Row.Cells[2].Text.Trim() == "4")
                        {
                            e.Row.Cells[2].Text = "Bank Statement";
                        }
                    }

                    LinkButton myLinkButton = new LinkButton();
                    myLinkButton = (LinkButton)e.Row.Cells[0].FindControl("lnkOFCPKIDLink");
                    //string fileLink = ConfigurationManager.AppSettings["RootPublicURL"] + "GetDocument.aspx?docID=" + myLinkButton.CommandArgument;
                    string fileLink = ConfigurationManager.AppSettings["RootPublicURL"] + "UploadTemp/" + Path.GetFileName(myLinkButton.CommandArgument);
                    myLinkButton.Visible = false;

                    hp.Text = myLinkButton.Text;
                    hp.NavigateUrl = fileLink;
                    //   string fileLink = ConfigurationManager.AppSettings["RootPublicURL"] + "UploadTemp/" + myLinkButton.CommandArgument;
                    //hp.Attributes.Add("OnClick", "window.open('" + ConfigurationManager.AppSettings["RootPublicURL"] + "GetDocument.aspx?docID=" + myLinkButton.CommandArgument + "','name','height=600, width=900,toolbar=no, directories=no,status=no, menubar=no,scrollbars=yes,resizable=no'); return false;");
                    hp.Attributes.Add("OnClick", "window.open('" + ConfigurationManager.AppSettings["RootPublicURL"] + "UploadTemp/" + Path.GetFileName(myLinkButton.CommandArgument) + "','name','height=600, width=900,toolbar=no, directories=no,status=no, menubar=no,scrollbars=yes,resizable=no'); return false;");
                    hp.Target = "_blank";
                    e.Row.Cells[0].Controls.Add(hp);
                }
            }
        }

        protected void btnSubmitBankDetails_Click(object sender, EventArgs e)
        {
            DataAccess da = new DataAccess();
            Common cm = new Classes.Common();

            lblPopErrorMessage.Text = "";
            lblPopErrorMessage.Visible = false;

            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }
            ERMSAgentProfileObject ermsAgentObj = new ERMSAgentProfileObject();
            ERMSAgentProfileResponse ermsAgentResp = new ERMSAgentProfileResponse();
            ermsAgentResp = bilAPIWS.GetERMSAgentProfile(usrSession.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

            BilletterieAPIWS.cbrsCustomerDetailObject custDetailObj = new BilletterieAPIWS.cbrsCustomerDetailObject();
            custDetailObj = PopulateCustomerBankDetails(usrSession.USR_PKID, lblERMSPKID.Text);

            if (custDetailObj.noError)
            {
                string errorMessage = cm.ValidateInputForXSS(custDetailObj);
                if (errorMessage == "")
                {
                    errorMessage = ValidateCustomerDetails(custDetailObj, ermsAgentResp.ermsAgentProfileObject.sAgentName, ermsAgentResp.ermsAgentProfileObject.sAgentIDNumber);
                    if (errorMessage == "")
                    {

                        #region No error
                        BilletterieAPIWS.InsertResponseObject custIns = new BilletterieAPIWS.InsertResponseObject();
                        custIns = bilAPIWS.InsertCustomerDetailRecord(custDetailObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //custIns = da.InsertCustomerDetailRecord(custDetailObj);
                        custDetailObj.CUS_PKID = Int32.Parse(custIns.insertedPKID);

                        if (GridViewCustUploadedDocs.Rows.Count > 0)
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
                        }

                        lblCommandName.Text = "Bank";
                        lblTicketConfirmation.Text = "Banking details successfully saved. You will here from CIPC in the coming days.";
                        lblSuccessHeading.Text = "Banking details successfully saved.";

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

                        #endregion

                        //Create ticket that correspond with refund request
                        if (bool.Parse(ConfigurationManager.AppSettings["CreateRefundTicket"]))
                        {
                            //Create new ticket
                            BilletterieAPIWS.ticketObject cbrsTicketObj = new BilletterieAPIWS.ticketObject();
                            cbrsTicketObj = PopulateNewCBRSTicketObject(custDetailObj, custIns.insertedPKID);
                            string tckPKID = CreateBankRefundTicket(cbrsTicketObj);

                            //Update CBRS with ticket number
                            BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                            updResp = bilAPIWS.UpdateCBRSCustomerRecordTicket(custIns.insertedPKID, tckPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //updResp = da.UpdateCBRSCustomerRecordTicket(custIns.insertedPKID, tckPKID);
                        }

                        ModalPopupExtenderSuccess.Show();
                    }
                    else
                    {
                        DataSet ds = new DataSet();
                        ds = bilAPIWS.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //ds = da.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1");
                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                GridViewCustUploadedDocs.DataSource = ds.Tables[0];
                                GridViewCustUploadedDocs.DataBind();
                            }
                        }

                        lblPopErrorMessage.Text = errorMessage;
                        lblPopErrorMessage.Visible = true;

                        ModalPopupExtenderCustomerDetail.Show();
                    }
                }
                else
                {
                    lblPopErrorMessage.Text = errorMessage;
                    lblPopErrorMessage.Visible = true;
                }
            }
            else
            {
                lblPopErrorMessage.Text = custDetailObj.errorMessage;
                lblPopErrorMessage.Visible = true;

                ModalPopupExtenderCustomerDetail.Show();
            }
        }

        private string CreateBankRefundTicket(BilletterieAPIWS.ticketObject tickObj)
        {
            string retValue = "";

            #region Captcha is valid

            DataAccess da = new DataAccess();
            string errMessage = "";
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();

            Common cm = new Common();
            BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();

            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }

            errMessage = cm.ValidateInput(tickObj);
            if (errMessage == "")
            {
                errMessage = cm.ValidateInputForXSS(tickObj);
                if (errMessage == "")
                {
                    BilletterieAPIWS.InsertResponseObject insResp = new BilletterieAPIWS.InsertResponseObject();
                    BilletterieAPIWS.UpdateResponseObject updResp = new BilletterieAPIWS.UpdateResponseObject();
                    //Save ticket
                    insResp = bilAPIWS.InsertBilletterieTicketRecord(tickObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //insResp = bilAPIWS.CreateNewTicket(tickObj, usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
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

        private BilletterieAPIWS.cbrsCustomerDetailObject PopulateCustomerBankDetails(int USRPKID, string ERSMStatus)
        {
            //EncryptDecrypt cipherWS = new EncryptDecrypt();
            Common cm = new Common();
            DataAccess da = new Classes.DataAccess();

            BilletterieAPIWS.cbrsCustomerDetailObject retValue = new BilletterieAPIWS.cbrsCustomerDetailObject();
            retValue.noError = true;
            retValue.CUS_PKID = 0;
            retValue.BKN_PKID = Int32.Parse(ddlBankName.SelectedValue);
            retValue.USR_PKID = USRPKID;
            retValue.CTS_PKID = 1;
            retValue.CUS_AgentCode = txtAgentAccount.Text;
            retValue.CUS_AgentBalance = txtAccountBalance.Text;

            retValue.CUS_IDNumber = bilAPIWS.GetEncryptedValue(txtIDNumber.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

            if (cm.ValidateAntiXSS(txtFullName.Text))
            {
                retValue.CUS_FullName = txtFullName.Text;
            }
            else
            {
                retValue.noError = false;
                retValue.errorMessage = "Invalid user input.";
            }

            if (ddlBankName.SelectedItem != null)
            {
                retValue.CUS_BankName = ddlBankName.SelectedItem.Text;
            }
            if (ddlBranchCode.SelectedItem != null)
            {
                retValue.CUS_BankBranchCode = ddlBranchCode.SelectedItem.Text;
            }

            if (cm.ValidateAntiXSS(txtAccountHolder.Text))
            {
                retValue.CUS_AccountHolderName = txtAccountHolder.Text;
            }
            else
            {
                retValue.noError = false;
                retValue.errorMessage = "Invalid user input.";
            }

            if (cm.ValidateAntiXSS(txtAccountNumber.Text))
            {
                retValue.CUS_AccountNumber = bilAPIWS.GetEncryptedValue(txtAccountNumber.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //retValue.CUS_AccountNumber = cipherWS.EncryptMethod("e38effb95631439d2affd2db94cc7053", txtAccountNumber.Text);
            }
            else
            {
                retValue.noError = false;
                retValue.errorMessage = "Invalid user input.";
            }

            if (cm.ValidateAntiXSS(txtCustContact.Text))
            {
                retValue.CUS_ContactNumber = bilAPIWS.GetEncryptedValue(txtCustContact.Text, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //retValue.CUS_ContactNumber = cipherWS.EncryptMethod("e38effb95631439d2affd2db94cc7053", txtCustContact.Text);
            }
            else
            {
                retValue.noError = false;
                retValue.errorMessage = "Invalid user input.";
            }

            retValue.CUS_ERMSStatus = ERSMStatus;

            retValue.CUS_DateCreated = DateTime.Now.ToString();

            string lastDepositDate = GetLastDepositDate(txtAgentAccount.Text);

            retValue.CUS_IsOldBalance = IsOldDepositDate(lastDepositDate);

            retValue.CUS_FromMobile = false;

            retValue.CUS_IsEncrypted = true;

            #region Populate file attachments
            //cm.CleanUpTempFiles(USRPKID);
            BilletterieAPIWS.fileAttachmentObject atchObj = new BilletterieAPIWS.fileAttachmentObject();
            ArrayList appAr = new ArrayList();

            if (GridViewCustUploadedDocs.Rows.Count > 0)
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

                            //retValue.AttachedFile = atchObj;
                            //usrSession.TCK_HasFile = true;
                            appAr.Add(atchObj);
                            atchObj = new BilletterieAPIWS.fileAttachmentObject();
                        }
                        retValue.AttachedFiles = new BilletterieAPIWS.fileAttachmentObject[appAr.Count];
                        appAr.CopyTo(retValue.AttachedFiles);
                        appAr.Clear();
                    }
                }
            }
            #endregion

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

        private string ValidateCustomerDetails(BilletterieAPIWS.cbrsCustomerDetailObject cbrsBankObj, string fullName, string idNumber)
        {
            //string retValue = "";
            if (cbrsBankObj.BKN_PKID == 0)
            {
                return "Bank name not selected.";
            }

            else if (cbrsBankObj.USR_PKID == 0)
            {
                return "Bank name not selected.";
            }

            else if (cbrsBankObj.CUS_BankName == "")
            {
                return "Bank not selected.";
            }

            else if (cbrsBankObj.CUS_BankBranchCode == "")
            {
                return "Bank branch code not selected.";
            }

            else if (cbrsBankObj.CUS_AccountHolderName == "")
            {
                return "Account holder name not provided.";
            }

            else if (cbrsBankObj.CUS_AccountNumber == "")
            {
                return "Account number not provided.";
            }

            else if (cbrsBankObj.CUS_ContactNumber == "")
            {
                return "Contact number not provided.";
            }

            else if (cbrsBankObj.CUS_IDNumber == "")
            {
                return "Identity number not provided.";
            }

            else if (cbrsBankObj.CUS_FullName == "")
            {
                return "Full names not provided.";
            }

            else if (cbrsBankObj.AttachedFiles == null)
            {
                return "Please upload supporting document.";
            }

            else if (cbrsBankObj.CUS_FullName.ToUpper() != fullName.ToUpper())
            {
                return "Full names are not matching with the CIPC profile.";
            }

            else if (cbrsBankObj.CUS_IDNumber.ToUpper() != idNumber.ToUpper())
            {
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

        protected void btnCancelForever_Click(object sender, EventArgs e)
        {
            Session["CustomerBankDetailDisplayed"] = true;
        }

        protected void ddlBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPopUpDropDowns(ddlBankName.SelectedValue);

            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }

            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1");
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    GridViewCustUploadedDocs.DataSource = ds.Tables[0];
                    GridViewCustUploadedDocs.DataBind();
                }
            }
           
            ModalPopupExtenderCustomerDetail.Show();
            txtAccountHolder.Focus();
        }

        protected void btnSaveUploadedCustFile_Click(object sender, EventArgs e)
        {
            try
            {
                Common cm = new Common();
                DataAccess da = new DataAccess();
                BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                if (Session["userObjectCookie"] != null)
                {
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                }

                if (flpCustResponseUpload.HasFile)
                {
                    MimeType mimeType = new MimeType();
                    string mimeTypeName = mimeType.GetMimeType(flpCustResponseUpload.FileBytes, flpCustResponseUpload.FileName);
                    if (cm.ValidMimeType(mimeTypeName, Path.GetExtension(flpCustResponseUpload.FileName)))
                    {
                        string linksPDF = ValidatePDFFileLinks(flpCustResponseUpload.FileBytes, Path.GetExtension(flpCustResponseUpload.FileName));
                        if (linksPDF == "")
                        {
                            #region
                            string errMessage = "";
                            if (!cm.ValidCBRSMimeType(flpCustResponseUpload.PostedFile.ContentType, Path.GetExtension(flpCustResponseUpload.FileName)))
                            {
                                errMessage = "File type '" + Path.GetExtension(flpCustResponseUpload.FileName) + "' is not supported.";
                                lblUploadErrorMessage.Text = "File type '" + Path.GetExtension(flpCustResponseUpload.FileName) + "' is not supported.";
                                lblUploadErrorMessage.Visible = true;
                                ModalPopupExtenderCustomerDetail.Show();
                            }

                            if (flpCustResponseUpload.FileBytes.Length > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
                            {
                                errMessage = "File '" + Path.GetFileName(flpCustResponseUpload.FileName) + "' is too large. Max file size is 8MB.";
                                lblUploadErrorMessage.Text = "File '" + Path.GetFileName(flpCustResponseUpload.FileName) + "' is too large. Max file size is 8MB.";
                                lblUploadErrorMessage.Visible = true;
                                ModalPopupExtenderCustomerDetail.Show();
                            }

                            if (ddlCustFileType.SelectedValue == "0")
                            {
                                errMessage = "Please select file type.";
                                lblUploadErrorMessage.Text = "Please select file type.";
                                lblUploadErrorMessage.Visible = true;
                                ModalPopupExtenderCustomerDetail.Show();
                            }

                            errMessage = ValidateBankingFileUpload(ddlCustFileType.SelectedValue);
                            if (errMessage != "")
                            {
                                //errMessage = "File type '" + Path.GetExtension(flpCustResponseUpload.FileName) + "' is not supported.";
                                lblUploadErrorMessage.Text = errMessage;    // "File type '" + Path.GetExtension(flpCustResponseUpload.FileName) + "' is not supported.";
                                lblUploadErrorMessage.Visible = true;
                                ModalPopupExtenderCustomerDetail.Show();
                            }

                            if (errMessage == "")
                            {
                                lblUploadErrorMessage.Text = "";
                                lblUploadErrorMessage.Visible = false;

                                attachmentList = new List<BilletterieAPIWS.fileAttachmentObject>();
                                BilletterieAPIWS.fileAttachmentObject fupObject = new BilletterieAPIWS.fileAttachmentObject();

                                fupObject.DCT_PKID = Int32.Parse(ddlCustFileType.SelectedValue);
                                fupObject.DCS_PKID = 1;
                                fupObject.DCL_PKID = 1;
                                fupObject.DCM_DocumentPath = ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + flpCustResponseUpload.FileName;
                                fupObject.DCM_OriginalName = flpCustResponseUpload.FileName;
                                fupObject.DCM_DerivedName = "doc" + Path.GetFileNameWithoutExtension(flpCustResponseUpload.FileName);
                                fupObject.DCM_Extention = Path.GetExtension(flpCustResponseUpload.FileName);
                                fupObject.STS_PKID = 30;
                                fupObject.AttachmentSize = flpCustResponseUpload.FileBytes.Length;
                                fupObject.MimeType = flpCustResponseUpload.PostedFile.ContentType;

                                string filePath = fupObject.DCM_DerivedName;    // ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + flpCustResponseUpload.FileName;
                                if (bool.Parse(ConfigurationManager.AppSettings["DeleteTemporaryDocuments"]))
                                {
                                    filePath = ConfigurationManager.AppSettings["LocalDocumentDrivePathTemp"] + usrSession.USR_PKID.ToString() + "_" + flpCustResponseUpload.FileName;
                                    flpCustResponseUpload.SaveAs(filePath);
                                }

                                BilletterieAPIWS.InsertResponseObject appResp = new BilletterieAPIWS.InsertResponseObject();
                                appResp = bilAPIWS.AddTempCBRSDocument(fupObject, usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //appResp = da.AddTempCBRSDocument(fupObject, usrSession.USR_PKID.ToString(), "1");

                                DataSet ds = new DataSet();
                                ds = bilAPIWS.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //ds = da.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1");
                                if (ds != null)
                                {
                                    if (ds.Tables.Count > 0)
                                    {
                                        GridViewCustUploadedDocs.DataSource = ds.Tables[0];
                                        GridViewCustUploadedDocs.DataBind();
                                    }
                                }
                            }
                            else
                            {
                                lblPopErrorMessage.Text = errMessage;
                                lblPopErrorMessage.Visible = true;

                                lblUploadErrorMessage.Text = errMessage;    // ex.Message;
                                lblUploadErrorMessage.Visible = true;
                            }
                            #endregion
                        }
                        else
                        {
                            lblPopErrorMessage.Text = linksPDF;
                            lblPopErrorMessage.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblUploadErrorMessage.Text = ex.Message;
                lblUploadErrorMessage.Visible = true;
            }
            ModalPopupExtenderCustomerDetail.Show();
        }

        private string ValidateBankingFileUpload(string documentType)
        {
            string retValue = "";
            if (documentType == "0")
            {
                retValue = "Please select document type.";
            }
            return retValue;
        }

        protected void btnCancelCustAllUpload_Click(object sender, EventArgs e)
        {

        }

        protected void GridViewCustUploadedDocs_RowBoundOperations(object sender, GridViewCommandEventArgs e)
        {
            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }

            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();

            try
            {
                if (e.CommandName == "DeleteAttachement")
                {
                    #region Delete
                    string tempAttachURL = bilAPIWS.GetTempCBRSDocumentURL(e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //string tempAttachURL = da.GetTempCBRSDocumentURL(e.CommandArgument.ToString());
                    if ((System.IO.File.Exists(tempAttachURL)))
                    {
                        System.IO.File.Delete(tempAttachURL);
                    }
                    BilletterieAPIWS.UpdateResponseObject apResp = new BilletterieAPIWS.UpdateResponseObject();
                    apResp = bilAPIWS.DeleteSingleCBRSTempDocuments(usrSession.USR_PKID.ToString(), e.CommandArgument.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //apResp = da.DeleteSingleCBRSTempDocuments(usrSession.USR_PKID.ToString(), e.CommandArgument.ToString());

                    ds = bilAPIWS.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1");
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            GridViewCustUploadedDocs.DataSource = ds.Tables[0];
                            GridViewCustUploadedDocs.DataBind();
                        }
                    }
                    #endregion
                }

                /*
                else if (e.CommandName == "OpenAttachement")
                {
                    #region Open
                    if (Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]))
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenWindow", "window.open('" + ConfigurationManager.AppSettings["LocalDownloadPathURL"] + e.CommandArgument + "','_newtab');", true);
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenWindow", "window.open('" + ConfigurationManager.AppSettings["DownloadPathURL"] + e.CommandArgument + "','_newtab');", true);
                    }
                    #endregion
                }
                */

            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
                lblErrorMessage.Visible = true;
            }

            ds = bilAPIWS.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1");
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    GridViewCustUploadedDocs.DataSource = ds.Tables[0];
                    GridViewCustUploadedDocs.DataBind();
                }
            }
            ModalPopupExtenderCustomerDetail.Show();

        }

        protected void lnkCancelCustAllUpload_Click(object sender, EventArgs e)
        {
            ModalPopupExtenderCustomerDetail.Show();
        }

        protected void btnRequestRefund_Click(object sender, EventArgs e)
        {
            DataAccess da = new Classes.DataAccess();

            BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
            if (Session["userObjectCookie"] != null)
            {
                usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
            }
            else
            {
                Response.Redirect("~/Index.aspx", false);
            }

            //Display popup for banking details
            if (bool.Parse(ConfigurationManager.AppSettings["RequireBankDetails"]))
            {
                if (UserIsNotRestricted(usrSession.USR_UserLogin.ToUpper().Trim()))
                {
                    if (!UserAlreadySubmitted(usrSession.USR_UserLogin.ToUpper().Trim()))
                    {
                        //IPCUBAERMSBillingWS ipERMS = new IPCUBAERMSBillingWS();
                        //ipERMS.Url = ConfigurationManager.AppSettings["IFX_WSURL"];
                        ERMSAgentProfileObject ermsAgentObj = new ERMSAgentProfileObject();
                        ERMSAgentProfileResponse ermsAgentResp = new ERMSAgentProfileResponse();

                        //Check if the customer code exist
                        if (bilAPIWS.GetAgentAccount(usrSession.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]))
                        {
                            #region Bank Details
                            if (Session["CustomerBankDetailDisplayed"] != null && bool.Parse(Session["CustomerBankDetailDisplayed"].ToString()) == false)
                            {
                                //Check if user is coming from Password Reset session
                                if (usrSession.USC_PKID == 1 || usrSession.USC_PKID == 3)
                                {

                                    #region Delete banking temporary documents
                                    DataSet dsA = new DataSet();
                                    dsA = bilAPIWS.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //dsA = da.GetTemporaryCBRSDocuments(usrSession.USR_PKID.ToString(), "1");
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
                                    apCustResp = bilAPIWS.DeleteAllCBRSTempDocuments(usrSession.USR_PKID.ToString(), "1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    //apCustResp = da.DeleteAllCBRSTempDocuments(usrSession.USR_PKID.ToString(), "1");

                                    GridViewCustUploadedDocs.DataSource = null;
                                    GridViewCustUploadedDocs.DataBind();
                                    #endregion

                                    ermsAgentResp = bilAPIWS.GetERMSAgentProfile(usrSession.USR_UserLogin.ToUpper(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    DoubleObjectResponse balObj = new DoubleObjectResponse();
                                    balObj = bilAPIWS.GetAgentBalance(usrSession.USR_UserLogin.ToUpper().Trim(), 0, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    LoadPopUpDropDowns();

                                    txtAccountBalance.Text = ermsAgentResp.ermsAgentProfileObject.sAgentBalance.Replace(',', '.');

                                    if (double.Parse(txtAccountBalance.Text.Trim(), CultureInfo.InvariantCulture) >= double.Parse(ConfigurationManager.AppSettings["MinRefundAmount"].ToString(), CultureInfo.InvariantCulture))
                                    {
                                        lblWarningMessage.Text = ConfigurationManager.AppSettings["CBRSWarningMessage"];    // "CIPC has started a process to refund all balances in the CIPC's prepaid accounts to ligitimate clients. Clients will be required to provide bank account details of the account that was used to transfer funds to CIPC. Supporting documents are required for positive verification of the funds transfered. Failure to provide correct supporting documents will result in refund being rejected.";
                                        lblCustomerDetailHeading.Text = ConfigurationManager.AppSettings["CBRSCustomerDetailHeading"];
                                        lblBottomMessage.Text = ConfigurationManager.AppSettings["CBRSBottomMessage"];  // "NOTE: Intellectual Property business users have an option to ignore this refund requirement as the prepaid system will temporarily remain in place for the purpose of Filing, Renewal and Searching of Copyright, Designs, Patents and Trade Marks";

                                        lblERMSPKID.Text = ermsAgentResp.ermsAgentProfileObject.sAgentStatus;
                                        txtAgentAccount.Text = usrSession.USR_UserLogin;
                                        //txtAccountBalance.Text = ermsAgentResp.ermsAgentProfileObject.sAgentBalance.Replace(',', '.');

                                        ModalPopupExtenderCustomerDetail.Show();
                                    }
                                    else
                                    {
                                        btnRequestRefund.Visible = false;
                                    }

                                    string auditErrorMessage = ValidateAccountForAudit(usrSession.USR_UserLogin.ToUpper().Trim());
                                    if (auditErrorMessage != "")
                                    {
                                        lblPopErrorMessage.Text = auditErrorMessage;
                                        lblPopErrorMessage.Visible = true;
                                    }

                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        btnRequestRefund.Visible = false;
                    }
                }
            }
            else
            {
                btnRequestRefund.Visible = false;
            }
        }

    }
}