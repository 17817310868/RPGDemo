#define USING_DOTWEENING

using UnityEngine;
using System;
using System.Collections.Generic;
using LuaInterface;
using LuaFramework;
using UnityEditor;

using BindType = ToLuaMenu.BindType;
using UnityEngine.UI;
using Net.Client;
using Net.Share;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.CodeDom;

public static class CustomSettings
{
    public static string FrameworkPath = AppConst.FrameworkRoot;
    public static string saveDir = FrameworkPath + "/ToLua/Source/Generate/";
    public static string luaDir = FrameworkPath + "/Lua/";
    public static string toluaBaseType = FrameworkPath + "/ToLua/BaseType/";
	public static string baseLuaDir = FrameworkPath + "/ToLua/Lua";
	public static string injectionFilesPath = Application.dataPath + "/ToLua/Injection/";

    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>
    {        
        typeof(UnityEngine.Application),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.Screen),
        typeof(UnityEngine.SleepTimeout),
        typeof(UnityEngine.Input),
        typeof(UnityEngine.Resources),
        typeof(UnityEngine.Physics),
        typeof(UnityEngine.RenderSettings),
        //typeof(UnityEngine.QualitySettings),
        typeof(UnityEngine.GL),
        typeof(UnityEngine.Graphics),
        //typeof(UnityEngine.UI.LayoutRebuilder)
    };

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList = 
    {        
        _DT(typeof(Action)),                
        _DT(typeof(UnityEngine.Events.UnityAction)),
        _DT(typeof(System.Predicate<int>)),
        _DT(typeof(System.Action<int>)),
        _DT(typeof(System.Comparison<int>)),
        _DT(typeof(System.Func<int, int>)),
        _DT(typeof(DG.Tweening.TweenCallback)),
    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList =
    {                
        //------------------------为例子导出--------------------------------
        //_GT(typeof(TestEventListener)),
        //_GT(typeof(TestProtol)),
        //_GT(typeof(TestAccount)),
        //_GT(typeof(Dictionary<int, TestAccount>)).SetLibName("AccountMap"),
        //_GT(typeof(KeyValuePair<int, TestAccount>)),
        //_GT(typeof(Dictionary<int, TestAccount>.KeyCollection)),
        //_GT(typeof(Dictionary<int, TestAccount>.ValueCollection)),
        //_GT(typeof(TestExport)),
        //_GT(typeof(TestExport.Space)),
        //-------------------------------------------------------------------        



        _GT(typeof(LuaInjectionStation)),
        _GT(typeof(InjectType)),
        _GT(typeof(Debugger)).SetNameSpace(null),          

#if USING_DOTWEENING
        _GT(typeof(DG.Tweening.DOTween)),
        _GT(typeof(DG.Tweening.Tween)).SetBaseType(typeof(System.Object)).AddExtendType(typeof(DG.Tweening.TweenExtensions)),
        _GT(typeof(DG.Tweening.Sequence)).AddExtendType(typeof(DG.Tweening.TweenSettingsExtensions)),
        _GT(typeof(DG.Tweening.Tweener)).AddExtendType(typeof(DG.Tweening.TweenSettingsExtensions)),
        _GT(typeof(DG.Tweening.LoopType)),
        _GT(typeof(DG.Tweening.PathMode)),
        _GT(typeof(DG.Tweening.PathType)),
        _GT(typeof(DG.Tweening.RotateMode)),
        _GT(typeof(Component)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Transform)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        //_GT(typeof(Light)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Material)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Rigidbody)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Camera)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(AudioSource)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        //_GT(typeof(LineRenderer)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        //_GT(typeof(TrailRenderer)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),    
#else

        _GT(typeof(Component)),
        _GT(typeof(Transform)),
        _GT(typeof(Material)),
        //_GT(typeof(Light)),
        _GT(typeof(Rigidbody)),
        _GT(typeof(Camera)),
        _GT(typeof(AudioSource)),
        //_GT(typeof(LineRenderer))
        //_GT(typeof(TrailRenderer))
#endif
        _GT(typeof(UnityEngine.Object)),
        _GT(typeof(Behaviour)),
        _GT(typeof(MonoBehaviour)),
        _GT(typeof(GameObject)),
        _GT(typeof(TrackedReference)),
        _GT(typeof(Application)),
        _GT(typeof(Physics)),
        _GT(typeof(Collider)),
        _GT(typeof(Time)),
        _GT(typeof(Texture)),
        _GT(typeof(Texture2D)),
        _GT(typeof(Shader)),
        _GT(typeof(Renderer)),
        _GT(typeof(WWW)),
        _GT(typeof(Screen)),
        _GT(typeof(CameraClearFlags)),
        _GT(typeof(AudioClip)),
        _GT(typeof(AssetBundle)),
        _GT(typeof(ParticleSystem)),
        _GT(typeof(AsyncOperation)).SetBaseType(typeof(System.Object)),
        _GT(typeof(LightType)),
        _GT(typeof(SleepTimeout)),
#if UNITY_5_3_OR_NEWER && !UNITY_5_6_OR_NEWER
        _GT(typeof(UnityEngine.Experimental.Director.DirectorPlayer)),
#endif
        _GT(typeof(Animator)),
        _GT(typeof(Input)),
        _GT(typeof(KeyCode)),
        _GT(typeof(SkinnedMeshRenderer)),
        _GT(typeof(Space)),


        _GT(typeof(MeshRenderer)),
#if !UNITY_5_4_OR_NEWER
        _GT(typeof(ParticleEmitter)),
        _GT(typeof(ParticleRenderer)),
        _GT(typeof(ParticleAnimator)), 
#endif

        _GT(typeof(BoxCollider)),
        _GT(typeof(MeshCollider)),
        _GT(typeof(SphereCollider)),
        _GT(typeof(CharacterController)),
        _GT(typeof(CapsuleCollider)),

        _GT(typeof(Animation)),
        _GT(typeof(AnimationClip)).SetBaseType(typeof(UnityEngine.Object)),
        _GT(typeof(AnimationState)),
        _GT(typeof(AnimationBlendMode)),
        _GT(typeof(QueueMode)),
        _GT(typeof(PlayMode)),
        _GT(typeof(WrapMode)),

        //_GT(typeof(QualitySettings)),
        _GT(typeof(RenderSettings)),
#if UNITY_2019
        _GT(typeof(SkinWeights)),
#else
        _GT(typeof(BlendWeights)),
#endif
        _GT(typeof(RenderTexture)), 
		_GT(typeof(Resources)),      
		_GT(typeof(LuaProfiler)),
          
        //for LuaFramework
        _GT(typeof(UIBehaviour)),
        _GT(typeof(Button)),
        _GT(typeof(Scrollbar)),
        _GT(typeof(RectTransform)),
        _GT(typeof(ScrollRect)),
        _GT(typeof(Rect)),
        _GT(typeof(Text)),
        _GT(typeof(InputField)),
        _GT(typeof(Toggle)),
        _GT(typeof(Slider)),
        _GT(typeof(RawImage)),
        _GT(typeof(Image)),
        _GT(typeof(ToggleGroup)),
        _GT(typeof(UnityEngine.UI.LayoutRebuilder)),
        _GT(typeof(Color)),

        _GT(typeof(Util)),
        _GT(typeof(AppConst)),
        _GT(typeof(LuaHelper)),
        _GT(typeof(ByteBuffer)),
        _GT(typeof(LuaBehaviour)),
        _GT(typeof(NetBehaviour)),
        _GT(typeof(Rpc)),
        _GT(typeof(RPCFun)),

        _GT(typeof(Actor)),
        _GT(typeof(buff)),
        _GT(typeof(Rank)),
        _GT(typeof(RoleLevelRank)),
        _GT(typeof(RolePowerRank)),

        //通信数据模型
        //_GT(typeof(MessageModel)),
        _GT(typeof(S2C_LoginOrRegisterCallback)),
        _GT(typeof(C2S_CreateMainRole)),
        _GT(typeof(S2C_OtherRoleInfo)),
        _GT(typeof(C2S_CheckInfo)),
        _GT(typeof(S2C_CheckInfo)),
        _GT(typeof(S2C_SceneInfo)),
        _GT(typeof(S2C_MainRoleInfo)),
        _GT(typeof(S2C_MoveInfo)),
        _GT(typeof(C2S_MoveInfo)),
        _GT(typeof(S2C_AddItemsInfo)),
        _GT(typeof(S2C_AddItemInfo)),
        _GT(typeof(C2S_BuyItem)),
        _GT(typeof(S2C_ReduceItem)),
        _GT(typeof(S2C_ReduceItems)),
        _GT(typeof(S2C_AddEquipInfo)),
        _GT(typeof(S2C_AddEquipsInfo)),
        _GT(typeof(C2S_UseItem)),
        _GT(typeof(C2S_DressEquip)),
        _GT(typeof(S2C_DressEquip)),
        _GT(typeof(C2S_TakeoffEquip)),
        _GT(typeof(S2C_TakeoffEquip)),
        _GT(typeof(C2S_DiscardItem)),
        _GT(typeof(C2S_MakeEquip)),
        _GT(typeof(C2S_AdvanceEquip)),
        _GT(typeof(C2S_InlayGem)),
        _GT(typeof(C2S_RemoveGem)),
        _GT(typeof(S2C_RemoveItemInfo)),
        _GT(typeof(S2C_RemoveItems)),
        _GT(typeof(S2C_UpdateItemInfo)),
        _GT(typeof(S2C_UpdateLeader)),
        _GT(typeof(S2C_AddSkill)),
        _GT(typeof(S2C_AddSkills)),
        _GT(typeof(S2C_RemoveSkill)),
        _GT(typeof(S2C_UpdateSkill)),
        _GT(typeof(C2S_UpgradeSkill)),
        _GT(typeof(C2S_LearnSkill)),
        _GT(typeof(S2C_TeamInfo)),
        _GT(typeof(C2S_JoinTeamRequest)),
        _GT(typeof(S2C_JoinTeamRequest)),
        _GT(typeof(C2S_JoinTeamReply)),
        _GT(typeof(S2C_JoinTeamReply)),
        _GT(typeof(C2S_VisiteRequest)),
        _GT(typeof(S2C_VisiteRequest)),
        _GT(typeof(C2S_VisiteReply)),
        _GT(typeof(S2C_VisiteReply)),
        _GT(typeof(C2S_BattleRequest)),
        _GT(typeof(S2C_BattleRequest)),
        _GT(typeof(C2S_BattleReply)),
        _GT(typeof(S2C_BattleReply)),
        _GT(typeof(C2S_BattleCommand)),
        _GT(typeof(S2C_Round)),
        _GT(typeof(S2C_BattlerInfo)),
        _GT(typeof(S2C_InitBattlers)),
        _GT(typeof(S2C_UpdateBattle)),
        _GT(typeof(S2C_UpdateBattles)),
        _GT(typeof(S2C_AcceptableTasks)),
        _GT(typeof(S2C_ConductTasks)),
        _GT(typeof(S2C_AllTaskInfo)),
        _GT(typeof(S2C_TaskProgress)),
        _GT(typeof(S2C_AcceptTask)),
        _GT(typeof(C2S_AcceptTask)),
        _GT(typeof(C2S_CompleteTask)),
        _GT(typeof(S2C_CompleteTask)),
        
        _GT(typeof(C2S_CheckRank)),
        _GT(typeof(S2C_RoleLevelRank)),
        _GT(typeof(S2C_RolePowerRank)),
        _GT(typeof(C2S_SendMail)),
        _GT(typeof(S2C_ReceiveMail)),
        _GT(typeof(S2C_ReceiveMails)),
        _GT(typeof(C2S_GetMailItems)),
        _GT(typeof(C2S_ReadMail)),

        _GT(typeof(C2S_Auction)),
        _GT(typeof(C2S_Bidding)),
        _GT(typeof(C2S_FixedBuy)),
        _GT(typeof(C2S_GetLots)),
        _GT(typeof(C2S_SearchLots)),
        _GT(typeof(S2C_LotsInfo)),
        _GT(typeof(S2C_LotsInfos)),
        


        //配置表
        //_GT(typeof(ConfigManager)),
        //_GT(typeof(ConfigClass)),
        //_GT(typeof(ProfessionConfig)),
        //_GT(typeof(SchoolConfig)),

        _GT(typeof(RoleCtrl)),
        _GT(typeof(RoleMgr)),
        _GT(typeof(NPCManager)),
        _GT(typeof(BattleMgr)),
        _GT(typeof(CameraMgr)),
        _GT(typeof(ClientManager)),
        _GT(typeof(GameManager)),
        _GT(typeof(LuaManager)),
        _GT(typeof(PanelManager)),
        _GT(typeof(SoundManager)),
        _GT(typeof(TimerManager)),
        _GT(typeof(ThreadManager)),
        _GT(typeof(NetworkManager)),
        _GT(typeof(ResourceManager)),		  
        _GT(typeof(EventManager)),
        _GT(typeof(ScenesManager)),
        _GT(typeof(ObjectPoolManager)),
        _GT(typeof(GameObjectPool)),
        _GT(typeof(StateMgr)),
        _GT(typeof(AudioManager)),

        


    };

    public static List<Type> dynamicList = new List<Type>()
    {
        typeof(MeshRenderer),
#if !UNITY_5_4_OR_NEWER
        typeof(ParticleEmitter),
        typeof(ParticleRenderer),
        typeof(ParticleAnimator),
#endif

        typeof(BoxCollider),
        typeof(MeshCollider),
        typeof(SphereCollider),
        typeof(CharacterController),
        typeof(CapsuleCollider),

        typeof(Animation),
        typeof(AnimationClip),
        typeof(AnimationState),
#if UNITY_2019
        typeof(SkinWeights),
#else
        typeof(BlendWeights),
#endif
        typeof(RenderTexture),
        typeof(Rigidbody),
    };

    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>()
    {
        
    };
        
    //ngui优化，下面的类没有派生类，可以作为sealed class
    public static List<Type> sealedList = new List<Type>()
    {
        /*typeof(Transform),
        typeof(UIRoot),
        typeof(UICamera),
        typeof(UIViewport),
        typeof(UIPanel),
        typeof(UILabel),
        typeof(UIAnchor),
        typeof(UIAtlas),
        typeof(UIFont),
        typeof(UITexture),
        typeof(UISprite),
        typeof(UIGrid),
        typeof(UITable),
        typeof(UIWrapGrid),
        typeof(UIInput),
        typeof(UIScrollView),
        typeof(UIEventListener),
        typeof(UIScrollBar),
        typeof(UICenterOnChild),
        typeof(UIScrollView),        
        typeof(UIButton),
        typeof(UITextList),
        typeof(UIPlayTween),
        typeof(UIDragScrollView),
        typeof(UISpriteAnimation),
        typeof(UIWrapContent),
        typeof(TweenWidth),
        typeof(TweenAlpha),
        typeof(TweenColor),
        typeof(TweenRotation),
        typeof(TweenPosition),
        typeof(TweenScale),
        typeof(TweenHeight),
        typeof(TypewriterEffect),
        typeof(UIToggle),
        typeof(Localization),*/
    };

    public static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    public static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }    


    [MenuItem("Lua/Attach Profiler", false, 151)]
    static void AttachProfiler()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("警告", "请在运行时执行此功能", "确定");
            return;
        }
        
        LuaClient.Instance.AttachProfiler();
    }

    [MenuItem("Lua/Detach Profiler", false, 152)]
    static void DetachProfiler()
    {
        if (!Application.isPlaying)
        {            
            return;
        }

        LuaClient.Instance.DetachProfiler();
    }
}
