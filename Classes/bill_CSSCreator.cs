using System;
using System.Collections.Generic;

using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;
using System.Drawing;


namespace NewBilletterie.Classes
{
    public class BillImages
    {
        public string Message { private set; get; }
        public bool Uploaded { private set; get; }

        public void UploadImage(FileUpload FileUpload1, string ImageType)
        {
            Uploaded = false;
            Message = string.Empty;

            if (FileUpload1.HasFile)
            {
                ///check file type
                if ((FileUpload1.PostedFile.ContentType == "image/jpg") ||
                    (FileUpload1.PostedFile.ContentType == "image/png") ||
                    (FileUpload1.PostedFile.ContentType == "image/bmp") ||
                    (FileUpload1.PostedFile.ContentType == "image/gif") ||
                    (FileUpload1.PostedFile.ContentType == "image/ico") ||
                    (FileUpload1.PostedFile.ContentType == "image/tiff"))
                {
                    ///check file size
                    if (Convert.ToInt64(FileUpload1.PostedFile.ContentLength) < 5000000)
                    {
                        //relative folder path
                        var photoFolder = HttpContext.Current.Server.MapPath("~/Images/");

                        ///check if folder exists
                        if (!Directory.Exists(photoFolder))
                        {
                            Directory.CreateDirectory(photoFolder);
                        }

                        ///give it a unique name
                        string extension = Path.GetExtension(FileUpload1.FileName);
                        string uniqueName = string.Empty;

                        ///upload
                        FileUpload1.SaveAs(Path.Combine(photoFolder, ImageType + extension));

                        Uploaded = true;
                        Message = "Image updated successfully";
                    }
                    else
                        Message = "File is too big, reduce the size to less than 5 MB and try again";
                }
                else
                    Message = "File is not an image, please select an image and try again";
            }
            else
                Message = "No file selected, please select a file and try again";
        }
        public bool DeleteImage(string StorageLocation)
        {
            bool deleted = false;

            if (File.Exists(HttpContext.Current.Server.MapPath(StorageLocation)))
            {
                File.Delete(HttpContext.Current.Server.MapPath(StorageLocation));
                deleted = true;
            }
            return deleted;
        }
    }
    public class bill_CSSCreator
    {
        public string ImagePath { set; get; }

        //CSS properties
        ///Favicon
        public string FaviconIcon { set; get; }
        
        ///Logo
        private string LogoBGImage { set; get; }
        private string LogoFontIDClass { set; get; }
        private string LogoImageIDClass { set; get; }
        private string LogoBGIDClass { set; get; }
        //Logo properties
        public string LogoBGColor { set; get; }
        public string LogoText { set; get; }
        public string LogoTextFont { set; get; }
        public string LogoTextFontColor { set; get; }

        ///Banner
        private string BannerBGImage { set; get; }
        private string BannerBGIDClass { set; get; }
        //private string BannerFontIDClass { set; get; }
        private string BannerCompanyNameFontIDClass { set; get; }
        private string BannerSystemNameFontIDClass { set; get; }
        private string BannerSystemSloganFontIDClass { set; get; }
        private string BannerImageIDClass { set; get; }
        //Banner properties
        public string BannerBGColor { set; get; }
        public string BannerCompanyNameText { set; get; }
        public string BannerCompanyNameFont { set; get; }
        public string BannerCompanyNameFontColor { set; get; }
        public string BannerSystemNameText { set; get; }
        public string BannerSystemNameFont { set; get; }
        public string BannerSystemNameFontColor { set; get; }
        public string BannerSystemSloganText { set; get; }
        public string BannerSystemSloganFont { set; get; }
        public string BannerSystemSloganFontColor { set; get; }

        ///Page Tabs
        private string PageTabsBGImage { set; get; }
        private string PageTabsFontIDClass { set; get; }
        private string PageTabsImageIDClass { set; get; }
        private string PageTabsBGIDClass { set; get; }
        //Page Tabs properies
        public string PageTabsBGColor { set; get; }
        //public string PageTabsText { set; get; }
        public string PageTabsTextFont { set; get; }
        public string PageTabsTextFontColor { set; get; }

