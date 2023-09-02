
using UnityEngine;
using System.Collections;
using Squick;
using Unity.VisualScripting;
using Rpc;
using System.IO;
using Google.Protobuf;
using System;

public class Sqk : MonoBehaviour
{
    public string loginURL = "http://127.0.0.1:10088/";
    public string startScene = "Scenes/Login";

    public static NetModule Net = null;
    public static PluginManager pm = null;
    public static ISEventModule Event = null;
    public static Sqk Instance = null;

    public IPluginManager GetPluginManager()
    {
        return pm;
    }

    private void Awake()
    {
        // 创建插件管理器
        pm = new PluginManager();

        Instance = this;
        // 注册SDK插件
        pm.Registered(new SquickPlugin(pm));

        // 获取基本模块
        Net = pm.FindModule<NetModule>();
        Event = pm.FindModule<ISEventModule>();
        pm.Awake();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        pm.Start();
        pm.AfterStart();
    }

    void OnDestroy()
    {
        Debug.Log("Root OnDestroy");
        pm.BeforeDestroy();
        pm.Destroy();
        pm = null;
    }

    void Update()
    {
        long tick = DateTime.Now.Ticks / 10000;
        //Debug.Log("time: " + tick);
        pm.Update(tick);
    }
}
