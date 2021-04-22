
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:商城面板视图层
*
*        description:
*            功能描述:实现商城面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


ShopPanel = {}
local this = ShopPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
ShopPanel.Ctrls = 
{
    Img_item = {  Path = 'itemType/Auto_item',  ControlType = 'Image'  },
    Img_equip = {  Path = 'itemType/Auto_equip',  ControlType = 'Image'  },
    Img_gem = {  Path = 'itemType/Auto_gem',  ControlType = 'Image'  },
    Img_activity = {  Path = 'itemType/Auto_activity',  ControlType = 'Image'  },
    Img_view = {  Path = 'Auto_view',  ControlType = 'Image'  },
    Img_content = {  Path = 'Auto_view/Viewport/Auto_content',  ControlType = 'Image'  },
    Img_addSilverBtn = {  Path = 'money/silver/Auto_addSilverBtn',  ControlType = 'Image'  },
    Img_addGoldBtn = {  Path = 'money/gold/Auto_addGoldBtn',  ControlType = 'Image'  },
    Img_addYuanBaoBtn = {  Path = 'money/yuanBao/Auto_addYuanBaoBtn',  ControlType = 'Image'  },
    Img_buys = {  Path = 'Auto_buys',  ControlType = 'Image'  },
    Img_buy = {  Path = 'Auto_buy',  ControlType = 'Image'  },
    Img_close = {  Path = 'Auto_close',  ControlType = 'Image'  },
    Text_silverNum = {  Path = 'money/silver/Auto_silverNum',  ControlType = 'Text'  },
    Text_goldNum = {  Path = 'money/gold/Auto_goldNum',  ControlType = 'Text'  },
    Text_yuanBaoNum = {  Path = 'money/yuanBao/Auto_yuanBaoNum',  ControlType = 'Text'  },
    Tog_item = {  Path = 'itemType/Auto_item',  ControlType = 'Toggle'  },
    Tog_equip = {  Path = 'itemType/Auto_equip',  ControlType = 'Toggle'  },
    Tog_gem = {  Path = 'itemType/Auto_gem',  ControlType = 'Toggle'  },
    Tog_activity = {  Path = 'itemType/Auto_activity',  ControlType = 'Toggle'  },
    Btn_buys = {  Path = 'Auto_buys',  ControlType = 'Button'  },
    Btn_buy = {  Path = 'Auto_buy',  ControlType = 'Button'  },
    Btn_close = {  Path = 'Auto_close',  ControlType = 'Button'  },
    Rect_view = {  Path = 'Auto_view',  ControlType = 'ScrollRect'  },
}

local EquipConfig = require 'Config/EquipConfig'
local ItemConfig = require 'Config/ItemConfig'
local GemConfig = require 'Config/GemConfig'

local gameObject
local transform

local currentType  --当前选择得物品类型
local currentItemId  --当前选择得物品id

local CommodityOptimizeSV  --商品优化滚动视图组件
local roleInfoObserver  --角色信息观察者

function ShopPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

local GetConfig = {
    [ItemClass.Item] = function ()
        return ItemConfig;
    end,
    [ItemClass.Equip] = function ()
        return EquipConfig;
    end,
    [ItemClass.Gem] = function ()
        return GemConfig;
    end
}

