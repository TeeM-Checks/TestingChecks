using NewBilletterie.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewBilletterie
{
    public partial class Site_Mobile : System.Web.UI.MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;

            //Set master page defaults
            this.Page.Title = ConfigurationManager.AppSettings["DefaultTitle"];
            litOrganisationName.Text = ConfigurationManager.AppSettings["OrganisationName"];
            litSystemTitle.Text = ConfigurationManager.AppSettings["SystemTitle"];
            litSloganText.Text = ConfigurationManager.AppSettings["SloganText"];

            imgMainLogo.ImageUrl = ConfigurationManager.AppSettings["MainLogoPath"];
            if (bool.Parse(ConfigurationManager.AppSettings["FixMainLogoWidth"]))
            {
                imgMainLogo.Width = Int32.Parse(ConfigurationManager.AppSettings["MainLogoWidth"]);
            }
            imgMainLogo.Height = Int32.Parse(ConfigurationManager.AppSettings["MobileLogoHeight"]);

            //Populate client footer links
            if (bool.Parse(ConfigurationManager.AppSettings["ShowFooterLine"]))
            {
                litFooterLine.Text = ConfigurationManager.AppSettings["FooterLine"];
            }

            //Populate Sword Footer links
            if (bool.Parse(ConfigurationManager.AppSettings["ShowCopyrightLine"]))
            {
                litCopyrightLine1.Text = ConfigurationManager.AppSettings["CopyrightLine1"] + DateTime.Now.Year.ToString();
                litCopyrightLine2.Text = ConfigurationManager.AppSettings["CopyrightLine2"];
            }

            mainMobileCSSLink.Href = ConfigurationManager.AppSettings["MainMobileCSSPath"];

        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Session["userObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();
                    usrProfile = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    btnLogout.Visible = true;
                    btnLogout.Text = "Logout";  //+usrProfile.USR_FirstName;
                    btnDesktopView.Visible = true;
                }
            }

        }

        protected void btnDesktopView_Click(object sender, EventArgs e)
        {
            string redirectURL = "";
            if (Session["DesktopViewLink"] != null)
            {
                redirectURL = (string)Session["DesktopViewLink"];
                Response.Redirect(redirectURL, false);
            }
           
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            string redirectURL = "~/MobileIndex.aspx";
            Session["userObjectCookie"] = null;
            Session["preferredEmail"] = null;
            Session["ViewResults"] = null;
            Session["ViewTime"] = null;
            Response.Redirect(redirectURL, false);
        }
    }
}