
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:交互面板控制层
*
*        description:
*            功能描述:实现交互面板具体功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



EachOtherRoleCtrl = {}
local this = EachOtherRoleCtrl

local gameObject
local transform

local currentTarget;

local otherRoleObserver

function EachOtherRoleCtrl:Awake()

    otherRoleObserver = Observer:New(function (roleGuid)
        currentTarget = roleGuid
        local roleData = RoleInfoMgr:GetRole(roleGuid)
        PanelMgr:OpenPanel('EachOtherRolePanel',UILayer.Middle,MaskEnum.TipsMask,function ()
            EachOtherRolePanel:ShowOtherRole(roleData)
        end)
    end)

    --监听显示其他角色信息事件(当玩家点击其他玩家时，显示交互面板)
    UIMgr:AddListener("ShowOtherRole",otherRoleObserver)
end

function EachOtherRoleCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function EachOtherRoleCtrl:Init()

end

function EachOtherRoleCtrl:CheckInfo()
    Client.Send("CheckInfo",C2S_CheckInfo.New(currentTarget))
end

function EachOtherRoleCtrl:Battle()
    Client.Send("BattleRequest",C2S_BattleRequest.New(currentTarget))
end

function EachOtherRoleCtrl:JoinRequest()
    Client.Send("JoinRequest",C2S_JoinTeamRequest.New(currentTarget))
end

function EachOtherRoleCtrl:VisiteRequest()
    Client.Send("VisiteRequest",C2S_VisiteRequest.New(currentTarget))
end