using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace HVT.Utility
{
    public class Excel
    {
        public Microsoft.Office.Interop.Excel.Application APP = null;
        public Microsoft.Office.Interop.Excel.Workbook WB = null;
        public Microsoft.Office.Interop.Excel.Worksheet WS = null;
        public Microsoft.Office.Interop.Excel.Range Range = null;

        public Excel()
        {

        }
        public Excel(DataGrid dataGrid)
        {
            this.APP = new Microsoft.Office.Interop.Excel.Application();
            this.Open("D:\\MyExcel.xlsx", 1);
            this.CreateHeader(dataGrid);
            this.InsertData(dataGrid);
            this.Close();
        }

        private void Open(string Location, int workSheet)
        {
            try
            {
                this.WB = this.APP.Workbooks.Open(Location);
                this.WS = (Microsoft.Office.Interop.Excel.Worksheet)WB.Sheets[workSheet];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //return this.WS;
        }
        private void CreateHeader(DataGrid dataGrid)
        {
            try
            {
                for (int ind = 0; ind < dataGrid.Columns.Count; ind++)
                {
                    this.WS.Cells[1, ind + 1] = dataGrid.Columns[ind].Header.ToString();
                    Console.WriteLine(dataGrid.Columns[ind].Header.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void InsertData(DataGrid dataGrid)
        {
            //int ind = 2;
            //foreach (var field in dataGrid.Items)
            //{
            //    for (int ind1 = 1; ind1 <= dataGrid.Columns.Count; ind1++)
            //    {
            //        this.WS.Cells[ind, ind1] = ((DataRowView)dataGrid.Items[ind]).Row.ItemArray[ind1].ToString();
            //        Console.WriteLine(((DataRowView)dataGrid.Items[ind]).Row.ItemArray[ind1].ToString());
            //    }
            //    ind++;
            //}
            //dataGrid.SelectAllCells();
            //int rowIndex = 0;
            //foreach (DataRowView row in dataGrid.Row)
            //{
            //    for (int ind1 = 0; ind1 < dataGrid.Columns.Count; ind1++)
            //    {
            //        try
            //        {
            //            var content = dataGrid.Columns[ind1].GetCellContent(row) as TextBlock;
            //            this.WS.Cells[rowIndex + 2, ind1 + 1] = row.Row.ItemArray[ind1].ToString();
            //            Console.WriteLine(row.Row.ItemArray[ind1].ToString());
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(rowIndex);
            //        }
            //        //Console.WriteLine(((DataRowView)dataGrid.Items[i]).Row.ItemArray[ind1].ToString());
            //    }
            //    rowIndex++;
            //}
            //for (int i = 0; i < dataGrid.Items.Count - 1; i++)
            //{
            //    for (int j = 0; j < dataGrid.Columns.Count; j++)
            //    {
            //        DataGridCell cell = GetCell(dataGrid, i, j);
            //        TextBlock tb = cell.Content as TextBlock;
            //        this.WS.Cells[i + 2, j + 1] = tb.Text;
            //    }
            //}
        }

        private void Close()
        {
            if (this.APP.ActiveWorkbook != null)
                this.APP.ActiveWorkbook.Save();
            if (this.APP != null)
            {
                if (this.WB != null)
                {
                    if (this.WS != null)
                        Marshal.ReleaseComObject(this.WS);
                    this.WB.Close(false, Type.Missing, Type.Missing);
                    Marshal.ReleaseComObject(this.WB);
                }
                this.APP.Quit();
                Marshal.ReleaseComObject(this.APP);
            }
        }
    }
}
