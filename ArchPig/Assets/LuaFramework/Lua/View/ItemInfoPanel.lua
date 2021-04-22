
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:物品信息面板视图层
*
*        description:
*            功能描述:实现物品信息面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


ItemInfoPanel = {}
local this = ItemInfoPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
ItemInfoPanel.Ctrls = 
{
    Img_mainItemInfo = {  Path = 'Auto_mainItemInfo',  ControlType = 'Image'  },
    Img_itemBg = {  Path = 'Auto_mainItemInfo/BaseDatas/Item/Auto_itemBg',  ControlType = 'Image'  },
    Img_itemImg = {  Path = 'Auto_mainItemInfo/BaseDatas/Item/Auto_itemImg',  ControlType = 'Image'  },
    Img_dress = {  Path = 'Auto_mainItemInfo/BaseDatas/Item/Auto_dress',  ControlType = 'Image'  },
    Img_itemInfo = {  Path = 'Auto_mainItemInfo/Auto_itemInfo',  ControlType = 'Image'  },
    Img_baseAttrs = {  Path = 'Auto_mainItemInfo/Auto_baseAttrs',  ControlType = 'Image'  },
    Img_gemAttrs = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs',  ControlType = 'Image'  },
    Img_firstGem = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_firstGem',  ControlType = 'Image'  },
    Img_fGemBg = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_firstGem/Auto_fGemBg',  ControlType = 'Image'  },
    Img_fGemImg = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_firstGem/Auto_fGemImg',  ControlType = 'Image'  },
    Img_secondGem = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_secondGem',  ControlType = 'Image'  },
    Img_sGemBg = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_secondGem/Auto_sGemBg',  ControlType = 'Image'  },
    Img_sGemImg = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_secondGem/Auto_sGemImg',  ControlType = 'Image'  },
    Img_itemBtn = {  Path = 'Auto_mainItemInfo/Auto_itemBtn',  ControlType = 'Image'  },
    Img_useBtn = {  Path = 'Auto_mainItemInfo/Auto_itemBtn/Auto_useBtn',  ControlType = 'Image'  },
    Img_discardItem = {  Path = 'Auto_mainItemInfo/Auto_itemBtn/Auto_discardItem',  ControlType = 'Image'  },
    Img_equipBtn = {  Path = 'Auto_mainItemInfo/Auto_equipBtn',  ControlType = 'Image'  },
    Img_dressBtn = {  Path = 'Auto_mainItemInfo/Auto_equipBtn/Auto_dressBtn',  ControlType = 'Image'  },
    Img_discardEquip = {  Path = 'Auto_mainItemInfo/Auto_equipBtn/Auto_discardEquip',  ControlType = 'Image'  },
    Img_equippedBtn = {  Path = 'Auto_mainItemInfo/Auto_equippedBtn',  ControlType = 'Image'  },
    Img_takeoffBtn = {  Path = 'Auto_mainItemInfo/Auto_equippedBtn/Auto_takeoffBtn',  ControlType = 'Image'  },
    Img_secondItemInfo = {  Path = 'Auto_secondItemInfo',  ControlType = 'Image'  },
    Img_S_itemBg = {  Path = 'Auto_secondItemInfo/baseDatas/Item/Auto_S_itemBg',  ControlType = 'Image'  },
    Img_S_itemImg = {  Path = 'Auto_secondItemInfo/baseDatas/Item/Auto_S_itemImg',  ControlType = 'Image'  },
    Img_S_dress = {  Path = 'Auto_secondItemInfo/baseDatas/Item/Auto_S_dress',  ControlType = 'Image'  },
    Img_S_itemInfo = {  Path = 'Auto_secondItemInfo/Auto_S_itemInfo',  ControlType = 'Image'  },
    Img_S_baseAttrs = {  Path = 'Auto_secondItemInfo/Auto_S_baseAttrs',  ControlType = 'Image'  },
    Img_S_gemAttrs = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs',  ControlType = 'Image'  },
    Img_S_firstGem = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_firstGem',  ControlType = 'Image'  },
    Img_S_fGemBg = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_firstGem/Auto_S_fGemBg',  ControlType = 'Image'  },
    Img_S_fGemImg = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_firstGem/Auto_S_fGemImg',  ControlType = 'Image'  },
    Img_S_secondGem = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_secondGem',  ControlType = 'Image'  },
    Img_S_sGemBg = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_secondGem/Auto_S_sGemBg',  ControlType = 'Image'  },
    Img_S_sGemImg = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_secondGem/Auto_S_sGemImg',  ControlType = 'Image'  },
    Img_S_equippedBtn = {  Path = 'Auto_secondItemInfo/Auto_S_equippedBtn',  ControlType = 'Image'  },
    Img_S_takeoffBtn = {  Path = 'Auto_secondItemInfo/Auto_S_equippedBtn/Auto_S_takeoffBtn',  ControlType = 'Image'  },
    Text_name = {  Path = 'Auto_mainItemInfo/BaseDatas/baseData/Auto_name',  ControlType = 'Text'  },
    Text_type = {  Path = 'Auto_mainItemInfo/BaseDatas/baseData/Auto_type',  ControlType = 'Text'  },
    Text_profession = {  Path = 'Auto_mainItemInfo/BaseDatas/baseData/Auto_profession',  ControlType = 'Text'  },
    Text_level = {  Path = 'Auto_mainItemInfo/BaseDatas/baseData/Auto_level',  ControlType = 'Text'  },
    Text_infoTitle = {  Path = 'Auto_mainItemInfo/Auto_itemInfo/Auto_infoTitle',  ControlType = 'Text'  },
    Text_itemInfos = {  Path = 'Auto_mainItemInfo/Auto_itemInfo/Auto_itemInfos',  ControlType = 'Text'  },
    Text_firstAttr = {  Path = 'Auto_mainItemInfo/Auto_baseAttrs/Auto_firstAttr',  ControlType = 'Text'  },
    Text_secondAttr = {  Path = 'Auto_mainItemInfo/Auto_baseAttrs/Auto_secondAttr',  ControlType = 'Text'  },
    Text_thirdAttr = {  Path = 'Auto_mainItemInfo/Auto_baseAttrs/Auto_thirdAttr',  ControlType = 'Text'  },
    Text_fGemAttr = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_firstGem/Auto_fGemAttr',  ControlType = 'Text'  },
    Text_sGemAttr = {  Path = 'Auto_mainItemInfo/Auto_gemAttrs/Auto_secondGem/Auto_sGemAttr',  ControlType = 'Text'  },
    Text_S_name = {  Path = 'Auto_secondItemInfo/baseDatas/Auto_S_baseDatas/Auto_S_name',  ControlType = 'Text'  },
    Text_S_type = {  Path = 'Auto_secondItemInfo/baseDatas/Auto_S_baseDatas/Auto_S_type',  ControlType = 'Text'  },
    Text_S_profession = {  Path = 'Auto_secondItemInfo/baseDatas/Auto_S_baseDatas/Auto_S_profession',  ControlType = 'Text'  },
    Text_S_level = {  Path = 'Auto_secondItemInfo/baseDatas/Auto_S_baseDatas/Auto_S_level',  ControlType = 'Text'  },
    Text_S_itemInfos = {  Path = 'Auto_secondItemInfo/Auto_S_itemInfo/Auto_S_itemInfos',  ControlType = 'Text'  },
    Text_S_firstAttr = {  Path = 'Auto_secondItemInfo/Auto_S_baseAttrs/Auto_S_firstAttr',  ControlType = 'Text'  },
    Text_S_secondAttr = {  Path = 'Auto_secondItemInfo/Auto_S_baseAttrs/Auto_S_secondAttr',  ControlType = 'Text'  },
    Text_S_thirdAttr = {  Path = 'Auto_secondItemInfo/Auto_S_baseAttrs/Auto_S_thirdAttr',  ControlType = 'Text'  },
    Text_S_fGemAttr = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_firstGem/Auto_S_fGemAttr',  ControlType = 'Text'  },
    Text_S_sGemAttr = {  Path = 'Auto_secondItemInfo/Auto_S_gemAttrs/Auto_S_secondGem/Auto_S_sGemAttr',  ControlType = 'Text'  },
    Btn_useBtn = {  Path = 'Auto_mainItemInfo/Auto_itemBtn/Auto_useBtn',  ControlType = 'Button'  },
    Btn_discardItem = {  Path = 'Auto_mainItemInfo/Auto_itemBtn/Auto_discardItem',  ControlType = 'Button'  },
    Btn_dressBtn = {  Path = 'Auto_mainItemInfo/Auto_equipBtn/Auto_dressBtn',  ControlType = 'Button'  },
    Btn_discardEquip = {  Path = 'Auto_mainItemInfo/Auto_equipBtn/Auto_discardEquip',  ControlType = 'Button'  },
    Btn_takeoffBtn = {  Path = 'Auto_mainItemInfo/Auto_equippedBtn/Auto_takeoffBtn',  ControlType = 'Button'  },
    Btn_S_takeoffBtn = {  Path = 'Auto_secondItemInfo/Auto_S_equippedBtn/Auto_S_takeoffBtn',  ControlType = 'Button'  },
}

