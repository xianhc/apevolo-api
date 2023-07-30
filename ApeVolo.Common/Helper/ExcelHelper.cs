using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace ApeVolo.Common.Helper;

public class ExcelHelper
{
    public int ExportMaxCount { get; set; }
    public int ExportExcelCount { get; set; }

    /// <summary>
    /// 导出文件第一行背景颜色，使用HSSFColor，例如：HSSFColor.Red.Index
    /// </summary>
    public short? ExportTitleBackColor { get; set; }

    /// <summary>
    /// 导出文件第一行文字颜色，使用HSSFColor，例如：HSSFColor.Red.Index
    /// </summary>
    public short? ExportTitleFontColor { get; set; }

    public virtual byte[] GenerateExcel(List<ExportBase> exportRows, out string mimeType)
    {
        mimeType = MimeTypes.TextXlsx; //默认xlsx
        ExportMaxCount = ExportMaxCount == 0 ? 10000 : (ExportMaxCount > 10000 ? 10000 : ExportMaxCount);
        ExportExcelCount = exportRows.Count < ExportMaxCount
            ? 1
            : ((exportRows.Count % ExportMaxCount) == 0
                ? (exportRows.Count / ExportMaxCount)
                : (exportRows.Count / ExportMaxCount + 1));

        //如果是1，直接下载Excel，如果是多个，下载ZIP包
        if (ExportExcelCount == 1)
        {
            return DownLoadExcel(exportRows);
        }

        mimeType = MimeTypes.ApplicationZip;
        return DownLoadZipPackage(exportRows);
    }

    private byte[] DownLoadExcel(List<ExportBase> exportRows)
    {
        var book = GenerateWorkBook(exportRows);
        byte[] rv = new byte[] { };
        using (MemoryStream ms = new MemoryStream())
        {
            book.Write(ms);
            rv = ms.ToArray();
        }

        return rv;
    }

    private byte[] DownLoadZipPackage(List<ExportBase> exportRows)
    {
        string fileName = nameof(ExportBase) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
        //文件夹目录
        string filePath = $"{AppSettings.WebRootPath}//exportFile//TmpFile";

        //压缩包目录
        string zipPath = $"{AppSettings.WebRootPath}//exportFile//TmpFile{fileName}.zip";

        //打开文件夹
        DirectoryInfo fileFolder = new DirectoryInfo(filePath);
        if (!fileFolder.Exists)
        {
            //创建文件夹
            fileFolder.Create();
        }
        else
        {
            //清空文件夹
            FileSystemInfo[] files = fileFolder.GetFileSystemInfos();
            foreach (var item in files)
            {
                if (item is DirectoryInfo)
                {
                    DirectoryInfo directory = new DirectoryInfo(item.FullName);
                    directory.Delete(true);
                }
                else
                {
                    File.Delete(item.FullName);
                }
            }
        }

        //放入数据
        for (int i = 0; i < ExportExcelCount; i++)
        {
            var list = exportRows.Skip(i * ExportMaxCount).Take(ExportMaxCount).ToList();
            var workBook = GenerateWorkBook(list);
            string savePath = $"{filePath}/{fileName}_{i + 1}.xlsx";
            using var fs = new FileStream(savePath, FileMode.CreateNew);
            workBook.Write(fs);
        }

        //生成压缩包
        ZipFile.CreateFromDirectory(filePath, zipPath);

        //读取压缩包
        FileStream ZipFS = new FileStream(zipPath, FileMode.Open, FileAccess.Read);
        byte[] bt = new byte[ZipFS.Length];
        ZipFS.Read(bt, 0, bt.Length);
        ZipFS.Close();

        //删除文件夹
        DirectoryInfo rootFolder = new DirectoryInfo(filePath);
        if (rootFolder.Exists)
        {
            rootFolder.Delete(true);
        }

        return bt;
    }


