/**********************************************************/
/*************** The path area
/**********************************************************/

using Android.Graphics;

namespace MyBodyShape.Android.Helpers
{
    /// <summary>
    /// The path area class
    /// </summary>
    public class PathArea
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        public PathArea(string identifier, Color color, string[] points)
        {
            this.Points = points;
            this.Id = identifier;
            this.Color = color;
        }

        /// <summary>
        /// The path area identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The circle color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The points along the path.
        /// </summary>
        public string[] Points { get; set; }
    }
}