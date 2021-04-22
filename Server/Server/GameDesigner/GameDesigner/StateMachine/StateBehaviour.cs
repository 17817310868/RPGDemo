using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameDesigner
{
	/// <summary>
	/// 状态行为数脚本 v2017/12/6
	/// </summary>
	public abstract class StateBehaviour : IBehaviour
	{
        /// <summary>
        /// 状态管理器转换组建
        /// </summary>
        public new Transform transform
        {
            get
            {
                return stateManager.transform;
            }
        }

        /// <summary>
        /// 当状态进入时
        /// </summary>
        /// <param name="stateManager">状态管理</param>
        /// <param name="currentState">当前状态</param>
        /// <param name="nextState">下一个状态</param>
        public virtual void OnEnter(State currentState , State nextState ) { }

        /// <summary>
        /// 当状态每一帧
        /// </summary>
        /// <param name="stateManager">状态管理</param>
        /// <param name="currentState">当前状态</param>
        /// <param name="nextState">下一个状态</param>
        public virtual void OnUpdate(State currentState , State nextState ) { }

        /// <summary>
        /// 当状态退出后
        /// </summary>
        /// <param name="stateManager">状态管理</param>
        /// <param name="currentState">当前状态</param>
        /// <param name="nextState">下一个状态</param>
        public virtual void OnExit(State currentState , State nextState ) { }
	}
}