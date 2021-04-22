
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:锻造面板视图层
*
*        description:
*            功能描述:实现锻造面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


ForgePanel = {}
local this = ForgePanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
ForgePanel.Ctrls = 
{
    Img_forgeType = {  Path = 'Auto_forgeType',  ControlType = 'Image'  },
    Img_makeTog = {  Path = 'Auto_forgeType/Auto_makeTog',  ControlType = 'Image'  },
    Img_inlayTog = {  Path = 'Auto_forgeType/Auto_inlayTog',  ControlType = 'Image'  },
    Img_advanceTog = {  Path = 'Auto_forgeType/Auto_advanceTog',  ControlType = 'Image'  },
    Img_makePanel = {  Path = 'Auto_makePanel',  ControlType = 'Image'  },
    Img_makeView = {  Path = 'Auto_makePanel/Auto_makeView',  ControlType = 'Image'  },
    Img_makeContent = {  Path = 'Auto_makePanel/Auto_makeView/Viewport/Auto_makeContent',  ControlType = 'Image'  },
    Img_materialView = {  Path = 'Auto_makePanel/detailPanel/Auto_materialView',  ControlType = 'Image'  },
    Img_materialContent = {  Path = 'Auto_makePanel/detailPanel/Auto_materialView/Viewport/Auto_materialContent',  ControlType = 'Image'  },
    Img_makeEquipInfo = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo',  ControlType = 'Image'  },
    Img_M_icon = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_icon',  ControlType = 'Image'  },
    Img_makeBtn = {  Path = 'Auto_makePanel/Auto_makeBtn',  ControlType = 'Image'  },
    Img_inlayPanel = {  Path = 'Auto_inlayPanel',  ControlType = 'Image'  },
    Img_equipView = {  Path = 'Auto_inlayPanel/Auto_equipView',  ControlType = 'Image'  },
    Img_equipContent = {  Path = 'Auto_inlayPanel/Auto_equipView/Viewport/Auto_equipContent',  ControlType = 'Image'  },
    Img_inlayEquipInfo = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo',  ControlType = 'Image'  },
    Img_I_equipBg = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_equipBg',  ControlType = 'Image'  },
    Img_I_equipIcon = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_equipIcon',  ControlType = 'Image'  },
    Img_firstGemTog = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_firstGemTog',  ControlType = 'Image'  },
    Img_firstGem = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_firstGemTog/Auto_firstGem',  ControlType = 'Image'  },
    Img_secondGemTog = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_secondGemTog',  ControlType = 'Image'  },
    Img_secondGem = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_secondGemTog/Auto_secondGem',  ControlType = 'Image'  },
    Img_gemView = {  Path = 'Auto_inlayPanel/Auto_gemView',  ControlType = 'Image'  },
    Img_gemContent = {  Path = 'Auto_inlayPanel/Auto_gemView/Viewport/Auto_gemContent',  ControlType = 'Image'  },
    Img_inlayBtn = {  Path = 'Auto_inlayPanel/Auto_inlayBtn',  ControlType = 'Image'  },
    Img_removeBtn = {  Path = 'Auto_inlayPanel/Auto_removeBtn',  ControlType = 'Image'  },
    Img_advancePanel = {  Path = 'Auto_advancePanel',  ControlType = 'Image'  },
    Img_A_equipView = {  Path = 'Auto_advancePanel/Auto_A_equipView',  ControlType = 'Image'  },
    Img_A_equipContent = {  Path = 'Auto_advancePanel/Auto_A_equipView/Viewport/Auto_A_equipContent',  ControlType = 'Image'  },
    Img_advanceEquipInfo = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo',  ControlType = 'Image'  },
    Img_oldEquipBg = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldEquipBg',  ControlType = 'Image'  },
    Img_oldEquipIcon = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldEquipIcon',  ControlType = 'Image'  },
    Img_newEquip = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip',  ControlType = 'Image'  },
    Img_newEquipBg = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newEquipBg',  ControlType = 'Image'  },
    Img_newEquipIcon = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newEquipIcon',  ControlType = 'Image'  },
    Img_advanceBtn = {  Path = 'Auto_advancePanel/Auto_advanceBtn',  ControlType = 'Image'  },
    Img_close = {  Path = 'Auto_close',  ControlType = 'Image'  },
    Text_M_name = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_name',  ControlType = 'Text'  },
    Text_M_level = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_level',  ControlType = 'Text'  },
    Text_M_profession = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_profession',  ControlType = 'Text'  },
    Text_M_type = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_type',  ControlType = 'Text'  },
    Text_M_firstAttr = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_firstAttr',  ControlType = 'Text'  },
    Text_M_secondAttr = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_secondAttr',  ControlType = 'Text'  },
    Text_M_thirdAttr = {  Path = 'Auto_makePanel/detailPanel/Auto_makeEquipInfo/Auto_M_thirdAttr',  ControlType = 'Text'  },
    Text_Tips = {  Path = 'Auto_makePanel/Auto_Tips',  ControlType = 'Text'  },
    Text_I_name = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_name',  ControlType = 'Text'  },
    Text_I_level = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_level',  ControlType = 'Text'  },
    Text_I_type = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_type',  ControlType = 'Text'  },
    Text_I_profession = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_profession',  ControlType = 'Text'  },
    Text_I_firstAttr = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_firstAttr',  ControlType = 'Text'  },
    Text_I_secondAttr = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_secondAttr',  ControlType = 'Text'  },
    Text_I_thirdAttr = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_I_thirdAttr',  ControlType = 'Text'  },
    Text_firstGemAttr = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_firstGemTog/Auto_firstGemAttr',  ControlType = 'Text'  },
    Text_secondGemAttr = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_secondGemTog/Auto_secondGemAttr',  ControlType = 'Text'  },
    Text_oldName = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldName',  ControlType = 'Text'  },
    Text_oldLevel = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldLevel',  ControlType = 'Text'  },
    Text_oldProfession = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldProfession',  ControlType = 'Text'  },
    Text_oldType = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldType',  ControlType = 'Text'  },
    Text_oldFirstAttr = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldFirstAttr',  ControlType = 'Text'  },
    Text_oldSecondAttr = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldSecondAttr',  ControlType = 'Text'  },
    Text_oldThirdAttr = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/oldEquip/Auto_oldThirdAttr',  ControlType = 'Text'  },
    Text_newName = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newName',  ControlType = 'Text'  },
    Text_newLevel = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newLevel',  ControlType = 'Text'  },
    Text_newProfession = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newProfession',  ControlType = 'Text'  },
    Text_newType = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newType',  ControlType = 'Text'  },
    Text_newFirstAttr = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newFirstAttr',  ControlType = 'Text'  },
    Text_newSecondAttr = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newSecondAttr',  ControlType = 'Text'  },
    Text_newThirdAttr = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_newEquip/Auto_newThirdAttr',  ControlType = 'Text'  },
    Text_money = {  Path = 'Auto_advancePanel/Auto_advanceEquipInfo/Auto_money',  ControlType = 'Text'  },
    Tog_makeTog = {  Path = 'Auto_forgeType/Auto_makeTog',  ControlType = 'Toggle'  },
    Tog_inlayTog = {  Path = 'Auto_forgeType/Auto_inlayTog',  ControlType = 'Toggle'  },
    Tog_advanceTog = {  Path = 'Auto_forgeType/Auto_advanceTog',  ControlType = 'Toggle'  },
    Tog_firstGemTog = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_firstGemTog',  ControlType = 'Toggle'  },
    Tog_secondGemTog = {  Path = 'Auto_inlayPanel/Auto_inlayEquipInfo/Auto_secondGemTog',  ControlType = 'Toggle'  },
    Btn_makeBtn = {  Path = 'Auto_makePanel/Auto_makeBtn',  ControlType = 'Button'  },
    Btn_inlayBtn = {  Path = 'Auto_inlayPanel/Auto_inlayBtn',  ControlType = 'Button'  },
    Btn_removeBtn = {  Path = 'Auto_inlayPanel/Auto_removeBtn',  ControlType = 'Button'  },
    Btn_advanceBtn = {  Path = 'Auto_advancePanel/Auto_advanceBtn',  ControlType = 'Button'  },
    Btn_close = {  Path = 'Auto_close',  ControlType = 'Button'  },
    Rect_makeView = {  Path = 'Auto_makePanel/Auto_makeView',  ControlType = 'ScrollRect'  },
    Rect_materialView = {  Path = 'Auto_makePanel/detailPanel/Auto_materialView',  ControlType = 'ScrollRect'  },
    Rect_equipView = {  Path = 'Auto_inlayPanel/Auto_equipView',  ControlType = 'ScrollRect'  },
    Rect_gemView = {  Path = 'Auto_inlayPanel/Auto_gemView',  ControlType = 'ScrollRect'  },
    Rect_A_equipView = {  Path = 'Auto_advancePanel/Auto_A_equipView',  ControlType = 'ScrollRect'  },
}

