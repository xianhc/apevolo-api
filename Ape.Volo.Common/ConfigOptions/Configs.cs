using System.Collections.Generic;

namespace Ape.Volo.Common.ConfigOptions;

/// <summary>
/// 全局配置类
/// </summary>
public class Configs
{
    #region 是否开发模式

    private bool? _isQuickDebug;

    /// <summary>
    /// 是否开发模式，生产环境建议设置为False
    /// </summary>
    public bool IsQuickDebug
    {
        get => _isQuickDebug ?? false;
        set => _isQuickDebug = value;
    }

    #endregion

    #region 是否初始DataTable

    private bool? _isInitTable;

    /// <summary>
    /// 是否初始DataTable
    /// </summary>
    public bool IsInitTable
    {
        get => _isInitTable ?? false;
        set => _isInitTable = value;
    }

    #endregion

    #region 是否初始数据

    private bool? _isInitData;

    /// <summary>
    /// 是否初始Data
    /// </summary>
    public bool IsInitData
    {
        get => _isInitData ?? false;
        set => _isInitData = value;
    }

    #endregion

    #region 是否开启读写分离

    private bool? _isCqrs;

    /// <summary>
    /// 是否开发模式
    /// </summary>
    public bool IsCqrs
    {
        get => _isCqrs ?? false;
        set => _isCqrs = value;
    }

    #endregion

    #region 是否记录Sql日志

    private bool? _isSqlLog;

    /// <summary>
    /// 是否记录Sql日志
    /// </summary>
    public bool IsSqlLog
    {
        get => _isSqlLog ?? false;
        set => _isSqlLog = value;
    }

    #endregion

    #region 是否输出sql到控制台

    private bool? _isOutSqlToConsole;

    /// <summary>
    /// 是否开发模式
    /// </summary>
    public bool IsOutSqlToConsole
    {
        get => _isOutSqlToConsole ?? false;
        set => _isOutSqlToConsole = value;
    }

    #endregion

    #region 默认DB

    private string _defaultDataBase;

    /// <summary>
    /// 默认DB
    /// </summary>
    public string DefaultDataBase
    {
        get => _defaultDataBase ?? "Ape.Volo.Sqlite";
        set => _defaultDataBase = value;
    }

    #endregion

    #region 数据库连接对象

    private DataConnection _dataConnection;

    /// <summary>
    ///  数据库连接对象
    /// </summary>
    public DataConnection DataConnection
    {
        get
        {
            if (_dataConnection == null)
            {
                _dataConnection = new DataConnection
                {
                    ConnectionItem = new List<ConnectionItem>()
                };
            }

            return _dataConnection;
        }
        set => _dataConnection = value;
    }

    #endregion

    #region Redis缓存

    private Redis _redis;

    /// <summary>
    ///  Redis缓存
    /// </summary>
    public Redis Redis
    {
        get
        {
            if (_redis == null)
            {
                _redis = new Redis
                {
                    Name = "Ape.Volo.Redis",
                    Host = "localhost",
                    Port = 6379
                };
            }

            return _redis;
        }
        set => _redis = value;
    }

    #endregion

    #region 跨域配置

    private Cors _cors;

    /// <summary>
    ///  跨域配置
    /// </summary>
    public Cors Cors
    {
        get
        {
            if (_cors == null)
            {
                _cors = new Cors
                {
                    EnableAll = false,
                    Name = "",
                    Policy = new List<Policy>()
                };
            }

            return _cors;
        }
        set => _cors = value;
    }

    #endregion

    #region 鉴权订阅人

    private JwtAuthOption _jwtAuthOptions;

    /// <summary>
    ///  鉴权订阅人
    /// </summary>
    public JwtAuthOption JwtAuthOptions
    {
        get
        {
            if (_jwtAuthOptions == null)
            {
                _jwtAuthOptions = new JwtAuthOption
                {
                    Audience = "http://localhost",
                    Issuer = "http://localhost",
                    SecurityKey = "5ixKD0BkJxYYroZTvdPs3w9NWRoiUacN",
                    Expires = 3600,
                    RefreshTokenExpires = 86400,
                    LoginPath = "/auth/login"
                };
            }

            return _jwtAuthOptions;
        }
        set => _jwtAuthOptions = value;
    }

    #endregion

    #region 事件总线

    private EventBus _eventBus;

