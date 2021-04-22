
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:登录背景面板控制层
*
*        description:
*            功能描述:实现登录背景面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

LoginBgCtrl = {}
local this = LoginBgCtrl

local gameObject
local transform

function LoginBgCtrl:Awake()

end

function LoginBgCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function LoginBgCtrl:Init()

end