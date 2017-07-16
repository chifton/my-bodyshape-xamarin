/**********************************************************/
/*************** The circle area
/**********************************************************/

using Android.Graphics;

namespace MyBodyShape.Android.Helpers
{
    /// <summary>
    /// The circle area class
    /// </summary>
    public class CircleArea
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="identifier">The circle identifier.</param>
        /// <param name="posX">The x position.</param>
        /// <param name="posY">The y position.</param>
        public CircleArea(string identifier, float posX, float posY, Color color)
        {
            this.PositionX = posX;
            this.PositionY = posY;
            this.Id = identifier;
            this.Color = color;
        }
        
        /// <summary>
        /// The circle area identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The circle color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The x position on the bitmap.
        /// </summary>
        public float PositionX { get; set; }

        /// <summary>
        /// The y position on the bitmap.
        /// </summary>
        public float PositionY { get; set; }

        /// <summary>
        /// The update position method.
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        public void UpdatePosition(float x, float y)
        {
            this.PositionX = x;
            this.PositionY = y;
        }
    }
}