// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Reflection;
using Godot;

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
        NodeName = nodeName;
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
        ResourceName = resourceName;
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
        Type type = target.GetType();

        foreach (PropertyInfo item in type.GetProperties(BindingFlags.NonPublic |
                         BindingFlags.Instance))
        {
            GodotNodeBindAttribute? bind = item.GetCustomAttribute<GodotNodeBindAttribute>();
            GodotResourceBindAttribute? resource = item.GetCustomAttribute<GodotResourceBindAttribute>();

            if (bind != null)
            {
                item.SetValue(target, source.FindChild(bind.NodeName == string.Empty ?
                    item.Name : bind.NodeName));
            }
            if (resource != null)
            {
                Godot.Resource ss = ResourceLoader.Load(resource.ResourceName == string.Empty ?
                    item.Name : resource.ResourceName);
                item.SetValue(target, ss);
            }
        }
        foreach (FieldInfo item in type.GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance))
        {
            GodotNodeBindAttribute? bind = item.GetCustomAttribute<GodotNodeBindAttribute>();
            GodotResourceBindAttribute? resource = item.GetCustomAttribute<GodotResourceBindAttribute>();

            if (bind != null)
            {
                item.SetValue(target, source.FindChild(bind.NodeName == string.Empty ?
                    item.Name : bind.NodeName));
            }
            if (resource != null)
            {
                Godot.Resource ss = ResourceLoader.Load(resource.ResourceName == string.Empty ?
                    item.Name : resource.ResourceName);
                item.SetValue(target, ss);
            }
        }
    }
}
