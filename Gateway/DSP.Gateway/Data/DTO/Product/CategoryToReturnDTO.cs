namespace DSP.Gateway.Data
{
    public class CategoryToReturnDTO
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string ImageUrl_L { get; set; }
        public string ImageUrl_M { get; set; }
        public string ImageUrl_S { get; set; }
        public int ArrangeId { get; set; }
    }
}
