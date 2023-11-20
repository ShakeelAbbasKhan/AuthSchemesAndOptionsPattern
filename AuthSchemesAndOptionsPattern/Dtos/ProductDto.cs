using AuthSchemesAndOptionsPattern.Model;

namespace AuthSchemesAndOptionsPattern.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }
}
