﻿using UnityEngine;
using System.Collections.Generic;

namespace GameDesigner
{
    /// <summary>
    /// 动画模式
    /// </summary>
    public enum AnimationMode
    {
        /// <summary>
        /// 旧版动画
        /// </summary>
        Animation,
        /// <summary>
        /// 新版动画
        /// </summary>
        Animator
    }

	/// <summary>
	/// 状态 -- v2017/12/6
	/// </summary>
	public sealed class State : IState
	{
        /// <summary>
        /// 状态ID
        /// </summary>
		public int stateID = 0;
        /// <summary>
        /// 下一个状态
        /// </summary>
		public State nextState = null;
        /// <summary>
        /// 状态连接集合
        /// </summary>
		public List<Transition> transitions = new List<Transition> ();
        /// <summary>
        /// 状态行为集合
        /// </summary>
		public List<StateBehaviour> behaviours = new List<StateBehaviour> ();
        /// <summary>
        /// 动作系统 使用为真 , 不使用为假
        /// </summary>
		public bool actionSystem = false;
        /// <summary>
        /// 旧动画系统
        /// </summary>
        public Animation anim = null;
        /// <summary>
        /// 新动画系统
        /// </summary>
        public Animator animator = null;
        /// <summary>
        /// 动画循环?
        /// </summary>
        public bool animLoop = true;
        /// <summary>
        /// 动画模式
        /// </summary>
        public AnimPlayMode animPlayMode = AnimPlayMode.Random;
        /// <summary>
        /// 动作索引
        /// </summary>
		public int actionIndex = 0;
        /// <summary>
        /// 音效索引
        /// </summary>
		public int audioIndex = 0;
        /// <summary>
        /// 动画速度
        /// </summary>
		public float animSpeed = 1;
        /// <summary>
        /// 动画结束是否进入下一个状态
        /// </summary>
        public bool isExitState = false;
        /// <summary>
        /// 动画结束进入下一个状态的ID
        /// </summary>
        public int DstStateID = 0;

        /// <summary>
        /// 状态动作集合
        /// </summary>
		public List<StateAction> actions = new List<StateAction> ();

        private State() { }

        /// <summary>
        /// 创建状态
        /// </summary>
		public static State CreateStateInstance (StateMachine stateMachine , string stateName, Vector2 position)
		{
			State state = new GameObject (stateName).AddComponent<State> ();
			state.transform.hideFlags = HideFlags.None;
			state.stateMachine = stateMachine;
			state.name = stateName;
			state.rect.position = position;
			stateMachine.states.Add (state); 
			state.transform.SetParent (stateMachine.transform);
			state.actions.Add (new StateAction ());
			return state;
		}

        /// <summary>
        /// 构造函数
        /// </summary>
		public State (StateMachine _stateMachine)
		{
			stateMachine = _stateMachine;
		}

        /// <summary>
        /// 构造函数
        /// </summary>
		public State (StateMachine _stateMachine, Vector2 position)
		{
			stateMachine = _stateMachine;
			rect.position = position;
		}

		/// <summary>
        /// 当前状态动作
        /// </summary>
		public StateAction Action{
			get{
				if (actionIndex >= actions.Count)
					actionIndex = 0;
				return actions [actionIndex];
			}
		}
        
        void Awake ()
		{
            enabled = true;
			anim = stateManager.GetComponent<Animation> ();
            animator = stateManager.GetComponent<Animator>();
        }

		void OnDestroy ()
		{
			foreach (StateAction act in actions) { // 当意外状态物体被销毁,删除对象池物体
				foreach (GameObject go in act.activeObjs) {
					if (go != null) {
						Destroy (go);
					}
				}
			}
		}

        /// <summary>
        /// 进入状态
        /// </summary>
		public void OnEnterState ()
		{
            foreach (ActionBehaviour behaviour in Action.behaviours) { //当子动作的动画开始进入时调用
				if (behaviour.Active) {
					behaviour.OnEnter (Action);
				}
			}
            
            if (animPlayMode == AnimPlayMode.Random) {
				actionIndex = Random.Range (0, actions.Count);
			} else {
				actionIndex = (actionIndex < actions.Count - 1) ? actionIndex + 1 : 0;
			}

			if (Action.isPlayAudio & Action.audioModel == AudioMode.EnterPlayAudio) {
				audioIndex = Random.Range (0, Action.audioClips.Count);
                AudioManager.Play(Action.audioClips[audioIndex]);
            }
            
			try {
                switch (stateMachine.animMode) {
                    case AnimationMode.Animation:
                    anim[Action.clipName].speed = animSpeed;
                    anim.Rewind(Action.clipName);
                    anim.Play(Action.clipName);
                    break;
                    case AnimationMode.Animator:
                    animator.speed = animSpeed;
                    animator.Rebind();
                    animator.Play(Action.clipName);
                    break;
                }
            } catch {
				Debug.Log ("需要动画组件或动画剪辑获取失败!");
			}
		}

