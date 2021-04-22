local datas = {

	[1] = 
		{
			id = 1,	--索引
			name = "初入侠岚",	--名称
			type = 1,	--类型(传话，打怪，收集物品)
			level = 0,	--等级要求
			premise = 0,	--前置任务
			receive = 1,	--接收任务NPC索引
			submit = 1,	--提交任务NPC索引
			content = "与辗迟对话",	--任务内容
			talk = "2001|2002|2003",	--对话内容
			talkNPC = "1|1|1",	--对话NPC
			paramId = -1,	--任务参数
			count = 0,	--任务需求数
			item = 12020010,	--奖励物品
			equip = -1,	--奖励装备
			gem = -1,	--奖励宝石
			gold = 500,	--奖励金币
			silver = 500,	--奖励银币
			experience = 100,	--经验
		},

	[2] = 
		{
			id = 2,	--索引
			name = "收集物资",	--名称
			type = 2,	--类型(传话，打怪，收集物品)
			level = 0,	--等级要求
			premise = 1,	--前置任务
			receive = 1,	--接收任务NPC索引
			submit = 1,	--提交任务NPC索引
			content = "收集三颗修元丹",	--任务内容
			talk = "2004|2005",	--对话内容
			talkNPC = "1|1",	--对话NPC
			paramId = 11010001,	--任务参数
			count = 3,	--任务需求数
			item = -1,	--奖励物品
			equip = -1,	--奖励装备
			gem = "14010001|14020001|14040001",	--奖励宝石
			gold = 0,	--奖励金币
			silver = 0,	--奖励银币
			experience = 1000,	--经验
		},

	[3] = 
		{
			id = 3,	--索引
			name = "打败恶兽",	--名称
			type = 3,	--类型(传话，打怪，收集物品)
			level = 0,	--等级要求
			premise = 2,	--前置任务
			receive = 1,	--接收任务NPC索引
			submit = 1,	--提交任务NPC索引
			content = "打败三只怪兽",	--任务内容
			talk = "2006|2007",	--对话内容
			talkNPC = "1|1",	--对话NPC
			paramId = 3001,	--任务参数
			count = 3,	--任务需求数
			item = -1,	--奖励物品
			equip = "10060012|10070012|10100012|10110012|10120012",	--奖励装备
			gem = -1,	--奖励宝石
			gold = 1000,	--奖励金币
			silver = 1000,	--奖励银币
			experience = 10000,	--经验
		},
}
return datas