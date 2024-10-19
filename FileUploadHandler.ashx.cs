using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using NewBilletterie.Classes;

namespace NewBilletterie
{
    /// <summary>
    /// Summary description for FileUploadHandler
    /// </summary>
    public class FileUploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Common cmn = new Common();

            if (context.Request.Files.Count > 0)
            {
                HttpFileCollection files = context.Request.Files;

                foreach (HttpFileCollection file in files)
                {

                    //validate file

                    //Number of files
                    if (cmn.ValidateFileCount(context.Request.Files.Count))
                    {
                        //Mime type

                        //File Size
                    }
                }
                
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}