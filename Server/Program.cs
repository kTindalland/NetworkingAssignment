using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Shared.Services;
using Server.Services;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check command line arguments.
            if (args.Length != 2)
            {
                Console.WriteLine("Command line arguments invalid.\nUsage: <ZodiacServer.exe> <ipaddress> <port>");
                Console.ReadLine();
                return;
            }

            // Initialise the unity container.
            var container = new UnityContainer();

            // Register types
            container.RegisterType<Server>();
            container.RegisterType<INetworkCredentialsPatternValidationService, NetworkCredentialsPatternValidationService>();
            container.RegisterType<IMessageDecoderService, MessageDecoderService>();
            container.RegisterType<IMessageHandlingService, MessageHandlingService>();
            container.RegisterSingleton<IUserTrackerService, UserTrackerService>();


            // Resolve server
            var server = container.Resolve<Server>();

            // Initialise the server
            if (!server.Initialise(args[0], args[1])) {
                Console.ReadKey();
                Console.ResetColor();
                return;
            }

            server.Start(); // Initial start
            server.MainLoop(); // Main loop
        }
    }
}
