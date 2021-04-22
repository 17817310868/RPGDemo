using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace LuaFramework {
    public class LuaBehaviour : View {
        //private string data = null;
        //private Dictionary<string, LuaFunction> buttons = new Dictionary<string, LuaFunction>();

        /// <summary>
        /// 以控件名字为key，存储当前面板下所有子控件的字典
        /// </summary>
        public Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

        protected void Awake() {
            InitPanel();
            Util.CallMethod(name, "Awake", gameObject);
        }

        protected void Start() {
            Util.CallMethod(name, "Start");
        }

        protected void OnClick() {
            Util.CallMethod(name, "OnClick");
        }

        protected void OnClickEvent(GameObject go) {
            Util.CallMethod(name, "OnClick", go);
        }

        /// <summary>
        /// 初始化面板，获取面板下对应类型的控件并存入控件字典
        /// </summary>
        void InitPanel()
        {
            FindControls<Button>();
            FindControls<InputField>();
            FindControls<Image>();
            FindControls<Text>();
            FindControls<Slider>();
            FindControls<Toggle>();
            
        }

        #region  获取某种类型的所有控件

        /// <summary>
        /// 遍历某种类型控件并存进字典
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        void FindControls<T>() where T : UIBehaviour
        {
            T[] controls = GetComponentsInChildren<T>();
            string controlName;
            for (int i = 0; i < controls.Length; i++)
            {
                controlName = controls[i].gameObject.name;
                List<UIBehaviour> controlList = null;
                if (controlDic.TryGetValue(controlName, out controlList))
                {
                    controlList.Add(controls[i]);
                }
                else
                {
                    controlDic.Add(controlName, new List<UIBehaviour>() { controls[i] });
                }
            }
        }

        #endregion

        #region  获取控件
        /// <summary>
        /// 根据名字获取当前面板下的对应子控件
        /// </summary>
        /// <typeparam name="T">指定控件类型</typeparam>
        /// <param name="name">指定控件名字</param>
        /// <returns></returns>
        public T GetControl<T>(string name) where T : UIBehaviour
        {
            List<UIBehaviour> controlList = null;
            if(controlDic.TryGetValue(name,out controlList))
            {
                foreach (UIBehaviour control in controlList)
                {
                    if (control is T)
                    {
                        return (control as T);
                    }
                }
            }
            Debugger.LogError($"-----------找不到{name}控件-----------");
            return null;
        }

        /// <summary>
        /// 获取Button
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Button GetButton(string name)
        {
            List<UIBehaviour> controlList = null;
            if (controlDic.TryGetValue(name, out controlList))
            {
                foreach (UIBehaviour control in controlList)
                {
                    if (control is Button)
                    {
                        return (control as Button);
                    }
                }
            }
            Debugger.LogError($"-----------找不到{name}-----------");
            return null;
        }

        /// <summary>
        /// 获取Toggle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Toggle GetToggle(string name)
        {
            List<UIBehaviour> controlList = null;
            if (controlDic.TryGetValue(name, out controlList))
            {
                foreach (UIBehaviour control in controlList)
                {
                    if (control is Toggle)
                    {
                        return (control as Toggle);
                    }
                }
            }
            Debugger.LogError($"-----------找不到{name}-----------");
            return null;
        }

        /// <summary>
        /// 获取Slider
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Slider GetSlider(string name)
        {
            List<UIBehaviour> controlList = null;
            if (controlDic.TryGetValue(name, out controlList))
            {
                foreach (UIBehaviour control in controlList)
                {
                    if (control is Slider)
                    {
                        return (control as Slider);
                    }
                }
            }
            Debugger.LogError($"-----------找不到{name}-----------");
            return null;
        }

        /// <summary>
        /// 获取InputField
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public InputField GetInputField(string name)
        {
            List<UIBehaviour> controlList = null;
            if (controlDic.TryGetValue(name, out controlList))
            {
                foreach (UIBehaviour control in controlList)
                {
                    if (control is InputField)
                    {
                        return (control as InputField);
                    }
                }
            }
            Debugger.LogError($"-----------找不到{name}-----------");
            return null;
        }

        /// <summary>
        /// 获取Image
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Image GetImage(string name)
        {
            List<UIBehaviour> controlList = null;
            if (controlDic.TryGetValue(name, out controlList))
            {
                foreach (UIBehaviour control in controlList)
                {
                    if (control is Image)
                    {
                        return (control as Image);
                    }
                }
            }
            Debugger.LogError($"-----------找不到{name}-----------");
            return null;
        }

        /// <summary>
        /// 获取Text
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Text GetText(string name)
        {
            List<UIBehaviour> controlList = null;
            if (controlDic.TryGetValue(name, out controlList))
            {
                foreach (UIBehaviour control in controlList)
                {
                    if (control is Text)
                    {
                        return (control as Text);
                    }
                }
            }
            Debugger.LogError($"-----------找不到{name}-----------");
            return null;
        }

        #endregion

        #region  给控件添加事件

        /// <summary>
        /// 给按钮添加监听事件
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="action">所要执行的行为</param>
        public void ButtonAddListener(string name, UnityAction action)
        {
            //Button button = GetControl<Button>(name);
            Button button = GetButton(name);
            if (button != null)
            {
                button.onClick.AddListener(action);
                return;
            }
            Debugger.LogError($"---------------找不到{name}-----------------");
        }

        /// <summary>
        /// 给toggle控件添加监听事件
        /// </summary>
        /// <param name="name">控件名字</param>
        /// <param name="action">所要执行的行为</param>
        public void ToggleAddListener(string name, UnityAction<bool> action)
        {
            Toggle toggle = GetControl<Toggle>(name);
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(action);
                return;
            }
            Debugger.LogError($"---------------找不到{name}-----------------");
        }

        /// <summary>
        /// 给滑动条添加监听事件
        /// </summary>
        /// <param name="name">控件名字</param>
        /// <param name="action">所要执行的行为</param>
        public void SliderAddListener(string name, UnityAction<float> action)
        {
            Slider slider = GetControl<Slider>(name);
            if (slider != null)
            {
                slider.onValueChanged.AddListener(action);
                return;
            }
            Debugger.LogError($"---------------找不到{name}-----------------");
        }

        /// <summary>
        /// 给输入框添加监听事件
        /// </summary>
        /// <param name="name">控件名字</param>
        /// <param name="action">所要执行的行为</param>
        public void AddListenerInputField(string name, UnityAction<string> action)
        {
            InputField inputField = GetControl<InputField>(name);
            if (inputField != null)
            {
                inputField.onValueChanged.AddListener(action);
                return;
            }
            Debugger.LogError($"---------------找不到{name}-----------------");
        }

        /// <summary>
        /// 修改图片的精灵图
        /// </summary>
        /// <param name="name">图片名字</param>
        /// <param name="sprite">新精灵图</param>
        public void ChangeSprite(string name, Sprite sprite)
        {
            Image image = GetControl<Image>(name);
            if (image != null)
            {
                image.sprite = sprite;
                return;
            }
            Debugger.LogError($"---------------找不到{name}-----------------");
        }

        /// <summary>
        /// 更改文本
        /// </summary>
        /// <param name="name">文本名称</param>
        /// <param name="context">新内容</param>
        public void ChangeText(string name, string context)
        {
            Text text = GetControl<Text>(name);
            if (text != null)
            {
                text.text = context;
                return;
            }
            Debugger.LogError($"---------------找不到{name}-----------------");
        }

        #endregion
        /// <summary>
        /// 添加单击事件
        /// </summary>
        //public void AddClick(GameObject go, LuaFunction luafunc) {
        //    if (go == null || luafunc == null) return;
        //    buttons.Add(go.name, luafunc);
        //    go.GetComponent<Button>().onClick.AddListener(
        //        delegate() {
        //            luafunc.Call(go);
        //        }
        //    );
        //}

        /// <summary>
        /// 删除单击事件
        /// </summary>
        /// <param name="go"></param>
        //public void RemoveClick(GameObject go) {
        //    if (go == null) return;
        //    LuaFunction luafunc = null;
        //    if (buttons.TryGetValue(go.name, out luafunc)) {
        //        luafunc.Dispose();
        //        luafunc = null;
        //        buttons.Remove(go.name);
        //    }
        //}

        /// <summary>
        /// 清除单击事件
        /// </summary>
        //public void ClearClick() {
        //    foreach (var de in buttons) {
        //        if (de.Value != null) {
        //            de.Value.Dispose();
        //        }
        //    }
        //    buttons.Clear();
        //}

        //-----------------------------------------------------------------
        //        protected void OnDestroy() {
        //            ClearClick();
        //#if ASYNC_MODE
        //            string abName = name.ToLower().Replace("panel", "");
        //            ResManager.UnloadAssetBundle(abName + AppConst.ExtName);
        //#endif
        //            Util.ClearMemory();
        //            Debug.Log("~" + name + " was destroy!");
        //        }
    }
}