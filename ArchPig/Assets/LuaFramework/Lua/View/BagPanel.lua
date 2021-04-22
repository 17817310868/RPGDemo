
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:背包面板视图层
*
*        description:
*            功能描述:实现背包面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


BagPanel = {}
local this = BagPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
BagPanel.Ctrls = 
{
    Img_schoolIcon = {  Path = 'title/Auto_schoolIcon',  ControlType = 'Image'  },
    Img_equipContent = {  Path = 'Auto_equipContent',  ControlType = 'Image'  },
    Img_helmet = {  Path = 'Auto_equipContent/Auto_helmet',  ControlType = 'Image'  },
    Img_weapon = {  Path = 'Auto_equipContent/Auto_weapon',  ControlType = 'Image'  },
    Img_belt = {  Path = 'Auto_equipContent/Auto_belt',  ControlType = 'Image'  },
    Img_necklace = {  Path = 'Auto_equipContent/Auto_necklace',  ControlType = 'Image'  },
    Img_clothes = {  Path = 'Auto_equipContent/Auto_clothes',  ControlType = 'Image'  },
    Img_shoes = {  Path = 'Auto_equipContent/Auto_shoes',  ControlType = 'Image'  },
    Img_bagData = {  Path = 'Auto_bagData',  ControlType = 'Image'  },
    Img_itemTog = {  Path = 'Auto_bagData/bagType/Auto_itemTog',  ControlType = 'Image'  },
    Img_equipTog = {  Path = 'Auto_bagData/bagType/Auto_equipTog',  ControlType = 'Image'  },
    Img_gemTog = {  Path = 'Auto_bagData/bagType/Auto_gemTog',  ControlType = 'Image'  },
    Img_addSilverBtn = {  Path = 'Auto_bagData/money/silver/Auto_addSilverBtn',  ControlType = 'Image'  },
    Img_addGoldBtn = {  Path = 'Auto_bagData/money/gold/Auto_addGoldBtn',  ControlType = 'Image'  },
    Img_addYuanBaoBtn = {  Path = 'Auto_bagData/money/yuanBao/Auto_addYuanBaoBtn',  ControlType = 'Image'  },
    Img_view = {  Path = 'Auto_bagData/Auto_view',  ControlType = 'Image'  },
    Img_bagContent = {  Path = 'Auto_bagData/Auto_view/Viewport/Auto_bagContent',  ControlType = 'Image'  },
    Img_closeBtn = {  Path = 'Auto_closeBtn',  ControlType = 'Image'  },
    Img_roleData = {  Path = 'Auto_roleData',  ControlType = 'Image'  },
    Img_bagTog = {  Path = 'dataTog/Auto_bagTog',  ControlType = 'Image'  },
    Img_roleTog = {  Path = 'dataTog/Auto_roleTog',  ControlType = 'Image'  },
    Text_name = {  Path = 'title/Auto_name',  ControlType = 'Text'  },
    Text_levelText = {  Path = 'title/Auto_levelText',  ControlType = 'Text'  },
    Text_silverNum = {  Path = 'Auto_bagData/money/silver/Auto_silverNum',  ControlType = 'Text'  },
    Text_goldNum = {  Path = 'Auto_bagData/money/gold/Auto_goldNum',  ControlType = 'Text'  },
    Text_yuanBaoNum = {  Path = 'Auto_bagData/money/yuanBao/Auto_yuanBaoNum',  ControlType = 'Text'  },
    Text_hpText = {  Path = 'Auto_roleData/hp/Auto_hpText',  ControlType = 'Text'  },
    Text_mpText = {  Path = 'Auto_roleData/mp/Auto_mpText',  ControlType = 'Text'  },
    Text_spText = {  Path = 'Auto_roleData/sp/Auto_spText',  ControlType = 'Text'  },
    Text_energyText = {  Path = 'Auto_roleData/energy/Auto_energyText',  ControlType = 'Text'  },
    Text_pAttackText = {  Path = 'Auto_roleData/physicalAttack/Auto_pAttackText',  ControlType = 'Text'  },
    Text_pDefenseText = {  Path = 'Auto_roleData/physicalDefense/Auto_pDefenseText',  ControlType = 'Text'  },
    Text_mAttackText = {  Path = 'Auto_roleData/magicAttack/Auto_mAttackText',  ControlType = 'Text'  },
    Text_mDefenseText = {  Path = 'Auto_roleData/magicDefense/Auto_mDefenseText',  ControlType = 'Text'  },
    Text_speedText = {  Path = 'Auto_roleData/speed/Auto_speedText',  ControlType = 'Text'  },
    Text_experienceText = {  Path = 'Auto_roleData/experience/Auto_experienceText',  ControlType = 'Text'  },
    Slider_hpSlider = {  Path = 'Auto_roleData/hp/Auto_hpSlider',  ControlType = 'Slider'  },
    Slider_mpSlider = {  Path = 'Auto_roleData/mp/Auto_mpSlider',  ControlType = 'Slider'  },
    Slider_spSlider = {  Path = 'Auto_roleData/sp/Auto_spSlider',  ControlType = 'Slider'  },
    Slider_energySlider = {  Path = 'Auto_roleData/energy/Auto_energySlider',  ControlType = 'Slider'  },
    Slider_experienceSlider = {  Path = 'Auto_roleData/experience/Auto_experienceSlider',  ControlType = 'Slider'  },
    Tog_itemTog = {  Path = 'Auto_bagData/bagType/Auto_itemTog',  ControlType = 'Toggle'  },
    Tog_equipTog = {  Path = 'Auto_bagData/bagType/Auto_equipTog',  ControlType = 'Toggle'  },
    Tog_gemTog = {  Path = 'Auto_bagData/bagType/Auto_gemTog',  ControlType = 'Toggle'  },
    Tog_bagTog = {  Path = 'dataTog/Auto_bagTog',  ControlType = 'Toggle'  },
    Tog_roleTog = {  Path = 'dataTog/Auto_roleTog',  ControlType = 'Toggle'  },
    Btn_closeBtn = {  Path = 'Auto_closeBtn',  ControlType = 'Button'  },
    Rect_view = {  Path = 'Auto_bagData/Auto_view',  ControlType = 'ScrollRect'  },
}