local ItemConfig = require 'Config/ItemConfig'  
local EquipConfig = require 'Config/EquipConfig'
local ProfessionConfig = require 'Config/ProfessionConfig'
local GemConfig = require 'Config/GemConfig'
local AttrConfig = require "Config/AttrConfig"
local FormulaConfig = require 'Config/FormulaConfig'

local gameObject
local transform

local currentMakeBook  --当前选择制造书
local currentEquip  --当前选择得装备

local materials  --存储所有材料对象

local currentType  --当前选择得锻造类型

local currentGemId  --当前选择得背包得宝石id
local currentGemHole  --当前选择得装备上得宝石孔
local currentChooseGem  --当前选择的装备上得宝石

local MakeBookOptimizeSV  --制造书滚动视图优化组件
local InalyEquipOptimizeSV  --镶嵌面板装备格子滚动视图优化组件
local GemOptimizeSV  --宝石格子滚动视图优化组件
local AdvanceEquipOpimizeSV  --进阶面板装备格子滚动视图优化组件

local makeEquipObserver  --制造面板观察者事件
local inlayGemObserver  --镶嵌面板观察者事件
local advanceObserver  --进阶面板观察者事件

function ForgePanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function ForgePanel:Init()

    makeEquipObserver = Observer:New(function ()  --制造面板事件回调
        self:CloseMakePanel()  --重新打开制造面板(即刷新)
        self:OpenMakePanel()
    end)
    inlayGemObserver = Observer:New(function ()  --镶嵌面板事件回调
        self:CloseInlayPanel()  --重新打开镶嵌面板(即刷新)
        self:OpenInlayPanel()
    end)
    advanceObserver = Observer:New(function ()  --进阶面板事件回调
        self:CloseAdvancePanel()  --重新打开进阶面板(即刷新)
        self:OpenAdvancePanel()
    end)

    MakeBookOptimizeSV = OptimizeSV:New()
    InalyEquipOptimizeSV = OptimizeSV:New()
    GemOptimizeSV = OptimizeSV:New()
    AdvanceEquipOpimizeSV = OptimizeSV:New()

    materials = {}

    --给右侧得打造按钮添加监听
    PanelMgr:ToggleAddListener(self.Tog_makeTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_makeTog,'btn2Highlighted','btn2Normal')  --更新按钮图标
        --if isChoose == true then
        if currentType == ForgeEnum.Make then
            return
        end
        currentType = ForgeEnum.Make
        self:CloseAdvancePanel()  --关闭进阶面板
        self:CloseInlayPanel()  --关闭镶嵌面板
        self:OpenMakePanel()  --打开打造面板
        --UIMgr:SetActive(self.Img_makePanel.transform,true)
        --UIMgr:SetActive(self.Img_inlayPanel.transform,false)
        --UIMgr:SetActive(self.Img_advancePanel.transform,false)
        --self:UpdateMakePanel()
        --end
    end)

    --给右侧得镶嵌按钮添加监听
    PanelMgr:ToggleAddListener(self.Tog_inlayTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_inlayTog,'btn2Highlighted','btn2Normal')  --更新按钮图标
        --if isChoose == true then
        if currentType == ForgeEnum.Inlay then
            return
        end
        currentType = ForgeEnum.Inlay
        self:CloseMakePanel()  --关闭打造面板
        self:CloseAdvancePanel()  --关闭进阶面板
        self:OpenInlayPanel()  --打开镶嵌面板
        --UIMgr:SetActive(self.Img_makePanel.transform,false)
        --UIMgr:SetActive(self.Img_inlayPanel.transform,true)
        --UIMgr:SetActive(self.Img_advancePanel.transform,false)
        --self:UpdateInlayPanel()
        --end
    end)

    --给右侧得进阶按钮添加监听
    PanelMgr:ToggleAddListener(self.Tog_advanceTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_advanceTog,'btn2Highlighted','btn2Normal')  --更新按钮图标
        --if isChoose == true then
        if currentType == ForgeEnum.Advance then
            return
        end
        currentType = ForgeEnum.Advance
        self:CloseMakePanel()  --关闭打造面板
        self:CloseInlayPanel()  --关闭镶嵌面板
        self:OpenAdvancePanel()  --打开进阶面板
        --UIMgr:SetActive(self.Img_makePanel.transform,false)
        --UIMgr:SetActive(self.Img_inlayPanel.transform,false)
        --UIMgr:SetActive(self.Img_advancePanel.transform,true)
        --self:UpdateAdvancePanel()
        --end
    end)

    --关闭按钮
    PanelMgr:ButtonAddListener(self.Btn_close,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()
    end)

    --打造按钮
    PanelMgr:ButtonAddListener(self.Btn_makeBtn,function ()
        AudioMgr:PlayEffect('button')
        if currentMakeBook == nil then
            return
        end
        ForgeCtrl:MakeEquip(currentMakeBook:GetGuid())
        --self:UpdateMakePanel()
    end)

    --镶嵌按钮
    PanelMgr:ButtonAddListener(self.Btn_inlayBtn,function ()
        AudioMgr:PlayEffect('button')
        if currentGemHole == nil then
            print('未选择宝石孔')
            return
        end
        if currentGemId ~= nil then
            print('该位置上已有宝石')
            return
        end
        if currentChooseGem == nil then
            return
        end

        ForgeCtrl:InlayGem(currentEquip:GetGuid(),currentGemHole,currentChooseGem:GetGuid())  --调用逻辑层得镶嵌宝石函数

    end)

    --移除按钮
    PanelMgr:ButtonAddListener(self.Btn_removeBtn,function ()
        AudioMgr:PlayEffect('button')
        if currentGemHole == nil then
            print('未选择宝石孔')
            return
        end
        if currentGemId == nil then
            print('未选择宝石')
            return
        end

        ForgeCtrl:RemoveGem(currentEquip:GetGuid(),currentGemHole)  --调用逻辑层得移除宝石函数

    end)

    --进阶按钮
    PanelMgr:ButtonAddListener(self.Btn_advanceBtn,function ()
        AudioMgr:PlayEffect('button')
        if EquipConfig[currentEquip:GetItemId()].quality > 4 then
            return
        end
        ForgeCtrl:Advance(currentEquip:GetGuid())  --调用逻辑层得进阶函数
    end)

