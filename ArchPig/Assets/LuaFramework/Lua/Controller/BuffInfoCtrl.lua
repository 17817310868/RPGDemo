
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:buff信息面板逻辑层
*
*        description:
*            功能描述:实现buff信息面板具体逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



BuffInfoCtrl = {}
local this = BuffInfoCtrl

local gameObject
local transform

local buffInfoObserver

function BuffInfoCtrl:Awake()

    buffInfoObserver = Observer:New(function (buffId,rounds)
        BuffInfoPanel:ShowBuffInfo(buffId,rounds)
    end)
    UIMgr:AddListener('ShowBuffInfo',buffInfoObserver)

end

function BuffInfoCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function BuffInfoCtrl:Init()

end