local ItemConfig = require "Config/ItemConfig"
local EquipConfig = require "Config/EquipConfig"
local GemConfig = require 'Config/GemConfig'
local ProfessionConfig = require "Config/ProfessionConfig"
local SchoolConfig = require 'Config/SchoolConfig'
local ExperienceConfig = require 'Config/ExperienceConfig'

local gameObject
local transform

local itemClass  --当前所选得物品类型
local currentItem  --当前选中的物品
local currentEquips  --存储当前存在的装备对象
local currentRole
local actor

local ItemOptimizeSV  --背包物品优化布局组件
local bagItemObserver  --物品监听观察者
local equipObserver  --装备栏监听观察者
local roleInfoObserver  --角色信息监听观察者

function BagPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function BagPanel:Init()

	itemClass = nil  --将当前所选物品类型置空
    currentItem = nil  --将当前选中物品置空
    currentEquips = {}
    ItemOptimizeSV = OptimizeSV:New()  --给布局组件赋值
    bagItemObserver = Observer:New(function ()
        self:UpdateBagItems()
    end)
    equipObserver = Observer:New(function ()
        self:UpdateEquips()
    end)
    roleInfoObserver = Observer:New(function ()
        self:UpdateBaseData()
        self:UpdateRoleInfo()
    end)

    --给背包按钮添加监听
    PanelMgr:ToggleAddListener(self.Tog_bagTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮图标
        UIMgr:ChangeTogImg(isChoose,self.Img_bagTog,'highlighted','normal')
        --显示物品面板，隐藏角色属性面板
        self:CloseRolePanel()
        self:OpenItemPanel()
        
    end)

    --给角色属性按钮添加监听
    PanelMgr:ToggleAddListener(self.Tog_roleTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮图标
        UIMgr:ChangeTogImg(isChoose,self.Img_roleTog,'highlighted','normal')
        --隐藏物品面板，显示角色属性面板
        self:CloseItemPanel()
        self:OpenRolePanel()
    end)

	--给物品类型按钮添加监听事件
    PanelMgr:ToggleAddListener(self.Tog_itemTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮图标
		UIMgr:ChangeTogImg(isChoose,self.Img_itemTog,'btnHighlighted','btnNormal')
		if ItemClass.Item == itemClass then
			return;
		end
		--更新当前所显示的物品类型
        itemClass = ItemClass.Item
        --刷新背包物品
		self:UpdateBagItems()
	end)

    --给装备类型按钮添加监听
    PanelMgr:ToggleAddListener(self.Tog_equipTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮图标
		UIMgr:ChangeTogImg(isChoose,self.Img_equipTog,'btnHighlighted','btnNormal')
		if ItemClass.Equip == itemClass then
			return;
		end
		--更新当前所显示的物品类型
        itemClass = ItemClass.Equip
        --刷新背包物品
		self:UpdateBagItems()
	end)

    --给宝石类型按钮添加监听
    PanelMgr:ToggleAddListener(self.Tog_gemTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮图标
		UIMgr:ChangeTogImg(isChoose,self.Img_gemTog,'btnHighlighted','btnNormal')
		if ItemClass.Gem == itemClass then
			return;
		end
		--更新当前所显示的物品类型
        itemClass = ItemClass.Gem
        --刷新背包物品
		self:UpdateBagItems()
	end)

	--关闭背包按钮
    PanelMgr:ButtonAddListener(self.Btn_closeBtn, function ()
        AudioMgr:PlayEffect('button')
		PanelMgr:ClosePanel()
	end)
