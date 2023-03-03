using System.Threading.Tasks;
using DemoCenter.Models.Subjects;

namespace DemoCenter.Services.Foundations.Subjects
{
    public interface ISubjectService
    {
        ValueTask<Subject> AddSubjectAsync(Subject subject);
    }
}