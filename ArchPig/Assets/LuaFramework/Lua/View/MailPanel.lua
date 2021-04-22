
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:邮件面板视图层
*
*        description:
*            功能描述:实现邮件面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


MailPanel = {}
local this = MailPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
MailPanel.Ctrls = 
{
    Img_mailView = {  Path = 'Auto_mailView',  ControlType = 'Image'  },
    Img_mails = {  Path = 'Auto_mailView/Viewport/Auto_mails',  ControlType = 'Image'  },
    Img_itemView = {  Path = 'Auto_itemView',  ControlType = 'Image'  },
    Img_items = {  Path = 'Auto_itemView/Viewport/Auto_items',  ControlType = 'Image'  },
    Img_back = {  Path = 'Auto_itemView/Auto_back',  ControlType = 'Image'  },
    Img_sendMail = {  Path = 'Auto_sendMail',  ControlType = 'Image'  },
    Img_receiveInput = {  Path = 'Auto_sendMail/Auto_receiveInput',  ControlType = 'Image'  },
    Img_titleInput = {  Path = 'Auto_sendMail/Auto_titleInput',  ControlType = 'Image'  },
    Img_contentInput = {  Path = 'Auto_sendMail/Auto_contentInput',  ControlType = 'Image'  },
    Img_sendItems = {  Path = 'Auto_sendMail/Auto_sendItems',  ControlType = 'Image'  },
    Img_moneyInput = {  Path = 'Auto_sendMail/Auto_moneyInput',  ControlType = 'Image'  },
    Img_receiveMail = {  Path = 'Auto_receiveMail',  ControlType = 'Image'  },
    Img_getItemsBtn = {  Path = 'Auto_receiveMail/Auto_getItemsBtn',  ControlType = 'Image'  },
    Img_receiveItems = {  Path = 'Auto_receiveMail/Auto_receiveItems',  ControlType = 'Image'  },
    Img_writeMailBtn = {  Path = 'Auto_writeMailBtn',  ControlType = 'Image'  },
    Img_sendMailBtn = {  Path = 'Auto_sendMailBtn',  ControlType = 'Image'  },
    Img_deleteMailBtn = {  Path = 'Auto_deleteMailBtn',  ControlType = 'Image'  },
    Img_deleteMailsBtn = {  Path = 'Auto_deleteMailsBtn',  ControlType = 'Image'  },
    Img_close = {  Path = 'Auto_close',  ControlType = 'Image'  },
    Text_addresserText = {  Path = 'Auto_receiveMail/Auto_addresserText',  ControlType = 'Text'  },
    Text_titleText = {  Path = 'Auto_receiveMail/Auto_titleText',  ControlType = 'Text'  },
    Text_contentText = {  Path = 'Auto_receiveMail/Auto_contentText',  ControlType = 'Text'  },
    Text_money = {  Path = 'Auto_receiveMail/Auto_money',  ControlType = 'Text'  },
    Btn_back = {  Path = 'Auto_itemView/Auto_back',  ControlType = 'Button'  },
    Btn_sendItems = {  Path = 'Auto_sendMail/Auto_sendItems',  ControlType = 'Button'  },
    Btn_getItemsBtn = {  Path = 'Auto_receiveMail/Auto_getItemsBtn',  ControlType = 'Button'  },
    Btn_receiveItems = {  Path = 'Auto_receiveMail/Auto_receiveItems',  ControlType = 'Button'  },
    Btn_writeMailBtn = {  Path = 'Auto_writeMailBtn',  ControlType = 'Button'  },
    Btn_sendMailBtn = {  Path = 'Auto_sendMailBtn',  ControlType = 'Button'  },
    Btn_deleteMailBtn = {  Path = 'Auto_deleteMailBtn',  ControlType = 'Button'  },
    Btn_deleteMailsBtn = {  Path = 'Auto_deleteMailsBtn',  ControlType = 'Button'  },
    Btn_close = {  Path = 'Auto_close',  ControlType = 'Button'  },
    Input_receiveInput = {  Path = 'Auto_sendMail/Auto_receiveInput',  ControlType = 'InputField'  },
    Input_titleInput = {  Path = 'Auto_sendMail/Auto_titleInput',  ControlType = 'InputField'  },
    Input_contentInput = {  Path = 'Auto_sendMail/Auto_contentInput',  ControlType = 'InputField'  },
    Input_moneyInput = {  Path = 'Auto_sendMail/Auto_moneyInput',  ControlType = 'InputField'  },
    Rect_mailView = {  Path = 'Auto_mailView',  ControlType = 'ScrollRect'  },
    Rect_itemView = {  Path = 'Auto_itemView',  ControlType = 'ScrollRect'  },
}

local ItemConfig = require 'Config/ItemConfig'
local EquipConfig = require 'Config/EquipConfig'
local GemConfig = require 'Config/GemConfig'

