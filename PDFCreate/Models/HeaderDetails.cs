using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCreate.Models
{
    public class HeaderDetails
    {
        public List<string> acct { get; set; }
        public List<string> fname { get; set; }
        public List<string> mname { get; set; }
        public List<string> lname { get; set; }
    }
}