    /// <summary>
    ///  鉴权订阅人
    /// </summary>
    public EventBus EventBus
    {
        get
        {
            if (_eventBus == null)
            {
                _eventBus = new EventBus
                {
                    Enabled = false,
                    SubscriptionClientName = "ape.volo.event"
                };
            }

            return _eventBus;
        }
        set => _eventBus = value;
    }

    #endregion

    #region Rabbit消息队列

    private Rabbit _rabbit;

    /// <summary>
    ///  Rabbit消息队列
    /// </summary>
    public Rabbit Rabbit
    {
        get
        {
            if (_rabbit == null)
            {
                _rabbit = new Rabbit
                {
                    Enabled = false
                };
            }

            return _rabbit;
        }
        set => _rabbit = value;
    }

    #endregion

    #region 中间件

    private Middleware _middleware;

    /// <summary>
    /// 中间件
    /// </summary>
    public Middleware Middleware
    {
        get
        {
            if (_middleware == null)
            {
                _middleware = new Middleware();

                _middleware.QuartzNetJob ??= new QuartzNetJob
                {
                    Enabled = false
                };
                _middleware.IpLimit ??= new IpLimit
                {
                    Enabled = false
                };
                _middleware.MiniProfiler ??= new MiniProfiler
                {
                    Enabled = false
                };
                _middleware.RabbitMq ??= new RabbitMq
                {
                    Enabled = false
                };
                _middleware.RedisMq ??= new RedisMq
                {
                    Enabled = false
                };
                _middleware.Elasticsearch ??= new Elasticsearch
                {
                    Enabled = false
                };
            }

            return _middleware;
        }
        set => _middleware = value;
    }

    #endregion

    #region Hmac密钥

    private string _hmacSecret;

    /// <summary>
    /// Hmac密钥
    /// </summary>
    public string HmacSecret
    {
        get => _hmacSecret ?? "QjooHE8shwN3FHrS";
        set => _hmacSecret = value;
    }

    #endregion

    #region RSA密钥

    private Rsa _rsa;

    /// <summary>
    /// 中间件启动
    /// </summary>
    public Rsa Rsa
    {
        get
        {
            if (_rsa == null)
            {
                _rsa = new Rsa()
                {
                    PrivateKey = "",
                    PublicKey = ""
                };
            }

            return _rsa;
        }
        set => _rsa = value;
    }

    #endregion

    #region 缓存类型

    private CacheOption _cacheOption;

    /// <summary>
    /// 缓存类型
    /// </summary>
    public CacheOption CacheOption
    {
        get
        {
            if (_cacheOption == null)
            {
                _cacheOption = new CacheOption();
                _cacheOption.DistributedCacheSwitch ??= new DistributedCacheSwitch
                {
                    Enabled = true
                };
                _cacheOption.RedisCacheSwitch ??= new RedisCacheSwitch
                {
                    Enabled = false
                };
            }

            return _cacheOption;
        }
        set => _cacheOption = value;
    }

    #endregion


    #region AOP

    private Aop _aop;

    /// <summary>
    /// AOP
    /// </summary>
    public Aop Aop
    {
        get
        {
            if (_aop == null)
            {
                _aop = new Aop();
                _aop.Tran ??= new Tran
                {
                    Enabled = false
                };
                _aop.Cache ??= new Cache
                {
                    Enabled = false
                };
            }

            return _aop;
        }
        set => _aop = value;
    }

    #endregion

    #region 性能分析

    private bool? _isMiniProfiler;

    /// <summary>
    /// 性能分析
    /// </summary>
    public bool IsMiniProfiler
    {
        get => _isMiniProfiler ?? false;
        set => _isMiniProfiler = value;
    }

    #endregion

    #region 文件上传大小限制

    private long _fileLimitSize;

    /// <summary>
    /// 文件上传大小限制
    /// </summary>
    public long FileLimitSize
    {
        get => _fileLimitSize == 0 ? 10 : _fileLimitSize;
        set => _fileLimitSize = value;
    }

    #endregion

    #region Swagger

    private Swagger _swagger;

    /// <summary>
    /// Swagger
    /// </summary>
    public Swagger Swagger
    {
        get
        {
            if (_swagger == null)
            {
                _swagger = new Swagger()
                {
                    Enabled = false
                };
            }

            return _swagger;
        }
        set => _swagger = value;
    }

    #endregion
}
