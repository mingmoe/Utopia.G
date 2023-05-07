//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Godot;
using System;

namespace Utopia.G;

public static class Utility
{

    /// <summary>
    /// 为场景设置背景图像
    /// </summary>
    /// <param name="root">场景的root节点</param>
    /// <param name="background">背景图像</param>
    public static void SetBackground(Node root, Sprite2D background)
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
    }
}
