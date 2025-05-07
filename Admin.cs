namespace Senior_Project
{
    public class Admin : User
    {
        public Admin(string username, string password)
            : base(username, password, "Admin") { }

        public override string GetTargetView() => "Admin";
    }
}