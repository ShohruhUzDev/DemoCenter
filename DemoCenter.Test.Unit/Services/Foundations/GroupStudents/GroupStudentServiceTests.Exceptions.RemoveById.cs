﻿using System;
using System.Threading.Tasks;
using DemoCenter.Models.GroupStudents;
using DemoCenter.Models.GroupStudents.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.GroupStudents
{
    public partial class GroupStudentServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid groupId = Guid.NewGuid();
            Guid studentId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedGroupStudentException =
                new LockedGroupStudentException(databaseUpdateConcurrencyException);

            var expectedGroupStudentDependencyValidationException =
                new GroupStudentDependencyValidationException(lockedGroupStudentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<GroupStudent> removeGroupStudentByIdTask =
                this.groupStudentService.RemoveGroupStudentByIdAsync(groupId, studentId);

            GroupStudentDependencyValidationException actualGroupStudentDependencyValidationException =
                await Assert.ThrowsAsync<GroupStudentDependencyValidationException>(
                    removeGroupStudentByIdTask.AsTask);

            // then
            actualGroupStudentDependencyValidationException.Should().BeEquivalentTo(
                expectedGroupStudentDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedGroupStudentDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteGroupStudentAsync(It.IsAny<GroupStudent>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

    }
}
