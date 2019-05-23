using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DAL;

namespace BLL
{
    public class EmployeeFileCtrl : IEmployeeFileCtrl
    {
        private Uow uow;

        public EmployeeFileCtrl()
        {
            if (uow == null)
            {
                uow = new Uow();
            }
        }
        public void SaveEmployeeFile(IList<Biz_EmployeeFile> employeeFiles)
        {
            foreach (Biz_EmployeeFile file in employeeFiles)
            {
                uow.Biz_EmployeeFile.Add(file);
            }
            uow.Commit();
        }

        public void DeleteEmployeeFile(IList<Biz_EmployeeFile> employeeFiles)
        {
            throw new NotImplementedException();
        }

        public List<Biz_EmployeeFile> GetEmployeeFiles(int employeeId)
        {
            throw new NotImplementedException();
        }

        public List<Biz_EmployeeFile> GetEmployeeFiles(int employeeId, string fileType)
        {
            throw new NotImplementedException();
        }

        public List<Biz_EmployeeFile> GetEmployeeFiles(int employeeId, string fileType, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
