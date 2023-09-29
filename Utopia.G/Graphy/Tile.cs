using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Collections;
using Utopia.Core.Utilities;

namespace Utopia.G.Graphy;

/// <summary>
/// 贴图
/// </summary>
public static class TileSource
{
    private const int _defaultSingleTileId = 1;

    /// <summary>
    /// 从单个纹理创建贴图
    /// </summary>
    /// <param name="texture2D">纹理</param>
    /// <returns>贴图的替代id</returns>
    public static int CreateSingleTile(this TileSetAtlasSource self, Texture2D texture2D)
    {
        ArgumentNullException.ThrowIfNull(texture2D);
        var size = texture2D.GetSize();
        self.Texture = texture2D;
        self.TextureRegionSize = new((int)size.X, (int)size.Y);
        self.CreateTile(new Vector2I(0, 0), new Vector2I(1, 1));
        self.CreateAlternativeTile(new Vector2I(0, 0), _defaultSingleTileId);
        return _defaultSingleTileId;
    }
}

/// <summary>
/// 贴图层,数字越大代表越上层
/// </summary>
public enum TileLayer : byte
{
    /// <summary>
    /// 通常是最底层(除非有人想hack),游戏核心不使用,留给mod使用.
    /// </summary>
    Lowest = 8,

    /// <summary>
    ///较底层,同样留给mod使用.
    /// </summary>
    Low = 16,

    /// <summary>
    /// 游戏核心使用,是地面层
    /// </summary>
    Floor = 32,

    /// <summary>
    /// 游戏核心使用,建筑or生物层
    /// </summary>
    BuildingAndBio = 64,

    /// <summary>
    /// 游戏核心使用,也叫做"家具层"
    /// </summary>
    Furniture = 128,

    /// <summary>
    /// 游戏核心使用,叫做"物品层"
    /// </summary>
    Stuff = 156,

    /// <summary>
    /// 高层,留给mod使用
    /// </summary>
    High = 196,

    /// <summary>
    /// 通常是最高处(同样可以hack),留给mod使用
    /// </summary>
    Highest = 216,
}
public static class TileMapHelper
{
    public static void CreateTileMapLayer(in TileMap map)
    {
        ArgumentNullException.ThrowIfNull(map);
        if (map.GetLayersCount() != 0)
        {
            while (map.GetLayersCount() > 0)
            {
                map.RemoveLayer(0);
            }
        }
        for (int i = 0; i <= byte.MaxValue; i++)
        {
            map.AddLayer(-1);
            map.SetLayerZIndex(i, -byte.MaxValue - 1 + i);
            map.SetLayerYSortEnabled(i, false);
        }
    }

    public static void InitlizeMap(in TileMap map)
    {
        map.Position = new Vector2I(0, 0);
    }

    public static Vector2 GetCameraPosition(in TileMap map,in Vector2I flatPosition)
    {
        var local = map.MapToLocal(flatPosition);
        var global = map.ToGlobal(local);

        return global;
    }

    public static void SetCell(this TileMap map, Vector2I position, Tile tile)
    {
        map.SetCell(tile.LayerIndex,
            position,
            tile.SourceId,
            null,
            tile.TileId);
    }
}

/// <summary>
/// 贴图信息
/// </summary>
public readonly struct Tile
{
    public int LayerIndex { get; init; }

    public int SourceId { get; init; }

    public int TileId { get; init; }

    public Tile(int layer, int sourceId, int tileId)
    {
        this.LayerIndex = layer;
        this.SourceId = sourceId;
        this.TileId = tileId;
    }
}

/// <summary>
/// 贴图管理器
/// </summary>
public class TileManager : SafeDictionary<Guuid, Tile>
{
}
