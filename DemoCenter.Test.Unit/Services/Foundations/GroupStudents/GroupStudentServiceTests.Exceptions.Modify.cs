using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.GroupStudents;
using DemoCenter.Models.Foundations.GroupStudents.Exceptions;
using DemoCenter.Models.GroupStudents.Exceptions;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

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


        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            //given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(randomDateTime);
            GroupStudent someGroupStudent = randomGroupStudent;
            randomGroupStudent.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            Guid groupId = someGroupStudent.GroupId;
            Guid studentId = someGroupStudent.StudentId;
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedGroupStudentException =
                new LockedGroupStudentException(databaseUpdateConcurrencyException);

            var expectedGroupStudentDependencyValidationException =
                new GroupStudentDependencyValidationException(lockedGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(groupId, studentId))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<GroupStudent> modifyGroupStudent =
                this.groupStudentService.ModifyGroupStudentAsync(someGroupStudent);

            GroupStudentDependencyValidationException actualGroupStudentDependencyValidationException =
                await Assert.ThrowsAsync<GroupStudentDependencyValidationException>(modifyGroupStudent.AsTask);

            //then
            actualGroupStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(groupId, studentId), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            //given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            GroupStudent randomGroupStudent = CreateRandomGroupStudent(randomDateTime);
            GroupStudent someGroupStudent = randomGroupStudent;
            Guid groupId = someGroupStudent.GroupId;
            Guid studentId = someGroupStudent.StudentId;
            someGroupStudent.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedGroupStudentService =
                new FailedGroupStudentServiceException(serviceException);

            var expectedGroupStudentServiceException =
                new GroupStudentServiceException(failedGroupStudentService);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(groupId, studentId)).ThrowsAsync(serviceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDateTime);

            //when
            ValueTask<GroupStudent> modifyGroupStudent =
                this.groupStudentService.ModifyGroupStudentAsync(someGroupStudent);

            GroupStudentServiceException actualGroupStudentServiceException =
                await Assert.ThrowsAsync<GroupStudentServiceException>(modifyGroupStudent.AsTask);

            //then
            actualGroupStudentServiceException.Should().BeEquivalentTo(
                expectedGroupStudentServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(groupId, studentId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentServiceException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            GroupStudent foreignKeyConflictedGroupStudent = CreateRandomGroupStudent();
            string randomMessage = GetRandomString();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidGroupStudentReferenceException =
                new InvalidGroupStudentReferenceException(foreignKeyConstraintConflictException);

            var expectedGroupStudentDependencyValidationException =
                new GroupStudentDependencyValidationException(invalidGroupStudentReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<GroupStudent> modifyGroupStudentTask =
                this.groupStudentService.ModifyGroupStudentAsync(foreignKeyConflictedGroupStudent);

            GroupStudentDependencyValidationException actualGroupStudentDependencyValidationException =
                await Assert.ThrowsAsync<GroupStudentDependencyValidationException>(
                    modifyGroupStudentTask.AsTask);

            // then
            actualGroupStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(
                    foreignKeyConflictedGroupStudent.GroupId,
                    foreignKeyConflictedGroupStudent.StudentId),
                         Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(foreignKeyConflictedGroupStudent),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }


    }
}
