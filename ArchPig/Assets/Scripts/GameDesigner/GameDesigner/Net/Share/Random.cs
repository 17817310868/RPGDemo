namespace Net.Share
{
    using System;

    /// <summary>
    /// 随机类
    /// </summary>
    public class Random : System.Random
    {
        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Range(int minValue, int maxValue)
        {
            System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
            if (minValue > maxValue)
            {
                return random.Next(maxValue, minValue);
            }
            return random.Next(minValue, maxValue);
        }

        /// <summary>
        /// 随机范围
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float Range(float minValue, float maxValue)
        {
            System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
            if (minValue > maxValue)
            {
                return random.Next((int)maxValue * 10, (int)minValue * 10) * 0.1f;
            }
            return random.Next((int)minValue * 10, (int)maxValue * 10) * 0.1f;
        }
    }
}