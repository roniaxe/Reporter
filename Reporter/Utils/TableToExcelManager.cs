using System;
using System.Collections.Generic;
using System.Threading;
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
                foreach (var table in tables)
                {
                    _Worksheet worksheet = workbook.Sheets.Add();
                    worksheet.Name = table.Tag as string;

                    // Creating Sheet Headers
                    var headerCounter = 1;
                    foreach (DataGridViewColumn column in table.Columns)
                    {
                        worksheet.Cells[1, headerCounter] = column.HeaderText;
                        headerCounter++;
                    }

                    var cellRowIndex = 2;
                    var cellColumnIndex = 1;

                    //Loop through each row and read value from each column. 
                    for (var i = 0; i < table.Rows.Count; i++)
                    {
                        for (var j = 0; j < table.Columns.Count; j++)
                        {
                            worksheet.Cells[cellRowIndex, cellColumnIndex] = table.Rows[i].Cells[j].Value?.ToString();
                            cellColumnIndex++;
                        }

                        cellColumnIndex = 1;
                        cellRowIndex++;
                    }

                    worksheet.Columns.AutoFit();
                    worksheet.Range["A1:Z100"].WrapText = false;
                    worksheet.Cells[1, 1].EntireRow.Font.Bold = true;
                }

                var t = new Thread(() =>
                {
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
                        MessageBox.Show(@"Export Successful", @"Report Generator", MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                            , MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }

                    fileName = saveDialog.FileName;
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
                Console.WriteLine(fileName);
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