        ///Footer
        private string FooterBGImage { set; get; }
        private string FooterFontIDClass { set; get; }
        private string FooterImageIDClass { set; get; }
        private string FooterBGIDClass { set; get; }
        //Footer properies
        public string FooterBGColor { set; get; }
        public string FooterText { set; get; }
        public string FooterTextFont { set; get; }
        public string FooterTextFontColor { set; get; }

        ///Content
        private string ContentFontIDClass { set; get; }
        private string ContentImageIDClass { set; get; }
        private string ContentBGIDClass { set; get; }
        //Page Tabs properies
        private string ContentBGImage { set; get; }
        public string ContentBGColor { set; get; }
        //public string  ContentText { set; get; }
        public string ContentTextFont { set; get; }
        public string ContentTextFontColor { set; get; }

        #region Still to Confirm


        ///ContentTabs
        public string ContentTabsFont { set; get; }
        public string ContentTabsFontColor { set; get; }

        public string ContentTabsBGColor { set; get; }
        public string ContentTabsBGImage { set; get; }

        ///Buttons
        public string ButtonsFont { set; get; }
        public string ButtonsFontColor { set; get; }

        public string ButtonsBGColor { set; get; }
        public string ButtonsBGImage { set; get; }

        ///AllText
        public string AllTextFont { set; get; }
        public string AllTextFontColor { set; get; }

        ///ContentBG

        private string CssFileName { set; get; }
        private string JSFileName { set; get; }

        #endregion

        public bill_CSSCreator()
        {
            JSFileName = HttpContext.Current.Server.MapPath("~/Scripts/bill_custom.js");
            CssFileName = HttpContext.Current.Server.MapPath("~/Styles/officeStyleCustom.css");
            ImagePath = HttpContext.Current.Server.MapPath("~/Images/");

            FaviconIcon = "favicon.ico";

            LogoBGImage = "logo2.png";
            LogoImageIDClass = "#imgMainLogo {";
            LogoFontIDClass = "div.logo_Text {";
            LogoBGIDClass = "#logo_pos {";

            BannerBGImage = "cipc_logo.jpg";
            BannerImageIDClass = "#header {";
            BannerBGIDClass = "#header {";
            BannerCompanyNameFontIDClass = "#logo_text h1 a {";
            BannerSystemNameFontIDClass = "h1#SystemName {";
            BannerSystemSloganFontIDClass = "p#SystemSlogan {";

            ContentBGImage = "global-bg.png";
            ContentImageIDClass = "body {";
            ContentBGIDClass = "body {";
            ContentFontIDClass = "*, h4, a {";

            FooterBGImage = "footer.png";
            FooterImageIDClass = "footer {";
            FooterFontIDClass = "#footer > p, #footer > p > a {";
            FooterBGIDClass = "footer {";

            PageTabsBGImage = string.Empty;
            PageTabsImageIDClass = "#menubar > table {";
            PageTabsFontIDClass = "input.toogleButtons[type='submit'] {";
            PageTabsBGIDClass = "input.toogleButtons[type='submit'] {";

            if (!File.Exists(CssFileName))
                File.Create(CssFileName);
        }

        private void UpdateFontFamily(string identifierClass, string font)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(identifierClass);
                sw.WriteLine("    font-family: " + font + ";");

                if (identifierClass == LogoFontIDClass)
                {
                    sw.WriteLine("    text-align: center;");
                    sw.WriteLine("    width: 100%;");
                }

