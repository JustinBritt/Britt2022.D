namespace Britt2022.D.Classes.Experiments
{
    using log4net;

    using Britt2022.D.Interfaces.Experiments;

    internal sealed class Experiment3 : IExperiment3
    {
        private ILog Log => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Experiment3()
        {
        }
    }
}