end

function ForgePanel:Show()
    --默认打开打造面板
    self.Tog_makeTog.isOn = false
    self.Tog_makeTog.isOn = true
    
end

function ForgePanel:Hide()
    self:CloseMakePanel()
    self:CloseInlayPanel()
    self:CloseAdvancePanel()
    currentType = nil
end
--[[
function ForgePanel:ChangeImg(isChoose,Img,trueIcon,falseIcon)
    local iconName
	if isChoose == true then
		iconName = trueIcon
	else
		iconName = falseIcon
    end
	ResMgr:LoadAssetSprite('othericon',{iconName},function (icon)
		Img.sprite = icon
	end);
end
--]]
--打开制造面板
function ForgePanel:OpenMakePanel()
    UIMgr:SetActive(self.Img_makePanel.transform,true)
    UIMgr:SetActive(self.Img_makeEquipInfo.transform,false)  --隐藏装备具体信息
    UIMgr:SetActive(self.Img_materialContent.transform,false)  --隐藏材料信息
    UIMgr:SetActive(self.Btn_makeBtn.transform,false)  --隐藏制造按钮
    UIMgr:SetActive(self.Text_Tips.transform,false)  --隐藏制造提示
    self:UpdateMakeInfo()
    UIMgr:AddListener('UpdateBagItems',makeEquipObserver)
end

--关闭制造面板
function ForgePanel:CloseMakePanel()
    PanelMgr:ScrollRectRemoveListener(self.Rect_makeView)
    MakeBookOptimizeSV:Clear()
    self:ClearMaterial()
    UIMgr:SetActive(self.Img_makePanel.transform,false)
    currentMakeBook = nil
    UIMgr:RemoveListener('UpdateBagItems',makeEquipObserver)
end

--打开镶嵌面板
function ForgePanel:OpenInlayPanel()
    UIMgr:SetActive(self.Img_inlayPanel.transform,true)
    self:UpdateInlayInfo()
    self:UpdateGem()
    UIMgr:AddListener('UpdateBagItems',inlayGemObserver)
end

--关闭镶嵌面板
function ForgePanel:CloseInlayPanel()

    PanelMgr:ScrollRectRemoveListener(self.Rect_equipView)
    InalyEquipOptimizeSV:Clear()
    PanelMgr:ScrollRectRemoveListener(self.Rect_gemView)
    GemOptimizeSV:Clear()

    UIMgr:SetActive(self.Img_inlayEquipInfo.transform,false)
    UIMgr:SetActive(self.Img_inlayPanel.transform,false)
    currentGemId = nil
    currentGemHole = nil
    currentChooseGem = nil
    currentEquip = nil

    UIMgr:RemoveListener('UpdateBagItems',inlayGemObserver)
end

--打开进阶面板
function ForgePanel:OpenAdvancePanel()
    UIMgr:SetActive(self.Img_advancePanel.transform,true)
    self:UpdateAdvanceInfo()
    UIMgr:AddListener('UpdateBagItems',advanceObserver)
end

--关闭进阶面板
function ForgePanel:CloseAdvancePanel()

    PanelMgr:ScrollRectRemoveListener(self.Rect_A_equipView)
    AdvanceEquipOpimizeSV:Clear()

    UIMgr:SetActive(self.Img_advanceEquipInfo.transform,false)
    UIMgr:SetActive(self.Img_advanceBtn.transform,false)
    UIMgr:SetActive(self.Img_advancePanel.transform,false)
    currentEquip = nil

    UIMgr:RemoveListener('UpdateBagItems',advanceObserver)
end

--刷新装备制作面板
function ForgePanel:UpdateMakeInfo()

    PanelMgr:ScrollRectRemoveListener(self.Rect_makeView)  --移除制造书优化滚动视图滑动监听
    MakeBookOptimizeSV:Clear()  --清空所有制造书格子

    local items = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Item)  --获取背包中的所有物品
    if items == nil then
        return;
    end

    local books = {}
    for index = 1, #items do
        if ItemConfig[items[index]:GetItemId()]._type == 19 then  --判断物品类型是否为制造书，是，则加入制造书表
            table.insert(books,items[index])
        end
    end
    
    if #books == 0 then
        return;
    end

    --给制造书滚动视图赋值
    MakeBookOptimizeSV:VerticalLayout(self.Rect_makeView.transform:GetComponent('RectTransform'),self.Img_makeContent.transform:
        GetComponent('RectTransform'),260,120,#books,10,10,20,0,5,ViewEnum.TopToBottom,CornerEnum.UpperLeft,'MakeBook',
        function (index,makeBook)  --当格子显示时调用的函数
            local composeId = ItemConfig[books[index+1]:GetItemId()].composeId  --获取制造书对应的装备id
            ResMgr:LoadAssetSprite('itemicon',{EquipConfig[composeId].icon},function (icon)
                makeBook.transform:Find('Auto_icon'):GetComponent('Image').sprite = icon  --加载装备图标并赋值
            end)
            makeBook.transform:Find('Auto_name'):GetComponent('Text').text = EquipConfig[composeId].name  --更新装备名称
            --设置单选框
            makeBook.transform:GetComponent('Toggle').group = self.Img_makeContent.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(makeBook.transform:GetComponent('Toggle'),function (isChoose)  --当玩家选择该制造书
                AudioMgr:PlayEffect('toggle')
                UIMgr:ChangeTogImg(isChoose,makeBook.transform:GetComponent('Image'),'chooseState','normalState')  --更新按钮图标
                if currentMakeBook == books[index+1]  then  --让玩家不可重复选择同一个，导致频繁刷新
                    return
                end
                --更新当前制造书
                currentMakeBook = books[index+1]
                self:UpdateMakeEquip()  --更新当前制造书对应得装备得具体信息
                self:UpdateMaterial()  --更新材料
            end)
        end,
        function (index,makeBook)  --当格子隐藏时显示的函数
            UIMgr:ChangeTogImg(false,makeBook:GetComponent('Image'),'chooseState','normalState')  --更新按钮图标
            PanelMgr:ToggleRemoveListener(makeBook.transform:GetComponent('Toggle'))  --移除按钮监听
        end
    )

    PanelMgr:ScrollRectAddListener(self.Rect_makeView,function (point)  --给制造书滚动视图添加滑动监听
        MakeBookOptimizeSV:Update()  --更新制造书格子
    end)
    --[[
    for index = 1, #books do
        PoolMgr:Get('MakeBook',function (makeBook)
            makeBook.transform:SetParent(self.Img_makeContent.transform)
            local composeId = ItemConfig[books[index]:GetItemId()].composeId
            ResMgr:LoadAssetSprite('itemicon',{EquipConfig[composeId].icon},function (icon)
                makeBook.transform:Find('Auto_icon'):GetComponent('Image').sprite = icon
            end);
            
            makeBook.transform:Find('Auto_name'):GetComponent('Text').text = EquipConfig[composeId].name
            makeBook.transform:GetComponent('Toggle').group = self.Img_makeContent.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(makeBook.transform:GetComponent('Toggle'),function (isChoose)
                self:ChangeImg(isChoose,makeBook.transform:GetComponent('Image'),'chooseState','normalState')
                if currentMakeBook == books[index]  then
                    return
                end
                currentMakeBook = books[index]
                self:UpdateMakeEquip()
                self:UpdateMaterial()
            end)
        end)
    end
    --]]
