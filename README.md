# Squick Unity SDK

## Intro

The squick Unity SDK

Unity version: 2020.03 above

version: 1.1.1

[Squick Src](https://github.com/pwnsky/squick)



## Feature

Keep it simple, Just a network library.

Surport TCP or Websocket connection with squick

So you can build you project to web applicaiton.



## Quick Start

### Step 1. Modify Login URL & Protocol type

Open project, make sure your **all scenes is loaded in Build Settings**

![image-20240414183421281](./Doc/Images/image-20240414183421281.png)

Open start up scene: **/Assets/Scenes/StartUp.unity**



In Hireachy panel select **Sqk** object. Then you can modify **Login URL** and **Rpc Protocol Type** in Inspector panel. Default infomation as follows.

![image-20240414183307921](./Doc/Images/image-20240414183307921.png)

The **Login URL** is squick login node URL.  this node default http port is 8088

The **Rpc Protocol Type** now surported **TCP Squick RPC** & **Web Socket Squick RPC**, But the **Web Socket Security Squick RPC** now squick not surpport it.



### Step 2. Start project

![image-20240414183307921](./Doc/Images/Snipaste_2024-04-14_18-37-03.png)



### Step 3. Login Succ

Login Succ infomation.

![image-20240414183307921](./Doc/Images/Snipaste_2024-04-14_18-37-21.png)



The server login succ infomation.

![image-20240414183307921](./Doc/Images/Snipaste_2024-04-14_18-37-40.png)

That's all, Just so so!
