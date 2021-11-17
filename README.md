## 系统说明

- 基于 .Net 5 、SqlSugar、Vue、RBAC、前后端分离的开箱则用的企业级中后台**权限管理系统**
- 无业务逻辑代码入侵，适用于任何 .NET/C# 应用程序。
- 体验地址：  [https://www.apevolo.com](https://www.apevolo.com)
- 开发文档：  [http://doc.apevolo.com](http://doc.apevolo.com)
- 账号密码： `apevolo / 123456`

#### 项目源码

|     |   后端源码  |   前端源码  |
|---  |--- | --- |
|  github   |  https://github.com/xianhc/apevolo-api   |  https://github.com/xianhc/apevolo-web   |

## 模块说明

```lua
ApeVolo.Api         -- 对外API
ApeVolo.IRepository -- 仓储接口
ApeVolo.IRepository -- 仓储实现
ApeVolo.IBusiness   -- 业务接口
ApeVolo.IBusiness   -- 业务实现
ApeVolo.Common      -- 通用工具
ApeVolo.Entity      -- 实体
ApeVolo.QuartzNetService -- 任务调度
```

## 系统特性
- 保持使用最新组件技术栈
- ORM SqlSugar 支持多种主流数据库操作(MySql、Oracle、SqlServer、Sqlite等等)
- 使用 async/await 异步编程
- 使用 仓储+服务+接口 架构模式开发；
- 审计功能，全局异常统一处理
- AOP切面编程，已实现Redis缓存、SQL事务处理
- CodeFirst,项目启动后自动建表并初始化基础数据
- 数据字典，方便地对一些状态进行管理
- 全局设置，方便处理一些功能开关
- 接口限流，避免恶意请求
- 接口权限、数据权限
- 自定义实体注解校验
- 服务器性能监控
- 采用雪花ID，友好的切换各种数据库

## 组件依赖
- JWT 自定义策略授权 
- Automapper 对象映射
- AutoFac 依赖注入
- Redis 缓存,消息队列
- IpRateLimiting 限流
- Swagger文档
- MiniProfiler接口性能分析
- Quartz.Net 任务调度
- Log4Net 日志
- CORS 跨域


### 任务计划
|          功能           | 进度          |
| ---------------------- | ------------- |
| 文件存储服务             |  预计12月      |
| 数据库读写分离            |  预计12月      |
| RabbitMQ 消息队列        |               |

## 快速开始

### 本地开发 运行

下载项目，编译无误，确保appsetting`数据库连接`与`Redis服务`配置正确。直接运系统会自动建数据库表并初始化基础数据，系统默认使用`Mysql`与`Redis`。不想使用的自行更改。

### 本地开发 环境
推荐使用 JetBrains `Rider`、`WebStorm`、`DataGrip`<br/>
电脑配置一般就 `Visual Studio`、`VSCode`、`Navicat Premium`<br/>
均在mac、linux、windows 环境运行测试通过，多系统切换开发时请注意编码格式，不然中文会乱码

## 给个星星! ⭐️
如果你喜欢这个项目或者它能帮助你, 请给 Star 开源不易✨。

## Apache JMeter 压测报告
单机部署 500线程、10000循环往系统单表插入五百万条数据。过程无异常，雪花ID无重复。
![Image text](http://file.apevolo.com/static/CD352CBDB7BE99487450E9DB6A259821.png)
![Image text](http://file.apevolo.com/static/04228304059E32FF91DFE9B44783147B.png)
Please Pull Request~

希望有共同爱好者能帮忙添加一些优秀的内容，为开源做一份贡献~ ^ ^ 快来PR吧~

##

#### 反馈交流
- QQ交流群：839263566


