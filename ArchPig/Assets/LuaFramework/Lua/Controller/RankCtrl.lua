
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:排行榜面板逻辑层
*
*        description:
*            功能描述:实现排行榜面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



RankCtrl = {}
local this = RankCtrl

local gameObject
local transform

function RankCtrl:Awake()

end

function RankCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function RankCtrl:Init()

end

function RankCtrl:ShowRanksInfo(S2C_ranksInfo)

    UIMgr:Trigger("ShowRanksInfo",S2C_ranksInfo.ranks)

end

function RankCtrl:GetRanksInfo(type)
    Client.Send("GetRanksInfo",C2S_CheckRank.New(type))
end