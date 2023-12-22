// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Kcp.Wrapper;
public class Ikcpcb
{
    public required IntPtr IkcpbPtr { get; init; }

    public void EnalbeOutputRecall(KcpLibrary.KcpOutputRecall recall)
    {
        KcpLibrary.ikcp_setoutput(IkcpbPtr, recall);
    }

    public void SwitchMode()
    {
        KcpLibrary.ikcp_nodelay(IkcpbPtr, 1, 10, 2, 1);
    }
}
