using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Global;
using Aspose.Cells;
using Range = Aspose.Cells.Range;

namespace ApeVolo.Common.Helper.Excel;

/// <summary>
/// Excel操作类
/// </summary>
public class ExcelHelper
{
    public ExcelHelper()
    {
    }

    #region 导出

    #region 实体

    /// <summary>
    /// 导出-泛型
    /// </summary>
    /// <param name="exportRows">列对应标题</param>
    /// <param name="fileName">导出文件名</param>
    /// <param name="isSerialNumber">是否显示序号</param>
    /// <param name="dateFormat">日期格式</param> 
    /// <param name="rowMerge">合并行</param> 
    /// <param name="isColDatasConvert">是否通过列数据源转换数据</param> 
    /// <returns>绝对路径</returns>
    public static string ExportData(List<ExportRowModel> exportRows, string fileName = "",
        bool isSerialNumber = false,
        string dateFormat = "yyyy-MM-dd HH:mm", Dictionary<string, List<string>> rowMerge = null,
        bool isColDatasConvert = true)
    {
        if (exportRows == null || exportRows.Count == 0)
        {
            throw new BadRequestException("没有数据可导出！");
        }

        var columns = exportRows[0].exportColumnModels;
        string newFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + ".xlsx";
        string savePath = Path.Combine(AppSettings.WebRootPath, "ExportFile");
        //string filePath = Path.Combine(_contentRoot, "wwwroot", "resources", "ip", "ip2region.db");
        var dataFolder = DateTime.Now.Date.ToString("yyyyMMdd");
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        //savePath += dataFolder;
        savePath = Path.Combine(savePath, dataFolder);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        Workbook workbook = new Workbook(); //工作簿  
        Worksheet sheet = workbook.Worksheets[0]; //工作表 
        Cells cells = sheet.Cells; //单元格 

        Style styleTitle = GetHeaderStyle(workbook);
        Style styleContent = GetContentStyle(workbook);
        Style styleFoot = GetFootStyle(workbook);


        //var startColumn = 0; //是否加序号列
        if (isSerialNumber)
        {
            //startColumn = 1;
            cells[0, 0].PutValue("#");
        }

        //列数,包括编码要转名称的
        var columnCount = columns.Count;
        //创建头部区域
        Range headRange = sheet.Cells.CreateRange(0, 0, 1, isSerialNumber ? columnCount + 1 : columnCount);
        headRange.SetStyle(styleTitle);
        //设置列头
        foreach (var col in columns)
        {
            cells[0, col.Point].PutValue(col.Key);
        }

        int headRow = 1;


        for (int index = 0; index < exportRows.Count; index++)
        {
            if (isSerialNumber)
            {
                cells[headRow + index, 0].SetStyle(styleContent);
                cells[headRow + index, 0].PutValue(index + 1);
            }


            foreach (var colModel in exportRows[index].exportColumnModels)
            {
                cells[headRow + index, colModel.Point].PutValue(colModel.Value);
            }

            //行合并 
            if (rowMerge != null)
            {
            }
        }

        Range contentRange = sheet.Cells.CreateRange(headRow, 0, exportRows.Count,
            isSerialNumber ? columnCount + 1 : columnCount);
        contentRange.SetStyle(styleContent);
        //自适应列宽
        foreach (var columnProp in columns)
        {
            sheet.AutoFitColumn(columnProp.Point);
        }

        workbook.Save(Path.Combine(savePath, newFileName));
        // return savePath + newFileName;
        return Path.Combine(savePath, newFileName);
    }

    #endregion

    #region Table

