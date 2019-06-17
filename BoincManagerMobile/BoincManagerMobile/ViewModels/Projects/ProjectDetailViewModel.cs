using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class ProjectDetailViewModel : BaseViewModel
    {
        public Project Project { get; set; }
        public ProjectDetailViewModel(Project project)
        {
            Title = project.Name;
            Project = project;
        }
    }
}
