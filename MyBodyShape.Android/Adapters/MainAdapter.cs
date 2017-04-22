/**********************************************************/
/*************** The main adapter
/**********************************************************/

using Android.Graphics.Drawables;
using AndroidText = Android.Text;
using Java.Lang;
using V4App = Android.Support.V4.App;

namespace MyBodyShape.Android.Adapters
{
    /// <summary>
    /// The main adapter.
    /// </summary>
    public class MainAdapter : V4App.FragmentPagerAdapter
    {
        /// <summary>
        /// The pages titles.
        /// </summary>
        public ICharSequence[] titles;

        /// <summary>
        /// The framents pages;
        /// </summary>
        public V4App.Fragment[] fragments { get; set; }

        /// <summary>
        /// The tabs icons.
        /// </summary>
        public Drawable FrontIcon { get; set; }
        public Drawable SideIcon { get; set; }
        public Drawable GenerationIcon { get; set; }
        public Drawable D3Icon { get; set; }
        public Drawable ResultsIcon { get; set; }

        /// <summary>
        /// The fragments count.
        /// </summary>
        public override int Count
        {
            get { return fragments.Length; }
        }

        /// <summary>
        /// The contructor.
        /// </summary>
        /// <param name="fragmentManager">The fragment manager</param>
        /// <param name="fragments">The fragments</param>
        public MainAdapter(V4App.FragmentManager fragmentManager, V4App.Fragment[] fragments, ICharSequence[] titles, Drawable frontIcon, Drawable sideIcon, Drawable generationIcon, Drawable d3Icon, Drawable resultsIcon) : base(fragmentManager)
        {
            this.titles = titles;
            this.fragments = fragments;
            this.FrontIcon = frontIcon;
            this.SideIcon = sideIcon;
            this.GenerationIcon = generationIcon;
            this.D3Icon = d3Icon;
            this.ResultsIcon = resultsIcon;
        }

        /// <summary>
        /// The get item method.
        /// </summary>
        /// <param name="position">The item position.</param>
        /// <returns>The desired fragment.</returns>
        public override V4App.Fragment GetItem(int position)
        {
            return fragments[position];
        }

        /// <summary>
        /// The get tab text at the give position.
        /// </summary>
        /// <param name="position">The item position.</param>
        /// <returns>The desired tab text.</returns>
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            Drawable myDrawable;
            switch (position)
            {
                case 0:
                    myDrawable = FrontIcon;
                    break;

                case 1:
                    myDrawable = SideIcon;
                    break;

                case 2:
                    myDrawable = GenerationIcon;
                    break;

                case 3:
                    myDrawable = ResultsIcon;
                    break;

                case 4:
                    myDrawable = D3Icon;
                    break;

                default:
                    myDrawable = ResultsIcon;
                    break;
            }

            var sb = new AndroidText.SpannableStringBuilder("  " + titles[position]);
            myDrawable.SetBounds(0, 0, myDrawable.IntrinsicWidth, myDrawable.IntrinsicHeight);
            var span = new AndroidText.Style.ImageSpan(myDrawable, AndroidText.Style.SpanAlign.Baseline);
            sb.SetSpan(span, 0, 1, AndroidText.SpanTypes.ExclusiveExclusive);
            return sb;
        }
    }
}