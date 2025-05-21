using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Phone { get; set; }
        
        [Required]
        public DateTime HireDate { get; set; }
        
        [Required]
        public string JobTitle { get; set; }
        
        [Required]
        public string Department { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
    }
} 