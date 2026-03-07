using Attributes;

namespace Unit;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
}


[GenerateDto(typeof(User))]
public partial class UserDto;


public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        UserDto dto = new()
        {
            Id = 1,
            Name = "Luka",
            Age = 20
        };
    }
}
