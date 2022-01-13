using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCreate.Models
{
    public class Payments
    {
        public string file_no { get; set; }
        public double? payment_index { get; set; }
        public DateTime? payment_date { get; set; }
        public string agy_Id { get; set; }
        public decimal? payment_amount { get; set; }
        public string pay_Description { get; set; }
    }
}
