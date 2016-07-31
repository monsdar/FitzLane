using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using FitzLanePlugin.Interfaces;

namespace FitzLanePlugin
{
    public class ErgSenderLoader
    {
        List<IErgSender> loadedSenders = new List<IErgSender>();

        public ErgSenderLoader(string pluginPath)
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
                    loadedSenders.Add(LoadErgSender(file.FullName));
                }
                catch (ArgumentException)
                {
                    //cannot load the given file...
                    continue;
                }
            }
        }

        public List<IErgSender> GetErgSender()
        {
            return loadedSenders;
        }

        private IErgSender LoadErgSender(string file)
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.LoadFile(file);
            }
            catch (Exception)
            {
                //unable to load...
                throw new ArgumentException("Unable to load IErgSender from " + file);
            }

            Type pluginInfo = null;
            try
            {
                Type[] types = asm.GetTypes();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly core = AppDomain.CurrentDomain.GetAssemblies().Single(x => x.GetName().Name.Equals("FitzLanePlugin"));
                Type type = core.GetType("FitzLanePlugin.Interfaces.IErgSender");
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
                    IErgSender ergSender = (IErgSender)o;
                    //returning from somewhere within the method... this needs to get refactored...
                    return ergSender;
                }
            }
            catch (Exception ex)
            {
                //unable to load...
                throw new ArgumentException("Unable to load IErgSender from " + file + "(" + ex.ToString() + ")");
            }

            //unable to load...
            throw new ArgumentException("Unable to load IErgSender from " + file);
        }
    }
}
