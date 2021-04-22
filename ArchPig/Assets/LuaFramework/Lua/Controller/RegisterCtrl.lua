
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:注册面板控制层
*
*        description:
*            功能描述:实现注册面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

RegisterCtrl = {}
local this = RegisterCtrl

local gameObject
local transform

local minLength;
local maxLength;

function RegisterCtrl:Awake()

end

function RegisterCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function RegisterCtrl:Init()
    --账号或密码的最小长度和最大长度
    minLength = 6;
    maxLength = 12;
end

function RegisterCtrl:Register(account,password,rePassword)
	
	--判断账号密码长度是否合格
	if #account < minLength or #password < minLength or #rePassword < minLength then 
		--log('账号或密码不得少于6位')
		UIMgr:Trigger("Message",MessageEnum.Message,'账号或密码不得少于6位')
		return;
	elseif #account > maxLength or #password > maxLength or #rePassword > maxLength then
		return;
	end
	
	if password ~= rePassword then
		--log('两次密码不一致')
		UIMgr:Trigger("Message",MessageEnum.Message,'两次密码不一致')
		return;
	end
	--向服务端发送注册请求
	Client.Send('Register',account,password,rePassword);
end

--服务器返回的注册回调
function RegisterCtrl:RegisterCallback(result)
	if result.result == true then
		PanelMgr:ClosePanel();
	end
	UIMgr:Trigger("Message",MessageEnum.Message,result.message)
end