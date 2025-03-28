﻿using GHelper.Gpu.AMD;
using GHelper.Input;
using GHelper.USB;
using HidSharp;
using System.Text;

namespace GHelper.Ally
{

    public enum ControllerMode : int
    {
        Auto = 0,
        Gamepad = 1,
        WASD = 2,
        Mouse = 3,
        Skip = -1,
    }

    public enum BindingZone : byte
    {
        DPadUpDown = 1,
        DPadLeftRight = 2,
        StickClick = 3,
        Bumper = 4,
        AB = 5,
        XY = 6,
        ViewMenu = 7,
        M1M2 = 8,
        Trigger = 9
    }

    public class AllyControl
    {
        System.Timers.Timer timer = default!;
        static AmdGpuControl amdControl = new AmdGpuControl();

        SettingsForm settings;

        static ControllerMode _mode = ControllerMode.Auto;
        static ControllerMode _applyMode = ControllerMode.Mouse;
        static int _autoCount = 0;

        static int fpsLimit = -1;

        public const string BindA = "01-01";
        public const string BindB = "01-02";
        public const string BindX = "01-03";
        public const string BindY = "01-04";
        public const string BindLB = "01-05";
        public const string BindRB = "01-06";
        public const string BindLS = "01-07";
        public const string BindRS = "01-08";
        public const string BindDU = "01-09";
        public const string BindDD = "01-0A";
        public const string BindDL = "01-0B";
        public const string BindDR = "01-0C";
        public const string BindVB = "01-11";
        public const string BindMB = "01-12";
        public const string BindM1 = "02-8F";
        public const string BindM2 = "02-8E";
        public const string BindLT = "01-0D";
        public const string BindRT = "01-0E";
        public const string BindXB = "01-13";

        public const string BindMouseL = "03-01";
        public const string BindMouseR = "03-02";

        public const string BindKBU = "02-98";
        public const string BindKBD = "02-99";
        public const string BindKBL = "02-9A";
        public const string BindKBR = "02-9B";

        public const string BindTab = "02-0D";
        public const string BindEnter = "02-5A";
        public const string BindBack = "02-66";
        public const string BindEsc = "02-76";

        public const string BindPgU = "02-96";
        public const string BindPgD = "02-97";

        public const string BindShift = "02-88";
        public const string BindCtrl = "02-8C";
        public const string BindAlt = "02-8A";
        public const string BindWin = "02-82";

        public const string BindTaskManager = "04-03-8C-88-76";
        public const string BindCloseWindow = "04-02-8A-0C";

        public const string BindBrightnessDown = "04-04-8C-88-8A-05";
        public const string BindBrightnessUp = "04-04-8C-88-8A-06";
        public const string BindXGM = "04-04-8C-88-8A-04";

        public const string BindOverlay = "04-03-8C-88-44";

        public const string BindShiftTab = "04-02-88-0D";
        public const string BindAltTab = "04-02-8A-0D";

        public const string BindVolUp = "05-03";
        public const string BindVolDown = "05-02";

        public const string BindPrintScrn = "02-C3";

        public const string BindScreenshot = "04-03-82-88-1B";
        public const string BindShowDesktop = "04-02-82-23";

        public const string BindShowKeyboard = "05-19";

        static byte[] CommandReady = new byte[] { AsusHid.INPUT_ID, 0xd1, 0x0a, 0x01 };
        static byte[] CommandSave = new byte[] { AsusHid.INPUT_ID, 0xd1, 0x0f, 0x20 };

