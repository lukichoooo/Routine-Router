using Attributes;

namespace Unit;

public class Car
{
    public Guid Id { get; set; }
    public string Driver { get; private set; } = string.Empty;
    public List<string>? Colors { get; set; } = [];
    public Wheel Wheel { get; set; } = new();
}

public class Wheel
{
    public Guid? Id { get; set; }
    public Guid CarId { get; set; }
    public string Color { get; set; } = string.Empty;
}


[GenerateDto(typeof(Car))]
[Map(nameof(Car.Wheel), typeof(WheelDto))]
public partial class CarDto;

[GenerateDto(typeof(Wheel))]
public partial class WheelDto;

public class AdvancedTests
{
    [Test]
    public void Test1()
    {
        Car car = new()
        {
            Id = Guid.NewGuid(),
            Colors = ["red", "blue"],
        };

        var dto = CarDto.From(car);

        dto.Wheel = new WheelDto();

        Assert.That(dto.Colors, Is.EquivalentTo(car.Colors));
        Assert.That(dto.Wheel, Is.TypeOf<WheelDto>());
    }
}

