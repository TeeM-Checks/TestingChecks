using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using NewBilletterie;

namespace NewBilletterie
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            //AuthConfig.RegisterOpenAuth();
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            try
            {
                Exception err = Server.GetLastError();
                if (err != null)
                {
                    Session.Add("LastError", err);
                }
            }
            catch (Exception) { }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started  
            Session["LastError"] = ""; //initialize the session
            if (Session["GlobalSession"] == null)
            {
                //Redirect to Welcome Page if Session is not null  
                Response.Redirect("~/Index.aspx");
            }
           
        }  

    }
}
