--[[
*=================================================================================
*
*        projectName:
*            项目名称:拱猪
*
*        title:
*            标题:滚动视图优化
*
*        description:
*            功能描述:封装优化滚动视图
*
*        author:
*            作者:照着教程敲出bug的程序员
*=================================================================================
*
--]]

OptimizeSV = {}
OptimizeSV.__index = OptimizeSV

--回收当前不需要显示的格子  (本身，新的格子起点索引，新的格子终点索引)
local Delete = function (this,newStartIndex,newEndIndex)
    --回收小于当前起点索引的所有显示的格子，并调用隐藏回调,移出字典
    if newStartIndex > this.oldStartIndex then
        for index = this.oldStartIndex,newStartIndex - 1 do
            if index >= 0 and index < this.boxCount then
                this.SetCallback(index,this.boxsDic[index])
                PoolMgr:Set(this.boxsDic[index].name,this.boxsDic[index])
                this.boxsDic[index] = nil
            end
        end
    end
    --回收大于当前终点所有的所有显示的格子，并调用隐藏回调，移出字典
    if newEndIndex < this.oldEndIndex then
        for index = newEndIndex + 1,this.oldEndIndex do
            if index >= 0 and index < this.boxCount then
                this.SetCallback(index,this.boxsDic[index])
                PoolMgr:Set(this.boxsDic[index].name,this.boxsDic[index])
                this.boxsDic[index] = nil
            end
        end
    end
    --更新显示起点和终点索引
    this.oldStartIndex = newStartIndex
    this.oldEndIndex = newEndIndex
end

