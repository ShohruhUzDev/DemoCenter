﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Models.Foundations.GroupStudents;

namespace DemoCenter.Services.Foundations.GroupStudents
{
    public partial class GroupStudentService : IGroupStudentService
    {
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public GroupStudentService(
            ILoggingBroker loggingBroker,
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.loggingBroker = loggingBroker;
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<GroupStudent> AddGroupStudentAsync(GroupStudent student) =>
        TryCatch(async () =>
        {
            ValidateGroupStudentOnAdd(student);

            return await this.storageBroker.InsertGroupStudentAsync(student);
        });

        public IQueryable<GroupStudent> RetrieveAllGroupStudents() =>
        TryCatch(() => this.storageBroker.SelectAllGroupStudents());


        public ValueTask<GroupStudent> RetrieveGroupStudentByIdAsync(Guid groupId, Guid studentId) =>
        TryCatch(async () =>
        {
            ValidateGroupStudentIds(groupId, studentId);
            GroupStudent maybeGroupStudent =
                 await this.storageBroker.SelectGroupStudentByIdAsync(groupId, studentId);

            ValidateStorageGroupStudent(maybeGroupStudent, groupId, studentId);

            return maybeGroupStudent;
        });


        public ValueTask<GroupStudent> ModifyGroupStudentAsync(GroupStudent groupStudent) =>
        TryCatch(async () =>
        {
            ValidateGroupStudentOnModify(groupStudent);
            GroupStudent maybeGroupStudent = await
                this.storageBroker
                    .SelectGroupStudentByIdAsync(groupStudent.GroupId, groupStudent.StudentId);

            ValidateAgainstStorageGroupStudentOnModify(groupStudent, maybeGroupStudent);

            return await this.storageBroker.UpdateGroupStudentAsync(groupStudent);
        });


        public ValueTask<GroupStudent> RemoveGroupStudentByIdAsync(Guid groupId, Guid studentId) =>
        TryCatch(async () =>
        {
            ValidateGroupStudentIds(groupId, studentId);

            GroupStudent maybeGroupStudent = await
                this.storageBroker.SelectGroupStudentByIdAsync(groupId, studentId);

            ValidateStorageGroupStudent(maybeGroupStudent, groupId, studentId);

            return await this.storageBroker.DeleteGroupStudentAsync(maybeGroupStudent);
        });

    }
}
