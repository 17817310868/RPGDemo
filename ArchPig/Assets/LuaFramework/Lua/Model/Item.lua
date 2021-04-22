--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题：物品信息类
*
*        description:
*            功能描述:设计各种物品信息
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

--物品基类
ItemBase = 
{
	itemId,  --物品id
	Guid,  --物品唯一标识
	itemType,  --物品类型
	inventory,  --所在物品栏
}

--ItemBase.__index = ItemBase

function ItemBase:New(inventory,itemType,itemId,Guid)
	local tab = {}
	--setmetatable(tab,ItemBase)
	tab.inventory = inventory
	tab.itemType = itemType
	tab.itemId = itemId
	tab.Guid = Guid
	return {
		GetItemId = function ()
			return tab.itemId;
		end,
		GetGuid = function ()
			return tab.Guid;
		end,
		GetItemType = function ()
			return tab.itemType;
		end,
		GetItemInventory = function ()
			return tab.inventory
		end,
	}
end

--普通物品信息

ItemInfo = {
	itemCount  --物品数量
}

--setmetatable(ItemInfo,ItemBase)
--ItemInfo.__index = ItemInfo

function ItemInfo:New(inventory,itemType,itemId,Guid,itemCount)
	local tab = ItemBase:New(inventory,itemType,itemId,Guid)
	--setmetatable(tab,ItemInfo)
	tab.itemCount = itemCount

	return {
		GetItemId = tab.GetItemId,
		GetGuid = tab.GetGuid,
		GetItemType = tab.GetItemType,
		GetItemInventory = tab.GetItemInventory,
		GetItemCount = function ()
			return tab.itemCount;
		end,
		AddItemCount = function (table,effect)
			tab.itemCount = tab.itemCount + effect
		end,
		ReduceItemCount = function (table,effect)
			tab.itemCount = tab.itemCount - effect
		end
	};
end

--装备物品信息

EquipInfo = {
	gems = {}  --用于存储装备所有动态属性的字典
}

--setmetatable(EquipInfo,ItemBase)
--EquipInfo.__index = EquipInfo

function EquipInfo:New(inventory,itemType,itemId,Guid,gems)
	local tab = ItemBase:New(inventory,itemType,itemId,Guid)
	--setmetatable(tab,EquipInfo)
	tab.gems = gems;
	return {
		GetItemId = tab.GetItemId,
		GetGuid = tab.GetGuid,
		GetItemType = tab.GetItemType,
		GetItemInventory = tab.GetItemInventory,
		SetGems = function (newGems)
			tab.gems = newGems;
		end,
		GetGems = function ()
			return tab.gems;
		end
	};
end

