
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:背包面板控制层
*
*        description:
*            功能描述:实现背包面板功能具体逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



BagCtrl = {}
local this = BagCtrl

local gameObject
local transform

function BagCtrl:Awake()

end

function BagCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function BagCtrl:Init()

end

function BagCtrl:AddItems(S2C_addItems)

    local items = {}
    local count = S2C_addItems.items.Count;
	for index = 0,count-1 do 
        local item = nil;
        local S2C_addItem = S2C_addItems.items[index]
	    if S2C_addItem.itemType == ItemClass.Item then
		    item = ItemInfo:New(S2C_addItem.inventory,S2C_addItem.itemType,S2C_addItem.itemId,S2C_addItem.Guid,S2C_addItem.count)
	    end
	    if S2C_addItem.itemType == ItemClass.Equip then
		    item = EquipInfo:New(S2C_addItem.inventory,S2C_addItem.itemType,S2C_addItem.itemId,S2C_addItem.Guid,S2C_addItem.gems)
	    end
	    if S2C_addItem.itemType == ItemClass.Gem then
		    item = ItemInfo:New(S2C_addItem.inventory,S2C_addItem.itemType,S2C_addItem.itemId,S2C_addItem.Guid,S2C_addItem.count)
        end
        table.insert(items,item)
    end
    InventoryMgr:AddItems(items)
    --刷新背包物品
    UIMgr:Trigger('UpdateBagItems')
    UIMgr:Trigger('UpdateRoleInfo')
    
end

function BagCtrl:ReduceItems(S2C_reduceItems)
    local effects = {}
    for index = 0,S2C_reduceItems.items.Count-1 do
        table.insert(effects,{Guid = S2C_reduceItems.items[index].Guid,effect = S2C_reduceItems.items[index].effect})
    end
    if #effects == 0 then
        return
    end
    InventoryMgr:ReduceItems(effects)
    UIMgr:Trigger('UpdateBagItems')
    UIMgr:Trigger('UpdateRoleInfo')
end

function BagCtrl:RemoveItems(S2C_removeItems)

    local Guids = {}
    local isUpdateBag = false
    local isUpdateEquip = false
    for index = 0,S2C_removeItems.Guids.Count-1 do
        local Guid = S2C_removeItems.Guids[index].Guid
        table.insert(Guids,S2C_removeItems.Guids[index].Guid)
    end
    InventoryMgr:RemoveItems(Guids)
    UIMgr:Trigger('UpdateBagItems')
    UIMgr:Trigger('UpdateRoleInfo')
end

function BagCtrl:UpdateItem(S2C_itemInfo)
    local item = nil
    if S2C_itemInfo.itemType == ItemClass.Item then
        item = ItemInfo:New(S2C_itemInfo.inventory,S2C_itemInfo.itemType,S2C_itemInfo.itemId,S2C_itemInfo.Guid,S2C_itemInfo.count)
    end
    if S2C_itemInfo.itemType == ItemClass.Equip then
        item = EquipInfo:New(S2C_itemInfo.inventory,S2C_itemInfo.itemType,S2C_itemInfo.itemId,S2C_itemInfo.Guid,S2C_itemInfo.gems)
    end
    if S2C_itemInfo.itemType == ItemClass.Gem then
        item = ItemInfo:New(S2C_itemInfo.inventory,S2C_itemInfo.itemType,S2C_itemInfo.itemId,S2C_itemInfo.Guid,S2C_itemInfo.count)
    end

    InventoryMgr:RemoveItem(S2C_itemInfo.Guid)
    
    InventoryMgr:AddItem(item)

    UIMgr:Trigger('UpdateBagItems')
    UIMgr:Trigger('UpdateRoleInfo')
    --UIMgr:Trigger('')
end


function BagCtrl:UpdateRoleInfo()

    BagPanel:UpdateRoleInfo()

end

function BagCtrl:UpdateBaseData()

    BagPanel:UpdateBaseData()

end
