using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Lua
{
    [CustomEditor(typeof(LuaComponent))]
    public class LuaComponentEdit : Editor
    {
        LuaComponent lua;

        private void OnEnable()
        {
            lua = target as LuaComponent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("打开编辑器")) {
                LuaEditor.Init();
            }
            if (GUILayout.Button("初始化组件")) {
                lua.initialize = true;
            }
            if (GUILayout.Button("执行Start方法")) {
                if (lua.target != null) {
                    try {
                        var type = lua.target.GetType();
                        var method = type.GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        method.Invoke(lua.target, null);
                    } catch { }
                }
            }
            if (lua.initialize) {
                lua.initialize = false;
                lua.Init();
            }
        }
    }
}