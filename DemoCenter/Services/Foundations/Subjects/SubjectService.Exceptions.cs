using System.Threading.Tasks;
using DemoCenter.Models.Subjects;
using DemoCenter.Models.Subjects.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Xeptions;

namespace DemoCenter.Services.Foundations.Subjects
{
    public partial class SubjectService
    {
        private delegate ValueTask<Subject> ReturningSubjectFunctions();

        private async ValueTask<Subject> TryCatch(ReturningSubjectFunctions returningSubjectFunctions)
        {
            try
            {
                return await returningSubjectFunctions();
            }
            catch (NullSubjectException nulSubjectException)
            {

                throw CreateAndLogValidationExcetion(nulSubjectException);
            }
            catch (InvalidSubjectException invalidSubjectException)
            {

                throw CreateAndLogValidationExcetion(invalidSubjectException);
            }
            catch (NotFoundSubjectException notFoundSubjectException)
            {

                throw CreateAndLogValidationExcetion(notFoundSubjectException);
            }
            catch(SqlException sqlException)
            {
               var failedSubjectStorageException=new FailedSubjectStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedSubjectStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsSubjectException = new AlreadyExistsSubjectException(duplicateKeyException);

                throw CreateAndDependencyValidationException(alreadyExistsSubjectException);
            }
            catch(DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedSubjectException=new LockedSubjectException(dbUpdateConcurrencyException);    
               
                throw CreateAndDependencyValidationException(lockedSubjectException);   
            }
        }

        private SubjectDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var subjectDependencyValidationException = new SubjectDependencyValidationException(exception);
            this.loggingBroker.LogError(subjectDependencyValidationException);

            return subjectDependencyValidationException;    
        }
        private SubjectDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var subjectDependencyException=new SubjectDependencyException(exception);
            this.loggingBroker.LogCritical(subjectDependencyException);

            return subjectDependencyException;
        }

        private SubjectValidationException CreateAndLogValidationExcetion(Xeption xeption)
        {
            var subjectValidationException =
                new SubjectValidationException(xeption);

            this.loggingBroker.LogError(subjectValidationException);

            return subjectValidationException;
        }
    }
}
