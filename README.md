# QMatrix GUI - 本地 LLM 推理框架可视化界面

![Architecture](https://img.shields.io/badge/Architecture-HTTP-blue)
![Core](https://img.shields.io/badge/Core-C%2B%2B-orange)
![Client](https://img.shields.io/badge/Client-C%23-green)
![GUI](https://img.shields.io/badge/GUI-MAUI-purple)
![License](https://img.shields.io/badge/License-MIT-yellow)

QMatrix GUI 是 QMatrix 本地 LLM 推理框架的可视化界面，基于 .NET MAUI 构建，提供现代化的跨平台用户界面，支持 Windows、Web 等平台。

## ✨ 特性

- 🎨 **现代化界面** - 基于 .NET MAUI 的跨平台图形界面
- 🌐 **多平台支持** - Windows 桌面端和 Web 端
- 💬 **实时对话** - 流式输出，实时显示 AI 回复
- 📱 **响应式设计** - 适配不同屏幕尺寸
- 🌍 **多语言支持** - 内置英文、西班牙文、法文、葡萄牙文
- 🎯 **智能提示** - 会话历史管理和智能建议
- 🔧 **配置管理** - 可视化配置界面

## 🚀 快速开始

### 环境要求

- Windows 10/11 (x64)
- Visual Studio 2022
- .NET 8.0 SDK
- MAUI 工作负载
- QMatrix Core 服务

### 运行方式

**方式 1 - VS2022 一键调试：**
1. 确保 QMatrix Core 服务已启动
2. 打开 `QMatrix GUI/QMatrix.GUI/QMatrix.GUI.sln`
3. 按 **F5** 启动

**方式 2 - 命令行：**
```powershell
# 1. 启动 QMatrix Core 服务（在 QMatrix 目录）
cd "f:\QMatrix Program\QMatrix\QMatrix.Core"
build\bin\Release\qmatrix-core.exe

# 2. 启动 GUI 客户端
cd "f:\QMatrix Program\QMatrix GUI\QMatrix.GUI"
dotnet run
```

## 🏗️ 架构

```
┌─────────────────────────────────────────┐
│           Core Service (C++)           │
│  - llama.cpp 推理引擎                   │
│  - HTTP Server (localhost:8888)        │
│  - CUDA / CPU 后端                      │
└─────────────────────────────────────────┘
                    ↑↓ HTTP
┌─────────────────────────────────────────┐
│           GUI Client (MAUI)            │
│  - 现代化图形界面                       │
│  - 流式输出显示                         │
│  - 对话历史管理                         │
│  - 多语言支持                           │
└─────────────────────────────────────────┘
```

## 📋 配置

编辑 `QMatrix GUI/QMatrix.GUI/QMatrix.GUI/appsettings.json`：

```json
{
  "Api": {
    "BaseUrl": "http://localhost:8888",
    "Timeout": 300
  },
  "UI": {
    "Language": "zh-CN",
    "Theme": "System"
  }
}
```

## 🛠️ 项目结构

```
QMatrix GUI/
├── README.md                # 项目说明
├── QMatrix API接口文档.md   # API 文档
├── QMatrix.GUI/             # MAUI 项目
│   ├── QMatrix.GUI.sln      # VS2022 解决方案
│   ├── QMatrix.GUI/         # 主项目
│   │   ├── Assets/          # 资源文件
│   │   ├── Controls/        # 自定义控件
│   │   ├── Converters/      # 数据转换器
│   │   ├── Models/          # 数据模型
│   │   ├── Platforms/       # 平台特定代码
│   │   ├── Services/        # 服务层
│   │   ├── Strings/         # 多语言资源
│   │   ├── Styles/          # 样式定义
│   │   ├── ViewModels/      # 视图模型
│   │   ├── Views/           # 视图
│   │   ├── App.xaml         # 应用入口
│   │   ├── MainPage.xaml    # 主界面
│   │   ├── appsettings.json # 配置文件
│   │   └── QMatrix.GUI.csproj # 项目文件
│   └── global.json          # .NET 版本配置
└── .gitignore               # Git 忽略文件
```

## 💻 使用示例

1. **启动应用** - 运行 GUI 客户端
2. **连接服务** - 确保 Core 服务已启动
3. **开始对话** - 在输入框中输入问题
4. **查看历史** - 左侧面板显示对话历史
5. **切换语言** - 在设置中选择语言

### 示例对话

```
你: 你好，请介绍一下自己

AI: 你好！我是 QMatrix AI 助手，基于本地部署的大型语言模型。
我可以在你的设备上运行，无需联网，保护隐私。

你: 什么是机器学习？

AI: 机器学习是人工智能的一个分支，它使计算机系统能够从数据中学习并改进性能，而无需被显式编程。
```

## 🎨 界面特色

- **现代化设计** - 采用 Fluent Design 风格
- **实时反馈** - 打字机效果显示 AI 回复
- **智能布局** - 自适应不同屏幕尺寸
- **多语言支持** - 内置多语言本地化
- **深色模式** - 支持系统主题自动切换

## 🌍 语言支持

- 🇺🇸 English (en)
- 🇪🇸 Español (es)
- 🇫🇷 Français (fr)
- 🇧🇷 Português (pt-BR)

## 🎯 路线图

- [x] 基础 GUI 界面
- [x] 实时流式输出
- [x] 对话历史管理
- [x] 多语言支持
- [ ] Web 端部署
- [ ] 模型选择界面
- [ ] 高级配置面板
- [ ] 主题自定义
- [ ] 插件系统集成

## 🤝 贡献

欢迎提交 Issue 和 PR！

## 📄 许可证

MIT License

## 🙏 致谢

- [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui) - 跨平台 UI 框架
- [QMatrix Core](https://github.com/yourusername/qmatrix) - 本地 LLM 推理引擎
- [llama.cpp](https://github.com/ggerganov/llama.cpp) - 高性能 LLM 推理
