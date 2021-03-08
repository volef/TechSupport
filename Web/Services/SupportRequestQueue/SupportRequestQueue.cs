using System;
using Microsoft.Extensions.Logging;
using Web.Data;

namespace Web.Services.SupportRequestQueue
{
    public class SupportRequestQueue : LockedQueue<SupportRequest>
    {
        private readonly ILogger<SupportRequestQueue> _logger;

        public SupportRequestQueue(ILogger<SupportRequestQueue> logger)
        {
            _logger = logger;
        }

        private bool _isInitialized;

        public bool IsInitialized
        {
            get => _isInitialized;
            set
            {
                _isInitialized = true;
                _logger.LogInformation("SupportRequestQueue initialized");
            }
        }


        public bool HasRequest()
        {
            return Count != 0;
        }

        public bool CanProcessRequest(int checkMs)
        {
            lock (_queue)
            {
                var checkSpan = TimeSpan.FromMilliseconds(checkMs);
                var request = _queue.Peek();
                var requestSpan = DateTime.Now - request.Created;
                return requestSpan > checkSpan;
            }
        }
    }
}