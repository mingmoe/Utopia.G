// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Utopia.Core.Utilities;

/// <summary>
/// globally unique universally identifier.
/// This is a constant class(like string).
/// 人类可读的唯一标识符。由name组成。至少必须要存在有两个name。
/// 第一个name称为root。其余的all name称为node(s)。
/// name的长度不能为0，只能由字母、数字组成。
/// 并且只能由字母开头。
/// guuid的字符串形式类似于：root:namespaces1:namespaces2...
/// </summary>
[MessagePackObject]
public readonly struct Guuid : IEnumerable<string>
{
    /// <summary>
    /// Use this regex pattern to check the guuid string
    /// </summary>
    public const string Pattern = @"^[a-zA-Z]{1}[a-zA-Z0-9]*(\.[a-zA-Z]{1}[a-zA-Z0-9]*)+$";

    /// <summary>
    /// utopia游戏所使用的GUUID的root.
    /// </summary>
    public const string UtopiaRoot = "Utopia";

    /// <summary>
    /// Will use this to separate root and each namespaces when use the string of Guuid.
    /// </summary>
    public const string Separator = ":";

    public static readonly Guuid Empty = new("Empty", "Empty");

    /// <summary>
    /// 检查name是否符合要求
    /// </summary>
    /// <param name="name">要检查的name</param>
    /// <returns>如果name合法，返回true。</returns>
    public static bool CheckName(string name) => !string.IsNullOrEmpty(name)
&& char.IsLetter(name.First()) && name.All((c) => char.IsLetter(c) || char.IsDigit(c));

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

        string[] strs = guuid.Split(Separator);

        // 至少要存在一个root和一个node
        return strs.Length >= 2 && CheckGuuid(strs.First(), strs[1..]);
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
        foreach (string node in nodes)
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

        Root = root;
        Nodes = nodes;
    }

    [Key(0)]
    public string Root { get; init; }

    [Key(1)]
    public string[] Nodes { get; init; }

    public static bool operator ==(Guuid c1, Guuid c2) => c1.Root == c2.Root && c1.Nodes.SequenceEqual(c2.Nodes);

    public static bool operator !=(Guuid c1, Guuid c2) => (c1.Root != c2.Root) || (!c1.Nodes.SequenceEqual(c2.Nodes));

    /// <summary>
    /// 把guuid转换为字符串形式. Will use <see cref="Separator"/> to separate root and each namespaces.
    /// For example,
    /// a guuid with root `r` and namespaces `a` and `b` will have a string form as `r:a:b`
    /// (If <see cref="Separator"/> is `.`)
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new();

        _ = builder.Append(Root);
        foreach (string node in Nodes)
        {
#pragma warning disable CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
            _ = builder.Append(Separator).Append(node);
#pragma warning restore CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
        }
        return builder.ToString();
    }

    /// <summary>
    /// 从字符串解析Guuid
    /// </summary>
    /// <param name="s">字符串应该是来自Guuid的ToString()的结果。</param>
    /// <exception cref="ArgumentException">输入的字符串有误</exception>
    public static Guuid Parse(string s) => !TryParse(s, out Guuid? result, out string? msg) ? throw new ArgumentException(msg) : result!.Value;

    /// <summary>
    /// See <see cref="Parse(string)"/>
    /// </summary>
    /// <param name="s"></param>
    /// <param name="result"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryParse(string s, out Guuid? result, out string? errorMessage)
    {
        if (string.IsNullOrEmpty(s))
        {
            throw new ArgumentException("param is empty or null");
        }
        result = null;
        errorMessage = null;

        string[] strs = s.Split(Separator);

        if (strs.Length < 2)
        {
            errorMessage = "the guuid format is illegal.(get too less substring from Split(),check the separator is right)";
            return false;
        }

        foreach (string item in strs)
        {
            if (!CheckName(item))
            {
                errorMessage = $"the name of guuid '{item}' is invalid.";
                return false;
            }
        }

        result = new Guuid(strs.First(), strs[1..]);
        return true;
    }

    /// <summary>
    /// 获取一个新的随机的标识符。
    /// </summary>
    public static Guuid Unique()
    {
        byte[] rno = RandomNumberGenerator.GetBytes(16);
        ulong high = BitConverter.ToUInt64(rno, 0);
        ulong low = BitConverter.ToUInt64(rno, 8);

        return new Guuid("Unique", string.Format("{0:X16}{1:X16}", high, low));
    }

    public override bool Equals(object? obj) => obj is not null && obj is Guuid guuid && this == guuid;

    public override int GetHashCode()
    {
        var hasher = new XxHash32(0);
        hasher.Append(Encoding.UTF8.GetBytes(Root));

        foreach (string node in Nodes)
        {
            hasher.Append(Encoding.UTF8.GetBytes(node));
        }

        byte[] bytes = hasher.GetHashAndReset();
        return BitConverter.ToInt32(bytes);
    }

    public Guuid Append(Guuid guuid)
    {
        string root = Root;
        var nodes = Nodes.ToList();
        nodes.Add(guuid.Root);
        nodes.AddRange(guuid.Nodes);
        return new Guuid(root, nodes.ToArray());
    }

    public Guuid Append(params string[] otherNodes)
    {
        string root = Root;
        var nodes = Nodes.ToList();
        nodes.AddRange(otherNodes);
        return new Guuid(root, nodes.ToArray());
    }

    /// <summary>
    /// Cover this guuid to C# identifier
    /// </summary>
    /// <returns></returns>
    public string ToCsIdentifier() => Nodes.Aggregate('@' + Root, (result, value) =>
                                           {
                                               return result + "_" + value;
                                           });

    public static Guuid NewUtopiaGuuid(params string[] nodes) => new(UtopiaRoot, nodes);

    public IEnumerator<string> GetEnumerator()
    {
        yield return Root;
        foreach (string s in Nodes)
        {
            yield return s;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        yield return (IEnumerable)GetEnumerator();
    }

    /// <summary>
    /// Check if another guuid is the child of this guuid.
    /// e.g. `a:b:c:d` is a child guuid of `a:b:c:d` or `a:b:c` or `a:b`.
    /// </summary>
    /// <returns>true if another guuid is the child of this.</returns>
    public bool HasChild(in Guuid id)
    {
        var node = id.GetEnumerator();
        node.MoveNext();
        foreach(var item in this)
        {
            if(node.Current != item)
            {
                return false;
            }

            // the child is short than(or equal in length) father!!!
            if (!node.MoveNext())
            {
                // check if two guuid are the same
                return id == this;
            }
        }

        return true;
    }

    /// <summary>
    /// Get an root of the guuid
    /// </summary>
    /// <returns></returns>
    public Guuid? GetParent()
    {
        if(Nodes.Length == 1)
        {
            return null;
        }

        return new Guuid(Root, Nodes[..(Nodes.Length - 1)]);
    }
}

