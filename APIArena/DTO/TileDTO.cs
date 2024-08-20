namespace APIArena.DTO
{
    public class TileDTO
    {
        public enum TileType
        {
            Empty,
            Base,
            EnemyBase,
            Player,
            Enemy,
            Gold
        }
        public TileType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
