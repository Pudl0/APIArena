using APIArena.Models;

namespace APIArena.DTO
{
    public class MapDTO(int height, int width)
    {
        public Guid Id { get; set; }
        public List<List<TileDTO>> Tiles { get; set; } = new();
        public int Height { get; set; } = height;
        public int Width { get; set; } = width;

        public void UpdateBasePositions(Player player)
        {
            Tiles[player.XPos][player.YPos].Type = TileDTO.TileType.Base;

            if (player.XPos == 0 && player.YPos == 0)
                Tiles[Width - 1][Height - 1].Type = TileDTO.TileType.EnemyBase;
            else
                Tiles[0][0].Type = TileDTO.TileType.EnemyBase;
        }
        public bool ValidPlayerPosition(Player player)
        {  
            return player.XPos >= 0 && player.XPos < Width && player.YPos >= 0 && player.YPos < Height;
        }
    }
}
