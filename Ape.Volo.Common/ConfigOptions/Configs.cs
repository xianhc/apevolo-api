namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// 全局配置类
/// </summary>
// public class Configs
// {
//     #region 是否开发模式
//
//     private bool? _isQuickDebug;
//
//     /// <summary>
//     /// 是否开发模式，生产环境建议设置为False
//     /// </summary>
//     public bool IsQuickDebug
//     {
//         get => _isQuickDebug ?? false;
//         set => _isQuickDebug = value;
//     }
//
//     #endregion
//
//     #region 是否初始DataTable
//
//     private bool? _isInitTable;
//
//     /// <summary>
//     /// 是否初始DataTable
//     /// </summary>
//     public bool IsInitTable
//     {
//         get => _isInitTable ?? false;
//         set => _isInitTable = value;
//     }
//
//     #endregion
//
//     #region 是否初始数据
//
//     private bool? _isInitData;
//
//     /// <summary>
//     /// 是否初始Data
//     /// </summary>
//     public bool IsInitData
//     {
//         get => _isInitData ?? false;
//         set => _isInitData = value;
//     }
//
//     #endregion
//
//     #region 是否开启读写分离
//
//     private bool? _isCqrs;
//
//     /// <summary>
//     /// 是否开发模式
//     /// </summary>
//     public bool IsCqrs
//     {
//         get => _isCqrs ?? false;
//         set => _isCqrs = value;
//     }
//
//     #endregion
//
//     #region 默认DB
//
//     private string _defaultDataBase;
//
//     /// <summary>
//     /// 默认DB
//     /// </summary>
//     public string DefaultDataBase
//     {
//         get => _defaultDataBase ?? "Ape.Volo.Sqlite.Master";
//         set => _defaultDataBase = value;
//     }
//
//     #endregion
//
//     #region 日志DB
//
//     private string _logDataBase;
//
//     /// <summary>
//     /// 默认DB
//     /// </summary>
//     public string LogDataBase
//     {
//         get => _logDataBase ?? "Ape.Volo.Log";
//         set => _logDataBase = value;
//     }
//
//     #endregion
//
//     #region 数据库连接对象
//
//     private DataConnectionOptions _dataConnectionOptions;
//
//     /// <summary>
//     ///  数据库连接对象
//     /// </summary>
//     public DataConnectionOptions DataConnectionOptions
//     {
//         get
//         {
//             if (_dataConnectionOptions == null)
//             {
//                 _dataConnectionOptions = new DataConnectionOptions
//                 {
//                     ConnectionItem = new List<ConnectionItem>()
//                 };
//             }
//
//             return _dataConnectionOptions;
//         }
//         set => _dataConnectionOptions = value;
//     }
//
//     #endregion
//
//     #region Redis缓存
//
//     private RedisOptions _redisOptions;
//
//     /// <summary>
//     ///  Redis缓存
//     /// </summary>
//     public RedisOptions RedisOptions
//     {
//         get
//         {
//             if (_redisOptions == null)
//             {
//                 _redisOptions = new RedisOptions
//                 {
//                     Name = "Ape.Volo.Redis",
//                     Host = "localhost",
//                     Port = 6379
//                 };
//             }
//
//             return _redisOptions;
//         }
//         set => _redisOptions = value;
//     }
//
//     #endregion
//
//     #region 跨域配置
//
//     private CorsOptions _corsOptions;
//
//     /// <summary>
//     ///  跨域配置
//     /// </summary>
//     public CorsOptions CorsOptions
//     {
//         get
//         {
//             if (_corsOptions == null)
//             {
//                 _corsOptions = new CorsOptions
//                 {
//                     EnableAll = false,
//                     Name = "",
//                     Policy = new List<Policy>()
//                 };
//             }
//
//             return _corsOptions;
//         }
//         set => _corsOptions = value;
//     }
//
//     #endregion
//
//     #region 鉴权订阅人
//
//     private JwtAuthOptions _jwtAuthOptionses;
//
//     /// <summary>
//     ///  鉴权订阅人
//     /// </summary>
//     public JwtAuthOptions JwtAuthOptionses
//     {
//         get
//         {
//             if (_jwtAuthOptionses == null)
//             {
//                 _jwtAuthOptionses = new JwtAuthOptions
//                 {
//                     Audience = "http://localhost",
//                     Issuer = "http://localhost",
//                     SecurityKey = "5ixKD0BkJxYYroZTvdPs3w9NWRoiUacN",
//                     Expires = 12,
//                     RefreshTokenExpires = 168,
//                     LoginPath = "/auth/login"
//                 };
//             }
//
//             return _jwtAuthOptionses;
//         }
//         set => _jwtAuthOptionses = value;
//     }
//
//     #endregion
//
//     #region 事件总线
//
//     private EventBusOptions _eventBusOptions;
//
//     /// <summary>
//     ///  鉴权订阅人
//     /// </summary>
//     public EventBusOptions EventBusOptions
//     {
//         get
//         {
//             if (_eventBusOptions == null)
//             {
//                 _eventBusOptions = new EventBusOptions
//                 {
//                     Enabled = false,
//                     SubscriptionClientName = "ape.volo.event"
//                 };
//             }
//
//             return _eventBusOptions;
//         }
//         set => _eventBusOptions = value;
//     }
//
//     #endregion
//
//     #region Rabbit消息队列
//
//     private RabbitOptions _rabbitOptions;
//
//     /// <summary>
//     ///  Rabbit消息队列
//     /// </summary>
//     public RabbitOptions RabbitOptions
//     {
//         get
//         {
//             if (_rabbitOptions == null)
//             {
//                 _rabbitOptions = new RabbitOptions
//                 {
//                     Enabled = false
//                 };
//             }
//
//             return _rabbitOptions;
//         }
//         set => _rabbitOptions = value;
//     }
//
//     #endregion
//
//     #region 中间件
//
//     private MiddlewareOptions _middlewareOptions;
//
//     /// <summary>
//     /// 中间件
//     /// </summary>
//     public MiddlewareOptions MiddlewareOptions
//     {
//         get
//         {
//             if (_middlewareOptions == null)
//             {
//                 _middlewareOptions = new MiddlewareOptions();
//
//                 _middlewareOptions.QuartzNetJob ??= new QuartzNetJob
//                 {
//                     Enabled = false
//                 };
//                 _middlewareOptions.IpLimit ??= new IpLimit
//                 {
//                     Enabled = false
//                 };
//                 _middlewareOptions.MiniProfiler ??= new MiniProfiler
//                 {
//                     Enabled = false
//                 };
//                 _middlewareOptions.RabbitMq ??= new RabbitMq
//                 {
//                     Enabled = false
//                 };
//                 _middlewareOptions.RedisMq ??= new RedisMq
//                 {
//                     Enabled = false
//                 };
//                 _middlewareOptions.Elasticsearch ??= new Elasticsearch
//                 {
//                     Enabled = false
//                 };
//             }
//
//             return _middlewareOptions;
//         }
//         set => _middlewareOptions = value;
//     }
//
//     #endregion
//
//     #region Hmac密钥
//
//     private string _hmacSecret;
//
//     /// <summary>
//     /// Hmac密钥
//     /// </summary>
//     public string HmacSecret
//     {
//         get => _hmacSecret ?? "QjooHE8shwN3FHrS";
//         set => _hmacSecret = value;
//     }
//
//     #endregion
//
//     #region RSA密钥
//
//     private RsaOptions _rsaOptions;
//
//     /// <summary>
//     /// 中间件启动
//     /// </summary>
//     public RsaOptions RsaOptions
//     {
//         get
//         {
//             if (_rsaOptions == null)
//             {
//                 _rsaOptions = new RsaOptions()
//                 {
//                     PrivateKey = "",
//                     PublicKey = ""
//                 };
//             }
//
//             return _rsaOptions;
//         }
//         set => _rsaOptions = value;
//     }
//
//     #endregion
//
//     #region 缓存类型
//
//     private CacheOptions _cacheOptions;
//
//     /// <summary>
//     /// 缓存类型
//     /// </summary>
//     public CacheOptions CacheOptions
//     {
//         get
//         {
//             if (_cacheOptions == null)
//             {
//                 _cacheOptions = new CacheOptions();
//                 _cacheOptions.DistributedCacheSwitch ??= new DistributedCacheSwitch
//                 {
//                     Enabled = true
//                 };
//                 _cacheOptions.RedisCacheSwitch ??= new RedisCacheSwitch
//                 {
//                     Enabled = false
//                 };
//             }
//
//             return _cacheOptions;
//         }
//         set => _cacheOptions = value;
//     }
//
//     #endregion
//
//     #region AOP
//
//     private AopOptions _aopOptions;
//
//     /// <summary>
//     /// AOP
//     /// </summary>
//     public AopOptions AopOptions
//     {
//         get
//         {
//             if (_aopOptions == null)
//             {
//                 _aopOptions = new AopOptions();
//                 _aopOptions.Tran ??= new Tran
//                 {
//                     Enabled = false
//                 };
//                 _aopOptions.Cache ??= new Cache
//                 {
//                     Enabled = false
//                 };
//             }
//
//             return _aopOptions;
//         }
//         set => _aopOptions = value;
//     }
//
//     #endregion
//
//
//     #region 文件上传大小限制
//
//     private long _fileLimitSize;
//
//     /// <summary>
//     /// 文件上传大小限制
//     /// </summary>
//     public long FileLimitSize
//     {
//         get => _fileLimitSize == 0 ? 10 : _fileLimitSize;
//         set => _fileLimitSize = value;
//     }
//
//     #endregion
//
//     #region Swagger
//
//     private SwaggerOptions _swaggerOptions;
//
//     /// <summary>
//     /// Swagger
//     /// </summary>
//     public SwaggerOptions SwaggerOptions
//     {
//         get
//         {
//             if (_swaggerOptions == null)
//             {
//                 _swaggerOptions = new SwaggerOptions()
//                 {
//                     Enabled = false
//                 };
//             }
//
//             return _swaggerOptions;
//         }
//         set => _swaggerOptions = value;
//     }
//
//     #endregion
//
//     #region 输入日志
//
//     private SqlLogOptions _sqlLogOptions;
//
//     /// <summary>
//     /// 输入日志
//     /// </summary>
//     public SqlLogOptions SqlLogOptions
//     {
//         get
//         {
//             if (_sqlLogOptions == null)
//             {
//                 _sqlLogOptions = new SqlLogOptions();
//                 _sqlLogOptions.Enabled = false;
//                 _sqlLogOptions.ToDb ??= new ToDb()
//                 {
//                     Enabled = false
//                 };
//                 _sqlLogOptions.ToFile ??= new ToFile()
//                 {
//                     Enabled = false
//                 };
//                 _sqlLogOptions.ToConsole ??= new ToConsole()
//                 {
//                     Enabled = false
//                 };
//                 _sqlLogOptions.ToElasticsearch ??= new ToElasticsearch()
//                 {
//                     Enabled = false
//                 };
//             }
//
//             return _sqlLogOptions;
//         }
//         set => _sqlLogOptions = value;
//     }
//
//     #endregion
// }