end

--刷新制造的装备具体信息
function ForgePanel:UpdateMakeEquip()

    UIMgr:SetActive(self.Img_makeEquipInfo.transform,true)  --激活制造的装备具体信息

    local composeId = ItemConfig[currentMakeBook:GetItemId()].composeId  --获取该制造书对应的装备id
    ResMgr:LoadAssetSprite('itemicon',{EquipConfig[composeId].icon},function (icon)
        self.Img_M_icon.sprite = icon  --加载该装备图标并赋值
    end);
    self.Text_M_name.text = EquipConfig[composeId].name  --更新该装备名称
    local roleData = RoleInfoMgr:GetMainRole();  --获取角色当前信息
    local level = EquipConfig[composeId].level  --装备所需等级
    if roleData:GetLevel() < level then  --判断角色等级是否小于装备需求等级
        self.Text_M_level.color = Color.red  --设置等级文本为红色
    else
        self.Text_M_level.color = Color.green  --设置等级文本为绿色
    end
    self.Text_M_level.text = '等级:'..tostring(level)  --更新等级文本
    
    if EquipConfig[composeId].profession == -1 then  --判断装备是否有需求职业
        UIMgr:SetActive(self.Text_M_profession.transform,false)  --隐藏职业文本
    else
        UIMgr:SetActive(self.Text_M_profession.transform,true)  --显示职业文本
        self.Text_M_profession.text = '职业:'..ProfessionConfig[EquipConfig[composeId].profession].name  --刷新职业文本
        if roleData:GetProfession() ~= EquipConfig[composeId].profession then  --判断自身职业是否与装备相符
            self.Text_M_profession.color = Color.red  --将文本设置为红色
        else
            self.Text_M_profession.color = Color.green  --将文本设置为绿色
        end
    end
    self.Text_M_type.text = '类型:'..EquipConfig[composeId].typeInfo  --更新装备类型

    if EquipConfig[composeId].firstAttr ~= nil then  --判断装备第一条属性是否存在，若存在，则进行赋值
        UIMgr:SetActive(self.Text_M_firstAttr.transform,true)
        self.Text_M_firstAttr.text = AttrConfig[EquipConfig[composeId].firstAttr].name..'+'
        ..tostring(EquipConfig[composeId].firstAttrValue)
    else
        UIMgr:SetActive(self.Text_M_firstAttr.transform,false)
    end

    if EquipConfig[composeId].secondAttr ~= nil then  --判断装备第二条属性是否存在，若存在，则进行赋值
        UIMgr:SetActive(self.Text_M_secondAttr.transform,true)
        self.Text_M_secondAttr.text = AttrConfig[EquipConfig[composeId].secondAttr].name..'+'
        ..tostring(EquipConfig[composeId].secondAttrValue)
    else
        UIMgr:SetActive(self.Text_M_secondAttr.transform,false)
    end

    if EquipConfig[composeId].thirdAttr ~= nil then  --判断装备第三条属性是否存在，若存在，则进行赋值
        UIMgr:SetActive(self.Text_M_thirdAttr.transform,true)
        self.Text_M_thirdAttr.text = AttrConfig[EquipConfig[composeId].thirdAttr].name..'+'
        ..tostring(EquipConfig[composeId].thirdAttrValue)
    else
        UIMgr:SetActive(self.Text_M_thirdAttr.transform,false)
    end

end

--清空材料
function ForgePanel:ClearMaterial()
    if #materials == 0 then
        return
    end
    for index = 1,#materials do
        PoolMgr:Set(materials[index].name,materials[index].gameObject)
    end
    materials = {}
end

--刷新材料
function ForgePanel:UpdateMaterial()

    UIMgr:SetActive(self.Img_materialContent.transform,true)  --激活材料父物体

    self:ClearMaterial()  --清空材料预制体

    local formulaId = ItemConfig[currentMakeBook:GetItemId()].formulaId  --获取该装备配方id

    --获取该配方所需要的材料id
    local materialOneId = FormulaConfig[formulaId].materialOne
    local materialTwoId = FormulaConfig[formulaId].materialTwo
    local materialThreeId = FormulaConfig[formulaId].materialThree
    local materialFourId = FormulaConfig[formulaId].materialFour

    local items = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Item)  --获取背包中的所有物品
    if items == nil then
        return;
    end

    local makeBooks = {}
    local materialOnes = {}
    local materialTwos = {}
    local materialThrees = {}
    local materialFours = {}

    for index = 1, #items do
        if items[index]:GetItemId() == currentMakeBook:GetItemId() then  --判断该物品是否为所需制造书，是，则加入制造书表
            table.insert(makeBooks,items[index])  
        end
        if items[index]:GetItemId() == materialOneId then  --判断该物品是否为所需材料之一，是，则加入对应的材料表
            table.insert(materialOnes,items[index])
        end
        if items[index]:GetItemId() == materialTwoId then  --判断该物品是否为所需材料之一，是，则加入对应的材料表
            table.insert(materialTwos,items[index])
        end
        if items[index]:GetItemId() == materialThreeId then  --判断该物品是否为所需材料之一，是，则加入对应的材料表
            table.insert(materialThrees,items[index])
        end
        if items[index]:GetItemId() == materialFourId then  --判断该物品是否为所需材料之一，是，则加入对应的材料表
            table.insert(materialFours,items[index])
        end
    end

    --加载材料
    self:LoadMaterial(currentMakeBook:GetItemId(),makeBooks)
    self:LoadMaterial(materialOneId,materialOnes)
    self:LoadMaterial(materialTwoId,materialTwos)
    self:LoadMaterial(materialThreeId,materialThrees)
    self:LoadMaterial(materialFourId,materialFours)

    if #materialOnes > 0 and #materialTwos > 0 and #materialThrees > 0 and #materialFours > 0 then  --判断玩家是否满足所需材料
        UIMgr:SetActive(self.Btn_makeBtn.transform,true)  --显示制造按钮
        UIMgr:SetActive(self.Text_Tips.transform,false)  --隐藏提示
    else
        UIMgr:SetActive(self.Btn_makeBtn.transform,false)  --隐藏制造按钮
        UIMgr:SetActive(self.Text_Tips.transform,true)  --显示材料不足提示
    end

