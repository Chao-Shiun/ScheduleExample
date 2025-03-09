# Schedule API 多環境配置

這個專案示範如何使用 Docker Compose 實現多環境部署，並通過依賴注入和開放封閉原則實現消息服務的無縫切換。

## 項目架構

- **ScheduleAPI**：.NET Core Web API 專案
- **消息服務**：支援 Redis 和 RabbitMQ 兩種實現
  - 測試環境：使用 Redis
  - 正式環境：使用 RabbitMQ

## 檔案架構

```
ScheduleExample/
│
├── ScheduleAPI/                            # Web API 專案主目錄
│   ├── Controllers/                        # API 控制器
│   │   └── OrderController.cs              # 處理訂單相關請求
│   │
│   ├── Models/                             # 資料模型
│   │   └── OrderModel.cs                   # 訂單資料模型
│   │
│   ├── Services/                           # 服務層
│   │   ├── IMessageService.cs              # 消息服務介面
│   │   ├── RedisMessageService.cs          # Redis實現
│   │   ├── RabbitMQMessageService.cs       # RabbitMQ實現
│   │   └── MessageServiceFactory.cs        # 消息服務工廠
│   │
│   ├── Program.cs                          # 應用程式入口點
│   ├── Startup.cs                          # 應用程式配置
│   ├── ScheduleAPI.csproj                  # 專案檔
│   ├── appsettings.json                    # 基本配置
│   ├── appsettings.Development.json        # 開發環境配置
│   └── appsettings.Production.json         # 生產環境配置
│
├── Dockerfile                              # Docker 建置檔
├── .dockerignore                           # Docker 忽略檔案清單
├── docker-compose.yml                      # 基本Docker Compose配置
├── docker-compose.development.yml          # 開發環境Docker配置
├── docker-compose.production.yml           # 生產環境Docker配置
│
├── ScheduleExample.sln                     # 解決方案檔
└── README.md                               # 說明文件
```

## 檔案功能說明

### 核心應用程式檔案

- **Program.cs**：應用程式入口點，負責初始化Web主機及執行應用程式。
- **Startup.cs**：配置應用程式服務和請求處理管道，包含依賴注入配置。

### 服務實現

- **IMessageService.cs**：定義消息服務的介面，包含發送訊息的方法。
- **RedisMessageService.cs**：使用Redis實現消息服務介面。
- **RabbitMQMessageService.cs**：使用RabbitMQ實現消息服務介面。
- **MessageServiceFactory.cs**：根據配置選擇並創建適當的消息服務實例。

### 配置文件

- **appsettings.json**：包含應用程式的基本配置設定。
- **appsettings.Development.json**：開發環境特定配置，設定Redis連接參數。
- **appsettings.Production.json**：生產環境特定配置，設定RabbitMQ連接參數。

### 建置與部署檔案

- **ScheduleAPI.csproj**：.NET Core專案文件，定義專案的依賴項、目標框架和建置配置。
  ```xml
  <Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
      <TargetFramework>net9.0</TargetFramework>
      <Nullable>enable</Nullable>
      <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>
    <ItemGroup>
      <!-- 相關套件參考，如 Redis、RabbitMQ 客戶端等 -->
    </ItemGroup>
  </Project>
  ```

- **Dockerfile**：定義容器映像的建置步驟，包含：
  - 使用SDK映像進行建置
  - 還原套件並編譯應用程式
  - 使用執行時映像運行應用程式
  ```Dockerfile
  FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
  WORKDIR /src
  COPY ["ScheduleAPI/ScheduleAPI.csproj", "ScheduleAPI/"]
  RUN dotnet restore "ScheduleAPI/ScheduleAPI.csproj"
  COPY . .
  WORKDIR "/src/ScheduleAPI"
  RUN dotnet build "ScheduleAPI.csproj" -c Release -o /app/build

  FROM build AS publish
  RUN dotnet publish "ScheduleAPI.csproj" -c Release -o /app/publish

  FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
  WORKDIR /app
  COPY --from=publish /app/publish .
  ENTRYPOINT ["dotnet", "ScheduleAPI.dll"]
  ```

- **docker-compose.yml**：定義基本的服務配置，包括API服務的基本設定。
  ```yaml
  version: '3.8'
  services:
    schedule-api:
      build: .
      ports:
        - "8080:80"
      environment:
        - ASPNETCORE_ENVIRONMENT=${ENVIRONMENT}
  ```

- **docker-compose.development.yml**：開發環境特定的Docker配置，增加Redis服務。
  ```yaml
  version: '3.8'
  services:
    schedule-api:
      environment:
        - MessageService__Type=Redis
        - MessageService__Redis__Connection=redis:6379
    
    redis:
      image: redis:alpine
      ports:
        - "6379:6379"
  ```

- **docker-compose.production.yml**：生產環境特定的Docker配置，增加RabbitMQ服務。
  ```yaml
  version: '3.8'
  services:
    schedule-api:
      environment:
        - MessageService__Type=RabbitMQ
        - MessageService__RabbitMQ__Host=rabbitmq
        - MessageService__RabbitMQ__Port=5672
        - MessageService__RabbitMQ__Username=guest
        - MessageService__RabbitMQ__Password=guest
    
    rabbitmq:
      image: rabbitmq:3-management
      ports:
        - "5672:5672"
        - "15672:15672"
  ```

- **.dockerignore**：指定Docker建置過程中要忽略的檔案和目錄，優化建置速度和映像大小。
  ```
  **/.vs/
  **/.vscode/
  **/bin/
  **/obj/
  **/node_modules/
  **/*.user
  ```

## 設計原則

1. **依賴注入 (DI)**：通過依賴注入容器註冊和解析消息服務
2. **開放封閉原則 (OCP)**：定義 `IMessageService` 接口，允許擴展新的實現而不修改客戶端代碼
3. **工廠模式**：使用 `MessageServiceFactory` 根據配置創建適當的消息服務實例

## 環境切換

### 配置文件
- `appsettings.json`：基本配置
- `appsettings.Development.json`：開發環境配置（Redis）
- `appsettings.Production.json`：生產環境配置（RabbitMQ）

### Docker Compose 文件
- `docker-compose.yml`：基本服務定義
- `docker-compose.development.yml`：開發環境配置（Redis）
- `docker-compose.production.yml`：生產環境配置（RabbitMQ）

## 啟動指令

### 開發環境（Redis）

```bash
# 設置環境變數
export ENVIRONMENT=Development
# 或在 Windows PowerShell 中
$env:ENVIRONMENT="Development"

# 使用開發環境配置啟動
./start-dev.sh
```

### 生產環境（RabbitMQ）

```bash
# 設置環境變數
export ENVIRONMENT=Production
# 或在 Windows PowerShell 中
$env:ENVIRONMENT="Production"

# 使用生產環境配置啟動
./start-prod.sh
```

## API 端點

- **POST /api/Order**：提交新訂單
  - 請求範例：
    ```json
    {
      "OrderId": "ORD-001",
      "Amount": 100.50
    }
    ```
  - 返回：訂單提交確認

## 擴展

如需添加新的消息服務實現：

1. 實現 `IMessageService` 介面
2. 在 `MessageServiceFactory` 中添加新的實現選擇 