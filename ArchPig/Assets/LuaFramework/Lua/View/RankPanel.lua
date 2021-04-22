
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:排行榜面板视图层
*
*        description:
*            功能描述:实现排行榜面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


RankPanel = {}
local this = RankPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
RankPanel.Ctrls = 
{
    Img_roleTog = {  Path = 'options/Auto_roleTog',  ControlType = 'Image'  },
    Img_roleLevelTog = {  Path = 'options/Auto_roleLevelTog',  ControlType = 'Image'  },
    Img_rolePowerTog = {  Path = 'options/Auto_rolePowerTog',  ControlType = 'Image'  },
    Img_roleLevelTitle = {  Path = 'Auto_roleLevelTitle',  ControlType = 'Image'  },
    Img_rolePowerTitle = {  Path = 'Auto_rolePowerTitle',  ControlType = 'Image'  },
    Img_view = {  Path = 'Auto_view',  ControlType = 'Image'  },
    Img_content = {  Path = 'Auto_view/Viewport/Auto_content',  ControlType = 'Image'  },
    Img_bar = {  Path = 'Auto_view/Auto_bar',  ControlType = 'Image'  },
    Img_selfRoleLevel = {  Path = 'Auto_selfRoleLevel',  ControlType = 'Image'  },
    Img_selfRolePower = {  Path = 'Auto_selfRolePower',  ControlType = 'Image'  },
    Img_close = {  Path = 'Auto_close',  ControlType = 'Image'  },
    Text_L_selfRank = {  Path = 'Auto_selfRoleLevel/Auto_L_selfRank',  ControlType = 'Text'  },
    Text_L_selfName = {  Path = 'Auto_selfRoleLevel/Auto_L_selfName',  ControlType = 'Text'  },
    Text_L_selfLevel = {  Path = 'Auto_selfRoleLevel/Auto_L_selfLevel',  ControlType = 'Text'  },
    Text_P_selfRank = {  Path = 'Auto_selfRolePower/Auto_P_selfRank',  ControlType = 'Text'  },
    Text_P_selfName = {  Path = 'Auto_selfRolePower/Auto_P_selfName',  ControlType = 'Text'  },
    Text_P_selfPower = {  Path = 'Auto_selfRolePower/Auto_P_selfPower',  ControlType = 'Text'  },
    Tog_roleTog = {  Path = 'options/Auto_roleTog',  ControlType = 'Toggle'  },
    Tog_roleLevelTog = {  Path = 'options/Auto_roleLevelTog',  ControlType = 'Toggle'  },
    Tog_rolePowerTog = {  Path = 'options/Auto_rolePowerTog',  ControlType = 'Toggle'  },
    Btn_close = {  Path = 'Auto_close',  ControlType = 'Button'  },
    Scrollbar_bar = {  Path = 'Auto_view/Auto_bar',  ControlType = 'Scrollbar'  },
    Rect_view = {  Path = 'Auto_view',  ControlType = 'ScrollRect'  },
}

require 'Tools/OptimizeSV'

local gameObject
local transform

local currentTypeRank
local currentRanks  --按索引存储目前显示的所有信息框
local currentRanksInfo  --当前的所有玩家排名信息
local contentRect  --显示排行榜内容的transformRect组件
local rankIndex  --当前需要显示的起点索引

local rankInfoObserver  --排行信息监听观察者

local optimizeSV

function RankPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function RankPanel:Init()

    rankInfoObserver = Observer:New(function (rankInfos)
        self:ShowRanksInfo(rankInfos)
    end)

    optimizeSV = OptimizeSV:New()
    currentRanks = {}
    contentRect = self.Img_content.transform:GetComponent('RectTransform')

    PanelMgr:ButtonAddListener(self.Btn_close,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()
    end)

    PanelMgr:ToggleAddListener(self.Tog_roleTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_roleTog,'btnHighlighted','btnNormal')
        if isChoose == true then
            self.Tog_roleLevelTog.isOn = false
            self.Tog_roleLevelTog.isOn = true
            UIMgr:SetActive(self.Img_roleLevelTog.transform,true)
            UIMgr:SetActive(self.Img_rolePowerTog.transform,true)
        else 
            UIMgr:SetActive(self.Img_roleLevelTog.transform,false)
            UIMgr:SetActive(self.Img_rolePowerTog.transform,false)
        end
    end)

    PanelMgr:ToggleAddListener(self.Tog_roleLevelTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_roleLevelTog,'chooseState','normalState')
        if currentTypeRank == RankEnum.RoleLevel then
            return
        end
        currentTypeRank = RankEnum.RoleLevel
        
        RankCtrl:GetRanksInfo(currentTypeRank)
        UIMgr:SetActive(self.Img_roleLevelTitle.transform,true)
        UIMgr:SetActive(self.Img_rolePowerTitle.transform,false)
        UIMgr:SetActive(self.Img_selfRoleLevel.transform,true)
        UIMgr:SetActive(self.Img_selfRolePower.transform,false)

    end) 

    PanelMgr:ToggleAddListener(self.Tog_rolePowerTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_rolePowerTog,'chooseState','normalState')
        if currentTypeRank == RankEnum.RolePower then
            return
        end
        currentTypeRank = RankEnum.RolePower
        RankCtrl:GetRanksInfo(currentTypeRank)
        UIMgr:SetActive(self.Img_roleLevelTitle.transform,false)
        UIMgr:SetActive(self.Img_rolePowerTitle.transform,true)
        UIMgr:SetActive(self.Img_selfRoleLevel.transform,false)
        UIMgr:SetActive(self.Img_selfRolePower.transform,true)
    end)

