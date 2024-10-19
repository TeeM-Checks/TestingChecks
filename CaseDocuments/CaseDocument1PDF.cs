using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections.Generic;
using NewBilletterie.Classes;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie.CaseDocuments
{
    public class CaseDocument1PDF
    {
        Common cm = new Common();
        GenericImageObject ImageObj = new GenericImageObject();

        string todayDate = string.Empty;

        string dtforce = string.Empty;
        string proprietor = string.Empty;
        string applicant = string.Empty;
        string picture = string.Empty;
        string disclaimer = string.Empty;
        string nmopponent = string.Empty;
        string opponent = string.Empty;
        string CLASS = string.Empty;
        string maindtapplication = string.Empty;
        string priority = string.Empty;
        string username = string.Empty;
        string userphone = string.Empty;
        string email = string.Empty;
        string dtapplication = string.Empty;
        string idapplications = string.Empty;
        string nmapplicant = string.Empty;
        string associated = string.Empty;
        string tinvention = string.Empty;
        string articles = string.Empty;
        string idapplication = string.Empty;
        string reference = string.Empty;
        string userid = string.Empty;
        string denomark = string.Empty;
        string addservice = string.Empty;
        string dtacceptance = string.Empty;
        string dtrelease = string.Empty;
        string idptaddition = string.Empty;
        string nmproprietor = string.Empty;
        string address = string.Empty;
        string dtregistration = string.Empty;
        string dtexpiry = string.Empty;
        string dtapplicationInwords = string.Empty;
        string applicantname = string.Empty;
        string nmpatentee = string.Empty;
        string part = string.Empty;
        string dtentry = string.Empty;
        string specif = string.Empty;
        string dtlodge = string.Empty;
        string mainidapplication = string.Empty;
        string dtdspriority = string.Empty;
        string idclass = string.Empty;
        string registrationtext = string.Empty;
        string dtregistrationInwords = string.Empty;
        string tmchecklist = string.Empty;
        string dtcomplete = string.Empty;

        string B3_associatedtm = string.Empty;
        string B3_associatedtm2 = string.Empty;
        string B7_associatedtm = string.Empty;

        string val_ChkA01 = string.Empty;
        string val_ChkA02 = string.Empty;
        string val_ChkA03 = string.Empty;
        string val_ChkA04 = string.Empty;
        string val_ChkA042 = string.Empty;
        string val_ChkA05 = string.Empty;
        string val_ChkA06 = string.Empty;
        string val_ChkA07 = string.Empty;
        string val_ChkA08 = string.Empty;
        string val_ChkA09 = string.Empty;
        string val_ChkA10 = string.Empty;
        string val_ChkA11 = string.Empty;

        string txt_ChkA01 = string.Empty;
        string txt_ChkA02 = string.Empty;
        string txt_ChkA03 = string.Empty;
        string txt_ChkA04 = string.Empty;
        string txt_ChkA042 = string.Empty;
        string txt_ChkA05 = string.Empty;
        string txt_ChkA06 = string.Empty;
        string txt_ChkA07 = string.Empty;
        string txt_ChkA08 = string.Empty;
        string txt_ChkA09 = string.Empty;
        string txt_ChkA10 = string.Empty;
        string txt_ChkA11 = string.Empty;
        string P010other = string.Empty;
        string section = string.Empty;

        string val_cb_admission = string.Empty;
        string val_cb_undertaking = string.Empty;
        string val_cb_specify = string.Empty;
        string val_cb_rectify = string.Empty;
        string val_cb_restrict = string.Empty;
        string val_cb_mark = string.Empty;
        string val_cb_priority = string.Empty;

        public string CreatePackage(string filePath, BilletterieAPIWS.ticketObject ticketObj, BilletterieAPIWS.userProfileObject usrProfObj)
        {
            string retValue = "";

            #region Tranfer parameters

            //todayDate = p_todayDate;

            //dtforce = p_dtforce;
            //proprietor = p_proprietor;
            //applicant = p_applicant;
            //picture = p_picture;
            //disclaimer = p_disclaimer;
            //nmopponent = p_nmopponent;
            //opponent = p_opponent;
            //CLASS = p_CLASS;
            //maindtapplication = p_maindtapplication;
            //priority = p_priority;
            //username = p_username;
            //userphone = p_userphone;
            //email = p_email;
            //dtapplication = p_dtapplication;
            //idapplications = p_idapplications;
            //nmapplicant = p_nmapplicant;
            //associated = p_associated;
            //tinvention = p_tinvention;
            //articles = p_articles;
            //idapplication = p_idapplication;
            //reference = p_reference;
            //userid = p_userid;
            //denomark = p_denomark;
            //addservice = p_addservice;
            //dtacceptance = p_dtacceptance;
            //dtrelease = p_dtrelease;
            //idptaddition = p_idptaddition;
            //nmproprietor = p_nmproprietor;
            //address = p_address;
            //dtregistration = p_dtregistration;
            //dtexpiry = p_dtexpiry;
            //dtapplicationInwords = p_dtapplicationInwords;
            //applicantname = p_applicantname;
            //nmpatentee = p_nmpatentee;
            //part = p_part;
            //dtentry = p_dtentry;
            //specif = p_specif;
            //dtlodge = p_dtlodge;
            //mainidapplication = p_mainidapplication;
            //dtdspriority = p_dtdspriority;
            //idclass = p_idclass;
            //registrationtext = p_registrationtext;
            //dtregistrationInwords = p_dtregistrationInwords;
            //tmchecklist = p_tmchecklist;
            //dtcomplete = p_dtcomplete;

            //B3_associatedtm = p_B3_associatedtm;
            //B3_associatedtm2 = p_B3_associatedtm2;
            //B7_associatedtm = p_B7_associatedtm;

            //val_ChkA02 = p_val_ChkA02;
            //val_ChkA03 = p_val_ChkA03;
            //val_ChkA04 = p_val_ChkA04;
            //val_ChkA042 = p_val_ChkA042;
            //val_ChkA05 = p_val_ChkA05;
            //val_ChkA06 = p_val_ChkA06;
            //val_ChkA07 = p_val_ChkA07;
            //val_ChkA08 = p_val_ChkA08;
            //val_ChkA09 = p_val_ChkA09;
            //val_ChkA10 = p_val_ChkA10;
            //val_ChkA11 = p_val_ChkA11;

            //txt_ChkA01 = p_txt_ChkA01;
            //txt_ChkA02 = p_txt_ChkA02;
            //txt_ChkA03 = p_txt_ChkA03;
            //txt_ChkA04 = p_txt_ChkA04;
            //txt_ChkA042 = p_txt_ChkA042;
            //txt_ChkA05 = p_txt_ChkA05;
            //txt_ChkA06 = p_txt_ChkA06;
            //txt_ChkA07 = p_txt_ChkA07;
            //txt_ChkA08 = p_txt_ChkA08;
            //txt_ChkA09 = p_txt_ChkA09;
            //txt_ChkA10 = p_txt_ChkA10;
            //txt_ChkA11 = p_txt_ChkA11;

            //P010other = p_P010other;

            //section = p_section;

            //val_cb_admission = p_val_cb_admission;
            //val_cb_undertaking = p_val_cb_undertaking;
            //val_cb_specify = p_val_cb_specify;
            //val_cb_rectify = p_val_cb_rectify;
            //val_cb_restrict = p_val_cb_restrict;
            //val_cb_mark = p_val_cb_mark;
            //val_cb_priority = p_val_cb_priority;
            #endregion

            Document doc = new Document(PageSize.A4);
            try
            {

                #region Declarations

                filePath = filePath.Replace(".docx", ".pdf");
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                Font fontSizeSpacer = new Font(Font.FontFamily.HELVETICA, 1, Font.NORMAL);
                Font fontSizeNormal = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
                Font fontSizeNormalItalic = new Font(Font.FontFamily.HELVETICA, 9, Font.ITALIC);
                Font fontSizeNormalGray = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.GRAY);
                Font fontSizeSmallerGray = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL, BaseColor.GRAY);
                Font fontSizeNormalBold = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD);
                BaseColor bsc = new BaseColor(System.Drawing.Color.Teal);
                Font fontSizeNormalBoldColour = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, bsc);

                #endregion

                doc.Open();

                #region Create Logo

                PdfPTable table = new PdfPTable(3);
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ConfigurationManager.AppSettings["CIPCLogoURL"]);
                img.Border = Rectangle.NO_BORDER;

                float imageWidthF = 100f;
                float imageHeightF = 90f;
                img.ScaleAbsolute(imageWidthF, imageHeightF);

                Paragraph paraEmpty = new Paragraph(" ", fontSizeNormal);
                paraEmpty.Alignment = Element.ALIGN_RIGHT;

                PdfPCell imgCell1 = new PdfPCell();
                imgCell1.AddElement(paraEmpty);
                imgCell1.Border = Rectangle.NO_BORDER;

                PdfPCell imgCell2 = new PdfPCell(img);
                imgCell2.Border = Rectangle.NO_BORDER;

                PdfPCell imgCell3 = new PdfPCell();
                imgCell3.AddElement(paraEmpty);
                imgCell3.Border = Rectangle.NO_BORDER;


                table.AddCell(imgCell1);
                table.AddCell(imgCell2);
                table.AddCell(imgCell3);

                #endregion

                #region Date Your Reference

                PdfPTable table1 = new PdfPTable(4);
                table1.DefaultCell.Border = Rectangle.NO_BORDER;
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCell1 = new PdfPCell();
                Paragraph paraDateText = new Paragraph("Date :", fontSizeNormalBoldColour);
                paraDateText.Alignment = Element.ALIGN_LEFT;
                textCell1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell1.AddElement(paraDateText);
                textCell1.Border = Rectangle.NO_BORDER;

                PdfPCell textCell2 = new PdfPCell();
                Paragraph paraDateValue = new Paragraph(todayDate, fontSizeNormal);
                paraDateValue.Alignment = Element.ALIGN_LEFT;
                textCell1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell2.AddElement(paraDateValue);
                textCell2.Border = Rectangle.NO_BORDER;

                PdfPCell textCell3 = new PdfPCell();
                Paragraph paraReferenceText = new Paragraph("Your reference:", fontSizeNormalBoldColour);
                paraReferenceText.Alignment = Element.ALIGN_LEFT;
                textCell1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell3.AddElement(paraReferenceText);
                textCell3.Border = Rectangle.NO_BORDER;

                PdfPCell textCell4 = new PdfPCell();
                Paragraph paraReferenceValue = new Paragraph(reference, fontSizeNormal);
                paraReferenceValue.Alignment = Element.ALIGN_LEFT;
                textCell1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell4.AddElement(paraReferenceValue);
                textCell4.Border = Rectangle.NO_BORDER;

                table1.AddCell(textCell1);
                table1.AddCell(textCell2);
                table1.AddCell(textCell3);
                table1.AddCell(textCell4);

                float[] pWidth1 = new float[] { 10f, 45f, 20f, 25f };
                table1.SetWidths(pWidth1);

                #endregion

                #region Address Our Reference etc

                PdfPTable table2 = new PdfPTable(2);
                table2.DefaultCell.Border = Rectangle.NO_BORDER;
                table2.WidthPercentage = 100;
                table2.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT2C1 = new PdfPCell();
                Paragraph paraAddressValue = new Paragraph(addservice, fontSizeNormal);
                paraAddressValue.Alignment = Element.ALIGN_LEFT;
                textCellT2C1.VerticalAlignment = Element.ALIGN_TOP;
                textCellT2C1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2C1.AddElement(paraAddressValue);
                textCellT2C1.Border = Rectangle.NO_BORDER;


                PdfPCell textCellT2C2 = new PdfPCell();
                Paragraph paraOtherText = new Paragraph("", fontSizeNormal);
                paraOtherText.Alignment = Element.ALIGN_LEFT;
                textCellT2C2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT2C2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2C2.AddElement(paraOtherText);
                textCellT2C2.Border = Rectangle.NO_BORDER;

                #region Our reference:

                PdfPTable table2Inner = new PdfPTable(2);
                table2Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                table2Inner.WidthPercentage = 100;
                table2Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT2InnerC1 = new PdfPCell();
                Paragraph paraOurRefText = new Paragraph("Our reference:", fontSizeNormalBoldColour);
                paraOurRefText.Alignment = Element.ALIGN_LEFT;
                textCellT2InnerC1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT2InnerC1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2InnerC1.AddElement(paraOurRefText);
                textCellT2InnerC1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT2InnerC2 = new PdfPCell();
                Paragraph paraOurRefValue = new Paragraph(idapplication, fontSizeNormal);
                paraOurRefValue.Alignment = Element.ALIGN_LEFT;
                textCellT2InnerC2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT2InnerC2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2InnerC2.AddElement(paraOurRefValue);
                textCellT2InnerC2.Border = Rectangle.NO_BORDER;

                table2Inner.AddCell(textCellT2InnerC1);
                table2Inner.AddCell(textCellT2InnerC2);

                float[] pWidth2A = new float[] { 44f, 56f };
                table2Inner.SetWidths(pWidth2A);


                #endregion

                #region Enquiries:

                PdfPTable table2Inner1 = new PdfPTable(2);
                table2Inner1.DefaultCell.Border = Rectangle.NO_BORDER;
                table2Inner1.WidthPercentage = 100;
                table2Inner1.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT2Inner1C1 = new PdfPCell();
                Paragraph paraEnquiriesText = new Paragraph("Enquiries:", fontSizeNormalBoldColour);
                paraEnquiriesText.Alignment = Element.ALIGN_LEFT;
                textCellT2Inner1C1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT2Inner1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2Inner1C1.AddElement(paraEnquiriesText);
                textCellT2Inner1C1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT2Inner1C2 = new PdfPCell();
                Paragraph paraEnquiriesValue = new Paragraph(username, fontSizeNormal);
                paraEnquiriesValue.Alignment = Element.ALIGN_LEFT;
                textCellT2Inner1C2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT2Inner1C2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2Inner1C2.AddElement(paraEnquiriesValue);
                textCellT2Inner1C2.Border = Rectangle.NO_BORDER;

                table2Inner1.AddCell(textCellT2Inner1C1);
                table2Inner1.AddCell(textCellT2Inner1C2);

                float[] pWidth2B = new float[] { 44f, 56f };
                table2Inner1.SetWidths(pWidth2B);


                #endregion

                #region Email address:

                PdfPTable table2Inner2 = new PdfPTable(2);
                table2Inner2.DefaultCell.Border = Rectangle.NO_BORDER;
                table2Inner2.WidthPercentage = 100;
                table2Inner2.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT2Inner2C1 = new PdfPCell();
                Paragraph paraEmailText = new Paragraph("Email address:", fontSizeNormalBoldColour);
                paraEmailText.Alignment = Element.ALIGN_LEFT;
                textCellT2Inner2C1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT2Inner2C1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2Inner2C1.AddElement(paraEmailText);
                textCellT2Inner2C1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT2Inner2C2 = new PdfPCell();
                Paragraph paraEmailValue = new Paragraph(email, fontSizeNormal);
                paraEmailValue.Alignment = Element.ALIGN_LEFT;
                textCellT2Inner2C2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT2Inner2C2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2Inner2C2.AddElement(paraEmailValue);
                textCellT2Inner2C2.Border = Rectangle.NO_BORDER;

                table2Inner2.AddCell(textCellT2Inner2C1);
                table2Inner2.AddCell(textCellT2Inner2C2);

                float[] pWidth2C = new float[] { 44f, 56f };
                table2Inner2.SetWidths(pWidth2C);


                #endregion

                textCellT2C2.AddElement(table2Inner);
                textCellT2C2.AddElement(table2Inner1);
                textCellT2C2.AddElement(table2Inner2);

                table2.AddCell(textCellT2C1);
                table2.AddCell(textCellT2C2);

                float[] pWidth2 = new float[] { 55f, 45f };
                table2.SetWidths(pWidth2);

                #endregion

                #region Check - 1. Endorse the meaning and derivation of the mark for further consideration of the application.

                PdfPTable tableTick1 = new PdfPTable(3);
                tableTick1.DefaultCell.Border = Rectangle.NO_BORDER;
                tableTick1.WidthPercentage = 100;
                tableTick1.HorizontalAlignment = Element.ALIGN_CENTER;

                string checkBox1ImageURL = ConfigurationManager.AppSettings["UnCheckedBoxURL"];
                if (val_ChkA10 == "1")
                {
                    checkBox1ImageURL = ConfigurationManager.AppSettings["CheckedBoxURL"];
                }
                iTextSharp.text.Image checkedBox1 = iTextSharp.text.Image.GetInstance(checkBox1ImageURL);
                float imageCheckBoxWidthF = 11f;
                float imageCheckBoxHeightF = 11f;
                checkedBox1.ScaleAbsolute(imageCheckBoxWidthF, imageCheckBoxHeightF);

                PdfPCell textCellTick11 = new PdfPCell(checkedBox1);
                textCellTick11.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick11.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick11.Border = Rectangle.NO_BORDER;

                PdfPCell textCellTick12 = new PdfPCell();
                Paragraph paraTick1Spacer = new Paragraph(" ", fontSizeNormal);
                paraTick1Spacer.Alignment = Element.ALIGN_LEFT;
                textCellTick12.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick12.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick12.AddElement(paraTick1Spacer);
                textCellTick12.Border = Rectangle.NO_BORDER;

                PdfPCell textCellTick13 = new PdfPCell();
                Paragraph paraTick1Text = new Paragraph("1.  It appears to be open to objection by virtue of Section 10 (" + section + ") of  Trade Mark Act (Act 194 of 1993)", fontSizeNormal);
                paraTick1Text.Alignment = Element.ALIGN_LEFT;
                textCellTick13.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick13.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick13.AddElement(paraTick1Text);
                textCellTick13.Border = Rectangle.NO_BORDER;

                tableTick1.AddCell(textCellTick11);
                tableTick1.AddCell(textCellTick12);
                tableTick1.AddCell(textCellTick13);

                float[] pWidthTick1 = new float[] { 4f, 2f, 94f };
                tableTick1.SetWidths(pWidthTick1);

                #endregion

                #region Values - 1. Endorse the meaning and derivation of the mark for further consideration of the application.

                PdfPTable tableTick1A = new PdfPTable(3);
                tableTick1A.DefaultCell.Border = Rectangle.NO_BORDER;
                tableTick1A.WidthPercentage = 100;
                tableTick1A.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellTick1A1 = new PdfPCell();
                Paragraph paraTick1Empty = new Paragraph("", fontSizeNormal);
                paraTick1Empty.Alignment = Element.ALIGN_LEFT;
                textCellTick1A1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick1A1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick1A1.AddElement(paraTick1Empty);
                textCellTick1A1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellTick1A2 = new PdfPCell();
                textCellTick1A2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick1A2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick1A2.AddElement(paraTick1Spacer);
                textCellTick1A2.Border = Rectangle.NO_BORDER;

                PdfPCell textCellTick1A3 = new PdfPCell();
                Paragraph paraTick1Value = new Paragraph("    " + txt_ChkA10, fontSizeNormalBold);
                paraTick1Value.Alignment = Element.ALIGN_LEFT;
                textCellTick1A3.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick1A3.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick1A3.AddElement(paraTick1Value);
                textCellTick1A3.Border = Rectangle.NO_BORDER;

                tableTick1A.AddCell(textCellTick1A1);
                tableTick1A.AddCell(textCellTick1A2);
                tableTick1A.AddCell(textCellTick1A3);

                float[] pWidthTick1A = new float[] { 4f, 2f, 94f };
                tableTick1A.SetWidths(pWidthTick1A);

                #endregion

                #region Check - 2. Lodge a power of attorney

                PdfPTable tableTick2 = new PdfPTable(4);
                tableTick2.DefaultCell.Border = Rectangle.NO_BORDER;
                tableTick2.WidthPercentage = 100;
                tableTick2.HorizontalAlignment = Element.ALIGN_CENTER;

                string checkBox2ImageURL = ConfigurationManager.AppSettings["UnCheckedBoxURL"];
                if (val_ChkA11 == "1")
                {
                    checkBox2ImageURL = ConfigurationManager.AppSettings["CheckedBoxURL"];
                }
                iTextSharp.text.Image checkedBox2 = iTextSharp.text.Image.GetInstance(checkBox2ImageURL);
                checkedBox2.ScaleAbsolute(imageCheckBoxWidthF, imageCheckBoxHeightF);

                PdfPCell textCellTick21 = new PdfPCell(checkedBox2);
                textCellTick21.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick21.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick21.Border = Rectangle.NO_BORDER;

                PdfPCell textCellTick22 = new PdfPCell();
                //Paragraph paraTick1Spacer = new Paragraph(" ", fontSizeNormal);
                //paraTick1Spacer.Alignment = Element.ALIGN_LEFT;
                textCellTick22.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick22.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick22.AddElement(paraTick1Spacer);
                textCellTick22.Border = Rectangle.NO_BORDER;

                PdfPCell textCellTick23 = new PdfPCell();
                Paragraph paraTick2Text = new Paragraph("2. ", fontSizeNormal);
                paraTick2Text.Alignment = Element.ALIGN_LEFT;
                textCellTick23.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick23.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick23.AddElement(paraTick2Text);
                textCellTick23.Border = Rectangle.NO_BORDER;

                PdfPCell textCellTick24 = new PdfPCell();
                Paragraph paraTick24Text = new Paragraph(txt_ChkA11, fontSizeNormalBold);
                paraTick24Text.Alignment = Element.ALIGN_LEFT;
                textCellTick24.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellTick24.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellTick24.AddElement(paraTick24Text);
                textCellTick24.Border = Rectangle.NO_BORDER;

                tableTick2.AddCell(textCellTick21);
                tableTick2.AddCell(textCellTick22);
                tableTick2.AddCell(textCellTick23);
                tableTick2.AddCell(textCellTick24);

                float[] pWidthTick2 = new float[] { 4f, 2f, 2.5f, 91.5f };
                tableTick2.SetWidths(pWidthTick2);

                #endregion

                doc.SetMargins(5f, 10f, 10f, 0f);

                doc.Add(table);
                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(table1);
                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(table2);
                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(new Paragraph("Dear Sir / Madam", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph("TRADE MARK APPLICATION NO. " + idapplication + " " + denomark + " in class " + idclass + " in the name of " + nmapplicant, fontSizeNormalBold));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph("The application has been refused provisionally for the reason(s) indicated hereunder –", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(tableTick1);
                doc.Add(tableTick1A);
                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(tableTick2);
                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(new Paragraph("Kindly note that in terms of Section 20 (2) of the Trade Marks Act (Act 194 of 1993) a response is required within three months from date hereof.", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph("Yours sincerely", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph("Document Unsigned / Electronically Issued", fontSizeNormalItalic));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph("………………………………………", fontSizeNormal));
                doc.Add(new Paragraph("REGISTRAR OF TRADE MARKS", fontSizeNormalBold));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeNormal));

                doc.Close();

                #region Insert water mark image

                string waterMarkText = ConfigurationManager.AppSettings["WaterMarkText"];

                string footerLine1 = "ISO 9001: 2008 Certified";
                string footerLine2 = "The dtiCampus (Block F - Entfutfukweni), 77 Meintjies Street, Sunnyside, Pretoria l P O Box 429, Pretoria, 0001";
                string footerLine3 = "Tel: +27 12 394 9973 | Fax: +27 12 394 1015 | Call Centre: 086 100 2472";
                string footerLine4 = "Website:www.cipc.co.za";

                PdfReader pdfReader = new PdfReader(filePath);
                FileStream stream = new FileStream(filePath.Replace("TEMP", ""), FileMode.OpenOrCreate);
                PdfStamper pdfStamper = new PdfStamper(pdfReader, stream);

                iTextSharp.text.Image imgWaterMark = iTextSharp.text.Image.GetInstance(ConfigurationManager.AppSettings["WaterMarkLogoURL"]);
                imgWaterMark.SetAbsolutePosition(100, 300);
                //imgWaterMark.SetAbsolutePosition(0, 0);
                float imageWaterMarkWidthF = 600f;
                float imageWaterMarkHeightF = 400f;
                //float imageWaterMarkWidthF = 839f;
                //float imageWaterMarkHeightF = 1191f;
                imgWaterMark.ScaleAbsolute(imageWaterMarkWidthF, imageWaterMarkHeightF);

                PdfContentByte waterMark;
                for (int pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
                {
                    waterMark = pdfStamper.GetOverContent(pageIndex);
                    waterMark.AddImage(imgWaterMark);
                }

                for (int pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
                {
                    //Rectangle class in iText represent geomatric representation... in this case, rectanle object would contain page geomatry     
                    Rectangle pageRectangle = pdfReader.GetPageSizeWithRotation(pageIndex);
                    //pdfcontentbyte object contains graphics and text content of page returned by pdfstamper     
                    PdfContentByte pdfData = pdfStamper.GetUnderContent(pageIndex);
                    //create fontsize for watermark     
                    pdfData.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 8);
                    //create new graphics state and assign opacity     
                    PdfGState graphicsState = new PdfGState();
                    graphicsState.FillOpacity = 0.5F;
                    //set graphics state to pdfcontentbyte     
                    pdfData.SetGState(graphicsState);
                    //set color of watermark     
                    pdfData.SetColorFill(BaseColor.BLACK);
                    //indicates start of writing of text     
                    pdfData.BeginText();
                    //show text as per position and rotation     
                    pdfData.ShowTextAligned(Element.ALIGN_BOTTOM, footerLine1, pageRectangle.Width / 2, 40f, 0);
                    pdfData.ShowTextAligned(Element.ALIGN_BOTTOM, footerLine2, 120f, 30f, 0);
                    pdfData.ShowTextAligned(Element.ALIGN_BOTTOM, footerLine3, 200, 20f, 0);
                    pdfData.ShowTextAligned(Element.ALIGN_BOTTOM, footerLine4, pageRectangle.Width / 2, 10f, 0);
                    //call endText to invalid font set     
                    pdfData.EndText();
                }

                pdfStamper.Close();
                stream.Close();
                pdfReader.Close();

                File.Delete(filePath);

                retValue = cm.Encrypt(Path.GetFileNameWithoutExtension(filePath.Replace("TEMP", "")), true);

                //File.Delete(ConfigurationManager.AppSettings["PDFDocumentsPath"] + Path.GetFileName(filePath));
                #endregion

            }
            catch (Exception ex)
            {
                retValue = ex.Message;
            }
            return retValue;
        }

    }

}