/**********************************************************/
/*************** The main adapter
/**********************************************************/

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
        public MainAdapter(V4App.FragmentManager fragmentManager, V4App.Fragment[] fragments, ICharSequence[] titles) : base(fragmentManager)
        {
            this.titles = titles;
            this.fragments = fragments;
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
            return titles[position];
        }
    }
}