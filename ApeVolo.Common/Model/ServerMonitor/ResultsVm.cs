namespace ApeVolo.Common.Model.ServerMonitor
{
    public class ResultsVm
    {
        /// <summary>
        /// 
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Sys Sys { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Cpu Cpu { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Memory Memory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Swap Swap { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Disk Disk { get; set; }
    }
}
