using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCreate.Models
{
    public class JKDetails
    {
        public string file_no { get; set; }
        public string Portfolio { get; set; }
        public string Portfolio_Purchase_Date { get; set; }
        public string OriginalBalance { get; set; }
        public double? Original_Principal { get; set; }
        public string Original_Interest { get; set; }
        public string Original_Costs { get; set; }
        public string Original_Fees { get; set; }
        public string Current_Principal { get; set; }
        public string Current_Interest { get; set; }
        public string Current_Costs { get; set; }
        public string Current_Fees { get; set; }
        public string CurrentBalance { get; set; }
        public string AccountOpenDate { get; set; }
        public string CHARGEOFF_DATE { get; set; }
        public string ChargeOff_Amount { get; set; }
        public string IssuerAccountNumber { get; set; }
        public string Primary_SSN { get; set; }
        public string IssuerLPDate { get; set; }
        public string IssuerLastPaidAmount { get; set; }
        public string ServicerLastPayDate { get; set; }
        public string ServicerLastPayAmount { get; set; }
        public string IssuerName { get; set; }
        public string Primary_Phone_Home {get;set;}
        public string Primary_ZipPostal { get; set; }
        public string Primary_City { get; set; }
        public string Primary_State { get; set; }
        public string owner_code { get; set; }
        public string Primary_FName { get; set; }
        public string Primary_Middle { get; set; }
        public string Primary_LName { get; set; }
        public string Primary_Address1 { get; set; }
        public string Primary_Address2 { get; set; }
        public string Primary_Email { get; set; }
        public string acct_status { get; set; }
        public string OWNER_STATUS { get; set; }
    }
}