--更新格子  （处理不同的四个排列方向）
local Update = {
    --当视图类型方向为从上往下时调用的方法，以下同理
    [1] = function (this)
        --判断当前显示的范围是否被Top占满，如果是，直接跳出
        if math.abs(this.contentRect.anchoredPosition.y + this.showHeight) < this.top then
            return 
        end
        --计算新的显示起点坐标和终点坐标
        newStartIndex = math.floor((math.abs(this.contentRect.anchoredPosition.y) - this.top) / (this.boxHeight + this.spacingY))
            * this.col
        newEndIndex = math.floor((math.abs(this.contentRect.anchoredPosition.y) + this.showHeight - this.top) / 
            (this.boxHeight + this.spacingY)) * this.col + this.col - 1

        if newStartIndex < 0 then
            newStartIndex = 0
        end
        if newEndIndex >= this.boxCount then
            newEndIndex = this.boxCount - 1
        end
        --判断该索引是否已显示，如果没有，则去对象池取出，并调用显示回调，添加进格子字典
        for index = newStartIndex,newEndIndex do
            if this.boxsDic[index] == nil then
            this.boxsDic[index] = 1  --占位
            PoolMgr:Get(this.resName,function (box)
                this.boxsDic[index] = box
                box.transform:SetParent(this.contentRect.transform)  --设置格子父物体
                box.transform:GetComponent('RectTransform').anchorMin = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').anchorMax = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').pivot = Vector2.New(0.5,0.5)
                --判断格子的起点位置，并根据索引与行列的关系设置位置坐标
                if this.cornerType == CornerEnum.UpperLeft then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(-this.contentWidth/2 + this.left + 
                        this.boxWidth / 2 + (index % this.col) * (this.spacingX + this.boxWidth) ,this.contentHeight / 2 -this.top
                         - this.boxHeight / 2 - math.floor(index / this.col) * (this.boxHeight + this.spacingY))
                end
                if this.cornerType == CornerEnum.UpperRight then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(this.contentWidth/2 - this.right - 
                        this.boxWidth / 2 - (index % this.col) * (this.spacingX + this.boxWidth) ,this.contentHeight / 2 -this.top
                         - this.boxHeight / 2 - math.floor(index / this.col) * (this.boxHeight + this.spacingY))
                end
                this.GetCallback(index,box)
            end)
            end
            
        end

        Delete(this,newStartIndex,newEndIndex)

    end,
    [2] = function (this)
        if math.abs(this.contentRect.anchoredPosition.y - this.showHeight) < this.bottom then
            return 
        end
        newStartIndex = math.floor((math.abs(this.contentRect.anchoredPosition.y) - this.bottom) / (this.boxHeight + this.spacingY))
            * this.col
        newEndIndex = math.floor((math.abs(this.contentRect.anchoredPosition.y) + this.showHeight - this.bottom) / 
            (this.boxHeight + this.spacingY)) * this.col + this.col - 1
        if newStartIndex < 0 then
            newStartIndex = 0
        end
        if newEndIndex >= this.boxCount then
            newEndIndex = this.boxCount - 1
        end
        for index = newStartIndex,newEndIndex do
            if this.boxsDic[index] == nil then
            this.boxsDic[index] = 1  --占位
            PoolMgr:Get(this.resName,function (box)
                this.boxsDic[index] = box
                box.transform:SetParent(this.contentRect.transform)
                box.transform:GetComponent('RectTransform').anchorMin = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').anchorMax = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').pivot = Vector2.New(0.5,0.5)
                if this.cornerType == CornerEnum.LowerLeft then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(-this.contentWidth/2 + this.left + 
                        this.boxWidth / 2 + (index % this.col) * (this.spacingX + this.boxWidth) ,-this.contentHeight / 2 +this.bottom
                         + this.boxHeight / 2 + math.floor(index / this.col) * (this.boxHeight + this.spacingY))
                end
                if this.cornerType == CornerEnum.LowerRight then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(this.contentWidth/2 - this.left - 
                        this.boxWidth / 2 - (index % this.col) * (this.spacingX + this.boxWidth) ,-this.contentHeight / 2 +this.bottom
                         + this.boxHeight / 2 + math.floor(index / this.col) * (this.boxHeight + this.spacingY))
                end
                this.GetCallback(index,box)
            end)
            end
        end

        Delete(this,newStartIndex,newEndIndex)
    end,
    [3] = function (this)
        if math.abs(this.contentRect.anchoredPosition.x + this.showWidth) < this.left then
            return 
        end
        newStartIndex = math.floor((math.abs(this.contentRect.anchoredPosition.x) - this.left) / (this.boxWidth + this.spacingX))
            * this.row
        newEndIndex = math.floor((math.abs(this.contentRect.anchoredPosition.x) + this.showWidth - this.left) / 
            (this.boxWidth + this.spacingX)) * this.row + this.row - 1
        if newStartIndex < 0 then
            newStartIndex = 0
        end
        if newEndIndex >= this.boxCount then
            newEndIndex = this.boxCount - 1
        end
        for index = newStartIndex,newEndIndex do
            if this.boxsDic[index] == nil then
            this.boxsDic[index] = 1  --占位
            PoolMgr:Get(this.resName,function (box)
                this.boxsDic[index] = box
                box.transform:SetParent(this.contentRect.transform)
                box.transform:GetComponent('RectTransform').anchorMin = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').anchorMax = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').pivot = Vector2.New(0.5,0.5)
                if this.cornerType == CornerEnum.UpperLeft then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(-this.contentWidth/2 + this.left + 
                        this.boxWidth / 2 + math.floor(index / this.row) * (this.spacingX + this.boxWidth) ,
                        this.contentHeight / 2 -this.top - this.boxHeight / 2 - (index % this.row) * (this.boxHeight +
                        this.spacingY))
                end
                if this.cornerType == CornerEnum.LowerLeft then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(-this.contentWidth/2 + this.left + 
                        this.boxWidth / 2 + math.floor(index / this.row) * (this.spacingX + this.boxWidth) ,
                        -this.contentHeight / 2 +this.top + this.boxHeight / 2 + (index % this.row) * (this.boxHeight +
                        this.spacingY))
                end
                this.GetCallback(index,box)
            end)
            end
        end

        Delete(this,newStartIndex,newEndIndex)
    end,
    [4] = function (this)
        if math.abs(this.contentRect.anchoredPosition.x + this.showWidth) < this.right then
            return 
        end
        newStartIndex = math.floor((math.abs(this.contentRect.anchoredPosition.x) - this.right) / (this.boxWidth + this.spacingX))
            * this.row
        newEndIndex = math.floor((math.abs(this.contentRect.anchoredPosition.x) + this.showWidth - this.right) / 
            (this.boxWidth + this.spacingX)) * this.row + this.row - 1
        if newStartIndex < 0 then
            newStartIndex = 0
        end
        if newEndIndex >= this.boxCount then
            newEndIndex = this.boxCount - 1
        end
        for index = newStartIndex,newEndIndex do
            if this.boxsDic[index] == nil then
            this.boxsDic[index] = 1  --占位
            PoolMgr:Get(this.resName,function (box)
                this.boxsDic[index] = box
                box.transform:SetParent(this.contentRect.transform)
                box.transform:GetComponent('RectTransform').anchorMin = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').anchorMax = Vector2.New(0.5,0.5)
                box.transform:GetComponent('RectTransform').pivot = Vector2.New(0.5,0.5)
                if this.cornerType == CornerEnum.UpperRight then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(this.contentWidth/2 - this.right - 
                        this.boxWidth / 2 - math.floor(index / this.row) * (this.spacingX + this.boxWidth) ,
                        this.contentHeight / 2 -this.top - this.boxHeight / 2 - (index % this.row) * (this.boxHeight +
                        this.spacingY))
                end
                if this.cornerType == CornerEnum.LowerRight then
                    box.transform:GetComponent('RectTransform').anchoredPosition  = Vector2.New(this.contentWidth/2 - this.right - 
                        this.boxWidth / 2 - math.floor(index / this.row) * (this.spacingX + this.boxWidth) ,
                        -this.contentHeight / 2 +this.top + this.boxHeight / 2 + (index % this.row) * (this.boxHeight +
                        this.spacingY))
                end
                this.GetCallback(index,box)
            end)
            end
        end

        Delete(this,newStartIndex,newEndIndex)
    end
}

