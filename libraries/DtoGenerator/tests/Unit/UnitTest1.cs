using Attributes;

namespace Unit;

public class User
{
    int Id { get; set; }
    string? Name { get; set; }
    int Age { get; set; }
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
        };
    }
}