function ShopPanel:Init()

    CommodityOptimizeSV = OptimizeSV:New()

    roleInfoObserver = Observer:New(function ()  --角色信息观察者回调
        self:UpdateMoney()  --刷新金钱
    end)

    --关闭按钮
    PanelMgr:ButtonAddListener(self.Btn_close,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()
    end)

    --给物品按钮添加监听事件
    PanelMgr:ToggleAddListener(self.Tog_item,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_item,'btnHighlighted','btnNormal')  --更新按钮图标
        if isChoose == true then
            if currentType == ItemClass.Item then
                return
            end
            currentItemId = nil  --重置选择得物品id
            currentType = ItemClass.Item  --更新物品类型
            self:UpdateCommodity()  --刷新商品
        end
    end)

    --给装备按钮添加监听事件
    PanelMgr:ToggleAddListener(self.Tog_equip,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_equip,'btnHighlighted','btnNormal')  --更新按钮图标
        if isChoose == true then
            if currentType == ItemClass.Equip then
                return
            end
            currentItemId = nil  --重置选择得物品id
            currentType = ItemClass.Equip  --更新物品类型
            self:UpdateCommodity()  --刷新商品
        end
    end)

    --给宝石按钮添加监听事件
    PanelMgr:ToggleAddListener(self.Tog_gem,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_gem,'btnHighlighted','btnNormal')  --更新按钮图标
        if isChoose == true then
            if currentType == ItemClass.Gem then
                return
            end
            currentItemId = nil  --重置选择得物品id
            currentType = ItemClass.Gem  --更新物品类型
            self:UpdateCommodity()  --刷新商品
        end
    end)

    --给活动按钮添加监听事件
    PanelMgr:ToggleAddListener(self.Tog_activity,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_activity,'btnHighlighted','btnNormal')
        currentType = nil
        currentItemId = nil
        PanelMgr:ScrollRectRemoveListener(self.Rect_view)
        CommodityOptimizeSV:Clear()
    end)

    --给购买按钮添加监听事件
    PanelMgr:ButtonAddListener(self.Btn_buy,function ()
        AudioMgr:PlayEffect('button')
        if currentItemId == nil then
            return
        end
        local config = GetConfig[currentType]()
        --弹出购买消息框
        UIMgr:Trigger("Message",MessageEnum.Buy,'确认要购买'..config[currentItemId].name..'吗?',currentType,currentItemId)
    end)

    --[[
    PanelMgr:ButtonAddListener(self.Btn_buys,function ()
    
    end)
    --]]
end

function ShopPanel:Show()

    self.Tog_item.isOn = false
    self.Tog_item.isOn = true
    self:UpdateMoney()
    UIMgr:AddListener('UpdateRoleInfo',roleInfoObserver)

end

function ShopPanel:Hide()
    
    PanelMgr:ScrollRectRemoveListener(self.Rect_view)  --移除滚动视图滑动监听
    CommodityOptimizeSV:Clear()  --清空商品格子
    UIMgr:RemoveListener('UpdateRoleInfo',roleInfoObserver)
    currentType = nil  --重置当前物品类型
    currentItemId = nil  --重置当前选择得物品id
