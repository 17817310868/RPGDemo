
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:对话面板视图层
*
*        description:
*            功能描述:实现对话面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

local TaskConfig = require 'Config/TaskConfig'
local ItemConfig = require 'Config/ItemConfig'
local EquipConfig = require 'Config/EquipConfig'
local GemConfig = require 'Config/GemConfig'
local NPCConfig = require 'Config/NPCConfig'
local TalkConfig = require 'Config/TalkConfig'

TalkPanel = {}
local this = TalkPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
TalkPanel.Ctrls = 
{
    Img_taskContent = {  Path = 'Auto_taskContent',  ControlType = 'Image'  },
    Img_goodsContent = {  Path = 'Auto_goodsContent',  ControlType = 'Image'  },
    Img_acceptBtn = {  Path = 'Auto_acceptBtn',  ControlType = 'Image'  },
    Img_submitBtn = {  Path = 'Auto_submitBtn',  ControlType = 'Image'  },
    Text_talkContent = {  Path = 'Auto_talkContent',  ControlType = 'Text'  },
    Btn_acceptBtn = {  Path = 'Auto_acceptBtn',  ControlType = 'Button'  },
    Btn_submitBtn = {  Path = 'Auto_submitBtn',  ControlType = 'Button'  },
}

local gameObject
local transform

local currentTaskId
local currentTalkOrder
local taskObserver

function TalkPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function TalkPanel:Init()
    currentTalkOrder = 0
    taskObserver = Observer:New(function (npcId)
        self:ShowTasks(npcId)
    end)
    

end

function TalkPanel:Show()

    UIMgr:AddListener('ShowTasks',taskObserver)
    transform:GetComponent('RectTransform').anchorMin = Vector2.New(0.5,0)
    transform:GetComponent('RectTransform').anchorMax = Vector2.New(0.5,0)
    transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(0,50)

    UIMgr:SetActive(self.Btn_acceptBtn.transform,false)
    UIMgr:SetActive(self.Btn_submitBtn.transform,false)

end

function TalkPanel:Hide()
    self:ClearTasksTitle()
    self:ClearTaskGoods()
    PanelMgr:ButtonRemoveListener(transform:GetComponent('Button'))
    PanelMgr:ButtonRemoveListener(self.Btn_acceptBtn)
    PanelMgr:ButtonRemoveListener(self.Btn_submitBtn)
    currentTaskId = nil
    currentTalkOrder = 0
    UIMgr:RemoveListener('ShowTasks',taskObserver)
end

function TalkPanel:ShowTasks(npcId)

    local acceptableTasks = TaskManager:GetNpcAcceptableTasks(npcId)
    local conductTasks = TaskManager:GetNpcConductTasks(npcId)

    if acceptableTasks ~= nil then
        for index = 1,#acceptableTasks do
            PoolMgr:Get('TaskTitle',function (taskTitle)
                taskTitle.transform:SetParent(self.Img_taskContent.transform)
                ResMgr:LoadAssetSprite('taskicon',{'keJie'},function (icon)
                    taskTitle.transform:Find('Auto_taskIcon'):GetComponent('Image').sprite = icon
                end)
                taskTitle.transform:Find('Auto_taskTitle'):GetComponent('Text').text = TaskConfig[acceptableTasks[index]].name
                PanelMgr:ButtonAddListener(taskTitle.transform:GetComponent('Button'),function ()
                    AudioMgr:PlayEffect('button')
                    self:ClearTasksTitle()
                    currentTaskId = acceptableTasks[index]
                    self:PlayTaskTalks()
                    PanelMgr:ButtonAddListener(transform:GetComponent('Button'),function ()
                        self:PlayTaskTalks()
                    end)
                    --self:ShowTaskGoods(acceptableTasks[index])
                    --UIMgr:SetActive(self.Btn_acceptBtn.transform,true)
                end)
            end)
        end
    end
    if conductTasks ~= nil then
        for index = 1,#conductTasks do
            PoolMgr:Get('TaskTitle',function (taskTitle)
                taskTitle.transform:SetParent(self.Img_taskContent.transform)
                local iconName;
                if TaskManager:IsCompleteTask(conductTasks[index]) == true then
                    iconName = 'yiWanCheng'
                else
                    iconName = 'weiWanCheng'
                end
                ResMgr:LoadAssetSprite('taskicon',{iconName},function (icon)
                    taskTitle.transform:Find('Auto_taskIcon'):GetComponent('Image').sprite = icon
                end)

                taskTitle.transform:Find('Auto_taskTitle'):GetComponent('Text').text = TaskConfig[conductTasks[index]].name

                PanelMgr:ButtonAddListener(taskTitle.transform:GetComponent('Button'),function ()
                    AudioMgr:PlayEffect('button')
                    self:ClearTasksTitle()
                    currentTaskId = conductTasks[index]
                    self:ShowTaskGoods()
                    --if TaskManager:IsCompleteTask(conductTasks[index]) == true then
                        --UIMgr:SetActive(self.Btn_submitBtn.transform,true)
                    --end
                end)
            end)
        end
    end
