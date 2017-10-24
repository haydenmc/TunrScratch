using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Tunr.Models
{
    [Table("Roles")]
    public class TunrRole: IdentityRole<Guid>
    {
        
    }
}