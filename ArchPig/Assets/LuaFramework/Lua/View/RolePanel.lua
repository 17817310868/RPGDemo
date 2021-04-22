
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:角色面板视图层
*
*        description:
*            功能描述:实现角色面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


RolePanel = {}
local this = RolePanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
RolePanel.Ctrls = 
{
    Img_schoolIcon = {  Path = 'Auto_schoolIcon',  ControlType = 'Image'  },
    Img_equipContent = {  Path = 'Auto_equipContent',  ControlType = 'Image'  },
    Img_helmet = {  Path = 'Auto_equipContent/Auto_helmet',  ControlType = 'Image'  },
    Img_weapon = {  Path = 'Auto_equipContent/Auto_weapon',  ControlType = 'Image'  },
    Img_belt = {  Path = 'Auto_equipContent/Auto_belt',  ControlType = 'Image'  },
    Img_necklace = {  Path = 'Auto_equipContent/Auto_necklace',  ControlType = 'Image'  },
    Img_clothes = {  Path = 'Auto_equipContent/Auto_clothes',  ControlType = 'Image'  },
    Img_shoes = {  Path = 'Auto_equipContent/Auto_shoes',  ControlType = 'Image'  },
    Img_close = {  Path = 'Auto_close',  ControlType = 'Image'  },
    Text_name = {  Path = 'Auto_name',  ControlType = 'Text'  },
    Text_levelText = {  Path = 'Auto_levelText',  ControlType = 'Text'  },
    Text_power = {  Path = 'power/Auto_power',  ControlType = 'Text'  },
    Btn_close = {  Path = 'Auto_close',  ControlType = 'Button'  },
}

local ProfessionConfig = require 'Config/ProfessionConfig'
local SchoolConfig = require 'Config/SchoolConfig'
local EquipConfig = require 'Config/EquipConfig'

local gameObject
local transform

local currentRole
local equips  --用于存储装备对象

local roleInfoObserver  --角色信息监听观察者

function RolePanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function RolePanel:Init()

    roleInfoObserver = Observer:New(function (S2C_info)
        self:ShowInfo(S2C_info)
    end)

    equips = {}
    PanelMgr:ButtonAddListener(self.Btn_close,function ()
        PanelMgr:ClosePanel()
    end)
end

function RolePanel:Show()
    UIMgr:AddListener('ShowRoleInfo',roleInfoObserver)
end

function RolePanel:Hide()
    UIMgr:RemoveListener('ShowRoleInfo',roleInfoObserver)
    if #equips ~= 0 then
        for index = 1,#equips do
            PanelMgr:ToggleRemoveListener(equips[index].transform:GetComponent('Toggle'))
            PoolMgr:Set(equips[index].name,equips[index])
        end
    end
    PoolMgr:Set(currentRole.name,currentRole)
    currentRole = nil
    equips = {}

end

--装备类型枚举对应的装备节点
local EquipRoot = {
	[1] = function ()
		return this.Img_helmet.transform
	end,
	[2] = function ()
		return this.Img_necklace.transform
	end,
	[3] = function ()
		return this.Img_weapon.transform
	end,
	[4] = function ()
		return this.Img_clothes.transform
	end,
	[5] = function ()
		return this.Img_belt.transform
	end,
	[6] = function ()
		return this.Img_shoes.transform
	end
}

function RolePanel:ShowInfo(S2C_info)

    local roleData = RoleInfoMgr:GetRole(S2C_info.Guid)
    self.Text_levelText.text = roleData:GetLevel()
	self.Text_name.text = roleData:GetName()
	ResMgr:LoadAssetSprite('schoolicon',{SchoolConfig[roleData:GetSchool()].schoolIconTrue},function (icon)
        self.Img_schoolIcon.sprite = icon
    end);
    self.Text_power.text = tostring(roleData:GetPower())
    
    PoolMgr:Get(ProfessionConfig[roleData:GetProfession()].model,function (role)
        currentRole = role
        role.transform.position = Vector3.New(0,0,0)
        role.transform.localEulerAngles = Vector3.New(0,0,0)
        local actor = Actor.New(role)
        
        if S2C_info.equips.Count == 0 then
            return
        end

        for index = 0,S2C_info.equips.Count-1 do
            actor:UpdateBind(S2C_info.equips[index].itemId)
            PoolMgr:Get('Item',function (equip)
                table.insert(equips,equip)
                equip.transform:GetComponent('button')
                ResMgr:LoadAssetSprite('itembg',{EquipConfig[S2C_info.equips[index].itemId].color},function (icon)
                    equip.transform:Find('Auto_itemBg'):GetComponent('Image').sprite = icon	
                end);
                equip.transform:Find('Auto_itemCount'):GetComponent('Text').text = ''
                ResMgr:LoadAssetSprite('itemicon',{EquipConfig[S2C_info.equips[index].itemId].icon},function (icon)
                    equip.transform:Find('Auto_itemImg'):GetComponent('Image').sprite = icon	
                end);
                equip.transform:SetParent(EquipRoot[EquipConfig[S2C_info.equips[index].itemId]._type]())
                PanelMgr:ToggleAddListener(equip.transform:GetComponent('Toggle'),function (isChoose)
                    PanelMgr:OpenPanel('ItemInfoPanel',UILayer.Top,MaskEnum.TipsMask,function (itemInfoPanel)
                        ItemInfoCtrl:ShowItemInfo(equip,EquipInfo:New(InventoryEnum.OtherRoleEquip,S2C_info.equips[index].itemType,
                        S2C_info.equips[index].itemId,S2C_info.equips[index].Guid,S2C_info.equips[index].gems))
                    end)
                end)
            end)
        end
    end)
end