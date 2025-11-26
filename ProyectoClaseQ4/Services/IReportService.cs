using ProyectoClaseQ4.Models;

namespace ProyectoClaseQ4.Services
{
    public interface IReportService
    {
        Task<TaskStatistics> GetTaskStatistics();
        Task<TaskStatistics> GetUserTaskStatistics(string userId);
        Task<List<TaskItem>> GetOverdueTasks();
        Task<List<TaskItem>> GetTasksDueSoon(int days = 7);
    }
}