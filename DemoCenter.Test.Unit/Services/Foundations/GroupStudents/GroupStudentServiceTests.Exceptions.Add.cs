using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using System;
using Xunit;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.Groups.Exceptions;
using FluentAssertions;
using DemoCenter.Models.GroupStudents.Exceptions;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent someGroupStudent = CreateRandomGroupStudent(randomDateTime);
            SqlException sqlException = CreateSqlException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(sqlException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Throws(sqlException);

            //when
            ValueTask<GroupStudent> addGroupStudentTask =
                this.groupStudentService.AddGroupStudentAsync(someGroupStudent);

            GroupStudentDependencyException actualGroupStudentDependencyException =
                await Assert.ThrowsAsync<GroupStudentDependencyException>(
                    addGroupStudentTask.AsTask);

            //then
            actualGroupStudentDependencyException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