end

function TalkPanel:ClearTasksTitle()

    local titles = Util.GetChildrens(self.Img_taskContent.transform)
	if titles == nil or titles.Length == 1 then
		return;
	end
	for index = 1,titles.Length-1 do
		if titles[index].name == 'TaskTitle' then
			PanelMgr:ButtonRemoveListener(titles[index]:GetComponent('Button'))
			PoolMgr:Set(titles[index].name,titles[index].gameObject)
		end
	end

end

function TalkPanel:PlayTaskTalks()

    currentTalkOrder = currentTalkOrder + 1
    local talks = self:StringToNumberTable(TaskConfig[currentTaskId].talk)
    local NPCs = self:StringToNumberTable(TaskConfig[currentTaskId].talkNPC)
    if currentTalkOrder > #talks then
        self:ShowTaskGoods(taskId)
        PanelMgr:ButtonRemoveListener(transform:GetComponent('Button'))
        return
    end

    self.Text_talkContent.text = NPCConfig[NPCs[currentTalkOrder]].name .. ':' .. TalkConfig[talks[currentTalkOrder]].content

end

function TalkPanel:ShowTaskGoods()
    
    self.Text_talkContent.text = TaskConfig[currentTaskId].content

    if TaskManager:IsAcceptableTask(currentTaskId) == true then
        UIMgr:SetActive(self.Btn_acceptBtn.transform,true)
        PanelMgr:ButtonAddListener(self.Btn_acceptBtn,function ()
            AudioMgr:PlayEffect('button')
            TalkCtrl:AcceptTask(currentTaskId)
            PanelMgr:ClosePanel()
        end)
    end
    if TaskManager:IsAcceptTask(currentTaskId) == true then
        if TaskManager:IsCompleteTask(currentTaskId)  == true then
            AudioMgr:PlayEffect('button')
            UIMgr:SetActive(self.Btn_submitBtn.transform,true)
            PanelMgr:ButtonAddListener(self.Btn_submitBtn,function ()
                TalkCtrl:SubmitTask(currentTaskId)
                PanelMgr:ClosePanel()
            end)
        else
            UIMgr:SetActive(self.Btn_submitBtn.transform,false)
        end
    end

    if TaskConfig[currentTaskId].item ~= -1 then
        local items = self:StringToNumberTable(tostring(TaskConfig[currentTaskId].item))
        for index = 1,#items do
            PoolMgr:Get('Goods',function (goods)
                goods.transform:SetParent(self.Img_goodsContent.transform)
                ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
                    goods.transform:Find('Auto_goodsBg'):GetComponent('Image').sprite = icon
                end)
                ResMgr:LoadAssetSprite('itemicon',{ItemConfig[items[index]].icon},function (icon)
                    goods.transform:Find('Auto_goodsIcon'):GetComponent('Image').sprite = icon
                end)
                goods.transform:Find('Auto_goodsCount'):GetComponent('Text').text = '1'
            end)
        end
    end

    if TaskConfig[currentTaskId].equip ~= -1 then
        local equips = self:StringToNumberTable(tostring(TaskConfig[currentTaskId].equip))
        for index = 1,#equips do
            PoolMgr:Get('Goods',function (goods)
                goods.transform:SetParent(self.Img_goodsContent.transform)
                ResMgr:LoadAssetSprite('itembg',{EquipConfig[equips[index]].color},function (icon)
                    goods.transform:Find('Auto_goodsBg'):GetComponent('Image').sprite = icon
                end)
                ResMgr:LoadAssetSprite('itemicon',{EquipConfig[equips[index]].icon},function (icon)
                    goods.transform:Find('Auto_goodsIcon'):GetComponent('Image').sprite = icon
                end)
                goods.transform:Find('Auto_goodsCount'):GetComponent('Text').text = ''
            end)
        end
    end

    if TaskConfig[currentTaskId].gem ~= -1 then
        local gems = self:StringToNumberTable(tostring(TaskConfig[currentTaskId].gem))
        for index = 1,#gems do
            PoolMgr:Get('Goods',function (goods)
                goods.transform:SetParent(self.Img_goodsContent.transform)
                ResMgr:LoadAssetSprite('itembg',{GemConfig[gems[index]].color},function (icon)
                    goods.transform:Find('Auto_goodsBg'):GetComponent('Image').sprite = icon
                end)
                ResMgr:LoadAssetSprite('itemicon',{GemConfig[gems[index]].icon},function (icon)
                    goods.transform:Find('Auto_goodsIcon'):GetComponent('Image').sprite = icon
                end)
                goods.transform:Find('Auto_goodsCount'):GetComponent('Text').text = '1'
            end)
        end
    end

    if TaskConfig[currentTaskId].gold ~= 0 then
        PoolMgr:Get('Goods',function (goods)
            goods.transform:SetParent(self.Img_goodsContent.transform)
            ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
                goods.transform:Find('Auto_goodsBg'):GetComponent('Image').sprite = icon
            end)
            ResMgr:LoadAssetSprite('othericon',{'gold'},function (icon)
                goods.transform:Find('Auto_goodsIcon'):GetComponent('Image').sprite = icon
            end)
            goods.transform:Find('Auto_goodsCount'):GetComponent('Text').text = tostring(TaskConfig[currentTaskId].gold)
        end)
    end

    if TaskConfig[currentTaskId].silver ~= 0 then
        PoolMgr:Get('Goods',function (goods)
            goods.transform:SetParent(self.Img_goodsContent.transform)
            ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
                goods.transform:Find('Auto_goodsBg'):GetComponent('Image').sprite = icon
            end)
            ResMgr:LoadAssetSprite('othericon',{'silver'},function (icon)
                goods.transform:Find('Auto_goodsIcon'):GetComponent('Image').sprite = icon
            end)
            goods.transform:Find('Auto_goodsCount'):GetComponent('Text').text = tostring(TaskConfig[currentTaskId].silver)
        end)
    end

    if TaskConfig[currentTaskId].experience ~= 0 then
        PoolMgr:Get('Goods',function (goods)
            goods.transform:SetParent(self.Img_goodsContent.transform)
            ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
                goods.transform:Find('Auto_goodsBg'):GetComponent('Image').sprite = icon
            end)
            ResMgr:LoadAssetSprite('itemicon',{'experience'},function (icon)
                goods.transform:Find('Auto_goodsIcon'):GetComponent('Image').sprite = icon
            end)
            goods.transform:Find('Auto_goodsCount'):GetComponent('Text').text = tostring(TaskConfig[currentTaskId].experience)
        end)
    end

end

function TalkPanel:ClearTaskGoods()

    local goods = Util.GetChildrens(self.Img_goodsContent.transform)
	if goods == nil or goods.Length == 1 then
		return;
	end
	for index = 1,goods.Length-1 do
		if goods[index].name == 'Goods' then
			PoolMgr:Set(goods[index].name,goods[index].gameObject)
		end
	end

end

function TalkPanel:StringToNumberTable(ItemsString)

    local currentIndex = 1
    local items = {}
    
    for index = 1,#ItemsString do
        if string.sub(ItemsString,index,index) == '|' then
            table.insert(items,tonumber(string.sub(ItemsString,currentIndex,index-1)))
            currentIndex = index + 1
        end
    end
    table.insert(items,tonumber(string.sub(ItemsString,currentIndex,#ItemsString)))
    return items

end