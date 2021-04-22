
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:物品信息面板控制层
*
*        description:
*            功能描述:实现物品信息面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

ItemInfoCtrl = {}
local this = ItemInfoCtrl

local gameObject
local transform

function ItemInfoCtrl:Awake()

end

function ItemInfoCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function ItemInfoCtrl:Init()

end

--使用物品
function ItemInfoCtrl:UseItem(Guid)

    Client.Send('UseItem',C2S_UseItem.New(Guid))

end

--丢弃物品
function ItemInfoCtrl:DiscardItem(Guid)

    Client.Send('DiscardItem',C2S_DiscardItem.New(Guid))

end

--穿戴装备
function ItemInfoCtrl:DressEquip(Guid)

    Client.Send('DressEquip',C2S_DressEquip.New(Guid))

end

--卸下装备
function ItemInfoCtrl:TakeoffEquip(Guid)

    Client.Send('TakeoffEquip',C2S_TakeoffEquip.New(Guid))

end

function ItemInfoCtrl:ShowItemInfo(itemGO,itemInfo)
    UIMgr:Trigger('ShowItemInfo',itemGO,itemInfo)
end