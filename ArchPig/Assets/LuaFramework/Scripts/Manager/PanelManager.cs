using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using LuaInterface;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;

namespace LuaFramework
{
    public enum ControlType
    {
        Text = 1,
        Image = 2,
        Button = 3,
        Slider = 4,
        Toggle = 5,
        InputField = 6,
        Scrollbar = 7,
        RawImage = 8
    }

    public class PanelManager : Manager
    {

        private GameObject canvas;  //画布
        private GameObject top;  //UI最上层,显示优先级最高
        private GameObject middle;  //UI中层，显示优先级中
        private GameObject bottom;  //UI底层，显示优先级最低
        private Transform parent;  //UILayer对应的父节点
        private byte UILayer;  //UI层级
        private byte panelType; //面板类型,1表示普通面板，2表示需要遮罩的面板，3表示提示面板(点击遮罩住即关闭当前面板) 
        private bool isMask;

        //<panelName,GameObject>用于存储当前场景中所存在的面板
        private Dictionary<string, GameObject> panelsDic = new Dictionary<string, GameObject>();

        //按面板层级存储显示状态的面板
        private Stack<GameObject> panelsStack = new Stack<GameObject>();

        //以onlyid为键存储所有加载了的面板
        //private Dictionary<string, GameObject> panelsDic = new Dictionary<string, GameObject>();

        //<panelName<ctrlPath,UICtrl>>  以面板为单位存储面板下的可使用控件
        //private Dictionary<string, Dictionary<string, UIBehaviour>> ctrlsDic = new Dictionary<string, Dictionary<string, UIBehaviour>>();

        //用于存储各个面板下的UI控件
        //key为面板名，value为各个UI控件名称对应的所有UI控件
        //private Dictionary<string, Dictionary<string, List<UIBehaviour>>> controlsDic = new Dictionary<string, Dictionary<string, List<UIBehaviour>>>();
        Transform Parent
        {
            get
            {
                if (parent == null)
                {
                    canvas = GameObject.FindWithTag("Canvas");
                    bottom = GameObject.FindWithTag("bottom");
                    middle = GameObject.FindWithTag("middle");
                    top = GameObject.FindWithTag("top");
                    if (canvas != null) canvas.transform.SetParent(null);
                    if (bottom != null) bottom.transform.SetParent(canvas.transform);
                    if (middle != null) middle.transform.SetParent(canvas.transform);
                    if (top != null) top.transform.SetParent(canvas.transform);

                    DontDestroyOnLoad(canvas);
                    DontDestroyOnLoad(GameObject.Find("EventSystem"));

                }
                switch (UILayer)
                {
                    case 1:
                        parent = bottom.transform;
                        break;
                    case 2:
                        parent = middle.transform;
                        break;
                    case 3:
                        parent = top.transform;
                        break;
                    default:
                        Debugger.LogError($"------------找不到{UILayer}UI层级--------");
                        break;
                }
                return parent;
            }
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        /// <param name="name">面板名称</param>
        /// <param name="UILayer">UI层级</param>
        /// <param name="panelType">面板类型 1:没有遮罩的面板,2:带有遮罩的面板,3:带有遮罩且点击遮罩自动关闭的面板</param>
        /// <param name="func">创建之后执行的函数</param>
        public void OpenPanel(string name, byte UILayer, byte panelType, LuaFunction func = null)
        {
            switch (panelType)
            {
                //没有遮罩的面板
                case 1:
                    break;
                //带有普通遮罩的面板
                case 2:
                    LuaHelper.GetObjectPoolManager().Get("MaskPanel", (go) =>
                    {
                        AddMaskPanel(ref go, "MaskPanel",UILayer);
                    });
                    break;
                //点击遮罩自动关闭的提示面板
                case 3:
                    LuaHelper.GetObjectPoolManager().Get("TipMaskPanel", (go) =>
                    {
                        AddMaskPanel(ref go, "TipMaskPanel",UILayer);
                        go.transform.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            ClosePanel();
                            //go.transform.GetComponent<Button>().onClick.RemoveAllListeners();
                        });
                    });
                    break;
            }

