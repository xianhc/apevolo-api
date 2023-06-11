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
using ApeVolo.IBusiness.Dto.System.FileRecord;
using ApeVolo.IBusiness.ExportModel.System;
using ApeVolo.IBusiness.Interface.System.FileRecord;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.System.FileRecord;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.Business.System.FileRecord;

public class FileRecordService : BaseServices<Entity.System.FileRecord>, IFileRecordService
{
    #region 字段

    #endregion

    #region 构造函数

    public FileRecordService(IMapper mapper, IFileRecordRepository fileRecordRepository, ICurrentUser currentUser)
    {
        Mapper = mapper;
        BaseDal = fileRecordRepository;
        CurrentUser = currentUser;
    }

    #endregion

    #region 基础方法

    public async Task<bool> CreateAsync(string description, IFormFile file)
    {
        if (await IsExistAsync(x => x.Description == description))
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

        var fileRecord = new Entity.System.FileRecord
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
        var oldFileRecord = await QueryFirstAsync(x => x.Id == createUpdateFileRecordDto.Id);
        if (oldFileRecord.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldFileRecord.Description != createUpdateFileRecordDto.Description && await IsExistAsync(x =>
                x.Description == createUpdateFileRecordDto.Description))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("FileRecord"),
                createUpdateFileRecordDto.Description));
        }

        var fileRecord = Mapper.Map<Entity.System.FileRecord>(createUpdateFileRecordDto);
        return await UpdateEntityAsync(fileRecord);
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var appSecretList = await QueryByIdsAsync(ids);
        foreach (var appSecret in appSecretList)
        {
            await DeleteEntityAsync(appSecret);
            FileHelper.Delete(appSecret.FilePath);
        }

        return true;
    }

    public async Task<List<FileRecordDto>> QueryAsync(FileRecordQueryCriteria fileRecordQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<Entity.System.FileRecord, bool>> whereLambda = r => true;
        if (!fileRecordQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereLambda = whereLambda.AndAlso(r =>
                r.Description.Contains(fileRecordQueryCriteria.KeyWords) ||
                r.OriginalName.Contains(fileRecordQueryCriteria.KeyWords));
        }

        if (!fileRecordQueryCriteria.CreateTime.IsNull())
        {
            whereLambda = whereLambda.AndAlso(r =>
                r.CreateTime >= fileRecordQueryCriteria.CreateTime[0] &&
                r.CreateTime <= fileRecordQueryCriteria.CreateTime[1]);
        }

        return Mapper.Map<List<FileRecordDto>>(await BaseDal.QueryPageListAsync(whereLambda, pagination));
    }

    public async Task<List<ExportBase>> DownloadAsync(FileRecordQueryCriteria fileRecordQueryCriteria)
    {
        var fileRecords = await QueryAsync(fileRecordQueryCriteria, new Pagination { PageSize = 9999 });
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
}