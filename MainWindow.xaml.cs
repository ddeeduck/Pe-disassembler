using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Disassembler
{
    public partial class MainWindow : Window
    {
        private byte[] currentFileData;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            DisassemblerBtn.IsEnabled = false;
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Все файлы (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                FilePathBox.Text = dialog.FileName;
                ResultText.Text = "";
            }
        }

        private async void Check_Click(object sender, RoutedEventArgs e)
        {
            string path = FilePathBox.Text;
            FileInfo fileInfo = new FileInfo(path);

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                ResultText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                ResultText.Text = "Файл не найден";
                return;
            }

            ResultText.Foreground = System.Windows.Media.Brushes.LightGray;
            ResultText.Text = "Проверяю...";

            try
            {
                currentFileData = await Task.Run(() => File.ReadAllBytes(path));

                (bool found, string details) result = await Task.Run(() =>
                {
                    bool found = PEChecker.FindMZandPE(currentFileData, out string details);
                    return (found, details);
                });

                if (result.found)
                {
                    ResultText.Foreground = System.Windows.Media.Brushes.LightGreen;
                    ResultText.Text = $"EXE файл найден\n{result.details}\nИмя файла: {fileInfo.Name}\n" +
                        $"Путь: {path}\nВремя создания: {fileInfo.CreationTime}\nРазмер: {fileInfo.Length} байт";
                    DisassemblerBtn.IsEnabled = true;
                }
                else
                {
                    ResultText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                    ResultText.Text = $"Ошибка: {result.details}";
                }
            }
            catch (Exception ex)
            {
                ResultText.Foreground = System.Windows.Media.Brushes.OrangeRed;
                ResultText.Text = "Ошибка: " + ex.Message;
            }
        }

        private void Disassembler_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "Все файлы (*.*)|*.*",
                FileName = "результат дизассемблирования.txt",
                DefaultExt = ".txt",
                Title = "Сохранить результат дизассемблирования"
            };

            if (saveFile.ShowDialog() == true)
            {
                WorkWithCode disassembler = new WorkWithCode(currentFileData);
                string result = disassembler.GetDisassemblyResult();

                File.WriteAllText(saveFile.FileName, result, System.Text.Encoding.UTF8);

                ResultText.Foreground = System.Windows.Media.Brushes.LightGreen;
                ResultText.Text = $"Дизассемблирование завершено!\nРезультат сохранен в:\n{saveFile.FileName}";
            }
        }
    }
}
