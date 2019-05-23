using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    /// <summary>
    /// 考核点
    /// </summary>
    public class Biz_ExaminationPoint
    {
        [Key]
        public int Id { get; set; }
        //考核点名称
        public string InstitutionName { get; set; }
        //社会信用代码
        public string SocialCreditCode { get; set; }
        //法定代表人
        public string LegalRepresentative { get; set; }
        //法定代表人电话
        public string LegalRepresentativeNumber { get; set; }
        //联系人
        public string ContactPerson { get; set; }
        //联系人电话
        public string ContactNumber { get; set; }
        //考核城市
        public string City { get; set; }
        //考核区域
        public string Area { get; set; }
        public string Email { get; set; }
        //地址
        public string Address { get; set; }

    }
}
