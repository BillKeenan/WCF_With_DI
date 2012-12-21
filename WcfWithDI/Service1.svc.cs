using WcfWithDI.Interfaces;

namespace WcfWithDI
{
    public class Service1 : IService1
    {
        private INeeded _needed;

        public Service1(INeeded needed)
        {
            _needed = needed;
        }

        public string GetData()
        {
            return _needed.GetWord();
        }

    }
}
