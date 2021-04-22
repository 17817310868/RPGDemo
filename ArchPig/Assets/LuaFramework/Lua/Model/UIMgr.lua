--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题：lua的ui管理器
*
*        description:
*            功能描述:初始化面板控件，封装UI的事件系统,封装一些UI方面的方法
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

UIMgr = {}
local this = UIMgr
local EventTable = {}
function UIMgr:InitCtrl (tableName,ctrlsDic)
    local dic = self:DicToLuaTable(ctrlsDic)
    local table = loadstring("return "..tableName)()
    for key,value in pairs(table.Ctrls) do
        table[key] = dic[tostring(key)]
    end
end

function UIMgr:DicToLuaTable(Dic)
    --将C#的Dic转成Lua的Table
    local dic = {}
    if Dic then
        local iter = Dic:GetEnumerator()
        while iter:MoveNext() do
            local k = iter.Current.Key
            local v = iter.Current.Value
            dic[k] = v
        end
    end
    return dic
end

function UIMgr:SetActive(GoTransform,bool)
	if GoTransform.gameObject.activeSelf == bool then
		return;
    end
    GoTransform.gameObject:SetActive(bool)
end

--更新物品类型按钮的图标   isChoose:是否选中   Img_tog:控件图片
function UIMgr:ChangeTogImg(isChoose,Img_tog,highlightedIcon,normalIcon)
	local iconName
	if isChoose == true then
		iconName = highlightedIcon
	else
		iconName = normalIcon
	end
	ResMgr:LoadAssetSprite('othericon',{iconName},function (icon)
		Img_tog.sprite = icon
	end)
end

Observer = {}
Observer.__index = Observer

function Observer:New(func)
    local tab = {}
    tab.Exe = function (...)
        func(...)
    end,
    setmetatable(tab,Observer)
    return tab
end


function UIMgr:AddListener(eventName,observer)
    if EventTable[eventName] == nil then
        EventTable[eventName] = {}
    end
    table.insert(EventTable[eventName],observer)
end

function UIMgr:Trigger(eventName,...)
    if EventTable[eventName] == nil or #EventTable[eventName] == 0 then
        return
    end
    for index = 1,#EventTable[eventName] do
        EventTable[eventName][index].Exe(...)
    end
    --[[
    local funcs = EventTable[eventName]
    if funcs == nil then
        return;
    end
    for index = 1,#funcs do
        funcs[index](...)
    end
    --]]
end

function UIMgr:RemoveListener(eventName,observer)
    if observer == nil then
        EventTable[eventName] = {}
    else
        if EventTable[eventName] == nil or #EventTable[eventName] == 0 then
            return
        end
        for index = 1,#EventTable[eventName] do
            if EventTable[eventName][index] == observer then
                table.remove(EventTable[eventName],index)
                return
            end
        end
    end
end
