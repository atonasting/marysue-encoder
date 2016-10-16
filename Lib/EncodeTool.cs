using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MareSueEncoder.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MareSueEncoder.Lib
{
    public class EncodeTool
    {
        #region 加密
        /// <summary>
        /// 把字符数组切成1~8随机长度的小数组、转换成ulong数字、生成密文再拼接为长字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Characters"></param>
        public static string ByteArrayToString(byte[] source)
        {
            var cursor = 0;
            var length = 0;
            var rand = new Random();
            var result = new List<string>();

            while (cursor < source.Length)
            {
                length = rand.Next(1, 8);
                if (cursor + length > source.Length) length = source.Length - cursor;

                if (source[cursor + length - 1] == 0)//如果小数组最后一位是0，会造成生成数字时无法判断此处为0还是空值，需要重新随机选一次
                    continue;

                var bytepiece = new byte[8];
                Array.Copy(source, cursor, bytepiece, 0, length);
                var number = BitConverter.ToUInt64(bytepiece, 0);
                result.Add(UlongToString(number, GlobalConfig.Material));
                cursor += length;
            }

            return string.Join(GlobalConfig.Spliter, result.ToArray());
        }

        /// <summary>
        ///把数字转为最长8个字符
        /// </summary>
        /// <param name="number"></param>
        /// <param name="Characters"></param>
        /// <returns></returns>
        private static string UlongToString(ulong number, string Characters)
        {
            var result = new List<char>();
            var t = number;
            var length = (uint)Characters.Length;

            ulong checkNumber = 0;
            
            while (t > 0)
            {
                var q = t % length;
                result.Add(Characters[Convert.ToInt32(q)]);
                checkNumber = checkNumber * length + q;
                t = t / length;
            }
            return string.Join("", result.ToArray());
        }
        #endregion

        #region 解密
        /// <summary>
        /// 把密文字符串拆分成小段，转换成ulong数字再转换成原文字符数组
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Characters"></param>
        public static byte[] StringToByteArray(string code)
        {
            var subCodes = code.Split(new string[] { GlobalConfig.Spliter }, StringSplitOptions.RemoveEmptyEntries);
            var source = new List<byte>();
            foreach (var subCode in subCodes)
            {
                var number = StringToUlong(subCode, GlobalConfig.Material);
                var sourceBytes = BitConverter.GetBytes(number);

                //去掉数组末尾的0
                int trimLength = sourceBytes.Length - 1;
                while (sourceBytes[trimLength] == 0) trimLength--;
                trimLength++;
                var sourceTrim = new byte[trimLength];
                Array.Copy(sourceBytes, sourceTrim, trimLength);

                source.AddRange(sourceTrim);
            }

            return source.ToArray();
        }

        /// <summary>
        /// 把小段密文字符串转换成数字
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Characters"></param>
        /// <returns></returns>
        private static ulong StringToUlong(string text, string Characters)
        {
            ulong number = 0;
            var length = (uint)Characters.Length;
            for (var i = text.Length - 1; i >= 0; i--)
            {
                var q = Characters.IndexOf(text[i]);
                if (q == -1) throw new ArgumentOutOfRangeException("illegal characters");
                number = number * length + (uint)q;
            }

            return number;
        }
        #endregion
    }
}
