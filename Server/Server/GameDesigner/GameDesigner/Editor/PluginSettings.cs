using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using GameDesigner;
using System.IO;

public class PluginSettings : EditorWindow
{
    private string chinesePath, englishPath;

    private void Awake()
    {
        DirectoryInfo info = new DirectoryInfo(Application.dataPath);
        var files = info.GetFiles("ChineseLanguage.language", SearchOption.AllDirectories);
        if (files.Length > 0) {
            chinesePath = files[0].FullName;
        }
        files = info.GetFiles("EnglishLanguage.language", SearchOption.AllDirectories);
        if (files.Length > 0)
        {
            englishPath = files[0].FullName;
        }
    }

    [MenuItem("StateMachine/PluginSettings")]
	static void Init()
	{
		var setting = GetWindow<PluginSettings>();
        setting.maxSize = new Vector2(300,100);
        setting.Show();
	}

	void OnGUI()
	{
        if (BlueprintGUILayout.Instance.language == PluginLanguage.Chinese & chinesePath != null)
        {
            BlueprintGUILayout.Instance.LANGUAGE = File.ReadAllLines(chinesePath);
        }
        else if(englishPath != null)
        {
            BlueprintGUILayout.Instance.LANGUAGE = File.ReadAllLines(englishPath);
        }

        BlueprintGUILayout.Instance.selectObjMode = (SelectObjMode)EditorGUILayout.EnumPopup(BlueprintGUILayout.Instance.LANGUAGE[81], BlueprintGUILayout.Instance.selectObjMode);
        BlueprintGUILayout.Instance.StateMachineHideFlags = (HideFlags)EditorGUILayout.EnumPopup(BlueprintGUILayout.Instance.LANGUAGE[112], BlueprintGUILayout.Instance.StateMachineHideFlags);
        BlueprintGUILayout.Instance.StateHideFlags = (HideFlags)EditorGUILayout.EnumPopup(BlueprintGUILayout.Instance.LANGUAGE[82], BlueprintGUILayout.Instance.StateHideFlags);
		BlueprintGUILayout.Instance.language = (PluginLanguage)EditorGUILayout.EnumPopup(BlueprintGUILayout.Instance.LANGUAGE[83], BlueprintGUILayout.Instance.language);
    }
}