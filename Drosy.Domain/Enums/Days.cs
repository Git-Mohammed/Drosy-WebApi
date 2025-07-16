namespace Drosy.Domain.Enums;

[Flags]
public enum Days : byte
{
    None      = 0,
    Sunday    = 1,    // 0000001
    Monday    = 2,    // 0000010
    Tuesday   = 4,    // 0000100
    Wednesday = 8,    // 0001000
    Thursday  = 16,   // 0010000
    Friday    = 32,   // 0100000
    Saturday  = 64    // 1000000
}