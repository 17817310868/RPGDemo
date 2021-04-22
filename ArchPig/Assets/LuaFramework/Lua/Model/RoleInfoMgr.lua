--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题：角色信息管理器
*
*        description:
*            功能描述:管理所有角色的信息
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]


RoleInfoMgr = {}
local RoleTable = {}
local this = RoleInfoMgr
local typeMgr = {}
local mainRoleGuid;

function RoleInfoMgr:AddMainRole(mainRole)
    log('增加主角信息')
    --[[
    local otherData = OtherData:New(S2C_mainRole.hp,S2C_mainRole.maxHp,S2C_mainRole.mp,
    S2C_mainRole.maxMp,S2C_mainRole.physicalAttack,S2C_mainRole.physicalDefense,S2C_mainRole.magicAttack,S2C_mainRole.magicDefense,
    S2C_mainRole.speed,S2C_mainRole.experience,S2C_mainRole.silver,S2C_mainRole.gold,S2C_mainRole.yuanBao)
    local mainRole = MainRoleInfo:New(S2C_mainRole.Guid,S2C_mainRole.roleType,S2C_mainRole.name,S2C_mainRole.level,
    S2C_mainRole.professionId,S2C_mainRole.schoolId,otherData)
    --]]
    local type = mainRole:GetType();
    local Guid = mainRole:GetGuid();
    if RoleTable[type] == nil then
        RoleTable[type] = {}
    end
    if RoleTable[type][Guid] ~= nil then
        log(Guid..'主角已存在');
        return;
    end
    mainRoleGuid = Guid;
    RoleTable[type][Guid] = mainRole
    typeMgr[Guid] = type
    print('增加主角信息完成')
    --log('初始化主ui面板')
end

function RoleInfoMgr:GetMainRoleGuid()
    return mainRoleGuid;
end

function RoleInfoMgr:GetMainRole()
    return self:GetRole(mainRoleGuid)
end

function RoleInfoMgr:AddRole(otherRole)

    --[[
    local otherRole = OtherRoleInfo:New(S2C_role.Guid,S2C_role.roleType,S2C_role.name,S2C_role.level,S2C_role.professionId,
    S2C_role.schoolId)
    --]]
    local type = otherRole:GetType();
    local Guid = otherRole:GetGuid();
    if RoleTable[type] == nil then
        RoleTable[type] = {}
    end
    if RoleTable[type][Guid] ~= nil then
        log(Guid..'该id对应的角色已存在');
        return;
    end
    RoleTable[type][Guid] = otherRole
    typeMgr[Guid] = type
end

function RoleInfoMgr:UpdateMainRole(mainRole)

    self:RemoveRole(mainRole:GetGuid())
    --[[
    local otherData = OtherData:New(S2C_mainRole.hp,S2C_mainRole.maxHp,S2C_mainRole.mp,
    S2C_mainRole.maxMp,S2C_mainRole.physicalAttack,S2C_mainRole.physicalDefense,S2C_mainRole.magicAttack,S2C_mainRole.magicDefense,
    S2C_mainRole.speed,S2C_mainRole.experience,S2C_mainRole.silver,S2C_mainRole.gold,S2C_mainRole.yuanBao)
    local mainRole = MainRoleInfo:New(S2C_mainRole.Guid,S2C_mainRole.roleType,S2C_mainRole.name,S2C_mainRole.level,
    S2C_mainRole.professionId,S2C_mainRole.schoolId,otherData)
    --]]
    local type = mainRole:GetType();
    local Guid = mainRole:GetGuid();
    if RoleTable[type] == nil then
        RoleTable[type] = {}
    end
    if RoleTable[type][Guid] ~= nil then
        log(Guid..'主角已存在');
        return;
    end
    mainRoleGuid = Guid;
    RoleTable[type][Guid] = mainRole
    typeMgr[Guid] = type

end

function RoleInfoMgr:RemoveRole(Guid)
    local role = self:GetRole(Guid)
    if role == nil then 
        return;
    end
    RoleTable[typeMgr[Guid]][Guid] = nil
    typeMgr[Guid] = nil
end

function RoleInfoMgr:ClearRole()
    for key,value in pairs(typeMgr) do
        self:RemoveRole(key)
    end
end

function RoleInfoMgr:GetRole(Guid)
    local type = typeMgr[Guid]
    if type == nil then
        log(Guid..'该Guid对应的角色类型不存在')
        return nil;
    end
    if RoleTable[type][Guid] == nil then
        log(Guid..'该Guid对应的角色不存在')
        return nil;
    end
    return RoleTable[type][Guid];
end


--角色结构
--其他玩家所拥有的数据
OtherRoleInfo = 
{
    Guid,  --角色的唯一标识
    type,  --角色类型
    name,  --角色名称
    level,  --角色等级
    profession,  --角色职业
    school,  --角色门派
    power  --角色战力
}
--OtherRoleInfo.__index = OtherRoleInfo;

