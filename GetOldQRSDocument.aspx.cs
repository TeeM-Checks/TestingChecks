using NewBilletterie.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewBilletterie
{
    public partial class GetOldQRSDocument : System.Web.UI.Page
    {

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
            downloadDocumentObject doc = new downloadDocumentObject();
            if (sObjID != null)
            {
                doc = GetOldLocalFileSystemDocument(sObjID);
                if (doc.noError)
                {
                    Response.Redirect(ConfigurationManager.AppSettings["OldDownloadPathURL"] + doc.fileName, true);
                }
            }
        }

        private downloadDocumentObject GetOldLocalFileSystemDocument(string documentUniqueID)
        {
            downloadDocumentObject returnValue = new downloadDocumentObject();
            DataAccess da = new DataAccess();
            DataTable dt = new DataTable();
            dt = da.GetGenericMySQLDataTable("select id,filename, filepath, users_id, tickets_id, date_mod, sha1sum from glpi_documents where sha1sum = '" + documentUniqueID + "'");
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    returnValue.noError = true;
                    returnValue.fileName = dt.Rows[0]["filepath"].ToString();
                }
            }
            return returnValue;
        }


    }
}