end
--[[
function ShopPanel:ChangeItemTypeImg(isChoose,Img_tog)
	local iconName
	if isChoose == true then
		iconName = 'btnHighlighted'
	else
		iconName = 'btnNormal'
	end
	ResMgr:LoadAssetSprite('bagicon',{iconName},function (icon)
		Img_tog.sprite = icon
	end);
end
--]]
--刷新商品
function ShopPanel:UpdateCommodity()

    PanelMgr:ScrollRectRemoveListener(self.Rect_view)  --移除滚动视图滑动监听
    CommodityOptimizeSV:Clear()  --清空商品格子

    local items = InventoryMgr:GetTypeItems(InventoryEnum.Shop,currentType)  --获取所有该类型得商品
    if items == nil then
        return
    end
    local config = GetConfig[currentType]()
    --给滚动视图赋值
    CommodityOptimizeSV:GridLayout(self.Rect_view.transform:GetComponent('RectTransform'),self.Img_content.transform:
        GetComponent('RectTransform'),200,100,#items,10,10,10,0,10,0,ViewEnum.TopToBottom,CornerEnum.UpperLeft,"Commodity",
        function (index,commodity)  --当商品显示时调用的函数
            ResMgr:LoadAssetSprite('itemicon',{config[items[index+1]:GetItemId()].icon},function (icon)
                commodity.transform:Find('Auto_icon'):GetComponent('Image').sprite = icon	--加载商品图标并赋值
            end);
            PanelMgr:ButtonAddListener(commodity.transform:Find('Auto_icon'):GetComponent('Button'),function (isChoose)
                AudioMgr:PlayEffect('button')
                --给图片添加事件，当点击时打开物品信息面板
                PanelMgr:OpenPanel('ItemInfoPanel',UILayer.Top,MaskEnum.TipsMask,function (itemInfoPanel)
                    ItemInfoCtrl:ShowItemInfo(commodity.transform:Find('Auto_icon').gameObject,items[index+1])
                end)
            end)
            --设置商品为单选框
            commodity.transform:GetComponent('Toggle').group = self.Img_content.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(commodity.transform:GetComponent('Toggle'),function (isChoose)
                AudioMgr:PlayEffect('button')
                if isChoose == true then 
                    currentItemId = items[index+1]:GetItemId()  --更新当前选择的物品id
                    commodity.transform:GetComponent('Image').color = Color.yellow  --将商品格子设置为黄色
                else
                    commodity.transform:GetComponent('Image').color = Color.white  --将商品格子设置为白色
                end
            end)
            ResMgr:LoadAssetSprite('othericon',{'gold'},function (icon)
                commodity.transform:Find('Auto_moneyType'):GetComponent('Image').sprite = icon	--加载金钱类型图标并赋值
            end);
            commodity.transform:Find('Auto_moneyNumber'):GetComponent('Text').text =   --更新价格
            tostring(config[items[index+1]:GetItemId()].buyPrice)
        end,
        function (index,commodity)  --当商品隐藏时调用的函数
            commodity:GetComponent('Image').color = Color.white  --将商品格子重置为白色
            PanelMgr:ToggleRemoveListener(commodity:GetComponent('Toggle'))  --移除监听
            PanelMgr:ButtonRemoveListener(commodity.transform:Find('Auto_icon'):GetComponent('Button'))  --移除图片监听
        end
    )

    PanelMgr:ScrollRectAddListener(self.Rect_view,function (point)  --给滚动视图添加滑动监听
        CommodityOptimizeSV:Update()  --刷新商品格子
    end)

    --[[
    self:Clear()
    
    
    
    for index = 1,#items do
        PoolMgr:Get("Commodity",function (commodity)
            commodity.transform:SetParent(self.Img_content.transform)
            ResMgr:LoadAssetSprite('itemicon',{config[items[index]:GetItemId()].icon},function (icon)
                commodity.transform:Find('Auto_icon'):GetComponent('Image').sprite = icon	
            end);
            PanelMgr:ButtonAddListener(commodity.transform:Find('Auto_icon'):GetComponent('Button'),function (isChoose)
                PanelMgr:OpenPanel('ItemInfoPanel',UILayer.Top,MaskEnum.TipsMask,function (itemInfoPanel)
                    ItemInfoCtrl:ShowItemInfo(commodity.transform:Find('Auto_icon').gameObject,items[index])
                end)
            end)
            commodity.transform:GetComponent('Toggle').group = self.Img_content.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(commodity.transform:GetComponent('Toggle'),function (isChoose)
                if isChoose == true then 
                    currentItemId = items[index]:GetItemId()
                    commodity.transform:GetComponent('Image').color = Color.yellow
                else
                    commodity.transform:GetComponent('Image').color = Color.white
                end
            end)
            ResMgr:LoadAssetSprite('bagicon',{'gold'},function (icon)
                commodity.transform:Find('Auto_moneyType'):GetComponent('Image').sprite = icon	
            end);
            commodity.transform:Find('Auto_moneyNumber'):GetComponent('Text').text = 
            tostring(config[items[index]:GetItemId()].buyPrice)
        end)
    end
    --]]
end

function ShopPanel:UpdateMoney()
    local roleData = RoleInfoMgr:GetMainRole();
	self.Text_silverNum.text = roleData.OtherData:GetSilver()
	self.Text_goldNum.text = roleData.OtherData:GetGold()
	self.Text_yuanBaoNum.text = roleData.OtherData:GetYuanBao()
end