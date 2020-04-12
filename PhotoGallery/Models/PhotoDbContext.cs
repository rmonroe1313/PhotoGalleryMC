using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Models
{
    public class PhotoDbContext : DbContext
    {
        public PhotoDbContext(DbContextOptions<PhotoDbContext> options)
            : base(options)
        { } 

        public DbSet<PhotoImage> PhotoImages { get; set; }
    }
}
