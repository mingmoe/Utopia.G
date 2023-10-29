// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using CommunityToolkit.Diagnostics;
using Godot;

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

        Viewport viewport = root.GetViewport();

        void lambda()
        {
            Vector2 rect = viewport.GetVisibleRect().Size;
            Vector2? tRect = background?.Texture?.GetSize();

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
