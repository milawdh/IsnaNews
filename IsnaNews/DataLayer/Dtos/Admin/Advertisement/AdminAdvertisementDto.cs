using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Advertisement
{
    public class AdminAdvertisementDto
    {
        [DisplayName("کد")]
        public int Id { get; set; }
        public string MainBaner { get; set; }
        [DisplayName("لینک تبلیغ")]
        public string Link { get; set; }
    }
}
