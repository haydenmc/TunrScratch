using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tunr.Models.Library
{
    public class TrackGenre
    {
        public Guid TrackId { get; set; }

        public Track Track { get; set; }

        public string Genre { get; set; }
    }
}