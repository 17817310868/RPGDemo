
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:头顶管理逻辑层
*
*        description:
*            功能描述:实现头顶面板管理逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



HeadMgrCtrl = {}
local this = HeadMgrCtrl

local gameObject
local transform

function HeadMgrCtrl:Awake()

    

end

function HeadMgrCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function HeadMgrCtrl:Init()

end

function HeadMgrCtrl:UpdateLeader(S2C_updateLeader)
    UIMgr:Trigger("UpdateHeadLeader",S2C_updateLeader.Guid, S2C_updateLeader.result)
end

function HeadMgrCtrl:OtherRoleBattle(S2C_UpdateBattles)
    updateBattles = S2C_UpdateBattles.updateBattles
    for index = 0,updateBattles.Count do
        UIMgr:Trigger('UpdateHeadBattle',updateBattles[index].Guid,updateBattles[index].result)
    end
end
