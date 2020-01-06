using System;
using System.Collections.Generic;
using System.Text;

namespace MosaicScreenServer
{
    
    class Singleton
    {
        private static volatile Singleton singleton=new Singleton();
        private Singleton() { }
        public static Singleton GetSingleton()
        {
            if (singleton==null)
            {
                singleton = new Singleton();
            }
            return singleton;
        }
    }
}
