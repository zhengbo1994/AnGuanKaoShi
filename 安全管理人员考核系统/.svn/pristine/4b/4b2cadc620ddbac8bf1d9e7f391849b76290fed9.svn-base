using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.baseFn;
using System.Data;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;

namespace Library
{
    public class ExcelHelper
    {
        private const string CONST_CONNECTIONSTRING = "Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;data source={0}";

        public string ExcelFilePath { get; set; }

        public ExcelHelper()
        { }

        public ExcelHelper(string path)
        {
            if (!path.IsNull())
            {
                ExcelFilePath = path;
            }
        }

        public List<Cell> GetExcelValueList(string sheetName)
        {
            if (ExcelFilePath.IsNull())
            {
                throw new Exception("请指定Excel文件路径");
            }

            string conString = String.Format(CONST_CONNECTIONSTRING, ExcelFilePath);

            string sql = String.Format("select * from [{0}$]", sheetName);

            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conString);
            adapter.Fill(dataSet);

            System.Data.DataTable table = dataSet.Tables[0];

            int rowNumber = 1;

            List<Cell> cellListResult = new List<Cell>();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    string columnName = column.ColumnName;
                    string columnValue = row[columnName].ToString();

                    Cell cell = new Cell()
                    {
                        RowNumber = rowNumber,
                        CellName = columnName,
                        CellValue = columnValue
                    };
                    cellListResult.Add(cell);
                }
                rowNumber += 1;
            }

            return cellListResult;
        }

        public void InsertExcelValueList(string sheetName, List<Cell> cellList)
        {
            const string script = "insert into [{0}$] ({1}) values ({2}) ";
            string sql = "";

            List<int> rowNumberList = cellList.Select(p => p.RowNumber).Distinct().ToList();

            foreach (int rowNumber in rowNumberList)
            {
                List<Cell> cellListForRow = cellList.Where(p => p.RowNumber == rowNumber).ToList();
                List<string> cellNameList = cellListForRow.Select(p => p.CellName).ToList();
                List<string> cellValueList = cellListForRow.Select(p => "'" + p.CellValue + "'").ToList();
                sql += String.Format(script, sheetName, String.Join(",", cellNameList), String.Join(",", cellValueList));
            }

            string conString = String.Format(CONST_CONNECTIONSTRING, ExcelFilePath);
            OleDbConnection con = new OleDbConnection(conString);
            OleDbCommand cmd = new OleDbCommand(sql, con);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }

        }

        public class Cell
        {
            public int RowNumber { get; set; }

            public string CellName { get; set; }

            public string CellValue { get; set; }
        }

        #region 文件转PDF
        public bool ConvertToPDF(string sourcePath, string targetPath)
        {
            return ConvertToPDF(sourcePath, targetPath, XlFixedFormatType.xlTypePDF);
        }



        private bool ConvertToPDF(string sourcePath, string targetPath, XlFixedFormatType targetType)
        {
            bool result;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.ApplicationClass application = null;
            Workbook workBook = null;
            try
            {
                application = new Microsoft.Office.Interop.Excel.ApplicationClass();
                object target = targetPath;
                object type = targetType;
                workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing, missing, missing, missing);

                workBook.ExportAsFixedFormat(targetType, target, XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
                result = true;
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        #endregion

    }


}
