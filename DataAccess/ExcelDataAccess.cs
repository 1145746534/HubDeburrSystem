using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HubDeburrSystem.DataAccess
{
    public class ExcelDataAccess
    {
        /// <summary>
        /// 判断文件是否打开
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileOpened(string filePath)//判断文件是否打开
        {
            bool result = false;
            try
            {
                FileStream fs = File.OpenWrite(filePath);
                fs.Close();
            }
            catch
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取Excel文件内所有表名
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string[] GetSheetName(string filePath)//获取Excel文件内所有表名
        {
            int sheetNumber = 0;
            var File = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (filePath.IndexOf(".xlsx") > 0)//2007
            {
                var xlsx = new XSSFWorkbook(File);
                sheetNumber = xlsx.NumberOfSheets;
                string[] sheetNames = new string[sheetNumber];
                for (int i = 0; i < sheetNumber; i++)
                {
                    sheetNames[i] = xlsx.GetSheetName(i);
                }
                return sheetNames;

            }
            else if (filePath.IndexOf(".xls") > 0)//2003
            {
                var xls = new HSSFWorkbook(File);
                sheetNumber = xls.NumberOfSheets;
                string[] sheetNames = new string[sheetNumber];
                for (int i = 0; i < sheetNumber; i++)
                {
                    sheetNames[i] = xls.GetSheetName(i);
                }
                return sheetNames;
            }
            else
            { return null; }

        }

        /// <summary>
        /// 将DataGridView的全部内容复制到DataTable中
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <returns></returns>
        public static DataTable DataGridViewToDataTable(DataGridView dataGridView)//将DataGridView的全部内容复制到DataTable中
        {
            DataTable dTable = new DataTable();
            for (int i = 0; i < dataGridView.Columns.Count; i++) //循环DataGridView中的栏位
            {
                DataColumn dColumn = new DataColumn(dataGridView.Columns[i].Name.ToString());
                dTable.Columns.Add(dColumn); //将DataGridView中栏位的标题写⼊到DataTable中
            }
            for (int k = 0; k < dataGridView.Rows.Count; k++) //循环DataGridView中的资料⾏
            {
                DataRow DRow = dTable.NewRow();
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (dataGridView.Rows[k].Cells[j].Value != null)
                        DRow[j] = dataGridView.Rows[k].Cells[j].Value.ToString();
                    else
                        DRow[j] = "_";
                }
                dTable.Rows.Add(DRow);
            }
            return dTable;
        }

        /// <summary>
        /// 读取Excel转换成DataTable
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="IsColumnName"></param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string filePath, string sheetName, bool isColumnName)//读取Excel转换成DataTable
        {
            DataTable dataTable = null;
            FileStream files = null;
            DataColumn column = null;
            DataRow row = null;
            IWorkbook workBook = null;
            ISheet sheet = null;
            IRow iRows = null;
            ICell iCells = null;
            int startRow = 0;
            try
            {
                using (files = File.OpenRead(filePath))
                {
                    if (filePath.IndexOf(".xlsx") > 0)// 版本后缀控制
                        workBook = new XSSFWorkbook(files);
                    else if (filePath.IndexOf(".xls") > 0)// 版本后缀控制
                        workBook = new HSSFWorkbook(files);

                    if (workBook != null)
                    {
                        sheet = workBook.GetSheet(sheetName);//读取sheet
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int RowCount = sheet.LastRowNum;//总行数
                            if (RowCount > 0)
                            {
                                IRow FirstRow = sheet.GetRow(0);//第一行
                                int CellCount = FirstRow.LastCellNum;//列数

                                //构建datatable的列
                                if (!isColumnName)//不需要列标题
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取
                                    for (int i = FirstRow.FirstCellNum; i < CellCount; ++i)
                                    {
                                        iCells = FirstRow.GetCell(i);
                                        if (iCells != null)
                                        {
                                            if (iCells.StringCellValue != null)
                                            {
                                                column = new DataColumn(iCells.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else//需要列标题
                                {
                                    for (int i = FirstRow.FirstCellNum; i < CellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行
                                for (int i = startRow; i <= RowCount; ++i)
                                {
                                    iRows = sheet.GetRow(i);
                                    if (iRows == null) continue;

                                    row = dataTable.NewRow();
                                    for (int j = iRows.FirstCellNum; j < CellCount; ++j)
                                    {
                                        iCells = iRows.GetCell(j);
                                        if (iCells == null)
                                        {
                                            row[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                            switch (iCells.CellType)
                                            {
                                                case CellType.Blank:
                                                    row[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short Format = iCells.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                    if (Format == 14 || Format == 31 || Format == 57 || Format == 58)
                                                        row[j] = iCells.DateCellValue;
                                                    else
                                                        row[j] = iCells.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    row[j] = iCells.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(row);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (files != null)
                {
                    files.Close();
                }
                return null;
            }
        }

        /// <summary>
        /// 将DataTable导出成Excel
        /// </summary>
        /// <param name="DTable"></param>
        /// <param name="SavePathPath"></param>
        /// <returns></returns>
        public static bool DataTableToExcel(DataTable dTable, string saveFilePath, out string exportResult)//将DataTable导出成Excel
        {
            FileStream files = null;
            IRow row = null;
            ISheet sheet = null;
            ICell column = null;
            try
            {
                if (dTable != null && dTable.Rows.Count > 0)
                {
                    string FileType = Path.GetExtension(saveFilePath).ToLower();
                    dynamic data;
                    if (FileType == ".xlsx")
                    {
                        XSSFWorkbook Xlsx = new XSSFWorkbook();
                        data = Xlsx;
                    }
                    else
                    {
                        HSSFWorkbook Xls = new HSSFWorkbook();
                        data = Xls;
                    }
                    sheet = data.CreateSheet("Sheet0");//创建一个名称为Sheet0的表
                    int RowCount = dTable.Rows.Count;//行数
                    int ColumnCount = dTable.Columns.Count;//列数

                    //设置列头
                    row = sheet.CreateRow(0);//excel第一行设为列头
                    for (int c = 0; c < ColumnCount; c++)
                    {
                        column = row.CreateCell(c);
                        column.SetCellValue(dTable.Columns[c].ColumnName);
                    }

                    //设置每行每列的单元格,
                    for (int i = 0; i < RowCount; i++)
                    {
                        row = sheet.CreateRow(i + 1);
                        for (int j = 0; j < ColumnCount; j++)
                        {
                            column = row.CreateCell(j);//excel第二行开始写入数据
                            column.SetCellValue(dTable.Rows[i][j].ToString());
                        }
                    }
                    using (files = File.OpenWrite(saveFilePath))
                    {
                        data.Write(files);//向打开的这个xls文件中写入数据
                    }
                }
                exportResult = "导出成功！";
                return true;
            }
            catch (Exception ex)
            {
                exportResult = ex.Message;
                return false;
            }
            finally
            {
                if (files != null) files.Close();
            }
        }

        /// <summary>
        /// 将List转换成DataTable
        /// </summary>
        /// <typeparam name="T">List数据格式</typeparam>
        /// <param name="List">List数据</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IEnumerable<T> list)//将List转换成DataTable
        {
            PropertyInfo[] modelItemType = typeof(T).GetProperties();
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(modelItemType.Select(Columns => new DataColumn(Columns.Name, Columns.PropertyType)).ToArray());
            if (list.Count() > 0)
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    ArrayList TempList = new ArrayList();
                    foreach (PropertyInfo pi in modelItemType)
                    {
                        object obj = pi.GetValue(list.ElementAt(i), null);
                        TempList.Add(obj);
                    }
                    object[] DataRow = TempList.ToArray();
                    dataTable.LoadDataRow(DataRow, true);
                }
            }
            return dataTable;
        }
    }
}
