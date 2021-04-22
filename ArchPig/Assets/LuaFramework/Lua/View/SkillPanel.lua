
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:技能面板视图层
*
*        description:
*            功能描述:实现技能面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


SkillPanel = {}
local this = SkillPanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
SkillPanel.Ctrls = 
{
    Img_oneIcon = {  Path = 'skillBar/skillOne/Auto_oneIcon',  ControlType = 'Image'  },
    Img_twoIcon = {  Path = 'skillBar/skillTwo/Auto_twoIcon',  ControlType = 'Image'  },
    Img_threeIcon = {  Path = 'skillBar/skillThree/Auto_threeIcon',  ControlType = 'Image'  },
    Img_fourIcon = {  Path = 'skillBar/skillFour/Auto_fourIcon',  ControlType = 'Image'  },
    Img_fiveIcon = {  Path = 'skillBar/skillFive/Auto_fiveIcon',  ControlType = 'Image'  },
    Img_sixIcon = {  Path = 'skillBar/skillSix/Auto_sixIcon',  ControlType = 'Image'  },
    Img_sevenIcon = {  Path = 'skillBar/skillSeven/Auto_sevenIcon',  ControlType = 'Image'  },
    Img_eightIcon = {  Path = 'skillBar/skillEight/Auto_eightIcon',  ControlType = 'Image'  },
    Img_nineIcon = {  Path = 'skillBar/skillNine/Auto_nineIcon',  ControlType = 'Image'  },
    Img_upgradeBtn = {  Path = 'skillInfo/btn/Auto_upgradeBtn',  ControlType = 'Image'  },
    Img_upgradesBtn = {  Path = 'skillInfo/btn/Auto_upgradesBtn',  ControlType = 'Image'  },
    Img_learnBtn = {  Path = 'skillInfo/btn/Auto_learnBtn',  ControlType = 'Image'  },
    Img_scrollbar = {  Path = 'skillInfo/mask/Auto_scrollbar',  ControlType = 'Image'  },
    Img_skillInfo = {  Path = 'skillInfo/mask/Auto_skillInfo',  ControlType = 'Image'  },
    Img_money = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_money',  ControlType = 'Image'  },
    Img_closeBtn = {  Path = 'Auto_closeBtn',  ControlType = 'Image'  },
    Text_oneName = {  Path = 'skillBar/skillOne/Auto_oneName',  ControlType = 'Text'  },
    Text_oneLevel = {  Path = 'skillBar/skillOne/Auto_oneLevel',  ControlType = 'Text'  },
    Text_twoName = {  Path = 'skillBar/skillTwo/Auto_twoName',  ControlType = 'Text'  },
    Text_twoLevel = {  Path = 'skillBar/skillTwo/Auto_twoLevel',  ControlType = 'Text'  },
    Text_threeName = {  Path = 'skillBar/skillThree/Auto_threeName',  ControlType = 'Text'  },
    Text_threelevel = {  Path = 'skillBar/skillThree/Auto_threelevel',  ControlType = 'Text'  },
    Text_fourName = {  Path = 'skillBar/skillFour/Auto_fourName',  ControlType = 'Text'  },
    Text_fourLevel = {  Path = 'skillBar/skillFour/Auto_fourLevel',  ControlType = 'Text'  },
    Text_fiveName = {  Path = 'skillBar/skillFive/Auto_fiveName',  ControlType = 'Text'  },
    Text_fiveLevel = {  Path = 'skillBar/skillFive/Auto_fiveLevel',  ControlType = 'Text'  },
    Text_sixName = {  Path = 'skillBar/skillSix/Auto_sixName',  ControlType = 'Text'  },
    Text_sixLevel = {  Path = 'skillBar/skillSix/Auto_sixLevel',  ControlType = 'Text'  },
    Text_sevenName = {  Path = 'skillBar/skillSeven/Auto_sevenName',  ControlType = 'Text'  },
    Text_sevenLevel = {  Path = 'skillBar/skillSeven/Auto_sevenLevel',  ControlType = 'Text'  },
    Text_eightName = {  Path = 'skillBar/skillEight/Auto_eightName',  ControlType = 'Text'  },
    Text_eightLevel = {  Path = 'skillBar/skillEight/Auto_eightLevel',  ControlType = 'Text'  },
    Text_nineName = {  Path = 'skillBar/skillNine/Auto_nineName',  ControlType = 'Text'  },
    Text_nineLevel = {  Path = 'skillBar/skillNine/Auto_nineLevel',  ControlType = 'Text'  },
    Text_skillName = {  Path = 'skillInfo/Auto_skillName',  ControlType = 'Text'  },
    Text_skillIntroduce = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_skillIntroduce',  ControlType = 'Text'  },
    Text_effectInfo = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_effectInfo',  ControlType = 'Text'  },
    Text_consume = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_consume',  ControlType = 'Text'  },
    Text_CD = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_CD',  ControlType = 'Text'  },
    Text_nextTitle = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_nextTitle',  ControlType = 'Text'  },
    Text_nextInfo = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_nextInfo',  ControlType = 'Text'  },
    Text_levelRequire = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_levelRequire',  ControlType = 'Text'  },
    Text_haveMoney = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_money/Auto_haveMoney',  ControlType = 'Text'  },
    Text_requireMoney = {  Path = 'skillInfo/mask/Auto_skillInfo/Auto_money/Auto_requireMoney',  ControlType = 'Text'  },
    Tog_oneIcon = {  Path = 'skillBar/skillOne/Auto_oneIcon',  ControlType = 'Toggle'  },
    Tog_twoIcon = {  Path = 'skillBar/skillTwo/Auto_twoIcon',  ControlType = 'Toggle'  },
    Tog_threeIcon = {  Path = 'skillBar/skillThree/Auto_threeIcon',  ControlType = 'Toggle'  },
    Tog_fourIcon = {  Path = 'skillBar/skillFour/Auto_fourIcon',  ControlType = 'Toggle'  },
    Tog_fiveIcon = {  Path = 'skillBar/skillFive/Auto_fiveIcon',  ControlType = 'Toggle'  },
    Tog_sixIcon = {  Path = 'skillBar/skillSix/Auto_sixIcon',  ControlType = 'Toggle'  },
    Tog_sevenIcon = {  Path = 'skillBar/skillSeven/Auto_sevenIcon',  ControlType = 'Toggle'  },
    Tog_eightIcon = {  Path = 'skillBar/skillEight/Auto_eightIcon',  ControlType = 'Toggle'  },
    Tog_nineIcon = {  Path = 'skillBar/skillNine/Auto_nineIcon',  ControlType = 'Toggle'  },
    Btn_upgradeBtn = {  Path = 'skillInfo/btn/Auto_upgradeBtn',  ControlType = 'Button'  },
    Btn_upgradesBtn = {  Path = 'skillInfo/btn/Auto_upgradesBtn',  ControlType = 'Button'  },
    Btn_learnBtn = {  Path = 'skillInfo/btn/Auto_learnBtn',  ControlType = 'Button'  },
    Btn_closeBtn = {  Path = 'Auto_closeBtn',  ControlType = 'Button'  },
    Scrollbar_scrollbar = {  Path = 'skillInfo/mask/Auto_scrollbar',  ControlType = 'Scrollbar'  },
}

