using System.ComponentModel.DataAnnotations;

namespace WaterMangoApp.Model.BusinessModels
{
    public class TaskViewModel
    {
        [Key]
        public int Id { get; set; }
        
        public string Message { get; set; }
        
        public string TaskName { get; set; }
        
        public bool Status { get; set; }
    }
}