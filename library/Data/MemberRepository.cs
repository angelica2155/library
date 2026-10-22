using library.Models;
using Npgsql;

namespace library.Data
{
    public class MemberRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public MemberRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        // Copies a database row into a Member.
        // Handles NULL values in the database.
        private static Member ReadMember(NpgsqlDataReader reader)
        {
            return new Member
            {
                MemberId = reader.GetInt64(0),

                FullName = reader.IsDBNull(1)
                    ? string.Empty
                    : reader.GetString(1),

                Email = reader.IsDBNull(2)
                    ? string.Empty
                    : reader.GetString(2),

                MemberType = reader.IsDBNull(3)
                    ? string.Empty
                    : reader.GetString(3)
            };
        }

        // READ: Get all members.
        public async Task<List<Member>> GetAllAsync()
        {
            const string sql =
                "SELECT member_id, full_name, email, member_type " +
                "FROM lending.member ORDER BY member_id;";

            var members = new List<Member>();

            await using var command = _dataSource.CreateCommand(sql);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                members.Add(ReadMember(reader));
            }

            return members;
        }

        // CREATE: Add a new member.
        public async Task AddAsync(Member member)
        {
            const string sql =
                "INSERT INTO lending.member " +
                "(full_name, email, member_type) " +
                "VALUES (@full_name, @email, @member_type);";

            await using var command = _dataSource.CreateCommand(sql);

            command.Parameters.AddWithValue(
                "full_name", member.FullName ?? string.Empty);

            command.Parameters.AddWithValue(
                "email", member.Email ?? string.Empty);

            command.Parameters.AddWithValue(
                "member_type", member.MemberType ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        // READ: Get one member by ID.
        public async Task<Member?> GetByIdAsync(long id)
        {
            const string sql =
                "SELECT member_id, full_name, email, member_type " +
                "FROM lending.member WHERE member_id = @id;";

            await using var command = _dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync()
                ? ReadMember(reader)
                : null;
        }

        // UPDATE: Change an existing member.
        public async Task UpdateAsync(Member member)
        {
            const string sql =
                "UPDATE lending.member " +
                "SET full_name = @full_name, " +
                "email = @email, " +
                "member_type = @member_type " +
                "WHERE member_id = @id;";

            await using var command = _dataSource.CreateCommand(sql);

            command.Parameters.AddWithValue(
                "full_name", member.FullName ?? string.Empty);

            command.Parameters.AddWithValue(
                "email", member.Email ?? string.Empty);

            command.Parameters.AddWithValue(
                "member_type", member.MemberType ?? string.Empty);

            command.Parameters.AddWithValue("id", member.MemberId);

            await command.ExecuteNonQueryAsync();
        }

        // DELETE: Remove a member.
        public async Task DeleteAsync(long id)
        {
            const string sql =
                "DELETE FROM lending.member WHERE member_id = @id;";

            await using var command = _dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
