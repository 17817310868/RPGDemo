
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:加载面板视图层
*
*        description:
*            功能描述:实现加载面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


LoadingPanel = {}
local this = LoadingPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
LoadingPanel.Ctrls = 
{
    Text_loadingProgress = {  Path = 'Auto_loadingProgress',  ControlType = 'Text'  },
    Slider_loadingslider = {  Path = 'Auto_loadingslider',  ControlType = 'Slider'  },
}

local gameObject
local transform

local loadingObserver

function LoadingPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function LoadingPanel:Init()
    loadingObserver = Observer:New(function (progress)
        self.Slider_loadingslider.value = progress / 0.9
        self.Text_loadingProgress.text = tostring(math.floor(progress / 0.9 * 100)).."%"
    end)
end

function LoadingPanel:Show()
    --设置全屏显示此面板
	PanelMgr:FullScreen(transform:GetComponent('RectTransform'))
    UIMgr:AddListener('UpdateLoading',loadingObserver)
end

function LoadingPanel:Hide()
    UIMgr:RemoveListener('UpdateLoading',loadingObserver)
end