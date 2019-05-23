using System;
using System.Linq;
using Microsoft.Office.Interop;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CLSLibrary
{
    /// <summary>
    /// Excel配置参数
    /// </summary>
    public class ExcelConfig
    {
        /// <summary>
        /// 初始化配置
        /// </summary>
        public ExcelConfig()
        {
            _visible = false;
            _disableVBA = true;
            _displayAlerts = false;
            _enableAutoRecover = false;
            _killExcelTimeout = new TimeSpan(0, 0, 30);
        }

        private bool _visible = false;
        /// <summary>
        /// 可视化编辑（默认false）
        /// </summary>
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        private bool _displayAlerts = false;
        /// <summary>
        /// 是否弹出保存确认对话框（默认false）
        /// </summary>
        public bool DisplayAlerts
        {
            get
            {
                return _displayAlerts;
            }
            set
            {
                _displayAlerts = value;
            }
        }

        private bool _disableVBA = false;
        /// <summary>
        /// 是否禁用VBA脚本（默认false）
        /// </summary>
        public bool DisableVBA
        {
            get
            {
                return _disableVBA;
            }
            set
            {
                _disableVBA = value;
            }
        }

        private bool _enableAutoRecover = false;
        /// <summary>
        /// 是否启用自动恢复
        /// </summary>
        public bool EnableAutoRecover
        {
            get
            {
                return _enableAutoRecover;
            }
            set { _enableAutoRecover = value; }
        }

        private TimeSpan _killExcelTimeout = new TimeSpan(0, 0, 30);
        /// <summary>
        /// 从创建excel进程开始到强制结束进程的超时时间（默认new TimeSpan(0, 0, 30)）
        /// </summary>
        public TimeSpan KillExcelTimeout
        {
            get
            {
                return _killExcelTimeout;
            }
            set
            {
                _killExcelTimeout = value;
            }
        }
    }

    /// <summary>
    /// Excel操作帮助类
    /// </summary>
    public class ExcelHelper : IDisposable
    {
        #region---------------------变量-----------------------------------------
        /// <summary>
        /// EXCEL应用程序
        /// </summary>
        private Excel._Application FApp;
        /// <summary>
        /// EXCEL文件集合
        /// </summary>
        private Excel.Workbooks workbooks;
        /// <summary>
        /// 单个EXCEl文件
        /// </summary>
        private Excel._Workbook workbook;
        /// <summary>
        /// sheet
        /// </summary>
        private Excel._Worksheet worksheet;
        /// <summary>
        /// aRg
        /// </summary>
        private Excel.Range aRg;
        /// <summary>
        /// xlLineStyle
        /// </summary>
        public Excel.XlLineStyle xlLineStyle { get; set; }

        public ExcelConfig Config { get; set; }

        /// <summary>
        /// 保存文件名称
        /// </summary>
        private string SaveName { get; set; }


        #endregion------------------------------------------------------------------------------------

        #region 强制结束进程
        private static object _locker = new object();
        private static List<Thread> _listKillProcessThread;

        /// <summary>
        /// 添加结束excel的线程
        /// </summary>
        /// <param name="processID"></param>
        private void AddApplicationProcess(int processID)
        {
            Thread tDispose = new Thread(new ParameterizedThreadStart((arg) =>
            {
                int pid = Convert.ToInt32(arg);
                Thread.Sleep(Config.KillExcelTimeout);
                try
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                    if (null != process)
                    {
                        process.Kill();
                    }
                }
                catch { }
                RemoveApplicationProcess(pid);
            }));
            tDispose.Name = "kill_" + processID;
            lock (_locker)
            {
                _listKillProcessThread.Add(tDispose);
            }
            tDispose.Start(processID);
        }

        /// <summary>
        /// 移除结束excel的进程
        /// </summary>
        /// <param name="processID"></param>
        private void RemoveApplicationProcess(int processID)
        {
            Thread tDispose = _listKillProcessThread.FirstOrDefault(x => x.Name == "kill_" + processID);
            if (null != tDispose)
            {
                lock (_locker)
                {
                    _listKillProcessThread.Remove(tDispose);
                }
                if (tDispose.ThreadState == ThreadState.Running)
                {
                    tDispose.Abort();
                }
                tDispose = null;
            }
        }

        /// <summary>
        /// 结束进程
        /// </summary>
        /// <param name="processID"></param>
        private void KillProcess(int processID)
        {
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(processID);
                if (null != process)
                {
                    process.Kill();
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取进程ID
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        #endregion
        /// <summary>
        /// 使用默认参数初始化实例
        /// </summary>
        public ExcelHelper() : this(new ExcelConfig()) { }

        /// <summary>
        /// 自定义参数初始化实例
        /// </summary>
        /// <param name="excelConfig">设置参数</param>
        public ExcelHelper(ExcelConfig excelConfig)
        {
            int excelProcessID = -1;
            Config = excelConfig;
            _listKillProcessThread = new List<Thread>();

            FApp = new Microsoft.Office.Interop.Excel.Application();

            //进程加入队列，超时强制关闭进程
            IntPtr excelPtr = new IntPtr(FApp.Hwnd);
            GetWindowThreadProcessId(excelPtr, out excelProcessID);
            AddApplicationProcess(excelProcessID);

            workbooks = FApp.Workbooks;
            //设置禁止弹出保存和覆盖的询问提示框
            FApp.DisplayAlerts = Config.DisplayAlerts;
            FApp.AlertBeforeOverwriting = Config.DisplayAlerts;
            FApp.UserControl = false;//如果只想用程序控制该excel而不想让用户操作时候，可以设置为false
            FApp.Visible = Config.Visible;
            if (Config.DisableVBA)
            {
                FApp.AutomationSecurity = MsoAutomationSecurity.msoAutomationSecurityForceDisable;
            }
            FApp.AutoRecover.Enabled = Config.EnableAutoRecover;
        }

        /// <summary>
        /// 创建一个Excel对象
        /// </summary>
        public void Create()
        {
            FApp.Visible = Config.Visible;
            workbooks = FApp.Workbooks;
            workbook = workbooks.Add(true);
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)FApp.ActiveSheet;
        }
        /// <summary>
        /// 打开 新Excel
        /// </summary>
        public void OpenExcel()
        {
            Create();
        }

        /// <summary>
        /// 打开已有Excel
        /// </summary>
        /// <param name="mfileName">Excel路径</param>
        public void OpenExcel(string mfileName)
        {
            SaveName = mfileName;
            workbooks.Open(SaveName, true);
            workbook = workbooks.get_Item(workbooks.Count);
            try
            {
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)FApp.ActiveSheet;
            }
            catch
            {
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(1);
            }
        }
        /// <summary>
        /// 获取单元格内容
        /// </summary>
        /// <param name="aRow">行</param>
        /// <param name="aCol">列</param>
        /// <returns>内容</returns>
        public string GetCellValue(int aRow, int aCol)
        {
            string result = "";
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[aRow, aCol];
            if (aRg.Text != null)
            {
                result = (string)aRg.Text;
            }
            aRg = null;
            return result;
        }
        /// <summary>
        /// 获取单元格内容
        /// </summary>
        /// <param name="aSheet">页</param>
        /// <param name="aRow">行</param>
        /// <param name="aCol">列</param>
        /// <returns>内容</returns>
        public string GetCellValue(int aSheet, int aRow, int aCol)
        {
            string result = "";
            Excel.Worksheet sheet = ((Excel.Worksheet)workbook.Worksheets.get_Item(aSheet));
            aRg = (Excel.Range)sheet.Cells[aRow, aCol];
            if (aRg.Text != null)
            {
                result = (string)aRg.Text;
            }
            sheet = null;
            aRg = null;
            return result;
        }
        /// <summary>
        /// 读取单元格内容
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        public string GetCellValue(string cellname)
        {
            string result = "";
            Excel.Names names = ((Excel._Workbook)workbook).Names;
            foreach (Excel.Name item in names)
            {
                if (item.Name.ToLower() != cellname.ToLower())//没找到名称时继续找
                {
                    continue;
                }

                //找到后就开始赋值
                string cellnamevalue = item.Value;//=检测报告!B$1$
                if (string.IsNullOrEmpty(cellnamevalue))//名称没有关联到单元格直接停止查找
                {
                    break;
                }
                string Formula = cellnamevalue.Remove(0, 1);
                string sheetName = Formula.Split('!')[0];
                string col = Formula.Split('$')[1];
                string row = Formula.Split('$')[2];
                //获取sheet
                Excel._Worksheet sheet = (Excel._Worksheet)Get_worksheet(sheetName);
                result = Convert.ToString(sheet.Range[col + row, Type.Missing].Value2);
                sheet = null;
                break;
            }
            names = null;
            return result;
        }
        /// <summary>
        /// 获取工作表的名称集合
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Excel.Name> GetNames()
        {
            System.Collections.Generic.List<Excel.Name> names = new System.Collections.Generic.List<Excel.Name>();
            foreach (Excel.Name item in worksheet.Names)
            {
                names.Add(item);
            }
            return names;
        }
        /// <summary>
        /// 获取工作表指定行的名称集合
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Excel.Name> GetNames(string rowindex)
        {
            System.Collections.Generic.List<Excel.Name> names = new System.Collections.Generic.List<Excel.Name>();
            foreach (Excel.Name item in worksheet.Names)
            {
                string[] temp = item.Value.Split('$');
                if (temp.Length < 3)
                {
                    continue;
                }
                if (temp[2] == rowindex)
                {
                    names.Add(item);
                }
            }
            return names;
        }
        /// <summary>
        /// 读取单元格名称对象(当前活动的sheet)
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        /// <param name="indextype">0：名称开头 1：名称的结尾 其他字符按为中间包含</param>
        /// <returns></returns>
        public Excel.Name GetCellName(string cellname, int indextype)
        {
            Excel.Name result = null;
            Excel.Names names = worksheet.Names;
            foreach (Excel.Name item in names)
            {
                bool flage = false;
                string namestr = item.Name.Split('!')[1];
                if (indextype == 0)
                {
                    flage = namestr.ToLower().StartsWith(cellname.ToLower());
                }
                else if (indextype == 1)
                {
                    flage = namestr.ToLower().EndsWith(cellname.ToLower());
                }
                else
                {
                    flage = namestr.ToLower().IndexOf(cellname.ToLower()) > -1;
                }
                if (!flage)//没找到名称时继续找
                {
                    continue;
                }
                else
                {
                    result = item;
                }
            }
            names = null;
            return result;
        }
        /// <summary>
        /// 读取单元格名称
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        /// <param name="indextype">0：名称开头 1：名称的结尾 其他字符按为中间包含</param>
        /// <returns></returns>
        public string GetCellName(int aRow, int aCol)
        {
            string result = null;
            try
            {
                result = ((Excel.Range)worksheet.Cells[aRow, aCol]).Name.ToString();
            }
            catch
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// 读取单元格内容(根据特定的前缀 将这类名称的单元格以字符串的形式连接起来例如 名称:value,... 若单元格数据为空时 不返回)
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        /// <param name="indextype">0：名称开头 1：名称的结尾 其他字符按为中间包含</param>
        public string GetCellValue(string cellname, int indextype)
        {
            string result = "";
            Excel.Names names = ((Excel._Workbook)workbook).Names;
            foreach (Excel.Name item in names)
            {
                bool flage = false;
                if (indextype == 0)
                {
                    flage = item.Name.ToLower().StartsWith(cellname.ToLower());
                }
                else if (indextype == 1)
                {
                    flage = item.Name.ToLower().EndsWith(cellname.ToLower());
                }
                else
                {
                    flage = item.Name.ToLower().IndexOf(cellname.ToLower()) > -1;
                }
                if (!flage)//没找到名称时继续找
                {
                    continue;
                }

                //找到后就开始赋值
                string cellnamevalue = item.Value;//=检测报告!B$1$
                if (string.IsNullOrEmpty(cellnamevalue))//名称没有关联到单元格直接停止查找
                {
                    break;
                }
                string Formula = cellnamevalue.Remove(0, 1);
                string sheetName = Formula.Split('!')[0];
                Formula = Formula.Split('!')[1];
                string col = Formula.Split('$')[1];
                string row = Formula.Split('$')[2];
                //获取sheet
                Excel._Worksheet sheet = (Excel._Worksheet)Get_worksheet(sheetName);
                result += item.Name + ":" + sheet.Range[col + row, Type.Missing].Value2 + ",";
                sheet = null;
            }
            names = null;
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
        /// <summary>
        /// 获取单元格公式
        /// </summary>
        /// <param name="aSheet"></param>
        /// <param name="aRow"></param>
        /// <param name="aCol"></param>
        /// <returns></returns>
        public string GetCellFormula(int aSheet, int aRow, int aCol)
        {
            string result = "";
            Excel.Worksheet sheet = ((Excel.Worksheet)workbook.Worksheets.get_Item(aSheet));
            aRg = (Microsoft.Office.Interop.Excel.Range)sheet.Cells[aRow, aCol];
            if (aRg.Formula != null)
            {
                result = (string)aRg.Formula;
            }
            sheet = null;
            aRg = null;
            return result;
        }


        #region 写入文本内容
        /// <summary>
        /// 单元格写入内容
        /// </summary>
        /// <param name="aRow">行</param>
        /// <param name="aCol">列</param>
        /// <param name="sValue">内容</param>
        public void SetCellValue(int aRow, int aCol, string sValue)
        {
            worksheet.Cells[aRow, aCol] = sValue;
        }
        /// <summary>
        /// 单元格写入内容
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        /// <param name="value">单元格文本</param>
        public void SetCellValue(string cellname, string value)
        {
            Excel.Names names = ((Excel._Workbook)workbook).Names;
            foreach (Excel.Name item in names)
            {
                if (item.Name.ToLower() != cellname.ToLower())//没找到名称时继续找
                {
                    continue;
                }

                //找到后就开始赋值
                string cellnamevalue = item.Value;//=检测报告!B$1$
                if (string.IsNullOrEmpty(cellnamevalue))//名称没有关联到单元格直接停止查找
                {
                    break;
                }
                string Formula = cellnamevalue.Remove(0, 1);
                string sheetName = Formula.Split('!')[0].Replace("'", "");
                string col = Formula.Split('$')[1];
                string row = Formula.Split('$')[2];
                //获取sheet
                Excel._Worksheet sheet = (Excel._Worksheet)Get_worksheet(sheetName);
                sheet.Range[col + row, Type.Missing].Value2 = value;
                sheet = null;
                break;
            }
            names = null;
        }
        /// <summary>
        /// 单元格写入内容
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        /// <param name="value">单元格文本</param>
        public void SetCellValueINActiveSheet(string cellname, string value)
        {
            Excel.Names names = worksheet.Names;
            foreach (Excel.Name item in names)
            {
                if (item.Name.Split('!')[1].ToLower() != cellname.ToLower())//没找到名称时继续找
                {
                    continue;
                }

                //找到后就开始赋值
                string cellnamevalue = item.Value;//=检测报告!B$1$
                if (string.IsNullOrEmpty(cellnamevalue))//名称没有关联到单元格直接停止查找
                {
                    break;
                }
                string Formula = cellnamevalue.Remove(0, 1);
                string sheetName = Formula.Split('!')[0].Replace("'", "");
                string col = Formula.Split('$')[1];
                string row = Formula.Split('$')[2];
                //获取sheet
                Excel._Worksheet sheet = (Excel._Worksheet)Get_worksheet(sheetName);
                sheet.Range[col + row, Type.Missing].Value2 = value;
                sheet = null;
                break;
            }
            names = null;
        }
        /// <summary>
        /// 单元格写入内容Range
        /// </summary>
        /// <param name="cellnamevalue">Rangestr(列+行)</param>
        /// <param name="value">要写入的文本</param>
        public void SetCellValueByCellNameVale(string Rangestr, string value)
        {
            worksheet.Range[Rangestr, Type.Missing].Value2 = value;
        }
        /// <summary>
        /// 指定单元格写入内容(索引从1开始)
        /// </summary>
        /// <param name="aSheet"></param>
        /// <param name="aRow"></param>
        /// <param name="aCol"></param>
        /// <param name="sValue"></param>
        public void SetCellValue(int aSheet, int aRow, int aCol, string sValue)
        {
            Excel.Worksheet sheet = ((Excel.Worksheet)workbook.Worksheets.get_Item(aSheet));
            sheet.Cells[aRow, aCol] = sValue;
            sheet = null;
        }

        #endregion

        #region 颜色写入
        /// <summary>
        /// 设置单元格颜色
        /// </summary>
        /// <param name="aRow">行</param>
        /// <param name="aCol">列</param>
        /// <param name="aColorIndex">颜色索引</param>
        public void SetCellColor(int aRow, int aCol, float aColorIndex)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[aRow, aCol];
            aRg.Interior.ColorIndex = aColorIndex;
            aRg = null;
        }

        /// <summary>
        /// 批量写入内容（内容相同）
        /// </summary>
        /// <param name="startRows">起始行</param>
        /// <param name="startColumns">起始列</param>
        /// <param name="endRows">结束行</param>
        /// <param name="endcolumns">结束列</param>
        /// <param name="sValue">文本内容</param>
        public void SetCellValue(int startRows, int startColumns, int endRows, int endcolumns, string sValue)
        {
            aRg = worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]];//(Microsoft.Office.Interop.Excel.Range)aWS.Cells[aRow, aCol];
            aRg.Value = sValue;
            aRg = null;
        }

        /// <summary>
        /// 单元格边框
        /// </summary>
        /// <param name="startRows">起始行</param>
        /// <param name="startColumns">起始列</param>
        /// <param name="endRows">结束行</param>
        /// <param name="endcolumns">结束列</param>
        public void SetBordersStyle(int startRows, int startColumns, int endRows, int endcolumns)
        {
            xlLineStyle = Excel.XlLineStyle.xlContinuous;
            //Borders.LineStyle 单元格边框线
            aRg = worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]];
            //单元格边框线类型(线型,虚线型)
            aRg.Borders.LineStyle = xlLineStyle;
            aRg.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle = xlLineStyle;
            aRg = null;
            aRg = worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]];
        }
        /// <summary>
        /// 批量设置单元格背景色
        /// </summary>
        /// <param name="startRows">起始行</param>
        /// <param name="startColumns">起始列</param>
        /// <param name="endRows">结束行</param>
        /// <param name="endcolumns">结束列</param>
        /// <param name="aColorIndex">颜色索引</param>
        public void SetCellColor(int startRows, int startColumns, int endRows, int endcolumns, float aColorIndex)
        {
            aRg = worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]];//(Microsoft.Office.Interop.Excel.Range)aWS.Cells[aRow, aCol];
            aRg.Interior.ColorIndex = aColorIndex;
            aRg = null;
        }
        /// <summary>
        /// 设置单元格背景色
        /// </summary>
        /// <param name="iRow">行</param>
        /// <param name="iCol">列</param>
        /// <param name="sColor">颜色</param>
        public void SetCellColor(int iRow, int iCol, Microsoft.Office.Interop.Excel.Constants sColor)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[iRow, iCol];
            aRg.Interior.Color = sColor;
            aRg = null;
        }
        #endregion

        #region 工作簿操作
        /// <summary>
        /// 显示指定sheet隐藏其他页
        /// </summary>
        /// <param name="sheetName"></param>
        public void ShowSheet(string sheetName)
        {

            foreach (Excel.Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name.IndexOf(sheetName) > -1)
                {
                    sheet.Visible = Excel.XlSheetVisibility.xlSheetVisible;
                }
                else
                {
                    sheet.Visible = Excel.XlSheetVisibility.xlSheetHidden;
                }
            }
        }
        /// <summary>
        /// 显示指定sheet隐藏其他页
        /// </summary>
        /// <param name="sheetName"></param>
        public void ShowSheetByVeryHiddenSheet(string sheetName)
        {

            foreach (Excel.Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name.IndexOf(sheetName) > -1)
                {
                    sheet.Visible = Excel.XlSheetVisibility.xlSheetVisible;
                }
                else
                {
                    sheet.Visible = Excel.XlSheetVisibility.xlSheetVeryHidden;
                }
            }
        }
        /// <summary>
        ///  设置工作簿名称
        /// </summary>
        /// <param name="SheetName">工作簿名称</param>
        public void SetActiveSheet(string SheetName)
        {
            worksheet.Activate();
            worksheet.Name = SheetName;
        }
        public void InsertSheet(object sheet)
        {
            workbook.Sheets.Add(sheet);
        }
        /// <summary>
        /// 获取_worksheet
        /// </summary>
        /// <returns></returns>
        public object Get_worksheet(string sheetname)
        {
            Excel._Worksheet osheet = null;
            foreach (Excel._Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name.IndexOf(sheetname) > -1)
                {
                    osheet = sheet;
                    break;
                }
            }
            return osheet;
        }

        /// <summary>
        /// 获取_worksheet的索引
        /// </summary>
        /// <returns></returns>
        public int Get_worksheetIndex(string sheetname)
        {
            int result = 0;
            Excel._Worksheet osheet = null;
            foreach (Excel._Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name.IndexOf(sheetname) > -1)
                {
                    osheet = sheet;
                    break;
                }
            }
            if (osheet != null)
            {
                result = osheet.Index;
            }
            osheet = null;
            return result;
        }
        /// <summary>
        /// 获取_worksheet
        /// </summary>
        /// <returns></returns>
        public object Get_worksheet(int sheetindex)
        {
            Excel._Worksheet osheet = (Excel.Worksheet)workbook.Worksheets[sheetindex];
            return osheet;
        }
        /// <summary>
        /// 复制sheet
        /// </summary>
        /// <param name="srcFilePath">要复制文件的路径</param>
        /// <param name="srcSheetIndex">sheet的索引</param>
        /// <param name="sheetname">复制的新sheet名称</param>
        public void CopySheet(string srcFilePath, int srcSheetIndex, string sheetname)
        {
            FApp.DisplayAlerts = Config.DisplayAlerts;
            bool IsOpen = false;//标识文件是否打开
            for (int i = 1; i <= workbooks.Count; i++)
            {
                if (workbooks.get_Item(i).FullName == srcFilePath)
                {
                    workbook = workbooks.get_Item(i);
                    IsOpen = true;
                }
            }
            if (!IsOpen)
            {
                workbook = workbooks.Add(srcFilePath);
            }
            Excel._Worksheet destSheet = (Excel._Worksheet)workbooks.get_Item(1).Worksheets.get_Item(workbooks.get_Item(1).Worksheets.Count);
            Excel._Worksheet srcSheet = (Excel._Worksheet)Get_worksheet(srcSheetIndex);
            srcSheet.Copy(Type.Missing, destSheet);
            Excel._Worksheet newsheet = (Excel._Worksheet)destSheet.Next;
            newsheet.Name = sheetname;
            workbook = workbooks.get_Item(1);
            //关闭(除第一个工作簿的其他工作簿)
            for (int i = 2; i <= workbooks.Count; i++)
            {
                workbooks.get_Item(i).Close(false);
            }

            destSheet = null;
            srcSheet = null;
            newsheet = null;
        }
        /// <summary>
        /// 复制sheet
        /// </summary>
        /// <param name="srcFilePath">要复制文件的路径</param>
        /// <param name="srcSheetName">sheet的索引</param>
        /// <param name="sheetname">复制的新sheet名称</param>
        public void CopySheet(string srcFilePath, string srcSheetName, string sheetname)
        {
            FApp.DisplayAlerts = Config.DisplayAlerts;
            bool IsOpen = false;//标识文件是否打开
            for (int i = 1; i <= workbooks.Count; i++)
            {
                if (workbooks.get_Item(i).FullName == srcFilePath)
                {
                    workbook = workbooks.get_Item(i);
                    IsOpen = true;
                }
            }
            if (!IsOpen)
            {
                workbook = workbooks.Add(srcFilePath);
            }
            Excel._Worksheet destSheet = (Excel._Worksheet)workbooks.get_Item(1).Worksheets.get_Item(workbooks.get_Item(1).Worksheets.Count);
            Excel._Worksheet srcSheet = (Excel._Worksheet)Get_worksheet(srcSheetName);
            srcSheet.Copy(Type.Missing, destSheet);
            Excel._Worksheet newsheet = (Excel._Worksheet)destSheet.Next;
            newsheet.Name = sheetname;
            workbook = workbooks.get_Item(1);
            //关闭(除第一个工作簿的其他工作簿)
            for (int i = 2; i <= workbooks.Count; i++)
            {
                workbooks.get_Item(i).Close(false);
            }
            destSheet = null;
            srcSheet = null;
            newsheet = null;
        }
        /// <summary>
        /// 复制sheet
        /// </summary>
        /// <param name="srcSheetIndex"></param>
        /// <param name="sheetname"></param>
        public void CopySheet(int srcSheetIndex, string newsheetname)
        {
            FApp.DisplayAlerts = Config.DisplayAlerts;
            Excel._Worksheet srcSheet = (Excel._Worksheet)workbook.Sheets.get_Item(srcSheetIndex);
            srcSheet.Copy(Type.Missing, srcSheet);
            Excel._Worksheet newsheet = (Excel._Worksheet)srcSheet.Next;
            newsheet.Name = newsheetname;

            newsheet = null;
            newsheet = null;
        }
        public void DeleteSheet(int sheetindex)
        {
            ((Excel.Worksheet)workbooks.get_Item(1).Worksheets.get_Item(sheetindex)).Delete();
        }
        /// <summary>
        /// 获取工作簿里sheet数
        /// </summary>
        /// <returns></returns>
        public int GetSheetCount()
        {
            int result = 0;
            if (workbook != null)
            {
                result = workbook.Worksheets.Count;
            }
            return result;
        }
        /// <summary>
        /// 获取sheet的名称
        /// </summary>
        /// <param name="sheetindex"></param>
        /// <returns></returns>
        public string GetSheetName(int sheetindex)
        {
            return ((Excel.Worksheet)workbook.Worksheets.get_Item(sheetindex)).Name;
        }

        /// <summary>
        /// 设置工作簿为活动页
        /// </summary>
        /// <param name="SheetIndex">工作簿的索引(从1开始)</param>
        public void SetSheetActive(int sheetindex)
        {
            ((Excel._Worksheet)workbooks.get_Item(1).Worksheets.get_Item(sheetindex)).Select();
        }
        /// <summary>
        /// 设置工作簿为活动页
        /// </summary>
        /// <param name="sheetname">工作簿的名称(模糊查询 只要含有这个关键字)</param>
        public void SetSheetActive(string sheetname)
        {
            foreach (Excel._Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name.IndexOf(sheetname) > -1)
                {
                    sheet.Select();
                    worksheet = (Excel.Worksheet)sheet;
                    break;
                }
            }

        }

        #endregion


        #region 行操作
        /// <summary>
        /// 在工作表中删除行
        /// </summary>
        /// <param name="sheet">当前工作表</param>
        /// <param name="rowIndex">欲删除的行索引</param>
        public void DeleteRows(int rowIndex)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows[rowIndex, Type.Missing];
            aRg.Delete(Microsoft.Office.Interop.Excel.XlDeleteShiftDirection.xlShiftUp);
            aRg = null;
        }
        /// <summary>
        /// 在工作表中插入行，并调整其他行以留出空间
        /// </summary>
        /// <param name="sheet">当前工作表</param>
        /// <param name="rowIndex">欲插入的行索引</param>
        public void InsertRows(int rowIndex)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Rows[rowIndex, Type.Missing];
            aRg.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftDown, Type.Missing);
            aRg = null;
        }


        #endregion

        #region 列操作
        /// <summary>
        /// 在工作表中删除列
        /// </summary>
        /// <param name="sheet">当前工作表</param>
        /// <param name="rowIndex">欲删除的行索引</param>
        public void DeleteColumn(int columnIndex)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, columnIndex];
            aRg.EntireColumn.Delete(0);
            aRg = null;
        }

        /// <summary>
        /// 在工作表中插入列
        /// </summary>
        /// <param name="columnIndex">列索引</param>
        public void InsertColumn(int columnIndex)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Columns[columnIndex, Type.Missing];
            aRg.Insert(Microsoft.Office.Interop.Excel.XlInsertShiftDirection.xlShiftDown, Type.Missing);
            aRg = null;
        }

        /// <summary>
        /// 列宽度
        /// </summary>
        /// <param name="columnIndex">列索引</param>
        /// <param name="width">宽度</param>
        public void SetColumnWidth(int columnIndex, float width)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, columnIndex];
            aRg.ColumnWidth = width;
            aRg = null;
        }
        #endregion



        #region 合并单元格
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="ws">Worksheet对象</param>
        /// <param name="Start_iRow">起始行</param>
        /// <param name="Start_iCol">起始列</param>
        /// <param name="End_iRow">结束行</param>
        /// <param name="End_iCol">结束列</param>
        public void UniteCells(int startRows, int startColumns, int endRows, int endcolumns)
        {
            worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]].Merge(Type.Missing);
        }

        #endregion

        #region 文字操作
        /// <summary>
        /// 文字剧中
        /// </summary>
        /// <param name="startRows">起始行</param>
        /// <param name="startColumns">起始列</param>
        /// <param name="endRows">结束行</param>
        /// <param name="endcolumns">结束列</param>
        public void SetValueAlign(int startRows, int startColumns, int endRows, int endcolumns)
        {
            aRg = worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]];

            // 可以提出Microsoft.Office.Interop.Excel.XlHAlign 让对齐方式更灵活
            aRg.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            aRg = null;
        }

        /// <summary>
        /// 文字加粗
        /// </summary>
        /// </summary>
        /// <param name="startRows">起始行</param>
        /// <param name="startColumns">起始列</param>
        /// <param name="endRows">结束行</param>
        /// <param name="endcolumns">结束列</param>
        public void SetValueBold(int startRows, int startColumns, int endRows, int endcolumns)
        {
            aRg = worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]];

            //
            aRg.Font.Bold = 2;
            aRg = null;
        }

        /// <summary>
        /// 加入超链接
        /// </summary>
        /// <param name="startRows">起始行</param>
        /// <param name="startColumns">起始列</param>
        /// <param name="endRows">结束行</param>
        /// <param name="endcolumns">结束列</param>
        /// <param name="link">链接地址</param>
        public void SetLink(int startRows, int startColumns, int endRows, int endcolumns, string link)
        {
            aRg = worksheet.Range[worksheet.Cells[startRows, startColumns], worksheet.Cells[endRows, endcolumns]];

            //参数说明：单元格，链接地址，missing，鼠标在超链接上显示内容，单元格内的文本
            aRg.Hyperlinks.Add(aRg.Cells, link, Type.Missing, link, link);
            aRg = null;
        }

        /// <summary>
        /// 设置单个单元格文字内容颜色
        /// </summary>
        /// <param name="iRow">行</param>
        /// <param name="iCol">列</param>
        /// <param name="sColor">颜色</param>
        public void SetCellFontColor(int iRow, int iCol, Microsoft.Office.Interop.Excel.Constants sColor)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[iRow, iCol];
            aRg.Font.Color = sColor;
            aRg = null;
        }
        /// <summary>
        /// 设置单个单元格文字内容颜色
        /// </summary>
        /// <param name="iRow">行</param>
        /// <param name="iCol">列</param>
        /// <param name="iColorIndex">索引</param>
        public void SetCellFontColor(int iRow, int iCol, int iColorIndex)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[iRow, iCol];
            aRg.Font.ColorIndex = iColorIndex;
            aRg = null;
        }

        /// <summary>
        /// 设置单个单元格内容文字大小
        /// </summary>
        /// <param name="iRow">行</param>
        /// <param name="iCol">列</param>
        /// <param name="iSize">大小</param>
        public void SetCellFontSize(int iRow, int iCol, int iSize)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[iRow, iCol];
            aRg.Font.Size = iSize;
            aRg = null;
        }

        /// <summary>
        /// 设置单个单元格内容对齐方式
        /// </summary>
        /// <param name="iRow">行</param>
        /// <param name="iCol">列</param>
        /// <param name="HorizontalAlignment">对齐方式</param>
        public void SetCellFontAlign(int iRow, int iCol, Microsoft.Office.Interop.Excel.Constants HorizontalAlignment)
        {
            aRg = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[iRow, iCol];
            aRg.HorizontalAlignment = HorizontalAlignment;
            aRg = null;
        }


        /// <summary>
        /// 设置多个单元格的属性   字体，   大小，颜色   ，对齐方式
        /// </summary>
        /// <param name="Start_iRow">起始行</param>
        /// <param name="Start_iCol">起始列</param>
        /// <param name="End_iRow">结束行</param>
        /// <param name="End_iCol">结束列</param>
        /// <param name="iSize">大小</param>
        /// <param name="fontName">字体</param>
        /// <param name="sColor">颜色</param>
        /// <param name="HorizontalAlignment">对齐方式</param>
        public void SetCellProperty(int Start_iRow, int Start_iCol, int End_iRow, int End_iCol, int iSize, string fontName, Microsoft.Office.Interop.Excel.Constants sColor, Microsoft.Office.Interop.Excel.Constants HorizontalAlignment)
        {
            worksheet.Range[worksheet.Cells[Start_iRow, Start_iCol], worksheet.Cells[End_iRow, End_iCol]].Font.Name = fontName;
            worksheet.Range[worksheet.Cells[Start_iRow, Start_iCol], worksheet.Cells[End_iRow, End_iCol]].Font.Size = iSize;
            worksheet.Range[worksheet.Cells[Start_iRow, Start_iCol], worksheet.Cells[End_iRow, End_iCol]].Font.Color = sColor;
            worksheet.Range[worksheet.Cells[Start_iRow, Start_iCol], worksheet.Cells[End_iRow, End_iCol]].HorizontalAlignment = HorizontalAlignment;
        }
        #endregion

        #region 内存数据写入Excel
        /// <summary>
        /// 将内存中数据表格插入到Excel指定工作表的指定位置 为在使用模板时控制格式时使用一
        /// </summary>
        /// <param name="dt">DataTable集合</param>
        /// <param name="ws">工作簿名称</param>
        /// <param name="iRow">起始行</param>
        /// <param name="iCol">起始列</param>
        public void InsertTable(System.Data.DataTable dt, int iRow, int iCol)
        {
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    worksheet.Cells[iRow + i, j + iCol] = "'" + dt.Rows[i][j].ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex">从1开始</param>
        /// <param name="startRowIndex">从1开始</param>
        /// <param name="startColIndex">从1开始</param>
        /// <param name="dt"></param>
        /// <param name="colNames"></param>
        public void InsertArray(int sheetIndex, int startRowIndex, int startColIndex, string[,] arr)
        {
            Excel.Worksheet sheet = null;
            Excel.Range rng = null;
            try
            {
                sheet = ((Excel.Worksheet)workbook.Worksheets.get_Item(sheetIndex));
                object[,] arrValues = null == arr ? new object[0, 0] : arr;
                int iRowCount = arrValues.GetLength(0);
                int iColCount = arrValues.GetLength(1);

                rng = sheet.get_Range(NumberToString26(startColIndex) + startRowIndex, NumberToString26(iColCount + startColIndex - 1) + (iRowCount + startRowIndex - 1));
                rng.Value2 = arrValues;
            }
            catch { throw; }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetIndex">从1开始</param>
        /// <param name="startRowIndex">从1开始</param>
        /// <param name="startColIndex">从1开始</param>
        /// <param name="dt"></param>
        /// <param name="colNames"></param>
        public void InsertTable(int sheetIndex, int startRowIndex, int startColIndex, System.Data.DataTable dt, string[] colNames = null)
        {
            Excel.Worksheet sheet = null;
            Excel.Range rng = null;
            if (null == dt || dt.Rows.Count == 0)
            {
                return;
            }
            if (null == colNames)
            {
                colNames = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    colNames[i] = dt.Columns[i].ColumnName;
                }
            }
            try
            {
                sheet = ((Excel.Worksheet)workbook.Worksheets.get_Item(sheetIndex));
                int iRowCount = dt.Rows.Count;
                int iColCount = colNames.Length;
                object[,] arrValues = new object[iRowCount, iColCount];
                //多线程处理
                int iThreadCount = 1;
                Task[] tasks = new Task[iThreadCount];
                for (var i = 0; i < iThreadCount; i++)
                {
                    tasks[i] = new Task(oIndex =>
                    {
                        int threadIndex = (int)oIndex;
                        for (var k = threadIndex; k < iRowCount; k = k + iThreadCount)
                        {
                            for (int m = 0; m < iColCount; m++)
                            {
                                arrValues[k, m] = dt.Rows[k][colNames[m]];
                            }
                        }
                    }, i);
                    tasks[i].Start();
                }
                Task.WaitAll(tasks);
                //for (int i = 0; i < iRowCount; i++)
                //{
                //    for (int k = 0; k < iColCount; k++)
                //    {
                //        arrValues[i, k] = dt.Rows[i][k];
                //    }
                //}

                rng = sheet.get_Range(NumberToString26(startColIndex) + startRowIndex, NumberToString26(iColCount + startColIndex - 1) + (iRowCount + startRowIndex - 1));
                rng.Value2 = arrValues;
            }
            catch { throw; }
            finally
            {
            }
        }

        /// <summary>
        /// 字母表
        /// </summary>
        protected const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        /// <summary>
        /// 10进制数字转26进制字母表文本（Excel）
        /// </summary>
        /// <returns></returns>
        public static string NumberToString26(int inputNum)
        {
            int num = inputNum;
            System.Text.StringBuilder sbResult = new System.Text.StringBuilder();
            while (num > 0)
            {
                int yushu = num % ALPHABET.Length;
                yushu = yushu == 0 ? 26 : yushu;
                sbResult.Insert(0, ALPHABET[yushu - 1]);
                num = (num - yushu) / 26;
            }
            return sbResult.ToString();
        }

        /// <summary>
        /// 26进制字母表字符串转10进制数字
        /// </summary>
        /// <returns></returns>
        public static int String26ToNumber(string inputStr)
        {
            int iResult = 0;
            inputStr = inputStr == null ? "" : inputStr;
            inputStr = inputStr.ToUpper();
            for (int i = inputStr.Length - 1, k = 1; i >= 0; i--, k *= ALPHABET.Length)
            {
                if (ALPHABET.IndexOf(inputStr[i]) < 0)
                {
                    return 0;
                }
                iResult += (ALPHABET.IndexOf(inputStr[i]) + 1) * k;
            }
            return iResult;
        }

        public void InsertHead(System.Data.DataTable dt, int iRow, int iCol)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[iRow, i + iCol] = dt.Columns[i].ToString();
            }
        }
        #endregion

        #region 插入图片
        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="Filename">图片名称地址</param>
        /// <param name="left">距离左边的距离</param>
        /// <param name="top">距离顶部的距离</param>
        /// <param name="Height">图片的高</param>
        /// <param name="Width">图片的宽</param>
        public void InsertPictures(string Filename, int left, int top, int Height, int Width)
        {
            worksheet.Shapes.AddPicture(Filename, MsoTriState.msoFalse, MsoTriState.msoTrue, left, top, Width, Height);
        }
        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        /// <param name="FileName">文件名</param>
        public void InsertPictures(string cellname, string FileName)
        {
            Excel.Names names = ((Excel._Workbook)workbook).Names;
            foreach (Excel.Name item in names)
            {
                if (item.Name.ToLower() != cellname.ToLower())//没找到名称时继续找
                {
                    continue;
                }

                //找到后就开始赋值
                string cellnamevalue = item.Value;//=检测报告!B$1$
                if (string.IsNullOrEmpty(cellnamevalue))//名称没有关联到单元格直接停止查找
                {
                    break;
                }
                string Formula = cellnamevalue.Remove(0, 1);
                string sheetName = Formula.Split('!')[0];
                string col = Formula.Split('$')[1];
                string row = Formula.Split('$')[2];
                //获取sheet
                Excel._Worksheet sheet = (Excel._Worksheet)Get_worksheet(sheetName);
                sheet.Application.ActiveWindow.Zoom = 100;//显示原样大小
                float left = Convert.ToSingle(sheet.Range[col + row, Type.Missing].Left);
                float top = Convert.ToSingle(sheet.Range[col + row, Type.Missing].Top);
                float Width = -1;//-1为表示根据具体的图片得到宽度。
                float Height = -1;
                //根据格子的大小添加图片
                //float Width = Convert.ToSingle(sheet.Range[col + row, Type.Missing].Width);
                //float Height = Convert.ToSingle(sheet.Range[col + row, Type.Missing].Height);
                //有相同名称的图片删除
                for (int i = 1; i <= worksheet.Shapes.Count; i++)
                {
                    if (worksheet.Shapes.Item(i).Name.ToLower() == cellname.ToLower())
                    {
                        worksheet.Shapes.Item(i).Delete();
                    }
                }
                worksheet.Shapes.AddPicture(FileName, MsoTriState.msoFalse, MsoTriState.msoTrue, left, top, Width, Height);

                worksheet.Shapes.Item(worksheet.Shapes.Count).Name = cellname;//设置图片名称
                sheet = null;
                break;
            }
            names = null;
        }
        /// <summary>
        /// 移除图片
        /// </summary>
        /// <param name="cellname">单元格名称</param>
        /// <param name="FileName">文件名</param>
        public void RemovePictures(string cellname)
        {
            Excel.Names names = ((Excel._Workbook)workbook).Names;
            foreach (Excel.Name item in names)
            {
                if (item.Name.ToLower() != cellname.ToLower())//没找到名称时继续找
                {
                    continue;
                }

                //找到后就开始赋值
                string cellnamevalue = item.Value;//=检测报告!B$1$
                if (string.IsNullOrEmpty(cellnamevalue))//名称没有关联到单元格直接停止查找
                {
                    break;
                }
                string Formula = cellnamevalue.Remove(0, 1);
                string sheetName = Formula.Split('!')[0];
                string col = Formula.Split('$')[1];
                string row = Formula.Split('$')[2];
                //获取sheet
                Excel._Worksheet sheet = (Excel._Worksheet)Get_worksheet(sheetName);
                sheet.Application.ActiveWindow.Zoom = 100;//显示原样大小
                float left = Convert.ToSingle(sheet.Range[col + row, Type.Missing].Left);
                float top = Convert.ToSingle(sheet.Range[col + row, Type.Missing].Top);
                for (int i = 1; i <= sheet.Shapes.Count; i++)
                {
                    Microsoft.Office.Interop.Excel.Shape shape = sheet.Shapes.Item(i);
                    if (shape.Name.ToLower() == cellname.ToLower())
                    {
                        shape.Delete();
                    }
                    shape = null;
                }

                sheet = null;
                break;
            }
            names = null;
        }
        #endregion

        #region 打印
        public void Print(string sheetName)
        {
            foreach (Excel.Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name == sheetName)
                {
                    Print(sheet);
                    break;
                }
            }
        }
        public void Print(int sheetIndex)
        {
            Print((Excel.Worksheet)workbook.Worksheets.get_Item(sheetIndex));
        }
        private void Print(Excel.Worksheet sheet)
        {
            sheet.PrintOutEx();
        }
        #endregion

        #region 关闭操作
        /// <summary>
        /// 把Excel文件转换成pdf文件
        /// </summary>
        /// <param name="targetPath">转换完成后的文件的路径和文件名名称</param>
        /// <param name="sheetname">要转换页的名字</param>
        /// <returns></returns>
        public bool ExcelToPdf(string targetPath, string sheetname)
        {
            bool result = false;
            Excel.XlFixedFormatType xlFixedFormatType = Excel.XlFixedFormatType.xlTypePDF; ;//导出文件所使用的格式 可以是xlTypePDF或xlTypeXPS
            Excel.XlFixedFormatQuality xlFixedFormatQuality = Excel.XlFixedFormatQuality.xlQualityStandard;//1.xlQualityStandard:质量标准，2.xlQualityMinimum;最低质量
            bool includeDocProperties = false;//设置为True ，以指示该文档应包含属性，或将其设置为False ，以指示省略它们。
            bool openAfterPublish = false;//如果设置为True在发布后在查看器中显示文件。如果设置为False将文件发布但不是显示。
            bool IgnorePrintAreas = true;//如果设置为True，将忽略在发布时设置的任何打印区域。如果设置为False，则使用在发布时设置的打印区域

            int From = Get_worksheetIndex(sheetname);//发布的起始页码。如果省略此参数，则从起始位置开始发布
            int To = From;//发布的终止页码。如果省略此参数，则发布至最后一页
            try
            {
                workbook.ExportAsFixedFormat(xlFixedFormatType, targetPath, xlFixedFormatQuality, includeDocProperties, IgnorePrintAreas, From, To, openAfterPublish, Type.Missing);
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 把Excel文件转换成pdf文件
        /// </summary>
        /// <param name="targetPath">转换完成后的文件的路径和文件名名称</param>
        /// <param name="sheetIndex">要转换页的索引号（1开始）</param>
        /// <returns></returns>
        public bool ExcelToPdf(string targetPath, int sheetIndex = 1)
        {
            string sheetName = ((Excel.Worksheet)workbook.Sheets[sheetIndex]).Name;
            return ExcelToPdf(targetPath, sheetName);
        }

        /// <summary>
        /// 保存Excel文档
        /// </summary>
        /// <param name="FileName">文件名称(文件名为null时按照打开的路径保存)</param>
        /// <returns>true:保存成功, false:失败</returns>
        public bool Save(string FileName)
        {
            FileName = string.IsNullOrEmpty(FileName) ? this.SaveName : FileName;
            try
            {
                FApp.DisplayAlerts = Config.DisplayAlerts;
                object FileFormat = null;
                if (FileName.EndsWith(".xlsx"))
                {
                    FileFormat = Excel.XlFileFormat.xlExcel12;
                }
                else if (FileName.EndsWith(".xls"))
                {
                    FileFormat = Excel.XlFileFormat.xlExcel8;
                }
                else
                {
                    FileFormat = workbook.FileFormat;
                }
                if (FileName.ToString() == SaveName || FileName == null)
                {
                    workbook.Save();
                }
                else
                {
                    if (File.Exists(FileName))
                    {
                        File.Delete(FileName);
                    }
                    workbook.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 保存Excel文档
        /// </summary>
        /// <param name="FileName">文件名称</param>
        /// <returns>true:保存成功, false:失败</returns>
        public bool Save(int WorkBookIndex, string FileName)
        {
            object FileFormat = null;
            if (FileName.EndsWith(".xlsx"))
            {
                FileFormat = Excel.XlFileFormat.xlExcel12;
            }
            else if (FileName.EndsWith(".xls"))
            {
                FileFormat = Excel.XlFileFormat.xlExcel8;
            }
            else
            {
                FileFormat = workbook.FileFormat;
            }
            try
            {
                FApp.DisplayAlerts = Config.DisplayAlerts;
                workbook = workbooks[WorkBookIndex];
                workbook.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 保存Excel文档
        /// </summary>
        /// <param name="FileName">文件名称</param>
        /// <returns>true:保存成功, false:失败</returns>
        public bool SaveCopy(object FileName)
        {
            try
            {
                worksheet.Cells.EntireColumn.AutoFit();
                FApp.DisplayAlerts = Config.DisplayAlerts;
                workbook.SaveCopyAs(FileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 放弃修改并退出程序
        /// </summary>
        public void Dispose()
        {
            if (FApp != null)
            {
                FApp.DisplayAlerts = Config.DisplayAlerts;
                if (FApp.Workbooks != null)
                {
                    for (int i = 1; i <= FApp.Workbooks.Count; i++)
                    {
                        FApp.Workbooks.get_Item(i).Close(false, Type.Missing, Type.Missing);
                    }
                    FApp.Workbooks.Close();
                }
                FApp.Quit();
            }
            if (aRg != null)
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(aRg);
                aRg = null;

            }
            if (worksheet != null)
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(worksheet);
                worksheet = null;
            }
            if (workbook != null)
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbook);
                workbook = null;
            }
            if (workbooks != null)
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbooks);
                workbooks = null;
            }
            if (FApp != null)
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(FApp);
                FApp = null;
            }

            GC.Collect();
        }
    }
}