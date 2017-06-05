using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Tunr.Models
{
    [Table("Users")]
    public class TunrUser: IdentityUser<Guid>
    {
        /// <summary>
        /// Offset of the end of the library object store
        /// </summary>
        public UInt64 LibraryOffset { get; set; } = 0;

        /// <summary>
        /// Maintains row version for optimistic SQL concurrency
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}