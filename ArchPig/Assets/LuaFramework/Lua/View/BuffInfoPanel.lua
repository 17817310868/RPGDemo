
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:Buff信息面板视图层
*
*        description:
*            功能描述:实现Buff信息面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

local BuffConfig = require 'Config/BuffConfig'

BuffInfoPanel = {}
local this = BuffInfoPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
BuffInfoPanel.Ctrls = 
{
    Img_buffImg = {  Path = 'Auto_buffImg',  ControlType = 'Image'  },
    Text_buffName = {  Path = 'Auto_buffName',  ControlType = 'Text'  },
    Text_buffInfo = {  Path = 'Auto_buffInfo',  ControlType = 'Text'  },
    Text_rounds = {  Path = 'Auto_rounds',  ControlType = 'Text'  },
}

local gameObject
local transform

function BuffInfoPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function BuffInfoPanel:Init()

end

function BuffInfoPanel:Show()

end

function BuffInfoPanel:Hide()

end

function BuffInfoPanel:ShowBuffInfo(buffId,rounds)
    self.Text_buffName.text = BuffConfig[buffId].name
    local iconName = BuffConfig[buffId].icon
    ResMgr:LoadAssetSprite('bufficon',{iconName},function (icon)
		self.Img_buffImg.sprite = icon
    end);
    self.Text_buffInfo.text = BuffConfig[buffId].info
    self.Text_rounds.text = '剩余回合：'..tostring(rounds)
end