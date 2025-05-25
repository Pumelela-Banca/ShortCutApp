using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ColorPickerDialog.xaml
    /// </summary>
    public partial class ColorPickerDialog : Window
    {
        public Color? SelectedColor { get; private set; }

        public ColorPickerDialog(Color? initialColor = null)
        {
            InitializeComponent();
            ColorSelector.SelectedColor = initialColor ?? Colors.White;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor = ColorSelector.SelectedColor;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ColorSelector_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Optional: Live update of SelectedColor if needed
        }
    }
}
