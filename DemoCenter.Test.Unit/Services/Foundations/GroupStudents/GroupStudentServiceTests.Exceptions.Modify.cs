using Microsoft.Data.SqlClient;
using Moq;
using System.Threading.Tasks;
using System;
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
            Guid postId = someGroupStudent.PostId;
            Guid profileId = someGroupStudent.ProfileId;
            SqlException sqlException = GetSqlException();

            var failedGroupStudentStorageException =
                new FailedGroupStudentStorageException(sqlException);

            var expectedGroupStudentDependencyException =
                new GroupStudentDependencyException(failedGroupStudentStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectGroupStudentByIdAsync(postId, profileId)).Throws(sqlException);

            //when
            ValueTask<GroupStudent> modifyGroupStudent =
                this.GroupStudentService.ModifyGroupStudentAsync(someGroupStudent);

            GroupStudentDependencyException actualGroupStudentDependencyException =
                await Assert.ThrowsAsync<GroupStudentDependencyException>(modifyGroupStudent.AsTask);

            //then
            actualGroupStudentDependencyException.Should().BeEquivalentTo(expectedGroupStudentDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedGroupStudentDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectGroupStudentByIdAsync(postId, profileId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateGroupStudentAsync(someGroupStudent), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }


    }
}
