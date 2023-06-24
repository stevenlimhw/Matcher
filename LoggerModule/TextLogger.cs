using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;

namespace TradingEngineServer.Logging.LoggingConfiguration
{
    public class TextLogger : AbstractLogger, ITextLogger
    {
        private readonly LoggerConfiguration _loggingConfiguration;
        public TextLogger(IOptions<LoggerConfiguration> loggingConfiguration) : base()
        {
            _loggingConfiguration = loggingConfiguration.Value ?? throw new ArgumentNullException(nameof(loggingConfiguration));
            if (_loggingConfiguration.LoggerType != LoggerType.Text)
                throw new InvalidOperationException($"{nameof(TextLogger)} doesn't match LoggerType of {_loggingConfiguration.LoggerType}");
            
            var now = DateTime.Now;
            string logDirectory = Path.Combine(_loggingConfiguration.TextLoggerConfiguration.Directory, $"{now:yyyy-MM-dd}");
            string uniqueLogName = $"Matcher_{now:HHmmss}";
            string baseLogName = Path.ChangeExtension(uniqueLogName, _loggingConfiguration.TextLoggerConfiguration.FileExtension);
            string filepath = Path.Combine(logDirectory, baseLogName);
            Directory.CreateDirectory(logDirectory);

            _ = Task.Run(() => LogAsync(filepath, _logQueue, _tokenSource.Token));
        }

        private static async Task LogAsync(string filepath, BufferBlock<LogInformation> logQueue, CancellationToken token)
        {
            using var fs = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(fs) { AutoFlush = true, };
            try
            {
                while (true)
                {
                    var logItem = await logQueue.ReceiveAsync(token).ConfigureAwait(false);
                    string formattedMessage = FormatLogItem(logItem);
                    await sw.WriteAsync(formattedMessage).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        private static string FormatLogItem(LogInformation logItem)
        {
            return $"[{logItem.Now:yyyy-MM-dd HH-mm-ss.fffffff}] [{logItem.ThreadName,-30}{logItem.ThreadId:000}]" +
                $"[{logItem.LogLevel}] {logItem.Message}\n";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock(_lock)

            if (_disposed)
                return;
            _disposed = true;

            if (disposing)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }


        }

        protected override void Log(LogLevel loglevel, string module, string message)
        {
            _logQueue.Post(new LogInformation(loglevel, module, message, 
                DateTime.Now, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name));
        }

        ~TextLogger()
        {
            Dispose(false);
        }

        private readonly BufferBlock<LogInformation> _logQueue = new BufferBlock<LogInformation>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly object _lock = new Object();
        private bool _disposed = false;
    }
}