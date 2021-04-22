--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题：技能管理器
*
*        description:
*            功能描述:管理所拥有的技能的id
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


SkillMgr = {}
local this = SkillMgr
local SkillTable = {}

--增加技能
function SkillMgr:AddSkill(skillId)
    if self:GetSkill(skillId) ~= nil then
        log(tostring(skillId)..'该技能已存在')
    end
    table.insert(SkillTable,skillId)
end

--[[
--增加多个技能  S2C_addSkills.skills为List<S2C_addSkill>
function SkillMgr:AddSkills(S2C_addSkills)
    print("添加技能")
    local skills = S2C_addSkills.skills
    local count = skills.Count
    for index = 0,count do
        self:AddSkill(skills[index])
    end
end
--]]

--移除技能
function SkillMgr:RemoveSkill(skillId)
    local skill = self:GetSkill(skillId)
    if skill == nil then
        log(tostring(skillId)..'该技能不存在')
        return;
    end
    
    if self:IsExist(skill) == nil then
        log(tostring(skill)..'该技能id不存在')
        return;
    end
    table.remove(SkillTable,self:IsExist(skill))
end

function SkillMgr:UpdateSkill(skillId)
    self:RemoveSkill(skillId)
    self:AddSkill(skillId)
end

--判断技能id是否存在
function SkillMgr:IsExist(skillId)
    for index = 1,#SkillTable do
        if SkillTable[index] == skillId then
            log(tostring(skillId)..'该技能id已存在')
            return index;
        end
    end
    return nil;
end

function SkillMgr:GetAllSkills()
    if #SkillTable < 1 then
        log('没有技能')
        return nil
    end
    local skills = {}
    for index = 1,#SkillTable do 
        if SkillTable[index] ~= 0 then
            table.insert(skills,SkillTable[index])
        end
    end
    return skills
end

--配置表中每十位都是相同技能的不同等级配置，此方法用于判断是否存在相同技能
function SkillMgr:GetSkill(skillId)
    local shi = math.floor(skillId / 10) % 10
    for index = 1,#SkillTable do 
        if SkillTable[index] ~= 0 then
            if (math.floor(SkillTable[index] /10) % 10) == shi then
                return SkillTable[index]
            end
        end
    end
    return nil
end