using NewBilletterie.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
using NewBilletterie.BilletterieAPIWS;


namespace NewBilletterie
{
    public partial class GetDocument : System.Web.UI.Page
    {

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        public class downloadDocumentObject
        {
            public string repositoryID;
            public string mimeType;
            public string fileName;
            public byte[] docObj;
            private bool _noError;
            private string _errorMessage;

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string sObjID = Request.QueryString["docID"];
            string sImageObjID = Request.QueryString["imageField"];
            downloadDocumentObject doc = new downloadDocumentObject();
            if (sObjID != null)
            {
                if (ValidateAntiXSS(sObjID) && ValidateDocumentName(sObjID))
                {
                    #region Get document from DB
                    doc = GetLocalFileSystemDocument(sObjID);
                    if (doc.noError)
                    {
                        //Check if local
                        if (Request.Url != null)
                        {
                            if (Request.Url.ToString().ToLower().Contains(ConfigurationManager.AppSettings["LocalIPAddress"]))
                            {
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                Response.ContentType = "application/pdf";
                                Response.BinaryWrite(doc.docObj);
                                Response.Flush();
                                Response.End();
                                //Response.Redirect(ConfigurationManager.AppSettings["LocalDownloadPathURL"] + doc.fileName, true);
                            }
                            else
                            {
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                Response.ContentType = "application/pdf";
                                Response.BinaryWrite(doc.docObj);
                                Response.Flush();
                                Response.End();
                                //Response.Redirect(ConfigurationManager.AppSettings["DownloadPathURL"] + doc.fileName, true);
                            }
                        }
                    }
                    else
                    {
                        lblError.Visible = true;
                    }
                    #endregion
                }
            }
            else if (sImageObjID != null)
            {
                if (ValidateAntiXSS(sImageObjID))
                {
                    if (Session["summaryPopupDocument"] != null)
                    {
                        byte[] sImageDocObjID = (byte[])Session["summaryPopupDocument"];
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = "application/pdf";
                        Response.BinaryWrite(sImageDocObjID);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.Text = "Document is empty";
                    }
                }
            }
        }

        private downloadDocumentObject GetLocalFileSystemDocument(string documentUniqueID)
        {
            downloadDocumentObject returnValue = new downloadDocumentObject();
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetLocalFileSystemDocument(documentUniqueID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_DCM_Document", "TB_DCM_DocumentDS", "select DCM_OriginalName, DCM_DerivedName, DCM_FileField from TB_DCM_Document where DCM_UniqueID = '" + documentUniqueID + "'");
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    returnValue.noError = true;
                    returnValue.fileName = ds.Tables[0].Rows[0]["DCM_DerivedName"].ToString();
                    if (ds.Tables[0].Rows[0]["DCM_FileField"].ToString() != "")
                    {
                        returnValue.docObj = (byte[])ds.Tables[0].Rows[0]["DCM_FileField"];
                    }
                    else
                    {
                        returnValue.noError = false;
                    }
                }
            }
            return returnValue;
        }

        private bool ValidateDocumentName(string inputParameter)
        {
            bool returnValue = false;
            if (inputParameter.Length == 36)
                returnValue = true;
            if (inputParameter.Contains(" "))
                returnValue = false;
            if (inputParameter.Contains(";"))
                returnValue = false;
            if (inputParameter.Contains("<"))
                returnValue = false;
            if (inputParameter.Contains(">"))
                returnValue = false;
            if (inputParameter.Contains("="))
                returnValue = false;
            if (inputParameter.Contains("@"))
                returnValue = false;
            if (inputParameter.Contains("!"))
                returnValue = false;
          
            if (!GuidEx.IsGuid(inputParameter))
            {
                returnValue = false;
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

        public static class GuidEx
        {
            public static bool IsGuid(string value)
            {
                Guid x;
                return Guid.TryParse(value, out x);
            }
        }


    }
}