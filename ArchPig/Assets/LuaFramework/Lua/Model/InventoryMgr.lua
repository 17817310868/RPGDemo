--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题：物品栏管理器
*
*        description:
*            功能描述:管理所有的物品栏中的物品
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

require "Enum/ItemEnum"
require "Model/Item"

--全局的物品栏管理器，管理所有玩家的背包管理器
--{物品栏类型{物品栏页签{物品类型{物品Id{物品Guid}}}}}
InventoryMgr = {}
local InventoryTable = {}
local this = InventoryMgr
--管理所有物品的Guid，存储每个Guid对应的itemId
local idMgr = {}
--管理所有物品的Guid，存储每个Guid对应的itemType
local typeMgr = {}
--管理所有物品的Guid，存储每个Guid对应的belongInventory
local inventoryMgr = {}


--获取onlyId对应的itemId
function InventoryMgr:GetId(Guid)
	if idMgr[Guid] == nil then
		log(Guid.."该Guid没有对应的itemId")
		return nil;
	end
	return idMgr[Guid];
end

--获取onlyId对应的itemType
function InventoryMgr:GetType(Guid)
	if typeMgr[Guid] == nil then
		log(Guid.."该Guid没有对应的itemClass")
		return nil;
	end
	return typeMgr[Guid];
end

--获取onlyId对应的belongInventory
function InventoryMgr:GetInventory(Guid)
	if inventoryMgr[Guid] == nil then
		log(Guid.."该Guid没有对应的belongInventory")
		return nil;
	end
	return inventoryMgr[Guid];
end


--[[
--添加新物品
function InventoryMgr.AddNewItem(roleId,belongInventory,inventoryPage,itemClass,itemId,onlyId,itemCount)
	item = ItemInfo.New(belongInventory,inventoryPage,itemClass,itemId,onlyId,itemCount)
	this.AddItem(roleId,item)
end
--]]

--添加物品
function InventoryMgr:AddItem(item)
	
	local inventory = item:GetItemInventory()
	local type = item:GetItemType()
	local id = item:GetItemId()
	local Guid = item:GetGuid()

	if 	InventoryTable[inventory] == nil then
		InventoryTable[inventory] = {}
	end
	if 	InventoryTable[inventory][type] == nil then
		InventoryTable[inventory][type] = {}
	end
	if 	InventoryTable[inventory][type][id] == nil then
		InventoryTable[inventory][type][id] = {}
	end
	if next(InventoryTable[inventory][type][id]) ~= nil then
		local oldItem = InventoryTable[inventory][type][id][next(InventoryTable[inventory][type][id])]
		if type == ItemClass.Item then
			oldItem:AddItemCount(item:GetItemCount())
			return
		end
	
		if type == ItemClass.Equip then

		end
		if type == ItemClass.Gem then
			oldItem:AddItemCount(item:GetItemCount())
			return
		end
		return;
	end
	if 	InventoryTable[inventory][type][id][Guid] ~= nil then
		return
	end

	idMgr[Guid] = id
	typeMgr[Guid] = type
	inventoryMgr[Guid] = inventory
	
	InventoryTable[inventory][type][id][Guid] = item

end

--添加多个物品
function InventoryMgr:AddItems(items)
	
	for index = 1,#items do 
		self:AddItem(items[index])
	end

	--[[
	UIMgr:Trigger('UpdateItem')
	UIMgr:Trigger('UpdateMakeBook')
	UIMgr:Trigger('UpdateInlayPanel')
	--]]
	
end

--删除物品
function InventoryMgr:RemoveItem(Guid)

	if inventoryMgr[Guid] == nil or typeMgr[Guid] == nil or idMgr[Guid] == nil then
		return
	end
	local inventory = inventoryMgr[Guid]
	local type = typeMgr[Guid]
	local id = idMgr[Guid]
	InventoryTable[inventory][type][id][Guid] = nil
	idMgr[Guid] = nil
	typeMgr[Guid] = nil
	inventoryMgr[Guid] = nil
	
	--[[
	UIMgr:Trigger('UpdateItem')
	UIMgr:Trigger('UpdateMakeBook')
	UIMgr:Trigger('UpdateInlayPanel')
	--]]

end


function InventoryMgr:RemoveItems(Guids)

	for index = 1,#Guids do
		self:RemoveItem(Guids[index])
	end

end

--减少物品
function InventoryMgr:ReduceItem(Guid,effect)

	if inventoryMgr[Guid] == nil or typeMgr[Guid] == nil or idMgr[Guid] == nil then
		return
	end
	local inventory = inventoryMgr[Guid]
	local type = typeMgr[Guid]
	local id = idMgr[Guid]
	if 	InventoryTable[inventory][type][id][Guid] == nil then
		return;
	end
	local Item = InventoryTable[inventory][type][id][Guid]
	if type == ItemClass.Item then
		Item:ReduceItemCount(effect)
		if Item:GetItemCount() > 0 then
			return
		end
	end
	if type == ItemClass.Equip then
		
	end
	if type == ItemClass.Gem then
		Item:ReduceItemCount(effect)
		if Item:GetItemCount() > 0 then
			return
		end
	end
	InventoryTable[inventory][type][id][Guid] = nil
	idMgr[Guid] = nil
	typeMgr[Guid] = nil
	inventoryMgr[Guid] = nil
	
	--[[
	UIMgr:Trigger('UpdateItem')
	UIMgr:Trigger('UpdateMakeBook')
	UIMgr:Trigger('UpdateInlayPanel')
	--]]

end

function InventoryMgr:ReduceItems(effects)
	for index = 1,#effects do
		self:ReduceItem(effects[index].Guid,effects[index].effect)
	end

end

function InventoryMgr:GetItem(Guid)

	local id = idMgr[Guid]
	local type = typeMgr[Guid]
	local inventory = inventoryMgr[Guid]

	return InventoryTable[inventory][type][id][Guid]

end


function InventoryMgr:GetTypeItems(inventory,type)
	if InventoryTable[inventory] == nil then
		return
	end
	if InventoryTable[inventory][type] == nil then
		return
	end
	local items = {}
	for key ,value in pairs(InventoryTable[inventory][type]) do
		for ikey, ivalue in pairs(value) do
			table.insert(items,ivalue)
		end
	end
	if #items == 0 then
		log("该角色物品栏不存在该类型物品")
		return nil;
	end
	return items;
end

function InventoryMgr:GetInventoryItem(inventory)
	if InventoryTable[inventory] == nil then
		return
	end
	local items = {}
	for key ,value in pairs(InventoryTable[inventory]) do
		for ikey, ivalue in pairs(value) do
			for xkey,xvalue in pairs(ivalue) do
				table.insert(items,xvalue)
			end
		end
	end

	if #items == 0 then
		log("该角色物品栏不存在该类型物品")
		return nil;
	end
	return items;
end