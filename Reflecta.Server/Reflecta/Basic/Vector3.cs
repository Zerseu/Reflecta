#region Using

using System;
using ProtoBuf;

#endregion

namespace Reflecta
{
    [ProtoContract]
    public struct Vector3
    {
        [ProtoMember(1)] public float X;
        [ProtoMember(2)] public float Y;
        [ProtoMember(3)] public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 Average(params Vector3[] values)
        {
            if (values == null || values.Length == 0)
                throw new Exception("Empty values list!");

            var ret = new Vector3();

            for (var i = 0; i < values.Length; i++)
            {
                ret.X += values[i].X;
                ret.Y += values[i].Y;
                ret.Z += values[i].Z;
            }

            ret.X /= values.Length;
            ret.Y /= values.Length;
            ret.Z /= values.Length;

            return ret;
        }

        #region Math

        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 One = new Vector3(1, 1, 1);
        public static readonly Vector3 Forward = new Vector3(0, 0, 1);
        public static readonly Vector3 Backward = new Vector3(0, 0, -1);
        public static readonly Vector3 Up = new Vector3(0, 1, 0);
        public static readonly Vector3 Down = new Vector3(0, -1, 0);
        public static readonly Vector3 Right = new Vector3(1, 0, 0);
        public static readonly Vector3 Left = new Vector3(-1, 0, 0);

        public static Vector3 Add(Vector3 v1, Vector3 v2)
        {
            v1.X += v2.X;
            v1.Y += v2.Y;
            v1.Z += v2.Z;
            return v1;
        }

        public static Vector3 Subtract(Vector3 v1, Vector3 v2)
        {
            v1.X -= v2.X;
            v1.Y -= v2.Y;
            v1.Z -= v2.Z;
            return v1;
        }

        public static Vector3 Negate(Vector3 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
            return v;
        }

        public static Vector3 Multiply(Vector3 v1, Vector3 v2)
        {
            v1.X *= v2.X;
            v1.Y *= v2.Y;
            v1.Z *= v2.Z;
            return v1;
        }

        public static Vector3 Multiply(Vector3 v, float scalar)
        {
            v.X *= scalar;
            v.Y *= scalar;
            v.Z *= scalar;
            return v;
        }

        public static Vector3 Divide(Vector3 v1, Vector3 v2)
        {
            v1.X /= v2.X;
            v1.Y /= v2.Y;
            v1.Z /= v2.Z;
            return v1;
        }

        public static Vector3 Normalize(Vector3 v)
        {
            var length = Length(v);
            v.X /= length;
            v.Y /= length;
            v.Z /= length;
            return v;
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            var ret = new Vector3
            {
                X = v1.Y*v2.Z - v2.Y*v1.Z,
                Y = -v1.X*v2.Z + v2.X*v1.Z,
                Z = v1.X*v2.Y - v2.X*v1.Y
            };
            return ret;
        }

        public static float LengthSquared(Vector3 v)
        {
            return v.X*v.X + v.Y*v.Y + v.Z*v.Z;
        }

        public static float Length(Vector3 v)
        {
            return (float) Math.Sqrt(LengthSquared(v));
        }

        public static Vector3 Lerp(Vector3 v1, Vector3 v2, float amount)
        {
            return new Vector3(MathHelper.Lerp(v1.X, v2.X, amount), MathHelper.Lerp(v1.Y, v2.Y, amount),
                MathHelper.Lerp(v1.Z, v2.Z, amount));
        }

        public static float DistanceSquared(Vector3 v1, Vector3 v2)
        {
            return (v1.X - v2.X)*(v1.X - v2.X) +
                   (v1.Y - v2.Y)*(v1.Y - v2.Y) +
                   (v1.Z - v2.Z)*(v1.Z - v2.Z);
        }

        public static float Distance(Vector3 v1, Vector3 v2)
        {
            return (float) Math.Sqrt(DistanceSquared(v1, v2));
        }

        public static Vector3 Reflect(Vector3 v, Vector3 n)
        {
            Vector3 ret;
            var dot = Dot(v, n);
            ret.X = v.X - 2*n.X*dot;
            ret.Y = v.Y - 2*n.Y*dot;
            ret.Z = v.Z - 2*n.Z*dot;
            return ret;
        }

        public static Vector3 Project(Vector3 vector, Vector3 normal)
        {
            var num = Dot(normal, normal);
            if (num < 0.000001f)
                return Zero;
            return Multiply(normal, Dot(vector, normal)/num);
        }

        public static Vector3 Transform(Vector3 v, Quaternion q)
        {
            var x = 2*(q.Y*v.Z - q.Z*v.Y);
            var y = 2*(q.Z*v.X - q.X*v.Z);
            var z = 2*(q.X*v.Y - q.Y*v.X);

            var ret = new Vector3
            {
                X = v.X + x*q.W + q.Y*z - q.Z*y,
                Y = v.Y + y*q.W + q.Z*x - q.X*z,
                Z = v.Z + z*q.W + q.X*y - q.Y*x
            };
            return ret;
        }

        public static void Add(ref Vector3 v1, ref Vector3 v2, out Vector3 result)
        {
            result.X = v1.X + v2.X;
            result.Y = v1.Y + v2.Y;
            result.Z = v1.Z + v2.Z;
        }