local gameObject
local transform

local ProfessionConfig = require "Config/ProfessionConfig"
local ItemConfig = require "Config/ItemConfig"
local EquipConfig = require "Config/EquipConfig"
local GemConfig = require 'Config/GemConfig'
local AttrConfig = require "Config/AttrConfig"

local currentitemGO;
local currentItemInfo;
local currentEquip;

local showItemObserver  --显示物品信息监听观察者

function ItemInfoPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function ItemInfoPanel:Init()
    
    UIMgr:SetActive(self.Img_secondItemInfo.transform,false)

    showItemObserver = Observer:New(function (itemGO,itemInfo)
        self:ShowItem(itemGO,itemInfo)
    end)
    
end

function ItemInfoPanel:Show()

    UIMgr:AddListener('ShowItemInfo',showItemObserver)
    --[[
    UIMgr:AddListener("ShowItemInfo",function (itemGO,itemInfo)
        self:ShowItem(itemGO,itemInfo)
    end)
    --]]
end

function ItemInfoPanel:Hide()
    UIMgr:RemoveListener("ShowItemInfo",showItemObserver)
    PanelMgr:ButtonRemoveListener(self.Btn_useBtn)
    PanelMgr:ButtonRemoveListener(self.Btn_discardItem)
    PanelMgr:ButtonRemoveListener(self.Btn_dressBtn)
    PanelMgr:ButtonRemoveListener(self.Btn_discardEquip)
    PanelMgr:ButtonRemoveListener(self.Btn_takeoffBtn)
    PanelMgr:ButtonRemoveListener(self.Btn_S_takeoffBtn)
    UIMgr:SetActive(self.Img_secondItemInfo.transform,false)
    currentitemGO = nil
    currentItemInfo = nil
    currentEquip = nil
