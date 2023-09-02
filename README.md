# Squick Unity SDK

squick Unity引擎SDK



## 功能

### 登录



### 连接代理服务器



### 逻辑交互

Version: 1.0

[squick](https://github.com/pwnsky/squick)





## 项目结构介绍



```
Assets/
├── Lib # 常用库，放json和protobuf序列化相关的dll
│   ├── Newtonsoft.Json
│   └── Protobuf
├── Prefab # demo中用到的预制体
├── Proto  # 根据proto文件生成的protobuf代码文件, 由后端squick工具生成
├── Scenes # demo 中所用到的场景
├── Script # demo 中的脚本
├── Squick # squick sdk 核心代码
│   ├── Base
│   ├── Core
│   │   └── Math
│   ├── Plugin
│   │   ├── Event
│   │   └── Net
│   ├── PluginManager
│   └── Utility
```

