
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:主UI面板视图层
*
*        description:
*            功能描述:实现主UI面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


MainUIPanel = {}
local this = MainUIPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
MainUIPanel.Ctrls = 
{
    Img_roleIconImg = {  Path = 'LeftTopUI/role/Auto_roleIconImg',  ControlType = 'Image'  },
    Img_switchBtn = {  Path = 'RightBottomUI/Auto_switchBtn',  ControlType = 'Image'  },
    Img_setBtn = {  Path = 'RightBottomUI/Auto_setBtn',  ControlType = 'Image'  },
    Img_skillBtn = {  Path = 'RightBottomUI/Auto_skillBtn',  ControlType = 'Image'  },
    Img_bagBtn = {  Path = 'RightBottomUI/Auto_bagBtn',  ControlType = 'Image'  },
    Img_forgeBtn = {  Path = 'RightBottomUI/Auto_forgeBtn',  ControlType = 'Image'  },
    Img_team = {  Path = 'RightUI/Auto_team',  ControlType = 'Image'  },
    Img_task = {  Path = 'RightUI/Auto_task',  ControlType = 'Image'  },
    Img_content = {  Path = 'RightUI/Scroll View/Viewport/Auto_content',  ControlType = 'Image'  },
    Img_shop = {  Path = 'leftUI/Auto_shop',  ControlType = 'Image'  },
    Img_rankBtn = {  Path = 'TopUI/Auto_rankBtn',  ControlType = 'Image'  },
    Img_mailBtn = {  Path = 'TopUI/Auto_mailBtn',  ControlType = 'Image'  },
    Text_roleLevelText = {  Path = 'LeftTopUI/role/Auto_roleLevelText',  ControlType = 'Text'  },
    Text_power = {  Path = 'LeftTopUI/power/Auto_power',  ControlType = 'Text'  },
    Slider_hpRoleSlider = {  Path = 'LeftTopUI/role/Auto_hpRoleSlider',  ControlType = 'Slider'  },
    Slider_mpRoleSlider = {  Path = 'LeftTopUI/role/Auto_mpRoleSlider',  ControlType = 'Slider'  },
    Slider_experienceSlider  = {  Path = 'LeftTopUI/role/Auto_experienceSlider ',  ControlType = 'Slider'  },
    Tog_team = {  Path = 'RightUI/Auto_team',  ControlType = 'Toggle'  },
    Tog_task = {  Path = 'RightUI/Auto_task',  ControlType = 'Toggle'  },
    Btn_roleIconImg = {  Path = 'LeftTopUI/role/Auto_roleIconImg',  ControlType = 'Button'  },
    Btn_switchBtn = {  Path = 'RightBottomUI/Auto_switchBtn',  ControlType = 'Button'  },
    Btn_setBtn = {  Path = 'RightBottomUI/Auto_setBtn',  ControlType = 'Button'  },
    Btn_skillBtn = {  Path = 'RightBottomUI/Auto_skillBtn',  ControlType = 'Button'  },
    Btn_bagBtn = {  Path = 'RightBottomUI/Auto_bagBtn',  ControlType = 'Button'  },
    Btn_forgeBtn = {  Path = 'RightBottomUI/Auto_forgeBtn',  ControlType = 'Button'  },
    Btn_shop = {  Path = 'leftUI/Auto_shop',  ControlType = 'Button'  },
    Btn_rankBtn = {  Path = 'TopUI/Auto_rankBtn',  ControlType = 'Button'  },
    Btn_mailBtn = {  Path = 'TopUI/Auto_mailBtn',  ControlType = 'Button'  },
}

local ProfessionConfig = require "Config/ProfessionConfig"
local TaskConfig = require 'Config/TaskConfig'
local ExperienceConfig = require 'Config/ExperienceConfig'

local gameObject
local transform

local currentContent 

local updateTaskInfoObserver
local updateTeamInfoObserver
local updateRoleInfoObserver
local receiveMailObserver

local ContentEnum = {
    Team = 1,
    Task = 2
}

function MainUIPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function MainUIPanel:Init()

    updateTaskInfoObserver = Observer:New(function ()
        self:UpdateTaskBox()
    end)
    updateTeamInfoObserver = Observer:New(function (isLeader,selfGuid,playersData)
        self:UpdateTeamMember(isLeader,selfGuid,playersData)
    end)
    updateRoleInfoObserver = Observer:New(function ()
        self:UpdateRoleInfo()
    end)

    receiveMailObserver = Observer:New(function ()
        ResMgr:LoadAssetSprite('othericon',{'mailHighlighted'},function (icon)
            self.Img_mailBtn.sprite = icon
        end)
    end)

    --给队伍单选框添加监听事件
    PanelMgr:ToggleAddListener(self.Tog_team,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        if isChoose == false then
            self.Img_team.color = Color.gray
            return
        else
            if currentContent == ContentEnum.Team then
                return
            end
            self:ClearContent()
            self.Img_team.color = Color.white
            currentContent = ContentEnum.Team
            MainUICtrl:UpdateTeamInfo()
        end
    end)
    
    --给任务单选框添加监听事件
    PanelMgr:ToggleAddListener(self.Tog_task,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        if isChoose == false then
            self.Img_task.color = Color.gray
            return
        else
            if currentContent == ContentEnum.Task then
                return
            end
            self.Img_task.color = Color.white
            currentContent = ContentEnum.Task
            self:UpdateTaskBox()
        end
    end)

    --给商店按钮添加监听事件
    PanelMgr:ButtonAddListener(self.Btn_shop,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:OpenPanel('ShopPanel',UILayer.Middle,MaskEnum.Mask)
    end)

    --添加背包图标按钮监听事件
    PanelMgr:ButtonAddListener(self.Btn_bagBtn,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:OpenPanel('BagPanel',UILayer.Middle,MaskEnum.Mask)
    end)

    --添加技能图标按钮监听事件
    PanelMgr:ButtonAddListener(self.Btn_skillBtn,function ()
        AudioMgr:PlayEffect('button')
		PanelMgr:OpenPanel("SkillPanel",UILayer.Middle,MaskEnum.Mask)
    end)

    --给锻造按钮添加监听事件
    PanelMgr:ButtonAddListener(self.Btn_forgeBtn,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:OpenPanel("ForgePanel",UILayer.Middle,MaskEnum.Mask)
    end)

    --给设置按钮添加监听事件
    PanelMgr:ButtonAddListener(self.Btn_setBtn,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:OpenPanel("SettingPanel",UILayer.Middle,MaskEnum.Mask)
    end)

    PanelMgr:ButtonAddListener(self.Btn_rankBtn,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:OpenPanel("RankPanel",UILayer.Middle,MaskEnum.Mask)
    end)

    PanelMgr:ButtonAddListener(self.Btn_mailBtn,function ()
        AudioMgr:PlayEffect('button')
        ResMgr:LoadAssetSprite('othericon',{'mailNormal'},function (icon)
            self.Img_mailBtn.sprite = icon
        end)
        PanelMgr:OpenPanel("MailPanel",UILayer.Middle,MaskEnum.Mask)
    end)

end

function MainUIPanel:Show()

    --设置全屏显示此面板
    PanelMgr:FullScreen(transform:GetComponent('RectTransform'))

    --默认高亮队伍单选框
    self.Img_team.color = Color.white
    self.Img_task.color = Color.gray
    self.Tog_team.isOn = true
    currentContent = ContentEnum.Team
    --self:UpdateRoleInfo()
    --清空内容
    self:ClearContent()

    UIMgr:AddListener('UpdateTeamInfo',updateTeamInfoObserver)
    UIMgr:AddListener('UpdateTaskBox',updateTaskInfoObserver)
    UIMgr:AddListener('UpdateRoleInfo',updateRoleInfoObserver)
    UIMgr:AddListener("ReceiveNewMail",receiveMailObserver)

end

function MainUIPanel:Hide()

    UIMgr:RemoveListener('UpdateTeamInfo',updateTeamInfoObserver)
    UIMgr:RemoveListener('UpdateTaskBox',updateTaskInfoObserver)
    UIMgr:RemoveListener('UpdateRoleInfo',updateRoleInfoObserver)
    UIMgr:RemoveListener("ReceiveNewMail",receiveMailObserver)
    currentContent = nil

end

--更新人物显示信息
function MainUIPanel:UpdateRoleInfo()

    local roleData = RoleInfoMgr:GetMainRole()
    --更新角色头像框的相关属性
    self.Text_roleLevelText.text = tostring(roleData:GetLevel())
	ResMgr:LoadAssetSprite('roleicon',{ProfessionConfig[roleData:GetProfession()].headIcon},function (icon)
        self.Img_roleIconImg.sprite = icon
    end)
    self.Text_power.text = roleData:GetPower()
    self.Slider_hpRoleSlider.value = roleData.OtherData:GetHp()/roleData.OtherData:GetMaxHp()
    self.Slider_mpRoleSlider.value = roleData.OtherData:GetMp()/roleData.OtherData:GetMaxMp()
    if roleData:GetLevel() >= 100 then
        self.Slider_experienceSlider.value = 1
    else
        self.Slider_experienceSlider.value = roleData.OtherData:GetExperience() / ExperienceConfig[roleData:GetLevel()+1].experience
    end

end

