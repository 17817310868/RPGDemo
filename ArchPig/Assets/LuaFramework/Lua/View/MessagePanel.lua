
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:消息面板视图层
*
*        description:
*            功能描述:实现消息面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


MessagePanel = {}
local this = MessagePanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
MessagePanel.Ctrls = 
{
    Img_know = {  Path = 'Auto_know',  ControlType = 'Image'  },
    Img_confirm = {  Path = 'Auto_confirm',  ControlType = 'Image'  },
    Img_cancel = {  Path = 'Auto_cancel',  ControlType = 'Image'  },
    Img_agree = {  Path = 'Auto_agree',  ControlType = 'Image'  },
    Img_refuse = {  Path = 'Auto_refuse',  ControlType = 'Image'  },
    Text_message = {  Path = 'Auto_message',  ControlType = 'Text'  },
    Btn_know = {  Path = 'Auto_know',  ControlType = 'Button'  },
    Btn_confirm = {  Path = 'Auto_confirm',  ControlType = 'Button'  },
    Btn_cancel = {  Path = 'Auto_cancel',  ControlType = 'Button'  },
    Btn_agree = {  Path = 'Auto_agree',  ControlType = 'Button'  },
    Btn_refuse = {  Path = 'Auto_refuse',  ControlType = 'Button'  },
}

local gameObject
local transform

local messages = {};

function MessagePanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function MessagePanel:Init()
    --默认隐藏所有按钮
    UIMgr:SetActive(self.Btn_know.transform,false)
    UIMgr:SetActive(self.Btn_confirm.transform,false)
    UIMgr:SetActive(self.Btn_cancel.transform,false)
    UIMgr:SetActive(self.Btn_agree.transform,false)
    UIMgr:SetActive(self.Btn_refuse.transform,false)
end

function MessagePanel:Show()
    
end

function MessagePanel:Hide()
    
end

function MessagePanel:AddMessage(messageType,...)
    --添加消息，把所有消息缓存起来
    table.insert(messages,#messages+1,{type = messageType,params = {...}})  --params首个参数为提示的消息内容，往后为具体参数
end

--处理不同消息类型的消息
local ShowMessageSwitch = {
    [MessageEnum.Message] = function ()  --普通消息，只需阅读
        UIMgr:SetActive(this.Btn_know.transform,true)  
        PanelMgr:ButtonAddListener(this.Btn_know,function ()
            AudioMgr:PlayEffect('button')
            this:ClosePanel()
        end)
    end,
    [MessageEnum.Buy] = function (params)  --购买物品消息，需要玩家确认
        UIMgr:SetActive(this.Btn_confirm.transform,true)
        UIMgr:SetActive(this.Btn_cancel.transform,true)
        PanelMgr:ButtonAddListener(this.Btn_confirm,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:Buy(params[2],params[3])  
            this:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_cancel,function ()
            AudioMgr:PlayEffect('button')
            this:ClosePanel()
        end)
    end,
    [MessageEnum.Throw] = function (params)  --丢弃物品，需要玩家确认
        UIMgr:SetActive(this.Btn_confirm.transform,true)
        UIMgr:SetActive(this.Btn_cancel.transform,true)
        PanelMgr:ButtonAddListener(this.Btn_confirm,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:Throw(params[2])
            this:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_cancel,function ()
            AudioMgr:PlayEffect('button')
            this:ClosePanel()
        end)
    end,
    [MessageEnum.Sell] = function (params)  --出售物品，需要玩家确认
        UIMgr:SetActive(this.Btn_confirm.transform,true)
        UIMgr:SetActive(this.Btn_cancel.transform,true)
        PanelMgr:ButtonAddListener(this.Btn_confirm,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:Sell(params[2])
            this:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_cancel,function ()
            AudioMgr:PlayEffect('button')
            this:ClosePanel()
        end)
    end,
    [MessageEnum.JoinTeam] = function (params)  --入队请求，需玩家选择
        UIMgr:SetActive(this.Btn_agree.transform,true)
        UIMgr:SetActive(this.Btn_refuse.transform,true)
        PanelMgr:ButtonAddListener(this.Btn_agree,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:JoinTeam(params[2],true)
            this:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_refuse,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:JoinTeam(params[2],false)
            this:ClosePanel();
        end)
    end,
    [MessageEnum.VisiteTeam] = function (params)  --邀请请求，需玩家选择
        UIMgr:SetActive(this.Btn_agree.transform,true)
        UIMgr:SetActive(this.Btn_refuse.transform,true)
        PanelMgr:ButtonAddListener(this.Btn_agree,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:VisiteTeam(params[2],true)
            this:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_refuse,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:VisiteTeam(params[2],false)
            this:ClosePanel();
        end)
    end,
    [MessageEnum.Business] = function (params)
        MessageCtrl:Business(paramId,result)
    end,
    [MessageEnum.Battle] = function (params)  --切磋请求，需要玩家选择
        UIMgr:SetActive(this.Btn_agree.transform,true)
        UIMgr:SetActive(this.Btn_refuse.transform,true)
        PanelMgr:ButtonAddListener(this.Btn_agree,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:Battle(params[2],true)
            this:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_refuse,function ()
            AudioMgr:PlayEffect('button')
            MessageCtrl:Battle(params[2],false)
            this:ClosePanel();
        end)
    end
}

function MessagePanel:ShowMessage()
    local message = messages[1]
    self.Text_message.text = message.params[1]  --显示消息提示内容
    ShowMessageSwitch[message.type](message.params)  --给不同消息按钮添加监听
end

--关闭面板
function MessagePanel:ClosePanel()
    local message = messages[1]
    
    --清除所有按钮监听，隐藏所有按钮
    PanelMgr:ButtonRemoveListener(self.Btn_know)
    PanelMgr:ButtonRemoveListener(self.Btn_confirm)
    PanelMgr:ButtonRemoveListener(self.Btn_cancel)
    PanelMgr:ButtonRemoveListener(self.Btn_agree)
    PanelMgr:ButtonRemoveListener(self.Btn_refuse)
    UIMgr:SetActive(self.Btn_know.transform,false)
    UIMgr:SetActive(self.Btn_confirm.transform,false)
    UIMgr:SetActive(self.Btn_cancel.transform,false)
    UIMgr:SetActive(self.Btn_agree.transform,false)
    UIMgr:SetActive(self.Btn_refuse.transform,false)

    --将消息从表中移除
    table.remove(messages,1)
    --判断表中是否还有未处理消息，有则继续处理，没有则关闭面板
    if #messages > 0 then
        MessagePanel:ShowMessage()
    else 
        PanelMgr:ClosePanel();
    end
end