    /// <summary>
    /// 导出 -table
    /// </summary> 
    /// <param name="listData">数据集</param>
    /// <param name="columns">列对应标题</param>
    /// <param name="sumData">合计行</param>
    /// <param name="relativePath">相对路径</param>
    /// <param name="fileName">导出文件名</param>
    /// <param name="isSerialNumber">是否显示序号</param>
    /// <param name="dateFormat"></param>
    /// <param name="boolToText"></param>
    /// <param name="rowMerge"></param>
    /// <returns>绝对路径</returns>
    public static string ExportDataToTable(DataTable listData, List<ExportColumnModel> columns,
        DataRow sumData, out string relativePath, string fileName = "", bool isSerialNumber = true,
        string dateFormat = "yyyy-MM-dd HH:mm", bool boolToText = false,
        Dictionary<string, List<string>> rowMerge = null)
    {
        if (listData == null || listData.Rows.Count == 0)
        {
            throw new System.Exception("没有数据导出");
        }

        string newFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + ".xlsx";
        string savePath = AppDomain.CurrentDomain.BaseDirectory + "ExportFile/";
        var dataFolder = DateTime.Now.Date.ToString("yyyyMMdd") + "/";
        relativePath = "ExportFile/" + dataFolder + newFileName;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        savePath += dataFolder;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        Workbook workbook = new Workbook(); //工作簿  
        Worksheet sheet = workbook.Worksheets[0]; //工作表 
        Cells cells = sheet.Cells; //单元格 

        Style styleTitle = GetHeaderStyle(workbook);
        Style styleContent = GetContentStyle(workbook);
        Style styleFoot = GetFootStyle(workbook);

        var listProp = new List<string>();
        //var colNum = 0;

        var startColumn = 0; //是否加序号列
        if (isSerialNumber)
        {
            startColumn = 1;
            cells[0, 0].PutValue("#");
            cells.SetColumnWidth(0, 10);
        }

        Range headRange = sheet.Cells.CreateRange(0, 0, 1,
            isSerialNumber ? columns.Count + 1 : columns.Count);
        headRange.SetStyle(styleTitle);
        var colNum = 0;
        var colNumDic = new Dictionary<string, int>();
        foreach (var col in columns)
        {
            cells.SetColumnWidth(colNum + startColumn, 20);
            cells[0, colNum + startColumn].PutValue(col.Value);
            colNumDic.Add(col.Key, colNum + startColumn);
            colNum++;
            listProp.Add(col.Key);
        }

        int headRow = 1;

        int rowNum = listData.Rows.Count + headRow; //表格行数 

        var rowMeageValue = ""; //行值 
        for (int index = 0; index < listData.Rows.Count; index++)
        {
            var item = listData.Rows[index];

            if (isSerialNumber)
            {
                cells[headRow + index, 0].SetStyle(styleContent);
                cells[headRow + index, 0].PutValue(index + 1);
            }

            for (int i = 0; i < listProp.Count; i++)
            {
                SetCell(item, cells[headRow + index, i + startColumn], listProp[i], dateFormat);
            }

            //行合并 
            if (rowMerge != null)
            {
                var rowValue = rowMerge.FirstOrDefault().Key;
                var columnNames = rowMerge.FirstOrDefault().Value;
                var propvalue = item[rowValue] == null ? "" : item[rowValue].ToString();
                var rows = rowMeageValue.Split(',');
                if (rows[0] != propvalue || listData.Rows.Count == index + 1 && rows[0] == propvalue)
                {
                    if (rows.Length == 2)
                    {
                        var totalRow = headRow + index - int.Parse(rows[1]);
                        if (listData.Rows.Count == index + 1 && rows[0] == propvalue)
                            totalRow = totalRow + 1;
                        if (totalRow > 1)
                        {
                            foreach (var columnName in columnNames)
                            {
                                var colIndex = colNumDic.FirstOrDefault(m => m.Key == columnName).Value;
                                cells.Merge(int.Parse(rows[1]), colIndex, totalRow, 1);
                            }
                        }
                    }

                    rowMeageValue = propvalue + "," + (headRow + index);
                }
            }
        }

        Range contentRange = sheet.Cells.CreateRange(headRow, 0, listData.Rows.Count,
            isSerialNumber ? columns.Count + 1 : columns.Count);
        contentRange.SetStyle(styleContent);
        if (sumData != null)
        {
            if (isSerialNumber)
            {
                cells[rowNum, 0].PutValue("#");
                sheet.AutoFitColumn(0);
            }

            for (int i = 0; i < listProp.Count; i++)
            {
                if (sumData.Table.Columns.Contains(listProp[i]))
                {
                    SetCell(sumData, cells[rowNum, i + startColumn], listProp[i], dateFormat);
                }
            }

            Range footRange = sheet.Cells.CreateRange(rowNum, 0, 1,
                isSerialNumber ? columns.Count + 1 : columns.Count);
            footRange.SetStyle(styleFoot);
        }

        //自适应列宽
        for (int i = 0; i < columns.Count; i++)
        {
            sheet.AutoFitColumn(i);
        }

        workbook.Save(savePath + newFileName);
        return savePath + newFileName;
    }


