using DevSim.Enums;




namespace DevSim.Interfaces
{
    public interface IKeyboardMouseInput
    {
        void Init();
        void SendKeyDown(string key);
        void SendKeyUp(string key);
        void SendMouseMove(double percentX, double percentY);
        void SendMouseWheel(int deltaY);
        void ToggleBlockInput(bool toggleOn);
        void SetKeyStatesUp();
        void SendMouseButtonAction(int button, ButtonAction buttonAction, double percentX, double percentY);
    }
}
