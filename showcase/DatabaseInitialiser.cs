using Microsoft.EntityFrameworkCore;

namespace showcase;

public class DatabaseInitialiser
{
    private readonly AppDbContext _context;

    public DatabaseInitialiser(AppDbContext context)
    {
        _context = context;
    }

    public void Initialise()
    {
        var sql = System.IO.File.ReadAllText("scripts/databaseFix.sql");
        _context.Database.ExecuteSqlRaw(sql);
    }
}