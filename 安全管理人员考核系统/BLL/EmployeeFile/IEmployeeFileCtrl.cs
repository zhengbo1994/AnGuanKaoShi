using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BLL
{
    public interface IEmployeeFileCtrl
    {
        void SaveEmployeeFile(IList<Biz_EmployeeFile> employeeFiles);
        void DeleteEmployeeFile(IList<Biz_EmployeeFile> employeeFiles);
        List<Biz_EmployeeFile> GetEmployeeFiles(int employeeId);
        List<Biz_EmployeeFile> GetEmployeeFiles(int employeeId, string fileType);
        List<Biz_EmployeeFile> GetEmployeeFiles(int employeeId, string fileType, string fileName);
    }
}
