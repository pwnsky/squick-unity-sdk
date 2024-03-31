using Newtonsoft.Json.Linq;
using Squick;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Rpc;
using Google.Protobuf;
using UnityEditor.Experimental.GraphView;

public class Login : MonoBehaviour
{
    // Start is called before the first frame update
    public Text account;
    public Text password;
    public Button login;

    public GameObject loginPanel;
    public GameObject worldPanel;

    public GameObject scrollViewContent;

    public GameObject buttonPrefab;

    string baseURL = "";

    enum LoginEvent
    {
        LOGIN_FAILED = 0,
    }
    
    void Start()
    {
        // 获取登录服务器的URL
        baseURL = Sqk.Instance.loginURL;
        GameData.loginURL = baseURL;

        // 绑定登录事件
        Sqk.Event.BindEvent("proxy", (int)NetModule.ProxyEvent.AUTH_SUCCESS, OnProxyAuthSuccess);
        Sqk.Net.AddReceiveCallBack((int)PlayerRPC.AckPlayerOnline, OnPlayerOnline);
        Sqk.Net.AddReceiveCallBack((int)PlayerRPC.AckPlayerData, OnAckPlayerData);

        login.onClick.AddListener(() =>
        {
            JObject data = new JObject();
            data.Add("account", account.text);
            data.Add("password", password.text);
            data.Add("device", SystemInfo.deviceName);
            data.Add("type", 0); // type 为0是采用 账号密码登录，详情查看服务端代码 squick/src/node/login/http/struct.h
            data.Add("device_uuid", SystemInfo.deviceUniqueIdentifier);
            data.Add("platform", 0);

            Debug.Log("请求: " + baseURL + "/login");
            HttpRestful.Instance.Post(baseURL + "/login", data.ToString(), new Action<bool, string, UnityWebRequest>((ok, str, request) =>
            {
                if (ok)
                {
                    Debug.Log("Recv: " + str);
                    JObject resJO = JObject.Parse(str);
                    if ((int)resJO["code"] == 0)
                    {
                        string cookie = request.GetResponseHeader("Set-Cookie");
                        Debug.Log("登录成功: Cookie: " + cookie);
                        HttpRestful.SetCookie(cookie); // 设置Cookie
                        GameData.loginCookie = cookie;
                        // 设置cookie

                        GameData.accountID = (string)resJO["account_id"];
                        GameData.proxyKey = (string)resJO["key"];
                        GameData.proxyIP = (string)resJO["ip"];
                        GameData.proxyPort = (int)resJO["port"];
                        GameData.proxyWsPort = (int)resJO["ws_port"];

                        int port = 0;
                        if(Sqk.Instance.rpcProtocolType == RpcProtocolType.TcpSquickRPC)
                        {
                            port = GameData.proxyPort;
                        }else if(Sqk.Instance.rpcProtocolType == RpcProtocolType.WebSocketSquickRPC)
                        {
                            port = GameData.proxyWsPort;
                        }

                        // 连接代理服务器
                        Sqk.Net.Connect(GameData.proxyIP, port, GameData.proxyKey, GameData.accountID, Sqk.Instance.rpcProtocolType);
                    }
                }
                else
                {
                    Debug.Log("网络错误，发送失败");
                }
            }));
        });
    }

    void OnProxyAuthSuccess()
    {
        Debug.Log("代理授权连接成功");
    }

    void OnProxyDisconnected()
    {
        Debug.Log("代理断开连接");
    }

    void OnPlayerOnline(int id, MemoryStream ms)
    {
        Debug.Log(" 玩家在线, 正在拉取玩家数据 ");
        ReqPlayerData req = new ReqPlayerData();
        Sqk.Net.SendMsg((int)PlayerRPC.ReqPlayerData, req.ToByteString());
    }

    void OnAckPlayerData(int id, MemoryStream ms)
    {
        Debug.Log(" 获取到玩家数据 ");

        // 拉取用户数据
        AckPlayerData ack = AckPlayerData.Parser.ParseFrom(ms);
        Debug.Log("Name: " + ack.Name + " Level: " + ack.Level + " playerID: " + ack.PlayerId);

        GameData.name = ack.Name;
        GameData.level = ack.Level;
        GameData.playerID = ack.PlayerId;
        GameData.account = ack.Account;
        GameData.ip = ack.Ip;
        GameData.createdTime = ack.CreatedTime;
        // 拉取玩家信息
        SceneManager.LoadScene("Scenes/Lobby");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
