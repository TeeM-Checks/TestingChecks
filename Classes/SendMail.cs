using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Collections;
using NewBilletterie.GlobalMailWS;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Runtime.Remoting.Messaging;
//using NewBilletterie.EmailWS;
using NewBilletterie.BilletterieAPIWS;

namespace NewBilletterie.Classes
{
    public enum Mail { SMTPServer = 0, From = 1, To = 2, Subject = 3, MailNotifications = 4, smtUserPass = 5, bcc = 6, MailBatchCount = 7, ToCIPCTM = 8, ToCIPCPT = 9, ToCIPCDS = 10, ToCIPCCP = 11, AltFrom = 12, FailedMailBatchCount = 13 };
    public enum AlternateEMailSettings { AltFrom = 0, AltSMTPUserPass = 1, AltSMTPServer = 2, MailBatchCount = 3, MailSortingOrder = 4 };


    public class SendMail : System.Net.Mail.MailMessage
    {
        BilletteriePrivateAPI bilAPIWS = new BilletteriePrivateAPI();

       EmailService cWS = new EmailService();

        public SMTPMailResponseObject SendSMTPMail(string sTo, string agentEmails, string sFrom, string sSubject, string sBody, string server)
        {
            cWS.Url = ConfigurationManager.AppSettings["GlobalMailWSURL"];
            SMTPMailResponseObject retValue = new SMTPMailResponseObject();
            SMTPMailResponse smtpResp = new SMTPMailResponse();
            try
            {
                smtpResp = cWS.SendCUBASMTPMail_MultipleBCC(GetMultipleEmailString(sTo), GetMultipleEmailString(agentEmails), sFrom, sSubject, sBody, server, GetConfig(Mail.smtUserPass.ToString()), GetConfig(Mail.bcc.ToString()));

                if (!smtpResp.noError)
                {
                    retValue.errorMessage = smtpResp.errorMessage;
                    retValue.noError = false;
                }
                else
                {
                    retValue.errorMessage = smtpResp.errorMessage;
                    retValue.noError = smtpResp.noError;
                }

            }
            catch (Exception ex)
            {
                retValue.errorMessage = ex.Message;
                retValue.noError = smtpResp.noError;

                //Add log entry here
            }
            return retValue;
        }

        public SMTPMailResponseObject SendSMTPMail(string sTo, string agentEmails, string sFrom, string sSubject, string sBody, string server, string cipcToEmail)
        {
            cWS.Url = ConfigurationManager.AppSettings["GlobalMailWSURL"];
            SMTPMailResponseObject retValue = new SMTPMailResponseObject();

            SMTPMailResponse smtpResp = new SMTPMailResponse();

            if (bool.Parse(GetConfig(Mail.MailNotifications.ToString())) == true)
            {
                try
                {
                    smtpResp = cWS.SendCUBASMTPMail_MultipleBCCSupport(GetMultipleEmailString(sTo), GetMultipleEmailString(agentEmails), sFrom, sSubject, sBody, server, GetConfig(Mail.smtUserPass.ToString()), GetConfig(Mail.bcc.ToString()), cipcToEmail);
                    if (!smtpResp.noError)
                    {
                        retValue.errorMessage = smtpResp.errorMessage;
                        retValue.noError = false;
                    }
                    else
                    {
                        retValue.errorMessage = smtpResp.errorMessage;
                        retValue.noError = smtpResp.noError;
                    }

                }
                catch (Exception ex)
                {
                    retValue.errorMessage = ex.Message;
                    retValue.noError = smtpResp.noError;
                    //Add log entry here
                }
            }
            return retValue;
        }