local gameObject
local transform

local currentGrid

local oneGridItemInfo
local twoGridItemInfo
local threeGridItemInfo

local sendItemsInfo
local sendItems
local receiveItems

local currentMailGuid

local MailBoxOptimizeSV
local BagItemOptimizeSV
local updateMailObserver

local isOpenItemPanel

function MailPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function MailPanel:Init()

    updateMailObserver = Observer:New(function (S2C_receiveMails)
        self:UpdateMails(S2C_receiveMails)
    end)

    isOpenItemPanel = false
    sendItems = {}
    sendItemsInfo = {}
    receiveItems = {}

    MailBoxOptimizeSV = OptimizeSV:New()
    BagItemOptimizeSV = OptimizeSV:New()

    PanelMgr:ButtonAddListener(self.Btn_close,function ()
        AudioMgr:PlayEffect('button')
        PanelMgr:ClosePanel()
    end)

    --当玩家点击写件按钮，关闭邮件信息面板，打开填写邮件信息面板
    PanelMgr:ButtonAddListener(self.Btn_writeMailBtn,function ()
        AudioMgr:PlayEffect('button')
        self:CloseReceivePanel()
        self:OpenWritePanel()
        --self:ClearMailInfo()
        --self:ClearMails()
        --UIMgr:SetActive(self.Img_sendMail.transform,true)
        --UIMgr:SetActive(self.Img_receiveMail.transform,false)
    end)

    --发送的物品栏
    PanelMgr:ButtonAddListener(self.Btn_sendItems,function ()
        AudioMgr:PlayEffect('button')
        if isOpenItemPanel == true then
            return
        end
        self:CloseMailBoxPanel()
        self:OpenItemPanel()
    end)
    
    --当玩家点击返回按钮，关闭物品面板，显示邮件格子面板
    PanelMgr:ButtonAddListener(self.Btn_back,function ()
        AudioMgr:PlayEffect('button')
        self:CloseItemPanel()
        self:OpenMailBoxPanel()
    end)
    --当玩家点击发送按钮，判断邮件信息是否填写完整，调用逻辑层的发送函数并传递参数
    PanelMgr:ButtonAddListener(self.Btn_sendMailBtn,function ()
        AudioMgr:PlayEffect('button')
        if self.Input_receiveInput.text == "" then
            return
        end
        if self.Input_titleInput.text == "" then
            return
        end
        if self.Input_contentInput.text == "" then
            return
        end
        local itemsGuid = {}
        if #sendItemsInfo > 0 then
            for index = 1,#sendItemsInfo do
                table.insert(itemsGuid,sendItemsInfo[index]:GetGuid())
            end
        end
        local gold
        if self.Input_moneyInput.text == '' then
            gold = 0
        else
            gold = tonumber(self.Input_moneyInput.text)
        end

        
        MailCtrl:SendMail(self.Input_receiveInput.text,self.Input_titleInput.text,self.Input_contentInput.text,gold,itemsGuid)

        self:CloseItemPanel()  --关闭显示背包物品的面板
        self:CloseWritePanel()  --关闭填写邮件信息的面板
        --[[
        self:ClearBagItems()
        self:ClearWriteInfo()
        UIMgr:SetActive(self.Rect_itemView.transform,false)
        UIMgr:SetActive(self.Rect_mailView.transform,true)
        --]]
    end)
    --当玩家点击领取邮件物品按钮,判断是否有所选邮件，有的话即调用逻辑层的获取邮件物品函数
    PanelMgr:ButtonAddListener(self.Btn_getItemsBtn,function ()
        AudioMgr:PlayEffect('button')
        if currentMailGuid == nil then
            return
        end
        MailCtrl:GetItems(currentMailGuid)
        self:CloseReceivePanel()  -- 
    end)

end

function MailPanel:Show()

    --[[
    currentGrid = nil

    UIMgr:SetActive(self.Img_sendMail.transform,false)
    UIMgr:SetActive(self.Img_receiveMail.transform,false)
    UIMgr:SetActive(self.Rect_itemView.transform,false)
    UIMgr:SetActive(self.Rect_mailView.transform,true)
    --]]
    self:CloseItemPanel()
    self:CloseReceivePanel()
    self:CloseWritePanel()
    self:OpenMailBoxPanel()

    

end

function MailPanel:Hide()

    --self:ClearBagItems()
    --self:ClearWriteInfo()

    self:CloseItemPanel()
    self:CloseMailBoxPanel()
    self:CloseReceivePanel()
    self:CloseWritePanel()

end

--打开显示背包物品的面板
function MailPanel:OpenItemPanel()
    UIMgr:SetActive(self.Img_itemView.transform,true)
    isOpenItemPanel = true
    self:UpdateBagItem()
end

