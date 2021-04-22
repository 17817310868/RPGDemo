
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:主UI面板控制层
*
*        description:
*            功能描述:实现主UI面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

local TaskConfig = require 'Config/TaskConfig'
local NPCConfig = require 'Config/NPCConfig'

MainUICtrl = {}
local this = MainUICtrl

local gameObject
local transform

function MainUICtrl:Awake()

end

function MainUICtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function MainUICtrl:Init()

end

--刷新队伍信息
function MainUICtrl:UpdateTeamInfo()

    Client.MySend("GetTeamInfo")

end

--刷新队伍成员
function MainUICtrl:UpdateTeamMember(S2C_teamInfo)

    local isLeader = S2C_teamInfo.isLeader
    local selfGuid = RoleInfoMgr:GetMainRoleGuid()
    local playersData = {}
    for index = 0,S2C_teamInfo.playersGuid.Count-1 do
        table.insert(playersData,RoleInfoMgr:GetRole(S2C_teamInfo.playersGuid[index]))
    end

    UIMgr:Trigger('UpdateTeamInfo',isLeader,selfGuid,playersData)

end

--切换队长
function MainUICtrl:ChangeLeader(Guid)

    Client.Send("ChangeLeader",Guid)

end

--退出队伍
function MainUICtrl:ExitTeam(Guid)

    Client.Send("ExitTeam",Guid)

end

function MainUICtrl:UpdateTaskInfo(S2C_allTaskInfo)

    --NPCMgr:UpdateTaskIcon(S2C_allTaskInfo.conductTasks,S2C_allTaskInfo.acceptableTasks)
    
    local acceptableTasks = {}
    for index = 0,S2C_allTaskInfo.acceptableTasks.Count-1 do
        table.insert(acceptableTasks,S2C_allTaskInfo.acceptableTasks[index])
    end

    local conductTasks = UIMgr:DicToLuaTable(S2C_allTaskInfo.conductTasks)

    TaskManager:UpdateTaskInfo(acceptableTasks,conductTasks)

    self:UpdateAllNpcTasksIcon()

    UIMgr:Trigger('UpdateTaskBox')

end

function MainUICtrl:UpdateConductTask(S2C_conductTasks)
    local conductTasks = UIMgr:DicToLuaTable(S2C_conductTasks.conductTasks)
    TaskManager:UpdateConductTasks(conductTasks)
    self:UpdateAllNpcTasksIcon()

    UIMgr:Trigger('UpdateTaskBox')
end

function MainUICtrl:UpdateAcceptableTasks(S2C_acceptableTasks)

    local acceptableTasks = {}
    for index = 0,S2C_acceptableTasks.acceptableTasks.Count-1 do
        table.insert(acceptableTasks,S2C_acceptableTasks.acceptableTasks[index])
    end
    TaskManager:UpdateAcceptableTasks(acceptableTasks)
    self:UpdateAllNpcTasksIcon()

end

function MainUICtrl:UpdateAllNpcTasksIcon()
    local npcNum = 2
    local npcsId = {}
    for index = 1,npcNum do
        if NPCConfig[index].type == NPCEnum.Task then
            table.insert(npcsId,index)
        end
    end
    for index = 1,#npcsId do
        self:UpdateNpcTaskIcon(npcsId[index])
    end
end

function MainUICtrl:DeleteAcceptableTask(S2C_acceptTask)

    --print(S2C_acceptTask.taskId)
    TaskManager:AcceptTask(S2C_acceptTask.taskId,S2C_acceptTask.progress)

    local npcId = TaskConfig[S2C_acceptTask.taskId].receive
    self:UpdateNpcTaskIcon(npcId)

    UIMgr:Trigger('UpdateTaskBox')

end

function MainUICtrl:DeleteConductTask(S2C_completeTask)

    TaskManager:CompleteTask(S2C_completeTask.taskId)

    local npcId = TaskConfig[S2C_completeTask.taskId].submit
    self:UpdateNpcTaskIcon(npcId)

    UIMgr:Trigger('UpdateTaskBox')

