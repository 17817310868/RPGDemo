
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:商城面板控制层
*
*        description:
*            功能描述:实现商城面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



ShopCtrl = {}
local this = ShopCtrl

local gameObject
local transform

function ShopCtrl:Awake()

end

function ShopCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function ShopCtrl:Init()

end

function ShopCtrl:Buy(itemType,itemId)

    if itemType == ItemClass.Item then
        Client.Send("BuyItem",C2S_BuyItem.New(itemId))
        return
    end
    if itemType == ItemClass.Equip then
        Client.Send("BuyEquip",C2S_BuyItem.New(itemId))
        return
    end
    if itemType == ItemClass.Gem then
        Client.Send("BuyGem",C2S_BuyItem.New(itemId))
        return
    end

end