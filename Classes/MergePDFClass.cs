using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections.Generic;


namespace NewBilletterie.Classes
{
    public class MergePDFClass
    {


        public string MergeDocuments(string outPutFilePath, params string[] filesPath)
        {
            string retValue = "";
            try
            {
                List<PdfReader> readerList = new List<PdfReader>();
                foreach (string filePath in filesPath)
                {
                    if (filePath != null && filePath != "")
                    {
                        PdfReader pdfReader = new PdfReader(filePath);
                        readerList.Add(pdfReader);
                    }
                }

                //Define a new output document and its size, type
                Document document = new Document(PageSize.A4, 0, 0, 0, 0);
                //Create blank output pdf file and get the stream to write on it.
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                document.Open();

                foreach (PdfReader reader in readerList)
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));
                    }
                }
                document.Close();

                retValue = outPutFilePath;
            }
            catch (Exception ex)
            {
                retValue = ex.Message;
            }
            return retValue;
        }

        public byte[] MergeDocumentsTemp(List<downloadDocumentObject> ListTicketPDFAttachments)
        {
            byte[] retValue = null;
            try
            {
                List<PdfReader> readerList = new List<PdfReader>();
                foreach (downloadDocumentObject fileArray in ListTicketPDFAttachments)
                {
                    if (fileArray != null)
                    {
                        PdfReader pdfReader = new PdfReader(fileArray.docObj);
                        readerList.Add(pdfReader);
                    }
                }

                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    //Define a new output document and its size, type
                    Document document = new Document(PageSize.A4, 0, 0, 0, 0);
                    //Create blank output pdf file and get the stream to write on it.
                    PdfWriter writer = PdfWriter.GetInstance(document, myMemoryStream);
                    document.Open();

                    foreach (PdfReader reader in readerList)
                    {
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            PdfImportedPage page = writer.GetImportedPage(reader, i);
                            document.Add(iTextSharp.text.Image.GetInstance(page));
                        }
                    }
                    document.Close();
                    retValue = myMemoryStream.ToArray();
                }
                //retValue = outPutFilePath;
            }
            catch (Exception ex)
            {
                retValue = null;    // ex.Message;
            }
            return retValue;
        }
   
    }
}