// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Tomlyn;

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
public sealed class Guuid : IEnumerable<string>
{
    /// <summary>
    /// utopia游戏所使用的GUUID的root.
    /// 不推荐插件使用,避免出现问题.
    /// </summary>
    public const string UTOPIA_ROOT = "Utopia";

    /// <summary>
    /// Will use this to separate root and each namespaces when use the string of Guuid.
    /// </summary>
    public const string SEPARATOR = ".";

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

        string[] strs = guuid.Split(SEPARATOR);

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
    public string Root { get; }

    [Key(1)]
    public string[] Nodes { get; }

    public static bool operator ==(Guuid c1, Guuid c2) => c1.Root == c2.Root && c1.Nodes.SequenceEqual(c2.Nodes);

    public static bool operator !=(Guuid c1, Guuid c2) => (c1.Root != c2.Root) || (!c1.Nodes.SequenceEqual(c2.Nodes));

    /// <summary>
    /// 把guuid转换为字符串形式. Will use <see cref="SEPARATOR"/> to separate root and each namespaces.
    /// For example,
    /// a guuid with root `r` and namespaces `a` and `b` will have a string form as `r.a.b`
    /// (If <see cref="SEPARATOR"/> is `.`)
    /// </summary>
    public override string ToString()
    {
        StringBuilder builder = new();

        _ = builder.Append(Root);
        foreach (string node in Nodes)
        {
#pragma warning disable CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
            _ = builder.Append(SEPARATOR).Append(node);
#pragma warning restore CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
        }
        return builder.ToString();
    }

    /// <summary>
    /// 从字符串解析Guuid
    /// </summary>
    /// <param name="s">字符串应该是来自Guuid的ToString()的结果。</param>
    /// <exception cref="ArgumentException">输入的字符串有误</exception>
    public static Guuid Parse(string s) => !TryParse(s, out Guuid? result, out string? msg) ? throw new ArgumentException(msg) : result!;

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

        string[] strs = s.Split(SEPARATOR);

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

    public static Guuid NewUtopiaGuuid(params string[] nodes) => new(UTOPIA_ROOT, nodes);

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

    public static TomlModelOptions AddTomlOption(TomlModelOptions? options = null)
    {
        options ??= new TomlModelOptions();

        Func<object, object?>? defaultToToml = options.ConvertToToml;
        Func<object, Type, object?>? defaultToModel = options.ConvertToModel;

        options.ConvertToToml = (obj) =>
        {
            return obj is Guuid id ? id.ToString() : (defaultToToml?.Invoke(obj));
        };
        options.ConvertToModel = (obj, type) =>
        {
            return type.IsAssignableTo(typeof(Guuid))
                ? Parse((string)obj)
                : obj is Guuid id && type.IsAssignableTo(typeof(string)) ? id.ToString() : (defaultToModel?.Invoke(obj, type));
        };
        return options;
    }
}

public class GuuidMsgPackFormatter : IMessagePackFormatter<Guuid>
{
    public static readonly GuuidMsgPackFormatter Instance = new();

    public Guuid Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null!;
        }

        options.Security.DepthStep(ref reader);

        string? id = reader.ReadString();

        reader.Depth--;

        return Guuid.Parse(id!)!;
    }

    public void Serialize(ref MessagePackWriter writer, Guuid value, MessagePackSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNil();
            return;
        }

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
