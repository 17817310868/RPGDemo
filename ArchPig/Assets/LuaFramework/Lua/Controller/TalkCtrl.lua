
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:对话面板逻辑层
*
*        description:
*            功能描述:实现对话面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



TalkCtrl = {}
local this = TalkCtrl

local gameObject
local transform

function TalkCtrl:Awake()

end

function TalkCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function TalkCtrl:Init()

end

function TalkCtrl:ShowTask(npcId)

    if TaskManager:GetNpcAcceptableTasks(npcId) == nil and TaskManager:GetNpcConductTasks(npcId) == nil then
        return
    end

    PanelMgr:OpenPanel('TalkPanel',UILayer.Middle,MaskEnum.TipsMask,function ()
        UIMgr:Trigger('ShowTasks',npcId)
    end)

end

function TalkCtrl:AcceptTask(taskId)
    Client.Send('AcceptTask',C2S_AcceptTask.New(taskId))
end

function TalkCtrl:SubmitTask(taskId)
    Client.Send('SubmitTask',C2S_CompleteTask.New(taskId))
end