local SkillConfig = require "Config/SkillConfig"

local gameObject
local transform

local currentSkillId

local skillObserver  --技能观察者

function SkillPanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function SkillPanel:Init()

    skillObserver = Observer:New(function ()
        self:Hide()
        self:Show()
    end)

    --关闭按钮
    PanelMgr:ButtonAddListener(self.Btn_closeBtn, function ()
        PanelMgr:ClosePanel()
    end)

    UIMgr:SetActive(self.Btn_learnBtn.transform,false)
    UIMgr:SetActive(self.Btn_upgradeBtn.transform,false)
    UIMgr:SetActive(self.Img_skillInfo.transform,false)

    PanelMgr:ButtonAddListener(self.Btn_learnBtn,function ()
        AudioMgr:PlayEffect('button')
        local roleData = RoleInfoMgr:GetMainRole()  --获取角色当前信息
        if currentSkillId == nil then
            return
        end
        if roleData.OtherData:GetSilver() < SkillConfig[currentSkillId].upgradeMoney then
            return
        end

        if roleData:GetLevel() < SkillConfig[currentSkillId].upgradeLevel then
            return
        end

        SkillCtrl:LearnSkill(currentSkillId)

    end)

    PanelMgr:ButtonAddListener(self.Btn_upgradeBtn,function ()
        AudioMgr:PlayEffect('button')
        local roleData = RoleInfoMgr:GetMainRole()  --获取角色当前信息
        if currentSkillId == nil then
            return
        end
        if roleData.OtherData:GetSilver() < SkillConfig[currentSkillId].upgradeMoney then
            return
        end

        if roleData:GetLevel() < SkillConfig[currentSkillId].upgradeLevel then
            return
        end

        SkillCtrl:UpgradeSkill(currentSkillId)
    end)
