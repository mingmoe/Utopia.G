#region copyright
// This file(may named Guuid.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System.Collections;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Utopia.Core.Utilities;

/// <summary>
/// globally unique universally identifier.
/// This is a constant class(like string).
/// 人类可读的唯一标识符。由name组成。至少必须要存在有两个name。
/// 第一个name称为root。其余的all name称为node(s)。
/// name的长度不能为0，只能由字母、数字、下划线组成。
/// 并且只能由字母开头。
/// guuid的字符串形式类似于：root:namespaces1:namespaces2...
/// </summary>
[MessagePackObject]
public sealed class Guuid : IEnumerable<string>
{
    /// <summary>
    /// utopia游戏所使用的GUUID的root.
    /// 不推荐插件使用,避免出现问题.
    /// </summary>
    public const string UTOPIA_ROOT = "utopia";

    /// <summary>
    /// Will use this to separate root and each namespaces when use the string of Guuid.
    /// </summary>
    public const string SEPARATOR = ".";

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

        var strs = guuid.Split(SEPARATOR);

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
    /// 把guuid转换为字符串形式. Will use <see cref="SEPARATOR"/> to separate root and each namespaces.
    /// For example,
    /// a guuid with root `r` and namespaces `a` and `b` will have a string form as `r.a.b`
    /// (If <see cref="SEPARATOR"/> is `.`)
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(this.Root);
        foreach (var node in this.Nodes)
        {
#pragma warning disable CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
            builder.Append(SEPARATOR).Append(node);
#pragma warning restore CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
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

        var strs = s.Split(SEPARATOR);

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
        int hash = this.Root.GetHashCode();

        foreach (var node in this.Nodes)
        {
            hash = HashCode.Combine(hash, XxHash64.Hash(Encoding.UTF8.GetBytes(node)));
        }
        return hash;
    }

    public Guuid Append(Guuid guuid)
    {
        string root = this.Root;
        var nodes = this.Nodes.ToList();
        nodes.Add(guuid.Root);
        nodes.AddRange(guuid.Nodes);
        return new Guuid(root, nodes.ToArray());
    }

    /// <summary>
    /// Cover this guuid to C# identifier
    /// </summary>
    /// <returns></returns>
    public string ToCsIdentifier()
    {
        return this.Nodes.Aggregate(this.Root,(result, value) =>
        {
            return result + "_" + value;
        });
    }

    public static Guuid NewUtopiaGuuid(params string[] nodes)
    {
        return new Guuid(UTOPIA_ROOT, nodes);
    }

    public IEnumerator<string> GetEnumerator()
    {
        yield return this.Root;
        foreach(var s in this.Nodes)
        {
            yield return s;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        yield return (IEnumerable)this.GetEnumerator();
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
