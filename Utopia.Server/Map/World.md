# 游戏地图的设计

**\*此文档是为服务器编写的\***

游戏地图主要由以下接口组成:
 - IWorldManager: 在一个游戏内只有一个单例
 - IWorld: 世界，每个游戏至少拥有一个世界
 - IArea: 区域，每个世界由许多区域组成，
 - IBlock: 是地图的最小单位