end

function RankPanel:Show()
    self.Tog_roleTog.isOn = false
    self.Tog_roleTog.isOn = true
    UIMgr:AddListener("ShowRanksInfo",rankInfoObserver)
    
end

function RankPanel:Hide()

    UIMgr:ChangeTogImg(false,self.Img_rolePowerTog,'chooseState','normalState')
    UIMgr:ChangeTogImg(false,self.Img_roleLevelTog,'chooseState','normalState')
    PanelMgr:ScrollRectRemoveListener(self.Rect_view)
    
    optimizeSV:Clear()
    UIMgr:RemoveListener('ShowRanksInfo',rankInfoObserver)
    currentTypeRank = nil

end

local ShowRank = {
    [RankEnum.RoleLevel] = function (rankInfos)
        optimizeSV:VerticalLayout(this.Rect_view.transform:GetComponent('RectTransform'),contentRect,600,40,rankInfos.Count,0,0,0,0,0,
        ViewEnum.TopToBottom,CornerEnum.UpperLeft,'RoleLevelRank',
        function (index,rank)
            rank.transform:Find('Auto_rank'):GetComponent('Text').text = tostring(index+1)
            rank.transform:Find('Auto_name'):GetComponent('Text').text = rankInfos[index].name
            rank.transform:Find('Auto_level'):GetComponent('Text').text = tostring(rankInfos[index].level)
            PanelMgr:ButtonAddListener(rank.transform:GetComponent('Button'),function ()
                AudioMgr:PlayEffect('button')
                Client.Send("CheckInfo",C2S_CheckInfo.New(rankInfos[index].Guid))
            end)
        end,
        function (index,rank)
            PanelMgr:ButtonRemoveListener(rank.transform:GetComponent('Button'))
        end)
        for index = 0,rankInfos.Count-1 do
            if rankInfos[index].Guid == RoleInfoMgr:GetMainRoleGuid() then
                this.Text_L_selfRank.text = index+1
                this.Text_L_selfName.text = rankInfos[index].name
                this.Text_L_selfLevel.text = rankInfos[index].level
                return
            end
        end
    end,
    [RankEnum.RolePower] = function (rankInfos)
        optimizeSV:VerticalLayout(this.Rect_view.transform:GetComponent('RectTransform'),contentRect,600,40,rankInfos.Count,0,0,0,0,0,
        ViewEnum.TopToBottom,CornerEnum.UpperLeft,'RolePowerRank',
        function (index,rank)
            rank.transform:Find('Auto_rank'):GetComponent('Text').text = tostring(index+1)
            rank.transform:Find('Auto_name'):GetComponent('Text').text = rankInfos[index].name
            rank.transform:Find('Auto_power'):GetComponent('Text').text = tostring(rankInfos[index].power)
            PanelMgr:ButtonAddListener(rank.transform:GetComponent('Button'),function ()
                AudioMgr:PlayEffect('button')
                Client.Send("CheckInfo",C2S_CheckInfo.New(rankInfos[index].Guid))
            end)
        end,
        function (index,rank)
            PanelMgr:ButtonRemoveListener(rank.transform:GetComponent('Button'))
        end)
        for index = 0,rankInfos.Count-1 do
            if rankInfos[index].Guid == RoleInfoMgr:GetMainRoleGuid() then
                this.Text_P_selfRank.text = index+1
                this.Text_P_selfName.text = rankInfos[index].name
                this.Text_P_selfPower.text = rankInfos[index].power
                return
            end
        end
    end
}

function RankPanel:ShowRanksInfo(rankInfos)

    PanelMgr:ScrollRectRemoveListener(self.Rect_view)

    optimizeSV:Clear()

    ShowRank[currentTypeRank](rankInfos)

    PanelMgr:ScrollRectAddListener(self.Rect_view,function (point)
        optimizeSV:Update()
    end)

end
--[[
function RankPanel:ChangeTypeImg(isChoose,Img_tog,highlightedIcon,normalIcon)
	local iconName
	if isChoose == true then
		iconName = highlightedIcon
	else
		iconName = normalIcon
	end
	ResMgr:LoadAssetSprite('othericon',{iconName},function (icon)
		Img_tog.sprite = icon
	end);
end
--]]