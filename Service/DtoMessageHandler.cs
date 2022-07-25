using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DevSim.Interfaces;
using DevSim.Enums;
using DevSim.Utilities;
using DevSim.Models;
using DevSim.Models.RemoteControlDtos;
using Newtonsoft.Json;

namespace DevSim.Services
{
    public interface IDtoMessageHandler
    {
        Task ParseMessage(byte[] message);
    }
    public class DtoMessageHandler : IDtoMessageHandler
    {
        public DtoMessageHandler(IKeyboardMouseInput keyboardMouseInput,
            IClipboardService clipboardService)
        {
            KeyboardMouseInput = keyboardMouseInput;
            ClipboardService = clipboardService;
        }

        private IClipboardService ClipboardService { get; }
        private IKeyboardMouseInput KeyboardMouseInput { get; }
        public async Task ParseMessage(byte[] message)
        {
            try
            {
                var baseDto = JsonConvert.DeserializeObject<BaseDto>(message.ToString());

                switch (baseDto.DtoType)
                {
                    case BaseDtoType.MouseMove:
                    case BaseDtoType.MouseDown:
                    case BaseDtoType.MouseUp:
                    case BaseDtoType.Tap:
                    case BaseDtoType.MouseWheel:
                    case BaseDtoType.KeyDown:
                    case BaseDtoType.KeyUp:
                    case BaseDtoType.CtrlAltDel:
                    case BaseDtoType.ToggleBlockInput:
                    case BaseDtoType.ClipboardTransfer:
                    case BaseDtoType.KeyPress:
                    case BaseDtoType.SetKeyStatesUp:
                        {
                            // if (!viewer.HasControl)
                            {
                                return;
                            }
                        }
                        break;
                    default:
                        break;
                }

                switch (baseDto.DtoType)
                {
                    case BaseDtoType.MouseMove:
                        MouseMove(message);
                        break;
                    case BaseDtoType.MouseDown:
                        MouseDown(message);
                        break;
                    case BaseDtoType.MouseUp:
                        MouseUp(message);
                        break;
                    case BaseDtoType.Tap:
                        Tap(message);
                        break;
                    case BaseDtoType.MouseWheel:
                        MouseWheel(message);
                        break;
                    case BaseDtoType.KeyDown:
                        KeyDown(message);
                        break;
                    case BaseDtoType.KeyUp:
                        KeyUp(message);
                        break;
                    case BaseDtoType.CtrlAltDel:
                        // await viewer.SendCtrlAltDel();
                        break;
                    case BaseDtoType.ToggleAudio:
                        ToggleAudio(message);
                        break;
                    case BaseDtoType.ToggleBlockInput:
                        ToggleBlockInput(message);
                        break;
                    case BaseDtoType.ClipboardTransfer:
                        await ClipboardTransfer(message);
                        break;
                    case BaseDtoType.KeyPress:
                        await KeyPress(message);
                        break;
                    case BaseDtoType.File:
                        await DownloadFile(message);
                        break;
                    case BaseDtoType.SetKeyStatesUp:
                        SetKeyStatesUp();
                        break;
                    case BaseDtoType.OpenFileTransferWindow:
                        // OpenFileTransferWindow(viewer);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private async Task ClipboardTransfer(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<ClipboardTransferDto>(message.ToString());
            if (dto.TypeText)
            {
                // TODO: 
                // KeyboardMouseInput.SendText(dto.Text);
            }
            else
            {
                await ClipboardService.SetText(dto.Text);
            }
        }

        private async Task DownloadFile(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<FileDto>(message.ToString());
            // await FileTransferService.ReceiveFile(dto.Buffer,
            //     dto.FileName,
            //     dto.MessageId,
            //     dto.EndOfFile,
            //     dto.StartOfFile);
        }



        private void KeyDown(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<KeyDownDto>(message.ToString());
            if (dto?.Key is null)
            {
                Logger.Write("Key input is empty.", EventType.Warning);
                return;
            }
            KeyboardMouseInput.SendKeyDown(dto.Key);
        }

        private async Task KeyPress(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<KeyPressDto>(message.ToString());

            if (dto?.Key is null)
            {
                Logger.Write("Key input is empty.", EventType.Warning);
                return;
            }

            KeyboardMouseInput.SendKeyDown(dto.Key);
            await Task.Delay(1);
            KeyboardMouseInput.SendKeyUp(dto.Key);
        }

        private void KeyUp(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<KeyUpDto>(message.ToString());
            if (dto?.Key is null)
            {
                Logger.Write("Key input is empty.", EventType.Warning);
                return;
            }
            KeyboardMouseInput.SendKeyUp(dto.Key);
        }

        private void MouseDown(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<MouseDownDto>(message.ToString());
            KeyboardMouseInput.SendMouseButtonAction(dto.Button, ButtonAction.Down, dto.PercentX, dto.PercentY);
        }

        private void MouseMove(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<MouseMoveDto>(message.ToString());
            KeyboardMouseInput.SendMouseMove(dto.PercentX, dto.PercentY);
        }

        private void MouseUp(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<MouseUpDto>(message.ToString());
            KeyboardMouseInput.SendMouseButtonAction(dto.Button, ButtonAction.Up, dto.PercentX, dto.PercentY);
        }

        private void MouseWheel(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<MouseWheelDto>(message.ToString());
            KeyboardMouseInput.SendMouseWheel(-(int)dto.DeltaY);
        }



        private void SelectScreen(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<SelectScreenDto>(message.ToString());
        }

        private void SetKeyStatesUp()
        {
            KeyboardMouseInput.SetKeyStatesUp();
        }

        private void Tap(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<TapDto>(message.ToString());
            KeyboardMouseInput.SendMouseButtonAction(0, ButtonAction.Down, dto.PercentX, dto.PercentY);
            KeyboardMouseInput.SendMouseButtonAction(0, ButtonAction.Up, dto.PercentX, dto.PercentY);
        }

        private void ToggleAudio(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<ToggleAudioDto>(message.ToString());
        }

        private void ToggleBlockInput(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<ToggleBlockInputDto>(message.ToString());
            KeyboardMouseInput.ToggleBlockInput(dto.ToggleOn);
        }

        private void ToggleWebRtcVideo(byte[] message)
        {
            var dto = JsonConvert.DeserializeObject<ToggleWebRtcVideoDto>(message.ToString());
        }
    }
}
