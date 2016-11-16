namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Interfaces;

    public class ExportEmployeeInfoService : IExportEmployeeInfoService
    {
        private const string TemplatePath = @"~\ExcelTemplates\EmployeeBasicInfoTemplate.xls";

        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int TotalColumns = 11;

        public void Export(IEnumerable<Employee> employees)
        {
            string fileName = "Employee_Infomation",
                templatePath = HttpContext.Current.Server.MapPath(TemplatePath);

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Employee Infomation");

            // Fill report title.
            writer.CopyRows(0, 2);

            // Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRow(employees, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRow(IEnumerable<Employee> employees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRow(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRow(
            Employee employee,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecord(employee, ref rowIndex);
            writer.WriteRow(RowStartBody, record);
        }

        private object CreateRecord(
            Employee employee,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                Username = employee.Username,
                FullName = employee.FullName,
                NickName = employee.NickName,
                Section = employee.Group != null ? employee.Group.GroupName : string.Empty,
                JobTitle = employee.JobTitle.JobTitleName,
                Category = employee.Category,
                JoinedDate = employee.JoinedDate.ToString(Constants.DateFormat),
                Email = employee.Email,
                Roles = string.Join(", ", employee.Roles.Select(x => x.RoleCode))
            };
            rowIndex++;

            return record;
        }
    }
}