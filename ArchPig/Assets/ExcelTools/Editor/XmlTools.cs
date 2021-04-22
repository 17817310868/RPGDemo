using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Text;
using System.Xml;
using System.IO;

public class XmlTools : EditorWindow {
    /// <summary>
	/// 当前编辑器窗口实例
	/// </summary>
	private static XmlTools instance;

    /// <summary>
    /// Excel文件列表
    /// </summary>
    private static List<string> excelList;

    /// <summary>
    /// 项目根路径	
    /// </summary>
    private static string pathRoot;

    /// <summary>
	/// 滚动窗口初始位置
	/// </summary>
	private static Vector2 scrollPos;

    /// <summary>
	/// 输出格式
	/// </summary>
	private static string formatOption = "xlsx";

    /// <summary>
	/// 编码选项
	/// </summary>
	private static string encodingOption = "UTF-8";

    private static XmlDocument xmlDoc = new XmlDocument();

    public static string[] XmlName;

    public static string xmlPath;


    [MenuItem("Plugins/xmlTools")]
    static void ShowExcelTools()
    {
        Init();
        //加载xml文件
        LoadXml();
        instance.Show();
    }

    void OnGUI()
    {
        
        DrawExport();
    }

    [MenuItem("Assets/ExportXmlName ")]
    public static void aaa()
    {
       

        XmlTools xy= new XmlTools();
        xy.bbb();
        /*
                foreach (Object obj in selection)
                {

                }*/
    }


    public void bbb()
    {
        string[] arrStrAudioPath = Directory.GetFiles(Application.dataPath+"/xml/" , "*.xml", SearchOption.AllDirectories);
        StringBuilder stringBuilder = new StringBuilder();

        //创建Xml文件头
        stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        stringBuilder.Append("\r\n");
        //创建根节点
        stringBuilder.Append("<1>");
        stringBuilder.Append("\r\n");
        //读取数据



        for (int i = 0; i < arrStrAudioPath.Length; i++)
        {

            Debug.Log(arrStrAudioPath[i]);
            // (string obj in arrStrAudioPath)

            xmlDoc.Load(arrStrAudioPath[i]);

            string NextNode = "";


            int rootlist = xmlDoc.GetElementsByTagName("root")[0].ChildNodes.Count;

            for (int k = 0; k < rootlist; k++)
            {
                NextNode = xmlDoc.GetElementsByTagName("root")[0].ChildNodes[k].Name;
                Debug.Log(NextNode);
            }


            //创建一个StringBuilder存储数据
            string xmlname = arrStrAudioPath[i].Replace("C:/Users/dell/Desktop/New Unity Project/Assets/xml/", "");
            //创建子节点
            stringBuilder.Append("<" + xmlname + ">");
            stringBuilder.Append("\r\n");
            stringBuilder.Append("< Value >");
            stringBuilder.Append(NextNode);
            stringBuilder.Append("</Value>");
            stringBuilder.Append("\r\n");




            stringBuilder.Append("  </"+ xmlname + " > ");
            //使用换行符分割每一行

            stringBuilder.Append("\r\n");

            using (FileStream fileStream = new FileStream(Application.dataPath + "/xmlName.xml", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.GetEncoding("utf-8")))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }
        
       


    }

    /// <summary>
    /// 绘制插件界面输出项
    /// </summary>
    private void DrawExport()
    {
        if (excelList == null) return;
        if (excelList.Count < 1)
        {
            EditorGUILayout.LabelField("目前没有XML文件被选中!");
        }
        else
        {
            //EditorGUILayout.LabelField("下列项目将被转换为" + formatOption[indexOfFormat] + ":");
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(150));
            foreach (string s in excelList)
            {
                
                GUILayout.BeginHorizontal();
                GUILayout.Toggle(true, s);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            //输出
            if (GUILayout.Button("转换"))
            {
                Convert();
            }
            //输出
            if (GUILayout.Button("导出"))
            {
                XmlNames();
            }
        }
    }

    private static void Convert()
    {
       
        foreach (string assetsPath in excelList)
        {
            //获取Excel文件的绝对路径
            xmlPath = pathRoot + "/" + assetsPath;
            //构造Excel工具类
            Debug.Log(pathRoot+ "pathRoot");
            Debug.Log(assetsPath + "assetsPath");
            XmlUtility xml = new XmlUtility();

            //判断输出类型
            string output = "";
            
                output = xmlPath.Replace(".xml", ".xlsx");
                xml.ConvertToXLSX(output,xmlPath);
           
            //刷新本地资源
            AssetDatabase.Refresh();
        }

        //转换完后关闭插件
        //这样做是为了解决窗口
        //再次点击时路径错误的Bug
        instance.Close();

    }


    private static void XmlNames()
    {
        object[] selection = (object[])Selection.objects;

        

            //XmlDocument xmlDoc = new XmlDocument();
       
    }


    private static void LoadXml()
    {
        if (excelList == null) excelList = new List<string>();
        excelList.Clear();
        //获取选中的对象
        object[] selection = (object[])Selection.objects;

      


        //判断是否有对象被选中

        if (selection.Length == 0)
            return;
        //遍历每一个对象判断是不是xml文件
        foreach (Object obj in selection)
        {
            string objPath = AssetDatabase.GetAssetPath(obj);
            if (objPath.EndsWith(".xml"))
            {
                excelList.Add(objPath);
            }
        }
    }

    private static void Init()
    {
        //获取当前实例
        instance = EditorWindow.GetWindow<XmlTools>();
        //初始化
        pathRoot = Application.dataPath;
        //注意这里需要对路径进行处理
        //目的是去除Assets这部分字符以获取项目目录
        //我表示Windows的/符号一直没有搞懂
        pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
        excelList = new List<string>();
        scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
    }

    void OnSelectionChange()
    {
        //当选择发生变化时重绘窗体
        Show();
        LoadXml();
        Repaint();
    }
}