    /// <summary>
    /// 导出 -table 列合并
    /// </summary>
    /// <param name="listData">数据集</param>
    /// <param name="columns">第一列 demo : {"area":"地区"}</param>
    /// <param name="relativePath">相对路径</param>
    /// <param name="fileName">导出文件名</param>
    /// <param name="isSerialNumber">是否加序号</param>
    /// <param name="dateFormat">时间格式</param>
    /// <param name="boolToText">布尔转换为中文</param>
    /// <param name="rowMerge">子列 demo:{"area" : [{"CS":"长沙"},{"CD":"常德"}]}</param>
    /// <returns></returns>
    public static string ExportDataToTableMergeColumn(DataTable listData, List<ExportColumnModel> columns,
        out string relativePath, string fileName = "",
        Dictionary<string, Dictionary<string, string>> rowMerge = null, bool isSerialNumber = true,
        string dateFormat = "yyyy-MM-dd HH:mm", bool boolToText = false)
    {
        if (listData == null || listData.Rows.Count == 0)
        {
            throw new System.Exception("没有数据导出");
        }

        string newFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + ".xlsx";
        string savePath = AppDomain.CurrentDomain.BaseDirectory + "ExportFile/";
        var dataFolder = DateTime.Now.Date.ToString("yyyyMMdd") + "/";
        relativePath = "ExportFile/" + dataFolder + newFileName;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        savePath += dataFolder;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        Workbook workbook = new Workbook(); //工作簿  
        Worksheet sheet = workbook.Worksheets[0]; //工作表 
        Cells cells = sheet.Cells; //单元格 

        Style styleTitle = GetHeaderStyle(workbook);
        Style styleContent = GetContentStyle(workbook);

        var listProp = new List<string>();
        var startColumn = 0; //是否加序号列
        if (isSerialNumber)
        {
            startColumn = 1;
            cells[0, 0].PutValue("#");
            cells.SetColumnWidth(0, 10);
        }

        if (rowMerge != null)
        {
            var totalColNum = rowMerge.Count * 2 +
                              columns.Count(t => !rowMerge.Select(m => m.Key).ToList().Contains(t.Key));
            Range headRange = sheet.Cells.CreateRange(0, 0, 2, //列为两行
                isSerialNumber ? totalColNum + startColumn : totalColNum);
            headRange.SetStyle(styleTitle);
            var colNum = startColumn;
            foreach (var col in columns)
            {
                cells[0, colNum].PutValue(col.Value); //填入列名
                if (rowMerge.Any(t => t.Key == col.Key))
                {
                    var mergerCols = rowMerge.FirstOrDefault(t => t.Key == col.Key).Value;
                    //存在需要列合并的列头
                    foreach (var i in mergerCols)
                    {
                        cells[1, colNum].PutValue(i.Value);
                        listProp.Add(i.Key); //数据列名
                        colNum++;
                    }
                }
                else
                {
                    listProp.Add(col.Key); //数据列名
                    colNum++;
                }
            }

            colNum = startColumn;
            if (isSerialNumber && rowMerge.Count > 0)
            {
                cells.Merge(0, 0, 2, 1); //合并表头序号名称
            }

            foreach (var col in columns)
            {
                //行合并 表头合并 
                if (rowMerge.All(t => t.Key != col.Key))
                {
                    cells.Merge(0, colNum, 2, 1);
                    colNum++;
                }
                else
                {
                    var mergeNum = rowMerge.FirstOrDefault(t => t.Key == col.Key).Value.Count();
                    cells.Merge(0, colNum, 1, mergeNum);
                    colNum += mergeNum;
                }
            }

            int headRow = 2;
            for (int index = 0; index < listData.Rows.Count; index++)
            {
                var item = listData.Rows[index];
                if (isSerialNumber)
                {
                    cells[headRow + index, 0].SetStyle(styleContent);
                    cells[headRow + index, 0].PutValue(index + 1);
                }

                for (int i = 0; i < listProp.Count; i++)
                {
                    SetCell(item, cells[headRow + index, i + startColumn], listProp[i], dateFormat);
                }

                Range contentRange = sheet.Cells.CreateRange(headRow, 0, listData.Rows.Count,
                    isSerialNumber ? totalColNum + startColumn : totalColNum);
                contentRange.SetStyle(styleContent);
            }

            //自适应列宽
            for (int i = 0; i < totalColNum + startColumn; i++)
            {
                sheet.AutoFitColumn(i);
            }
        }

        workbook.Save(savePath + newFileName);

        return savePath + newFileName;
    }

