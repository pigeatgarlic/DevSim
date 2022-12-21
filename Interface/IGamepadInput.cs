using DevSim.Enums;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DevSim.Interfaces
{
    public interface IGamepadInput
    {
        public bool Status();
        public void Connect();
        public void DisConnect();
        public void pressButton(int index, bool pressed);
        public void pressSlider(int index, float val);
        public void pressAxis(int index, float val);
        public Xbox360FeedbackReceivedEventArgs? getFeedback();
    }
}
