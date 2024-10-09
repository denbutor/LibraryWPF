using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Library.Data;

namespace Library
{
    public partial class ReadersWindow : Window
    {
        private List<Reader> readers = new List<Reader>();
        private Reader selectedReader;

        public ReadersWindow()
        {
            InitializeComponent();
            LoadReaders();
        }

        private void LoadReaders()
        {
            using (var context = new LibraryContext())
            {
                readers = context.Readers.ToList();
                ReadersListBox.ItemsSource = readers;
            }
        }

        private void AddReader_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateReaderFields())
            {
                using (var context = new LibraryContext())
                {
                    var reader = new Reader
                    {
                        Id = Guid.NewGuid(),
                        FirstName = NewFirstNameTextBox.Text,
                        LastName = NewLastNameTextBox.Text,
                        PhoneNumber = NewPhoneNumberTextBox.Text,
                        Email = NewEmailTextBox.Text,
                    };

                    context.Readers.Add(reader);
                    context.SaveChanges();
                }

                ClearFormFields();
                LoadReaders();
            }
        }

        private bool ValidateReaderFields()
        {
            if (string.IsNullOrWhiteSpace(NewFirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(NewLastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(NewPhoneNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(NewEmailTextBox.Text))
            {
                MessageBox.Show("Будь ласка, введіть коректні дані для всіх полів.");
                return false;
            }
            return true;
        }

        private void ReadersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedReader = ReadersListBox.SelectedItem as Reader;

            if (selectedReader != null)
            {
                NewFirstNameTextBox.Text = selectedReader.FirstName;
                NewLastNameTextBox.Text = selectedReader.LastName;
                NewPhoneNumberTextBox.Text = selectedReader.PhoneNumber;
                NewEmailTextBox.Text = selectedReader.Email;
            }
        }


        private void DeleteReader_Click(object sender, RoutedEventArgs e)
        {
            if (selectedReader != null)
            {
                using (var context = new LibraryContext())
                {
                    var readerToRemove = context.Readers.Find(selectedReader.Id);
                    if (readerToRemove != null)
                    {
                        context.Readers.Remove(readerToRemove);
                        context.SaveChanges();
                    }
                }

                LoadReaders();
                ClearFormFields(); 
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть читача для видалення.");
            }
        }
        
        private void SearchReadersButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                LoadReaders();
            }
            else
            {
                var filteredReaders = readers.Where(r =>
                    r.FirstName.ToLower().Contains(searchQuery) || 
                    r.LastName.ToLower().Contains(searchQuery)).ToList();
        
                ReadersListBox.ItemsSource = filteredReaders;
            }
        }


        private void ClearFormFields()
        {
            NewFirstNameTextBox.Clear();
            NewLastNameTextBox.Clear();
            NewPhoneNumberTextBox.Clear();
            NewEmailTextBox.Clear();
            selectedReader = null;
        }

        private void EditReader_Click(object sender, RoutedEventArgs e)
        {
            if (selectedReader != null && ValidateReaderFields())
            {
                using (var context = new LibraryContext())
                {
                    var readerToUpdate = context.Readers.Find(selectedReader.Id);
                    if (readerToUpdate != null)
                    {
                        readerToUpdate.FirstName = NewFirstNameTextBox.Text;
                        readerToUpdate.LastName = NewLastNameTextBox.Text;
                        readerToUpdate.PhoneNumber = NewPhoneNumberTextBox.Text;
                        readerToUpdate.Email = NewEmailTextBox.Text;
                        context.SaveChanges();
                    }
                }

                ClearFormFields();
                LoadReaders();
            }
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
    }
}
