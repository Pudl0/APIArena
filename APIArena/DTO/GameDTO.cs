﻿namespace APIArena.DTO
{
    public class GameDTO
    {
        public required Guid Id { get; set; }
        public required PlayerDTO Player { get; set; }
    }
}