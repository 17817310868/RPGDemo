﻿namespace GameDesigner
{
    using Net.Client;
    using UnityEngine;

    /// <summary>
    /// 状态行为基类 2019.3.3
    /// </summary>
    public abstract class IBehaviour : MonoBehaviour
	{
        /// <summary>
        /// 展开编辑器检视面板
        /// </summary>
        [HideInInspector]
		public bool show = true;

        /// <summary>
        /// 脚本是否启用?
        /// </summary>
        public bool Active{
			get{ return enabled; }
			set{ enabled = value; }
		}

		private StateMachine _stateMachine = null;
        /// <summary>
        /// 状态机
        /// </summary>
		public StateMachine stateMachine{
			get{
				if(_stateMachine == null){
					_stateMachine = transform.GetComponentInParent<StateMachine>();
				}
				return _stateMachine;
			}
			set{ _stateMachine = value; }
		}

		private StateManager _stateManager = null;
        /// <summary>
        /// 状态管理
        /// </summary>
		public StateManager stateManager{
			get{
				if(_stateManager == null){
					_stateManager = transform.GetComponentInParent<StateManager>();
				}
				return _stateManager;
			}
			set{ _stateManager = value; }
		}

		private State _state = null;
        /// <summary>
        /// 当前状态
        /// </summary>
        public State state{
			get{
				if(_state==null)
					_state = transform.GetComponentInParent<State> ();
				return _state;
			}
		}
        
        /// <summary>
        /// 当组件被删除调用一次
        /// </summary>
        public virtual void OnDestroyComponent() { }

        /// <summary>
        /// 当绘制编辑器检视面板 (重要提示!你想自定义编辑器检视面板则返回真,否则显示默认编辑器检视面板)
        /// </summary>
        /// <param name="state">当前状态</param>
        /// <returns></returns>
        public virtual bool OnInspectorGUI( State state )
		{
			return false; //返回假: 绘制默认监视面板 | 返回真: 绘制扩展自定义监视面板
		}
    }
}