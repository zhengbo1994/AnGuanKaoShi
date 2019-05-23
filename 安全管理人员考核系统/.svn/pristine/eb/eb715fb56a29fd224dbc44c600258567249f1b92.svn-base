using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Biz_News
    {
        [Key]
        public string Id { get; set; }
        public string NewsTitle { get; set; }
        public string NewsAbstract { get; set; }
        public string NewsContent { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }
        public bool IsDeleted { get; set; }

        public Biz_News()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToString();
            }
            IsDeleted = false;
            CreateDate = DateTime.Now;
        }
    }
}
