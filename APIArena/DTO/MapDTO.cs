namespace APIArena.DTO
{
    public class MapDTO(int height, int width)
    {
        public List<List<TileDTO>> Tiles { get; set; } = new();
        public int Height { get; set; } = height;
        public int Width { get; set; } = width;
    }
}
