using DevSim.Interfaces;
using DevSim.Utilities;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace DevSim.Service
{
    public class ClipboardServiceWin : IClipboardService
    {
        private CancellationTokenSource _cancelTokenSource;

        public event EventHandler<string> ClipboardTextChanged;

        private string ClipboardText { get; set; }

        public void BeginWatching()
        {


            StopWatching();

            _cancelTokenSource = new CancellationTokenSource();


            WatchClipboard(_cancelTokenSource.Token);
        }

        public Task SetText(string clipboardText)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    // TODO clipboard
                    if (string.IsNullOrWhiteSpace(clipboardText))
                    {
                        // Clipboard.Clear();
                    }
                    else
                    {
                        // Clipboard.SetText(clipboardText);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            return Task.CompletedTask;
        }

        public void StopWatching()
        {
            try
            {
                _cancelTokenSource?.Cancel();
                _cancelTokenSource?.Dispose();
            }
            catch { }
        }


        private void WatchClipboard(CancellationToken cancelToken)
        {
            var thread = new Thread(() =>
            {

                while (!cancelToken.IsCancellationRequested)
                {

                    try
                    {
                        // TODO clipboard
                        // Win32Interop.SwitchToInputDesktop();
                        // if (Clipboard.ContainsText() && Clipboard.GetText() != ClipboardText)
                        // {
                        //     ClipboardText = Clipboard.GetText();
                        //     ClipboardTextChanged?.Invoke(this, ClipboardText);
                        // }
                    }
                    catch { }
                    Thread.Sleep(500);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
