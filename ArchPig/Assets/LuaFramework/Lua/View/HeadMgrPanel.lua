
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:人物头顶管理面板
*
*        description:
*            功能描述:管理所有人物头顶预制体的显示
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

local BuffConfig = require 'Config/BuffConfig'


HeadMgrPanel = {}
local this = HeadMgrPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
HeadMgrPanel.Ctrls = 
{
}

local gameObject
local transform

local addheadObserver 
local addDetailHeadObserver
local addHeadOtherInfoObserver
local removeHeadObserver
local clearHeadObserver
local updateHeadPositionObserver
local updateHeadLeaderObserver
local updateHeadBattleObserver
local updateHeadNameObserver
local updateHeadOtherInfoObserver
local hurtAnimObserver
local updateBuffObserver

local Heads = {}

function HeadMgrPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function HeadMgrPanel:Init()
    addheadObserver = Observer:New(function (Guid,position,isLeader,isBattle,name,func)
        self:AddHead(Guid,position,isLeader,isBattle,name,func)
    end)

    addDetailHeadObserver = Observer:New(function (Guid,position,isLeader,isBattle,name,hp,maxHp,mp,maxMp)
        self:AddDetailHead(Guid,position,isLeader,isBattle,name,hp,maxHp,mp,maxMp)
    end)

    addHeadOtherInfoObserver = Observer:New(function (Guid,hp,maxHp,mp,maxMp)
        self:AddHeadOtherInfo(Guid,hp,maxHp,mp,maxMp)
    end)

    removeHeadObserver = Observer:New(function(Guid)
        self:RemoveHead(Guid)
    end)

    clearHeadObserver = Observer:New(function ()
        self:ClearHead()
    end)

    updateHeadPositionObserver = Observer:New(function ()
        self:UpdateHeadPosition()
    end)

    updateHeadLeaderObserver = Observer:New(function (Guid,isLeader)
        self:UpdateHeadLeader(Guid,isLeader)
    end)

    updateHeadBattleObserver = Observer:New(function (Guid, isBattle)
        print(2)
        self:UpdateHeadBattle(Guid,isBattle)
    end)

    updateHeadNameObserver = Observer:New(function (Guid,name)
        self:UpdateHeadName(Guid,name)
    end)

    updateHeadOtherInfoObserver = Observer:New(function (Guid,hp,maxHp,mp,maxMp)
        self:UpdateHeadOtherInfo(Guid,hp,maxHp,mp,maxMp)
    end)

    hurtAnimObserver = Observer:New(function (positionStart,positionEnd,damage)
        self:HurtAnim(positionStart,positionEnd,damage)
    end)

    updateBuffObserver = Observer:New(function (Guid,buffs)
        self:UpdateBuff(Guid,buffs)
    end)
end

function HeadMgrPanel:Show()
    --设置全屏显示此面板
    PanelMgr:FullScreen(transform:GetComponent('RectTransform'))
    
    UIMgr:AddListener('AddHead',addheadObserver)

    UIMgr:AddListener('AddDetailHead',addDetailHeadObserver)

    UIMgr:AddListener('AddHeadOtherInfo',addHeadOtherInfoObserver)

    UIMgr:AddListener('RemoveHead',removeHeadObserver)

    UIMgr:AddListener('ClearHead',clearHeadObserver)

    UIMgr:AddListener('UpdateHeadPosition',updateHeadPositionObserver)

    UIMgr:AddListener('UpdateHeadLeader',updateHeadLeaderObserver)

    UIMgr:AddListener('UpdateHeadBattle',updateHeadBattleObserver)

    UIMgr:AddListener('UpdateHeadName',updateHeadNameObserver)
    
    UIMgr:AddListener('UpdateHeadOtherInfo',updateHeadOtherInfoObserver)

    UIMgr:AddListener("HurtAnim",hurtAnimObserver)

    UIMgr:AddListener("UpdateBuff",updateBuffObserver)
    
end

