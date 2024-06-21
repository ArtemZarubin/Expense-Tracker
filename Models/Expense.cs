using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Category")] // Вказуємо, що CategoryId є зовнішнім ключем для Category
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } // Навігаційна властивість для доступу до категорії

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
