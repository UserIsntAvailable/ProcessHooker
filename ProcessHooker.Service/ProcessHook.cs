namespace ProcessHooker.Service {
    /// <summary>
    /// Maps process hooks from the appsettings.json file
    /// </summary>
    public record ProcessHook(string Name, string HookedFile);
}
