// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Globalization;

namespace Utopia.Core.Translation;

/// <summary>
/// 翻译唯一标识符号，用于标识一个翻译。
/// </summary>
public readonly struct TranslateIdentifence
{
    /// <summary>
    /// ISO 639-1所指定的两位字母语言编码。
    /// </summary>
    public readonly string Language;

    /// <summary>
    /// ISO 3166-1所指定的两位字母地区编码。
    /// </summary>
    public readonly string Location;

    public TranslateIdentifence()
    {
        Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
        Location = RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower();
    }

    /// <summary>
    /// Can parse [LANGUAGE][separator][LOCATION].
    /// [separator] can be '-' or '_' or ' '(SPACE).
    /// [LANGUAGE] and [LOCATION] obey the ISO 639-1 and ISO 3166-1 standard(Two letter version).
    /// </summary>
    public static TranslateIdentifence Parse(string id) => !TryParse(id, out TranslateIdentifence? identifence)
            ? throw new ArgumentException("the format of TranslateIdentifence is invalid")
            : identifence!.Value;

    /// <summary>
    /// see <see cref="Parse(string)"/>
    /// </summary>
    public static bool TryParse(string id, out TranslateIdentifence? result)
    {
        ArgumentNullException.ThrowIfNull(id);

        string[] parts = id.Split('_', '-', ' ');

        if (parts.Length != 2)
        {
            result = null;
            return false;
        }

        result = new TranslateIdentifence(parts[0], parts[1]);
        return true;
    }

    /// <summary>
    /// 构造一个翻译条目
    /// </summary>
    /// <param name="language">ISO 639-1标准语言代码(Two letter)</param>
    /// <param name="location">ISO 3166-1标准地区代码(Two letter)</param>
    /// <exception cref="ArgumentException">如果参数不符合标准。</exception>
    public TranslateIdentifence(string language, string location)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(location);

        if (language.Length != 2 || location.Length != 2)
        {
            throw new ArgumentException("the length of language or location is not 2");
        }
        if (!(language.All(char.IsLetter) && location.All(char.IsLetter)))
        {
            throw new ArgumentException("the language or location is not all letter");
        }

        Language = language.ToLower();
        Location = location.ToLower();
    }

    public override string ToString() => Language + "_" + Location;

    public override int GetHashCode() => ToString().GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType().IsAssignableFrom(GetType()))
        {
            var o = (TranslateIdentifence)obj;
            return o.Language == Language && o.Location == Location;
        }
        return false;
    }

    public static bool operator ==(TranslateIdentifence left, TranslateIdentifence right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TranslateIdentifence left, TranslateIdentifence right)
    {
        return !(left == right);
    }
}
