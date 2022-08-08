using DevSim.Enums;




namespace DevSim.Interfaces
{
    public interface IKeyboardMouseInput
    {
        void SendKeyDown(string key);
        void SendKeyUp(string key);
        void SendMouseMove(float percentX, float percentY);
        void SendMouseWheel(int deltaY);
        void SetKeyStatesUp();
        void SendMouseButtonAction(ButtonCode button, ButtonAction buttonAction);
    }
}
