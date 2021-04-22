using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Reflection;
using System.IO;
using UnityEngine.UI;

namespace LuaFramework {
    public class GameManager : Manager {
        protected static bool initialize = false;
        private List<string> downloadFiles = new List<string>();
        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        void Awake() {
            Init();
            //LuaManager.Require("Model/UIMgr");
            GameObject camera = GameObject.FindWithTag("MainCamera");
            DontDestroyOnLoad(camera);
            CameraMgr.Instance.camera = camera.transform;
            AudioManager.Instance.Init(camera);
            
        }

        void Update()
        {

            //实时更新所有角色的状态
            //更新摄像机跟随
            CameraMgr.Instance.Follow();

            RoleCtrl.Instance.Update();

            
        }

        void FixedUpdate()
        {
            StateMgr.Instance.UpdateState();
        }

        void LateUpdate()
        {
            
        }



        /// <summary>
        /// 初始化
        /// </summary>
        void Init() {
            DontDestroyOnLoad(gameObject);  //防止销毁自己

            CheckExtractResource(); //释放资源
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void CheckExtractResource() {
            bool isExists = Directory.Exists(Util.DataPath) &&
              Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
            if (isExists || AppConst.DebugMode) {
                StartCoroutine(OnUpdateResource());
                return;   //文件已经解压过了，自己可添加检查文件列表逻辑
            }
            StartCoroutine(OnExtractResource());    //启动释放协成 
        }

        IEnumerator OnExtractResource() {
            string dataPath = Util.DataPath;  //数据目录
            string resPath = Util.AppContentPath(); //游戏包资源目录
            
            if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
            Directory.CreateDirectory(dataPath);

            string infile = resPath + "files.txt";
            string outfile = dataPath + "files.txt";
            if (File.Exists(outfile)) File.Delete(outfile);

            string message = "正在解包文件:>files.txt";
            EventManager.Instance.EventTrigger<string>("UpdateResLoadText", message);
            if (Application.platform == RuntimePlatform.Android) {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone) {
                    File.WriteAllBytes(outfile, www.bytes);
                    
                }
                yield return 0;
            } else File.Copy(infile, outfile, true);
            yield return new WaitForEndOfFrame();

            //释放所有文件到数据目录
            string[] files = File.ReadAllLines(outfile);
            float i = 0;
            int length = files.Length;
            foreach (var file in files) {
                i++;
                string[] fs = file.Split('|');
                infile = resPath + fs[0];  //
                outfile = dataPath + fs[0];

                //message = ;
                //Debug.Log("正在解包文件:>" + infile);
                EventManager.Instance.EventTrigger<string>("UpdateResLoadText", "正在解包文件:>" + fs[0]);
                EventManager.Instance.EventTrigger<float>("UpdateResLoadProgress", i/ length);
                facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

                string dir = Path.GetDirectoryName(outfile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (Application.platform == RuntimePlatform.Android) {
                    WWW www = new WWW(infile);
                    yield return www;

                    if (www.isDone) {
                        File.WriteAllBytes(outfile, www.bytes);
                    }
                    yield return 0;
                } else {
                    if (File.Exists(outfile)) {
                        File.Delete(outfile);
                    }
                    File.Copy(infile, outfile, true);
                }
                yield return new WaitForEndOfFrame();
            }
            message = "解包完成!!!";
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
            yield return new WaitForSeconds(0.1f);

            message = string.Empty;
            //释放完成，开始启动更新资源
            StartCoroutine(OnUpdateResource());
        }

        /// <summary>
        /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
        /// </summary>
        IEnumerator OnUpdateResource() {
            if (!AppConst.UpdateMode) {
                OnResourceInited();
                yield break;
            }
            EventManager.Instance.EventTrigger<string>("UpdateResLoadText", "正在更新资源");
            string dataPath = Util.DataPath;  //数据目录
            string url = AppConst.WebUrl;
            string message = string.Empty;
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string listUrl = url + "files.txt?v=" + random;
            Debug.LogWarning("LoadUpdate---->>>" + listUrl);

            WWW www = new WWW(listUrl); yield return www;
            if (www.error != null) {
                OnUpdateFailed(string.Empty);
                yield break;
            }
            if (!Directory.Exists(dataPath)) {
                Directory.CreateDirectory(dataPath);
            }
            File.WriteAllBytes(dataPath + "files.txt", www.bytes);
            string filesText = www.text;
            string[] files = filesText.Split('\n');
            float index = 0;
            for (int i = 0; i < files.Length; i++) {
                index++;
                if (string.IsNullOrEmpty(files[i])) continue;
                string[] keyValue = files[i].Split('|');
                string f = keyValue[0];
                string localfile = (dataPath + f).Trim();
                string path = Path.GetDirectoryName(localfile);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                string fileUrl = url + f + "?v=" + random;
                bool canUpdate = !File.Exists(localfile);
                if (!canUpdate) {
                    string remoteMd5 = keyValue[1].Trim();
                    string localMd5 = Util.md5file(localfile);
                    canUpdate = !remoteMd5.Equals(localMd5);
                    if (canUpdate) File.Delete(localfile);
                }
                if (canUpdate) {   //本地缺少文件
                    Debug.Log(fileUrl);
                    message = "downloading>>" + fileUrl;
                    EventManager.Instance.EventTrigger<string>("UpdateResLoadText", message);
                    EventManager.Instance.EventTrigger<float>("UpdateResLoadProgress", index / files.Length);
                    facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
                    /*
                    www = new WWW(fileUrl); yield return www;
                    if (www.error != null) {
                        OnUpdateFailed(path);   //
                        yield break;
                    }
                    File.WriteAllBytes(localfile, www.bytes);
                     */
                    //这里都是资源文件，用线程下载
                    BeginDownload(fileUrl, localfile);
                    while (!(IsDownOK(localfile))) { yield return new WaitForEndOfFrame(); }
                }
            }
            yield return new WaitForEndOfFrame();
            
            message = "更新完成!!";
            EventManager.Instance.EventTrigger<string>("UpdateResLoadText", "更新完成");
            EventManager.Instance.EventTrigger<float>("UpdateResLoadProgress", 1);
            EventManager.Instance.RemoveListener("UpdateResLoadText");
            EventManager.Instance.RemoveListener("UpdateResLoadProgress");
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);

            OnResourceInited();
        }

