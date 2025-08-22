using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Item
{
    [Key] // Indicates ID as the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { set; get; }

    [Required(ErrorMessage = "Name is required")] // This value cannot be empty
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1-100 characters")]
    public required string Name { set; get; } = "";

    [Range(0, float.MaxValue, ErrorMessage = "Price cannot be negative")]
    public float Price { set; get; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { set; get; }

    public Item() { }

    public Item(string name, float price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}