using WcfWithDI.Interfaces;

namespace WcfWithDI.Library
{
    public class Needed : INeeded
    {
        public string GetWord()
        {
            return "bird";
        }
    }
}