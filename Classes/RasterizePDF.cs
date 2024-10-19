using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace NewBilletterie.Classes
{
    public class RasterizePDF
    {

        public List<string> rasterizePDFFile(byte[] inputStream)
        {
            List<string> returnValue = new List<string>();

            PdfReader pdfdocument = new PdfReader(inputStream);

            for (int i = 1; i <= pdfdocument.NumberOfPages; i++)
            {
                returnValue = GetPdfLinks(inputStream, i);
                if (returnValue != null)
                    break;
            }

            return returnValue;
        }


        private static List<string> GetPdfLinks(byte[] inputStream, int page)
        {
            PdfReader R = new PdfReader(inputStream);

            //Get the current page
            PdfDictionary PageDictionary = R.GetPageN(page);

            //Get all of the annotations for the current page
            PdfArray Annots = PageDictionary.GetAsArray(PdfName.ANNOTS);

            //Make sure we have something
            if ((Annots == null) || (Annots.Length == 0))
                return null;

            List<string> Ret = new List<string>();

            //Loop through each annotation
            foreach (PdfObject A in Annots.ArrayList)
            {
                //Convert the itext-specific object as a generic PDF object
                PdfDictionary AnnotationDictionary = (PdfDictionary)PdfReader.GetPdfObject(A);

                //Make sure this annotation has a link
                if (!AnnotationDictionary.Get(PdfName.SUBTYPE).Equals(PdfName.LINK))
                    continue;

                //Make sure this annotation has an ACTION
                if (AnnotationDictionary.Get(PdfName.A) == null)
                    continue;

                // Unable to cast object of type 'iTextSharp.text.pdf.PRIndirectReference' to type 'iTextSharp.text.pdf.PdfDictionary'.
                try
                {
                    PRIndirectReference AnnotationActionA = (PRIndirectReference)AnnotationDictionary.Get(PdfName.A);

                    //Test if it is a URI action (There are tons of other types of actions, some of which might mimic URI, such as JavaScript, but those need to be handled seperately)
                    if (AnnotationActionA.Reader.JavaScript != "")
                    {
                        Ret.Add(AnnotationActionA.Reader.JavaScript);
                    }

                }
                catch (Exception) { }



                ////Get the ACTION for the current annotation
                //try
                //{
                //    PdfObject AnnotationActionB = (PdfObject)AnnotationDictionary.Get(PdfName.A);

                //    //Test if it is a URI action (There are tons of other types of actions, some of which might mimic URI, such as JavaScript, but those need to be handled seperately)
                //    if (AnnotationActionB.Reader.JavaScript != "")
                //    {
                //        Ret.Add(AnnotationActionB.Reader.JavaScript);
                //    }
                //}
                //catch (Exception) { }


                PdfDictionary AnnotationAction = new PdfDictionary();
                try
                {
                    AnnotationAction = (PdfDictionary)AnnotationDictionary.Get(PdfName.A);

                    //Test if it is a URI action (There are tons of other types of actions, some of which might mimic URI, such as JavaScript, but those need to be handled seperately)
                    if (AnnotationAction.Get(PdfName.S).Equals(PdfName.URI))
                    {
                        PdfString Destination = AnnotationAction.GetAsString(PdfName.URI);
                        if (Destination != null)
                            Ret.Add(Destination.ToString());
                    }
                }
                catch (Exception)
                {

                }
               

            }


            //PdfStamper stamper = new PdfStamper(R, inputStream);
            for (int i = 0; i <= R.XrefSize; i++)
            {
                PdfDictionary pd = R.GetPdfObject(i) as PdfDictionary;
                if (pd != null)
                {
                    PdfObject poAA = pd.Get(PdfName.AA); //Gets automatic execution objects
                    if (poAA != null)
                        Ret.Add(poAA.ToString());

                    PdfObject poJS = pd.Get(PdfName.JS); // Gets javascript objects
                    if (poJS != null)
                        Ret.Add(poJS.ToString());

                    PdfObject poJavaScript = pd.Get(PdfName.JAVASCRIPT); // Gets other javascript objects
                    if (poJavaScript != null)
                        Ret.Add(poJavaScript.ToString());
                }
            }
            //stamper.Close();
            //R.Close();

            return Ret;

        }


    }
}