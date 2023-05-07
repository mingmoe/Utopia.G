//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Translate;

/// <summary>
/// 翻译唯一标识符号，用于标识一个翻译。
/// </summary>
public class TranslateIdentifence
{
    /// <summary>
    /// ISO 639-3所指定的三位字母语言编码。
    /// </summary>
    public readonly string Language;

    /// <summary>
    /// ISO 3166-1所指定的三位字母地区编码。
    /// </summary>
    public readonly string Location;

    /// <summary>
    /// 构造一个翻译条目
    /// </summary>
    /// <param name="language">ISO 639-3标准语言代码</param>
    /// <param name="location">ISO 3166-1标准地区代码</param>
    /// <exception cref="ArgumentException">如果参数不符合标准。</exception>
    public TranslateIdentifence(string language, string location)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(location);

        if (language.Length != 3 || location.Length != 3)
        {
            throw new ArgumentException("the length of language or location is not 3");
        }
        if (!(language.All(char.IsLetter) && location.All(char.IsLetter)))
        {
            throw new ArgumentException("the language or location is not all letter");
        }

        Language = language.ToLower();
        Location = location.ToLower();
    }

    public override string ToString()
    {
        return Language + "_" + Location;
    }

    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType().IsAssignableFrom(this.GetType()))
        {
            var o = (TranslateIdentifence)obj;
            return o.Language == this.Language && o.Location == this.Location;
        }
        return false;
    }

}