end

local BagSwitch = {
    --ItemClass.Item
    [ItemClass.Item] = function ()
        this:ShowItemInfo(currentItemInfo)
        --只显示物品可使用的按钮
        UIMgr:SetActive(this.Img_itemBtn.transform,true)
        UIMgr:SetActive(this.Img_equipBtn.transform,false)
        UIMgr:SetActive(this.Img_equippedBtn.transform,false)
        PanelMgr:ButtonAddListener(this.Btn_useBtn,function ()
            AudioMgr:PlayEffect('button')
            --print('使用'..ItemConfig[currentItemInfo:GetItemId()].name)
            ItemInfoCtrl:UseItem(currentItemInfo:GetGuid())
            PanelMgr:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_discardItem,function ()
            AudioMgr:PlayEffect('button')
            --print('丢弃'..ItemConfig[currentItemInfo:GetItemId()].name)
            local name = ItemConfig[currentItemInfo:GetItemId()].name
            local Guid = currentItemInfo:GetGuid()
            PanelMgr:ClosePanel()
            UIMgr:Trigger("Message",MessageEnum.Throw,'确认要丢弃'..name..'吗?',Guid)
            
            --ItemInfoCtrl:DiscardItem(currentItemInfo:GetGuid())
        end)
    end,
    --ItemClass.Equip
    [ItemClass.Equip] = function ()
		UIMgr:SetActive(this.Img_itemBtn.transform,false)
		UIMgr:SetActive(this.Img_equipBtn.transform,true)
        UIMgr:SetActive(this.Img_equippedBtn.transform,false)
        PanelMgr:ButtonAddListener(this.Btn_dressBtn,function ()
            AudioMgr:PlayEffect('button')
            --print("装备"..EquipConfig[currentItemInfo:GetItemId()].name)
            ItemInfoCtrl:DressEquip(currentItemInfo:GetGuid())
            PanelMgr:ClosePanel()
        end)
        PanelMgr:ButtonAddListener(this.Btn_discardEquip,function ()
            AudioMgr:PlayEffect('button')
            --print('丢弃'..EquipConfig[currentItemInfo:GetItemId()].name)
            local name = ItemConfig[currentItemInfo:GetItemId()].name
            local Guid = currentItemInfo:GetGuid()
            PanelMgr:ClosePanel()
            UIMgr:Trigger("Message",MessageEnum.Throw,'确认要丢弃'..name..'吗?',Guid)
        end)
        this:ShowEquipInfo(currentItemInfo)
        --获取装备栏中的所有装备
        local equips = InventoryMgr:GetInventoryItem(InventoryEnum.Equip)
        
        --判断角色是否已装备该部位的装备，有的话则显示该装备信息
        if equips == nil then
            return
        end
		for index = 1,#equips do 
            if EquipConfig[equips[index]:GetItemId()]._type == EquipConfig[currentItemInfo:GetItemId()]._type then
                --更新当前已装备的装备
				currentEquip = equips[index]
			end
		end
		if currentEquip == nil then
			return;
        end
        --显示已装备信息
        UIMgr:SetActive(this.Img_secondItemInfo.transform,true)
        this:ShowSecondEquipInfo(currentEquip)
        PanelMgr:ButtonAddListener(this.Btn_S_takeoffBtn,function ()
            AudioMgr:PlayEffect('button')
            --print('卸下'..EquipConfig[currentEquip:GetItemId()].name)
            ItemInfoCtrl:TakeoffEquip(currentEquip:GetGuid())
            PanelMgr:ClosePanel()
        end)
    end,
    --ItemClass.Gem
    [ItemClass.Gem] = function ()
        this:ShowGemInfo(currentItemInfo)
        --只显示物品可使用的按钮
		UIMgr:SetActive(this.Img_itemBtn.transform,true)
        UIMgr:SetActive(this.Img_equipBtn.transform,false)
        UIMgr:SetActive(this.Img_equippedBtn.transform,false)
        PanelMgr:ButtonAddListener(this.Btn_useBtn,function ()
            AudioMgr:PlayEffect('button')
            print('使用'..GemConfig[currentItemInfo:GetItemId()].name)
            --Client.Send('UseItem',C2S_UseItem.New(currentItemInfo:GetGuid()))
            PanelMgr:ClosePanel()
            PanelMgr:ClosePanel()
            PanelMgr:OpenPanel("ForgePanel",UILayer.Middle,MaskEnum.Mask)
        end)
        PanelMgr:ButtonAddListener(this.Btn_discardItem,function ()
            AudioMgr:PlayEffect('button')
            local name = ItemConfig[currentItemInfo:GetItemId()].name
            local Guid = currentItemInfo:GetGuid()
            PanelMgr:ClosePanel()
            UIMgr:Trigger("Message",MessageEnum.Throw,'确认要丢弃'..name..'吗?',Guid)
            --ItemInfoCtrl:DiscardItem(currentItemInfo:GetGuid())
        end)
    end
}

