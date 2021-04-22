
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:创建角色面板视图层
*
*        description:
*            功能描述:实现创建角色面板表现逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

CreateRolePanel = {}
local this = CreateRolePanel

--此表是自动生成的，切勿修改(该表存储该面板下所有需要用到的控件)
CreateRolePanel.Ctrls = 
{
    Img_nanQiangToggle = {  Path = 'profession/Auto_nanQiangToggle',  ControlType = 'Image'  },
    Img_nvGongToggle = {  Path = 'profession/Auto_nvGongToggle',  ControlType = 'Image'  },
    Img_nanShanToggle = {  Path = 'profession/Auto_nanShanToggle',  ControlType = 'Image'  },
    Img_nvZhangToggle = {  Path = 'profession/Auto_nvZhangToggle',  ControlType = 'Image'  },
    Img_nanJianToggle = {  Path = 'profession/Auto_nanJianToggle',  ControlType = 'Image'  },
    Img_schoolFirstTog = {  Path = 'school/Auto_schoolFirstTog',  ControlType = 'Image'  },
    Img_schoolSecondTog = {  Path = 'school/Auto_schoolSecondTog',  ControlType = 'Image'  },
    Img_schoolInfoBg = {  Path = 'school/Auto_schoolInfoBg',  ControlType = 'Image'  },
    Img_nameInput = {  Path = 'Auto_nameInput',  ControlType = 'Image'  },
    Img_beginBtn = {  Path = 'Auto_beginBtn',  ControlType = 'Image'  },
    Text_schoolInfoText = {  Path = 'school/Auto_schoolInfoBg/Auto_schoolInfoText',  ControlType = 'Text'  },
    Tog_nanQiangToggle = {  Path = 'profession/Auto_nanQiangToggle',  ControlType = 'Toggle'  },
    Tog_nvGongToggle = {  Path = 'profession/Auto_nvGongToggle',  ControlType = 'Toggle'  },
    Tog_nanShanToggle = {  Path = 'profession/Auto_nanShanToggle',  ControlType = 'Toggle'  },
    Tog_nvZhangToggle = {  Path = 'profession/Auto_nvZhangToggle',  ControlType = 'Toggle'  },
    Tog_nanJianToggle = {  Path = 'profession/Auto_nanJianToggle',  ControlType = 'Toggle'  },
    Tog_schoolFirstTog = {  Path = 'school/Auto_schoolFirstTog',  ControlType = 'Toggle'  },
    Tog_schoolSecondTog = {  Path = 'school/Auto_schoolSecondTog',  ControlType = 'Toggle'  },
    Btn_beginBtn = {  Path = 'Auto_beginBtn',  ControlType = 'Button'  },
    Input_nameInput = {  Path = 'Auto_nameInput',  ControlType = 'InputField'  },
}

local ProfessionConfig = require "Config/ProfessionConfig"
local SchoolConfig = require "Config/SchoolConfig"

local gameObject
local transform

--当前选择的职业Id
local currentProfession;

--当前选择的门派Id
local currentSchool;

--当前显示的角色模型
local currentRole;

