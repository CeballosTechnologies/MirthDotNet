﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MirthDotNet.Model
{
    [Serializable]
    public class MirthDateTime
    {
        public long time { get; set; }
        public string timezone { get; set; }

        public DateTime DateTime { get { return new DateTime(time); } }
    }
}