local SwitchInventory = {
    --InventoryEnum.Bag
    [InventoryEnum.Bag] = function ()
        BagSwitch[currentItemInfo:GetItemType()]()
    end,
    --InventoryEnum.Equip
    [InventoryEnum.Equip] = function ()
        this:ShowEquipInfo(currentItemInfo)
        --只显示已装备装备可使用的按钮
		UIMgr:SetActive(this.Img_itemBtn.transform,false)
		UIMgr:SetActive(this.Img_equipBtn.transform,false)
        UIMgr:SetActive(this.Img_equippedBtn.transform,true)
        PanelMgr:ButtonAddListener(this.Btn_takeoffBtn,function ()
            AudioMgr:PlayEffect('button')
            --print('卸下'..EquipConfig[currentItemInfo:GetItemId()].name)
            ItemInfoCtrl:TakeoffEquip(currentItemInfo:GetGuid())
            PanelMgr:ClosePanel()
        end)
    end,
    --InventoryEnum.Shop
    [InventoryEnum.Shop] = function ()
        UIMgr:SetActive(this.Img_itemBtn.transform,false)
		UIMgr:SetActive(this.Img_equipBtn.transform,false)
        UIMgr:SetActive(this.Img_equippedBtn.transform,false)
        if currentItemInfo:GetItemType() == ItemClass.Item then
            this:ShowItemInfo(currentItemInfo)
        end
        if currentItemInfo:GetItemType() == ItemClass.Equip then
            this:ShowEquipInfo(currentItemInfo)
        end
        if currentItemInfo:GetItemType() == ItemClass.Gem then
            this:ShowGemInfo(currentItemInfo)
        end
    end,
    --InventoryEnum.Auction
    [InventoryEnum.Auction] = function ()
        UIMgr:SetActive(this.Img_itemBtn.transform,false)
		UIMgr:SetActive(this.Img_equipBtn.transform,false)
        UIMgr:SetActive(this.Img_equippedBtn.transform,false)
        if currentItemInfo:GetItemType() == ItemClass.Item then
            this:ShowItemInfo(currentItemInfo)
        end
        if currentItemInfo:GetItemType() == ItemClass.Equip then
            this:ShowEquipInfo(currentItemInfo)
        end
        if currentItemInfo:GetItemType() == ItemClass.Gem then
            this:ShowGemInfo(currentItemInfo)
        end
    end,
    --InventoryEnum.OtherRoleEquip
    [InventoryEnum.OtherRoleEquip] = function ()
        UIMgr:SetActive(this.Img_itemBtn.transform,false)
		UIMgr:SetActive(this.Img_equipBtn.transform,false)
        UIMgr:SetActive(this.Img_equippedBtn.transform,false)
        if currentItemInfo:GetItemType() == ItemClass.Item then
            this:ShowItemInfo(currentItemInfo)
        end
        if currentItemInfo:GetItemType() == ItemClass.Equip then
            this:ShowEquipInfo(currentItemInfo)
        end
        if currentItemInfo:GetItemType() == ItemClass.Gem then
            this:ShowGemInfo(currentItemInfo)
        end
    end
}

