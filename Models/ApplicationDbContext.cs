using Microsoft.EntityFrameworkCore;
using SeuProjeto.Models;
using System.Collections.Generic;

namespace SeuProjeto.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}