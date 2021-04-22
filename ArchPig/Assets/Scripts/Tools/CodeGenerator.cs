using Lua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodeGenerator : MonoBehaviour
{

    public static void CodeHead(ref StringBuilder content, string projectName, string title, string description, string author)
    {
        content.Append("--[[");
        content.Append("\n");
        content.Append("*=================================================================================");
        content.Append("\n");
        content.Append("*");
        content.Append("\n");
        content.Append("*        projectName:");
        content.Append("\n");
        content.Append($"*            项目名称:{projectName}");
        content.Append("\n");
        content.Append("*");
        content.Append("\n");
        content.Append("*        title:");
        content.Append("\n");
        content.Append($"*            标题:{title}");
        content.Append("\n");
        content.Append("*");
        content.Append("\n");
        content.Append("*        description:");
        content.Append("\n");
        content.Append($"*            功能描述:{description}");
        content.Append("\n");
        content.Append("*");
        content.Append("\n");
        content.Append("*        author:");
        content.Append("\n");
        content.Append($"*            作者:{author}");
        content.Append("\n");
        content.Append("*=================================================================================");
        content.Append("\n");
        content.Append("*");
        content.Append("\n");
        content.Append("--]]");
        content.Append("\n");
        content.Append("");
        content.Append("\n");
    }
    public static void GenerateCtrlCode(GameObject go,string projectName,string title,string description,string author)
    {
        StringBuilder content = new StringBuilder();
        content.Append("\n");
        CodeHead(ref content, projectName, title, description, author);
        string name = go.name.Substring(0, go.name.Length - 5) + "Ctrl";
        content.Append("\n");
        content.Append("\n");
        content.Append($"{name} = " + "{" + "}");
        content.Append("\n");
        content.Append($"local this = {name}");
        content.Append("\n");
        content.Append("\n");
        content.Append($"local gameObject");
        content.Append("\n");
        content.Append($"local transform");
        content.Append("\n");
        content.Append("\n");
        content.Append($"function {name}" + ":Awake()");
        content.Append("\n");
        content.Append("\n");
        content.Append("end");
        content.Append("\n");
        content.Append("\n");
        content.Append($"function {name}" + ":OnCreate(go)");
        content.Append("\n");
        content.Append("    gameObject = go");
        content.Append("\n");
        content.Append("    transform = go.transform");
        content.Append("\n");
        content.Append("    self:Init()");
        content.Append("\n");
        content.Append("end");
        content.Append("\n");
        content.Append("\n");
        content.Append($"function {name}" + ":Init()");
        content.Append("\n");
        content.Append("\n");
        content.Append("end");

        string viewCodePath = Application.dataPath + "/LuaFramework/Lua/Controller/";
        FileStream stream = new FileStream(viewCodePath + name + ".lua", FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Close();
        writer.Dispose();

    }
    public static void GenerateViewCode(GameObject go, string projectName, string title, string description, string author)
    {
        StringBuilder content = new StringBuilder();
        content.Append("\n");
        
        CodeHead(ref content, projectName, title, description, author);
        content.Append("\n");
        content.Append($"{go.name} = " + "{" + "}");
        content.Append("\n");
        content.Append($"local this = {go.name}");
        content.Append("\n");
        content.Append("\n");

        content.Append("--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)");
        content.Append("\n");
        content.Append($"{go.name}" + ".Ctrls" + " = ");
        content.Append("\n");
        content.Append("{");
        content.Append("\n");
        FindCtrl<Image>(go,ref content);
        FindCtrl<Text>(go, ref content);
        FindCtrl<RawImage>(go, ref content);
        FindCtrl<Slider>(go, ref content);
        FindCtrl<Toggle>(go, ref content);
        FindCtrl<Button>(go, ref content);
        FindCtrl<InputField>(go, ref content);
        FindCtrl<Scrollbar>(go, ref content);
        FindCtrl<ScrollRect>(go, ref content);


        content.Append("}");
        

 
        content.Append("\n");
        content.Append("\n");
        content.Append($"local gameObject");
        content.Append("\n");
        content.Append($"local transform");
        content.Append("\n");
        content.Append("\n");
        content.Append($"function {go.name}" + ":Awake(go)");
        content.Append("\n");
        content.Append("    gameObject = go");
        content.Append("\n");
        content.Append("    transform = go.transform");
        content.Append("\n");
        content.Append("    self:Init()");
        content.Append("\n");
        content.Append("end");
        content.Append("\n");
        content.Append("\n");
        content.Append($"function {go.name}" + ":Init()");
        content.Append("\n");
        content.Append("\n");
        content.Append("end");
        content.Append("\n");
        content.Append("\n");
        content.Append($"function {go.name}" + ":Show()");
        content.Append("\n");
        content.Append("\n");
        content.Append("end");
        content.Append("\n");
        content.Append("\n");
        content.Append($"function {go.name}" + ":Hide()");
        content.Append("\n");
        content.Append("\n");
        content.Append("end");

        string viewCodePath = Application.dataPath + "/LuaFramework/Lua/View/";
        FileStream stream = new FileStream(viewCodePath + go.name + ".lua", FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Close();
        writer.Dispose();

    }

    static void FindCtrl<T>(GameObject go,ref StringBuilder content) where T : UIBehaviour
    {
        T[] controls = go.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; i++)
        {
            if (!controls[i].name.StartsWith("Auto"))
                continue;
            string path = controls[i].name;
            Transform temp = controls[i].transform;
            while (temp.transform.parent.name != go.name)
            {
                path = temp.transform.parent.name + "/" + path;
                temp = temp.transform.parent;
            }
            
            string name = SwitchAuto(typeof(T)) + controls[i].name.Substring(4, controls[i].name.Length-4);
            content.Append("    " + name + " = " + "{  Path" + " = " + "\'" + path + "\'" + ","
                + "  ControlType = " + "\'" + typeof(T).Name + "\'" + "  },");
            content.Append("\n");
        }
    }

    static string SwitchAuto(Type type)
    {
        switch (type.Name)
        {
            case "Image":
                return "Img";
            case "Button":
                return "Btn";
            case "Text":
                return "Text";
            case "Slider":
                return "Slider";
            case "Toggle":
                return "Tog";
            case "InputField":
                return "Input";
            case "Scrollbar":
                return "Scrollbar";
            case "RawImage":
                return "RImg";
            case "ScrollRect":
                return "Rect";
            default:
                break;
        }
        return null;
    }

    //static string SwitchType(Type type)
    //{
    //    switch (type.Name)
    //    {
    //        case "Image":
    //            return "CtrlEnum.Image";
    //        case "Button":
    //            return "CtrlEnum.Button";
    //        case "Text":
    //            return "CtrlEnum.Text";
    //        case "Slider":
    //            return "CtrlEnum.Slider";
    //        case "Toggle":
    //            return "CtrlEnum.Toggle";
    //        case "InputField":
    //            return "CtrlEnum.InputField";
    //        case "Scrollbar":
    //            return "CtrlEnum.Scrollbar";
    //        case "RawImage":
    //            return "CtrlEnum.RawImage";
    //        default:
    //            break;
    //    }
    //    return null;
    //}
}
