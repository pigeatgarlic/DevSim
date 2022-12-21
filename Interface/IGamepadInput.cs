using DevSim.Enums;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DevSim.Interfaces
{
    public interface IGamepadInput
    {
        public bool Status();
        public void Connect();
        public void DisConnect();
        public Task pressButton(int index, bool pressed);
        public Task pressSlider(int index, float val);
        public Task pressAxis(int index, float val);
        public Xbox360FeedbackReceivedEventArgs? getFeedback();
    }
}
