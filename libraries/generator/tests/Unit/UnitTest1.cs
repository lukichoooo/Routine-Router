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

[GenerateDto(typeof(User), Include = [nameof(User.Name)])]
public partial class UserDtoIncludedName;

[GenerateDto(typeof(User), Exclude = [nameof(User.Name)])]
public partial class UserDtoExcludedName;

public class Tests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void Test1()
    {
        UserDto dto = new()
        {
            Id = 1,
            Name = "Luka",
            Age = 20
        };

        UserDtoIncludedName dtoIncludedName = new()
        {
            Name = "Luka",
        };

        UserDtoExcludedName dtoExcludedName = new()
        {
            Id = 1,
            Age = 20
        };
    }
}
