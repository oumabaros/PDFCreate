using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFCreate.Models
{
    public class PdfModel
    {
        public List<JDetails> jdt { get; set; }
        public List<JKDetails> jkdt { get; set; }
        public List<Payments> pmnts { get; set; }
    }
}