/// <summary>
/// This class was used for xml.
/// Do not use it in other code.
/// </summary>
[XmlSchemaProvider(nameof(GetGuuidSchemaOfType))]
public sealed class XmlGuuid : IXmlSerializable
{
    public Guuid Guuid { get; set; } = Guuid.Empty;

    public XmlGuuid()
    {

    }

    public XmlGuuid(Guuid guuid)
    {
        Guuid = guuid;
    }

    public override bool Equals(object? obj) => Guuid.Equals(obj);

    public override int GetHashCode() => Guuid.GetHashCode();

    public override string ToString() => Guuid.ToString();

    public static XmlSchemaSimpleType GetGuuidSchemaOfType(XmlSchemaSet set)
    {
        XmlSchemaSimpleType type = new();

        {
            XmlSchemaSimpleTypeRestriction restriction = new();
            restriction.BaseTypeName = new("string", "http://www.w3.org/2001/XMLSchema");
            {
                XmlSchemaPatternFacet facet = new();
                facet.Value = Guuid.Pattern;
                restriction.Facets.Add(facet);
            }
            type.Content = restriction;
        }

        return type;
    }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        Guuid = Guuid.Parse(reader.ReadElementContentAsString());
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteString(Guuid.ToString());
    }
}

public class GuuidMsgPackFormatter : IMessagePackFormatter<Guuid>
{
    public static readonly GuuidMsgPackFormatter Instance = new();

    public Guuid Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        options.Security.DepthStep(ref reader);

        string? id = reader.ReadString();

        reader.Depth--;

        return Guuid.Parse(id!)!;
    }

    public void Serialize(ref MessagePackWriter writer, Guuid value, MessagePackSerializerOptions options)
    {

        string v = value.ToString();

        writer.WriteString(Encoding.UTF8.GetBytes(v));
    }

    public static MessagePackSerializerOptions CreateOption()
    {
        IFormatterResolver resolver = CompositeResolver.Create(
            new[] { Instance },
             new[] { StandardResolver.Instance });

        return MessagePackSerializerOptions.Standard.WithResolver(resolver);
    }
}