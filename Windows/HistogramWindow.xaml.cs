using ExpenseTracker.Data;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ExpenseTracker.Windows
{
    public partial class HistogramWindow : Window
    {
        public List<string> Categories { get; set; }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        private DateTime startDate;
        private DateTime endDate;

        public HistogramWindow(DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            this.startDate = startDate;
            this.endDate = endDate;

            SeriesCollection = new SeriesCollection();
            Categories = new List<string>();

            using (var context = new ExpenseTrackerContext())
            {
                Categories = context.Expenses.Select(e => e.Category).Distinct().ToList();
            }

            UpdateHistogram();
        }

        public void UpdateHistogram()
        {
            using (var context = new ExpenseTrackerContext())
            {
                // Отримання даних для гістограми
                var expenses = context.Expenses
                    .Where(e => e.Date >= startDate && e.Date <= endDate)
                    .GroupBy(e => e.Date)
                    .Select(g => new { Date = g.Key, TotalAmount = g.Sum(e => e.Amount) })
                    .ToList();

                // Створення словника з датами та сумами витрат
                Dictionary<DateTime, decimal> expensesByDate = expenses.ToDictionary(e => e.Date, e => e.TotalAmount);

                // Створення серії для гістограми
                SeriesCollection.Clear();
                SeriesCollection.Add(new ColumnSeries
                {
                    Title = "Витрати",
                    Values = new ChartValues<decimal>(expenses.Select(e => e.TotalAmount)),
                });

                // Встановлення міток для осі X
                Labels = expenses.Select(e => e.Date.ToShortDateString()).ToArray();
            }

            DataContext = this;
        }

        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            startDate = startDatePicker.SelectedDate ?? DateTime.MinValue;
            endDate = endDatePicker.SelectedDate ?? DateTime.MaxValue;
            UpdateHistogram();
        }
    }
}