            //加载面板
            string onlyId = string.Empty;
            LuaHelper.GetObjectPoolManager().Get(name, (go) =>
            {
                go.name = name;
                
                //判断该面板是否存在
                if (!panelsDic.ContainsKey(name))
                {
                    CreatePanel(ref go, onlyId);
                }
                //执行lua脚本中的Show方法
                
                PushPanel(go);
                this.UILayer = UILayer;
                go.transform.SetParent(Parent);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                Util.CallMethod(go.name, "Show", LuaManager.GetTable(go.name));
                if (func != null) func.Call(go);
            });
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        /// <param name="go"></param>
        /// <param name="onlyId"></param>
        void CreatePanel(ref GameObject go, string onlyId)
        {
            //调用面板对应的控制器脚本的OnCreate方法
            string ctrlName = go.name.Substring(0, go.name.Length - 5) + "Ctrl";
            Util.CallMethod(ctrlName, "OnCreate", LuaManager.GetTable(ctrlName),go);
            //加载面板对应的视图层脚本
            LuaManager.Require("View/" + go.name);
            //获取面板控件表
            LuaTable ctrls = LuaManager.GetTable(go.name + ".Ctrls");
            LuaDictTable ctrlDic = ctrls.ToDictTable();
            if (ctrlDic.Count() > 0)
            {
                Dictionary<string, UIBehaviour> ctrlsDic = new Dictionary<string, UIBehaviour>();
                //遍历面板表下所有控件,将控件存进面板
                foreach (var ctrl in ctrlDic)
                {
                    string path = (string)LuaManager.GetTable(go.name + ".Ctrls." + ctrl.Key as string)["Path"];
                    string type = (string)LuaManager.GetTable(go.name + ".Ctrls." + ctrl.Key as string)["ControlType"];
                    var control = go.transform.Find(path).GetComponent(type);
                    if (control == null)
                    {
                        Debugger.LogError($"{go.name}该物体下的{path}不存在控件");
                        continue;
                    }
                    if (ctrlsDic.ContainsKey(ctrl.Key as string))
                    {
                        Debugger.Log($"{ctrl.Key as string}已存在");
                        continue;
                    }
                    ctrlsDic.Add(ctrl.Key as string, (UIBehaviour)control);
                }
                LuaTable UIMgr = LuaManager.GetTable("UIMgr");
                LuaManager.GetFunction("UIMgr.InitCtrl").LazyCall(UIMgr,go.name, ctrlsDic);
                ctrlsDic.Clear();
            }
            //执行面板Lua脚本中的Awake方法
            Util.CallMethod(go.name, "Awake", LuaManager.GetTable(go.name), go);
            //将面板以句柄为key添加进字典
            panelsDic.Add(go.name, go);
        }

