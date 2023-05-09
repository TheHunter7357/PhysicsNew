using System;
using System.Runtime.CompilerServices;

namespace GXPEngine.Core
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }


        //adding vectors
        public static Vector2 operator +(Vector2 vec1, Vector2 vec2) => new Vector2(vec1.x + vec2.x, vec1.y + vec2.y);
        //subtracting vectors
        public static Vector2 operator -(Vector2 vec1, Vector2 vec2) => new Vector2(vec1.x - vec2.x, vec1.y - vec2.y);
        //multiplying vectors
        public static Vector2 operator -(Vector2 vec1) => new Vector2(-vec1.x, -vec1.y);
        //multiplying vectors
        public static Vector2 operator *(Vector2 vec1, Vector2 vec2) => new Vector2(vec1.x * vec2.x, vec1.y * vec2.y);
        //multiplying vector by a value
        public static Vector2 operator *(Vector2 vec1, float val) => new Vector2(vec1.x * val, vec1.y * val);
        public static Vector2 operator *(float val, Vector2 vec1) => new Vector2(vec1.x * val, vec1.y * val);

        //dividing a vector by a value
        public static Vector2 operator /(Vector2 vec1, float val)
        {
            if (val == 0)
            {
                throw new DivideByZeroException();
            }
            return new Vector2(vec1.x / val, vec1.y / val);
        }

        public Vector2 Normalize()
        {
            if (x != 0 || y != 0)
            {
                float fac = Mathf.Sqrt(x * x + y * y);
                x /= fac;
                y /= fac;
            }
            return this;
        }

        public Vector2 Normalized()
        {
            if (x != 0 || y != 0)
            {
                float fac = Mathf.Sqrt(x * x + y * y);
                float tempX = x / fac;
                float tempY = y / fac;
                return new Vector2(tempX, tempY);
            }
            else
            {
                return this;
            }
        }

        public float Length()
        {
            return Mathf.Sqrt(x * x + y * y);
        }

        public void SetXY(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float DistanceTo(Vector2 point)
        {
            return Mathf.Sqrt((point.x - x) * (point.x - x) + (point.y - y) * (point.y - y));
        }

        public float DistanceTo(float pointX, float pointY)
        {
            return Mathf.Sqrt((pointX - x) * (pointX - x) + (pointY - y) * (pointY - y));
        }

        //ROTATION CALCULATIONS
        public static float Deg2Rad(float degrees)
        {
            return (degrees / 180f) * Mathf.PI;
        }

        public static float Rad2Deg(float radians)
        {
            return (radians / Mathf.PI) * 180f;
        }

        public static Vector2 GetUnitVectorDeg(float degrees)
        {
            return new Vector2(Mathf.Cos(Deg2Rad(degrees)), Mathf.Sin(Deg2Rad(degrees)));
        }

        public static Vector2 GetUnitVectorRad(float Radians)
        {
            return new Vector2(Mathf.Cos(Radians), Mathf.Sin(Radians));
        }

        public static Vector2 RandomUnitVector()
        {
            float randomRad = Utils.Random(0, 2 * Mathf.PI);
            return new Vector2(Mathf.Cos(randomRad), Mathf.Sin(randomRad));
        }


        public Vector2 SetAngleDegrees(float degrees)
        {
            this = GetUnitVectorDeg(degrees) * Length();
            return this;
        }

        public Vector2 SetAngleRadians(float radians)
        {
            this = GetUnitVectorRad(radians) * Length();
            return this;
        }


        public float GetAngleDegrees()
        {
            return Rad2Deg(Mathf.Atan2(y, x));
        }

        public float GetAngleRadians()
        {
            return Mathf.Atan2(y, x);
        }

        public Vector2 RotateDegrees(float degrees)
        {
            return SetAngleDegrees(GetAngleDegrees() + degrees);
        }

        public Vector2 RotateRadians(float radians)
        {
            return SetAngleRadians(GetAngleRadians() + radians);
        }

        public Vector2 RotateAroundDegrees(Vector2 point, float degrees)
        {
            this -= point;
            SetAngleDegrees(GetAngleDegrees() + degrees);
            return this += point;
        }

        public Vector2 RotateAroundRadians(Vector2 point, float radians)
        {
            this -= point;
            SetAngleRadians(GetAngleRadians() + radians);
            return this += point;
        }

        public float AngleToDegrees(Vector2 point)
        {
            return Rad2Deg(Mathf.Atan2(y - point.y, x - point.x));
        }

        public float AngleToRadians(Vector2 point)
        {
            return Mathf.Atan2(y - point.y, x - point.x);
        }

        public Vector2 Normal()
        {
            return new Vector2(-y, x);
        }

        public Vector2 UnitNormal()
        {
            return new Vector2(-y, x).Normalized();
        }

        public static Vector2 GetNormal(Vector2 vec)
        {
            return new Vector2(-vec.y, vec.x);
        }

        public float Dot(Vector2 point)
        {
            return x * point.x + y * point.y;
        }


        override public string ToString()
        {
            return "[" + x + ", " + y + "]";
        }
    }
}

