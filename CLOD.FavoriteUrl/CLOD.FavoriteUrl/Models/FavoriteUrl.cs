using System;

namespace CLOD.FavoriteUrl.Models
{
    public class FavoriteUrl
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime? EditedDate { get; set; }
        public string? EditedUserId { get; set; }
    }
}
