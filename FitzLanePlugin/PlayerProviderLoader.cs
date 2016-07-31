using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using FitzLanePlugin.Interfaces;

namespace FitzLanePlugin
{
    public class PlayerProviderLoader
    {
        List<IPlayerProvider> loadedProviders = new List<IPlayerProvider>();

        public PlayerProviderLoader(string pluginPath)
        {
            if (!Directory.Exists(pluginPath))
            {
                //the plugin directory does not exist
                return;
            }

            DirectoryInfo d = new DirectoryInfo(pluginPath);
            foreach (var file in d.GetFiles("*.dll"))
            {
                try
                {
                    loadedProviders.Add(LoadPlayerProvider(file.FullName));
                }
                catch (ArgumentException)
                {
                    //cannot load the given file...
                    continue;
                }
            }
        }

        public List<string> GetPlayerNames()
        {
            List<string> names = new List<string>();
            foreach (IPlayerProvider provider in loadedProviders)
            {
                names.Add(provider.GetPlayerName());
            }
            return names;
        }

        public List<IPlayerProvider> GetPlayerProvider()
        {
            return loadedProviders;
        }

        private IPlayerProvider LoadPlayerProvider(string file)
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.LoadFile(file);
            }
            catch (Exception)
            {
                //unable to load...
                throw new ArgumentException("Unable to load IPlayerProvider from " + file);
            }

            Type pluginInfo = null;
            try
            {
                Type[] types = asm.GetTypes();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly core = AppDomain.CurrentDomain.GetAssemblies().Single(x => x.GetName().Name.Equals("FitzLanePlugin"));
                Type type = core.GetType("FitzLanePlugin.Interfaces.IPlayerProvider");
                foreach (var t in types)
                {
                    if (type.IsAssignableFrom((Type)t))
                    {
                        pluginInfo = t;
                        break;
                    }
                }

                if (pluginInfo != null)
                {
                    object o = Activator.CreateInstance(pluginInfo);
                    IPlayerProvider playerProvider = (IPlayerProvider)o;
                    //returning from somewhere within the method... this needs to get refactored...
                    return playerProvider;
                }
            }
            catch (Exception ex)
            {
                //unable to load...
                throw new ArgumentException("Unable to load IPlayerProvider from " + file + "(" + ex.ToString() + ")");
            }
            
            //unable to load...
            throw new ArgumentException("Unable to load IPlayerProvider from " + file);
        }
    }
}
