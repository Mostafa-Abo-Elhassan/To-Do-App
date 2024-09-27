using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace To_Do_App.Models
{
    public class Todo
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        // Foreign key to the Category table
        [ForeignKey("Category")]
        [Required(ErrorMessage = "Category is required")]
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        // Foreign key to the Status table
        [ForeignKey("GetStatus")]
        [Required(ErrorMessage = "Status is required")]
        public string StatusId { get; set; }
        public virtual Status GetStatus { get; set; } = null!;

        // Check if the task is open
        public bool IsOpened { get; set; }
    }

}
