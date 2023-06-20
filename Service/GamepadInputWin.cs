using DevSim.Interfaces;
using DevSim.Enums;
using DevSim.Utilities;
using System;
using System.Collections.Concurrent;
using System.Threading;
using static DevSim.Win32.User32;
using DevSim.Win32;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DevSim.Services
{
    public class GamepadInput : IGamepadInput
    {
        public bool failed {get;}
        private readonly ViGEmClient vigem;
        private ConcurrentDictionary<string,IXbox360Controller> xboxs;
        private ConcurrentDictionary<string,GamepadFeedbackHandler> feedbacks;
        private string SingleID = "unique";

        public GamepadInput()
        {
            xboxs     = new ConcurrentDictionary<string, IXbox360Controller>();
            feedbacks = new ConcurrentDictionary<string, GamepadFeedbackHandler>();

            try {
                // initializes the SDK instance
                vigem = new ViGEmClient();

                var xbox = vigem.CreateXbox360Controller();
                xbox.AutoSubmitReport = true;
                xbox.FeedbackReceived += (object sender,Xbox360FeedbackReceivedEventArgs arg) => {
                    this.feedbacks.ToList().ForEach(x => {
                        x.Value(arg);
                    });
                };

                xbox.Connect();
                xboxs.TryAdd(this.SingleID,xbox);
                failed = false;
            } catch (Exception e){
                Console.WriteLine($"unable to setup gampad driver {e.StackTrace}");
                failed = true;
            }
        }


        public string Connect(GamepadFeedbackHandler handler)
        {
            if (this.failed) {
                return ""; 
            }

            var random = (new Random()).Next().ToString();
            this.feedbacks.TryAdd(random,handler);
            return random;
        }

        public void Disconnect(string id)
        {
            if (this.failed) {
                return; 
            }

            this.feedbacks.TryRemove(id,out var handler);
        }

        public async Task pressButton(int index, bool pressed)
        {
            if (this.failed) {
                return ; 
            }

            Xbox360Button button;
            switch (index)
            {
                case 0:
                    button = Xbox360Button.A;
                    break;
                case 1:
                    button = Xbox360Button.B;
                    break;
                case 2:
                    button = Xbox360Button.X;
                    break;
                case 3:
                    button = Xbox360Button.Y;
                    break;
                case 4:
                    button = Xbox360Button.LeftShoulder;
                    break;
                case 5:
                    button = Xbox360Button.RightShoulder;
                    break;

                case 8:
                    button = Xbox360Button.Back;
                    break;
                case 9:
                    button = Xbox360Button.Start;
                    break;
                case 10:
                    button = Xbox360Button.LeftThumb;
                    break;
                case 11:
                    button = Xbox360Button.RightThumb;
                    break;
                case 12:
                    button = Xbox360Button.Up;
                    break;
                case 13:
                    button = Xbox360Button.Down;
                    break;
                case 14:
                    button = Xbox360Button.Left;
                    break;
                case 15:
                    button = Xbox360Button.Right;
                    break;
                case 16:
                    button = Xbox360Button.Guide;
                    break;
                default:
                    Console.WriteLine($"unknown button {index}");
                    return;
            }

            xboxs.TryGetValue(this.SingleID,out var xbox);
            if(xbox != null) {
                xbox.SetButtonState(button,pressed);
            }
        }


        public async Task pressSlider(int index, float val)
        {
            if (this.failed) {
                return; 
            }

            Xbox360Slider slider;
            switch (index)
            {
                case 6:
                    slider = Xbox360Slider.LeftTrigger;
                    break;
                case 7:
                    slider = Xbox360Slider.RightTrigger;
                    break;
                default:
                    Console.WriteLine($"unknown slider {index}");
                    return;
            }
            
            xboxs.TryGetValue(this.SingleID,out var xbox);
            if(xbox != null) {
                xbox.SetSliderValue(slider,(byte)( val * Byte.MaxValue));
            }
        }

        public async Task pressAxis(int index, float val)
        {
            if (this.failed) {
                return; 
            }

            Xbox360Axis slider;
            switch (index)
            {
                case 0:
                    slider = Xbox360Axis.LeftThumbX;
                    break;
                case 1:
                    slider = Xbox360Axis.LeftThumbY;
                    val = -val;
                    break;
                case 2:
                    slider = Xbox360Axis.RightThumbX;
                    break;
                case 3:
                    slider = Xbox360Axis.RightThumbY;
                    val = -val;
                    break;
                default:
                    Console.WriteLine($"unknown axis {index}");
                    return;
            }

            xboxs.TryGetValue(this.SingleID,out var xbox);
            if(xbox != null) {
                xbox.SetAxisValue(slider,(short) (val * 32767));
            }
        }
    }
}