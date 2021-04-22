using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using System.Threading;

namespace GameDesigner
{
	public class AddBlueprintNode : EditorWindow
	{
        public static Blueprint designer = null;
        private static TypeInfo typeInfo;
		private static AddBlueprintNode window;
        public static AddBlueprintNode Instance{
			get{
				if( window == null )
					window = GetWindow<AddBlueprintNode>();
				return window;
			}
		}

		public static bool WindowIsNull {
			get{
				return (window != null);
			}
		}

		[MenuItem ("StateMachine/AddBlueprintNode")]
        public static void Init () 
		{
			if( window == null )
				window = GetWindow<AddBlueprintNode>();
			window.name = "节点窗口";
			window.minSize = new Vector2(300,400 );
			window.ShowUtility();
		}

        public static void Init ( Vector2 position ) 
		{
			if( window == null )
				window = GetWindow<AddBlueprintNode>();
			window.name = "节点窗口";
			window.minSize = new Vector2(300,400 );
			window.position = new Rect( new Vector2(position.x+18,(+position.y+50)) , window.position.size );
			window.ShowUtility();
		}

		void OnGUI()
		{
            typeInfo = TypeInfo.instance;
            TypeInfo.UpdateCheckTypeInfo(typeInfo);
			try{
                DrawTypeInfo();
            } catch{}
			Repaint();
		}

        private static Vector2 scrollPosition;
        private static bool Empty;
        private static string tooltip = "";

        public static void DrawTypeInfo()
        {
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            if (Empty) {
                typeInfo.findtype = string.Empty;
                typeInfo.findtypeBool = string.Empty;
                typeInfo.selectTypeIndex = 0;
                typeInfo.findTools = false;
                Empty = false;
            }
            typeInfo.findtype = GUILayout.TextField(typeInfo.findtype, GUI.skin.GetStyle("SearchTextField"));
            if (GUILayout.Button("", "SearchCancelButton")) {
                Empty = true;
                typeInfo.nameSpaces = new List<string>();
            }
            GUILayout.Space(4);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Back", "ButtonMid")) {
                if (typeInfo.selectTypeIndex == 3) {
                    typeInfo.selectTypeIndex = 2;
                } else if (typeInfo.selectTypeIndex == 2 | typeInfo.selectTypeIndex == 0) {
                    typeInfo.selectTypeIndex = 0;
                }
                typeInfo.findTools = false;
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
            try { FindTypeName(); } catch { }
            GUI.skin.label.normal.textColor = Color.black;
            if (typeInfo.selectTypeIndex == 3) {
                try {
                    if (GUILayout.Button("Constructors --<★>-- (构造器)" + "  [" + typeInfo.Constructors.Count + "]", BlueprintGUILayout.Instance.typeFPMStyle)) {
                        typeInfo.constrsFolt = !typeInfo.constrsFolt;
                    }
                    if (typeInfo.constrsFolt) {
                        BlueprintGUILayout.BeginSpaceHorizontal(20);
                        for (int i = 0; i < typeInfo.Constructors.Count; ++i) {
                            SelectMethod(typeInfo, typeInfo.Constructors[i]);
                        }
                        BlueprintGUILayout.EndSpaceHorizontal();
                    }
                    if (GUILayout.Button("Fields --<★>-- (字段)" + "  [" + typeInfo.Fields.Count + "]", BlueprintGUILayout.Instance.typeFPMStyle)) {
                        typeInfo.fieldsFolt = !typeInfo.fieldsFolt;
                    }
                    if (typeInfo.fieldsFolt) {
                        BlueprintGUILayout.BeginSpaceHorizontal(20);
                        for (int i = 0; i < typeInfo.Fields.Count; ++i) {
                            SelectMethod(typeInfo, typeInfo.Fields[i]);
                        }
                        BlueprintGUILayout.EndSpaceHorizontal();
                    }
                    if (GUILayout.Button("Propertys --<★>-- (属性)" + "  [" + typeInfo.Propertys.Count + "]", BlueprintGUILayout.Instance.typeFPMStyle)) {
                        typeInfo.propesFolt = !typeInfo.propesFolt;
                    }
                    if (typeInfo.propesFolt) {
                        BlueprintGUILayout.BeginSpaceHorizontal(20);
                        for (int i = 0; i < typeInfo.Propertys.Count; ++i) {
                            SelectMethod(typeInfo, typeInfo.Propertys[i]);
                        }
                        BlueprintGUILayout.EndSpaceHorizontal();
                    }
                    if (GUILayout.Button("Methods --<★>-- (方法)" + "  [" + typeInfo.Methods.Count + "]", BlueprintGUILayout.Instance.typeFPMStyle)) {
                        typeInfo.methodsFolt = !typeInfo.methodsFolt;
                    }
                    if (typeInfo.methodsFolt) {
                        BlueprintGUILayout.BeginSpaceHorizontal(20);
                        for (int i = 0; i < typeInfo.Methods.Count; ++i) {
                            SelectMethod(typeInfo, typeInfo.Methods[i]);
                        }
                        BlueprintGUILayout.EndSpaceHorizontal();
                    }
                } catch { }
            }
            GUILayout.EndScrollView();
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.TextArea(new Rect(new Vector2(rect.x - 4, rect.y - 17), new Vector2(rect.width + 9, 50)), tooltip, BlueprintGUILayout.Instance.selectMethodStyl);
        }

