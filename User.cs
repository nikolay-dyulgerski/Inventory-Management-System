using System;
using System.IO;
using System.Text;

namespace Senior_Project
{
    public abstract class User
    {
        public string Username { get; set; }
        public string Passwordhash { get; set; }
        public string Role { get; set; }
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.dat");

        public User(string username, string passwordhash, string role)
        {
            Username = username;
            Passwordhash = HashPassword(passwordhash);
            Role = role;
        }

        public abstract string GetTargetView();

        public static string HashPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= 0x5A;
                bytes[i] = (byte)((((bytes[i] << 3)) | (bytes[i] >> 5)) & 0xFF);
            }
            return Convert.ToBase64String(bytes);
        }
        public void RegisterUser()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Passwordhash) || string.IsNullOrEmpty(Role))
            {
                throw new InvalidOperationException("Invalid registration data.");
            }

            try
            {
                using var fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.None);
                using var writer = new BinaryWriter(fs);
                writer.Write(Username);
                writer.Write(Passwordhash);
                writer.Write(Role);
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
            }
        }


        public static User LoginUser(string username, string password)
        {
            if (!File.Exists(FilePath)) return null;

            try
            {
                using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        string storedUsername = reader.ReadString();
                        string storedPassword = reader.ReadString();
                        string storedRole = reader.ReadString();

                        if (storedUsername == username && storedPassword == HashPassword(password))
                        {
                            return storedRole switch
                            {
                                "Admin" => new Admin(storedUsername, password),
                                "Employee" => new Employee(storedUsername, password),
                                _ => null
                            };
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }
    }
}