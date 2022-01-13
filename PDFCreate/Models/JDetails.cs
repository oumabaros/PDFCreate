using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCreate.Models
{
    public class JDetails
    {
        public string file_no { get; set; }
        public string data_id { get; set; }
        public string purchase_description { get; set; }
        public DateTime? purchase_date { get; set; }
        public string purchase_id { get; set; }
        public double? data_bal_total_start { get; set; }
        public double? data_bal_total { get; set; }
        public double? data_bal_principal { get; set; }
        public double? data_bal_interest { get; set; }
        public double? data_bal_other { get; set; }
        public double? data_bal_principal_start { get; set; }
        public double? data_bal_interest_start { get; set; }
        public double? data_bal_other_start { get; set; }
        public DateTime? acctopen { get; set; }
        public double? intrate { get; set; }
        public DateTime? chargeoff { get; set; }
        public double? chargeoffamt { get; set; }
        public string pri_acctno { get; set; }
        public string sec_acctno { get; set; }
        public DateTime? delinquency { get; set; }
        public DateTime? lastpayment { get; set; }
        public DateTime? lastpayment_ptc { get; set; }
        public double? lastpaymentamt { get; set; }
        public double? lastpaymentamt_ptc { get; set; }
        public string originator { get; set; }
        public string merchant { get; set; }
        public DateTime? STATUTE_DATE { get; set; }
        public string DEBT_TYPE { get; set; }
        public DateTime? legal_placement_date { get; set; }
        public double? judgment_award { get; set; }
        public string HOME_PHONE { get; set; }
        public string pri_zip { get; set; }
        public string pri_city { get; set; }
        public string pri_state { get; set; }
        public string entity_name { get; set; }
        public string pri_first { get; set; }
        public string pri_middle { get; set; }
        public string pri_last { get; set; }
        public string pri_city1 { get; set; }
        public string pri_state1 { get; set; }
        public string pri_zip1 { get; set; }
        public DateTime? pri_dob { get; set; }
        public string pri_ssn { get; set; }
        public string pri_add1 { get; set; }
        public string pri_add2 { get; set; }
        public string pri_email { get; set; }
        public string comaker_first { get; set; }
        public string comaker_middle { get; set; }
        public string comaker_last { get; set; }
        public string comaker_city { get; set; }
        public string comaker_state { get; set; }
        public string comaker_zip { get; set; }
        public DateTime? comaker_dob { get; set; }
        public string comaker_add1 { get; set; }
        public string comaker_add2 { get; set; }
        public string Grouping { get; set; }
        public string Lpurchase_id { get; set; }
        public string AF_acct_status { get; set; }
        public string AF_owner_status { get; set; }
    }
}
