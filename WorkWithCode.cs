using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disassembler
{
    internal class WorkWithCode
    {
        private StringBuilder result;

        public WorkWithCode(byte[] data)
        {
            uint e_lfanew = BitConverter.ToUInt32(data, 0x3C);

            ushort numberOfSections = BitConverter.ToUInt16(data, (int)e_lfanew + 6);
            ushort sizeOptionalHeader = BitConverter.ToUInt16(data, (int)e_lfanew + 20);

            long sectionTable = e_lfanew + 24 + sizeOptionalHeader;

            uint codeOffset = 0;
            uint codeSize = 0;
            bool found = false;

            result = new StringBuilder();

            for (int i = 0; i < numberOfSections; i++)
            {
                long sectionPosition = sectionTable + (i * 40);

                string sectionName = Encoding.ASCII.GetString(data, (int)sectionPosition, 8).TrimEnd('\0');

                if (sectionName == ".text" || sectionName == "CODE" || sectionName == ".code")
                {
                    codeOffset = BitConverter.ToUInt32(data, (int)sectionPosition + 20);
                    codeSize = BitConverter.ToUInt32(data, (int)sectionPosition + 16);
                    found = true;
                    break;
                }
            }

            if (found == true && codeSize > 0)
            {
                uint currentPosition = 0;

                while (currentPosition < codeSize)
                {
                    string instruction = InstructionCommand(codeOffset + currentPosition, ref currentPosition, data);
                    result.AppendLine(instruction);
                }
            }
        }

        private string InstructionCommand(uint fileOffset, ref uint currentPosition, byte[] data)
        {
            byte opcode = data[fileOffset];

            switch (opcode)
            {
                case 0x90: // NOP
                    currentPosition += 1;
                    return "nop";

                case 0x16: // PUSH SS
                    currentPosition += 1;
                    return "push ss";

                case 0x17: // POP SS  
                    currentPosition += 1;
                    return "pop ss";

                case 0x0E: // PUSH CS
                    currentPosition += 1;
                    return "push cs";

                case 0x1E: // PUSH DS
                    currentPosition += 1;
                    return "push ds";

                case 0x1F: // POP DS
                    currentPosition += 1;
                    return "pop ds";

                case 0x98: // CBW перевод байта в слово
                    currentPosition += 1;
                    return "cbw";

                case 0x99: // CWD перевод слова в дабл слово
                    currentPosition += 1;
                    return "cwd";

                case 0x9C: // PUSHFD push флаг
                    currentPosition += 1;
                    return "pushfd";

                case 0x9D: // POPFD pop флаг
                    currentPosition += 1;
                    return "popfd";

                case 0xC3: // RET
                    currentPosition += 1;
                    return "ret";

                case 0x50:
                case 0x51:
                case 0x52:
                case 0x53: // PUSH регистров (Помещение операнда в стек)
                case 0x54:
                case 0x55:
                case 0x56:
                case 0x57:
                    string pushReg = GetRegisterName(opcode - 0x50);
                    currentPosition += 1;
                    return $"push {pushReg}";

                case 0x58:
                case 0x59:
                case 0x5A:
                case 0x5B: // POP регистров (Извлечение операнда из стека)
                case 0x5C:
                case 0x5D:
                case 0x5E:
                case 0x5F:
                    string popReg = GetRegisterName(opcode - 0x58);
                    currentPosition += 1;
                    return $"pop {popReg}";

                case 0xB8:
                case 0xB9:
                case 0xBA:
                case 0xBB: // MOV регистр, значение
                case 0xBC:
                case 0xBD:
                case 0xBE:
                case 0xBF:
                    uint valueMov = BitConverter.ToUInt32(data, (int)fileOffset + 1);
                    string movReg = GetRegisterName(opcode - 0xB8);
                    currentPosition += 5;
                    return $"mov {movReg}, 0x{valueMov:X8}";

                case 0xE8: // CAL ближайший вызов с 32-ух битным смещением
                    int callOffset = BitConverter.ToInt32(data, (int)fileOffset + 1);
                    uint callTarget = fileOffset + 5 + (uint)callOffset;
                    currentPosition += 5;
                    return $"call 0x{callTarget:X8}";

                case 0x05: // ADD EAX, imm32
                    uint valueAdd = BitConverter.ToUInt32(data, (int)fileOffset + 1);
                    currentPosition += 5;
                    return $"add eax, 0x{valueAdd:X8}";

                case 0x2D: // SUB EAX, imm32
                    uint valueSub = BitConverter.ToUInt32(data, (int)fileOffset + 1);
                    currentPosition += 5;
                    return $"sub eax, 0x{valueSub:X8}";

                case 0x25: // AND EAX, imm32
                    uint valueAnd = BitConverter.ToUInt32(data, (int)fileOffset + 1);
                    currentPosition += 5;
                    return $"and eax, 0x{valueAnd:X8}";

                case 0x3D: // CMP EAX, imm32
                    uint valueCmp = BitConverter.ToUInt32(data, (int)fileOffset + 1);
                    currentPosition += 5;
                    return $"cmp eax, 0x{valueCmp:X8}";

                case 0xE9:  // JMP ближайший прыжок с 32-ух битным смещением
                    int jmpOffset = BitConverter.ToInt32(data, (int)fileOffset + 1);
                    uint jmpTarget = fileOffset + 5 + (uint)jmpOffset;
                    currentPosition += 5;
                    return $"jmp 0x{jmpTarget:X8}";

                case 0xEB: // JMP короткий прыжок с 8-ми битным смещением
                    sbyte shortOffset = (sbyte)data[fileOffset + 1];
                    uint shortTarget = fileOffset + 2 + (uint)shortOffset;
                    currentPosition += 2;
                    return $"jmp 0x{shortTarget:X8}";

                //прыжок если
                case 0x72: // JB ниже (беззнаковое сравнение)
                case 0x73: // JNB не ниже (беззнаковое сравнение)
                case 0x74: // JZ ноль
                case 0x75: // JNZ не ноль
                case 0x76: // JBE ниже или равно (беззнаковое сравнение)
                case 0x77: // JNBE не ниже и не равно (беззнаковое сравнение)
                    sbyte offset = (sbyte)data[fileOffset + 1];
                    uint target = fileOffset + 2 + (uint)offset;

                    string[] jumpMnemonics = { "jb", "jnb", "jz", "jnz", "jbe", "jnbe" };
                    string mnemonic = jumpMnemonics[opcode - 0x72];

                    currentPosition += 2;
                    return $"{mnemonic} 0x{target:X8}";

                default:
                    currentPosition += 1;
                    return "Дизассемблер не поддерживает эту команду";
            }
        }

        private string GetRegisterName(int regIndex)
        {
            string[] registers = { "eax", "ecx", "edx", "ebx", "esp", "ebp", "esi", "edi" };
            return regIndex < registers.Length ? registers[regIndex] : "unknown";
        }

        public string GetDisassemblyResult()
        {
            return result.ToString();
        }
    }
}
