namespace Senior_Project
{
    public class Employee : User
    {
        public Employee(string username, string password)
            : base(username, password, "Employee") { }

        public override string GetTargetView() => "Employee";
    }
}
