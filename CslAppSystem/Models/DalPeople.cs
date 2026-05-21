using Dapper;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Data;
namespace CslAppSystem.Models
{
    public class DalPeople
    {
        private readonly IDbConnection _connection;

        public DalPeople(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<People> Get()
        {
            return _connection.Query<People>("SELECT id AS Id, name as Name, date_birthday as DateBirthday FROM people ORDER BY id ASC");
        }

        public People Get(long id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("id", id);
            return _connection.QueryFirstOrDefault<People>("SELECT id AS Id, name as Name, date_birthday as DateBirthday FROM people WHERE id=@Id", parameters);
        }

        public People Create(People value)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("name", value.Name);
            parameters.Add("date_birthday", value.DateBirthday);
            ulong id = _connection.ExecuteScalar<ulong>(SQL.AppendLastInsertId("INSERT INTO people(name, date_birthday) values(@name,@date_birthday);"), parameters);
            value.Id = (long)id;
            return value;
        }

        public bool Update(People value)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("id", value.Id);
            parameters.Add("name", value.Name);
            parameters.Add("date_birthday", value.DateBirthday);
            int count = _connection.Execute("UPDATE people SET name=@name, date_birthday=@date_birthday WHERE id=@id", parameters);
            return count > 0;
        }
    }
}
