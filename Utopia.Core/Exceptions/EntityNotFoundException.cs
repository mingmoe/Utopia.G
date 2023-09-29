using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities;

namespace Utopia.Core.Exceptions;
public class EntityNotFoundException : System.Exception
{
    public Guuid EntityID { get; init; }

    public EntityNotFoundException(Guuid entityId) : base($"the entity {entityId} not found")
    {
        this.EntityID = entityId;
    }
}
