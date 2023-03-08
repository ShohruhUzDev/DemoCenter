using System.ComponentModel.DataAnnotations;
using Xeptions;

namespace DemoCenter.Models.Subjects.Exceptions
{
    public class SubjectValidationException : Xeption
    {
        
        public SubjectValidationException(Xeption exception) :
            base (message: " Group validation error occured, fix the error and try again.", exception)
        {
            
        }


          }
}
