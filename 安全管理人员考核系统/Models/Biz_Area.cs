using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Biz_Area
    {
        [Key]
        public int Id { get; set; }

        public int CityId { get; set; }

        public string AreaName { get; set; }

        public int Seq { get; set; }
    }
}