end

--获取技能格子相关物体
local SkillBarSwitch = {
    [0] = function ()
        return this.Tog_oneIcon,this.Img_oneIcon,this.Text_oneName,this.Text_oneLevel
    end,
    [1] = function ()
        return this.Tog_twoIcon,this.Img_twoIcon,this.Text_twoName,this.Text_twoLevel
    end,
    [2] = function ()
        return this.Tog_threeIcon,this.Img_threeIcon,this.Text_threeName,this.Text_threelevel
    end,
    [3] = function ()
        return this.Tog_fourIcon,this.Img_fourIcon,this.Text_fourName,this.Text_fourLevel
    end,
    [4] = function ()
        return this.Tog_fiveIcon,this.Img_fiveIcon,this.Text_fiveName,this.Text_fiveLevel
    end,
    [5] = function ()
        return this.Tog_sixIcon,this.Img_sixIcon,this.Text_sixName,this.Text_sixLevel
    end,
    [6] = function ()
        return this.Tog_sevenIcon,this.Img_sevenIcon,this.Text_sevenName,this.Text_sevenLevel
    end,
    [7] = function ()
        return this.Tog_eightIcon,this.Img_eightIcon,this.Text_eightName,this.Text_eightLevel
    end,
    [8] = function ()
        return this.Tog_nineIcon,this.Img_nineIcon,this.Text_nineName,this.Text_nineLevel
    end
}

function SkillPanel:Show()

    --self.Text_skillName.text = ""
    --UIMgr:SetActive(self.Img_skillInfo.transform,false)
    --UIMgr:SetActive(self.Btn_learnBtn.transform,false)
    --UIMgr:SetActive(self.Btn_upgradeBtn.transform,false)
    --UIMgr:SetActive(self.Btn_upgradeBtn.transform,false)

    local roleData = RoleInfoMgr:GetMainRole()  --获取角色当前信息
    local profession = roleData:GetProfession()  --获取玩家职业
    for index = 0,8 do  --技能配置表百位数为职业id，十位数为技能id，个位数为技能等级
        local skillId = (profession * 100) + (index * 10)  --获取等级为0的该技能
        if SkillMgr:GetSkill(skillId) ~= nil and SkillMgr:GetSkill(skillId) ~= 0 then  --判断该玩家是否拥有该技能的其他等级
            skillId = SkillMgr:GetSkill(skillId)  --更新技能id
        end
        local Tog_icon,Img_icon,Text_name,Text_level = SkillBarSwitch[index]()  --获取该技能栏位相关物体
        local skillLevel = SkillConfig[skillId].skillLevel  --获取技能等级
        ResMgr:LoadAssetSprite('skillicon',{SkillConfig[skillId].icon},function (icon)
            Img_icon.sprite = icon  --加载技能图标并赋值
            if skillLevel == 0 then  --判断技能等级是否为0
                Img_icon.color = Color:New(105/255,105/255,105/255,1)  --将图片设置为灰色
            else
                Img_icon.color = Color.white  --将图片设置为白色
            end
        end);
        Text_name.text = SkillConfig[skillId].name  --更新技能名称
        Text_level.text = skillLevel  --更新技能等级
        PanelMgr:ToggleAddListener(Tog_icon,function (isChoose)  --添加监听
            AudioMgr:PlayEffect('button')
            if isChoose == false then
                return;
            end
            currentSkillId = skillId  --更新当前技能id
            
            if skillLevel == 0 or skillLevel > 3 then  --根据技能等级显示对应的按钮和隐藏按钮
                if skillLevel == 0 then
                    UIMgr:SetActive(self.Btn_learnBtn.transform,true)
                    UIMgr:SetActive(self.Btn_upgradeBtn.transform,false)
                end
                if skillLevel > 3 then
                    UIMgr:SetActive(self.Btn_learnBtn.transform,false)
                    UIMgr:SetActive(self.Btn_upgradeBtn.transform,false)
                end
            else 
                UIMgr:SetActive(self.Btn_upgradeBtn.transform,true)
            end
            self:ShowSkillInfo(isChoose)  --显示技能具体信息
        end)
    end

    UIMgr:AddListener('UpdateSkill',skillObserver)

