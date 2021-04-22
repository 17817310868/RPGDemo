
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:设置面板视图层
*
*        description:
*            功能描述:实现设置面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


SettingPanel = {}
local this = SettingPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
SettingPanel.Ctrls = 
{
    Img_close = {  Path = 'Auto_close',  ControlType = 'Image'  },
    Slider_bgmSlider = {  Path = 'Auto_bgmSlider',  ControlType = 'Slider'  },
    Slider_effectSlider = {  Path = 'Auto_effectSlider',  ControlType = 'Slider'  },
    Btn_close = {  Path = 'Auto_close',  ControlType = 'Button'  },
}

local gameObject
local transform

function SettingPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function SettingPanel:Init()
    PanelMgr:ButtonAddListener(self.Btn_close,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()
    end)
    PanelMgr:SliderAddListener(self.Slider_bgmSlider,function (value)
        AudioMgr:SetBgmVolume(value)
    end)

    PanelMgr:SliderAddListener(self.Slider_effectSlider,function (value)
        AudioMgr:SetEffectVolume(value)
    end)
end

function SettingPanel:Show()

    self.Slider_bgmSlider.value = AudioMgr:GetBgmVolume()
    self.Slider_effectSlider.value = AudioMgr:GetEffectVolume()

end

function SettingPanel:Hide()
    --PanelMgr:SliderRemoveListener(self.Slider_bgmSlider)
    --PanelMgr:SliderRemoveListener(self.Slider_effectSlider)
end