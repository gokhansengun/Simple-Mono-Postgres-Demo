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
using Microsoft.AspNet.Identity;

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
            // pass userId to the query so as to provide only to the self 
            // (security check - parameter tempering)

            var userId = RequestContext.Principal.Identity.GetUserId();

            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                IEnumerable<Course> courses = 
                    connection.Query<Course>(
                        FileHandler.ReadFileContent("Registration.CoursesByStudent.sql"), 
                            new { studentId = studentId, userId = userId });

                return Ok(courses);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Users")]
        public IHttpActionResult AddCourseForStudent([FromBody]NewCourseRequest ncr)
        {
            // TODO: gseng - add parameter tempering guard here.
            
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