        /// <summary>
        /// 状态每一帧
        /// </summary>
		public void OnUpdateState (StateManager stateManager, State state)
		{
            var isPlaying = true;
            switch (stateMachine.animMode) {
                case AnimationMode.Animation:
                Action.animTime = anim[Action.clipName].time / anim[Action.clipName].length * 100;
                isPlaying = anim.isPlaying;
                break;
                case AnimationMode.Animator:
                Action.animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime / 1 * 100;
                break;
            }
            if (Action.animTime >= Action.animEventTime & !Action.eventEnter) {
				if (Action.effectSpwan != null) {
					if (Action.activeMode == ActiveMode.Instantiate) {
                        Destroy(InstantiateSpwan (stateManager), Action.spwanTime);
					}
					if (Action.activeMode == ActiveMode.SetActive) {
						bool active = false;
						foreach (GameObject go in Action.activeObjs) {
							if (go == null) {
								Action.activeObjs.Remove (go);
								break;
							}
							if (!go.activeSelf) {
								go.SetActive (true);
								go.transform.SetParent (null);
								SetPosition (stateManager, go);
								active = true;
								break;
							}
						}
						if (!active) {
							GameObject go = InstantiateSpwan (stateManager);
							Action.activeObjs.Add (go);
						}
					}
				}
				if (Action.isPlayAudio & Action.audioModel == AudioMode.AnimEventPlayAudio) {
					audioIndex = Random.Range (0, Action.audioClips.Count);
                    AudioManager.Play(Action.audioClips[audioIndex]);
				}
				Action.eventEnter = true;

				foreach (ActionBehaviour behaviour in Action.behaviours) { //当子动作的动画事件进入
					if (behaviour.Active) {
						behaviour.OnAnimationEvent (Action, Action.animEventTime);
					}
				}
			}

			if (Action.animTime >= Action.animTimeMax | !isPlaying) {
                Action.eventEnter = false;
				if (isExitState & state.transitions.Count != 0) {
					state.transitions [DstStateID].isEnterNextState = true;
					return;
				}
				if (animLoop) {
					OnExitState ();//退出函数
					OnEnterState ();//重载进入函数
                    return;
				}else{
                    OnStop();
                }
			}
            
            foreach (ActionBehaviour behaviour in Action.behaviours) {
				if (behaviour.Active) {
					behaviour.OnUpdate (Action);
				}
			}
		}

        /// <summary>
        /// 设置技能位置
        /// </summary>
		private void SetPosition (StateManager stateManager, GameObject go)
		{
			switch (Action.spwanmode) {
			case SpwanMode.localPosition:
				go.transform.localPosition = stateManager.transform.TransformPoint (Action.effectPostion);
				go.transform.rotation = stateManager.transform.rotation;
				break;
			case SpwanMode.SetParent:
				Action.parent = Action.parent ? Action.parent : stateManager.transform;
				go.transform.SetParent (Action.parent);
				go.transform.position = Action.parent.TransformPoint (Action.effectPostion);
				go.transform.rotation = Action.parent.rotation;
				break;
			case SpwanMode.SetInTargetPosition:
				Action.parent = Action.parent ? Action.parent : stateManager.transform;
				go.transform.SetParent (Action.parent);
				go.transform.position = Action.parent.TransformPoint (Action.effectPostion);
				go.transform.rotation = Action.parent.rotation;
				go.transform.parent = null;
				break;
			}
			foreach (ActionBehaviour behaviour in Action.behaviours) { // 当实例化技能物体调用
				if (behaviour.Active) {
					behaviour.OnInstantiateSpwan (Action, go);
				}
			}
		}

        /// <summary>
        /// 技能实例化
        /// </summary>
		private GameObject InstantiateSpwan (StateManager stateManager)
		{
			GameObject go = (GameObject)Instantiate (Action.effectSpwan);
			SetPosition (stateManager, go);
			return go;
		}

        /// <summary>
        /// 当退出状态
        /// </summary>
		public void OnExitState ()
		{
            if (Action.isPlayAudio & Action.audioModel == AudioMode.ExitPlayAudio) {
				audioIndex = Random.Range (0, Action.audioClips.Count);
                AudioManager.Play(Action.audioClips[audioIndex]);
            }
			foreach (ActionBehaviour behaviour in Action.behaviours) { //当子动作结束
				if (behaviour.Active) {
					behaviour.OnExit (Action);
				}
			}
			Action.eventEnter = false;
		}

        /// <summary>
        /// 当动作停止
        /// </summary>
        public void OnStop()
        {
            foreach (ActionBehaviour behaviour in Action.behaviours) { //当子动作停止
                if (behaviour.Active) {
                    behaviour.OnStop(Action);
                }
            }
        }
    }
}