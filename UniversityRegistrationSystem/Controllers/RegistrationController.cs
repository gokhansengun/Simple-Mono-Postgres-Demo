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
        public IHttpActionResult AddCourseForStudent([FromBody]NewCourseRequestDto ncr)
        {
            // TODO: gseng - add parameter tempering guard here.
            
            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                var affectedRows = connection.Execute(
                        FileHandler.ReadFileContent("Registration.AddCourseForStudent.sql"),
                            new { studentId = ncr.StudentId, courseId = ncr.CourseId });

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

        [HttpGet]
        public IHttpActionResult ListCoursesByDepartmentYearSeason(
            string departmentName, int year, int season)
        {
            using (var connection = DbConnectionFactory.NewDbConnection())
            {
                IEnumerable<Course> courses =
                    connection.Query<Course>(
                        FileHandler.ReadFileContent("Registration.ListCoursesByDepartmentYearSeason.sql"),
                            new { departmentName, year, season });

                return Ok(courses);
            }
        }
    }
}
