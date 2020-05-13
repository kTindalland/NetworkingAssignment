﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Interfaces.Services;
using NetworkingAssignment.Services;
using NetworkingAssignment.Views;
using Prism.Ioc;
using Prism.Unity;

namespace NetworkingAssignment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomeView>("Home");
            containerRegistry.RegisterForNavigation<ChatRoomView>("ChatRoom");

            containerRegistry.Register<INetworkCredentialsPatternValidationService, NetworkCredentialsPatternValidationService>();
            containerRegistry.Register<INetworkClientService, NetworkClientService>();
        }
    }
}
