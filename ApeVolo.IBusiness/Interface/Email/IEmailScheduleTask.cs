using System.Threading.Tasks;

namespace ApeVolo.IBusiness.Interface.Email
{
    public interface IEmailScheduleTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="emailId">邮件队列ID</param>
        /// <returns></returns>
        Task ExecuteAsync(string emailId = "");
    }
}