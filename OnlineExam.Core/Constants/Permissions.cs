﻿namespace OnlineExam.Core.Constants
{
    public static class Permissions
    {
        public const string Type = "permissions";

        public static class Subjects
        {
            public const string View = "Permissions.Subjects.View";
        }

        public static class Exams
        {
            public const string View = "Permissions.Exams.View";
            public const string ViewById = "Permissions.Exams.ViewById";
            public const string Create = "Permissions.Exams.Create";
            public const string Edit = "Permissions.Exams.Edit";
            public const string Delete = "Permissions.Exams.Delete";
            public const string AssignStudents = "Permissions.Exams.AssignStudents";
            public const string CreateChooseQuestion = "Permissions.Exams.CreateChooseQuestion";
            public const string EditChooseQuestion = "Permissions.Exams.EditChooseQuestion";
            public const string DeleteChooseQuestion = "Permissions.Exams.DeleteChooseQuestion";
        }

        public static class Users
        {
            public const string AddStudent = "Permissions.Users.AddStudent";
            public const string AddTeacher = "Permissions.Users.AddTeacher";
        }

        public static IList<string> GetPermissions()
        {
            return typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .Select(f => f.GetValue(null) as string))
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList()!;
        }
    }
}


