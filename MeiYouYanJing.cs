using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[BepInPlugin("com.ybang7.lc2.noeyes", "没有眼睛", "1.0.0")]
public class MeiYouYanJing : BasePlugin
{
    public static ManualLogSource Log;
    public ConfigEntry<bool> Enabled;

    GameObject runner;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("没有眼睛 插件加载中...");

        Enabled = Config.Bind("General", "Enabled", true, "启用/禁用 隐藏眼睛 功能（热键 F9 切换）");

        // 创建一个持久的 MonoBehaviour 运行器来做 Update / Scene 事件监听
        runner = new GameObject("MeiYouYanJing_Runner");
        Object.DontDestroyOnLoad(runner);
        var comp = runner.AddComponent<EyeHiderBehaviour>();
        comp.plugin = this;

        Log.LogInfo($"没有眼睛 已初始化（Enabled = {Enabled.Value}），按 F9 切换。");
    }

    // 内部 MonoBehaviour：负责周期扫描、热键、场景加载时触发
    public class EyeHiderBehaviour : MonoBehaviour
    {
        public MeiYouYanJing plugin;
        HashSet<Renderer> hidden = new HashSet<Renderer>();
        float scanInterval = 2.0f;
        float timer = 0f;

        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            timer = 0f;
            if (plugin == null)
            {
                Debug.LogWarning("MeiYouYanJing: plugin 引用为 null");
            }
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            RestoreAll();
        }

        void Update()
        {
            // 热键切换（默认 F9）
            if (Input.GetKeyDown(KeyCode.F9))
            {
                plugin.Enabled.Value = !plugin.Enabled.Value;
                plugin.Config.Save();
                plugin.Log.LogInfo($"没有眼睛: Enabled -> {plugin.Enabled.Value}");
                if (!plugin.Enabled.Value) RestoreAll();
            }

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = scanInterval;
                if (plugin.Enabled.Value) ScanAndHide();
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (plugin.Enabled.Value) ScanAndHide();
        }

        void ScanAndHide()
        {
            // 查找所有 Renderer（包括未激活的），并基于 GameObject 名称匹配关键字隐藏
            var all = Resources.FindObjectsOfTypeAll<Renderer>();
            int found = 0;
            foreach (var r in all)
            {
                if (r == null || r.gameObject == null) continue;
                string name = r.gameObject.name.ToLowerInvariant();
                if (name.Contains("eye") || name.Contains("眼"))
                {
                    found++;
                    if (r.enabled)
                    {
                        r.enabled = false;
                        hidden.Add(r);
                    }
                }
            }
            plugin.Log.LogInfo($"没有眼睛: 扫描完成，检测到可能的眼睛渲染器 {found} 个，已隐藏集合中 {hidden.Count} 个。");
        }

        void RestoreAll()
        {
            int restored = 0;
            foreach (var r in hidden)
            {
                if (r)
                {
                    r.enabled = true;
                    restored++;
                }
            }
            hidden.Clear();
            plugin.Log.LogInfo($"没有眼睛: 恢复了 {restored} 个渲染器。");
        }
    }
}
