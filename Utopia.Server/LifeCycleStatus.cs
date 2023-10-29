// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Server;

public enum LifeCycle
{
    /// <summary>
    /// 注意：这个代表系统已经初始化完毕之后，并非代表系统正在初始化。
    /// 初始化的时候事件总线尚未存在。
    /// </summary>
    InitializedSystem,
    LoadPlugin,
    LoadSaveings,
    StartLogicThread,
    StartNetThread,
    Crash,
    GracefulShutdown,
    /// <summary>
    /// 对于崩溃和正常关闭，都会触发此生命周期的事件。
    /// </summary>
    Stop,
}
