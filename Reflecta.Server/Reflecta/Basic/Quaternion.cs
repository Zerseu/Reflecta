#region Using

using System;
using ProtoBuf;

#endregion

namespace Reflecta
{
    [ProtoContract]
    public struct Quaternion
    {
        [ProtoMember(1)] public float X;
        [ProtoMember(2)] public float Y;
        [ProtoMember(3)] public float Z;
        [ProtoMember(4)] public float W;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Quaternion Average(params Quaternion[] values)
        {
            if (values == null || values.Length == 0)
                throw new Exception("Empty values list!");

            var amount = 1/(float) values.Length;

            var ret = new Quaternion();

            for (var i = 0; i < values.Length; i++)
            {
                var crt = values[i];

                if (!AreClose(crt, ret))
                    crt = Negate(crt);

                ret.X += crt.X*amount;
                ret.Y += crt.Y*amount;
                ret.Z += crt.Z*amount;
                ret.W += crt.W*amount;
            }

            return ret;
        }

        #region Math

        public static readonly Quaternion Zero = new Quaternion(0, 0, 0, 0);
        public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);

        public static Quaternion Add(Quaternion q1, Quaternion q2)
        {
            Quaternion ret;
            ret.X = q1.X + q2.X;
            ret.Y = q1.Y + q2.Y;
            ret.Z = q1.Z + q2.Z;
            ret.W = q1.W + q2.W;
            return ret;
        }

        public static Quaternion Subtract(Quaternion q1, Quaternion q2)
        {
            Quaternion ret;
            ret.X = q1.X - q2.X;
            ret.Y = q1.Y - q2.Y;
            ret.Z = q1.Z - q2.Z;
            ret.W = q1.W - q2.W;
            return ret;
        }

        public static Quaternion Negate(Quaternion q)
        {
            Quaternion ret;
            ret.X = -q.X;
            ret.Y = -q.Y;
            ret.Z = -q.Z;
            ret.W = -q.W;
            return ret;
        }

        public static Quaternion Conjugate(Quaternion q)
        {
            Quaternion ret;
            ret.X = -q.X;
            ret.Y = -q.Y;
            ret.Z = -q.Z;
            ret.W = q.W;
            return ret;
        }

        public static Quaternion Inverse(Quaternion q)
        {
            Quaternion ret;
            var num2 = q.X*q.X + q.Y*q.Y + q.Z*q.Z + q.W*q.W;
            var num = 1/num2;
            ret.X = -q.X*num;
            ret.Y = -q.Y*num;
            ret.Z = -q.Z*num;
            ret.W = q.W*num;
            return ret;
        }

        public static Quaternion Multiply(Quaternion q1, Quaternion q2)
        {
            Quaternion ret;
            var x = q1.X;
            var y = q1.Y;
            var z = q1.Z;
            var w = q1.W;
            var num4 = q2.X;
            var num3 = q2.Y;
            var num2 = q2.Z;
            var num = q2.W;
            var num12 = y*num2 - z*num3;
            var num11 = z*num4 - x*num2;
            var num10 = x*num3 - y*num4;
            var num9 = x*num4 + y*num3 + z*num2;
            ret.X = x*num + num4*w + num12;
            ret.Y = y*num + num3*w + num11;
            ret.Z = z*num + num2*w + num10;
            ret.W = w*num - num9;
            return ret;
        }

        public static Quaternion Multiply(Quaternion q, float scalar)
        {
            Quaternion ret;
            ret.X = q.X*scalar;
            ret.Y = q.Y*scalar;
            ret.Z = q.Z*scalar;
            ret.W = q.W*scalar;
            return ret;
        }

        public static Quaternion Divide(Quaternion q1, Quaternion q2)
        {
            Quaternion ret;
            var x = q1.X;
            var y = q1.Y;
            var z = q1.Z;
            var w = q1.W;
            var num14 = q2.X*q2.X + q2.Y*q2.Y + q2.Z*q2.Z +
                        q2.W*q2.W;
            var num5 = 1/num14;
            var num4 = -q2.X*num5;
            var num3 = -q2.Y*num5;
            var num2 = -q2.Z*num5;
            var num = q2.W*num5;
            var num13 = y*num2 - z*num3;
            var num12 = z*num4 - x*num2;
            var num11 = x*num3 - y*num4;
            var num10 = x*num4 + y*num3 + z*num2;
            ret.X = x*num + num4*w + num13;
            ret.Y = y*num + num3*w + num12;
            ret.Z = z*num + num2*w + num11;
            ret.W = w*num - num10;
            return ret;
        }

        public static Quaternion Normalize(Quaternion q)
        {
            Quaternion ret;
            var num2 = q.X*q.X + q.Y*q.Y + q.Z*q.Z + q.W*q.W;
            var num = (float) (1/Math.Sqrt(num2));
            ret.X = q.X*num;
            ret.Y = q.Y*num;
            ret.Z = q.Z*num;
            ret.W = q.W*num;
            return ret;
        }

        public static float Dot(Quaternion q1, Quaternion q2)
        {
            return q1.X*q2.X + q1.Y*q2.Y + q1.Z*q2.Z + q1.W*q2.W;
        }

