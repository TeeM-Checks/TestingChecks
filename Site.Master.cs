using NewBilletterie.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

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
            litOrganisationName.Text = ConfigurationManager.AppSettings["OrganisationNameTop"];
            litSystemTitle.Text = ConfigurationManager.AppSettings["SystemTitle"];
            litSloganText.Text = ConfigurationManager.AppSettings["SloganText"];

            if (bool.Parse(ConfigurationManager.AppSettings["UseDefaultLabels"]) == false)
            {
                try
                {
                    //btnNewOfficeTicket.Text = ConfigurationManager.AppSettings["btnNewOfficeTicketText"];
                    btnNewTicket.Text = ConfigurationManager.AppSettings["btnNewTicketText"];
                    btnViewTickets.Text = ConfigurationManager.AppSettings["btnViewTicketsText"];  
                    //btnViewOfficeTickets.Text = ConfigurationManager.AppSettings["btnViewOfficeTicketsText"]; 
                    //btnAdministration.Text = ConfigurationManager.AppSettings["btnAdministrationText"]; 
                    //btnReports.Text = ConfigurationManager.AppSettings["btnReportsText"];  
                    //btnFAQAdmin.Text = ConfigurationManager.AppSettings["btnFAQAdminText"];  
                    btnFAQ.Text = ConfigurationManager.AppSettings["btnFAQText"];  
                    
                    //btnMobileView.Text = ConfigurationManager.AppSettings["btnMobileViewText"];  

                    //btnSettings.Text = ConfigurationManager.AppSettings["btnSettings"]; 

                }
                catch (Exception)
                {

                }
            }

            imgMainLogo.ImageUrl = ConfigurationManager.AppSettings["MainLogoPath"];
            if (bool.Parse(ConfigurationManager.AppSettings["FixMainLogoWidth"]))
            {
                imgMainLogo.Width = Int32.Parse(ConfigurationManager.AppSettings["MainLogoWidth"]);
            }
            imgMainLogo.Height = Int32.Parse(ConfigurationManager.AppSettings["MainLogoHeight"]);

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

            mainCSSLink.Href = ConfigurationManager.AppSettings["MainCSSPath"];

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
                #region Display notice
                if (Session["noticeViewed"] != null)
                {
                    if (bool.Parse(Session["noticeViewed"].ToString()))
                    {
                        pnlUserInfo.Visible = false;
                        lnkMasterNotice.Text = "";
                    }
                    else
                    {
                        try
                        {

                            DataAccess ipDA = new DataAccess();
                            DataSet dsNotice = new DataSet();
                            dsNotice = bilAPIWS.GetGenericIPOnlineDataSet("TB_SNT_SiteNotice", "TB_SNT_SiteNoticeDS", "select * from TB_SNT_SiteNotice where SNT_SystemID in (0,2) and SNT_VisibleToPublic = 1 and SNT_NoticeDateFrom <= '" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "' and SNT_NoticeDateTo >= '" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //dsNotice = ipDA.GetGenericIPOnlineDataSet("TB_SNT_SiteNotice", "TB_SNT_SiteNoticeDS", "select * from TB_SNT_SiteNotice where SNT_SystemID in (0,2) and SNT_VisibleToPublic = 1 and SNT_NoticeDateFrom <= '" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "' and SNT_NoticeDateTo >= '" + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "'");
                            if (dsNotice != null)
                            {
                                if (dsNotice.Tables[0] != null)
                                {
                                    if (dsNotice.Tables[0].Rows.Count > 0)
                                    {
                                        pnlUserInfo.Visible = true;
                                        for (int i = 0; i < dsNotice.Tables[0].Rows.Count; i++)
                                        {
                                            lnkMasterNotice.Text = lnkMasterNotice.Text + dsNotice.Tables[0].Rows[i]["SNT_NoticeText"].ToString() + "<br/><br/>";
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    try
                    {
                        Session["noticeViewed"] = "false";
                        DataAccess ipDA = new DataAccess();
                        DataSet dsNotice = new DataSet();
                        dsNotice = bilAPIWS.GetGenericIPOnlineDataSet("TB_SNT_SiteNotice", "TB_SNT_SiteNoticeDS", "select * from TB_SNT_SiteNotice where SNT_SystemID in (0,2) and SNT_NoticeDateFrom <= '" + FormatDateTimeSQL(DateTime.Now.ToString()) + "' and SNT_NoticeDateTo > '" + FormatDateTimeSQL(DateTime.Now.ToString()) + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //dsNotice = ipDA.GetGenericIPOnlineDataSet("TB_SNT_SiteNotice", "TB_SNT_SiteNoticeDS", "select * from TB_SNT_SiteNotice where SNT_SystemID in (0,2) and SNT_NoticeDateFrom <= '" + FormatDateTimeSQL(DateTime.Now.ToString()) + "' and SNT_NoticeDateTo > '" + FormatDateTimeSQL(DateTime.Now.ToString()) + "'");
                        if (dsNotice != null)
                        {
                            if (dsNotice.Tables[0] != null)
                            {
                                if (dsNotice.Tables[0].Rows.Count > 0)
                                {
                                    pnlUserInfo.Visible = true;
                                    for (int i = 0; i < dsNotice.Tables[0].Rows.Count; i++)
                                    {
                                        lnkMasterNotice.Text = lnkMasterNotice.Text + dsNotice.Tables[0].Rows[i]["SNT_NoticeText"].ToString() + "<br/><br/>";
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
                #endregion


 
                if (Session["userObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();
                    usrProfile = (BilletterieAPIWS.userProfileObject)Session["userObjectCookie"];
                    btnLogout.Visible = true;
                    btnLogout.Text = "Logout " + usrProfile.USR_FirstName;
                    btnFAQ.Visible = true;

                    if (usrProfile.OFL_PKID != 0)
                    {
                        if (usrProfile.OFL_PKID == 3)
                        {
                            btnNewTicket.Visible = false;
                            btnViewTickets.Visible = false;
                            //btnMobileView.Visible = false;
                            btnFAQ.Visible = false;
                        }
                        else
                        {
                            btnNewTicket.Visible = true;
                            btnViewTickets.Visible = true;
                            //btnMobileView.Visible = true;
                            btnFAQ.Visible = true;
                        }
                    }
                    else
                    {
                        btnNewTicket.Visible = true;
                        btnViewTickets.Visible = true;
                        //btnMobileView.Visible = true;
                        btnFAQ.Visible = true;
                    }
                }

                if (Session["officerObjectCookie"] != null)
                {
                    BilletterieAPIWS.userProfileObject usrProfile = new BilletterieAPIWS.userProfileObject();
                    usrProfile = (BilletterieAPIWS.userProfileObject)Session["officerObjectCookie"];
                    btnLogout.Visible = true;
                    btnLogout.Text = "Logout " + usrProfile.USR_FirstName;
                    //btnNewOfficeTicket.Visible = true;
                    //btnSettings.Visible = true;
                    //btnViewOfficeTickets.Visible = true;
                    //if (usrProfile.OFL_PKID == 1 || usrProfile.OFL_PKID == 2)
                    //{
                    //    btnAdministration.Visible = true;
                    //}
                    //else if (usrProfile.OFC_CanEdit)
                    //{
                    //    btnAdministration.Visible = true;
                    //}
                    //else
                    //{
                    //    btnAdministration.Visible = false;
                    //}

                    //if (usrProfile.OFC_IsApprover == 1 || usrProfile.OFC_IsApprover == 2 || usrProfile.OFC_IsApprover == 3 || usrProfile.OFC_IsApprover == 4)
                    //{
                    //    btnFinanceRefunds.Visible = true;
                    //    btnFAQAdmin.Visible = false;
                    //}
                    //else
                    //{
                    //    btnFinanceRefunds.Visible = false;
                    //    btnFAQAdmin.Visible = true;
                    //}
                    //btnReports.Visible = true;
                    
                }
            }

        }

        private string FormatDateTimeSQL(string inputDateString)
        {
            string retVaue = "";
            DateTime dDate;

            CultureInfo culture;
            DateTimeStyles styles;
            styles = DateTimeStyles.None;

            culture = CultureInfo.InvariantCulture;

            if (DateTime.TryParseExact(inputDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dDate))
            {
                String.Format("{0:yyyy-MM-dd}", dDate);
                retVaue = dDate.ToString("yyyy-MM-dd");
            }
            else
            {
                DateTime dt;
                if (DateTime.TryParseExact(inputDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    String.Format("{0:yyyy-MM-dd}", dt);
                    retVaue = dt.ToString("yyyy-MM-dd");
                }
                else
                {
                    retVaue = "";
                }
            }
            return retVaue;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            string redirectURL = "~/Index.aspx";
            if (Session["officerObjectCookie"] != null)
            {
                redirectURL = "~/Index.aspx";
            }
            else
            {
                redirectURL = "~/Index.aspx";
            }
            Session["officerObjectCookie"] = null;
            Session["userObjectCookie"] = null;
            Session["preferredEmail"] = null;
            Session["ViewResultsExport"] = null;
            Session["ViewResults"] = null;
            Session["ViewTime"] = null;

            Response.Redirect(redirectURL, false);
        }

        protected void btnNewOfficeTicket_Click(object sender, EventArgs e)
        {
            Response.Redirect("../InternalPages/NewOfficeTicket.aspx", false);
        }

        protected void btnNewTicket_Click(object sender, EventArgs e)
        {
            Response.Redirect("../ExternalPages/NewTicket.aspx", false);
        }

        protected void btnViewOfficeTickets_Click(object sender, EventArgs e)
        {
            Response.Redirect("../InternalPages/OfficeTicketView.aspx", false);
        }

        protected void btnViewTickets_Click(object sender, EventArgs e)
        {
            Response.Redirect("../ExternalPages/ViewTickets.aspx", false);
        }

        protected void btnAdministration_Click(object sender, EventArgs e)
        {
            Response.Redirect("../InternalPages/Administration.aspx", false);
        }

        protected void btnReports_Click(object sender, EventArgs e)
        {
            if (Session["officerObjectCookie"] != null)
            {
                BilletterieAPIWS.userProfileObject officerSession = new BilletterieAPIWS.userProfileObject();
                officerSession = (BilletterieAPIWS.userProfileObject)Session["officerObjectCookie"];

                //Response.Redirect("../InternalPages/NewReports.aspx", false);

                if (UserIsReportViewer(officerSession.USR_PKID.ToString()))
                {
                    Response.Redirect("../InternalPages/NewReports.aspx", false);
                }
                else
                {
                    Response.Redirect("../InternalPages/Reports.aspx", false);
                }
            }

            
        }

        private bool UserIsReportViewer(string OFCPKID)
        {
            bool returnValue = false;
            string[] stringArray = null;
            stringArray = ConfigurationManager.AppSettings["ReportViewersOFC"].ToString().Split(';');
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i] != "")
                {
                    if (OFCPKID == stringArray[i])
                    {
                        returnValue = true;
                        break;
                    }
                }
            }
            return returnValue;
        }

        protected void btnFAQ_Click(object sender, EventArgs e)
        {
            Response.Redirect("../ExternalPages/FAQ.aspx", false);
        }

        protected void btnFAQAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("../InternalPages/FAQAdmin.aspx");
        }

        protected void btnMobileView_Click(object sender, EventArgs e)
        {
            string redirectURL = "";
            if (Session["MobileViewLink"] != null)
            {
                redirectURL = (string)Session["MobileViewLink"];
                Response.Redirect(redirectURL, false);
            }

        }

        protected void btnSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect("../InternalPages/UserSettings.aspx", false);
        }

        protected void btnFinanceRefunds_Click(object sender, EventArgs e)
        {
            Response.Redirect("../InternalPages/ProcessRefunds.aspx", false);
        }


        protected void lnkMasterNotice_Click(object sender, EventArgs e)
        {
            Session["noticeViewed"] = "true";
            pnlUserInfo.Visible = false;
        }

        protected void imgExitNoticePanel_Click(object sender, ImageClickEventArgs e)
        {
            Session["noticeViewed"] = "true";
            pnlUserInfo.Visible = false;
        }


        
    }
}