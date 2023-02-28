﻿using DemoCenter.Models.Teachers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCenter.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Teacher> InsertTeacherAsync(Teacher teacher);
        IQueryable<Teacher> SelectAllTeachers();
        ValueTask<Teacher> SelectTeacherByIdAsync(Guid id);
        ValueTask<Teacher> UpdateTeacherAsync(Teacher teacher);
        ValueTask<Teacher> DeleteTeacherAsync(Teacher teacher);
    }
}
