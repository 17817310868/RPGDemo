
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:交互面板视图层
*
*        description:
*            功能描述:实现交互面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

local ProfessionConfig = require "Config/ProfessionConfig"
local School = require "Config/SchoolConfig"


EachOtherRolePanel = {}
local this = EachOtherRolePanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
EachOtherRolePanel.Ctrls = 
{
    Img_roleIcon = {  Path = 'roleBg/Auto_roleIcon',  ControlType = 'Image'  },
    Img_checkInfo = {  Path = 'Auto_checkInfo',  ControlType = 'Image'  },
    Img_businessRequest = {  Path = 'Auto_businessRequest',  ControlType = 'Image'  },
    Img_battleRequest = {  Path = 'Auto_battleRequest',  ControlType = 'Image'  },
    Img_visiteRequest = {  Path = 'Auto_visiteRequest',  ControlType = 'Image'  },
    Img_joinRequest = {  Path = 'Auto_joinRequest',  ControlType = 'Image'  },
    Text_level = {  Path = 'Auto_level',  ControlType = 'Text'  },
    Text_roleName = {  Path = 'Auto_roleName',  ControlType = 'Text'  },
    Text_roleProfession = {  Path = 'Auto_roleProfession',  ControlType = 'Text'  },
    Text_roleSchool = {  Path = 'Auto_roleSchool',  ControlType = 'Text'  },
    Btn_checkInfo = {  Path = 'Auto_checkInfo',  ControlType = 'Button'  },
    Btn_businessRequest = {  Path = 'Auto_businessRequest',  ControlType = 'Button'  },
    Btn_battleRequest = {  Path = 'Auto_battleRequest',  ControlType = 'Button'  },
    Btn_visiteRequest = {  Path = 'Auto_visiteRequest',  ControlType = 'Button'  },
    Btn_joinRequest = {  Path = 'Auto_joinRequest',  ControlType = 'Button'  },
}

local ProfessionConfig = require "Config/ProfessionConfig"
local SchoolConfig = require "Config/SchoolConfig"

local gameObject
local transform

--local currentRole

function EachOtherRolePanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function EachOtherRolePanel:Init()
    PanelMgr:ButtonAddListener(self.Btn_checkInfo,function ()
        AudioMgr:PlayEffect('button')
        EachOtherRoleCtrl:CheckInfo()
        PanelMgr:ClosePanel()
    end)
    PanelMgr:ButtonAddListener(self.Btn_businessRequest,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()
    end)
    PanelMgr:ButtonAddListener(self.Btn_battleRequest,function ()
        AudioMgr:PlayEffect('button')
        EachOtherRoleCtrl:Battle()
        PanelMgr:ClosePanel()
    end)
    PanelMgr:ButtonAddListener(self.Btn_visiteRequest,function ()
        AudioMgr:PlayEffect('button')
        EachOtherRoleCtrl:VisiteRequest()
        PanelMgr:ClosePanel()
    end)
    PanelMgr:ButtonAddListener(self.Btn_joinRequest,function ()
        AudioMgr:PlayEffect('button')
        EachOtherRoleCtrl:JoinRequest()
        PanelMgr:ClosePanel()
    end)
end

function EachOtherRolePanel:Show()

end

function EachOtherRolePanel:Hide()

end

function EachOtherRolePanel:ShowOtherRole(roleData)

    --更新指定玩家id
    --currentRole = roleGuid
    --获取其他玩家角色信息
    --local roleData = RoleInfoMgr:GetRole(currentRole)
    --更新交互面板上该玩家的相关信息
    ResMgr:LoadAssetSprite('roleicon',{ProfessionConfig[roleData:GetProfession()].headIcon},function (icon)
		self.Img_roleIcon.sprite = icon	
    end)
    self.Text_roleName.text = '玩家名：'..roleData:GetName()
    self.Text_roleProfession.text = '职业：'..ProfessionConfig[roleData:GetProfession()].name
    self.Text_roleSchool.text = '门派：'..SchoolConfig[roleData:GetSchool()].name

end