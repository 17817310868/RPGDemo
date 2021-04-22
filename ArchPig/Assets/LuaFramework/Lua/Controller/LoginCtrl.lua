
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:登录面板控制层
*
*        description:
*            功能描述:实现登录面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

LoginCtrl = {}
local this = LoginCtrl

local gameObject
local transform

local minLength;
local MaxLength;

function LoginCtrl:Awake()

end

function LoginCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function LoginCtrl:Init()
    --账号或密码的最小长度和最大长度
    minLength = 6;  
	maxLength = 12;  
end

--登录按钮的具体处理逻辑
function LoginCtrl:Login(account,password)

	--判断账号密码长度是否合格
	if #account <  minLength or #password < minLength then
		--log('账号或密码不得少于6位');
		UIMgr:Trigger("Message",MessageEnum.Message,'账号或密码不得少于6位')
		return;
	elseif #account > maxLength or #password > maxLength then
		return;
	end

	--向服务端发送登录请求
	Client.Send('Login',account,password);
	
end

--服务器返回的登录回调
function LoginCtrl:LoginCallback(result)
	if result.result == false then
		UIMgr:Trigger("Message",MessageEnum.Message,result.message)
	end
end

--进入创建角色界面
function LoginCtrl:CreateRole()
	PanelMgr:ClosePanel();
	PanelMgr:ClosePanel();
	Scenes:ScenesLoadAsync('createrole',function ()
		PanelMgr:OpenPanel('CreateRolePanel',UILayer.Bottom,MaskEnum.Mask);
	end)
end

function LoginCtrl:ChangeScene(result)
	PanelMgr:ClearPanel();
	PanelMgr:OpenPanel('LoadingPanel',UILayer.Bottom,MaskEnum.None,function ()
		Scenes:ScenesLoadAsync(result.sceneId, function()
			PanelMgr:ClosePanel()
			PanelMgr:OpenPanel('HeadMgrPanel',UILayer.Bottom,MaskEnum.None)
			Client.MySend('InitRole');
			--Client.MySend('InitItem');
			log('初始化角色请求')
			--RoleCtrl:ChangeCanCtrl()
			AudioMgr:PlayBgm('taoyuanzhen')
		end)
	end)
	
end