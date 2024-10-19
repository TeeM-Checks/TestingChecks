using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using NewBilletterie.BilletterieAPIWS;
using System.Text.RegularExpressions;


namespace NewBilletterie.Classes
{
    public class Common
    {

        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

        //public string GetMD5Hash(string input)
        //{
        //    System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //    byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
        //    bs = x.ComputeHash(bs);
        //    System.Text.StringBuilder s = new System.Text.StringBuilder();
        //    foreach (byte b in bs)
        //    {
        //        s.Append(b.ToString("x2").ToLower());
        //    }
        //    string password = s.ToString();
        //    return password;
        //}

        //static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments.
        //    if (plainText == null || plainText.Length <= 0)
        //        throw new ArgumentNullException("plainText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("IV");
        //    byte[] encrypted;

        //    // Create an AesCryptoServiceProvider object
        //    // with the specified key and IV.
        //    using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        //    {
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create an encryptor to perform the stream transform.
        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for encryption.
        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    //Write all data to the stream.
        //                    swEncrypt.Write(plainText);
        //                }
        //            }
        //            encrypted = msEncrypt.ToArray();
        //        }
        //    }

        //    // Return the encrypted bytes from the memory stream.
        //    return encrypted;
        //}

        //static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments.
        //    if (cipherText == null || cipherText.Length <= 0)
        //        throw new ArgumentNullException("cipherText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("IV");

        //    // Declare the string used to hold
        //    // the decrypted text.
        //    string plaintext = null;

        //    // Create an AesCryptoServiceProvider object
        //    // with the specified key and IV.
        //    using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        //    {
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create a decryptor to perform the stream transform.
        //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        //        // Create the streams used for decryption.
        //        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                {

        //                    // Read the decrypted bytes from the decrypting stream
        //                    // and place them in a string.
        //                    plaintext = srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    return plaintext;
        //}

        public string GetMD5Hash(string input)
        {
            string returnValue = "";
            string ciKey = bilAPIWS.GetCurrentCipherKey(ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            returnValue = GetHashedKeyString(ciKey, input);
            return returnValue;
        }

        private static string GetHashedKeyString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        private static string GetPlainKeyString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public string GetPlainMD5Hash(string hashedText)
        {
            string returnValue = "";
            string ciKey = bilAPIWS.GetCurrentCipherKey(ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            returnValue = GetPlainKeyString(ciKey, hashedText);
            return returnValue;
        }

        public BilletterieAPIWS.userProfileObject GetOfficerProfileObject(string username, string password)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieOfficerObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = bilAPIWS.GetBilletterieOfficerObject(username, password);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFC_PKID"].ToString());
                        returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                        returnValue.OFL_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFL_PKID"].ToString());
                        returnValue.ODP_PKID = Int32.Parse(ds.Tables[0].Rows[0]["ODP_PKID"].ToString());
                        returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["OFC_UserLogin"].ToString();
                        returnValue.USR_PassKey = ds.Tables[0].Rows[0]["OFC_PassKey"].ToString();
                        returnValue.USR_HashedPassword = System.Text.Encoding.UTF8.GetBytes(ds.Tables[0].Rows[0]["USR_HashedPassword"].ToString());
                        returnValue.USR_FirstName = ds.Tables[0].Rows[0]["OFC_FirstName"].ToString();
                        returnValue.USR_LastName = ds.Tables[0].Rows[0]["OFC_Surname"].ToString();
                        returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["OFC_MobileNumber"].ToString();
                        returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["OFC_EmailAccount"].ToString();
                        returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["OFC_DateCreated"].ToString();
                        returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["OFC_ActivationDate"].ToString();
                        returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                        returnValue.OFC_PaginateResults = bool.Parse(ds.Tables[0].Rows[0]["OFC_PaginateResults"].ToString());
                        returnValue.OFC_CanEdit = bool.Parse(ds.Tables[0].Rows[0]["OFC_CanEdit"].ToString());
                        returnValue.OFC_HideAssigned = bool.Parse(ds.Tables[0].Rows[0]["OFC_HideAssigned"].ToString());
                        returnValue.OFC_IsApprover = Int32.Parse(ds.Tables[0].Rows[0]["OFC_IsApprover"].ToString());
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
                    }
                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = "Database connection error. Please contact system administrator on " + ConfigurationManager.AppSettings["AdminContact"];
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        //public BilletterieAPIWS.userProfileObject GetOfficerProfileObject(string username, string password)
        //{
        //    BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
        //    try
        //    {
        //        DataAccess da = new DataAccess();
        //        DataSet ds = new DataSet();
        //        ds = bilAPIWS.GetBilletterieOfficerObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //        //ds = bilAPIWS.GetBilletterieOfficerObject(username, password);
        //        if (ds != null)
        //        {
        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFC_PKID"].ToString());
        //                returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
        //                returnValue.OFL_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFL_PKID"].ToString());
        //                returnValue.ODP_PKID = Int32.Parse(ds.Tables[0].Rows[0]["ODP_PKID"].ToString());
        //                returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["OFC_UserLogin"].ToString();
        //                returnValue.USR_PassKey = ds.Tables[0].Rows[0]["OFC_PassKey"].ToString();
        //                returnValue.USR_FirstName = ds.Tables[0].Rows[0]["OFC_FirstName"].ToString();
        //                returnValue.USR_LastName = ds.Tables[0].Rows[0]["OFC_Surname"].ToString();
        //                returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["OFC_MobileNumber"].ToString();
        //                returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["OFC_EmailAccount"].ToString();
        //                returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["OFC_DateCreated"].ToString();
        //                returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["OFC_ActivationDate"].ToString();
        //                returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
        //                returnValue.OFC_PaginateResults = bool.Parse(ds.Tables[0].Rows[0]["OFC_PaginateResults"].ToString());
        //                returnValue.OFC_CanEdit = bool.Parse(ds.Tables[0].Rows[0]["OFC_CanEdit"].ToString());
        //                returnValue.OFC_HideAssigned = bool.Parse(ds.Tables[0].Rows[0]["OFC_HideAssigned"].ToString());
        //                returnValue.OFC_IsApprover = Int32.Parse(ds.Tables[0].Rows[0]["OFC_IsApprover"].ToString());
        //                returnValue.noError = true;
        //            }
        //            else
        //            {
        //                returnValue.noError = false;
        //                returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
        //            }
        //        }
        //        else
        //        {
        //            returnValue.noError = false;
        //            returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
        //        }
        //    }
        //    catch (System.Data.SqlClient.SqlException ex)
        //    {
        //        returnValue.noError = false;
        //        returnValue.errorMessage = "Database connection error. Please contact system administrator on " + ConfigurationManager.AppSettings["AdminContact"];
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue.noError = false;
        //        returnValue.errorMessage = ex.Message;
        //    }
        //    return returnValue;
        //}

        public BilletterieAPIWS.userProfileObject GetOfficerProfileObject(string userUniqueID)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieOfficerObjectID(userUniqueID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetBilletterieOfficerObject(userUniqueID);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFC_PKID"].ToString());
                        returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                        returnValue.OFL_PKID = Int32.Parse(ds.Tables[0].Rows[0]["OFL_PKID"].ToString());
                        returnValue.ODP_PKID = Int32.Parse(ds.Tables[0].Rows[0]["ODP_PKID"].ToString());
                        returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["OFC_UserLogin"].ToString();
                        returnValue.USR_PassKey = ds.Tables[0].Rows[0]["OFC_PassKey"].ToString();
                        returnValue.USR_FirstName = ds.Tables[0].Rows[0]["OFC_FirstName"].ToString();
                        returnValue.USR_LastName = ds.Tables[0].Rows[0]["OFC_Surname"].ToString();
                        returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["OFC_MobileNumber"].ToString();
                        returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["OFC_EmailAccount"].ToString();
                        returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["OFC_DateCreated"].ToString();
                        returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["OFC_ActivationDate"].ToString();
                        returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                        returnValue.OFC_PaginateResults = bool.Parse(ds.Tables[0].Rows[0]["OFC_PaginateResults"].ToString());
                        returnValue.OFC_CanEdit = bool.Parse(ds.Tables[0].Rows[0]["OFC_CanEdit"].ToString());
                        returnValue.OFC_HideAssigned = bool.Parse(ds.Tables[0].Rows[0]["OFC_HideAssigned"].ToString());
                        returnValue.OFC_IsApprover = Int32.Parse(ds.Tables[0].Rows[0]["OFC_IsApprover"].ToString());
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
                    }
                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = "Database connection error. Please contact system administrator on " + ConfigurationManager.AppSettings["AdminContact"];
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        //public BilletterieAPIWS.userProfileObject GetUserProfileObject(string username, string password)
        //{
        //    BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
        //    try
        //    {
        //        DataAccess da = new DataAccess();
        //        DataSet ds = new DataSet();
        //        ds = bilAPIWS.GetBilletterieUserObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //        //ds = da.GetBilletterieUserObject(username, password);
        //        if (ds != null)
        //        {
        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
        //                returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
        //                returnValue.USG_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USG_PKID"].ToString());
        //                returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
        //                returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
        //                returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
        //                returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
        //                returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
        //                returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
        //                returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
        //                returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
        //                returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
        //                returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
        //                returnValue.noError = true;
        //            }
        //            else
        //            {
        //                returnValue.noError = false;
        //                returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
        //            }
        //        }
        //        else
        //        {
        //            returnValue.noError = false;
        //            returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
        //        }
        //    }
        //    catch (System.Data.SqlClient.SqlException)
        //    {
        //        returnValue.noError = false;
        //        returnValue.errorMessage = "Database connection error. Please contact system administrator on " + ConfigurationManager.AppSettings["AdminContact"];
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue.noError = false;
        //        returnValue.errorMessage = ex.Message;
        //    }
        //    return returnValue;
        //}

        public BilletterieAPIWS.userProfileObject GetUserProfileObject(string username, string password)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieUserObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetBilletterieUserObject(username, password);
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                        returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                        returnValue.USG_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USG_PKID"].ToString());
                        returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
                        returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
                        returnValue.USR_HashedPassword = System.Text.Encoding.UTF8.GetBytes(ds.Tables[0].Rows[0]["USR_HashedPassword"].ToString());
                        returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
                        returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
                        returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
                        returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
                        returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
                        returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
                        returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
                        returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
                    }
                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "Incorrect Username/Password. Please try again.";
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                returnValue.noError = false;
                returnValue.errorMessage = "Database connection error. Please contact system administrator on " + ConfigurationManager.AppSettings["AdminContact"];
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        public BilletterieAPIWS.userProfileObject GetERMSUserProfileObjectNOPWD(string username, string idNumber)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                //IPCUBAERMSBillingWS ifx = new IPCUBAERMSBillingWS();
                //ifx.Url = ConfigurationManager.AppSettings["IFX_WSURL"];

