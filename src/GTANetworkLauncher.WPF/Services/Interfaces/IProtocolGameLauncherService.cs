namespace GrandTheftMultiplayer.Launcher.Services.Interfaces
{
    public interface IProtocolGameLauncherService
    {
        void CheckIfLaunchArgumentExists();

        void TryVerifyProtocolRegistration();
    }
}