        public static void Subtract(ref Vector3 v1, ref Vector3 v2, out Vector3 result)
        {
            result.X = v1.X - v2.X;
            result.Y = v1.Y - v2.Y;
            result.Z = v1.Z - v2.Z;
        }

        public static void Negate(ref Vector3 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
        }

        public static void Multiply(ref Vector3 v1, ref Vector3 v2, out Vector3 result)
        {
            result.X = v1.X*v2.X;
            result.Y = v1.Y*v2.Y;
            result.Z = v1.Z*v2.Z;
        }

        public static void Multiply(ref Vector3 v, ref float scalar, out Vector3 result)
        {
            result.X = v.X*scalar;
            result.Y = v.Y*scalar;
            result.Z = v.Z*scalar;
        }

        public static void Divide(ref Vector3 v1, ref Vector3 v2, out Vector3 result)
        {
            result.X = v1.X/v2.X;
            result.Y = v1.Y/v2.Y;
            result.Z = v1.Z/v2.Z;
        }

        public static void Normalize(ref Vector3 v)
        {
            float length;
            Length(ref v, out length);
            v.X /= length;
            v.Y /= length;
            v.Z /= length;
        }

        public static void Dot(ref Vector3 v1, ref Vector3 v2, out float result)
        {
            result = v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z;
        }

        public static void Cross(ref Vector3 v1, ref Vector3 v2, out Vector3 result)
        {
            result.X = v1.Y*v2.Z - v2.Y*v1.Z;
            result.Y = -v1.X*v2.Z + v2.X*v1.Z;
            result.Z = v1.X*v2.Y - v2.X*v1.Y;
        }

        public static void LengthSquared(ref Vector3 v, out float result)
        {
            result = v.X*v.X + v.Y*v.Y + v.Z*v.Z;
        }

        public static void Length(ref Vector3 v, out float result)
        {
            LengthSquared(ref v, out result);
            result = (float) Math.Sqrt(result);
        }

        public static void Lerp(ref Vector3 v1, ref Vector3 v2, ref float amount, out Vector3 result)
        {
            result.X = MathHelper.Lerp(v1.X, v2.X, amount);
            result.Y = MathHelper.Lerp(v1.Y, v2.Y, amount);
            result.Z = MathHelper.Lerp(v1.Z, v2.Z, amount);
        }

        public static void DistanceSquared(ref Vector3 v1, ref Vector3 v2, out float result)
        {
            result = (v1.X - v2.X)*(v1.X - v2.X) +
                     (v1.Y - v2.Y)*(v1.Y - v2.Y) +
                     (v1.Z - v2.Z)*(v1.Z - v2.Z);
        }

        public static void Distance(ref Vector3 v1, ref Vector3 v2, out float result)
        {
            DistanceSquared(ref v1, ref v2, out result);
            result = (float) Math.Sqrt(result);
        }

        public static void Reflect(ref Vector3 v, ref Vector3 n, out Vector3 result)
        {
            float dot;
            Dot(ref v, ref n, out dot);
            result.X = v.X - 2*n.X*dot;
            result.Y = v.Y - 2*n.Y*dot;
            result.Z = v.Z - 2*n.Z*dot;
        }

        public static void Project(ref Vector3 v, ref Vector3 n, out Vector3 result)
        {
            float dotnn;
            Dot(ref n, ref n, out dotnn);
            if (dotnn < 0.000001f)
                result = Zero;
            float dotvn;
            Dot(ref v, ref n, out dotvn);
            dotvn /= dotnn;
            Multiply(ref n, ref dotvn, out result);
        }

        public static void Transform(ref Vector3 v, ref Quaternion q, out Vector3 result)
        {
            var x = 2*(q.Y*v.Z - q.Z*v.Y);
            var y = 2*(q.Z*v.X - q.X*v.Z);
            var z = 2*(q.X*v.Y - q.Y*v.X);

            result.X = v.X + x*q.W + q.Y*z - q.Z*y;
            result.Y = v.Y + y*q.W + q.Z*x - q.X*z;
            result.Z = v.Z + z*q.W + q.X*y - q.Y*x;
        }

        #endregion

        #region Operators

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                var aux = (Vector3) obj;
                return X == aux.X && Y == aux.Y && Z == aux.Z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int) (X + Y + Z);
        }

        public override string ToString()
        {
            return string.Format("X={0} Y={1} Z={2}", X, Y, Z);
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return Add(v1, v2);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return Subtract(v1, v2);
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return Multiply(v1, v2);
        }

        public static Vector3 operator /(Vector3 v1, Vector3 v2)
        {
            return Divide(v1, v2);
        }

        public static Vector3 operator *(Vector3 v, float scalar)
        {
            return Multiply(v, scalar);
        }

        public static Vector3 operator *(float scalar, Vector3 v)
        {
            return Multiply(v, scalar);
        }

        public static Vector3 operator /(Vector3 v, float scalar)
        {
            return Multiply(v, 1/scalar);
        }

        public static Vector3 operator /(float scalar, Vector3 v)
        {
            return Divide(new Vector3(scalar, scalar, scalar), v);
        }

        public static Vector3 operator +(Vector3 v)
        {
            return v;
        }

        public static Vector3 operator -(Vector3 v)
        {
            return Negate(v);
        }

        #endregion
    }
}