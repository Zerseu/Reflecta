#region Using

using System;

#endregion

namespace Reflecta
{
    public static class MathHelper
    {
        public const float E = 2.71828182846f;
        public const float Pi = 3.14159265359f;
        public const float TwoPi = Pi*2;
        public const float PiOver2 = Pi/2;
        public const float PiOver3 = Pi/3;
        public const float PiOver4 = Pi/4;
        public const float PiOver6 = Pi/6;
        public const float Radians2Degrees = 57.29577951308f;
        public const float Degrees2Radians = 0.01745329251f;

        public static float Clamp(float v, float min, float max)
        {
            if (v < min)
                return min;
            if (v > max)
                return max;
            return v;
        }

        public static float Lerp(float v1, float v2, float amount)
        {
            return v1 + (v2 - v1)*amount;
        }

        public static float Average(params float[] values)
        {
            if (values == null || values.Length == 0)
                throw new Exception("Empty values list!");

            float ret = 0;

            for (var i = 0; i < values.Length; i++)
                ret += values[i];

            ret /= values.Length;

            return ret;
        }
    }
}