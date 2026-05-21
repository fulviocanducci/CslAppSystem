namespace CslAppSystem
{
    public static class SQL
    {
        public static string AppendLastInsertId(string sql)
        {
            return sql + "SELECT LAST_INSERT_ID();";
        }
    }
}