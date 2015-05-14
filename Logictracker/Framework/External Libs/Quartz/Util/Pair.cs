namespace Quartz.Util
{
    /// <summary>
    /// Utility class for storing two pieces of information together.
    /// </summary>
    /// <author><a href="mailto:jeff@binaryfeed.org">Jeffrey Wescott</a></author>
    public class Pair
    {
        private object first;
        private object second;

        /// <summary> 
        /// Get or sets the first object in the pair.
        /// </summary>
        public virtual object First
        {
            get { return first; }
            set { first = value; }
        }

        /// <summary> 
        /// Get or sets the second object in the pair.
        /// </summary>
        public virtual object Second
        {
            get { return second; }
            set { second = value; }
        }

        /// <summary>
        /// Test equality of this object with that.
        /// </summary>
        /// <param name="that">object to compare </param>
        /// <returns> true if objects are equal, false otherwise</returns>
        public override bool Equals(object that)
        {
            if (this == that)
            {
                return true;
            }
            if (that is Pair)
            {
                var other = (Pair) that;
                if (first == null && second == null)
                {
                    return (other.first == null && other.second == null);
                }
                else if (first == null)
                {
                    return second.Equals(other.second);
                }
                else if (second == null)
                {
                    return first.Equals(other.first);
                }
                else
                {
                    return (first.Equals(other.first) && second.Equals(other.second));
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return (17*first.GetHashCode()) + second.GetHashCode();
        }
    }
}