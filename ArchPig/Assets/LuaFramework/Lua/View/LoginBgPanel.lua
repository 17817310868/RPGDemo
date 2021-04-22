
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:登录背景面板视图层
*
*        description:
*            功能描述:实现登录背景面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


LoginBgPanel = {}
local this = LoginBgPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
LoginBgPanel.Ctrls = 
{
    Img_Bgm = {  Path = 'Auto_Bgm',  ControlType = 'Image'  },
    Img_effect = {  Path = 'Auto_effect',  ControlType = 'Image'  },
    Btn_Bgm = {  Path = 'Auto_Bgm',  ControlType = 'Button'  },
    Btn_effect = {  Path = 'Auto_effect',  ControlType = 'Button'  },
}

local gameObject
local transform

local isCloseBgm
local isCloseEffect

function LoginBgPanel:Awake(go)

    UnityEngine.Object.Destroy(GameObject.Find("ResLoadPanel"))

    gameObject = go
    transform = go.transform
    self:Init()
end

function LoginBgPanel:Init()
    isCloseBgm = false
    isCloseEffect = false
    PanelMgr:ButtonAddListener(self.Btn_Bgm,function ()
        AudioMgr:PlayEffect('button')
        if isCloseBgm == true then
            self.Img_Bgm.color = Color.white
            AudioMgr:SetBgmVolume(0.5)
            isCloseBgm = false
        else
            self.Img_Bgm.color = Color:New(150/255,150/255,150/255,1)
            AudioMgr:SetBgmVolume(0)
            isCloseBgm = true
        end
    end)
    PanelMgr:ButtonAddListener(self.Btn_effect,function ()
        AudioMgr:PlayEffect('button')
        if isCloseEffect == true then
            self.Img_effect.color = Color.white
            AudioMgr:SetEffectVolume(0.5)
            isCloseEffect = false
        else
            self.Img_effect.color = Color:New(150/255,150/255,150/255,1)
            AudioMgr:SetEffectVolume(0)
            isCloseEffect = true
        end
    end)
end

function LoginBgPanel:Show()
	--设置全屏显示此面板
    PanelMgr:FullScreen(transform:GetComponent('RectTransform'))
    AudioMgr:PlayBgm("LoginBgm")
end

function LoginBgPanel:Hide()

end