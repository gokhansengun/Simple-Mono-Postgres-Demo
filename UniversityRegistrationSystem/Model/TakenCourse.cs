using System;

namespace UniversityRegistrationSystem.Model
{
    public class TakenCourse
    {
        public string StudentId { get; set; }
        public string CourseId { get; set; }
        public int Year { get; set; }
        public int Season { get; set; }
    }
}
