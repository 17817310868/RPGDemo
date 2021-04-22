using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.IO;

public class BlueprintSetting : ScriptableObject
{
	static private BlueprintSetting _instance = null;
	static public BlueprintSetting Instance{
		get{
			if( _instance == null ){
                _instance = Resources.Load<BlueprintSetting>("BlueprintSetting");
                if (_instance == null)
                {
                    _instance = CreateInstance<BlueprintSetting>();
                    var path = "Assets/" + GetGameDesignerPath.Split(new string[] { @"Assets\" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    UnityEditor.AssetDatabase.CreateAsset(_instance, path + "/Editor/Resources/BlueprintSetting.asset");
                }
            }
            return _instance;
		}
	}

    public static string GetGameDesignerPath {
        get {
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
            var dirs = directoryInfo.GetDirectories("GameDesigner", SearchOption.AllDirectories);
            if (dirs.Length > 0)
                return dirs[0].FullName;
            return string.Empty;
        }
    }

    /// <summary>
    /// 解释 : 判断type的基类是否是Typeof类型,是返回真,不是返回假
    /// </summary>

    static public bool IsSubclassOf( Type type , Type Typeof )
	{
		if( type == null | Typeof == null )
			return false;
		if( type.IsSubclassOf(Typeof) | type == Typeof )
			return true;
		return false;
	}

	/// <summary>
	/// 设置类的变量值,解决派生类的值控制父类的变量值 ( 被赋值变量对象 , 赋值变量对象 ) [尽可能的使用此方法,此方法产生GC]
	/// </summary>

	static public void SetPropertyValue( object target , object setValue , bool affect = false)
	{
        foreach ( PropertyInfo property in target.GetType().GetProperties() ){
			if (!property.CanWrite)
				continue;
			if ( IsSubclassOf( property.PropertyType , typeof(UnityEngine.Object) ) | property.PropertyType == typeof(string) | property.PropertyType == typeof(object) | property.PropertyType.IsValueType | property.PropertyType.IsEnum ) {
				property.SetValue( target , property.GetValue( setValue , null ) , null );
			} else {
				PropertyFor ( property.GetValue( target , null ) , property.GetValue( setValue , null ) );
			}
		}

        if (affect) {
            GUIStyle style = target as GUIStyle;
            style.normal.background = Resources.Load<Texture2D>("RadioButton_Off");
            style.normal.textColor = UnityEngine.Random.ColorHSV();
            style.font = Resources.Load<Font>("1234567890");
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
        }
    }

	static void PropertyFor( object target , object setValue )
	{
		if( target == null )
			return;

		foreach( PropertyInfo property in target.GetType().GetProperties() ){
			if (!property.CanWrite)
				continue;
			if ( IsSubclassOf( property.PropertyType , typeof(UnityEngine.Object) ) | property.PropertyType == typeof(string) | property.PropertyType == typeof(object) | property.PropertyType.IsValueType | property.PropertyType.IsEnum ) {
				property.SetValue( target , property.GetValue( setValue , null ) , null );
			} else {
				PropertyFor ( property.GetValue( target , null ) , property.GetValue( setValue , null ) );
			}
		}
	}

#if UNITY_EDITOR || DEBUG
	[Header("int型皮肤")]
	public string intStyleName = "button";
	public bool initIntStyle = false;
	[SerializeField]
	private GUIStyle _intStyle = null;
	public GUIStyle intStyle{
		get{
			if( _intStyle == null | initIntStyle ){
				_intStyle = new GUIStyle();
				SetPropertyValue( _intStyle , GUI.skin.GetStyle( intStyleName ), true);
            }
			return _intStyle;
		}
	}

	[Header("float型皮肤")]
	public string floatStyleName = "button";
	public bool initFloatStyle = false;
	[SerializeField]
	private GUIStyle _floatStyle = null;
	public GUIStyle floatStyle{
		get{
			if( _floatStyle == null | initFloatStyle ){
				_floatStyle = new GUIStyle();
				SetPropertyValue( _floatStyle , GUI.skin.GetStyle( floatStyleName ), true);
            }
			return _floatStyle;
		}
	}

	[Header("string型皮肤")]
	public string stringStyleName = "button";
	public bool initStringStyle = false;
	[SerializeField]
	private GUIStyle _stringStyle = null;
	public GUIStyle stringStyle{
		get{
			if( _stringStyle == null | initStringStyle ){
				_stringStyle = new GUIStyle();
				SetPropertyValue( _stringStyle , GUI.skin.GetStyle( stringStyleName ), true);
            }
			return _stringStyle;
		}
	}

	[Header("bool型皮肤")]
	public string boolStyleName = "button";
	public bool initBoolStyle = false;
	[SerializeField]
	private GUIStyle _boolStyle = null;
	public GUIStyle boolStyle{
		get{
			if( _boolStyle == null | initBoolStyle ){
				_boolStyle = new GUIStyle();
				SetPropertyValue( _boolStyle , GUI.skin.GetStyle( boolStyleName ), true);
            }
			return _boolStyle;
		}
	}

	[Header("vector2 型皮肤")]
	public string vector2StyleName = "button";
	public bool initVector2Style = false;
	[SerializeField]
	private GUIStyle _vector2Style = null;
	public GUIStyle vector2Style{
		get{
			if( _vector2Style == null | initVector2Style ){
				_vector2Style = new GUIStyle();
				SetPropertyValue( _vector2Style , GUI.skin.GetStyle( vector2StyleName ), true);
            }
			return _vector2Style;
		}
	}

	[Header("vector3 型皮肤")]
	public string vector3StyleName = "button";
	public bool initVector3Style = false;
	[SerializeField]
	private GUIStyle _vector3Style = null;
	public GUIStyle vector3Style{
		get{
			if( _vector3Style == null | initVector3Style ){
				_vector3Style = new GUIStyle();
				SetPropertyValue( _vector3Style , GUI.skin.GetStyle( vector3StyleName ), true);
            }
			return _vector3Style;
		}
	}

	[Header("vector4 型皮肤")]
	public string vector4StyleName = "button";
	public bool initVector4Style = false;
	[SerializeField]
	private GUIStyle _vector4Style = null;
	public GUIStyle vector4Style{
		get{
			if( _vector4Style == null | initVector4Style ){
				_vector4Style = new GUIStyle();
				SetPropertyValue( _vector4Style , GUI.skin.GetStyle( vector4StyleName ), true);
            }
			return _vector4Style;
		}
	}

	[Header("rect 型皮肤")]
	public string rectStyleName = "button";
	public bool initRectStyle = false;
	[SerializeField]
	private GUIStyle _rectStyle = null;
	public GUIStyle rectStyle{
		get{
			if( _rectStyle == null | initRectStyle ){
				_rectStyle = new GUIStyle();
				SetPropertyValue( _rectStyle , GUI.skin.GetStyle( rectStyleName ), true);
            }
			return _rectStyle;
		}
	}

	[Header("quaternion 型皮肤")]
	public string quaternionStyleName = "button";
	public bool initQuaternionStyle = false;
	[SerializeField]
	private GUIStyle _quaternionStyle = null;
	public GUIStyle quaternionStyle{
		get{
			if( _quaternionStyle == null | initQuaternionStyle ){
				_quaternionStyle = new GUIStyle();
				SetPropertyValue( _quaternionStyle , GUI.skin.GetStyle( quaternionStyleName ), true);
            }
			return _quaternionStyle;
		}
	}

	[Header("color 型皮肤")]
	public string colorStyleName = "button";
	public bool initColorStyle = false;
	[SerializeField]
	private GUIStyle _colorStyle = null;
	public GUIStyle colorStyle{
		get{
			if( _colorStyle == null | initColorStyle ){
				_colorStyle = new GUIStyle();
				SetPropertyValue( _colorStyle , GUI.skin.GetStyle( colorStyleName ), true);
            }
			return _colorStyle;
		}
	}

	[Header("animCurve 型皮肤")]
	public string animCurveStyleName = "button";
	public bool initAnimCurveStyle = false;
	[SerializeField]
	private GUIStyle _animCurveStyle = null;
	public GUIStyle animCurveStyle{
		get{
			if( _animCurveStyle == null | initAnimCurveStyle ){
				_animCurveStyle = new GUIStyle();
				SetPropertyValue( _animCurveStyle , GUI.skin.GetStyle( animCurveStyleName ), true);
            }
			return _animCurveStyle;
		}
	}

	[Header("enum 型皮肤")]
	public string enumStyleName = "button";
	public bool initEnumStyle = false;
	[SerializeField]
	private GUIStyle _enumStyle = null;
	public GUIStyle enumStyle{
		get{
			if( _enumStyle == null | initEnumStyle ){
				_enumStyle = new GUIStyle();
				SetPropertyValue( _enumStyle , GUI.skin.GetStyle( enumStyleName ), true);
            }
			return _enumStyle;
		}
	}

	[Header("type 型皮肤")]
	public string typeStyleName = "button";
	public bool initTypeStyle = false;
	[SerializeField]
	private GUIStyle _typeStyle = null;
	public GUIStyle typeStyle{
		get{
			if( _typeStyle == null | initTypeStyle ){
				_typeStyle = new GUIStyle();
				SetPropertyValue( _typeStyle , GUI.skin.GetStyle( typeStyleName ), true);
            }
			return _typeStyle;
		}
	}

	[Header("class对象类型皮肤")]
	public string classStyleName = "button";
	public bool initClassStyle = false;
	[SerializeField]
	private GUIStyle _classStyle = null;
	public GUIStyle classStyle{
		get{
			if( _classStyle == null | initClassStyle ){
				_classStyle = new GUIStyle();
				SetPropertyValue( _classStyle , GUI.skin.GetStyle( classStyleName ));
            }
			return _classStyle;
		}
	}

	[Header("null类型皮肤")]
	public string nullStyleName = "button";
	public bool initNullStyle = false;
	[SerializeField]
	private GUIStyle _nullStyle = null;
	public GUIStyle nullStyle{
		get{
			if( _nullStyle == null | initNullStyle ){
				_nullStyle = new GUIStyle();
				SetPropertyValue( _nullStyle , GUI.skin.GetStyle( nullStyleName ), true);
            }
			return _nullStyle;
		}
	}

	[Header("添加节点皮肤")]
	[SerializeField]
	private Texture _unityEngineImage = null;
	public Texture unityEngineImage{
		get{
			if(_unityEngineImage == null){
				return _unityEngineImage = Resources.Load<Texture>("new_Add");
            }
			return _unityEngineImage;
		}
	}

	[Header("其他类型皮肤")]
	public string systemStyleName = "button";
	public bool initSystemStyle = false;
	[SerializeField]
	private GUIStyle _systemStyle = null;
	public GUIStyle systemStyle{
		get{
			if( _systemStyle == null | initSystemStyle ){
				_systemStyle = new GUIStyle();
				SetPropertyValue( _systemStyle , GUI.skin.GetStyle( systemStyleName ), true);
            }
			return _systemStyle;
		}
	}

	[Header("c#皮肤")]
	[SerializeField]
	private Texture _cshImage = null;
	public Texture cshImage{
		get{
			if(_cshImage == null){
				return _cshImage = Resources.Load<Texture>("Icon_Csh");
			}
			return _cshImage;
		}
	}

	[Header("状态机皮肤")]
	[SerializeField]
	private Texture _stateMachineImage = null;
	public Texture stateMachineImage{
		get{
			if(_stateMachineImage == null){
				return _stateMachineImage = Resources.Load<Texture>("stateStyle");
            }
			return _stateMachineImage;
		}
	}

	[Header("状态机名称皮肤")]
	public string stateMachineStyleName = "GUIEditor.BreadcrumbLeft";
	public bool initStateMachineStyle = false;
	[SerializeField]
	private GUIStyle _stateMachineStyle = null;
	public GUIStyle stateMachineStyle{
		get{
			if( _stateMachineStyle == null | initStateMachineStyle ){
				_stateMachineStyle = new GUIStyle();
				SetPropertyValue( _stateMachineStyle , GUI.skin.GetStyle( stateMachineStyleName ), true);
            }
			return _stateMachineStyle;
		}
	}

	[Header("参数类型皮肤")]
	[SerializeField]
	private Texture _parameterTypeImage = null;
	public Texture parameterTypeImage{
		get{
			if(_parameterTypeImage == null){
				return _parameterTypeImage = GUI.skin.GetStyle("MeTransPlayhead").normal.background;
			}
			return _parameterTypeImage;
		}
	}

	[Header("设置值皮肤")]
	public string setValueStyleName = "PreSliderThumb";
	public bool initsetValueStyle = false;
	[SerializeField]
	private GUIStyle _setValueStyle = null;
	public GUIStyle setValueStyle{
		get{
			if( _setValueStyle == null | initsetValueStyle ){
				_setValueStyle = new GUIStyle();
				SetPropertyValue( _setValueStyle , GUI.skin.GetStyle( setValueStyleName ));
			}
			return _setValueStyle;
		}
	}

	[Header("获取值皮肤")]
	public string getValueStyleName = "PreSliderThumb";
	public bool initgetValueStyle = false;
	[SerializeField]
	private GUIStyle _getValueStyle = null;
	public GUIStyle getValueStyle{
		get{
			if( _getValueStyle == null | initgetValueStyle ){
				_getValueStyle = new GUIStyle();
				SetPropertyValue( _getValueStyle , GUI.skin.GetStyle( getValueStyleName ));
			}
			return _getValueStyle;
		}
	}

	[Header("设置运行路径皮肤")]
	public string setRuntimeStyleName = "PreSliderThumb";
	public bool initsetRuntimeStyle = false;
	[SerializeField]
	private GUIStyle _setRuntimeStyle = null;
	public GUIStyle setRuntimeStyle{
		get{
			if( _setRuntimeStyle == null | initsetRuntimeStyle ){
				_setRuntimeStyle = new GUIStyle();
				SetPropertyValue( _setRuntimeStyle , GUI.skin.GetStyle( setRuntimeStyleName ));
			}
			return _setRuntimeStyle;
		}
	}

	[Header("获取运行路径皮肤")]
	public string getRuntimeStyleName = "PreSliderThumb";
	public bool initgetRuntimeStyle = false;
	[SerializeField]
	private GUIStyle _getRuntimeStyle = null;
	public GUIStyle getRuntimeStyle{
		get{
			if( _getRuntimeStyle == null | initgetRuntimeStyle ){
				_getRuntimeStyle = new GUIStyle();
				SetPropertyValue( _getRuntimeStyle , GUI.skin.GetStyle( getRuntimeStyleName ));
			}
			return _getRuntimeStyle;
		}
	}

	[Header("设置参数名称皮肤")]
	public string setParamsStyleName = "PreSliderThumb";
	public bool initsetParamsStyle = false;
	[SerializeField]
	private GUIStyle _setParamsStyle = null;
	public GUIStyle setParamsStyle{
		get{
			if( _setParamsStyle == null | initsetParamsStyle ){
				_setParamsStyle = new GUIStyle();
				SetPropertyValue( _setParamsStyle , GUI.skin.GetStyle( setParamsStyleName ));
			}
			return _setParamsStyle;
		}
	}

	[Header("对象名称皮肤")]
	public string ObjectStyleStyleName = "label";
	public bool initObjectStyle = false;
	[SerializeField]
	private GUIStyle _ObjectStyleStyle = null;
	public GUIStyle ObjectStyle{
		get{
			if( _ObjectStyleStyle == null | initObjectStyle ){
				_ObjectStyleStyle = new GUIStyle();
				SetPropertyValue( _ObjectStyleStyle , GUI.skin.GetStyle( ObjectStyleStyleName ));
			}
			return _ObjectStyleStyle;
		}
	}

	[Header("类型皮肤")]
	public string classStyles = "ProfilerSelectedLabel";

	[Header("node名称皮肤")]
	public string nodeNameStyle = "LODSceneText";
	[SerializeField]
	private GUIStyle _nodeStyle = null;
	public GUIStyle nodeStyle{
		get{
			if( _nodeStyle == null ){
				_nodeStyle = new GUIStyle();
				SetPropertyValue( _nodeStyle , GUI.skin.GetStyle( nodeNameStyle ));
			}
			return _nodeStyle;
		}
	}

	[Header("对象类型名称皮肤")]
	public string targetTypeNameStyle = "BoldLabel";
	[SerializeField]
	private GUIStyle _targetTypeStyle = null;
	public GUIStyle targetTypeStyle{
		get{
			if( _targetTypeStyle == null ){
				_targetTypeStyle = new GUIStyle();
				SetPropertyValue( _targetTypeStyle , GUI.skin.GetStyle( targetTypeNameStyle ));
			}
			return _targetTypeStyle;
		}
	}

	[Header("方法名称皮肤")]
	public string methodNameStyle = "ErrorLabel";
	[SerializeField]
	private GUIStyle _methodStyle = null;
	public GUIStyle methodStyle{
		get{
			if( _methodStyle == null ){
				_methodStyle = new GUIStyle();
				SetPropertyValue( _methodStyle , GUI.skin.GetStyle( methodNameStyle ));
                _methodStyle.normal.textColor = Color.white;
                _methodStyle.alignment = TextAnchor.UpperCenter;
            }
			return _methodStyle;
		}
	}

	[Header("node字段,属性,方法皮肤")]
	public string typeFPMStyleNames = "ButtonMid";
	[SerializeField]
	private GUIStyle _typeFPMStyles = null;
	public GUIStyle typeFPMStyle{
		get{
			if( _typeFPMStyles == null ){
				_typeFPMStyles = new GUIStyle();
				SetPropertyValue( _typeFPMStyles , GUI.skin.GetStyle( typeFPMStyleNames ));
                _typeFPMStyles.fontStyle = FontStyle.Bold;
                _typeFPMStyles.alignment = TextAnchor.UpperLeft;
            }
			return _typeFPMStyles;
		}
	}

    [Header("横向间隔条皮肤")]
    public string horSpaceStyleNames = "ButtonMid";
    [SerializeField]
    private GUIStyle horSpaceStyle = null;
    public GUIStyle HorSpaceStyle {
        get {
            if (horSpaceStyle == null)
            {
                horSpaceStyle = new GUIStyle();
                SetPropertyValue(horSpaceStyle, GUI.skin.GetStyle(horSpaceStyleNames));
            }
            return horSpaceStyle;
        }
    }

    [Header("node选择方法皮肤")]
	public string selectMethodStyleName = "box";
	public bool updateStyles = false;
	[SerializeField]
	private GUIStyle _selectMethodStyl = null;
	public GUIStyle selectMethodStyl{
		get{
			if( _selectMethodStyl == null | updateStyles ){
				_selectMethodStyl = new GUIStyle();
				SetPropertyValue( _selectMethodStyl , GUI.skin.GetStyle( selectMethodStyleName ));
                _selectMethodStyl.fontStyle = FontStyle.Bold;
                _selectMethodStyl.alignment = TextAnchor.UpperLeft;
            }
			return _selectMethodStyl;
		}
	}

	public ScriptableObject GraphEditor = null;
	public ScriptableObject BlueprintEditor = null;
	#endif
	public GameDesigner.SelectObjMode selectObjMode = GameDesigner.SelectObjMode.SelectionStateManager;
	public HideFlags StateHideFlags = HideFlags.HideInHierarchy; 
    public HideFlags StateMachineHideFlags = HideFlags.None;
    public PluginLanguage language = PluginLanguage.Chinese;
    public string[] LANGUAGE = new string[120];
}

/// <summary>
/// 插件语言
/// </summary>
public enum PluginLanguage
{
    /// <summary>
    /// 英文
    /// </summary>
    English,
    /// <summary>
    /// 中文
    /// </summary>
    Chinese
}