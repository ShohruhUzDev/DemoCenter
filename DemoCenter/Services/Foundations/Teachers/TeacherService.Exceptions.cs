﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Teachers;
using DemoCenter.Models.Foundations.Teachers.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace DemoCenter.Services.Foundations.Teachers
{
    public partial class TeacherService
    {
        private delegate ValueTask<Teacher> ReturningTeacherFunction();
        private delegate IQueryable<Teacher> ReturningTeachersFunction();
        private async ValueTask<Teacher> TryCatch(ReturningTeacherFunction returningTeacherFunction)
        {
            try
            {
                return await returningTeacherFunction();
            }
            catch (NullTeacherException nullTeacherException)
            {
                throw CreateAndLogValidationException(nullTeacherException);
            }
            catch (InvalidTeacherException invalidTeacherException)
            {
                throw CreateAndLogValidationException(invalidTeacherException);
            }
            catch (NotFoundTeacherException notFoundTeacherException)
            {
                throw CreateAndLogValidationException(notFoundTeacherException);
            }
            catch (SqlException sqlException)
            {
                var failedTeacherStorageException = new FailedTeacherStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedTeacherStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var failedTeacherDependencyValidationException =
                     new AlreadyExistTeacherException(duplicateKeyException);

                throw CreateAndDependencyValidationException(failedTeacherDependencyValidationException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedTeacherException = new LockedTeacherException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedTeacherException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedTeacherStorageException = new FailedTeacherStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedTeacherStorageException);
            }
            catch (Exception serviceException)
            {
                var failedTeacherServiceException = new FailedTeacherServiceException(serviceException);

                throw CreateAndLogServiceException(failedTeacherServiceException);
            }

        }
        private IQueryable<Teacher> TryCatch(ReturningTeachersFunction returningTeachersFunction)
        {
            try
            {
                return returningTeachersFunction();
            }
            catch (SqlException sqlException)
            {
                var failedTeacherStorageException = new FailedTeacherStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedTeacherStorageException);
            }
            catch (Exception serviceException)
            {
                var failedTeacherServiceException = new FailedTeacherServiceException(serviceException);

                throw CreateAndLogServiceException(failedTeacherServiceException);
            }
        }

        private TeacherDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var teacherDependencyException = new TeacherDependencyException(exception);
            this.loggingBroker.LogError(teacherDependencyException);

            return teacherDependencyException;
        }
        private TeacherServiceException CreateAndLogServiceException(Exception exception)
        {
            var teacherServiceException = new TeacherServiceException(exception);
            this.loggingBroker.LogError(teacherServiceException);

            return teacherServiceException;
        }

        private TeacherDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var teacherDependencyValidationException = new TeacherDependencyValidationException(exception);
            this.loggingBroker.LogError(teacherDependencyValidationException);

            return teacherDependencyValidationException;
        }
        private TeacherDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var teacherDependencyException = new TeacherDependencyException(exception);
            this.loggingBroker.LogCritical(teacherDependencyException);

            return teacherDependencyException;
        }

        private TeacherValidationException CreateAndLogValidationException(Xeption xeption)
        {
            var teacherValidationException =
                new TeacherValidationException(xeption);

            this.loggingBroker.LogError(teacherValidationException);

            return teacherValidationException;
        }
    }
}
