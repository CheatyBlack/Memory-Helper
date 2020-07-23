using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
 
namespace Memory
{
   public class MemoryHelper
    {
        #region WINAPI
        int PROCESS_VM_OPERATION = 0x0008;
        int PROCESS_VM_READ = 0x0010;
        int PROCESS_VM_WRITE = 0x0020;
 
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
 
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, int size, out IntPtr lpNumberOfBytesRead);
 
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);
 
        #endregion
 
        Process _process;
        IntPtr _handle;
        public MemoryHelper(Process p)
        {
            _process = p;
            _handle = Open();
        }
 
        public IntPtr BaseAddress
        {
            get
            {
                return _process.MainModule.BaseAddress;
            }
        }
        private IntPtr Open()
        {
            int OpenMode = PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE;
            return OpenProcess(OpenMode, false, _process.Id);
        }
 
        public IntPtr ReadMemory64(IntPtr Address, int bytes)
        {
                int bytesRead = 0;
                byte[] buffer = new byte[bytes];
                IntPtr pBytesRead = IntPtr.Zero;
                bool result = ReadProcessMemory(_handle, Address, buffer, bytes, out pBytesRead);
                bytesRead = pBytesRead.ToInt32();
                return new IntPtr(BitConverter.ToInt64(buffer, 0));
        }
 
        public IntPtr ReadMemory32(IntPtr Address, int bytes)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[bytes];
            IntPtr pBytesRead = IntPtr.Zero;
            bool result = ReadProcessMemory(_handle, Address, buffer, bytes, out pBytesRead);
            bytesRead = pBytesRead.ToInt32();
            return new IntPtr(BitConverter.ToInt32(buffer, 0));
        }
 
        public byte[] ReadMemoryByte(IntPtr Address, int bytes)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[bytes];
            IntPtr pBytesRead = IntPtr.Zero;
            bool result = ReadProcessMemory(_handle, Address, buffer, bytes, out pBytesRead);
            bytesRead = pBytesRead.ToInt32();
            return buffer;
        }
 
        public void WriteMemory(IntPtr Address, int Value)
        {
            int br = 0;
            byte[] buffer = BitConverter.GetBytes(Value);
            WriteProcessMemory(_handle, Address, buffer, (uint)buffer.Length, out br);
        }
 
        public IntPtr OffsetCalculator(IntPtr BaseAddress, int[] Offsets)
        {
            var address = BaseAddress.ToInt64();
            foreach(int i in Offsets)
            {
                address = BitConverter.ToInt64(ReadMemoryByte((IntPtr)address, 8), 0) + i;
            }
            return (IntPtr)address;
        }
    }
}