        public static Dictionary<string, string> BindCodes = new Dictionary<string, string>
        {
            { "", "--------" },
            { "00-00", "[ Disabled ]" },

            { BindM1, "M1" },
            { BindM2, "M2" },

            { BindA, "A" },
            { BindB, "B" },

            { BindX, "X" },
            { BindY, "Y" },

            { BindLB, "Left Bumper" },
            { BindRB, "Right Bumper" },

            { BindLS, "Left Stick Click" },
            { BindRS, "Right Stick Click" },

            { BindDU, "DPad Up" },
            { BindDD, "DPad Down" },

            { BindDL, "DPad Left" },
            { BindDR, "DPad Right" },

            { BindVB, "View Button" },
            { BindMB, "Menu Button" },

            { BindXB, "XBox/Steam" },

            { BindVolUp, "Vol Up" },
            { BindVolDown, "Vol Down" },
            { BindBrightnessUp, "Bright Up" },
            { BindBrightnessDown, "Bright Down" },

            { BindShowKeyboard, "Show Keyboard" },
            { BindShowDesktop, "Show Desktop" },
            { BindScreenshot, "Screenshot" },

            { BindOverlay, "AMD Overlay" },
            { BindTaskManager, "Task Manager" },
            { BindCloseWindow, "Close Window" },
            { BindShiftTab, "Shift-Tab" },
            { BindAltTab, "Alt-Tab" },
            { BindXGM, "XGM Toggle" },


            { BindEsc, "Esc" },
            { BindBack, "Backspace" },
            { BindTab, "Tab" },
            { BindEnter, "Enter" },
            { BindShift, "LShift" },
            { BindAlt, "LAlt" },
            { BindCtrl, "LCtl" },
            { BindWin, "WIN" },
            { BindPrintScrn, "PrntScn" },

            { BindPgU, "PgUp" },
            { BindPgD, "PgDwn" },
            { BindKBU, "UpArrow" },
            { BindKBD, "DownArrow" },
            { BindKBL, "LeftArrow" },
            { BindKBR, "RightArrow" },

            { "02-05", "F1" },
            { "02-06", "F2" },
            { "02-04", "F3" },
            { "02-0C", "F4" },
            { "02-03", "F5" },
            { "02-0B", "F6" },
            { "02-80", "F7" },
            { "02-0A", "F8" },
            { "02-01", "F9" },
            { "02-09", "F10" },
            { "02-78", "F11" },
            { "02-07", "F12" },
            { "02-0E", "`" },
            { "02-16", "1" },
            { "02-1E", "2" },
            { "02-26", "3" },
            { "02-25", "4" },
            { "02-2E", "5" },
            { "02-36", "6" },
            { "02-3D", "7" },
            { "02-3E", "8" },
            { "02-46", "9" },
            { "02-45", "0" },
            { "02-4E", "-" },
            { "02-55", "=" },
            { "02-15", "Q" },
            { "02-1D", "W" },
            { "02-24", "E" },
            { "02-2D", "R" },
            { "02-2C", "T" },
            { "02-35", "Y" },
            { "02-3C", "U" },
            { "02-44", "O" },
            { "02-4D", "P" },
            { "02-54", "[" },
            { "02-5B", "]" },
            { "02-5D", "|" },
            { "02-58", "Caps" },
            { "02-1C", "A" },
            { "02-1B", "S" },
            { "02-23", "D" },
            { "02-2B", "F" },
            { "02-34", "G" },
            { "02-33", "H" },
            { "02-3B", "J" },
            { "02-42", "k" },
            { "02-4B", "l" },
            { "02-4C", ";" },
            { "02-52", "'" },
            { "02-22", "X" },
            { "02-1A", "Z" },
            { "02-21", "C" },
            { "02-2A", "V" },
            { "02-32", "B" },
            { "02-31", "N" },
            { "02-3A", "M" },
            { "02-41", "," },
            { "02-49", "." },
            { "02-89", "RShift" },
            { "02-29", "Space" },
            { "02-8B", "RAlt" },
            { "02-84", "App menu" },
            { "02-8D", "RCtl" },
            { "02-7E", "ScrLk" },
            { "02-C2", "Insert" },
            { "02-C0", "Delete" },
            { "02-94", "Home" },
            { "02-95", "End" },
            { "02-77", "NumLock" },
            { "02-90", "NumSlash" },
            { "02-7C", "NumStar" },
            { "02-7B", "NumHyphen" },
            { "02-70", "Num0" },
            { "02-69", "Num1" },
            { "02-72", "Num2" },
            { "02-7A", "Num3" },
            { "02-6B", "Num4" },
            { "02-73", "Num5" },
            { "02-74", "Num6" },
            { "02-6C", "Num7" },
            { "02-75", "Num8" },
            { "02-7D", "Num9" },
            { "02-79", "NumPlus" },
            { "02-81", "NumEnter" },
            { "02-71", "NumPeriod" },

            { BindMouseL, "Mouse left click" },
            { BindMouseR, "Mouse right click" },
            { "03-03", "Mouse middle click" },
            { "03-04", "Mouse scroll up" },
            { "03-05", "Mouse scroll down" },

            //{ "05-16", "Screenshot" },
            //{ "05-1C", "Show desktop" },

            { "05-1E", "Begin recording" },
            { "05-01", "Mic off" },

        };

        public AllyControl(SettingsForm settingsForm)
        {
            if (!AppConfig.IsAlly()) return;

            settings = settingsForm;

            timer = new System.Timers.Timer(300);
            timer.Elapsed += Timer_Elapsed;

        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            float fps = amdControl.GetFPS();

            ControllerMode newMode = (fps > 0) ? ControllerMode.Gamepad : ControllerMode.Mouse;

            if (_applyMode != newMode) _autoCount++;
            else _autoCount = 0;

            if (_mode != ControllerMode.Auto) return;

            if (_autoCount > 2)
            {
                _autoCount = 0;
                ApplyMode(newMode);
                Logger.WriteLine(fps.ToString());
            }

        }

