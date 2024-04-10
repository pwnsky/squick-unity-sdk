using Rpc;
using Squick;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    // Start is called before the first frame update
    public Text info;
    void Start()
    {
        string showText = "Account: " + GameData.account + "\nAccountID: " + GameData.accountID;
        showText += "\nUID: " + GameData.uid + "\nName: " + GameData.name + "\nLevel: " + GameData.level;
        showText += "\nCreatedTime: " + GameData.createdTime;
        showText += "\nClientIP: " + GameData.ip;
        showText += "\nProxyHost: " + GameData.proxyIP + ": " + GameData.proxyPort;
        showText += "\n\nProxy connect key: " + GameData.proxyKey;
        showText += "\nLogin Url: " + GameData.loginURL;
        showText += "\nLogin Cookie: " + GameData.loginCookie;
        info.text = showText;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