function OtherRoleInfo:New(Guid,type,name,level,profession,school,power)

    local tab = {};
    --setmetatable(tab,OtherRoleInfo);
    tab.Guid = Guid;
    tab.type = type;
    tab.name = name;
    tab.level = level;
    tab.profession = profession;
    tab.school = school;
    tab.power = power;

    return {
        GetGuid = function ()
            return tab.Guid;
        end,
        GetType = function ()
            return tab.type;
        end,
        SetName = function (newName)
            tab.name = newName;
        end,
        GetName = function ()
            return tab.name;
        end,
        SetLevel = function (newLevel)
            tab.level = newLevel;
        end,
        GetLevel = function ()
            return tab.level
        end,
        SetProfession = function (newProfession)
            tab.profession = newProfession;
        end,
        GetProfession =function ()
            return tab.profession;
        end,
        SetSchool = function (newSchool)
            tab.school = newSchool;
        end,
        GetSchool = function ()
            return tab.school;
        end,
        SetPower = function (power)
            tab.power = power;
        end,
        GetPower = function ()
            return tab.power
        end
    }
end

--客户端主角所拥有的数据
MainRoleInfo = 
{
    otherData  --角色的其他属性
}
--setmetatable(MainRoleInfo,OtherRoleInfo)
--MainRoleInfo.__index = MainRoleInfo;

function MainRoleInfo:New(Guid,type,name,level,profession,school,power,otherData)

    local tab = OtherRoleInfo:New(Guid,type,name,level,profession,school,power);
    --setmetatable(tab,MainRoleInfo);
    tab.otherData = otherData;

    return {
        GetGuid = tab.GetGuid,
        GetType = tab.GetType,
        SetName = tab.SetName,
        GetName = tab.GetName,
        SetLevel = tab.SetLevel,
        GetLevel = tab.GetLevel,
        SetProfession = tab.SetProfession,
        GetProfession = tab.GetProfession,
        SetSchool = tab.SetSchool,
        GetSchool = tab.GetSchool,
        SetPower = tab.SetPower,
        GetPower = tab.GetPower,
        OtherData = tab.otherData
    };

end


--角色的其他属性(其他客户端玩家没有该属性,需要时从数据的请求)
OtherData = 
{
    hp,  --气血
    maxHp,  --气血上限
    mp,  --元气值
    maxMp,  --元气值上限
    physicalAttack,  --物理攻击
    physicalDefense,  --物理防御
    magicAttack,  --元气攻击
    magicDefense,  --元气防御
    speed,  --出手速度
    experience  --经验值
}

--OtherData.__index = OtherData;

function OtherData:New(hp,maxHp,mp,maxMp,physicalAttack,physicalDefense,magicAttack,magicDefense,speed,experience,silver,gold,yuanBao)

    local tab = {};
    --setmetatable(tab,OtherData);
    tab.hp = hp;
    tab.maxHp = maxHp;
    tab.mp = mp;
    tab.maxMp = maxMp;
    tab.physicalAttack = physicalAttack;
    tab.physicalDefense = physicalDefense;
    tab.magicAttack = magicAttack;
    tab.magicDefense = magicDefense;
    tab.speed = speed;
    tab.experience = experience;
    tab.silver = silver;
    tab.gold = gold
    tab.yuanBao = yuanBao
    return {
        SetHp = function (newHp)
            tab.hp = newHp;
        end,
        GetHp = function ()
            return tab.hp;
        end,
        SetMaxHp = function (newMaxHp)
            tab.maxHp = newMaxHp;
        end,
        GetMaxHp = function ()
            return tab.maxHp;
        end,
        SetMp = function (newMp)
            tab.mp = newMp;
        end,
        GetMp = function ()
            return tab.mp;
        end,
        SetMaxMp = function (newMaxMp)
            tab.maxMp = newMaxMp;
        end,
        GetMaxMp = function ()
            return tab.maxMp;
        end,
        SetPhysicalAttack = function (newPhysicalAttack)
            tab.physicalAttack = newPhysicalAttack;
        end,
        GetPhysicalAttack = function ()
            return tab.physicalAttack;
        end,
        SetPhysicalDefense = function (newPhysicalDefense)
            tab.physicalDefense = newPhysicalDefense;
        end,
        GetPhysicalDefense = function ()
            return tab.physicalDefense;
        end,
        SetMagicAttack = function (newMagicAttack)
            tab.magicAttack = newMaxMp;
        end,
        GetMagicAttack = function ()
            return tab.magicAttack;
        end,
        SetMagicDefense = function (newMagicDefense)
            tab.magicDefense = newMagicDefense;
        end,
        GetMagicDefense = function ()
            return tab.magicDefense;
        end,
        SetSpeed = function (newSpeed)
            tab.speed = newSpeed;
        end,
        GetSpeed = function ()
            return tab.speed;
        end,
        SetExperience = function (newExperience)
            tab.experience = newExperience;
        end,
        GetExperience = function ()
            return tab.experience;
        end,
        SetSilver = function (newSilver)
            tab.silver = newSilver
        end,
        GetSilver = function ()
            return tab.silver
        end,
        SetGold = function (newGold)
            tab.gold = newGold
        end,
        GetGold = function ()
            return tab.gold
        end,
        SetYuanBao = function (newYuanBao)
            tab.yuanBao = newYuanBao
        end,
        GetYuanBao = function ()
            return tab.yuanBao
        end
    };

end