        public void SendSMTPMail(string sTo, string sFrom, string sSubject, string sBody, string server)
        {
            cWS.Url = ConfigurationManager.AppSettings["GlobalMailWSURL"];
            SMTPMailResponse smtpResp = new SMTPMailResponse();

            if (bool.Parse(GetConfig(Mail.MailNotifications.ToString())) == true)
            {
                try
                {
                    smtpResp = cWS.SendCUBASMTPMail_Single(GetMultipleEmailString(sTo), sFrom, sSubject, sBody, server, GetConfig(Mail.smtUserPass.ToString()));

                    if (!smtpResp.noError)
                    {
                        //Add log entry here
                    }

                }
                catch (Exception)
                {
                    //Add log entry here
                }
            }
        }

        private string GetConfig(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (ConfigurationException ex)
            {
                throw ex;
            }
        }

        private string GetMultipleEmailString(string longEmailString)
        {
            string retValue = "";
            string[] multiEmail = null;
            try
            {
                if (longEmailString.Contains(','))
                {
                    multiEmail = longEmailString.Split(',');
                    if (multiEmail != null)
                    {
                        multiEmail = RemoveDuplicates(multiEmail);
                        for (int i = 0; i < multiEmail.Length; i++)
                        {
                            if (multiEmail[i].Trim() != "")
                            {
                                retValue = retValue + ", " + multiEmail[i];
                            }
                        }
                        retValue = CleanUpValues(retValue).Trim();
                    }
                    else if (longEmailString != "")
                    {
                        retValue = longEmailString.Trim();
                    }
                }
                else
                {
                    retValue = longEmailString.Trim();
                }
            }
            catch (Exception)
            {
                retValue = "";
            }
            return retValue;
        }

        public string[] RemoveDuplicates(string[] myList)
        {
            ArrayList newList = new ArrayList();
            foreach (string str in myList)
                if (!newList.Contains(str.Trim()))
                    newList.Add(str.Trim());
            return (string[])newList.ToArray(typeof(string));
        }

        public string CleanUpValues(string unCleanValue)
        {
            string retValue = "";
            if (unCleanValue != "")
            {
                retValue = unCleanValue.Remove(0, 1);
            }
            return retValue;
        }

        public SMTPMailResponseObject SendSMTPMailAsync(string sTo, string agentEmails, string sFrom, string sSubject, string sBody, string server, string keyField, string keyValue, string emailDomain, int callDuration, out int threadId)
        {
            Thread.Sleep(callDuration);
            threadId = Thread.CurrentThread.ManagedThreadId;

            cWS.Url = ConfigurationManager.AppSettings["GlobalMailWSURL"];

            System.Net.ServicePointManager.Expect100Continue = false; 

            SMTPMailResponseObject retValue = new SMTPMailResponseObject();

            SMTPMailResponse smtpResp = new SMTPMailResponse();

            if (bool.Parse(GetConfig(Mail.MailNotifications.ToString())) == true)
            {
                try
                {
                    smtpResp = cWS.SendCUBASMTPMail_MultipleBCC(GetMultipleEmailString(sTo), GetMultipleEmailString(agentEmails), sFrom, sSubject, sBody, server, GetConfig(Mail.smtUserPass.ToString()), GetConfig(Mail.bcc.ToString()));

                    BilletterieAPIWS.EmailMessageObject emlObj = new BilletterieAPIWS.EmailMessageObject();
                    if (smtpResp.noError)
                    {
                        emlObj.EML_Status = "2";
                    }
                    else
                    {
                        emlObj.EML_Status = "1";
                    }
                    emlObj = PopulateEmailObject(sTo, agentEmails, sFrom, sSubject, sBody, server, emlObj.EML_Status, keyField, keyValue,emailDomain);

                    if (bool.Parse(ConfigurationManager.AppSettings["LogSentEmails"]))
                    {
                        BilletterieAPIWS.InsertResponseObject opResp = new BilletterieAPIWS.InsertResponseObject();
                        //EmailDispatcherService emsWS = new EmailDispatcherService();
                        //emsWS.Url = ConfigurationManager.AppSettings["EmailWSURL"];
                        opResp = bilAPIWS.AddEmailRecord(emlObj, ConfigurationManager.AppSettings["BillAPIUSR"], ConfigurationManager.AppSettings["BillAPIPWD"], ConfigurationManager.AppSettings["serviceKey"]);
                    }

                    if (!smtpResp.noError)
                    {
                        retValue.errorMessage = smtpResp.errorMessage;
                        retValue.noError = false;
                    }
                    else
                    {
                        retValue.errorMessage = smtpResp.errorMessage;
                        retValue.noError = smtpResp.noError;
                    }

                }
                catch (Exception ex)
                {
                    retValue.errorMessage = ex.Message;
                    retValue.noError = smtpResp.noError;
                    //Add log entry here
                }
            }
            return retValue;
        }

