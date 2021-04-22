
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:拍卖面板视图层
*
*        description:
*            功能描述:实现拍卖面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


AuctionPanel = {}
local this = AuctionPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
AuctionPanel.Ctrls = 
{
    Img_buyPanel = {  Path = 'Auto_buyPanel',  ControlType = 'Image'  },
    Img_itemTog = {  Path = 'Auto_buyPanel/Auto_itemTog',  ControlType = 'Image'  },
    Img_equipTog = {  Path = 'Auto_buyPanel/Auto_equipTog',  ControlType = 'Image'  },
    Img_gemTog = {  Path = 'Auto_buyPanel/Auto_gemTog',  ControlType = 'Image'  },
    Img_time = {  Path = 'Auto_buyPanel/itemTitle/Auto_time',  ControlType = 'Image'  },
    Img_timeImg = {  Path = 'Auto_buyPanel/itemTitle/Auto_time/Auto_timeImg',  ControlType = 'Image'  },
    Img_bidding = {  Path = 'Auto_buyPanel/itemTitle/Auto_bidding',  ControlType = 'Image'  },
    Img_biddingImg = {  Path = 'Auto_buyPanel/itemTitle/Auto_bidding/Auto_biddingImg',  ControlType = 'Image'  },
    Img_fixed = {  Path = 'Auto_buyPanel/itemTitle/Auto_fixed',  ControlType = 'Image'  },
    Img_fixedImg = {  Path = 'Auto_buyPanel/itemTitle/Auto_fixed/Auto_fixedImg',  ControlType = 'Image'  },
    Img_nameInput = {  Path = 'Auto_buyPanel/Auto_nameInput',  ControlType = 'Image'  },
    Img_search = {  Path = 'Auto_buyPanel/Auto_search',  ControlType = 'Image'  },
    Img_auctionView = {  Path = 'Auto_buyPanel/Auto_auctionView',  ControlType = 'Image'  },
    Img_auctionContent = {  Path = 'Auto_buyPanel/Auto_auctionView/Viewport/Auto_auctionContent',  ControlType = 'Image'  },
    Img_sellPanel = {  Path = 'Auto_sellPanel',  ControlType = 'Image'  },
    Img_item = {  Path = 'Auto_sellPanel/Auto_item',  ControlType = 'Image'  },
    Img_biddingInput = {  Path = 'Auto_sellPanel/Auto_biddingInput',  ControlType = 'Image'  },
    Img_fixedInput = {  Path = 'Auto_sellPanel/Auto_fixedInput',  ControlType = 'Image'  },
    Img_timeInput = {  Path = 'Auto_sellPanel/Auto_timeInput',  ControlType = 'Image'  },
    Img_auctionBtn = {  Path = 'Auto_sellPanel/Auto_auctionBtn',  ControlType = 'Image'  },
    Img_itemView = {  Path = 'Auto_sellPanel/Auto_itemView',  ControlType = 'Image'  },
    Img_itemContent = {  Path = 'Auto_sellPanel/Auto_itemView/Viewport/Auto_itemContent',  ControlType = 'Image'  },
    Img_moneyInput = {  Path = 'Auto_moneyInput',  ControlType = 'Image'  },
    Img_sellBtn = {  Path = 'Auto_sellBtn',  ControlType = 'Image'  },
    Img_buyBtn = {  Path = 'Auto_buyBtn',  ControlType = 'Image'  },
    Img_fixedBtn = {  Path = 'Auto_fixedBtn',  ControlType = 'Image'  },
    Img_biddingBtn = {  Path = 'Auto_biddingBtn',  ControlType = 'Image'  },
    Img_close = {  Path = 'Auto_close',  ControlType = 'Image'  },
    Text_service = {  Path = 'Auto_sellPanel/Auto_service',  ControlType = 'Text'  },
    Tog_itemTog = {  Path = 'Auto_buyPanel/Auto_itemTog',  ControlType = 'Toggle'  },
    Tog_equipTog = {  Path = 'Auto_buyPanel/Auto_equipTog',  ControlType = 'Toggle'  },
    Tog_gemTog = {  Path = 'Auto_buyPanel/Auto_gemTog',  ControlType = 'Toggle'  },
    Tog_time = {  Path = 'Auto_buyPanel/itemTitle/Auto_time',  ControlType = 'Toggle'  },
    Tog_bidding = {  Path = 'Auto_buyPanel/itemTitle/Auto_bidding',  ControlType = 'Toggle'  },
    Tog_fixed = {  Path = 'Auto_buyPanel/itemTitle/Auto_fixed',  ControlType = 'Toggle'  },
    Btn_search = {  Path = 'Auto_buyPanel/Auto_search',  ControlType = 'Button'  },
    Btn_auctionBtn = {  Path = 'Auto_sellPanel/Auto_auctionBtn',  ControlType = 'Button'  },
    Btn_sellBtn = {  Path = 'Auto_sellBtn',  ControlType = 'Button'  },
    Btn_buyBtn = {  Path = 'Auto_buyBtn',  ControlType = 'Button'  },
    Btn_fixedBtn = {  Path = 'Auto_fixedBtn',  ControlType = 'Button'  },
    Btn_biddingBtn = {  Path = 'Auto_biddingBtn',  ControlType = 'Button'  },
    Btn_close = {  Path = 'Auto_close',  ControlType = 'Button'  },
    Input_nameInput = {  Path = 'Auto_buyPanel/Auto_nameInput',  ControlType = 'InputField'  },
    Input_biddingInput = {  Path = 'Auto_sellPanel/Auto_biddingInput',  ControlType = 'InputField'  },
    Input_fixedInput = {  Path = 'Auto_sellPanel/Auto_fixedInput',  ControlType = 'InputField'  },
    Input_timeInput = {  Path = 'Auto_sellPanel/Auto_timeInput',  ControlType = 'InputField'  },
    Input_moneyInput = {  Path = 'Auto_moneyInput',  ControlType = 'InputField'  },
    Rect_auctionView = {  Path = 'Auto_buyPanel/Auto_auctionView',  ControlType = 'ScrollRect'  },
    Rect_itemView = {  Path = 'Auto_sellPanel/Auto_itemView',  ControlType = 'ScrollRect'  },
}

