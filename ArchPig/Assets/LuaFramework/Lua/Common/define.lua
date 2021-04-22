
CtrlNames = {
	Login = "LoginCtrl",
	LoginBg = "LoginBgCtrl",
	Register = "RegisterCtrl",
	CreateRole = "CreateRoleCtrl",
	ItemInfo = "ItemInfoCtrl",
	MainUI = "MainUICtrl",
	Skill = "SkillCtrl",
	Bag = "BagCtrl",
	EachOtherRole = "EachOtherRoleCtrl",
	Message = "MessageCtrl",
	Battle = "BattleCtrl",
	BuffInfo = "BuffInfoCtrl",
	Shop = "ShopCtrl",
	Forge = "ForgeCtrl",
	Task = "TaskCtrl",
	HeadMgr = "HeadMgrCtrl",
	Talk = "TalkCtrl",
	Rank = "RankCtrl",
	Mail = 'MailCtrl',
	Auction = 'AuctionCtrl',
	Role = 'RoleCtrl'
}

PanelNames = {
	"LoginPanel",
	"LoginBgPanel",
	"RegisterPanel",
	"CreateRolePanel",
	"MainUIPanel",
	"BagPanel",
	"RolePanel",
	"ItemInfoPanel",
	"SkillPanel",
	"EachOtherRolePanel",
	"MessagePanel",
	"BattlePanel",
	"HeadMgrPanel",
	"BuffInfoPanel",
	"ShopPanel",
	"ForgePanel"
}

EnumNames = {
	"CtrlEnum",
	"EquipAttrEnum",
	"EventTriggerEnum",
	"InventoryEnum",
	"ItemEnum",
	"ModelEnum",
	"ProfessionEnum",
	"RoleEnum",
	"SchoolEnum",
	"UILayerEnum",
	"MaskEnum",
	"MessageEnum",
	"ActionType",
	"ForgeEnum",
	"NPCEnum",
	"TaskEnum",
	"RankEnum",
	"AuctionSortEnum",
	"ViewEnum"
}

ModelNames = {
	"InventoryMgr",
	"Item",
	"RoleInfoMgr",
	"UIMgr",
	"SkillMgr",
	"ClientMgr",
	"TaskManager",
}

ToolNames = {
	"OptimizeSV"
}


--协议类型--
ProtocalType = {
	BINARY = 0,
	PB_LUA = 1,
	PBC = 2,
	SPROTO = 3,
}
--当前使用的协议类型--
TestProtoType = ProtocalType.BINARY;

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;
ByteBuffer = LuaFramework.ByteBuffer;

ResMgr = LuaHelper.GetResManager();
PanelMgr = LuaHelper.GetPanelManager();
SoundMgr = LuaHelper.GetSoundManager();
AudioMgr = LuaHelper.GetAudioMgr();
--networkMgr = LuaHelper.GetNetManager();
EventMgr = LuaHelper.GetEventManager();
Scenes = LuaHelper.GetScenesManager();
Client = LuaHelper.GetClientManager();
ConfigMgr = LuaHelper.GetConfigManager();
PoolMgr = LuaHelper.GetObjectPoolManager();
StateMgr = LuaHelper.GetStateMgr();

RoleMgr = LuaHelper.GetRoleMgr();
BattleMgr = LuaHelper.GetBattleMgr();
CameraMgr = LuaHelper.GetCameraMgr();
NPCMgr = LuaHelper.GetNPCMgr();
RoleCtrl = LuaHelper.GetRoleCtrl();

WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;