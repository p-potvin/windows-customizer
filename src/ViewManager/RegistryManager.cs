using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ViewManager
{
    internal class RegistryManager
    {
        private const string BagsPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags";

        public string? FindBagIdForPath(string targetPath)
        {
            IntPtr targetPidl = IntPtr.Zero;
            try
            {
                targetPidl = Shell.ILCreateFromPathW(targetPath);
                if (targetPidl == IntPtr.Zero)
                {
                    Console.Error.WriteLine("Failed to get PIDL for the specified path.");
                    return null;
                }

                using var bagsKey = Registry.CurrentUser.OpenSubKey(BagsPath);
                if (bagsKey == null) return null;

                foreach (var bagId in bagsKey.GetSubKeyNames())
                {
                    using var bagKey = bagsKey.OpenSubKey(bagId);
                    if (bagKey?.GetValue("NodeSlot") is byte[] nodeSlotPidlBytes)
                    {
                        if (PidlEquals(targetPidl, nodeSlotPidlBytes))
                        {
                            return bagId;
                        }
                    }
                }
            }
            finally
            {
                if (targetPidl != IntPtr.Zero)
                {
                    Shell.ILFree(targetPidl);
                }
            }

            return null;
        }

        public Dictionary<string, object>? GetBagData(string bagId)
        {
            var data = new Dictionary<string, object>();
            using var bagKey = Registry.CurrentUser.OpenSubKey(Path.Combine(BagsPath, bagId));
            if (bagKey == null) return null;

            foreach(var valueName in bagKey.GetValueNames())
            {
                data[valueName] = bagKey.GetValue(valueName);
            }
            return data;
        }

        public void ApplyBagData(string bagId, Dictionary<string, object> bagData)
        {
            using var bagKey = Registry.CurrentUser.CreateSubKey(Path.Combine(BagsPath, bagId));
            
            // Clear existing values before applying new ones
            foreach(var valueName in bagKey.GetValueNames())
            {
                bagKey.DeleteValue(valueName);
            }

            foreach(var kvp in bagData)
            {
                var value = kvp.Value;
                RegistryValueKind kind = GetValueKind(value);
                bagKey.SetValue(kvp.Key, value, kind);
            }
        }
        
        public void RestartExplorer()
        {
            foreach (var process in Process.GetProcessesByName("explorer"))
            {
                process.Kill();
            }
        }

        private RegistryValueKind GetValueKind(object value)
        {
            if (value is int || value is uint) return RegistryValueKind.DWord;
            if (value is long || value is ulong) return RegistryValueKind.QWord;
            if (value is byte[]) return RegistryValueKind.Binary;
            return RegistryValueKind.String;
        }

        private bool PidlEquals(IntPtr pidl1, byte[] pidl2Bytes)
        {
            int pidl1Size = (int)ILGetSize(pidl1);
            if (pidl1Size != pidl2Bytes.Length) return false;

            byte[] pidl1Bytes = new byte[pidl1Size];
            Marshal.Copy(pidl1, pidl1Bytes, 0, pidl1Size);

            return pidl1Bytes.SequenceEqual(pidl2Bytes);
        }

        private uint ILGetSize(IntPtr pidl)
        {
            if (pidl == IntPtr.Zero) return 0;
            uint size = 0;
            IntPtr current = pidl;
            while (Marshal.ReadInt16(current) != 0)
            {
                ushort itemSize = (ushort)Marshal.ReadInt16(current);
                size += itemSize;
                current = (IntPtr)(current.ToInt64() + itemSize);
            }
            size += sizeof(ushort); // Add the size of the terminating NULL
            return size;
        }
    }
}