function ItemInfoPanel:ShowItem(itemGO,itemInfo)
	currentitemGO = itemGO
	currentItemInfo = itemInfo
    transform.position = currentitemGO.transform.position
    
    SwitchInventory[currentItemInfo:GetItemInventory()]()
    --若当前物品存在于背包
    --[[
	if currentItemInfo:GetItemInventory() == InventoryEnum.Bag then
		--若当前物品是普通物品
        if currentItemInfo:GetItemType() == ItemClass.Item then
            --显示物品信息
            
            return
		end
		--若当前物品是装备
        if currentItemInfo:GetItemType() == ItemClass.Equip then
            --显示装备信息
			--UIMgr:SetActive(self.Img_mainItemInfo.transform,true)
			
			
        end
        --如果当前物品是宝石
        if currentItemInfo:GetItemType() == ItemClass.Gem then
            
        end
    end
    
	--若物品当前存在于装备栏
    if currentItemInfo:GetItemInventory() == InventoryEnum.Equip then
        --显示装备信息
        
    end
    
	if currentItemInfo:GetItemInventory() == InventoryEnum.Shop then
        
    end
    --]]
end

function ItemInfoPanel:ShowItemInfo(itemInfo)
    --更新物品背景框
	ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
		self.Img_itemBg.sprite = icon	
    end);
    --更新物品图标
	ResMgr:LoadAssetSprite('itemicon',{ItemConfig[itemInfo:GetItemId()].icon},function (icon)
		self.Img_itemImg.sprite = icon	
    end);
    --隐藏已装备图标
	UIMgr:SetActive(self.Img_dress.transform,false)
	--更新物品名称
    self.Text_name.text = ItemConfig[itemInfo:GetItemId()].name
    --更新物品类型
    self.Text_type.text = '类型:'..ItemConfig[itemInfo:GetItemId()].typeInfo
    --更新物品职业需求
	if ItemConfig[itemInfo:GetItemId()].profession ~= -1 then
		UIMgr:SetActive(self.Text_profession.transform,true)
		self.Text_profession.text = '职业:'..ProfessionConfig[ItemConfig[itemInfo:GetItemId()].profession].name
	else
		UIMgr:SetActive(self.Text_profession.transform,false)
    end
    --更新物品等级需求
	if ItemConfig[itemInfo:GetItemId()].level ~= -1 then
        UIMgr:SetActive(self.Text_level.transform,true)
        local roleData = RoleInfoMgr:GetMainRole();
        local level = ItemConfig[itemInfo:GetItemId()].level
        if roleData:GetLevel() < level then
            self.Text_level.color = Color.red
        else
            self.Text_level.color = Color.green
        end
		self.Text_level.text = '等级:'..tostring(level)
	else
		UIMgr:SetActive(self.Text_level.transform,false)
    end
    --更新物品详情
    UIMgr:SetActive(self.Img_itemInfo.transform,true)
	self.Text_itemInfos.text = ItemConfig[itemInfo:GetItemId()].info
	--隐藏装备属性
	UIMgr:SetActive(self.Img_baseAttrs.transform,false)
	UIMgr:SetActive(self.Img_gemAttrs.transform,false)
