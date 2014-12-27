using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel
{
    public class ImportExcel
    {
        private static string GetConnectionString(string pFilePath, bool headerRow = true)
        {
            string str = string.Empty;
            string extension = Path.GetExtension(pFilePath);
            string format;
            if (extension == ".xls")
            {
                format = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties= \"Excel 8.0;HDR={1};\"";
            }
            else
            {
                if (!(extension == ".xlsx") && !(extension == ".xlsm"))
                    throw new ArgumentOutOfRangeException("Excel file extenstion is not known.");
                format = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR={1}\"";
            }
            return string.Format(format, (object)pFilePath, headerRow ? (object)"YES" : (object)"NO");
        }

        public static DataTable ExcelToDatatable(string path, string SheetName = "Sheet", string range = "A1:X65000")
        {
            DataTable dataTable = new DataTable();
            using (OleDbConnection oleDbConnection = new OleDbConnection(GetConnectionString(path, true)))
            {
                try
                {
                    oleDbConnection.Open();
                    string str = string.Format("SELECT * FROM [{0}${1}]", (object)SheetName, (object)range);
                    OleDbCommand selectCommand = new OleDbCommand();
                    selectCommand.CommandText = str;
                    selectCommand.Connection = oleDbConnection;
                    ((DbDataAdapter)new OleDbDataAdapter(selectCommand)).Fill(dataTable);
                }
                catch (Exception ex)
                {
                    oleDbConnection.Close();
                    throw ex;
                }
            }
            return dataTable;
        }
    }
}