    /// <summary>
    ///导出-table(大数据导出)
    /// </summary> 
    /// <param name="listData">数据集</param>
    /// <param name="columns">列对应标题</param>
    /// <param name="sumData">合计行</param>
    /// <param name="relativePath">相对路径</param>
    /// <param name="fileName">导出文件名</param>
    /// <param name="isSerialNumber">是否显示序号</param>
    /// <param name="dateFormat">日期格式</param>
    /// <param name="boolToText">布尔转换为中文</param>
    /// <returns>绝对路径</returns>
    public static string ExportDataToTableLargeData(DataTable listData,
        List<ExportColumnModel> columns,
        DataRow sumData,
        out string relativePath,
        string fileName = "",
        bool isSerialNumber = true,
        string dateFormat = "yyyy-MM-dd HH:mm",
        bool boolToText = false)
    {
        if (listData == null || listData.Rows.Count == 0)
        {
            throw new System.Exception("没有数据导出");
        }

        string newFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss_ffff") + ".xlsx";
        string savePath = AppDomain.CurrentDomain.BaseDirectory + "ExportFile/";
        var dataFolder = DateTime.Now.Date.ToString("yyyyMMdd") + "/";
        relativePath = "ExportFile/" + dataFolder + newFileName;

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        savePath += dataFolder;
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        Workbook workbook = new Workbook(); //工作簿  
        Worksheet sheet = workbook.Worksheets[0]; //工作表  
        Cells cells = sheet.Cells; //单元格 

        Style styleTitle = GetHeaderStyle(workbook);
        Style styleContent = GetContentStyle(workbook);
        Style styleFoot = GetFootStyle(workbook);

        var listProp = new List<string>();

        var startColumn = 0; //是否加序号列
        if (isSerialNumber)
        {
            startColumn = 1;
            cells[0, 0].PutValue("#");
            cells.SetColumnWidth(0, 10);
        }

        Range headRange = sheet.Cells.CreateRange(0, 0, 1, isSerialNumber ? columns.Count + 1 : columns.Count);
        headRange.SetStyle(styleTitle);
        var colNum = 0;
        var colNumDic = new Dictionary<string, int>();
        foreach (var col in columns)
        {
            cells.SetColumnWidth(colNum + startColumn, 20);
            cells[0, colNum + startColumn].PutValue(col.Value);
            colNumDic.Add(col.Key, colNum + startColumn);
            colNum++;
            listProp.Add(col.Key);
        }

        int headRow = 1;

        int rowNum = listData.Rows.Count + headRow; //表格行数 
        if (sumData != null)
        {
            rowNum++;
        }

        string[,] datas = new string[rowNum, isSerialNumber ? listProp.Count + 1 : listProp.Count];

        for (int index = 0; index < listData.Rows.Count; index++)
        {
            var item = listData.Rows[index];

            if (isSerialNumber)
            {
                datas[index, 0] = (index + 1).ToString();
            }

            for (int i = 0; i < listProp.Count; i++)
            {
                var value = GetCell(item, listProp[i], dateFormat);
                datas[index, isSerialNumber ? i + 1 : i] = value; //在obj.ToString()前加单引号是为了防止自动转化格式  
            }
        }

        if (sumData != null)
        {
            if (isSerialNumber)
            {
                datas[rowNum, 0] = "#";
                sheet.AutoFitColumn(0);
            }

            for (int i = 0; i < listProp.Count; i++)
            {
                if (sumData.Table.Columns.Contains(listProp[i]))
                {
                    var value = GetCell(sumData, listProp[i], dateFormat);
                    datas[rowNum, i + 1] =
                        value == null ? "" : "'" + value.Trim(); //在obj.ToString()前加单引号是为了防止自动转化格式  
                }
            }

            Range footRange =
                sheet.Cells.CreateRange(rowNum, 0, 1, isSerialNumber ? columns.Count + 1 : columns.Count);
            footRange.SetStyle(styleFoot);
        }

        Range contentRange =
            sheet.Cells.CreateRange(headRow, 0, rowNum, isSerialNumber ? columns.Count + 1 : columns.Count);
        contentRange.SetStyle(styleContent);
        contentRange.Value = datas;


        //自适应列宽
        for (int i = 0; i < columns.Count; i++)
        {
            sheet.AutoFitColumn(i);
        }

        workbook.Save(savePath + newFileName);
        return savePath + newFileName;
    }

    #endregion

    /// <summary>
    /// 头部样式
    /// </summary>
    /// <param name="workbook"></param>
    /// <returns></returns>
    private static Style GetHeaderStyle(Workbook workbook)
    {
        Style headerStyle = workbook.CreateStyle(); //新增样式 
        headerStyle.HorizontalAlignment = TextAlignmentType.Center; //文字居中 
        headerStyle.VerticalAlignment = TextAlignmentType.Center; //文字居中 
        headerStyle.Font.Name = "宋体"; //文字字体 
        headerStyle.Font.Size = 12; //文字大小 
        headerStyle.Font.IsBold = true; //粗体 
        headerStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        headerStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        headerStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        headerStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        headerStyle.ForegroundColor = Color.LightGray;
        headerStyle.Pattern = BackgroundType.Solid;
        headerStyle.Font.Color = Color.MidnightBlue;
        return headerStyle;
    }

