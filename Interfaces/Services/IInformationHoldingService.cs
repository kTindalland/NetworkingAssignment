﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IInformationHoldingService
    {
        List<string> ActiveUsers { get; set; }
    }
}