    /// <summary>
    /// 根据集合生成单个Excel
    /// </summary>
    /// <param name="exportBases"></param>
    /// <returns></returns>
    private IWorkbook GenerateWorkBook(List<ExportBase> exportBases)
    {
        IWorkbook book = new XSSFWorkbook();
        ISheet sheet = book.CreateSheet();
        IRow rowNum = sheet.CreateRow(0);

        //创建表头样式
        ICellStyle headerStyle = book.CreateCellStyle();
        headerStyle.FillBackgroundColor =
            ExportTitleBackColor == null ? HSSFColor.LightBlue.Index : ExportTitleBackColor.Value;
        headerStyle.FillPattern = FillPattern.SolidForeground;
        headerStyle.FillForegroundColor =
            ExportTitleBackColor == null ? HSSFColor.LightBlue.Index : ExportTitleBackColor.Value;
        headerStyle.BorderBottom = BorderStyle.Thin;
        headerStyle.BorderTop = BorderStyle.Thin;
        headerStyle.BorderLeft = BorderStyle.Thin;
        headerStyle.BorderRight = BorderStyle.Thin;
        IFont font = book.CreateFont();
        font.FontName = "Calibri";
        font.FontHeightInPoints = 12;
        font.Color = ExportTitleFontColor == null ? HSSFColor.Black.Index : ExportTitleFontColor.Value;
        headerStyle.SetFont(font);

        ICellStyle cellStyle = book.CreateCellStyle();
        cellStyle.BorderBottom = BorderStyle.Thin;
        cellStyle.BorderTop = BorderStyle.Thin;
        cellStyle.BorderLeft = BorderStyle.Thin;
        cellStyle.BorderRight = BorderStyle.Thin;

        //生成表头
        var props = exportBases[0].GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic |
                                                           BindingFlags.Public);
        MakeExcelHeader(sheet, props, 0, 0, headerStyle);

        //放入数据
        for (var i = 0; i < exportBases.Count; i++)
        {
            var colIndex = 0;
            var row = sheet.CreateRow(i + 1);
            foreach (var pi in props)
            {
                if (pi.Name.ToUpper().Equals("ID"))
                {
                    continue;
                }

                var propertyValue = exportBases[i].GetPropertyValue(pi.Name);
                var text = Regex.Replace(
                    propertyValue == null ? string.Empty : propertyValue.ToString() ?? string.Empty,
                    @"<[^>]*>", string.Empty);

                var piType = pi.PropertyType;
                if (piType.IsEnum())
                {
                    if (int.TryParse(text, out var enumValue))
                    {
                        var eName = piType.GetEnumName(enumValue);
                        if (string.IsNullOrEmpty(eName))
                        {
                            text = "";
                        }
                        else
                        {
                            var field = piType.GetField(eName);
                            if (field != null)
                            {
                                var display = field
                                    .GetCustomAttributes(typeof(DisplayAttribute), true)
                                    .OfType<DisplayAttribute>()
                                    .FirstOrDefault();
                                text = display == null ? field.Name : display.Name;
                                //}
                            }
                        }
                    }
                }

                //建立excel单元格
                ICell cell;
                if (piType.IsNumber())
                {
                    cell = row.CreateCell(colIndex, CellType.Numeric);
                    try
                    {
                        cell.SetCellValue(Convert.ToDouble(text));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else
                {
                    cell = row.CreateCell(colIndex);
                    cell.SetCellValue(text);
                }

                cell.CellStyle = cellStyle;
                colIndex++;
            }
        }

        return book;
    }

    private void MakeExcelHeader(ISheet sheet, PropertyInfo[] propertyInfos, int rowIndex, int colIndex,
        ICellStyle style)
    {
        var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);

        //循环所有属性
        foreach (var p in propertyInfos)
        {
            if (p.Name.ToUpper().Equals("ID"))
            {
                continue;
            }

            //添加新单元格
            var cell = row.CreateCell(colIndex);
            cell.CellStyle = style;


            var display = p
                .GetCustomAttributes(typeof(DisplayAttribute), true)
                .OfType<DisplayAttribute>()
                .FirstOrDefault();
            cell.SetCellValue(display == null ? p.Name : display.Name);

            var cellRangeAddress = new CellRangeAddress(rowIndex, rowIndex, colIndex, colIndex);
            sheet.AddMergedRegion(cellRangeAddress);
            // 居中
            //cell.CellStyle.Alignment = HorizontalAlignment.Center;
            //cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            for (int i = cellRangeAddress.FirstRow; i <= cellRangeAddress.LastRow; i++)
            {
                IRow r = CellUtil.GetRow(i, sheet);
                for (int j = cellRangeAddress.FirstColumn; j <= cellRangeAddress.LastColumn; j++)
                {
                    ICell c = CellUtil.GetCell(r, (short)j);
                    c.CellStyle = style;
                }
            }

            colIndex++;
        }
    }
}