        public static float LengthSquared(Quaternion q)
        {
            return q.X*q.X + q.Y*q.Y + q.Z*q.Z + q.W*q.W;
        }

        public static float Length(Quaternion q)
        {
            var num = q.X*q.X + q.Y*q.Y + q.Z*q.Z + q.W*q.W;
            return (float) Math.Sqrt(num);
        }

        public static Quaternion Concatenate(Quaternion q1, Quaternion q2)
        {
            Quaternion ret;
            var x = q2.X;
            var y = q2.Y;
            var z = q2.Z;
            var w = q2.W;
            var num4 = q1.X;
            var num3 = q1.Y;
            var num2 = q1.Z;
            var num = q1.W;
            var num12 = y*num2 - z*num3;
            var num11 = z*num4 - x*num2;
            var num10 = x*num3 - y*num4;
            var num9 = x*num4 + y*num3 + z*num2;
            ret.X = x*num + num4*w + num12;
            ret.Y = y*num + num3*w + num11;
            ret.Z = z*num + num2*w + num10;
            ret.W = w*num - num9;
            return ret;
        }

        public static Quaternion AxisAngle(Vector3 axis, float angle)
        {
            angle *= MathHelper.Degrees2Radians;

            Quaternion ret;
            var num2 = angle*0.5f;
            var num = (float) Math.Sin(num2);
            var num3 = (float) Math.Cos(num2);
            ret.X = axis.X*num;
            ret.Y = axis.Y*num;
            ret.Z = axis.Z*num;
            ret.W = num3;
            return ret;
        }

        public static Quaternion FromEuler(Vector3 euler)
        {
            var yaw = euler.Y*MathHelper.Degrees2Radians;
            var pitch = euler.X*MathHelper.Degrees2Radians;
            var roll = euler.Z*MathHelper.Degrees2Radians;

            var rollOver2 = roll*0.5f;
            var sinRollOver2 = (float) Math.Sin(rollOver2);
            var cosRollOver2 = (float) Math.Cos(rollOver2);
            var pitchOver2 = pitch*0.5f;
            var sinPitchOver2 = (float) Math.Sin(pitchOver2);
            var cosPitchOver2 = (float) Math.Cos(pitchOver2);
            var yawOver2 = yaw*0.5f;
            var sinYawOver2 = (float) Math.Sin(yawOver2);
            var cosYawOver2 = (float) Math.Cos(yawOver2);

            Quaternion ret;
            ret.W = cosYawOver2*cosPitchOver2*cosRollOver2 + sinYawOver2*sinPitchOver2*sinRollOver2;
            ret.X = cosYawOver2*sinPitchOver2*cosRollOver2 + sinYawOver2*cosPitchOver2*sinRollOver2;
            ret.Y = sinYawOver2*cosPitchOver2*cosRollOver2 - cosYawOver2*sinPitchOver2*sinRollOver2;
            ret.Z = cosYawOver2*cosPitchOver2*sinRollOver2 - sinYawOver2*sinPitchOver2*cosRollOver2;
            return ret;
        }

        public static Vector3 ToEuler(Quaternion q)
        {
            var sqw = q.W*q.W;
            var sqx = q.X*q.X;
            var sqy = q.Y*q.Y;
            var sqz = q.Z*q.Z;

            var unit = sqx + sqy + sqz + sqw;
            var test = q.X*q.W - q.Y*q.Z;

            Vector3 ret;

            if (test > 0.499f*unit)
            {
                ret.Y = 2*(float) Math.Atan2(q.Y, q.X);
                ret.X = MathHelper.PiOver2;
                ret.Z = 0;
                return ret*MathHelper.Radians2Degrees;
            }
            if (test < -0.499f*unit)
            {
                ret.Y = -2*(float) Math.Atan2(q.Y, q.X);
                ret.X = -MathHelper.PiOver2;
                ret.Z = 0;
                return ret*MathHelper.Radians2Degrees;
            }

            q = new Quaternion(q.W, q.Z, q.X, q.Y);

            ret.Y = (float) Math.Atan2(2*q.X*q.W + 2*q.Y*q.Z, 1 - 2*(q.Z*q.Z + q.W*q.W));
            ret.X = (float) Math.Asin(2*(q.X*q.Z - q.W*q.Y));
            ret.Z = (float) Math.Atan2(2*q.X*q.Y + 2*q.Z*q.W, 1 - 2*(q.Y*q.Y + q.Z*q.Z));

            return ret*MathHelper.Radians2Degrees;
        }