end

function MainUICtrl:UpdateNpcTaskIcon(npcId)

    local IsConductTask = false
    local IsAcceptTask = false
    local IsCompleteTask = false

    local acceptTasksId = TaskManager:GetNpcAcceptableTasks(npcId)

    if acceptTasksId ~= nil then
        IsAcceptTask = true
    end
    
    local conductTasksId = TaskManager:GetNpcConductTasks(npcId)

    if conductTasksId ~= nil then
        for index = 1,#conductTasksId do 
            if TaskManager:IsCompleteTask(conductTasksId[index]) == true then
                IsCompleteTask = true
            else
                IsConductTask = true
            end
        end
    end

    if IsCompleteTask == true then
        NPCMgr:AddTaskIcon(npcId,'yiWanCheng')
        return
    end
    if IsAcceptTask == true then
        NPCMgr:AddTaskIcon(npcId,'keJie')
        return
    end
    if IsConductTask == true then
        NPCMgr:AddTaskIcon(npcId,'weiWanCheng')
        return
    end

    NPCMgr:HideTaskIcon(npcId)
    
end

function MainUICtrl:ReceiveNewMail()
    UIMgr:Trigger('ReceiveNewMail')
end

function MainUICtrl:InitMainRole(S2C_mainRole)
    local otherData = OtherData:New(S2C_mainRole.hp,S2C_mainRole.maxHp,S2C_mainRole.mp,
    S2C_mainRole.maxMp,S2C_mainRole.physicalAttack,S2C_mainRole.physicalDefense,S2C_mainRole.magicAttack,S2C_mainRole.magicDefense,
    S2C_mainRole.speed,S2C_mainRole.experience,S2C_mainRole.silver,S2C_mainRole.gold,S2C_mainRole.yuanBao)
    local mainRole = MainRoleInfo:New(S2C_mainRole.Guid,S2C_mainRole.roleType,S2C_mainRole.name,S2C_mainRole.level,
    S2C_mainRole.professionId,S2C_mainRole.schoolId,S2C_mainRole.power,otherData)
    RoleInfoMgr:AddMainRole(mainRole)
    PanelMgr:OpenPanel('MainUIPanel',UILayer.Middle,MaskEnum.None,function ()
        UIMgr:Trigger('UpdateRoleInfo')
        Client.MySend('InitSkill');
		Client.MySend('InitTask');
		Client.MySend('InitMails')
    end)
end

function MainUICtrl:InitOtherRole(S2C_role)
    local otherRole = OtherRoleInfo:New(S2C_role.Guid,S2C_role.roleType,S2C_role.name,S2C_role.level,S2C_role.professionId,
    S2C_role.schoolId,S2C_role.power)
    RoleInfoMgr:AddRole(otherRole)
end

function MainUICtrl:UpdateMainRole(S2C_mainRole)
    local otherData = OtherData:New(S2C_mainRole.hp,S2C_mainRole.maxHp,S2C_mainRole.mp,
    S2C_mainRole.maxMp,S2C_mainRole.physicalAttack,S2C_mainRole.physicalDefense,S2C_mainRole.magicAttack,S2C_mainRole.magicDefense,
    S2C_mainRole.speed,S2C_mainRole.experience,S2C_mainRole.silver,S2C_mainRole.gold,S2C_mainRole.yuanBao)
    local mainRole = MainRoleInfo:New(S2C_mainRole.Guid,S2C_mainRole.roleType,S2C_mainRole.name,S2C_mainRole.level,
    S2C_mainRole.professionId,S2C_mainRole.schoolId,S2C_mainRole.power,otherData)
    RoleInfoMgr:UpdateMainRole(mainRole)
    UIMgr:Trigger('UpdateRoleInfo')
end