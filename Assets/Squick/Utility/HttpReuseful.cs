using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
/*
    参考：https://cloud.tencent.com/developer/article/1696658
 */
public class HttpRestful : MonoBehaviour
{
    private static HttpRestful _instance;
    private static string cookie = "";

    public static HttpRestful Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject goRestful = new GameObject("HttpRestful");
                _instance = goRestful.AddComponent<HttpRestful>();
            }
            return _instance;
        }
    }

    #region Get请求
    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="actionResult"></param>
    /// 
    public static void SetCookie(string newCookie)
    {
        cookie = newCookie;
    }
    public void Get(string url, Action<bool, string, UnityWebRequest> actionResult = null)
    {
        StartCoroutine(_Get(url, actionResult));
    }

    private IEnumerator _Get(string url, Action<bool, string, UnityWebRequest> action)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {

            if(cookie != "" && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                request.SetRequestHeader("Cookie", cookie);
            }

            yield return request.SendWebRequest();

            string resstr = "";
            bool isSuccess = false;
            if (UnityWebRequest.Result.Success != request.result)
            {
                resstr = request.error;
            }
            else
            {
                resstr = request.downloadHandler.text;
                isSuccess = true;
            }

            if (action != null)
            {
                action(isSuccess, resstr, request);
            }
        }
    }
    #endregion


    #region POST请求
    public void Post(string url, string data, Action<bool, string, UnityWebRequest> actionResult = null)
    {
        StartCoroutine(_Post(url, data, actionResult));
    }

    private IEnumerator _Post(string url, string data, Action<bool, string, UnityWebRequest> action)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            
            if (cookie != "" && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                request.SetRequestHeader("Cookie", cookie);
            }
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            string resstr = "";
            bool isSuccess = false;
            if (UnityWebRequest.Result.Success != request.result)
            {
                resstr = request.error;
            }
            else
            {
                resstr = request.downloadHandler.text;
                isSuccess = true;
            }

            if (action != null)
            {
                action(isSuccess, resstr, request);
            }
        }
    }
    #endregion
}