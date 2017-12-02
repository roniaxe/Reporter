using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace Reporter.Utils
{
    public static class TableToExcelManager
    {
        public static string ExportToExcel(List<DataGridView> tables, string env, string db)
        {
            // Creating a Excel object. 
            _Application excel = new Application();
            _Workbook workbook = excel.Workbooks.Add(Type.Missing);
            string fileName = null;
            try
            {
                for (int x = 0; x < tables.Count; x++)
                {
                    _Worksheet worksheet = workbook.Sheets.Add();
                    worksheet.Name = $@"Errors Sheet - {x+1}";
                    var cellRowIndex = 1;
                    var cellColumnIndex = 1;

                    //Loop through each row and read value from each column. 
                    for (var i = 0; i < tables[x].Rows.Count; i++)
                     {
                     for (var j = 0; j < tables[x].Columns.Count; j++)
                     {
                      // Excel index starts from 1,1. As first Row would have the Column headers, adding a condition check. 
                     if (cellRowIndex == 1)
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] = tables[x].Columns[j].HeaderText;
                                            else
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                                    tables[x].Rows[i].Cells[j].Value?.ToString();
                                            cellColumnIndex++;
                                        }
                                        cellColumnIndex = 1;
                                        cellRowIndex++;
                                    }
                                    worksheet.Columns.AutoFit();
                                    worksheet.Range["A1:Z100"].WrapText = false;
                                    worksheet.Cells[1, 1].EntireRow.Font.Bold = true;                
                }
                
                


                //Getting the location and file name of the excel to save from user. 
                var saveDialog =
                    new SaveFileDialog
                    {
                        Filter = @"Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        FilterIndex = 2,
                        FileName = 
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop) 
                        + @"\Daily_Report_" + env
                        + "_" + db
                        + "_" + DateTime.Today.ToString("M") + ".xlsx"
                    };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveDialog.FileName);
                    MessageBox.Show(@"Export Successful");
                }
                fileName = saveDialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                excel.Quit();
            }
            return fileName;
        }
    }
}
