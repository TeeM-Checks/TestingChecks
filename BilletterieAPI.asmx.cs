using NewBilletterie.Classes;
using NewBilletterie.CUBAServerService;
using NewBilletterie.EmailWS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services;

namespace NewBilletterie
{
    /// <summary>
    /// Summary description for BilletterieAPI
    /// </summary>
    [WebService(Namespace = "http://billetterie.sword.za/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BilletterieAPI : System.Web.Services.WebService
    {


        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                              + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				                                                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                              + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        [WebMethod(Description = "Create a new Billetterie ticket.", EnableSession = false)]
        public NewBilletterie.Classes.InsertResponseObject CreateNewTicket(ticketObject newBilletterieTicket, string userPKID, string userType, string serviceKey)
        {

            NewBilletterie.Classes.InsertResponseObject returnValue = new NewBilletterie.Classes.InsertResponseObject();
            try
            {

                DataAccess da = new DataAccess();
                Common cm = new Common();


                string userValidation = "";
                userValidation = ValidUserID(userPKID, userType);
                if (userValidation == "")
                {
                    EmailWS.OperationResponseObject opResp = new EmailWS.OperationResponseObject();

                    string errMessage = "";
                    errMessage = ValidateFilesBeforeUpload(userPKID, userType);

                    if (errMessage == "")
                    {
                        #region
                        errMessage = ValidateTicketInput(newBilletterieTicket);
                        if (errMessage == "")
                        {
                            newBilletterieTicket.TCK_FromMobile = false;
                            Classes.InsertResponseObject insResp = new Classes.InsertResponseObject();
                            UpdateResponseObject updResp = new UpdateResponseObject();
                            //Save ticket
                            insResp = da.InsertBilletterieTicketRecord(newBilletterieTicket);
                            if (!insResp.noError)
                            {
                                //Show error message
                                returnValue.errorMessage = "[" + insResp.errorMessage + "]";
                            }
                            else
                            {
                                //Update ticket number
                                string insertedPKID = insResp.insertedPKID;
                                newBilletterieTicket.TCK_PKID = Int32.Parse(insertedPKID);
                                newBilletterieTicket.TCK_DateCreated = DateTime.Now.ToString();

                                if (newBilletterieTicket.AttachedFile != null)
                                {
                                    if (newBilletterieTicket.AttachedFiles.Length > 0)
                                    {
                                        //loop to get all documents
                                        //Save document
                                        for (int i = 0; i < newBilletterieTicket.AttachedFiles.Length; i++)
                                        {
                                            insResp = da.InsertBilletterieDocumentRecord(newBilletterieTicket.AttachedFiles[i]);
                                            string destFile = cm.MoveDocuments(newBilletterieTicket.AttachedFiles[i].DCM_DocumentPath, insResp.insertedPKID.ToString());
                                            updResp = da.UpdateBilletterieDocumentRecord(insResp.insertedPKID.ToString(), destFile, insertedPKID);
                                        }
                                    }
                                }

                                string ticketNumber = GenerateTicketNumber(insertedPKID, userPKID);

                                updResp = da.UpdateBilletterieTicketRecord(insertedPKID, ticketNumber);

                                newBilletterieTicket.TCK_TicketNumber = ticketNumber;

                                //Update if file has been attached from external
                                if (newBilletterieTicket.TCK_HasFile == true)
                                {
                                    if (newBilletterieTicket.AttachedFiles != null)
                                    {
                                        if (newBilletterieTicket.AttachedFiles.Length > 0)
                                        {
                                            da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TKR_HasFile = 1 where TCK_PKID = " + insertedPKID);
                                        }
                                    }
                                }

                                if (bool.Parse(ConfigurationManager.AppSettings["GenerateCaseForm"]))
                                {

                                }

                                if (!updResp.noError)
                                {
                                    //Show error message
                                    returnValue.errorMessage = "[" + updResp.errorMessage + "]";
                                }
                                else
                                {
                                    newBilletterieTicket.TCK_TicketNumber = ticketNumber;

                                    //Assign due date and auto assign if possible
                                    if (bool.Parse(ConfigurationManager.AppSettings["AutoAssignDueDate"]))
                                    {
                                        UpdateTicketDueDate(newBilletterieTicket.TCK_PKID.ToString());
                                    }

                                    //Send emails if configured
                                    if (bool.Parse(ConfigurationManager.AppSettings["MailNotifications"]))
                                    {
                                        //Send ticket creation email
                                        bool categoryNotifies = CheckCategoryNotifiesEmail(newBilletterieTicket.CAT_PKID);
                                        string emailList = "";
                                        if (categoryNotifies)
                                        {
                                            emailList = cm.GetCategoryMembersList(newBilletterieTicket.CAT_PKID.ToString());

                                            if (emailList.Trim() != "")
                                            {
                                                EmailDispatcherService emsWS = new EmailDispatcherService();
                                                emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];

                                                EmailWS.EmailMessageObject emlObj = new EmailWS.EmailMessageObject();
                                                emlObj = PopulateNewTicketEmailObject(emailList, newBilletterieTicket);
                                                opResp = emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);
                                            }
                                        }

                                        if (bool.Parse(ConfigurationManager.AppSettings["SendEmailOnDemand"]))
                                        {
                                            EmailWS.EmailMessageObject emlObj = new EmailWS.EmailMessageObject();
                                            emlObj = PopulateEmailObject(newBilletterieTicket);
                                            SendMail sm = new SendMail();
                                            SMTPMailResponseObject smtRespObj = new SMTPMailResponseObject();

                                            #region Normal sending of emails

                                            #endregion

                                            #region Asynchronous sending of email

                                            int threadId;
                                            // Create an instance of the test class.
                                            SendMail ad = new SendMail();
                                            // Create the delegate.
                                            Classes.SendMail.AsyncMethodCaller caller = new Classes.SendMail.AsyncMethodCaller(ad.SendSMTPMailAsync);
                                            // Initiate the asychronous call.
                                            IAsyncResult result = caller.BeginInvoke(emlObj.EML_ToEmailAdmin, emlObj.EML_ToEmailList, emlObj.EML_FromEmail, emlObj.EML_Subject, emlObj.EML_MailBody, emlObj.EML_SMTPServer, "TCK_PKID", newBilletterieTicket.TCK_PKID.ToString(), "0", 5000, out threadId, null, null);
                                            Thread.Sleep(0);
                                            // Call EndInvoke to wait for the asynchronous call to complete,
                                            // and to retrieve the results.
                                            SMTPMailResponseObject smtpReturnValue = caller.EndInvoke(out threadId, result);

                                            #endregion

                                        }
                                        else
                                        {
                                            EmailDispatcherService emsWS = new EmailDispatcherService();
                                            emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];

                                            EmailWS.EmailMessageObject emlObj = new EmailWS.EmailMessageObject();
                                            emlObj = PopulateEmailObject(newBilletterieTicket);
                                            opResp = emsWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["serviceKey"]);
                                        }



                                    }

                                    returnValue.errorMessage = ""; 
                                    returnValue.noError = true;
                                }
                            }
                        }
                        else
                        {
                            returnValue.errorMessage = "[" + errMessage + "]";
                        }
                        #endregion
                    }
                    else
                    {
                        returnValue.errorMessage = errMessage;
                        returnValue.noError = false;
                    }
                }
                else
                {
                    returnValue.errorMessage = userValidation;
                    returnValue.noError = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.errorMessage = ex.StackTrace;
                returnValue.noError = false;
            }
            return returnValue;
        }

