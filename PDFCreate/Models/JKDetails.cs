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
        public DateTime? Portfolio_Purchase_Date { get; set; }
        public decimal? OriginalBalance { get; set; }
        public decimal? Original_Principal { get; set; }
        public decimal? Original_Interest { get; set; }
        public decimal? Original_Costs { get; set; }
        public decimal? Original_Fees { get; set; }
        public decimal? Current_Principal { get; set; }
        public decimal? Current_Interest { get; set; }
        public decimal? Current_Costs { get; set; }
        public decimal? Current_Fees { get; set; }
        public decimal? CurrentBalance { get; set; }
        public DateTime? AccountOpenDate { get; set; }
        public DateTime? CHARGEOFF_DATE { get; set; }
        public decimal? ChargeOff_Amount { get; set; }
        public decimal? IssuerAccountNumber { get; set; }
        public string Primary_SSN { get; set; }
        public DateTime? IssuerLPDate { get; set; }
        public decimal? IssuerLastPaidAmount { get; set; }
        public DateTime? ServicerLastPayDate { get; set; }
        public decimal? ServicerLastPayAmount { get; set; }
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
