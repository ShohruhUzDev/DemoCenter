﻿using DemoCenter.Models.Teachers;
using DemoCenter.Models.Teachers.Exceptions;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    public partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Teacher someTeacher = CreateRandomTeacher();
            SqlException sqlException = CreateSqlException();
            var failedTeacherStorageException = new FailedTeacherStorageException(sqlException);

            var expectedTeacherDependencyException =
                new TeacherDependencyException(failedTeacherStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrenDateTime()).Throws(sqlException);

            //when
            ValueTask<Teacher> addTeacherTask = this.teacherService.AddTeacherAsync(someTeacher);

            TeacherDependencyException actualTeacherDependencyException =
                await Assert.ThrowsAsync<TeacherDependencyException>(addTeacherTask.AsTask);

            //then
            actualTeacherDependencyException.Should().BeEquivalentTo(expectedTeacherDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedTeacherDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeacherAsync(It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldTrowDependencyValidationExceptionOnAddIfDuplicateErrorOccursAndLogItAsync()
        {
            // given
            Teacher someTeacher = CreateRandomTeacher();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsTeacherException =
                new AlreadyExistTeacherException(duplicateKeyException);

            var expectedTeacherDependencyValidationException =
                new TeacherDependencyValidationException(alreadyExistsTeacherException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrenDateTime())
                .Throws(duplicateKeyException);

            // when
            ValueTask<Teacher> addTeacherTask = this.teacherService.AddTeacherAsync(someTeacher);

            TeacherDependencyValidationException actualTeacherDependencyValidationException =
                await Assert.ThrowsAsync<TeacherDependencyValidationException>(addTeacherTask.AsTask);

            // then
            actualTeacherDependencyValidationException.Should().BeEquivalentTo(
                expectedTeacherDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrenDateTime(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(SameExceptionAs(
                expectedTeacherDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker => broker.InsertTeacherAsync(
               It.IsAny<Teacher>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

    }
}
