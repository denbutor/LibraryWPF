using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Library.Data;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public partial class BooksWindow : Window
    {
        private ObservableCollection<Book> books = new ObservableCollection<Book>();
        private Book selectedBook;
        private Reader selectedReader;

        public BooksWindow()
        {
            InitializeComponent();
            LoadBooks();
            LoadReaders();
        }

        private void LoadBooks()
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    books.Clear();
                    var loadedBooks = context.Books.Include(b => b.Reader).ToList();
                    foreach (var book in loadedBooks)
                    {
                        books.Add(book);
                    }
                    BooksListBox.ItemsSource = books;

                    
                    MessageBox.Show($"Завантажено книг: {books.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні книг: {ex.Message}");
            }
        }

        private void LoadReaders()
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    var readers = context.Readers.ToList();
                    ReadersComboBox.ItemsSource = readers;

                    if (!readers.Any())
                    {
                        MessageBox.Show("Читачі не знайдені.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні читачів: {ex.Message}");
            }
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateBookFields())
            {
                using (var context = new LibraryContext())
                {
                    var book = new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = NewTitleTextBox.Text,
                        Author = NewAuthorTextBox.Text,
                        Genre = NewGenreTextBox.Text,
                        Year = int.Parse(NewYearTextBox.Text),
                        ReaderId = (ReadersComboBox.SelectedItem as Reader)?.Id
                    };

                    context.Books.Add(book);
                    context.SaveChanges();
                    books.Add(book);
                }

                ClearFormFields();
            }
        }

        private bool ValidateBookFields()
        {
            if (string.IsNullOrWhiteSpace(NewTitleTextBox.Text) ||
                string.IsNullOrWhiteSpace(NewAuthorTextBox.Text) ||
                string.IsNullOrWhiteSpace(NewGenreTextBox.Text) ||
                !int.TryParse(NewYearTextBox.Text, out _) ||
                ReadersComboBox.SelectedItem == null)
            {
                MessageBox.Show("Будь ласка, введіть коректні дані для всіх полів і виберіть читача.");
                return false;
            }
            return true;
        }

        private void BooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedBook = BooksListBox.SelectedItem as Book;

            if (selectedBook != null)
            {
                NewTitleTextBox.Text = selectedBook.Title;
                NewAuthorTextBox.Text = selectedBook.Author;
                NewGenreTextBox.Text = selectedBook.Genre;
                NewYearTextBox.Text = selectedBook.Year.ToString();
                ReadersComboBox.SelectedItem = selectedBook.Reader;
            }
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook != null)
            {
                using (var context = new LibraryContext())
                {
                    var bookToRemove = context.Books.Find(selectedBook.Id);
                    if (bookToRemove != null)
                    {
                        context.Books.Remove(bookToRemove);
                        context.SaveChanges();
                        books.Remove(selectedBook);
                    }
                }

                ClearFormFields();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть книгу для видалення.");
            }
        }

        private void SearchBooksButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                LoadBooks();
            }
            else
            {
                var filteredBooks = books.Where(b => b.Title.ToLower().Contains(searchQuery)).ToList();
                BooksListBox.ItemsSource = new ObservableCollection<Book>(filteredBooks);
            }
        }

        private void ClearFormFields()
        {
            NewTitleTextBox.Clear();
            NewAuthorTextBox.Clear();
            NewGenreTextBox.Clear();
            NewYearTextBox.Clear();
            ReadersComboBox.SelectedItem = null;
            selectedBook = null;
        }
        
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Tag.ToString() == textBox.Text)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void ReadersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedReader = ReadersComboBox.SelectedItem as Reader;
        }
    }
}