local ItemConfig = require 'Config/ItemConfig'
local EquipConfig = require 'Config/EquipConfig'
local GemConfig = require 'Config/GemConfig'

local gameObject
local transform

local currentItem  --当前选中拍卖物品
local currentItemInfo  --当前选中拍卖物品信息

local currentItemClass  --当前选中物品类型

local currentBoxGuid  --当前选中的拍卖品Guid

local lotsInfos  --缓存服务端传过来的所有拍卖品信息

local currentSortType  --当前排序类型

local AuctionOptimizeSV  --拍卖品的优化滚动视图
local ItemOptimizeSV  --显示背包物品的优化滚动视图
local auctionBoxObserver  --拍卖物品监听观察者
local sellInfoObserver  --出售物品监听观察者


function AuctionPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function AuctionPanel:Init()

    AuctionOptimizeSV = OptimizeSV:New()  --初始化
    ItemOptimizeSV = OptimizeSV:New()
    auctionBoxObserver = Observer:New(function (S2C_lotsInfos)
        self:UpdateAuctionBox(S2C_lotsInfos)
    end)
    sellInfoObserver = Observer:New(function ()
        self:UpdateBagItems()
        self:ClearSellInfo()
    end)

    --关闭按钮
    PanelMgr:ButtonAddListener(self.Btn_close,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()  --关闭面板
    end)

    --购买按钮
    PanelMgr:ButtonAddListener(self.Btn_buyBtn,function ()
        AudioMgr:PlayEffect('toggle')
        self:CloseSellPanel()  --关闭出售界面
        self:OpenBuyPanel()  --打开购买界面
    end)

    --出售按钮
    PanelMgr:ButtonAddListener(self.Btn_sellBtn,function ()
        AudioMgr:PlayEffect('toggle')
        self:CloseBuyPanel()  --关闭购买界面
        self:OpenSellPanel()  --打开出售界面
    end)

    --时间的排序按钮
    PanelMgr:ToggleAddListener(self.Tog_time,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮箭头图标
        if isChoose == false then
            UIMgr:ChangeTogImg(false,self.Img_timeImg,'down','up')
        end
        if currentSortType ~= AuctionSortEnum.TimeAscend then
            UIMgr:ChangeTogImg(true,self.Img_timeImg,'down','up')
            currentSortType = AuctionSortEnum.TimeAscend
        else
            UIMgr:ChangeTogImg(false,self.Img_timeImg,'down','up')
            currentSortType = AuctionSortEnum.TimeDescend
        end
        --根据时间进行排序
        self:SortLots(currentSortType)
        self:UpdateAuctionBox(lotsInfos)  --刷新拍卖视图
    end)

    --竞价的排序按钮
    PanelMgr:ToggleAddListener(self.Tog_bidding,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮箭头图标
        if isChoose == false then
            UIMgr:ChangeTogImg(false,self.Img_biddingImg,'down','up')
        end
        if currentSortType ~= AuctionSortEnum.BiddingAscend then
            UIMgr:ChangeTogImg(true,self.Img_biddingImg,'down','up')
            currentSortType = AuctionSortEnum.BiddingAscend
        else
            UIMgr:ChangeTogImg(false,self.Img_biddingImg,'down','up')
            currentSortType = AuctionSortEnum.BiddingDescend
        end
        self:SortLots(currentSortType)  --根据竞价进行排序
        self:UpdateAuctionBox(lotsInfos)  --刷新拍卖视图
    end)

    --一口价的的排序按钮
    PanelMgr:ToggleAddListener(self.Tog_fixed,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        --更新按钮箭头图标
        if isChoose == false then
            UIMgr:ChangeTogImg(false,self.Img_fixedImg,'down','up')
        end
        if currentSortType ~= AuctionSortEnum.FixedAscend then
            UIMgr:ChangeTogImg(true,self.Img_fixedImg,'down','up')
            currentSortType = AuctionSortEnum.FixedAscend
        else
            UIMgr:ChangeTogImg(false,self.Img_fixedImg,'down','up')
            currentSortType = AuctionSortEnum.FixedDescend
        end
        self:SortLots(currentSortType)  --根据一口价进行排序
        self:UpdateAuctionBox(lotsInfos)  --刷新拍卖视图
    end)

    --物品类型按钮
    PanelMgr:ToggleAddListener(self.Tog_itemTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_itemTog,'chooseState','normalState')  --更新按钮图标
        if currentItemClass == ItemClass.Item then
            return
        end
        --AuctionOptimizeSV:Clear()
        currentItemClass = ItemClass.Item  --更新物品类型
        AuctionCtrl:GetLotsInfos(currentItemClass)  --调用逻辑层的获取该类型拍卖品函数
    end)

    --装备类型按钮
    PanelMgr:ToggleAddListener(self.Tog_equipTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_equipTog,'chooseState','normalState')  --更新按钮图标
        if currentItemClass == ItemClass.Equip then
            return
        end
        --AuctionOptimizeSV:Clear()
        currentItemClass = ItemClass.Equip  --更新物品类型
        AuctionCtrl:GetLotsInfos(currentItemClass)  --调用逻辑层的获取该类型的拍卖品函数
    end)

    --宝石类型按钮
    PanelMgr:ToggleAddListener(self.Tog_gemTog,function (isChoose)
        AudioMgr:PlayEffect('toggle')
        UIMgr:ChangeTogImg(isChoose,self.Img_gemTog,'chooseState','normalState')  --更新按钮图标
        if currentItemClass == ItemClass.Gem then
            return
        end
        --AuctionOptimizeSV:Clear()
        currentItemClass = ItemClass.Gem  --更新物品类型
        AuctionCtrl:GetLotsInfos(currentItemClass)  --调用逻辑层的获取该类型的拍卖品函数
    end)

    --拍卖按钮
    PanelMgr:ButtonAddListener(self.Btn_auctionBtn,function ()
        AudioMgr:PlayEffect('button')
        --判断拍卖所填信息是否完整，一口价是否不小于竞拍价且都不小于0
        if self.Input_biddingInput.text == '' or self.Input_fixedInput.text == '' or self.Input_timeInput.text == ''then
            return
        end
        if tonumber(self.Input_biddingInput.text) < 1 or tonumber(self.Input_fixedInput.text) < 1 or
         tonumber(self.Input_timeInput.text) < 1 then
            return
        end
        if tonumber(self.Input_biddingInput.text) > tonumber(self.Input_fixedInput.text) then
            return;
        end
        if currentItemInfo == nil then
            return
        end
        --调用逻辑层的拍卖函数
        AuctionCtrl:Auction(tonumber(self.Input_biddingInput.text),tonumber(self.Input_fixedInput.text),
        tonumber(self.Input_timeInput.text),currentItemInfo:GetGuid())

    end)

    PanelMgr:ButtonAddListener(self.Btn_search,function ()
        AudioMgr:PlayEffect('button')
        if self.Input_nameInput.text == '' or currentItemClass == nil then
            return
        end
        AuctionCtrl:Search(currentItemClass,self.Input_nameInput.text)

    end)

    PanelMgr:ButtonAddListener(self.Btn_biddingBtn,function ()
        AudioMgr:PlayEffect('button')
        if currentBoxGuid == nil or currentItemClass == nil then
            return
        end
        if self.Input_moneyInput.text == '' then
            return
        end
        AuctionCtrl:Bidding(currentBoxGuid,currentItemClass,tonumber(self.Input_moneyInput.text))
    end)

    PanelMgr:ButtonAddListener(self.Btn_fixedBtn,function ()
        AudioMgr:PlayEffect('button')
        if currentBoxGuid == nil or currentItemClass == nil then
            return
        end
        AuctionCtrl:FixedBuy(currentBoxGuid,currentItemClass)
    end)

