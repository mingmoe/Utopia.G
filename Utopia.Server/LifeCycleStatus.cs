#region copyright
// This file(may named LifeCycleStatus.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