        [WebMethod(Description = "Create a new Billetterie ticket reesponse.", EnableSession = false)]
        public NewBilletterie.Classes.InsertResponseObject CreateNewTicketResp(ticketResponseObject newBilletterieTicketResp, string serviceKey)
        {
            NewBilletterie.Classes.InsertResponseObject retValue = new NewBilletterie.Classes.InsertResponseObject();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                string errMessage = "";
                errMessage = ValidateTicketResponseInput(newBilletterieTicketResp);
                if (errMessage == "")
                {
                    DataAccess da = new DataAccess();
                    ticketResponseObject tickRespObj = new ticketResponseObject();
                    tickRespObj.RST_PKID = 5;
                    retValue = da.InsertBilletterieTicketResponseRecord(newBilletterieTicketResp);
                }
            }
            else
            {
                retValue.errorMessage = "Wrong Service Key. Contact the system administrator.";
                retValue.noError = false;
            }
            return retValue;
        }

        [WebMethod(Description = "Create a new Billetterie ticket reesponse.", EnableSession = false)]
        public NewBilletterie.Classes.InsertResponseObject AddTicketResponseDocument(fileAttachmentObject ticketRespDocument, ticketResponseObject tickRespObj, string serviceKey)
        {
            NewBilletterie.Classes.InsertResponseObject retValue = new NewBilletterie.Classes.InsertResponseObject();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                //string errMessage = "";
                string errMessage = ValidateFileBeforeUpload(ticketRespDocument);
                if (errMessage == "")
                {
                    DataAccess da = new DataAccess();
                    retValue = da.InsertBilletterieDocumentRecord(ticketRespDocument);

                    if (tickRespObj.TKR_HasFile == true)
                    {
                        da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + ", TCK_HasFile = 1 where TCK_PKID = " + tickRespObj.TCK_PKID.ToString());
                    }
                    else
                    {
                        da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = " + tickRespObj.STS_PKID.ToString() + " where TCK_PKID = " + tickRespObj.TCK_PKID.ToString());
                    }

                }
            }
            else
            {
                retValue.errorMessage = "Wrong Service Key. Contact the system administrator.";
                retValue.noError = false;
            }
            return retValue;
        }

        [WebMethod(Description = "Get category list for selected deprtment", EnableSession = false)]
        public DataSet GetCategoryDataset(string deptPKID, string serviceKey)
        {
            DataSet retValue = new DataSet();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                DataAccess da = new DataAccess();
                retValue = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select 0 [CAT_PKID], 0 [CAT_Order], '' [CAT_CategoryName], '' CAT_ShortName union select CAT_PKID, CAT_Order, CAT_CategoryName, CAT_ShortName from TB_CAT_Category where CAT_MasterID = '" + deptPKID + "' and STS_PKID = 50 and CAT_Visible = 1 order by CAT_CategoryName asc");
            }
            else
            {
                retValue = null;
            }
            return retValue;
        }

        [WebMethod(Description = "Get allowed mime types", EnableSession = false)]
        public DataSet GetAllowedMimeTypesDataset(string serviceKey)
        {
            DataSet retValue = new DataSet();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                DataAccess da = new DataAccess();
                retValue = da.GetGenericBilletterieDataSet("TB_AMT_AllowedMimeType", "TB_AMT_AllowedMimeTypeDS", "select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType");
            }
            else
            {
                retValue = null;
            }
            return retValue;
        }

        [WebMethod(Description = "Get if category attachment is allowed", EnableSession = false)]
        public bool GetCategoryAttachmentRequirement(int catPKID, string serviceKey)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            SelectStringResponseObject selObj = new SelectStringResponseObject();
            if (catPKID != 0)
            {
                selObj = da.GetBilletterieGenericIntScalar("select CAT_RequireAttachment from TB_CAT_Category where CAT_PKID =  " + catPKID.ToString());
                returnValue = bool.Parse(selObj.selectedPKID);
            }
            return returnValue;
        }

        [WebMethod(Description = "Get province dataset", EnableSession = false)]
        public DataSet GetProvinceDataset(string serviceKey)
        {
            DataSet retValue = new DataSet();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                DataAccess da = new DataAccess();
                retValue = da.GetGenericBilletterieDataSet("TB_PRV_Province", "TB_PRV_ProvinceDS", "select 0 PRV_PKID, '' PRV_Province union select PRV_PKID, PRV_Province  from TB_PRV_Province");
            }
            else
            {
                retValue = null;
            }
            return retValue;
        }

        [WebMethod(Description = "Get BLT user PKID", EnableSession = false)]
        public string GetBilletterieUSERRPKID(string username, string userPasword, string userSource, string serviceKey)
        {
            Common cm = new Common();
            string retValue = "0";
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                DataAccess da = new DataAccess();
                DataSet dsUserDS = new DataSet();
                dsUserDS = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID from TB_USR_User where USR_UserLogin = '" + username + "' and USR_PassKey = '" + userPasword + "'");

                if (dsUserDS != null)
                {
                    if (dsUserDS.Tables.Count > 0)
                    {
                        if (dsUserDS.Tables[0].Rows.Count > 0)
                        {
                            retValue = dsUserDS.Tables[0].Rows[0]["USR_PKID"].ToString().Trim();
                        }
                        //Get users from CUBA
                        else
                        {
                            userProfileObject usrProfile = new userProfileObject();
                            usrProfile = cm.GetCUBAUserProfileObject(username, userPasword);
                            if (usrProfile.noError)
                            {
                                #region Authenticate from CUBA record
                                if (usrProfile.USR_UserLogin.ToLower() == username.ToLower() && usrProfile.USR_PassKey.ToLower() == userPasword.ToLower())
                                {
                                    if (!IsEmail(usrProfile.USR_EmailAccount))
                                    {

                                    }
                                    else
                                    {
                                        retValue = usrProfile.USR_PKID.ToString();

                                        #region Record user log details

                                        #endregion
                                    }
                                }
                                #endregion
                            }
                        }
                    }

                    
                }
            }
            return retValue;
        }

        [WebMethod(Description = "Get category list for selected deprtment", EnableSession = false)]
        public DataSet GetUserTicketsDataset(string userPKID, bool viewOpenOnly, string serviceKey)
        {
            DataSet retValue = new DataSet();
            try
            {
                if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
                {
                    DataAccess da = new DataAccess();
                    if (viewOpenOnly)
                    {
                        retValue = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName, TCK_Reference from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5) and TCK_Viewable = 1 and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc");
                    }
                    else
                    {
                        retValue = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, S.STS_StatusName, TCK_DateCreated, TCK_DateClosed, TCK_AlternateEmail, C.CAT_CategoryName, TCK_Reference from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where T.STS_PKID in (1,2,3,4,5,6,7,8) and TCK_Viewable = 1 and USR_PKID = " + userPKID.ToString() + " order by TCK_PKID desc");
                    }
                }
                else
                {
                    retValue = null;
                }
            }
            catch (Exception ex)
            {
                retValue = null;

            }
            return retValue;
        }

        [WebMethod(Description = "Get category list for selected deprtment", EnableSession = false)]
        public bool ValidMimeType(string mimeType, string fileExtention, string serviceKey)
        {
            bool mimeTypeFound = false;
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = da.GetGenericBilletterieDataSet("TB_AMT_AllowedMimeType", "TB_AMT_AllowedMimeTypeDS", "select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (mimeType.ToLower() == ds.Tables[0].Rows[i]["AMT_Name"].ToString().ToLower() || fileExtention.ToLower() == ds.Tables[0].Rows[i]["AMT_Extention"].ToString().ToLower())
                {
                    mimeTypeFound = true;
                    break;
                }
            }
            return mimeTypeFound;
        }

        [WebMethod(Description = "Get category list for selected deprtment", EnableSession = false)]
        public DataSet GetTicketDataset(string ticketPKID, string serviceKey)
        {
            DataSet retValue = new DataSet();
            try
            {
                if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
                {
                    DataAccess da = new DataAccess();
                    retValue = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select TCK_PKID, T.CAT_PKID, USR_PKID, OFC_PKID, T.TPT_PKID, SUBSTRING(TCK_TicketNumber,CHARINDEX('T',TCK_TicketNumber),LEN(TCK_TicketNumber)) as TCK_TicketNumber, TCK_Subject, TCK_Message, TCK_DateCreated, TCK_DateClosed, TCK_DateDue, TCK_AlternateEmail, TCK_UniqueID, T.STS_PKID, CAT_CategoryName, STS_StatusName, TCK_Reference from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_STS_Status S on T.STS_PKID = S.STS_PKID where TCK_PKID = " + ticketPKID);
                }
                else
                {
                    retValue = null;
                }
            }
            catch (Exception ex)
            {
                retValue = null;

            }
            return retValue;
        }

        [WebMethod(Description = "Get category list for selected deprtment", EnableSession = false)]
        public DataSet GetTicketResponseDataset(string ticketPKID, string serviceKey)
        {
            DataSet retValue = new DataSet();
            try
            {
                if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
                {
                    DataAccess da = new DataAccess();
                    retValue = da.GetGenericBilletterieDataSet("TB_TKR_TicketResponse", "TB_TKR_TicketResponseDS", "select TKR_PKID, TCK_PKID, OFC_PKID, UST_PKID, TKR_ResponseMessage, TKR_ResponseDate, TKR_VisibleToClient, R.STS_PKID, S.STS_StatusName, D.DCM_OriginalName, D.DCM_DerivedName, DCM_PKID, DCM_UniqueID from TB_TKR_TicketResponse R inner join TB_STS_Status S on R.STS_PKID  = S.STS_PKID left outer join TB_DCM_Document D on (R.TKR_PKID = D.TNK_PKID and D.DCL_PKID = 2) where TCK_PKID = " + ticketPKID + " and TKR_VisibleToClient = 1 order by TKR_PKID desc");
                }
                else
                {
                    retValue = null;
                }
            }
            catch (Exception ex)
            {
                retValue = null;

            }
            return retValue;
        }

        [WebMethod(Description = "Get category list for selected deprtment", EnableSession = false)]
        public string GetTicketResponseCount(string ticketPKID, string serviceKey)
        {
            string returnValue = "";
            SelectStringResponseObject localResponse = new SelectStringResponseObject();
            try
            {
                if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
                {
                    DataAccess da = new DataAccess();
                    localResponse = da.GetBilletterieGenericIntScalar("select count(*) from TB_TKR_TicketResponse R where TCK_PKID = " + ticketPKID + " and STS_PKID = 3");
                    returnValue = localResponse.selectedPKID;

                }
                else
                {
                    returnValue = "0";
                }
            }
            catch (Exception)
            {
                returnValue = "0";

            }
            return returnValue;
        }

        [WebMethod(Description = "Get category list for selected deprtment", EnableSession = false)]
        public DataSet GetTicketDocumentsDataset(string ticketPKID, string serviceKey)
        {
            DataSet retValue = new DataSet();
            try
            {
                if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
                {
                    DataAccess da = new DataAccess();
                    retValue = da.GetGenericBilletterieDataSet("TB_DCM_Document", "TB_DCM_DocumentDS", "select DCM_UniqueID, DCM_OriginalName, DCM_PKID from TB_DCM_Document where DCT_PKID = 1 and DCL_PKID = 1 and TNK_PKID = " + ticketPKID);
                }
                else
                {
                    retValue = null;
                }
            }
            catch (Exception ex)
            {
                retValue = null;

            }
            return retValue;
        }

        [WebMethod(Description = "Close Billetterie ticket.", EnableSession = false)]
        public NewBilletterie.Classes.UpdateResponseObject CloseTicket(string ticketNumberPKID, string serviceKey)
        {
            NewBilletterie.Classes.UpdateResponseObject retValue = new NewBilletterie.Classes.UpdateResponseObject();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                DataAccess da = new DataAccess();
                da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 6, TCK_ClosedBy = 1, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + ticketNumberPKID);
            }
            else
            {
                retValue.errorMessage = "Wrong Service Key. Contact the system administrator.";
                retValue.noError = false;
            }
            return retValue;
        }

        [WebMethod(Description = "Close Billetterie ticket.", EnableSession = false)]
        public NewBilletterie.Classes.UpdateResponseObject RejectTicketResponse(string ticketNumberPKID, string serviceKey)
        {
            NewBilletterie.Classes.UpdateResponseObject retValue = new NewBilletterie.Classes.UpdateResponseObject();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                DataAccess da = new DataAccess();
                da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 3 where TCK_PKID = " + ticketNumberPKID);
            }
            else
            {
                retValue.errorMessage = "Wrong Service Key. Contact the system administrator.";
                retValue.noError = false;
            }
            return retValue;
        }

        [WebMethod(Description = "Close Billetterie ticket.", EnableSession = false)]
        public NewBilletterie.Classes.UpdateResponseObject DeleteTicket(string ticketNumberPKID, string serviceKey)
        {
            NewBilletterie.Classes.UpdateResponseObject retValue = new NewBilletterie.Classes.UpdateResponseObject();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                DataAccess da = new DataAccess();
                da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set STS_PKID = 7, TCK_DateClosed = '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TCK_PKID = " + ticketNumberPKID);
            }
            else
            {
                retValue.errorMessage = "Wrong Service Key. Contact the system administrator.";
                retValue.noError = false;
            }
            return retValue;
        }

        [WebMethod]
        public DataSet GetBilletterieDataSet(string selectStatement, string serviceKey)
        {
            DataAccess da = new DataAccess();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                return da.GetGenericBilletterieDataSet("GenericTable", "GenericDataSet", selectStatement);
            }
            else
            {
                return null;
            }
        }

        [WebMethod]
        public NewBilletterie.Classes.UpdateResponseObject UpdateBilletterieTableRecord(string _sqlStatement, string serviceKey)
        {
            NewBilletterie.Classes.UpdateResponseObject retValue = new NewBilletterie.Classes.UpdateResponseObject();
            DataAccess da = new DataAccess();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                NewBilletterie.Classes.UpdateResponseObject val = new NewBilletterie.Classes.UpdateResponseObject();
                val = da.UpdateGenericBilletterieRecord(_sqlStatement);

                retValue.errorMessage = val.errorMessage;
                retValue.noError = val.noError;
            }
            else
            {
                retValue.errorMessage = "Wrong Service Key. Contact the system administrator.";
                retValue.noError = false;
            }
            return retValue;
        }

        [WebMethod]
        public NewBilletterie.Classes.InsertResponseObject InsertBilletterieRecord(string _sqlStatement, string serviceKey)
        {
            NewBilletterie.Classes.InsertResponseObject retValue = new NewBilletterie.Classes.InsertResponseObject();
            DataAccess da = new DataAccess();
            if (serviceKey == ConfigurationManager.AppSettings["serviceKey"])
            {
                NewBilletterie.Classes.lOperationResponse val = new NewBilletterie.Classes.lOperationResponse();

                val = da.InsertBilletterieGenericRecord(_sqlStatement);
                
                retValue.errorMessage = val.errorMessage;
                retValue.noError = val.noError;
            }
            else
            {
                retValue.errorMessage = "Wrong Service Key. Contact the system administrator.";
                retValue.noError = false;
            }
            return retValue;
        }

        private string ValidateFileBeforeUpload(fileAttachmentObject ticketRespDocument)
        {
            string returnValue = "";
            try
            {
                if (!ValidLocalMimeType(ticketRespDocument.MimeType, ticketRespDocument.DCM_OriginalName))
                {
                    return returnValue = "File type not supported.";
                }
                if (ticketRespDocument.AttachmentSize > 10000000)
                {
                    return returnValue = "File is too large.";
                }
            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        public bool ValidLocalMimeType(string mimeType, string fileExtention)
        {
            bool mimeTypeFound = false;
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = da.GetGenericBilletterieDataSet("TB_AMT_AllowedMimeType", "TB_AMT_AllowedMimeTypeDS", "select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (mimeType.ToLower() == ds.Tables[0].Rows[i]["AMT_Name"].ToString().ToLower() || fileExtention.ToLower() == ds.Tables[0].Rows[i]["AMT_Extention"].ToString().ToLower())
                {
                    mimeTypeFound = true;
                    break;
                }
            }
            return mimeTypeFound;
        }

        private string ValidateTicketInput(ticketObject tckObj)
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

        private string ValidateTicketResponseInput(ticketResponseObject tckObj)
        {
            Common cm = new Common();
            string returnValue = "";

            if (tckObj != null)
            {

            }
            else
            {
                return returnValue = "Ticket object is empty.";
            }
            return returnValue;
        }

        private string ValidateFilesBeforeUpload(string userPKID, string userType)
        {
            DataAccess da = new Classes.DataAccess();
            string returnValue = "";
            try
            {
                DataSet ds = new DataSet();
                ds = da.GetTemporaryDocuments(userPKID, userType);
                int documentCount = 0;
                int totalFileSize = 0;

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        documentCount = ds.Tables[0].Rows.Count;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            totalFileSize = Int32.Parse(ds.Tables[0].Rows[i]["TPD_FileSize"].ToString());
                        }
                    }
                }

                if (documentCount > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadFiles"]))
                {
                    return returnValue = "Number of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadFiles"];
                }

                if (totalFileSize > Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
                {
                    return returnValue = "Total size of attached files cannot be more than " + ConfigurationManager.AppSettings["MaxUploadSize"] + " MB";
                }


            }
            catch (Exception ex)
            {
                returnValue = ex.Message;
            }
            return returnValue;
        }

        private string GenerateTicketNumber(string tickID, string userPKID)
        {
            string returnValue = "";
            try
            {
                returnValue = userPKID;
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
            DataAccess da = new DataAccess();
            SelectStringResponseObject selObj = new SelectStringResponseObject();
            selObj = da.GetBilletterieGenericIntScalar("select USR_PKID from TB_TCK_Ticket where TCK_PKID = " + tckPKID);
            string returnValue = selObj.selectedPKID;
            return returnValue;
        }

        private EmailWS.EmailMessageObject PopulateEmailObject(ticketObject tckObj)
        {
            EmailWS.EmailMessageObject returnValue = new EmailWS.EmailMessageObject();

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

        private string GetConfirmationEmailBody(ticketObject tckObj)
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
            ds = da.GetGenericBilletterieDataSet("TB_SVL_ServiceLevel", "TB_SVL_ServiceLevelDS", "select V.SVL_Hours, T.TCK_DateCreated from TB_TCK_Ticket T inner join TB_CAT_Category C on T.CAT_PKID = C.CAT_PKID inner join TB_SVL_ServiceLevel V on C.SVL_PKID = V.SVL_PKID where TCK_PKID = " + ticketPKID);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DateTime newCalculatedDate = DateTime.Parse(ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString()).AddHours(double.Parse(ds.Tables[0].Rows[0]["SVL_Hours"].ToString()));
                    string newDateString = "";
                    if (bool.Parse(ConfigurationManager.AppSettings["GetCalculatedDates"]))
                    {
                        ServerServices svrWS = new ServerServices();
                        svrWS.Url = ConfigurationManager.AppSettings["CUBAServerServiceURL"];

                        DataSet fbdDS = new DataSet();
                        fbdDS = svrWS.GetGenericDataSet("select * from TB_FBD_ForbiddenDates where FBD_Year = " + DateTime.Now.Year.ToString() + " and FBD_ForbiddenDate between '" + DateTime.Parse(ds.Tables[0].Rows[0]["TCK_DateCreated"].ToString()).ToString("yyyy-MM-dd") + "' and '" + newCalculatedDate.ToString("yyyy-MM-dd") + "'", ConfigurationManager.AppSettings["serviceKey"]);
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
                        da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TCK_DateDue = '" + newDateString + "' where TCK_PKID = " + ticketPKID);
                    }
                    else
                    {
                        newDateString = GetFormattedOfficeDate(newCalculatedDate.ToString("yyyy-MM-dd"));
                        da.UpdateGenericBilletterieRecord("update TB_TCK_Ticket set TCK_DateDue = '" + newDateString + "' where TCK_PKID = " + ticketPKID);

                    }
                }
            }
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
                    ServerServices svrWS = new ServerServices();
                    svrWS.Url = ConfigurationManager.AppSettings["CUBAServerServiceURL"];

                    ds = svrWS.GetGenericDataSet("select top 1 CONVERT(VARCHAR, FBD_AllowedDate, 106) from TB_FBD_ForbiddenDates where FBD_ForbiddenDate = '" + formattedDate + "' and FBD_Deleted = 0", ConfigurationManager.AppSettings["serviceKey"]);
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

        private bool CheckCategoryNotifiesEmail(int catPKID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            SelectStringResponseObject stndResponObj = new SelectStringResponseObject();
            stndResponObj = da.GetBilletterieGenericScalar("select CAT_NotifyEmail from TB_CAT_Category where CAT_PKID = " + catPKID);
            if (stndResponObj.selectedPKID == "1" || stndResponObj.selectedPKID.ToLower() == "true")
            {
                returnValue = true;
            }
            return returnValue;
        }

        private NewBilletterie.EmailWS.EmailMessageObject PopulateNewTicketEmailObject(string emailList, ticketObject tckObj)
        {
            NewBilletterie.EmailWS.EmailMessageObject returnValue = new NewBilletterie.EmailWS.EmailMessageObject();
            returnValue.EML_ToEmailAdmin = emailList.Trim();
            returnValue.EML_ToEmailList = emailList.Trim();
            returnValue.EML_FromEmail = ConfigurationManager.AppSettings["From"];
            returnValue.EML_Subject = ConfigurationManager.AppSettings["Subject"] + ":" + tckObj.TCK_Subject;
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

        private string ValidUserID(string userPKID, string userType)
        {
            return "";

        }

        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }



    }
}