end

function BagPanel:Show()

    --监听刷新装备事件
	UIMgr:AddListener('UpdateBagItems',equipObserver)
    --监听刷新角色基本属性事件
    UIMgr:AddListener('UpdateRoleInfo',roleInfoObserver)

    --默认打开背包
    self.Tog_bagTog.isOn = false
    self.Tog_bagTog.isOn = true
    
	self:UpdateEquips()  --刷新所穿戴的装备
    self:UpdateBaseData()  --刷新角色基础信息
    --self:CloseRolePanel()  --关闭角色属性面板
    --self:OpenItemPanel()  --打开物品面板
	
end

function BagPanel:Hide()

    self.Tog_roleTog.isOn = false  --将角色属性按钮设置为非选中
    self:ClearEquips()
    self:CloseItemPanel()  --关闭物品面板
    self:CloseRolePanel()  --关闭角色属性面板
    UIMgr:RemoveListener('UpdateBagItems',equipObserver)  --取消刷新装备监听
    UIMgr:RemoveListener('UpdateRoleInfo',roleInfoObserver)  --取消刷新角色基本属性监听
    
end
--[[
--更新物品类型按钮的图标   isChoose:是否选中   Img_tog:控件图片
function BagPanel:ChangeTypeImg(isChoose,Img_tog,highlightedIcon,normalIcon)
	local iconName
	if isChoose == true then
		iconName = highlightedIcon
	else
		iconName = normalIcon
	end
	ResMgr:LoadAssetSprite('bagicon',{iconName},function (icon)
		Img_tog.sprite = icon
	end)
end
--]]
--打开物品面板
function BagPanel:OpenItemPanel()
    UIMgr:SetActive(self.Img_bagData.transform,true)  --激活背包面板父物体
    UIMgr:AddListener('UpdateBagItems',bagItemObserver)
    self.Tog_itemTog.isOn = false  --默认激活物品类型
    self.Tog_itemTog.isOn = true
end

--关闭物品面板
function BagPanel:CloseItemPanel()
    UIMgr:RemoveListener('UpdateBagItems',bagItemObserver)  --取消刷新背包物品监听
    self.Tog_equipTog.isOn = false  --将装备类型按钮设置为非选中
    self.Tog_gemTog.isOn = false  --将宝石类型按钮设置为非选中
    PanelMgr:ScrollRectRemoveListener(self.Rect_view)
    ItemOptimizeSV:Clear()  --清空物品
    currentItem = nil  --将当前选中物品置空
    itemClass = nil  --将当前物品类型置空
    UIMgr:SetActive(self.Img_bagData.transform,false)  --隐藏背包面板父物体
end

--显示角色属性面板
function BagPanel:OpenRolePanel()
    UIMgr:SetActive(self.Img_roleData.transform,true)  --激活角色属性面板父物体
    UIMgr:AddListener('UpdateRoleInfo',roleInfoObserver)
    self:UpdateRoleInfo()  --刷新角色属性
end

