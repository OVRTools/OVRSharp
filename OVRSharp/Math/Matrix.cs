using Valve.VR;
using System.Numerics;

namespace OVRSharp.Math
{
    public static class MatrixExtension
    {
        /// <summary>
        /// Converts a <see cref="Matrix4x4"/> to a <see cref="HmdMatrix34_t"/>.
        /// <br/>
        /// <br/>
        /// From: <br/>
        /// 11 12 13 14 <br/>
        /// 21 22 23 24 <br/>
        /// 31 32 33 34 <br/>
        /// 41 42 43 44
        /// <br/><br/>
        /// To: <br/>
        /// 11 12 13 41 <br/>
        /// 21 22 23 42 <br/>
        /// 31 32 33 43
        /// </summary>
        public static HmdMatrix34_t ToHmdMatrix34_t(this Matrix4x4 matrix)
        {
            return new HmdMatrix34_t()
            {
                m0 = matrix.M11,
                m1 = matrix.M12,
                m2 = matrix.M13,
                m3 = matrix.M41,
                m4 = matrix.M21,
                m5 = matrix.M22,
                m6 = matrix.M23,
                m7 = matrix.M42,
                m8 = matrix.M31,
                m9 = matrix.M32,
                m10 = matrix.M33,
                m11 = matrix.M43,
            };
        }

        /// <summary>
        /// Converts a <see cref="HmdMatrix34_t"/> to a <see cref="Matrix4x4"/>.
        /// <br/>
        /// <br/>
        /// From: <br/>
        /// 11 12 13 14 <br/>
        /// 21 22 23 24 <br/>
        /// 31 32 33 34
        /// <br/><br/>
        /// To: <br/>
        /// 11 12 13 XX <br/>
        /// 21 22 23 XX <br/>
        /// 31 32 33 XX <br/>
        /// 14 24 34 XX
        /// </summary>
        public static Matrix4x4 ToMatrix4x4(this HmdMatrix34_t matrix)
        {
            return new Matrix4x4(
                matrix.m0, matrix.m1, matrix.m2, 0,
                matrix.m4, matrix.m5, matrix.m6, 0,
                matrix.m8, matrix.m9, matrix.m10, 0,
                matrix.m3, matrix.m7, matrix.m11, 1
            );
        }
    }
}
