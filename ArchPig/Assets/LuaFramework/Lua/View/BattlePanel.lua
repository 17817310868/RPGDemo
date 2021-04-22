
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:战斗面板视图层
*
*        description:
*            功能描述:实现战斗面板显示逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


BattlePanel = {}
local this = BattlePanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
BattlePanel.Ctrls = 
{
    Img_commonds = {  Path = 'Auto_commonds',  ControlType = 'Image'  },
    Img_automatic = {  Path = 'Auto_commonds/Auto_automatic',  ControlType = 'Image'  },
    Img_prop = {  Path = 'Auto_commonds/Auto_prop',  ControlType = 'Image'  },
    Img_skill = {  Path = 'Auto_commonds/Auto_skill',  ControlType = 'Image'  },
    Img_attack = {  Path = 'Auto_commonds/Auto_attack',  ControlType = 'Image'  },
    Img_escape = {  Path = 'Auto_commonds/Auto_escape',  ControlType = 'Image'  },
    Img_box = {  Path = 'Auto_box',  ControlType = 'Image'  },
    Img_scrollbar = {  Path = 'Auto_box/mask/Auto_scrollbar',  ControlType = 'Image'  },
    Img_content = {  Path = 'Auto_box/mask/Auto_content',  ControlType = 'Image'  },
    Img_timeBg = {  Path = 'Auto_timeBg',  ControlType = 'Image'  },
    Img_battleSettle = {  Path = 'Auto_battleSettle',  ControlType = 'Image'  },
    Img_win = {  Path = 'Auto_battleSettle/Auto_win',  ControlType = 'Image'  },
    Img_lose = {  Path = 'Auto_battleSettle/Auto_lose',  ControlType = 'Image'  },
    Img_back = {  Path = 'Auto_battleSettle/Auto_back',  ControlType = 'Image'  },
    Text_round = {  Path = 'round/Auto_round',  ControlType = 'Text'  },
    Text_time = {  Path = 'Auto_timeBg/Auto_time',  ControlType = 'Text'  },
    Text_actionTips = {  Path = 'Auto_actionTips',  ControlType = 'Text'  },
    Btn_automatic = {  Path = 'Auto_commonds/Auto_automatic',  ControlType = 'Button'  },
    Btn_prop = {  Path = 'Auto_commonds/Auto_prop',  ControlType = 'Button'  },
    Btn_skill = {  Path = 'Auto_commonds/Auto_skill',  ControlType = 'Button'  },
    Btn_attack = {  Path = 'Auto_commonds/Auto_attack',  ControlType = 'Button'  },
    Btn_escape = {  Path = 'Auto_commonds/Auto_escape',  ControlType = 'Button'  },
    Btn_back = {  Path = 'Auto_battleSettle/Auto_back',  ControlType = 'Button'  },
    Scrollbar_scrollbar = {  Path = 'Auto_box/mask/Auto_scrollbar',  ControlType = 'Scrollbar'  },
}

local SkillConfig = require "Config/SkillConfig"
local ItemConfig = require "Config/ItemConfig"

local gameObject
local transform

local actionType
local paramId

local roundObserver  --回合开始监听观察者
local showAnimObserver  --播放动画开始监听观察者
local actionObserver  --行动监听观察者
local timeObserver  --倒计时监听观察者
local battleEndObserver  --战斗结束监听
--local escapeObserver  --逃跑监听

function BattlePanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function BattlePanel:Init()
    --[[
    escapeObserver = Observer:New(function ()
        self:Escape()
    end)
    --]]
    --战斗结束监听
    battleEndObserver = Observer:New(function (isWin)
        --根据战斗胜负显示战斗结束面板
        self:BattleEnd(isWin)
    end)

    --计时监听
    timeObserver = Observer:New(function (time)
        --根据传进来的数目更新计时提示
        self.Text_time.text = tostring(time)
    end)

    --动画播放开始监听
    showAnimObserver = Observer:New(function ()
        --当动画开始播放，清空战斗提示，移除该监听
        self.Text_actionTips.text = ""
        UIMgr:RemoveListener("ShowBattleAnim",showAnimObserver)
    end)

    --行动指令操作监听
    actionObserver = Observer:New(function (actorIndex,victim)
        --发送的目标站位索引不规范，提示玩家选择攻击目标
        if victim == -1 then
            self.Text_actionTips.text = "请选择攻击目标"
            UIMgr:SetActive(self.Img_commonds.transform,true)
            return
        end
        --发送的行动类型不规范，提示玩家选择攻击类型
        if actionType < 1 or actionType > ActionType.Escape then
            self.Text_actionTips.text = '请选择攻击类型'
            UIMgr:SetActive(self.Img_commonds.transform,true)
            return;
        end
        --发送的行动类型若为技能或物品，则判断具体参数是否规范
        if actionType == ActionType.Skill or actionType == ActionType.Item then
            if paramId == -1 then
                self.Text_actionTips.text = '请选择攻击类型'
                UIMgr:SetActive(self.Img_commonds.transform,true)
                return;
            end
        end
        Client.Send('AddBattleCommand',C2S_BattleCommand.New(actorIndex,victim,actionType,paramId))  --向服务端发送战斗指令
        UIMgr:RemoveListener("Time",timeObserver)  --移除计时监听
        UIMgr:SetActive(self.Img_timeBg.transform,false)  --隐藏计时背景
        UIMgr:SetActive(self.Img_commonds.transform,false)  --隐藏战斗指令操作图标
        UIMgr:SetActive(self.Img_box.transform,false)  --隐藏盒子
        self.Text_actionTips.text = "等待其他玩家"  --更新战斗提示
        UIMgr:RemoveListener("Action",actionObserver)  --移除行动监听
    end)

    --新的回合，由服务端发送信号过来触发
    roundObserver = Observer:New(function (round)
        --清空操作提示
        self.Text_actionTips.text = ""
        self.Text_round.text = '第'..round..'回合'  --更新回合数
        UIMgr:SetActive(self.Img_timeBg.transform,true)  --显示回合数背景
        UIMgr:AddListener("Time",timeObserver)  --监听时间，由战斗管理器触发更新计时数
        UIMgr:SetActive(self.Img_commonds.transform,true)  --显示战斗指令图标
        UIMgr:SetActive(self.Img_box.transform,false)  --隐藏技能或物品盒子
        actionType = -1  --初始化行动类型和行动参数
        paramId = -1
        --监听是否开始播放动画
        UIMgr:AddListener('ShowBattleAnim',showAnimObserver)
        --监听玩家行动操作
        UIMgr:AddListener('Action',actionObserver)
    end)
    

    PanelMgr:ButtonAddListener(self.Btn_attack,function ()
        actionType = ActionType.Attack
        self:ShowAttack()
    end)
    PanelMgr:ButtonAddListener(self.Btn_skill,function ()
        actionType = ActionType.Skill
        self:ShowSkill()
    end)
    PanelMgr:ButtonAddListener(self.Btn_prop,function ()
        actionType = ActionType.Item
        self:ShowProp()
    end)
    PanelMgr:ButtonAddListener(self.Btn_escape,function ()
        actionType = ActionType.Escape
    end)

    PanelMgr:ButtonAddListener(self.Btn_back,function()
        AudioMgr:PlayBgm('taoyuanzhen')
        PanelMgr:ClosePanel();
        CameraMgr:ChangeFollow()
        PanelMgr:OpenPanel('MainUIPanel',UILayer.Middle,MaskEnum.None,function ()
            MainUICtrl:UpdateTeamInfo()
            UIMgr:Trigger('UpdateRoleInfo')
        end)
    end)
end

function BattlePanel:Show()
    AudioMgr:PlayBgm('battle')
    print("显示战斗")
    --设置全屏显示此面板
    PanelMgr:FullScreen(transform:GetComponent('RectTransform'))

    UIMgr:SetActive(self.Img_battleSettle.transform,false)
    --[[
    UIMgr:AddListener("NewRound",function (round)
        self.Text_actionTips.text = ""
        self.Text_round.text = '第'..round..'回合'
        UIMgr:SetActive(self.Img_timeBg.transform,true)
        UIMgr:AddListener("Time",function (time)
            self.Text_time.text = tostring(time)
        end)
        UIMgr:SetActive(self.Img_commonds.transform,true)
        UIMgr:SetActive(self.Img_box.transform,false)
        actionType = -1
        paramId = -1
        UIMgr:AddListener("ShowBattleAnim",function ()
            self.Text_actionTips.text = ""
            UIMgr:RemoveListener("ShowBattleAnim")
        end)
        UIMgr:AddListener("Action",function (actorIndex,victim)
            if victim == -1 then
                self.Text_actionTips.text = "请选择攻击目标"
                UIMgr:SetActive(self.Img_commonds.transform,true)
                return
            end
            if actionType < 1 or actionType > ActionType.Escape then
                self.Text_actionTips.text = '请选择攻击类型'
                UIMgr:SetActive(self.Img_commonds.transform,true)
                return;
            end
            if actionType == ActionType.Skill or actionType == ActionType.Item then
                if paramId == -1 then
                    self.Text_actionTips.text = '请选择攻击类型'
                    UIMgr:SetActive(self.Img_commonds.transform,true)
                    return;
                end
            end
            Client.Send('AddBattleCommand',C2S_BattleCommand.New(actorIndex,victim,actionType,paramId))
            UIMgr:RemoveListener("Time")
            UIMgr:SetActive(self.Img_timeBg.transform,false)
            UIMgr:SetActive(self.Img_commonds.transform,false)
            UIMgr:SetActive(self.Img_box.transform,false)
            self.Text_actionTips.text = "等待其他玩家"
            UIMgr:RemoveListener("Action")
        end)
    end)
    
    --]]

    UIMgr:AddListener('NewRound',roundObserver)
    UIMgr:AddListener("BattleEnd",battleEndObserver)
    --UIMgr:AddListener("Escape",escapeObserver)
    Client.MySend("AddBattleCallback")
