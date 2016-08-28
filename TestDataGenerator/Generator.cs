using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TestDataGenerator
{
    public static class Generator
    {
        static string[] DepartmentInitials = { "EE", "CMPE", "PHYS", "CHEM", "MECH", "HIST" };
        readonly static int CourseIdStart = 100;
        readonly static int CourseIdEnd = 600;

        readonly static int NoOfStudents = 10000;
        readonly static int CourseByStudent = 4;
        readonly static int StudentIdStart = 10000;

        static List<Guid> userIds;
        static List<Course> CourseList;
        static List<Student> StudentList;
        static List<TakenCourse> TakenCourseList;

        public static void GenerateAll()
        {
            GenerateUsers();
            GenerateCourses();
            GenerateStudents();
            GenerateTakenCourses();

            DumpUsersToSQL();
            DumpCoursesToSQL();
            DumpStudentsToSQL();
            DumpTakenCoursesToSQL();
        }

        private static void GenerateUsers()
        {
            userIds = new List<Guid>();

            for (int i = 0; i < NoOfStudents; ++i)
            {
                userIds.Add(Guid.NewGuid());
            }
        }

        private static void DumpUsersToSQL()
        {
            const string fileName = "V003__UsersAndRoles.sql";
            string sqlInsertUserStatementFormat = "INSERT INTO asp_net_users(id, email, email_confirmed, password_hash, security_stamp, " +
                                        "phone_number, phone_number_confirmed, two_factor_enabled, lockout_end_date_utc, " +
                                        "lockout_enabled, access_failed_count, user_name) " +
                                        "VALUES('{0}', 'student-{1}@xxx.edu', true, 'Abc123def?', '{2}', NULL, false, " +
                                        "false, NULL, false, 0, 'student-{1}@xxx.edu');";

            string sqlInsertRoleStatementFormat = "INSERT INTO asp_net_roles(id, description, name) VALUES('{0}', '{1}', '{2}');";
            string sqlInsertUserRoleStatementFormat = "INSERT INTO asp_net_user_roles(user_id, role_id) VALUES('{0}', '{1}');";

            Guid studentRoleId = Guid.NewGuid();
            Guid adminRoleId = Guid.NewGuid();

            using (StreamWriter file = new StreamWriter(fileName))
            {
                string statement = string.Empty;

                // Generate roles first
                statement = string.Format(sqlInsertRoleStatementFormat,
                                          adminRoleId,
                                          "Admin User",
                                          "Admin");

                file.WriteLine(statement);

                statement = string.Format(sqlInsertRoleStatementFormat,
                                          studentRoleId,
                                          "Plain User - The Student",
                                          "Users");
                
                file.WriteLine(statement);

                // Generate the users, and user_roles
                for (int i = 0; i < NoOfStudents; ++i)
                {
                    Guid userId = userIds[i];

                    statement = string.Format(sqlInsertUserStatementFormat,
                                              userId,
                                              StudentIdStart + i,
                                              Guid.NewGuid());
                    
                    file.WriteLine(statement);

                    statement = string.Format(sqlInsertUserRoleStatementFormat,
                                              userId,
                                              studentRoleId);

                    file.WriteLine(statement);
                }

                // Lastly add the admin user
                Guid adminUserId = Guid.NewGuid();
                statement = string.Format(sqlInsertUserStatementFormat,
                                          adminUserId,
                                          1,
                                          Guid.NewGuid());

                file.WriteLine(statement);

                statement = string.Format(sqlInsertUserRoleStatementFormat,
                                          adminUserId,
                                          adminRoleId);

                file.WriteLine(statement);
            }
        }

        private static void GenerateCourses()
        {
            CourseList = new List<Course>();
            var rnd = new Random();

            foreach (var departmentInitial in DepartmentInitials)
            {
                for (int i = CourseIdStart; i < CourseIdEnd; ++i)
                {
                    CourseList.Add(new Course
                    {
                        CourseId = string.Format("{0}{1}", departmentInitial, i),
                        Credit = rnd.Next(2, 5),
                        Department = departmentInitial,
                        Instructor = string.Format("Instructor-{0}", new Random().Next(2, 5)),
                        Season = rnd.Next(0, 2),
                        Year = 2016
                    });
                }
            }
        }

        private static void DumpCoursesToSQL()
        {
            const string fileName = "V004__Courses.sql";
            string sqlStatementFormat = "INSERT INTO courses(course_id, credit, department, instructor, season, year) values('{0}', {1}, '{2}', '{3}', '{4}', '{5}');";

            using (StreamWriter file = new StreamWriter(fileName))
            {
                foreach (var course in CourseList)
                {
                    string statement = string.Format(sqlStatementFormat,
                                                     course.CourseId,
                                                     course.Credit,
                                                     course.Department,
                                                     course.Instructor,
                                                     course.Season,
                                                     course.Year);
                    file.WriteLine(statement);
                }
            }
        }

        private static void GenerateStudents()
        {
            StudentList = new List<Student>();

            for (int i = 0; i < NoOfStudents; ++i)
            {
                StudentList.Add(new Student
                {
                    StudentId = (StudentIdStart + i).ToString(),
                    FirstName = string.Format("FN-{0}", StudentIdStart + i),
                    LastName = string.Format("LN-{0}", StudentIdStart + i),
                    UserId = userIds[i].ToString()
                });
            }
        }

        private static void DumpStudentsToSQL()
        {
            const string fileName = "V005__Students.sql";
            string sqlStatementFormat = "INSERT INTO students(student_id, first_name, last_name, user_id) values('{0}', '{1}', '{2}', '{3}');";

            using (StreamWriter file = new StreamWriter(fileName))
            {
                foreach (var student in StudentList)
                {
                    string statement = string.Format(sqlStatementFormat,
                                                     student.StudentId,
                                                     student.FirstName,
                                                     student.LastName,
                                                     student.UserId);
                    file.WriteLine(statement);
                }
            }
        }

        private static void GenerateTakenCourses()
        {
            TakenCourseList = new List<TakenCourse>();
            Random rnd = new Random();

            for (int i = 0; i < NoOfStudents; ++i)
            {
                for (int j = 0; j < CourseByStudent; ++j)
                {
                    // randomly select a course
                    Course course = CourseList[rnd.Next(0, CourseList.Count)];

                    TakenCourseList.Add(new TakenCourse
                    {
                        CourseId = course.CourseId,
                        Season = course.Season,
                        Year = course.Year,
                        StudentId = StudentList[i].StudentId
                    });
                }
            }
        }

        private static void DumpTakenCoursesToSQL()
        {
            const string fileName = "V006__TakenCourses.sql";
            string sqlStatementFormat = "INSERT INTO taken_courses(student_id, course_id, year, season) values('{0}', '{1}', {2}, {3});";

            using (StreamWriter file = new StreamWriter(fileName))
            {
                foreach (var takenCourse in TakenCourseList)
                {
                    string statement = string.Format(sqlStatementFormat,
                                                     takenCourse.StudentId,
                                                     takenCourse.CourseId,
                                                     takenCourse.Year,
                                                     takenCourse.Season);
                    file.WriteLine(statement);
                }
            }
        }
    }
}
