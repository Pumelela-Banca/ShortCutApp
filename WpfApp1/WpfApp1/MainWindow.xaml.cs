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
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            LoadButtons();
            LoadButtonColors();
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
                backGround = Brushes.LightYellow;
            }
            
            Button newButton = new Button
            {
                Content = name,
                Background = backGround,
                Tag = path,
                ContextMenu = new ContextMenu()
            };

            // Add delete
            MenuItem deleteItem = new MenuItem { Header = "Delete" };
            deleteItem.Click += (s, args) => ButtonsPannel.Children.Remove(newButton);
            newButton.ContextMenu.Items.Add(deleteItem);

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

                // Add button to window
                ButtonsPannel.Children.Add(selectFileBtn);
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
                    UseShellExecute = true,
                    FileName = filePath,
                    CreateNoWindow = true,
                };

                Process.Start(startInfor);
            });

        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folder = dialog.FileName;
                
                string btnName = Interaction.InputBox("Enter Button Name", "Button name", "");
                if (string.IsNullOrEmpty(btnName))
                {
                    btnName = System.IO.Path.GetFileName(dialog.FileName);
                }
             
                Button selectFileBtn = createButton(btnName, folder, "Folder");

                selectFileBtn.Click += Button_Click;

                // Add button to window
                ButtonsPannel.Children.Add(selectFileBtn);
            }

        }


        private void AddURL_Click(object sender, RoutedEventArgs e)
        {
            string urlToSave = Interaction.InputBox("Enter URL To Save", "URL", "");
            
            if (!string.IsNullOrEmpty(urlToSave))
            {
                MessageBox.Show(urlToSave);
                string urlName = Interaction.InputBox("Enter URL Name", "Button name", "");
                if (string.IsNullOrEmpty(urlName))
                {
                    urlName = urlToSave;
                }
                Button selectFileBtn = createButton(urlName, urlToSave, "URL");
                selectFileBtn.Click += Button_Click;

                // Add button to window
                ButtonsPannel.Children.Add(selectFileBtn);
            }
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
                        newButton.Click += Button_Click;
                        ButtonsPannel.Children.Add(newButton);
                    }
                }
            }
        }

        private void SaveButtonColors()
        {
            List<ButtonData> buttons = new List<ButtonData>();

            buttons.Add( new ButtonData
            {
                ButtonName = AddFile.Name,
                ActionPath = AddFile.Background.ToString()
            });
            buttons.Add(new ButtonData
            {
                ButtonName = AddFolder.Name,
                ActionPath = AddFolder.Background.ToString()
            });
            buttons.Add(new ButtonData
            {
                ButtonName = AddURL.Name,
                ActionPath = AddURL.Background.ToString()
            });

            XmlSerializer serializer = new XmlSerializer(typeof(List<ButtonData>));
            using (TextWriter writer = new StreamWriter("buttonColors.xml"))
            {
                serializer.Serialize(writer, buttons);
            }
        }

        private void LoadButtonColors()
        {
            if (File.Exists("buttonColors.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ButtonData>));
                using (TextReader reader = new StreamReader("buttonColors.xml"))
                {
                    List<ButtonData>? buttonDataList = (List<ButtonData>?)serializer.Deserialize(reader);

                    if (buttonDataList != null)
                    {
                        foreach (var buttonData in buttonDataList)
                        {
                            switch (buttonData.ButtonName)
                            {
                                case "AddFile":

                                    SetButtonColor(AddFile, buttonData.ActionPath);
                                    break;
                                case "AddFolder":
                                    SetButtonColor(AddFolder, buttonData.ActionPath);
                                    break;
                                case "AddURL":
                                    SetButtonColor(AddURL, buttonData.ActionPath);
                                    break;
                            }
                        }
                    }
                }
            }
                
        }

        public void SetButtonColor(Button button, string? userColor)
        {
            try
            {
#pragma warning disable
                // Convert the user-provided string into a SolidColorBrush
                button.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(userColor);
#pragma warning disable
            }
            catch
            {
                // Handle invalid color strings
                button.Background = Brushes.Transparent; // Default fallback color
            }
        }




        protected void FileExit_Click(object sender, RoutedEventArgs e )
        {
            OnClosed(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            SaveButtons();
            SaveButtonColors();
            base.OnClosed(e);
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorPickerDialog();
            if (dialog.ShowDialog() == true && dialog.SelectedColor.HasValue)
            {
                Color selected = dialog.SelectedColor.Value;
                AddFile.Background = new SolidColorBrush(selected); 
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorPickerDialog();
            if (dialog.ShowDialog() == true && dialog.SelectedColor.HasValue)
            {
                Color selected = dialog.SelectedColor.Value;
                AddFolder.Background = new SolidColorBrush(selected); 
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorPickerDialog();
            if (dialog.ShowDialog() == true && dialog.SelectedColor.HasValue)
            {
                Color selected = dialog.SelectedColor.Value;
                AddURL.Background = new SolidColorBrush(selected);
            }
        }
    }
}