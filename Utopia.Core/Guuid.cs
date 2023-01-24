//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Standart.Hash.xxHash;
using System.Security.Cryptography;
using System.Text;

namespace Utopia.Core;

/// <summary>
/// 人类可读的唯一标识符。由name组成。至少必须要存在有两个name。
/// 第一个name称为root。其余的name称为node。
/// name的长度不能为0，没有其他限制。
/// 但是一些人类readable的名称不是更好么:-)。
/// </summary>
public class Guuid
{

    /// <summary>
    /// 检查name是否符合要求
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool CheckNameIllegal(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return true;
    }

    ///
    /// <exception cref="ArgumentException">如果root或者nodes不符合规范则抛出</exception>
    public Guuid(string root, params string[] nodes)
    {
        ArgumentNullException.ThrowIfNull(root, nameof(root));
        ArgumentNullException.ThrowIfNull(nodes, nameof(nodes));


        if (!CheckNameIllegal(root))
        {
            throw new ArgumentException("the root name is illegal");
        }
        foreach (var node in nodes)
        {
            if (!CheckNameIllegal(node))
            {
                throw new ArgumentException("the node name is illegal");
            }
        }

        this.Root = root;
        this.Nodes = nodes;
    }
    public string Root { get; }
    public string[] Nodes { get; }

    public static bool operator ==(Guuid c1, Guuid c2)
    {
        return c1.Root == c2.Root && Enumerable.SequenceEqual(c1.Nodes, c2.Nodes);
    }

    public static bool operator !=(Guuid c1, Guuid c2)
    {
        return c1.Root != c2.Root || !Enumerable.SequenceEqual(c1.Nodes, c2.Nodes);
    }


    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(this.Root.Replace("\\", "\\\\").Replace(":", "\\:"));
        foreach (var node in Nodes)
        {
            builder.Append(':');
            builder.Append(node.Replace("\\", "\\\\").Replace(":", "\\:"));
        }

        return builder.ToString();
    }

    /// <summary>
    /// 从字符串解析Guuid
    /// </summary>
    /// <param name="s">字符串应该是来自Guuid的ToString()的结果。</param>
    /// <exception cref="ArgumentException">参数异常</exception>
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

        var result = strs.Select(x => x.Replace("\\\\", "\\").Replace("\\:", ":")).ToArray();

        return new Guuid(result.First(), result[1..]);
    }

    /// <summary>
    /// 获取一个新的随机的标识符。
    /// </summary>
    public static Guuid Unique()
    {
        ulong high = 0;
        ulong low = 0;
        using (var rg = RandomNumberGenerator.Create())
        {

            byte[] rno = new byte[16];
            rg.GetNonZeroBytes(rno);
            high = BitConverter.ToUInt64(rno, 0);
            low = BitConverter.ToUInt64(rno, 8);
        }

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

}