end

--加载材料预制体并赋值
function ForgePanel:LoadMaterial(materialId,materialsInfo)
    PoolMgr:Get('Material',function (material)  --加载材料预制体
        table.insert(materials,material)  --存进材料对象表
        material.transform:SetParent(self.Img_materialContent.transform)  --设置父物体
        ResMgr:LoadAssetSprite('itemicon',{ItemConfig[materialId].icon},function (icon)
            material.transform:Find('Auto_icon'):GetComponent('Image').sprite = icon  --加载材料图标并赋值
        end);
        material.transform:Find('Auto_name'):GetComponent('Text').text = ItemConfig[materialId].name  --更新材料名称
        material.transform:Find('Auto_info'):GetComponent('Text').text = ItemConfig[materialId].info  --更新材料信息
        
        if materialsInfo == nil or #materialsInfo == 0 then  --判断该材料是否存在
            material.transform:Find('Auto_count'):GetComponent('Text').color = Color.red  --将数量文本设置为红色
            material.transform:Find('Auto_count'):GetComponent('Text').text = '0/1'  --更新数量
        else
            local count = 0
            for index = 1, #materialsInfo do
                count = materialsInfo[index]:GetItemCount() + count  --算出材料总数
            end
            material.transform:Find('Auto_count'):GetComponent('Text').color = Color.green  --将数量文本设置为绿色
            material.transform:Find('Auto_count'):GetComponent('Text').text = tostring(count)..'/1'  --更新数量
        end
    end)
end

--刷新宝石镶嵌面板基本信息
function ForgePanel:UpdateInlayInfo()

    PanelMgr:ScrollRectRemoveListener(self.Rect_equipView)  --移除宝石面板装备优化滚动视图滑动监听
    InalyEquipOptimizeSV:Clear()  --清空所有镶嵌面板的装备格子

    UIMgr:SetActive(self.Btn_inlayBtn.transform,false)  --激活镶嵌按钮
    UIMgr:SetActive(self.Btn_removeBtn.transform,false)  --激活移除按钮

    currentGemHole = nil  --重置当前选中的宝石孔
    currentGemId = nil  --重置当前选中的宝石
    currentEquip = nil  --重置当前选中的装备信息

    local allEquips = {}
    local equips = InventoryMgr:GetTypeItems(InventoryEnum.Equip,ItemClass.Equip)  --获取装备栏的所有装备(即已穿戴的装备)
    if equips ~= nil then
        for index = 1,#equips do
            table.insert(allEquips,equips[index])
        end
        --self:LoadEquip(equips,self.Img_equipContent.transform,InventoryEnum.Equip,function ()
            --self:UpdateInlayEquip()
        --end)
    end

    local bagEquips = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Equip)  --获取背包的所有装备
    if bagEquips ~= nil then
        for index = 1,#bagEquips do
            table.insert(allEquips,bagEquips[index])
        end
        --self:LoadEquip(bagEquips,self.Img_equipContent.transform,InventoryEnum.Bag,function ()
            --self:UpdateInlayEquip()
        --end)
    end

    if #allEquips == 0 then
        return
    end

    --给宝石面板装备滚动视图赋值
    InalyEquipOptimizeSV:VerticalLayout(self.Rect_equipView.transform:GetComponent('RectTransform'),self.Img_equipContent.transform:
        GetComponent('RectTransform'),260,120,#allEquips,10,10,20,0,5,ViewEnum.TopToBottom,CornerEnum.UpperLeft,'Equip',
        function (index,equip)  --当格子显示时调用的函数
            self:ShowEquipBox(equip,allEquips[index+1])
            equip.transform:GetComponent('Toggle').group = self.Img_equipContent.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(equip.transform:GetComponent('Toggle'),function (isChoose)
                AudioMgr:PlayEffect('toggle')
                currentEquip = allEquips[index+1]
                UIMgr:ChangeTogImg(isChoose,equip.transform:GetComponent('Image'),'chooseState','normalState')
                self:UpdateInlayEquip()
            end)
        end,
        function (index,equip)  --当格子隐藏时调用的函数
            UIMgr:ChangeTogImg(false,equip:GetComponent('Image'),'chooseState','normalState')
            PanelMgr:ToggleRemoveListener(equip.transform:GetComponent('Toggle'))
        end
    )

    PanelMgr:ScrollRectAddListener(self.Rect_equipView,function (point)  --给镶嵌面板装备滚动视图添加滑动监听
        InalyEquipOptimizeSV:Update()  --刷新装备格子
    end)

end

--显示装备格子信息
function ForgePanel:ShowEquipBox(equip,equipInfo)
    if equipInfo:GetItemInventory() == InventoryEnum.Bag then  --判断该装备是否存在于背包
        UIMgr:SetActive(equip.transform:Find('Auto_dress'),false)  --隐藏已装备图标
    else
        UIMgr:SetActive(equip.transform:Find('Auto_dress'),true)  --显示已装备图标
    end
    ResMgr:LoadAssetSprite('itembg',{EquipConfig[equipInfo:GetItemId()].color},function (icon)
        equip.transform:Find('Auto_equipBg'):GetComponent('Image').sprite = icon  --加载装备背景图标并赋值
    end);
    ResMgr:LoadAssetSprite('itemicon',{EquipConfig[equipInfo:GetItemId()].icon},function (icon)
        equip.transform:Find('Auto_equipIcon'):GetComponent('Image').sprite = icon  --加载装备图标并赋值
    end);
    equip.transform:Find('Auto_name'):GetComponent('Text').text =   --更新装备名称
    '<color='..EquipConfig[equipInfo:GetItemId()].color..'>'..EquipConfig[equipInfo:GetItemId()].name..'</color>'
end

