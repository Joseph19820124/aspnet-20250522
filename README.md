# ASP.NET Core Todo List 应用

一个现代化的Todo List应用，使用ASP.NET Core 8.0构建，连接Supabase PostgreSQL数据库。

## 🚀 功能特性

- ✅ **完整的CRUD操作** - 创建、读取、更新、删除任务
- 🎯 **任务状态管理** - 标记任务为已完成/未完成
- 📱 **响应式设计** - 适配桌面和移动设备
- 🔄 **实时更新** - 即时反映数据变更
- 🛡️ **错误处理** - 完善的异常处理和用户反馈
- 🌐 **RESTful API** - 标准的REST API设计
- 🎨 **现代UI** - 美观的渐变色界面设计

## 🏗️ 技术栈

- **后端**: ASP.NET Core 8.0
- **数据库**: Supabase PostgreSQL
- **ORM**: Entity Framework Core
- **前端**: HTML5, CSS3, JavaScript (原生)
- **API文档**: Swagger/OpenAPI

## 📋 系统要求

- .NET 8.0 SDK
- Supabase账户和项目
- Visual Studio 2022 或 VS Code

## ⚡ 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/Joseph19820124/aspnet-20250522.git
cd aspnet-20250522
```

### 2. 配置Supabase连接

1. 登录到 [Supabase](https://supabase.com)
2. 创建新项目或使用现有项目
3. 在项目设置中找到数据库连接信息
4. 更新 `appsettings.json` 和 `appsettings.Development.json` 中的连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_SUPABASE_HOST;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

**替换以下参数：**
- `YOUR_SUPABASE_HOST`: 你的Supabase数据库主机地址
- `YOUR_PASSWORD`: 你的数据库密码

### 3. 运行应用

```bash
# 恢复依赖包
dotnet restore

# 运行应用
dotnet run
```

应用将在以下地址启动：
- **Web界面**: https://localhost:7000 或 http://localhost:5000
- **API文档**: https://localhost:7000/swagger

## 🗄️ 数据库结构

应用会自动创建 `todos` 表，结构如下：

```sql
CREATE TABLE todos (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    is_completed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);
```

## 🔌 API端点

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/todo` | 获取所有任务 |
| GET | `/api/todo/{id}` | 获取特定任务 |
| POST | `/api/todo` | 创建新任务 |
| PUT | `/api/todo/{id}` | 更新任务 |
| DELETE | `/api/todo/{id}` | 删除任务 |
| PATCH | `/api/todo/{id}/toggle` | 切换任务完成状态 |

### API请求示例

**创建任务：**
```bash
curl -X POST "https://localhost:7000/api/todo" \
     -H "Content-Type: application/json" \
     -d '{
       "title": "学习ASP.NET Core",
       "description": "完成Todo List项目开发"
     }'
```

**获取所有任务：**
```bash
curl -X GET "https://localhost:7000/api/todo"
```

## 🎨 界面预览

- **现代化设计** - 使用渐变色和卡片式布局
- **响应式布局** - 自适应不同屏幕尺寸
- **直观操作** - 一键切换任务状态
- **实时反馈** - 操作结果即时显示

## 📁 项目结构

```
├── Controllers/
│   └── TodoController.cs      # API控制器
├── Data/
│   └── TodoContext.cs         # 数据库上下文
├── Models/
│   └── TodoItem.cs           # 数据模型
├── wwwroot/
│   └── index.html            # 前端界面
├── Program.cs                # 应用入口点
├── TodoApp.csproj           # 项目文件
└── appsettings.json         # 配置文件
```

## 🔧 配置说明

### 环境变量
可以通过环境变量覆盖配置：

```bash
export ConnectionStrings__DefaultConnection="你的连接字符串"
```

### CORS设置
默认配置允许所有来源，生产环境请根据需要调整：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## 🚀 部署指南

### 本地部署
```bash
dotnet publish -c Release
dotnet run --configuration Release
```

### Docker部署
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TodoApp.csproj", "."]
RUN dotnet restore "./TodoApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TodoApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApp.dll"]
```

## 🧪 测试

运行单元测试：
```bash
dotnet test
```

## 📝 开发说明

### 添加新功能
1. 修改 `TodoItem` 模型
2. 更新 `TodoContext` 数据库上下文
3. 扩展 `TodoController` API端点
4. 更新前端界面

### 数据库迁移
```bash
# 添加迁移
dotnet ef migrations add MigrationName

# 更新数据库
dotnet ef database update
```

## 🤝 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/) - Web框架
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) - ORM框架
- [Supabase](https://supabase.com/) - 后端服务平台
- [Swagger](https://swagger.io/) - API文档工具

## 📞 联系方式

如有问题或建议，请通过以下方式联系：

- GitHub Issues: [提交问题](https://github.com/Joseph19820124/aspnet-20250522/issues)
- 项目主页: [aspnet-20250522](https://github.com/Joseph19820124/aspnet-20250522)

---

⭐ 如果这个项目对你有帮助，请给它一个星标！
