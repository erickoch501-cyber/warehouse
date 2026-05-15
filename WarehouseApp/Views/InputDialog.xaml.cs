using System.Windows;

namespace WarehouseApp.Views
{
    public partial class InputDialog : Window
    {
        public string Answer { get; private set; } = "";

        public InputDialog(string prompt)
        {
            InitializeComponent();
            PromptTextBlock.Text = prompt;
            AnswerTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Answer = AnswerTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}