function CreateRolePanel:Awake(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function CreateRolePanel:Init()

	currentProfession = -1;
    currentSchool = -1;
    currentRole = nil;

    --给各个职业选择按钮添加监听事件，并传递职业枚举，和默认选择的门派按钮
	PanelMgr:ToggleAddListener(self.Tog_nanQiangToggle,function (isChoose)
		AudioMgr:PlayEffect('button')
		self:ChooseProfession(isChoose,self.Img_nanQiangToggle,ProfessionEnum.NanQiang)
		--CreateRoleCtrl:ChooseProfession(isChoose,ProfessionEnum.NanQiang);
	end);

	PanelMgr:ToggleAddListener(self.Tog_nvGongToggle,function (isChoose)
		AudioMgr:PlayEffect('button')
		self:ChooseProfession(isChoose,self.Img_nvGongToggle,ProfessionEnum.NvGong)
		--CreateRoleCtrl:ChooseProfession(isChoose,ProfessionEnum.NvGong);
	end);
	
	PanelMgr:ToggleAddListener(self.Tog_nanShanToggle,function (isChoose)
		AudioMgr:PlayEffect('button')
		self:ChooseProfession(isChoose,self.Img_nanShanToggle,ProfessionEnum.NanShan)
		--CreateRoleCtrl:ChooseProfession(isChoose,ProfessionEnum.NanShan);
	end);
	
	PanelMgr:ToggleAddListener(self.Tog_nvZhangToggle,function (isChoose)
		AudioMgr:PlayEffect('button')
		self:ChooseProfession(isChoose,self.Img_nvZhangToggle,ProfessionEnum.NvZhang)
		--CreateRoleCtrl:ChooseProfession(isChoose,ProfessionEnum.NvZhang);
	end);

	PanelMgr:ToggleAddListener(self.Tog_nanJianToggle,function (isChoose)
		AudioMgr:PlayEffect('button')
		self:ChooseProfession(isChoose,self.Img_nanJianToggle,ProfessionEnum.NanJian)
		--CreateRoleCtrl:ChooseProfession(isChoose,ProfessionEnum.NanJian);
    end);
    
    --给各个门派选择按钮添加监听事件，并传递按钮参数，按钮名称
	PanelMgr:ToggleAddListener(self.Tog_schoolFirstTog,function (isChoose)
		AudioMgr:PlayEffect('button')
		self:ChooseSchool(isChoose,self.Img_schoolFirstTog,1)
		--CreateRoleCtrl:ChooseSchool(isChoose,1)
	end);
	
	PanelMgr:ToggleAddListener(self.Tog_schoolSecondTog,function (isChoose)
		AudioMgr:PlayEffect('button')
	    self:ChooseSchool(isChoose,self.Img_schoolSecondTog,2)
		--CreateRoleCtrl:ChooseSchool(isChoose,2)
	end);
	
	--给开始游戏按钮添加监听事件，并传递玩家名称
	PanelMgr:ButtonAddListener(self.Btn_beginBtn,function ()
		AudioMgr:PlayEffect('button')
		if currentProfession == -1 or currentSchool == -1 then
			return
		end
		CreateRoleCtrl:CreateRole(self.Input_nameInput.text,currentProfession,currentSchool)
	end)
end

function CreateRolePanel:Show()
    --初始化登录面板位置
	transform:GetComponent('RectTransform').anchorMin = Vector2.zero;
	transform:GetComponent('RectTransform').anchorMax = Vector2.one;
end

function CreateRolePanel:Hide()

end

--选择职业
function CreateRolePanel:ChooseProfession(isChoose,Img_profession,professionId)

	if isChoose == true then
		--更新当前职业id
		if currentProfession == professionId then
			return
		end
		currentProfession = professionId
		--currentSchool = SchoolEnum[currentProfession][1]
		--从配置表读取职业激活的图标资源名，并刷新该按钮图标
		local iconName = ProfessionConfig[professionId].professionIconTrue;
		--[[
		if iconName == Img_profession.sprite.name then
			return;
		end
		--]]
		ResMgr:LoadAssetSprite('professionicon',{iconName},function (icon)
			Img_profession.sprite = icon
		end);
		--改变按钮大小
		PanelMgr:ChangeScale(Img_profession,Vector2.New(1.1,1.3));
		
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
        
		--选择职业默认门派
		self.Tog_schoolFirstTog.isOn = false
		self.Tog_schoolFirstTog.isOn = true
		--加载门派图标并更新按钮图标
		--[[
		ResMgr:LoadAssetSprite('schoolicon',{SchoolConfig[currentSchool].schoolIconTrue},function (icon)
			self.Img_schoolFirstTog.sprite = icon
		end);
		--]]
	    --self:ChooseSchool(true,self.Img_schoolFirstTog,1)
	else
		--根据职业id，从配置表读取职业失活图标资源名
		local iconName = ProfessionConfig[professionId].professionIconFalse;
		--[[
		if iconName == Img_profession.sprite.name then
			return;
		end
		--]]
		--加载图标并更新按钮图标
		ResMgr:LoadAssetSprite('professionicon',{iconName},function (icon)
			Img_profession.sprite = icon
		end);
		--改变按钮大小
		PanelMgr:ChangeScale(Img_profession,Vector2.New(1,1));
	end
end



--选择门派
function CreateRolePanel:ChooseSchool(isChoose,Img_school,number)
	--当玩家还没有选择职业时，不触发该函数
	if currentProfession == -1 then
		return;
	end
	if isChoose == true then
		if currentSchool == SchoolEnum[currentProfession][number] then
			return
		end
		currentSchool = SchoolEnum[currentProfession][number]
		--根据当前职业id和传递进来的按钮参数，从配置表中读取对应的门派激活图标资源名
		local iconName = SchoolConfig[SchoolEnum[currentProfession][number]].schoolIconTrue;
		--[[
		if iconName == Img_school.sprite.name then
			return;
		end
		--]]
		--加载门派图标并更新按钮图标
		ResMgr:LoadAssetSprite('schoolicon',{iconName},function (icon)
			Img_school.sprite = icon
		end);
		--从配置表读取当前门派详细描述
		local schoolInfo = SchoolConfig[SchoolEnum[currentProfession][number]].schoolInfo;
		--更新门派信息
		self.Text_schoolInfoText.text = schoolInfo
		
		--根据按钮传递进来的参数，加载对应的门派描述框背景图，并更新
		if number == 1 then 
			ResMgr:LoadAssetSprite('schoolicon',{'schoolInfoFirst'},function (icon)
				self.Img_schoolInfoBg.sprite = icon
			end)
		end
		
		if number == 2 then 
			ResMgr:LoadAssetSprite('schoolicon',{'schoolInfoSecond'},function (icon)
				self.Img_schoolInfoBg.sprite = icon
			end)
		end
	else
		--根据当前职业id和传递进来的按钮参数，从配置表中读取对应的门派失活图标资源名
		iconName = SchoolConfig[SchoolEnum[currentProfession][number]].schoolIconFalse;
		--[[
		if iconName == Img_school.sprite.name then
			return;
		end
		--]]
		print(iconName)
		--加载门派图标并更新按钮图标
		ResMgr:LoadAssetSprite('schoolicon',{iconName},function (icon)
			Img_school.sprite = icon
		end);
	end
end