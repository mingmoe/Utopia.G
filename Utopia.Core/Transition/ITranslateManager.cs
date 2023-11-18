// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Utilities;

namespace Utopia.Core.Transition;

/// <summary>
/// 翻译管理器，可以通过此接口获取翻译。要求线程安全.
/// </summary>
public interface ITranslateManager : ISafeDictionary<Guuid, ITranslateProvider>
{
    /// <summary>
    /// 获取尝试获取翻译
    /// </summary>
    /// <param name="language">要获取的翻译语言</param>
    /// <param name="translateProviderId">
    /// 翻译提供者ID，如果为null，则在所有翻译提供者中搜索翻译条目。
    /// 如果多个翻译提供者提供相同的翻译条目，返回结果不定。</param>
    /// <param name="translateItemId">翻译条目ID</param>
    /// <param name="result">获取到的结果，如果获取失败，设置为null。</param>
    /// <returns>如果获取成功，返回true。</returns>
    bool TryGetTranslate(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId, out string? result);

    /// <summary>
    /// 检查翻译条目是否存在
    /// </summary>
    /// <param name="language">目标翻译语言</param>
    /// <param name="translateProviderId">翻译提供者id，如果为null，则在所有提供者中寻找翻译条目</param>
    /// <param name="translateItemId">要寻找的翻译条目Id</param>
    /// <returns>如果翻译条目存在，则返回true，否则返回false。</returns>
    bool Contains(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId);

    /// <summary>
    /// 翻译ID，<see cref="TranslateID"/>，用于标识一个特定的翻译。如果ID更新，则说明缓存的翻译失效，需要重新获取。
    /// </summary>
    long TranslateID { get; }

    /// <summary>
    /// 提示翻译管理器，所有翻译缓存作废，应该重新获取翻译。
    /// </summary>
    void UpdateTranslate();

    /// <summary>
    /// 翻译管理器更新事件。参数是this翻译管理器，事件可取消。
    /// 这个事件用于通知翻译管理器已经更新，请重新获取翻译。
    /// 事件**只能**由<see cref="UpdateTranslate"/>触发
    /// </summary>
    IEventManager<EventWithParam<ITranslateManager>> TranslateUpdatedEvent { get; }
}