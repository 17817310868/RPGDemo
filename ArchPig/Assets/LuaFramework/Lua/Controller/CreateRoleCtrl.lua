
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:创建角色面板控制层
*
*        description:
*            功能描述:实现创建角色面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

CreateRoleCtrl = {}
local this = CreateRoleCtrl

local gameObject
local transform



function CreateRoleCtrl:Awake()

end

function CreateRoleCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function CreateRoleCtrl:Init()
	--[[
    currentProfession = -1;
    currentSchool = -1;
	currentRole = nil;
	--]]
end

--[[
function CreateRoleCtrl:ChooseProfession(isChoose,professionId)

	if isChoose == false then
		return;
	end
	
	if currentProfession == professionId then
		return;
	end;
	
	--更新当前职业id
	currentProfession = professionId;

	currentSchool = SchoolEnum[currentProfession][1];
	--若上一个职业模型存在，则放进对象池
	if currentRole ~= nil then
		PoolMgr:Set(currentRole.name,currentRole);
	end
	--从配置表读取职业模型资源名，并从对象池取出模型
	local modelName = ProfessionConfig[professionId].model;
	PoolMgr:Get(modelName,function (obj)
		currentRole = obj;  --更新当前模型
	
		--初始化模型位置
		obj.transform.position = Vector3.New(-100,100,0);
		obj.transform:SetParent(nil);
	
		--获取武器绑定节点
		--local hand = obj.transform:Find('shannan_com/Bip001 Prop1/Slot_R_Hand')
		local hand = PoolMgr:GetChild(obj.transform,'Slot_R_Hand'); 
		--根据职业id，查看该模型下的武器绑定点下是否有该职业的武器
		local weapon = PoolMgr:GetChild(obj.transform,WeaponEnum[currentProfession]);
		--若不存在武器，则加载，若存在，即跳过
		if weapon == nil then
			--根据职业id，从配置表读取对应的武器资源名
			weaponName = ProfessionConfig[professionId].weapon;
			--从对象池取出武器模型
			PoolMgr:Get(weaponName,function(go)
				--初始化模型位置
				go.transform.position = hand.position;
				go.transform:SetParent(hand);
				go.transform.localPosition = Vector3.New(0,0,0);
				go.transform.localEulerAngles = Vector3.New(0,0,0);
				--根据不同职业做出调整
				--local x,y,z = ProfessionConfig[currentProfession].weaponPX,ProfessionConfig[currentProfession].weaponPY,ProfessionConfig[currentProfession].weaponPZ;
				--go.transform.localPosition = Vector3.New(x,y,z);
				--local x,y,z = ProfessionConfig[currentProfession].weaponEX,ProfessionConfig[currentProfession].weaponEY,ProfessionConfig[currentProfession].weaponEZ;
				--go.transform.localEulerAngles = Vector3.New(x,y,z);
			
			end)
		end
		--触发技能展示协程
		Client:StartCoroutine(StateMgr:ShowPose(obj))
		--coroutine.start(self:CoFunc(obj))
		--obj:GetComponent('Animation'):CrossFade('pose');
	end);
end



function CreateRoleCtrl:ChooseSchool(isChoose,number)

	--当玩家还没有选择职业时，不触发该函数
	if currentProfession == -1 then
		return;
	end
	
	--更新当前门派id
	currentSchool = SchoolEnum[currentProfession][number];
	
end

function CreateRoleCtrl:CoFunc (obj)
	local animation = obj:GetComponent('Animation')
	animation:CrossFade('pose')
	while animation.isPlaying == true do
		coroutine.step()
	end
	animation:CrossFade('stand')
end

--]]

function CreateRoleCtrl:CreateRole(nameText,currentProfession,currentSchool)
	Client.Send("CreateMainRole",C2S_CreateMainRole.New(nameText,currentProfession,currentSchool))
end