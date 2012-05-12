
namespace Resonance
{
    class GraphicsSettings
    {
        /// Max texture size for XBOX is 4096.

        /// <summary>
        /// Reflections appear on the floor if true
        /// </summary>
#if XBOX
        public static bool FLOOR_REFLECTIONS = false;
#else
        public static bool FLOOR_REFLECTIONS = true;
#endif

        /// <summary>
        /// Shadows appear on the floor if true
        /// </summary>
#if XBOX
        public static bool FLOOR_SHADOWS = false;
#else
        public static bool FLOOR_SHADOWS = true;
#endif

        /// <summary>
        /// Resolution of the reflection texture of the floor reflections.
        /// </summary>
        public static int REFLECTION_TEXTURE_SIZE = 2048;

        /// <summary>
        /// Resolution of the shadows texture of the shadows.
        /// </summary>
        public static int SHADOWS_TEXTURE_SIZE = 2048;

        /// <summary>
        /// Distance from the camera to the clip plane.
        /// </summary>
        public static float DRAW_DISTANCE = 300.0f;

        /// <summary>
        /// Distance from the camera to the start of the fog.
        /// </summary>
        public static float FOG_START = 70.0f;

        /// <summary>
        /// Distance from the camera to the end of the fog.
        /// </summary>
        public static float FOG_END = 150.0f;
    }
}
