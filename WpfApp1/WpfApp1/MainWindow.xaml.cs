using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Diagnostics;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //LoadButtons();
        }


        private Button createButton(string name, string path, string parent)
        {
            Brush backGround;
            if (parent == "File")
            {
                backGround = Brushes.LightBlue;
            }
            else if (parent == "Folder")
            {
                backGround = Brushes.LightGreen;
            }
            else
            {
                backGround = Brushes.DarkOliveGreen;
            }
            
            Button newButton = new Button
            {
                Content = name,
                Margin = new Thickness(5),
                Padding = new Thickness(10, 5, 10, 5),
                Background = backGround,
                Tag = path
            };

            return newButton;
        }
        private void AddFile_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFile = new OpenFileDialog();
            

            if (openFile.ShowDialog() == true)
            {
                
                string fileName = openFile.FileName;
                string btnName = Interaction.InputBox("Enter Button Name", "Button name", "");
                if (string.IsNullOrEmpty(btnName))
                {
                    btnName = fileName;
                }

                Button selectFileBtn = createButton(btnName, openFile.FileName, "File");
                selectFileBtn.Click += Button_Click;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button clickedButton)
            {
                string? filePath = clickedButton.Tag?.ToString();
                RunProcessAsync(filePath);
            }
        }

        private async Task RunProcessAsync(string? filePath)
        {
            await Task.Run(() => 
            {
                var startInfor = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    Arguments = filePath,
                    CreateNoWindow = true,
                };

                Process.Start(startInfo);
            });

        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddURL_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButtons()
        {
            List<ButtonData> buttons = new List<ButtonData>();

            foreach (Button btn in ButtonsPannel.Children)
            {
                buttons.Add(new ButtonData
                {
                    ButtonName = btn.Content.ToString(),
                    ActionPath = btn.Tag as string,
                });
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<ButtonData>));
            using (TextWriter writer = new StreamWriter("buttons.xml"))
            {
                serializer.Serialize(writer, buttons);
            }
        }

        private void LoadButtons()
        {
            if (File.Exists("buttons.xml"))
            {
                XmlSerializer serializer = new XmlSerializer (typeof(List<ButtonData>));
                using (TextReader reader = new StreamReader("buttons.xml"))
                {
                    List<ButtonData> buttons = (List<ButtonData>)serializer.Deserialize(reader);

                    foreach (var btnData in buttons)
                    {
                        Button newButton = new Button
                        {
                            Content = btnData.ButtonName,
                            Tag = btnData.ActionPath
                        };
                        //newButton.Click += 
                        ButtonsPannel.Children.Add(newButton);
                    }
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            //SaveButtons();
            base.OnClosed(e);
        }

    }
}