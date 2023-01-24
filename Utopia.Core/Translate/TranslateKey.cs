//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Translate;

/// <summary>
/// 翻译条目结果，非线程安全。
/// </summary>
/// <param name="Cached">翻译缓存</param>
/// <param name="Id">翻译缓存的ID，这个数值可以用于热更新翻译</param>
public record class TranslateResult(string? Cached, long Id);

/// <summary>
/// 翻译键，非线程安全。
/// </summary>
/// <param name="TranslateProviderId">翻译提供者ID</param>
/// <param name="TranslateItemId">翻译条目ID</param>
/// <param name="Id">翻译缓存</param>
public record struct TranslateKey(in Guuid? TranslateProviderId, in Guuid TranslateItemId, TranslateResult Id);
