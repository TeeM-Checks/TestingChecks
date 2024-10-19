using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NewBilletterie.Classes
{

    public class downloadDocumentObject
    {
        public string repositoryID;
        public string mimeType;
        public string fileName;
        public byte[] docObj;
        private bool _noError;
        private string _errorMessage;

        public bool noError
        {
            get { return _noError; }
            set { _noError = value; }
        }

        public string errorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

    }

    public class lOperationResponse
        {
            private bool _noError;
            private string _errorMessage;

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }
        }

        public class allowedDateResponse
        {
            private bool _dateAllowed;
            private string _displayMessage;

            public bool dateAllowed
            {
                get { return _dateAllowed; }
                set { _dateAllowed = value; }
            }

            public string displayMessage
            {
                get { return _displayMessage; }
                set { _displayMessage = value; }
            }
        }

        public class InsertResponseObject
        {
            private string _insertedPKID;
            private bool _noError;
            private string _errorMessage;

            public string insertedPKID
            {
                get { return _insertedPKID; }
                set { _insertedPKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class UpdateResponseObject
        {
            private bool _noError;
            private string _errorMessage;

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class DeleteResponseObject
        {
            private bool _noError;
            private string _errorMessage;

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class SelectStringResponseObject
        {
            private string _selectedPKID;
            private bool _noError;
            private string _errorMessage;

            public string selectedPKID
            {
                get { return _selectedPKID; }
                set { _selectedPKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class SelectBoolResponseObject
        {
            private bool _selectedPKID;
            private bool _noError;
            private string _errorMessage;

            public bool selectedPKID
            {
                get { return _selectedPKID; }
                set { _selectedPKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class userProfileObject
        {
            private int _USR_PKID;
            private int _USC_PKID;
            private int _OFL_PKID;
            private int _ODP_PKID;
            private string _USR_UserLogin;
            private string _USR_PassKey;
            private string _USR_FirstName;
            private string _USR_LastName;
            private string _USR_MobileNumber;
            private string _USR_EmailAccount;
            private string _USR_DateCreated;
            private string _USR_ActivationDate;
            private string _USR_FaceImage;
            private int _STS_PKID;
            private int _USG_PKID;
            private int _OGA_PKID;
            private string _USR_Comments;
            private bool _OFC_PaginateResults;
            private string _USR_Salutation;
            private bool _OFC_NewTicketEmail;
            private bool _OFC_CanEdit;
            private bool _OFC_HideAssigned;
            private int _OFC_IsApprover;
            private bool _noError;
            private string _errorMessage;

            public int USR_PKID
            {
                get { return _USR_PKID; }
                set { _USR_PKID = value; }
            }

            public int USC_PKID
            {
                get { return _USC_PKID; }
                set { _USC_PKID = value; }
            }

            public int OFL_PKID
            {
                get { return _OFL_PKID; }
                set { _OFL_PKID = value; }
            }

            public int ODP_PKID
            {
                get { return _ODP_PKID; }
                set { _ODP_PKID = value; }
            }

            public string USR_UserLogin
            {
                get { return _USR_UserLogin; }
                set { _USR_UserLogin = value; }
            }

            public string USR_PassKey
            {
                get { return _USR_PassKey; }
                set { _USR_PassKey = value; }
            }

            public string USR_FirstName
            {
                get { return _USR_FirstName; }
                set { _USR_FirstName = value; }
            }

            public string USR_LastName
            {
                get { return _USR_LastName; }
                set { _USR_LastName = value; }
            }

            public string USR_MobileNumber
            {
                get { return _USR_MobileNumber; }
                set { _USR_MobileNumber = value; }
            }

            public string USR_EmailAccount
            {
                get { return _USR_EmailAccount; }
                set { _USR_EmailAccount = value; }
            }

            public string USR_DateCreated
            {
                get { return _USR_DateCreated; }
                set { _USR_DateCreated = value; }
            }

            public string USR_ActivationDate
            {
                get { return _USR_ActivationDate; }
                set { _USR_ActivationDate = value; }
            }

            public string USR_FaceImage
            {
                get { return _USR_FaceImage; }
                set { _USR_FaceImage = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public int USG_PKID
            {
                get { return _USG_PKID; }
                set { _USG_PKID = value; }
            }

            public int OGA_PKID
            {
                get { return _OGA_PKID; }
                set { _OGA_PKID = value; }
            }

            public string USR_Comments
            {
                get { return _USR_Comments; }
                set { _USR_Comments = value; }
            }

            public bool OFC_PaginateResults
            {
                get { return _OFC_PaginateResults; }
                set { _OFC_PaginateResults = value; }
            }

            public string USR_Salutation
            {
                get { return _USR_Salutation; }
                set { _USR_Salutation = value; }
            }

            public bool OFC_NewTicketEmail
            {
                get { return _OFC_NewTicketEmail; }
                set { _OFC_NewTicketEmail = value; }
            }

            public bool OFC_CanEdit
            {
                get { return _OFC_CanEdit; }
                set { _OFC_CanEdit = value; }
            }

            public bool OFC_HideAssigned
            {
                get { return _OFC_HideAssigned; }
                set { _OFC_HideAssigned = value; }
            }

            public int OFC_IsApprover
            {
                get { return _OFC_IsApprover; }
                set { _OFC_IsApprover = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }


        public class ticketInformationObject
        {
            private int _TIF_PKID;
            private string _TCK_PKID;

            private string _CustomerCode;
            private string _IdentityNumber;
            private string _Names;
            private string _EmailAccount;
            private string _PhoneNumber;
            private string _Province;

            private string _PhyStreetAddress1;
            private string _PhyStreetAddress2;
            private string _PhyCityTown;
            private string _PhyStateProvince;
            private string _PhyAreaCode;

            private string _PosStreetAddress1;
            private string _PosStreetAddress2;
            private string _PosCityTown;
            private string _PosStateProvince;
            private string _PosAreaCode;

            private bool _noError;
            private string _errorMessage;

            public int TIF_PKID
            {
                get { return _TIF_PKID; }
                set { _TIF_PKID = value; }
            }
            public string TCK_PKID
            {
                get { return _TCK_PKID; }
                set { _TCK_PKID = value; }
            }

            public string CustomerCode
            {
                get { return _CustomerCode; }
                set { _CustomerCode = value; }
            }
            public string IdentityNumber
            {
                get { return _IdentityNumber; }
                set { _IdentityNumber = value; }
            }
            public string Names
            {
                get { return _Names; }
                set { _Names = value; }
            }
            public string EmailAccount
            {
                get { return _EmailAccount; }
                set { _EmailAccount = value; }
            }
            public string PhoneNumber
            {
                get { return _PhoneNumber; }
                set { _PhoneNumber = value; }
            }
            public string Province
            {
                get { return _Province; }
                set { _Province = value; }
            }




            public string PhyStreetAddress1
            {
                get { return _PhyStreetAddress1; }
                set { _PhyStreetAddress1 = value; }
            }
            public string PhyStreetAddress2
            {
                get { return _PhyStreetAddress2; }
                set { _PhyStreetAddress2 = value; }
            }
            public string PhyCityTown
            {
                get { return _PhyCityTown; }
                set { _PhyCityTown = value; }
            }
            public string PhyStateProvince
            {
                get { return _PhyStateProvince; }
                set { _PhyStateProvince = value; }
            }
            public string PhyAreaCode
            {
                get { return _PhyAreaCode; }
                set { _PhyAreaCode = value; }
            }



            public string PosStreetAddress1
            {
                get { return _PosStreetAddress1; }
                set { _PosStreetAddress1 = value; }
            }
            public string PosStreetAddress2
            {
                get { return _PosStreetAddress2; }
                set { _PosStreetAddress2 = value; }
            }
            public string PosCityTown
            {
                get { return _PosCityTown; }
                set { _PosCityTown = value; }
            }
            public string PosStateProvince
            {
                get { return _PosStateProvince; }
                set { _PosStateProvince = value; }
            }
            public string PosAreaCode
            {
                get { return _PosAreaCode; }
                set { _PosAreaCode = value; }
            }



            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }
            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }


    public class cbrsCustomerDetailObject
    {
        private int _CUS_PKID;
        private int _BKN_PKID;
        private int _CTS_PKID;
        private int _USR_PKID;
        private string _CUS_AgentCode;
        private string _CUS_AgentBalance;
        private string _CUS_IDNumber;
        private string _CUS_FullName;
        private string _CUS_BankName;
        private string _CUS_BankBranchCode;
        private string _CUS_AccountHolderName;
        private string _CUS_AccountNumber;
        private string _CUS_ContactNumber;
        private string _CUS_ERMSStatus;
        private string _CUS_DateCreated;
        private string _CUS_DatePaid;
        private bool _CUS_IsOldBalance;
        private string _CUS_EmailAddress;
        private bool _CUS_FromMobile;
        private bool _CUS_IsEncrypted;
        private fileAttachmentObject[] _AttachedFiles;
        private bool _noError;
        private string _errorMessage;

        public int CUS_PKID
        {
            get { return _CUS_PKID; }
            set { _CUS_PKID = value; }
        }

        public int BKN_PKID
        {
            get { return _BKN_PKID; }
            set { _BKN_PKID = value; }
        }

        public int CTS_PKID
        {
            get { return _CTS_PKID; }
            set { _CTS_PKID = value; }
        }

        public int USR_PKID
        {
            get { return _USR_PKID; }
            set { _USR_PKID = value; }
        }

        public string CUS_AgentCode
        {
            get { return _CUS_AgentCode; }
            set { _CUS_AgentCode = value; }
        }

        public string CUS_AgentBalance
        {
            get { return _CUS_AgentBalance; }
            set { _CUS_AgentBalance = value; }
        }

        public string CUS_IDNumber
        {
            get { return _CUS_IDNumber; }
            set { _CUS_IDNumber = value; }
        }

        public string CUS_FullName
        {
            get { return _CUS_FullName; }
            set { _CUS_FullName = value; }
        }

        public string CUS_BankName
        {
            get { return _CUS_BankName; }
            set { _CUS_BankName = value; }
        }

        public string CUS_BankBranchCode
        {
            get { return _CUS_BankBranchCode; }
            set { _CUS_BankBranchCode = value; }
        }

        public string CUS_AccountHolderName
        {
            get { return _CUS_AccountHolderName; }
            set { _CUS_AccountHolderName = value; }
        }

        public string CUS_AccountNumber
        {
            get { return _CUS_AccountNumber; }
            set { _CUS_AccountNumber = value; }
        }

        public string CUS_ContactNumber
        {
            get { return _CUS_ContactNumber; }
            set { _CUS_ContactNumber = value; }
        }

        public string CUS_ERMSStatus
        {
            get { return _CUS_ERMSStatus; }
            set { _CUS_ERMSStatus = value; }
        }

        public string CUS_DateCreated
        {
            get { return _CUS_DateCreated; }
            set { _CUS_DateCreated = value; }
        }

        public string CUS_DatePaid
        {
            get { return _CUS_DatePaid; }
            set { _CUS_DatePaid = value; }
        }

        public bool CUS_IsOldBalance
        {
            get { return _CUS_IsOldBalance; }
            set { _CUS_IsOldBalance = value; }
        }

        public string CUS_EmailAddress
        {
            get { return _CUS_EmailAddress; }
            set { _CUS_EmailAddress = value; }
        }
     
        public fileAttachmentObject[] AttachedFiles
        {
            get { return _AttachedFiles; }
            set { _AttachedFiles = value; }
        }

        public bool CUS_FromMobile
        {
            get { return _CUS_FromMobile; }
            set { _CUS_FromMobile = value; }
        }

        public bool CUS_IsEncrypted
        {
            get { return _CUS_IsEncrypted; }
            set { _CUS_IsEncrypted = value; }
        }

        public bool noError
        {
            get { return _noError; }
            set { _noError = value; }
        }

        public string errorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

    }


        public class cbrsApprovalDetailObject
        {
            private int _APR_PKID;
            private int _CUS_PKID;
            private int _OFC_PKID;
            private int _APS_PKID;
            private int _APR_Level;
            private string _APR_ApprovedAmount;
            private string _MaximumAmount;
            private string _APR_DateApproved;
            private string _APR_Comments;
            private string _CUS_AgentAccount;
            private string _TCK_PKID;

            public int APR_PKID
            {
                get { return _APR_PKID; }
                set { _APR_PKID = value; }
            }

            public int CUS_PKID
            {
                get { return _CUS_PKID; }
                set { _CUS_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public int APS_PKID
            {
                get { return _APS_PKID; }
                set { _APS_PKID = value; }
            }

            public int APR_Level
            {
                get { return _APR_Level; }
                set { _APR_Level = value; }
            }

            public string APR_ApprovedAmount
            {
                get { return _APR_ApprovedAmount; }
                set { _APR_ApprovedAmount = value; }
            }

            public string MaximumAmount
            {
                get { return _MaximumAmount; }
                set { _MaximumAmount = value; }
            }

            public string APR_DateApproved
            {
                get { return _APR_DateApproved; }
                set { _APR_DateApproved = value; }
            }

            public string APR_Comments
            {
                get { return _APR_Comments; }
                set { _APR_Comments = value; }
            }

            public string CUS_AgentAccount
            {
                get { return _CUS_AgentAccount; }
                set { _CUS_AgentAccount = value; }
            }

            public string TCK_PKID
            {
                get { return _TCK_PKID; }
                set { _TCK_PKID = value; }
            }
        }



        public class cbrsErrorLogObject
        {
            private int _ERL_PKID;
            private int _USR_PKID;
            private string _ERL_PageName;
            private string _ERL_MethodName;
            private string _ERL_ErrorDecsription;
            private string _ERL_DateTime;
            private string _ERL_UserIdentifier;
            private string _ERL_RecordIdentifier;
            private string _ERL_EncryptedCode;

            public int ERL_PKID
            {
                get { return _ERL_PKID; }
                set { _ERL_PKID = value; }
            }

            public int USR_PKID
            {
                get { return _USR_PKID; }
                set { _USR_PKID = value; }
            }

            public string ERL_PageName
            {
                get { return _ERL_PageName; }
                set { _ERL_PageName = value; }
            }

            public string ERL_MethodName
            {
                get { return _ERL_MethodName; }
                set { _ERL_MethodName = value; }
            }

            public string ERL_ErrorDecsription
            {
                get { return _ERL_ErrorDecsription; }
                set { _ERL_ErrorDecsription = value; }
            }

            public string ERL_DateTime
            {
                get { return _ERL_DateTime; }
                set { _ERL_DateTime = value; }
            }

            public string ERL_UserIdentifier
            {
                get { return _ERL_UserIdentifier; }
                set { _ERL_UserIdentifier = value; }
            }

            public string ERL_RecordIdentifier
            {
                get { return _ERL_RecordIdentifier; }
                set { _ERL_RecordIdentifier = value; }
            }

            public string ERL_EncryptedCode
            {
                get { return _ERL_EncryptedCode; }
                set { _ERL_EncryptedCode = value; }
            }

        }



        public class cbrsFlatTextObject
        {
            private string _outputFileName;
            private byte[] _refundLines;

            public string outputFileName
            {
                get { return _outputFileName; }
                set { _outputFileName = value; }
            }

            public byte[] refundLines
            {
                get { return _refundLines; }
                set { _refundLines = value; }
            }

        }


        public class cbrsFlatFileObject
        {
            private int _TXF_PKID;
            private int _OFC_PKID;
            private int _TXF_RecordCount;
            private string _TXF_FilePath;
            private string _TXF_TotalApprovedAmount;
            private string _TXF_DateCreated;
            private int _TXF_Status;
            private string _TXF_Comments;

            public int TXF_PKID
            {
                get { return _TXF_PKID; }
                set { _TXF_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public int TXF_RecordCount
            {
                get { return _TXF_RecordCount; }
                set { _TXF_RecordCount = value; }
            }

            public string TXF_FilePath
            {
                get { return _TXF_FilePath; }
                set { _TXF_FilePath = value; }
            }

            public string TXF_TotalApprovedAmount
            {
                get { return _TXF_TotalApprovedAmount; }
                set { _TXF_TotalApprovedAmount = value; }
            }

            public string TXF_DateCreated
            {
                get { return _TXF_DateCreated; }
                set { _TXF_DateCreated = value; }
            }

            public int TXF_Status
            {
                get { return _TXF_Status; }
                set { _TXF_Status = value; }
            }

            public string TXF_Comments
            {
                get { return _TXF_Comments; }
                set { _TXF_Comments = value; }
            }

        }

        public class userDisplayProfileObject
        {
            private int _USR_PKID;
            private int _USC_PKID;
            private int _OFL_PKID;
            private int _ODP_PKID;
            private string _USR_UserLogin;
            private string _USR_PassKey;
            private string _USR_FirstName;
            private string _USR_LastName;
            private string _USR_MobileNumber;
            private string _USR_EmailAccount;
            private string _USR_DateCreated;
            private string _USR_ActivationDate;
            private string _USR_FaceImage;
            private int _STS_PKID;
            private int _USG_PKID;
            private int _OGA_PKID;
            private string _USR_Comments;
            private bool _OFC_PaginateResults;
            private string _USR_Salutation;
            private string _UserSourceName;
            private bool _noError;
            private string _errorMessage;

            public int USR_PKID
            {
                get { return _USR_PKID; }
                set { _USR_PKID = value; }
            }

            public int USC_PKID
            {
                get { return _USC_PKID; }
                set { _USC_PKID = value; }
            }

            public int OFL_PKID
            {
                get { return _OFL_PKID; }
                set { _OFL_PKID = value; }
            }

            public int ODP_PKID
            {
                get { return _ODP_PKID; }
                set { _ODP_PKID = value; }
            }

            public string USR_UserLogin
            {
                get { return _USR_UserLogin; }
                set { _USR_UserLogin = value; }
            }

            public string USR_PassKey
            {
                get { return _USR_PassKey; }
                set { _USR_PassKey = value; }
            }

            public string USR_FirstName
            {
                get { return _USR_FirstName; }
                set { _USR_FirstName = value; }
            }

            public string USR_LastName
            {
                get { return _USR_LastName; }
                set { _USR_LastName = value; }
            }

            public string USR_MobileNumber
            {
                get { return _USR_MobileNumber; }
                set { _USR_MobileNumber = value; }
            }

            public string USR_EmailAccount
            {
                get { return _USR_EmailAccount; }
                set { _USR_EmailAccount = value; }
            }

            public string USR_DateCreated
            {
                get { return _USR_DateCreated; }
                set { _USR_DateCreated = value; }
            }

            public string USR_ActivationDate
            {
                get { return _USR_ActivationDate; }
                set { _USR_ActivationDate = value; }
            }

            public string USR_FaceImage
            {
                get { return _USR_FaceImage; }
                set { _USR_FaceImage = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public int USG_PKID
            {
                get { return _USG_PKID; }
                set { _USG_PKID = value; }
            }

            public int OGA_PKID
            {
                get { return _OGA_PKID; }
                set { _OGA_PKID = value; }
            }

            public string USR_Comments
            {
                get { return _USR_Comments; }
                set { _USR_Comments = value; }
            }

            public bool OFC_PaginateResults
            {
                get { return _OFC_PaginateResults; }
                set { _OFC_PaginateResults = value; }
            }

            public string USR_Salutation
            {
                get { return _USR_Salutation; }
                set { _USR_Salutation = value; }
            }

            public string UserSourceName
            {
                get { return _UserSourceName; }
                set { _UserSourceName = value; }
            }
            
            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class organisationObject
        {
            private int _OGA_PKID;
            private string _OGA_OrganisationName;
            private string _OGA_AddressLine;
            private string _OGA_Surburb;
            private string _OGA_City;
            private string _OGA_Country;
            private string _OGA_Code;
            private bool _noError;
            private string _errorMessage;

            public int OGA_PKID
            {
                get { return _OGA_PKID; }
                set { _OGA_PKID = value; }
            }

            public string OGA_OrganisationName
            {
                get { return _OGA_OrganisationName; }
                set { _OGA_OrganisationName = value; }
            }

            public string OGA_AddressLine
            {
                get { return _OGA_AddressLine; }
                set { _OGA_AddressLine = value; }
            }

            public string OGA_Surburb
            {
                get { return _OGA_Surburb; }
                set { _OGA_Surburb = value; }
            }

            public string OGA_City
            {
                get { return _OGA_City; }
                set { _OGA_City = value; }
            }

            public string OGA_Country
            {
                get { return _OGA_Country; }
                set { _OGA_Country = value; }
            }

            public string OGA_Code
            {
                get { return _OGA_Code; }
                set { _OGA_Code = value; }
            }

            //public string OGA_ContactNumber
            //{
            //    get { return _OGA_ContactNumber; }
            //    set { _OGA_ContactNumber = value; }
            //}

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

    public class RecordAttachments
    {
        public string FileDescription { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }
        public string FileMimeType { get; set; }
        public int FileSize { get; set; }
        public byte[] FileField { get; set; }
        public string FileURL { get; set; }
        public string OriginalFileName { get; set; }
        public string DerivedFileName { get; set; }
        public string recordPKID { get; set; }
    }

    public class fileAttachmentObject
    {

        private int _DCM_PKID;
        private int _DCT_PKID;
        private int _DCS_PKID;
        private int _DCL_PKID;
        private string _DCM_DocumentPath;
        private string _DCM_OriginalName;
        private string _DCM_DerivedName;
        private string _DCM_Extention;
        private int _STS_PKID;
        private byte[] _DCM_FileField;
        private int _AttachmentSize;
        private string _MimeType;

        public int DCM_PKID
        {
            get { return _DCM_PKID; }
            set { _DCM_PKID = value; }
        }

        public int DCT_PKID
        {
            get { return _DCT_PKID; }
            set { _DCT_PKID = value; }
        }

        public int DCS_PKID
        {
            get { return _DCS_PKID; }
            set { _DCS_PKID = value; }
        }

        public int DCL_PKID
        {
            get { return _DCL_PKID; }
            set { _DCL_PKID = value; }
        }

        public string DCM_DocumentPath
        {
            get { return _DCM_DocumentPath; }
            set { _DCM_DocumentPath = value; }
        }

        public string DCM_OriginalName
        {
            get { return _DCM_OriginalName; }
            set { _DCM_OriginalName = value; }
        }

        public string DCM_DerivedName
        {
            get { return _DCM_DerivedName; }
            set { _DCM_DerivedName = value; }
        }

        public string DCM_Extention
        {
            get { return _DCM_Extention; }
            set { _DCM_Extention = value; }
        }

        public int STS_PKID
        {
            get { return _STS_PKID; }
            set { _STS_PKID = value; }
        }

        public byte[] DCM_FileField
        {
            get { return _DCM_FileField; }
            set { _DCM_FileField = value; }
        }

        public int AttachmentSize
        {
            get { return _AttachmentSize; }
            set { _AttachmentSize = value; }
        }

        public string MimeType
        {
            get { return _MimeType; }
            set { _MimeType = value; }
        }

    }

        public class ticketObject
        {
            private int _TCK_PKID;
            private int _CAT_PKID;
            private int _USR_PKID;
            private int _OFC_PKID;
            private int _UST_PKID;
            private int _TPT_PKID;
            private int _TCT_PKID;
            private string _TCK_TicketNumber;
            private string _TCK_Subject;
            private string _TCK_Message;
            private string _TCK_AlternateEmail;
            private string _TCK_DateCreated;
            private string _TCK_DateClosed;
            private string _TCK_DateDue;
            private bool _TCK_Viewable;
            private int _STS_PKID;
            private int _PTY_PKID;
            private string _TCK_Reference;
            private bool _TCK_HasFile;
            private bool _TCK_FromMobile;
            private bool _CAT_RequireAttachment;

            private int _PRV_PKID;
            private bool _TCK_IsLog;
            private string _STR_PKID;
            private string _TCK_CaseIdentifier;
            private string _CAT_ForcedFieldID;
            private int _TCK_Creator;

            private fileAttachmentObject _AttachedFile;
            private fileAttachmentObject[] _AttachedFiles;

            public int TCK_PKID
            {
                get { return _TCK_PKID; }
                set { _TCK_PKID = value; }
            }

            public int CAT_PKID
            {
                get { return _CAT_PKID; }
                set { _CAT_PKID = value; }
            }

            public int USR_PKID
            {
                get { return _USR_PKID; }
                set { _USR_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public int UST_PKID
            {
                get { return _UST_PKID; }
                set { _UST_PKID = value; }
            }

            public int TPT_PKID
            {
                get { return _TPT_PKID; }
                set { _TPT_PKID = value; }
            }

            public int TCT_PKID
            {
                get { return _TCT_PKID; }
                set { _TCT_PKID = value; }
            }

            public string TCK_TicketNumber
            {
                get { return _TCK_TicketNumber; }
                set { _TCK_TicketNumber = value; }
            }

            public string TCK_Subject
            {
                get { return _TCK_Subject; }
                set { _TCK_Subject = value; }
            }

            public string TCK_Message
            {
                get { return _TCK_Message; }
                set { _TCK_Message = value; }
            }

            public string TCK_AlternateEmail
            {
                get { return _TCK_AlternateEmail; }
                set { _TCK_AlternateEmail = value; }
            }

            public string TCK_DateCreated
            {
                get { return _TCK_DateCreated; }
                set { _TCK_DateCreated = value; }
            }

            public string TCK_DateClosed
            {
                get { return _TCK_DateClosed; }
                set { _TCK_DateClosed = value; }
            }

            public string TCK_DateDue
            {
                get { return _TCK_DateDue; }
                set { _TCK_DateDue = value; }
            }

            public bool TCK_Viewable
            {
                get { return _TCK_Viewable; }
                set { _TCK_Viewable = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public int PTY_PKID
            {
                get { return _PTY_PKID; }
                set { _PTY_PKID = value; }
            }

            public string TCK_Reference
            {
                get { return _TCK_Reference; }
                set { _TCK_Reference = value; }
            }

            public bool TCK_HasFile
            {
                get { return _TCK_HasFile; }
                set { _TCK_HasFile = value; }
            }

            public bool TCK_FromMobile
            {
                get { return _TCK_FromMobile; }
                set { _TCK_FromMobile = value; }
            }
            public bool CAT_RequireAttachment
            {
                get { return _CAT_RequireAttachment; }
                set { _CAT_RequireAttachment = value; }
            }

            public int PRV_PKID
            {
                get { return _PRV_PKID; }
                set { _PRV_PKID = value; }
            }

            public bool TCK_IsLog
            {
                get { return _TCK_IsLog; }
                set { _TCK_IsLog = value; }
            }

            public string STR_PKID
            {
                get { return _STR_PKID; }
                set { _STR_PKID = value; }
            }

            public string TCK_CaseIdentifier
            {
                get { return _TCK_CaseIdentifier; }
                set { _TCK_CaseIdentifier = value; }
            }

            public string CAT_ForcedFieldID
            {
                get { return _CAT_ForcedFieldID; }
                set { _CAT_ForcedFieldID = value; }
            }

            public int  TCK_Creator
            {
                get { return _TCK_Creator; }
                set { _TCK_Creator = value; }
            }

            public fileAttachmentObject AttachedFile
            {
                get { return _AttachedFile; }
                set { _AttachedFile = value; }
            }

            public fileAttachmentObject[] AttachedFiles
            {
                get { return _AttachedFiles; }
                set { _AttachedFiles = value; }
            }

        }

        public class ERMSCustomerObject
        {
            private bool _selectedPKID;
            private bool _noError;
            private string _errorMessage;

            public bool selectedPKID
            {
                get { return _selectedPKID; }
                set { _selectedPKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        #region Display Objects

        public class ticketDetailObject
        {
            private int _TCK_PKID;
            private int _CAT_PKID;
            private int _USR_PKID;
            private int _OFC_PKID;
            private int _TPT_PKID;
            private string _TCK_TicketNumber;
            private string _TCK_Subject;
            private string _TCK_Message;
            private string _TCK_AlternateEmail;
            private string _TCK_DateCreated;
            private string _TCK_DueDate;
            private bool _TCK_Viewable;
            private int _STS_PKID;
            private int _PTY_PKID;
            private string _TCK_Reference;
            private bool _TCK_HasFile;
            private bool _CAT_RequireAttachment;
            private string _ProblemTypeName;
            private string _TimeToResolve;
            private string _TCT_PKID;
            private string _TCK_CaseIdentifier;
            private fileAttachmentObject _AttachedFile;
            private userDisplayProfileObject _ClientObject;
            private ticketDisplayCategoryObject _CategoryObject;
            private ticketResponseDisplayObject[] _ResponseObject;

            public int TCK_PKID
            {
                get { return _TCK_PKID; }
                set { _TCK_PKID = value; }
            }

            public int CAT_PKID
            {
                get { return _CAT_PKID; }
                set { _CAT_PKID = value; }
            }

            public int USR_PKID
            {
                get { return _USR_PKID; }
                set { _USR_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public int TPT_PKID
            {
                get { return _TPT_PKID; }
                set { _TPT_PKID = value; }
            }

            public string TCK_TicketNumber
            {
                get { return _TCK_TicketNumber; }
                set { _TCK_TicketNumber = value; }
            }

            public string TCK_Subject
            {
                get { return _TCK_Subject; }
                set { _TCK_Subject = value; }
            }

            public string TCK_Message
            {
                get { return _TCK_Message; }
                set { _TCK_Message = value; }
            }

            public string TCK_AlternateEmail
            {
                get { return _TCK_AlternateEmail; }
                set { _TCK_AlternateEmail = value; }
            }

            public string TCK_DateCreated
            {
                get { return _TCK_DateCreated; }
                set { _TCK_DateCreated = value; }
            }

            public string TCK_DueDate
            {
                get { return _TCK_DueDate; }
                set { _TCK_DueDate = value; }
            }

            public bool TCK_Viewable
            {
                get { return _TCK_Viewable; }
                set { _TCK_Viewable = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public int PTY_PKID
            {
                get { return _PTY_PKID; }
                set { _PTY_PKID = value; }
            }

            public string TCK_Reference
            {
                get { return _TCK_Reference; }
                set { _TCK_Reference = value; }
            }

            public bool TCK_HasFile
            {
                get { return _TCK_HasFile; }
                set { _TCK_HasFile = value; }
            }

            public bool CAT_RequireAttachment
            {
                get { return _CAT_RequireAttachment; }
                set { _CAT_RequireAttachment = value; }
            }

            public string ProblemTypeName
            {
                get { return _ProblemTypeName; }
                set { _ProblemTypeName = value; }
            }

            public string TimeToResolve
            {
                get { return _TimeToResolve; }
                set { _TimeToResolve = value; }
            }

            public string TCT_PKID
            {
                get { return _TCT_PKID; }
                set { _TCT_PKID = value; }
            }

            public string TCK_CaseIdentifier
            {
                get { return _TCK_CaseIdentifier; }
                set { _TCK_CaseIdentifier = value; }
            }

            public fileAttachmentObject AttachedFile
            {
                get { return _AttachedFile; }
                set { _AttachedFile = value; }
            }

            public userDisplayProfileObject ClientObject
            {
                get { return _ClientObject; }
                set { _ClientObject = value; }
            }

            public ticketDisplayCategoryObject CategoryObject
            {
                get { return _CategoryObject; }
                set { _CategoryObject = value; }
            }

            public ticketResponseDisplayObject[] ResponseObject
            {
                get { return _ResponseObject; }
                set { _ResponseObject = value; }
            }
        }

        public class ticketDisplayCategoryObject
        {
            private string _DepartmentName;
            private string _CategoryName;
            private string _SubCategoryName;
            private bool _noError;
            private string _errorMessage;

            public string DepartmentName
            {
                get { return _DepartmentName; }
                set { _DepartmentName = value; }
            }

            public string CategoryName
            {
                get { return _CategoryName; }
                set { _CategoryName = value; }
            }

            public string SubCategoryName
            {
                get { return _SubCategoryName; }
                set { _SubCategoryName = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class ticketResponseDisplayObject
        {

            private int _TKR_PKID;
            private int _TCK_PKID;
            private int _OFC_PKID;
            private int _UST_PKID;
            private string _TKR_ResponseMessage;
            private string _TKR_ResponseDate;
            private bool _TKR_VisibleToClient;
            private int _STS_PKID;
            private bool _TKR_HasFile;
            private bool _TKR_FromMobile;
            private string _StatusName;
            private string _UserNames;

            private fileAttachmentObject _AttachedFile;

            public int TKR_PKID
            {
                get { return _TKR_PKID; }
                set { _TKR_PKID = value; }
            }

            public int TCK_PKID
            {
                get { return _TCK_PKID; }
                set { _TCK_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public int UST_PKID
            {
                get { return _UST_PKID; }
                set { _UST_PKID = value; }
            }

            public string TKR_ResponseMessage
            {
                get { return _TKR_ResponseMessage; }
                set { _TKR_ResponseMessage = value; }
            }

            public string TKR_ResponseDate
            {
                get { return _TKR_ResponseDate; }
                set { _TKR_ResponseDate = value; }
            }

            public bool TKR_VisibleToClient
            {
                get { return _TKR_VisibleToClient; }
                set { _TKR_VisibleToClient = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public bool TKR_HasFile
            {
                get { return _TKR_HasFile; }
                set { _TKR_HasFile = value; }
            }

            public bool TKR_FromMobile
            {
                get { return _TKR_FromMobile; }
                set { _TKR_FromMobile = value; }
            }

            public fileAttachmentObject AttachedFile
            {
                get { return _AttachedFile; }
                set { _AttachedFile = value; }
            }

            public string StatusName
            {
                get { return _StatusName; }
                set { _StatusName = value; }
            }

            public string UserNames
            {
                get { return _UserNames; }
                set { _UserNames = value; }
            }

        }

        public class ermsStatementObject
        {

//            select
//trans_id, agent_code, trans_date, trans_type_id, trak_no, ent_no, form_code, trans_reference, service_rend_code, trans_desc, original_amount,
//open_amount, on_hold, trans_status_id, effective_date, closing_balance
//from agent_trans where agent_code = 'AMOSNA

            private string _trans_id;
            private string _trans_date;
            private string _trans_type_id;
            private string _trak_no;
            private string _ent_no;
            private string _form_code;
            private string _trans_reference;
            private string _service_rend_code;
            private string _trans_desc;
            private string _original_amount;
            private string _open_amount;
            private string _on_hold;
            private string _trans_status_id;
            private string _effective_date;
            private string _closing_balance;
            private bool _noError;
            private string _errorMessage;

            public string trans_id
            {
                get { return _trans_id; }
                set { _trans_id = value; }
            }

            public string trans_date
            {
                get { return _trans_date; }
                set { _trans_date = value; }
            }

            public string trans_type_id
            {
                get { return _trans_type_id; }
                set { _trans_type_id = value; }
            }

            public string trak_no
            {
                get { return _trak_no; }
                set { _trak_no = value; }
            }

            public string ent_no
            {
                get { return _ent_no; }
                set { _ent_no = value; }
            }

            public string form_code
            {
                get { return _form_code; }
                set { _form_code = value; }
            }

            public string trans_reference
            {
                get { return _trans_reference; }
                set { _trans_reference = value; }
            }

            public string service_rend_code
            {
                get { return _service_rend_code; }
                set { _service_rend_code = value; }
            }

            public string trans_desc
            {
                get { return _trans_desc; }
                set { _trans_desc = value; }
            }

            public string original_amount
            {
                get { return _original_amount; }
                set { _original_amount = value; }
            }

            public string open_amount
            {
                get { return _open_amount; }
                set { _open_amount = value; }
            }

            public string on_hold
            {
                get { return _on_hold; }
                set { _on_hold = value; }
            }

            public string trans_status_id
            {
                get { return _trans_status_id; }
                set { _trans_status_id = value; }
            }

            public string effective_date
            {
                get { return _effective_date; }
                set { _effective_date = value; }
            }

            public string closing_balance
            {
                get { return _closing_balance; }
                set { _closing_balance = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }
        }

        public class ermsStatementResponseObject
        {
            private string _agentAccount;
            private string _customerNames;
            private string _ermsStatus;
            private string _ermsBalance;
            private bool _noError;
            private string _errorMessage;
            private ermsStatementObject[] _responseDataSet;

            public string agentAccount
            {
                get { return _agentAccount; }
                set { _agentAccount = value; }
            }

            public string customerNames
            {
                get { return _customerNames; }
                set { _customerNames = value; }
            }

            public string ermsStatus
            {
                get { return _ermsStatus; }
                set { _ermsStatus = value; }
            }

            public string ermsBalance
            {
                get { return _ermsBalance; }
                set { _ermsBalance = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

            public ermsStatementObject[] responseDataSet
            {
                get { return _responseDataSet; }
                set { _responseDataSet = value; }
            }
        }
      
        #endregion

        public class ticketResponseObject
        {

            private int _TKR_PKID;
            private int _TCK_PKID;
            private int _OFC_PKID;
            private int _UST_PKID;
            private string _TKR_ResponseMessage;
            private string _TKR_ResponseDate;
            private bool _TKR_VisibleToClient;
            private int _STS_PKID;
            private bool _TKR_HasFile;
            private bool _TKR_FromMobile;
            private int _RST_PKID;
            private int _STR_PKID;
            private int _CAT_PKID;
            private fileAttachmentObject _AttachedFile;

            public int TKR_PKID
            {
                get { return _TKR_PKID; }
                set { _TKR_PKID = value; }
            }

            public int TCK_PKID
            {
                get { return _TCK_PKID; }
                set { _TCK_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public int UST_PKID
            {
                get { return _UST_PKID; }
                set { _UST_PKID = value; }
            }

            public string TKR_ResponseMessage
            {
                get { return _TKR_ResponseMessage; }
                set { _TKR_ResponseMessage = value; }
            }

            public string TKR_ResponseDate
            {
                get { return _TKR_ResponseDate; }
                set { _TKR_ResponseDate = value; }
            }

            public bool TKR_VisibleToClient
            {
                get { return _TKR_VisibleToClient; }
                set { _TKR_VisibleToClient = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public bool TKR_HasFile
            {
                get { return _TKR_HasFile; }
                set { _TKR_HasFile = value; }
            }

            public bool TKR_FromMobile
            {
                get { return _TKR_FromMobile; }
                set { _TKR_FromMobile = value; }
            }

            public int RST_PKID
            {
                get { return _RST_PKID; }
                set { _RST_PKID = value; }
            }

            public int STR_PKID
            {
                get { return _STR_PKID; }
                set { _STR_PKID = value; }
            }

        public int CAT_PKID
        {
            get { return _CAT_PKID; }
            set { _CAT_PKID = value; }
        }

        public fileAttachmentObject AttachedFile
            {
                get { return _AttachedFile; }
                set { _AttachedFile = value; }
            }

        }

        public class userCategoryObject
        {
            private int _CAT_PKID;
            private int _CTL_PKID;
            private int _OFC_PKID;
           
            public int CAT_PKID
            {
                get { return _CAT_PKID; }
                set { _CAT_PKID = value; }
            }

            public int CTL_PKID
            {
                get { return _CTL_PKID; }
                set { _CTL_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }
        }

        public class categoryObject
        {

            private int _CAT_PKID;
            private int _TPT_PKID;
            private int _CTL_PKID;
            private int _SVL_PKID;
            private int _CAT_Order;
            private int _CAT_MasterID;
            private string _CAT_CategoryName;
            private int _STS_PKID;
            private bool _CAT_RequireAttachment;
            private bool _CAT_Visible;
            private bool _CAT_NotifyEmail;
            private string _CAT_CategoryDescription;
            private string _CAT_ShortName;
            private bool _noError;
            private string _errorMessage;

            public int CAT_PKID
            {
                get { return _CAT_PKID; }
                set { _CAT_PKID = value; }
            }

            public int TPT_PKID
            {
                get { return _TPT_PKID; }
                set { _TPT_PKID = value; }
            }

            public int CTL_PKID
            {
                get { return _CTL_PKID; }
                set { _CTL_PKID = value; }
            }

            public int SVL_PKID
            {
                get { return _SVL_PKID; }
                set { _SVL_PKID = value; }
            }

            public int CAT_Order
            {
                get { return _CAT_Order; }
                set { _CAT_Order = value; }
            }

            public int CAT_MasterID
            {
                get { return _CAT_MasterID; }
                set { _CAT_MasterID = value; }
            }

            public string CAT_CategoryName
            {
                get { return _CAT_CategoryName; }
                set { _CAT_CategoryName = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public bool CAT_RequireAttachment
            {
                get { return _CAT_RequireAttachment; }
                set { _CAT_RequireAttachment = value; }
            }

            public bool CAT_Visible
            {
                get { return _CAT_Visible; }
                set { _CAT_Visible = value; }
            }

            public bool CAT_NotifyEmail
            {
                get { return _CAT_NotifyEmail; }
                set { _CAT_NotifyEmail = value; }
            }

            public string CAT_CategoryDescription
            {
                get { return _CAT_CategoryDescription; }
                set { _CAT_CategoryDescription = value; }
            }

            public string CAT_ShortName
            {
                get { return _CAT_ShortName; }
                set { _CAT_ShortName = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class slaObject
        {
            //select SVL_PKID, SVL_Hours, SVL_Description from TB_SVL_ServiceLevel
            private int _SVL_PKID;
            private int _SVL_Hours;
            private string _SVL_Description;
            private bool _noError;
            private string _errorMessage;

            public int SVL_PKID
            {
                get { return _SVL_PKID; }
                set { _SVL_PKID = value; }
            }

            public int SVL_Hours
            {
                get { return _SVL_Hours; }
                set { _SVL_Hours = value; }
            }

            public string SVL_Description
            {
                get { return _SVL_Description; }
                set { _SVL_Description = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class userLogObject
        {
            //USL_PKID, USL_Description, ULT_PKID, USL_DateCreated, OFC_PKID
            private int _USL_PKID;
            private string _USL_Description;
            private int _ULT_PKID;
            private string _USL_DateCreated;
            private int _OFC_PKID;

            private bool _noError;
            private string _errorMessage;

            public int USL_PKID
            {
                get { return _USL_PKID; }
                set { _USL_PKID = value; }
            }

            public string USL_Description
            {
                get { return _USL_Description; }
                set { _USL_Description = value; }
            }

            public int ULT_PKID
            {
                get { return _ULT_PKID; }
                set { _ULT_PKID = value; }
            }

            public string USL_DateCreated
            {
                get { return _USL_DateCreated; }
                set { _USL_DateCreated = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }
         

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class escalationListObject
        {
            private int _PKID;
            private int _CAT_PKID;
            private int _ESL_PKID;
            private int _OFC_PKID;
            private bool _noError;
            private string _errorMessage;

            public int PKID
            {
                get { return _PKID; }
                set { _PKID = value; }
            }

            public int CAT_PKID
            {
                get { return _CAT_PKID; }
                set { _CAT_PKID = value; }
            }

            public int ESL_PKID
            {
                get { return _ESL_PKID; }
                set { _ESL_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class templateObject
        {
            private int _STR_PKID;
            private int _CAT_PKID;
            private string _STR_ResponseTitle;
            private string _STR_ResponseMessage;
            private int _STS_PKID;
            private int _OFC_PKID;
            private int _STR_DedicatedCAT_PKID;
            private bool _noError;
            private string _errorMessage;

            private fileAttachmentObject _AttachedFile;

            public int STR_PKID
            {
                get { return _STR_PKID; }
                set { _STR_PKID = value; }
            }

            public int CAT_PKID
            {
                get { return _CAT_PKID; }
                set { _CAT_PKID = value; }
            }

            public string STR_ResponseTitle
            {
                get { return _STR_ResponseTitle; }
                set { _STR_ResponseTitle = value; }
            }

            public string STR_ResponseMessage
            {
                get { return _STR_ResponseMessage; }
                set { _STR_ResponseMessage = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }

            }

            public int STR_DedicatedCAT_PKID
            {
                get { return _STR_DedicatedCAT_PKID; }
                set { _STR_DedicatedCAT_PKID = value; }
            }

            public fileAttachmentObject AttachedFile
            {
                get { return _AttachedFile; }
                set { _AttachedFile = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class faqObject
        {
            private int _FAQ_PKID;
            private int _OFC_PKID;
            private int _CAT_PKID;
            private string _FAQ_QuestionNumber;
            private string _FAQ_EntryText;
            private int _FAQ_EntryType;
            private int _STS_PKID;
            private string _FAQ_Comment;
            private bool _noError;
            private string _errorMessage;
            //FAQ_PKID, OFC_PKID, CAT_PKID, FAQ_QuestionNumber, FAQ_EntryText, FAQ_EntryType, STS_PKID, FAQ_Comment

            public int FAQ_PKID
            {
                get { return _FAQ_PKID; }
                set { _FAQ_PKID = value; }
            }

            public int OFC_PKID
            {
                get { return _OFC_PKID; }
                set { _OFC_PKID = value; }
            }

            public int CAT_PKID
            {
                get { return _CAT_PKID; }
                set { _CAT_PKID = value; }
            }
            public string FAQ_QuestionNumber
            {
                get { return _FAQ_QuestionNumber; }
                set { _FAQ_QuestionNumber = value; }
            }

            public string FAQ_EntryText
            {
                get { return _FAQ_EntryText; }
                set { _FAQ_EntryText = value; }
            }

            public int FAQ_EntryType
            {
                get { return _FAQ_EntryType; }
                set { _FAQ_EntryType = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public string FAQ_Comment
            {
                get { return _FAQ_Comment; }
                set { _FAQ_Comment = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class mimeTypeObject
        {
            private int _AMT_PKID;
            private string _AMT_Name;
            private string _AMT_Extention;
            private string _AMT_Description;
            private int _STS_PKID;
            private string _AMT_Creator;
            private bool _noError;
            private string _errorMessage;

            public int AMT_PKID
            {
                get { return _AMT_PKID; }
                set { _AMT_PKID = value; }
            }

            public string AMT_Name
            {
                get { return _AMT_Name; }
                set { _AMT_Name = value; }
            }

            public string AMT_Extention
            {
                get { return _AMT_Extention; }
                set { _AMT_Extention = value; }
            }

            public string AMT_Description
            {
                get { return _AMT_Description; }
                set { _AMT_Description = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

        public string AMT_Creator
        {
            get { return _AMT_Creator; }
            set { _AMT_Creator = value; }
        }

        public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class userOfficeObject
        {
            private int _USG_PKID;
            private string _USG_UserGroupName;
            private string _USG_Comments;
            private string _USG_Description;
            private int _STS_PKID;
            private bool _noError;
            private string _errorMessage;

            public int USG_PKID
            {
                get { return _USG_PKID; }
                set { _USG_PKID = value; }
            }

            public string USG_UserGroupName
            {
                get { return _USG_UserGroupName; }
                set { _USG_UserGroupName = value; }
            }

            public string USG_Comments
            {
                get { return _USG_Comments; }
                set { _USG_Comments = value; }
            }

            public string USG_Description
            {
                get { return _USG_Description; }
                set { _USG_Description = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class problemTypeObject
        {
            private int _PTY_PKID;
            private string _PTY_ProblemTypeName;
            private string _PTY_Comments;
            private string _PTY_Description;
            private int _STS_PKID;
            private bool _noError;
            private string _errorMessage;

            public int PTY_PKID
            {
                get { return _PTY_PKID; }
                set { _PTY_PKID = value; }
            }

            public string PTY_ProblemTypeName
            {
                get { return _PTY_ProblemTypeName; }
                set { _PTY_ProblemTypeName = value; }
            }

            public string PTY_Comments
            {
                get { return _PTY_Comments; }
                set { _PTY_Comments = value; }
            }

            public string PTY_Description
            {
                get { return _PTY_Description; }
                set { _PTY_Description = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class resolvingTimeObject
        {
            private int _TTR_PKID;
            private string _TTR_TimeToResolve;
            private string _TTR_Comments;
            private string _TTR_Description;
            private int _STS_PKID;
            private bool _noError;
            private string _errorMessage;

            public int TTR_PKID
            {
                get { return _TTR_PKID; }
                set { _TTR_PKID = value; }
            }

            public string TTR_TimeToResolve
            {
                get { return _TTR_TimeToResolve; }
                set { _TTR_TimeToResolve = value; }
            }

            public string TTR_Comments
            {
                get { return _TTR_Comments; }
                set { _TTR_Comments = value; }
            }

            public string TTR_Description
            {
                get { return _TTR_Description; }
                set { _TTR_Description = value; }
            }

            public int STS_PKID
            {
                get { return _STS_PKID; }
                set { _STS_PKID = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class ticketValidationObject
        {
            private string _errorNumber;
            private bool _noError;
            private string _errorMessage;

            public string errorNumber
            {
                get { return _errorNumber; }
                set { _errorNumber = value; }
            }

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class SMTPMailResponseObject
        {
            private bool _noError;
            private string _errorMessage;

            public bool noError
            {
                get { return _noError; }
                set { _noError = value; }
            }

            public string errorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }

        }

        public class GenericImageObject
        {
            //public string imageMasterPKID = string.Empty;
            public System.Drawing.Image imageData = null;
            public string extenstion = string.Empty;
            public string ImageType = string.Empty;
            public int tmImageHeight = 0;
            public int tmImageWidth = 0;
        }



        public class SelectStringResponseOfficerObject
        {
            private string _selectedUserPKID;
            private bool _noErrors;
            private string _errorMessages;

            public string selectedUserPKID
            {
                get { return _selectedUserPKID; }
                set { _selectedUserPKID = value; }
            }

            public bool noErrors
            {
                get { return _noErrors; }
                set { _noErrors = value; }
            }

            public string errorMessages
            {
                get { return _errorMessages; }
                set { _errorMessages = value; }
            }

        }



}