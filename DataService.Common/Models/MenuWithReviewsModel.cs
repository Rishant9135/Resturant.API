using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionData.Common.Models
{
    public class MenuWithReviewsModel
    {
        public long MenuId { get; set; }
        public string EnglishName { get; set; }
        public string HindiName { get; set; }
        public string CategoryEnglishName { get; set; }
        public string CategoryHindiName { get; set; }
        public long Price { get; set; }
        public string Image { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVeg { get; set; }

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<MenuReviewTbl> Reviews { get; set; }
    }

}