--清空队伍或任务内容
function MainUIPanel:ClearContent()

    local objs = Util.GetChildrens(self.Img_content.transform)
    if objs == nil or objs.Length == 1 then
        return;
    end

    for index = 1,objs.Length-1 do
        local obj = objs[index]
        if obj.name == 'Member' then
            PanelMgr:ButtonRemoveListener(obj.transform:Find('Auto_changeLeader'):GetComponent('Button'))
            PanelMgr:ButtonRemoveListener(obj.transform:Find('Auto_removeMember'):GetComponent('Button'))
            PoolMgr:Set(obj.name,obj.gameObject)
        end
        if obj.name == 'TaskBox' then
            PanelMgr:ButtonRemoveListener(obj.transform:GetComponent('Button'))
            PoolMgr:Set(obj.name,obj.gameObject)
        end
    end

end

function MainUIPanel:UpdateTaskBox()

    print('刷新任务')
    if currentContent ~= ContentEnum.Task then
        return
    end
    self:ClearContent()
    local conductTasks = TaskManager:GetConductTasks()
    
    if next(conductTasks) == nil then
        return
    end
    for key,value in pairs(conductTasks) do
        PoolMgr:Get('TaskBox',function (TaskBox)
            TaskBox.transform:SetParent(self.Img_content.transform)
            TaskBox.transform:Find('Auto_taskName'):GetComponent('Text').text = TaskConfig[key].name
            TaskBox.transform:Find('Auto_taskContent'):GetComponent('Text').text = TaskConfig[key].content
            if TaskConfig[key].type == TaskEnum.Talk then
                UIMgr:SetActive(TaskBox.transform:Find('Auto_taskProgress'),false)
            else
                UIMgr:SetActive(TaskBox.transform:Find('Auto_taskProgress'),true)
                if value >= TaskConfig[key].count then
                    TaskBox.transform:Find('Auto_taskProgress'):GetComponent('Text').color = Color.green
                else
                    TaskBox.transform:Find('Auto_taskProgress'):GetComponent('Text').color = Color.red
                end
                TaskBox.transform:Find('Auto_taskProgress'):GetComponent('Text').text = '进度:'..tostring(value)..'/'
                ..tostring(TaskConfig[key].count)
            end
        end)
    end

end

--更新队伍成员显示
function MainUIPanel:UpdateTeamMember(isLeader,selfGuid,playersData)

    print('刷新队伍成员')
    if currentContent ~= ContentEnum.Team then
        return
    end
    self:ClearContent()
    for index = 1,#playersData do
        PoolMgr:Get("Member",function (member)
            if index == 1 then 
                UIMgr:SetActive(member.transform:Find('Auto_leader'),true)
            else
                UIMgr:SetActive(member.transform:Find('Auto_leader'),false)
            end
            local Guid = playersData[index]:GetGuid()
            local roleData = playersData[index]
            member.transform:SetParent(self.Img_content.transform)
            ResMgr:LoadAssetSprite('roleicon',{ProfessionConfig[roleData.GetProfession()].headIcon},function (icon)
                member.transform:Find('Auto_roleIcon'):GetComponent('Image').sprite = icon
            end);
            member.transform:Find('Auto_level'):GetComponent('Text').text = "等级:"..tostring(roleData:GetLevel())
            member.transform:Find('Auto_name'):GetComponent('Text').text = roleData:GetName()
            if isLeader == true and Guid ~= selfGuid then
                UIMgr:SetActive(member.transform:Find('Auto_changeLeader'),true)
                PanelMgr:ButtonAddListener(member.transform:Find('Auto_changeLeader'):GetComponent('Button'),function ()
                    MainUICtrl:ChangeLeader(roleData:GetGuid())
                end)
            else
                UIMgr:SetActive(member.transform:Find('Auto_changeLeader'),false)
            end
            if isLeader == true and Guid ~= selfGuid then
                UIMgr:SetActive(member.transform:Find('Auto_removeMember'),true)
                PanelMgr:ButtonAddListener(member.transform:Find('Auto_removeMember'):GetComponent('Button'),function ()
                    MainUICtrl:ExitTeam(roleData:GetGuid())
                end)
            else
                UIMgr:SetActive(member.transform:Find('Auto_removeMember'),false)
            end
            if Guid == selfGuid then
                UIMgr:SetActive(member.transform:Find('Auto_exitTeam'),true)
                PanelMgr:ButtonAddListener(member.transform:Find('Auto_exitTeam'):GetComponent('Button'),function ()
                    MainUICtrl:ExitTeam(roleData:GetGuid())
                end)
            else
                UIMgr:SetActive(member.transform:Find('Auto_exitTeam'),false)
            end
        end)
    end
end