        public void Init()
        {
            if (AppConfig.IsAlly()) settings.VisualiseAlly(true);
            else return;

            SetMode((ControllerMode)AppConfig.Get("controller_mode", (int)ControllerMode.Auto));

            settings.VisualiseBacklight(InputDispatcher.GetBacklight());
            settings.VisualiseFPSLimit(amdControl.GetFPSLimit());

        }

        public void ToggleFPSLimit()
        {
            switch (fpsLimit)
            {
                case 30:
                    fpsLimit = 45;
                    break;
                case 45:
                    fpsLimit = 60;
                    break;
                case 60:
                    fpsLimit = 90;
                    break;
                case 90:
                    fpsLimit = 120;
                    break;
                case 120:
                    fpsLimit = 240;
                    break;
                default:
                    fpsLimit = 30;
                    break;
            }

            int result = amdControl.SetFPSLimit(fpsLimit);
            Logger.WriteLine($"FPS Limit {fpsLimit}: {result}");

            settings.VisualiseFPSLimit(fpsLimit);

        }


        public void ToggleBacklight()
        {
            InputDispatcher.SetBacklight(4, true);
            settings.VisualiseBacklight(InputDispatcher.GetBacklight());
        }

        static private byte[] DecodeBinding(string binding = "")
        {
            byte[] bytes;

            if (binding == "" || binding is null) return new byte[2];

            try
            {
                bytes = AppConfig.StringToBytes(binding);
            }
            catch
            {
                return new byte[2];
            }

            byte[] code = new byte[10];
            code[0] = bytes[0];

            switch (bytes[0])
            {
                case 0x02:
                    code[2] = bytes[1];
                    break;
                case 0x03:
                    code[4] = bytes[1];
                    break;
                case 0x04:
                    bytes.Skip(1).ToArray().CopyTo(code, 5);
                    break;
                case 0x05:
                    code[3] = bytes[1];
                    break;
                default:
                    code[1] = bytes[1];
                    break;
            }

            return code;
        }

        static private void BindZone(BindingZone zone)
        {
            string KeyL1, KeyR1;
            string KeyL2, KeyR2;

            bool desktop = (_applyMode == ControllerMode.Mouse);

            switch (zone)
            {
                case BindingZone.DPadUpDown:
                    KeyL1 = AppConfig.GetString("bind_du", desktop ? BindKBU : BindDU);
                    KeyR1 = AppConfig.GetString("bind_dd", desktop ? BindKBD : BindDD);
                    KeyL2 = AppConfig.GetString("bind2_du", BindShowKeyboard);
                    KeyR2 = AppConfig.GetString("bind2_dd", BindShowDesktop);
                    break;
                case BindingZone.DPadLeftRight:
                    KeyL1 = AppConfig.GetString("bind_dl", desktop ? BindKBL : BindDL);
                    KeyR1 = AppConfig.GetString("bind_dr", desktop ? BindKBR : BindDR);
                    KeyL2 = AppConfig.GetString("bind2_dl", BindBrightnessDown);
                    KeyR2 = AppConfig.GetString("bind2_dr", BindBrightnessUp);
                    break;
                case BindingZone.StickClick:
                    KeyL1 = AppConfig.GetString("bind_ls", desktop ? BindShift : BindLS);
                    KeyR1 = AppConfig.GetString("bind_rs", desktop ? BindMouseL : BindRS);
                    KeyL2 = AppConfig.GetString("bind2_ls");
                    KeyR2 = AppConfig.GetString("bind2_rs");
                    break;
                case BindingZone.Bumper:
                    KeyL1 = AppConfig.GetString("bind_lb", desktop ? BindTab : BindLB);
                    KeyR1 = AppConfig.GetString("bind_rb", desktop ? BindMouseL : BindRB);
                    KeyL2 = AppConfig.GetString("bind2_lb");
                    KeyR2 = AppConfig.GetString("bind2_rb");
                    break;
                case BindingZone.AB:
                    KeyL1 = AppConfig.GetString("bind_a", desktop ? BindEnter : BindA);
                    KeyR1 = AppConfig.GetString("bind_b", desktop ? BindEsc : BindB);
                    KeyL2 = AppConfig.GetString("bind2_a");
                    KeyR2 = AppConfig.GetString("bind2_b");
                    break;
                case BindingZone.XY:
                    KeyL1 = AppConfig.GetString("bind_x", desktop ? BindPgD : BindX);
                    KeyR1 = AppConfig.GetString("bind_y", desktop ? BindPgU : BindY);
                    KeyL2 = AppConfig.GetString("bind2_x", BindScreenshot);
                    KeyR2 = AppConfig.GetString("bind2_y", BindOverlay);
                    break;
                case BindingZone.ViewMenu:
                    KeyL1 = AppConfig.GetString("bind_vb", BindVB);
                    KeyR1 = AppConfig.GetString("bind_mb", BindMB);
                    KeyL2 = AppConfig.GetString("bind2_vb");
                    KeyR2 = AppConfig.GetString("bind2_mb");
                    break;
                case BindingZone.M1M2:
                    KeyL1 = AppConfig.GetString("bind_m2", BindM2);
                    KeyR1 = AppConfig.GetString("bind_m1", BindM1);
                    KeyL2 = AppConfig.GetString("bind2_m2", BindM2);
                    KeyR2 = AppConfig.GetString("bind2_m1", BindM1);
                    break;
                default:
                    KeyL1 = AppConfig.GetString("bind_trl", desktop ? BindShiftTab : BindLT);
                    KeyR1 = AppConfig.GetString("bind_trr", desktop ? BindMouseR : BindRT);
                    KeyL2 = AppConfig.GetString("bind2_trl");
                    KeyR2 = AppConfig.GetString("bind2_trr");
                    break;
            }

            if (KeyL1 == "" && KeyR1 == "") return;

            byte[] bindings = new byte[50];
            byte[] init = new byte[] { AsusHid.INPUT_ID, 0xd1, 0x02, (byte)zone, 0x2c };

            init.CopyTo(bindings, 0);

            DecodeBinding(KeyL1).CopyTo(bindings, 5);
            DecodeBinding(KeyL2).CopyTo(bindings, 16);

            DecodeBinding(KeyR1).CopyTo(bindings, 27);
            DecodeBinding(KeyR2).CopyTo(bindings, 38);

            AsusHid.WriteInput(CommandReady, null);
            AsusHid.WriteInput(bindings, $"B{zone}");



        }