--初始化  （处理不同方向）
local Init = {
    --根据视图方向类型进行初始化，设置锚点与宽高
    [1] = function (this)
        this.viewRect.transform:GetComponent('ScrollRect').horizontal = false
        this.viewRect.transform:GetComponent('ScrollRect').vertical = true
        this.contentRect.anchorMin = Vector2.New(0,1)
        this.contentRect.anchorMax = Vector2.New(1,1)
        this.contentRect.pivot = Vector2.New(0,1)
        this.contentWidth = this.viewRect.sizeDelta.x
        this.col = math.floor(((this.contentWidth - (this.left + this.right)) + this.spacingX) / (this.boxWidth + this.spacingX))
        this.row = math.ceil(this.boxCount / this.col)
        this.contentHeight = this.top + (this.row * this.boxHeight) + ((this.row - 1) * this.spacingY) + this.bottom
        this.contentRect.sizeDelta  = Vector2.New(0,this.contentHeight)
        this.oldStartIndex = -1
        this.oldEndIndex = -1
        Update[this.type](this)

    end,
    [2] = function (this)
        this.viewRect.transform:GetComponent('ScrollRect').horizontal = false
        this.viewRect.transform:GetComponent('ScrollRect').vertical = true
        this.contentRect.anchorMin = Vector2.New(0,0)
        this.contentRect.anchorMax = Vector2.New(1,0)
        this.contentRect.pivot = Vector2.New(1,0)
        this.contentWidth = this.viewRect.sizeDelta.x
        this.col = math.floor(((this.contentWidth - (this.left + this.right)) + this.spacingX) / (this.boxWidth + this.spacingX))
        this.row = math.ceil(this.boxCount / this.col)
        this.contentHeight = this.top + (this.row * this.boxHeight) + ((this.row - 1) * this.spacingY) + this.bottom
        this.contentRect.sizeDelta  = Vector2.New(0,this.contentHeight)
        this.oldStartIndex = -1
        this.oldEndIndex = -1
        Update[this.type](this)
    end,
    [3] = function (this)
        this.viewRect.transform:GetComponent('ScrollRect').horizontal = true
        this.viewRect.transform:GetComponent('ScrollRect').vertical = false
        this.contentRect.anchorMin = Vector2.New(0,0)
        this.contentRect.anchorMax = Vector2.New(0,1)
        this.contentRect.pivot = Vector2.New(0,1)
        this.contentHeight = this.viewRect.sizeDelta.y
        this.row = math.floor(((this.contentHeight - (this.top + this.bottom)) + this.spacingY) / (this.boxHeight + this.spacingY))
        this.col = math.ceil(this.boxCount / this.row)
        this.contentWidth = this.left + (this.col * this.boxWidth) + ((this.col - 1) * this.spacingX) + this.right
        this.contentRect.sizeDelta  = Vector2.New(this.contentWidth,0)
        this.oldStartIndex = -1
        this.oldEndIndex = -1
        Update[this.type](this)

    end,
    [4] = function (this)
        this.viewRect.transform:GetComponent('ScrollRect').horizontal = true
        this.viewRect.transform:GetComponent('ScrollRect').vertical = false
        this.contentRect.anchorMin = Vector2.New(1,0)
        this.contentRect.anchorMax = Vector2.New(1,1)
        this.contentRect.pivot = Vector2.New(1,1)
        this.contentHeight = this.viewRect.sizeDelta.y
        this.row = math.floor(((this.contentHeight - (this.top + this.bottom)) + this.spacingY) / (this.boxHeight + this.spacingY))
        this.col = math.ceil(this.boxCount / this.row)
        this.contentWidth = this.left + (this.col * this.boxWidth) + ((this.col - 1) * this.spacingX) + this.right
        this.contentRect.sizeDelta  = Vector2.New(this.contentWidth,0)
        this.oldStartIndex = -1
        this.oldEndIndex = -1
        Update[this.type](this)
    end
}

