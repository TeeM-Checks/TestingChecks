using NewBilletterie.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;


namespace NewBilletterie
{
    public partial class FAQ : System.Web.UI.Page
    {
        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();
        Common cm = new Common();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrSession = new BilletterieAPIWS.userProfileObject();
                    usrSession = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    PopulateFAQGrid(txtFAQSearch.Text);
                }
            }
        }

        protected void gridFAQs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                gridFAQs.PageIndex = e.NewPageIndex;
                dt = (DataTable)Session["ViewFAQResults"];
                gridFAQs.DataSource = dt;
                gridFAQs.DataBind();

            }
            catch (Exception)
            {

            }
        }

        protected void gridFAQs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                gridFAQs.Columns[0].HeaderText = "Department";
            }

            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Attributes.Add("style", "cursor:pointer;font-size:12px;font-weight:600;");
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#6BAB4D'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string decodedText = HttpUtility.HtmlDecode(e.Row.Cells[0].Text);
                e.Row.Cells[0].Text = decodedText;
            }
        }

        protected void gridFAQs_RowBoundOperations(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void btnFindFAQ_Click(object sender, EventArgs e)
        {
            string errorMessage = cm.ValidateInputForXSS(txtFAQSearch.Text.Trim());
            if (errorMessage == "")
            {
                PopulateFAQGrid(txtFAQSearch.Text);
                lblExtUserSearchErr.Text = "";
                lblExtUserSearchErr.Visible = false;
            }
            else
            {
                lblExtUserSearchErr.Text = errorMessage;
                lblExtUserSearchErr.Visible = true;
            }
        }

        private void PopulateFAQGrid()
        {
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select FAQ_PKID, OFC_PKID, CAT_PKID, FAQ_QuestionNumber, + '<b>Q. ' + FAQ_EntryText + '</b><br /><br /> A. ' + (select top 1 FAQ_EntryText from TB_FAQ_QuestionAnswer A where A.FAQ_QuestionNumber = Q.FAQ_QuestionNumber and A.FAQ_EntryType = 2 and Q.CAT_PKID = A.CAT_PKID) + '<br /><br />' [FAQ_EntryText], FAQ_EntryType, STS_PKID from TB_FAQ_QuestionAnswer Q where FAQ_EntryType = 1 and STS_PKID = 120 order by FAQ_QuestionNumber, FAQ_EntryType", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            if (ds != null)
            {
                Session["ViewFAQResults"] = ds.Tables[0];
                gridFAQs.DataSource = null;
                gridFAQs.DataSource = ds.Tables[0];
                gridFAQs.DataBind();
                gridFAQs.PageIndex = 0;
                lblNoOfFAQs.Text = ds.Tables[0].Rows.Count.ToString();
            }
        }

        private void PopulateFAQGrid(string freeSearchText)
        {
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select FAQ_PKID, OFC_PKID, CAT_PKID, FAQ_QuestionNumber, + '<b>Q. ' + FAQ_EntryText + '</b><br /><br /> A. ' + (select top 1 FAQ_EntryText from TB_FAQ_QuestionAnswer A where A.FAQ_QuestionNumber = Q.FAQ_QuestionNumber and A.FAQ_EntryType = 2 and Q.CAT_PKID = A.CAT_PKID) + '<br /><br />' [FAQ_EntryText], FAQ_EntryType, STS_PKID from TB_FAQ_QuestionAnswer Q where FAQ_EntryType = 1 and STS_PKID = 120 and FAQ_EntryText like '%" + txtFAQSearch.Text + "%' order by FAQ_QuestionNumber, FAQ_EntryType", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_FAQ_QuestionAnswer", "TB_FAQ_QuestionAnswerDS", "select FAQ_PKID, OFC_PKID, CAT_PKID, FAQ_QuestionNumber, + '<b>Q. ' + FAQ_EntryText + '</b><br /><br /> A. ' + (select top 1 FAQ_EntryText from TB_FAQ_QuestionAnswer A where A.FAQ_QuestionNumber = Q.FAQ_QuestionNumber and A.FAQ_EntryType = 2 and Q.CAT_PKID = A.CAT_PKID) + '<br /><br />' [FAQ_EntryText], FAQ_EntryType, STS_PKID from TB_FAQ_QuestionAnswer Q where FAQ_EntryType = 1 and STS_PKID = 120 and FAQ_EntryText like '%" + txtFAQSearch.Text + "%' order by FAQ_QuestionNumber, FAQ_EntryType");
            if (ds != null)
            {
                Session["ViewFAQResults"] = ds.Tables[0];
                gridFAQs.DataSource = null;
                gridFAQs.DataSource = ds.Tables[0];
                gridFAQs.DataBind();
                gridFAQs.PageIndex = 0;
                lblNoOfFAQs.Text = ds.Tables[0].Rows.Count.ToString();
            }
        }




    }
}