end

function SkillPanel:Hide()

    UIMgr:RemoveListener('UpdateSkill',skillObserver)

    for index = 0,8 do 
        Tog_icon = SkillBarSwitch[index]()
        PanelMgr:ToggleRemoveListener(Tog_icon)
    end
    self.Text_skillName.text = ""
    UIMgr:SetActive(self.Btn_learnBtn.transform,false)
    UIMgr:SetActive(self.Btn_upgradeBtn.transform,false)
    UIMgr:SetActive(self.Img_skillInfo.transform,false)

end

function SkillPanel:ShowSkillInfo(isChoose)
    
    if isChoose == false then
        return;
    end
    UIMgr:SetActive(self.Img_skillInfo.transform,true)
    local roleData = RoleInfoMgr:GetMainRole()  --获取角色当前信息
    self.Text_skillName.text = SkillConfig[currentSkillId].name  --更新技能名称
    self.Text_skillIntroduce.text = SkillConfig[currentSkillId].introduce  --更新技能简介
    self.Text_effectInfo.text = SkillConfig[currentSkillId].effect  --更新技能效果
    self.Text_consume.text = '消耗MP:'..tostring(SkillConfig[currentSkillId].consume)  --更新技能消耗mp
    self.Text_CD.text = '冷却回合数:'..tostring(SkillConfig[currentSkillId].CD)  --更新技能cd
    if SkillConfig[currentSkillId].skillLevel > 3 then  --判断技能等级是否大于3
        UIMgr:SetActive(self.Text_nextInfo.transform,false)  --隐藏下一级技能效果
        UIMgr:SetActive(self.Img_money.transform,false)  --隐藏金币
        UIMgr:SetActive(self.Text_levelRequire.transform,false)  --隐藏升级所需金币
        UIMgr:SetActive(self.Text_nextTitle.transform,false)  --隐藏下一级
        --刷新布局组件
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(self.Img_skillInfo.transform:GetComponent('RectTransform'))
        return;
    end

    UIMgr:SetActive(self.Img_money.transform,true)  --显示金币
    self.Text_haveMoney.text = roleData.OtherData:GetSilver()
    self.Text_requireMoney.text = SkillConfig[currentSkillId].upgradeMoney  --更新升级技能所需金币
    if roleData.OtherData:GetSilver() >= SkillConfig[currentSkillId].upgradeMoney then
        self.Text_requireMoney.color = Color.green
    else
        self.Text_requireMoney.color = Color.Red
    end
    UIMgr:SetActive(self.Text_levelRequire.transform,true)  --显示升级技能所需金币
    self.Text_levelRequire.text = '需求等级:'.. SkillConfig[currentSkillId].upgradeLevel  --更新技能升级需求等级
    if roleData:GetLevel() >= SkillConfig[currentSkillId].upgradeLevel then
        self.Text_levelRequire.color = Color.green
    else
        self.Text_levelRequire.color = Color.red
    end
    if SkillConfig[currentSkillId].skillLevel == 0 then  --判断技能等级是否为0
        UIMgr:SetActive(self.Text_nextInfo.transform,false)  --隐藏下一级技能效果
        UIMgr:SetActive(self.Text_nextTitle.transform,false)  --隐藏下一级
        --刷新布局组件
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(self.Img_skillInfo.transform:GetComponent('RectTransform'))
        return;
    end
    UIMgr:SetActive(self.Text_nextTitle.transform,true)  --显示下一级
    UIMgr:SetActive(self.Text_nextInfo.transform,true)  --显示下一级技能效果
    self.Text_nextInfo.text = SkillConfig[currentSkillId].upgradeInfo  --更新下一级技能效果
    --刷新布局组件
    UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(self.Img_skillInfo.transform:GetComponent('RectTransform'))
end
    
