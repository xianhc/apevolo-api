## 系统说明

- 基于 .Net 8 、SqlSugar ORM、Vue 2.X、RBAC、前后端分离的开箱则用的企业级中后台**权限管理系统**
- 无业务逻辑代码入侵，适用于任何 .NET/C# 应用程序。
- 预览体验：  [https://www.apevolo.com](https://apevolo.com)
- 开发文档：  [http://doc.apevolo.com](http://doc.apevolo.com)
- 账号密码： `apevolo / 123456`

## 项目源码

|     |   后端源码  |   前端源码  |
|---  |--- | --- |
|  Github   |  https://github.com/xianhc/apevolo-api   |  https://github.com/xianhc/apevolo-web   |
|  Gitee   |  https://gitee.com/xianhc/apevolo-api   |  https://gitee.com/xianhc/apevolo-web   |

## 模块说明

| # | 模块功能                      |  项目文件                    | 说明|
|---|-------------------------------|-------------------------------|-------------------------------|
| 1 | Web 控制器 |Ape.Volo.Api | 接口交互层 |
| 2 | 数据库仓储 |Ape.Volo.Repository | 常用的增删查改操作 |
| 3 | 业务接口 |Ape.Volo.IBusiness | 业务接口、DTO传输对象等 |
| 4 | 业务接口实现 |Ape.Volo.Business | 业务具体实现 |
| 5 | 系统通用 |Ape.Volo.Common | 通用的工具类；扩展方法、文件、图像操作等 |
| 6 | 实体 |Ape.Volo.Entity | 数据库实体映射类 |
| 7 | 事件总线 |Ape.Volo.EventBus | 事件总线|
| 8 | 任务调度 |Ape.Volo.QuartzNetService | 系统定时任务实现 |

## 系统特性
- 使用  Async Await 异步编程
- 使用 仓储+服务+接口 架构模式开发
- 使用 SqlSugar ORM 组件, CodeFirst 模式, 封装 BaseService 数据库基础操作类
- 使用Redis与DistributedCache两种缓存并扩展实现SqlSugar二级缓存处理数据
- 使用 Autofac 依赖注入 Ioc 容器, 实现批量自动注入所有服务
- 使用 Swagger UI 自动生成 WebAPI 说明文档
- 使用 Serilog 日志组件(输出到数据库、输出到控制台、输出到文件、输出到Elasticsearch)模式
- 使用 Quartz.Net 封装任务调度中心功能
- 封装异常过滤器  实现统一记录系统异常日志
- 封装审计过滤器  实现统一记录接口请求日志
- 封装缓存拦截器  实现对业务方法结果缓存处理
- 封装事务拦截器  实现对业务方法操作数据库事务处理
- 封装系统appsettings.json配置Configs类
- 重写ASP.NET Core 授权AuthorizationHandler组件  实现自定义授权规则
- 支持多种主流数据库(MySql、SqlServer、Sqlite、Oracle 、 postgresql、达梦、神通数据库、华为 GaussDB)等等；
- 支持RabbitMQ、RedisMQ消息队列
- 支持 CORS 跨域配置
- 支持数据库操作 读写分离、多库、分表
- 支持多租户 ID隔离 、 库隔离
- 支持接口限流 避免恶意请求攻击
- 支持数据权限 (全部、本人、本部门、本部门及以下、自定义)
- 支持数据字典、自定义设置处理

## 组件依赖
- JWT 自定义策略授权 
- AutoMapper 对象映射
- Autofac 依赖注入
- StackexChange.Redis 缓存,消息队列
- DotNetCore.NPOI 处理Excel
- AspNetCoreRateLimit 限流
- Swagger UI 文档
- MiniProfiler.AspNetCore 接口性能分析
- Quartz.Net 任务调度
- Serilog 日志
- RabbitMQ 消息队列
- IP2Region.Net IP库
- SixLabors.ImageSharp 绘图
- Shyjus.BrowserDetector 客户端信息

## 快速开始

#### 环境
推荐使用 `JetBrains Rider`、`WebStorm`<br/>
或者 `Visual Studio`、`VSCode`

#### 运行

1. 下载项目，编译无误。然后启动`Ape.Volo.Api`
2. 系统便会自动创建数据库表并初始化相关基础数据
3. 系统默认使用`Sqlite`数据库与`DistributedCache`缓存

## 支持作者! ⭐️
如果你喜欢这个项目或者它能帮助你, 请帮我在 [Github](https://github.com/xianhc/apevolo-api)或者 [Gitee](https://gitee.com/xianhc/apevolo-api)点个 Star ✨这将是对我极大的鼓励与支持。
希望有共同爱好者能帮忙添加一些优秀的内容，为开源做一份贡献~ ^ ^ 快来PR吧~

## 反馈交流
### QQ群：839263566
| QQ 群 |
|  :---:  |
| <img width="150" src="https://www.apevolo.com/uploads/file/wechat/20230723172503.jpg"> 

### 微信群
| 微信 |
|  :---:  | 
| <img width="150" src="https://www.apevolo.com/uploads/file/wechat/20230723172451.jpg"> 

添加微信，备注"加群"

## 捐赠

如果你觉得这个项目对你有帮助，你可以请作者喝饮料 :tropical_drink: [点我](http://doc.apevolo.com/donate/)

## 致谢

![JetBrains Logo (Main) logo](https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg)