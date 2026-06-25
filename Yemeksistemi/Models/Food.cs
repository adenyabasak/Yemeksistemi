using Yemeksistemi.Models;

public class Food
{
    public int Id { get; set; }
    public string FoodName { get; set; }
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}