using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Map;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Entity;

/// <summary>
/// 草地实体，曾经一段时间被用于测试
/// </summary>
public class GrassEntity : IEntity
{
    public TranslateKey Name => ResourcePack.Entity.GrassEntity.TranslateKey;

    public bool Accessible => false;

    public bool Collidable => false;

    public Guuid Id => ResourcePack.Entity.GrassEntity.ID;

    public WorldPosition WorldPosition { get; set; }

    public void LogicUpdate()
    {

    }
}
