using System.Windows;

namespace Library
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReadersButton_Click(object sender, RoutedEventArgs e)
        {
            ReadersWindow readersWindow = new ReadersWindow();
            readersWindow.Show();
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            BooksWindow booksWindow = new BooksWindow();
            booksWindow.Show();
        }
    }
}