end

function AuctionPanel:Show()
    self:CloseSellPanel()
    self:OpenBuyPanel()
end

function AuctionPanel:Hide()
    self:CloseSellPanel()
    self:OpenBuyPanel()
end
--[[
function AuctionPanel:ChangeTypeImg(isChoose,Img_tog,highlightedIcon,normalIcon)
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
function AuctionPanel:OpenBuyPanel()

    UIMgr:SetActive(self.Img_buyPanel.transform,true)
    UIMgr:AddListener('UpdateAuctionBox',auctionBoxObserver)
    self.Tog_itemTog.isOn = false
    self.Tog_itemTog.isOn = true

end

function AuctionPanel:CloseBuyPanel()

    PanelMgr:ScrollRectRemoveListener(self.Rect_auctionView)
    AuctionOptimizeSV:Clear()
    self:ClearAuctionInfo()

    UIMgr:RemoveListener('UpdateAuctionBox',auctionBoxObserver)
    UIMgr:ChangeTogImg(false,self.Img_itemTog,'chooseState','normalState')
    UIMgr:ChangeTogImg(false,self.Img_equipTog,'chooseState','normalState')
    UIMgr:ChangeTogImg(false,self.Img_gemTog,'chooseState','normalState')
    UIMgr:ChangeTogImg(false,self.Img_timeImg,'down','up')
    UIMgr:ChangeTogImg(false,self.Img_biddingImg,'down','up')
    UIMgr:ChangeTogImg(false,self.Img_fixedImg,'down','up')

    UIMgr:SetActive(self.Img_buyPanel.transform,false)

    self.Tog_equipTog.isOn = false
    self.Tog_gemTog.isOn = false
    currentItemClass = nil
    lotsInfos = nil
    currentSortType = nil

end

function AuctionPanel:OpenSellPanel()
    UIMgr:SetActive(self.Img_sellPanel.transform,true)
    self:UpdateBagItems()
    UIMgr:AddListener('UpdateSellInfo',sellInfoObserver)
    PanelMgr:InputAddListener(self.Input_timeInput,function (service)
        self.Text_service.text = service
    end)
end

function AuctionPanel:CloseSellPanel()
    PanelMgr:ScrollRectRemoveListener(self.Rect_itemView)
    ItemOptimizeSV:Clear()
    self:ClearSellInfo()
    UIMgr:SetActive(self.Img_sellPanel.transform,false)
    UIMgr:RemoveListener('UpdateSellInfo',sellInfoObserver)
    PanelMgr:InputRemoveListener(self.Input_timeInput)
end

function AuctionPanel:ClearSellInfo()

    --[[
    if currentItem ~= nil then
        PanelMgr:ToggleRemoveListener(currentItem.transform:GetComponent('Toggle'))
        PoolMgr:Set(currentItem.name,currentItem)
        currentItem = nil 
    end
    --]]
    if currentItem ~= nil then
        PanelMgr:ToggleRemoveListener(currentItem.transform:GetComponent('Toggle'))
        PoolMgr:Set(currentItem.name,currentItem)
    end
    currentItem = nil
    currentItemInfo = nil

    self.Input_biddingInput.text = ''
    self.Input_fixedInput.text = ''
    self.Input_timeInput.text = ''
    self.Text_service.text = '0'

    

end

function AuctionPanel:ClearAuctionInfo()

    self.Input_nameInput.text = ''

end

local ShowAuctionBox = {
    [1] = function (box,lotsInfo)
        ResMgr:LoadAssetSprite('itemicon',{ItemConfig[lotsInfo.itemInfo.itemId].icon},function (icon)
            box.transform:Find('Auto_itemIcon'):GetComponent('Image').sprite = icon	
        end)
        box.transform:Find('Auto_itemName'):GetComponent('Text').text = '<color=white>'
        ..ItemConfig[lotsInfo.itemInfo.itemId].name..'</color>'
        box.transform:Find('Auto_bidderName'):GetComponent('Text').text = lotsInfo.bidderName
        box.transform:Find('Auto_time'):GetComponent('Text').text = '剩余'..tostring(lotsInfo.remainTime)..'小时'
        box.transform:Find('Auto_biddingPrice'):GetComponent('Text').text = tostring(lotsInfo.auctionPrice)
        box.transform:Find('Auto_fixedPrice'):GetComponent('Text').text = tostring(lotsInfo.fixedPrice)
        box.transform:GetComponent('Toggle').group = this.Img_auctionContent.transform:GetComponent('ToggleGroup')
        PanelMgr:ToggleAddListener(box.transform:GetComponent('Toggle'),function (isChoose)
            UIMgr:ChangeTogImg(isChoose,box.transform:GetComponent('Image'),'hight','nor')
            currentBoxGuid = lotsInfo.Guid
        end)
        PanelMgr:ButtonAddListener(box.transform:Find('Auto_itemIcon'):GetComponent('Button'),function ()
            local itemInfo = ItemInfo:New(InventoryEnum.Auction,lotsInfo.itemInfo.itemType,lotsInfo.itemInfo.itemId,
            lotsInfo.itemInfo.Guid,lotsInfo.itemInfo.count)
            PanelMgr:OpenPanel('ItemInfoPanel',UILayer.Top,MaskEnum.TipsMask,function (itemInfoPanel)
                ItemInfoCtrl:ShowItemInfo(box.transform:Find('Auto_itemIcon').gameObject,itemInfo)
            end)
        end)
    end,
    [2] = function (box,lotsInfo)
        ResMgr:LoadAssetSprite('itemicon',{EquipConfig[lotsInfo.equipInfo.itemId].icon},function (icon)
            box.transform:Find('Auto_itemIcon'):GetComponent('Image').sprite = icon	
        end)
        box.transform:Find('Auto_itemName'):GetComponent('Text').text = '<color='..EquipConfig[lotsInfo.equipInfo.itemId].color..'>'
        ..EquipConfig[lotsInfo.equipInfo.itemId].name..'</color>'
        box.transform:Find('Auto_bidderName'):GetComponent('Text').text = lotsInfo.bidderName
        box.transform:Find('Auto_time'):GetComponent('Text').text = '剩余'..tostring(lotsInfo.remainTime)..'小时'
        box.transform:Find('Auto_biddingPrice'):GetComponent('Text').text = tostring(lotsInfo.auctionPrice)
        box.transform:Find('Auto_fixedPrice'):GetComponent('Text').text = tostring(lotsInfo.fixedPrice)
        box.transform:GetComponent('Toggle').group = this.Img_auctionContent.transform:GetComponent('ToggleGroup')
        PanelMgr:ToggleAddListener(box.transform:GetComponent('Toggle'),function (isChoose)
            UIMgr:ChangeTogImg(isChoose,box.transform:GetComponent('Image'),'hight','nor')
            currentBoxGuid = lotsInfo.Guid
        end)
        PanelMgr:ButtonAddListener(box.transform:Find('Auto_itemIcon'):GetComponent('Button'),function ()
            local equipInfo = EquipInfo:New(InventoryEnum.Auction,lotsInfo.equipInfo.itemType,lotsInfo.equipInfo.itemId,
            lotsInfo.equipInfo.Guid,lotsInfo.equipInfo.gems)
            PanelMgr:OpenPanel('ItemInfoPanel',UILayer.Top,MaskEnum.TipsMask,function (itemInfoPanel)
                ItemInfoCtrl:ShowItemInfo(box.transform:Find('Auto_itemIcon').gameObject,equipInfo)
            end)
        end)
    end,
    [3] = function (box,lotsInfo)
        ResMgr:LoadAssetSprite('itemicon',{GemConfig[lotsInfo.itemInfo.itemId].icon},function (icon)
            box.transform:Find('Auto_itemIcon'):GetComponent('Image').sprite = icon	
        end)
        box.transform:Find('Auto_itemName'):GetComponent('Text').text = '<color='..GemConfig[lotsInfo.itemInfo.itemId].color..'>'
        ..GemConfig[lotsInfo.itemInfo.itemId].name..'</color>'
        box.transform:Find('Auto_bidderName'):GetComponent('Text').text = lotsInfo.bidderName
        box.transform:Find('Auto_time'):GetComponent('Text').text = '剩余'..tostring(lotsInfo.remainTime)..'小时'
        box.transform:Find('Auto_biddingPrice'):GetComponent('Text').text = tostring(lotsInfo.auctionPrice)
        box.transform:Find('Auto_fixedPrice'):GetComponent('Text').text = tostring(lotsInfo.fixedPrice)
        box.transform:GetComponent('Toggle').group = this.Img_auctionContent.transform:GetComponent('ToggleGroup')
        PanelMgr:ToggleAddListener(box.transform:GetComponent('Toggle'),function (isChoose)
            UIMgr:ChangeTogImg(isChoose,box.transform:GetComponent('Image'),'hight','nor')
            currentBoxGuid = lotsInfo.Guid
        end)
        PanelMgr:ButtonAddListener(box.transform:Find('Auto_itemIcon'):GetComponent('Button'),function ()
            local gemInfo = ItemInfo:New(InventoryEnum.Auction,lotsInfo.itemInfo.itemType,lotsInfo.itemInfo.itemId,
            lotsInfo.itemInfo.Guid,lotsInfo.itemInfo.count)
            PanelMgr:OpenPanel('ItemInfoPanel',UILayer.Top,MaskEnum.TipsMask,function (itemInfoPanel)
                ItemInfoCtrl:ShowItemInfo(box.transform:Find('Auto_itemIcon').gameObject,gemInfo)
            end)
        end)
    end
}

function AuctionPanel:UpdateAuctionBox(S2C_lotsInfos)

    PanelMgr:ScrollRectRemoveListener(self.Rect_auctionView)
    lotsInfos = S2C_lotsInfos
    AuctionOptimizeSV:Clear()
    AuctionOptimizeSV:VerticalLayout(self.Rect_auctionView.transform:GetComponent('RectTransform'),
    self.Img_auctionContent.transform:GetComponent('RectTransform'),650,50,S2C_lotsInfos.lotsInfos.Count,0,0,0,0,0,
    ViewEnum.TopToBottom,CornerEnum.UpperLeft,'AuctionBox',
    function (index,box)
        ShowAuctionBox[S2C_lotsInfos.itemType](box,S2C_lotsInfos.lotsInfos[index])
    end,
    function (index,box)
        UIMgr:ChangeTogImg(false,box.transform:GetComponent('Image'),'hight','nor')
        PanelMgr:ButtonRemoveListener(box.transform:Find('Auto_itemIcon'):GetComponent('Button'))
        PanelMgr:ToggleRemoveListener(box.transform:GetComponent('Toggle'))
    end)

    PanelMgr:ScrollRectAddListener(self.Rect_auctionView,function (point)
        AuctionOptimizeSV:Update()
    end)

end

local ShowItem = {
    [1] = function (item,itemInfo)
        ResMgr:LoadAssetSprite('itembg',{'white'},function (icon)
			item.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
        end)
        ResMgr:LoadAssetSprite('itemicon',{ItemConfig[itemInfo:GetItemId()].icon},function (icon)
            item.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
        end);
        item.transform:Find('Auto_itemCount'):GetComponent('Text').text = tostring(itemInfo:GetItemCount());
    end,
    [2] = function (equip,equipInfo)
        ResMgr:LoadAssetSprite('itembg',{EquipConfig[equipInfo:GetItemId()].color},function (icon)
			equip.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
        end);
        ResMgr:LoadAssetSprite('itemicon',{EquipConfig[equipInfo:GetItemId()].icon},function (icon)
            equip.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
        end);
		equip.transform:Find('Auto_itemCount'):GetComponent('Text').text = ''
    end,
    [3] = function (gem,gemInfo)
        ResMgr:LoadAssetSprite('itembg',{GemConfig[gemInfo:GetItemId()].color},function (icon)
			gem.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
        end);
        ResMgr:LoadAssetSprite('itemicon',{GemConfig[gemInfo:GetItemId()].icon},function (icon)
            gem.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
        end);
		gem.transform:Find('Auto_itemCount'):GetComponent('Text').text = tostring(gemInfo:GetItemCount())
    end
}

function AuctionPanel:AddItemListener(item,itemInfo)
    PanelMgr:ToggleAddListener(item.transform:GetComponent('Toggle'),function ()
        if currentItemInfo ~= nil then
            return 
        end

        item.transform:Find('Auto_itemImg'):GetComponent('Image').color = Color.green
        currentItemInfo = itemInfo
        PoolMgr:Get('Item',function (newItem)
            currentItem = newItem
            newItem.transform.position = self.Img_item.transform.position
            newItem.transform:SetParent(self.Img_item.transform)
            ShowItem[itemInfo:GetItemType()](newItem,itemInfo)
            PanelMgr:ToggleAddListener(newItem.transform:GetComponent('Toggle'),function ()
                item.transform:Find('Auto_itemImg'):GetComponent('Image').color = Color.white
                currentItem = nil
                currentItemInfo = nil
                PanelMgr:ToggleRemoveListener(newItem.transform:GetComponent('Toggle'))
                PoolMgr:Set(newItem.name,newItem)
                self:AddItemListener(item,itemInfo)
            end)
        end)
        PanelMgr:ToggleRemoveListener(item.transform:GetComponent('Toggle'))
    end)
end

function AuctionPanel:UpdateBagItems()

    PanelMgr:ScrollRectRemoveListener(self.Rect_itemView)

    ItemOptimizeSV:Clear()
    
    local itemSum = {}
    local items = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Item)

    if items ~= nil then
        for index = 1,#items do
            table.insert(itemSum,items[index])
        end
    end
    
    local equips = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Equip)

    if equips ~= nil then
        for index = 1,#equips do
            table.insert(itemSum,equips[index])
        end
    end

    local gems = InventoryMgr:GetTypeItems(InventoryEnum.Bag,ItemClass.Gem)

    if gems ~= nil then
        for index = 1,#gems do
            table.insert(itemSum,gems[index])
        end
    end
    if #itemSum == 0 then
        return
    end

    ItemOptimizeSV:GridLayout(self.Rect_itemView.transform:GetComponent('RectTransform'),self.Img_itemContent.transform
        :GetComponent('RectTransform'),100,100,#itemSum,5,0,25,0,0,0,ViewEnum.TopToBottom,CornerEnum.UpperLeft,'Item',
        function (index,item)
            ShowItem[itemSum[index+1]:GetItemType()](item,itemSum[index+1])
            self:AddItemListener(item,itemSum[index+1])
        end,
        function (index,item)
            item.transform:Find('Auto_itemImg'):GetComponent('Image').color = Color.white
            PanelMgr:ToggleRemoveListener(item.transform:GetComponent('Toggle'))
        end
    )

    PanelMgr:ScrollRectAddListener(self.Rect_itemView,function (point)
        ItemOptimizeSV:Update()
    end)

