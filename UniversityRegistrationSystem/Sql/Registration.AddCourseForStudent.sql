/*
 * Registration.AddCourseForStudent.sql
 */

INSERT INTO taken_courses(
    student_id, 
    course_id, 
    year, 
    season) 
values(@studentId, @courseId, 1, 2016);

