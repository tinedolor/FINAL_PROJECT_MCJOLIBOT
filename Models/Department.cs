using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
}
}