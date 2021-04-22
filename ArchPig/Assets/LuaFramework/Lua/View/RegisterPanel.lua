
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:注册面板视图层
*
*        description:
*            功能描述:实现注册面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

RegisterPanel = {}
local this = RegisterPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
RegisterPanel.Ctrls = 
{
    Img_accountInput = {  Path = 'account/Auto_accountInput',  ControlType = 'Image'  },
    Img_passwordInput = {  Path = 'password/Auto_passwordInput',  ControlType = 'Image'  },
    Img_rePasswordInput = {  Path = 'rePassword/Auto_rePasswordInput',  ControlType = 'Image'  },
    Img_backBtn = {  Path = 'Auto_backBtn',  ControlType = 'Image'  },
    Img_registerBtn = {  Path = 'Auto_registerBtn',  ControlType = 'Image'  },
    Btn_backBtn = {  Path = 'Auto_backBtn',  ControlType = 'Button'  },
    Btn_registerBtn = {  Path = 'Auto_registerBtn',  ControlType = 'Button'  },
    Input_accountInput = {  Path = 'account/Auto_accountInput',  ControlType = 'InputField'  },
    Input_passwordInput = {  Path = 'password/Auto_passwordInput',  ControlType = 'InputField'  },
    Input_rePasswordInput = {  Path = 'rePassword/Auto_rePasswordInput',  ControlType = 'InputField'  },
}

local gameObject
local transform

function RegisterPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function RegisterPanel:Init()

    --给对应控件添加监听事件
    PanelMgr:ButtonAddListener(self.Btn_registerBtn,function ()
        AudioMgr:PlayEffect('button')
        RegisterCtrl:Register(self.Input_accountInput.text,self.Input_passwordInput.text,self.Input_rePasswordInput.text)
        self.Input_accountInput.text = nil
        self.Input_passwordInput.text = nil
        self.Input_rePasswordInput.text = nil
	end)
	
    PanelMgr:ButtonAddListener(self.Btn_backBtn,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()
    end);
end

function RegisterPanel:Show()

end

function RegisterPanel:Hide()

end