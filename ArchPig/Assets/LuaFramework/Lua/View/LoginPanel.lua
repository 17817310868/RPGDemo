
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:登录面板视图层
*
*        description:
*            功能描述:实现登录面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

LoginPanel = {}
local this = LoginPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
LoginPanel.Ctrls = 
{
    Img_accountInput = {  Path = 'account/Auto_accountInput',  ControlType = 'Image'  },
    Img_passwordInput = {  Path = 'password/Auto_passwordInput',  ControlType = 'Image'  },
    Img_loginBtn = {  Path = 'Auto_loginBtn',  ControlType = 'Image'  },
    Img_registerBtn = {  Path = 'Auto_registerBtn',  ControlType = 'Image'  },
    Btn_loginBtn = {  Path = 'Auto_loginBtn',  ControlType = 'Button'  },
    Btn_registerBtn = {  Path = 'Auto_registerBtn',  ControlType = 'Button'  },
    Input_accountInput = {  Path = 'account/Auto_accountInput',  ControlType = 'InputField'  },
    Input_passwordInput = {  Path = 'password/Auto_passwordInput',  ControlType = 'InputField'  },
}

local gameObject
local transform

function LoginPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function LoginPanel:Init()
    
    --给按钮添加监听事件
    PanelMgr:ButtonAddListener(self.Btn_loginBtn,function ()
        AudioMgr:PlayEffect('button')
        LoginCtrl:Login(self.Input_accountInput.text,self.Input_passwordInput.text);
        self.Input_accountInput.text = nil
        self.Input_passwordInput.text = nil
	end)
    PanelMgr:ButtonAddListener(self.Btn_registerBtn,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:OpenPanel('RegisterPanel',UILayer.Middle,MaskEnum.None);
    end);

end

function LoginPanel:Show()

    --初始化面板位置
    transform:GetComponent('RectTransform').anchoredPosition = Vector2.New(0,-100);

end

function LoginPanel:Hide()

end