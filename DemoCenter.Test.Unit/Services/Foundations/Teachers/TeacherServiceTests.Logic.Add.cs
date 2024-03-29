﻿using System;
using System.Threading.Tasks;
using DemoCenter.Models.Foundations.Teachers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace DemoCenter.Test.Unit.Services.Foundations.Teachers
{
    partial class TeacherServiceTests
    {
        [Fact]
        public async Task ShouldAddTeacherAsync()
        {
            //given
            DateTimeOffset randomDate = GetRandomDateTime();
            Teacher randomTeacher = CreateRandomTeacher(randomDate);
            Teacher inputTeacher = randomTeacher;
            Teacher persistedTeacher = inputTeacher;
            Teacher expectedTeacher = persistedTeacher.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertTeacherAsync(inputTeacher)).ReturnsAsync(persistedTeacher);

            //when
            Teacher actualTeacher = await this.teacherService.AddTeacherAsync(inputTeacher);

            //then
            actualTeacher.Should().BeEquivalentTo(expectedTeacher);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertTeacherAsync(inputTeacher), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