--竖直布局
function OptimizeSV:VerticalLayout(viewRect,contentRect,boxWidth,boxHeight,boxCount,top,bottom,left,right,spacing,type,cornerType,
    resName,getCallback,setCallback)

    self:GridLayout(viewRect,contentRect,boxWidth,boxHeight,boxCount,top,bottom,left,right,0,spacing,type,cornerType,resName,
        getCallback,setCallback)

end

--水平布局
function OptimizeSV:HorizontalLayout(viewRect,contentRect,boxWidth,boxHeight,boxCount,top,bottom,left,right,spacing,type,cornerType,
    resName,getCallback,setCallback)

    self:GridLayout(viewRect,contentRect,boxWidth,boxHeight,boxCount,top,bottom,left,right,spacing,0,type,cornerType,resName,
        getCallback,setCallback)

end

--格子布局
function OptimizeSV:GridLayout(viewRect,contentRect,boxWidth,boxHeight,boxCount,top,bottom,left,right,spacingX,spacingY,type,cornerType,
    resName,getCallback,setCallback)

    self.boxsDic = {}  --存储当前显示的所有格子对象
    self.viewRect = viewRect  --对应的ScrollRect组件
    self.showHeight = viewRect.sizeDelta.y  --显示的范围的高度
    self.showWidth = viewRect.sizeDelta.x  --显示的范围的宽度
    self.contentRect = contentRect  --内容组件
    self.boxHeight = boxHeight  --格子对象的高度
    self.boxWidth = boxWidth  --格子对象的宽度
    self.boxCount = boxCount  --格子总数
    self.top = top  --首格子与最上边的距离
    self.bottom = bottom  --首格子与最下边的距离
    self.left = left  --首格子与最左边的距离
    self.right = right  --首格子与最右边的距离
    self.spacingX = spacingX  --格子间横向的间隔
    self.spacingY = spacingY  --格子间竖直的间隔
    self.type = type  --排列方向，（上到下，下到上，左到右，右到左）
    self.cornerType = cornerType  --起点位置（左上，右上，左下，右下）
    self.resName = resName  --格子资源名
    self.GetCallback = function (index,box)  --当格子显示时调用的函数
        getCallback(index,box)
    end
    self.SetCallback = function (index,box)  --当格子隐藏时调用的函数
        setCallback(index,box)
    end
    self.contentRect.anchoredPosition = Vector2.New(0,0)  --初始化内容坐标为原点
    Init[self.type](self)  --初始化
    
end

--更新显示
function OptimizeSV:Update()
    Update[self.type](self)
end

--清除显示的东西
function OptimizeSV:Clear()
    if self.boxsDic == nil then
        return
    end
    if next(self.boxsDic) == nil then
        return
    end
    for key,value in pairs(self.boxsDic) do
        self.SetCallback(key,value)
        PoolMgr:Set(value.name,value)
    end
    self.oldStartIndex = -1
    self.oldEndIndex = -1
    self.boxsDic = {}
end

--构造函数
function OptimizeSV:New()
    local tab = {}
    setmetatable(tab,OptimizeSV)
    return tab
end