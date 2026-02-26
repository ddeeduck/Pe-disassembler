# PE Disassembler

![WPF](https://img.shields.io/badge/Platform-WPF-blue)
![C#](https://img.shields.io/badge/Language-C%23-green)
![License](https://img.shields.io/badge/License-MIT-yellow)

A lightweight Windows application that disassembles 32-bit PE files (.exe) and displays the assembly instructions from the `.text` section. Built with WPF and C#.

The Russian version is available [here](README.ru.md).

## Description
This tool allows you to explore the internal structure of executable files by extracting and decoding machine code into human-readable assembly instructions. It is designed for educational purposes and basic reverse engineering, demonstrating how PE files are structured and how x86 instructions are encoded.

## Technologies Used
- **C#** – core programming language
- **WPF (Windows Presentation Foundation)** – for the graphical user interface
- **.NET Framework / .NET Core** – application framework

## Features
- Select any file and check for valid MZ/PE signatures.
- Verify that the target is an x86 executable.
- Extract and decode a limited but common set of x86 instructions:
  - Data movement: `MOV`, `PUSH`, `POP`
  - Arithmetic: `ADD`, `SUB`, `AND`
  - Control flow: `JMP`, `CALL`, `RET`, conditional jumps
  - Other: `NOP`, `CBW`, `CWD`, `PUSHFD` / `POPFD`
- Save the disassembly listing to a text file.

## Screenshots

<img width="600" src="images/mainWindow.png">

## Example Output

<img width="600" src="images/output.png">

## Installation & Usage
### Requirements
- Windows OS with .NET Framework 4.7.2+ or .NET 5/6/7/8 (with WPF support)
- Visual Studio 2019/2022 (for building from source)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/ddeeduck/Pe-disassembler.git
2. Open the solution file (Disassembler.sln) in Visual Studio.

3. Build the solution (Ctrl+Shift+B).

4. Run the application (F5).

### How to Use
1. Launch the application.

2. Click "Browse" and select an .exe file.

3. Click "Check" to validate the PE structure.

4. If the file is valid, click "Disassemble" to generate the instruction list.

5. Save the result to a .txt file.

### Limitations
- Only supports x86 (32-bit) PE files.
- The instruction decoder covers only the opcodes implemented in the source; many instructions are not yet recognized.
- The disassembly is based on raw file offsets, not virtual addresses (no relocation handling).

### Author

Daria – [GitHub](https://github.com/ddeeduck), [Telegram](https://t.me/deeduck), [LinkedIn](www.linkedin.com/in/deeduck), Email: dehterevich.daria@gmail.com

### License
This project is licensed under the MIT License – see the LICENSE file for details.
