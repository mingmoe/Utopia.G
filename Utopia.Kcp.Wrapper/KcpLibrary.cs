// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Runtime.InteropServices;

namespace Utopia.Kcp.Wrapper;

public static class KcpLibrary
{
    public const string LibraryName = "Utopia.Kcp";

    public delegate int KcpOutputRecall(IntPtr buffer,int length,IntPtr ikcpb, IntPtr user);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern IntPtr ikcp_create(uint conv,IntPtr user);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern void ikcp_setoutput(IntPtr kcp, KcpOutputRecall recall);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern void ikcp_update(IntPtr kcp, uint current);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern uint ikcp_check(IntPtr kcp, uint current);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern void ikcp_input(IntPtr kcp, IntPtr buffer,long size);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern uint ikcp_flush(IntPtr kcp);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern int ikcp_nodelay(IntPtr kcp, int nodelay, int interval, int resend, int nc);

    [DllImport(LibraryName, CharSet = CharSet.Unicode)]
    public static extern int ikcp_wndsize(IntPtr kcp, int sndwnd, int recvwnd);


    public static Ikcpcb CreateKcp(uint conv,IntPtr userData)
    {
        return new() {
            IkcpbPtr = ikcp_create(conv, userData)
        };
    }

}
