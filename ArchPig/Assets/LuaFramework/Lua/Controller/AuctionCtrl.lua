
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:拍卖面板逻辑层
*
*        description:
*            功能描述:实现拍卖面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



AuctionCtrl = {}
local this = AuctionCtrl

local gameObject
local transform

function AuctionCtrl:Awake()

end

function AuctionCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function AuctionCtrl:Init()

end

function AuctionCtrl:Auction(biddingPrice,fixedPrice,time,Guid)

    Client.Send('Auction',C2S_Auction.New(Guid,biddingPrice,fixedPrice,time))

end

function AuctionCtrl:GetLotsInfos(itemClass)

    Client.Send('GetLotsInfos',C2S_GetLots.New(itemClass))

end

function AuctionCtrl:UpdateAuctionBox(S2C_lotsInfos)

    UIMgr:Trigger('UpdateAuctionBox',S2C_lotsInfos)

end

function AuctionCtrl:UpdateSellInfo()

    UIMgr:Trigger('UpdateSellInfo')

end

function AuctionCtrl:Search(type,name)
    Client.Send('SearchLots',C2S_SearchLots.New(type,name))
end

function AuctionCtrl:Bidding(Guid,type,money)
    Client.Send('Bidding',C2S_Bidding.New(Guid,type,money))
end

function AuctionCtrl:FixedBuy(Guid,type)
    Client.Send('FixedBuy',C2S_FixedBuy.New(Guid,type))
end