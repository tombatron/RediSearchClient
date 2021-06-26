using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RediSearchClient
{
    /// <summary>
    /// Collection abstraction to enable easier access to spell check results.
    /// </summary>
    public class SpellCheckResultCollection : IReadOnlyCollection<SpellCheckResult>
    {
        private readonly SpellCheckResult[] _inner;

        internal SpellCheckResultCollection(SpellCheckResult[] inner)
        {
            _inner = inner;
        }

        /// <summary>
        /// The count of spell check results.
        /// </summary>
        public int Count => _inner?.Length ?? 0;

        /// <summary>
        /// Enumerator linked to the underlying collection of spell check responses. 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SpellCheckResult> GetEnumerator() =>
            ((IEnumerable<SpellCheckResult>)_inner).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable<SpellCheckResult>)_inner).GetEnumerator();

        /// <summary>
        /// Access suggestion result by index.
        /// </summary>
        /// <value></value>
        public SpellCheckResult this[int index]
        {
            get
            {
                if (index > -1 && index < Count)
                {
                    return _inner[index];
                }

                return default;
            }
        }

        /// <summary>
        /// Access suggestion result by term.
        /// </summary>
        /// <value></value>
        public SpellCheckResult this[string term]
        {
            get
            {
                if (Count > 0 && _inner.Any(x => x.Term == term))
                {
                    return _inner.First(x => x.Term == term);
                }

                return default;
            }
        }
    }
}