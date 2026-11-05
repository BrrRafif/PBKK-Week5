using Microsoft.Data.SqlClient;
using StudentManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManager.Data
{
    public class StudentRepository
    {
        private readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;
            Database=StudentDB;
            Trusted_Connection=True;
            TrustServerCertificate=True;";

        public List<Student> GetAll()
        {
            var students = new List<Student>();

            using SqlConnection connection =
                new SqlConnection(connectionString);

            string sql =
                "SELECT id, NRP, Nama, Jurusan, Gender, Email " +
                "FROM Students ORDER BY id DESC";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            connection.Open();

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                students.Add(new Student
                {
                    id = Convert.ToInt32(reader["id"]),
                    NRP = reader["NRP"].ToString()!,
                    Nama = reader["Nama"].ToString()!,
                    Jurusan = reader["Jurusan"].ToString()!,
                    Gender = reader["Gender"].ToString()!,
                    Email = reader["Email"].ToString()!
                });
            }

            return students;
        }

         // insert
        public void Insert(Student student)
        {
            using SqlConnection connection =
                new SqlConnection(connectionString);

            string sql =
                "INSERT INTO Students " +
                "(NRP, Nama, Jurusan, Gender, Email) " +
                "VALUES (@NRP,@Nama,@Jurusan,@Gender,@Email)";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@NRP", student.NRP);
            command.Parameters.AddWithValue("@Nama", student.Nama);
            command.Parameters.AddWithValue("@Jurusan", student.Jurusan);
            command.Parameters.AddWithValue("@Gender", student.Gender);
            command.Parameters.AddWithValue("@Email", student.Email);

            connection.Open();
            command.ExecuteNonQuery();
        }

        // Update
        public void Update(Student student)
        {
            using SqlConnection connection =
                new SqlConnection(connectionString);

            string sql =
                "UPDATE Students SET NRP=@NRP, Nama=@Nama, " +
                "Jurusan=@Jurusan, Gender=@Gender, Email=@Email " +
                "WHERE id=@id";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", student.id);
            command.Parameters.AddWithValue("@NRP", student.NRP);
            command.Parameters.AddWithValue("@Nama", student.Nama);
            command.Parameters.AddWithValue("@Jurusan", student.Jurusan);
            command.Parameters.AddWithValue("@Gender", student.Gender);
            command.Parameters.AddWithValue("@Email", student.Email);

            connection.Open();
            command.ExecuteNonQuery();
        }

        // Delete
        public void Delete(int id)
        {
            using SqlConnection connection =
                new SqlConnection(connectionString);

            using SqlCommand command =
                new SqlCommand("DELETE FROM Students WHERE id=@id", connection);

            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        // Search
        public List<Student> Search(string keyword)
        {
            var students = new List<Student>();

            using SqlConnection connection =
                new SqlConnection(connectionString);

            string sql =
                "SELECT id, NRP, Nama, Jurusan, Gender, Email " +
                "FROM Students WHERE NRP LIKE @Keyword " +
                "OR Nama LIKE @Keyword OR Jurusan LIKE @Keyword " +
                "OR Email LIKE @Keyword " +
                "ORDER BY id DESC";

            using SqlCommand command =
                new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                students.Add(new Student
                {
                    id = Convert.ToInt32(reader["id"]),
                    NRP = reader["NRP"].ToString()!,
                    Nama = reader["Nama"].ToString()!,
                    Jurusan = reader["Jurusan"].ToString()!,
                    Gender = reader["Gender"].ToString()!,
                    Email = reader["Email"].ToString()!
                });
            }
            return students;
        }

        public bool NrpExists(string nrp, int excludeId)
        {
            using SqlConnection connection =
                new SqlConnection(connectionString);

            using SqlCommand command = new SqlCommand(
                "SELECT COUNT(*) FROM Students WHERE NRP=@NRP AND Id<>@Id", connection);

            command.Parameters.AddWithValue("@NRP", nrp);
            command.Parameters.AddWithValue("@Id", excludeId);

            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        private static Student Map(SqlDataReader reader) => new Student
        {
            id = Convert.ToInt32(reader["Id"]),
            NRP = reader["NRP"].ToString()!,
            Nama = reader["Nama"].ToString()!,
            Jurusan = reader["Jurusan"].ToString()!,
            Gender = reader["Gender"].ToString()!,
            Email = reader["Email"].ToString()!
        };
    }
}
