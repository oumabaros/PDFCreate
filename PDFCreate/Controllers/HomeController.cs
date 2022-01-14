using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PDFCreate.Models;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCreate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment Environment;
        private readonly IConfiguration Configuration;

        public HomeController(IWebHostEnvironment _environment, IConfiguration _configuration)
        {
            Environment = _environment;
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile postedFile)
        {
            PdfModel pdfm = new PdfModel();

            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(this.Environment.WebRootPath, "FileImports");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable JDetailsDT = new DataTable();
                DataTable JKDetailsDT = new DataTable();
                DataTable PaymentsDT = new DataTable();

                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of JDetails Sheet.
                            connExcel.Open();
                            DataTable JDetailsSchema;
                            JDetailsSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string JDetailsSheet = "J Details$";// JDetailsSchema.Rows[1]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Get the name of JKDetails Sheet.
                            connExcel.Open();
                            DataTable JKDetailsSchema;
                            JKDetailsSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string JKDetailsSheet = "JK Details$";// JKDetailsSchema.Rows[2]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Get the name of Payments Sheet.
                            connExcel.Open();
                            DataTable PaymentsSchema;
                            PaymentsSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string PaymentsSheet = "Payments$"; //PaymentsSchema.Rows[3]["TABLE_NAME"].ToString();
                            connExcel.Close();


                            //Read Data from JDetailsSheet.
                            connExcel.Open();
                            cmdExcel.CommandText = string.Format("SELECT * From [" + JDetailsSheet + "]"); 
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(JDetailsDT);
                            connExcel.Close();

                            //Read Data from JKDetailsSheet.
                            connExcel.Open();
                            cmdExcel.CommandText = string.Format("SELECT * From [" + JKDetailsSheet + "]");
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(JKDetailsDT);
                            connExcel.Close();

                            //Read Data from PaymentsSheet.
                            connExcel.Open();
                            cmdExcel.CommandText = string.Format("SELECT * From [" + PaymentsSheet + "]");
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(PaymentsDT);
                            connExcel.Close();
                        }
                    }
                }
                List<JDetails> jdt = CreateJDetails(JDetailsDT);
                List<JKDetails> jkdt = CreateJKDetails(JKDetailsDT);
                List<Payments> pmnts = CreatePayments(PaymentsDT);
                
                pdfm.jdt = jdt;
                pdfm.jkdt = jkdt;
                pdfm.pmnts = pmnts;
                              
            }
           
            return CreatePdf(pdfm);
        }

        public IActionResult CreatePdf(PdfModel pdfm)
        {
            string webRootPath = Environment.WebRootPath;
            string contentRootPath = Environment.ContentRootPath;

            var header = contentRootPath + "\n" + webRootPath + "static/Header.html#pagetext=Page&oftext=Of";
            var footer = contentRootPath + "\n" + webRootPath + "static/Footer.html#pagetext=Page&oftext=Of";

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                   "--header-spacing \"0\" " +
                                   "--footer-html \"{1}\" " +
                                   "--footer-spacing \"10\" " +
                                   "--footer-font-size \"10\" " +
                                   "--header-font-size \"10\" ", header, footer);

            return new ViewAsPdf("JDetailsReport", pdfm)
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 8",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageMargins = { Top = 20, Bottom = 22 },

            };
        }

        private List<Payments> CreatePayments(DataTable dt)
        {
            List<Payments> pmnts = new List<Payments>();
            var payments_query = from p in dt.AsEnumerable()
                                  select
                                  new
                                  {
                                      file_no = p.Field<string>("file no"),
                                      payment_index = p.Field<double?>("payment index"),
                                      payment_date = p.Field<DateTime?>("payment date"),
                                      agy_Id = p.Field<string>("agy Id"),
                                      payment_amount = p.Field<decimal?>("payment amount"),
                                      pay_Description = p.Field<string>("pay_Description"),
                                  };

            foreach (var q in payments_query)
            {
                Payments pm = new Payments();
                pm.file_no = q.file_no;
                pm.payment_index = q.payment_index;
                pm.payment_date = q.payment_date;
                pm.agy_Id = q.agy_Id;
                pm.payment_amount = q.payment_amount;
                pm.pay_Description = q.pay_Description;
                pmnts.Add(pm);
            }
                return pmnts;
        }
        private List<JKDetails> CreateJKDetails(DataTable dt)
        {
            List<JKDetails> jkdts = new List<JKDetails>();

            var jkdetails_query = from p in dt.AsEnumerable()
                                 select
                                 new
                                 {
                                     file_no = p.Field<string>("file no"),
                                     Portfolio = p.Field<string>("Portfolio"),
                                     Portfolio_Purchase_Date = p.Field<string>("Portfolio_Purchase_Date"),
                                     OriginalBalance = p.Field<string>("OriginalBalance"),
                                     Original_Principal = p.Field<double?>("Original_Principal"),
                                     Original_Interest = p.Field<string>("Original_Interest"),
                                     Original_Costs = p.Field<string>("Original_Costs"),
                                     Original_Fees = p.Field<string>("Original_Fees"),
                                     Current_Principal = p.Field<string>("Current_Principal"),
                                     Current_Interest = p.Field<string>("Current_Interest"),
                                     Current_Costs = p.Field<string>("Current_Costs"),
                                     Current_Fees = p.Field<string>("Current_Fees"),
                                     CurrentBalance = p.Field<string>("CurrentBalance"),
                                     AccountOpenDate = p.Field<string>("AccountOpenDate"),
                                     CHARGEOFF_DATE = p.Field<string>("CHARGEOFF DATE"),
                                     ChargeOff_Amount = p.Field<string>("ChargeOff_Amount"),
                                     IssuerAccountNumber = p.Field<string>("IssuerAccountNumber"),
                                     Primary_SSN = p.Field<string>("Primary_SSN"),
                                     IssuerLPDate = p.Field<string>("IssuerLPDate"),
                                     IssuerLastPaidAmount = p.Field<string>("IssuerLastPaidAmount"),
                                     ServicerLastPayDate = p.Field<string>("ServicerLastPayDate"),
                                     ServicerLastPayAmount = p.Field<string>("ServicerLastPayAmount"),
                                     IssuerName = p.Field<string>("IssuerName"),
                                     Primary_Phone_Home = p.Field<string>("Primary_Phone-Home"),
                                     Primary_ZipPostal = p.Field<string>("Primary_ZipPostal"),
                                     Primary_City = p.Field<string>("Primary_City"),
                                     Primary_State = p.Field<string>("Primary_State"),
                                     owner_code = p.Field<string>("owner code"),
                                     Primary_FName = p.Field<string>("Primary_FName"),
                                     Primary_Middle = p.Field<string>("Primary_Middle"),
                                     Primary_LName = p.Field<string>("Primary_LName"),
                                     Primary_Address1 = p.Field<string>("Primary_Address1"),
                                     Primary_Address2 = p.Field<string>("Primary_Address2"),
                                     Primary_Email = p.Field<string>("Primary_Email"),
                                     acct_status = p.Field<string>("acct status"),
                                     OWNER_STATUS = p.Field<string>("OWNER STATUS"),

                                 };
            foreach (var q in jkdetails_query)
            {
                JKDetails jkd = new JKDetails();
                jkd.file_no = q.file_no;
                jkd.Portfolio = q.Portfolio;
                jkd.Portfolio_Purchase_Date = q.Portfolio_Purchase_Date;
                jkd.OriginalBalance = q.OriginalBalance;
                jkd.Original_Principal = q.Original_Principal;
                jkd.Original_Interest = q.Original_Interest;
                jkd.Original_Costs = q.Original_Costs;
                jkd.Original_Fees = q.Original_Fees;
                jkd.Current_Principal = q.Current_Principal;
                jkd.Current_Interest = q.Current_Interest;
                jkd.Current_Costs = q.Current_Costs;
                jkd.Current_Fees = q.Current_Fees;
                jkd.CurrentBalance = q.CurrentBalance;
                jkd.AccountOpenDate = q.AccountOpenDate;
                jkd.CHARGEOFF_DATE = q.CHARGEOFF_DATE;
                jkd.ChargeOff_Amount = q.ChargeOff_Amount;
                jkd.IssuerAccountNumber = q.IssuerAccountNumber;
                jkd.Primary_SSN = q.Primary_SSN;
                jkd.IssuerLPDate = q.IssuerLPDate;
                jkd.IssuerLastPaidAmount = q.IssuerLastPaidAmount;
                jkd.ServicerLastPayDate = q.ServicerLastPayDate;
                jkd.ServicerLastPayAmount = q.ServicerLastPayAmount;
                jkd.IssuerName = q.IssuerName;
                jkd.Primary_Phone_Home = q.Primary_Phone_Home;
                jkd.Primary_ZipPostal = q.Primary_ZipPostal;
                jkd.Primary_City = q.Primary_City;
                jkd.Primary_State = q.Primary_State;
                jkd.owner_code = q.owner_code;
                jkd.Primary_FName = q.Primary_FName;
                jkd.Primary_Middle = q.Primary_Middle;
                jkd.Primary_LName = q.Primary_LName;
                jkd.Primary_Address1 = q.Primary_Address1;
                jkd.Primary_Address2 = q.Primary_Address2;
                jkd.Primary_Email = q.Primary_Email;
                jkd.acct_status = q.acct_status;
                jkd.OWNER_STATUS = q.OWNER_STATUS;
                jkdts.Add(jkd);

            }
                return jkdts;
        }

        private List<JDetails> CreateJDetails(DataTable dt)
        {
            List<JDetails> jdts = new List<JDetails>();

            var jdetails_query = from p in dt.AsEnumerable()
                                 select
                                 new
                                 {
                                     file_no = p.Field<string>("file no"),
                                     data_id = p.Field<string>("data_id"),
                                     purchase_description = p.Field<string>("purchase_description"),
                                     purchase_date = p.Field<DateTime?>("purchase date"),
                                     purchase_id = p.Field<string>("purchase id"),
                                     data_bal_total_start = p.Field<double?>("data_bal_total_start"),
                                     data_bal_total = p.Field<double?>("data_bal_total"),
                                     data_bal_principal = p.Field<double?>("data_bal_principal"),
                                     data_bal_interest = p.Field<double?>("data_bal_interest"),
                                     data_bal_other = p.Field<double?>("data_bal_other"),
                                     data_bal_principal_start = p.Field<double?>("data_bal_principal_start"),
                                     data_bal_interest_start = p.Field<double?>("data_bal_interest_start"),
                                     data_bal_other_start = p.Field<double?>("data_bal_other_start"),
                                     acctopen = p.Field<DateTime?>("acctopen"),
                                     intrate = p.Field<double?>("intrate"),
                                     chargeoff = p.Field<DateTime?>("chargeoff"),
                                     chargeoffamt = p.Field<double?>("chargeoffamt"),
                                     pri_acctno = p.Field<string>("pri_acctno"),
                                     sec_acctno = p.Field<string>("sec_acctno"),
                                     delinquency = p.Field<DateTime?>("delinquency"),
                                     lastpayment = p.Field<DateTime?>("lastpayment"),
                                     lastpayment_ptc = p.Field<DateTime?>("lastpayment_ptc"),
                                     lastpaymentamt = p.Field<double?>("lastpaymentamt"),
                                     lastpaymentamt_ptc = p.Field<double?>("lastpaymentamt_ptc"),
                                     originator = p.Field<string>("originator"),
                                     merchant = p.Field<string>("merchant"),
                                     STATUTE_DATE = p.Field<DateTime?>("STATUTE DATE"),
                                     DEBT_TYPE = p.Field<string>("DEBT TYPE"),
                                     legal_placement_date = p.Field<DateTime?>("legal_placement_date"),
                                     judgment_award = p.Field<double?>("judgment_award"),
                                     HOME_PHONE = p.Field<string>("HOME PHONE"),
                                     pri_zip = p.Field<string>("pri_zip"),
                                     pri_city = p.Field<string>("pri_city"),
                                     pri_state = p.Field<string>("pri_state"),
                                     entity_name = p.Field<string>("entity_name"),
                                     pri_first = p.Field<string>("pri_first"),
                                     pri_middle = p.Field<string>("pri_middle"),
                                     pri_last = p.Field<string>("pri_last"),
                                     pri_city1 = p.Field<string>("pri_city1"),
                                     pri_state1 = p.Field<string>("pri_state1"),
                                     pri_zip1 = p.Field<string>("pri_zip1"),
                                     pri_dob = p.Field<DateTime?>("pri_dob"),
                                     pri_ssn = p.Field<string>("pri_ssn"),
                                     pri_add1 = p.Field<string>("pri_add1"),
                                     pri_add2 = p.Field<string>("pri_add2"),
                                     pri_email = p.Field<string>("pri_email"),
                                     comaker_first = p.Field<string>("comaker_first"),
                                     comaker_middle = p.Field<string>("comaker_middle"),
                                     comaker_last = p.Field<string>("comaker_last"),
                                     comaker_city = p.Field<string>("comaker_city"),
                                     comaker_state = p.Field<string>("comaker_state"),
                                     comaker_zip = p.Field<string>("comaker_zip"),
                                     comaker_dob = p.Field<DateTime?>("comaker_dob"),
                                     comaker_add1 = p.Field<string>("comaker_add1"),
                                     comaker_add2 = p.Field<string>("comaker_add2"),
                                     Grouping = p.Field<string>("Grouping"),
                                     Lpurchase_id = p.Field<string>("Lpurchase_id"),
                                     AF_acct_status = p.Field<string>("AF_acct_status"),
                                     AF_owner_status = p.Field<string>("AF_owner_status"),
                                 };

            foreach (var q in jdetails_query)
            {
                JDetails jd = new JDetails();
                jd.file_no = q.file_no;
                jd.data_id = q.data_id;
                jd.purchase_description = q.purchase_description;
                jd.purchase_date = q.purchase_date;
                jd.purchase_id = q.purchase_id;
                jd.data_bal_total_start = q.data_bal_total_start;
                jd.data_bal_total = q.data_bal_total;
                jd.data_bal_principal = q.data_bal_principal;
                jd.data_bal_interest = q.data_bal_interest;
                jd.data_bal_other = q.data_bal_other;
                jd.data_bal_principal_start = q.data_bal_principal_start;
                jd.data_bal_interest_start = q.data_bal_interest_start;
                jd.data_bal_other_start = q.data_bal_other_start;
                jd.acctopen = q.acctopen;
                jd.intrate = q.intrate;
                jd.chargeoff = q.chargeoff;
                jd.chargeoffamt = q.chargeoffamt;
                jd.pri_acctno = q.pri_acctno;
                jd.sec_acctno = q.sec_acctno;
                jd.delinquency = q.delinquency;
                jd.lastpayment = q.lastpayment;
                jd.lastpayment_ptc = q.lastpayment_ptc;
                jd.lastpaymentamt = q.lastpaymentamt;
                jd.lastpaymentamt_ptc = q.lastpaymentamt_ptc;
                jd.originator = q.originator;
                jd.merchant = q.merchant;
                jd.STATUTE_DATE = q.STATUTE_DATE;
                jd.DEBT_TYPE = q.DEBT_TYPE;
                jd.legal_placement_date = q.legal_placement_date;
                jd.judgment_award = q.judgment_award;
                jd.HOME_PHONE = q.HOME_PHONE;
                jd.pri_zip = q.pri_zip;
                jd.pri_city = q.pri_city;
                jd.pri_state = q.pri_state;
                jd.entity_name = q.entity_name;
                jd.pri_first = q.pri_first;
                jd.pri_middle = q.pri_middle;
                jd.pri_last = q.pri_last;
                jd.pri_city1 = q.pri_city1;
                jd.pri_state1 = q.pri_state1;
                jd.pri_zip1 = q.pri_zip1;
                jd.pri_dob = q.pri_dob;
                jd.pri_ssn = q.pri_ssn;
                jd.pri_add1 = q.pri_add1;
                jd.pri_add2 = q.pri_add2;
                jd.pri_email = q.pri_email;
                jd.comaker_first = q.comaker_first;
                jd.comaker_middle = q.comaker_middle;
                jd.comaker_last = q.comaker_last;
                jd.comaker_city = q.comaker_city;
                jd.comaker_state = q.comaker_state;
                jd.comaker_zip = q.comaker_zip;
                jd.comaker_dob = q.comaker_dob;
                jd.comaker_add1 = q.comaker_add1;
                jd.comaker_add2 = q.comaker_add2;
                jd.Grouping = q.Grouping;
                jd.Lpurchase_id = q.Lpurchase_id;
                jd.AF_acct_status = q.AF_acct_status;
                jd.AF_owner_status = q.AF_owner_status;
                
                jdts.Add(jd);

            }
            return jdts;
        }
    }
}
