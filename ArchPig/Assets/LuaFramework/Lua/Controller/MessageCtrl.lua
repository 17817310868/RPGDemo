
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:消息面板控制层
*
*        description:
*            功能描述:实现消息面板功能具体逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

require "View/MessagePanel"

MessageCtrl = {}
local this = MessageCtrl

local gameObject
local transform

local messageObserver  --消息监听观察者

function MessageCtrl:Awake()

    messageObserver = Observer:New(function (messageEnum,...)
        MessagePanel:AddMessage(messageEnum,...)
        if gameObject == nil or (not gameObject.activeSelf) then
            PanelMgr:OpenPanel("MessagePanel",UILayer.Middle,MaskEnum.Mask,function (go)
                MessagePanel:ShowMessage()
            end)
        end
    end)

    UIMgr:AddListener("Message",messageObserver)
    --[[
    UIMgr:AddListener("Message", function (messageEnum,messageContent,paramId)
        if paramId == nil then
            MessagePanel:AddMessage(messageEnum,messageContent)
        else 
            MessagePanel:AddMessage(messageEnum,messageContent,paramId)
        end
        if gameObject == nil or (not gameObject.activeSelf) then
            PanelMgr:OpenPanel("MessagePanel",UILayer.Middle,MaskEnum.Mask,function (go)
                MessagePanel:ShowMessage()
            end)
        end
    end)
    --]]
end

function MessageCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function MessageCtrl:Init()

end

function MessageCtrl:Buy(itemType,itemId)
    ShopCtrl:Buy(itemType,itemId)
end

function MessageCtrl:Throw(Guid)
    ItemInfoCtrl:DiscardItem(Guid)
end

function MessageCtrl:Sell(Guid)
    --UIMgr:Trigger("Sell",Guid)
end

function MessageCtrl:JoinTeam(paramId,result)
    Client.Send("JoinReply",C2S_JoinTeamReply.New(paramId,result))
end

function MessageCtrl:VisiteTeam(paramId,result)
    Client.Send("VisiteReply",C2S_VisiteReply.New(paramId,result))
end

function MessageCtrl:Business(paramId,result)
    UIMgr:Trigger("Business",paramId,result)
end

function MessageCtrl:Battle(paramId,result)
    Client.Send("BattleReply",C2S_BattleReply.New(paramId,result))
end

function MessageCtrl:JoinRequest(S2C_joinRequest)
    local playerId = S2C_joinRequest.playerId
    local playerName = RoleInfoMgr:GetRole(playerId):GetName()
    UIMgr:Trigger("Message",MessageEnum.JoinTeam,'玩家'..playerName.."请求加入你的队伍",playerId)
end

function MessageCtrl:JoinReply(S2C_joinReply)
    if S2C_joinReply.result == true then
        return;
    end
    local playerName = RoleInfoMgr:GetRole(S2C_joinReply.playerId):GetName()
    UIMgr:Trigger("Message",MessageEnum.Message,'玩家'..playerName.."拒绝了你的进队请求")
end

function MessageCtrl:VisiteRequest(S2C_visiteRequest)
    local playerId = S2C_visiteRequest.playerId
    local playerName = RoleInfoMgr:GetRole(playerId):GetName()
    UIMgr:Trigger("Message",MessageEnum.VisiteTeam,'玩家'..playerName.."邀请你加入队伍",playerId)
end

function MessageCtrl:VisiteReply(S2C_visiteReply)
    if S2C_visiteReply.result == true then
        return;
    end
    local playerName = RoleInfoMgr:GetRole(S2C_visiteReply.playerId):GetName()
    UIMgr:Trigger("Message",MessageEnum.Message,'玩家'..playerName.."拒绝了你的邀请")
end

function MessageCtrl:BattleRequest(S2C_battleRequest)
    local playerId = S2C_battleRequest.playerId
    local playerName = RoleInfoMgr:GetRole(playerId):GetName()
    UIMgr:Trigger("Message",MessageEnum.Battle,'玩家'..playerName.."请求与你切磋",playerId)
end

function MessageCtrl:BattleReply(S2C_battleReply)
    if S2C_battleReply.reply == true then
        return;
    end
    local playerName = RoleInfoMgr:GetRole(S2C_battleReply.playerId):GetName()
    UIMgr:Trigger("Message",MessageEnum.Message,'玩家'..playerName.."拒绝了你的切磋请求")
end