end

function BattlePanel:Hide()
    UIMgr:RemoveListener("NewRound",roundObserver)
    UIMgr:RemoveListener("BattleEnd",battleEndObserver)
    --UIMgr:RemoveListener("Escape",escapeObserver)
end

function BattlePanel:ClearItem()
    local items = Util.GetChildrens(self.Img_content.transform)
	if items == nil or items.Length == 1 then
		return;
	end
	for index = 1,items.Length-1 do
		if items[index].name == 'Item' then
			Util.ToggleRemoveListener(items[index]:GetComponent('Toggle'))
			PoolMgr:Set(items[index].name,items[index].gameObject)
		end
    end
end

function BattlePanel:ShowAttack()
    UIMgr:SetActive(self.Img_commonds.transform,false)
    UIMgr:SetActive(self.Img_box.transform,false)
    self.Text_actionTips.text = "请选择攻击目标"
end

function BattlePanel:ShowSkill()
    self:ClearItem()
    UIMgr:SetActive(self.Img_box.transform,true)
    local skills = SkillMgr:GetAllSkills()
    if skills == nil then
        return;
    end
    for index = 1,#skills do
        PoolMgr:Get("Item",function (item)
            item.transform:SetParent(self.Img_content.transform)
            ResMgr:LoadAssetSprite('skillicon',{SkillConfig[skills[index]].icon},function (icon)
                item.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
            end)
            ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
                item.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
            end);
            item.transform:Find('Auto_itemCount'):GetComponent('Text').text = ""
            item.transform:GetComponent('Toggle').group = self.Img_box.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(item.transform:GetComponent('Toggle'),function (isChoose)
                if not isChoose then
                    return;
                end
                paramId = skills[index]
                UIMgr:SetActive(self.Img_commonds.transform,false)
                UIMgr:SetActive(self.Img_box.transform,false)
                self.Text_actionTips.text = "请选择攻击目标"
            end)
        end)
    end
end

function BattlePanel:ShowProp()
    self:ClearItem()
    UIMgr:SetActive(self.Img_box.transform,true)
    local items = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Item)
    if items == nil then
        return;
    end
    for index = 1,#items do 
		PoolMgr:Get("Item",function (item)
			item.transform:SetParent(self.Img_content.transform)
			ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
                item.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
            end);
            item.transform:Find('Auto_itemCount'):GetComponent('Text').text = tostring(items[index].GetItemCount());
            ResMgr:LoadAssetSprite('itemicon',{ItemConfig[items[index]:GetItemId()].icon},function (icon)
                item.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
            end);
            PanelMgr:ToggleAddListener(item.transform:GetComponent('Toggle'),function (isChoose)
                if not isChoose then
                    return;
                end
                paramId = items[index]:GetItemId()
                UIMgr:SetActive(self.Img_commonds.transform,false)
                UIMgr:SetActive(self.Img_box.transform,false)
                self.Text_actionTips.text = "请选择攻击目标"
            end);
		end)
    end
end

function BattlePanel:BattleEnd(isWin)
    UIMgr:SetActive(self.Img_battleSettle.transform,true)
    if isWin == true then
        UIMgr:SetActive(self.Img_win.transform,true)
        UIMgr:SetActive(self.Img_lose.transform,false)
    else 
        UIMgr:SetActive(self.Img_win.transform,false)
        UIMgr:SetActive(self.Img_lose.transform,true)
    end
end
--[[
function BattlePanel:Escape()
    AudioMgr:PlayBgm('taoyuanzhen')
    PanelMgr:ClosePanel();
    CameraMgr:ChangeFollow()
    PanelMgr:OpenPanel('MainUIPanel',UILayer.Middle,MaskEnum.None,function ()
        MainUICtrl:UpdateTeamInfo()
        UIMgr:Trigger('UpdateRoleInfo')
    end)
end
--]]