    /// <summary>
    /// 内容样式
    /// </summary>
    /// <param name="workbook"></param>
    /// <returns></returns>
    private static Style GetContentStyle(Workbook workbook)
    {
        Style contentStyle = workbook.CreateStyle(); //新增样式
        //contentStyle.HorizontalAlignment = TextAlignmentType.Center; //文字居中 
        contentStyle.VerticalAlignment = TextAlignmentType.Center; //文字居中 
        contentStyle.Font.Name = "宋体"; //文字字体 
        contentStyle.Font.Size = 12; //文字大小 
        contentStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        contentStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        contentStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        contentStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        return contentStyle;
    }

    /// <summary>
    /// 尾行样式
    /// </summary>
    /// <param name="workbook"></param>
    /// <returns></returns>
    private static Style GetFootStyle(Workbook workbook)
    {
        Style footStyle = workbook.CreateStyle(); //新增样式 
        footStyle.Font.Name = "宋体"; //文字字体 
        footStyle.Font.Size = 12; //文字大小 
        footStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        footStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        footStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        footStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        footStyle.ForegroundColor = Color.LightGray;
        footStyle.Pattern = BackgroundType.Solid;
        return footStyle;
    }


    /// <summary>
    /// 设置列值
    /// </summary>
    /// <param name="data">行数据</param>
    /// <param name="cell">列</param>
    /// <param name="cloumnName">列名称</param>
    /// <param name="dateFormat">日期格式</param>
    public static void SetCell(DataRow data, Cell cell, string cloumnName, string dateFormat)
    {
        var value = data[cloumnName];

        if (value == null || value == DBNull.Value) return;
        var dataTypeFullName = data.Table.Columns[cloumnName].DataType.FullName;
        if (dataTypeFullName != null)
        {
            var type = dataTypeFullName.ToUpper();
            if (type == "SYSTEM.DATETIME" || type == "SYSTEM.NULLABLE`1[SYSTEM.DATETIME]")
            {
                cell.PutValue(Convert.ToDateTime(value).ToString(dateFormat));
            }
            else if (type == "SYSTEM.DECIMAL" || type == "SYSTEM.NULLABLE`1[SYSTEM.DECIMAL]")
            {
                cell.PutValue(Convert.ToDecimal(value).ToString());
            }
            else
            {
                cell.PutValue(value);
            }
        }
    }

    /// <summary>
    /// 获取列值
    /// </summary>
    /// <param name="data">行数据</param>
    /// <param name="cloumnName">列名称</param>
    /// <param name="dateFormat">日期格式</param>
    public static string GetCell(DataRow data, string cloumnName, string dateFormat)
    {
        var value = data[cloumnName];
        if (value == null || value == DBNull.Value) return "";
        var dataTypeFullName = data.Table.Columns[cloumnName].DataType.FullName;
        if (dataTypeFullName == null) return value.ToString();
        var type = dataTypeFullName.ToUpper();
        switch (type)
        {
            case "SYSTEM.DATETIME":
            case "SYSTEM.NULLABLE`1[SYSTEM.DATETIME]":
                return Convert.ToDateTime(value).ToString(dateFormat);
            case "SYSTEM.DECIMAL":
            case "SYSTEM.NULLABLE`1[SYSTEM.DECIMAL]":
                return Convert.ToDecimal(value).ToString();
        }

        return value.ToString();
    }

    #endregion

    #region 导入

    /// <summary>
    /// 导入数据
    /// </summary>
    /// <typeparam name="T">实体</typeparam>
    /// <param name="excelfile">文件名</param>
    /// <param name="columDictionary">列集合</param>
    /// <param name="firstRow">起始行</param>
    /// <param name="firstColumn">起始列</param>
    /// <returns>数据集</returns>
    public static List<T> ImportData<T>(string excelfile, Dictionary<string, string> columDictionary,
        int firstRow = 0, int firstColumn = 0) where T : new()
    {
        Workbook workbook = new Workbook(excelfile);
        Cells cells = workbook.Worksheets[0].Cells;
        DataTable dt = cells.ExportDataTableAsString(firstRow, firstColumn, cells.MaxDataRow + 1,
            cells.MaxColumn + 1, true);
        RemoveEmpty(dt);
        if (dt == null || dt.Rows.Count == 0)
        {
            throw new System.Exception("没有数据导入");
        }

        PropertyInfo[] properties = typeof(T).GetProperties(); //获取实体类型的属性集合 

        var proDictionary = properties.Where(pro => columDictionary.ContainsKey(pro.Name))
            .ToDictionary(pro => pro.Name);

        List<T> list = new List<T>();
        foreach (DataRow dr in dt.Rows)
        {
            var model = new T();
            var rowDataNotEmpty = false;
            foreach (var proItem in proDictionary)
            {
                var value = dr[columDictionary[proItem.Key]];
                if (value != DBNull.Value && value != null && value.ToString() != "")
                {
                    rowDataNotEmpty = true;
                    var dataValue = GetParseValue(proItem.Value.PropertyType, value.ToString().Trim());
                    proItem.Value.SetValue(model, dataValue, null);
                }
            }

            if (rowDataNotEmpty)
            {
                list.Add(model);
            }
        }

        return list;
    }


