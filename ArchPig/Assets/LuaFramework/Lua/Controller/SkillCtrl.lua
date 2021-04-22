
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:技能面板控制层
*
*        description:
*            功能描述:实现技能面板具体功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

SkillCtrl = {}
local this = SkillCtrl

function SkillCtrl:Awake()

end

function SkillCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function SkillCtrl:Init()

end

function SkillCtrl:LearnSkill(skillId)
    Client.Send('LearnSkill',C2S_LearnSkill.New(skillId))
end

function SkillCtrl:UpgradeSkill(skillId)
    Client.Send('UpgradeSkill',C2S_UpgradeSkill.New(skillId))
end

--增加技能
function SkillCtrl:AddSkill(S2C_addSkill)
    local skillId = S2C_addSkill.skillId
    print(skillId)
    SkillMgr:AddSkill(skillId)
    UIMgr:Trigger('UpdateSkill')
end

--增加多个技能  S2C_addSkills.skills为List<S2C_addSkill>
function SkillCtrl:AddSkills(S2C_addSkills)
    local skills = S2C_addSkills.skills
    local count = skills.Count
    for index = 0,count-1 do
        SkillMgr:AddSkill(skills[index].skillId)
    end
    UIMgr:Trigger('UpdateSkill')
end

--移除技能
function SkillCtrl:RemoveSkill(S2C_removeSkill)
    local skillId = S2C_removeSkill.skillId
    SkillMgr:RemoveSkill(skillId)
    UIMgr:Trigger('UpdateSkill')
end

function SkillCtrl:UpdateSkill(S2C_updateSkill)
    local skillId = S2C_updateSkill.skillId
    SkillMgr:UpdateSkill(skillId)
    UIMgr:Trigger('UpdateSkill')
end