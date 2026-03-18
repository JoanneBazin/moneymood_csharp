using Microsoft.EntityFrameworkCore;

namespace MoneyMood.Exceptions;

public static class DbExceptionHelper
{
    public static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is Npgsql.PostgresException pgEx && pgEx.SqlState == "23505";
    }
}