using System;
using System.Diagnostics;
using System.IO;
using GrandTheftMultiplayer.Launcher.Helpers;
using GrandTheftMultiplayer.Launcher.Models.DllInjectionService;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Runtime
{
	public class GameInjectionService : IGameInjectionService
	{
	    public bool InjectIntoProcess(Process process)
	    {
	        throw new NotImplementedException();
	    }

	    public void InjectCustomAsiFiles(Process targetProcess, string gamePath)
	    {
	        throw new NotImplementedException();
	    }
	}
}