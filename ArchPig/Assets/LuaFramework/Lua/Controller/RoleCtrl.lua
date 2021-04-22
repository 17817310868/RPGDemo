
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:角色面板逻辑层
*
*        description:
*            功能描述:实现角色面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



RoleCtrl = {}
local this = RoleCtrl

local gameObject
local transform

function RoleCtrl:Awake()

end

function RoleCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function RoleCtrl:Init()

end

function RoleCtrl:ShowInfo(S2C_info)
    PanelMgr:OpenPanel("RolePanel",UILayer.Middle,MaskEnum.Mask,function ()
        UIMgr:Trigger('ShowRoleInfo',S2C_info)
    end)
end