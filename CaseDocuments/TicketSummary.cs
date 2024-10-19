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
    public class TicketSummary
    {
        Common cm = new Common();

        public string CreatePackage(string filePath, ticketDetailObject ticketObj, BilletterieAPIWS.userProfileObject usrProfObj)
        {
            string retValue = "";

            Document doc = new Document(PageSize.A4);
            try
            {

                #region Declarations

                PdfWriter.GetInstance(doc, new FileStream(filePath + "T.pdf", FileMode.Create));
                Font fontSizeSpacer = new Font(Font.FontFamily.HELVETICA, 4, Font.NORMAL);
                Font fontSizeNormal = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
                Font fontSizeNormalSmall = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL);
                Font fontSizeNormalItalic = new Font(Font.FontFamily.HELVETICA, 9, Font.ITALIC);
                Font fontSizeNormalItalicSmall = new Font(Font.FontFamily.HELVETICA, 7, Font.ITALIC);
                Font fontSizeNormalBoldItalic = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLDITALIC);
                Font fontSizeNormalGray = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.GRAY);
                Font fontSizeSmallerGray = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL, BaseColor.GRAY);
                Font fontSizeNormalBold = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD);
                BaseColor bsc = new BaseColor(System.Drawing.Color.Teal);
                Font fontSizeNormalBoldColour = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, bsc);

                #endregion

                doc.Open();

                doc.SetMargins(5f, 10f, 10f, 50f);

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

                doc.Add(table);
                doc.Add(new Paragraph(" ", fontSizeSpacer));

                #region Ticket Number : Created Date:

                PdfPTable table1 = new PdfPTable(4);
                table1.DefaultCell.Border = Rectangle.NO_BORDER;
                table1.WidthPercentage = 100;
                table1.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCell1 = new PdfPCell();
                Paragraph paraTicketNoText = new Paragraph("Ticket Number :", fontSizeNormalBoldColour);
                paraTicketNoText.Alignment = Element.ALIGN_LEFT;
                textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell1.AddElement(paraTicketNoText);
                textCell1.Border = Rectangle.NO_BORDER;

                PdfPCell textCell2 = new PdfPCell();
                Paragraph paraTicketNoValue = new Paragraph(ticketObj.TCK_TicketNumber, fontSizeNormal);
                paraTicketNoValue.Alignment = Element.ALIGN_MIDDLE;
                textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell2.AddElement(paraTicketNoValue);
                textCell2.Border = Rectangle.BOX;

                PdfPCell textCell3 = new PdfPCell();
                Paragraph paraCreatedDateText = new Paragraph(" Created Date:", fontSizeNormalBoldColour);
                paraCreatedDateText.Alignment = Element.ALIGN_LEFT;
                textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell3.AddElement(paraCreatedDateText);
                textCell3.Border = Rectangle.NO_BORDER;

                PdfPCell textCell4 = new PdfPCell();
                Paragraph paraCreatedDateValue = new Paragraph(ticketObj.TCK_DateCreated, fontSizeNormal);
                paraCreatedDateValue.Alignment = Element.ALIGN_LEFT;
                textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCell4.AddElement(paraCreatedDateValue);
                textCell4.Border = Rectangle.BOX;

                table1.AddCell(textCell1);
                table1.AddCell(textCell2);
                table1.AddCell(textCell3);
                table1.AddCell(textCell4);

                float[] pWidth1 = new float[] { 15f, 35f, 15f, 35f };
                table1.SetWidths(pWidth1);

                #endregion

                doc.Add(table1);
                doc.Add(new Paragraph(" ", fontSizeSpacer));

                #region Reference: Due Date:

                PdfPTable table2 = new PdfPTable(4);
                table2.DefaultCell.Border = Rectangle.NO_BORDER;
                table2.WidthPercentage = 100;
                table2.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT2C1 = new PdfPCell();
                Paragraph paraReferenceText = new Paragraph("Reference :", fontSizeNormalBoldColour);
                paraReferenceText.Alignment = Element.ALIGN_LEFT;
                textCellT2C1.VerticalAlignment = Element.ALIGN_CENTER;
                textCellT2C1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2C1.AddElement(paraReferenceText);
                textCellT2C1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT2C2 = new PdfPCell();
                Paragraph paraReferenceValue = new Paragraph(ticketObj.TCK_Reference, fontSizeNormal);
                paraReferenceValue.Alignment = Element.ALIGN_MIDDLE;
                textCellT2C2.VerticalAlignment = Element.ALIGN_CENTER;
                textCellT2C2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2C2.AddElement(paraReferenceValue);
                textCellT2C2.Border = Rectangle.BOX;

                PdfPCell textCellT2C3 = new PdfPCell();
                Paragraph paraDueDateText = new Paragraph(" Due Date:", fontSizeNormalBoldColour);
                paraDueDateText.Alignment = Element.ALIGN_LEFT;
                textCellT2C3.VerticalAlignment = Element.ALIGN_CENTER;
                textCellT2C3.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2C3.AddElement(paraDueDateText);
                textCellT2C3.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT2C4 = new PdfPCell();
                Paragraph paraDueDateValue = new Paragraph(ticketObj.TCK_DueDate, fontSizeNormal);
                paraDueDateValue.Alignment = Element.ALIGN_LEFT;
                textCellT2C4.VerticalAlignment = Element.ALIGN_CENTER;
                textCellT2C4.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT2C4.AddElement(paraDueDateValue);
                textCellT2C4.Border = Rectangle.BOX;

                table2.AddCell(textCellT2C1);
                table2.AddCell(textCellT2C2);
                table2.AddCell(textCellT2C3);
                table2.AddCell(textCellT2C4);

                float[] pWidth2 = new float[] { 15f, 35f, 15f, 35f };
                table2.SetWidths(pWidth2);


                
                #endregion

                doc.Add(table2);
                doc.Add(new Paragraph(" ", fontSizeSpacer));

                #region Subject :

                PdfPTable tableSubject = new PdfPTable(2);
                tableSubject.DefaultCell.Border = Rectangle.NO_BORDER;
                tableSubject.WidthPercentage = 100;
                tableSubject.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textSubjectCell1 = new PdfPCell();
                Paragraph paraSubjectText = new Paragraph("Subject :", fontSizeNormalBoldColour);
                paraSubjectText.Alignment = Element.ALIGN_LEFT;
                textSubjectCell1.VerticalAlignment = Element.ALIGN_CENTER;
                textSubjectCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textSubjectCell1.AddElement(paraSubjectText);
                textSubjectCell1.Border = Rectangle.NO_BORDER;

                PdfPCell textSubjectCell2 = new PdfPCell();
                Paragraph paraSubjectValue = new Paragraph(ticketObj.TCK_Subject, fontSizeNormalBold);
                paraSubjectValue.Alignment = Element.ALIGN_MIDDLE;
                textSubjectCell2.VerticalAlignment = Element.ALIGN_CENTER;
                textSubjectCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                textSubjectCell2.AddElement(paraSubjectValue);
                textSubjectCell2.Border = Rectangle.BOX;

                tableSubject.AddCell(textSubjectCell1);
                tableSubject.AddCell(textSubjectCell2);

                float[] pWidth1A = new float[] { 15f, 85f };
                tableSubject.SetWidths(pWidth1A);

                #endregion

                doc.Add(tableSubject);
                doc.Add(new Paragraph(" ", fontSizeNormal));

                #region Enterprise Number:
                PdfPTable tableEnterprise = new PdfPTable(2);
                tableEnterprise.DefaultCell.Border = Rectangle.NO_BORDER;
                tableEnterprise.WidthPercentage = 100;
                tableEnterprise.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textEnterpriseCell1 = new PdfPCell();
                Paragraph paraEnterpriseText = new Paragraph("Enterprise No:", fontSizeNormalBoldColour);
                paraEnterpriseText.Alignment = Element.ALIGN_LEFT;
                textEnterpriseCell1.VerticalAlignment = Element.ALIGN_CENTER;
                textEnterpriseCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                textEnterpriseCell1.AddElement(paraEnterpriseText);
                textEnterpriseCell1.Border = Rectangle.NO_BORDER;

                PdfPCell textEnterpriseCell2 = new PdfPCell();
                Paragraph paraEnterpriseValue = new Paragraph();
                if (ticketObj.TCK_CaseIdentifier != null)
                {
                    paraEnterpriseValue = new Paragraph(ticketObj.TCK_CaseIdentifier, fontSizeNormalBold);
                }
                else
                {
                    paraEnterpriseValue = new Paragraph("", fontSizeNormalBold);
                }
                paraEnterpriseValue.Alignment = Element.ALIGN_MIDDLE;
                textEnterpriseCell2.VerticalAlignment = Element.ALIGN_CENTER;
                textEnterpriseCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                textEnterpriseCell2.AddElement(paraEnterpriseValue);
                textEnterpriseCell2.Border = Rectangle.BOX;

                tableEnterprise.AddCell(textEnterpriseCell1);
                tableEnterprise.AddCell(textEnterpriseCell2);

                float[] pWidth1Z = new float[] { 15f, 85f };
                tableEnterprise.SetWidths(pWidth1Z);

                #endregion

                doc.Add(tableEnterprise);
                doc.Add(new Paragraph(" ", fontSizeNormal));

                #region Client Details:

                PdfPTable table3 = new PdfPTable(3);
                table3.DefaultCell.Border = Rectangle.NO_BORDER;
                table3.WidthPercentage = 100;
                table3.HorizontalAlignment = Element.ALIGN_CENTER;

                #region Table 1
                PdfPCell textCellT3C1Main = new PdfPCell();
                Paragraph paraClientDetailsText = new Paragraph("Customer Details:", fontSizeNormalBoldColour);
                paraClientDetailsText.Alignment = Element.ALIGN_LEFT;
                textCellT3C1Main.VerticalAlignment = Element.ALIGN_CENTER;
                textCellT3C1Main.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3C1Main.AddElement(paraClientDetailsText);
                textCellT3C1Main.Border = Rectangle.NO_BORDER;

                //PdfPCell textCellT3C1Main = new PdfPCell();
                //Paragraph paraClientText = new Paragraph("Client Details:", fontSizeNormal);
                //paraClientText.Alignment = Element.ALIGN_LEFT;
                //textCellT3C1Main.VerticalAlignment = Element.ALIGN_TOP;
                //textCellT3C1Main.HorizontalAlignment = Element.ALIGN_LEFT;
                //textCellT3C1Main.AddElement(paraClientText);
                //textCellT3C1Main.Border = Rectangle.NO_BORDER;

                #region Client Details : Commented

                //PdfPTable table3_C1_1Inner = new PdfPTable(2);
                //table3_C1_1Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                //table3_C1_1Inner.WidthPercentage = 95;
                //table3_C1_1Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                //PdfPCell textCellT3InnerC1_1Inner1 = new PdfPCell();
                //Paragraph paraClientDetailsText = new Paragraph("Client Details:", fontSizeNormalBoldColour);
                //paraClientDetailsText.Alignment = Element.ALIGN_LEFT;
                //textCellT3InnerC1_1Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                //textCellT3InnerC1_1Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                //textCellT3InnerC1_1Inner1.AddElement(paraClientDetailsText);
                //textCellT3InnerC1_1Inner1.Border = Rectangle.NO_BORDER;

                //PdfPCell textCellT3InnerC1_1Inner2 = new PdfPCell();
                //Paragraph paraClientDetailsValue = new Paragraph(ticketObj.TCK_DateCreated, fontSizeNormal);
                //paraClientDetailsValue.Alignment = Element.ALIGN_LEFT;
                //textCellT3InnerC1_1Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                //textCellT3InnerC1_1Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                //textCellT3InnerC1_1Inner2.AddElement(paraClientDetailsValue);
                //textCellT3InnerC1_1Inner2.Border = Rectangle.NO_BORDER;

                //table3_C1_1Inner.AddCell(textCellT3InnerC1_1Inner1);
                //table3_C1_1Inner.AddCell(textCellT3InnerC1_1Inner2);

                //float[] pWidth3A = new float[] { 25f, 70f };
                //table3_C1_1Inner.SetWidths(pWidth3A);

                //textCellT3C1Main.AddElement(table3_C1_1Inner);
                #endregion


                #endregion

                #region Table 2

                PdfPCell textCellT3C2Main = new PdfPCell();
                textCellT3C2Main.VerticalAlignment = Element.ALIGN_CENTER;
                textCellT3C2Main.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3C2Main.AddElement(new Phrase(GetUserProfileString(ticketObj.ClientObject), fontSizeNormal));
                textCellT3C2Main.Border = Rectangle.NO_BORDER;

                #endregion

                #region Table 3

                PdfPCell textCellT3C3Main = new PdfPCell();
                Paragraph paraCategoriesEmpty = new Paragraph("", fontSizeNormal);
                paraCategoriesEmpty.Alignment = Element.ALIGN_LEFT;
                textCellT3C3Main.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT3C3Main.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3C3Main.AddElement(paraCategoriesEmpty);
                textCellT3C3Main.Border = Rectangle.NO_BORDER;

                #region Department:

                PdfPTable table3_C2_1Inner = new PdfPTable(2);
                table3_C2_1Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                table3_C2_1Inner.WidthPercentage = 100;
                table3_C2_1Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT3InnerC2_1Inner1 = new PdfPCell();
                Paragraph paraDepartmentText = new Paragraph("Department:", fontSizeNormalBoldColour);
                paraDepartmentText.Alignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_1Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT3InnerC2_1Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_1Inner1.AddElement(paraDepartmentText);
                textCellT3InnerC2_1Inner1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT3InnerC2_1Inner2 = new PdfPCell();
                Paragraph paraDepartmentValue = new Paragraph(ticketObj.CategoryObject.DepartmentName, fontSizeNormal);
                paraDepartmentValue.Alignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_1Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT3InnerC2_1Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_1Inner2.AddElement(paraDepartmentValue);
                textCellT3InnerC2_1Inner2.Border = Rectangle.NO_BORDER;

                table3_C2_1Inner.AddCell(textCellT3InnerC2_1Inner1);
                table3_C2_1Inner.AddCell(textCellT3InnerC2_1Inner2);

                float[] pWidth2A = new float[] { 30f, 70f };
                table3_C2_1Inner.SetWidths(pWidth2A);


                #endregion

                #region Category:

                PdfPTable table3_C2_2Inner = new PdfPTable(2);
                table3_C2_2Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                table3_C2_2Inner.WidthPercentage = 100;
                table3_C2_2Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT3InnerC2_2Inner1 = new PdfPCell();
                Paragraph paraCategoryText = new Paragraph("Category:", fontSizeNormalBoldColour);
                paraCategoryText.Alignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_2Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT3InnerC2_2Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_2Inner1.AddElement(paraCategoryText);
                textCellT3InnerC2_2Inner1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT3InnerC2_2Inner2 = new PdfPCell();
                Paragraph paraCategoryValue = new Paragraph(ticketObj.CategoryObject.CategoryName, fontSizeNormal);
                paraCategoryValue.Alignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_2Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT3InnerC2_2Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_2Inner2.AddElement(paraCategoryValue);
                textCellT3InnerC2_2Inner2.Border = Rectangle.NO_BORDER;

                table3_C2_2Inner.AddCell(textCellT3InnerC2_2Inner1);
                table3_C2_2Inner.AddCell(textCellT3InnerC2_2Inner2);

                float[] pWidth2B = new float[] { 30f, 70f };
                table3_C2_2Inner.SetWidths(pWidth2B);


                #endregion

                #region Sub Category:

                PdfPTable table3_C2_3Inner = new PdfPTable(2);
                table3_C2_3Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                table3_C2_3Inner.WidthPercentage = 100;
                table3_C2_3Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellT3InnerC2_3Inner1 = new PdfPCell();
                Paragraph paraSubCategoryText = new Paragraph("Sub-Category:", fontSizeNormalBoldColour);
                paraSubCategoryText.Alignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_3Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT3InnerC2_3Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_3Inner1.AddElement(paraSubCategoryText);
                textCellT3InnerC2_3Inner1.Border = Rectangle.NO_BORDER;

                PdfPCell textCellT3InnerC2_3Inner2 = new PdfPCell();
                Paragraph paraSubCategoryValue = new Paragraph(ticketObj.CategoryObject.SubCategoryName, fontSizeNormal);
                paraSubCategoryValue.Alignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_3Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                textCellT3InnerC2_3Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellT3InnerC2_3Inner2.AddElement(paraSubCategoryValue);
                textCellT3InnerC2_3Inner2.Border = Rectangle.NO_BORDER;

                table3_C2_3Inner.AddCell(textCellT3InnerC2_3Inner1);
                table3_C2_3Inner.AddCell(textCellT3InnerC2_3Inner2);

                float[] pWidth2C = new float[] { 30f, 70f };
                table3_C2_3Inner.SetWidths(pWidth2C);


                #endregion

                textCellT3C3Main.AddElement(table3_C2_1Inner);
                textCellT3C3Main.AddElement(table3_C2_2Inner);
                textCellT3C3Main.AddElement(table3_C2_3Inner);

                #endregion

                table3.AddCell(textCellT3C1Main);
                table3.AddCell(textCellT3C2Main);
                table3.AddCell(textCellT3C3Main);

                float[] pWidth3 = new float[] { 15f, 35f, 50f };
                table3.SetWidths(pWidth3);
                
                #endregion

                doc.Add(table3);
                doc.Add(new Paragraph("Message", fontSizeNormalBold));

                #region Message:

                PdfPTable tableMessage = new PdfPTable(1);
                tableMessage.DefaultCell.Border = Rectangle.NO_BORDER;
                tableMessage.WidthPercentage = 100;
                tableMessage.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell textCellMessage = new PdfPCell();
                Paragraph paraMessageValue = new Paragraph();
                if (ticketObj.TCK_HasFile)
                {
                    paraMessageValue = new Paragraph(GetAttachementString(ticketObj.TCK_Message, "File has been attached."), fontSizeNormalBold);
                }
                else
                {
                    paraMessageValue = new Paragraph(ticketObj.TCK_Message, fontSizeNormalBold);
                }
                paraMessageValue.Alignment = Element.ALIGN_LEFT;
                textCellMessage.VerticalAlignment = Element.ALIGN_CENTER;
                textCellMessage.HorizontalAlignment = Element.ALIGN_LEFT;
                textCellMessage.AddElement(paraMessageValue);
                textCellMessage.Border = Rectangle.BOX;

                tableMessage.AddCell(textCellMessage);

                float[] pWidthMessage = new float[] { 100f };
                tableMessage.SetWidths(pWidthMessage);

                #endregion

                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(tableMessage);
                doc.Add(new Paragraph("…………………………………………………………………………………………………………………………………………………", fontSizeNormal));
                doc.Add(new Paragraph(" ", fontSizeSpacer));

                #region Response Values

                //PdfPTable tableResponsesText = new PdfPTable(8);
                //PdfPTable tableResponsesValue = new PdfPTable(1);
                if (ticketObj.ResponseObject != null)
                {
                    for (int i = 0; i < ticketObj.ResponseObject.Length; i++)
                    {

                        #region Response User Status Visibility Text

                        PdfPTable tableResponsesText = new PdfPTable(8);
                        tableResponsesText.DefaultCell.Border = Rectangle.NO_BORDER;
                        tableResponsesText.WidthPercentage = 100;
                        tableResponsesText.HorizontalAlignment = Element.ALIGN_CENTER;

                        PdfPCell textCellResponses1 = new PdfPCell();
                        Paragraph paraResponseText = new Paragraph("Response Date:", fontSizeNormalBoldColour);
                        paraResponseText.Alignment = Element.ALIGN_LEFT;
                        textCellResponses1.VerticalAlignment = Element.ALIGN_CENTER;
                        textCellResponses1.HorizontalAlignment = Element.ALIGN_LEFT;
                        textCellResponses1.AddElement(paraResponseText);
                        textCellResponses1.Border = Rectangle.NO_BORDER;

                        PdfPCell textCellResponses2 = new PdfPCell();
                        Paragraph paraResponseValue = new Paragraph(ticketObj.ResponseObject[i].TKR_ResponseDate, fontSizeNormalSmall);
                        paraResponseValue.Alignment = Element.ALIGN_LEFT;
                        textCellResponses2.VerticalAlignment = Element.ALIGN_BOTTOM;
                        textCellResponses2.HorizontalAlignment = Element.ALIGN_LEFT;
                        textCellResponses2.AddElement(paraResponseValue);
                        textCellResponses2.Border = Rectangle.NO_BORDER;

                        PdfPCell textCellResponses3 = new PdfPCell();
                        Paragraph paraUserText = new Paragraph("User:", fontSizeNormalBoldColour);
                        paraUserText.Alignment = Element.ALIGN_RIGHT;
                        textCellResponses3.VerticalAlignment = Element.ALIGN_CENTER;
                        textCellResponses3.HorizontalAlignment = Element.ALIGN_RIGHT;
                        textCellResponses3.AddElement(paraUserText);
                        textCellResponses3.Border = Rectangle.NO_BORDER;

                        PdfPCell textCellResponses4 = new PdfPCell();
                        Paragraph paraUserValue = new Paragraph();
                        if (ticketObj.ResponseObject[i].UST_PKID == 1)
                        {
                            paraUserValue = new Paragraph("CLIENT", fontSizeNormal);
                        }
                        else
                        {
                            paraUserValue = new Paragraph(ticketObj.ResponseObject[i].UserNames.ToString(), fontSizeNormalSmall);
                        }
                        paraUserValue.Alignment = Element.ALIGN_LEFT;
                        textCellResponses4.VerticalAlignment = Element.ALIGN_BOTTOM;
                        textCellResponses4.HorizontalAlignment = Element.ALIGN_LEFT;
                        textCellResponses4.AddElement(paraUserValue);
                        textCellResponses4.Border = Rectangle.NO_BORDER;


                        PdfPCell textCellResponses5 = new PdfPCell();
                        Paragraph paraStatusText = new Paragraph("Status:", fontSizeNormalBoldColour);
                        paraStatusText.Alignment = Element.ALIGN_RIGHT;
                        textCellResponses5.VerticalAlignment = Element.ALIGN_CENTER;
                        textCellResponses5.HorizontalAlignment = Element.ALIGN_RIGHT;
                        textCellResponses5.AddElement(paraStatusText);
                        textCellResponses5.Border = Rectangle.NO_BORDER;

                        PdfPCell textCellResponses6 = new PdfPCell();
                        Paragraph paraStatusValue = new Paragraph(ticketObj.ResponseObject[i].StatusName, fontSizeNormalSmall);
                        paraStatusValue.Alignment = Element.ALIGN_MIDDLE;
                        textCellResponses6.VerticalAlignment = Element.ALIGN_BOTTOM;
                        textCellResponses6.HorizontalAlignment = Element.ALIGN_LEFT;
                        textCellResponses6.AddElement(paraStatusValue);
                        textCellResponses6.Border = Rectangle.NO_BORDER;

                        PdfPCell textCellResponses7 = new PdfPCell();
                        Paragraph paraVisibleText = new Paragraph("Visible:", fontSizeNormalBoldColour);
                        paraVisibleText.Alignment = Element.ALIGN_RIGHT;
                        textCellResponses7.VerticalAlignment = Element.ALIGN_CENTER;
                        textCellResponses7.HorizontalAlignment = Element.ALIGN_RIGHT;
                        textCellResponses7.AddElement(paraVisibleText);
                        textCellResponses7.Border = Rectangle.NO_BORDER;

                        PdfPCell textCellResponses8 = new PdfPCell();
                        Paragraph paraVisibleValue = new Paragraph(ticketObj.ResponseObject[i].TKR_VisibleToClient.ToString(), fontSizeNormalSmall);
                        paraVisibleValue.Alignment = Element.ALIGN_LEFT;
                        textCellResponses8.VerticalAlignment = Element.ALIGN_BOTTOM;
                        textCellResponses8.HorizontalAlignment = Element.ALIGN_LEFT;
                        textCellResponses8.AddElement(paraVisibleValue);
                        textCellResponses8.Border = Rectangle.NO_BORDER;

                        tableResponsesText.AddCell(textCellResponses1);
                        tableResponsesText.AddCell(textCellResponses2);
                        tableResponsesText.AddCell(textCellResponses3);
                        tableResponsesText.AddCell(textCellResponses4);
                        tableResponsesText.AddCell(textCellResponses5);
                        tableResponsesText.AddCell(textCellResponses6);
                        tableResponsesText.AddCell(textCellResponses7);
                        tableResponsesText.AddCell(textCellResponses8);

                        float[] pWidthResponse = new float[] { 14f, 18f, 9f, 18f, 9f, 12f, 10f, 10f };
                        tableResponsesText.SetWidths(pWidthResponse);


                        #endregion

                        doc.Add(tableResponsesText);
                        doc.Add(new Paragraph(" ", fontSizeSpacer));

                        #region Response Message Values

                        PdfPTable tableResponsesValue = new PdfPTable(1);
                        tableResponsesValue.DefaultCell.Border = Rectangle.NO_BORDER;
                        tableResponsesValue.WidthPercentage = 100;
                        tableResponsesValue.HorizontalAlignment = Element.ALIGN_CENTER;

                        PdfPCell textCellResponsesValue = new PdfPCell();
                        Paragraph paraResponsesValue = new Paragraph();
                        if (ticketObj.ResponseObject[i].UST_PKID == 1)
                        {
                            if (ticketObj.ResponseObject[i].TKR_HasFile)
                            {
                                paraResponsesValue = new Paragraph(GetAttachementString(ticketObj.ResponseObject[i].TKR_ResponseMessage, "Response has attachememt"), fontSizeNormalBoldItalic);
                            }
                            else
                            {
                                paraResponsesValue = new Paragraph(ticketObj.ResponseObject[i].TKR_ResponseMessage, fontSizeNormalBoldItalic);
                            }
                        }
                        else
                        {
                            if (ticketObj.ResponseObject[i].TKR_HasFile)
                            {
                                paraResponsesValue = new Paragraph(GetAttachementString(ticketObj.ResponseObject[i].TKR_ResponseMessage, "Response has attachememt"), fontSizeNormalItalic);
                            }
                            else
                            {
                                paraResponsesValue = new Paragraph(ticketObj.ResponseObject[i].TKR_ResponseMessage, fontSizeNormalItalic);
                            }
                        }

                        paraResponsesValue.Alignment = Element.ALIGN_LEFT;
                        textCellResponsesValue.VerticalAlignment = Element.ALIGN_CENTER;
                        textCellResponsesValue.HorizontalAlignment = Element.ALIGN_LEFT;
                        textCellResponsesValue.AddElement(paraResponsesValue);
                        textCellResponsesValue.Border = Rectangle.BOX;

                        tableResponsesValue.AddCell(textCellResponsesValue);

                        float[] pWidthResponsesValue = new float[] { 100f };
                        tableResponsesValue.SetWidths(pWidthResponsesValue);

                        #endregion

                        doc.Add(tableResponsesValue);
                        doc.Add(new Paragraph("…………………………………………………………………………………………………………………………………………………", fontSizeNormal));
                    }
                }

                #endregion

                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(new Paragraph(" ", fontSizeSpacer));
                doc.Add(new Paragraph(" ", fontSizeSpacer));

                #region Priority Type : Time to resolve:

                PdfPTable table5 = new PdfPTable(4);
                table5.DefaultCell.Border = Rectangle.NO_BORDER;
                table5.WidthPercentage = 100;
                table5.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell texttable5Cell1 = new PdfPCell();
                Paragraph paraProblemText = new Paragraph("Problem Type :", fontSizeNormalBoldColour);
                paraProblemText.Alignment = Element.ALIGN_LEFT;
                texttable5Cell1.VerticalAlignment = Element.ALIGN_CENTER;
                texttable5Cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable5Cell1.AddElement(paraProblemText);
                texttable5Cell1.Border = Rectangle.NO_BORDER;

                PdfPCell texttable5Cell2 = new PdfPCell();
                Paragraph paraProblemValue = new Paragraph(ticketObj.ProblemTypeName, fontSizeNormal);
                paraProblemValue.Alignment = Element.ALIGN_MIDDLE;
                texttable5Cell2.VerticalAlignment = Element.ALIGN_CENTER;
                texttable5Cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable5Cell2.AddElement(paraProblemValue);
                texttable5Cell2.Border = Rectangle.NO_BORDER;

                PdfPCell texttable5Cell3 = new PdfPCell();
                Paragraph paraResolveText = new Paragraph(" Time to resolve:", fontSizeNormalBoldColour);
                paraResolveText.Alignment = Element.ALIGN_LEFT;
                texttable5Cell3.VerticalAlignment = Element.ALIGN_CENTER;
                texttable5Cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable5Cell3.AddElement(paraResolveText);
                texttable5Cell3.Border = Rectangle.NO_BORDER;

                PdfPCell texttable5Cell4 = new PdfPCell();
                Paragraph paraResolveValue = new Paragraph();
                if (ticketObj.TimeToResolve != "")
                {
                    paraResolveValue = new Paragraph(ticketObj.TimeToResolve + " " + ConfigurationManager.AppSettings["ResolveTimeNotation"], fontSizeNormal);
                }
                else
                {
                    paraResolveValue = new Paragraph(ticketObj.TimeToResolve, fontSizeNormal);
                }
                paraResolveValue.Alignment = Element.ALIGN_LEFT;
                texttable5Cell4.VerticalAlignment = Element.ALIGN_CENTER;
                texttable5Cell4.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable5Cell4.AddElement(paraResolveValue);
                texttable5Cell4.Border = Rectangle.NO_BORDER;

                table5.AddCell(texttable5Cell1);
                table5.AddCell(texttable5Cell2);
                table5.AddCell(texttable5Cell3);
                table5.AddCell(texttable5Cell4);

                float[] pWidthtable51 = new float[] { 15f, 35f, 15f, 35f };
                table5.SetWidths(pWidthtable51);

                #endregion

                doc.Add(table5);
                doc.Add(new Paragraph(" ", fontSizeSpacer));

                #region Priority Date : Printed By:

                PdfPTable table6 = new PdfPTable(4);
                table6.DefaultCell.Border = Rectangle.NO_BORDER;
                table6.WidthPercentage = 100;
                table6.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell texttable6Cell1 = new PdfPCell();
                Paragraph paraPrintDateText = new Paragraph("Print Date :", fontSizeNormalBoldColour);
                paraPrintDateText.Alignment = Element.ALIGN_LEFT;
                texttable6Cell1.VerticalAlignment = Element.ALIGN_CENTER;
                texttable6Cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable6Cell1.AddElement(paraPrintDateText);
                texttable6Cell1.Border = Rectangle.NO_BORDER;

                PdfPCell texttable6Cell2 = new PdfPCell();
                Paragraph paraPrintDateValue = new Paragraph(DateTime.Now.ToString("dd-MM-yyyy"), fontSizeNormal);
                paraPrintDateValue.Alignment = Element.ALIGN_MIDDLE;
                texttable6Cell2.VerticalAlignment = Element.ALIGN_CENTER;
                texttable6Cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable6Cell2.AddElement(paraPrintDateValue);
                texttable6Cell2.Border = Rectangle.NO_BORDER;

                PdfPCell texttable6Cell3 = new PdfPCell();
                Paragraph paraPrintByText = new Paragraph(" Printed By:", fontSizeNormalBoldColour);
                paraPrintByText.Alignment = Element.ALIGN_LEFT;
                texttable6Cell3.VerticalAlignment = Element.ALIGN_CENTER;
                texttable6Cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable6Cell3.AddElement(paraPrintByText);
                texttable6Cell3.Border = Rectangle.NO_BORDER;

                PdfPCell texttable6Cell4 = new PdfPCell();
                Paragraph paraPrintByValue = new Paragraph(usrProfObj.USR_FirstName + " " + usrProfObj.USR_LastName, fontSizeNormal);
                paraPrintByValue.Alignment = Element.ALIGN_LEFT;
                texttable6Cell4.VerticalAlignment = Element.ALIGN_CENTER;
                texttable6Cell4.HorizontalAlignment = Element.ALIGN_LEFT;
                texttable6Cell4.AddElement(paraPrintByValue);
                texttable6Cell4.Border = Rectangle.NO_BORDER;

                table6.AddCell(texttable6Cell1);
                table6.AddCell(texttable6Cell2);
                table6.AddCell(texttable6Cell3);
                table6.AddCell(texttable6Cell4);

                float[] pWidthtable61 = new float[] { 15f, 35f, 15f, 35f };
                table6.SetWidths(pWidthtable61);

                #endregion

                doc.Add(table6);
                doc.Add(new Paragraph(" ", fontSizeSpacer));

                doc.Close();

                #region Insert water mark image

                string waterMarkText = ConfigurationManager.AppSettings["WaterMarkText"];

                string footerLine1 = ConfigurationManager.AppSettings["docFooterLine1"]; // "ISO 9001: 2008 Certified";
                string footerLine2 = ConfigurationManager.AppSettings["docFooterLine2"]; //"The dtiCampus (Block F - Entfutfukweni), 77 Meintjies Street, Sunnyside, Pretoria l P O Box 429, Pretoria, 0001";
                string footerLine3 = ConfigurationManager.AppSettings["docFooterLine3"]; //"Tel: +27 12 394 9973 | Fax: +27 12 394 1015 | Call Centre: 086 100 2472";
                string footerLine4 = ConfigurationManager.AppSettings["docFooterLine4"]; //"Website:www.cipc.co.za";

                PdfReader pdfReader = new PdfReader(filePath + "T.pdf");
                FileStream stream = new FileStream(filePath + ".pdf", FileMode.OpenOrCreate);
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
                    pdfData.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 60);
                    //create new graphics state and assign opacity     
                    PdfGState graphicsState = new PdfGState();
                    graphicsState.FillOpacity = 0.3F;
                    //set graphics state to pdfcontentbyte     
                    pdfData.SetGState(graphicsState);
                    //set color of watermark     
                    pdfData.SetColorFill(BaseColor.GRAY);
                    //indicates start of writing of text     
                    pdfData.BeginText();
                    //show text as per position and rotation     
                    pdfData.ShowTextAligned(Element.ALIGN_CENTER, waterMarkText, pageRectangle.Width / 2, pageRectangle.Height / 2, 45);
                    //call endText to invalid font set     
                    pdfData.EndText();
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

                #endregion

                File.Delete(filePath + "T.pdf");
                retValue = Path.GetFileName(filePath + ".pdf");
            }
            catch (Exception ex)
            {
                retValue = ex.Message;
            }
            return retValue;
        }

        public downloadDocumentObject CreatePackageTemp(ticketDetailObject ticketObj, BilletterieAPIWS.userProfileObject usrProfObj)
        {
            downloadDocumentObject returnValue = new downloadDocumentObject();
            byte[] retValue = null;
            byte[] retValueTemp = null;
            try
            {
                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    #region Create main document
                    Document doc = new Document(PageSize.A4);

                    #region Declarations
                    PdfWriter myPDFWriter = PdfWriter.GetInstance(doc, myMemoryStream);
                    Font fontSizeSpacer = new Font(Font.FontFamily.HELVETICA, 4, Font.NORMAL);
                    Font fontSizeNormal = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
                    Font fontSizeNormalSmall = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL);
                    Font fontSizeNormalItalic = new Font(Font.FontFamily.HELVETICA, 9, Font.ITALIC);
                    Font fontSizeNormalItalicSmall = new Font(Font.FontFamily.HELVETICA, 7, Font.ITALIC);
                    Font fontSizeNormalBoldItalic = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLDITALIC);
                    Font fontSizeNormalGray = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.GRAY);
                    Font fontSizeSmallerGray = new Font(Font.FontFamily.HELVETICA, 7, Font.NORMAL, BaseColor.GRAY);
                    Font fontSizeNormalBold = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD);
                    BaseColor bsc = new BaseColor(System.Drawing.Color.Teal);
                    Font fontSizeNormalBoldColour = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, bsc);

                    #endregion

                    doc.Open();

                    doc.SetMargins(5f, 10f, 10f, 50f);

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

                    doc.Add(table);
                    doc.Add(new Paragraph(" ", fontSizeSpacer));

                    #region Ticket Number : Created Date:

                    PdfPTable table1 = new PdfPTable(4);
                    table1.DefaultCell.Border = Rectangle.NO_BORDER;
                    table1.WidthPercentage = 100;
                    table1.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textCell1 = new PdfPCell();
                    Paragraph paraTicketNoText = new Paragraph("Ticket Number :", fontSizeNormalBoldColour);
                    paraTicketNoText.Alignment = Element.ALIGN_LEFT;
                    textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                    textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCell1.AddElement(paraTicketNoText);
                    textCell1.Border = Rectangle.NO_BORDER;

                    PdfPCell textCell2 = new PdfPCell();
                    Paragraph paraTicketNoValue = new Paragraph(ticketObj.TCK_TicketNumber, fontSizeNormal);
                    paraTicketNoValue.Alignment = Element.ALIGN_MIDDLE;
                    textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                    textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCell2.AddElement(paraTicketNoValue);
                    textCell2.Border = Rectangle.BOX;

                    PdfPCell textCell3 = new PdfPCell();
                    Paragraph paraCreatedDateText = new Paragraph(" Created Date:", fontSizeNormalBoldColour);
                    paraCreatedDateText.Alignment = Element.ALIGN_LEFT;
                    textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                    textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCell3.AddElement(paraCreatedDateText);
                    textCell3.Border = Rectangle.NO_BORDER;

                    PdfPCell textCell4 = new PdfPCell();
                    Paragraph paraCreatedDateValue = new Paragraph(ticketObj.TCK_DateCreated, fontSizeNormal);
                    paraCreatedDateValue.Alignment = Element.ALIGN_LEFT;
                    textCell1.VerticalAlignment = Element.ALIGN_CENTER;
                    textCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCell4.AddElement(paraCreatedDateValue);
                    textCell4.Border = Rectangle.BOX;

                    table1.AddCell(textCell1);
                    table1.AddCell(textCell2);
                    table1.AddCell(textCell3);
                    table1.AddCell(textCell4);

                    float[] pWidth1 = new float[] { 15f, 35f, 15f, 35f };
                    table1.SetWidths(pWidth1);

                    #endregion

                    doc.Add(table1);
                    doc.Add(new Paragraph(" ", fontSizeSpacer));

                    #region Reference: Due Date:

                    PdfPTable table2 = new PdfPTable(4);
                    table2.DefaultCell.Border = Rectangle.NO_BORDER;
                    table2.WidthPercentage = 100;
                    table2.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textCellT2C1 = new PdfPCell();
                    Paragraph paraReferenceText = new Paragraph("Reference :", fontSizeNormalBoldColour);
                    paraReferenceText.Alignment = Element.ALIGN_LEFT;
                    textCellT2C1.VerticalAlignment = Element.ALIGN_CENTER;
                    textCellT2C1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT2C1.AddElement(paraReferenceText);
                    textCellT2C1.Border = Rectangle.NO_BORDER;

                    PdfPCell textCellT2C2 = new PdfPCell();
                    Paragraph paraReferenceValue = new Paragraph(ticketObj.TCK_Reference, fontSizeNormal);
                    paraReferenceValue.Alignment = Element.ALIGN_MIDDLE;
                    textCellT2C2.VerticalAlignment = Element.ALIGN_CENTER;
                    textCellT2C2.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT2C2.AddElement(paraReferenceValue);
                    textCellT2C2.Border = Rectangle.BOX;

                    PdfPCell textCellT2C3 = new PdfPCell();
                    Paragraph paraDueDateText = new Paragraph(" Due Date:", fontSizeNormalBoldColour);
                    paraDueDateText.Alignment = Element.ALIGN_LEFT;
                    textCellT2C3.VerticalAlignment = Element.ALIGN_CENTER;
                    textCellT2C3.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT2C3.AddElement(paraDueDateText);
                    textCellT2C3.Border = Rectangle.NO_BORDER;

                    PdfPCell textCellT2C4 = new PdfPCell();
                    Paragraph paraDueDateValue = new Paragraph(ticketObj.TCK_DueDate, fontSizeNormal);
                    paraDueDateValue.Alignment = Element.ALIGN_LEFT;
                    textCellT2C4.VerticalAlignment = Element.ALIGN_CENTER;
                    textCellT2C4.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT2C4.AddElement(paraDueDateValue);
                    textCellT2C4.Border = Rectangle.BOX;

                    table2.AddCell(textCellT2C1);
                    table2.AddCell(textCellT2C2);
                    table2.AddCell(textCellT2C3);
                    table2.AddCell(textCellT2C4);

                    float[] pWidth2 = new float[] { 15f, 35f, 15f, 35f };
                    table2.SetWidths(pWidth2);



                    #endregion

                    doc.Add(table2);
                    doc.Add(new Paragraph(" ", fontSizeSpacer));

                    #region Subject :

                    PdfPTable tableSubject = new PdfPTable(2);
                    tableSubject.DefaultCell.Border = Rectangle.NO_BORDER;
                    tableSubject.WidthPercentage = 100;
                    tableSubject.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textSubjectCell1 = new PdfPCell();
                    Paragraph paraSubjectText = new Paragraph("Subject :", fontSizeNormalBoldColour);
                    paraSubjectText.Alignment = Element.ALIGN_LEFT;
                    textSubjectCell1.VerticalAlignment = Element.ALIGN_CENTER;
                    textSubjectCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textSubjectCell1.AddElement(paraSubjectText);
                    textSubjectCell1.Border = Rectangle.NO_BORDER;

                    PdfPCell textSubjectCell2 = new PdfPCell();
                    Paragraph paraSubjectValue = new Paragraph(ticketObj.TCK_Subject, fontSizeNormalBold);
                    paraSubjectValue.Alignment = Element.ALIGN_MIDDLE;
                    textSubjectCell2.VerticalAlignment = Element.ALIGN_CENTER;
                    textSubjectCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    textSubjectCell2.AddElement(paraSubjectValue);
                    textSubjectCell2.Border = Rectangle.BOX;

                    tableSubject.AddCell(textSubjectCell1);
                    tableSubject.AddCell(textSubjectCell2);

                    float[] pWidth1A = new float[] { 15f, 85f };
                    tableSubject.SetWidths(pWidth1A);

                    #endregion

                    doc.Add(tableSubject);
                    doc.Add(new Paragraph(" ", fontSizeNormal));

                    #region Enterprise Number:
                    PdfPTable tableEnterprise = new PdfPTable(2);
                    tableEnterprise.DefaultCell.Border = Rectangle.NO_BORDER;
                    tableEnterprise.WidthPercentage = 100;
                    tableEnterprise.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textEnterpriseCell1 = new PdfPCell();
                    Paragraph paraEnterpriseText = new Paragraph("Enterprise No:", fontSizeNormalBoldColour);
                    paraEnterpriseText.Alignment = Element.ALIGN_LEFT;
                    textEnterpriseCell1.VerticalAlignment = Element.ALIGN_CENTER;
                    textEnterpriseCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textEnterpriseCell1.AddElement(paraEnterpriseText);
                    textEnterpriseCell1.Border = Rectangle.NO_BORDER;

                    PdfPCell textEnterpriseCell2 = new PdfPCell();
                    Paragraph paraEnterpriseValue = new Paragraph();
                    if (ticketObj.TCK_CaseIdentifier != null)
                    {
                        paraEnterpriseValue = new Paragraph(ticketObj.TCK_CaseIdentifier, fontSizeNormalBold);
                    }
                    else
                    {
                        paraEnterpriseValue = new Paragraph("", fontSizeNormalBold);
                    }
                    paraEnterpriseValue.Alignment = Element.ALIGN_MIDDLE;
                    textEnterpriseCell2.VerticalAlignment = Element.ALIGN_CENTER;
                    textEnterpriseCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    textEnterpriseCell2.AddElement(paraEnterpriseValue);
                    textEnterpriseCell2.Border = Rectangle.BOX;

                    tableEnterprise.AddCell(textEnterpriseCell1);
                    tableEnterprise.AddCell(textEnterpriseCell2);

                    float[] pWidth1Z = new float[] { 15f, 85f };
                    tableEnterprise.SetWidths(pWidth1Z);

                    #endregion

                    doc.Add(tableEnterprise);
                    doc.Add(new Paragraph(" ", fontSizeNormal));

                    #region Client Details:

                    PdfPTable table3 = new PdfPTable(3);
                    table3.DefaultCell.Border = Rectangle.NO_BORDER;
                    table3.WidthPercentage = 100;
                    table3.HorizontalAlignment = Element.ALIGN_CENTER;

                    #region Table 1
                    PdfPCell textCellT3C1Main = new PdfPCell();
                    Paragraph paraClientDetailsText = new Paragraph("Customer Details:", fontSizeNormalBoldColour);
                    paraClientDetailsText.Alignment = Element.ALIGN_LEFT;
                    textCellT3C1Main.VerticalAlignment = Element.ALIGN_CENTER;
                    textCellT3C1Main.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3C1Main.AddElement(paraClientDetailsText);
                    textCellT3C1Main.Border = Rectangle.NO_BORDER;

                    //PdfPCell textCellT3C1Main = new PdfPCell();
                    //Paragraph paraClientText = new Paragraph("Client Details:", fontSizeNormal);
                    //paraClientText.Alignment = Element.ALIGN_LEFT;
                    //textCellT3C1Main.VerticalAlignment = Element.ALIGN_TOP;
                    //textCellT3C1Main.HorizontalAlignment = Element.ALIGN_LEFT;
                    //textCellT3C1Main.AddElement(paraClientText);
                    //textCellT3C1Main.Border = Rectangle.NO_BORDER;

                    #region Client Details : Commented

                    //PdfPTable table3_C1_1Inner = new PdfPTable(2);
                    //table3_C1_1Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                    //table3_C1_1Inner.WidthPercentage = 95;
                    //table3_C1_1Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                    //PdfPCell textCellT3InnerC1_1Inner1 = new PdfPCell();
                    //Paragraph paraClientDetailsText = new Paragraph("Client Details:", fontSizeNormalBoldColour);
                    //paraClientDetailsText.Alignment = Element.ALIGN_LEFT;
                    //textCellT3InnerC1_1Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                    //textCellT3InnerC1_1Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //textCellT3InnerC1_1Inner1.AddElement(paraClientDetailsText);
                    //textCellT3InnerC1_1Inner1.Border = Rectangle.NO_BORDER;

                    //PdfPCell textCellT3InnerC1_1Inner2 = new PdfPCell();
                    //Paragraph paraClientDetailsValue = new Paragraph(ticketObj.TCK_DateCreated, fontSizeNormal);
                    //paraClientDetailsValue.Alignment = Element.ALIGN_LEFT;
                    //textCellT3InnerC1_1Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                    //textCellT3InnerC1_1Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                    //textCellT3InnerC1_1Inner2.AddElement(paraClientDetailsValue);
                    //textCellT3InnerC1_1Inner2.Border = Rectangle.NO_BORDER;

                    //table3_C1_1Inner.AddCell(textCellT3InnerC1_1Inner1);
                    //table3_C1_1Inner.AddCell(textCellT3InnerC1_1Inner2);

                    //float[] pWidth3A = new float[] { 25f, 70f };
                    //table3_C1_1Inner.SetWidths(pWidth3A);

                    //textCellT3C1Main.AddElement(table3_C1_1Inner);
                    #endregion


                    #endregion

                    #region Table 2

                    PdfPCell textCellT3C2Main = new PdfPCell();
                    textCellT3C2Main.VerticalAlignment = Element.ALIGN_CENTER;
                    textCellT3C2Main.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3C2Main.AddElement(new Phrase(GetUserProfileString(ticketObj.ClientObject), fontSizeNormal));
                    textCellT3C2Main.Border = Rectangle.NO_BORDER;

                    #endregion

                    #region Table 3

                    PdfPCell textCellT3C3Main = new PdfPCell();
                    Paragraph paraCategoriesEmpty = new Paragraph("", fontSizeNormal);
                    paraCategoriesEmpty.Alignment = Element.ALIGN_LEFT;
                    textCellT3C3Main.VerticalAlignment = Element.ALIGN_BOTTOM;
                    textCellT3C3Main.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3C3Main.AddElement(paraCategoriesEmpty);
                    textCellT3C3Main.Border = Rectangle.NO_BORDER;

                    #region Department:

                    PdfPTable table3_C2_1Inner = new PdfPTable(2);
                    table3_C2_1Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                    table3_C2_1Inner.WidthPercentage = 100;
                    table3_C2_1Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textCellT3InnerC2_1Inner1 = new PdfPCell();
                    Paragraph paraDepartmentText = new Paragraph("Department:", fontSizeNormalBoldColour);
                    paraDepartmentText.Alignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_1Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                    textCellT3InnerC2_1Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_1Inner1.AddElement(paraDepartmentText);
                    textCellT3InnerC2_1Inner1.Border = Rectangle.NO_BORDER;

                    PdfPCell textCellT3InnerC2_1Inner2 = new PdfPCell();
                    Paragraph paraDepartmentValue = new Paragraph(ticketObj.CategoryObject.DepartmentName, fontSizeNormal);
                    paraDepartmentValue.Alignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_1Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                    textCellT3InnerC2_1Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_1Inner2.AddElement(paraDepartmentValue);
                    textCellT3InnerC2_1Inner2.Border = Rectangle.NO_BORDER;

                    table3_C2_1Inner.AddCell(textCellT3InnerC2_1Inner1);
                    table3_C2_1Inner.AddCell(textCellT3InnerC2_1Inner2);

                    float[] pWidth2A = new float[] { 30f, 70f };
                    table3_C2_1Inner.SetWidths(pWidth2A);


                    #endregion

                    #region Category:

                    PdfPTable table3_C2_2Inner = new PdfPTable(2);
                    table3_C2_2Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                    table3_C2_2Inner.WidthPercentage = 100;
                    table3_C2_2Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textCellT3InnerC2_2Inner1 = new PdfPCell();
                    Paragraph paraCategoryText = new Paragraph("Category:", fontSizeNormalBoldColour);
                    paraCategoryText.Alignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_2Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                    textCellT3InnerC2_2Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_2Inner1.AddElement(paraCategoryText);
                    textCellT3InnerC2_2Inner1.Border = Rectangle.NO_BORDER;

                    PdfPCell textCellT3InnerC2_2Inner2 = new PdfPCell();
                    Paragraph paraCategoryValue = new Paragraph(ticketObj.CategoryObject.CategoryName, fontSizeNormal);
                    paraCategoryValue.Alignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_2Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                    textCellT3InnerC2_2Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_2Inner2.AddElement(paraCategoryValue);
                    textCellT3InnerC2_2Inner2.Border = Rectangle.NO_BORDER;

                    table3_C2_2Inner.AddCell(textCellT3InnerC2_2Inner1);
                    table3_C2_2Inner.AddCell(textCellT3InnerC2_2Inner2);

                    float[] pWidth2B = new float[] { 30f, 70f };
                    table3_C2_2Inner.SetWidths(pWidth2B);


                    #endregion

                    #region Sub Category:

                    PdfPTable table3_C2_3Inner = new PdfPTable(2);
                    table3_C2_3Inner.DefaultCell.Border = Rectangle.NO_BORDER;
                    table3_C2_3Inner.WidthPercentage = 100;
                    table3_C2_3Inner.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textCellT3InnerC2_3Inner1 = new PdfPCell();
                    Paragraph paraSubCategoryText = new Paragraph("Sub-Category:", fontSizeNormalBoldColour);
                    paraSubCategoryText.Alignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_3Inner1.VerticalAlignment = Element.ALIGN_BOTTOM;
                    textCellT3InnerC2_3Inner1.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_3Inner1.AddElement(paraSubCategoryText);
                    textCellT3InnerC2_3Inner1.Border = Rectangle.NO_BORDER;

                    PdfPCell textCellT3InnerC2_3Inner2 = new PdfPCell();
                    Paragraph paraSubCategoryValue = new Paragraph(ticketObj.CategoryObject.SubCategoryName, fontSizeNormal);
                    paraSubCategoryValue.Alignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_3Inner2.VerticalAlignment = Element.ALIGN_BOTTOM;
                    textCellT3InnerC2_3Inner2.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellT3InnerC2_3Inner2.AddElement(paraSubCategoryValue);
                    textCellT3InnerC2_3Inner2.Border = Rectangle.NO_BORDER;

                    table3_C2_3Inner.AddCell(textCellT3InnerC2_3Inner1);
                    table3_C2_3Inner.AddCell(textCellT3InnerC2_3Inner2);

                    float[] pWidth2C = new float[] { 30f, 70f };
                    table3_C2_3Inner.SetWidths(pWidth2C);


                    #endregion

                    textCellT3C3Main.AddElement(table3_C2_1Inner);
                    textCellT3C3Main.AddElement(table3_C2_2Inner);
                    textCellT3C3Main.AddElement(table3_C2_3Inner);

                    #endregion

                    table3.AddCell(textCellT3C1Main);
                    table3.AddCell(textCellT3C2Main);
                    table3.AddCell(textCellT3C3Main);

                    float[] pWidth3 = new float[] { 15f, 35f, 50f };
                    table3.SetWidths(pWidth3);

                    #endregion

                    doc.Add(table3);
                    doc.Add(new Paragraph("Message", fontSizeNormalBold));

                    #region Message:

                    PdfPTable tableMessage = new PdfPTable(1);
                    tableMessage.DefaultCell.Border = Rectangle.NO_BORDER;
                    tableMessage.WidthPercentage = 100;
                    tableMessage.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell textCellMessage = new PdfPCell();
                    Paragraph paraMessageValue = new Paragraph();
                    if (ticketObj.TCK_HasFile)
                    {
                        paraMessageValue = new Paragraph(GetAttachementString(ticketObj.TCK_Message, "File has been attached."), fontSizeNormalBold);
                    }
                    else
                    {
                        paraMessageValue = new Paragraph(ticketObj.TCK_Message, fontSizeNormalBold);
                    }
                    paraMessageValue.Alignment = Element.ALIGN_LEFT;
                    textCellMessage.VerticalAlignment = Element.ALIGN_CENTER;
                    textCellMessage.HorizontalAlignment = Element.ALIGN_LEFT;
                    textCellMessage.AddElement(paraMessageValue);
                    textCellMessage.Border = Rectangle.BOX;

                    tableMessage.AddCell(textCellMessage);

                    float[] pWidthMessage = new float[] { 100f };
                    tableMessage.SetWidths(pWidthMessage);

                    #endregion

                    doc.Add(new Paragraph(" ", fontSizeSpacer));
                    doc.Add(tableMessage);
                    doc.Add(new Paragraph("…………………………………………………………………………………………………………………………………………………", fontSizeNormal));
                    doc.Add(new Paragraph(" ", fontSizeSpacer));

                    #region Response Values

                    //PdfPTable tableResponsesText = new PdfPTable(8);
                    //PdfPTable tableResponsesValue = new PdfPTable(1);
                    if (ticketObj.ResponseObject != null)
                    {
                        for (int i = 0; i < ticketObj.ResponseObject.Length; i++)
                        {

                            #region Response User Status Visibility Text

                            PdfPTable tableResponsesText = new PdfPTable(8);
                            tableResponsesText.DefaultCell.Border = Rectangle.NO_BORDER;
                            tableResponsesText.WidthPercentage = 100;
                            tableResponsesText.HorizontalAlignment = Element.ALIGN_CENTER;

                            PdfPCell textCellResponses1 = new PdfPCell();
                            Paragraph paraResponseText = new Paragraph("Response Date:", fontSizeNormalBoldColour);
                            paraResponseText.Alignment = Element.ALIGN_LEFT;
                            textCellResponses1.VerticalAlignment = Element.ALIGN_CENTER;
                            textCellResponses1.HorizontalAlignment = Element.ALIGN_LEFT;
                            textCellResponses1.AddElement(paraResponseText);
                            textCellResponses1.Border = Rectangle.NO_BORDER;

                            PdfPCell textCellResponses2 = new PdfPCell();
                            Paragraph paraResponseValue = new Paragraph(ticketObj.ResponseObject[i].TKR_ResponseDate, fontSizeNormalSmall);
                            paraResponseValue.Alignment = Element.ALIGN_LEFT;
                            textCellResponses2.VerticalAlignment = Element.ALIGN_BOTTOM;
                            textCellResponses2.HorizontalAlignment = Element.ALIGN_LEFT;
                            textCellResponses2.AddElement(paraResponseValue);
                            textCellResponses2.Border = Rectangle.NO_BORDER;

                            PdfPCell textCellResponses3 = new PdfPCell();
                            Paragraph paraUserText = new Paragraph("User:", fontSizeNormalBoldColour);
                            paraUserText.Alignment = Element.ALIGN_RIGHT;
                            textCellResponses3.VerticalAlignment = Element.ALIGN_CENTER;
                            textCellResponses3.HorizontalAlignment = Element.ALIGN_RIGHT;
                            textCellResponses3.AddElement(paraUserText);
                            textCellResponses3.Border = Rectangle.NO_BORDER;

                            PdfPCell textCellResponses4 = new PdfPCell();
                            Paragraph paraUserValue = new Paragraph();
                            if (ticketObj.ResponseObject[i].UST_PKID == 1)
                            {
                                paraUserValue = new Paragraph("CLIENT", fontSizeNormal);
                            }
                            else
                            {
                                paraUserValue = new Paragraph(ticketObj.ResponseObject[i].UserNames.ToString(), fontSizeNormalSmall);
                            }
                            paraUserValue.Alignment = Element.ALIGN_LEFT;
                            textCellResponses4.VerticalAlignment = Element.ALIGN_BOTTOM;
                            textCellResponses4.HorizontalAlignment = Element.ALIGN_LEFT;
                            textCellResponses4.AddElement(paraUserValue);
                            textCellResponses4.Border = Rectangle.NO_BORDER;


                            PdfPCell textCellResponses5 = new PdfPCell();
                            Paragraph paraStatusText = new Paragraph("Status:", fontSizeNormalBoldColour);
                            paraStatusText.Alignment = Element.ALIGN_RIGHT;
                            textCellResponses5.VerticalAlignment = Element.ALIGN_CENTER;
                            textCellResponses5.HorizontalAlignment = Element.ALIGN_RIGHT;
                            textCellResponses5.AddElement(paraStatusText);
                            textCellResponses5.Border = Rectangle.NO_BORDER;

                            PdfPCell textCellResponses6 = new PdfPCell();
                            Paragraph paraStatusValue = new Paragraph(ticketObj.ResponseObject[i].StatusName, fontSizeNormalSmall);
                            paraStatusValue.Alignment = Element.ALIGN_MIDDLE;
                            textCellResponses6.VerticalAlignment = Element.ALIGN_BOTTOM;
                            textCellResponses6.HorizontalAlignment = Element.ALIGN_LEFT;
                            textCellResponses6.AddElement(paraStatusValue);
                            textCellResponses6.Border = Rectangle.NO_BORDER;

                            PdfPCell textCellResponses7 = new PdfPCell();
                            Paragraph paraVisibleText = new Paragraph("Visible:", fontSizeNormalBoldColour);
                            paraVisibleText.Alignment = Element.ALIGN_RIGHT;
                            textCellResponses7.VerticalAlignment = Element.ALIGN_CENTER;
                            textCellResponses7.HorizontalAlignment = Element.ALIGN_RIGHT;
                            textCellResponses7.AddElement(paraVisibleText);
                            textCellResponses7.Border = Rectangle.NO_BORDER;

                            PdfPCell textCellResponses8 = new PdfPCell();
                            Paragraph paraVisibleValue = new Paragraph(ticketObj.ResponseObject[i].TKR_VisibleToClient.ToString(), fontSizeNormalSmall);
                            paraVisibleValue.Alignment = Element.ALIGN_LEFT;
                            textCellResponses8.VerticalAlignment = Element.ALIGN_BOTTOM;
                            textCellResponses8.HorizontalAlignment = Element.ALIGN_LEFT;
                            textCellResponses8.AddElement(paraVisibleValue);
                            textCellResponses8.Border = Rectangle.NO_BORDER;

                            tableResponsesText.AddCell(textCellResponses1);
                            tableResponsesText.AddCell(textCellResponses2);
                            tableResponsesText.AddCell(textCellResponses3);
                            tableResponsesText.AddCell(textCellResponses4);
                            tableResponsesText.AddCell(textCellResponses5);
                            tableResponsesText.AddCell(textCellResponses6);
                            tableResponsesText.AddCell(textCellResponses7);
                            tableResponsesText.AddCell(textCellResponses8);

                            float[] pWidthResponse = new float[] { 14f, 18f, 9f, 18f, 9f, 12f, 10f, 10f };
                            tableResponsesText.SetWidths(pWidthResponse);


                            #endregion

                            doc.Add(tableResponsesText);
                            doc.Add(new Paragraph(" ", fontSizeSpacer));

                            #region Response Message Values

                            PdfPTable tableResponsesValue = new PdfPTable(1);
                            tableResponsesValue.DefaultCell.Border = Rectangle.NO_BORDER;
                            tableResponsesValue.WidthPercentage = 100;
                            tableResponsesValue.HorizontalAlignment = Element.ALIGN_CENTER;

                            PdfPCell textCellResponsesValue = new PdfPCell();
                            Paragraph paraResponsesValue = new Paragraph();
                            if (ticketObj.ResponseObject[i].UST_PKID == 1)
                            {
                                if (ticketObj.ResponseObject[i].TKR_HasFile)
                                {
                                    paraResponsesValue = new Paragraph(GetAttachementString(ticketObj.ResponseObject[i].TKR_ResponseMessage, "Response has attachememt"), fontSizeNormalBoldItalic);
                                }
                                else
                                {
                                    paraResponsesValue = new Paragraph(ticketObj.ResponseObject[i].TKR_ResponseMessage, fontSizeNormalBoldItalic);
                                }
                            }
                            else
                            {
                                if (ticketObj.ResponseObject[i].TKR_HasFile)
                                {
                                    paraResponsesValue = new Paragraph(GetAttachementString(ticketObj.ResponseObject[i].TKR_ResponseMessage, "Response has attachememt"), fontSizeNormalItalic);
                                }
                                else
                                {
                                    paraResponsesValue = new Paragraph(ticketObj.ResponseObject[i].TKR_ResponseMessage, fontSizeNormalItalic);
                                }
                            }

                            paraResponsesValue.Alignment = Element.ALIGN_LEFT;
                            textCellResponsesValue.VerticalAlignment = Element.ALIGN_CENTER;
                            textCellResponsesValue.HorizontalAlignment = Element.ALIGN_LEFT;
                            textCellResponsesValue.AddElement(paraResponsesValue);
                            textCellResponsesValue.Border = Rectangle.BOX;

                            tableResponsesValue.AddCell(textCellResponsesValue);

                            float[] pWidthResponsesValue = new float[] { 100f };
                            tableResponsesValue.SetWidths(pWidthResponsesValue);

                            #endregion

                            doc.Add(tableResponsesValue);
                            doc.Add(new Paragraph("…………………………………………………………………………………………………………………………………………………", fontSizeNormal));
                        }
                    }

                    #endregion

                    doc.Add(new Paragraph(" ", fontSizeSpacer));
                    doc.Add(new Paragraph(" ", fontSizeSpacer));
                    doc.Add(new Paragraph(" ", fontSizeSpacer));

                    #region Priority Type : Time to resolve:

                    PdfPTable table5 = new PdfPTable(4);
                    table5.DefaultCell.Border = Rectangle.NO_BORDER;
                    table5.WidthPercentage = 100;
                    table5.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell texttable5Cell1 = new PdfPCell();
                    Paragraph paraProblemText = new Paragraph("Problem Type :", fontSizeNormalBoldColour);
                    paraProblemText.Alignment = Element.ALIGN_LEFT;
                    texttable5Cell1.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable5Cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable5Cell1.AddElement(paraProblemText);
                    texttable5Cell1.Border = Rectangle.NO_BORDER;

                    PdfPCell texttable5Cell2 = new PdfPCell();
                    Paragraph paraProblemValue = new Paragraph(ticketObj.ProblemTypeName, fontSizeNormal);
                    paraProblemValue.Alignment = Element.ALIGN_MIDDLE;
                    texttable5Cell2.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable5Cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable5Cell2.AddElement(paraProblemValue);
                    texttable5Cell2.Border = Rectangle.NO_BORDER;

                    PdfPCell texttable5Cell3 = new PdfPCell();
                    Paragraph paraResolveText = new Paragraph(" Time to resolve:", fontSizeNormalBoldColour);
                    paraResolveText.Alignment = Element.ALIGN_LEFT;
                    texttable5Cell3.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable5Cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable5Cell3.AddElement(paraResolveText);
                    texttable5Cell3.Border = Rectangle.NO_BORDER;

                    PdfPCell texttable5Cell4 = new PdfPCell();
                    Paragraph paraResolveValue = new Paragraph();
                    if (ticketObj.TimeToResolve != "")
                    {
                        paraResolveValue = new Paragraph(ticketObj.TimeToResolve + " " + ConfigurationManager.AppSettings["ResolveTimeNotation"], fontSizeNormal);
                    }
                    else
                    {
                        paraResolveValue = new Paragraph(ticketObj.TimeToResolve, fontSizeNormal);
                    }
                    paraResolveValue.Alignment = Element.ALIGN_LEFT;
                    texttable5Cell4.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable5Cell4.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable5Cell4.AddElement(paraResolveValue);
                    texttable5Cell4.Border = Rectangle.NO_BORDER;

                    table5.AddCell(texttable5Cell1);
                    table5.AddCell(texttable5Cell2);
                    table5.AddCell(texttable5Cell3);
                    table5.AddCell(texttable5Cell4);

                    float[] pWidthtable51 = new float[] { 15f, 35f, 15f, 35f };
                    table5.SetWidths(pWidthtable51);

                    #endregion

                    doc.Add(table5);
                    doc.Add(new Paragraph(" ", fontSizeSpacer));

                    #region Priority Date : Printed By:

                    PdfPTable table6 = new PdfPTable(4);
                    table6.DefaultCell.Border = Rectangle.NO_BORDER;
                    table6.WidthPercentage = 100;
                    table6.HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPCell texttable6Cell1 = new PdfPCell();
                    Paragraph paraPrintDateText = new Paragraph("Print Date :", fontSizeNormalBoldColour);
                    paraPrintDateText.Alignment = Element.ALIGN_LEFT;
                    texttable6Cell1.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable6Cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable6Cell1.AddElement(paraPrintDateText);
                    texttable6Cell1.Border = Rectangle.NO_BORDER;

                    PdfPCell texttable6Cell2 = new PdfPCell();
                    Paragraph paraPrintDateValue = new Paragraph(DateTime.Now.ToString("dd-MM-yyyy"), fontSizeNormal);
                    paraPrintDateValue.Alignment = Element.ALIGN_MIDDLE;
                    texttable6Cell2.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable6Cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable6Cell2.AddElement(paraPrintDateValue);
                    texttable6Cell2.Border = Rectangle.NO_BORDER;

                    PdfPCell texttable6Cell3 = new PdfPCell();
                    Paragraph paraPrintByText = new Paragraph(" Printed By:", fontSizeNormalBoldColour);
                    paraPrintByText.Alignment = Element.ALIGN_LEFT;
                    texttable6Cell3.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable6Cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable6Cell3.AddElement(paraPrintByText);
                    texttable6Cell3.Border = Rectangle.NO_BORDER;

                    PdfPCell texttable6Cell4 = new PdfPCell();
                    Paragraph paraPrintByValue = new Paragraph(usrProfObj.USR_FirstName + " " + usrProfObj.USR_LastName, fontSizeNormal);
                    paraPrintByValue.Alignment = Element.ALIGN_LEFT;
                    texttable6Cell4.VerticalAlignment = Element.ALIGN_CENTER;
                    texttable6Cell4.HorizontalAlignment = Element.ALIGN_LEFT;
                    texttable6Cell4.AddElement(paraPrintByValue);
                    texttable6Cell4.Border = Rectangle.NO_BORDER;

                    table6.AddCell(texttable6Cell1);
                    table6.AddCell(texttable6Cell2);
                    table6.AddCell(texttable6Cell3);
                    table6.AddCell(texttable6Cell4);

                    float[] pWidthtable61 = new float[] { 15f, 35f, 15f, 35f };
                    table6.SetWidths(pWidthtable61);

                    #endregion

                    doc.Add(table6);
                    doc.Add(new Paragraph(" ", fontSizeSpacer));

                    doc.Close();

                    retValue = myMemoryStream.ToArray();

                    #endregion
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    #region Insert water mark
                    Document docu = new Document();

                    PdfWriter myPDFWriter = PdfWriter.GetInstance(docu, ms);
                    PdfReader pdfReader = new PdfReader(retValue);
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, ms);

                    docu.Open();
                    docu.SetMargins(5f, 10f, 10f, 50f);

                    string waterMarkText = ConfigurationManager.AppSettings["WaterMarkText"];
                    string footerLine1 = ConfigurationManager.AppSettings["docFooterLine1"]; // "ISO 9001: 2008 Certified";
                    string footerLine2 = ConfigurationManager.AppSettings["docFooterLine2"]; //"The dtiCampus (Block F - Entfutfukweni), 77 Meintjies Street, Sunnyside, Pretoria l P O Box 429, Pretoria, 0001";
                    string footerLine3 = ConfigurationManager.AppSettings["docFooterLine3"]; //"Tel: +27 12 394 9973 | Fax: +27 12 394 1015 | Call Centre: 086 100 2472";
                    string footerLine4 = ConfigurationManager.AppSettings["docFooterLine4"]; //"Website:www.cipc.co.za";

                    iTextSharp.text.Image imgWaterMark = iTextSharp.text.Image.GetInstance(ConfigurationManager.AppSettings["WaterMarkLogoURL"]);
                    imgWaterMark.SetAbsolutePosition(100, 300);
                    float imageWaterMarkWidthF = 600f;
                    float imageWaterMarkHeightF = 400f;
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
                        pdfData.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 60);
                        //create new graphics state and assign opacity     
                        PdfGState graphicsState = new PdfGState();
                        graphicsState.FillOpacity = 0.3F;
                        //set graphics state to pdfcontentbyte     
                        pdfData.SetGState(graphicsState);
                        //set color of watermark     
                        pdfData.SetColorFill(BaseColor.GRAY);
                        //indicates start of writing of text     
                        pdfData.BeginText();
                        //show text as per position and rotation     
                        pdfData.ShowTextAligned(Element.ALIGN_CENTER, waterMarkText, pageRectangle.Width / 2, pageRectangle.Height / 2, 45);
                        //call endText to invalid font set     
                        pdfData.EndText();
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

                    //docu.Close();
                    pdfStamper.Close();
                    pdfReader.Close();

                    retValueTemp = ms.ToArray();
                    #endregion
                }

            }
            catch (Exception ex)
            {
                returnValue.errorMessage = ex.Message + " - " + ex.StackTrace;  // retValueTemp = null; 
                returnValue.noError = false;
            }

            if (retValueTemp != null)
            {
                returnValue.docObj = retValueTemp;
                returnValue.noError = true;
                returnValue.errorMessage = "";
            }
           

            return returnValue;
        }

        public string GetUserProfileString(userDisplayProfileObject p_userProfile)
        {
            string retValue = "";
            StringBuilder sb = new StringBuilder();
            if (p_userProfile != null)
            {
                p_userProfile.USR_FirstName = p_userProfile.USR_FirstName.Replace("\r\n", " ").Trim();
                p_userProfile.USR_FirstName = p_userProfile.USR_FirstName.Replace("\r", " ").Trim();
                p_userProfile.USR_FirstName = p_userProfile.USR_FirstName.Replace("\n", " ").Trim();
                sb.AppendLine(p_userProfile.USR_FirstName.Replace("\r\n", " ").Trim());

                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\r\n", " ").Trim();
                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\r", " ").Trim();
                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\n", " ").Trim();
                sb.AppendLine(p_userProfile.USR_MobileNumber.Replace("\r\n", " ").Trim());

                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\r\n", " ").Trim();
                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\r", " ").Trim();
                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\n", " ").Trim();
                sb.AppendLine(p_userProfile.USR_EmailAccount.Replace("\r\n", " ").Trim());

                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\r\n", " ").Trim();
                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\r", " ").Trim();
                p_userProfile.USR_MobileNumber = p_userProfile.USR_MobileNumber.Replace("\n", " ").Trim();
                sb.Append("Username: " + p_userProfile.USR_UserLogin.Replace("\r\n", " ").Trim());

            }
            retValue += sb.ToString();
            return retValue;
        }

        public string GetAttachementString(string p_ticketMessage, string attachmentMessage)
        {
            string retValue = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(p_ticketMessage);
            sb.Append(attachmentMessage);
            retValue += sb.ToString();
            return retValue;
        }

    }
}