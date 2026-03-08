using Attributes.GeneratorAttributes;

namespace Unit.SourceDtoGeneration;

// ============= Base Classes =============

public class EntityBase
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
}

public class PersonBase : EntityBase
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

// ============= Single Level Inheritance =============

public class Employee : PersonBase
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}

[GenerateDto(typeof(Employee))]
public partial class EmployeeDto;

// ============= Multi Level Inheritance =============

public class Manager : Employee
{
    public int TeamSize { get; set; }
    public string Department { get; set; } = string.Empty;
}

[GenerateDto(typeof(Manager))]
public partial class ManagerDto;

// ============= Override in Derived Class =============

public class VehicleBase2
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
}

public class Car2 : VehicleBase2
{
    public new string Brand { get; set; } = "Toyota";  // Override with default
}

[GenerateDto(typeof(Car2))]
public partial class Car2Dto;

// ============= Only Base Class Properties =============

public class EmptyDerived : EntityBase
{
    // No additional properties, only inherits from EntityBase
}

[GenerateDto(typeof(EmptyDerived))]
public partial class EmptyDerivedDto;

// ============= Multiple Levels with New Properties =============

public class GrandParent
{
    public Guid Id { get; set; }
    public string GrandParentName { get; set; } = string.Empty;
}

public class Parent : GrandParent
{
    public string ParentName { get; set; } = string.Empty;
}

public class Child : Parent
{
    public string ChildName { get; set; } = string.Empty;
    public int Age { get; set; }
}

[GenerateDto(typeof(Child))]
public partial class ChildDto;

// ============= Abstract Base Class =============

public abstract class AbstractEntity
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}

public class ConcreteEntity : AbstractEntity
{
    public string Name { get; set; } = string.Empty;
}

[GenerateDto(typeof(ConcreteEntity))]
public partial class ConcreteEntityDto;


public class InheritanceTests
{
    [Test]
    public void From_SingleLevelInheritance_IncludesAllProperties()
    {
        var entity = new Employee
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Description = "Test employee",
            FirstName = "John",
            LastName = "Doe",
            EmployeeNumber = "EMP001",
            Salary = 50000m
        };

