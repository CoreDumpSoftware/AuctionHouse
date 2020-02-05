using System;
using System.Threading.Tasks;
using System.Timers;

namespace AuctionHouse.Common.Models
{
    public class Timed<T> : IDisposable where T : class
    {
        private Timer timer;
        private Type typeOf;
        private T value;
        private bool deletesValue;

        public delegate void ExpirationEvent(Timed<T> timedObject);

        public T Value { get => Get(); private set => Set(value); }
        public DateTime ExpiresAt { get; private set; }
        public bool IsExpired { get; private set; }
        public event ExpirationEvent OnExpiration;

        public Timed(T value, DateTime expiresAt, bool deletesValue = true)
        {
            Value = value;
            ExpiresAt = expiresAt;
            typeOf = typeof(T);
            StartExpirationTimer();
        }

        public void Dispose()
        {
            lock (value)
            {
                if (deletesValue)
                {
                    if (typeOf is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    else if (typeOf is IAsyncDisposable asyncDisposable)
                    {
                        Task.Run(async () => await asyncDisposable.DisposeAsync()).Wait();
                    }

                    value = null;
                }


                timer.Dispose();
                IsExpired = true;

                OnExpiration?.Invoke(this);
            }
        }

        private T Get()
        {
            lock (value)
            {
                return IsExpired ? Value : default;
            }
        }

        private void Set(T value)
        {
            lock (value)
            {
                this.value = value;
            }
        }

        private void StartExpirationTimer()
        {
            var interval = (ExpiresAt - DateTime.Now).TotalMilliseconds;
            timer = new Timer(interval)
            {
                AutoReset = false
            };

            timer.Elapsed += (sender, args) =>
            {
                Dispose();
            };
            timer.Start();
        }
    }
}