end

function ItemInfoPanel:ShowGemInfo(itemInfo)

    --更新物品背景框
	ResMgr:LoadAssetSprite('itembg',{GemConfig[itemInfo:GetItemId()].color},function (icon)
		self.Img_itemBg.sprite = icon	
    end);
    --更新物品图标
	ResMgr:LoadAssetSprite('itemicon',{GemConfig[itemInfo:GetItemId()].icon},function (icon)
		self.Img_itemImg.sprite = icon	
    end);
    --隐藏已装备图标
	UIMgr:SetActive(self.Img_dress.transform,false)
	--更新物品名称
    self.Text_name.text = '<color='..GemConfig[itemInfo:GetItemId()].color..'>'..GemConfig[itemInfo:GetItemId()].name..'</color>'
    --更新物品类型
    self.Text_type.text = '类型:宝石'
    --更新物品职业需求
	UIMgr:SetActive(self.Text_profession.transform,false)

    --更新物品等级需求
	UIMgr:SetActive(self.Text_level.transform,false)

    --更新物品详情
    UIMgr:SetActive(self.Img_itemInfo.transform,true)
	self.Text_itemInfos.text = GemConfig[itemInfo:GetItemId()].info
	--隐藏装备属性
	UIMgr:SetActive(self.Img_baseAttrs.transform,false)
	UIMgr:SetActive(self.Img_gemAttrs.transform,false)
end

