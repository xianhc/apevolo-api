using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.System;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.ExportModel.System;
using ApeVolo.IBusiness.Interface.System;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Business.System;

public class FileRecordService : BaseServices<FileRecord>, IFileRecordService
{
    #region 构造函数

    public FileRecordService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(string description, IFormFile file)
    {
        if (await TableWhere(x => x.Description == description).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("FileRecord"),
                description));
        }

        var fileExtensionName = FileHelper.GetExtensionName(file.FileName);
        var fileTypeName = FileHelper.GetFileTypeName(fileExtensionName);
        var fileTypeNameEn = FileHelper.GetFileTypeNameEn(fileTypeName);

        string fileName = DateTime.Now.ToString("yyyyMMdd") + IdHelper.GetId() +
                          file.FileName.Substring(Math.Max(file.FileName.LastIndexOf('.'), 0));
        string filePath = Path.Combine(AppSettings.WebRootPath, "file", fileTypeNameEn);
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        filePath = Path.Combine(filePath, fileName);
        await using (var fs = new FileStream(filePath, FileMode.CreateNew))
        {
            await file.CopyToAsync(fs);
            fs.Flush();
        }

        var fileRecord = new FileRecord
        {
            Description = description,
            OriginalName = file.FileName,
            NewName = fileName,
            FilePath = filePath,
            Size = FileHelper.GetFileSize(file.Length),
            ContentType = file.ContentType,
            ContentTypeName = fileTypeName,
            ContentTypeNameEn = fileTypeNameEn
        };
        return await AddEntityAsync(fileRecord);
    }

    public async Task<bool> UpdateAsync(CreateUpdateFileRecordDto createUpdateFileRecordDto)
    {
        //取出待更新数据
        var oldFileRecord = await TableWhere(x => x.Id == createUpdateFileRecordDto.Id).FirstAsync();
        if (oldFileRecord.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldFileRecord.Description != createUpdateFileRecordDto.Description &&
            await TableWhere(x => x.Description == createUpdateFileRecordDto.Description).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("FileRecord"),
                createUpdateFileRecordDto.Description));
        }

        var fileRecord = ApeContext.Mapper.Map<FileRecord>(createUpdateFileRecordDto);
        return await UpdateEntityAsync(fileRecord);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var appSecretList = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        await LogicDelete<FileRecord>(x => ids.Contains(x.Id));
        foreach (var appSecret in appSecretList)
        {
            FileHelper.Delete(appSecret.FilePath);
        }

        return true;
    }

    public async Task<List<FileRecordDto>> QueryAsync(FileRecordQueryCriteria fileRecordQueryCriteria,
        Pagination pagination)
    {
        var whereExpression = GetWhereExpression(fileRecordQueryCriteria);
        return ApeContext.Mapper.Map<List<FileRecordDto>>(
            await SugarRepository.QueryPageListAsync(whereExpression, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(FileRecordQueryCriteria fileRecordQueryCriteria)
    {
        var whereExpression = GetWhereExpression(fileRecordQueryCriteria);
        var fileRecords = await TableWhere(whereExpression).ToListAsync();
        List<ExportBase> fileRecordExports = new List<ExportBase>();
        fileRecordExports.AddRange(fileRecords.Select(x => new FileRecordExport()
        {
            Description = x.Description,
            ContentType = x.ContentType,
            ContentTypeName = x.ContentTypeName,
            ContentTypeNameEn = x.ContentTypeNameEn,
            OriginalName = x.OriginalName,
            NewName = x.NewName,
            FilePath = x.FilePath,
            Size = x.Size,
            CreateTime = x.CreateTime
        }));
        return fileRecordExports;
    }

    #endregion

    #region 条件表达式

    private static Expression<Func<FileRecord, bool>> GetWhereExpression(
        FileRecordQueryCriteria fileRecordQueryCriteria)
    {
        Expression<Func<FileRecord, bool>> whereExpression = r => true;
        if (!fileRecordQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.Description.Contains(fileRecordQueryCriteria.KeyWords) ||
                r.OriginalName.Contains(fileRecordQueryCriteria.KeyWords));
        }

        if (!fileRecordQueryCriteria.CreateTime.IsNull())
        {
            whereExpression = whereExpression.AndAlso(r =>
                r.CreateTime >= fileRecordQueryCriteria.CreateTime[0] &&
                r.CreateTime <= fileRecordQueryCriteria.CreateTime[1]);
        }

        return whereExpression;
    }

    #endregion
}
