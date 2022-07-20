using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadUpDsiplay.Model
{
    public class LineViewDto
    {
        public List<TitleHead> TileHeads { get; set; }
        public List<MaterialConfig> Materials { get; set; }
        public List<ParameterHead> Parameters { get; set; }
    }
    public class TitleHead
    {
        public string Title { get; set; }
        public string Status { get; set; }
    }



    public class ParameterHead
    {
        public string ParameterName { get; set; }
        public string Spc { get; set; }

    }

    public class MaterialConfig
    {
        public string CHR_NUB_HMK_HYP { get; set; }
        public string CHR_NUB_HMK_NAME { get; set; }

    }
}
