using System;
using DemoCenter.Models.Teachers.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedTeacherStorageException =
                new FailedTeacherStorageException(sqlException);

            var expectedTeacherDependencyException =
                new TeacherDependencyException(failedTeacherStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTeachers()).Throws(sqlException);

            //when
            Action retrieveAllTeachersAction = () =>
                this.teacherService.RetrieveAllTeachers();

            TeacherDependencyException actualTeacherDependencyException =
                Assert.Throws<TeacherDependencyException>(retrieveAllTeachersAction);

            //then
            actualTeacherDependencyException.Should().BeEquivalentTo(expectedTeacherDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTeachers(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTeacherDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllWhenAllServiceErrorOccursAndLogIt()
        {
            //given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedTeacherServiceException =
                new FailedTeacherServiceException(serviceException);

            var expectedTeacherServiceException =
                new TeacherServiceException(failedTeacherServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllTeachers()).Throws(serviceException);

            //when
            Action retrieveAllTeacherAction = () =>
                this.teacherService.RetrieveAllTeachers();

            TeacherServiceException actualTeacherServiceException =
                Assert.Throws<TeacherServiceException>(retrieveAllTeacherAction);

            //then
            actualTeacherServiceException.Should().BeEquivalentTo(expectedTeacherServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllTeachers(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedTeacherServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
