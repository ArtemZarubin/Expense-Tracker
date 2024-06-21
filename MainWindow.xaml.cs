using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace ExpenseTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Error;

            try
            {
                UpdateExpensesDataGrid();
            }
            catch (Exception ex)
            {
                // Вивести повідомлення про помилку, якщо щось піде не так
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public void UpdateExpensesDataGrid()
        {
            using (var context = new ExpenseTrackerContext())
            {
                var expenses = context.Expenses.Include("Category").ToList();
                expensesDataGrid.ItemsSource = expenses;
                expensesDataGrid.Items.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
        }

        private void addExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow();
            addExpenseWindow.Owner = this;
            addExpenseWindow.Show();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримати вибраний елемент
            var selectedItem = expensesDataGrid.SelectedItem;

            // Перевірити, чи це дійсно об'єкт Expense
            if (selectedItem is Expense selectedExpense)
            {
                // Відкрити вікно редагування
                EditExpenseWindow editWindow = new EditExpenseWindow(selectedExpense);
                editWindow.Owner = this;
                if (editWindow.ShowDialog() == true)
                {
                    // Оновити DataGrid після редагування
                    UpdateExpensesDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Select an expense to edit!");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримати вибраний елемент
            var selectedItem = expensesDataGrid.SelectedItem;

            // Перевірити, чи це дійсно об'єкт Expense
            if (selectedItem is Expense selectedExpense)
            {
                // Запитати підтвердження видалення
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete an expense:\nCategory: {selectedExpense.Category.Name}\nAmount: {selectedExpense.Amount}\nDate: {selectedExpense.Date:dd.MM.yyyy}?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Видалити витрату з бази даних
                    using (var context = new ExpenseTrackerContext())
                    {
                        context.Expenses.Attach(selectedExpense);
                        context.Expenses.Remove(selectedExpense);
                        context.SaveChanges();
                    }

                    // Оновити DataGrid
                    UpdateExpensesDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Select an expense to delete!");
            }
        }

        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = startDatePicker.SelectedDate;
            DateTime? endDate = endDatePicker.SelectedDate;

            using (var context = new ExpenseTrackerContext())
            {
                // Фільтруємо дані за датою
                var expenses = context.Expenses.Include("Category")
                    .Where(expense =>
                        (startDate.HasValue && endDate.HasValue && expense.Date >= startDate.Value && expense.Date <= endDate.Value) ||
                        (startDate.HasValue && !endDate.HasValue && expense.Date == startDate.Value) ||
                        (!startDate.HasValue && endDate.HasValue && expense.Date == endDate.Value))
                    .ToList();

                expensesDataGrid.ItemsSource = expenses;
            }
        }

        private void showPieChartButton_Click(object sender, RoutedEventArgs e)
        {
            PieChartWindow pieChartWindow = new PieChartWindow();
            pieChartWindow.Show();
        }

        private void showHistogramButton_Click(object sender, RoutedEventArgs e)
        {
            // Отримання початкової та кінцевої дат з DatePicker
            DateTime startDate = startDatePicker.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = endDatePicker.SelectedDate ?? DateTime.MaxValue;

            // Створення та відображення вікна гістограми
            HistogramWindow histogramWindow = new HistogramWindow(startDate, endDate);
            histogramWindow.Show();
        }
    }
}