--刷新镶嵌面板的装备具体信息
function ForgePanel:UpdateInlayEquip()

    --激活相关组件
    UIMgr:SetActive(self.Img_inlayEquipInfo.transform,true)
    UIMgr:SetActive(self.Btn_inlayBtn.transform,true)
    UIMgr:SetActive(self.Btn_removeBtn.transform,true)

    ResMgr:LoadAssetSprite('itembg',{EquipConfig[currentEquip:GetItemId()].color},function (icon)
        self.Img_I_equipBg.sprite = icon  --加载装备背景并赋值
    end);
    ResMgr:LoadAssetSprite('itemicon',{EquipConfig[currentEquip:GetItemId()].icon},function (icon)
        self.Img_I_equipIcon.sprite = icon  --加载装备图标并赋值
    end);

    self.Text_I_name.text =   --给装备名字赋值
    '<color='..EquipConfig[currentEquip:GetItemId()].color..'>'..EquipConfig[currentEquip:GetItemId()].name..'</color>'

    local roleData = RoleInfoMgr:GetMainRole();  --获取角色当前信息
    local level = EquipConfig[currentEquip:GetItemId()].level  --装备需求等级
    if roleData:GetLevel() < level then  --判断角色等级是否小于装备需求等级
        self.Text_I_level.color = Color.red  --将等级文本设置为红色
    else
        self.Text_I_level.color = Color.green  --将等级文本设置为绿色
    end
    self.Text_I_level.text = '等级:'..tostring(level)  --给等级文本赋值

    if EquipConfig[currentEquip:GetItemId()].profession == -1 then  --判断装备是否有职业需求
        UIMgr:SetActive(self.Text_I_profession.transform,false)  --隐藏职业文本
    else
        UIMgr:SetActive(self.Text_I_profession.transform,true)  --显示职业文本
        --更新职业文本
        self.Text_I_profession.text = '职业:'..ProfessionConfig[EquipConfig[currentEquip:GetItemId()].profession].name
        if roleData:GetProfession() ~= EquipConfig[currentEquip:GetItemId()].profession then  --判断自身职业是否与装备相符
            self.Text_I_profession.color = Color.red  --将文本设置为红色
        else
            self.Text_I_profession.color = Color.green  --将文本设置为绿色
        end
    end
    self.Text_I_type.text = '类型:'..EquipConfig[currentEquip:GetItemId()].typeInfo  --更新装备类型

    if EquipConfig[currentEquip:GetItemId()].firstAttr ~= nil then  --判断装备第一条属性是否存在，若有，则赋值
        UIMgr:SetActive(self.Text_I_firstAttr.transform,true)
        self.Text_I_firstAttr.text = AttrConfig[EquipConfig[currentEquip:GetItemId()].firstAttr].name..'+'
        ..tostring(EquipConfig[currentEquip:GetItemId()].firstAttrValue)
    else
        UIMgr:SetActive(self.Text_I_firstAttr.transform,false)
    end

    if EquipConfig[currentEquip:GetItemId()].secondAttr ~= nil then  --判断装备第二条属性是否存在，若有，则赋值
        UIMgr:SetActive(self.Text_I_secondAttr.transform,true)
        self.Text_I_secondAttr.text = AttrConfig[EquipConfig[currentEquip:GetItemId()].secondAttr].name..'+'
        ..tostring(EquipConfig[currentEquip:GetItemId()].secondAttrValue)
    else
        UIMgr:SetActive(self.Text_I_secondAttr.transform,false)
    end

    if EquipConfig[currentEquip:GetItemId()].thirdAttr ~= nil then  --判断装备第三条属性是否存在，若有，则赋值
        UIMgr:SetActive(self.Text_I_thirdAttr.transform,true)
        self.Text_I_thirdAttr.text = AttrConfig[EquipConfig[currentEquip:GetItemId()].thirdAttr].name..'+'
        ..tostring(EquipConfig[currentEquip:GetItemId()].thirdAttrValue)
    else
        UIMgr:SetActive(self.Text_I_thirdAttr.transform,false)
    end

    if currentEquip:GetGems()[0] == 0 then  --判断装备第一个孔是否存在宝石
        ResMgr:LoadAssetSprite('othericon',{'hole'},function (icon)
            self.Img_firstGem.sprite = icon  --加载宝石孔图标并赋值
        end);
        self.Text_firstGemAttr.text = '镶嵌宝石'  --更新提示文本
    else
        local gemId = currentEquip:GetGems()[0]
        ResMgr:LoadAssetSprite('itemicon',{GemConfig[gemId].icon},function (icon)
            self.Img_firstGem.sprite = icon  --加载宝石图标并赋值
        end);
        self.Text_firstGemAttr.text =   --更新宝石属性
        '<color='..GemConfig[gemId].color..'>'..AttrConfig[GemConfig[gemId].attr].name..'+'..GemConfig[gemId].attrValue..'</color>'
    end

    if currentEquip:GetGems()[1] == 0 then  --显示逻辑同上
        ResMgr:LoadAssetSprite('othericon',{'hole'},function (icon)
            self.Img_secondGem.sprite = icon
        end);
        self.Text_secondGemAttr.text = '镶嵌宝石'
    else
        local gemId = currentEquip:GetGems()[1]
        ResMgr:LoadAssetSprite('itemicon',{GemConfig[gemId].icon},function (icon)
            self.Img_secondGem.sprite = icon
        end);
        self.Text_secondGemAttr.text = 
        '<color='..GemConfig[gemId].color..'>'..AttrConfig[GemConfig[gemId].attr].name..'+'..GemConfig[gemId].attrValue..'</color>'
    end

    --重置当前选择宝石孔和宝石信息
    currentGemHole = nil
    currentGemId = nil
    --初始化两个宝石孔图标
    UIMgr:ChangeTogImg(false,self.Img_firstGemTog,'gemChoose','gemNormal')
    UIMgr:ChangeTogImg(false,self.Img_secondGemTog,'gemChoose','gemNormal')

    PanelMgr:ToggleRemoveListener(self.Tog_firstGemTog)  --移除宝石孔监听

    PanelMgr:ToggleAddListener(self.Tog_firstGemTog, function (isChoose)
        UIMgr:ChangeTogImg(isChoose,self.Img_firstGemTog,'gemChoose','gemNormal')  --更新图标信息
        currentGemHole = 0  --将选择宝石孔设置为0，对应c#中的第一个索引
        print('当前选中的宝石位是:'..tostring(currentGemHole))
        if currentEquip:GetGems()[0] ~= 0 then  --判断装备的第一个孔是否已存在宝石
            local gemId = currentEquip:GetGems()[0]
            currentGemId = gemId  --更新当前选中的宝石
            print('当前选中的宝石的Id是:'..tostring(currentGemId))
        else
            currentGemId = nil
        end
    end)

    PanelMgr:ToggleRemoveListener(self.Tog_secondGemTog)  --表现逻辑同上

    PanelMgr:ToggleAddListener(self.Tog_secondGemTog, function (isChoose)
        UIMgr:ChangeTogImg(isChoose,self.Img_secondGemTog,'gemChoose','gemNormal')
        currentGemHole = 1
        print('当前选中的宝石位是:'..tostring(currentGemHole))
        if currentEquip:GetGems()[1] ~= 0 then
            local gemId = currentEquip:GetGems()[1]
            currentGemId = gemId
            print('当前选中的宝石的Id是:'..tostring(currentGemId))
        else
            currentGemId = nil
        end
    end)

end

