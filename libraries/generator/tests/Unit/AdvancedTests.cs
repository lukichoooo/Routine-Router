using Attributes;

namespace Unit;

public class Car
{
    public Guid? Id { get; set; }
    public string? Driver { get; private set; } = string.Empty;
    public List<string>? Colors { get; set; } = [];
}

[GenerateDto(typeof(Car))]
public partial class CarDto;

public class AdvancedTests
{
    [Test]
    public void Test1()
    {
        Car car = new()
        {
            Id = Guid.NewGuid(),
            Colors = new() { "red", "blue" },
        };

        var dto = CarDto.From(car);

        Assert.That(car.Driver, Is.EqualTo(string.Empty));
        Assert.That(dto.Colors, Is.EquivalentTo(car.Colors));
    }
}

