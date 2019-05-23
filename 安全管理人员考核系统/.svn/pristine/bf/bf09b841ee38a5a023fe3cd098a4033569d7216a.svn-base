using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using CLSLibrary;
using System.Data;

namespace PrintApp.Common
{
    public class CertificatePrinter
    {
        private enum Enum_PrintType
        {
            Normal, Change, Extend
        }

        private string DownloadCertificateTemplate(Enum_PrintType printType)
        {
            throw new NotImplementedException();
        }

        private string DownloadCertificatePhoto()
        {
            throw new NotImplementedException();
        }

        public void PrintCertificate(string printTemplatePath, Dictionary<string, string> certificateInfo)
        {
            //往模板中填数据
            using (ExcelAdoDotNetHelper excelAdo = new ExcelAdoDotNetHelper("Microsoft.ACE.OLEDB.12.0", "Excel 8.0;HDR=no"))
            {

                excelAdo.Open(printTemplatePath);
                //获取数据
                string selectSql = "select * from [数据$]";
                DataTable dt = excelAdo.ExecuteDataTale(selectSql);
                Dictionary<string, string> dicUpdateFiled = new Dictionary<string, string>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string colname = dt.Rows[0][i].ToString().Trim();
                    DataColumn col = dt.Columns[i];
                    if (colname == "标识")
                    {
                        dicUpdateFiled.Add(col.ColumnName, dt.Rows[1][i].ToString());
                        continue;
                    }
                    if (colname.Trim() == "")
                    {
                        break;
                    }
                    if (!certificateInfo.Keys.Contains(colname))
                    {
                        throw new Exception("证书模板上的字段【" + colname + "】不存在");
                    }
                    dicUpdateFiled.Add(col.ColumnName, certificateInfo[colname]);
                }
                //更新数据
                //拼接SQL语句
                string strUpdateFiled = "";
                var fieldList = dicUpdateFiled.Select(p => new { p.Key, p.Value }).ToList();
                //摄取第一个字段不更新 作为更新的条件
                for (int i = 1; i < fieldList.Count(); i++)
                {
                    strUpdateFiled += ", [" + fieldList[i].Key + "]='" + fieldList[i].Value + "'";
                }
                string updateSQL = "update [数据$] set " + strUpdateFiled.Substring(1) + " where [" + fieldList[0].Key + "]='" + fieldList[0].Value + "'";
                excelAdo.ExecuteNonQuery(updateSQL);
                excelAdo.Close();
            };
            //打印
            using (ExcelHelper excelHelper = new ExcelHelper())
            {
                excelHelper.OpenExcel(printTemplatePath);
                excelHelper.Print(1);
                excelHelper.Dispose();
            };

        }

        public void PrintCertificateChange(string printTemplatePath, CertificateChangeInfo certificateChangeInfo)
        {
            throw new NotImplementedException();
        }

        public void PrintCertificateExtend(string printTemplatePath, CertificateExtendInfo certificateExtendInfo)
        {
            throw new NotImplementedException();
        }
    }
}