function HeadMgrPanel:Hide()

    UIMgr:RemoveListener('AddHead',addheadObserver)

    UIMgr:RemoveListener('AddDetailHead',addDetailHeadObserver)

    UIMgr:RemoveListener('AddHeadOtherInfo',addHeadOtherInfoObserver)

    UIMgr:RemoveListener('RemoveHead',removeHeadObserver)

    UIMgr:RemoveListener('ClearHead',clearHeadObserver)

    UIMgr:RemoveListener('UpdateHeadPosition',updateHeadPositionObserver)

    UIMgr:RemoveListener('UpdateHeadLeader',updateHeadLeaderObserver)

    UIMgr:RemoveListener('UpdateHeadBattle',updateHeadBattleObserver)

    UIMgr:RemoveListener('UpdateHeadName',updateHeadNameObserver)
    
    UIMgr:RemoveListener('UpdateHeadOtherInfo',updateHeadOtherInfoObserver)

    UIMgr:RemoveListener("HurtAnim",hurtAnimObserver)

    UIMgr:RemoveListener("UpdateBuff",updateBuffObserver)

    self:ClearHead()

end

--以角色Guid为键添加一个头顶面板
function HeadMgrPanel:AddHead(Guid,position,isLeader,isBattle,name,func)
    if Heads[Guid] ~= nil then
        print(Guid..'该id对应的头顶面板已存在，不允许重复添加')
        return
    end
    PoolMgr:Get("Head",function (head)
        --添加改头顶面板进字典中
        Heads[Guid] = head.transform
        --默认隐藏其他信息
        UIMgr:SetActive(head.transform:Find('Auto_otherInfo'),false)
        --默认隐藏npc图标
        UIMgr:SetActive(head.transform:Find('Auto_npc'),false)
        --默认隐藏任务图标
        UIMgr:SetActive(head.transform:Find('Auto_task'),false)
        --将该头顶面板设置为头顶面板管理的子物体
        head.transform:SetParent(transform)
        --更新队长图标
        self:UpdateHeadLeader(Guid,isLeader)
        --更新是否再战斗图标
        self:UpdateHeadBattle(Guid,isBattle)
        --更新角色名称
        self:UpdateHeadName(Guid,name)
        --更新头顶面板位置
        self:UpdateHeadPosition(Guid,position)
        if func ~= nil then
            func()
        end
    end)
end


function HeadMgrPanel:AddDetailHead(Guid,position,isLeader,isBattle,name,hp,maxHp,mp,maxMp)
    self:AddHead(Guid,position,isLeader,isBattle,name, function ()
        --添加其他信息
        self:AddHeadOtherInfo(Guid,hp,maxHp,mp,maxMp)
    end)
end

--将指定角色Guid对应的头顶面板显示其他信息
function HeadMgrPanel:AddHeadOtherInfo(Guid,hp,maxHp,mp,maxMp)
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    local head = Heads[Guid]
    UIMgr:SetActive(head:Find('Auto_otherInfo'),true)
    self:UpdateHeadOtherInfo(Guid,hp,maxHp,mp,maxMp)
end

--移除指定角色Guid对应的头顶面板
function HeadMgrPanel:RemoveHead(Guid)
    if Heads[Guid] ~= nil then
        self:ClearBuff(Guid)
        PoolMgr:Set(Heads[Guid].name,Heads[Guid].gameObject)
    end
    Heads[Guid] = nil
end

--清空所有头顶面板
function HeadMgrPanel:ClearHead()
    for key,value in pairs(Heads) do
        self:RemoveHead(key)
    end
end

--更新指定角色Guid对应头顶面板的位置
function HeadMgrPanel:UpdateHeadPosition()

    for key,value in pairs(Heads) do
        if type(key) == 'string' then
            value.position = RoleMgr:GetRolePosToUI(key)
        else
            value.position = BattleMgr:GetBattlerPosToUI(key)
        end
    end

    --[[
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    Heads[Guid].position = position

    --]]
end

