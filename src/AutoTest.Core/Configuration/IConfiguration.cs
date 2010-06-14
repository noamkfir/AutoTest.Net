﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Configuration
{
    public interface IConfiguration
    {
        string DirectoryToWatch { get; }
        string BuildExecutable { get; }
        string UnitTestExe { get; }
        string IgnoreFolder { get; }
    }
}
