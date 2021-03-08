using System.Collections.Generic;

namespace Web.Services.SupportRequestQueue
{
    public class LockedQueue<T>
    {
        protected Queue<T> _queue;

        public LockedQueue()
        {
            _queue = new Queue<T>();
        }

        public virtual T Peek()
        {
            lock (_queue)
            {
                return _queue.Peek();
            }
        }

        public virtual T Dequeue()
        {
            lock (_queue)
            {
                return _queue.Dequeue();
            }
        }

        public virtual void Enqueue(T request)
        {
            lock (_queue)
            {
                _queue.Enqueue(request);
            }
        }

        public int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }
    }
}