    /// <summary>
    /// 导入数据(只读取第一个工作簿数据)
    /// </summary> 
    /// <typeparam name="T">实体</typeparam>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="excelfile">文件名</param>
    /// <param name="columDictionary">列集合</param>
    /// <param name="firstRow">起始行</param>
    /// <param name="firstColumn">起始列</param>
    /// <returns>数据集</returns>
    /// <returns></returns>
    public static List<T> ImportDataByOne<T>(out string errorMessage, string excelfile,
        List<ImportColumnModel> columDictionary, int firstRow = 0, int firstColumn = 0)
        where T : new()
    {
        errorMessage = string.Empty;
        Workbook workbook = new Workbook(excelfile);
        Cells cells = workbook.Worksheets[0].Cells;
        return GetImportData<T>(ref errorMessage, columDictionary, firstRow, firstColumn, cells);
    }

    /// <summary>
    /// 导入数据(通过工作簿名称读取数据)
    /// </summary>
    /// <typeparam name="T">实体</typeparam>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="excelfile">文件名</param>
    /// <param name="columDictionary">列集合</param>
    /// <param name="firstRow">起始行</param>
    /// <param name="firstColumn">起始列</param>
    /// <param name="sheetName">工作薄名称</param>
    /// <returns></returns>
    public static List<T> ImportData<T>(out string errorMessage, string excelfile,
        List<ImportColumnModel> columDictionary, string sheetName = "数据模板", int firstRow = 0, int firstColumn = 0)
        where T : new()
    {
        errorMessage = string.Empty;
        Workbook workbook = new Workbook(excelfile);
        var woksheets = workbook.Worksheets[sheetName];
        if (woksheets == null)
        {
            throw new System.Exception(string.Format("未找到工作簿", sheetName));
        }

        Cells cells = workbook.Worksheets[sheetName].Cells;
        return GetImportData<T>(ref errorMessage, columDictionary, firstRow, firstColumn, cells);
    }

    /// <summary>
    /// 导入数据
    /// </summary>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="excelfile">文件名</param> 
    /// <param name="firstRow">起始行</param>
    /// <param name="firstColumn">起始列</param>
    /// <returns>DataTable</returns>
    public static DataTable ImportTable(out string errorMessage, string excelfile, int firstRow = 0,
        int firstColumn = 0)
    {
        errorMessage = string.Empty;
        Workbook workbook = new Workbook(excelfile);
        Cells cells = workbook.Worksheets[0].Cells;
        DataTable dt = cells.ExportDataTableAsString(firstRow, firstColumn, cells.MaxDataRow + 1,
            cells.MaxColumn + 1, true);
        RemoveEmpty(dt);
        return dt;
    }

