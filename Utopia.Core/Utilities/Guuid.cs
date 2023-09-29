//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using CommunityToolkit.Diagnostics;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Utopia.Core.Utilities;

/// <summary>
/// 人类可读的唯一标识符。由name组成。至少必须要存在有两个name。
/// 第一个name称为root。其余的name称为node(s)。
/// name的长度不能为0，只能由字母、数字、下划线组成。
/// 并且只能由字母开头。
/// guuid的字符串形式类似于：root:namespaces1:namespaces2...
/// </summary>
[MessagePackObject]
public sealed class Guuid
{
    /// <summary>
    /// utopia游戏所使用的GUUID的root.
    /// 不推荐插件使用,避免出现问题.
    /// </summary>
    public const string UTOPIA_ROOT = "utopia";

    /// <summary>
    /// 检查name是否符合要求
    /// </summary>
    /// <param name="name">要检查的name</param>
    /// <returns>如果name合法，返回true。</returns>
    public static bool CheckName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }
        if (!char.IsLetter(name.First()))
        {
            return false;
        }
        if (!name.All((c) => char.IsLetter(c) || char.IsDigit(c) || c == '_'))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 检查整个guuid是否符合要求。
    /// </summary>
    /// <param name="guuid">guuid字符串</param>
    /// <returns>如果符合要求，则返回true，否则返回false</returns>
    public static bool CheckGuuid(string guuid)
    {
        ArgumentNullException.ThrowIfNull(guuid);
        if (string.IsNullOrEmpty(guuid))
        {
            return false;
        }

        var strs = guuid.Split(':');

        // 至少要存在一个root和一个node
        if (strs.Length < 2)
        {
            return false;
        }

        return CheckGuuid(strs.First(), strs[1..]);
    }

    /// <summary>
    /// 检查guuid是否符合要求
    /// </summary>
    /// <param name="root">guuid的root</param>
    /// <param name="nodes">guuid的节点</param>
    /// <returns>如果符合要求，返回true，否则返回false。</returns>
    public static bool CheckGuuid(string root, params string[] nodes)
    {
        ArgumentNullException.ThrowIfNull(root, nameof(root));
        ArgumentNullException.ThrowIfNull(nodes, nameof(nodes));

        if (!CheckName(root))
        {
            return false;
        }
        foreach (var node in nodes)
        {
            if (!CheckName(node))
            {
                return false;
            }
        }

        return true;
    }

    ///
    /// <exception cref="ArgumentException">如果root或者nodes不符合规范则抛出</exception>
    [SerializationConstructor]
    public Guuid(string root, params string[] nodes)
    {
        if (!CheckGuuid(root, nodes))
        {
            throw new ArgumentException("the guuid name is illegal");
        }

        this.Root = root;
        this.Nodes = nodes;
    }

    [Key(0)]
    public string Root { get; }

    [Key(1)]
    public string[] Nodes { get; }

    public static bool operator ==(Guuid c1, Guuid c2)
    {
        return c1.Root == c2.Root && c1.Nodes.SequenceEqual(c2.Nodes);
    }

    public static bool operator !=(Guuid c1, Guuid c2)
    {
        return c1.Root != c2.Root || !c1.Nodes.SequenceEqual(c2.Nodes);
    }

    /// <summary>
    /// 把guuid转换为字符串形式
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(this.Root);
        foreach (var node in this.Nodes)
        {
            builder.Append(':');
            builder.Append(node);
        }

        return builder.ToString();
    }

    /// <summary>
    /// 从字符串解析Guuid
    /// </summary>
    /// <param name="s">字符串应该是来自Guuid的ToString()的结果。</param>
    /// <exception cref="ArgumentException">输入的字符串有误</exception>
    public static Guuid ParseString(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            throw new ArgumentException("param is empty or null");
        }

        var strs = s.Split(':');

        if (strs.Length < 2)
        {
            throw new ArgumentException("the guuid format is illegal");
        }

        return new Guuid(strs.First(), strs[1..]);
    }

    /// <summary>
    /// 获取一个新的随机的标识符。
    /// </summary>
    public static Guuid Unique()
    {
        byte[] rno = RandomNumberGenerator.GetBytes(16);
        ulong high = BitConverter.ToUInt64(rno, 0);
        ulong low = BitConverter.ToUInt64(rno, 8);

        return new Guuid("unique", string.Format("{0:X16}{1:X16}", high, low));
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (obj is Guuid guuid)
        {
            return this == guuid;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Root, this.Nodes);
    }

    public static Guuid NewUtopiaGuuid(params string[] nodes)
    {
        return new Guuid(UTOPIA_ROOT,nodes);
    }
}

public class GuuidMsgPackFormatter : IMessagePackFormatter<Guuid>
{
    public readonly static GuuidMsgPackFormatter Instance = new();

    public Guuid Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null!;
        }

        options.Security.DepthStep(ref reader);

        var id = reader.ReadString();

        reader.Depth--;

        return Guuid.ParseString(id!);
    }

    public void Serialize(ref MessagePackWriter writer, Guuid value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

        var v = value.ToString();

        writer.WriteString(Encoding.UTF8.GetBytes(v));
    }

    public static MessagePackSerializerOptions CreateOption()
    {
        var resolver = CompositeResolver.Create(
            new[] { Instance },
             new[] { StandardResolver.Instance });

        return MessagePackSerializerOptions.Standard.WithResolver(resolver);
    }
}
