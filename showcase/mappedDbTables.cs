namespace showcase;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public int Role { get; set; }
}

public class Course
{
    public int Id { get; set; }
    public string Shortcut { get; set; }
    public string Name { get; set; }
    public string Annotation { get; set; }
    public int Credits { get; set; }
    public string GuarantorName { get; set; }
}