# Squick Unity SDK

squick Unity引擎SDK

Unity版本: 2020.03 以上

打开场景: Scenes/StartUp 运行

version: 1.0.4

[服务端 1.0.4](https://github.com/pwnsky/squick)



## 功能

### 登录



### 连接代理服务器



### 逻辑交互





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



