using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Tunr.Models
{
    [Table("Users")]
    public class TunrUser: IdentityUser<Guid>
    {

    }
}