        private static void FindTypeName()
        {
            if (thread == null) {
                dataPath = Application.dataPath;
                thread = new Thread(ThreadFindType) {
                    IsBackground = true
                };
                thread.Start();
            } else if (thread.ThreadState.HasFlag(ThreadState.Stopped)) {
                dataPath = Application.dataPath;
                thread = new Thread(ThreadFindType) {
                    IsBackground = true
                };
                thread.Start();
            }
            if (typeInfo.findTools) {
                foreach (Type s in typeInfo.findTypes) {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    if (rect.Contains(Event.current.mousePosition)) {
                        if (s.IsSubclassOf(typeof(Object)) & s.FullName.Contains("Unity"))
                            GUILayout.Label(new GUIContent(s.FullName, EditorGUIUtility.ObjectContent(null, s).image), "LODSliderRangeSelected");
                        else
                            GUILayout.Label(new GUIContent(s.FullName, BlueprintGUILayout.Instance.cshImage), "LODSliderRangeSelected");
                        if (Event.current.type == EventType.MouseDown) {
                            SelectionType(s);
                        }
                    } else if (s.IsSubclassOf(typeof(Object)) & s.FullName.Contains("Unity"))
                        GUILayout.Label(new GUIContent(s.FullName, EditorGUIUtility.ObjectContent(null, s).image), GUILayout.MaxHeight(20));
                    else
                        GUILayout.Label(new GUIContent(s.FullName, BlueprintGUILayout.Instance.cshImage), GUILayout.MaxHeight(20));
                    EditorGUILayout.EndHorizontal();
                }
            } else if (typeInfo.selectTypeIndex == 0) {
                lock (typeInfo.nameSpaces) {
                    foreach (string s in typeInfo.nameSpaces) {
                        Rect rect = EditorGUILayout.BeginHorizontal();
                        if (rect.Contains(Event.current.mousePosition)) {
                            GUILayout.Label(new GUIContent(s, BlueprintGUILayout.Instance.unityEngineImage), "LODSliderRangeSelected");
                            if (Event.current.type == EventType.MouseDown) {
                                typeInfo.nameSpace = s;
                                typeInfo.selectTypeIndex = 1;
                            }
                        } else
                            GUILayout.Label(new GUIContent(s, BlueprintGUILayout.Instance.unityEngineImage), GUILayout.MaxHeight(20));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            } else if (typeInfo.selectTypeIndex == 2) {//选择类型
                try {
                    foreach (Type s in typeInfo.findTypes) {
                        Rect rect = EditorGUILayout.BeginHorizontal();
                        if (rect.Contains(Event.current.mousePosition)) {
                            if (s.IsSubclassOf(typeof(Object)) & s.FullName.Contains("Unity"))
                                GUILayout.Label(new GUIContent(s.Name, EditorGUIUtility.ObjectContent(null, s).image), "LODSliderRangeSelected");
                            else
                                GUILayout.Label(new GUIContent(s.Name, BlueprintGUILayout.Instance.cshImage), "LODSliderRangeSelected");
                            if (Event.current.type == EventType.MouseDown) {
                                SelectionType(s);
                            }
                        } else {
                            if (s.IsSubclassOf(typeof(Object)) & s.FullName.Contains("Unity"))
                                GUILayout.Label(new GUIContent(s.Name, EditorGUIUtility.ObjectContent(null, s).image), GUILayout.MaxHeight(20));
                            else
                                GUILayout.Label(new GUIContent(s.Name, BlueprintGUILayout.Instance.cshImage), GUILayout.MaxHeight(20));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                } catch { }
            }
        }

        public static Thread thread = null;
        private static string dataPath = "";
        private static void ThreadFindType()
        {
            while (thread != null) {
                Thread.Sleep(1);
                try {
                    if (typeInfo.findtype != typeInfo.findtypeBool) {
                        typeInfo.findTypes = new List<Type>();
                        foreach (var type in typeInfo.types) {
                            if (type.Name.IndexOf(typeInfo.findtype, StringComparison.OrdinalIgnoreCase) == 0)
                                typeInfo.findTypes.Add(type);
                        }
                        typeInfo.findtypeBool = typeInfo.findtype;
                        typeInfo.findTools = true;
                    }
                    if (typeInfo.selectTypeIndex == 0 & !typeInfo.findTools) {
                        if (typeInfo.nameSpaces.Count == 0) {
                            SystemType.GetTypes(types => {
                                foreach (var type in types)
                                {
                                    if (!typeInfo.nameSpaces.Contains(type.Namespace))
                                    {
                                        typeInfo.nameSpaces.Add(type.Namespace);
                                    }
                                    typeInfo.types.Add(type);
                                }
                            });
                        }
                    } else if (typeInfo.selectTypeIndex == 1 & !typeInfo.findTools) {//查找命名空间下的所有类型
                        typeInfo.findTypes = new List<Type>();
                        foreach (var type in typeInfo.types) {
                            if (type.Namespace == null & type.Assembly.GetName().Name == typeInfo.nameSpace)
                                typeInfo.findTypes.Add(type);
                            else if (type.Namespace == typeInfo.nameSpace)
                                typeInfo.findTypes.Add(type);
                        }
                        typeInfo.selectTypeIndex = 2;
                    }
                } catch { }
            }
        }

        /// <summary>
        /// 当点击选择类型,赋值类信息的值
        /// </summary>
        private static void SelectionType(Type type)
		{
            typeInfo.nameSpace = type.Namespace;
            typeInfo.typeName = type.FullName;
            typeInfo.findtype = type.FullName;
            typeInfo.type = type;
            typeInfo.findtypeBool = type.FullName;
            typeInfo.findTools = false;
            typeInfo.selectTypeIndex = 3;
		}

		/// <summary>
		/// 初始化命名空间
		/// </summary>
		private static void InitNameSpaces(Type type)
		{
			if(typeInfo.nameSpaces.Count == 0 ){
				if( type.Namespace != null )
                    typeInfo.nameSpaces.Add( type.Namespace );
				else
                    typeInfo.nameSpaces.Add( type.Assembly.GetName().Name );
			}else{
				bool n = false;
				foreach( string ns in typeInfo.nameSpaces ){
					if(type.Namespace==null&type.Assembly.GetName().Name==ns){
						n = false;
						break;
					}else if( type.Namespace == ns ){
						n = false;
						break;
					}else
						n = true;
				}
				if( n ){
					if( type.Namespace != null ){
                        typeInfo.nameSpaces.Add( type.Namespace );
					}else{
                        typeInfo.nameSpaces.Add( type.Assembly.GetName().Name );
					}
				}
			}
		}
        
		private static void SelectMethod( TypeInfo t , Method methods )
		{
			try{
				Rect rect = EditorGUILayout.BeginHorizontal();
                string name = methods.name + ToArrtsString( methods.Parameters.ToArray() ) + " ☞ " + ConvertUtility.StringSplitEndOne(methods.returnTypeName);
				if( rect.Contains(Event.current.mousePosition) ){
					tooltip = name;
					if( typeInfo.type.IsSubclassOf(typeof(Object)) & typeInfo.type.FullName.Contains("Unity"))
						GUILayout.Label ( new GUIContent(name,EditorGUIUtility.ObjectContent(null,t.type).image,methods.xmlTexts),"LODSliderRangeSelected");
					else 
						GUILayout.Label ( new GUIContent(name,BlueprintGUILayout.Instance.cshImage,methods.xmlTexts),"LODSliderRangeSelected");
					if( Event.current.type == EventType.MouseDown ){
						BlueprintNode body = BlueprintNode.CreateBlueprintNodeInstance( designer , methods.name , designer.mousePosition );
						SystemType.SetFieldValue( body.method , methods );
						body.method.Parameters = methods.Parameters;
						body.method.genericArguments = methods.genericArguments;
						body.method.typeModel = typeInfo.typeModel;
						body.method.xmlTexts = name;
						body.method.nodeName = "m_" + methods.targetType.Name;
						Instance.Close();
					}
				}else{
					if( typeInfo.type.IsSubclassOf(typeof(Object)) & typeInfo.type.FullName.Contains("Unity"))
						GUILayout.Label ( new GUIContent(name,EditorGUIUtility.ObjectContent(null,t.type).image,methods.xmlTexts),GUILayout.MaxHeight(20));
					else 
						GUILayout.Label ( new GUIContent(name,BlueprintGUILayout.Instance.cshImage,methods.xmlTexts),GUILayout.MaxHeight(20));
				}
				EditorGUILayout.EndHorizontal();
			}catch (Exception e){ Debug.Log("请先在蓝图编辑器按下鼠标右键定位在添加事件！ => "+e); }
		}

		static public string ToArrtsString( Parameter[] ps  )
		{
			if( ps.Length == 0 )
				return "";
			
			string str = "(";
			foreach( Parameter p in ps ){
				str += "  " + ConvertUtility.StringSplitEndOne(p.parameterTypeName) + "  " + p.name + "  ";
			}
			return str + ")";
		}
	}
}