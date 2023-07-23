## 系统说明

- 基于 .Net 6 、SqlSugar、Vue、RBAC、前后端分离的开箱则用的企业级中后台**权限管理系统**
- 无业务逻辑代码入侵，适用于任何 .NET/C# 应用程序。
- 预览体验：  [https://www.apevolo.com](https://www.apevolo.com)
- 开发文档：  [http://doc.apevolo.com](http://doc.apevolo.com)
- 账号密码： `apevolo / 123456`

#### 项目源码

|     |   后端源码  |   前端源码  |
|---  |--- | --- |
|  Github   |  https://github.com/xianhc/apevolo-api   |  https://github.com/xianhc/apevolo-web   |
|  Gitee   |  https://gitee.com/xianhc/apevolo-api   |  https://gitee.com/xianhc/apevolo-web   |

## 模块说明

```lua
ApeVolo.Api              -- 对外API
ApeVolo.Repository       -- 仓储层
ApeVolo.IBusiness        -- 业务接口
ApeVolo.Business         -- 业务实现
ApeVolo.Common           -- 通用工具
ApeVolo.Entity           -- 实体
ApeVolo.EventBus         --事件总线
ApeVolo.QuartzNetService -- 任务调度
```

## 系统特性
- 保持使用最新组件技术栈
- ORM SqlSugar 支持多种主流数据库操作(MySql、Oracle、SqlServer、Sqlite等等)
- 使用 async/await 异步编程
- 使用 仓储+服务+接口 架构模式开发；
- 审计功能，全局异常统一处理
- AOP切面编程，已实现缓存、SQL事务
- CodeFirst(优先模式),项目启动后自动建库建表并初始化基础数据
- 数据字典，方便地对一些状态进行管理
- 全局设置，方便处理一些功能开关
- 接口限流，避免恶意请求
- 接口权限、数据权限
- 自定义实体注解校验
- 服务器性能监控
- 雪花ID，友好的切换各种数据库以及分库分表
- 支持数据库读写分离
- 语言本地化

## 组件依赖
- JWT 自定义策略授权 
- Automapper 对象映射
- AutoFac 依赖注入
- Redis 缓存,消息队列
- IpRateLimiting 限流
- Swagger文档
- MiniProfiler接口性能分析
- Quartz.Net 任务调度
- Serilog 日志
- CORS 跨域
- 事件总线(EventBus)
- RabbitMQ消息队列

## 快速开始

### 本地开发 运行

下载项目，编译无误。直接启动`ApeVolo.Api`->系统便会自动创建数据库表并初始化相关基础数据，系统默认使用`Sqlite`数据库与`DistributedCache`缓存。

### 本地开发 环境
推荐使用 JetBrains `Rider`、`WebStorm`<br/>
或者 `Visual Studio`、`VSCode`

## 支持作者! ⭐️
如果你喜欢这个项目或者它能帮助你, 请帮我在 [Github](https://github.com/xianhc/apevolo-api)或者 [Gitee](https://gitee.com/xianhc/apevolo-api)点个 Star ✨这将是对我极大的鼓励与支持。

Please Pull Request~
希望有共同爱好者能帮忙添加一些优秀的内容，为开源做一份贡献~ ^ ^ 快来PR吧~

## 反馈交流
### QQ群：839263566
| QQ 群 |
|  :---:  |
| <img width="150" src="https://www.apevolo.com/file/wechat/20230723172503.jpg"> 

### 微信群
| 微信 |
|  :---:  | 
| <img width="150" src="https://www.apevolo.com/file/wechat/20230723172451.jpg"> 

添加微信，备注"加入apevolo交流群"

## 捐赠

如果你觉得这个项目对你有帮助，你可以请作者喝饮料 :tropical_drink: [点我](http://doc.apevolo.com/donate/)