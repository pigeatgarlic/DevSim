using DevSim.Enums;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DevSim.Interfaces
{
    public delegate void GamepadFeedbackHandler(Xbox360FeedbackReceivedEventArgs e);
    public interface IGamepadInput
    {
        public bool failed {get;}
        public string Connect(GamepadFeedbackHandler rumble);
        public void Disconnect(string gamepad);

        
        public Task pressButton(int index, bool pressed);
        public Task pressSlider(int index, float val);
        public Task pressAxis(int index, float val);
    }
}
