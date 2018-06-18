using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrandTheftMultiplayer.Launcher.Helpers
{
    public class TaskQueue
    {
        private readonly SemaphoreSlim _semaphore;
	    private readonly int _queueSize;
	    private readonly bool _cancelIfOverflow;

        private bool _clearingQueue;

        public bool IsRunning => _semaphore.CurrentCount == 0;

        public TaskQueue(int queueSize = 1, bool cancelIfOverflow = true)
        {
            _semaphore = new SemaphoreSlim(queueSize);
			_queueSize = queueSize;
	        _cancelIfOverflow = cancelIfOverflow;
        }

        public async Task ClearQueue()
        {
            _clearingQueue = true;

            while (_semaphore.CurrentCount > 1)
            {
                await _semaphore.WaitAsync();
                _semaphore.Release();
            }

            _clearingQueue = false;
        }

        private bool ShouldCancel => _clearingQueue || !_cancelIfOverflow && _semaphore.CurrentCount >= _queueSize;

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator, CancellationTokenSource cancellationTokenSource = null)
        {
            await _semaphore.WaitAsync();

            if (ShouldCancel) return default(T);

            try
            {
                return await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Enqueue(Func<Task> taskGenerator, CancellationTokenSource cancellationTokenSource = null)
        {
            await _semaphore.WaitAsync();

            if (ShouldCancel) return;

            try
            {
                await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
