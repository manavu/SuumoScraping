namespace SuumoScraping.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// IDictionary 型の拡張メソッドを管理するクラス
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// 指定したキーに関連付けられている値を取得します。
        /// キーが存在しない場合は既定値を返します
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> self,
            TKey key,
            TValue defaultValue = default(TValue))
        {
            return self.TryGetValue(key,out TValue value) ? value : defaultValue;
        }
    }
}