function ItemInfoPanel:ShowEquipInfo(itemInfo)
    --更新装备背景框
	ResMgr:LoadAssetSprite('itembg',{EquipConfig[itemInfo:GetItemId()].color},function (icon)
		self.Img_itemBg.sprite = icon	
    end);
    --更新装备图标
    ResMgr:LoadAssetSprite('itemicon',{EquipConfig[itemInfo:GetItemId()].icon},function (icon)
		self.Img_itemImg.sprite = icon	
	end);
    --更新装备名称
	self.Text_name.text = '<color='..EquipConfig[itemInfo:GetItemId()].color..'>'..EquipConfig[itemInfo:GetItemId()].name..'</color>'
	--更新装备穿戴状态
	if itemInfo:GetItemInventory() ~= InventoryEnum.Equip then
		UIMgr:SetActive(self.Img_dress.transform,false)
	else
		UIMgr:SetActive(self.Img_dress.transform,true)
    end
    --更新装备类型
    self.Text_type.text = '类型:'..EquipConfig[itemInfo:GetItemId()].typeInfo
    
    --更新装备需求等级
    local roleData = RoleInfoMgr:GetMainRole();
    --更新装备需求职业
	if EquipConfig[itemInfo:GetItemId()].profession ~= -1 then
		UIMgr:SetActive(self.Text_profession.transform,true)
        self.Text_profession.text = '职业:'..ProfessionConfig[EquipConfig[itemInfo:GetItemId()].profession].name
        if roleData:GetProfession() == EquipConfig[itemInfo:GetItemId()].profession then
            self.Text_profession.color = Color.green
        else
            self.Text_profession.color = Color.red
        end
    else
        UIMgr:SetActive(self.Text_profession.transform,false)
    end
    local level = EquipConfig[itemInfo:GetItemId()].level
    if roleData:GetLevel() < level then
        self.Text_level.color = Color.red
    else
        self.Text_level.color = Color.green
    end
	UIMgr:SetActive(self.Text_level.transform,true)
    self.Text_level.text = '等级:'..tostring(level)
    --激活装备基础属性
	UIMgr:SetActive(self.Img_baseAttrs.transform,true)
	--更新装备第一条基础属性
	if EquipConfig[itemInfo:GetItemId()].firstAttr ~= -1 then
		UIMgr:SetActive(self.Text_firstAttr.transform,true)
		self.Text_firstAttr.text = AttrConfig[EquipConfig[itemInfo:GetItemId()].firstAttr].name..'+'..EquipConfig[itemInfo:GetItemId()].firstAttrValue
	else
		UIMgr:SetActive(self.Text_firstAttr.transform,false)
    end
    --更新装备第二条基本属性
	if EquipConfig[itemInfo:GetItemId()].secondAttr ~= -1 then
		UIMgr:SetActive(self.Text_secondAttr.transform,true)
		self.Text_secondAttr.text = AttrConfig[EquipConfig[itemInfo:GetItemId()].secondAttr].name..'+'..EquipConfig[itemInfo:GetItemId()].secondAttrValue
	else
        UIMgr:SetActive(self.Text_secondAttr.transform,false)
    end
    --更新装备第三条基本属性
	if EquipConfig[itemInfo:GetItemId()].thirdAttr ~= -1 then
		UIMgr:SetActive(self.Text_thirdAttr.transform,true)
		self.Text_thirdAttr.text = AttrConfig[EquipConfig[itemInfo:GetItemId()].thirdAttr].name..'+'..EquipConfig[itemInfo:GetItemId()].thirdAttrValue
	else
		UIMgr:SetActive(self.Text_thirdAttr.transform,false)
    end
    --隐藏物品详情和宝石属性
    UIMgr:SetActive(self.Img_itemInfo.transform,false)
    
    if itemInfo:GetGems()[0] == 0 and itemInfo:GetGems()[1] == 0 then
        UIMgr:SetActive(self.Img_gemAttrs.transform,false)
        return
    else
        UIMgr:SetActive(self.Img_gemAttrs.transform,true)
    end
    if itemInfo:GetGems()[0] == 0 then
        UIMgr:SetActive(self.Img_firstGem.transform,false)
    else
        UIMgr:SetActive(self.Img_firstGem.transform,true)
        ResMgr:LoadAssetSprite('itembg',{GemConfig[itemInfo:GetGems()[0]].color},function (icon)
            self.Img_fGemBg.sprite = icon	
        end);
        ResMgr:LoadAssetSprite('itemicon',{GemConfig[itemInfo:GetGems()[0]].icon},function (icon)
            self.Img_fGemImg.sprite = icon	
        end);
        self.Text_fGemAttr.text = '<color='..GemConfig[itemInfo:GetGems()[0]].color..'>'..
        AttrConfig[GemConfig[itemInfo:GetGems()[0]].attr].name..'+'..tostring(GemConfig[itemInfo:GetGems()[0]].attrValue)..'</color>'
    end

    if itemInfo:GetGems()[1] == 0 then
        UIMgr:SetActive(self.Img_secondGem.transform,false)
    else
        UIMgr:SetActive(self.Img_secondGem.transform,true)
        ResMgr:LoadAssetSprite('itembg',{GemConfig[itemInfo:GetGems()[1]].color},function (icon)
            self.Img_sGemBg.sprite = icon	
        end);
        ResMgr:LoadAssetSprite('itemicon',{GemConfig[itemInfo:GetGems()[1]].icon},function (icon)
            self.Img_sGemImg.sprite = icon	
        end);
        self.Text_sGemAttr.text = '<color='..GemConfig[itemInfo:GetGems()[1]].color..'>'..
        AttrConfig[GemConfig[itemInfo:GetGems()[1]].attr].name..'+'..tostring(GemConfig[itemInfo:GetGems()[1]].attrValue)..'</color>'
    end