--关闭显示背包物品的面板
function MailPanel:CloseItemPanel()
    currentGrid = nil
    isOpenItemPanel = false
    BagItemOptimizeSV:Clear()
    UIMgr:SetActive(self.Img_itemView.transform,false)
end

--打开显示所有邮件格子的面板
function MailPanel:OpenMailBoxPanel()
    UIMgr:SetActive(self.Img_mailView.transform,true)
    UIMgr:AddListener('UpdateMails',updateMailObserver)
    MailCtrl:GetMails()
end

--关闭显示所有邮件格子的面板
function MailPanel:CloseMailBoxPanel()
    currentMailGuid = nil
    PanelMgr:ScrollRectRemoveListener(self.Rect_mailView)
    MailBoxOptimizeSV:Clear()
    UIMgr:RemoveListener('UpdateMails',updateMailObserver)
    UIMgr:SetActive(self.Img_mailView.transform,false)
end

--打开填写邮件信息的面板
function MailPanel:OpenWritePanel()
    UIMgr:SetActive(self.Img_sendMail.transform,true)
end

--关闭填写邮件信息的面板
function MailPanel:CloseWritePanel()
    self:ClearWriteInfo()
    UIMgr:SetActive(self.Img_sendMail.transform,false)
end

--打开显示接收邮件的信息的面板
function MailPanel:OpenReceivePanel()
    UIMgr:SetActive(self.Img_receiveMail.transform,true)
end

--关闭显示接收邮件的信息的面板
function MailPanel:CloseReceivePanel()
    self:ClearMailInfo()
    UIMgr:SetActive(self.Img_receiveMail.transform,false)
end

function MailPanel:UpdateMails(S2C_receiveMails)

    PanelMgr:ScrollRectRemoveListener(self.Rect_mailView)
    MailBoxOptimizeSV:Clear()
    MailBoxOptimizeSV:VerticalLayout(self.Rect_mailView.transform:GetComponent('RectTransform'),self.Img_mails.transform:
        GetComponent('RectTransform'),280,100,S2C_receiveMails.mails.Count,10,0,10,0,10,ViewEnum.TopToBottom,CornerEnum.UpperLeft,
        'MailBox',
        function (index,mailBox)
            local mail = S2C_receiveMails.mails[index]
            mailBox.transform:Find('Auto_title'):GetComponent('Text').text = mail.title
            mailBox.transform:Find('Auto_name'):GetComponent('Text').text = mail.addresserName
            if mail.isRead == true then
                mailBox.transform:Find('Auto_read'):GetComponent('Text').text = '已读'
            else
                mailBox.transform:Find('Auto_read'):GetComponent('Text').text = '未读'
            end
            if mail.isExistItem == true then
                UIMgr:SetActive(mailBox.transform:Find('Auto_haveItem'),true)
            else
                UIMgr:SetActive(mailBox.transform:Find('Auto_haveItem'),false)
            end
            mailBox.transform:GetComponent('Toggle').group = self.Rect_mailView.transform:GetComponent('ToggleGroup')
            PanelMgr:ToggleAddListener(mailBox.transform:GetComponent('Toggle'),function (isChoose)
                AudioMgr:PlayEffect('toggle')
                UIMgr:ChangeTogImg(isChoose,mailBox:GetComponent('Image'),'chooseState','normalState')
                if isChoose == true then
                    self:ClearWriteInfo()
                    mailBox.transform:Find('Auto_read'):GetComponent('Text').text = '已读'
                    if mail.isRead == false then
                        MailCtrl:ReadMail(mail.Guid)
                    end
                    UIMgr:SetActive(self.Img_sendMail.transform,false)
                    UIMgr:SetActive(self.Img_receiveMail.transform,true)
                    self:ClearMailInfo()
                    currentMailGuid = mail.Guid
                    self:ShowMailInfo(mail)
                    
                end
            end)
        end,
        function (index,mailBox)
            UIMgr:ChangeTogImg(false,mailBox:GetComponent('Image'),'chooseState','normalState')
            PanelMgr:ToggleRemoveListener(mailBox:GetComponent('Toggle'))
        end
    )

    PanelMgr:ScrollRectAddListener(self.Rect_mailView,function (point)
        MailBoxOptimizeSV:Update()
    end)
end

function MailPanel:ClearMailInfo()

    self.Text_addresserText.text = ''
    self.Text_titleText.text = ''
    self.Text_contentText.text = ''
    self.Text_money.text = ''
    
    if #receiveItems == 0 then
        return
    end

    for index = 1,#receiveItems do
        PoolMgr:Set(receiveItems[index].name,receiveItems[index])
    end

    currentMailGuid = nil
    receiveItems = {}

