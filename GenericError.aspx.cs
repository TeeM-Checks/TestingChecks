using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NewBilletterie
{
    public partial class GenericError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Exception err = Session["LastError"] as Exception;
                if (err != null)
                {
                    Session["userObjectCookie"] = null;
                    Session["preferredEmail"] = null;
                    Response.Redirect("~/Index.aspx");
                }
            }
            catch (Exception) { }
        }
    }
}