end

function ItemInfoPanel:ShowSecondEquipInfo(itemInfo)
    --更新装备背景框
	ResMgr:LoadAssetSprite('itembg',{EquipConfig[itemInfo:GetItemId()].color},function (icon)
		self.Img_S_itemBg.sprite = icon	
    end);
    --更新装备图标
    ResMgr:LoadAssetSprite('itemicon',{EquipConfig[itemInfo:GetItemId()].icon},function (icon)
		self.Img_S_itemImg.sprite = icon	
	end);
    --更新装备名称
	self.Text_S_name.text = '<color='..EquipConfig[itemInfo:GetItemId()].color..'>'..EquipConfig[itemInfo:GetItemId()].name..'</color>'
	--更新装备穿戴状态
	if itemInfo:GetItemInventory() ~= InventoryEnum.Equip then
		UIMgr:SetActive(self.Img_S_dress.transform,false)
	else
		UIMgr:SetActive(self.Img_S_dress.transform,true)
    end
    --更新装备类型
    self.Text_S_type.text = '类型:'..EquipConfig[itemInfo:GetItemId()].typeInfo
    --更新装备需求职业
	if EquipConfig[itemInfo:GetItemId()].profession ~= -1 then
		UIMgr:SetActive(self.Text_S_profession.transform,true)
		self.Text_S_profession.text = '职业:'..ProfessionConfig[EquipConfig[itemInfo:GetItemId()].profession].name
    end
    --更新装备需求等级
	UIMgr:SetActive(self.Text_S_level.transform,true)
    self.Text_S_level.text = '等级:'..tostring(EquipConfig[itemInfo:GetItemId()].level)
    --激活装备基础属性
	UIMgr:SetActive(self.Img_S_baseAttrs.transform,true)
	--更新装备第一条基础属性
	if EquipConfig[itemInfo:GetItemId()].firstAttr ~= -1 then
		UIMgr:SetActive(self.Text_S_firstAttr.transform,true)
		self.Text_S_firstAttr.text = AttrConfig[EquipConfig[itemInfo:GetItemId()].firstAttr].name..'+'..EquipConfig[itemInfo:GetItemId()].firstAttrValue
	else
		UIMgr:SetActive(self.Text_S_firstAttr.transform,false)
    end
    --更新装备第二条基本属性
	if EquipConfig[itemInfo:GetItemId()].secondAttr ~= -1 then
		UIMgr:SetActive(self.Text_S_secondAttr.transform,true)
		self.Text_S_secondAttr.text = AttrConfig[EquipConfig[itemInfo:GetItemId()].secondAttr].name..'+'..EquipConfig[itemInfo:GetItemId()].secondAttrValue
	else
        UIMgr:SetActive(self.Text_S_secondAttr.transform,false)
    end
    --更新装备第三条基本属性
	if EquipConfig[itemInfo:GetItemId()].thirdAttr ~= -1 then
		UIMgr:SetActive(self.Text_S_thirdAttr.transform,true)
		self.Text_S_thirdAttr.text = AttrConfig[EquipConfig[itemInfo:GetItemId()].thirdAttr].name..'+'..EquipConfig[itemInfo:GetItemId()].thirdAttrValue
	else
		UIMgr:SetActive(self.Text_S_thirdAttr.transform,false)
    end
    --隐藏物品详情和宝石属性
	UIMgr:SetActive(self.Img_S_itemInfo.transform,false)
	UIMgr:SetActive(self.Img_S_gemAttrs.transform,false)
end

