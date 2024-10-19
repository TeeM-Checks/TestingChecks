using NewBilletterie.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewBilletterie
{
    public partial class MobileMainMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["DesktopViewLink"] = "../ExternalPages/ViewTickets.aspx";

                //UpdateCaptchaText();
                if (Session["userObjectCookie"] != null)
                {
                    NewBilletterie.BilletterieAPIWS.userProfileObject usrSession = new NewBilletterie.BilletterieAPIWS.userProfileObject();
                    usrSession = (NewBilletterie.BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                }
                else
                {
                    Response.Redirect("~/MobileIndex.aspx", false);
                }

                //LoadDropDowns();
                //if (bool.Parse(ConfigurationManager.AppSettings["HideOldLinks"]))
                //{
                //    btnViewOldTickets.Visible = false;
                //}

                //if (bool.Parse(ConfigurationManager.AppSettings["ShowUserGroup"]))
                //{
                //    lblUserGroup.Visible = true;
                //    ddlUserGroup.Visible = true;
                //}


            }
        }

        protected void btnViewTickets_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Mobile/MobileViewTickets.aspx", false);
        }

        protected void btnCreateNewTicket_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Mobile/MobileNewTicket.aspx", false);
        }

        protected void btnFAQs_Click(object sender, EventArgs e)
        {

        }

    }
}