                DataAccess da = new DataAccess();
                InformixDataSetResponse dsResp = new InformixDataSetResponse();
                DataSet ds = new DataSet();

                dsResp = bilAPIWS.GetInformixDataSet("select agent_code, password, agent_name, tel_code, tel_no, email, cell_no, agent_id_no from agents where agent_code = '" + username.ToUpper() + "' and agent_id_no = '" + idNumber.ToUpper() + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                if (dsResp.noError)
                {
                    ds = dsResp.ermsDataSetObject;
                    BilletterieAPIWS.userProfileObject ermsUserObj = new BilletterieAPIWS.userProfileObject();
                    ermsUserObj = PopulateUserObjectFromERMS(ds);
                    ermsUserObj.OFL_PKID = 3;

                    if (ermsUserObj.noError)
                    {

                        returnValue = ermsUserObj;
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "User not found";
                    }

                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "User not found";
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        public BilletterieAPIWS.userProfileObject GetERMSUserProfileObjectNOPWD(string username, string idNumber, ticketInformationObject ticketInfoObj)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                //IPCUBAERMSBillingWS ifx = new IPCUBAERMSBillingWS();
                //ifx.Url = ConfigurationManager.AppSettings["IFX_WSURL"];

                DataAccess da = new DataAccess();
                InformixDataSetResponse dsResp = new InformixDataSetResponse();
                DataSet ds = new DataSet();

                dsResp = bilAPIWS.GetInformixDataSet("select agent_code, password, agent_name, tel_code, tel_no, email, cell_no, agent_id_no from agents where agent_code = '" + username.ToUpper() + "' and agent_id_no = '" + idNumber.ToUpper() + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                if (dsResp.noError)
                {
                    ds = dsResp.ermsDataSetObject;
                    BilletterieAPIWS.userProfileObject ermsUserObj = new BilletterieAPIWS.userProfileObject();
                    ermsUserObj = PopulateUserObjectFromERMS(ds);
                    ermsUserObj.OFL_PKID = 3;
                    ermsUserObj.USR_MobileNumber = ticketInfoObj.PhoneNumber;   // ifxDS.Tables[0].Rows[0]["cell_no"].ToString();
                    ermsUserObj.USR_EmailAccount = ticketInfoObj.EmailAccount;  // ifxDS.Tables[0].Rows[0]["email"].ToString();

                    if (ermsUserObj.noError)
                    {

                        returnValue = ermsUserObj;
                        returnValue.noError = true;

                        #region ERMS User processing

                        BilletterieAPIWS.SelectStringResponseObject sclResp = new BilletterieAPIWS.SelectStringResponseObject();
                        sclResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        if (sclResp.noError)
                        {
                            if (Int32.Parse(sclResp.selectedPKID) > 0)
                            {
                                DataSet usDS = new DataSet();
                                usDS = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                if (usDS != null)
                                {
                                    if (usDS.Tables[0].Rows.Count > 0)
                                    {
                                        //Upadte Billetterie database
                                        bilAPIWS.UpdateBilletterieUserRecord(ermsUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                    }
                                }

                            }
                            else
                            {
                                //Write to Billetterie database
                                bilAPIWS.InsertBilletterieUserRecord(ermsUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            }
                        }

                        //Re-read Billetterie database.
                        ds = bilAPIWS.GetBilletterieNoPasswordUserObject(username, ticketInfoObj.EmailAccount, ticketInfoObj.PhoneNumber, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //ds = da.GetBilletterieNoPasswordUserObject(username, ticketInfoObj.EmailAccount, ticketInfoObj.PhoneNumber);

                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                                returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                                returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
                                returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
                                returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
                                returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
                                returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
                                returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
                                returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
                                returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
                                returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
                                returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                                returnValue.OFL_PKID = 3;
                                returnValue.noError = true;
                            }
                        }
                        else
                        {
                            returnValue.noError = false;
                            returnValue.errorMessage = "User not found";
                        }
                        #endregion

                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "User not found";
                    }

                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "User not found";
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        public BilletterieAPIWS.userProfileObject GetERMSUserProfileObject(string username, string emailAccount, string mobilePhoneNumber, string idNumber)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                //IPCUBAERMSBillingWS ifx = new IPCUBAERMSBillingWS();
                //ifx.Url = ConfigurationManager.AppSettings["IFX_WSURL"];

                DataAccess da = new DataAccess();
                InformixDataSetResponse dsResp = new InformixDataSetResponse();
                DataSet ds = new DataSet();
                if (emailAccount != "" && mobilePhoneNumber == "")
                {
                    dsResp = bilAPIWS.GetInformixDataSet("select agent_code, password, agent_name, tel_code, tel_no, email, cell_no, agent_id_no from agents where agent_code = '" + username.ToUpper() + "' and email = '" + emailAccount.ToUpper() + "' and agent_id_no = '" + idNumber.ToUpper() + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                }
                else if (emailAccount == "" && mobilePhoneNumber != "")
                {
                    dsResp = bilAPIWS.GetInformixDataSet("select agent_code, password, agent_name, tel_code, tel_no, email, cell_no, agent_id_no from agents where agent_code = '" + username.ToUpper() + "' and cell_no = '" + mobilePhoneNumber.ToUpper() + "' and agent_id_no = '" + idNumber.ToUpper() + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                }
                else if (emailAccount != "" && mobilePhoneNumber != "")
                {
                    dsResp = bilAPIWS.GetInformixDataSet("select agent_code, password, agent_name, tel_code, tel_no, email, cell_no, agent_id_no from agents where agent_code = '" + username.ToUpper() + "' and email = '" + emailAccount.ToUpper() + "' and cell_no = '" + mobilePhoneNumber.ToUpper() + "' and agent_id_no = '" + idNumber.ToUpper() + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                }

                if (dsResp.noError)
                {
                    ds = dsResp.ermsDataSetObject;
                    BilletterieAPIWS.userProfileObject ermsUserObj = new BilletterieAPIWS.userProfileObject();
                    ermsUserObj = PopulateUserObjectFromERMS(ds);

                    if (ermsUserObj.noError)
                    {
                        #region ERMS User processing

                        BilletterieAPIWS.SelectStringResponseObject sclResp = new BilletterieAPIWS.SelectStringResponseObject();
                        sclResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //sclResp = da.GetBilletterieGenericScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'");
                        if (sclResp.noError)
                        {
                            if (Int32.Parse(sclResp.selectedPKID) > 0)
                            {
                                DataSet usDS = new DataSet();
                                usDS = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //usDS = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'");
                                if (usDS != null)
                                {
                                    if (usDS.Tables[0].Rows.Count > 0)
                                    {
                                        //Upadte Billetterie database
                                        bilAPIWS.UpdateBilletterieUserRecord(ermsUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //da.UpdateBilletterieUserRecord(ermsUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString());
                                    }
                                }

                            }
                            else
                            {
                                //Write to Billetterie database
                                bilAPIWS.InsertBilletterieUserRecord(ermsUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //da.InsertBilletterieUserRecord(ermsUserObj);
                            }
                        }

                        //Re-read Billetterie database.
                        ds = bilAPIWS.GetBilletterieNoPasswordUserObject(username, emailAccount, mobilePhoneNumber, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //ds = da.GetBilletterieNoPasswordUserObject(username, emailAccount, mobilePhoneNumber);

                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                                returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                                returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
                                returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();

                                returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
                                returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
                                returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
                                returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
                                returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
                                returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
                                returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
                                returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                                returnValue.OFL_PKID = 3;
                                returnValue.noError = true;
                            }
                        }
                        else
                        {
                            returnValue.noError = false;
                            returnValue.errorMessage = "User not found";
                        }
                        #endregion
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "User not found";
                    }

                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "User not found";
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        public BilletterieAPIWS.userProfileObject GetERMSUserProfileObject(string username, string password)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                DataAccess da = new DataAccess();
                InformixDataSetResponse dsResp = new InformixDataSetResponse();
                DataSet ds = new DataSet();
                dsResp = bilAPIWS.GetInformixDataSet("select agent_code, password, agent_name, tel_code, tel_no, email, cell_no, agent_id_no from agents where agent_code = '" + username + "' and password = '" + password + "' and status = 'A'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                if (dsResp.noError)
                {
                    ds = dsResp.ermsDataSetObject;
                    BilletterieAPIWS.userProfileObject ermsUserObj = new BilletterieAPIWS.userProfileObject();
                    ermsUserObj = PopulateUserObjectFromERMS(ds);

                    if (ermsUserObj.noError)
                    {
                        #region ERMS User processing

                        BilletterieAPIWS.SelectStringResponseObject sclResp = new BilletterieAPIWS.SelectStringResponseObject();
                        sclResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        if (sclResp.noError)
                        {
                            if (Int32.Parse(sclResp.selectedPKID) > 0)
                            {
                                DataSet usDS = new DataSet();
                                usDS = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //usDS = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'");
                                if (usDS != null)
                                {
                                    if (usDS.Tables[0].Rows.Count > 0)
                                    {
                                        //Upadte Billetterie database
                                        bilAPIWS.UpdateBilletterieUserRecord(ermsUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //da.UpdateBilletterieUserRecord(ermsUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString());
                                    }
                                }

                            }
                            else
                            {
                                //Write to Billetterie database
                                bilAPIWS.InsertBilletterieUserRecord(ermsUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //da.InsertBilletterieUserRecord(ermsUserObj);
                            }
                        }

                        //Re-read Billetterie database.
                        ds = bilAPIWS.GetBilletterieUserObject(username, GetMD5Hash(password), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //ds = da.GetBilletterieUserObject(username, GetMD5Hash(password));
                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                                returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                                returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
                                returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
                                returnValue.USR_HashedPassword = System.Text.Encoding.UTF8.GetBytes(ds.Tables[0].Rows[0]["USR_HashedPassword"].ToString());// ds.Tables[0].Rows[0]["USR_HashedPassword"].ToString();
                                returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
                                returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
                                returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
                                returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
                                returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
                                returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
                                returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
                                returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                                returnValue.noError = true;
                            }
                        }
                        else
                        {
                            returnValue.noError = false;
                            returnValue.errorMessage = "User not found";
                        }
                        #endregion
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "User not found";
                    }

                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "User not found";
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        //public BilletterieAPIWS.userProfileObject GetCUBAUserProfileObject(string username, string password)
        //{
        //    BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
        //    try
        //    {
        //        //ServerServices servWS = new ServerServices();
        //        //servWS.Url = ConfigurationManager.AppSettings["CUBAServerServiceURL"];
                
        //        DataAccess da = new DataAccess();
        //        DataSet dsResp = new DataSet();
        //        DataSet ds = new DataSet();
        //        //dsResp = servWS.GetGenericDataSet("select USR_Username, USR_Password, USR_Email, USR_AddForServiceName, USR_Phone from TB_USR_Users where USR_Username = '" + username + "' and USR_Password = '" + password + "' and USR_Activated = '1'", ConfigurationManager.AppSettings["serviceKey"]);
        //        dsResp = bilAPIWS.GetCUBADataSet("select USR_Username, USR_UserPassword USR_Password, USR_EmailAccount USR_Email, USR_Names USR_AddForServiceName, A.ADS_AddressMobileNumber USR_Phone from [USERMANAGEMENT].dbo.TB_USR_User U inner join [USERMANAGEMENT].dbo.TB_ADS_Address A on U.ADS_PKID = A.ADS_PKID where USR_Username = '" + username + "' and USR_UserPassword = '" + password + "' and USR_Status = '1'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

        //        if (dsResp != null)
        //        {
        //            BilletterieAPIWS.userProfileObject cubaUserObj = new BilletterieAPIWS.userProfileObject();
        //            cubaUserObj = PopulateUserObjectFromCUBA(dsResp);

        //            if (cubaUserObj.noError)
        //            {
        //                #region CUBA User processing

        //                BilletterieAPIWS.SelectStringResponseObject sclResp = new BilletterieAPIWS.SelectStringResponseObject();
        //                sclResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                //sclResp = da.GetBilletterieGenericScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'");
        //                if (sclResp.noError)
        //                {
        //                    if (Int32.Parse(sclResp.selectedPKID) > 0)
        //                    {
        //                        DataSet usDS = new DataSet();
        //                        usDS = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                        //usDS = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'");
        //                        if (usDS != null)
        //                        {
        //                            if (usDS.Tables[0].Rows.Count > 0)
        //                            {
        //                                //Upadte Billetterie database
        //                                bilAPIWS.UpdateBilletterieUserRecord(cubaUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                                //da.UpdateBilletterieUserRecord(cubaUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString());
        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        //Write to Billetterie database
        //                        bilAPIWS.InsertBilletterieUserRecord(cubaUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                        //da.InsertBilletterieUserRecord(cubaUserObj);
        //                    }
        //                }

        //                //Re-read Billetterie database.
        //                ds = bilAPIWS.GetBilletterieOfficerObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                //ds = da.GetBilletterieUserObject(username, password);
        //                if (ds != null)
        //                {
        //                    if (ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
        //                        returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
        //                        returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
        //                        returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
        //                        returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
        //                        returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
        //                        returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
        //                        returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
        //                        returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
        //                        returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
        //                        returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
        //                        returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
        //                        returnValue.noError = true;
        //                    }
        //                }
        //                else
        //                {
        //                    //returnValue = null;
        //                    returnValue.noError = false;
        //                    returnValue.errorMessage = "User not found";
        //                }
        //                #endregion
        //            }
        //            else
        //            {
        //                returnValue.noError = false;
        //                returnValue.errorMessage = "User not found";
        //            }

        //        }
        //        else
        //        {
        //            returnValue.noError = false;
        //            returnValue.errorMessage = "User not found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue.noError = false;
        //        returnValue.errorMessage = ex.Message;
        //    }
        //    return returnValue;
        //}

        public BilletterieAPIWS.userProfileObject GetCUBAUserProfileObject(string username, string password)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                DataAccess da = new DataAccess();
                DataSet dsResp = new DataSet();
                DataSet ds = new DataSet();
                //dsResp = servWS.GetGenericDataSet("select USR_Username, USR_Password, USR_Email, USR_AddForServiceName, USR_Phone from TB_USR_Users where USR_Username = '" + username + "' and USR_Password = '" + password + "' and USR_Activated = '1'", ConfigurationManager.AppSettings["serviceKey"]);
                dsResp = bilAPIWS.GetCUBADataSet("select USR_Username, USR_UserPassword USR_Password, USR_EmailAccount USR_Email, USR_Names USR_AddForServiceName, A.ADS_AddressMobileNumber USR_Phone from [USERMANAGEMENT].dbo.TB_USR_User U inner join [USERMANAGEMENT].dbo.TB_ADS_Address A on U.ADS_PKID = A.ADS_PKID where USR_Username = '" + username + "' and USR_UserPassword = '" + password + "' and USR_Status = '1'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                if (dsResp != null)
                {
                    BilletterieAPIWS.userProfileObject cubaUserObj = new BilletterieAPIWS.userProfileObject();
                    cubaUserObj = PopulateUserObjectFromCUBA(dsResp);

                    if (cubaUserObj.noError)
                    {
                        #region CUBA User processing

                        BilletterieAPIWS.SelectStringResponseObject sclResp = new BilletterieAPIWS.SelectStringResponseObject();
                        sclResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //sclResp = da.GetBilletterieGenericScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'");
                        if (sclResp.noError)
                        {
                            if (Int32.Parse(sclResp.selectedPKID) > 0)
                            {
                                DataSet usDS = new DataSet();
                                usDS = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //usDS = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'");
                                if (usDS != null)
                                {
                                    if (usDS.Tables[0].Rows.Count > 0)
                                    {
                                        //Upadte Billetterie database
                                        bilAPIWS.UpdateBilletterieUserRecord(cubaUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //da.UpdateBilletterieUserRecord(cubaUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString());
                                    }
                                }

                            }
                            else
                            {
                                //Write to Billetterie database
                                bilAPIWS.InsertBilletterieUserRecord(cubaUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //da.InsertBilletterieUserRecord(cubaUserObj);
                            }
                        }

                        //Re-read Billetterie database.
                        ds = bilAPIWS.GetBilletterieOfficerObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //ds = da.GetBilletterieUserObject(username, password);
                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                                returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                                returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
                                returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
                                returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
                                returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
                                returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
                                returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
                                returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
                                returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
                                returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
                                returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                                returnValue.noError = true;
                            }
                        }
                        else
                        {
                            //returnValue = null;
                            returnValue.noError = false;
                            returnValue.errorMessage = "User not found";
                        }
                        #endregion
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "User not found";
                    }

                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "User not found";
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        //public BilletterieAPIWS.userProfileObject GetPtolemyUserProfileObject(string username, string password)
        //{
        //    BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
        //    try
        //    {
        //        DataAccess da = new DataAccess();
        //        DataSet dsResp = new DataSet();
        //        DataSet ds = new DataSet();

        //        //OraDataAccessWS oraWS = new OraDataAccessWS();
        //        //oraWS.Url = ConfigurationManager.AppSettings["GetPTOUserURL"];

        //        dsResp = bilAPIWS.GetOracleDataSet("select name, login, phone, email, pwd, iduser from ptouser where LOWER(login) = '" + username + "' and pwd = '" + password + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

        //        if (dsResp != null)
        //        {
        //            BilletterieAPIWS.userProfileObject ptolemyUserObj = new BilletterieAPIWS.userProfileObject();
        //            ptolemyUserObj = PopulateUserObjectFromPtolemy(dsResp);

        //            if (ptolemyUserObj.noError)
        //            {
        //                #region Ptolemy User processing

        //                BilletterieAPIWS.SelectStringResponseObject sclResp = new BilletterieAPIWS.SelectStringResponseObject();
        //                sclResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                //sclResp = da.GetBilletterieGenericScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'");
        //                if (sclResp.noError)
        //                {
        //                    if (Int32.Parse(sclResp.selectedPKID) > 0)
        //                    {
        //                        DataSet usDS = new DataSet();
        //                        usDS = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                        //usDS = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'");
        //                        if (usDS != null)
        //                        {
        //                            if (usDS.Tables[0].Rows.Count > 0)
        //                            {
        //                                //Upadte Billetterie database
        //                                bilAPIWS.UpdateBilletterieUserRecord(ptolemyUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                                //da.UpdateBilletterieUserRecord(ptolemyUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString());
        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        //Write to Billetterie database
        //                        bilAPIWS.InsertBilletterieUserRecord(ptolemyUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                        //da.InsertBilletterieUserRecord(ptolemyUserObj);
        //                    }
        //                }

        //                //Re-read Billetterie database.
        //                ds = bilAPIWS.GetBilletterieOfficerObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        //                //ds = da.GetBilletterieUserObject(username, password);
        //                if (ds != null)
        //                {
        //                    if (ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
        //                        returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
        //                        returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
        //                        returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
        //                        returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
        //                        returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
        //                        returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
        //                        returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
        //                        returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
        //                        returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
        //                        returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
        //                        returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
        //                        returnValue.noError = true;
        //                    }
        //                }
        //                else
        //                {
        //                    //returnValue = null;
        //                    returnValue.noError = false;
        //                    returnValue.errorMessage = "User not found";
        //                }
        //                #endregion
        //            }
        //            else
        //            {
        //                returnValue.noError = false;
        //                returnValue.errorMessage = "User not found";
        //            }

        //        }
        //        else
        //        {
        //            returnValue.noError = false;
        //            returnValue.errorMessage = "User not found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue.noError = false;
        //        returnValue.errorMessage = ex.Message;
        //    }
        //    return returnValue;
        //}

        public BilletterieAPIWS.userProfileObject GetPtolemyUserProfileObject(string username, string password)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                DataAccess da = new DataAccess();
                DataSet dsResp = new DataSet();
                DataSet ds = new DataSet();

                //OraDataAccessWS oraWS = new OraDataAccessWS();
                //oraWS.Url = ConfigurationManager.AppSettings["GetPTOUserURL"];

                dsResp = bilAPIWS.GetOracleDataSet("select name, login, phone, email, pwd, iduser from ptouser where LOWER(login) = '" + username + "' and pwd = '" + password + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                if (dsResp != null)
                {
                    BilletterieAPIWS.userProfileObject ptolemyUserObj = new BilletterieAPIWS.userProfileObject();
                    ptolemyUserObj = PopulateUserObjectFromPtolemy(dsResp);

                    if (ptolemyUserObj.noError)
                    {
                        #region Ptolemy User processing

                        BilletterieAPIWS.SelectStringResponseObject sclResp = new BilletterieAPIWS.SelectStringResponseObject();
                        sclResp = bilAPIWS.GetBilletterieScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //sclResp = da.GetBilletterieGenericScalar("select count(*) from TB_USR_User where USR_UserLogin = '" + username + "'");
                        if (sclResp.noError)
                        {
                            if (Int32.Parse(sclResp.selectedPKID) > 0)
                            {
                                DataSet usDS = new DataSet();
                                usDS = bilAPIWS.GetBilletterieDataSet("select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //usDS = da.GetGenericBilletterieDataSet("TB_USR_User", "TB_USR_UserDS", "select USR_PKID, USC_PKID, USR_UserLogin from TB_USR_User where USR_UserLogin = '" + username + "'");
                                if (usDS != null)
                                {
                                    if (usDS.Tables[0].Rows.Count > 0)
                                    {
                                        //Upadte Billetterie database
                                        bilAPIWS.UpdateBilletterieUserRecord(ptolemyUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //da.UpdateBilletterieUserRecord(ptolemyUserObj, usDS.Tables[0].Rows[0]["USR_PKID"].ToString());
                                    }
                                }

                            }
                            else
                            {
                                //Write to Billetterie database
                                bilAPIWS.InsertBilletterieUserRecord(ptolemyUserObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //da.InsertBilletterieUserRecord(ptolemyUserObj);
                            }
                        }

                        //Re-read Billetterie database.
                        ds = bilAPIWS.GetBilletterieOfficerObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                        //ds = da.GetBilletterieUserObject(username, password);
                        if (ds != null)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                returnValue.USR_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USR_PKID"].ToString());
                                returnValue.USC_PKID = Int32.Parse(ds.Tables[0].Rows[0]["USC_PKID"].ToString());
                                returnValue.USR_UserLogin = ds.Tables[0].Rows[0]["USR_UserLogin"].ToString();
                                returnValue.USR_PassKey = ds.Tables[0].Rows[0]["USR_PassKey"].ToString();
                                returnValue.USR_FirstName = ds.Tables[0].Rows[0]["USR_FirstName"].ToString();
                                returnValue.USR_LastName = ds.Tables[0].Rows[0]["USR_LastName"].ToString();
                                returnValue.USR_MobileNumber = ds.Tables[0].Rows[0]["USR_MobileNumber"].ToString();
                                returnValue.USR_EmailAccount = ds.Tables[0].Rows[0]["USR_EmailAccount"].ToString();
                                returnValue.USR_DateCreated = ds.Tables[0].Rows[0]["USR_DateCreated"].ToString();
                                returnValue.USR_ActivationDate = ds.Tables[0].Rows[0]["USR_ActivationDate"].ToString();
                                returnValue.USR_FaceImage = ds.Tables[0].Rows[0]["USR_FaceImage"].ToString();
                                returnValue.STS_PKID = Int32.Parse(ds.Tables[0].Rows[0]["STS_PKID"].ToString());
                                returnValue.USR_HashedPassword = System.Text.Encoding.UTF8.GetBytes(ds.Tables[0].Rows[0]["USR_HashedPassword"].ToString());
                                
                                returnValue.noError = true;
                            }
                        }
                        else
                        {
                            //returnValue = null;
                            returnValue.noError = false;
                            returnValue.errorMessage = "User not found";
                        }
                        #endregion
                    }
                    else
                    {
                        returnValue.noError = false;
                        returnValue.errorMessage = "User not found";
                    }

                }
                else
                {
                    returnValue.noError = false;
                    returnValue.errorMessage = "User not found";
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;
        }

        private BilletterieAPIWS.userProfileObject PopulateUserObjectFromERMS(DataSet ifxDS)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                if (ifxDS != null)
                {
                    if (ifxDS.Tables[0].Rows.Count > 0)
                    {
                        returnValue.USC_PKID = 1;
                        returnValue.USR_UserLogin = ifxDS.Tables[0].Rows[0]["agent_code"].ToString();
                        returnValue.USR_PassKey = GetMD5Hash(ifxDS.Tables[0].Rows[0]["password"].ToString());
                        returnValue.USR_HashedPassword = System.Text.Encoding.UTF8.GetBytes(ifxDS.Tables[0].Rows[0]["password"].ToString()); //GetMD5Hash(ifxDS.Tables[0].Rows[0]["password"].ToString());
                        returnValue.USR_FullName = ifxDS.Tables[0].Rows[0]["agent_name"].ToString().Trim(); 
                        returnValue.USR_IDNumber = ifxDS.Tables[0].Rows[0]["agent_id_no"].ToString().Trim();
                        returnValue.USR_FirstName = GetFirstSplitValue(ifxDS.Tables[0].Rows[0]["agent_name"].ToString()).Trim();
                        returnValue.USR_LastName = GetLastSplitValue(ifxDS.Tables[0].Rows[0]["agent_name"].ToString()).Trim();
                        returnValue.USR_MobileNumber = ifxDS.Tables[0].Rows[0]["cell_no"].ToString();
                        returnValue.USR_EmailAccount = ifxDS.Tables[0].Rows[0]["email"].ToString();
                        returnValue.USR_DateCreated = DateTime.Now.ToString();
                        returnValue.USR_ActivationDate = DateTime.Now.ToString();
                        returnValue.STS_PKID = 14;  //Account is new
                        returnValue.USG_PKID = Int32.Parse(ConfigurationManager.AppSettings["DefaultUserOffice"]);
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                    }
                }
                else
                {
                    returnValue.noError = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;

        }

        private BilletterieAPIWS.userProfileObject PopulateUserObjectFromCUBA(DataSet cubaDS)
        {
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                if (cubaDS != null)
                {
                    if (cubaDS.Tables[0].Rows.Count > 0)
                    {
                        returnValue.USC_PKID = 3;
                        returnValue.USR_UserLogin = cubaDS.Tables[0].Rows[0]["USR_Username"].ToString();
                        returnValue.USR_PassKey = cubaDS.Tables[0].Rows[0]["USR_Password"].ToString();

                        returnValue.USR_FirstName = GetFirstSplitValue(cubaDS.Tables[0].Rows[0]["USR_AddForServiceName"].ToString()).Trim();
                        returnValue.USR_LastName = GetLastSplitValue(cubaDS.Tables[0].Rows[0]["USR_AddForServiceName"].ToString()).Trim();
                        returnValue.USR_MobileNumber = cubaDS.Tables[0].Rows[0]["USR_Phone"].ToString();
                        returnValue.USR_EmailAccount = cubaDS.Tables[0].Rows[0]["USR_Email"].ToString();
                        returnValue.USR_DateCreated = DateTime.Now.ToString();
                        returnValue.USR_ActivationDate = DateTime.Now.ToString();
                        returnValue.STS_PKID = 10;
                        returnValue.USG_PKID = Int32.Parse(ConfigurationManager.AppSettings["DefaultUserOffice"]);
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                    }
                }
                else
                {
                    returnValue.noError = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;

        }

        private BilletterieAPIWS.userProfileObject PopulateUserObjectFromPtolemy(DataSet ptoDS)
        {
            Common cm = new Common();
            BilletterieAPIWS.userProfileObject returnValue = new BilletterieAPIWS.userProfileObject();
            try
            {
                if (ptoDS != null)
                {
                    if (ptoDS.Tables[0].Rows.Count > 0)
                    {
                        returnValue.USC_PKID = 4;
                        returnValue.USR_UserLogin = ptoDS.Tables[0].Rows[0]["login"].ToString();
                        returnValue.USR_PassKey = ptoDS.Tables[0].Rows[0]["pwd"].ToString();
                        returnValue.USR_FirstName = cm.GetFirstSplitValue(ptoDS.Tables[0].Rows[0]["name"].ToString()).Trim();
                        returnValue.USR_LastName = cm.GetLastSplitValue(ptoDS.Tables[0].Rows[0]["name"].ToString()).Trim();
                        returnValue.USR_MobileNumber = ptoDS.Tables[0].Rows[0]["phone"].ToString();
                        returnValue.USR_EmailAccount = ptoDS.Tables[0].Rows[0]["email"].ToString();
                        returnValue.USR_DateCreated = DateTime.Now.ToString();
                        returnValue.USR_ActivationDate = DateTime.Now.ToString();
                        returnValue.STS_PKID = 10;
                        returnValue.USG_PKID = Int32.Parse(ConfigurationManager.AppSettings["DefaultUserOffice"]);
                        returnValue.noError = true;
                    }
                    else
                    {
                        returnValue.noError = false;
                    }
                }
                else
                {
                    returnValue.noError = false;
                }
            }
            catch (Exception ex)
            {
                returnValue.noError = false;
                returnValue.errorMessage = ex.Message;
            }
            return returnValue;

        }

        public string GetFirstSplitValue(string fullTextValue)
        {
            string returnValue = fullTextValue;
            string[] multiValues = fullTextValue.Split(' ');
            if (fullTextValue.Contains(" "))
            {
                returnValue = multiValues[0];
            }
            return returnValue;
        }

        public string GetLastSplitValue(string fullTextValue)
        {
            string returnValue = "";
            string[] multiValues = fullTextValue.Split(' ');
            if (multiValues.Length > 1)
            {
                for (int i = 1; i < multiValues.Length; i++)
                {
                    returnValue = returnValue + " " + multiValues[i];
                }
            }
            else
            {
                returnValue = fullTextValue;
            }
            return returnValue;
        }

        public string CleanUpValues(string unCleanValue)
        {
            string retValue = "";
            
            if (unCleanValue != "" && unCleanValue != null)
            {
                retValue = unCleanValue.Remove(0, 1);
            }
            return retValue;
        }

        public string GetAllOfficerCategoryList(string officerPKID)
        {
            string returnValue = ",0";
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                //, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                ds = bilAPIWS.GetBilletterieDataSet("select F.CAT_PKID, C.CTL_PKID from TB_CATOFC F inner join TB_CAT_Category C on F.CAT_PKID = C.CAT_PKID where OFC_PKID = " + officerPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select F.CAT_PKID, C.CTL_PKID from TB_CATOFC F inner join TB_CAT_Category C on F.CAT_PKID = C.CAT_PKID where OFC_PKID = " + officerPKID );
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["CAT_PKID"].ToString();
                        }

                        #region Populate second level
                       
                        DataSet ds1 = new DataSet();
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {

                            if (ds.Tables[0].Rows[j]["CTL_PKID"].ToString() == "1")
                            {
                                ds1 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //ds1 = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString());
                                if (ds1 != null)
                                {
                                    if (ds1.Tables[0].Rows.Count > 0)
                                    {
                                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                        {
                                            returnValue = returnValue + ", " + ds1.Tables[0].Rows[k]["CAT_PKID"].ToString();
                                        }

                                        #region Populate 2nd level
                                        DataSet ds2 = new DataSet();
                                        for (int m = 0; m < ds1.Tables[0].Rows.Count; m++)
                                        {
                                            if (ds1.Tables[0].Rows[m]["CTL_PKID"].ToString() == "2")
                                            {
                                                ds2 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds1.Tables[0].Rows[m]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                //ds2 = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds1.Tables[0].Rows[m]["CAT_PKID"].ToString());
                                                if (ds2 != null)
                                                {
                                                    if (ds2.Tables[0].Rows.Count > 0)
                                                    {
                                                        for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                                        {
                                                            returnValue = returnValue + ", " + ds2.Tables[0].Rows[n]["CAT_PKID"].ToString();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }

                            }
                            else if (ds.Tables[0].Rows[j]["CTL_PKID"].ToString() == "2")
                            {
                                ds1 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                //ds1 = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString());
                                if (ds1 != null)
                                {
                                    if (ds1.Tables[0].Rows.Count > 0)
                                    {
                                        for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                        {
                                            returnValue = returnValue + ", " + ds1.Tables[0].Rows[k]["CAT_PKID"].ToString();
                                        }

                                        #region Populate 3rd level
                                        DataSet ds2 = new DataSet();
                                        for (int m = 0; m < ds1.Tables[0].Rows.Count; m++)
                                        {
                                            if (ds1.Tables[0].Rows[m]["CTL_PKID"].ToString() == "3")
                                            {
                                                ds2 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds1.Tables[0].Rows[m]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                                //ds2 = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select CAT_PKID, CTL_PKID from TB_CAT_Category where CAT_MasterID = " + ds1.Tables[0].Rows[m]["CAT_PKID"].ToString());
                                                if (ds2 != null)
                                                {
                                                    if (ds2.Tables[0].Rows.Count > 0)
                                                    {
                                                        for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                                        {
                                                            returnValue = returnValue + ", " + ds2.Tables[0].Rows[n]["CAT_PKID"].ToString();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }

                            }


                        #endregion
                        }
                    }
                }


            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetOfficerCategoryList(string officerPKID)
        {
            string returnValue = ",0";
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select F.CAT_PKID from TB_CATOFC F inner join TB_CAT_Category C on F.CAT_PKID = C.CAT_PKID where OFC_PKID = " + officerPKID + " and C.CAT_MasterID = 0", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select F.CAT_PKID from TB_CATOFC F inner join TB_CAT_Category C on F.CAT_PKID = C.CAT_PKID where OFC_PKID = " + officerPKID + " and C.CAT_MasterID = 0");
                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["CAT_PKID"].ToString();
                        }

                    }
                }
            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetTicketAssigneeList(string[] tckPKIDs)
        {
            string cleanedUpValue = ",0";
            string returnValue = "";
            string ticketList = "";
            try
            {

                for (int i = 0; i < tckPKIDs.Length; i++)
                {
                    if (tckPKIDs[i] != "")
                    {
                        ticketList = ticketList + "," + tckPKIDs[i];
                    }
                }

                ticketList = CleanUpValues(ticketList);

                DataAccess da = new DataAccess();

                DataTable dTable = new DataTable();
                dTable = bilAPIWS.GetBilletterieGenericDataTable("select CAT_PKID from TB_TCK_Ticket where TCK_PKID in (" + ticketList + ")", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dTable = da.GetBilletterieGenericDataTable("select CAT_PKID from TB_TCK_Ticket where TCK_PKID in (" + ticketList + ")");
                string CATList = "";
                if (dTable != null)
                {
                    if (dTable.Rows.Count > 0)
                    {
                        for (int i = 0; i < dTable.Rows.Count; i++)
                        {
                            CATList = CATList + ", " + dTable.Rows[i]["CAT_PKID"].ToString();
                        }
                    }
                }
                CATList = CleanUpValues(CATList);

                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID in (" + CATList + ")", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID in (" + CATList + ")");

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            returnValue = returnValue + ", " + GetParentCategoryList(ds.Tables[0].Rows[j]["CAT_PKID"].ToString());
                        }
                    }
                }

                string catList = CleanUpValues(returnValue);
                DataSet dsCatList = new DataSet();
                dsCatList = bilAPIWS.GetBilletterieDataSet("select OFC_PKID from TB_CATOFC F where CAT_PKID in (" + catList + ")", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dsCatList = da.GetGenericBilletterieDataSet("TB_CATOFC", "TB_CATOFCDS", "select OFC_PKID from TB_CATOFC F where CAT_PKID in (" + catList + ")");
                if (dsCatList != null)
                {
                    if (dsCatList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsCatList.Tables[0].Rows.Count; i++)
                        {
                            cleanedUpValue = cleanedUpValue + ", " + dsCatList.Tables[0].Rows[i]["OFC_PKID"].ToString();
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

            return CleanUpValues(cleanedUpValue);
        }

        public string GetTicketAssigneeList(string tckPKID)
        {
            string cleanedUpValue = ",0";
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();

                BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                selResp = bilAPIWS.GetBilletterieScalar("select CAT_PKID from TB_TCK_Ticket where TCK_PKID = " + tckPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //selResp = da.GetBilletterieGenericScalar("select CAT_PKID from TB_TCK_Ticket where TCK_PKID = " + tckPKID);

                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID = " + selResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID = " + selResp.selectedPKID);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue = GetParentCategoryList(ds.Tables[0].Rows[0]["CAT_PKID"].ToString());
                    }
                }

                string catList = CleanUpValues(returnValue);
                DataSet dsCatList = new DataSet();
                dsCatList = bilAPIWS.GetBilletterieDataSet("select OFC_PKID from TB_CATOFC F where CAT_PKID in (" + catList + ")", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dsCatList = da.GetGenericBilletterieDataSet("TB_CATOFC", "TB_CATOFCDS", "select OFC_PKID from TB_CATOFC F where CAT_PKID in (" + catList + ")");
                if (dsCatList != null)
                {
                    if (dsCatList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsCatList.Tables[0].Rows.Count; i++)
                        {
                            cleanedUpValue = cleanedUpValue + ", " + dsCatList.Tables[0].Rows[i]["OFC_PKID"].ToString();
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

            return CleanUpValues(cleanedUpValue);
        }

        public string GetTicketAssigneeList(int catPKID)
        {
            string cleanedUpValue = ",0";
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID = " + catPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_TCK_Ticket", "TB_TCK_TicketDS", "select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID = " + catPKID);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue = GetParentCategoryList(ds.Tables[0].Rows[0]["CAT_PKID"].ToString());
                    }
                }

                string catList = CleanUpValues(returnValue);
                DataSet dsCatList = new DataSet();
                dsCatList = bilAPIWS.GetBilletterieDataSet("select OFC_PKID from TB_CATOFC F where CAT_PKID in (" + catList + ")", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dsCatList = da.GetGenericBilletterieDataSet("TB_CATOFC", "TB_CATOFCDS", "select OFC_PKID from TB_CATOFC F where CAT_PKID in (" + catList + ")");
                if (dsCatList != null)
                {
                    if (dsCatList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsCatList.Tables[0].Rows.Count; i++)
                        {
                            cleanedUpValue = cleanedUpValue + ", " + dsCatList.Tables[0].Rows[i]["OFC_PKID"].ToString();
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
            return CleanUpValues(cleanedUpValue);
        }

        public string GetCategoryMembersList(string catPKID)
        {
            string cleanedUpValue = "";
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID = " + catPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select C.CAT_PKID, C.CTL_PKID, C.CAT_MasterID from TB_CAT_Category C where C.CAT_PKID = " + catPKID);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue = GetParentCategoryList(ds.Tables[0].Rows[0]["CAT_PKID"].ToString());
                    }
                }

                string catList = CleanUpValues(returnValue);
                DataSet dsCatList = new DataSet();
                dsCatList = bilAPIWS.GetBilletterieDataSet("select F.OFC_PKID, OFC_EmailAccount from TB_CATOFC F inner join TB_CAT_Category C on F.CAT_PKID = C.CAT_PKID inner join TB_OFC_Officer O on F.OFC_PKID = O.OFC_PKID where C.CAT_NotifyEmail = 1 and O.OFC_NewTicketEmail = 1 and F.CAT_PKID in (" + catList + ")", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //dsCatList = da.GetGenericBilletterieDataSet("TB_CATOFC", "TB_CATOFCDS", "select F.OFC_PKID, OFC_EmailAccount from TB_CATOFC F inner join TB_CAT_Category C on F.CAT_PKID = C.CAT_PKID inner join TB_OFC_Officer O on F.OFC_PKID = O.OFC_PKID where C.CAT_NotifyEmail = 1 and O.OFC_NewTicketEmail = 1 and F.CAT_PKID in (" + catList + ")");
                if (dsCatList != null)
                {
                    if (dsCatList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsCatList.Tables[0].Rows.Count; i++)
                        {
                            cleanedUpValue = cleanedUpValue + "; " + dsCatList.Tables[0].Rows[i]["OFC_EmailAccount"].ToString();
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

            return CleanUpValues(cleanedUpValue);
        }

        public bool ValidMimeType(string mimeType, string fileExtention)
        {
            bool mimeTypeFound = false;
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_AMT_AllowedMimeType", "TB_AMT_AllowedMimeTypeDS", "select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (mimeType.ToLower() == ds.Tables[0].Rows[i]["AMT_Name"].ToString().ToLower() && fileExtention.ToLower() == ds.Tables[0].Rows[i]["AMT_Extention"].ToString().ToLower())
                {
                    mimeTypeFound = true;
                    break;
                }
            }
            return mimeTypeFound;
        }

        public string GetPDFHyperLinks(byte[] fileField, string fileExtention)
        {
            string returnValue = "";
            if (fileExtention.ToLower() == ".pdf")
            {
                List<string> listValue = new List<string>();
                RasterizePDF rasterPDF = new RasterizePDF();
                listValue = rasterPDF.rasterizePDFFile(fileField);
                if (listValue != null)
                {
                    returnValue = "Invalid PDF file document.";
                }
            }
            else
            {
                returnValue = "Invalid file type.";
            }
            return returnValue;
        }

        public bool ValidCBRSMimeType(string mimeType, string fileExtention)
        {
            bool mimeTypeFound = false;
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetCBRSDataSet("select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericCBRSDataSet("TB_AMT_AllowedMimeType", "TB_AMT_AllowedMimeTypeDS", "select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (mimeType.ToLower() == ds.Tables[0].Rows[i]["AMT_Name"].ToString().ToLower() && fileExtention.ToLower() == ds.Tables[0].Rows[i]["AMT_Extention"].ToString().ToLower())
                {
                    mimeTypeFound = true;
                    break;
                }
            }
            return mimeTypeFound;
        }

        public bool ValidateFileCount(int count)
        {
            bool inRange = false;

            if (count <= Int32.Parse(ConfigurationManager.AppSettings["MaxUploadFiles"]))
            {
                inRange = true;
            }

            return inRange;
        }
        
        public bool ValidFileSize(int fileSize)
        {
            bool fileSizeValid = false;

            if (fileSize <= Int32.Parse(ConfigurationManager.AppSettings["MaxUploadSize"]))
            {
                fileSizeValid = true;
            }
            return fileSizeValid;
        }

        public string GetValidMimeType()
        {
            string mimeTypeString = "";
            DataAccess da = new DataAccess();
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_AMT_AllowedMimeType", "TB_AMT_AllowedMimeTypeDS", "select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType where STS_PKID = 60");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["AMT_Extention"].ToString().ToLower().Trim() != "")
                {
                    mimeTypeString = mimeTypeString + ds.Tables[0].Rows[i]["AMT_Extention"].ToString().ToLower().Trim().Replace(".", ",");
                }
            }
            return CleanUpValues(mimeTypeString);
        }

        public void CleanUpTempFiles(int userID)
        {
            DataAccess da = new DataAccess();
            string fileName = ConfigurationManager.AppSettings["LocalDocumentsTempPath"] + "doc" + userID;
            DataSet ds = new DataSet();
            ds = bilAPIWS.GetBilletterieDataSet("select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //ds = da.GetGenericBilletterieDataSet("TB_AMT_AllowedMimeType", "TB_AMT_AllowedMimeTypeDS", "select AMT_PKID, AMT_Name, AMT_Extention, AMT_Description from TB_AMT_AllowedMimeType");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if ((System.IO.File.Exists(fileName + ds.Tables[0].Rows[i]["AMT_Extention"].ToString())))
                {
                    System.IO.File.Delete(fileName + ds.Tables[0].Rows[i]["AMT_Extention"].ToString());
                }
            }
        }

        public string MoveDocuments(string documentFullPath, string documentID)
        {
            string destinationFileName = ConfigurationManager.AppSettings["LocalDocumentsContentPath"] + "ticket" + documentID + Path.GetExtension(documentFullPath);
            if (!(System.IO.File.Exists(destinationFileName)))
            {
                System.IO.File.Copy(documentFullPath, destinationFileName);
            }
            return destinationFileName;
        }

        public string MoveCBRSDocuments(string documentFullPath, string documentID)
        {
            string destinationFileName = ConfigurationManager.AppSettings["LocalDocumentsCBRSContentPath"] + "cbrs" + documentID + Path.GetExtension(documentFullPath);
            if (!(System.IO.File.Exists(destinationFileName)))
            {
                System.IO.File.Copy(documentFullPath, destinationFileName);
            }
            return destinationFileName;
        }

        public string DeleteDocuments(string documentFullPath)
        {
            string retValue = "";
            try
            {
                if (!(System.IO.File.Exists(documentFullPath)))
                {
                    System.IO.File.Delete(documentFullPath);
                }
            }
            catch (Exception ex)
            {
                retValue = ex.Message;
            }
            return retValue;
        }

        public string MoveTemplates(string documentFullPath, string documentID)
        {
            string destinationFileName = ConfigurationManager.AppSettings["LocalDocumentsContentPath"] + "template" + documentID + Path.GetExtension(documentFullPath);
            if (!(System.IO.File.Exists(destinationFileName)))
            {
                System.IO.File.Copy(documentFullPath, destinationFileName);
                System.IO.File.Delete(documentFullPath);
            }
            return destinationFileName;
        }

        public string GetChildCategoryList(string CATPKID)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();

                returnValue = returnValue + ", " + CATPKID;
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + CATPKID);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["CAT_PKID"].ToString();
                        }

                        #region Populate second level
                        DataSet ds1 = new DataSet();
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            ds1 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //ds1 = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString());
                            if (ds1 != null)
                            {
                                if (ds1.Tables[0].Rows.Count > 0)
                                {
                                    for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                    {
                                        returnValue = returnValue + ", " + ds1.Tables[0].Rows[k]["CAT_PKID"].ToString();
                                    }

                                    #region Populate 3rd level
                                    DataSet ds2 = new DataSet();
                                    for (int m = 0; m < ds1.Tables[0].Rows.Count; m++)
                                    {
                                        ds2 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //ds2 = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString());
                                        if (ds2 != null)
                                        {
                                            if (ds2.Tables[0].Rows.Count > 0)
                                            {
                                                for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                                {
                                                    returnValue = returnValue + ", " + ds2.Tables[0].Rows[n]["CAT_PKID"].ToString();
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }

                        }
                        #endregion

                    }
                }


            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetWeekDayCount(string weekDate)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();

                returnValue = returnValue + ", 1";
                DataSet ds = new DataSet();
                ds = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_MasterID = 1", ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_MasterID = 1");

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["CAT_PKID"].ToString();
                        }

                        #region Populate second level
                        DataSet ds1 = new DataSet();
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            ds1 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //ds1 = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString());
                            if (ds1 != null)
                            {
                                if (ds1.Tables[0].Rows.Count > 0)
                                {
                                    for (int k = 0; k < ds1.Tables[0].Rows.Count; k++)
                                    {
                                        returnValue = returnValue + ", " + ds1.Tables[0].Rows[k]["CAT_PKID"].ToString();
                                    }

                                    #region Populate 3rd level
                                    DataSet ds2 = new DataSet();
                                    for (int m = 0; m < ds1.Tables[0].Rows.Count; m++)
                                    {
                                        ds2 = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                                        //ds2 = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString());
                                        if (ds2 != null)
                                        {
                                            if (ds2.Tables[0].Rows.Count > 0)
                                            {
                                                for (int n = 0; n < ds2.Tables[0].Rows.Count; n++)
                                                {
                                                    returnValue = returnValue + ", " + ds2.Tables[0].Rows[n]["CAT_PKID"].ToString();
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }

                        }
                        #endregion

                    }
                }


            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetParentCategoryList(string CATPKID)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();

                returnValue = returnValue + ", " + CATPKID;

               BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                selResp = bilAPIWS.GetBilletterieScalar("select CTL_PKID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //selResp = da.GetBilletterieGenericScalar("select CTL_PKID from TB_CAT_Category where CAT_PKID = " + CATPKID);

                //Department
                if (selResp.selectedPKID == "1")
                {
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_MasterID = " + CATPKID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["CAT_PKID"].ToString();
                        }
                      
                    }
                }

                else if (selResp.selectedPKID == "2")
                {

                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //masterResp = da.GetBilletterieGenericScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + CATPKID);

                    //Get super category 
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_PKID = " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_PKID = " + masterResp.selectedPKID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["CAT_PKID"].ToString();
                        }
                    }
                }

                else if (selResp.selectedPKID == "3")
                {

                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //masterResp = da.GetBilletterieGenericScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + CATPKID);

                    //Get 2nd level category
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_PKID = " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_PKID from TB_CAT_Category where CAT_PKID = " + masterResp.selectedPKID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["CAT_PKID"].ToString();
                        }

                        #region Populate second level
                       
                        DataSet ds1 = new DataSet();

                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            BilletterieAPIWS.SelectStringResponseObject level1Resp = new BilletterieAPIWS.SelectStringResponseObject();
                            level1Resp = bilAPIWS.GetBilletterieScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //level1Resp = da.GetBilletterieGenericScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + ds.Tables[0].Rows[j]["CAT_PKID"].ToString());

                            returnValue = returnValue + ", " + level1Resp.selectedPKID;
                        }
                        
                        #endregion

                    }
                }


            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetParentDepartmentScalar(string CATPKID)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();
                BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                selResp = bilAPIWS.GetBilletterieIntScalar("select CTL_PKID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                if (selResp.selectedPKID == "2")
                {
                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieIntScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    //Get super category 
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select CAT_PKID from TB_CAT_Category where CAT_PKID = " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[0]["CAT_PKID"].ToString();
                    }
                }

            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetParentCategoryName(string CATPKID)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();
               BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                selResp = bilAPIWS.GetBilletterieIntScalar("select CTL_PKID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                if (selResp.selectedPKID == "3")
                {
                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieIntScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    //Get super category 
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select CAT_CategoryName, CAT_PKID from TB_CAT_Category where CAT_PKID = " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = bilAPIWS.GetBilletterieOfficerObject(username, password, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    string categoryName = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                       categoryName = ds.Tables[0].Rows[0]["CAT_CategoryName"].ToString();
                    }

                    BilletterieAPIWS.SelectStringResponseObject categResp = new BilletterieAPIWS.SelectStringResponseObject();
                    categResp = bilAPIWS.GetBilletterieIntScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + ds.Tables[0].Rows[0]["CAT_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    DataSet ds1 = new DataSet();
                    ds1 = bilAPIWS.GetBilletterieDataSet("select CAT_CategoryName, CAT_PKID from TB_CAT_Category where CAT_PKID = " + categResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        returnValue = ds1.Tables[0].Rows[0]["CAT_CategoryName"].ToString() + @" \ " + categoryName;
                    }
                }

                if (selResp.selectedPKID == "2")
                {
                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieIntScalar("select CAT_MasterID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    //Get super category 
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select CAT_CategoryName, CAT_PKID from TB_CAT_Category where CAT_PKID = " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue = ds.Tables[0].Rows[0]["CAT_CategoryName"].ToString();
                    }

                }

            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        public string GetCategoryName(string CATPKID)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();
                //Get super category 
                DataSet ds = new DataSet();
                //BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                ds = bilAPIWS.GetBilletterieDataSet("select CAT_CategoryName, CAT_PKID from TB_CAT_Category where CAT_PKID = " + CATPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //ds = bilAPIWS.GetBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_CategoryName, CAT_PKID from TB_CAT_Category where CAT_PKID = " + CATPKID);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    returnValue = ds.Tables[0].Rows[0]["CAT_CategoryName"].ToString();
                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        public string GetUniqueDateTimeStamp()
        {
            string returnValue = "";
            returnValue = returnValue + DateTime.Now.Year.ToString();
            returnValue = returnValue + DateTime.Now.Month.ToString("D2");
            returnValue = returnValue + DateTime.Now.Day.ToString("D2");
            returnValue = returnValue + DateTime.Now.Hour.ToString("D2");
            returnValue = returnValue + DateTime.Now.Minute.ToString("D2");
            returnValue = returnValue + DateTime.Now.Second.ToString("D2");
            return returnValue;
        }

        public DataSet GetGenericDataSet(string _tableName, string _dataSetName, string _sqlStatement)
        {
            return bilAPIWS.GetBilletterieDataSet(_sqlStatement, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
        }

        public BilletterieAPIWS.lOperationResponse InsertGenericRecord(string _sqlStatement)
        {
            DataAccess da = new DataAccess();
            BilletterieAPIWS.lOperationResponse retValue = new BilletterieAPIWS.lOperationResponse();
            retValue = bilAPIWS.InsertBilletterieGenericRecord(_sqlStatement, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            return retValue;
        }

        public BilletterieAPIWS.lOperationResponse UpdateTableRecord(string _sqlStatement)
        {
            //DataAccess da = new DataAccess();
            BilletterieAPIWS.lOperationResponse retValue = new BilletterieAPIWS.lOperationResponse();
            BilletterieAPIWS.UpdateResponseObject l_retValue = new BilletterieAPIWS.UpdateResponseObject();
            l_retValue = bilAPIWS.UpdateBilletterieRecord(_sqlStatement, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            retValue.noError = l_retValue.noError;
            retValue.errorMessage = l_retValue.errorMessage;
            return retValue;
        }

        public string CustomizeDate(string unformattedDate)
        {
            string retValue = "";
            try
            {
                retValue = DateTime.Parse(unformattedDate).Year.ToString() + "-" + DateTime.Parse(unformattedDate).Month.ToString("D2") + "-" + DateTime.Parse(unformattedDate).Day.ToString("D2") + " 23:59:00.000";
            }
            catch (Exception)
            {
                retValue = "";
            }
            return retValue;
        }

        public string CustomizeDateNoTime(string unformattedDate)
        {
            string retValue = "";
            try
            {
                retValue = DateTime.Parse(unformattedDate).Year.ToString() + "/" + DateTime.Parse(unformattedDate).Month.ToString("D2") + "/" + DateTime.Parse(unformattedDate).Day.ToString("D2");
            }
            catch (Exception)
            {
                retValue = "";
            }
            return retValue;
        }

        public bool GetCategoryAttachmentRequirement(int catPKID)
        {
            bool returnValue = false;
            DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject selObj = new BilletterieAPIWS.SelectStringResponseObject();
            if (catPKID != 0)
            {
                selObj = bilAPIWS.GetBilletterieIntScalar("select CAT_RequireAttachment from TB_CAT_Category where CAT_PKID =  " + catPKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //selObj = da.GetBilletterieGenericIntScalar("select CAT_RequireAttachment from TB_CAT_Category where CAT_PKID =  " + catPKID.ToString());
                returnValue = bool.Parse(selObj.selectedPKID);
            }
            return returnValue;
        }

        public string GetCategoryForcedFieldID(int catPKID)
        {
            string returnValue = "";
            DataAccess da = new DataAccess();
            BilletterieAPIWS.SelectStringResponseObject selObj = new BilletterieAPIWS.SelectStringResponseObject();
            if (catPKID != 0)
            {
                selObj = bilAPIWS.GetBilletterieIntScalar("select CAT_ForcedFieldID from TB_CAT_Category where CAT_PKID =  " + catPKID.ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //selObj = da.GetBilletterieGenericIntScalar("select CAT_ForcedFieldID from TB_CAT_Category where CAT_PKID =  " + catPKID.ToString());
                returnValue = selObj.selectedPKID;
            }
            return returnValue;
        }

        public string GetMainCategoryDescription(string CAT_PKID)
        {
            string retValue = "";
            Common cm = new Common();
            string masterCategory = cm.GetParentCategoryName(CAT_PKID);
            if (masterCategory != "")
            {
                retValue = masterCategory;
            }
            return retValue;
        }

        public string ValidateInput(BilletterieAPIWS.ticketObject tckObj)
        {
            Common cm = new Common();
            string returnValue = "";

            if (tckObj != null)
            {

                if (tckObj.CAT_PKID == 0)
                {
                    return returnValue = "Please select department/category.";
                }
                if (tckObj.USR_PKID == 0)
                {
                    return returnValue = "Please make sure you are logged in.";
                }
                if (tckObj.TCK_Subject == "")
                {
                    return returnValue = "Ticket subject is required.";
                }
                if (tckObj.TCK_Subject.Trim().Length > 150)
                {
                    return returnValue = "Ticket subject is too long. Max is 150.";
                }
                if (tckObj.TCK_Message == "")
                {
                    return returnValue = "Ticket message is required.";
                }

                if (tckObj.TCK_Message.Trim().Length > 2500)
                {
                    return returnValue = "Ticket message is too long. Max is 2500.";
                }

                if (tckObj.AttachedFiles != null)
                {
                    if (!cm.ValidMimeType(tckObj.AttachedFiles[0].MimeType, tckObj.AttachedFiles[0].DCM_Extention))
                    {
                        return returnValue = "File type not supported.";
                    }
                    if (tckObj.AttachedFiles[0].AttachmentSize > 10000000)
                    {
                        return returnValue = "File is too large.";
                    }
                }

                if (tckObj.AttachedFiles != null)
                {

                    for (int i = 0; i < tckObj.AttachedFiles.Length; i++)
                    {
                        //List<string> linkList = new List<string>();
                        string errorMessage = "";
                        errorMessage = cm.GetPDFHyperLinks(tckObj.AttachedFiles[i].DCM_FileField, tckObj.AttachedFiles[i].DCM_Extention);
                        //if (linkList != null)
                        //{
                            return returnValue = errorMessage;  // "Invalid file type.";
                        //}
                    }
                }


                if (tckObj.CAT_RequireAttachment == true)
                {
                    if (tckObj.AttachedFile == null)
                    {
                        return returnValue = "You need to attach proof of transaction for this type of query.";
                    }
                }

                if (tckObj.CAT_ForcedFieldID != null && tckObj.CAT_ForcedFieldID != "")
                {
                    if (tckObj.CAT_ForcedFieldID == "1")
                    {
                        if (tckObj.TCK_CaseIdentifier == "")
                        {
                            return returnValue = "You need to provide Enterprise Number for this type of query.";
                        }
                    }
                }

                if (tckObj.PRV_PKID == 0)
                {
                    return returnValue = "Please select province.";
                }

            }
            else
            {
                return returnValue = "Ticket object is empty.";
            }

        

            return returnValue;
        }

        public string ValidateInputInternal(BilletterieAPIWS.ticketObject tckObj)
        {
            Common cm = new Common();
            string returnValue = "";

            if (tckObj != null)
            {

                if (tckObj.CAT_PKID == 0)
                {
                    return returnValue = "Please select department/category.";
                }
                if (tckObj.USR_PKID == 0)
                {
                    return returnValue = "Please make sure you are logged in.";
                }
                if (tckObj.TCK_Subject == "")
                {
                    return returnValue = "Ticket subject is required.";
                }
                if (tckObj.TCK_Subject.Trim().Length > 150)
                {
                    return returnValue = "Ticket subject is too long. Max is 150.";
                }
                if (tckObj.TCK_Message == "")
                {
                    return returnValue = "Ticket message is required.";
                }

                if (tckObj.TCK_Message.Trim().Length > 2500)
                {
                    return returnValue = "Ticket message is too long. Max is 2500.";
                }

                if (tckObj.AttachedFile != null)
                {
                    if (!cm.ValidMimeType(tckObj.AttachedFile.MimeType, tckObj.AttachedFile.DCM_Extention))
                    {
                        return returnValue = "File type not supported.";
                    }
                    if (tckObj.AttachedFile.AttachmentSize > 10000000)
                    {
                        return returnValue = "File is too large.";
                    }
                }

                if (tckObj.CAT_RequireAttachment == true && tckObj.TCK_IsLog != true)
                {
                    if (tckObj.AttachedFile == null)
                    {
                        return returnValue = "You need to attach proof of transaction for this type of query.";
                    }
                }

                if (tckObj.PRV_PKID == 0)
                {
                    return returnValue = "Please select province.";
                }

            }
            else
            {
                return returnValue = "Ticket object is empty.";
            }
            return returnValue;
        }

        public GenericImageObject GetImageFromFileSystem(string sPath)
        {
            GenericImageObject imgObj = new GenericImageObject();
            //Initialize byte array with a null value initially.
            byte[] data = null;
            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;
            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);
            //When you use BinaryReader, you need to 
            //supply number of bytes to read from file.
            //In this case we want to read entire file. 
            //So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);

            MemoryStream ms = new MemoryStream(data);
            imgObj.imageData = System.Drawing.Image.FromStream(ms, true);
            imgObj.extenstion = Path.GetExtension(sPath);
            imgObj.ImageType = Path.GetExtension(sPath);
            imgObj.tmImageHeight = imgObj.imageData.Height;
            imgObj.tmImageWidth = imgObj.imageData.Width;

            return imgObj;

        }

        public string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file

            string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string GetToolTipMessage(string masterPKID)
        {
            string returnValue = "";
            DataAccess da = new DataAccess();
            DataSet dsCategory = new DataSet();
            dsCategory = bilAPIWS.GetBilletterieDataSet("select CAT_CategoryDescription from TB_CAT_Category where CAT_PKID = " + masterPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
            //dsCategory = da.GetGenericBilletterieDataSet("TB_CAT_Category", "TB_CAT_CategoryDS", "select CAT_CategoryDescription from TB_CAT_Category where CAT_PKID = " + masterPKID);
            if (dsCategory != null)
            {
                if (dsCategory.Tables[0].Rows.Count > 0)
                {
                    returnValue = dsCategory.Tables[0].Rows[0]["CAT_CategoryDescription"].ToString();
                }
            }
            return returnValue;
        }

        public string GetOfficerList(string OFCPKID)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();
                returnValue = returnValue + ", " + OFCPKID;
                BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                selResp = bilAPIWS.GetBilletterieScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //selResp = da.GetBilletterieGenericOfficeScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID);

                //Department
                if (selResp.selectedPKID == "1")
                {
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_OFC_Officer", "TB_OFC_OfficerDS", "select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["OFC_PKID"].ToString();
                        }

                    }
                }

                else if (selResp.selectedPKID == "2")
                {

                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID =  " + OFCPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //masterResp = da.GetBilletterieGenericOfficeScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID =  " + OFCPKID);

                    //Get super category 
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select OFC_PKID from TB_OFC_Officer where OFC_PKID =  " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]); 
                    //ds = da.GetGenericBilletterieDataSet("TB_OFC_Officer", "TB_OFC_OfficerDS", "select OFC_PKID from TB_OFC_Officer where OFC_PKID =  " + masterResp.selectedUserPKID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["OFC_PKID"].ToString();
                        }
                    }
                }

                else if (selResp.selectedPKID == "3")
                {

                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //masterResp = da.GetBilletterieGenericOfficeScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID);

                    //Get 2nd level category
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_OFC_Officer", "TB_OFC_OfficerDS", "select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + masterResp.selectedUserPKID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            returnValue = returnValue + ", " + ds.Tables[0].Rows[i]["OFC_PKID"].ToString();
                        }

                        #region Populate second level

                        DataSet ds1 = new DataSet();
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            BilletterieAPIWS.SelectStringResponseObject level1Resp = new BilletterieAPIWS.SelectStringResponseObject();
                            level1Resp = bilAPIWS.GetBilletterieScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + ds.Tables[0].Rows[j]["OFC_PKID"].ToString(), ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                            //level1Resp = da.GetBilletterieGenericOfficeScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + ds.Tables[0].Rows[j]["OFC_PKID"].ToString());

                            returnValue = returnValue + ", " + level1Resp.selectedPKID;
                        }

                        #endregion

                    }
                }


            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetOfficerScalar(string OFCPKID)
        {
            string returnValue = "";
            try
            {
                DataAccess da = new DataAccess();
                BilletterieAPIWS.SelectStringResponseObject selResp = new BilletterieAPIWS.SelectStringResponseObject();
                selResp = bilAPIWS.GetBilletterieScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                //selResp = da.GetBilletterieGenericOfficeScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID);
                if (selResp.selectedPKID == "2")
                {
                    BilletterieAPIWS.SelectStringResponseObject masterResp = new BilletterieAPIWS.SelectStringResponseObject();
                    masterResp = bilAPIWS.GetBilletterieScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //masterResp = da.GetBilletterieGenericOfficeScalar("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + OFCPKID);

                    //Get super category 
                    DataSet ds = new DataSet();
                    ds = bilAPIWS.GetBilletterieDataSet("select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + masterResp.selectedPKID, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    //ds = da.GetGenericBilletterieDataSet("TB_OFC_Officer", "TB_OFC_OfficerDS", "select OFC_PKID from TB_OFC_Officer where OFC_PKID = " + masterResp.selectedUserPKID);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        returnValue = returnValue + ", " + ds.Tables[0].Rows[0]["OFC_PKID"].ToString();
                    }
                }

            }
            catch (Exception)
            {

            }
            return CleanUpValues(returnValue);
        }

        public string GetDecodedText(string inputText)
        {
            StringWriter outputWriter = new StringWriter();
            HttpUtility.HtmlDecode(inputText, outputWriter);
            return outputWriter.ToString();
        }

        public string GetEncodedText(string inputText)
        {
            return HttpUtility.HtmlEncode(inputText);
        }

        public string ValidateInputForXSS(string inputStringValue)
        {
            Common cm = new Common();
            string returnValue = "";

            if (!ValidateAntiXSS(inputStringValue))
            {
                return returnValue = "Search text contains illegal characters.";
            }
            if (!ValidateAntiXSSS(inputStringValue))
            {
                return returnValue = "Search text contains illegal characters.";
            }
            return returnValue;
        }

        public bool ValidateAntiXSS(string inputParameter)
        {
            if (string.IsNullOrEmpty(inputParameter))
                return true;

            var pattren = new StringBuilder();

            //Checks any js events i.e. onKeyUp(), onBlur(), alerts and custom js functions etc.             
            pattren.Append(@"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))");

            //Checks any html tags i.e. <script, <embed, <object etc.
            pattren.Append(@"|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))");

            return !Regex.IsMatch(System.Web.HttpUtility.UrlDecode(inputParameter), pattren.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public bool ValidateAntiXSSS(string inputParameter)
        {
            bool returnValue = true;

            if (string.IsNullOrEmpty(inputParameter))
                return true;

            if (inputParameter.Contains(">"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("<"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("="))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("+"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("%"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("!"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("@"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("#"))
            {
                returnValue = false;
            }

            if (inputParameter.Contains("&"))
            {
                returnValue = false;
            }

            return returnValue;
        }

        public string ValidateInputForXSS(BilletterieAPIWS.ticketObject tckObj)
        {
            Common cm = new Common();
            string returnValue = "";

            if (tckObj != null)
            {

                if (!ValidateAntiXSS(tckObj.TCK_Subject))
                {
                    return returnValue = "Ticket subject contains illegal characters.";
                }
                if (!ValidateAntiXSSS(tckObj.TCK_Subject))
                {
                    return returnValue = "Ticket subject contains illegal characters.";
                }

                if (!ValidateAntiXSS(tckObj.TCK_Message))
                {
                    return returnValue = "Ticket message contains illegal characters.";
                }
                if (!ValidateAntiXSSS(tckObj.TCK_Message))
                {
                    return returnValue = "Ticket message contains illegal characters.";
                }

                if (!ValidateAntiXSS(tckObj.TCK_Reference))
                {
                    return returnValue = "Reference contains illegal characters.";
                }
                if (!ValidateAntiXSSS(tckObj.TCK_Reference))
                {
                    return returnValue = "Reference contains illegal characters.";
                }

                //if (!ValidateAntiXSS(tckObj.TCK_AlternateEmail))
                //{
                //    return returnValue = "Email contains illegal characters.";
                //}
                //if (!ValidateAntiXSSS(tckObj.TCK_AlternateEmail))
                //{
                //    return returnValue = "Email contains illegal characters.";
                //}

                if (!ValidateAntiXSS(tckObj.TCK_CaseIdentifier))
                {
                    return returnValue = "Enterprise number contains illegal characters.";
                }
                if (!ValidateAntiXSSS(tckObj.TCK_CaseIdentifier))
                {
                    return returnValue = "Enterprise number contains illegal characters.";
                }

            }
            else
            {
                return returnValue = "Ticket object is empty.";
            }
            return returnValue;
        }

        public string ValidateInputForXSS(BilletterieAPIWS.cbrsCustomerDetailObject custDetailObj)
        {
            Common cm = new Common();
            string returnValue = "";

            if (custDetailObj != null)
            {
                if (!ValidateAntiXSS(custDetailObj.CUS_AccountHolderName))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }
                if (!ValidateAntiXSSS(custDetailObj.CUS_AccountHolderName))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }



                if (!ValidateAntiXSS(custDetailObj.CUS_AccountNumber))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }
                if (!ValidateAntiXSSS(custDetailObj.CUS_AccountNumber))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }

                if (!ValidateAntiXSS(custDetailObj.CUS_BankBranchCode))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }

                if (!ValidateAntiXSS(custDetailObj.CUS_ContactNumber))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }
                if (!ValidateAntiXSSS(custDetailObj.CUS_ContactNumber))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }


                if (!ValidateAntiXSS(custDetailObj.CUS_FullName))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }
                if (!ValidateAntiXSSS(custDetailObj.CUS_FullName))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }


                if (!ValidateAntiXSS(custDetailObj.CUS_IDNumber))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }
                if (!ValidateAntiXSSS(custDetailObj.CUS_IDNumber))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }

                if (!ValidateAntiXSS(custDetailObj.CUS_AgentCode))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }
                if (!ValidateAntiXSSS(custDetailObj.CUS_AgentCode))
                {
                    return returnValue = "Refund request contains illegal characters.";
                }

            }
            else
            {
                return returnValue = "Refund request object is empty.";
            }
            return returnValue;
        }



    }
}