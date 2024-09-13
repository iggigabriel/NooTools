using System.Threading;

namespace Noo.Tools
{
    /// <summary>Simple generic double buffer implementation class</summary>
    public class DoubleBuffer<T> where T : class
    {
        private T current;
        private T next;

        public DoubleBuffer(T current, T next)
        {
            this.current = current;
            this.next = next;
        }

        public T Current => current;
        public T Next => next;

        /// <summary>Swaps between the buffers</summary>
        /// <returns>Returns the buffer previously used before the swap</returns>
        public T Swap()
        {
            var swappedBuffer = current;
            Interlocked.Exchange(ref current, next);
            next = swappedBuffer;
            return swappedBuffer;
        }
    }
}