    /// <summary>
    /// 获取导入数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="columDictionary">列集合</param>
    /// <param name="firstRow">起始行</param>
    /// <param name="firstColumn">起始列</param>
    /// <param name="cells">列</param>
    /// <returns></returns>
    private static List<T> GetImportData<T>(ref string errorMessage, List<ImportColumnModel> columDictionary,
        int firstRow, int firstColumn,
        Cells cells) where T : new()
    {
        DataTable dt = cells.ExportDataTableAsString(firstRow, firstColumn, cells.MaxDataRow + 1,
            cells.MaxColumn + 1, true);
        RemoveEmpty(dt);
        if (dt == null || dt.Rows.Count == 0)
        {
            return new List<T>();
        }

        PropertyInfo[] properties = typeof(T).GetProperties(); //获取实体类型的属性集合 

        var proDictionary =
            properties.Where(pro => columDictionary.Any(t => t.PropertyName == pro.Name))
                .ToDictionary(pro => pro.Name);

        List<T> list = new List<T>();

        var allEmptyBuilder = new Dictionary<string, StringBuilder>();
        var allErrorBuilder = new Dictionary<string, StringBuilder>();
        var allFormatBuilder = new Dictionary<string, StringBuilder>();
        var allExitBuilder = new Dictionary<string, StringBuilder>();

        var rowEmptyColumns = new List<string>();
        var rowErrorColumns = new List<string>();
        var rowFormatColumns = new List<string>();
        var rowExistColumns = new List<string>();
        var excelRow = firstRow + 1;
        foreach (DataRow dr in dt.Rows)
        {
            excelRow++;
            var model = new T();
            var rowDataNotEmpty = false;
            rowEmptyColumns.Clear();
            rowErrorColumns.Clear();
            rowFormatColumns.Clear();
            rowExistColumns.Clear();
            foreach (var proItem in proDictionary)
            {
                var columnItem = columDictionary.First(t => t.PropertyName == proItem.Value.Name);
                var value = dr[columnItem.ColumnName];
                if (value.ToString().Trim() == "是") value = true;
                if (value.ToString().Trim() == "否") value = false;
                if (value == DBNull.Value || value.ToString().Trim() == "")
                {
                    if (columnItem.IsRequired)
                    {
                        rowEmptyColumns.Add(columnItem.ColumnName);
                    }

                    continue;
                }

                rowDataNotEmpty = true;
                object dataValue;
                try
                {
                    dataValue = GetParseValue(proItem.Value.PropertyType, value.ToString().Trim());
                }
                catch
                {
                    rowFormatColumns.Add(columnItem.ColumnName);
                    continue;
                }

                if (dataValue != null && columnItem.DataSource != null &&
                    Convert.ToString(dataValue).Trim() != "" && !columnItem.DataSource.Contains(dataValue))
                {
                    rowErrorColumns.Add(columnItem.ColumnName);
                    continue;
                }

                if (columnItem.ExistList != null && columnItem.ExistList.Contains(dataValue))
                {
                    rowExistColumns.Add(columnItem.ColumnName);
                    continue;
                }

                proItem.Value.SetValue(model, dataValue, null);
            }

            if (rowDataNotEmpty)
            {
                //数据不能为空
                if (rowEmptyColumns.Count > 0)
                {
                    foreach (var columnName in rowEmptyColumns)
                    {
                        AddError(allEmptyBuilder, columnName, excelRow, ",");
                    }
                }

                //数据不符合要求
                if (rowErrorColumns.Count > 0)
                {
                    foreach (var columnName in rowErrorColumns)
                    {
                        AddError(allErrorBuilder, columnName, excelRow, ",");
                    }
                }

                //数据格式不正常
                if (rowFormatColumns.Count > 0)
                {
                    foreach (var columnName in rowFormatColumns)
                    {
                        AddError(allFormatBuilder, columnName, excelRow, ",");
                    }
                }

                //数据已存在
                if (rowExistColumns.Count > 0)
                {
                    foreach (var columnName in rowExistColumns)
                    {
                        AddError(allExitBuilder, columnName, excelRow, ",");
                    }
                }

                list.Add(model);
            }
        }

        if (allEmptyBuilder.Count > 0)
        {
            errorMessage += allEmptyBuilder.Aggregate(string.Empty,
                (current, pair) => current + string.Format("Excel不能为空" + "\r\n", pair.Key,
                    pair.Value.Length > 50
                        ? pair.Value.ToString().Substring(0, 50) + "..."
                        : pair.Value.ToString()));
        }

        if (allErrorBuilder.Count > 0)
        {
            errorMessage += allErrorBuilder.Aggregate(string.Empty,
                (current, pair) => current + string.Format("Excel不匹配" + "\r\n", pair.Key,
                    pair.Value.Length > 50
                        ? pair.Value.ToString().Substring(0, 50) + "..."
                        : pair.Value.ToString()));
        }

        if (allFormatBuilder.Count > 0)
        {
            errorMessage += allFormatBuilder.Aggregate(string.Empty,
                (current, pair) => current + string.Format("Excel不正确" + "\r\n", pair.Key,
                    pair.Value.Length > 50
                        ? pair.Value.ToString().Substring(0, 50) + "..."
                        : pair.Value.ToString()));
        }

        if (allExitBuilder.Count > 0)
        {
            string result = string.Empty;
            foreach (var pair in allExitBuilder)
            {
                var item = "Excel已存在";
                var columnInfo = columDictionary.FirstOrDefault(t => t.ColumnName == pair.Key);
                if (columnInfo != null && !string.IsNullOrEmpty(columnInfo.ExistListErrorMessage))
                {
                    item = columnInfo.ExistListErrorMessage;
                }

                result = result + string.Format(item + "；\r\n",
                    pair.Value.Length > 50
                        ? pair.Value.ToString().Substring(0, 50) + "..."
                        : pair.Value.ToString());
            }

            errorMessage += result;
        }

        if (!list.Any())
        {
            return new List<T>();
        }

        return list;
    }