--刷新宝石
function ForgePanel:UpdateGem()

    --self:ClearGem()
    PanelMgr:ScrollRectRemoveListener(self.Rect_gemView)  --移除宝石滚动视图滑动监听
    GemOptimizeSV:Clear()  --清空宝石格子

    local gems = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Gem)  --获取背包中的所有宝石
    if gems == nil then
        return
    end
    
    --给宝石优化滚动视图赋值
    GemOptimizeSV:GridLayout(self.Rect_gemView.transform:GetComponent('RectTransform'),self.Img_gemContent.transform:
        GetComponent('RectTransform'),65,65,#gems,5,5,5,0,7,7,ViewEnum.TopToBottom,CornerEnum.UpperLeft,'Gem',
        function (index,gem)  --当格子显示时调用的函数
            ResMgr:LoadAssetSprite('itembg',{GemConfig[gems[index+1]:GetItemId()].color},function (icon)
                gem.transform:Find('Auto_gemBg'):GetComponent('Image').sprite = icon
            end);  --加载宝石背景图并赋值
            ResMgr:LoadAssetSprite('itemicon',{GemConfig[gems[index+1]:GetItemId()].icon},function (icon)
                gem.transform:Find('Auto_gemIcon'):GetComponent('Image').sprite = icon
            end);  --加载宝石图标并赋值
            gem.transform:Find('Auto_count'):GetComponent('Text').text = gems[index+1]:GetItemCount()  --更新宝石数
            PanelMgr:ButtonAddListener(gem.transform:GetComponent('Button'),function ()  --当点击宝石时
                AudioMgr:PlayEffect('toggle')
                if currentGemHole == nil then  --判断是否选择了宝石孔，若无，直接跳出
                    print('未选择宝石孔')
                    return
                end
                if currentGemId ~= nil then  --判断该宝石孔是否已存在宝石，若有，则直接跳出
                    print('该位置已有宝石')
                    return
                end
                currentChooseGem = gems[index+1]  --更新当前宝石信息
                ResMgr:LoadAssetSprite('itemicon',{GemConfig[gems[index+1]:GetItemId()].icon},function (icon)
                    --加载宝石图标并给对应的宝石孔赋值
                    if currentGemHole == 0 then
                        self.Img_firstGem.sprite = icon
                        if currentEquip:GetGems()[1] == 0 then
                            ResMgr:LoadAssetSprite('othericon',{'hole'},function (hole)
                                self.Img_secondGem.sprite = hole
                            end)
                        end
                    else
                        self.Img_secondGem.sprite = icon
                        if currentEquip:GetGems()[0] == 0 then
                            ResMgr:LoadAssetSprite('othericon',{'hole'},function (hole)
                                self.Img_firstGem.sprite = hole
                            end)
                        end
                    end
                end)
            end)
        end,
        function (index,gem)  --当格子隐藏时调用的函数
            PanelMgr:ButtonRemoveListener(gem.transform:GetComponent('Button'))  --移除监听
        end
    )

    PanelMgr:ScrollRectAddListener(self.Rect_gemView,function (point)  --给宝石滚动视图添加滑动监听
        GemOptimizeSV:Update()  --更新宝石格子
    end)

    --[[
    for index = 1,#gems do
        PoolMgr:Get('Gem',function (gem)
            gem.transform:SetParent(self.Img_gemContent.transform)
            ResMgr:LoadAssetSprite('itembg',{GemConfig[gems[index]:GetItemId()].color},function (icon)
                gem.transform:Find('Auto_gemBg'):GetComponent('Image').sprite = icon
            end);
            ResMgr:LoadAssetSprite('itemicon',{GemConfig[gems[index]:GetItemId()].icon},function (icon)
                gem.transform:Find('Auto_gemIcon'):GetComponent('Image').sprite = icon
            end);
            gem.transform:Find('Auto_count'):GetComponent('Text').text = gems[index]:GetItemCount()
            PanelMgr:ButtonAddListener(gem.transform:GetComponent('Button'),function ()
                if currentGemHole == -1 then
                    print('未选择宝石孔')
                    return
                end
                if currentGemId ~= -1 then
                    print('该位置已有宝石')
                    return
                end
                currentChooseGem = gems[index]
                ResMgr:LoadAssetSprite('itemicon',{GemConfig[gems[index]:GetItemId()].icon},function (icon)
                    if currentGemHole == 0 then
                        self.Img_firstGem.sprite = icon
                        if currentEquip:GetGems()[1] == 0 then
                            ResMgr:LoadAssetSprite('othericon',{'hole'},function (hole)
                                self.Img_secondGem.sprite = hole
                            end)
                        end
                    else
                        self.Img_secondGem.sprite = icon
                        if currentEquip:GetGems()[0] == 0 then
                            ResMgr:LoadAssetSprite('othericon',{'hole'},function (hole)
                                self.Img_firstGem.sprite = hole
                            end)
                        end
                    end
                end);

            end)
        end)
    end
    --]]

end

--刷新进阶面板基本信息
function ForgePanel:UpdateAdvanceInfo()

    --self:ClearEquip(self.Img_A_equipContent.transform)

    PanelMgr:ScrollRectRemoveListener(self.Rect_A_equipView)  --移除进阶面板装备格子滚动视图监听
    AdvanceEquipOpimizeSV:Clear()  --清空所有装备格子

    currentEquip = nil  --将当前选择装备置空

    local allEquips = {}

    local equips = InventoryMgr:GetTypeItems(InventoryEnum.Equip,ItemClass.Equip)  --获取装备栏中的所有装备(即已穿戴的装备)
    if equips ~= nil then
        for index = 1,#equips do
            table.insert(allEquips,equips[index])
        end
        --self:LoadEquip(equips,self.Img_A_equipContent.transform,InventoryEnum.Equip,function ()
            --self:UpdateAdvanceEquip()
        --end)
    end

    local bagEquips = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Equip)  --获取背包中的所有装备
    if bagEquips ~= nil then
        for index = 1,#bagEquips do
            table.insert(allEquips,bagEquips[index])
        end
        --self:LoadEquip(bagEquips,self.Img_A_equipContent.transform,InventoryEnum.Bag,function ()
            --self:UpdateAdvanceEquip()
        --end)
    end

    if #allEquips == 0 then
        return
    end

    --给优化滚动视图组件赋值
    AdvanceEquipOpimizeSV:VerticalLayout(self.Rect_A_equipView.transform:GetComponent('RectTransform'),self.Img_A_equipContent.
    transform:GetComponent('RectTransform'),260,120,#allEquips,10,10,20,0,5,ViewEnum.TopToBottom,CornerEnum.UpperLeft,'Equip',
        function (index,equip)  --当格子显示时调用的函数
            self:ShowEquipBox(equip,allEquips[index+1])  --给装备格子的显示赋值
            --设置单选框
            equip.transform:GetComponent('Toggle').group = self.Img_A_equipContent.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(equip.transform:GetComponent('Toggle'),function (isChoose)  --添加监听
                AudioMgr:PlayEffect('toggle')
                currentEquip = allEquips[index+1]  --更新当前选择装备
                UIMgr:ChangeTogImg(isChoose,equip.transform:GetComponent('Image'),'chooseState','normalState')  --更新格子图标
                self:UpdateAdvanceEquip()  --更新进阶装备具体信息
            end)
        end,
        function (index,equip)  --当格子隐藏时调用的函数
            UIMgr:ChangeTogImg(false,equip:GetComponent('Image'),'chooseState','normalState')  --还原格子图标
            PanelMgr:ToggleRemoveListener(equip.transform:GetComponent('Toggle'))  --移除监听
        end
    )

    PanelMgr:ScrollRectAddListener(self.Rect_A_equipView,function (point)  --给视图添加滑动监听
        AdvanceEquipOpimizeSV:Update()  --刷新格子
    end)

end

