
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:任务逻辑层
*
*        description:
*            功能描述:处理任务方面的逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

TaskCtrl = {}
local this = TaskCtrl

function TaskCtrl:Awake()

end

function TaskCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function TaskCtrl:Init()

end

function TaskCtrl:UpdateTaskInfo(S2C_allTaskInfo)

    local conductTasks = UIMgr:DicToLuaTable(S2C_allTasksInfo.conductTasks)
    local acceptableTasks = {}
    local acceptableTasksList = S2C_allTasksInfo.acceptableTasks
    for index = 0,acceptableTasksList.Count-1 do
        table.insert(acceptableTasks,acceptableTasksList[index])
    end

    TaskManager:UpdateTaskInfo(acceptableTasks,conductTasks)

end

function TaskCtrl:UpdateAcceptableTasks(S2C_acceptableTasks)

    local acceptableTasks = {}
    local acceptableTasksList = S2C_acceptableTasks.acceptableTasks
    for index = 0,acceptableTasksList.Count-1 do
        table.insert(acceptableTasks,acceptableTasksList[index])
    end
    TaskManager:UpdateAcceptableTasks(acceptableTasks)

end

function TaskCtrl:UpdateConductTasks(S2C_conductTasks)

    local conductTasks = UIMgr:DicToLuaTable(S2C_conductTasks.conductTasks)
    TaskManager:UpdateConductTasks(conductTasks)

end

function TaskCtrl:UpdateTaskProgress(S2C_taskProgress)

    TaskManager:UpdateTaskProgress(S2C_taskProgress.taskId,S2C_taskProgress.progress)

end

function TaskCtrl:AcceptTask(S2C_acceptTask)

    TaskManager:AcceptTask(S2C_acceptTask.taskId)

end

function TaskCtrl:CompleteTask(S2C_completeTask)

    TaskManager:CompleteTask(S2C_completeTask.taskId)
    
end
