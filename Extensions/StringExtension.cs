namespace SuumoScraping.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Stringクラス拡張メソッド
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToNGram(this string self, byte ngramSize)
        {
            var ret = new List<string>();

            // 空白は余計なので除去する
            var text = self.Replace("　", "").Replace(" ", "");

            for (var i = 0; i < text.Length - 1; i++)
            {
                // 残りの文字数を求める
                var rem = text.Length - i;

                rem = rem >= ngramSize ? ngramSize : rem;

                var word = text.Substring(i, rem);

                ret.Add(word);

                // 分かち文字数以下になったらループを抜ける
                if (rem < ngramSize)
                {
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// 文字列を数値化
        /// </summary>
        /// <param name="self">Stringインスタンス</param>
        /// <returns></returns>
        public static decimal ToDigit(this string self)
        {
            var number = 0m;
            var stack = new StringBuilder("0");

            foreach (var c in self)
            {
                var result = (decimal)0;
                if (decimal.TryParse(c.ToString(), out result))
                {
                    stack.Append(c);
                }
                else
                {
                    if (c.ToString() == ".")
                    {
                        stack.Append(c);
                    }
                    else if (c.ToString() == "億")
                    {
                        number += decimal.Parse(stack.ToString()) * 100000000m;
                        stack = stack.Clear().Append("0");
                    }
                    else if (c.ToString() == "万")
                    {
                        number += decimal.Parse(stack.ToString()) * 10000m;
                        stack = stack.Clear().Append("0");
                    }
                    else if (c.ToString() == "円")
                    {
                        number += decimal.Parse(stack.ToString()) * 1m;
                        stack = stack.Clear().Append("0");
                    }
                }
            }

            return number;
        }
    }
}