                sw.WriteLine("}");
            }
        }
        private void UpdateFontSize(string identifierClass, string fSize)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(identifierClass);
                sw.WriteLine("    font-size: " + fSize + "px;");
                sw.WriteLine("}");
            }
        }
        private void UpdateFontColour(string identifierClass, string fColour)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(identifierClass);
                sw.WriteLine("    color: " + fColour + ";");
                sw.WriteLine("}");
            }
        }
        private void UpdateBGColour(string identifierClass, string bgColour)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                if (identifierClass == ContentBGIDClass)
                {
                    sw.WriteLine("header, #body {");
                    sw.WriteLine("    background: none;");
                    sw.WriteLine("}");
                }
                if (identifierClass == FooterBGIDClass)
                {
                    sw.WriteLine("#footer {");
                    sw.WriteLine("    background: none;");
                    sw.WriteLine("}");
                }
                
                //default BG image remove and colour update
                sw.WriteLine(identifierClass);
                sw.WriteLine("    background-color: " + bgColour + ";");
                sw.WriteLine("    background-image: none;");
                
                if (identifierClass == LogoBGIDClass)
                {
                    sw.WriteLine("    margin: 0px;");
                    sw.WriteLine("    margin-bottom: 5px;");
                }
                if (identifierClass == FooterBGIDClass)
                {
                    sw.WriteLine("    height: 100%");
                }
                //end of BG image remove
                sw.WriteLine("}");
            }
        }
        private void HideBGColour(string identifierClass)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(identifierClass);
                sw.WriteLine("    background-color: transparent;");
                sw.WriteLine("}");
            }
        }
        public void ShowBGImage(string identifierClass, string bgImage)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(identifierClass);
                sw.WriteLine("    background-color: transparent;");
                sw.WriteLine("    background-image: url('../Images/" + bgImage + "');");
                if (identifierClass == ContentBGImage)
                {
                    sw.WriteLine("    background-repeat: repeat-y;");
                    sw.WriteLine("    background-size: contain;");
                }
                sw.WriteLine("}");
            }
        }
        public void HideBGImage(string identifierClass)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(identifierClass);
                sw.WriteLine("    background-color: transparent;");
                sw.WriteLine("    background-image: none;");
                sw.WriteLine("}");
            }
        }

        public void CenterText(string identifierClass)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(identifierClass);
                sw.WriteLine("    display: flex;");
                sw.WriteLine("    align-items: center;");
                sw.WriteLine("    justify-content: center;");
                if (identifierClass == LogoFontIDClass)
                    sw.WriteLine("    min-height: 100%;");
                sw.WriteLine("}");
            }
        }

        public void LoadFontDropDown(DropDownList ddl)
        {
            ddl.Items.Add("(Not Set)");
            foreach (FontFamily ft in FontFamily.Families)
            {
                ddl.Items.Add(ft.Name.ToString());
            }
        }
        public void LoadFontSizeDropDowns(DropDownList ddl)
        {
            ddl.Items.Add("(Not Set)");
            for (int i = 6; i < 42; i += 2)
            {
                ddl.Items.Add(i.ToString());
            }
        }


        #region Logo Methods
        public void HideLogoImage()
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(LogoImageIDClass);
                sw.WriteLine("    background-image: none;");
                sw.WriteLine("}");
            }
        }
        public void HideLogoBG()
        {

            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(LogoBGIDClass);
                sw.WriteLine("    background-color: transparent;");
                sw.WriteLine("}");
            }
        }

        public void ShowLogoImage()
        {
            JSShowLogoImg();
        }
        public void UpdateLogoBG(string colour)
        {
            UpdateBGColour(LogoBGIDClass, colour);
        }
        public void UpdateLogoFontFamily(string font)
        {
            UpdateFontFamily(LogoFontIDClass, font);
        }
        public void UpdateLogoFontSize(string fSize)
        {
            UpdateFontSize(LogoFontIDClass, fSize);
        }
        public void UpdateLogoFontColour(string fColour)
        {
            UpdateFontColour(LogoFontIDClass, fColour);
        }
        public void CenterLogoText()
        {
            CenterText(LogoFontIDClass);
        }
        #endregion

        #region Banner Methods
        public void ShowBannerImage()
        {
            ShowBGImage(BannerImageIDClass, BannerBGImage);
        }
        public void UpdateBannerBGColor(string colour)
        {
            UpdateBGColour(BannerBGIDClass, colour);
        }

        public void UpdateBNRCompanyNameFontFamily(string font)
        {
            UpdateFontFamily(BannerCompanyNameFontIDClass, font);
        }
        public void UpdateBNRCompanyNameFontSize(string fSize)
        {
            UpdateFontSize(BannerCompanyNameFontIDClass, fSize);
        }
        public void UpdateBNRCompanyNameFontColour(string fColour)
        {
            UpdateFontColour(BannerCompanyNameFontIDClass, fColour);
        }
        public void UpdateBNRSystemNameFontFamily(string font)
        {
            UpdateFontFamily(BannerSystemNameFontIDClass, font);
        }
        public void UpdateBNRSystemNameFontSize(string fSize)
        {
            UpdateFontSize(BannerSystemNameFontIDClass, fSize);
        }
        public void UpdateBNRSystemNameFontColour(string fColour)
        {
            UpdateFontColour(BannerSystemNameFontIDClass, fColour);
        }
        public void UpdateBNRSystemSloganFontFamily(string font)
        {
            UpdateFontFamily(BannerSystemSloganFontIDClass, font);
        }
        public void UpdateBNRSystemSloganFontSize(string fSize)
        {
            UpdateFontSize(BannerSystemSloganFontIDClass, fSize);
        }
        public void UpdateBNRSystemSloganFontColour(string fColour)
        {
            UpdateFontColour(BannerSystemSloganFontIDClass, fColour);
        }
        public void CenterBannerText()
        {
            CenterText(BannerBGIDClass);
        }
        #endregion

        #region Content Methods
        public void HideContentBGColour()
        {
            HideBGColour(ContentBGIDClass);
        }
        public void ShowContentImage()
        {
            ShowBGImage(ContentImageIDClass, ContentBGImage);
        }
        public void UpdateContentBGColour(string colour)
        {
            UpdateBGColour(ContentBGIDClass, colour);
        }
        public void UpdateContentFontFamily(string font)
        {
            UpdateFontFamily(ContentFontIDClass, font);
        }
        public void UpdateContentFontSize(string fSize)
        {
            UpdateFontSize(ContentFontIDClass, fSize);
        }
        public void UpdateContentFontColour(string fColour)
        {
            UpdateFontColour(ContentFontIDClass, fColour);
        }
        #endregion

        #region Footer Methods
        public void HideFooterBGColour()
        {
            HideBGColour(FooterBGIDClass);
        }
        public void ShowFooterImage()
        {
            ShowBGImage(FooterImageIDClass, FooterBGImage);
        }
        public void UpdateFooterBGColour(string colour)
        {
            UpdateBGColour(FooterBGIDClass, colour);
        }
        public void UpdateFooterFontFamily(string font)
        {
            UpdateFontFamily(FooterFontIDClass, font);
        }
        public void UpdateFooterFontSize(string fSize)
        {
            UpdateFontSize(FooterFontIDClass, fSize);
        }
        public void UpdateFooterFontColour(string fColour)
        {
            UpdateFontColour(FooterFontIDClass, fColour);
        }
        #endregion


        #region PageTabs Methods
        public void HidePageTabsImage()
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(PageTabsImageIDClass);
                sw.WriteLine("    background-image: none;");
                sw.WriteLine("}");
            }
        }
        public void HidePageTabsBG()
        {

            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine(PageTabsBGIDClass);
                sw.WriteLine("    background-color: transparent;");
                sw.WriteLine("}");
            }
        }

        public void ShowPageTabsImage()
        {
            ShowBGImage(PageTabsImageIDClass, PageTabsBGImage);
        }
        public void UpdatePageTabsBG(string colour)
        {
            UpdateBGColour(PageTabsBGIDClass, colour);
        }
        public void UpdatePageTabsFontFamily(string font)
        {
            UpdateFontFamily(PageTabsFontIDClass, font);
        }
        public void UpdatePageTabsFontSize(string fSize)
        {
            UpdateFontSize(PageTabsFontIDClass, fSize);
        }
        public void UpdatePageTabsFontColour(string fColour)
        {
            UpdateFontColour(PageTabsFontIDClass, fColour);
        }
        #endregion


        public void RemoveContentBG()
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine("header, #body {");
                sw.WriteLine("    background-color: transparent;");
                sw.WriteLine("    background-image: none;");
                sw.WriteLine("}");

                sw.WriteLine("body {");
                sw.WriteLine("    background-color: transparent;");
                sw.WriteLine("    background-image: none;");
                sw.WriteLine("}");
            }
        }
        public void UpdateButtonsBG(string colour)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine("input.allButtons[type=\"submit\"] {");
                sw.WriteLine("    border: none;");
                sw.WriteLine("    border-radius: 0;");
                sw.WriteLine("    -webkit-border-radius: 0;");
                sw.WriteLine("    -moz-border-radius: 0;");
                sw.WriteLine("    background-color: " + colour + ";");
                sw.WriteLine("    padding: 9px;");
                sw.WriteLine("    min-width: 100px;");
                sw.WriteLine("    width: auto;");
                sw.WriteLine("}");
            }
        }
        public void UpdateFooterBG(string colour)
        {
            using (StreamWriter sw = new StreamWriter(CssFileName, true))
            {
                sw.WriteLine("footer {");
                sw.WriteLine("    background-color: " + colour + ";");
                sw.WriteLine("    background-image: none;");
                
                sw.WriteLine("}");
            }
        }
        private void CreateCSSFile()
        {

        }
        private void DeleteCSSFile(string fileName)
        {
            // Check if file already exists. If yes, delete it. 
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }


        public void JSRemoveLogoText()
        {
            using (StreamWriter sw = new StreamWriter(JSFileName, true))
            {
                sw.WriteLine("RemoveLogoText();");
            }
        }
        public void JSUpdateLogoText(string logoText)
        {
            using (StreamWriter sw = new StreamWriter(JSFileName, true))
            {
                sw.WriteLine("AddLogoText('" + logoText + "');");
            }

            //Center align text using CSS
            CenterLogoText();
        }


        public void JSChangeBannerText(string compName, string sysName)
        {
            using (StreamWriter sw = new StreamWriter(JSFileName, true))
            {
                sw.WriteLine("ChangeCompName('" + compName + "', '" + sysName + "');");
            }
        }
        public void JSChangeBannerSlogan(string compSlogan)
        {
            using (StreamWriter sw = new StreamWriter(JSFileName, true))
            {
                sw.WriteLine("ChangeSlogan('" + compSlogan + "');");
            }
        }
        public void JSShowLogoImg()
        {
            using (StreamWriter sw = new StreamWriter(JSFileName, true))
            {
                sw.WriteLine("ShowLogoImg();");
            }
        }


        public void JSUpdateSystemName(string systemName)
        {
            using (StreamWriter sw = new StreamWriter(JSFileName, true))
            {
                sw.WriteLine("ChangeSystemName('" + systemName + "');");
            }
        }
        public void JSUpdateSystemSlogan(string systemSlogan)
        {
            using (StreamWriter sw = new StreamWriter(JSFileName, true))
            {
                sw.WriteLine("ChangeSystemSlogan('" + systemSlogan + "');");
            }
        }

    } 

    public class Favicon
    {
        public string StorageLocation { set; get; }
        public string ImageType { private set; get; }
        private BillImages Img { set; get; }

        public Favicon()
        {
            StorageLocation = "~/Images/favicon.ico";
            ImageType = "favicon";
            Img = new BillImages();
        }

        public void UploadFavicon(FileUpload fu)
        {
            Img.UploadImage(fu, ImageType);
        }
        public bool DeleteFavicon()
        {
            return Img.DeleteImage(StorageLocation);
        }


    }
    public class Logo
    {
        public string StorageLocation { set; get; }
        public string ImageType { private set; get; }
        private BillImages Img { set; get; }
        public void UploadLogo(FileUpload fu)
        {
            Img.UploadImage(fu, ImageType);
        }
        public bool DeleteLogo()
        {
            return Img.DeleteImage(StorageLocation);
        }

        public Logo()
        {
            StorageLocation = "~/Images/logo.jpg";
            ImageType = "logo";
            Img = new BillImages();
        }
    }
    public class Banner
    {
        public string StorageLocation { set; get; }
        public string ImageType { private set; get; }
        private BillImages Img { set; get; }
        public void UploadLogo(FileUpload fu)
        {
            Img.UploadImage(fu, ImageType);
        }
        public bool DeleteLogo()
        {
            return Img.DeleteImage(StorageLocation);
        }

        public Banner()
        {
            StorageLocation = "~/Images/cipc_logo.jpg";
            ImageType = "banner";
            Img = new BillImages();
        }
    }
    public class PageTabs
    {
        public string PageTabsFont { set; get; }
        public string PageTabsFontColor { set; get; }

        public string PageTabsBGColor { set; get; }
        public string PageTabsBGImage { set; get; }
    }
    public class ContentTabs
    {

    }
    public class Buttons
    {

    }
    public class Inputs
    {

    }
    public class ContentText
    {

    }
}