    /// <summary>
    /// 循环去除datatable中的空行
    /// </summary>
    /// <param name="dt"></param>
    private static void RemoveEmpty(DataTable dt)
    {
        if (dt != null && dt.Rows.Count != 0)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool rowdataisnull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {
                        rowdataisnull = false;
                    }
                }

                if (rowdataisnull)
                {
                    removelist.Add(dt.Rows[i]);
                }
            }

            foreach (DataRow t in removelist)
            {
                dt.Rows.Remove(t);
            }
        }
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    /// <param name="messageBuilder"></param>
    /// <param name="key"></param>
    /// <param name="objString"></param>
    /// <param name="separate"></param>
    private static void AddError(Dictionary<string, StringBuilder> messageBuilder, string key, object objString,
        string separate = "")
    {
        if (messageBuilder.ContainsKey(key))
        {
            messageBuilder[key].Append(separate + objString);
        }
        else
        {
            messageBuilder.Add(key, new StringBuilder());
            messageBuilder[key].Append(objString);
        }
    }

    /// <summary>
    /// 获取转换数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static object GetParseValue(Type type, object value)
    {
        if (value == null) return null;
        try
        {
            switch (type.ToString().ToUpper())
            {
                case DataTypeString.Decimal:
                    return Convert.ToDecimal(value);
                case DataTypeString.NullableDecimal:
                    return Convert.ToDecimal(value);
                case DataTypeString.DataTime:
                case DataTypeString.NullableDateTime:
                    return Convert.ToDateTime(value);
                case DataTypeString.Int:
                case DataTypeString.NullableInt:
                    return Convert.ToInt32(value);
                case DataTypeString.String:
                    return Convert.ToString(value);
                case DataTypeString.Single:
                    return Convert.ToSingle(value);
                case DataTypeString.Boolean:
                case DataTypeString.NullableBoolean:
                    return Convert.ToBoolean(value);
                case DataTypeString.Double:
                    return Convert.ToDouble(value);
                default:
                    return Convert.ToString(value);
            }
        }
        catch (System.Exception err)
        {
            throw new System.Exception(err.Message);
        }
    }

    /// <summary>
    /// 数据类型字符串
    /// </summary>
    public struct DataTypeString
    {
        /// <summary>
        /// 
        /// </summary>
        public const string DataTime = "SYSTEM.DATETIME";

        /// <summary>
        /// 
        /// </summary>
        public const string NullableDateTime = "SYSTEM.NULLABLE`1[SYSTEM.DATETIME]";

        /// <summary>
        /// 
        /// </summary>
        public const string NullableInt = "SYSTEM.NULLABLE`1[SYSTEM.INT32]";

        /// <summary>
        /// 
        /// </summary>
        public const string NullableDecimal = "SYSTEM.NULLABLE`1[SYSTEM.DECIMAL]";

        /// <summary>
        /// 
        /// </summary>
        public const string Int = "SYSTEM.INT32";

        /// <summary>
        /// 
        /// </summary>
        public const string Int64 = "SYSTEM.INT64";

        /// <summary>
        /// 
        /// </summary>
        public const string String = "SYSTEM.STRING";

        /// <summary>
        /// 
        /// </summary>
        public const string Decimal = "SYSTEM.DECIMAL";

        /// <summary>
        /// 
        /// </summary>
        public const string Single = "SYSTEM.SINGLE";

        /// <summary>
        /// 
        /// </summary>
        public const string Boolean = "SYSTEM.BOOLEAN";

        /// <summary>
        /// 
        /// </summary>
        public const string NullableBoolean = "SYSTEM.NULLABLE`1[SYSTEM.BOOLEAN]";

        /// <summary>
        /// 
        /// </summary>
        public const string Double = "SYSTEM.DOUBLE";

        /// <summary>
        /// 
        /// </summary>
        public const string Guid = "SYSTEM.GUID";

        /// <summary>
        /// 
        /// </summary>
        public const string NullableGuid = "SYSTEM.NULLABLE`1[SYSTEM.GUID]";

        /// <summary>
        /// 
        /// </summary>
        public const string EnumerableString = "SYSTEM.COLLECTIONS.GENERIC.IENUMERABLE`1[SYSTEM.STRING]";

        /// <summary>
        /// 
        /// </summary>
        public const string Byte = "SYSTEM.BYTE";

        /// <summary>
        /// 
        /// </summary>
        public const string NullByte = "SYSTEM.NULLABLE`1[SYSTEM.BYTE]";
    }

    #endregion
}