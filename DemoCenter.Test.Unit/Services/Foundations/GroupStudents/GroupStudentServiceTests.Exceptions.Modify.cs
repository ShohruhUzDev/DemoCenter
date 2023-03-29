using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using System;
using Xunit;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.GroupStudents.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            //given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomModifyGroupStudent(randomDateTime);
            GroupStudent someGroupStudent = randomGroupStudent;
            Guid grouId = someGroupStudent.GroupId;
            Guid studentId = someGroupStudent.StudentId;
            SqlException sqlException = CreateSqlException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(sqlException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Throws(sqlException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(grouId, studentId)).Throws(sqlException);

            //when
            ValueTask<GroupStudent> modifyGroupStudent =
                this.groupStudentService.ModifyGroupStudentAsync(someGroupStudent);

            GroupStudentDependencyException actualGroupStudentDependencyException =
                await Assert.ThrowsAsync<GroupStudentDependencyException>(modifyGroupStudent.AsTask);

            //then
            actualGroupStudentDependencyException.Should().BeEquivalentTo(expectedGroupStudentDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedGroupStudentDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(grouId, studentId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(someGroupStudent), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            //given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(randomDateTime);
            GroupStudent somePostIpression = randomGroupStudent;
            Guid groupId = somePostIpression.GroupId;
            Guid studentId = somePostIpression.StudentId;
            somePostIpression.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(databaseUpdateException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(groupId, studentId)).ThrowsAsync(databaseUpdateException);

            //when
            ValueTask<GroupStudent> modifyGroupStudent =
                this.groupStudentService.ModifyGroupStudentAsync(somePostIpression);

            GroupStudentDependencyException actualGroupStudentDependencyException =
                await Assert.ThrowsAsync<GroupStudentDependencyException>(modifyGroupStudent.AsTask);

            //then
            actualGroupStudentDependencyException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(groupId, studentId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
