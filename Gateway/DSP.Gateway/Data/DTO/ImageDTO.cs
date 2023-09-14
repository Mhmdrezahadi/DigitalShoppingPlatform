namespace DSP.Gateway.Data.DTO
{
    public class ImageDTO
    {
        public Guid Id { get; set; }

        public Byte[] Full { get; set; }
        public Byte[] Thumb { get; set; }
        public DateTime TimeCreated { get; set; }

    }

}
