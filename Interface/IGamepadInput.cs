using DevSim.Enums;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DevSim.Interfaces
{
    public interface IGamepadInput
    {
        public IXbox360Controller Connect(int gamepad_id);
        public void DisConnect(int gamepad_id);
        public Task pressButton(int gamepad_id, int index, bool pressed);
        public Task pressSlider(int gamepad_id, int index, float val);
        public Task pressAxis(int gamepad_id, int index, float val);
        // public Xbox360FeedbackReceivedEventArgs? getFeedback(int gamepad_id);
    }
}
