using Attributes.GeneratorAttributes;

namespace Unit.SourceDtoGeneration;

public class Car
{
    public Guid Id { get; set; }
    public string Driver { get; private set; } = string.Empty;
    public List<Color>? Colors { get; set; } = [];
    public Wheel? Wheel { get; set; } = null;
}

public class Wheel
{
    public Guid? Id { get; set; }
    public Guid CarId { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class Color
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}


[GenerateDto(typeof(Car))]
[Map(nameof(Car.Wheel), typeof(WheelDto))]
// [Map(nameof(Car.Colors), typeof(ColorDto))]
public partial class CarDto;

[GenerateDto(typeof(Wheel))]
public partial class WheelDto;

[GenerateDto(typeof(Color))]
public partial class ColorDto;

public class AdvancedTests
{
    [Test]
    public void Test1()
    {
        Car car = new()
        {
            Id = Guid.NewGuid(),
            Wheel = new Wheel { Id = Guid.NewGuid(), Color = "Blue" },
            Colors = [
                new() { Id = Guid.NewGuid(), Name = "Red" },
                new() { Id = Guid.NewGuid(), Name = "Black" }
            ],
        };

        var dto = CarDto.From(car);

        Assert.That(dto.Colors, Is.EquivalentTo(car.Colors));
        Assert.That(dto.Wheel, Is.TypeOf<WheelDto>());
        Assert.That(dto.Wheel.Color, Is.EqualTo("Blue"));
    }

    [Test]
    public void Test_NullableWheel_StaysNull()
    {
        Car car = new()
        {
            Id = Guid.NewGuid(),
            Wheel = null,
        };

        var dto = CarDto.From(car);

        Assert.That(dto.Wheel, Is.Null);
    }
}