--刷新装备进阶的具体装备信息
function ForgePanel:UpdateAdvanceEquip()
    UIMgr:SetActive(self.Img_advanceEquipInfo.transform,true)  --激活装备信息父物体
    ResMgr:LoadAssetSprite('itembg',{EquipConfig[currentEquip:GetItemId()].color},function (icon)  --给物品背景添加图片
        self.Img_oldEquipBg.sprite = icon
    end);
    ResMgr:LoadAssetSprite('itemicon',{EquipConfig[currentEquip:GetItemId()].icon},function (icon)  --给物品图标赋值
        self.Img_oldEquipIcon.sprite = icon
    end);
    self.Text_oldName.text =   --物品名字
    '<color='..EquipConfig[currentEquip:GetItemId()].color..'>'..EquipConfig[currentEquip:GetItemId()].name..'</color>'
    local roleData = RoleInfoMgr:GetMainRole();  --当前角色信息
    local level = EquipConfig[currentEquip:GetItemId()].level  --装备需求等级
    if roleData:GetLevel() < level then  --判断角色等级是否小于装备需求等级
        self.Text_oldLevel.color = Color.red  --将字体设置为红色
    else
        self.Text_oldLevel.color = Color.green  --将字体设置为绿色
    end
    self.Text_oldLevel.text = '等级:'..tostring(level)  --给等级文本赋值

    if EquipConfig[currentEquip:GetItemId()].profession == -1 then  --判断装备是否有需求职业
        UIMgr:SetActive(self.Text_oldProfession.transform,false)  --隐藏职业文本
    else
        UIMgr:SetActive(self.Text_oldProfession.transform,true)  --激活职业文本
        --给职业文本赋值
        self.Text_oldProfession.text = '职业:'..ProfessionConfig[EquipConfig[currentEquip:GetItemId()].profession].name
        if roleData:GetProfession() ~= EquipConfig[currentEquip:GetItemId()].profession then  --判断自身职业是否与装备相符
            self.Text_oldProfession.color = Color.red  --将字体设置为红色
        else
            self.Text_oldProfession.color = Color.green  --将字体设置为绿色
        end
    end
    self.Text_oldType.text = '类型:'..EquipConfig[currentEquip:GetItemId()].typeInfo  --给装备类型赋值

    if EquipConfig[currentEquip:GetItemId()].firstAttr ~= nil then  --判断装备第一条属性是否存在，有则赋值，没有则隐藏
        UIMgr:SetActive(self.Text_oldFirstAttr.transform,true)
        self.Text_oldFirstAttr.text = AttrConfig[EquipConfig[currentEquip:GetItemId()].firstAttr].name..'+'
        ..tostring(EquipConfig[currentEquip:GetItemId()].firstAttrValue)
    else
        UIMgr:SetActive(self.Text_oldFirstAttr.transform,false)
    end

    if EquipConfig[currentEquip:GetItemId()].secondAttr ~= nil then  --判断装备第二条属性是否存在，有则赋值，没有则隐藏
        UIMgr:SetActive(self.Text_oldSecondAttr.transform,true)
        self.Text_oldSecondAttr.text = AttrConfig[EquipConfig[currentEquip:GetItemId()].secondAttr].name..'+'
        ..tostring(EquipConfig[currentEquip:GetItemId()].secondAttrValue)
    else
        UIMgr:SetActive(self.Text_oldSecondAttr.transform,false)
    end

    if EquipConfig[currentEquip:GetItemId()].thirdAttr ~= nil then  --判断装备第二条属性是否存在，有则赋值，没有则隐藏
        UIMgr:SetActive(self.Text_oldThirdAttr.transform,true)
        self.Text_oldThirdAttr.text = AttrConfig[EquipConfig[currentEquip:GetItemId()].thirdAttr].name..'+'
        ..tostring(EquipConfig[currentEquip:GetItemId()].thirdAttrValue)
    else
        UIMgr:SetActive(self.Text_oldThirdAttr.transform,false)
    end

    if EquipConfig[currentEquip:GetItemId()].quality > 4 then  --判断该装备的品质是否大于4，大于4则满级无法进阶
        UIMgr:SetActive(self.Img_newEquip.transform,false)
        UIMgr:SetActive(self.Img_advanceBtn.transform,false)
        UIMgr:SetActive(self.Text_money.transform,false)
        return  --直接跳出
    end

    --以下为显示进阶后的装备信息，显示逻辑同上
    UIMgr:SetActive(self.Img_newEquip.transform,true)
    local newEquipItemId = currentEquip:GetItemId()+1
    ResMgr:LoadAssetSprite('itembg',{EquipConfig[newEquipItemId].color},function (icon)
        self.Img_newEquipBg.sprite = icon
    end);
    ResMgr:LoadAssetSprite('itemicon',{EquipConfig[newEquipItemId].icon},function (icon)
        self.Img_newEquipIcon.sprite = icon
    end);
    self.Text_newName.text = 
    '<color='..EquipConfig[newEquipItemId].color..'>'..EquipConfig[newEquipItemId].name..'</color>'
    local roleData = RoleInfoMgr:GetMainRole();
    --装备需求等级
    local level = EquipConfig[newEquipItemId].level
    if roleData:GetLevel() < level then
        self.Text_newLevel.color = Color.red
    else
        self.Text_newLevel.color = Color.green
    end
    self.Text_newLevel.text = '等级:'..tostring(level)

    if EquipConfig[newEquipItemId].profession == -1 then
        UIMgr:SetActive(self.Text_newProfession.transform,false)
    else
        UIMgr:SetActive(self.Text_newProfession.transform,true)
        self.Text_newProfession.text = '职业:'..ProfessionConfig[EquipConfig[newEquipItemId].profession].name
        if roleData:GetProfession() ~= EquipConfig[newEquipItemId].profession then
            self.Text_newProfession.color = Color.red
        else
            self.Text_newProfession.color = Color.green
        end
    end

    self.Text_newType.text = '类型:'..EquipConfig[newEquipItemId].typeInfo

    if EquipConfig[newEquipItemId].firstAttr ~= nil then
        UIMgr:SetActive(self.Text_newFirstAttr.transform,true)
        self.Text_newFirstAttr.text = AttrConfig[EquipConfig[newEquipItemId].firstAttr].name..'+'
        ..tostring(EquipConfig[newEquipItemId].firstAttrValue)
    else
        UIMgr:SetActive(self.Text_newFirstAttr.transform,false)
    end

    if EquipConfig[newEquipItemId].secondAttr ~= nil then
        UIMgr:SetActive(self.Text_newSecondAttr.transform,true)
        self.Text_newSecondAttr.text = AttrConfig[EquipConfig[newEquipItemId].secondAttr].name..'+'
        ..tostring(EquipConfig[newEquipItemId].secondAttrValue)
    else
        UIMgr:SetActive(self.Text_newSecondAttr.transform,false)
    end

    if EquipConfig[newEquipItemId].thirdAttr ~= nil then
        UIMgr:SetActive(self.Text_newThirdAttr.transform,true)
        self.Text_newThirdAttr.text = AttrConfig[EquipConfig[newEquipItemId].thirdAttr].name..'+'
        ..tostring(EquipConfig[newEquipItemId].thirdAttrValue)
    else
        UIMgr:SetActive(self.Text_newThirdAttr.transform,false)
    end

    local haveGold = roleData.OtherData:GetGold()
    local needGold = EquipConfig[currentEquip:GetItemId()].advanceGold
    if haveGold < needGold then
        self.Text_money.color = Color.red
        UIMgr:SetActive(self.Text_money.transform,true)
    else
        self.Text_money.color = Color.green
        UIMgr:SetActive(self.Img_advanceBtn.transform,true)
        UIMgr:SetActive(self.Text_money.transform,true)
    end
    self.Text_money.text = needGold

end