--关闭角色属性面板
function BagPanel:CloseRolePanel()
    UIMgr:RemoveListener('UpdateRoleInfo',roleInfoObserver)  --取消监听刷新角色属性事件
    UIMgr:SetActive(self.Img_roleData.transform,false)  --隐藏角色属性面板父物体
end

--刷新背包物品
function BagPanel:UpdateBagItems()

    PanelMgr:ScrollRectRemoveListener(self.Rect_view)
    ItemOptimizeSV:Clear()  --清空物品
    
    local items = InventoryMgr:GetTypeItems(InventoryEnum.Bag,itemClass)  --获取目前该物品栏中的所有该类型物品
    
	if items == nil then
		return;
    end
    --添加布局信息(scrollRect,content,格子宽度，格子高度，格子数量，top,bottom,left,right,spacingX,spacingY,排列方向,起点位置,资源名,
    --显示回调，隐藏回调)
	ItemOptimizeSV:GridLayout(self.Rect_view.transform:GetComponent('RectTransform'),
    self.Img_bagContent.transform:GetComponent('RectTransform'),75,75,#items,10,10,16,0,10,10,
    ViewEnum.TopToBottom,CornerEnum.UpperLeft,'Item',
    function (index,box)
        self:ShowItem(box,items[index+1])  --调用显示物品函数并传递参数
    end,
    function (index,box)
        PanelMgr:ToggleRemoveListener(box.transform:GetComponent('Toggle'))  --移除物品上的toggle事件
    end)

    PanelMgr:ScrollRectAddListener(self.Rect_view,function (point)
        ItemOptimizeSV:Update()
    end)

end

--清空装备栏上的所有装备对象
function BagPanel:ClearEquips()

    if currentRole ~= nil then
        PoolMgr:Set(currentRole.name,currentRole)
        currentRole = nil
    end
    actor = nil

    if #currentEquips == 0 then
        return
    end
    for index = 1,#currentEquips do
        PanelMgr:ToggleRemoveListener(currentEquips[index].transform:GetComponent('Toggle'))  --移除装备上的toggle事件
        PoolMgr:Set(currentEquips[index].name,currentEquips[index])  --放入对象池
    end
    --重置装备字典
    currentEquips = {}
end

--根据物品类型做出不同的调整
local ItemSwitch = {
	--当物品为普通物品时，从物品配置表查找，普通物品背景框统一为白色
	[1] = function (itemGO,itemInfo)
		ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
			itemGO.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
		end);
		itemGO.transform:Find('Auto_itemCount'):GetComponent('Text').text = tostring(itemInfo:GetItemCount());
		return ItemConfig;
	end,
	--当物品为装备时，从装备配置表查找，物品背景框由装备质量决定
	[ItemClass.Equip] = function (itemGO,itemInfo)
		ResMgr:LoadAssetSprite('itembg',{EquipConfig[itemInfo:GetItemId()].color},function (icon)
			itemGO.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
		end);
		itemGO.transform:Find('Auto_itemCount'):GetComponent('Text').text = ''
		return EquipConfig;
    end,
    --当物品为宝石时，从宝石配置表查找，物品背景框由颜色决定
	[ItemClass.Gem] = function (itemGO,itemInfo)
		ResMgr:LoadAssetSprite('itembg',{GemConfig[itemInfo:GetItemId()].color},function (icon)
			itemGO.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
		end);
		itemGO.transform:Find('Auto_itemCount'):GetComponent('Text').text = tostring(itemInfo:GetItemCount());
		return GemConfig;
	end
}

--显示物品   itemGO:物品GO   itemInfo:物品信息
function BagPanel:ShowItem(itemGO,itemInfo)
	local Config = ItemSwitch[itemInfo:GetItemType()](itemGO,itemInfo);
    --加载物品图标
	ResMgr:LoadAssetSprite('itemicon',{Config[itemInfo:GetItemId()].icon},function (icon)
		itemGO.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
	end);
	--物品按钮为单选,背包面板下只能选择一个物品
	itemGO.transform:GetComponent('Toggle').group = transform:GetComponent('ToggleGroup')
	--当点击物品时，显示物品信息面板
    PanelMgr:ToggleAddListener(itemGO.transform:GetComponent('Toggle'),function (isChoose)
        AudioMgr:PlayEffect('toggle')
		if isChoose == false then
			return;
		end
		--更新当前物品信息
		currentItem = itemInfo
		PanelMgr:OpenPanel('ItemInfoPanel',UILayer.Top,MaskEnum.TipsMask,function (itemInfoPanel)
			ItemInfoCtrl:ShowItemInfo(itemGO,itemInfo)
		end)
	end)
