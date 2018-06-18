using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace GrandTheftMultiplayer.Launcher.Models.GameLauncherService
{
	public class RequiredDependency
	{
		private readonly string _keyName;
		private readonly string _registryPath;
		private readonly Func<object, bool> _verifyValue;
	    private readonly List<string> _equivalentDependencies;


        public string Name { get; }

		public string DownloadUrl { get; set; }


		public RequiredDependency(string registryPath, string keyName, Func<object, bool> verifyValue, string name, List<string> equivalentDependencies)
		{
			_registryPath = registryPath;
			_keyName = keyName;
			_verifyValue = verifyValue;
		    _equivalentDependencies = equivalentDependencies;

            Name = name;
		}

		public RequiredDependency(string registryPath, string keyName, string errorMessage)
			:this(registryPath, keyName, null, errorMessage, null)
		{
		}

	    public RequiredDependency(string registryPath, string keyName, string errorMessage, List<string> equivalentDependencies)
	        : this(registryPath, keyName, null, errorMessage, equivalentDependencies)
	    {
	    }

        public bool IsInstalledCorrectly()
		{
			var value = Registry.GetValue(_registryPath, _keyName, null);

			if (value == null) return false;

			return _verifyValue?.Invoke(value) ?? true;
		}

	    public bool IsAnyEquivalentInstalledCorrectly()
	    {
	        return _equivalentDependencies?.Any() == true && 
                _equivalentDependencies.Select(equivalentDependency => Registry.GetValue(equivalentDependency, _keyName, null)).Any(value => value != null);
	    }
    }
}