        public static Quaternion Lerp(Quaternion q1, Quaternion q2, float amount)
        {
            var num = amount;
            var num2 = 1 - num;
            var ret = new Quaternion();
            var num5 = q1.X*q2.X + q1.Y*q2.Y + q1.Z*q2.Z +
                       q1.W*q2.W;
            if (num5 >= 0)
            {
                ret.X = num2*q1.X + num*q2.X;
                ret.Y = num2*q1.Y + num*q2.Y;
                ret.Z = num2*q1.Z + num*q2.Z;
                ret.W = num2*q1.W + num*q2.W;
            }
            else
            {
                ret.X = num2*q1.X - num*q2.X;
                ret.Y = num2*q1.Y - num*q2.Y;
                ret.Z = num2*q1.Z - num*q2.Z;
                ret.W = num2*q1.W - num*q2.W;
            }
            var num4 = ret.X*ret.X + ret.Y*ret.Y + ret.Z*ret.Z +
                       ret.W*ret.W;
            var num3 = (float) (1/Math.Sqrt(num4));
            ret.X *= num3;
            ret.Y *= num3;
            ret.Z *= num3;
            ret.W *= num3;
            return ret;
        }

        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float amount)
        {
            float num2;
            float num3;
            Quaternion ret;
            var num = amount;
            var num4 = q1.X*q2.X + q1.Y*q2.Y + q1.Z*q2.Z +
                       q1.W*q2.W;
            var flag = false;
            if (num4 < 0)
            {
                flag = true;
                num4 = -num4;
            }
            if (num4 > 0.999999f)
            {
                num3 = 1 - num;
                num2 = flag ? -num : num;
            }
            else
            {
                var num5 = (float) Math.Acos(num4);
                var num6 = (float) (1/Math.Sin(num5));
                num3 = (float) (Math.Sin((1 - num)*num5)*num6);
                num2 = flag ? (float) (-Math.Sin(num*num5)*num6) : (float) (Math.Sin(num*num5)*num6);
            }
            ret.X = num3*q1.X + num2*q2.X;
            ret.Y = num3*q1.Y + num2*q2.Y;
            ret.Z = num3*q1.Z + num2*q2.Z;
            ret.W = num3*q1.W + num2*q2.W;
            return ret;
        }

        public static Quaternion SlerpNeighborhood(Quaternion q1, Quaternion q2, float amount)
        {
            q2 = EnsureNeighborhood(q1, q2);
            return Slerp(q1, q2, amount);
        }

        public static bool AreClose(Quaternion q1, Quaternion q2)
        {
            return Dot(q1, q2) >= 0;
        }

        public static Quaternion EnsureNeighborhood(Quaternion q1, Quaternion q2)
        {
            if (!AreClose(q1, q2))
                return Negate(q2);
            return q2;
        }

        public static float Angle(Quaternion q)
        {
            q = Normalize(q);
            return (float) (2*Math.Acos(q.W))*MathHelper.Radians2Degrees;
        }

        public static Quaternion RotationFromTo(Vector3 v1, Vector3 v2)
        {
            v1 = Vector3.Normalize(v1);
            v2 = Vector3.Normalize(v2);
            var angle = (float) Math.Acos(Vector3.Dot(v1, v2));
            var axis = Vector3.Cross(v1, v2);

            if (angle < 0.001f)
                axis = Vector3.Up;
            if (Vector3.Length(axis) < 0.001f)
                return Identity;

            axis = Vector3.Normalize(axis);
            return AxisAngle(axis, angle);
        }

        public static Quaternion RotationBetween(Quaternion q1, Quaternion q2)
        {
            q2 = EnsureNeighborhood(q1, q2);
            return Multiply(Inverse(q1), q2);
        }

        public static Quaternion Constrain(Quaternion q, Vector3 minAngles, Vector3 maxAngles)
        {
            var e = ToEuler(q);
            e.X = MathHelper.Clamp(e.X, minAngles.X, maxAngles.X);
            e.Y = MathHelper.Clamp(e.Y, minAngles.Y, maxAngles.Y);
            e.Z = MathHelper.Clamp(e.Z, minAngles.Z, maxAngles.Z);
            return FromEuler(e);
        }

        #endregion

        #region Operators

        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
            {
                var aux = (Quaternion) obj;
                return X == aux.X && Y == aux.Y && Z == aux.Z && W == aux.W;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int) (X + Y + Z + W);
        }

        public override string ToString()
        {
            return string.Format("X={0} Y={1} Z={2} W={3}", X, Y, Z, W);
        }

        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            return q1.X == q2.X && q1.Y == q2.Y && q1.Z == q2.Z && q1.W == q2.W;
        }

        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            return q1.X != q2.X || q1.Y != q2.Y || q1.Z != q2.Z || q1.W != q2.W;
        }

        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return Add(q1, q2);
        }

        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return Subtract(q1, q2);
        }

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return Multiply(q1, q2);
        }

        public static Quaternion operator /(Quaternion q1, Quaternion q2)
        {
            return Divide(q1, q2);
        }

        public static Quaternion operator *(Quaternion q, float scalar)
        {
            return Multiply(q, scalar);
        }

        public static Quaternion operator *(float scalar, Quaternion q)
        {
            return Multiply(q, scalar);
        }

        public static Quaternion operator /(Quaternion q, float scalar)
        {
            return Multiply(q, 1/scalar);
        }

        public static Quaternion operator /(float scalar, Quaternion q)
        {
            return Multiply(Inverse(q), scalar);
        }

        public static Quaternion operator +(Quaternion q)
        {
            return q;
        }

        public static Quaternion operator -(Quaternion q)
        {
            return Negate(q);
        }

        #endregion
    }
}