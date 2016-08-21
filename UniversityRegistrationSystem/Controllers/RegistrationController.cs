using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Dapper;
using UniversityRegistrationSystem.DataAccess;
using UniversityRegistrationSystem.Model;
using UniversityRegistrationSystem.Utility;

namespace UniversityRegistrationSystem
{
    public class NewCourseRequest
    {
        public string studentId { get; set; }
        public string courseId { get; set; }
    }

    [Authorize]
    [RoutePrefix("api/Registration")]
    public class RegistrationController : ApiController
    {
        [HttpGet]
        [Authorize(Roles = "Admin, Users")]
        public IHttpActionResult CoursesByStudent(string studentId)
        {
            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                IEnumerable<Course> courses = 
                    connection.Query<Course>(
                        FileHandler.ReadFileContent("Registration.CoursesByStudent.sql"), 
                            new { studentId = studentId });

                return Ok(courses);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Users")]
        public IHttpActionResult AddCourseForStudent([FromBody]NewCourseRequest ncr)
        {
            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                var affectedRows = connection.Execute(
                        FileHandler.ReadFileContent("Registration.AddCourseForStudent.sql"),
                            new { studentId = ncr.studentId, courseId = ncr.courseId });

                if (affectedRows > 0)
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
        }
    }
}
