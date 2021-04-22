
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:战斗UI面板控制层
*
*        description:
*            功能描述:实现战斗ui面板具体功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



BattleCtrl = {}
local this = BattleCtrl

local gameObject
local transform

local playerId --存放切磋玩家id

function BattleCtrl:Awake()
    
end

function BattleCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function BattleCtrl:Init()

end

function BattleCtrl:StartBattle()

    PanelMgr:ClosePanel();
    PanelMgr:OpenPanel('BattlePanel',UILayer.Middle,MaskEnum.None)

end