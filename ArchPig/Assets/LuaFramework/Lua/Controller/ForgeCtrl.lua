
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:锻造面板控制层
*
*        description:
*            功能描述:实现锻造面板具体功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



ForgeCtrl = {}
local this = ForgeCtrl

local gameObject
local transform

function ForgeCtrl:Awake()

end

function ForgeCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function ForgeCtrl:Init()

end

function ForgeCtrl:MakeEquip(Guid)

    Client.Send('MakeEquip',C2S_MakeEquip.New(Guid))

end

function ForgeCtrl:InlayGem(Guid,hole,gemGuid)

    Client.Send('InlayGem',C2S_InlayGem.New(Guid,hole,gemGuid))

end

function ForgeCtrl:RemoveGem(Guid,hole)

    Client.Send('RemoveGem',C2S_RemoveGem.New(Guid,hole))

end

function ForgeCtrl:Advance(Guid)

    Client.Send('AdvanceEquip',C2S_AdvanceEquip.New(Guid))

end
