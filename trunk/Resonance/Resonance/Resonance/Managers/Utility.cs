using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class Utility
    {
        /// <summary>
        /// Get component of normalised vector that is parallel to another vector
        /// </summary>
        /// <param name="vector">the normalised vector</param>
        /// <param name="unitBasis">the "another" normalised vector</param>
        /// <returns>Vector3 parallel component</returns>
        public static Vector3 parallelComponent(Vector3 vector, Vector3 anotherVector)
        {
            float projection = Vector3.Dot(vector, anotherVector);
            return anotherVector * projection;
        }

        /// <summary>
        /// Get component of normalised vector that is perpendicular to another normalised vector
        /// </summary>
        /// <param name="vector">the normalised vector</param>
        /// <param name="unitBasis">the "another" normalised vector</param>
        /// <returns>Vector3 perpendicular component</returns>
        public static Vector3 perpendicularComponent(Vector3 vector, Vector3 anotherVector)
        {
            return (vector - parallelComponent(vector, anotherVector));
        }

        /// <summary>
        /// Converts a quaternion into Vector3 of Euler co-ordinates
        /// </summary>
        /// <param name="quat">the quaternion</param>
        /// <returns>the euler co-ordinates as Vector3</returns>
        public static Vector3 QuaternionToEuler(Quaternion quat)
        {
            float w = quat.W;
            float y = quat.Y;
            float x = quat.X;
            float z = quat.Z;

            Vector3 radAngles = new Vector3();
            radAngles.Y = (float)Math.Atan2(2 * (w * y + x * z), 1 - 2 * (Math.Pow(y, 2) + Math.Pow(x, 2)));
            radAngles.X = (float)Math.Asin(2 * (w * x - z * y));
            radAngles.Z = (float)Math.Atan2(2 * (w * z + y * x), 1 - 2 * (Math.Pow(x, 2) + Math.Pow(z, 2)));
            return radAngles;
        }

        /// <summary>
        /// Rotate a Vector2 by the angle theta
        /// </summary>
        /// <returns>The rotated Vector2</returns>
        public static Vector2 rotateVector2(Vector2 vec, float theta)
        {
            Vector2 result = new Vector2();

            result.X = (float)((vec.X * Math.Cos(theta)) - (vec.Y * Math.Sin(theta)));
            result.Y = (float)((vec.X * Math.Sin(theta)) + (vec.Y * Math.Cos(theta)));

            return result;
        }
    }
}
