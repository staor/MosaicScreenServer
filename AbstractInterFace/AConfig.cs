using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractInterFace
{
    public abstract class AConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Online { get; set; }
        public string Usb { get; set; }
        public string  Resolution { get; set; }
        public string Group { get; set; }
        public string DevType { get; set; }
        public string Version { get; set; }

    }
}
