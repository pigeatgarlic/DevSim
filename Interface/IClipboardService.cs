using System;
using System.Threading.Tasks;

namespace DevSim.Interfaces
{
    public interface IClipboardService
    {
        event EventHandler<string> ClipboardTextChanged;

        void BeginWatching();

        Task SetText(string clipboardText);
    }
}