        var dto = EmployeeDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.CreatedAt, Is.EqualTo(entity.CreatedAt));
        Assert.That(dto.Description, Is.EqualTo(entity.Description));
        Assert.That(dto.FirstName, Is.EqualTo(entity.FirstName));
        Assert.That(dto.LastName, Is.EqualTo(entity.LastName));
        Assert.That(dto.EmployeeNumber, Is.EqualTo(entity.EmployeeNumber));
        Assert.That(dto.Salary, Is.EqualTo(entity.Salary));
    }

    [Test]
    public void Dto_SingleLevelInheritance_HasAllProperties()
    {
        var dtoType = typeof(EmployeeDto);

        Assert.That(dtoType.GetProperty("Id"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("CreatedAt"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("Description"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("FirstName"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("LastName"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("EmployeeNumber"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("Salary"), Is.Not.Null);
    }

    [Test]
    public void From_MultiLevelInheritance_IncludesAllProperties()
    {
        var entity = new Manager
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Description = "Test manager",
            FirstName = "Jane",
            LastName = "Smith",
            EmployeeNumber = "MGR001",
            Salary = 100000m,
            TeamSize = 10,
            Department = "Engineering"
        };

        var dto = ManagerDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.CreatedAt, Is.EqualTo(entity.CreatedAt));
        Assert.That(dto.Description, Is.EqualTo(entity.Description));
        Assert.That(dto.FirstName, Is.EqualTo(entity.FirstName));
        Assert.That(dto.LastName, Is.EqualTo(entity.LastName));
        Assert.That(dto.EmployeeNumber, Is.EqualTo(entity.EmployeeNumber));
        Assert.That(dto.Salary, Is.EqualTo(entity.Salary));
        Assert.That(dto.TeamSize, Is.EqualTo(entity.TeamSize));
        Assert.That(dto.Department, Is.EqualTo(entity.Department));
    }

    [Test]
    public void Dto_MultiLevelInheritance_HasAllProperties()
    {
        var dtoType = typeof(ManagerDto);

        // From EntityBase
        Assert.That(dtoType.GetProperty("Id"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("CreatedAt"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("Description"), Is.Not.Null);

        // From PersonBase
        Assert.That(dtoType.GetProperty("FirstName"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("LastName"), Is.Not.Null);

        // From Employee
        Assert.That(dtoType.GetProperty("EmployeeNumber"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("Salary"), Is.Not.Null);

        // From Manager
        Assert.That(dtoType.GetProperty("TeamSize"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("Department"), Is.Not.Null);
    }

    [Test]
    public void From_OverrideInDerivedClass_UsesDerivedProperty()
    {
        var entity = new Car2
        {
            Id = Guid.NewGuid(),
            Brand = "Honda"
        };

        var dto = Car2Dto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.Brand, Is.EqualTo(entity.Brand));
    }

    [Test]
    public void Dto_OverrideInDerivedClass_HasSingleProperty()
    {
        var dtoType = typeof(Car2Dto);

        // Should only have one Brand property (from derived class)
        Assert.That(dtoType.GetProperty("Brand"), Is.Not.Null);

        // Should not have duplicates
        var properties = dtoType.GetProperties();
        var brandProperties = properties.Where(p => p.Name == "Brand").ToList();
        Assert.That(brandProperties, Has.Count.EqualTo(1));
    }

    [Test]
    public void From_EmptyDerived_IncludesBaseProperties()
    {
        var entity = new EmptyDerived
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Description = "Test description"
        };

        var dto = EmptyDerivedDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.CreatedAt, Is.EqualTo(entity.CreatedAt));
        Assert.That(dto.Description, Is.EqualTo(entity.Description));
    }

    [Test]
    public void Dto_EmptyDerived_HasBaseProperties()
    {
        var dtoType = typeof(EmptyDerivedDto);

        Assert.That(dtoType.GetProperty("Id"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("CreatedAt"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("Description"), Is.Not.Null);
    }

    [Test]
    public void From_ThreeLevelInheritance_IncludesAllProperties()
    {
        var entity = new Child
        {
            Id = Guid.NewGuid(),
            GrandParentName = "Grandpa",
            ParentName = "Mom",
            ChildName = "Kid",
            Age = 10
        };

        var dto = ChildDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.GrandParentName, Is.EqualTo(entity.GrandParentName));
        Assert.That(dto.ParentName, Is.EqualTo(entity.ParentName));
        Assert.That(dto.ChildName, Is.EqualTo(entity.ChildName));
        Assert.That(dto.Age, Is.EqualTo(entity.Age));
    }

    [Test]
    public void Dto_ThreeLevelInheritance_HasAllProperties()
    {
        var dtoType = typeof(ChildDto);

        // From GrandParent
        Assert.That(dtoType.GetProperty("Id"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("GrandParentName"), Is.Not.Null);

        // From Parent
        Assert.That(dtoType.GetProperty("ParentName"), Is.Not.Null);

        // From Child
        Assert.That(dtoType.GetProperty("ChildName"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("Age"), Is.Not.Null);
    }

    [Test]
    public void From_AbstractBase_IncludesAbstractProperties()
    {
        var entity = new ConcreteEntity
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            Name = "Test Entity"
        };

        var dto = ConcreteEntityDto.From(entity);

        Assert.That(dto.Id, Is.EqualTo(entity.Id));
        Assert.That(dto.IsActive, Is.EqualTo(entity.IsActive));
        Assert.That(dto.Name, Is.EqualTo(entity.Name));
    }

    [Test]
    public void Dto_AbstractBase_HasAllProperties()
    {
        var dtoType = typeof(ConcreteEntityDto);

        // From AbstractEntity
        Assert.That(dtoType.GetProperty("Id"), Is.Not.Null);
        Assert.That(dtoType.GetProperty("IsActive"), Is.Not.Null);

        // From ConcreteEntity
        Assert.That(dtoType.GetProperty("Name"), Is.Not.Null);
    }
}
