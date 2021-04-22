﻿using UnityEngine;
using System.Collections.Generic;

namespace GameDesigner
{
	/// <summary>
	/// 蓝图基类,想要自定义蓝图可继承此类
	/// </summary>

	public class Blueprint : MonoBehaviour
	{
		public List<BlueprintNode> methods = new List<BlueprintNode> ();
		public List<BlueprintNode> selectNodes = new List<BlueprintNode>();
        public FunBlockNode selectBlock;
        public List<FunBlockNode> functionalBlocks = new List<FunBlockNode>();

        [HideInInspector] public Vector2 mousePosition;
        [HideInInspector] public string InheritedClassName = "MonoBehaviour";

        public Blueprint() { }

        public BlueprintNode selectMethod{
			get{
				if (selectNodes.Count > 0)
					return selectNodes [0];
				return null;
			}
			set{
				if(!selectNodes.Contains(value)&value!=null){
					selectNodes.Add (value);
				}
			}
		}
        
		/// 创建状态机实例
		public static Blueprint CreateBlueprintInstance ( string name = "我的脚本设计器" )
		{
			Blueprint designer = new GameObject(name).AddComponent<Blueprint>();
			designer.name = name;
			return designer;
		}
	}
}