using Application.UseCases.Identity;

namespace Infrastructure.Services.Identity;


public sealed class IdentityProvider : IIdentityProvider
{
    private readonly Guid _userId;
    public IdentityProvider() { _userId = LoadOrCreateUserId(); }


    public Guid GetCurrentUserId()
        => _userId;

    public string GetCurrentUserName()
        => Environment.UserName;


    private static Guid LoadOrCreateUserId()
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
                return existing;
        }

        var id = Guid.NewGuid();
        File.WriteAllText(filePath, id.ToString("N"));
        return id;
    }

}



