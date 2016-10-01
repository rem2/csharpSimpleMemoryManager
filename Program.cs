class memoryFunctions
{
    private IntPtr processHandle; //for the handle to the process so we can read data from the process, would be HANDLE processHandle in c++
    private string processName;

    private IntPtr createHandle()
    {
        Process[] processes = Process.GetProcessesByName(processName);

        foreach (Process p in processes) //get the specified process
        {
            return windowsAPIs.OpenProcess(0xFFFF, false, p.Id); //open a handle to the process using the process id with full privileges (0xFFFF)
        }
        return (IntPtr)0; //better way to do this?
    }
    private IntPtr setModuleAddress(string moduleName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        int length = moduleName.Length;

        foreach (Process p in processes) //get the specified process
        {
            foreach (ProcessModule module in p.Modules)
            {
                if (((module.ModuleName).IndexOf(moduleName) != -1) && ((module.ModuleName).Length == length)) //checks to make sure that we dont get a different module that contains part of the same module name that we're searching for
                {
                    IntPtr m = module.BaseAddress; //get the address of the client.dll
                    module.Dispose();
                    return m;
                }
            }
        }
        return (IntPtr)0; //better way to do this?
    }
    public void setProcessName(string pName)
    {
        processName = pName;
    }
    public IntPtr getHandle()
    {
        processHandle = createHandle(); //this is used for reading/writing later
        return processHandle;
    }
    public IntPtr getModuleAddress(string moduleName)
    {
        return setModuleAddress(moduleName);
    }
    public void deconstruct() //cleanup added 9/27/2016
    {
        if (processHandle != IntPtr.Zero) //check to make sure that we have a Handle opened
        {
           bool hresult = CloseHandle(processHandle);
        }
    }


    //memory reading//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private byte[] readMem(int offset, int size) //offset is the adddress to whatever we're trying to read
    {
        byte[] buffer = new byte[size];
        windowsAPIs.ReadProcessMemory((int)processHandle, offset, buffer, size, 0); //0 because it doesnt matter how many bytes we have written
        return buffer;
    }
    public int readInt(int offset)
    {
        return BitConverter.ToInt32(readMem(offset, 4), 0); //converts bytes to int, 4 is the size that you will need 99% of the time
    }
    public float readFloat(int offset)
    {
        return BitConverter.ToSingle(readMem(offset, 4), 0); //single = float aparently
    }
    public double readDouble(int offset)
    {
        return BitConverter.ToDouble(readMem(offset, 4), 0);
    }
    public string readString(int offset)
    {
        return BitConverter.ToString(readMem(offset, 4), 0);
    }
    public bool readBool(int offset)
    {
        return BitConverter.ToBoolean(readMem(offset, 4), 0);
    }
    public uint readUnsignedInt(int offset)
    {
        return BitConverter.ToUInt32(readMem(offset, 4), 0);
    }


    //memory writing//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void writeMem(int offset, int size, byte[] value) //offset is the adddress to whatever we're trying to read
    {
        windowsAPIs.WriteProcessMemory((int)processHandle, offset, value, size, 0); //0 because it doesnt matter how many bytes we have written
    }
    public void writeInt(int offset, int value)
    {
        writeMem(offset, 4, BitConverter.GetBytes(value)); //converts int to byte, 4 is the size that you will need 99% of the time
    }
    public void writeFloat(int offset, float value)
    {
        writeMem(offset, 4, BitConverter.GetBytes(value));
    }
    public void writeDouble(int offset, double value)
    {
        writeMem(offset, 4, BitConverter.GetBytes(value));
    }
    public void writeStringU(int offset, string value)
    {
        writeMem(offset, 4, Encoding.Unicode.GetBytes(value));
    }
    public void writeStringA(int offset, string value)
    {
        writeMem(offset, 4, Encoding.ASCII.GetBytes(value));
    }
    public void writeBool(int offset, bool value)
    {
        writeMem(offset, 4, BitConverter.GetBytes(value));
    }
    public void writeUnsignedInt(int offset, uint value)
    {
        writeMem(offset, 4, BitConverter.GetBytes(value));
    }
}
class windowsAPIs
{
    [DllImport("user32.dll")]
    public static extern ushort GetAsyncKeyState(int vKey); //so we can see if our triggerkey is pressed

    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo); //so we can simulate a mouse click

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId); //so we can get access to the process

    [DllImport("Kernel32")] //added 9/27/2016
    public static extern Boolean CloseHandle(IntPtr handle);
    
    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead); //so we can read data from the process using our handle

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesWritten); //could be ref int lpNumberOfBytesWritten but im not interested in how much ive written to the process

    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;

    public void doLeftMouseClick()
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
    public bool isKeyPushedDown(int vKey)
    {
        return 0 != (GetAsyncKeyState(vKey) & 0x8000); //bitwise checks to see if key is pressed down
    }
}
