#region copyright
// This file(may named GodotBinder.cs) is a part of the project: Utopia.G.
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

using Godot;
using System;
using System.Reflection;

namespace Utopia.G.Scene;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class GodotNodeBindAttribute : Attribute
{
    /// <summary>
    /// 如果是Empty,那么则将field或者property的名字作为NodeName
    /// </summary>
    public string NodeName { get; init; } = string.Empty;

    public GodotNodeBindAttribute(string nodeName = "")
    {
        ArgumentNullException.ThrowIfNull(nodeName);
        this.NodeName = nodeName;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class GodotResourceBindAttribute : Attribute
{
    /// <summary>
    /// 如果是Empty,那么则将field或者property的名字作为ResourceName
    /// </summary>
    public string ResourceName { get; init; } = string.Empty;

    public GodotResourceBindAttribute(string resourceName = "")
    {
        ArgumentNullException.ThrowIfNull(resourceName);
        this.ResourceName = resourceName;
    }
}

public static class GodotBinder
{
    /// <summary>
    /// 绑定node
    /// </summary>
    /// <param name="source">
    /// 要绑定node的对象
    /// </param>
    /// <param name="target">
    /// 绑定的node的来源
    /// </param>
    public static void Bind(object target, Node source)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        var type = target.GetType();

        foreach (var item in type.GetProperties(BindingFlags.NonPublic |
                         BindingFlags.Instance))
        {
            var bind = item.GetCustomAttribute<GodotNodeBindAttribute>();
            var resource = item.GetCustomAttribute<GodotResourceBindAttribute>();

            if (bind != null)
            {
                item.SetValue(target, source.FindChild(bind.NodeName == string.Empty ?
                    item.Name : bind.NodeName));
            }
            if (resource != null)
            {
                var ss = ResourceLoader.Load(resource.ResourceName == string.Empty ?
                    item.Name : resource.ResourceName);
                item.SetValue(target, ss);
            }
        }
        foreach (var item in type.GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance))
        {
            var bind = item.GetCustomAttribute<GodotNodeBindAttribute>();
            var resource = item.GetCustomAttribute<GodotResourceBindAttribute>();

            if (bind != null)
            {
                item.SetValue(target, source.FindChild(bind.NodeName == string.Empty ?
                    item.Name : bind.NodeName));
            }
            if (resource != null)
            {
                var ss = ResourceLoader.Load(resource.ResourceName == string.Empty ?
                    item.Name : resource.ResourceName);
                item.SetValue(target, ss);
            }
        }
    }
}
