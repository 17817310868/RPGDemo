using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Security.Cryptography;

public class GenerateCode : Editor
{
    [MenuItem("GameObject/AddCodeGenerator",false,0)]
    static void AddCodeGenerator()
    {
        GameObject gameObject = Selection.gameObjects.First();
        gameObject.AddComponent<CodeGenerator>();
    }

}

[CustomEditor(typeof(CodeGenerator))]
public class CodeGeneratorInspector : Editor
{
    private string viewCodePath;
    private string ctrlCodePath;
    public string projectName = "拱猪";
    public string title = "";
    public string description = "";
    public string author = "照着教程敲出bug的程序员";

    private void OnEnable()
    {
        viewCodePath = "../LuaFramework/Lua/View/";
        ctrlCodePath = "../LuaFramework/Lua/Controller/";
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("projectName:");
        projectName = EditorGUILayout.TextField(projectName);
        EditorGUILayout.LabelField("title:");
        title = EditorGUILayout.TextField(title);
        EditorGUILayout.LabelField("description:");
        description = EditorGUILayout.TextField(description);
        EditorGUILayout.LabelField("author:");
        author = EditorGUILayout.TextField(author);
        GameObject go = Selection.gameObjects.First();
        EditorGUILayout.LabelField("视图层lua脚本生成路径:");
        EditorGUILayout.LabelField(viewCodePath + go.name + ".lua");
        if(GUILayout.Button("生成" + go.name + ".lua脚本"))
        {
            CodeGenerator.GenerateViewCode(go, projectName, title, description, author);
            AssetDatabase.Refresh();
        }
        EditorGUILayout.LabelField("控制层层lua脚本生成路径:");
        EditorGUILayout.LabelField(ctrlCodePath + go.name.Substring(0,go.name.Length-5) + "Ctrl" + ".lua");
        if(GUILayout.Button("生成" + go.name.Substring(0, go.name.Length - 5) + "Ctrl" + ".lua脚本"))
        {
            CodeGenerator.GenerateCtrlCode(go, projectName, title, description, author);
            AssetDatabase.Refresh();
        }

    }
}