        private BilletterieAPIWS.EmailMessageObject PopulateEmailObject(string sTo, string agentEmails, string sFrom, string sSubject, string sBody, string server, string emailStatus, string keyField, string keyValue, string emailDomain)
        {
            BilletterieAPIWS.EmailMessageObject returnValue = new BilletterieAPIWS.EmailMessageObject();
            returnValue.EML_FromEmail = sFrom;
            returnValue.EML_ToEmailList = agentEmails;
            returnValue.EML_ToEmailAdmin = sTo;
            returnValue.EML_Subject = sSubject;
            returnValue.EML_MailBody = sBody;

            returnValue.EML_SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            returnValue.EML_SMTPPassword = ConfigurationManager.AppSettings["smtUserPass"];
            returnValue.EML_EmailDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
            returnValue.EML_Status = emailStatus;
            returnValue.EML_CCEmail = ConfigurationManager.AppSettings["bcc"];
            returnValue.EML_KeyField = keyField;
            returnValue.EML_KeyValue = keyValue;
            returnValue.EML_Domain = emailDomain;
            returnValue.EML_SupportToEmail = ConfigurationManager.AppSettings["ToCIPC"];
            return returnValue;
        }
       
        // The delegate must have the same signature as the method
        // it will call asynchronously.
        public delegate SMTPMailResponseObject AsyncMethodCaller(string sTo, string agentEmails, string sFrom, string sSubject, string sBody, string server, string keyField, string keyValue, string emailDomain, int callDuration, out int threadId);

        private void MyTaskWorker(string[] files)
        {
            foreach (string file in files)
            {
                // a time consuming operation with a file (compression, encryption etc.)
                Thread.Sleep(1000);
            }
        }

        private delegate void MyTaskWorkerDelegate(string[] files);

        private bool _myTaskIsRunning = false;

        public bool IsBusy
        {
            get { return _myTaskIsRunning; }
        }

        private readonly object _sync = new object();

        public void MyTaskAsync(string[] files)
        {
            MyTaskWorkerDelegate worker = new MyTaskWorkerDelegate(MyTaskWorker);
            AsyncCallback completedCallback = new AsyncCallback(MyTaskCompletedCallback);

            lock (_sync)
            {
                if (_myTaskIsRunning)
                    throw new InvalidOperationException("The control is currently busy.");

                AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                worker.BeginInvoke(files, completedCallback, async);
                _myTaskIsRunning = true;
            }
        }

        public event AsyncCompletedEventHandler MyTaskCompleted;

        private void MyTaskCompletedCallback(IAsyncResult ar)
        {
            // get the original worker delegate and the AsyncOperation instance
            MyTaskWorkerDelegate worker =
              (MyTaskWorkerDelegate)((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async = (AsyncOperation)ar.AsyncState;

            // finish the asynchronous operation
            worker.EndInvoke(ar);

            // clear the running task flag
            lock (_sync)
            {
                _myTaskIsRunning = false;
            }

            // raise the completed event
            AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null,
              false, null);
            async.PostOperationCompleted(
              delegate(object e) { OnMyTaskCompleted((AsyncCompletedEventArgs)e); },
              completedArgs);
        }

        protected virtual void OnMyTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (MyTaskCompleted != null)
                MyTaskCompleted(this, e);
        }
        

    }
}
