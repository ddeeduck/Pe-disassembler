using System;
using System.IO;

namespace Disassembler
{
    public static class PEChecker
    {
        public static bool FindMZandPE(byte[] data, out string details)
        {
            details = "";

            try
            {
                if (data.Length < 2 || data[0] != 0x4D || data[1] != 0x5A)
                {
                    details = "Нет сигнатуры MZ";
                    return false;
                }

                if (data.Length < 0x40) //0x40 = 64 байта
                {
                    details = "Файл слишком маленький для PE";
                    return false;
                }

                uint e_lfanew = BitConverter.ToUInt32(data, 0x3C); //uint -- целое беззнаковое 32-ух битное число
                if (e_lfanew >= data.Length - 4)
                {
                    details = "Неверное значение смещения до PE заголовка";
                    return false;
                }

                uint peSignature = BitConverter.ToUInt32(data,(int)e_lfanew);
                if (peSignature != 0x00004550) // 0x00004550 = "PE\0\0"
                {
                    details = "Нет сигнатуры PE";
                    return false;
                }

                ushort architecture = BitConverter.ToUInt16(data, (int)e_lfanew + 4); // ushort -- беззнаковое 16-ти битное число
                if (architecture != 0x014C) //0x014C = x86
                {
                    details = "Дизассемблер поддерживает только x86";
                    return false;
                }

                details = "PE файл верный";
                return true;
            }
            catch (Exception ex)
            {
                details = $"Ошибка: {ex.Message}";
                return false;
            }
        }
    }
}
