using System;
using System.Collections.Generic;

namespace ApeVolo.Common.Helper.Excel
{
    /// <summary>
    /// 导入列实体
    /// </summary>
    public class ImportColumnModel
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Excel 列名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public List<string> DataSource { get; set; }

        /// <summary>
        /// 已经存在
        /// </summary>
        public List<string> ExistList { get; set; }
        /// <summary>
        /// 已经存在错误提示
        /// </summary>
        public string ExistListErrorMessage { get; set; }

        /// <summary>
        /// 快捷编码
        /// </summary>
        public string ShortCut { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Type DateType { get; set; }
    }
}
