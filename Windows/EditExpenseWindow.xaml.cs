using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ExpenseTracker.Windows
{
    /// <summary>
    /// Interaction logic for EditExpenseWindow.xaml
    /// </summary>
    public partial class EditExpenseWindow : Window
    {
        private Expense _expense; // Змінна для зберігання витрати, яку редагуємо

        public EditExpenseWindow(Expense expense)
        {
            InitializeComponent();
            _expense = expense;

            // Завантаження категорій з бази даних
            using (var context = new ExpenseTrackerContext())
            {
                // Завантажуємо всі категорії, крім "Other"
                var categories = context.Categories
                    .Where(c => c.Name != "Other")
                    .OrderBy(c => c.Name)
                    .Select(c => c.Name)
                    .ToList();

                // Додаємо категорію "Other" в кінець списку
                categories.Add("Other");

                // Встановлюємо список категорій як джерело даних для ComboBox
                categoryComboBox.ItemsSource = categories;
            }

            // Заповнити поля форми даними витрати
            categoryComboBox.SelectedItem = _expense.Category; // Встановлюємо обрану категорію
            amountTextBox.Text = _expense.Amount.ToString();
            datePicker.SelectedDate = _expense.Date;
            descriptionTextBox.Text = _expense.Description;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримати дані з форми
            string category = categoryComboBox.SelectedItem as string; // Отримуємо з ComboBox
            decimal amount;
            if (!decimal.TryParse(amountTextBox.Text, out amount))
            {
                MessageBox.Show("Incorrect amount format!");
                return;
            }
            DateTime date = datePicker.SelectedDate ?? DateTime.Now;
            string description = descriptionTextBox.Text;

            // Оновити дані витрати
            _expense.Category = category;
            _expense.Amount = amount;
            _expense.Date = date;
            _expense.Description = description;

            // Зберегти зміни в базі даних
            using (var context = new ExpenseTrackerContext())
            {
                context.Entry(_expense).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            // Повідомити про успішне збереження
            MessageBox.Show("Changes saved!");

            // Закрити вікно редагування
            DialogResult = true; // Повернути true, щоб MainWindow знав, що дані були змінені
        }
    }
}