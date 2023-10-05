using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities;

namespace Utopia.Core.Exceptions;

/// <summary>
/// The formatters(see <see cref="Net.IPacketFormatter"/>) not found
/// </summary>
internal class FormatterNotFoundExceptiom : System.Exception
{

    public readonly Guuid PacketId;

    public FormatterNotFoundExceptiom(Guuid packetId) : base(packetId.ToString())
    {
        this.PacketId = packetId;
    }

}
