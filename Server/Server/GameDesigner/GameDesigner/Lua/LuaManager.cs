using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Lua
{
    public class LuaManager : MonoBehaviour
    {
        public static LuaManager Instance;
        public static Dictionary<string, Assembly> Assemblys = new Dictionary<string, Assembly>();
        public string[] dllFiles = new string[0];

        void Awake()
        {
            Instance = this;
            Assemblys = new Dictionary<string, Assembly>();
        }
    }
}