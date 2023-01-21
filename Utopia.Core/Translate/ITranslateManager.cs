//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Translate
{
    /// <summary>
    /// 翻译管理器，可以通过此类获取翻译。
    /// </summary>
    public interface ITranslateManager
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
        /// 尝试注册翻译提供者
        /// </summary>
        /// <param name="id">翻译提供者Id</param>
        /// <param name="provider">翻译提供者</param>
        /// <returns>如果翻译提供者Id已经被注册，则返回false。否则返回true，注册成功。</returns>
        bool TryRegisterTranslateProvider(Guuid id, ITranslateProvider provider);

        /// <summary>
        /// 删除翻译提供者
        /// </summary>
        /// <param name="id">翻译提供者Id</param>
        void RemoveTranslateProvider(Guuid id);

        /// <summary>
        /// 尝试获取翻译提供者
        /// </summary>
        /// <param name="id">翻译提供者Id</param>
        /// <param name="result">结果，如果翻译提供者不存在，返回null。</param>
        /// <returns>如果翻译提供者存在返回true，否则返回false</returns>
        bool TryGetTranslateProvider(Guuid id, out ITranslateProvider? result);

        /// <summary>
        /// 检查翻译提供者Id是否已经被注册
        /// </summary>
        /// <param name="id">翻译提供者Id</param>
        /// <returns>如果已经被注册，返回true</returns>
        bool ContainsTranslateProvider(Guuid id);

        /// <summary>
        /// 检查翻译条目是否存在
        /// </summary>
        /// <param name="language">目标翻译语言</param>
        /// <param name="translateProviderId">翻译提供者id，如果为null，则在所有提供者中寻找翻译条目</param>
        /// <param name="translateItemId">要寻找的翻译条目Id</param>
        /// <returns>如果翻译条目存在，则返回true，否则返回false。</returns>
        bool ContainsTranslateItem(TranslateIdentifence language, Guuid? translateProviderId, Guuid translateItemId);

        /// <summary>
        /// 获取所有翻译提供者
        /// </summary>
        IReadOnlyCollection<KeyValuePair<Guuid, ITranslateProvider>> GetTranslateProviders();
    }
}
