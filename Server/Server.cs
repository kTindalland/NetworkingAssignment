using Interfaces.Services;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace Server
{
    public class Server
    {
        private readonly INetworkCredentialsPatternValidationService _patternValidationService;
        private readonly IServerMessageHandlingService _messageHandlingService;
        private readonly IUserTrackerService _userTracker;
        private readonly IQueueService<Chat> _chatQueue;
        private readonly string _version = "0.1";

        private IPEndPoint _endPoint;
        private TcpListener _listener;

        private readonly object _lock;
        private readonly object _messageLock;
        private bool _exitListening;

        public Server(
            INetworkCredentialsPatternValidationService patternValidationService,
            IServerMessageHandlingService messageHandlingService,
            IUserTrackerService userTracker,
            IQueueService<Chat> _chatQueue)
        {
            _patternValidationService = patternValidationService;
            _messageHandlingService = messageHandlingService;
            _userTracker = userTracker;
            this._chatQueue = _chatQueue;
            _lock = new object();
            _messageLock = new object();
            _exitListening = false;
        }

        public void Splashscreen()
        {
            var fullString = @"
                                                  dddddddd                                                    
ZZZZZZZZZZZZZZZZZZZ                               d::::::d   iiii                                             
Z:::::::::::::::::Z                               d::::::d  i::::i                                             
Z:::::::::::::::::Z                               d::::::d   iiii                                              
Z:::ZZZZZZZZ:::::Z                                d:::::d                                                     
ZZZZZ     Z:::::Z      ooooooooooo        ddddddddd:::::d  iiiiiii    aaaaaaaaaaaaa       cccccccccccccccc    
        Z:::::Z      oo:::::::::::oo    dd::::::::::::::d  i:::::i    a::::::::::::a    cc:::::::::::::::c    
       Z:::::Z      o:::::::::::::::o  d::::::::::::::::d   i::::i    aaaaaaaaa:::::a  c:::::::::::::::::c    
      Z:::::Z       o:::::ooooo:::::o d:::::::ddddd:::::d   i::::i             a::::a c:::::::cccccc:::::c    
     Z:::::Z        o::::o     o::::o d::::::d    d:::::d   i::::i      aaaaaaa:::::a c::::::c     ccccccc    
    Z:::::Z         o::::o     o::::o d:::::d     d:::::d   i::::i    aa::::::::::::a c:::::c                
   Z:::::Z          o::::o     o::::o d:::::d     d:::::d   i::::i   a::::aaaa::::::a c:::::c                 
ZZZ:::::Z     ZZZZZ o::::o     o::::o d:::::d     d:::::d   i::::i  a::::a    a:::::a c::::::c     ccccccc 
Z::::::ZZZZZZZZ:::Z o:::::ooooo:::::o d::::::ddddd::::::dd i::::::i a::::a    a:::::a c:::::::cccccc:::::c  
Z:::::::::::::::::Z o:::::::::::::::o  d:::::::::::::::::d i::::::i a:::::aaaa::::::a  c:::::::::::::::::c  
Z:::::::::::::::::Z  oo:::::::::::oo    d:::::::::ddd::::d i::::::i  a::::::::::aa:::a cc:::::::::::::::c  
ZZZZZZZZZZZZZZZZZZZ    ooooooooooo       ddddddddd   ddddd iiiiiiii   aaaaaaaaaa  aaaa   cccccccccccccccc    
                                                                                                        
==========================================================================================================
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
==========================================================================================================
";

            var lines = fullString.Split('\r');

            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (var line in lines)
            {
                Console.Write(line + "\r");
                Thread.Sleep(15);
            }
            Console.ResetColor();

            Console.WriteLine();
            Console.Beep();

        }

        public bool Initialise(string ipAddress, string port)
        {
            Splashscreen();

            Console.WriteLine($"> Zodiac version {_version}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"> ! ATTEMPTING TO INITIALISE USING IP: {ipAddress} AND PORT: {port}");

            // Validate patterns
            var ipAddressNotValid = !_patternValidationService.ValidateIpAddressPattern(ipAddress);
            var portNotValid = !_patternValidationService.ValidatePortPattern(port);

            var fatalError = ipAddressNotValid || portNotValid;

            if (ipAddressNotValid)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"> !! FATAL !! IP address is not a valid pattern.");
            }

            if (portNotValid)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"> !! FATAL !! Port is not a valid pattern.");
            }

            if (fatalError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"> A fatal error has occured. Halting execution.");
                return false;
            }

            

            var IP = IPAddress.Parse(ipAddress);
            _endPoint = new IPEndPoint(IP, int.Parse(port));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("> Initialisation checks passed. IPEndpoint accepted.");
            Console.ResetColor();

            return true;
        }

        public void Start()
        {
            _listener = new TcpListener(_endPoint);
            _listener.Start();

            lock (_lock)
            {
                _exitListening = false;
            }

            Task.Run(ListenLoop);
            Task.Run(IncrementHeartbeats);
            Task.Run(SendUpdates);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("> Listening");
            Console.ResetColor();
        }

        public void MainLoop()
        {
            var looping = true;
            Console.WriteLine("> Your zodiac server is now running nominally. For a list of availible commands, type help.\n> This terminal is case sensitive.");

            while (looping)
            {
                // Main loop

                Console.Write("> ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "help":
                        Console.WriteLine("> Your availible commands are: help, start, stop, quit, kick, kickall, info");
                        break;

                    case "start":
                        Stop();
                        Start();
                        break;

                    case "stop":
                        Stop();
                        break;

                    case "quit":
                        Stop();
                        looping = false;
                        break;


                    case "kick":
                        Console.WriteLine("> kick is not implemented yet.");
                        break;

                    case "kickall":
                        Console.WriteLine("> kickall is not implemented yet.");
                        break;

                    case "info":
                        lock (_lock)
                        {
                            Console.WriteLine($"> listening? : {!_exitListening}. ip : {_endPoint.Address} port: {_endPoint.Port}");
                        }
                        break;

                    default:
                        Console.WriteLine("> Invalid command. For a list of availible commands, type help.");
                        break;
                }
            }

            Console.WriteLine("> Exited safely. Bye for now!");
            Thread.Sleep(500);
        }

        public void Stop()
        {
            lock (_lock)
            {
                _exitListening = true;
            }

            lock (_userTracker.TrackerLock)
            {
                //var streams = _userTracker.Users.Values.Select(r => r.Stream).ToList();

                //var disconnectMsg = new DisconnectMessage();
                //var buffer = disconnectMsg.Pack();
                //foreach(var stream in streams)
                //{
                //    stream.Write(buffer, 0, buffer.Length);
                //}

                var sockets = _userTracker.Users.Values.Select(r => r.Socket);

                foreach (var socket in sockets)
                {
                    socket.Close();
                }

                _userTracker.Users = new Dictionary<Socket, Interfaces.Structures.IUser>();
            }



            Thread.Sleep(100);
        }

        private async Task ListenLoop()
        {
            bool running = !_exitListening;

            while (running)
            {
                lock (_lock) {
                    if (_exitListening) running = false;
                }

                if (_listener.Pending())
                {
                    var socket = await _listener.AcceptSocketAsync();
                    ProcessClient(socket);
                }
                

                Thread.Sleep(50);
            }


            Console.WriteLine("> Stopped listening.");
            
            _listener.Stop();
            
        }

        private async Task ProcessClient(Socket socket)
        {
            var stream = new NetworkStream(socket);
            var breakout = false;

            // Wait for message
            while (!(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0) && !breakout)
            {
                lock (_lock)
                {
                    if (_exitListening)
                    {
                        break;
                    }
                }

                if (stream.DataAvailable)
                {
                    int twoKiloBytes = 2048;
                    byte[] buffer = new byte[twoKiloBytes];

                    int messageLength = await stream.ReadAsync(buffer, 0, twoKiloBytes);
                    await stream.FlushAsync();

                    byte[] actualMessage = buffer.Take(messageLength).ToArray();

                    lock(_messageLock)
                    {
                        Task.Run(() => _messageHandlingService.HandleMessage(actualMessage, socket, stream));
                    }
                }
                lock (_userTracker.TrackerLock)
                {
                    if (_userTracker.Users.ContainsKey(socket))
                    {
                        var missedBeats = _userTracker.Users[socket].MissedHeartbeats;
                        if (missedBeats >= 5)
                        {
                            breakout = true;
                        }
                    }
                    
                }
            }

            socket.Close();
            _userTracker.Users.Remove(socket);

        }

        private async Task IncrementHeartbeats()
        {
            bool running = !_exitListening;
            while (running)
            {
                lock (_lock)
                {
                    if (_exitListening) running = false;
                }

                lock(_userTracker.TrackerLock)
                {
                    foreach (var socket in _userTracker.Users.Keys)
                    {
                        _userTracker.Users[socket].MissedHeartbeats++;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private async Task SendUpdates()
        {
            bool running = !_exitListening;
            while (running)
            {
                lock (_lock)
                {
                    if (_exitListening) running = false;
                }

                var message = new RegularUpdateMessage();

                lock (_chatQueue.QueueLock)
                {
                    while (_chatQueue.ItemAvailable)
                    {
                        message.NewChats.Add(_chatQueue.Dequeue());
                    }
                }

                List<Stream> streams;

                lock (_userTracker.TrackerLock)
                {
                    message.ActiveUsers = _userTracker.Users.Select(r => r.Value.Username).ToList();
                    streams = _userTracker.Users.Select(r => r.Value.Stream).ToList();
                }

                var buffer = message.Pack();
                foreach (var stream in streams)
                {
                    stream.Write(buffer, 0, buffer.Length);
                }

                Thread.Sleep(100);
            }

        }
    }
}
