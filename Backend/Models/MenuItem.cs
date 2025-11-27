namespace Backend.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; } // PK
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } // So admin can hide meny items.
        public int? CategoryId { get; set; } // FK
        public virtual Category? Category { get; set; } // Navigation property
        public string? ImagePath { get; set; }
    }
}