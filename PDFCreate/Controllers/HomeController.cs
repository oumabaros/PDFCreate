using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PDFCreate.Models;
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
                            string JDetailsSheet = JDetailsSchema.Rows[1]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Get the name of JKDetails Sheet.
                            connExcel.Open();
                            DataTable JKDetailsSchema;
                            JKDetailsSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string JKDetailsSheet = JKDetailsSchema.Rows[2]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Get the name of Payments Sheet.
                            connExcel.Open();
                            DataTable PaymentsSchema;
                            PaymentsSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string PaymentsSheet = PaymentsSchema.Rows[3]["TABLE_NAME"].ToString();
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
                List<JDetails> jdt = new List<JDetails>();
                jdt = CreateJDetails(JDetailsDT);
                List<JKDetails> jkdt = new List<JKDetails>();
                List<Payments> pmnts = new List<Payments>();

                

                foreach(JDetails jd in jdt)
                {
                    System.Diagnostics.Debug.WriteLine("FILENO: " + jd.file_no.ToString());
                }
            }

            return Redirect("~/Home/Index");
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
                                     purchase_date = p.Field<DateTime>("purchase date"),
                                     purchase_id = p.Field<int>("purchase id"),
                                     data_bal_total_start = p.Field<decimal>("data_bal_total_start"),
                                     data_bal_total = p.Field<decimal>("data_bal_total"),
                                     data_bal_principal = p.Field<decimal>("data_bal_principal"),
                                     data_bal_interest = p.Field<decimal>("data_bal_interest"),
                                     data_bal_other = p.Field<decimal>("data_bal_other"),
                                     data_bal_principal_start = p.Field<decimal>("data_bal_principal_start"),
                                     data_bal_interest_start = p.Field<decimal>("data_bal_interest_start"),
                                     data_bal_other_start = p.Field<decimal>("data_bal_other_start"),
                                     acctopen = p.Field<DateTime>("acctopen"),
                                     intrate = p.Field<decimal>("intrate"),
                                     chargeoff = p.Field<DateTime>("chargeoff"),
                                     chargeoffamt = p.Field<decimal>("chargeoffamt"),
                                     pri_acctno = p.Field<string>("pri_acctno"),
                                     sec_acctno = p.Field<string>("sec_acctno"),
                                     delinquency = p.Field<DateTime>("delinquency"),
                                     lastpayment = p.Field<DateTime>("lastpayment"),
                                     lastpayment_ptc = p.Field<DateTime>("lastpayment_ptc"),
                                     lastpaymentamt = p.Field<decimal>("lastpaymentamt"),
                                     lastpaymentamt_ptc = p.Field<decimal>("lastpaymentamt_ptc"),
                                     originator = p.Field<string>("originator"),
                                     merchant = p.Field<string>("merchant"),
                                     STATUTE_DATE = p.Field<DateTime>("STATUTE DATE"),
                                     DEBT_TYPE = p.Field<string>("DEBT TYPE"),
                                     legal_placement_date = p.Field<DateTime>("legal_placement_date"),
                                     judgment_award = p.Field<decimal>("judgment_award"),
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
                                     pri_dob = p.Field<DateTime>("pri_dob"),
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
                                     comaker_dob = p.Field<DateTime>("comaker_dob"),
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
                jd.file_no = q.file_no.ToString();
                jdts.Add(jd);

            }
            return jdts;
        }
    }
}
