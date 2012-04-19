namespace BananaHook.Asm
{
    public enum OpCode : byte
    {
        Push = 0x68,
        RetnAndPurge = 0xC3,
        Retn = 0xC3,
        Int3 = 0xCC
    }
}