        /// <summary>
        /// 添加遮罩面板
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        void AddMaskPanel(ref GameObject go, string name,byte UILayer)
        {
            this.UILayer = UILayer;
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            go.transform.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            go.transform.GetComponent<RectTransform>().anchorMax = Vector2.one;
            go.name = name;
            PushPanel(go);
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="name"></param>
        public void ClosePanel()
        {
            GameObject go = panelsStack.Pop();
            Util.CallMethod(go.name, "Hide", LuaManager.GetTable(go.name));
            LuaHelper.GetObjectPoolManager().Set(go.name, go);
            if (panelsStack.Count < 1)
                return;
            GameObject mask = panelsStack.Peek();
            if (mask.tag == "mask")
            {
                if (mask.name == "TipMaskPanel")
                {
                    mask.GetComponent<Button>().onClick.RemoveAllListeners();
                }
                mask = panelsStack.Pop();
                LuaHelper.GetObjectPoolManager().Set(mask.name, mask);
            }

            

            //var panelName = name + "Panel";
            //var panelObj = Parent.Find(panelName);
            //if (panelObj == null) return;
            //Destroy(panelObj.gameObject);
        }

        public void ClearPanel()
        {
            while (panelsStack.Count > 0)
            {
                ClosePanel();
            }
        }

        //添加面板
        private void PushPanel(GameObject panel)
        {
            panelsStack.Push(panel);
            //panelsDic.Add(name,panel);
        }




        //public UIBehaviour GetControl(LuaTable table)
        //{
        //    LuaDictTable dic = table.ToDictTable();
            
        //    string panel = (string)dic["Panel"];
        //    string path = (string)dic["Path"];
        //    string type = (string)dic["ControlType"];
        //    if (!ctrlsDic.TryGetValue(panel, out Dictionary<string, UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"{panel}该面板下不存在可使用的控件");
        //        return null;
        //    }
        //    if (!ctrlsDic[panel].TryGetValue(path+type, out UIBehaviour ctrl))
        //    {
        //        Debugger.LogError($"{panel}该面板下的{path}不存在物体");
        //        return null;
        //    }
        //    if (ctrl == null)
        //    {
        //        Debugger.LogError($"{panel}该面板下的{path}不存在物体");
        //        return null;
        //    }
        //    return ctrl;

        //}

        public void ButtonAddListener(Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public void ButtonRemoveListener(Button button)
        {
            button.onClick.RemoveAllListeners();
        }


        //public void ButtonAddListener(LuaTable table, UnityAction action)
        //{
        //    Button button = GetControl(table) as Button;
        //    if (button == null)
        //    {
        //        Debugger.LogError("button不存在");
        //        return;
        //    }
        //    button.onClick.AddListener(action);
        //}

        public void ScrollRectAddListener(ScrollRect scrollRect,UnityAction<Vector2> action)
        {
            scrollRect.onValueChanged.AddListener(action);
        }

        public void ScrollRectRemoveListener(ScrollRect scrollRect)
        {
            scrollRect.onValueChanged.RemoveAllListeners();
        }

        public void ToggleAddListener(Toggle toggle, UnityAction<bool> action)
        {
            toggle.onValueChanged.AddListener(action);
        }

        public void ToggleRemoveListener(Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }

        //public void ToggleAddListener(LuaTable table, UnityAction<bool> action)
        //{
        //    Toggle toggle = GetControl(table) as Toggle;
        //    if (toggle == null)
        //    {
        //        Debugger.LogError("toggle按钮不存在");
        //        return;
        //    }
        //    toggle.onValueChanged.AddListener(action);
        //}

        public void SliderAddListener(Slider slider, UnityAction<float> action)
        {
            slider.onValueChanged.AddListener(action);
            
        }

        public void SliderRemoveListener(Slider slider)
        {
            slider.onValueChanged.RemoveAllListeners();
        }

        public void InputAddListener(InputField input,UnityAction<string> action)
        {
            input.onValueChanged.AddListener(action);
        }

        public void InputRemoveListener(InputField input)
        {
            input.onValueChanged.RemoveAllListeners();   
        }

        //public void SliderAddListener(LuaTable table, UnityAction<float> action)
        //{
        //    Slider slider = GetControl(table) as Slider;
        //    if (slider == null)
        //    {
        //        Debugger.LogError("slider不存在");
        //        return;
        //    }
        //    slider.onValueChanged.AddListener(action);
        //}

        //public void AddListenerInputField(LuaTable table, UnityAction<string> action)
        //{
        //    InputField inputField = GetControl(table) as InputField;
        //    if (inputField == null)
        //    {
        //        Debugger.LogError("inputfield不存在");
        //        return;
        //    }
        //    inputField.onValueChanged.AddListener(action);
        //}

        public void ChangeSprite(Image image, Sprite sprite)
        {
            image.sprite = sprite;
        }

        //public void ChangeSprite(LuaTable table, Sprite sprite)
        //{
        //    Image image = GetControl(table) as Image;
        //    if (image == null)
        //    {
        //        Debugger.LogError("image不存在");
        //        return;
        //    }
        //    image.sprite = sprite;
        //}

        public void ChangeText(Text text, string context)
        {
            text.text = context;
        }

        //public void ChangeText(LuaTable table, string context)
        //{
        //    Text text = GetControl(table) as Text;
        //    if (text == null)
        //    {
        //        Debugger.LogError("text不存在");
        //        return;
        //    }
        //    text.text = context;
        //}

        public void ChangeScale(Image image, Vector2 scale)
        {
            image.transform.localScale = scale;
        }

        //public void ChangeScale(LuaTable table, Vector2 scale)
        //{
        //    Image image = GetControl(table) as Image;
        //    if (image == null)
        //    {
        //        Debugger.LogError($"控件不存在");
        //        return;
        //    }
        //    image.transform.localScale = scale;
        //}

        public void FullScreen(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(0,0);
            rect.offsetMax = new Vector2(0,0);
        }



        /// <summary>
        /// 添加鼠标监测相关事件
        /// </summary>
        /// <param name="transform">需要添加事件的对象</param>
        /// <param name="action">事件具体行为</param>
        /// <param name="triggerType">事件类型</param>
        public void AddPointerEvent(Transform transform, int triggerType, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = transform.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = transform.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = (EventTriggerType)triggerType;
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// 转换鼠标指针
        /// </summary>
        /// <param name="baseEventData"></param>
        /// <returns></returns>
        public PointerEventData ChangeEventDate(BaseEventData baseEventData)
        {
            if (!(baseEventData is PointerEventData))
            {
                Debugger.Log($"--------------转换鼠标指针失败--------------");
                return null;
            }

            return baseEventData as PointerEventData;
        }



        //=============================================以下代码已废弃======================================


        #region 初始化面板控件
        /// <summary>
        /// 初始化面板，获取面板下对应类型的控件并存入控件字典
        /// </summary>
        //void InitPanel(GameObject panel)
        //{
        //    FindControls<Button>(panel);
        //    FindControls<InputField>(panel);
        //    FindControls<Image>(panel);
        //    FindControls<Text>(panel);
        //    FindControls<Slider>(panel);
        //    FindControls<Toggle>(panel);
        //    FindControls<RawImage>(panel);

        //}

        #endregion

        #region  获取某种类型的所有控件

        /// <summary>
        /// 遍历某种类型控件并存进字典
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        //void FindControls<T>(GameObject panel) where T : UIBehaviour
        //{
        //    T[] controls = panel.GetComponentsInChildren<T>();
        //    if (controls.Length < 1)
        //        return;
        //    string controlName;
        //    for (int i = 0; i < controls.Length; i++)
        //    {
        //        controlName = controls[i].name;
        //        if (!controlName.StartsWith("Auto_"))
        //            continue;

        //        if(!controlsDic.TryGetValue(panel.name,out Dictionary<string, List<UIBehaviour>> ctrlNames))
        //            controlsDic.Add(panel.name, new Dictionary<string, List<UIBehaviour>>());

        //        if(!controlsDic[panel.name].TryGetValue(controlName,out List<UIBehaviour> ctrls))
        //            controlsDic[panel.name].Add(controlName, new List<UIBehaviour>());

        //        controlsDic[panel.name][controlName].Add(controls[i]);
        //if (controlDic.TryGetValue(controlName, out controlList))
        //{
        //    controlList.Add(controls[i]);
        //}
        //else
        //{
        //    controlDic.Add(controlName, new List<UIBehaviour>() { controls[i] });
        //}
        //    }
        //}

        #endregion

        #region  获取控件
        /// <summary>
        /// 根据名字获取当前面板下的对应子控件
        /// </summary>
        /// <typeparam name="T">指定控件类型</typeparam>
        /// <param name="name">指定控件名字</param>
        /// <returns></returns>
        //public T GetControl<T>(string panelName,string controlName) where T : UIBehaviour
        //{
        //    if(!controlsDic.TryGetValue(panelName,out Dictionary<string ,List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if(!controlsDic[panelName].TryGetValue(controlName,out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach( UIBehaviour ctrl in ctrls)
        //    {
        //        if(ctrl is T)
        //        {
        //            return ctrl as T;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;

        //    //List<UIBehaviour> controlList = null;
        //    //if (controlDic.TryGetValue(name, out controlList))
        //    //{
        //    //    foreach (UIBehaviour control in controlList)
        //    //    {
        //    //        if (control is T)
        //    //        {
        //    //            return (control as T);
        //    //        }
        //    //    }
        //    //}
        //    //Debugger.LogError($"-----------找不到{name}控件-----------");
        //    //return null;
        //}

        ///// <summary>
        ///// 获取Button
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public Button GetButton(string panelName,string controlName)
        //{
        //    if (!controlsDic.TryGetValue(panelName, out Dictionary<string, List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if (!controlsDic[panelName].TryGetValue(controlName, out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach (UIBehaviour ctrl in ctrls)
        //    {
        //        if (ctrl is Button)
        //        {
        //            return ctrl as Button;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;
        //    //List<UIBehaviour> controlList = null;
        //    //if (controlDic.TryGetValue(name, out controlList))
        //    //{
        //    //    foreach (UIBehaviour control in controlList)
        //    //    {
        //    //        if (control is Button)
        //    //        {
        //    //            return (control as Button);
        //    //        }
        //    //    }
        //    //}
        //    //Debugger.LogError($"-----------找不到{name}-----------");
        //    //return null;
        //}

        ///// <summary>
        ///// 获取Toggle
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public Toggle GetToggle(string panelName,string controlName)
        //{
        //    if (!controlsDic.TryGetValue(panelName, out Dictionary<string, List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if (!controlsDic[panelName].TryGetValue(controlName, out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach (UIBehaviour ctrl in ctrls)
        //    {
        //        if (ctrl is Toggle)
        //        {
        //            return ctrl as Toggle;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;
        //}

        ///// <summary>
        ///// 获取Slider
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public Slider GetSlider(string panelName,string controlName)
        //{
        //    if (!controlsDic.TryGetValue(panelName, out Dictionary<string, List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if (!controlsDic[panelName].TryGetValue(controlName, out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach (UIBehaviour ctrl in ctrls)
        //    {
        //        if (ctrl is Slider)
        //        {
        //            return ctrl as Slider;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;
        //}

        ///// <summary>
        ///// 获取InputField
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public InputField GetInputField(string panelName,string controlName)
        //{
        //    if (!controlsDic.TryGetValue(panelName, out Dictionary<string, List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if (!controlsDic[panelName].TryGetValue(controlName, out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach (UIBehaviour ctrl in ctrls)
        //    {
        //        if (ctrl is InputField)
        //        {
        //            return ctrl as InputField;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;
        //}

        ///// <summary>
        ///// 获取Image
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public Image GetImage(string panelName,string controlName)
        //{
        //    if (!controlsDic.TryGetValue(panelName, out Dictionary<string, List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if (!controlsDic[panelName].TryGetValue(controlName, out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach (UIBehaviour ctrl in ctrls)
        //    {
        //        if (ctrl is Image)
        //        {
        //            return ctrl as Image;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;
        //}

        ///// <summary>
        ///// 获取Image
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public RawImage GetRawImage(string panelName, string controlName)
        //{
        //    if (!controlsDic.TryGetValue(panelName, out Dictionary<string, List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if (!controlsDic[panelName].TryGetValue(controlName, out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach (UIBehaviour ctrl in ctrls)
        //    {
        //        if (ctrl is RawImage)
        //        {
        //            return ctrl as RawImage;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;
        //}

        ///// <summary>
        ///// 获取Text
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public Text GetText(string panelName,string controlName)
        //{
        //    if (!controlsDic.TryGetValue(panelName, out Dictionary<string, List<UIBehaviour>> ctrlsName))
        //    {
        //        Debugger.LogError($"-----------------找不到{panelName}----------------");
        //        return null;
        //    }

        //    if (!controlsDic[panelName].TryGetValue(controlName, out List<UIBehaviour> ctrls))
        //    {
        //        Debugger.LogError($"-----------------找不到{controlName}----------------");
        //        return null;
        //    }

        //    foreach (UIBehaviour ctrl in ctrls)
        //    {
        //        if (ctrl is Text)
        //        {
        //            return ctrl as Text;
        //        }
        //    }

        //    Debugger.LogError($"-----------------找不到{controlName}----------------");
        //    return null;
        //}

        #endregion

        #region  给控件添加事件

        ///// <summary>
        ///// 给按钮添加监听事件
        ///// </summary>
        ///// <param name="name">按钮名称</param>
        ///// <param name="action">所要执行的行为</param>
        //public void ButtonAddListener(string panelName,string controlName, UnityAction action)
        //{
        //    //Button button = GetControl<Button>(name);
        //    Button button = GetButton(panelName,controlName);
        //    if (button == null)
        //    {
        //        Debugger.LogError($"---------------找不到{controlName}-----------------");
        //        return;
        //    }

        //    button.onClick.AddListener(action);

        //}

        ///// <summary>
        ///// 给toggle控件添加监听事件
        ///// </summary>
        ///// <param name="name">控件名字</param>
        ///// <param name="action">所要执行的行为</param>
        //public void ToggleAddListener(string panelName,string controlName, UnityAction<bool> action)
        //{

        //    Toggle toggle = GetToggle(panelName, controlName);
        //    if (toggle == null)
        //    {
        //        Debugger.LogError($"---------------找不到{controlName}-----------------");
        //        return;
        //    }
        //    toggle.onValueChanged.AddListener(action);
        //}

        ///// <summary>
        ///// 给滑动条添加监听事件
        ///// </summary>
        ///// <param name="name">控件名字</param>
        ///// <param name="action">所要执行的行为</param>
        //public void SliderAddListener(string panelName,string controlName, UnityAction<float> action)
        //{
        //    Slider slider = GetSlider(panelName,controlName);
        //    if (slider == null)
        //    {
        //        Debugger.LogError($"---------------找不到{controlName}-----------------");
        //        return;
        //    }
        //    slider.onValueChanged.AddListener(action);
        //}

        ///// <summary>
        ///// 给输入框添加监听事件
        ///// </summary>
        ///// <param name="name">控件名字</param>
        ///// <param name="action">所要执行的行为</param>
        //public void AddListenerInputField(string panelName,string controlName, UnityAction<string> action)
        //{
        //    InputField inputField = GetInputField(panelName,controlName);
        //    if (inputField == null)
        //    {
        //        Debugger.LogError($"---------------找不到{controlName}-----------------");
        //        return;
        //    }
        //    inputField.onValueChanged.AddListener(action);
        //}

        ///// <summary>
        ///// 修改图片的精灵图
        ///// </summary>
        ///// <param name="name">图片名字</param>
        ///// <param name="sprite">新精灵图</param>
        //public void ChangeSprite(string panelName,string controlName, Sprite sprite)
        //{
        //    Image image = GetImage(panelName,controlName);
        //    if (image == null)
        //    {
        //        Debugger.LogError($"---------------找不到{controlName}-----------------");
        //        return;
        //    }
        //    image.sprite = sprite;
        //}

        ///// <summary>
        ///// 更改文本
        ///// </summary>
        ///// <param name="name">文本名称</param>
        ///// <param name="context">新内容</param>
        //public void ChangeText(string panelName,string controlName, string context)
        //{
        //    Text text = GetText(panelName,controlName);
        //    if (text == null)
        //    {
        //        Debugger.LogError($"---------------找不到{name}-----------------");
        //        return;
        //    }
        //    text.text = context;
        //}

        ///// <summary>
        ///// 改变大小
        ///// </summary>
        ///// <param name="panelName"></param>
        ///// <param name="controlName"></param>
        ///// <param name="scale"></param>
        //public void ChangeScale(string panelName, string controlName, Vector2 scale)
        //{
        //    Image image = GetImage(panelName, controlName);
        //    if (image == null)
        //    {
        //        Debugger.LogError($"---------------找不到{controlName}-----------------");
        //        return;
        //    }
        //    image.transform.localScale = scale;
        //}

        #endregion


    }
}