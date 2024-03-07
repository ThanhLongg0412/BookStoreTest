using System.Data.SqlClient;

namespace BookStore.Models
{
    public class RolePermission
    {
        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        public string RoleName { get; set; }

        public string PermissionName { get; set; }
    }

    public class RolePermissionModel
    {
        private readonly IConfiguration _configuration;

        public RolePermissionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("bookstoreCon"));
        }

        public List<RolePermission> GetAllRolePermissions()
        {
            List<RolePermission> rolePermissions = new List<RolePermission>();

            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "SELECT role_permissions.role_id, " +
                    "role_permissions.permission_id, roles.name AS role_name, permissions.name " +
                    "AS permission_name FROM role_permissions INNER JOIN roles ON " +
                    "role_permissions.role_id = roles.id INNER JOIN permissions ON " +
                    "role_permissions.permission_id = permissions.id";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        RolePermission rolePermission = new RolePermission
                        {
                            RoleId = (int)reader["role_id"],
                            PermissionId = (int)reader["permissions_id"],
                            RoleName = reader["role_name"].ToString(),
                            PermissionName = reader["permission_name"].ToString()
                        };
                        rolePermissions.Add(rolePermission);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return rolePermissions;
        }

        public bool AddRolePermission(RolePermission rolePermission)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "INSERT INTO role_permissions (role_id, permission_id) VALUES " +
                    "(@role_id, @permission_id)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@role_id", rolePermission.RoleId);
                command.Parameters.AddWithValue("@permission_id", rolePermission.PermissionId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }

        public bool UpdateRolePermission(int role_id, int permission_id, RolePermission 
            rolePermission)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "UPDATE role_permissions SET role_id = @role_id, " +
                    "permission_id = @permission_id WHERE role_id = @role_id, " +
                    "permission_id = @permission_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@role_id", rolePermission.RoleId);
                command.Parameters.AddWithValue("@permission_id", rolePermission.PermissionId);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }

        public bool DeletePermission(int role_id, int permission_id)
        {
            using (SqlConnection connection = GetSqlConnection())
            {
                string query = "DELETE FROM role_permissions WHERE role_id = @role_id, " +
                    "permisson_id = @permission_id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@role_id", role_id);
                command.Parameters.AddWithValue("@permission_id", permission_id);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
