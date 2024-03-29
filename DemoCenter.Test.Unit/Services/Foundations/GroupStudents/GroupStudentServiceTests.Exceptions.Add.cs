﻿using System;
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

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfGroupStudentAlreadyExistsAndLogItAsync()
        {
            //given
            GroupStudent randomGroupStudent = CreateRandomGroupStudent();
            GroupStudent alreadyExistsGroupStudent = randomGroupStudent;
            string randomMessage = GetRandomString();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsGroupStudentException =
                new AlreadyExistsGroupStudentException(duplicateKeyException);

            var expectedGroupStudentDependencyValidationException =
                new GroupStudentDependencyValidationException(alreadyExistsGroupStudentException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Throws(duplicateKeyException);

            //when
            ValueTask<GroupStudent> addGroupStudentTask =
                this.groupStudentService.AddGroupStudentAsync(alreadyExistsGroupStudent);

            GroupStudentDependencyValidationException actualGroupStudentDependencyValidationException =
                await Assert.ThrowsAsync<GroupStudentDependencyValidationException>(
                    addGroupStudentTask.AsTask);

            //then
            actualGroupStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            //given
            GroupStudent someGroupStudent = CreateRandomGroupStudent();

            var databaseUpdateException =
                new DbUpdateException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(databaseUpdateException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Throws(databaseUpdateException);

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

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            //given
            GroupStudent someGroupStudent = CreateRandomGroupStudent();
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

            //when
            ValueTask<GroupStudent> addGroupStudentTask =
                this.groupStudentService.AddGroupStudentAsync(someGroupStudent);

            GroupStudentDependencyValidationException actualGroupStudentDependencyValidationException =
                await Assert.ThrowsAsync<GroupStudentDependencyValidationException>(
                    addGroupStudentTask.AsTask);

            //then
            actualGroupStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            //given
            GroupStudent someGroupStudent = CreateRandomGroupStudent();
            var serviceException = new Exception();

            var failedGroupStudentServiceException =
                new FailedGroupStudentServiceException(serviceException);

            var expectedGroupStudentServiceException =
                new GroupStudentServiceException(failedGroupStudentServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                    .Throws(serviceException);

            //when
            ValueTask<GroupStudent> addGroupStudentTask =
                this.groupStudentService.AddGroupStudentAsync(someGroupStudent);

            GroupStudentServiceException actualGroupStudentServiceException =
                await Assert.ThrowsAsync<GroupStudentServiceException>(
                    addGroupStudentTask.AsTask);

            //then
            actualGroupStudentServiceException.Should().BeEquivalentTo(
                expectedGroupStudentServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
