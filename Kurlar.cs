using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace gorsel_programlam_odev2
{
    public class Kurlar
    {
        public class Altn
        {
            public string isim { get; set; }
            public double alis { get; set; }
            public double satis { get; set; }
            public string durum { get; set; }
        }

        public class Root
        {
            public string tarih { get; set; }
            public string ceyrekAltin { get; set; }
            public string gramAltin { get; set; }
            public string ceyrekCumhuriyet { get; set; }
            public string yarimCumhuriyet { get; set; }
            public string tamCumhuriyet { get; set; }
            public string ataCumhuriyet { get; set; }
            public List<Altn> altn { get; set; }
        }
    }
}