        static void WakeUp()
        {
            AsusHid.WriteInput(Encoding.ASCII.GetBytes("ZASUS Tech.Inc."), "Init");
        }

        static public void SetDeadzones()
        {
            WakeUp();

            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 4, 4,
                (byte)AppConfig.Get("ls_min", 0),
                (byte)AppConfig.Get("ls_max", 100),
                (byte)AppConfig.Get("rs_min", 0),
                (byte)AppConfig.Get("rs_max", 100)
            }, "StickDeadzone");

            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 5, 4,
                (byte)AppConfig.Get("lt_min", 0),
                (byte)AppConfig.Get("lt_max", 100),
                (byte)AppConfig.Get("rt_min", 0),
                (byte)AppConfig.Get("rt_max", 100)
            }, "TriggerDeadzone");

            AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 6, 2,
                (byte)AppConfig.Get("vibra", 100),
                (byte)AppConfig.Get("vibra", 100)
            }, "Vibration");

        }

        public static void ApplyMode(ControllerMode applyMode = ControllerMode.Auto)
        {
            Task.Run(() =>
            {

                if (applyMode == ControllerMode.Skip) return;

                HidStream? input = AsusHid.FindHidStream(AsusHid.INPUT_ID);
                int count = 0;

                while (input == null && count++ < 5)
                {
                    input = AsusHid.FindHidStream(AsusHid.INPUT_ID);
                    Thread.Sleep(2000);
                }

                if (input == null)
                {
                    Logger.WriteLine($"Controller not found");
                    return;
                }

                if (applyMode != ControllerMode.Auto) _applyMode = applyMode;

                InputDispatcher.SetBacklightAuto(true);
                WakeUp();

                AsusHid.WriteInput(new byte[] { AsusHid.INPUT_ID, 0xd1, 0x01, 0x01, (byte)_applyMode }, "Controller");
                AsusHid.WriteInput(CommandSave, null);

                BindZone(BindingZone.M1M2);

                BindZone(BindingZone.DPadUpDown);
                BindZone(BindingZone.DPadLeftRight);
                BindZone(BindingZone.StickClick);
                BindZone(BindingZone.Bumper);
                BindZone(BindingZone.AB);
                BindZone(BindingZone.XY);
                BindZone(BindingZone.ViewMenu);
                BindZone(BindingZone.Trigger);

                AsusHid.WriteInput(CommandSave, null);
                SetDeadzones();

            });
        }

        private void SetMode(ControllerMode mode)
        {

            _mode = mode;
            AppConfig.Set("controller_mode", (int)mode);

            ApplyMode(mode);

            if (mode == ControllerMode.Auto)
            {
                amdControl.StartFPS();
                timer.Start();
            }
            else
            {
                timer.Stop();
                amdControl.StopFPS();
            }

            settings.VisualiseController(mode);
        }

        public void ToggleMode()
        {

            switch (_mode)
            {
                case ControllerMode.Auto:
                    SetMode(ControllerMode.Gamepad);
                    break;
                case ControllerMode.Gamepad:
                    SetMode(ControllerMode.Mouse);
                    break;
                case ControllerMode.Mouse:
                    SetMode(ControllerMode.Skip);
                    break;
                case ControllerMode.Skip:
                    SetMode(ControllerMode.Auto);
                    break;
            }

        }

    }
}
