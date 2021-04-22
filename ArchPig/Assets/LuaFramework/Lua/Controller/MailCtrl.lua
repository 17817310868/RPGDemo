
--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:邮件面板逻辑层
*
*        description:
*            功能描述:实现邮件面板功能逻辑
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]



MailCtrl = {}
local this = MailCtrl

local gameObject
local transform

function MailCtrl:Awake()

end

function MailCtrl:OnCreate(go)
    gameObject = go
    transform = go.transform
    self:Init()
end

function MailCtrl:Init()

end

function MailCtrl:SendMail(receive,title,content,gold,items)

    local C2S_sendMail = C2S_SendMail.New()
    C2S_sendMail.receiveName = receive
    C2S_sendMail.title = title
    C2S_sendMail.content = content
    if #items > 0 then
        for index = 1,#items do
            C2S_sendMail.itemsGuid:Add(items[index])
        end
    end
    C2S_sendMail.gold = gold
    Client.Send('SendMail',C2S_sendMail)
end

function MailCtrl:GetMails()

    Client.MySend('GetMailsInfo')

end

function MailCtrl:UpdateMails(S2C_receiveMails)

    UIMgr:Trigger('UpdateMails',S2C_receiveMails)

end

function MailCtrl:GetItems(mailGuid)

    Client.Send('GetMailItems',C2S_GetMailItems.New(mailGuid))

end

function MailCtrl:ReadMail(mailGuid)

    Client.Send('ReadMail',C2S_ReadMail.New(mailGuid))

end