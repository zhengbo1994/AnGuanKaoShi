﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Biz_CertificateAuthenticationLog
    {
        [Key]
        public int Id { get; set; }
        public string XM { get; set; }
        public string XB { get; set; }
        public string MZ { get; set; }
        public DateTime? CSRQ { get; set; }
        public string JTDZ { get; set; }
        public string SFZH { get; set; }
        public string FZJG { get; set; }
        public DateTime? SFZYXKSRQ { get; set; }
        public DateTime? SFZYXJZRQ { get; set; }
        public string SBH { get; set; }
        public byte[] ZP { get; set; }
        public string SJHM { get; set; }
        public string QYMC { get; set; }
        public string ZZJGDM { get; set; }
        public string ZW { get; set; }
        public string ZC { get; set; }
        public string ZSLX { get; set; }
        public string ZSBH { get; set; }
        public string ZSBM { get; set; }
        public DateTime? FZRQ { get; set; }
        public DateTime? ZSYXJZRQ { get; set; }
        public string STATE { get; set; }
        public string Result { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
