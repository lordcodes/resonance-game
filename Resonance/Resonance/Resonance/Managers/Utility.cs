using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        /// <summary>
        /// Draws a line between two points on a SpriteBatch, as it can be a bit awkward.
        /// </summary>
        public static void drawLine(SpriteBatch spriteBatch, Texture2D tex, Vector2 p1, Vector2 p2, Color c, int thickness) {
            float length = (float) Math.Sqrt(Math.Pow((p1.X - p2.X), 2d) + Math.Pow((p1.Y - p2.Y), 2d));
            float ang    = (float) Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            spriteBatch.Draw(tex, p1, null, c, ang, new Vector2(0f, 0f), new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Exactly the same as above, only a line's thickness is added to the length so that the box corners will be flush.
        /// </summary>
        public static void drawBoxEdge(SpriteBatch spriteBatch, Texture2D tex, Vector2 p1, Vector2 p2, Color c, int thickness) {
            float length = (float) Math.Sqrt(Math.Pow((p1.X - p2.X), 2d) + Math.Pow((p1.Y - p2.Y), 2d));
            length += thickness;
            float ang    = (float) Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            spriteBatch.Draw(tex, p1, null, c, ang, new Vector2(0f, 0f), new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        /// <summary>
        ///  Draws a box on a SpriteBatch
        /// </summary>
        public static void drawBox(SpriteBatch spriteBatch, Texture2D tex, Vector2[] corners, Color c, int thickness) {
            Utility.drawBoxEdge(spriteBatch, tex, corners[0], corners[1], c, thickness);
            Utility.drawBoxEdge(spriteBatch, tex, corners[1], corners[2], c, thickness);
            Utility.drawBoxEdge(spriteBatch, tex, corners[2], corners[3], c, thickness);
            Utility.drawBoxEdge(spriteBatch, tex, corners[3], corners[0], c, thickness);
        }
    }
}