--更新指定角色Guid对应头顶面板的队长图标
function HeadMgrPanel:UpdateHeadLeader(Guid,isLeader)
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    local head = Heads[Guid]
    if isLeader == true then
        UIMgr:SetActive(head:Find('base/Auto_leader'),true)
    else 
        UIMgr:SetActive(head:Find('base/Auto_leader'),false)
    end
end

--更新指定角色Guid对应头顶面板的名称
function HeadMgrPanel:UpdateHeadName(Guid,name)
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    local head = Heads[Guid]
    head:Find('base/Auto_name'):GetComponent('Text').text = name
end

--更新指定角色Guid对应头顶面板的其他信息
function HeadMgrPanel:UpdateHeadOtherInfo(Guid,hp,maxHp,mp,maxMp)
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    local head = Heads[Guid]
    head:Find('Auto_otherInfo/Auto_hpSlider'):GetComponent('Slider').value = hp/maxHp
    head:Find('Auto_otherInfo/Auto_mpSlider'):GetComponent('Slider').value = mp/maxMp
    head:Find('Auto_otherInfo/Auto_hpSlider/Auto_hpText'):GetComponent('Text').text = tostring(hp)..'/'..tostring(maxHp)
    head:Find('Auto_otherInfo/Auto_mpSlider/Auto_mpText'):GetComponent('Text').text = tostring(mp)..'/'..tostring(maxMp)
end

function HeadMgrPanel:UpdateHeadBattle(Guid,isBattle)
    print(3)
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    local head = Heads[Guid]
    if isBattle == true then
        UIMgr:SetActive(head:Find('Auto_battle'),true)
    else 
        UIMgr:SetActive(head:Find('Auto_battle'),false)
    end
end

function HeadMgrPanel:HurtAnim(positionStart,positionEnd,damage)
    PoolMgr:Get("Hurt",function (hurt)
        hurt.transform:SetParent(transform)
        hurt.transform:GetComponent('Text').text = tostring(damage)
        if damage > 0 then
            hurt.transform:GetComponent('Text').color = Color.green
        else 
            hurt.transform:GetComponent('Text').color = Color.red
        end
        hurt.transform.position = positionStart
        hurt.transform:DOMove(positionEnd,0.5):OnComplete(function ()
            PoolMgr:Set(hurt.name,hurt)
        end)
    end)
end

function HeadMgrPanel:AddBuff(Guid,buffId,rounds)
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    local head = Heads[Guid]
    PoolMgr:Get("Buff",function (buff)
        buff.transform:SetParent(head:Find('Auto_buff'))
        local iconName = BuffConfig[buffId].icon
        ResMgr:LoadAssetSprite('buffIcon',{iconName},function (icon)
            buff:GetComponent('Image').sprite = icon
        end)
        PanelMgr:ButtonAddListener(buff.transform:GetComponent('Button'),function ()
            UIMgr:Trigger('ShowBuffInfo',buffId,rounds)
        end)
    end)
end

function HeadMgrPanel:ClearBuff(Guid)
    if Heads[Guid] == nil then
        print(Guid..'该id对应的头顶面板不存在')
        return
    end
    local head = Heads[Guid]

	local oldBuffs = Util.GetChildrens(head.transform:Find('Auto_buff'))
	if oldBuffs == nil or oldBuffs.Length == 1 then
		return;
	end
	for index = 1,oldBuffs.Length-1 do
        if oldBuffs[index].name == 'Buff' then
            PanelMgr:ButtonRemoveListener(oldBuffs[index]:GetComponent('Button'))
            PoolMgr:Set(oldBuffs[index].name,oldBuffs[index].gameObject)
		end
    end
end

function HeadMgrPanel:UpdateBuff(Guid,buffs)

    self:ClearBuff(Guid)

    if buffs == nil then
        print(Guid..'该id对应的角色没有buff')
        return
    end
    print(buffs.Length)
    for i = 0,buffs.Length - 1 do
        self:AddBuff(Guid,buffs[i].buffId,buffs[i].rounds)
    end

end