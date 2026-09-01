# 没有眼睛 (MeiYouYanJing)

Lost Castle 2 的 BepInEx 插件：隐藏角色 GameObject 名称中含 "eye"/"眼" 的渲染器，从而让角色看起来“没有眼睛”。

功能
- 启动时自动扫描场景并隐藏匹配名称的渲染器（包括 MeshRenderer / SkinnedMeshRenderer / SpriteRenderer）。
- 支持动态发现：在场景加载后或每隔一段时间（默认 2 秒）重新扫描。
- 热键 F9 切换启用/禁用（也会写入 BepInEx 配置文件）。
- 配置项保存在 BepInEx 的配置目录（例如：`BepInEx/config/com.ybang7.lc2.noeyes.cfg`）。

警告与注意
- 本插件通过名称匹配隐藏渲染器，可能会误伤名字中包含关键字的其他对象（例如 UI、道具等）。如果出现误伤，我可以：
  - 改为更严格的匹配（例如只匹配后缀/前缀或只匹配某个父对象路径）。
  - 提供白名单/黑名单关键字配置项。
- 如果需要对特定 Boss/角色只隐藏眼睛（而不是全部匹配对象），请告诉我目标对象的精确名称或提供运行时的日志，我会调整匹配策略。

编译与部署
1. 在安装了 .NET 6 SDK 的机器上打开项目根目录（包含 MeiYouYanJing.csproj）。
2. 执行：
   ```
   dotnet build -c Release
   ```
3. 编译产物位于 `bin/Release/net6.0/MeiYouYanJing.dll`（默认）。
4. 将 DLL 放到游戏目录的 `BepInEx/plugins/` 下，或者使用 csproj 中 `GameDir` 设置的自动拷贝（如果路径正确并且你从该环境构建）。
5. 启动游戏并查看 BepInEx 控制台/日志（`BepInEx/LogOutput.log`），查找 "没有眼睛" 的加载与扫描日志。

调试建议
- 如果眼睛没有被隐藏：在游戏控制台查看日志，确认插件是否加载（应有启动日志）。
- 如果隐藏过多或过少：告诉我示例对象名称，我会调整匹配逻辑或添加配置项来自定义关键字列表或匹配规则。
