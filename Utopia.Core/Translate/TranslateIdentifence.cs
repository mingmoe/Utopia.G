#region copyright
// This file(may named TranslateIdentifence.cs) is a part of the project: Utopia.Core.
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

using System.Globalization;

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

    public TranslateIdentifence()
    {
        this.Language = CultureInfo.CurrentCulture.ThreeLetterISOLanguageName.ToLower();
        this.Location = RegionInfo.CurrentRegion.ThreeLetterISORegionName.ToLower();
    }

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

        this.Language = language.ToLower();
        this.Location = location.ToLower();
    }

    public override string ToString()
    {
        return this.Language + "_" + this.Location;
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
