using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Core.Utilities
{
    public static class ExcelAdaptor
    {
        public static List<ExcelUserDto> GetExcel()
        {
            List<ExcelUserDto> AllUser = new List<ExcelUserDto>();
            string FileName = "C:\\voting-users.xlsx";
            Microsoft.Office.Interop.Excel.Application Excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook WorkBook;
            Microsoft.Office.Interop.Excel.Worksheet WorKSheet;
            Microsoft.Office.Interop.Excel.Range CellRange;
            string conn = string.Empty;
            string fileExt = Path.GetExtension(FileName);
            WorkBook = Excel.Workbooks.Open(FileName);
            WorKSheet = WorkBook.Sheets[1];
            CellRange = WorKSheet.UsedRange;
            int rowCount = CellRange.Rows.Count;
            int colCount = CellRange.Columns.Count;
            try
            {
                for (int i = 2; i <= rowCount; i++)
                {
                    ExcelUserDto NewUser = new ExcelUserDto();

                    NewUser.FirstName = (string)(CellRange.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value2;
                    NewUser.LastName = (string)(CellRange.Cells[i, 3] as Microsoft.Office.Interop.Excel.Range).Value2;
                    NewUser.Group = (string)(CellRange.Cells[i, 4] as Microsoft.Office.Interop.Excel.Range).Value2;
                    NewUser.NationalCode = (string)(CellRange.Cells[i, 6] as Microsoft.Office.Interop.Excel.Range).Value2;
                    NewUser.Email = (string)(CellRange.Cells[i, 7] as Microsoft.Office.Interop.Excel.Range).Value2;
                    NewUser.PhoneNumber = (string)(CellRange.Cells[i, 8] as Microsoft.Office.Interop.Excel.Range).Value2;
                    NewUser.UserName = (string)(CellRange.Cells[i, 9] as Microsoft.Office.Interop.Excel.Range).Value2;
                    NewUser.Password = (string)(CellRange.Cells[i, 10] as Microsoft.Office.Interop.Excel.Range).Value2;
                    AllUser.Add(NewUser);
                }
            }
            catch (Exception E)
            {

            }

            return AllUser;
        }
    }

    public class ExcelUserDto
    {
        public string Email;
        public string UserName;
        public string Password;
        public string FirstName;
        public string LastName;
        public string PhoneNumber;
        public string NationalCode;
        public string Group;
        public string Id { get; set; }

    }
}