end

--装备类型枚举对应的装备节点
local EquipRoot = {
	[1] = function ()
		return this.Img_helmet.transform
	end,
	[2] = function ()
		return this.Img_necklace.transform
	end,
	[3] = function ()
		return this.Img_weapon.transform
	end,
	[4] = function ()
		return this.Img_clothes.transform
	end,
	[5] = function ()
		return this.Img_belt.transform
	end,
	[6] = function ()
		return this.Img_shoes.transform
	end
}

--刷新装备栏的装备
function BagPanel:UpdateEquips()

	--清空原有的装备
	self:ClearEquips()
	
    local roleData = RoleInfoMgr:GetMainRole()
    if currentRole == nil then
        PoolMgr:Get(ProfessionConfig[roleData:GetProfession()].model,function (role)
            role.transform.position = Vector3.New(0,0,0)
            role.transform.localEulerAngles = Vector3.New(0,0,0)
            currentRole = role
            actor = Actor.New(role)
            local equips = InventoryMgr:GetInventoryItem(InventoryEnum.Equip)
	        if equips == nil then
		        return
            end

            for index = 1,#equips do
                actor:UpdateBind(equips[index]:GetItemId())
                PoolMgr:Get('Item',function (equip)
                    --从配置表读取该装备信息对应的装备类型，再去拿对应的装备节点
                    equip.transform:SetParent(EquipRoot[EquipConfig[equips[index]:GetItemId()]._type]())
                    self:ShowItem(equip,equips[index])  --显示装备
                    table.insert(currentEquips,equip)  --添加进装备对象字典
                end)
            end
        end)
    end
    
end

function BagPanel:LoadEquips()

    --获取装备栏中所有的装备数据
    
end

--刷新角色属性基本属性
function BagPanel:UpdateBaseData()

	local roleData = RoleInfoMgr:GetMainRole();
	self.Text_levelText.text = roleData:GetLevel()
	self.Text_name.text = roleData:GetName()
	ResMgr:LoadAssetSprite('schoolicon',{SchoolConfig[roleData:GetSchool()].schoolIconTrue},function (icon)
        self.Img_schoolIcon.sprite = icon
    end);
	self.Text_silverNum.text = roleData.OtherData:GetSilver()
	self.Text_goldNum.text = roleData.OtherData:GetGold()
	self.Text_yuanBaoNum.text = roleData.OtherData:GetYuanBao()

end

--刷新角色属性
function BagPanel:UpdateRoleInfo()
	
	local roleData = RoleInfoMgr:GetMainRole();
    self.Slider_hpSlider.value = roleData.OtherData:GetHp() / roleData.OtherData:GetMaxHp()
	self.Text_hpText.text = tostring(roleData.OtherData:GetHp())..'/'..tostring(roleData.OtherData:GetMaxHp())
	self.Slider_mpSlider.value = roleData.OtherData:GetMp() / roleData.OtherData:GetMaxMp()
	self.Text_mpText.text = tostring(roleData.OtherData:GetMp())..'/'..tostring(roleData.OtherData:GetMaxMp())
	self.Text_pAttackText.text = tostring(roleData.OtherData:GetPhysicalAttack())
	self.Text_pDefenseText.text = tostring(roleData.OtherData:GetPhysicalDefense())
	self.Text_mAttackText.text = tostring(roleData.OtherData:GetMagicAttack())
	self.Text_mDefenseText.text = tostring(roleData.OtherData:GetMagicDefense())
    self.Text_speedText.text = tostring(roleData.OtherData:GetSpeed())
    if roleData:GetLevel() >= 100 then
        self.Slider_experienceSlider.value = 1
        self.Text_experienceText.text = roleData.OtherData:GetExperience()..'/Max'
    else
        self.Slider_experienceSlider.value = roleData.OtherData:GetExperience() / ExperienceConfig[roleData:GetLevel()+1].experience
        self.Text_experienceText.text = roleData.OtherData:GetExperience()..'/'..ExperienceConfig[roleData:GetLevel()+1].experience
    end
    
	
end