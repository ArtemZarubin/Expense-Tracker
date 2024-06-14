using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System;
using System.Linq;
using System.Windows;

namespace ExpenseTracker.Windows
{
    public partial class AddExpenseWindow : Window
    {
        public AddExpenseWindow()
        {
            InitializeComponent();

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
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримати дані з текстових полів
            string category = categoryComboBox.SelectedItem as string; // Отримуємо обрану категорію з ComboBox

            decimal amount;
            if (!decimal.TryParse(amountTextBox.Text, out amount))
            {
                MessageBox.Show("Invalid amount format!");
                return;
            }
            DateTime date = datePicker.SelectedDate ?? DateTime.Now;
            string description = descriptionTextBox.Text;

            // Обробка нової категорії
            if (category == "Add new")
            {
                // Перевірка на пусту назву категорії
                if (string.IsNullOrEmpty(category))
                {
                    MessageBox.Show("Enter the name of the new category!");
                    return;
                }

                // Перевірка, чи така категорія вже існує
                using (var context = new ExpenseTrackerContext())
                {
                    if (context.Categories.Any(c => c.Name == category))
                    {
                        MessageBox.Show("This category is already in existence!");
                        return;
                    }

                    // Додати нову категорію до бази даних
                    context.Categories.Add(new Category { Name = category });
                    context.SaveChanges();

                    // Додаємо нову категорію до ComboBox
                    categoryComboBox.Items.Add(category);
                    categoryComboBox.SelectedItem = category; // Вибираємо нову категорію в ComboBox
                }
            }

            // Створити новий об'єкт Expense
            Expense newExpense = new Expense
            {
                Category = category,
                Amount = amount,
                Date = date,
                Description = description
            };

            // Зберегти витрату в базі даних
            using (var context = new ExpenseTrackerContext())
            {
                context.Expenses.Add(newExpense);
                context.SaveChanges();
            }

            // Оновити DataGrid в MainWindow
            ((MainWindow)Owner).UpdateExpensesDataGrid();

            // Повідомити про успішне збереження
            MessageBox.Show("Data saved!");

            // Очистити поля форми
            categoryComboBox.SelectedItem = null; // Очищаємо вибір в ComboBox
            amountTextBox.Text = "";
            datePicker.SelectedDate = null;
            descriptionTextBox.Text = "";
        }
    }
}