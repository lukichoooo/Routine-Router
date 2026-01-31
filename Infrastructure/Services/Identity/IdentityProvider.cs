using Application.UseCases.Identity;
using Domain.Entities.Users.ValueObjects;

namespace Infrastructure.Services.Identity;


public sealed class IdentityProvider : IIdentityProvider
{
    private readonly UserId _userId;
    public IdentityProvider() { _userId = LoadOrCreateUserId(); }


    public UserId GetCurrentUserId()
        => _userId;

    public string GetCurrentUserName()
        => Environment.UserName;


    private static UserId LoadOrCreateUserId()
    {
        var basePath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var appDir = Path.Combine(basePath, "Routine-Router");
        var filePath = Path.Combine(appDir, "user.id");

        Directory.CreateDirectory(appDir);

        if (File.Exists(filePath))
        {
            var text = File.ReadAllText(filePath).Trim();

            if (Guid.TryParse(text, out var existing))
                return new UserId(existing);
        }

        var userId = new UserId(Guid.NewGuid());
        File.WriteAllText(filePath, userId.Value.ToString("N"));
        return userId;
    }

}



