--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题：任务管理器
*
*        description:
*            功能描述:管理玩家的所有任务
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

local TaskConfig = require "Config/TaskConfig"


TaskManager = {}
local AcceptableTasks = {}  --可领取的任务
local ConductTasks = {}  --正在进行的任务
local this = TaskManager

--根据服务端传过来的任务信息更新任务信息
function TaskManager:UpdateTaskInfo(acceptableTasks,conductTasks)

    AcceptableTasks = acceptableTasks
    ConductTasks = conductTasks

end

--刷新可领取的任务
function TaskManager:UpdateAcceptableTasks(acceptableTasks)

    AcceptableTasks = acceptableTasks

end

--刷新正在进行的任务
function TaskManager:UpdateConductTasks(conductTasks)

    ConductTasks = conductTasks

end

--获取玩家可以领取的所有任务
function TaskManager:GetAcceptableTasks()
    return AcceptableTasks;
end

--获取玩家正在进行的所有任务
function TaskManager:GetConductTasks()
    return ConductTasks
end

--刷新任务进度
function TaskManager:UpdateTaskProgress(taskId,progress)
    ConductTasks[taskId] = progress
end

--领取任务
function TaskManager:AcceptTask(taskId,progress)
    for index = 1, #AcceptableTasks do
        if AcceptableTasks[index] == taskId then
            table.remove(AcceptableTasks,index)
        end
    end
    ConductTasks[taskId] = progress
end

--完成任务
function TaskManager:CompleteTask(taskId)

    ConductTasks[taskId] = nil
    for index = 1, #AcceptableTasks do
        if AcceptableTasks[index] == taskId then
            table.remove(AcceptableTasks,index)
            return
        end
    end

end

--判断该任务是否可接
function TaskManager:IsAcceptableTask(taskId)

    for index = 1,#AcceptableTasks do
        if AcceptableTasks[index] == taskId then
            return true
        end
    end

    return false

end

--判断该任务是否已接
function TaskManager:IsAcceptTask(taskId)

    for key,value in pairs(ConductTasks) do
        if key == taskId then
            return true
        end
    end
    return false

end

--判断该任务是否可提交
function TaskManager:IsCompleteTask(taskId)

    if ConductTasks[taskId] == nil then
        return
    end

    if ConductTasks[taskId] >= TaskConfig[taskId].count then
        return true
    end

    return false

end

--获取该NPC上可接的所有任务
function TaskManager:GetNpcAcceptableTasks(npcId)

    if #AcceptableTasks == 0 then
        return
    end
    local acceptableTasks = {}
    for index = 1,#AcceptableTasks do
        if TaskConfig[AcceptableTasks[index]].receive == npcId then
            table.insert(acceptableTasks,AcceptableTasks[index])
        end
    end
    if #acceptableTasks == 0 then
        return nil
    end

    return acceptableTasks

end

--获取该NPC上玩家正在进行的所有任务
function TaskManager:GetNpcConductTasks(npcId)

    local conductTasks = {}
    for key,value in pairs(ConductTasks) do
        if TaskConfig[key].receive == npcId then
            table.insert(conductTasks,key)
        end
    end
    if #conductTasks == 0 then
        return nil
    end
    
    return conductTasks

end