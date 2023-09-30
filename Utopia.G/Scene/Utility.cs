#region copyright
// This file(may named Utility.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using CommunityToolkit.Diagnostics;
using Godot;
using System;

namespace Utopia.G.Scene;

public static class Utility
{

    /// <summary>
    /// 为场景设置背景图像
    /// </summary>
    /// <param name="root">场景的root节点</param>
    /// <param name="background">背景图像</param>
    public static object SetBackground(Node root, Sprite2D background)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(background);

        var viewport = root.GetViewport();

        void lambda()
        {
            var rect = viewport.GetVisibleRect().Size;
            var tRect = background?.Texture?.GetSize();

            if (!tRect.HasValue)
            {
                return;
            }

            // keep the background fill the viewport
            background!.Scale = new(rect.X / tRect.Value.X,
                rect.Y / tRect.Value.Y);
        }

        viewport.SizeChanged += lambda;

        lambda();
        return (object)lambda;
    }

    public static void CancelBackground(Node root, object key)
    {
        Guard.IsNotNull(root);
        Guard.IsNotNull(key);
        root.GetViewport().SizeChanged -= (Action)key;
    }

}
