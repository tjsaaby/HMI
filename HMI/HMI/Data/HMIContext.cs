using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HMI.Models;

namespace HMI.Data
{
    public class HMIContext : DbContext
    {
        public HMIContext (DbContextOptions<HMIContext> options)
            : base(options)
        {
        }

        public DbSet<HMI.Models.Names> Names { get; set; }
    }
}
