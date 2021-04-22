using UnityEngine;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Lua
{
    public enum InitMode
    {
        Compile,
        Load
    }

    /// <summary>
    /// 热更新组件
    /// </summary>
    public class LuaComponent : MonoBehaviour
    {
        [Header("C#源文件, 但格式是.txt文件, 编辑器模式下快速编译")]
        public TextAsset csSourceFile;
        [Header("C#Dll文件, 但格式是.bytes文件, 发布后的热更新文件")]
        public TextAsset dllFile;//dll文件 将dll文件格式改成.bytes ,然后放入Assets资源里面, 拖到此次赋值即可
        [Header("热更新组件类型")]
        public string monoType = "MonoTest";//组件类名称, 如果有命名空间 必须要写上命名空间
        public Component target;
        public bool initialize = false;
        public InitMode mode = InitMode.Compile;
        public bool local = true;
        [Header("动态编译包含的dll文件列表")]
        public string[] dllFiles = new string[]{
            "System.dll",
            @"D:\2018.4.20f1\Editor\Data\Managed\UnityEngine.dll"
        };

        public void Init()
        {
#if UNITY_EDITOR
            if (mode == InitMode.Compile)
                EditorHandle();
            else
                Runtime();
#else
            Runtime();
#endif
        }

        private void Runtime()
        {
            if (dllFile == null) {
                Debug.Log($"没有找到:{dllFile}热更新动态链接库文件.");
                return;
            }
            Assembly assembly;
            if (LuaManager.Assemblys.ContainsKey(dllFile.name))
                assembly = LuaManager.Assemblys[dllFile.name];
            else {
                assembly = Assembly.Load(dllFile.bytes);
                LuaManager.Assemblys.Add(dllFile.name, assembly);
            }
            if (assembly == null) {
                Debug.Log($"获取程序集失败!");
                return;
            }
            var type = assembly.GetType(monoType);
            if (type == null) {
                Debug.Log($"获取组件类型失败!");
                return;
            }
            if (target != null)//在重编译dll后发送
                DestroyImmediate(target);
            target = gameObject.AddComponent(type);
            if (target == null) {
                Debug.Log($"添加组件失败!");
                return;
            }
        }

        private void EditorHandle()
        {
            if (csSourceFile == null) {
                Debug.Log($"没有找到:{dllFile}热更新C#源文件.");
                return;
            }
            if (LuaManager.Assemblys.ContainsKey(csSourceFile.name))
                goto JUMP;
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            CompilerParameters objCompilerParameters = new CompilerParameters();
            if (local)
                objCompilerParameters.ReferencedAssemblies.AddRange(dllFiles);
            else
                objCompilerParameters.ReferencedAssemblies.AddRange(LuaManager.Instance.dllFiles);
            objCompilerParameters.GenerateInMemory = true;
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, csSourceFile.text);
            Assembly assembly = cr.CompiledAssembly;
            if (assembly == null) {
                Debug.Log($"获取程序集失败!");
                return;
            }
            if (!LuaManager.Assemblys.ContainsKey(csSourceFile.name))
                LuaManager.Assemblys.Add(csSourceFile.name, assembly);
            JUMP: var type = LuaManager.Assemblys[csSourceFile.name].GetType(monoType);
            if (type == null) {
                Debug.Log($"获取组件类型失败!");
                return;
            }
            if (target != null)//在重编译dll后发送
                DestroyImmediate(target);
            target = gameObject.AddComponent(type);
            if (target == null) {
                Debug.Log($"添加组件失败!");
                return;
            }
        }

        public virtual void Start()
        {
            Init();
        }

#if UNITY_EDITOR
        public void Update()
        {
            if(initialize){
                initialize = false;
                Init();
            }
        }
#endif

    }
}