        void OnUpdateFailed(string file) {
            string message = "更新失败!>" + file;
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
        }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        bool IsDownOK(string file) {
            return downloadFiles.Contains(file);
        }

        /// <summary>
        /// 线程下载
        /// </summary>
        void BeginDownload(string url, string file) {     //线程下载
            object[] param = new object[2] { url, file };

            ThreadEvent ev = new ThreadEvent();
            ev.Key = NotiConst.UPDATE_DOWNLOAD;
            ev.evParams.AddRange(param);
            ThreadManager.AddEvent(ev, OnThreadCompleted);   //线程下载
        }

        /// <summary>
        /// 线程完成
        /// </summary>
        /// <param name="data"></param>
        void OnThreadCompleted(NotiData data) {
            switch (data.evName) {
                case NotiConst.UPDATE_EXTRACT:  //解压一个完成
                //
                break;
                case NotiConst.UPDATE_DOWNLOAD: //下载一个完成
                downloadFiles.Add(data.evParam.ToString());
                break;
            }
        }

        /// <summary>
        /// 资源初始化结束
        /// </summary>
        public void OnResourceInited() {
#if ASYNC_MODE
            ResManager.Initialize(AppConst.AssetDir, delegate() {
                Debug.Log("Initialize OK!!!");
                this.OnInitialize();
            });
#else
            ResManager.Initialize();
            this.OnInitialize();
#endif
        }

        void OnInitialize() {
            LuaManager.InitStart();
            LuaManager.DoFile("Logic/Game");         //加载游戏
            //LuaManager.DoFile("Logic/Network");      //加载网络
            //NetManager.OnInit();                     //初始化网络
            Util.CallMethod("Game", "OnInitOK");     //初始化完成

            initialize = true;

            ConfigManager.Instance.InitConfig();

            //TableManager.Instance.Init();  //加载游戏中需要的配置表

            //Debugger.Log(stack.Count);

            //类对象池测试
            //var classObjPool = ObjPoolManager.CreatePool<TestObjectClass>(OnPoolGetElement, OnPoolPushElement);
            //方法1
            //objPool.Release(new TestObjectClass("abcd", 100, 200f));
            //var testObj1 = objPool.Get();

            //方法2
            //ObjPoolManager.Release<TestObjectClass>(new TestObjectClass("abcd", 100, 200f));
            //var testObj1 = ObjPoolManager.Get<TestObjectClass>();

            //Debugger.Log("TestObjectClass--->>>" + testObj1.ToString());

            //游戏对象池测试
            //var prefab = Resources.Load("TestGameObjectPrefab", typeof(GameObject)) as GameObject;
            //var gameObjPool = ObjPoolManager.CreatePool("TestGameObject", 5, 10, prefab);

            //var gameObj = Instantiate(prefab) as GameObject;
            //gameObj.name = "TestGameObject_01";
            //gameObj.transform.localScale = Vector3.one;
            //gameObj.transform.localPosition = Vector3.zero;

            //ObjPoolManager.Release("TestGameObject", gameObj);
            //var backObj = ObjPoolManager.Get("TestGameObject");
            //backObj.transform.SetParent(null);

            //Debug.Log("TestGameObject--->>>" + backObj);
        }

        /// <summary>
        /// 当从池子里面获取时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolGetElement(TestObjectClass obj) {
            Debug.Log("OnPoolGetElement--->>>" + obj);
        }

        /// <summary>
        /// 当放回池子里面时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolPushElement(TestObjectClass obj) {
            Debug.Log("OnPoolPushElement--->>>" + obj);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy() {
            if (NetManager != null) {
                NetManager.Unload();
            }
            if (LuaManager != null) {
                LuaManager.Close();
            }
            Debug.Log("~GameManager was destroyed");
        }
    }
}