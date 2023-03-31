using DemoCenter.Models.Foundations.GroupStudents.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(sqlException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllGroupStudents()).Throws(sqlException);

            //when
            Action retrieveAllGroupStudentsAction = () =>
                this.groupStudentService.RetrieveAllGroupStudents();

            GroupStudentDependencyException actualGroupStudentDependencyException =
                Assert.Throws<GroupStudentDependencyException>(retrieveAllGroupStudentsAction);

            //then
            actualGroupStudentDependencyException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllGroupStudents(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllWhenAllServiceErrorOccursAndLogIt()
        {
            //given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedGroupStudentServiceException =
                new FailedGroupStudentServiceException(serviceException);

            var expectedGroupStudentServiceException =
                new GroupStudentServiceException(failedGroupStudentServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllGroupStudents()).Throws(serviceException);

            //when
            Action retrieveAllGroupStudentAction = () =>
                this.groupStudentService.RetrieveAllGroupStudents();

            GroupStudentServiceException actualGroupStudentServiceException =
                Assert.Throws<GroupStudentServiceException>(retrieveAllGroupStudentAction);

            //then
            actualGroupStudentServiceException.Should().BeEquivalentTo(expectedGroupStudentServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllGroupStudents(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