end

local Judge = {
    [1] = function (newLots,oldLots)
        if newLots.remainTime < oldLots.remainTime then
            return true
        else
            return false
        end
    end,
    [2] = function (newLots,oldLots)
        if newLots.remainTime > oldLots.remainTime then
            return true
        else
            return false
        end
    end,
    [3] = function (newLots,oldLots)
        if newLots.auctionPrice < oldLots.auctionPrice then
            return true
        else
            return false
        end
    end,
    [4] = function (newLots,oldLots)
        if newLots.auctionPrice > oldLots.auctionPrice then
            return true
        else
            return false
        end
    end,
    [5] = function (newLots,oldLots)
        if newLots.fixedPrice < oldLots.fixedPrice then
            return true
        else
            return false
        end
    end,
    [6] = function (newLots,oldLots)
        if newLots.fixedPrice > oldLots.fixedPrice then
            return true
        else
            return false
        end
    end,
}

function AuctionPanel:SortLots(sortType)

    local sum = lotsInfos.lotsInfos.Count
    local currentNum = 1
    for index = 1,sum-1 do
        local lots = lotsInfos.lotsInfos[index]
        lotsInfos.lotsInfos:Remove(lots)
        for i = 0,currentNum-1 do
            if Judge[sortType](lots,lotsInfos.lotsInfos[i]) then
                lotsInfos.lotsInfos:Insert(i,lots)
                break
            end 
            if i == currentNum-1 then
                lotsInfos.lotsInfos:Insert(i+1,lots)
            end
        end
        currentNum = currentNum + 1
    end

end