end
--[[
function MailPanel:ChangeImg(isChoose,Img,trueIcon,falseIcon)
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

function MailPanel:ItemAddListener(item,itemInfo)
    PanelMgr:ToggleAddListener(item.transform:GetComponent('Toggle'),function ()
        AudioMgr:PlayEffect('button')
        if #sendItemsInfo > 2 then
            return
        end
        item.transform:Find('Auto_itemImg'):GetComponent('Image').color = Color.green
        table.insert(sendItemsInfo,itemInfo)
        local index = #sendItemsInfo
        PoolMgr:Get('Item',function (newItem)
            table.insert(sendItems,newItem)
            newItem.transform:SetParent(self.Img_sendItems.transform)
            ShowItem[itemInfo:GetItemType()](newItem,itemInfo)
            PanelMgr:ToggleAddListener(newItem.transform:GetComponent('Toggle'),function ()
                item.transform:Find('Auto_itemImg'):GetComponent('Image').color = Color.white
                table.remove(sendItemsInfo,index)
                table.remove(sendItems,index)
                PanelMgr:ToggleRemoveListener(newItem.transform:GetComponent('Toggle'))
                PoolMgr:Set(newItem.name,newItem)
                self:ItemAddListener(item,itemInfo)
            end)
        end)
        PanelMgr:ToggleRemoveListener(item.transform:GetComponent('Toggle'))
    end)
end

function MailPanel:ClearWriteInfo()

    self.Input_receiveInput.text = ''
    self.Input_titleInput.text = ''
    self.Input_contentInput.text = ''
    self.Input_moneyInput.text = ''

    if #sendItems == 0 then
        return;
    end

    for index = 1,#sendItems do
        PanelMgr:ToggleRemoveListener(sendItems[index]:GetComponent('Toggle'))
        PoolMgr:Set(sendItems[index].name,sendItems[index])
    end

    sendItems = {}
    sendItemsInfo = {}

end

function MailPanel:ShowMailInfo(mail)
    self.Text_addresserText.text = mail.addresserName
    self.Text_titleText.text = mail.title
    self.Text_contentText.text = mail.content
    self.Text_money.text = mail.gold
    if mail.items.Count ~= 0 then
        for i = 0,mail.items.Count-1 do
            local itemInfo = mail.items[i]
            
            PoolMgr:Get('Item',function (item)
                item.transform:SetParent(self.Img_receiveItems.transform)
                local itemBgIcon
                if itemInfo.itemType == ItemClass.Item then
                    itemBgIcon = 'white'
                else
                    itemBgIcon = GemConfig[itemInfo.itemId].color
                end
                ResMgr:LoadAssetSprite('itembg',{itemBgIcon},function (icon)
                    item.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
                end)
                local itemIcon
                if itemInfo.itemType == ItemClass.Item then
                    itemIcon = ItemConfig[itemInfo.itemId].icon
                else
                    itemIcon = GemConfig[itemInfo.itemId].icon
                end
                ResMgr:LoadAssetSprite('itemicon',{itemIcon},function (icon)
                    item.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
                end)
                item.transform:Find('Auto_itemCount'):GetComponent('Text').text = tostring(itemInfo.count)
                table.insert(receiveItems,item)
            end)
        end
    end
    if mail.equips.Count ~= 0 then
        for i = 0,mail.equips.Count-1 do
            local equipInfo = mail.equips[i]
            PoolMgr:Get('Item',function (equip)
                equip.transform:SetParent(self.Img_receiveItems.transform)
                local equipBgIcon = EquipConfig[equipInfo.itemId].color
                ResMgr:LoadAssetSprite('itembg',{equipBgIcon},function (icon)
                    equip.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
                end)
                local equipIcon = EquipConfig[equipInfo.itemId].icon
                ResMgr:LoadAssetSprite('itemicon',{equipIcon},function (icon)
                    equip.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
                end)
                equip.transform:Find('Auto_itemCount'):GetComponent('Text').text = ''
                table.insert(receiveItems,equip)
            end)
        end
    end
end

function MailPanel:UpdateBagItem()

    --UIMgr:SetActive(self.Rect_mailView.transform,false)
    --UIMgr:SetActive(self.Rect_itemView.transform,true)

    BagItemOptimizeSV:Clear()
    
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

    BagItemOptimizeSV:GridLayout(self.Img_itemView.transform:GetComponent('RectTransform'),self.Img_items.transform:
        GetComponent('RectTransform'),100,100,#itemSum,0,0,0,0,0,0,ViewEnum.TopToBottom,CornerEnum.UpperLeft,'Item',
        function (index,item)
            ShowItem[itemSum[index+1]:GetItemType()](item,itemSum[index+1])
            self:ItemAddListener(item,itemSum[index+1])
            --ItemAddListener[currentGrid](item,itemSum[index+1])
        end,
        function (index,item)
            item.transform:Find('Auto_itemImg'):GetComponent('Image').color = Color.white
            PanelMgr:ToggleRemoveListener(item:GetComponent('Toggle'))
        end
    )

end
