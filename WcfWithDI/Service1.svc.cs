using System;
using WcfWithDI.Interfaces;
using WcfWithDI.Library;

namespace WcfWithDI
{
    public class Service1 : IService1
    {
        public string GetData()
        {
            var needed = new Needed();
            return needed.GetWord();
        }

    }
}
