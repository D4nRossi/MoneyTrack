using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyTrack.Models {
    public class Transaction {
        [Key]
        public int TransactionId { get; set; }

        //CategoryId
        [Range(1, int.MaxValue, ErrorMessage = "Selecione pelo menos uma opção")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Necessário ser maior que 0")]
        public int Amount { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon {
            get {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string FormattedAmount {
            get {
                return ((Category == null || Category.Type=="Expense") ? "- " : "+ ") + Amount.ToString("C0");
            }
        }
    }
}
