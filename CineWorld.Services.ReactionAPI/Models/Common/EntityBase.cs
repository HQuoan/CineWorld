﻿using System.ComponentModel.DataAnnotations.Schema;

namespace CineWorld.Services.ReactionAPI.Models.Common
{
    public class EntityBase
    {
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
